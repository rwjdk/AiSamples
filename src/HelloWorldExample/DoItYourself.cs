namespace HelloWorld;

public static class DoItYourself
{
    public static async Task Run()
    {
        #region Step 1

        //Install Nuget: Microsoft.SemanticKernel + Microsoft.SemanticKernel.Agents.Core  (NB: It is a pre-release package so need to be turned on)

        #endregion

        #region Step 2

        //Create an Azure Open AI Service: https://portal.azure.com/
        //- Create the Service
        //- Find Endpoint and API Key
        //- Deploy a Model via Azure Open AI Studio
        const string azureOpenAiEndpoint = "";
        const string azureOpenAiApiKey = ""; //Todo - Should not be directly in real code!!! 
        const string modelDeploymentName = "gpt-4o-mini";

        #endregion

        #region Step 3

        //Build a kernel (Kernel.CreateBuilder) and register AddAzureOpenAIChatCompletion

        #endregion

        #region Step 4

        //- Create a ChatCompletionAgent instance, give it a name, bind it to the kernel and give it instructions (Developer/System Message)

        #endregion

        #region Step 5

        //Create a ChatHistory Object

        #endregion

        #region Step 6

        //Let's build a Chat-while(true) loop where is question the agent and let make the answer streaming so it feel more alive
        //- Ask for input and add that to history as a user-message
        //- Invoke the agent (A bit of a "funky" syntax if you are not used to IAsyncEnumerable)
        //- Iterate the response-content
        //- Tip: Console.OutputEncoding = Encoding.UTF8; to get Emoji to work

        #endregion
    }
}