﻿using BankApp.Models;
using BankApp.Repository.AccountRepository;
using BankApp.Services.Interface;

namespace BankApp.Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repo;

        public AccountService(IAccountRepository repo)
        {
            _repo = repo;
        }
        public Account Authenticate(string AccountNumber, string Pin)
        {
            return _repo.Authenticate(AccountNumber, Pin);
        }

        public Account CreateAccount(Account account, string Pin, string ConfirmPin)
        {
            return _repo.Create(account, Pin, ConfirmPin);
        }

        public async Task Delete(int Id)
        {
            await _repo.Delete(Id);
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return _repo.GetAllAccounts();
        }

        public Account GetByAccountNumber(string AccountNumber)
        {
            return _repo.GetByAccountNumber(AccountNumber);
        }

        public Task<Account> GetById(int Id)
        {
            return _repo.GetById(Id);
        }

        public async Task Update(Account account, string Pin = null)
        {
            await _repo.Update(account, Pin);
        }
    }
}
