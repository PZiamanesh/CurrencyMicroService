namespace CurrencyMicroService.Core.Exceptions.MessageTemplates
{
    public static class ExceptionMessages
    {
        // General templates
        public static string UnexpectedError => "An unexpected error occurred";

        // Database operation templates
        #region Database operation errors
        public static string DatabaseRetrieveError => "Failed to retrieve {0} data from database";
        public static string DatabaseRetrieveByFilterError => "Failed to retrieve {0} data for {1}";
        public static string DatabaseAddError => "Failed to add {0} data";
        public static string DatabaseUpdateError => "Failed to update {0} data";
        public static string DatabaseSaveChangesError => "An unexpected error occurred while saving changes to the database";
        #endregion

        // Application settings templates
        #region Application settings errors
        public static string ApplicationSettingKeyNotFoundError => "No setting key found for {0}";
        public static string ApplicationSettingsInitializationError => "Error initializing application settings";
        #endregion

        // Job related templates
        #region Job related errors
        public static string JobExecutionError => "Error occurred in {0}";
        public static string JobRegistrationError => "Error registering {0}";
        #endregion

        // HTTP client templates
        #region HTTP client errors
        public static string HttpFetchError => "Error fetching {0} list from remote site";
        public static string ETSCurrencyHttpRowParsingError => "Error processing {0} row";
        #endregion
    }
}
