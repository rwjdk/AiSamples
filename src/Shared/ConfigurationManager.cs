using Microsoft.Extensions.Configuration;

namespace Shared;

public class ConfigurationManager
{
    /* This ConfigurationManager relies on .NET User Secrets in the following format
    ************************************************************************************************************************************************
    {
      "Endpoint": "todo", //URL of your Azure OpenAI Service
      "Key": "todo", //Key of your Azure OpenAI Service
      "ChatDeploymentName": "todo" //DeploymentName of your Azure OpenAI Chat-model (example: "gpt-4o-mini")
    }
    ************************************************************************************************************************************************
    - See the how-to guides on how to create your Azure Resources in the ReadMe
    - See https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets on how to work with user-secrets
    ************************************************************************************************************************************************
    */

    public static Configuration GetConfiguration()
    {
        IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddUserSecrets<ConfigurationManager>().Build();
        Exception notSetupException = new Exception("It seems you have not yet set up you ConfigurationManager in the Shared Project. Please go there to do so");
        string endpoint = configurationRoot["Endpoint"] ?? throw notSetupException;
        string key = configurationRoot["Key"] ?? throw notSetupException;
        string chatDeploymentName = configurationRoot["ChatDeploymentName"] ?? throw notSetupException;

        return new Configuration(endpoint, key, chatDeploymentName);
    }
}