using System.ClientModel;
using System.ComponentModel;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using OpenAI.Embeddings;
using Shared;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

Configuration configuration = ConfigurationManager.GetConfiguration();

AzureOpenAIClient azureOpenAiClient = new(new Uri(configuration.Endpoint), new AzureKeyCredential(configuration.Key));
IChatClient chatClient = new ChatClientBuilder(azureOpenAiClient.GetChatClient(configuration.ChatDeploymentName).AsIChatClient())
    .UseFunctionInvocation()
    .Build();

//Embedding sample
EmbeddingClient embeddingClient = azureOpenAiClient.GetEmbeddingClient(configuration.EmbeddingModelName);
ClientResult<OpenAIEmbedding> result = await embeddingClient.GenerateEmbeddingAsync("Hello World");
ReadOnlyMemory<float> vector = result.Value.ToFloats();

//Chat with tools sample
ChatOptions chatOptions = new()
{
    Tools = [AIFunctionFactory.Create(GetWeather)]
};

List<ChatMessage> messages = [];
while (true)
{
    Console.Write("> ");
    string? inputFromUser = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(inputFromUser))
    {
        messages.Add(new ChatMessage(ChatRole.User, inputFromUser));
        await foreach (ChatResponseUpdate response in chatClient.GetStreamingResponseAsync(messages, chatOptions))
        {
            Console.Write(response.Text);
        }
    }

    messages.Clear();

    Console.WriteLine();
    Console.WriteLine(string.Empty.PadLeft(50, '*'));
    Console.WriteLine();
}

[Description("Gets the weather")]
static string GetWeather() => Random.Shared.NextDouble() > 0.5 ? "It's sunny" : "It's raining";