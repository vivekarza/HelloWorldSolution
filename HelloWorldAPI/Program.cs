using HelloWorldAPI.Interfaces;
using HelloWorldAPI.Middlewares;
using HelloWorldAPI.Models;
using HelloWorldAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

//Add Services
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
//builder.Services.AddScoped<IGreetingService, GreetingService>();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi("v1");

// Use in-memory database
//builder.Services.AddDbContext<GreetingDbContext>(options => options.UseInMemoryDatabase("GreetingsDB"));
builder.Services.AddDbContext<GreetingDbContext>(options=>options.UseSqlServer("Server=DESKTOP-O9R6TRK;Database=GreetingsDB;Trusted_Connection=True;MultipleActiveResultSets=true"));

//add global rate limiting config
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "global", factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromSeconds(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        }));
    options.RejectionStatusCode = 429;
});


var app = builder.Build();
//middleware pipeline
app.UseMiddleware<ExceptionMiddleware>();
//use the rate limiter
app.UseRateLimiter();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger(); //serves /swagger/v1/swagger.json
    app.UseSwaggerUI(); // serves swagger UI at /swagger
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

public partial class Program { }

//integration test rate limiter needs the above line
//Required for WebApplicationFactory
//The WebApplicationFactory<T> bootstraps your actual Web API project like a mini-hosted server, and it needs:
//testhost.deps.json