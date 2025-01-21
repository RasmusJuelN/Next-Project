using API.services;
using Database;
using Microsoft.EntityFrameworkCore;
using Settings.Default;
using Settings.Models;

const string settingsFile = "config.json";

var builder = WebApplication.CreateBuilder(args);

if (!File.Exists(settingsFile))
{
    DefaultSettings defaultSettings = new();
    Serializer serializer = new();
    
    string json = serializer.Serialize(defaultSettings);
    File.WriteAllText(settingsFile, json);

    // TODO: Maybe check if the required settings are actually set before allowing the user to continue?
    Console.WriteLine(@"
    A new default settings file has been generated. 
    Some settings are required to be set for the application to work.
    Press Enter to continue when completed.
    ");
    
    // Visual Studio Code can, depending on its configuration, redirect everything to the debug console. This catches that.
    if (Console.IsInputRedirected) Console.Read();
    else Console.ReadKey(true);
}

builder.Configuration.AddJsonFile(settingsFile, optional: false, reloadOnChange: true);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

DatabaseSettings databaseSettings = new SettingsBinder(builder.Configuration).Bind<DatabaseSettings>();

builder.Services.AddDbContext<Context>(o => o.UseSqlServer(databaseSettings.ConnectionString, b => b.MigrationsAssembly("Database")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
