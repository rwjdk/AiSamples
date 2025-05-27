# How to Create an Azure Open AI Service Resource in Azure

## Step 1. How to Create the Resource

- Go to portal.azure.com and log into you Azure Subscription (If you do not have any, use this guide: https://azure.microsoft.com/en-us/pricing/purchase-options/azure-account)
- Go to "Hamburger Menu" top Left and choose "Create a Resource"
- In the "Search service and marketplace" search-box type "Azure openai"
- Choose "Azure OpenAI", click Create > "Azure OpenAI" (you are take to the creation page)
- Choose your Subscription and Resource Group (or create a new)
- Choose your region (We recommend US-East or Swenden Central for most options)
- Choose a name for the resource (this will be part of your resource URL: https://<yourname>.openai.azure.com/)
- Choose Standard S0 (only option) as pricing tier (It is a 100% pay for what you use resource so don't worry about having a dorment resource as there is no fixed price)
- Click the Next button
- In Type choose "All Networks"
- Click Next
- Leave Tags blank and Click Next
- Finally Click create (it normally takes less than a minute to complete)
- Click on "Go to Resource"

## Step 2. How to get Endpoint and API Key
- Go to you Azure OpenAI Resource
- In the Left Sidebar go to "Resource Management" > "Keys and Endpoint"
- Note down one of your 2 keys (do not matter which one) and you endpoint (Location/Region is just information and will not be needed in your code)
(Should any of your keys be compromized use the "Regenerate Key x" at the top of this page)

## Step 3. How to deploy a model
- Go to you Azure OpenAI Resource and in Overview press "Go to Azure AI Foundery Portal" (or https://ai.azure.com/ for faster access)
- Once on Azure AI Foundery Portal go to "Deployments" in the left sidebar
- Press the "+ Deploy Model"
- Deploy the Model you wish to have available (note down the name you give it as that is needed later in code)
  - We recommend you deploy at least the following models for the Hackathon: "gpt-4o-mini" and "text-embedding-ada-002"
    - Some models are Request Access only. Feel free to request them but it can take up to a month or longer to get the access so do not assume you can try them during the hackathon

(You should now have 3 pieces of info: An Endpoint, a Key and a Model name. If that is the case you are ready to proceed with the code part 🙌)
