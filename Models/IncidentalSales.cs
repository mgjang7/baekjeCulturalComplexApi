using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaekjeCulturalComplexApi.Models
{
    [Table("tb_incidental_sales_status")]
    public class IncidentalSales
    {
        [Key]
        [Column("seq")]
        public Guid Seq { get; set; }
        [Column("division_code")]
        public string DivisionCode { get; set; }
        [Column("item_nm")]
        public string ItemName { get; set; }
        [Column("sales_dt")]
        public DateTime SalesDate { get; set; }
        [Column("supply_price")]
        public int SupplyPrice { get; set; }
        [Column("surtax")]
        public int Surtax { get; set; }
        [Column("sum_price")]
        public int SumPrice { get; set; }
        [Column("note")]
        public string Note { get; set; }
        [Column("reg_dt")]
        public DateTime RegDate { get; set; }
        [Column("update_dt")]
        public DateTime UpdateDate { get; set; }
    }
}
