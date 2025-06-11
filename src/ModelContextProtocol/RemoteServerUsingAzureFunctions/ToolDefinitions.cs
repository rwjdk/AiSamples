namespace RemoteServerUsingAzureFunctions;

public static class ToolDefinitions
{
    public static class Tool1
    {
        public const string Name = "MyFirstTool";
        public const string Description = "A simple tool that just say Hello";
    }

    public static class Tool2
    {
        public const string Name = "MySecondToolWith2Parameters";
        public const string Description = "A tool that shows paramater usage by asking for two numbers and add them together";

        public static class Param1
        {
            public const string Name = "FirstNumber";
            public const string Description = "The first number to add";
        }

        public static class Param2
        {
            public const string Name = "SecondNumber";
            public const string Description = "The second number to add";
        }
    }

    public static class DataTypes
    {
        public const string Number = "number";
    }
}