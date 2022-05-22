using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MojecStockERP.Models
{
    public class Installation
    {
        [Display(Name = "Meter No")]
        public string MeterNo { get; set; }
        [Display(Name = "Meter Type")]
        public string MeterType { get; set; }
        [Display(Name = "Installed")]
        public string Installed { get; set; }
        [Display(Name = "Disco")]
        public string Disco { get; set; }
        [Display(Name = "Date Installed")]
        [DataType(DataType.Date)]
        public string DateInstalled { get; set; }

    }
}