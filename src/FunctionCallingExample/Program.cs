using System.Text;
using FunctionCallingExample;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Plugins.Core;
using Shared;

Configuration configuration = ConfigurationManager.GetConfiguration();
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(configuration.ChatDeploymentName, configuration.Endpoint, configuration.Key);

builder.Services.AddSingleton<IAutoFunctionInvocationFilter, AutoInvocationFilter>();

Kernel kernel = builder.Build();

kernel.ImportPluginFromType<TimePlugin>();

var myPlugin = new MyFirstPlugin();
kernel.ImportPluginFromObject(myPlugin);

var agent = new ChatCompletionAgent
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
    var question = Console.ReadLine() ?? "";
    conversation.Add(new ChatMessageContent(AuthorRole.User, question));

    await foreach (var response in agent.InvokeStreamingAsync(conversation))
    {
        Console.Write(response.Message);
    }

    Console.WriteLine();
    Console.WriteLine("*********************");
    Console.WriteLine();
}