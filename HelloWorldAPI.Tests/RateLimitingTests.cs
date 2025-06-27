using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorldAPI.Tests
{
    public class RateLimitingTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _httpClient;
        public RateLimitingTests(WebApplicationFactory<Program> applicationFactory)
        {
            _httpClient = applicationFactory.CreateClient();
        }
        [Fact]
        public async Task GetGreeting_Should_RespectRateLimit()
        {
            string endpoint = "/api/greeting?name=RateLimitTest";
            HttpResponseMessage? response = null;
            for (int i = 0; i < 6; i++)
            {
                response = await _httpClient.GetAsync(endpoint);
            }
            Assert.Equal(HttpStatusCode.TooManyRequests, response?.StatusCode);
        }
        [Fact]
        public async Task PostGreeting_Should_RespectRateLimit()
        {
            var payload = new { name = "RateLimitedUser" };
            HttpResponseMessage? response = null;
            for (int i = 0; i < 6; i++)
            {
                response = await _httpClient.PostAsJsonAsync("/api/greeting", payload);
            }
            Assert.Equal(HttpStatusCode.TooManyRequests, response?.StatusCode);
        }
    }
}
