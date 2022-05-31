using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MojecStockERP.Models
{
    public class StoredMeters
    {
        public int MeterID { get; set; }
        [Display(Name = "Meter No")]
        public string MeterNo { get; set; }
        [Display(Name = "Meter Type")]
        public string MeterType { get; set; }
        [Display(Name = "Model")]
        public string Model { get; set; }
        [Display(Name = "Software Version")]
        public string SoftwareVersion { get; set; }
        [Display(Name = "Hardware Version")]
        public string HardwareVersion { get; set; }
        [Display(Name = "Partners")]
        public string Partners { get; set; }
        public string Disco { get; set; }
        public string SGC { get; set; }
        public string TarrifIndex { get; set; }
    }
}