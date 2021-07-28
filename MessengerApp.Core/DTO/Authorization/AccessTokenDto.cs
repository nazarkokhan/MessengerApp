using System;
// ReSharper disable All

namespace MessengerApp.Core.DTO.Authorization
{
    public class AccessTokenDto
    {
        public AccessTokenDto(string token, DateTime expTime)
        {
            Token = token;
            ExpTime = expTime;
        }

        public string Token { get; }

        public DateTime ExpTime { get; }
    }
}