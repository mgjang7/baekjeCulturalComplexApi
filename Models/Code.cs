using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BaekjeCulturalComplexApi.Models
{
    [Table("tb_code")]
    public class CodeItem
    {
        [Key]
        [Column("seq")]
        public Guid Seq { get; set; }
        [Column("group_code")]
        public string GroupCode { get; set; }
        [Column("group_code_nm")]
        public string GroupCodeName { get; set; }
        [Column("code")]
        public string Code { get; set; }
        [Column("code_nm")]
        public string CodeName { get; set; }
        [Column("is_use")]
        public bool IsUse { get; set; }
        [Column("reg_dt")]
        public DateTime RegDate { get; set; }
        [Column("update_dt")]
        public DateTime UpdateDate { get; set; }
    }
}