using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace RemoteServer.Tools;

public class MyServerTool
{
    [KernelFunction, Description("Description of Tool1")]
    public static string Tool1()
    {
        return "Response from Tool1";
    }

    [KernelFunction, Description("Description of Tool2")]
    public static string Tool2(string input1)
    {
        return $"Response from Tool2. Your input was: {input1}";
    }
}