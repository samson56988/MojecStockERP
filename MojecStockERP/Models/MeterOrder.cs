using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MojecStockERP.Models
{
    public class MeterOrder
    {
        public int MeterID { get; set; }
        public string MeterType { get; set; }
        public string Model { get; set; }
        public string Partners { get; set; }
        public string Quantity { get; set; }
        public string BillofLaden { get; set; }
        [Display(Name = "Shipment Date")]
        public string Date { get; set; }

        [Display(Name = "Arrival Date")]
        public string Arrivaldate { get; set; }
        public string CommunicationTye { get; set; }
    }
}