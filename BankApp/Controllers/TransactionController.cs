using AutoMapper;
using BankApp.DTO;
using BankApp.Models;
using BankApp.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ITransactionService _service;
        IMapper _mapper;
        private const int PageSize = 5;

        public TransactionController(ITransactionService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }
        public IActionResult TransactionIndex(int page = 1)
        {
            var transactionsResponse = _service.GetAll();

            if (transactionsResponse.ResponseCode == "00" && transactionsResponse.Data is List<Transaction> transactions)
            {
                var paginatedTransactions = transactions
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                var model = new PaginatedList<Transaction>(paginatedTransactions, transactions.Count, page, PageSize);
                return View(model);
            }
            else
            {
                var emptyList = new PaginatedList<Transaction>(new List<Transaction>(), 0, 1, PageSize);
                return View(emptyList);
            }
        }
        public async Task<IActionResult> MakeDeposit()
        {

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin)
        {

            Response response = _service.MakeDeposit(AccountNumber, Amount, TransactionPin);

            if (response.ResponseCode == "03")
            {
                return BadRequest("Invalid username or pin");
            }
            else if (response.ResponseCode == "05")
            {
                return BadRequest("Insufficient balance for deposit");
            }

            if (response != null)
            {
                return RedirectToAction("TransactionIndex");
            }
            else
            {
               
                ModelState.AddModelError(string.Empty, "Failed to make a deposit.");
                return View();
            }
            
        }
        public async Task<IActionResult> MakeTransfer()
        {

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MakeTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {

            Response response = _service.MakeFundsTransfer(FromAccount, ToAccount,Amount, TransactionPin);

            if (response.ResponseCode == "03")
            {
                return BadRequest("Invalid username or pin");
            }
            else if (response.ResponseCode == "05")
            {
                return BadRequest("Insufficient balance for deposit");
            }

            if (response != null)
            {
                return RedirectToAction("TransactionIndex");
            }
            else
            {

                ModelState.AddModelError(string.Empty, "Failed to make a transfer.");
                return View();
            }

        }
        public async Task<IActionResult> MakeWithdrawal()
        {

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MakeWithdrawal(string AccountNumber, decimal Amount, string TransactionPin)
        {

            Response response = _service.MakeWithdrawal(AccountNumber, Amount, TransactionPin);

            if (response.ResponseCode == "03")
            {
                return BadRequest("Invalid username or pin");
            }
            else if (response.ResponseCode == "05")
            {
                return BadRequest("Insufficient balance for deposit");
            }

            if (response != null)
            {
                return RedirectToAction("TransactionIndex");
            }
            else
            {

                ModelState.AddModelError(string.Empty, "Failed to make a withdrawal.");
                return View();
            }

        }
        public async Task<IActionResult> DeleteTransaction(int Id)
        {

            var account = await _service.GetById(Id);

            if (account == null)
            {
                return NotFound();
            }
            TransactionRequest updateModel = new TransactionRequest
            {
                Id = account.Id,
                TransactionAmount = account.TransactionAmount,
                TransactionDate = account.TransactionDate,
                TransactionUniqueReference = account.TransactionUniqueReference,
                TransactionSourceAccount = account.TransactionSourceAccount,
                TransactionDestinationAccount = account.TransactionDestinationAccount,
                TransactionStatus = account.TransactionStatus,
                TransactionParticulars = account.TransactionParticulars
            };

            return View(updateModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTransaction(TransactionRequest model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Phone number or email already exists.");
                return View(model);
            }

            var account = _mapper.Map<Transaction>(model);

            await _service.DeleteTransaction(model.Id);

            return RedirectToAction(nameof(TransactionIndex));



        }
    }
}
