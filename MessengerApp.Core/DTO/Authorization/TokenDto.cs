using System;
using System.Text.Json.Serialization;

namespace MessengerApp.Core.DTO.Authorization
{
    public class TokenDto
    {
        public TokenDto(string token, DateTime tokenExpTime, string refreshToken, DateTime refreshTokenExpTime)
        {
            Token = token;
            TokenExpTime = tokenExpTime;
            RefreshToken = refreshToken;
            RefreshTokenExpTime = refreshTokenExpTime;
        }

        [JsonPropertyName("token")]
        public string Token { get; }
        
        [JsonPropertyName("tokenExpTime")]
        public DateTime TokenExpTime { get; }
        
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; }
        
        [JsonPropertyName("refreshTokenExpTime")]
        public DateTime RefreshTokenExpTime { get; }
    }
}