using HelloWorldAPI.Controllers;
using HelloWorldAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;

namespace HelloWorldAPI.Tests;

public class GreetingControllerTests
{
    private readonly GreetingController _controller;
    private readonly GreetingDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly IDbContextTransaction _transaction;
    public GreetingControllerTests()
    {
        var options = new DbContextOptionsBuilder<GreetingDbContext>().UseSqlServer("Server=DESKTOP-O9R6TRK;Database=GreetingsDB;Trusted_Connection=True;MultipleActiveResultSets=true").Options;
        _context = new GreetingDbContext(options);
        _memoryCache = new MemoryCache(new MemoryCacheOptions());

        _controller = new GreetingController(_context, _memoryCache);
        _transaction = _context.Database.BeginTransaction(); // Begin transaction
    }
    public void Dispose()
    {
        _transaction.Rollback();
        _transaction.Dispose();
        _context.Dispose();
    }
    [Fact]
    public async Task GetGreeting_ReturnsDefault_WhenNameNotProvided()
    {
        var result = await _controller.GetGreeting();
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Hello, World", ok.Value);
    }
    [Fact]
    public async Task PostGreeting_AddsNewGreeting_WhenNameDoesNotExist()
    {
        // Arrange
        var greeting = new Greeting { Name = "TestUser", Message = "Hello" };

        // Act
        var result = await _controller.PostGreeting(greeting);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Hello, TestUser", okResult.Value);

        var saved = await _context.Greetings.FirstOrDefaultAsync(g => g.Name == "TestUser");
        Assert.NotNull(saved);
        Assert.Equal("Hello, TestUser", saved.Message);
    }

    [Fact]
    public async Task PostGreeting_UpdatesGreeting_WhenNameExists()
    {


        // Seed existing greeting
        var initial = new Greeting { Name = "UpdateMe", Message = "Old Greeting" };
        _context.Greetings.Add(initial);
        await _context.SaveChangesAsync();

        var updated = new Greeting { Name = "UpdateMe", Message = "Hello" };

        // Act
        var result = await _controller.PostGreeting(updated);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Hello, UpdateMe", okResult.Value);

        var saved = await _context.Greetings.FirstOrDefaultAsync(g => g.Name == "UpdateMe");
        Assert.NotNull(saved);
        Assert.Equal("Hello, UpdateMe", saved.Message);
    }

}
