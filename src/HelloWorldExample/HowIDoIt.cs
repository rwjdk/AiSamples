using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Shared;

namespace HelloWorld;

public static class HowIDoIt
{
    public static async Task Run()
    {
        Configuration configuration = ConfigurationManager.GetConfiguration();

        IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.AddAzureOpenAIChatCompletion(configuration.ChatDeploymentName, configuration.Endpoint, configuration.Key);
        Kernel kernel = kernelBuilder.Build();

        ChatCompletionAgent agent = new()
        {
            Kernel = kernel,
            Instructions = "You are a friendly AI, helping the user to answer questions",
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
    }
}