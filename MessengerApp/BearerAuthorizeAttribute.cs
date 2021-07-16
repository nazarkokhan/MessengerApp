using MessengerApp.Core.ResultConstants.AuthorizationConstants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
// ReSharper disable All

namespace MessengerApp
{
    public class BearerAuthorizeAttribute : AuthorizeAttribute
    {
        public BearerAuthorizeAttribute(Roles roles) : this()
        {
            Roles = roles.ToString();
        }

        public BearerAuthorizeAttribute()
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
        }
    }
}