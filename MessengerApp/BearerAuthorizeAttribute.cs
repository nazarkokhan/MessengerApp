using MessengerApp.Core.ResultConstants.AuthorizationConstants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace MessengerApp
{
    public class BearerAuthorizeAttribute : AuthorizeAttribute
    {
        public BearerAuthorizeAttribute(Role roles) : this()
        {
            Roles = roles.ToString();
        }

        public BearerAuthorizeAttribute()
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
        }
    }
}