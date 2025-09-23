using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Plugins.Core;

#pragma warning disable SKEXP0070
string apiKey = "todo";
string modelId = "todo";

IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddGoogleAIGeminiChatCompletion(modelId, apiKey);
Kernel kernel = kernelBuilder.Build();
kernel.ImportPluginFromType<TimePlugin>();

ChatCompletionAgent agent = new()
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
    string? inputFromUser = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(inputFromUser))
    {
        conversation.Add(new ChatMessageContent(AuthorRole.User, inputFromUser));
        await foreach (AgentResponseItem<StreamingChatMessageContent> response in agent.InvokeStreamingAsync(conversation))
        {
            conversation.Add(new ChatMessageContent(AuthorRole.Assistant, response.Message.Content));
            Console.Write(response.Message);
        }
    }

    Console.WriteLine();
    Console.WriteLine(string.Empty.PadLeft(50, '*'));
    Console.WriteLine();
}