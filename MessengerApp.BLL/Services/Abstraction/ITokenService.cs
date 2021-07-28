using System.Threading.Tasks;
using MessengerApp.Core.DTO.Authorization;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.Entities.Authorization;

namespace MessengerApp.BLL.Services.Abstraction
{
    public interface ITokenService
    {
        Result<AccessTokenDto> GenerateTempToken(
            User user);

        Result<RefreshTokenDto> GenerateRefreshToken(
            User user);
    }
}