using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MojecStockERP.Models
{
    public class UserLogin
    {
        public int UserID { get; set; }
        public string Fullname { get; set; }
        public string Username { get; set;}
        public string Password { get; set;}
        public string Confirmpassword { get; set; }
        public string Role { get; set;}
        public string Disco { get; set; }

    }
}