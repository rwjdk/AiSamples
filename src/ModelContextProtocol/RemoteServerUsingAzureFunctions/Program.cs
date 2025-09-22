using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using RemoteServerUsingAzureFunctions;

FunctionsApplicationBuilder builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();
builder.EnableMcpToolMetadata();

builder.ConfigureMcpTool(ToolDefinitions.Tool1.Name);

builder.ConfigureMcpTool(ToolDefinitions.Tool2.Name)
    .WithProperty(ToolDefinitions.Tool2.Param1.Name, ToolDefinitions.DataTypes.Number, ToolDefinitions.Tool2.Param1.Description)
    .WithProperty(ToolDefinitions.Tool2.Param2.Name, ToolDefinitions.DataTypes.Number, ToolDefinitions.Tool2.Param2.Description);

builder.Build().Run();