using Azure;
using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Shared;

//Video covering this sample: https://youtu.be/vHpvWsksFoU

await RunAgentConversation();

async Task RunAgentConversation()
{
    Configuration configuration = ConfigurationManager.GetConfiguration();
    string agentId = configuration.AzureAiFoundryAgentId;
    string endpoint = configuration.AzureAiFoundryAgentEndpoint;
    string message = "Hello World.";
    //message = "Hello World. What is the weather like in paris right now (use Bing Search tool) and generate a png of the Weather-situation (use code interpreter)";

    //Original Code that do not exist. #Fail!
    //var endpoint = new Uri("myUrl");
    //AIProjectClient projectClient = new(endpoint, new DefaultAzureCredential());
    //PersistentAgentsClient agentsClient = projectClient.GetPersistentAgentsClient();

    PersistentAgentsClient agentsClient = new(endpoint, new DefaultAzureCredential());

    PersistentAgent agent = agentsClient.Administration.GetAgent(agentId);

    PersistentAgentThread thread = agentsClient.Threads.CreateThread();
    //Original
    //PersistentAgentThread thread = agentsClient.Threads.GetThread("thread_kaAc9Q1nGxFiVqnxmQJSX60S");


    PersistentThreadMessage messageResponse = agentsClient.Messages.CreateMessage(
        thread.Id,
        MessageRole.User,
        message);

    ThreadRun run = agentsClient.Runs.CreateRun(
        thread.Id,
        agent.Id);

    // Poll until the run reaches a terminal status
    do
    {
        await Task.Delay(TimeSpan.FromMilliseconds(500));
        run = agentsClient.Runs.GetRun(thread.Id, run.Id);
    } while (run.Status == RunStatus.Queued || run.Status == RunStatus.InProgress);

    if (run.Status != RunStatus.Completed)
    {
        throw new InvalidOperationException($"Run failed or was canceled: {run.LastError?.Message}");
    }

    Pageable<PersistentThreadMessage> messages = agentsClient.Messages.GetMessages(thread.Id, order: ListSortOrder.Ascending);

    // Display messages
    foreach (PersistentThreadMessage threadMessage in messages)
    {
        Console.Write($"{threadMessage.CreatedAt:yyyy-MM-dd HH:mm:ss} - {threadMessage.Role,10}: ");
        foreach (MessageContent contentItem in threadMessage.ContentItems)
        {
            if (contentItem is MessageTextContent textItem)
            {
                Console.Write(textItem.Text);
            }
            else if (contentItem is MessageImageFileContent imageFileItem)
            {
                Console.Write($"<image from ID: {imageFileItem.FileId}");
            }

            Console.WriteLine();
        }
    }
}