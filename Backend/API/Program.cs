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

// Add services to the container.

builder.Services.AddScoped<LdapService>();
builder.Services.AddScoped<JsonSerializerService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<QuestionnaireTemplateService>();
builder.Services.AddScoped<ActiveQuestionnaireService>();
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
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(SQLGenericRepository<>));
builder.Services.AddScoped<IQuestionnaireTemplateRepository, SQLQuestionnaireTemplateRepository>();
builder.Services.AddScoped<IActiveQuestionnaireRepository, ActiveQuestionnaireRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddControllers(options =>{
    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "NEXT questionnaire API", Version = "v1"});

    options.UseAllOfToExtendReferenceSchemas();
    options.UseOneOfForPolymorphism();

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

var app = builder.Build();

// Ensure the database is created and migrated
using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    Context context = services.GetRequiredService<Context>();
    if (context.Database.GetService<IDatabaseCreator>() is RelationalDatabaseCreator databaseCreator)
    {
        context.Database.Migrate();
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

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseWebSockets();

app.MapControllers();

app.Run();
