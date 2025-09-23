using LocalServer.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using ModelContextProtocol.Server;

//Video covering this sample: https://youtu.be/uM-RYDCSkzs

#pragma warning disable SKEXP0001

HostApplicationBuilder builder = Host.CreateApplicationBuilder();

//Semantic Kernel Part
Kernel kernel = Kernel.CreateBuilder().Build();
KernelPlugin skTool = kernel.ImportPluginFromType<MyServerTool>();

//Turn SK Plugins into MCP ServerTools
List<McpServerTool> mcpTools = [];
mcpTools.AddRange(skTool.Select(x => McpServerTool.Create(x.Clone())));

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools(mcpTools);

await builder.Build().RunAsync();