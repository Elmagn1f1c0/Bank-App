using BankApp.Data;
using BankApp.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BankApp.DTO;

namespace BankApp.Repository.AccountRepository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _db;

        public AccountRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public Account Authenticate(string AccountNumber, string Pin)
        {
            var account = _db.Accounts.Where(x => x.AccountNumberGenerated == AccountNumber).SingleOrDefault();
            if (account == null || !VerifyPinHash(Pin, account.PinHash, account.PinSalt))
            {
                return null;
            }

            return account;
        }

        private static bool VerifyPinHash(string Pin, byte[] pinHash, byte[] pinSalt)
        {
            if (string.IsNullOrWhiteSpace(Pin)) throw new ArgumentNullException("Pin");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(pinSalt))
            {
                var computedPinHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Pin));
                for (int i = 0; i < computedPinHash.Length; i++)
                {
                    if (computedPinHash[i] != pinHash[i]) return false;
                }
            }
            return true;
        }

        public Account Create(Account account, string Pin, string ConfirmPin)
        {
            var response = new Response();
            if (_db.Accounts.Any(x => x.Email == account.Email))
            {
                response.ResponseCode = "EmailExists";
                response.ResponseMessage = "Email already exists in the database.";
                response.Data = null;
                //throw new ApplicationException("An account already exists with this email");
            }
            if (_db.Accounts.Any(x => x.PhoneNumber == account.PhoneNumber))
            {
                //throw new ApplicationException("An account already exists with this phone number");
                response.ResponseCode = "PhoneNumber Exist";
                response.ResponseMessage = "Phone Number already exists in the database.";
                response.Data = null;
            }

            if (!Pin.Equals(ConfirmPin)) throw new ArgumentException("Pins do not match", "Pin");


            //Now that all validation passes, let us create account
            //we re hashing/encrypting pin first 
            byte[] pinHash, pinSalt;
            CreatePinHash(Pin, out pinHash, out pinSalt);

            account.PinHash = pinHash;
            account.PinSalt = pinSalt;

            _db.Accounts.Add(account);
            _db.SaveChanges();
            return account;

        }
        private static void CreatePinHash(string pin, out byte[] pinHash, out byte[] pinSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                pinSalt = hmac.Key;
                pinHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pin));
            }
        }

        public async Task Delete(int Id)
        {
            var response = new Response();
            var account =  _db.Accounts.Find(Id);
            if (account == null)
            {
                response.ResponseCode = "400";
                response.ResponseMessage = "There wasa product deleting an account";
                response.Data = null;

            }
            _db.Remove(account);
            await _db.SaveChangesAsync();

        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return _db.Accounts.ToList();
        }

        public Account GetByAccountNumber(string AccountNumber)
        {
            var account = _db.Accounts.Where(x => x.AccountNumberGenerated == AccountNumber).FirstOrDefault();
            if (account == null)
            {
                throw new ArgumentException("Account number is not correct.");
            }

            return account;
        }

        public async Task<Account> GetById(int Id)
        {
            var account = await _db.Accounts.Where(x => x.Id == Id).FirstOrDefaultAsync();
            if (account == null) return null;

            return account;
        }
        public async Task Update(Account account, string Pin = null)
        {
            var response = new Response();
            var accountToBeUpdated = _db.Accounts.Find(account.Id);
            if (accountToBeUpdated == null)
            {
                throw new ApplicationException("Account does not exist");
            }

            if (!string.IsNullOrWhiteSpace(account.Email))
            {
                if (_db.Accounts.Any(x => x.Email == account.Email && x.Id != account.Id))
                {
                    response.ResponseCode = "EmailExists";
                    response.ResponseMessage = "Email already exists in the database.";
                    response.Data = null;
                }

                accountToBeUpdated.Email = account.Email;
            }

            if (!string.IsNullOrWhiteSpace(account.PhoneNumber))
            {
                if (_db.Accounts.Any(x => x.PhoneNumber == account.PhoneNumber && x.Id != account.Id))
                {
                    response.ResponseCode = "PhoneNumberExist";
                    response.ResponseMessage = "Phone number already exists in the database.";
                    response.Data = null;
                }

                accountToBeUpdated.PhoneNumber = account.PhoneNumber;
            }
            if (!string.IsNullOrWhiteSpace(Pin))
            {
                byte[] pinHash, pinSalt;
                CreatePinHash(Pin, out pinHash, out pinSalt);
                accountToBeUpdated.PinHash = pinHash;
                accountToBeUpdated.PinSalt = pinSalt;
            }

            accountToBeUpdated.DateLastUpdated = DateTime.UtcNow;

            _db.Accounts.Update(accountToBeUpdated);
            await _db.SaveChangesAsync();
        }


    }
}
