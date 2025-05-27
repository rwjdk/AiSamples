using LocalServer.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using ModelContextProtocol.Server;

#pragma warning disable SKEXP0001

var builder = Host.CreateApplicationBuilder();

//Semantic Kernel Part
Kernel kernel = Kernel.CreateBuilder().Build();
KernelPlugin skTool = kernel.ImportPluginFromType<MyServerTool>();

//Turn SK Plugins into MCP ServerTools
List<McpServerTool> mcpTools = [];
mcpTools.AddRange(skTool.Select(x => McpServerTool.Create(x.AsAIFunction())));

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools(mcpTools);

await builder.Build().RunAsync();