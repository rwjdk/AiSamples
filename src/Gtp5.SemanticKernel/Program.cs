using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using OpenAI.Chat;
using Shared;
using ChatMessageContent = Microsoft.SemanticKernel.ChatMessageContent;

Configuration configuration = ConfigurationManager.GetConfiguration();

IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
string deploymentName = "gpt-5-mini"; //4 Options: gtp-5 / gpt-5-mini / gpt-5-nano (+ gpt-5-chat)
kernelBuilder.AddAzureOpenAIChatCompletion(deploymentName, configuration.Endpoint, configuration.Key);
Kernel kernel = kernelBuilder.Build();

ChatCompletionAgent agent = new()
{
    Kernel = kernel,
    Instructions = "You are a friendly AI, helping the user to answer questions.",
    Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings
    {
        //Temperature = 0, //NB: Not allowed anymore on the GPT-5 Models

        ReasoningEffort = "high", //low, medium, high (there is a 'minimal' setting in raw API but not exposed in NUGET)
        //- NB: Default is medium!
        //- NB: You can't get the Reasoning Text (Raw OpenAI API can create a summary, but not exposed in the OpenAI Nuget Package)

        //New: verbosity also exist, but not yet exposed  //low, medium, high
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
        string answer = string.Empty;
        await foreach (AgentResponseItem<ChatMessageContent> response in agent.InvokeAsync(conversation))
        {
            answer += response.Message.Content;
            Console.Write(response.Message);

            //Optional: Get how many tokens the interaction cost
            if (response.Message.Metadata?.TryGetValue("Usage", out object? usage) == true && usage is ChatTokenUsage chatTokenUsage)
            {
                Console.WriteLine();
                Console.WriteLine($"[Token Usage: " +
                                  $"{chatTokenUsage.InputTokenCount} In | " +
                                  $"{chatTokenUsage.OutputTokenCount} Out (" +
                                  $"{chatTokenUsage.OutputTokenDetails.ReasoningTokenCount} was for used for reasoning" +
                                  $")]");
            }
        }

        conversation.Add(new ChatMessageContent(AuthorRole.Assistant, answer));
    }

    Console.WriteLine();
    Console.WriteLine(string.Empty.PadLeft(50, '*'));
    Console.WriteLine();
}