using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SK_MEAI_Comparison;

public static class Tools
{
    [Description("Gets the weather")]
    public static string GetWeather() => Random.Shared.NextDouble() > 0.5 ? "It's sunny" : "It's raining";
}

public class SemanticKernelTools
{
    [KernelFunction("GetWeather")]
    public static string GetWeather() => Random.Shared.NextDouble() > 0.5 ? "It's sunny" : "It's raining";
}