using BankApp.DTO;

namespace BankApp.Services.Interface
{
    public interface IAuthService
    {
        string GenerateTokenString(LoginUser user);
        Task<ResponseDto<bool>> RegisterUser(RegisterUser user);
        Task<ResponseDto<bool>> Login(LoginUser user);
    }
}
