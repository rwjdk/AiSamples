using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Shared;
using StructuredOutputExample.Models;

//Video covering this sample: https://youtu.be/mW9ocIMHe7s

//Getting Configuration
Configuration configuration = ConfigurationManager.GetConfiguration();

Console.WriteLine("Welcome to the Structured Output Sample");
Console.WriteLine("1. When you ask an LLM a question, it will default decide how it respond respond back to you (plain text or markdown)");
Console.WriteLine("2. You can try to control the format output with Prompt-engineering, but it is still not a guarantee");
Console.WriteLine("3. This means that if we as an example ask the LLM to list the top 10 Movies according to IMDB we will get the list, but sometimes we just the names, other times we get Titles and Year and sometime with will mention the Director");
Console.WriteLine("4. The concept of structured output 'Fixes' this issue in that we define a ResponseFormat that result in the response from the LLM to always be JSON in the format of the Object we define");
Console.WriteLine(string.Empty.PadLeft(100, '*'));
Console.WriteLine("Let's try it out. Below give you option to choose Normal and Structured Output so you can see the difference:");

//Creating the Semantic Kernel Kernel-object
IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddAzureOpenAIChatCompletion(configuration.ChatDeploymentName, configuration.Endpoint, configuration.Key);
Kernel kernel = kernelBuilder.Build();

ChatMessageContent messageToLlm = new(AuthorRole.User, "What are the top 10 Movies according to IMDB?");

while (true)
{
    Console.Write("Press 'N' for Normal or 'S' for Structured output (or 'E' to Exit)");
    ConsoleKeyInfo key = Console.ReadKey();
    Console.Clear();
    switch (key.Key)
    {
        case ConsoleKey.N: //Normal Response
        {
            //Define an agent
            ChatCompletionAgent agent = new()
            {
                Kernel = kernel,
                Instructions = "You are a Movie Expert"
            };
            Console.WriteLine("Asking LLM 'What is the top 10 movies according to IMDB' using a normal response");

            await foreach (AgentResponseItem<StreamingChatMessageContent> response in agent.InvokeStreamingAsync(messageToLlm))
            {
                Console.Write(response.Message.Content);
            }

            Console.WriteLine("*** Please try this option a few times and notice that the response is slightly different every time ****");

            break;
        }
        case ConsoleKey.S: //Structured Output Response
        {
            //Define an agent (In structured we need to define the Response-format for the agent)
            ChatCompletionAgent agent = new()
            {
                Kernel = kernel,
                Instructions = "You are a Movie Expert",
                Arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings
                {
                    ResponseFormat = typeof(MovieResult) //This enables structured output (Must be an object (can't be a collection))
                })
            };


            Console.WriteLine("Asking LLM 'What is the top 10 movies according to IMDB' using a Structured Output");
            await foreach (AgentResponseItem<ChatMessageContent> response in agent.InvokeAsync(messageToLlm)) //Note that streaming content back do not make sense in structured output
            {
                string json = response.Message.Content!;
                MovieResult? movieResult = JsonSerializer.Deserialize<MovieResult>(json, new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter() } //Needed if you use enums as LLM get them as name strings
                });

                //Now that response is JSON we are in charge of the format
                int counter = 1;
                Console.WriteLine(movieResult!.MessageBack);
                foreach (Movie movie in movieResult.Top10Movies)
                {
                    Console.WriteLine($"{counter}: {movie.Title} ({movie.YearOfRelease}) - Genre: {movie.Genre} - Director: {movie.Director} - IMDB Score: {movie.ImdbScore}");
                    counter++;
                }

                Console.WriteLine("Average Score 'guessed' by LLM: " + movieResult.AverageScoreOfThe10Movies + " (yes the still can't count)");
                Console.WriteLine("Real Average Score via Code: " + movieResult.RealAverageScore);
            }

            break;
        }
        case ConsoleKey.E:
            Environment.Exit(-1);
            break;
        default:
            Console.WriteLine("Invalid selection. Choose 'S' for Structured Output or 'N' for Normal");
            break;
    }

    Console.WriteLine();
}