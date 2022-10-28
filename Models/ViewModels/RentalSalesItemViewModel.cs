using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BaekjeCulturalComplexApi.Models
{
    public class RentalSalesItemViewModel
    {
        public int Order { get; set; }
        public Guid Seq { get; set; }

        public string DivisionCode { get; set; }

        public string DivisionName { get; set; }

        public string ItemName { get; set; }

        public bool IsUse { get; set; }

        public string Note { get; set; }

        public string RegDate { get; set; }

        public string UpdateDate { get; set; }
    }
}