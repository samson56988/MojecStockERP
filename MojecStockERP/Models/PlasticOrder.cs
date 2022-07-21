using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MojecStockERP.Models
{
    public class PlasticOrder
    {
        public int orderId { get; set; }
        public string Chemicals { get; set; }
        public string Quantity { get; set; }
        [Display(Name = "Shipment Date")]
        public string Date { get; set; }
        [Display(Name = "Arrival Date")]
        public string ArrivalDate { get; set; }

        
    }
}