using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MojecStockERP.Models
{
    public class MetersProduced
    {
        [Display(Name = "Meter No")]
        public string MeterNo { get; set; }
        [Display(Name = "Meter Type")]
        public string MeterType { get; set; }
        [Display(Name = "Model")]
        public string Model { get; set; }
        [Display(Name = "Version")]
        public string Version { get; set; }
        [Display(Name = "Supply Date")]
        [DataType(DataType.Date)]
        public string DateOfSupply { get; set; }
        [Display(Name = "Partners")]
        public string Partners { get; set; }
    }
}