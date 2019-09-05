using System;

namespace RealEstateCore.Core.BusinessModels.DTO
{
    public class PropertyTransactionsDTO
    {
        public Guid UserId { get; set; }
        public Guid PropertyId { get; set; }
        public Guid TransactionId { get; set; }
        public DateTime DatePaid { get; set; }
        public double AmountDue { get; set; }
        public DateTime NextDateDue { get; set; }
        public DateTime DaysBeforeDue { get; set; }
        public Guid RenterId { get; set; }
        public double Balance { get; set; }
        public string PaymentFor { get; set; }
        public double AmountPaid { get; set; }
    }
}