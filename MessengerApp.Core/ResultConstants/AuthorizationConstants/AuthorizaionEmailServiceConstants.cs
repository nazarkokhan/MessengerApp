namespace MessengerApp.Core.ResultConstants.AuthorizationConstants
{
    public static class AccountEmailServiceConstants
    {
        /// <summary>
        /// Represents message when user sends token for password change.
        /// </summary>
        public const string ConfirmPasswordReset = "Confirm password reset.";
        
        /// <summary>
        /// Represents message in email subject when user sends token for email change.
        /// </summary>
        public const string ConfirmEmailReset = "Confirm email reset.";
        
        /// <summary>
        /// Represents message when user uses token for email confirmation.
        /// </summary>
        public const string UseThisUrlToConfirmRegistration = "Use this url to confirm registration: ";
        
        /// <summary>
        /// Represents message when user wants to register using email.
        /// </summary>
        public const string ConfirmRegistration = "Confirm your registration in Library.";

        /// <summary>
        /// Represents message when user successfully confirmed email by token.
        /// </summary>
        public const string RegistrationConfirmed = "Thank you for registration in Pipe Messenger.";
    }
}