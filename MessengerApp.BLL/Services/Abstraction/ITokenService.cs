using MessengerApp.Core.DTO.Authorization;
using MessengerApp.Core.ResultModel.Generics;
using MessengerApp.DAL.Entities.Authorization;

namespace MessengerApp.BLL.Services.Abstraction
{
    public interface ITokenService
    {
        Result<GenerateTokenDto> GenerateTempToken(
            User user);

        Result<GenerateTokenDto> GenerateRefreshToken(
            User user);
    }
}