namespace MessengerApp.Core.ResultConstants.AuthorizationConstants
{
    public static class AccountEmailServiceConstants
    {
        /// <summary>
        /// Represents message when user sends token for password change.
        /// </summary>
        public const string ConfirmPasswordReset = "Confirm password reset.";
        
        /// <summary>
        /// Represents message when user sends token for email change.
        /// </summary>
        public const string ConfirmEmailReset = "Confirm email reset.";
        
        /// <summary>
        /// Represents message when user wants to register using email.
        /// </summary>
        public const string ConfirmRegistration = "Confirm your registration in Library.";

        /// <summary>
        /// Represents message when user successfully confirmed email by token.
        /// </summary>
        public const string RegistrationConfirmed = "Thanks for your registration.";
    }
}