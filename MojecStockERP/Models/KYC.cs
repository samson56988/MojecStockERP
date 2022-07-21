using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MojecStockERP.Models
{
    public class KYC
    {
        [Required(ErrorMessage = "Please enter Details")]
        public string DateShared { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        public string ACno1 { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        public string Acno2 { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        public string SBCsMain { get; set;}
        [Required(ErrorMessage = "Please enter Details")]
        public string ARN { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        public string BU { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        public string Tarriff { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        public string MeterStatus { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        public string TypeOfApartment { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        public string MeterRecommended { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        public string AdditionalComment { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        public string Disco { get; set; }
    }
}