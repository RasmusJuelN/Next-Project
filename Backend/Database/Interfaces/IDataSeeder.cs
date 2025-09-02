using Newtonsoft.Json;

namespace Database.Utils;

/// <summary>
/// Defines a contract for data seeding operations that can load seed data from JSON files and initialize data in the system.
/// </summary>
/// <typeparam name="T">The type of data model that will be used for seeding operations.</typeparam>
public interface IDataSeeder<T>
{

    /// <summary>
    /// Loads and deserializes seed data from a JSON file.
    /// </summary>
    /// <param name="path">The relative path to the JSON file containing the seed data.</param>
    /// <returns>
    /// The deserialized object of type T if successful; otherwise, null if the file cannot be read or deserialization fails.
    /// </returns>
    /// <exception cref="FileNotFoundException">Thrown when the specified file path does not exist.</exception>
    /// <exception cref="JsonException">Thrown when the JSON content cannot be deserialized to the specified type T.</exception>
    public static T? LoadSeed(string path)
    {
        T? seed;

        string json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, path));
        seed = JsonConvert.DeserializeObject<T>(json);

        return seed;
    }

    /// <summary>
    /// Initializes the database with seed data.
    /// This method is typically called during application startup to populate the database
    /// with initial or default data required for the application to function properly.
    /// </summary>
    public void InitializeData();
}
