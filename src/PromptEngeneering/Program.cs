using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Shared;

Configuration configuration = ConfigurationManager.GetConfiguration();

IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddAzureOpenAIChatCompletion(configuration.ChatDeploymentName, configuration.Endpoint, configuration.Key);
Kernel kernel = kernelBuilder.Build();

string developerMessage = "You are a friendly AI, helping the user to answer questions";
//developerMessage = "Speak like a pirate";
//developerMessage = "Always speak French back to the user";
//developerMessage = "return your answer in XML format";
//developerMessage = "Your answer should max be 10 words";
//developerMessage = "Return movie titles in French when asked for them";

ChatCompletionAgent agent = new()
{
    Kernel = kernel,
    Instructions = developerMessage,
    Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings
    {
        //ResponseFormat = typeof(OutputObject)
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

class OutputObject
{
    [Description("Always return the German Title")]
    public required string MovieName { get; set; }

    public required int ReleaseYear { get; set; }

    public required string Director { get; set; }
}