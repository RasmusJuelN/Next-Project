using Newtonsoft.Json;

namespace Database.Utils;

public interface IDataSeeder<T>
{
    public static T? LoadSeed(string path)
    {
        T? seed;

        string json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, path));
        seed = JsonConvert.DeserializeObject<T>(json);

        return seed;
    }

    public void InitializeData();
}
