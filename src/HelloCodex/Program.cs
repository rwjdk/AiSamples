using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Shared;

var configuration = ConfigurationManager.GetConfiguration();

var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddAzureOpenAIChatCompletion(configuration.ChatDeploymentName, configuration.Endpoint, configuration.Key);
var kernel = kernelBuilder.Build();

var agent = new ChatCompletionAgent
{
    Kernel = kernel,
    Instructions = "You are a friendly AI, helping the user to answer questions",
};

List<ChatMessageContent> conversation = [];
while (true)
{
    Console.Write("> ");
    var inputFromUser = Console.ReadLine();
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
