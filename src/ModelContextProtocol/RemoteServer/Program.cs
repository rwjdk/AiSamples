using Microsoft.SemanticKernel;
using ModelContextProtocol.Server;
using RemoteServer.Tools;

//Video covering this sample: https://youtu.be/uM-RYDCSkzs

#pragma warning disable SKEXP0001
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Kernel kernel = Kernel.CreateBuilder().Build();
KernelPlugin skTool = kernel.ImportPluginFromType<MyServerTool>();

//Turn SK Plugins into MCP ServerTools
List<McpServerTool> mcpTools = [];
mcpTools.AddRange(skTool.Select(x => McpServerTool.Create(x.Clone())));

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithTools(mcpTools);

WebApplication app = builder.Build();
app.UseHttpsRedirection();

app.MapMcp();

app.Run();