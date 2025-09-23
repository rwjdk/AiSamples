# AI Instructions
- This file provides guidance to AI when working with code in this repository.
- Refer to README.md for additional information.

## Planning Mode
- If a user prefixes their request with 'Plan: ', assume you are in planning mode
- IMPORTANT: While in Planning mode, do not write or change code. Only update plans in folder `ImplementationPlans`

### How to plan
Step 1: While in Planning mode, make a new file called `<yyyyMMdd> - <name_of_plan>.md` that is based on the `ImplementationPlans\NewFeatureTemplatePlan.md` file
Step 2: Look at the existing codebase and once ready **in details** make a plan on how you intend to implement the user's feature request.
Step 3: Then ask the user any questions regarding the plan that are unclear (in your response: not the Implementation File). Present these questions in the chat in a numbered list so the user can refer to them and answer you.
Step 4: With the answers from the user, repeat Step 2 and Step 3.
Step 5: If you do not have any other questions, inform the user and request them to say 'Start Implementation'.
Step 6: If the user say 'Start Implementation', then start you code-generation work (show the user the implementation steps in your responses and how you one the fly check them off).

## Commands: Building, Running, and Package Management
- **Build solution**: `dotnet build Website.sln`
- **Run application**: `dotnet run --project src/Website/Website.csproj`
- **Run tests**: `dotnet test src/Tests/Website.Tests.csproj`
- **Add package**: `dotnet add src/Website package <PackageName>`

## Available Tools
- **Context7**: For questions about MudBlazor code and other NuGet packages use the context7 tools to gather information
- **Playwright**: Can open the website when running and navigate it using this tool (Use it if you need to inspect HTML)
- **TrelloDotNet-MCP**: Can tell you how to use the TrelloDotNet NuGet Package

## Project Details
- **Target Framework**: .NET 9.0 and C# 13 (Never downgrade version and always use the latest C# Features)
- **Project Type**: MicrosoftAspNetCore (Blazor Server + Minimal API)
- **Database**: Microsoft SQL Server
- **UI Framework**: Blazor Server with [MudBlazor](https://mudblazor.com/docs/overview) and [Radzen](https://blazor.radzen.com/) components
- **AI Framework**: [Semantic Kernel](https://learn.microsoft.com/en-us/semantic-kernel/overview/) working against [OpenAI API](https://openai.com/api/) and [Azure OpenAI API](https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/openai/Azure.AI.OpenAI)
- **Testing**: NUnit framework
- **Code Style**: Warnings treated as errors, EditorConfig enforced
- **Logging**: Serilog with structured logging

### Key Design Patterns
- **Extension Methods**: Extensive use for service registration and configuration
- **Minimal APIs**: Job and service endpoints mapped via extension methods
- **Repository Pattern**: Implied through EF Core DbContext usage
- **Background Jobs**: External scheduler integration rather than hosted services
- **Event-Driven**: Heavy reliance on webhooks for real-time data synchronization

### Solution structure
The Solution has two projects. The Main Project and the Test Project

```
src
├─ Website                                  # The Main Project
|   ├─ AiTools                              # Semantic Kernel AI Function Calling Tools
|   ├─ Api                                  # Minimal API exposed via the Hub
|   ├─ Components                           # Reusable MudBlazor-based Blazor Components (Prefer these over raw MudBlazor)
|   ├─ Constants                            # Various Constant IDs used throughout the project
|   ├─ Dialogs                              # Blazor Dialogs that are used on multiple pages implemented in MudBlazor (Prefer these over raw HTML)
|   ├─ Extensions                           # Various Extension Methods
|   ├─ HtmlReport                           # Legacy Report (Do not add new)
|   ├─ Interfaces                           # General Purpose Interfaces
|   ├─ Jobs                                 # Various Jobs that trigger by an HTTP Put Request
|   ├─ Models                               # Shared Model Classes
|   ├─ Pages                                # All Blazor Pages go here (each in their own folder with code-behind file)
|   ├─ Services                             # Public Facing Endpoints Offering various services
|   ├─ Systems                              # Various External Systems
|   |  ├─ Ai                                # Integration with OpenAi and AzureOpenAi (Semantic Kernel NuGet)
|   |  ├─ Browser                           # IJSRuntime Integrations with users prowser
|   |  ├─ GitHub                            # Integrations with the GitHub API (Octokit NuGet)
|   |  ├─ HubSpot                           # Integrations with the HubSpot API (REST)
|   |  ├─ MicrosoftGraph                    # Integrations with the Microsoft Graph API (REST)
|   |  ├─ MicrosoftTeams                    # Integrations with the Microsoft Teams (Incoming Webhooks)
|   |  ├─ Nager                             # Integrations with the Naget.at (Public Holidays via REST)
|   |  ├─ Shared                            # Integrations that interact with multiple of the other services
|   |  ├─ Slack                             # Integrations with the Slack API (Slack.NetStandard NuGet)
|   |  ├─ SqlServer                         # Integrations with the Hub's Database
|   |  ├─ SqlServer.ModelContextProtocol    # Integrations with external MCP Database (For stats)
|   |  ├─ SqlServer.MyPortal                # Integrations with external Portal Database (For External Job and Error Logs)
|   |  ├─ SqlServer.VectorStore             # Integrations with SQL Server as VectorStore
|   |  └─ Trello                            # Integrations with Trello (TrelloDotNet NuGet)
|   ├─ Utilities                            # Various smaller utilities
|   └─ Webhooks                             # Various Webhooks from external Systems
|      ├─ GitHub                            # Webhooks from GitHub (AI Reviews, Check States, and Pull Request move of card in Trello)
|      ├─ HubSpot                           # Webhooks from HubSpot (New Deals Ready for Onboarding)
|      ├─ Slack                             # Webhooks from Slack (New Messages, Replies, and Reactions from Customer and Partner Channels)
|      └─ Trello                            # Webhooks from Trello (Various automations on Boards)
└─ Tests                                    # UnitTest Project
```

---

## Code Generation Rules

### General Code Generation Rules
- Always enforce rules in file `src/Website/.editorconfig`
- Never use Nuget packages that are not MIT or Apache 2 unless specifically instructed to.
- If the new code is big and relevant for the README.md, then include the changes (do not include minor tweaks and changes)
- If there is anything for me to do after the fact, then mark it with //Todo: <what to do>
- Unless specified, do not expose functionality via Endpoints, but use it directly in the Blazor Pages and Jobs
- Do not add Tests unless specified
- Remove any unused usings if the new files you create

### Blazor Code Generation Rules
- All new pages should have their own Folder under the 'Pages' folder, always use Code-Behind files, and the .razor file should be suffixed with 'Page.razor' (Example: 'MyNewFeaturePage.razor')
- Always remember that all new pages also need a menu item in a MenuStructure so the user can navigate to the new page (Add to `src\Website\Models\MenuPageId.cs` and place it in the `src\Website\Models\MenuStructure.cs`)
- All GUI should be implemented with [MudBlazor](https://github.com/MudBlazor/MudBlazor) except Charts that should be implemented by Radzen Blazor
- Always prefer components in the 'Components' folder over raw MudBlazor components if available (Example 1: use <RButton> instead of <MudButton>. Example 2: use <RSelect> instead of <MudSelect>)
- Place all Dialogs that are used on multiple pages in the 'Dialogs' Folder, else place it in a Dialogs sub-folder under the page-folder

### Entity Framework-specific Code Generation Rules
- You are allowed to make new EF Core entities, but never make the Entity Framework Migrations (I will do that myself).
- String in EF Core Entities should specify their length, or ignore Resharper warnings if they need to be NVARCHAR(MAX)
- Always use the MaxLength attribute on strings in EF Core. If the string should have no maximum length, then add `// ReSharper disable once EntityPropertyHasNoSizeLimit` comment for the string.
- Always use inject SqlServerQuery and SqlServerCommand for Entity Framework Queries and Commands (Never inject a DBContext directly)

### Trello-Specific Code Generation Rules
- All code around Trello is written with NuGet TrelloDotNet. If in doubt of how to use then check the [Source Code](https://github.com/rwjdk/TrelloDotNet/tree/main/src/TrelloDotNet) or the [Wiki](https://github.com/rwjdk/TrelloDotNet/wiki).
- When using new Properties (aka fields in the REST API) from Card or Board, always check if the source of the Cards/Boards has these property fields included in their GetCardOptions/GetBoardOptions.
- Never use other packages for Trello.

### AI Integration Code Generation Rules
- For AI Integration [Semantic Kernel](https://github.com/microsoft/semantic-kernel) is used against OpenAI and Azure OpenAI

---

## Code Styles Rules
- Always use braces, even if it is not technically needed (example: Add braces for an if statement with a single line of code inside it).
- When initalizing collections prefer '[]' instead of example 'new List<>()' or 'Array.Empty<>' (Example: ```[]``` instead of ```Array.Empty<Car>()``` and ```new List<Car>()```);
- Never use the var keyword (always use the full qualifying name).
- Prefer primary constructors over regular constructors
- Prefer 'simplified new' over explicit new (Example: ```Car c = new();``` instead of ```Car c = new Car();```)
- Private fields should be prefixed with "_" to limit the need for this keyword
- Don't add XML Summaries and trivial comments (This is an internal project)
- Check that you do not have any unused usings
- Always place private classes at the bottom of parent classes
- Never make classes sealed
- All methods that have the async keyword should be named with the 'Async' suffix