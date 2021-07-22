namespace MessengerApp.Core.ResultConstants
{
    public static class CommonResultConstants
    {
        /// <summary>
        /// Represents message when a unexpected error was occurred. For example when DB, or some service doesn't response.
        /// </summary>
        public const string Unexpected = "UNEXPECTED";
        
        /// <summary>
        /// Represents message when you dont have rules to do this.
        /// </summary>
        public const string NoRules = "NO_RULES";
    }
}