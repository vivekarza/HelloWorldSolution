using HelloWorldAPI.Models;
using HelloWorldAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HelloWorldAPI.Tests
{
    public class GreetingServiceTests
    {
        private GreetingDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<GreetingDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new GreetingDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task CreateOrUpdateGreeting_AddsNewGreeting_WhenNameNotExists()
        {
            var context = GetDbContext();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var service = new GreetingService(context, memoryCache);

            var name = "TestUser";
            var result = await service.CreateOrUpdateGreetingAsync(name);

            Assert.Equal("Hello, TestUser", result);
            Assert.NotNull(await context.Greetings.FirstOrDefaultAsync(g => g.Name == name));
        }

        [Fact]
        public async Task CreateOrUpdateGreeting_UpdatesGreeting_WhenNameExists()
        {
            var context = GetDbContext();
            context.Greetings.Add(new Greeting
            {
                Name = "UpdateMe",
                Message = "Old message",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            });
            await context.SaveChangesAsync();

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var service = new GreetingService(context, memoryCache);

            var result = await service.CreateOrUpdateGreetingAsync("UpdateMe");

            Assert.Equal("Hello, UpdateMe", result);
            var updated = await context.Greetings.FirstOrDefaultAsync(g => g.Name == "UpdateMe");
            Assert.Equal("Hello, UpdateMe", updated.Message);
        }

        [Fact]
        public async Task GetGreetingAsync_ReturnsNull_IfNotInDatabase()
        {
            var context = GetDbContext();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var service = new GreetingService(context, memoryCache);

            var result = await service.GetGreetingAsync("MissingUser");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetGreetingAsync_ReturnsGreeting_IfInDatabase()
        {
            var context = GetDbContext();
            context.Greetings.Add(new Greeting
            {
                Name = "KnownUser",
                Message = "Hello, KnownUser",
                CreatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var service = new GreetingService(context, memoryCache);

            var result = await service.GetGreetingAsync("KnownUser");

            Assert.Equal("Hello, KnownUser", result);
        }
    }
}
