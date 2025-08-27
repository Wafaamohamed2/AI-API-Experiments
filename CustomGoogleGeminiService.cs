using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Integration
{
    public class CustomGoogleGeminiService : IChatCompletionService
    {
        private readonly string _apiKey;
        private readonly string _model;
        private readonly HttpClient _httpClient;
        public IReadOnlyDictionary<string, object?> Attributes { get; }

        public CustomGoogleGeminiService(string model, string apiKey)
        {
            _apiKey = apiKey;
            _model = model;
            _httpClient = new HttpClient();
            Attributes = new Dictionary<string, object?>();
        }

      

        public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(
            ChatHistory chatHistory,
            PromptExecutionSettings? executionSettings = null, 
            Kernel? kernel = null,
            CancellationToken cancellationToken = default)
        {
            var contents = chatHistory.Select(message=> new
            {
               parts = new[] { new { text = message.Content } },
               role = message.Role.Label.ToLower() == "user"  ? "user" : "model"

           }).ToArray();

            var requestBody = new
            {
                contents = contents
            };
            var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

            var response = await _httpClient.PostAsync(url, content);
            //Console.WriteLine($"Response Status Code: {response.StatusCode}");


            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var responseObj = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(responseJson);
          


            var result = responseObj
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();


            return new[] { new ChatMessageContent(AuthorRole.Assistant, result) };


        }

        public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(
            ChatHistory chatHistory, 
            PromptExecutionSettings? executionSettings = null,
            Kernel? kernel = null,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Streaming not implemented in this example");
        }



        public async Task<ChatMessageContent> GetChatMessageContentAsync(
           ChatHistory chatHistory,
           PromptExecutionSettings? executionSettings = null,
           Kernel? kernel = null,
           CancellationToken cancellationToken = default)
        {
            var results = await GetChatMessageContentsAsync(chatHistory, executionSettings, kernel, cancellationToken);
            return results.First();
        }

        public async Task<ChatMessageContent> GetChatMessageContentAsync(
            string prompt,
            PromptExecutionSettings? executionSettings = null,
            Kernel? kernel = null,
            CancellationToken cancellationToken = default)
        {
            var chatHistory = new ChatHistory();
            chatHistory.AddUserMessage(prompt);
            return await GetChatMessageContentAsync(chatHistory, executionSettings, kernel, cancellationToken);
        }






        public void Dispose()
        {
            _httpClient?.Dispose();
        }

    }
}
