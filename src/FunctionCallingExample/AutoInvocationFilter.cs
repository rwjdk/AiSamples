using Microsoft.SemanticKernel;

namespace FunctionCallingExample;

#pragma warning disable SKEXP0001
public class AutoInvocationFilter : IAutoFunctionInvocationFilter
{
    public async Task OnAutoFunctionInvocationAsync(AutoFunctionInvocationContext context, Func<AutoFunctionInvocationContext, Task> next)
    {
        try
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Function: {context.Function.Name} called");
            if (context.Arguments != null)
            {
                for (int i = 0; i < context.Arguments.Count; i++)
                {
                    Console.WriteLine($"- Arg: {context.Arguments.ToList()[i]}");
                }
            }


            await next(context);
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error:" + e.Message);
            throw;
        }
        finally
        {
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}