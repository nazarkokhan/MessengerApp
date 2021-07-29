using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MessengerApp.Core
{
    public static class AuthOptions
    {
        public const string Issuer = "MessengerApiServer";

        public const string Audience = "MessengerApiClient";

        private const string Key = "mysupersecret_secretkey!123";

        public const int TokenLifetime = 1; // 1 minute
        
        public const int RefreshTokenLifetime = 525_600; // 365 days

        public static SymmetricSecurityKey SymmetricSecurityKey =>
            new(Encoding.ASCII.GetBytes(Key));
    }
}