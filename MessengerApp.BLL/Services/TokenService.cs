using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using MessengerApp.BLL.Services.Abstraction;
using MessengerApp.Core;
using MessengerApp.Core.DTO.Authorization;
using MessengerApp.Core.ResultConstants;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.Entities.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MessengerApp.BLL.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<User> _userManager;

        public TokenService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public Result<GenerateTokenDto> GenerateTempToken(
            User user)
        {
            try
            {
                var notBefore = DateTime.UtcNow;

                var expires = notBefore.Add(TimeSpan.FromMinutes(AuthOptions.TokenLifetime));

                var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.Issuer,
                    audience: AuthOptions.Audience,
                    notBefore: notBefore,
                    claims: new List<Claim>
                    {
                        new(ClaimTypes.Email, user.Email),
                        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new(ClaimTypes.Role, 
                            _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault()!)
                    },
                    expires: expires,
                    signingCredentials: new SigningCredentials(AuthOptions.SymmetricSecurityKey,
                        SecurityAlgorithms.HmacSha256)
                );

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                return Result<GenerateTokenDto>.CreateSuccess(new GenerateTokenDto(encodedJwt, expires));
            }
            catch (Exception e)
            {
                return Result<GenerateTokenDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public Result<GenerateTokenDto> GenerateRefreshToken(
            User user)
        {
            try
            {
                var notBefore = DateTime.UtcNow;

                var expires = notBefore.Add(TimeSpan.FromMinutes(AuthOptions.RefreshTokenLifetime));

                var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.Issuer,
                    audience: AuthOptions.Audience,
                    notBefore: notBefore,
                    claims: new List<Claim>
                    {
                        new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                        new(JwtRegisteredClaimNames.Jti, AuthOptions.SymmetricSecurityKey.ToString())
                    },
                    expires: expires,
                    signingCredentials: new SigningCredentials(AuthOptions.SymmetricSecurityKey,
                        SecurityAlgorithms.HmacSha512)
                );

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                return Result<GenerateTokenDto>.CreateSuccess(new GenerateTokenDto(encodedJwt, expires));
            }
            catch (Exception e)
            {
                return Result<GenerateTokenDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }
    }
}