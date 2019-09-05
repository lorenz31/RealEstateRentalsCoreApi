using System;

namespace RealEstateCore.Core.Models
{
    public class TransactionHistory : BaseClass
    {
        public DateTime DatePaid { get; set; }
        public decimal AmountDue { get; set; }
        public decimal AmountPaid { get; set; }
        public string PaymentFor { get; set; }
        public decimal Balance { get; set; }
        public DateTime NextDateDue { get; set; }
        public DateTime DaysBeforeDue { get; set; }

        public Renter Renter { get; set; }
        public Guid RenterId { get; set; }
    }
}