using System.Net.Http.Json;

namespace HelloworldClient
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly Dictionary<string, string> cache = new();

        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter your Name:");
            var name = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(name)) name = "World";
            if (cache.TryGetValue(name, out var cachedGreeting))
            {
                Console.WriteLine($"[Cache] {cachedGreeting}");
                return;
            }
            try
            {
                var response = await client.GetAsync($"https://localhost:7026/api/greeting?name={name}");
                if (response.IsSuccessStatusCode)
                {
                    var greeting = await response.Content.ReadAsStringAsync();
                    cache[name] = greeting;
                    Console.WriteLine(greeting);
                    Console.ReadLine();

                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound) {
                    Console.WriteLine("Greeting not found. Creating one...");
                    var greeting = new
                    {
                        name = name,
                        message = "Hello",
                    };
                    var postResponse = await client.PostAsJsonAsync("https://localhost:7026/api/greeting", greeting);
                    if (postResponse.IsSuccessStatusCode)
                    {
                        var newGreeting = await postResponse.Content.ReadAsStringAsync();
                        cache[name] = newGreeting;
                        Console.WriteLine(newGreeting);
                        Console.ReadLine();

                    }
                    else
                    {
                        Console.WriteLine($"Error creating greeting: {postResponse.StatusCode}");
                    }
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                }

            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}

