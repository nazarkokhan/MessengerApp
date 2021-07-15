using System;

namespace MessengerApp.Core.ResultConstants.AuthorizationConstants
{
    [Flags]
    public enum Role
    {
        /// <summary>
        /// Represents User role.
        /// </summary>
        User = 1,
        
        /// <summary>
        /// Represents Admin role.
        /// </summary>
        Admin = 2
    }
}