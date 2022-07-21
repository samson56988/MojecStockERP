using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MojecStockERP.Models
{
    public class UserLogin
    {
        public int UserID { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        public string Fullname { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        public string Username { get; set;}
        [Required(ErrorMessage = "Please enter Details")]
        public string Password { get; set;}
        [Required(ErrorMessage = "Please enter Details")]
        public string Confirmpassword { get; set; }
        [Required(ErrorMessage = "Please enter Details")]
        public string Role { get; set;}
        [Required(ErrorMessage = "Please enter Details")]
        public string Disco { get; set; }

    }
}