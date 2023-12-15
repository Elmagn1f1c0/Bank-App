using BankApp.DTO;
using BankApp.Models;

namespace BankApp.Services.Interface
{
    public interface IAccountService
    {
        Account Authenticate(string AccountNumber, string Pin);
        IEnumerable<Account> GetAllAccounts();
        Task<Response> Create(Account account, string Pin, string ConfirmPin);
        Task<Response> Update(Account account, string Pin = null);
        Task Delete(int Id);
        Task<Account> GetById(int Id);
        Account GetByAccountNumber(string AccountNumber);
    }
}
