# How to Create an Azure Search Service Resource in Azure

## Step 1. How to Create the Resource

- Go to portal.azure.com and log into you Azure Subscription (If you do not have any, use this guide: https://azure.microsoft.com/en-us/pricing/purchase-options/azure-account)
- Go to "Hamburger Menu" top Left and choose "Create a Resource"
- In the "Search service and marketplace" search-box type "Azure AI Search"
- Choose "Azure AI Search", click Create > "Azure AI Search" (you are take to the creation page)
- Choose your Subscription and Resource Group (or create a new)
- Choose your region (We recommend US-East or Swenden Central for most options)
- Choose a service name for the resource
- Press Change Pricing tier, Select Free and press Select
- Press Review + create
- Click on "Go to Resource"

## Step 2. How to get Endpoint and API Key
- Go to your Azure Search Resource
- In overview note down you URL on the top right
- In the Left Sidebar go to "Settings" > "Keys"
- Note down you primary Admin Key
(Should any of your keys be compromized use the "Regenerate" button. You properly should not use the Admin key for everything in production)

(You should now have 2 pieces of info: An URL and a Key. If that is the case you are ready to proceed with the code part 🙌)
