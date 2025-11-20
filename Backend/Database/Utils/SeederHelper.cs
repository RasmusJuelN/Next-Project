 using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Database.Utils;

/// <summary>
/// A utility class that automatically discovers and executes data seeders that implement the IDataSeeder interface.
/// </summary>
/// <param name="modelBuilder">The Entity Framework ModelBuilder instance used for configuring entity models and seeding data.</param>
public class SeederHelper(ModelBuilder modelBuilder)
{
    private readonly ModelBuilder _modelBuilder = modelBuilder;

    /// <summary>
    /// Automatically discovers and executes all data seeder classes that implement IDataSeeder&lt;T&gt; interface
    /// within the current assembly. This method uses reflection to find seeder types, instantiate them with
    /// the ModelBuilder, and invoke their InitializeData methods to populate the database with seed data.
    /// </summary>
    /// <remarks>
    /// The method performs the following operations:
    /// <list type="number">
    /// <item>Scans the executing assembly for classes implementing IDataSeeder&lt;T&gt;</item>
    /// <item>Filters out abstract classes and interfaces</item>
    /// <item>Creates instances of each seeder using the ModelBuilder constructor parameter</item>
    /// <item>Invokes the InitializeData method on each seeder instance</item>
    /// </list>
    /// Any exceptions during seeder instantiation or execution are caught and logged to the console,
    /// allowing other seeders to continue processing even if one fails.
    /// </remarks>
    /// <exception cref="Exception">
    /// Individual seeder failures are caught and logged but do not stop the overall seeding process.
    /// </exception>
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
                Console.WriteLine($"Error executing seeder {seederType.Name}: {ex}");
            }
        }
    }
}
