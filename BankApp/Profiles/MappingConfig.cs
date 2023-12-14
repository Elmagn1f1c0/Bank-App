using AutoMapper;
using BankApp.DTO;
using BankApp.Models;

namespace BankApp.Profiles
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<RegisterNewAccountModel, Account>();
            CreateMap<UpdateAccountModel, Account>();
            CreateMap<Account, GetAccountModel>();
            CreateMap<AccountDTO, Account>();
            CreateMap<TransactionRequest, Transaction>();
            
        }
    }
}
