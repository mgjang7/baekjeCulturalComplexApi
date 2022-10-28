using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BaekjeCulturalComplexApi.Models
{
    [Table("tb_manager")]
    public class Manager
    {
        [Key]
        [Column("seq")]
        public Guid Seq { get; set; }
        [Column("id")]
        public string Id { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("is_auth")]
        public bool IsAuth { get; set; }
        [Column("reg_dt")]
        public DateTime RegDate { get; set; }
        [Column("update_dt")]
        public DateTime UpdateDate { get; set; }
    }
}