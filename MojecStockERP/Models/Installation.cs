using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MojecStockERP.Models
{
    public class Installation
    {
        [Required(ErrorMessage = "Please enter Details")]
        [Display(Name = "Meter No")]
        public string MeterNo { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        [Display(Name = "Meter Type")]
        public string MeterType { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        [Display(Name = "Installed")]
        public string Installed { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        [Display(Name = "Disco")]
        public string Disco { get; set; }

        [Required(ErrorMessage = "Please enter Date")]
        [Display(Name = "Date Installed")]
        [DataType(DataType.Date)]
        public string DateInstalled { get; set; }

        public string BU { get; set; }

    }
}