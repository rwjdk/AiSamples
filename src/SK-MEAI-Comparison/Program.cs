using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using OpenAI.Chat;
using Shared;
using SK_MEAI_Comparison;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;
using ChatMessageContent = Microsoft.SemanticKernel.ChatMessageContent;

// ReSharper disable UnreachableSwitchCaseDueToIntegerAnalysis
#pragma warning disable SKEXP0010
#pragma warning disable OPENAI001

//Shared Config
Configuration configuration = ConfigurationManager.GetConfiguration();
string chatModel = "gpt-5-nano";
string embeddingModel = "text-embedding-3-small";
HttpClient fiveMinTimeoutHttpClient = new()
{
    Timeout = TimeSpan.FromMinutes(5)
};

FrameworkToUse framework = FrameworkToUse.SemanticKernel;

HostApplicationBuilder builder = Host.CreateApplicationBuilder();

#region Dependency Injection (ChatClient and Embedding Service)

switch (framework)
{
    case FrameworkToUse.MicrosoftExtensionsAi:
    {
        AzureOpenAIClient azureOpenAiClient = new(new Uri(configuration.Endpoint), new AzureKeyCredential(configuration.Key), new AzureOpenAIClientOptions
        {
            Transport = new HttpClientPipelineTransport(fiveMinTimeoutHttpClient)
        });
        builder.Services.AddChatClient(azureOpenAiClient.GetChatClient(chatModel).AsIChatClient());
        builder.Services.AddEmbeddingGenerator(azureOpenAiClient.GetEmbeddingClient(embeddingModel).AsIEmbeddingGenerator());
    }
        break;
    case FrameworkToUse.SemanticKernel:
    {
        builder.Services.AddAzureOpenAIChatClient(chatModel, configuration.Endpoint, configuration.Key, httpClient: fiveMinTimeoutHttpClient);
        builder.Services.AddAzureOpenAIEmbeddingGenerator(embeddingModel, configuration.Endpoint, configuration.Key);
    }
        break;
    default:
        throw new ArgumentOutOfRangeException();
}

#endregion

IHost app = builder.Build();

#region On the fly setup

Kernel kernel = null!;
switch (framework)
{
    case FrameworkToUse.MicrosoftExtensionsAi:
        AzureOpenAIClient azureOpenAiClient = new(new Uri(configuration.Endpoint), new AzureKeyCredential(configuration.Key), new AzureOpenAIClientOptions
        {
            Transport = new HttpClientPipelineTransport(fiveMinTimeoutHttpClient)
        });
        IChatClient chatClient = azureOpenAiClient.GetChatClient(chatModel).AsIChatClient();
        IEmbeddingGenerator<string, Embedding<float>> embeddingGeneratorFromMEAI = azureOpenAiClient.GetEmbeddingClient(embeddingModel).AsIEmbeddingGenerator();
        break;
    case FrameworkToUse.SemanticKernel:
        IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.AddAzureOpenAIChatClient(chatModel, configuration.Endpoint, configuration.Key, httpClient: fiveMinTimeoutHttpClient);
        kernelBuilder.AddAzureOpenAIChatCompletion(chatModel, configuration.Endpoint, configuration.Key); //Old way
        kernelBuilder.AddAzureOpenAIEmbeddingGenerator(embeddingModel, configuration.Endpoint, configuration.Key);
        kernel = kernelBuilder.Build();
        IChatClient chatClientFromKernel = kernel.GetRequiredService<IChatClient>();
        IEmbeddingGenerator<string, Embedding<float>> embeddingGeneratorFromKernel = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
        break;
    default:
        throw new ArgumentOutOfRangeException();
}

#endregion

#region Chat

string question = "What is the capital of Denmark and what is its population?";

//This work for both
{
    IChatClient chatClient = app.Services.GetRequiredService<IChatClient>();

    //Basic
    ChatResponse response = await chatClient.GetResponseAsync(question, new ChatOptions
    {
        RawRepresentationFactory = _ => new ChatCompletionOptions()
        {
            ReasoningEffortLevel = ChatReasoningEffortLevel.Low,
        },
    });
    if (response.Usage != null)
    {
        long? inputTokenCount = response.Usage.InputTokenCount;
        long? outputTokenCount = response.Usage.OutputTokenCount;
        if (response.Usage.AdditionalCounts != null)
        {
            long reasoningTokenCount = response.Usage.AdditionalCounts["OutputTokenDetails.ReasoningTokenCount"];
        }
    }

    Console.WriteLine(response);

    //Streaming
    await foreach (ChatResponseUpdate chatResponseUpdate in chatClient.GetStreamingResponseAsync(question))
    {
        Console.Write(chatResponseUpdate);
    }

    //Structured Output
    ChatResponse<City> structuredOutputResponse = await chatClient.GetResponseAsync<City>(question);
    City result = structuredOutputResponse.Result;

    //Function Calling
    ChatResponse responseFromToolCall = await chatClient.GetResponseAsync("How is the Weather", new ChatOptions
    {
        Tools = [AIFunctionFactory.Create(Tools.GetWeather)]
    });
    Console.WriteLine(responseFromToolCall);
}

#endregion

#region Agents

switch (framework)
{
    case FrameworkToUse.MicrosoftExtensionsAi:
    {
        Console.WriteLine("MEAI do not have this concept (you need to use other packages to link up to example AI Foundry)");
    }
        break;
    case FrameworkToUse.SemanticKernel:
    {
        //Stand-alone agent (for single use or in Agent Group Chat)
        ChatCompletionAgent agent = new()
        {
            Kernel = kernel,
            Instructions = "Some kind of System Message"
        };

        //Basic
        await foreach (AgentResponseItem<ChatMessageContent> item in agent.InvokeAsync(question))
        {
            Console.WriteLine(item.Message);
            if (item.Message.Metadata?.TryGetValue("Usage", out object? usage) == true && usage is ChatTokenUsage chatTokenUsage)
            {
                Console.WriteLine();
                Console.WriteLine($"[Token Usage: " +
                                  $"{chatTokenUsage.InputTokenCount} In | " +
                                  $"{chatTokenUsage.OutputTokenCount} Out (" +
                                  $"{chatTokenUsage.OutputTokenDetails.ReasoningTokenCount} was for used for reasoning" +
                                  $")]");
            }
        }

        //Streaming
        await foreach (AgentResponseItem<StreamingChatMessageContent> item in agent.InvokeStreamingAsync(question))
        {
            Console.Write(item.Message);
        }

        //Structured Output
        ChatCompletionAgent agentWithStructuredOutput = new()
        {
            Kernel = kernel,
            Instructions = "You know a lot about cities",
            Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings
            {
                ResponseFormat = typeof(City) //Warning (this is not wired up to use IChatClient and need the ChatCompletion instead) [Bugfix in progress]
            })
        };
        await foreach (AgentResponseItem<ChatMessageContent> response in agentWithStructuredOutput.InvokeAsync(question))
        {
            string json = response.Message.Content!;
            City? city = JsonSerializer.Deserialize<City>(json, new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() } //Needed if you use enums as LLM get them as name strings
            });
        }

        //Function Calling
        kernel.Plugins.AddFromType<SemanticKernelTools>();
        ChatCompletionAgent agentWithTool = new()
        {
            Kernel = kernel,
            Instructions = "You know a lot about weather",
            Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            })
        };
        await foreach (AgentResponseItem<ChatMessageContent> response in agentWithTool.InvokeAsync("How is the weather"))
        {
            Console.WriteLine(response.Message);
        }

        //Can also use AzureAIAgent (via Microsoft.SemanticKernel.Agents.AzureAI) to connect to AI Foundry
        //Can also use OpenAIAgents to connect to OpenAI (if using OpenAI directly as provider)
    }
        break;
    default:
        throw new ArgumentOutOfRangeException();
}

#endregion

#region Embeddings

//This work for both
IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = app.Services.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

Embedding<float> vector = await embeddingGenerator.GenerateAsync("Something to vectorize");

#endregion

#region Vector Store Work

switch (framework)
{
    case FrameworkToUse.MicrosoftExtensionsAi:

        Console.WriteLine("MEAI do not have any thing to help here. You can get the vector, but else you need to work with the Vector Store Provider's NuGet Packages");
        break;
    case FrameworkToUse.SemanticKernel:

        //SK have support for various Vector Integrations (InMemory, AzureAISearch, CosmosDB, SQL Server, Postgres, Qdrant, redis, SQLLite, etc.)
        //All happen via Microsoft.Extensions.VectorData which SK Team is currently owner of

        //Create Vector Store (Can also be dependency injected)
        VectorStore store = new InMemoryVectorStore(new InMemoryVectorStoreOptions
        {
            EmbeddingGenerator = embeddingGenerator
        });

        //Insert Data
        VectorStoreEntity myData = new()
        {
            Id = Guid.NewGuid().ToString(),
            Description = "Hello World"
        };
        VectorStoreCollection<string, VectorStoreEntity> collection = store.GetCollection<string, VectorStoreEntity>("myStore");
        await collection.EnsureCollectionExistsAsync();
        await collection.UpsertAsync(myData);

        //Search
        await foreach (VectorSearchResult<VectorStoreEntity> vectorSearchResult in collection.SearchAsync("Worlds",
                           top: 3,
                           new VectorSearchOptions<VectorStoreEntity>
                           {
                               IncludeVectors = false
                           }))
        {
            Console.WriteLine(vectorSearchResult.Record.Description);
        }

        break;
    default:
        throw new ArgumentOutOfRangeException();
}

#endregion