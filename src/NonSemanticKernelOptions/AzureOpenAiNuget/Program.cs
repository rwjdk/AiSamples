using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string azureOpenAiEndpoint = config["AiEndpoint"]!;
string azureOpenAiKey = config["AiKey"]!;
const string chatModel = "gpt-4o-mini";


AzureOpenAIClient client = new(new Uri(azureOpenAiEndpoint), new AzureKeyCredential(azureOpenAiKey));
ChatClient chatClient = client.GetChatClient(chatModel);

List<ChatMessage> messages = [];
while (true)
{
    Console.Write("> ");
    string? inputFromUser = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(inputFromUser))
    {
        messages.Add(new UserChatMessage(inputFromUser));
        await foreach (StreamingChatCompletionUpdate? response in chatClient.CompleteChatStreamingAsync(messages))
        {
            if (response.ContentUpdate.Count > 0)
            {
                Console.Write(response.ContentUpdate[0].Text);
            }
        }
    }

    Console.WriteLine();
    Console.WriteLine(string.Empty.PadLeft(50, '*'));
    Console.WriteLine();
}