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
            Console.WriteLine(item.Message.Content);

            //Hack
            Pageable<PersistentThreadMessage> persistentThreadMessages = client.Messages.GetMessages(item.Thread.Id);
            PersistentThreadMessage lastMessage = persistentThreadMessages.AsPages().Select(x => x.Values.Where(y => y.Attachments.Count > 0)).Last().Last();
            Response<PersistentAgentFileInfo> file = client.Files.GetFile(lastMessage.Attachments.Last().FileId);
        }
    }
    finally
    {
        await agentThread.DeleteAsync();
    }
}