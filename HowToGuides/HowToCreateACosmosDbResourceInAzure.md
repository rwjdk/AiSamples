# How to Create an Cosmos DB Service Resource in Azure

## Step 1. How to Create the Resource

- Go to portal.azure.com and log into you Azure Subscription (If you do not have any, use this guide: https://azure.microsoft.com/en-us/pricing/purchase-options/azure-account)
- Go to "Hamburger Menu" top Left and choose "Create a Resource"
- In the "Search service and marketplace" search-box type "cosmosdb"
- Choose "Azure Cosmos DB", click Create > "Azure Cosmos DB" (you are take to the creation page)
- Choose "Azure Cosmos DB for NoSQL"
- Choose your Subscription and Resource Group (or create a new)
- Choose a service name for the resource
- Choose Availability Zones = Disabled
- Choose Capacity Mode = Serverless
- Press Review + create (this can take some time)
- Click on "Go to Resource"

## Step 2. How to get Connection String
- Go to your Cosmos DB Resource
- In the Left Sidebar go to "Settings" > "Keys"
- Note down your PRIMARY CONNECTION STRING
(Should any of your keys be compromized use the "Regenerate" button. You properly should not use the Admin key for everything in production)

## Step 3. Create Database
- Go to your Cosmob DB Resource
- In the Left Sidebar go to "Data Explorer"
- Choose the dropdown next to where is says "+ New Container" and Choose new Database
- Enter the name of the Database and click OK

## Step 4. Enable Vector Store Support
- Go to your Cosmos DB Resource
- In the Left Sidebar go to "Settings" > "Features"
- Click on 'Vector Search for NoSQL API' and Enable it (this can take a while)

(You should now have 2 pieces of info: An Connection-string and a Database name. If that is the case you are ready to proceed with the code part 🙌)
