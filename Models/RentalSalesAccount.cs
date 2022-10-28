using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaekjeCulturalComplexApi.Models
{
    [Table("tb_rental_sales_account")]
    public class RentalSalesAccount
    {
        [Key]
        [Column("seq")]
        public Guid Seq { get; set; }
        [Column("account_nm")]
        public string AccountName { get; set; }
        [Column("transaction_start_dt")]
        public DateTime TransactionStartDate { get; set; }
        [Column("transaction_end_dt")]
        public DateTime TransactionEndDate { get; set; }
        [Column("division_code")]
        public string DivisionCode { get; set; }
        [Column("item_nm")]
        public string ItemName { get; set; }
        [Column("is_use")]
        public bool IsUse { get; set; }
        [Column("note")]
        public string Note { get; set; }
        // public string SalesItemsJson { get; set; }
        [Column("reg_dt")]
        public DateTime RegDate { get; set; }
        [Column("update_dt")]
        public DateTime UpdateDate { get; set; }
    }
}
