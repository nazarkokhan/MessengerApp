using MessengerApp.Core;
using MessengerApp.Core.ResultConstants.AuthorizationConstants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MessengerApp.Extensions
{
    public static class ConfigureExtensions
    {
        public static void ConfigurePassword(this IdentityOptions options)
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = IdentityPasswordConstants.RequiredLength;
            options.Password.RequireNonAlphanumeric = false;
        }
        
        public static void JwtBearerOptions(this JwtBearerOptions options)
        {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = AuthOptions.Issuer,

                ValidateAudience = true,
                ValidAudience = AuthOptions.Audience,

                ValidateLifetime = true,

                IssuerSigningKey = AuthOptions.SymmetricSecurityKey,
                ValidateIssuerSigningKey = true,
            };
        }

        public static string GetLogFileName(this IConfiguration configuration) 
            => configuration.GetSection("FileLogging")["FileName"];
    }
}