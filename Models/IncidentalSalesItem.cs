using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BaekjeCulturalComplexApi.Models
{
    [Table("tb_incidental_sales_item")]
    public class IncidentalSalesItem
    {
        [Key]
        [Column("seq")]
        public Guid Seq { get; set; }
        [Column("division_code")]
        public string DivisionCode { get; set; }
        [Column("division_nm")]
        public string DivisionName { get; set; }
        [Column("item_nm")]
        public string ItemName { get; set; }
        [Column("is_use")]
        public bool IsUse { get; set; }
        [Column("note")]
        public string Note { get; set; }
        [Column("reg_dt")]
        public DateTime RegDate { get; set; }
        [Column("update_dt")]
        public DateTime UpdateDate { get; set; }
    }
}