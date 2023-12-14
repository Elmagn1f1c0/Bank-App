﻿using BankApp.Enums;

namespace BankApp.DTO
{
    public class TransactionRequest
    {
        
        public decimal TransactionAmount { get; set; }
        public string TransactionSourceAccount { get; set; }
        public string TransactionDestinationAccount { get; set; }
        public TranType TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}