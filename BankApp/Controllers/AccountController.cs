using AutoMapper;
using BankApp.DTO;
using BankApp.Models;
using BankApp.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BankApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _service;
        protected Response _response;
        private readonly IMapper _mapper;
        public AccountController(IAccountService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }
        public IActionResult AccountIndex()
        {
            var accounts = _service.GetAllAccounts();
            var cleanAccounts = _mapper.Map<IList<GetAccountModel>>(accounts);
            return View(cleanAccounts);
        }
        public IActionResult Search(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return RedirectToAction(nameof(AccountIndex));
            }

            var accounts = _service.GetAllAccounts();

            if (accounts == null)
            {
                return BadRequest("It can't be empty");
            }

            searchString = searchString.ToLower();

            var searchResults = accounts
                .Where(account =>
                    account.AccountName.ToLower().Contains(searchString) ||
                    account.FirstName.ToLower().Contains(searchString) ||
                    account.LastName.ToLower().Contains(searchString) ||
                    account.AccountType.ToString().ToLower() == searchString ||
                    account.PhoneNumber.Contains(searchString) ||
                    account.AccountNumberGenerated.Contains(searchString))
                .Select(account => new GetAccountModel
                {
                    Id = account.Id,
                    FirstName = account.FirstName,
                    LastName = account.LastName,
                    AccountName = account.AccountName,
                    PhoneNumber = account.PhoneNumber,
                    Email = account.Email,
                    CurrentAccountBalance = account.CurrentAccountBalance,
                    AccountType = account.AccountType,
                    AccountNumberGenerated = account.AccountNumberGenerated,
                    DateCreated = account.DateCreated,
                    DateLastUpdated = account.DateLastUpdated
                })
                .ToList();

            if (searchResults.Count > 0)
            {
                return View("AccountIndex", searchResults);
            }

            return View("AccountIndex", new List<GetAccountModel>());
        }

        public async Task<IActionResult> CreateAccount()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAccount(RegisterNewAccountModel model)
        {
                var respone = new Response();
                if (!ModelState.IsValid)
                {
                    return BadRequest(model);
                }

                var account = _mapper.Map<Account>(model);
                var result = _service.CreateAccount(account, model.Pin, model.ConfirmPin);

                if (result != null)
                {
                    return RedirectToAction(nameof(AccountIndex));
                }

                return View(model);
           
        }



        public async Task<IActionResult> UpdateAccount(int id)
        {
            var account = await _service.GetById(id);

            if (account == null)
            {
                return NotFound(); 
            }
            UpdateAccountModel updateModel = new UpdateAccountModel
            {
                Id = account.Id,
                PhoneNumber = account.PhoneNumber,
                Email = account.Email,
                Pin = string.Empty,
                ConfirmPin = string.Empty, 
                DateLastUpdated = account.DateLastUpdated 
            };

            return View(updateModel); 
        }


        [HttpPost]
        public async Task<IActionResult> UpdateAccount(UpdateAccountModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Phone number or email already exists.");
                return View(model);
            }

            var account = _mapper.Map<Account>(model);

            await _service.Update(account);

            return RedirectToAction(nameof(AccountIndex));
        }

        public async Task<IActionResult> DeleteAccount(int Id)
        {

            var account = await _service.GetById(Id);

            if (account == null)
            {
                return NotFound();
            }
            AccountDTO updateModel = new AccountDTO
            {
                Id = account.Id,
                FirstName = account.FirstName,  
                LastName = account.LastName,
                AccountNumberGenerated = account.AccountNumberGenerated,
                AccountType = account.AccountType,
                PhoneNumber = account.PhoneNumber,
                Email = account.Email,
                DateLastUpdated = account.DateLastUpdated
            };

            return View(updateModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount(AccountDTO model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Phone number or email already exists.");
                return View(model);
            }

            var account = _mapper.Map<Account>(model);

            await _service.Delete(model.Id);

            return RedirectToAction(nameof(AccountIndex));



        }


    }
}
