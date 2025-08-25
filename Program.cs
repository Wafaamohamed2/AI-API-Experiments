
using System;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace AI_Integration
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Make sure to set the GOOGLE_AI_API_KEY environment variable before running the program.
            var apiKey = Environment.GetEnvironmentVariable("GOOGLE_AI_API_KEY");

            
            var client = new HttpClient();

            // Prepare the request body
            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = "What is the color of the sky?" } } }
                }
            };

            // Serialize the request body to JSON
            var json = JsonSerializer.Serialize(requestBody);

            // Make the HTTP POST request
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent?key={apiKey}";

            // Send the request and get the response
            var response = await client.PostAsync(url, content);
            var responseJson = await response.Content.ReadAsStringAsync();

            // Parse the response JSON and extract the generated text
            var responseObj = JsonSerializer.Deserialize<JsonElement>(responseJson);

            // Extract the generated text from the response
            var result = responseObj.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
            Console.WriteLine(result);
        }
    }
}
