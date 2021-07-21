using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MessengerApp.Core
{
    public static class AuthOptions
    {
        public const string Issuer = "MessengerApiServer";

        public const string Audience = "MessengerApiClient";

        private const string Key = "mysupersecret_secretkey!123";

        public const int Lifetime = 10080; //7 days

        public static SymmetricSecurityKey SymmetricSecurityKey =>
            new(Encoding.ASCII.GetBytes(Key));
    }
}