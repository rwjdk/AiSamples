using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Plugins.Core;

//Video covering this sample: https://youtu.be/B44eE62aQJg

#pragma warning disable SKEXP0070
const string chatModel = "llama3.2";
const string endpoint = "http://127.0.0.1:11434";

IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddOllamaChatCompletion(modelId: chatModel, new Uri(endpoint));
Kernel kernel = kernelBuilder.Build();
kernel.ImportPluginFromType<TimePlugin>();

ChatCompletionAgent agent = new()
{
    Kernel = kernel,
    Instructions = "You are a friendly AI, helping the user to answer questions",
    Arguments = new KernelArguments(new OllamaPromptExecutionSettings
    {
        Temperature = 0.5f,
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
    })
};

List<ChatMessageContent> conversation = [];
while (true)
{
    Console.Write("> ");
    string? inputFromUser = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(inputFromUser))
    {
        conversation.Add(new ChatMessageContent(AuthorRole.User, inputFromUser));
        await foreach (AgentResponseItem<ChatMessageContent> response in agent.InvokeAsync(conversation))
        {
            conversation.Add(new ChatMessageContent(AuthorRole.Assistant, response.Message.Content));
            Console.Write(response.Message);
        }
    }

    Console.WriteLine();
    Console.WriteLine(string.Empty.PadLeft(50, '*'));
    Console.WriteLine();
}