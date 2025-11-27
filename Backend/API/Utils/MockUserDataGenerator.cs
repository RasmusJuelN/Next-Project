using API.Mock;

namespace API.Utils;

public class MockUserDataGenerator
{
    private static readonly string[] FirstNames = {
        "James", "Mary", "John", "Patricia", "Robert", "Jennifer", "Michael", "Linda",
        "William", "Elizabeth", "David", "Barbara", "Richard", "Susan", "Joseph", "Jessica",
        "Thomas", "Sarah", "Christopher", "Karen", "Charles", "Nancy", "Daniel", "Lisa",
        "Matthew", "Betty", "Anthony", "Helen", "Mark", "Sandra", "Donald", "Donna",
        "Steven", "Carol", "Paul", "Ruth", "Andrew", "Sharon", "Joshua", "Michelle",
        "Kenneth", "Laura", "Kevin", "Sarah", "Brian", "Kimberly", "George", "Deborah",
        "Edward", "Dorothy", "Ronald", "Lisa", "Timothy", "Nancy", "Jason", "Karen"
    };

    private static readonly string[] LastNames = {
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis",
        "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas",
        "Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez", "Thompson", "White",
        "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson", "Walker", "Young",
        "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores",
        "Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell",
        "Carter", "Roberts", "Gomez", "Phillips", "Evans", "Turner", "Diaz", "Parker"
    };

    public static void GenerateMockUsers()
    {
        var users = new List<MockedUser>();
        var random = new Random();
        var usedUsernames = new HashSet<string>();

        foreach (UserRoles role in Enum.GetValues(typeof(UserRoles)))
        {
            for (int i = 0; i < 100; i++)
            {
                string firstName = FirstNames[random.Next(FirstNames.Length)];
                string lastName = LastNames[random.Next(LastNames.Length)];
                string username = GenerateUniqueUsername(firstName, lastName, usedUsernames, random);
                
                users.Add(new MockedUser
                {
                    Id = Guid.NewGuid(),
                    FullName = $"{firstName} {lastName}",
                    Username = username,
                    Role = role,
                    Password = GeneratePassword()
                });
            }
        }

        // Serialize to JSON
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        string json = JsonSerializer.Serialize(users, options);
        File.WriteAllText("./mocked_user_data.json", json);
        
        Console.WriteLine($"Generated {users.Count} mock users and saved to mocked_user_data.json");
    }

    private static string GenerateUniqueUsername(string firstName, string lastName, HashSet<string> usedUsernames, Random random)
    {
        string baseUsername = $"{firstName.ToLower()}.{lastName.ToLower()}";
        string username = baseUsername;
        int counter = 1;

        while (usedUsernames.Contains(username))
        {
            username = $"{baseUsername}{counter}";
            counter++;
        }

        usedUsernames.Add(username);
        return username;
    }

    private static string GeneratePassword(string[]? chars = null, int length = 12)
    {
        chars ??= [
            "abcdefghijklmnopqrstuvwxyz",
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
            "0123456789",
            "!@#$%^&*()-_=+[]{}|;:,.<>?/"
        ];

        var random = new Random();
        var passwordChars = new List<char>();

        // Ensure at least one character from each category
        foreach (var charSet in chars)
        {
            passwordChars.Add(charSet[random.Next(charSet.Length)]);
        }

        // Fill the rest of the password length
        while (passwordChars.Count < length)
        {
            var charSet = chars[random.Next(chars.Length)];
            passwordChars.Add(charSet[random.Next(charSet.Length)]);
        }

        // Shuffle the characters to ensure randomness
        return new string([.. passwordChars.OrderBy(x => random.Next())]);
    }
}