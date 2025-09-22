using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.InMemory;
using RagExample.Models;
using Shared;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

//Video covering this sample: https://youtu.be/OkH9l_4IX4o

#pragma warning disable SKEXP0010

//Configuration
Configuration configuration = ConfigurationManager.GetConfiguration();

//Kernel
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddAzureOpenAIChatCompletion(configuration.ChatDeploymentName, configuration.Endpoint, configuration.Key);
kernelBuilder.AddAzureOpenAIEmbeddingGenerator(configuration.EmbeddingModelName, configuration.Endpoint, configuration.Key);
kernelBuilder.Services.AddScoped<VectorStore, InMemoryVectorStore>(options => new InMemoryVectorStore(new InMemoryVectorStoreOptions
{
    EmbeddingGenerator = options.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>()
}));

Kernel kernel = kernelBuilder.Build();

VectorStoreCollection<string, SuperHeroVectorEntity> collection = kernel.GetRequiredService<VectorStore>().GetCollection<string, SuperHeroVectorEntity>("heroes");

await AddDataToVectorStore(collection);

ChatCompletionAgent agent = new()
{
    Kernel = kernel,
    Name = "ComicBookNerd",
    Instructions = "You are a comic book nerd, that answer answer questions about Super Heroes." +
                   "DO NOT USE YOUR GENERAL KNOWLEDGE. ONLY THE SUPERHEROES I GIVE YOU!"
};

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine("Ask your Question about the Heroes");
while (true)
{
    Console.Write("> ");
    string input = Console.ReadLine()!;
    if (!string.IsNullOrWhiteSpace(input))
    {
        string[] searchResultData = await RagSearch(input);
        List<ChatMessageContent> messages = [];
        messages.Add(new ChatMessageContent(AuthorRole.User, $"Superheroes what match question: {string.Join($"{Environment.NewLine}***{Environment.NewLine}", searchResultData)}"));
        messages.Add(new ChatMessageContent(AuthorRole.User, input));
        await foreach (AgentResponseItem<StreamingChatMessageContent> content in agent.InvokeStreamingAsync(messages))
        {
            Console.Write(content.Message);
        }

        Console.WriteLine();
        Console.WriteLine("******************************************************");
        Console.WriteLine();
    }
}

async Task AddDataToVectorStore(VectorStoreCollection<string, SuperHeroVectorEntity> vectorStoreRecordCollection)
{
    Console.WriteLine("Adding Data to InMemory VectorStore");
    string jsonData = File.ReadAllText("Data.json");
    await vectorStoreRecordCollection.EnsureCollectionExistsAsync();
    SuperHeroData data = JsonSerializer.Deserialize<SuperHeroData>(jsonData)!;
    foreach (SuperHero superHero in data.Heroes)
    {
        StringBuilder description = new();
        description.AppendLine($"Name: {superHero.Name}");
        description.AppendLine($"Sex: {superHero.Sex}");
        description.AppendLine($"Description: {superHero.Description}");
        description.AppendLine($"Strength: {superHero.Strength}");
        description.AppendLine($"Weakness: {superHero.Weakness}");
        description.AppendLine($"BackgroundStory: {superHero.BackgroundStory}");
        await vectorStoreRecordCollection.UpsertAsync(new SuperHeroVectorEntity
        {
            Id = superHero.Id,
            Sex = superHero.Sex,
            Name = superHero.Name,
            Description = description.ToString(),
        });
    }
}

async Task<string[]> RagSearch(string input)
{
    Console.WriteLine($"RAG Search for '{input}'");
    List<string> searchResults = new();
    await foreach (VectorSearchResult<SuperHeroVectorEntity> searchResult in collection.SearchAsync(
                       input,
                       top: 5,
                       new VectorSearchOptions<SuperHeroVectorEntity>
                       {
                           //Filter = entity => entity.Sex == "Female",
                       }))
    {
        searchResults.Add(searchResult.Record.Description);
    }

    return searchResults.ToArray();
}