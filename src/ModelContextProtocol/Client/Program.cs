using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using ModelContextProtocol.Client;
using Shared;

#pragma warning disable SKEXP0001

Configuration configuration = Shared.ConfigurationManager.GetConfiguration();

ServerToConnectTo serverToConnectTo = ServerToConnectTo.RemoteServer;

switch (serverToConnectTo)
{
    case ServerToConnectTo.LocalServerExe:
    {
        string pathToLocalServerExe = "LocalServer.exe"; //todo: Set path to local Server
        var clientTransport = new StdioClientTransport(new StdioClientTransportOptions
        {
            Name = "LocalServer",
            Command = pathToLocalServerExe,
            Arguments = [],
        });

        var client = await McpClientFactory.CreateAsync(clientTransport);

        await ListAvailableTools(client);
        await CallATool(client, "MyServerTool_Tool1", []);
        await CallATool(client, "MyServerTool_Tool2", new Dictionary<string, object> { { "input1", "MyInput" } });
        break;
    }
    case ServerToConnectTo.GitHub:
    {
        var clientTransport = new StdioClientTransport(new StdioClientTransportOptions
        {
            Name = "GitHub",
            Command = "npx",
            Arguments = ["-y", "@modelcontextprotocol/server-github"],
        });

        var client = await McpClientFactory.CreateAsync(clientTransport); //Notice: No Authentication

        await ListAvailableTools(client);
        Kernel kernel = GetKernel();
        IList<McpClientTool> tools = await client.ListToolsAsync();
        kernel.Plugins.AddFromFunctions("GitHub", tools.Select(aiFunction => aiFunction.AsKernelFunction()));

        ChatCompletionAgent agent = new()
        {
            Instructions = "You are an AI that can talk to GitHub (using tools)",
            Kernel = kernel,
            Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { RetainArgumentTypes = true }), //Notice: RetainArgumentTypes
            })
        };

        const string prompt = "Summarize the last four commits to the microsoft/semantic-kernel repository.";
        await foreach (AgentResponseItem<ChatMessageContent> response in agent.InvokeAsync(prompt))
        {
            Console.WriteLine(response.Message);
        }

        break;
    }
    case ServerToConnectTo.RemoteServer:
    {
        string urlToRemoteServer = "https://localhost:7200"; //todo: Set URL of Remote server
        var clientTransport = new SseClientTransport(new SseClientTransportOptions
        {
            Name = "RemoteServer",
            UseStreamableHttp = true,
            Endpoint = new Uri(urlToRemoteServer),
        });

        var client = await McpClientFactory.CreateAsync(clientTransport);

        await ListAvailableTools(client);
        await CallATool(client, "MyServerTool_Tool1", []);
        await CallATool(client, "MyServerTool_Tool2", new Dictionary<string, object> { { "input1", "MyInput" } });
        break;
    }
}

async Task ListAvailableTools(IMcpClient mcpClient)
{
    Console.WriteLine("Available Tools");
    foreach (var tool in await mcpClient.ListToolsAsync())
    {
        Console.WriteLine($"- {tool.Name} ({tool.Description})");
    }
}

async Task CallATool(IMcpClient mcpClient, string toolName, Dictionary<string, object> args)
{
    Console.WriteLine($"Call tool: {toolName}");
    var result = await mcpClient.CallToolAsync(
        toolName,
        args,
        cancellationToken: CancellationToken.None);
    Console.WriteLine(result.Content.First(c => c.Type == "text").Text);
}

Kernel GetKernel()
{
    IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
    kernelBuilder.Services.AddAzureOpenAIChatCompletion(configuration.ChatDeploymentName, configuration.Endpoint, configuration.Key);
    Kernel kernel1 = kernelBuilder.Build();
    return kernel1;
}

public enum ServerToConnectTo
{
    GitHub,
    LocalServerExe,
    RemoteServer
}