using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using OpenAI.Chat;
using System.Text;
using Microsoft.SemanticKernel.ChatCompletion;
using Shared;
using ChatMessageContent = Microsoft.SemanticKernel.ChatMessageContent;

namespace HelloWorld;

public static class HowIDoItWithOptionalStuff
{
    public static async Task Run()
    {
        Configuration configuration = Shared.ConfigurationManager.GetConfiguration();

        IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.AddAzureOpenAIChatCompletion(configuration.ChatDeploymentName, configuration.Endpoint, configuration.Key);
        Kernel kernel = kernelBuilder.Build();
        ChatCompletionAgent agent = new()
        {
            Kernel = kernel,
            Name = "FriendlyAI", //Optional: Not that it really do anything. Warning name can't contain spaces
            Instructions = "You are a friendly AI, helping the user to answer questions",

            //Optional
            Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings
            {
                Temperature = 1, //Control how creative the model is (Between 0 and 2, Default is 1)
            }),
        };

        List<ChatMessageContent> conversation = [];
        //Optional todo: Add anything you wish the AI to know up front (Like you name, age, preferences etc.) [Could also be in the Agent instructions if need be to ensure it is all chats]
        //conversation.Add(new ChatMessageContent(AuthorRole.User, "My name is <your name here>"));

        Console.OutputEncoding = Encoding.UTF8;

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

                    //Optional: Get how many tokens the interaction cost
                    if (response.Message.Metadata?.TryGetValue("Usage", out object? usage) == true && usage is ChatTokenUsage chatTokenUsage)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"[Token Usage: {chatTokenUsage.InputTokenCount} In | {chatTokenUsage.OutputTokenCount} Out]");
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine(string.Empty.PadLeft(50, '*'));
            Console.WriteLine();
        }
    }
}