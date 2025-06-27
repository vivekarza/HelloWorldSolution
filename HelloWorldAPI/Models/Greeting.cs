using System.ComponentModel.DataAnnotations;

namespace HelloWorldAPI.Models
{
    public class Greeting
    {
        public int Id { get; set; }
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only letters and spaces allowed.")]
        public required string Name { get; set; }
        public required string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
