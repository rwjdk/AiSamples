using System.Text;
using FunctionCallingExample;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Plugins.Core;
using Shared;

//Video covering this sample: https://youtu.be/MaIpcjoL9Gc

Configuration configuration = ConfigurationManager.GetConfiguration();
IKernelBuilder builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(configuration.ChatDeploymentName, configuration.Endpoint, configuration.Key);

builder.Services.AddSingleton<IAutoFunctionInvocationFilter, AutoInvocationFilter>();

Kernel kernel = builder.Build();

kernel.ImportPluginFromType<TimePlugin>();

MyFirstPlugin myPlugin = new();
kernel.ImportPluginFromObject(myPlugin);

ChatCompletionAgent agent = new()
{
    Name = "MyFilesAgent",
    Kernel = kernel,
    Instructions = $"You are File Manager that can create and list files and folders. " +
                   $"When you create files and folder you need to give the full path based" +
                   $" on this root folder: {myPlugin.RootFolder}",

    Arguments = new KernelArguments(
        new AzureOpenAIPromptExecutionSettings
        {
            Temperature = 0.5,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        })
};

List<ChatMessageContent> conversation = [];
Console.OutputEncoding = Encoding.UTF8;
while (true)
{
    Console.Write("> ");
    string question = Console.ReadLine() ?? "";
    conversation.Add(new ChatMessageContent(AuthorRole.User, question));

    await foreach (AgentResponseItem<StreamingChatMessageContent> response in agent.InvokeStreamingAsync(conversation))
    {
        conversation.Add(new ChatMessageContent(AuthorRole.Assistant, response.Message.Content));
        Console.Write(response.Message);
    }

    Console.WriteLine();
    Console.WriteLine("*********************");
    Console.WriteLine();
}