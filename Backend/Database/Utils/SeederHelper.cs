using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Database.Utils;

/// <summary>
/// A utility class that automatically discovers and executes data seeders that implement the IDataSeeder interface.
/// This class uses reflection to find all seeder classes in the current assembly and invokes their InitializeData methods.
/// </summary>
/// <param name="modelBuilder">The Entity Framework ModelBuilder instance used for configuring entity models and seeding data.</param>
/// <remarks>
/// The SeederHelper looks for classes that:
/// <list type="bullet">
/// <item><description>Are concrete classes (not abstract)</description></item>
/// <item><description>Implement the IDataSeeder&lt;T&gt; interface</description></item>
/// <item><description>Have a constructor that accepts a ModelBuilder parameter</description></item>
/// </list>
/// 
/// Each discovered seeder is instantiated and its InitializeData method is called.
/// If any seeder fails to initialize, an error message is logged to the console and execution continues with the next seeder.
/// </remarks>
public class SeederHelper(ModelBuilder modelBuilder)
{
    private readonly ModelBuilder _modelBuilder = modelBuilder;

    public void Seed()
    {
        // Get all types from the current assembly
        var assembly = Assembly.GetExecutingAssembly();
        
        // Find all classes that implement IDataSeeder<T>
        var seederTypes = assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract)
            .Where(type => type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDataSeeder<>)))
            .ToList();

        // Create instances and call InitializeData on each
        foreach (var seederType in seederTypes)
        {
            try
            {
                // Create instance with ModelBuilder constructor parameter
                var seederInstance = Activator.CreateInstance(seederType, _modelBuilder);
                
                // Get the InitializeData method
                var initializeMethod = seederType.GetMethod("InitializeData");
                
                if (initializeMethod != null && seederInstance != null)
                {
                    // Call InitializeData
                    initializeMethod.Invoke(seederInstance, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize seeder {seederType.Name}: {ex.Message}");
            }
        }
    }
}
