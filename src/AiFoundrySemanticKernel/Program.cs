using Azure;
using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.AzureAI;
using Microsoft.SemanticKernel.ChatCompletion;
using Shared;

//Video covering this sample: https://youtu.be/vHpvWsksFoU

#pragma warning disable SKEXP0110
Configuration configuration = ConfigurationManager.GetConfiguration();
string agentId = configuration.AzureAiFoundryAgentId;
string endpoint = configuration.AzureAiFoundryAgentEndpoint;

PersistentAgentsClient client = AzureAIAgent.CreateAgentsClient(endpoint, new DefaultAzureCredential());

//await client.Administration.CreateAgentAsync("model", "name", "description", "instructions" /*tools, etc*/);

PersistentAgent definition = await client.Administration.GetAgentAsync(agentId);

AzureAIAgent agent = new(definition, client);

await Basic();
await CodeTool();
await BingSearch();

async Task Basic()
{
    AzureAIAgentThread agentThread = new(agent.Client);
    try
    {
        ChatMessageContent message = new(AuthorRole.User, "Hello");

        await foreach (AgentResponseItem<ChatMessageContent> item in agent.InvokeAsync(message, agentThread))
        {
            Console.WriteLine(item.Message.Content);
        }
    }
    finally
    {
        await agentThread.DeleteAsync();
    }
}

async Task CodeTool()
{
    AzureAIAgentThread agentThread = new(agent.Client);
    try
    {
        ChatMessageContent message = new(AuthorRole.User, "Hello. Please generate a chart of top countries by population (Use Code interpreter to do it and give it back as a PNG)");

        await foreach (AgentResponseItem<ChatMessageContent> item in agent.InvokeAsync(message, agentThread))
        {
            string filePath = string.Empty;
            string orgFilename = string.Empty;
            foreach (KernelContent content in item.Message.Items)
            {
                if (content is AnnotationContent annotationContent) //Note: There are various other content types: FileReferenceContent,ImageContent,FunctionCallContent and FunctionResultContent
                {
                    Response<PersistentAgentFileInfo>? file = await client.Files.GetFileAsync(annotationContent.ReferenceId);
                    orgFilename = file.Value.Filename;
                    Response<BinaryData> fileContent = await client.Files.GetFileContentAsync(annotationContent.ReferenceId);
                    filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");
                    File.WriteAllBytes(filePath, fileContent.Value.ToArray());
                    await Task.Factory.StartNew(() =>
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = true
                        });
                    });
                }
            }

            bool isCode = item.Message.Metadata?.ContainsKey("code") ?? false;
            if (!isCode)
            {
                Console.WriteLine(item.Message.Content!.Replace("sandbox:" + orgFilename, filePath));
            }
        }
    }
    finally
    {
        await agentThread.DeleteAsync();
    }
}

async Task BingSearch()
{
    //Note: This key need to be in format: /subscriptions/<subscription_id>/resourceGroups/<resource_group_name>/providers/Microsoft.CognitiveServices/accounts/<ai_service_name>/projects/<project_name>/connections/<connection_name>
    BingGroundingSearchConfiguration bingToolConfiguration = new(configuration.BingApiKey);
    BingGroundingSearchToolParameters bingToolParameters = new([bingToolConfiguration]);
    PersistentAgent definitionWithBing = await client.Administration.CreateAgentAsync(
        name: "Bing Search Agent",
        model: configuration.ChatDeploymentName,
        tools: [new BingGroundingToolDefinition(bingToolParameters)]);

    AzureAIAgent agentWithBing = new(definitionWithBing, client);
    AzureAIAgentThread agentThread = new(agentWithBing.Client);
    try
    {
        ChatMessageContent message = new(AuthorRole.User, "What is the weather like in paris right now (use Bing Search tool)");
        await foreach (AgentResponseItem<ChatMessageContent> item in agentWithBing.InvokeAsync(message, agentThread))
        {
            Console.WriteLine(item.Message.Content);
        }
    }
    finally
    {
        await agentThread.DeleteAsync();
        await client.Administration.DeleteAgentAsync(agentWithBing.Id);
    }
}