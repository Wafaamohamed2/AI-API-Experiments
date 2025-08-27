using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AI_Integration
{
    public class CustomMistralAIService : IChatCompletionService
    {
        public IReadOnlyDictionary<string, object?> Attributes { get; }
        private readonly string _apiKey;
        private readonly string _model;
        private readonly HttpClient _httpClient;

        public CustomMistralAIService(string apiKey, string model)
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
            var messages = chatHistory.Select(message => new
            {
                role = message.Role.Label.ToLower(),
                content = message.Content
            }).ToArray();

            var requestBody = new
            {
                model = _model,
                messages = messages,
                max_tokens = 1000
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var response = await _httpClient.PostAsync("https://api.mistral.ai/v1/chat/completions", content, cancellationToken);
            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

            var responseObj = JsonSerializer.Deserialize<JsonElement>(responseJson);
            var result = responseObj
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return new[] { new ChatMessageContent(AuthorRole.Assistant, result) };

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

        public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Streaming not implemented in this example");
        }




        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
