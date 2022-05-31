using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MojecStockERP.Models
{
    public class KYC
    {
        public string DateShared { get; set; }
        public string ACno1 { get; set; }
        public string Acno2 { get; set; }
        public string SBCsMain { get; set;}
        public string ARN { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string BU { get; set; }
        public string Tarriff { get; set; }
        public string MeterStatus { get; set; }
        public string TypeOfApartment { get; set; }
        public string MeterRecommended { get; set; }
        public string AdditionalComment { get; set; }
        public string Disco { get; set; }
    }
}