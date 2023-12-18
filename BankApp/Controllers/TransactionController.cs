using AutoMapper;
using BankApp.DTO;
using BankApp.Models;
using BankApp.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Controllers
{
    [Authorize]
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
        public IActionResult MakeDeposit()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin)
        {
            try
            {
                Response response = _service.MakeDeposit(AccountNumber, Amount, TransactionPin);

                if (response.ResponseCode == "03")
                {
                    ModelState.AddModelError(string.Empty, "Invalid account number or pin");
                    return View();
                }
                else if (response.ResponseCode == "05")
                {
                    ModelState.AddModelError(string.Empty, "Insufficient balance for deposit");
                    return View();
                }
                else if (response.ResponseCode == "06")
                {
                    ModelState.AddModelError(string.Empty, "Deposit must not exceed 200,000");
                    return View();
                }
                else if (response != null && response.ResponseCode == "00")
                {
                    return RedirectToAction("TransactionIndex");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to make a deposit.");
                    return View();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Deposit amount should not be within the deposit range"))
                {
                    ModelState.AddModelError("Deposit", "Deposit amount should not be within the deposit range");
                    return BadRequest(ModelState);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while depositing");
                }
                return View();
            }
        }

        public IActionResult MakeTransfer()
        {

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MakeTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {
            try
            {
                Response response = _service.MakeFundsTransfer(FromAccount, ToAccount, Amount, TransactionPin);

                if (response.ResponseCode == "03")
                {
                    ModelState.AddModelError(string.Empty, "Invalid account number or pin");
                    return View();
                }
                else if (response.ResponseCode == "05")
                {
                    ModelState.AddModelError(string.Empty, "Insufficient balance for transfer");
                    return View();
                }
                else if (response.ResponseCode == "06")
                {
                    ModelState.AddModelError(string.Empty, "Transfer must not exceed 200,000");
                    return View();
                }

                else if (response != null)
                {
                    return RedirectToAction("TransactionIndex");
                }
                else
                {

                    ModelState.AddModelError(string.Empty, "Failed to make a transfer.");
                    return View();
                }
            }catch (Exception ex)
            {
                if (ex.Message.Contains("Transfer amount should not be within the certain range"))
                {
                    ModelState.AddModelError("Transfer", "Transfer amount should not be within the transfer range");
                    return BadRequest(ModelState);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while Transferring");
                }
                return View();
            }

            

        }
        public IActionResult MakeWithdrawal()
        {

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MakeWithdrawal(string AccountNumber, decimal Amount, string TransactionPin)
        {
            try
            {
                Response response = _service.MakeWithdrawal(AccountNumber, Amount, TransactionPin);

                if (response.ResponseCode == "03")
                {
                    ModelState.AddModelError(string.Empty, "Invalid account number or pin");
                    return View();
                }
                else if (response.ResponseCode == "05")
                {
                    //return BadRequest("Insufficient balance for withdrawal");
                    ModelState.AddModelError(string.Empty, "Insufficient balance for withdrawal");
                    return View();
                }
                else if (response.ResponseCode == "06")
                {
                    ModelState.AddModelError(string.Empty, "Withdrawal must not exceed 200,000");
                    return View();
                }

                else if (response != null)
                {
                    return RedirectToAction("TransactionIndex");
                }
                else
                {

                    ModelState.AddModelError(string.Empty, "Failed to make a withdrawal.");
                    return View();
                }

            }catch (Exception ex)
            {
                if (ex.Message.Contains("Withdrawal amount should not be within the certain range"))
                {
                    ModelState.AddModelError("Withdrawal", "Withdrawal amount should not be within the withdraw range");
                    return BadRequest(ModelState);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while withdrawaing");
                }
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
                return View(model);
            }

            var account = _mapper.Map<Transaction>(model);

            await _service.DeleteTransaction(model.Id);

            return RedirectToAction(nameof(TransactionIndex));

        }
    }
}
