using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Plugins.Core;

#pragma warning disable SKEXP0070
var apiKey = "todo";
string modelId = "todo";

var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddGoogleAIGeminiChatCompletion(modelId, apiKey);
var kernel = kernelBuilder.Build();
kernel.ImportPluginFromType<TimePlugin>();

var agent = new ChatCompletionAgent
{
    Kernel = kernel,
    Instructions = "You are a friendly AI, helping the user to answer questions",
    Arguments = new KernelArguments(new GeminiPromptExecutionSettings
    {
        Temperature = 0.5f,
        ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions,
    })
};

List<ChatMessageContent> conversation = [];
while (true)
{
    Console.Write("> ");
    var inputFromUser = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(inputFromUser))
    {
        conversation.Add(new ChatMessageContent(AuthorRole.User, inputFromUser));
        await foreach (var response in agent.InvokeStreamingAsync(conversation))
        {
            Console.Write(response.Message);
        }
    }

    Console.WriteLine();
    Console.WriteLine(string.Empty.PadLeft(50, '*'));
    Console.WriteLine();
}