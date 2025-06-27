//using HelloWorldAPI.Interfaces;
//using HelloWorldAPI.Models;
//using Microsoft.Extensions.Caching.Memory;
//using System.Text.RegularExpressions;

//namespace HelloWorldAPI.Services
//{
//    public class GreetingService
//    {
//    }
//}
using HelloWorldAPI.Interfaces;
using HelloWorldAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;

namespace HelloWorldAPI.Services
{
    public class GreetingService : IGreetingService
    {
        private readonly GreetingDbContext _context;
        private readonly IMemoryCache _cache;

        public GreetingService(GreetingDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<string> GetGreetingAsync(string name)
        {
            var key = name.ToLower();

            if (_cache.TryGetValue(key, out string greeting))
                return greeting;

            var record = await _context.Greetings.FirstOrDefaultAsync(g => g.Name.ToLower() == key);
            if (record == null)
                return null;

            greeting = record.Message;
            _cache.Set(key, greeting, TimeSpan.FromMinutes(5));

            return greeting;
        }

        public async Task<string> CreateOrUpdateGreetingAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required");

            if (!Regex.IsMatch(name, @"^[a-zA-Z\s]+$"))
                throw new ArgumentException("Name must contain only letters and spaces.");

            var key = name.ToLower();
            var greeting = await _context.Greetings.FirstOrDefaultAsync(g => g.Name.ToLower() == key);

            if (greeting != null)
            {
                greeting.Message = $"Hello, {name}";
                greeting.CreatedAt = DateTime.UtcNow;
                _context.Greetings.Update(greeting);
            }
            else
            {
                greeting = new Greeting
                {
                    Name = name,
                    Message = $"Hello, {name}",
                    CreatedAt = DateTime.UtcNow
                };
                _context.Greetings.Add(greeting);
            }

            await _context.SaveChangesAsync();

            _cache.Set(key, greeting.Message, TimeSpan.FromMinutes(5));

            return greeting.Message;
        }
    }
}
