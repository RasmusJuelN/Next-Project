namespace Database.Utils;

/// <summary>
/// A utility class that automatically discovers and executes data seeders that implement the IDataSeeder interface.
/// Seeders are executed in order based on the SeederOrderAttribute, with lower values executing first.
/// </summary>
/// <param name="modelBuilder">The Entity Framework ModelBuilder instance used for configuring entity models and seeding data.</param>
public class SeederHelper(ModelBuilder modelBuilder)
{
    private readonly ModelBuilder _modelBuilder = modelBuilder;

    /// <summary>
    /// Automatically discovers and executes all data seeder classes that implement IDataSeeder&lt;T&gt; interface
    /// within the current assembly. Seeders are executed in order based on their SeederOrderAttribute.
    /// </summary>
    /// <remarks>
    /// The method performs the following operations:
    /// <list type="number">
    /// <item>Scans the executing assembly for classes implementing IDataSeeder&lt;T&gt;</item>
    /// <item>Filters out abstract classes and interfaces</item>
    /// <item>Orders seeders by their SeederOrderAttribute (lower values first, default is 0)</item>
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
            .Select(type => new
            {
                Type = type,
                Order = type.GetCustomAttribute<SeederOrderAttribute>()?.Order ?? 0
            })
            .OrderBy(x => x.Order)
            .ThenBy(x => x.Type.Name) // Secondary sort by name for consistent ordering
            .ToList();

        Console.WriteLine($"Found {seederTypes.Count} seeders to execute in the following order:");
        foreach (var seederInfo in seederTypes)
        {
            Console.WriteLine($"  Order {seederInfo.Order}: {seederInfo.Type.Name}");
        }

        // Create instances and call InitializeData on each in order
        foreach (var seederInfo in seederTypes)
        {
            try
            {
                Console.WriteLine($"Executing seeder: {seederInfo.Type.Name} (Order: {seederInfo.Order})");
                
                // Create instance with ModelBuilder constructor parameter
                var seederInstance = Activator.CreateInstance(seederInfo.Type, _modelBuilder);

                // Get the InitializeData method
                var initializeMethod = seederInfo.Type.GetMethod("InitializeData");

                if (initializeMethod != null && seederInstance != null)
                {
                    // Call InitializeData
                    initializeMethod.Invoke(seederInstance, null);
                    Console.WriteLine($"Successfully executed seeder: {seederInfo.Type.Name}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing seeder {seederInfo.Type.Name}: {ex}");
            }
        }
    }
}
