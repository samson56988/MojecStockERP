using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MojecStockERP.Models
{
    public class MetersDispatched
    {
        public int MeterID { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        [Display(Name = "Meter No")]
        public string MeterNo { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        [Display(Name = "Meter Type")]
        public string MeterType { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        [Display(Name = "Model")]
        public string Model { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        [Display(Name = "Software Version")]
        public string SoftwareVersion { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        [Display(Name = "Hardware Version")]
        public string HardwareVersion { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        [Display(Name = "Dispatched Date")]
        [DataType(DataType.Date)]
        public string DateOfDispatch { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        [Display(Name = "Partners")]
        public string Partners { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        public string Disco { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        public string SGC { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        public string TarrifIndex { get; set; }
    }
}