using BankApp.Models;

namespace BankApp.Repository.AccountRepository
{
    public interface IAccountRepository
    {
        Account Authenticate(string AccountNumber, string Pin);
        IEnumerable<Account> GetAllAccounts();
        Account Create(Account account, string Pin, string ConfirmPin);
        Task Update(Account account, string Pin = null);
        Task Delete(int Id);
        Task<Account> GetById(int Id);
        Account GetByAccountNumber(string AccountNumber);
    }
}
