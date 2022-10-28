using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaekjeCulturalComplexApi.Models
{
    public class RentalSalesAccountViewModel
    {
        public Guid Seq { get; set; }
        public int Order { get; set; }
        public string AccountName { get; set; }
        public string TransactionStartDate { get; set; }
        public string TransactionEndDate { get; set; }

        public string TransactionPeriod { get; set; }
        //public DateTime TransactionStartDate { get; set; }
        //public DateTime TransactionEndDate { get; set; }
        public string DivisionCode { get; set; }
        public string ItemName { get; set; }
        public bool IsUse { get; set; }
        public string Note { get; set; }
        public string SalesItemsJson { get; set; }
        public string RegDate { get; set; }
        public string UpdateDate { get; set; }
        //public DateTime RegDate { get; set; }
        //public DateTime UpdateDate { get; set; }
    }
}
