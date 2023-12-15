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

        public TransactionController(ITransactionService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }
        public IActionResult TransactionIndex()
        {;
            var transactionsResponse = _service.GetAll();
            if (transactionsResponse.ResponseCode == "00" && transactionsResponse.Data is List<Transaction> transactions)
            {
                return View(transactions);
            }
            else
            {
                var emptyList = new List<Transaction>(); 
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
    }
}
