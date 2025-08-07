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

        var kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.AddAzureOpenAIChatCompletion("gpt-5-mini", configuration.Endpoint, configuration.Key);
        var kernel = kernelBuilder.Build();

        var agent = new ChatCompletionAgent
        {
            Kernel = kernel,
            Instructions = "You are a friendly AI, helping the user to answer questions", //Give it some personality
        };

        List<ChatMessageContent> conversation = [];
        while (true)
        {
            Console.Write("> ");
            var inputFromUser = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(inputFromUser)) //Ignore if no user input
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