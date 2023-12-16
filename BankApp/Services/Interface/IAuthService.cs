using BankApp.DTO;

namespace BankApp.Services.Interface
{
    public interface IAuthService
    {
        Task<ResponseDto<string>> ExternalLogin(string email, string firstName, string surname);
    }
}
