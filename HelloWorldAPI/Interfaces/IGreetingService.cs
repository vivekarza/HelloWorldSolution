namespace HelloWorldAPI.Interfaces
{
    public interface IGreetingService
    {
        Task<string> GetGreetingAsync(string name);
        Task<string> CreateOrUpdateGreetingAsync(string name);
    }
}
