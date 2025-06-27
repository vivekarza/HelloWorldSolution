using Microsoft.EntityFrameworkCore;

namespace HelloWorldAPI.Models
{
    public class GreetingDbContext : DbContext
    {
        public GreetingDbContext(DbContextOptions<GreetingDbContext> options): base(options) 
        {
            
        }
        public DbSet<Greeting> Greetings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Greeting>().HasData(new Greeting
            {
                Id = 1,
                Name = "World",
                Message = "Hello, World",
                CreatedAt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Utc)
            });
        }
    }
}
