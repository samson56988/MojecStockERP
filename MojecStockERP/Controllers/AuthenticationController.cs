using MojecStockERP.Config;
using MojecStockERP.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MojecStockERP.Controllers
{
    public class AuthenticationController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["StockManagementERP"].ConnectionString);
        SqlCommand com = new SqlCommand();

        void connectionString()
        {
            con.ConnectionString = "Data Source=mojecserver.database.windows.net;Initial Catalog=StockManagementSystemMojec;User ID=mojec;Password=Admin123";
        }
        // GET: Authentication
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(UserLogin login)
        {
            login.Role = "";
            SqlDataReader dr;
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "Select * from UserLogin_Tbl where UserName = '" + login.Username + "'and Password = '" + login.Password + "' and Active ='Active'";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                if (dr.Read())
                {
                    login.Username = dr["Username"].ToString();
                    login.Role = dr["Role"].ToString();
                    login.Disco = dr["Disco"].ToString();
                    
                }

                if(login.Role == "Admin")
                {
                    FormsAuthentication.SetAuthCookie(login.Username, true);
                    Session["Username"] = login.Username.ToString();
                    return RedirectToAction("Admin", "Admin");
                }
                if(login.Role == "Disco")
                {
                    FormsAuthentication.SetAuthCookie(login.Username, true);
                    Session["Username"] = login.Username.ToString();
                    FormsAuthentication.SetAuthCookie(login.Disco, true);
                    Session["Disco"] = login.Disco.ToString();
                    return RedirectToAction("Dashboard", "Installation");
                }
                if (login.Role == "Store")
                {
                    FormsAuthentication.SetAuthCookie(login.Username, true);
                    Session["Username"] = login.Username.ToString();
                    return RedirectToAction("StoreDashboard", "Store");
                }
                if (login.Role == "Factory")
                {
                    FormsAuthentication.SetAuthCookie(login.Username, true);
                    Session["Username"] = login.Username.ToString();
                    return RedirectToAction("FactoryDashboard", "Factory");
                }
                if (login.Role == "Board")
                {
                    FormsAuthentication.SetAuthCookie(login.Username, true);
                    Session["Username"] = login.Username.ToString();
                    return RedirectToAction("Dashboard", "Board");
                }
            }
            else
            {
                ViewBag.Error = "Invalid Login Details";
            }
     
            return View();
        }
    }
}