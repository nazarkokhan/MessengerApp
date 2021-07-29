using System;
// ReSharper disable All

namespace MessengerApp.Core.DTO.Authorization
{
    public class RefreshTokenDto
    {
        public RefreshTokenDto(string token)
        {
            Token = token;
        }

        public string Token { get; }
    }
}