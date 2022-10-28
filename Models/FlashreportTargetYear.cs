using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaekjeCulturalComplexApi.Models
{
    [Table("tb_flashreport_target_year")]
    public class FlashreportTargetYear
    {
        [Key]
        [Column("seq")]
        public Guid Seq { get; set; }

        [Column("target_year")]
        public int TargetYear { get; set; }

        [Column("target_month")]
        public int TargetMonth { get; set; }


        [Column("pay_complex_day_visitor")]
        public int PayComplexDayVisitor { get; set; }

        [Column("pay_complex_night_visitor")]
        public int PayComplexNightVisitor { get; set; }

        [Column("pay_historyhall_visitor")]
        public int PayHistoryhallVisitor { get; set; }

        [Column("free_complex_visitor")]
        public int FreeComplexVisitor { get; set; }

        [Column("free_historyhall_visitor")]
        public int FreeHistoryhallVisitor { get; set; }

        [Column("pay_complex_day_sales")]
        public int PayComplexDaySales { get; set; }

        [Column("pay_complex_night_sales")]
        public int PayComplexNightSales { get; set; }

        [Column("pay_historyhall_sales")]
        public int PayHistoryhallSales { get; set; }

        [Column("Incidental_sabiro_sales")]
        public int IncidentalSabiroSales { get; set; }

        [Column("Incidental_experience_sales")]
        public int IncidentalExperienceSales { get; set; }

        [Column("Incidental_rental_sales")]
        public int IncidentalRentalSales { get; set; }

        [Column("Incidental_goods_sales")]
        public int IncidentalGoodsSales { get; set; }

        [Column("Incidental_rme_sales")]
        public int IncidentalRmeSales { get; set; }

        [Column("rental_sales")]
        public int RentalSales { get; set; }



        [Column("reg_dt")]
        public DateTime RegDate { get; set; }
        [Column("update_dt")]
        public DateTime UpdateDate { get; set; }
    }
}
