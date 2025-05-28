# AI Samples
In this Repo, we explore various AI Samples to learn to be successful with AI in C# and Semantic Kernel

## Setup
Most of these Samples rely on an Azure OpenAI Resource with the following to be set up with .NET User Secrets in the Shared Project (in the following format)

> **secrets.json**
```js    

{
  "Endpoint": "todo", //URL of your Azure OpenAI Service
  "Key": "todo", //Key of your Azure OpenAI Service
  "ChatDeploymentName": "todo", //DeploymentName of your Azure OpenAI Chat-model (example: "gpt-4o-mini")
  "EmbeddingModelName": "todo", //[Optional] Embedding Model for RAG (example: "text-embedding-ada-002")
  "AzureAiFoundryAgentEndpoint" : "todo", //[Optional] Endpoint for the Azure AI Foundry Agents (if you wish to test those demos)
  "AzureAiFoundryAgentId" : "todo", //[Optional] ID of your agent for the Azure AI Foundry Agents (if you wish to test those demos)
  "BingApiKey" : "todo" //[OPTIONAL] If you wish to use BingSearch in AI Agents
}
```

- See https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets on how to work with user-secrets
- See the how-to guides on how to create your Azure Resources

## Samples:
- [Hello World](src/HelloWorldExample)
- [Structured Output](src/StructuredOutput)
- [Model Context Protocol](src/ModelContextProtocol)
- [Function Calling](src/FunctionCallingExample)
- [RAG](src/RagExample)
- [Azure AI Foundry Agents](/src/AiFoundryHelloWorld)
- [Non Azure OpenAI options](src/NonAzureOpenAiOptions)
- [Non Semantic Kernel options](src/NonSemanticKernelOptions)

## How-to Guides
- General
  - [How to create an Azure OpenAI Service in Azure](HowToGuides/HowToCreateAnAzureOpenAiServiceResourceInAzure.md#how-to-create-an-azure-open-ai-service-resource-in-azure)
 - For RAG
   - [How to Create an Azure Search Service Resource in Azure](HowToGuides/HowToCreateAnAzureSearchResourceInAzure.md#how-to-create-an-azure-search-service-resource-in-azure)
   - [How to Create a CosmosDB Service Resource in Azure](HowToGuides/HowToCreateACosmosDbResourceInAzure.md)
  
## Video Material
For more AI Tutorials and videos, please see my [YouTube Channel](https://www.youtube.com/@rwj_dk/videos)

