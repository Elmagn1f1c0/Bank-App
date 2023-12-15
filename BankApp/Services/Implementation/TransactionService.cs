using BankApp.DTO;
using BankApp.Models;
using BankApp.Repository.TransactionRepository;
using BankApp.Services.Interface;

namespace BankApp.Services.Implementation
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repo;

        public TransactionService(ITransactionRepository repo)
        {
            _repo = repo;
        }
        public Response CreateNewTransaction(Transaction transaction)
        {
            return _repo.CreateNewTransaction(transaction);
        }

        public async Task DeleteTransaction(int Id)
        {
            await  _repo.DeleteTransaction(Id);
        }

        public Response FindTransactionByDate(DateTime date)
        {
            return _repo.FindTransactionByDate(date);
        }

        public Response GetAll()
        {
            return _repo.GetAll();
        }

        public async Task<Transaction> GetById(int Id)
        {
           return await _repo.GetById(Id);
        }

        public Response MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin)
        {
            return _repo.MakeDeposit(AccountNumber, Amount, TransactionPin);
        }

        public Response MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {
            return _repo.MakeFundsTransfer(FromAccount, ToAccount, Amount, TransactionPin);
        }

        public Response MakeWithdrawal(string AccountNumber, decimal Amount, string TransactionPin)
        {
            return _repo.MakeWithdrawal(AccountNumber, Amount, TransactionPin);
        }
    }
}
