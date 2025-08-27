using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Microsoft.Extensions.DependencyInjection;

namespace AI_Integration
{
    internal class Program
    {
        static async Task Main(string[] args)
        {


            #region
            //// Make sure to set the GOOGLE_AI_API_KEY environment variable before running the program.
            //var apiKey = Environment.GetEnvironmentVariable("GOOGLE_AI_API_KEY");


            //var client = new HttpClient();

            //// Prepare the request body
            //var requestBody = new
            //{
            //    contents = new[]
            //    {
            //        new { parts = new[] { new { text = "What is the color of the sky?" } } }
            //    }
            //};

            //// Serialize the request body to JSON
            //var json = JsonSerializer.Serialize(requestBody);

            //// Make the HTTP POST request
            //var content = new StringContent(json, Encoding.UTF8, "application/json");
            //var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent?key={apiKey}";

            //// Send the request and get the response
            //var response = await client.PostAsync(url, content);
            //var responseJson = await response.Content.ReadAsStringAsync();

            //// Parse the response JSON and extract the generated text
            //var responseObj = JsonSerializer.Deserialize<JsonElement>(responseJson);

            //// Extract the generated text from the response
            //var result = responseObj.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
            //Console.WriteLine(result);
            #endregion

            // Select the AI provider (OpenAI, Google Gemini, MistralAI)

            Console.WriteLine("Choose your AI provider:");
            Console.WriteLine("1. OpenAI (GPT-4)");
            Console.WriteLine("2. Google Gemini");
            Console.Write("Enter choice (1 or 2): ");

            var choice = Console.ReadLine();
            AIProvider selectedProvider = choice == "1" ? AIProvider.OpenAI : AIProvider.GoogleGemini;

            var kernal = CreateKernel(selectedProvider);
            var chatService = kernal.GetRequiredService<IChatCompletionService>();

            ChatHistory history = new ChatHistory();

            while (true)
            {

                Console.Write("Q:");
                history.AddUserMessage(Console.ReadLine()!);  // Add user message to history to remember context

                var assistant =  await chatService.GetChatMessageContentAsync(history);
                history.Add(assistant);    // Add assistant response to history

                Console.WriteLine($"A: {assistant.Content}");



            }


        }

        // Factory method to create the appropriate chat completion service based on the selected provider
        static IChatCompletionService CreateChatService(AIProvider provider)
        {
            return provider switch
            {
                AIProvider.OpenAI => CreateOpenAIService(),
                AIProvider.GoogleGemini => CreateGoogleGeminiService(),
                AIProvider.MistralAI => CreateMistralAIService(),
                _ => throw new ArgumentException("Unsupported AI Provider")
            };
        }


        // Methods to create instances of  OpenAI
        static IChatCompletionService CreateOpenAIService()
        {
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set");

            return new OpenAIChatCompletionService("gpt-4", apiKey);
        }


        // Methods to create instances of Google Gemini
        static IChatCompletionService CreateGoogleGeminiService()
        {
            var apiKey = Environment.GetEnvironmentVariable("GOOGLE_AI_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("GOOGLE_AI_API_KEY environment variable is not set");

             return new CustomGoogleGeminiService("gemini-1.5-flash-latest", apiKey);
        }


        // Methods to create instances of MistralAI
        static IChatCompletionService CreateMistralAIService()
        {
            var apiKey = Environment.GetEnvironmentVariable("MISTRAL_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("MISTRAL_API_KEY environment variable is not set");

            return new CustomMistralAIService("mistral-7b-instant-v0.1", apiKey);
        }


        // Enum to represent different AI providers
        public enum AIProvider
        {
            OpenAI,
            GoogleGemini,
            MistralAI
        }


        // Demonstration of a chat conversation using the selected AI service
        static async Task DemonstrateChatConversation(IChatCompletionService chatService)
        {
            Console.WriteLine("\n--- Chat Conversation Demo ---");

            var chatHistory = new ChatHistory();
            chatHistory.AddUserMessage("What is artificial intelligence?");

            var response1 = await chatService.GetChatMessageContentAsync(chatHistory);
            chatHistory.AddAssistantMessage(response1.Content!);
            Console.WriteLine($"AI: {response1.Content}");

            // Continue the conversation
            chatHistory.AddUserMessage("Can you give me a simple example?");
            var response2 = await chatService.GetChatMessageContentAsync(chatHistory);
            Console.WriteLine($"AI: {response2.Content}");
        }


        // Method to create a Kernel with the selected AI provider
        static Kernel CreateKernel(AIProvider provider)
        {
            var builder = Kernel.CreateBuilder();

            switch (provider)
            {
                case AIProvider.OpenAI:
                    var OpenAIKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
                    if (string.IsNullOrEmpty(OpenAIKey))
                        throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set");

                    builder.AddOpenAIChatCompletion("gpt-4", OpenAIKey);
                    
                     break;

                case AIProvider.GoogleGemini:
                    var googleAIKey = Environment.GetEnvironmentVariable("GOOGLE_AI_API_KEY");
                    if (string.IsNullOrEmpty(googleAIKey))
                        throw new InvalidOperationException("GOOGLE_AI_API_KEY environment variable is not set");

                    builder.Services.AddSingleton<IChatCompletionService>(sp => new CustomGoogleGeminiService("gemini-1.5-flash-latest", googleAIKey));
                      break;

                default:
                    throw new ArgumentException("Unsupported AI Provider");
            }
            return builder.Build();

        }
    }



}
