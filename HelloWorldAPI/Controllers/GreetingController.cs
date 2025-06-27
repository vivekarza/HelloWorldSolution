using HelloWorldAPI.Interfaces;
using HelloWorldAPI.Models;
using HelloWorldAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HelloWorldAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GreetingController : ControllerBase
    {
        private readonly GreetingDbContext _context;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        //private readonly IGreetingService _greetingService;

        public GreetingController(GreetingDbContext context, IMemoryCache cache)
        {
            _context = context;
            _memoryCache = cache;
            //_greetingService = greetingService;
        }
        // GET: api/<GreetingController>
        [HttpGet]
        public async Task<IActionResult> GetGreeting([FromQuery] string name = "World")
        {
            var key = name.ToLower();
            if (!_memoryCache.TryGetValue(key, out var greeting))
            {
                var record = await _context.Greetings.FirstOrDefaultAsync(x => x.Name.ToLower() == key);
                if (record == null) return NotFound($"Greenting for '{name}' not found.");
                greeting = record?.Message ?? $"Hello, {name}";
                _memoryCache.Set(key, greeting, TimeSpan.FromMinutes(5));
            }
            return Ok(greeting);
        }

        // POST api/<GreetingController>
        [HttpPost]
        public async Task<IActionResult> PostGreeting([FromBody] Greeting greetingRequest)
        {
            if (string.IsNullOrWhiteSpace(greetingRequest.Name))
                return BadRequest("Name is required");

            if (!Regex.IsMatch(greetingRequest.Name, @"^[a-zA-Z\s]+$"))
                return BadRequest("Invalid characters in name.");

            var existing = await _context.Greetings.FirstOrDefaultAsync(g => g.Name.ToLower() == greetingRequest.Name.ToLower());
            if (existing != null)
            {
                existing.Message = $"{greetingRequest.Message}, {greetingRequest.Name}";
                existing.CreatedAt = DateTime.UtcNow;
                _context.Greetings.Update(existing);
            }
            else
            {
                greetingRequest.Message = $"{greetingRequest.Message}, {greetingRequest.Name}";
                greetingRequest.CreatedAt = DateTime.UtcNow;
                _context.Greetings.Add(greetingRequest);


            }

            await _context.SaveChangesAsync();

            _memoryCache.Set(greetingRequest.Name.ToLower(), greetingRequest.Message, TimeSpan.FromMinutes(5));
            return Ok(existing?.Message ?? greetingRequest.Message);
        }

        [HttpGet("fail")]
        public IActionResult Fail()
        {
            throw new Exception("Simulated server error");
        }

        // PUT api/<GreetingController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<GreetingController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
