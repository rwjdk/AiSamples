using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Mcp;

namespace RemoteServerUsingAzureFunctions;

public class StandardFunction()
{
    [Function("StandardFunction")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post")]
        HttpRequest req
    )
    {
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}

public class Functions()
{
    [Function("FunctionMcpTool1")]
    public IActionResult Run(
        [McpToolTrigger(ToolDefinitions.Tool1.Name, ToolDefinitions.Tool1.Description)]
        ToolInvocationContext context
    )
    {
        return new OkObjectResult("Hi. I'm Tool 1!");
    }
}

public class FunctionMcpTool2()
{
    [Function("FunctionMcpTool2")]
    public IActionResult Run(
        [McpToolTrigger(ToolDefinitions.Tool2.Name, ToolDefinitions.Tool2.Description)]
        ToolInvocationContext context,
        [McpToolProperty(ToolDefinitions.Tool2.Param1.Name, ToolDefinitions.DataTypes.Number, ToolDefinitions.Tool2.Param1.Description)]
        int firstNumber,
        [McpToolProperty(ToolDefinitions.Tool2.Param2.Name, ToolDefinitions.DataTypes.Number, ToolDefinitions.Tool2.Param2.Description)]
        int secondNumber)
    {
        return new OkObjectResult($"Hi. I'm Tool2!. The two numbers added together is: {firstNumber + secondNumber}");
    }
}