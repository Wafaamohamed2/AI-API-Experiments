### AI Integration with Semantic Kernel
          A .NET console application that demonstrates integration with multiple AI providers using Microsoft's Semantic Kernel framework. 
          This project provides a unified interface to interact with different AI services including OpenAI GPT-4, Google Gemini, and Mistral AI.


## Features:
  - Multiple AI Provider Support: Seamlessly switch between OpenAI, Google Gemini, and Mistral AI
  - Conversational Memory: Maintains chat history for context-aware conversations
  - Custom Service Implementations: Custom implementations for Google Gemini and Mistral AI services
  - Semantic Kernel Integration: Built on Microsoft's Semantic Kernel framework
  - Interactive Console Interface: Real-time chat experience through console application


## Supported AI Providers:
   1. OpenAI GPT-4 - Using official Semantic Kernel connector
   2. Google Gemini - Custom implementation with Gemini 1.5 Flash
   3. Mistral AI - Custom implementation (ready for future use)


## Key Components
   ## 1. CustomGoogleGeminiService:
         A custom implementation of IChatCompletionService that integrates with Google's Gemini API:
              - Handles chat history conversion
              - Manages HTTP requests to Google's Generative Language API
              - Processes responses and converts them to Semantic Kernel format 

   ## 2. CustomMistralAIService:
          A custom implementation for Mistral AI integration:
               - Compatible with Mistral's chat completions API
               - Supports conversation context
               - Ready for production use

   ## 3. Program.cs:
          Main application logic featuring:
               - AI provider selection
               - Kernel configuration
               - Interactive chat loop
               - Environment variable management
          
