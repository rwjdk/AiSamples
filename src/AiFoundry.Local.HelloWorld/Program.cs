using Microsoft.AI.Foundry.Local;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Diagnostics;

#region Check if Foundry Local is installed

string packageId = "Microsoft.FoundryLocal";
Process process = new()
{
    StartInfo = new ProcessStartInfo
    {
        FileName = "winget",
        Arguments = $"list --id={packageId}",
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
    }
};
process.Start();
string output = process.StandardOutput.ReadToEnd();
process.WaitForExit();
bool isFoundryInstalled = output.Contains(packageId, StringComparison.OrdinalIgnoreCase);

#endregion

#region Install Part (if needed)

if (!isFoundryInstalled)
{
    Console.WriteLine("Foundry Local not yet installed. Installing... (this might take a few minutes)");

    Process installProcess = new()
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "winget",
            Arguments = "install Microsoft.FoundryLocal --accept-package-agreements --accept-source-agreements --silent",
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };
    installProcess.Start();
    installProcess.WaitForExit();
}

#endregion

#region Start Foundry and download model if needed

string modelAlias = "qwen2.5-coder-0.5b";
Console.WriteLine($"Starting AI Model '{modelAlias}'. If not already started / cached this might take a while...");
FoundryLocalManager manager = await Microsoft.AI.Foundry.Local.FoundryLocalManager.StartModelAsync(modelAlias);
ModelInfo? modelInfo = await manager.GetModelInfoAsync(modelAlias);

#endregion

#region Normal AI but with local runtime

IKernelBuilder builder = Kernel.CreateBuilder();

builder.AddOpenAIChatCompletion(modelInfo!.ModelId, manager.Endpoint, "no-api-key");
Kernel kernel = builder.Build();

ChatCompletionAgent agent = new()
{
    Kernel = kernel,
    Instructions = "You are a nice local agent"
};

Console.WriteLine("Write to start chatting :-)");

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

#endregion