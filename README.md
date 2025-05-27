# AI Samples
In this Repo, we explore various AI Samples to learn to be successful with AI and Semantic Kernel

# Setup
These Samples rely on an Azure OpenAI Resource with the following to be set up with .NET User Secrets in the Shared Project (in the following format)

> **secrets.json**
```js    

{
  "Endpoint": "todo", //URL of your Azure OpenAI Service
  "Key": "todo", //Key of your Azure OpenAI Service
  "ChatDeploymentName": "todo", //DeploymentName of your Azure OpenAI Chat-model (example: "gpt-4o-mini")
  "EmbeddingModelName": "todo" //Embedding Model for RAG (example: "text-embedding-ada-002")
}
```

- See https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets on how to work with user-secrets
- See the how-to guides on how to create your Azure Resources

# Samples:
- [Structured Output](src/StructuredOutput)
- [Model Context Protocol](src/ModelContextProtocol)

# How-to Guides
- General
  - [How to create an Azure OpenAI Service in Azure](HowToGuides/HowToCreateAnAzureOpenAiServiceResourceInAzure.md#how-to-create-an-azure-open-ai-service-resource-in-azure)
 - For RAG
   - [How to Create an Azure Search Service Resource in Azure](HowToGuides/HowToCreateAnAzureSearchResourceInAzure.md#how-to-create-an-azure-search-service-resource-in-azure)
   - [How to Create a CosmosDB Service Resource in Azure](HowToGuides/HowToCreateACosmosDbResourceInAzure.md)
