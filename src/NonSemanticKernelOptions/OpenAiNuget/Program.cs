using OpenAI;
using OpenAI.Chat;

string openAiKey = "todo";
const string chatModel = "todo";

OpenAIClient client = new(openAiKey);
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