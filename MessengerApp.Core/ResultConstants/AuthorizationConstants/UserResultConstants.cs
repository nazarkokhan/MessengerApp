namespace MessengerApp.Core.ResultConstants.AuthorizationConstants
{
    public static class UserResultConstants
    {
        /// <summary>
        /// Represents message when user with the same email already exists in system.
        /// </summary>
        public const string ErrorCreatingUser = "ERROR_CREATING_USER";
        
        /// <summary>
        /// Represents message when something goes wrong removing users password.
        /// </summary>
        public const string ErrorRemovingPassword = "ERROR_REMOVING_PASSWORD";
        
        /// <summary>
        /// Represents message when something goes wrong adding password to user.
        /// </summary>
        public const string ErrorAddingPassword = "ERROR_ADDING_PASSWORD";
        
        /// <summary>
        /// Represents message when user with the same email already exists in system.
        /// </summary>
        public const string UserAlreadyExists = "USER_EXISTS";

        /// <summary>
        /// Represents message when user with this email does not exist.
        /// </summary>
        public const string UserNotFound = "USER_NOT_FOUND";

        /// <summary>
        /// Represents message when user sent invalid username or password.
        /// </summary>
        public const string InvalidUserNameOrPassword = "IVALID_USERNAME_OR_PASSWORD";

        /// <summary>
        /// Represents message when user sent invalid code for password resetting.
        /// </summary>
        public const string InvalidRegistrationToken = "IVALID_REGISTRATION_TOKEN";

        /// <summary>
        /// Represents message when user sent invalid code for password resetting.
        /// </summary>
        public const string InvalidResetPasswordToken = "IVALID_RESET_PASSWORD_TOKEN";

        /// <summary>
        /// Represents message when user sent invalid code for email resetting.
        /// </summary>
        public const string InvalidResetEmailToken = "IVALID_RESET_EMAIL_TOKEN";

        /// <summary>
        /// Represents message when refresh token is not valid.
        /// </summary>
        public const string InvalidRefreshToken = "INVALID_REFRESH_TOKEN";
        
        /// <summary>
        /// Represents message when refresh token is expired.
        /// </summary>
        public const string ExpiredRefreshToken = "EXPIRED_REFRESH_TOKEN";

        /// <summary>
        /// Represents message when user sent valid code for resetting, but it is already expired.
        /// </summary>
        public const string ExpiredResetPasswordToken = "EXPIRED_RESET_TOKEN";
        
        /// <summary>
        /// Represents message when the user sent valid code for resetting, but it is already expired.
        /// </summary>
        public const string UserEmailNotConfirmed = "USER_EMAIL_NOT_CONFIRMED";
        
        
        /// <summary>
        /// Represents message when the user sent valid code for resetting, but it is already expired.
        /// </summary>
        public const string UserDoesntHaveRole = "USER_DOESNT_HAVE_ROLE";
    }
}