using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Database.Utils;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<Context>
{
    public Context CreateDbContext(string[] args)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        
        string? connectionString = config.GetConnectionString("DefaultConnection");

        if (connectionString is null)
        {
            throw new InvalidOperationException("Could not find a connection string named 'DefaultConnection'.");
        }
        else
        {
            DbContextOptionsBuilder<Context> optionsBuilder = new();
            optionsBuilder.UseSqlServer(connectionString);
            return new Context(optionsBuilder.Options);
        }
    }
}
