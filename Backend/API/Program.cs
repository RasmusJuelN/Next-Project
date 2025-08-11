using API.Services;
using Database;
using Microsoft.EntityFrameworkCore;
using Settings.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using API.Utils;
using Microsoft.OpenApi.Models;
using Logging.Extensions;
using Settings.Default;
using Database.Repository;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Reflection;
using Database.Interfaces;
using API.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

const string settingsFile = "config.json";

var builder = WebApplication.CreateBuilder(args);

SettingsHelper settingsHelper = new(settingsFile);

if (!settingsHelper.SettingsExists())
{
    settingsHelper.CreateDefault();
}

builder.Configuration.AddJsonFile(settingsFile, optional: false, reloadOnChange: true);

builder.Logging.ClearProviders();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();
builder.Logging.AddFileLogger(configure => builder.Configuration.GetSection("Loggings:FileLogger").Get<DefaultFileLogger>());
builder.Logging.AddDBLogger(configure => builder.Configuration.GetSection("Loggings:DBLogger").Get<DefaultDBLogger>());

// TODO: Check if config version is lower than default, and if it is, "upgrade" the config with any new settings

DatabaseSettings databaseSettings = ConfigurationBinderService.Bind<DatabaseSettings>(builder.Configuration);
JWTSettings jWTSettings = ConfigurationBinderService.Bind<JWTSettings>(builder.Configuration);
SystemSettings systemSettings = ConfigurationBinderService.Bind<SystemSettings>(builder.Configuration);

// Add services to the container.

builder.Services.AddScoped<LdapService>();
builder.Services.AddScoped<JsonSerializerService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<QuestionnaireTemplateService>();
builder.Services.AddScoped<ActiveQuestionnaireService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddSingleton<LdapSessionCacheService>();
builder.Services.AddMemoryCache();
builder.Services.AddAuthentication(cfg => {
    cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer("AccessToken", x => {
    x.RequireHttpsMetadata = false;
    x.SaveToken = false;
    x.TokenValidationParameters = JwtService.GetAccessTokenValidationParameters(jWTSettings.AccessTokenSecret, issuer: jWTSettings.Issuer, audience: jWTSettings.Audience);

    // ASP.NET likes to map JWT claim names to their own URL schema claims
    // making it difficult to work with incoming tokens. This disables that.
    x.MapInboundClaims = false;
}).AddJwtBearer("RefreshToken", x => {
    x.RequireHttpsMetadata = false;
    x.SaveToken = false;
    x.TokenValidationParameters = JwtService.GetRefreshTokenValidationParameters(jWTSettings.RefreshTokenSecret);

    // ASP.NET likes to map JWT claim names to their own URL schema claims
    // making it difficult to work with incoming tokens. This disables that.
    x.MapInboundClaims = false;
});

builder.Services.Configure<RouteOptions>(o => {
    o.LowercaseUrls = true;
    o.LowercaseQueryStrings = true;
});

// Repositories
builder.Services.AddScoped<IQuestionnaireTemplateRepository, QuestionnaireTemplateRepository>();
builder.Services.AddScoped<IActiveQuestionnaireRepository, ActiveQuestionnaireRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITrackedRefreshTokenRepository, TrackedRefreshTokenRepository>();
builder.Services.AddScoped<IApplicationLogRepository, ApplicationLogRepository>();

builder.Services.AddControllers(options =>{
    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "NEXT questionnaire API", Version = "v1"});

    options.UseAllOfToExtendReferenceSchemas();

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your token in the field below."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });

    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddDbContext<Context>(o =>
    o.UseSqlServer(databaseSettings.ConnectionString,
        options => {
            options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            }));

// We have to configure Kestrel before building the app instance
string environment = builder.Configuration["ASPNETCORE_ENVIRONMENT"] ?? "Production";
if (environment != "Development")
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        IPAddress? address = null;
        if (!string.IsNullOrEmpty(systemSettings.ListenIP))
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(systemSettings.ListenIP);
            address = hostEntry.AddressList.SingleOrDefault(host => host.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) ?? IPAddress.Loopback;
        }

        if (address is not null)
        {
            options.Listen(address, systemSettings.HttpPort);
        }
        else
        {
            options.ListenAnyIP(systemSettings.HttpPort);
        }
        
        if (systemSettings.UseSSL)
        {
            if (address is not null)
            {
                options.Listen(address, systemSettings.HttpsPort, listenOptions =>
                {
                    listenOptions.UseHttps(systemSettings.PfxCertificatePath);
                });
            }
            else
            {
                options.ListenAnyIP(systemSettings.HttpsPort, listenOptions =>
                {
                    listenOptions.UseHttps(systemSettings.PfxCertificatePath);
                });
            }
        }
    });
}

// CORS
builder.Services.AddCors(options => {
    options.AddPolicy(name: "AllowedOrigins",
        policy => {
            policy.WithOrigins("http://10.0.1.5")
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

// Access Policies
builder.Services.AddAuthorizationBuilder()
                      .AddPolicy("AdminOnly", policy => policy.RequireRole("admin"))
                      .AddPolicy("TeacherOnly", policy => policy.RequireRole("teacher"))
                      .AddPolicy("StudentOnly", policy => policy.RequireRole("student"))
                      .AddPolicy("StudentAndTeacherOnly", policy => policy.RequireRole("student", "teacher"));


var app = builder.Build();

app.UseCors("AllowedOrigins");

// Ensure the database is created and migrated
using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    Context context = services.GetRequiredService<Context>();
    if (context.Database.GetService<IDatabaseCreator>() is RelationalDatabaseCreator databaseCreator)
    {
        ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
        int max_attempts = 3;
        
        for (int attempt = 0; attempt < max_attempts; attempt++)
        {
            if (context.Database.CanConnect())
            {
                context.Database.Migrate();
            }
            else if (!databaseCreator.Exists())
            {
                logger.LogInformation("Database does not exist, creating it...");
                databaseCreator.Create();
                context.Database.Migrate();
            }
            else
            {
                logger.LogWarning("Waiting for database to be created/migrated... ({attempt}/{max_attempts})", attempt + 1, max_attempts);
                Thread.Sleep(TimeSpan.FromSeconds(30));
                if (attempt == max_attempts - 1)
                {
                    logger.LogCritical("Database is not reachable, exiting.");
                    Environment.Exit(1);
                }
            }
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

if (systemSettings.UseSSL)
{
    app.UseHttpsRedirection();
}


app.UseAuthentication();

app.UseAuthorization();

app.UseWebSockets();

app.MapControllers();

app.Run();
