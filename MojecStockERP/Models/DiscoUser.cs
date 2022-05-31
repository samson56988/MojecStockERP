using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MojecStockERP.Models
{
    public class DiscoUser
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Confirmpassword { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public string Disco { get; set; }
    }
}