using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MojecStockERP.Controllers
{
    public class HomeController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["StockManagementERP"].ConnectionString);
        public ActionResult Home()
        {
            return View();
        }
        public ActionResult Index()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }

            con.Open();
            SqlCommand cmd = new SqlCommand("select Count(*) from StockManagement_Tbl", con);
            int r = Convert.ToInt32(cmd.ExecuteScalar());
            ViewBag.TotalMeter = r;

            SqlCommand cmd2 = new SqlCommand("select Count(*) from DispatchedMeters_Tbl", con);
            int r2 = Convert.ToInt32(cmd2.ExecuteScalar());
            ViewBag.TotalDispatched = r2;

            SqlCommand cmd3 = new SqlCommand("select Count(*) from Installation_Tbl", con);
            int r3 = Convert.ToInt32(cmd3.ExecuteScalar());
            ViewBag.Installation = r3;

            ViewBag.AvailableMeters = r - r2;
            return View();
        }



        public ActionResult Admin()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }
            con.Open();
            SqlCommand cmd = new SqlCommand("select Count(*) from StockManagement_Tbl", con);
            int r = Convert.ToInt32(cmd.ExecuteScalar());
            ViewBag.TotalMeter = r;

            SqlCommand cmd2 = new SqlCommand("select Count(*) from DispatchedMeters_Tbl", con);
            int r2 = Convert.ToInt32(cmd2.ExecuteScalar());
            ViewBag.TotalDispatched = r2;

            SqlCommand cmd3 = new SqlCommand("select Count(*) from Installation_Tbl", con);
            int r3 = Convert.ToInt32(cmd3.ExecuteScalar());
            ViewBag.Installation = r3;

            ViewBag.AvailableMeters = r - r2;
            return View();
        }

        public ActionResult Board()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }
            con.Open();
            SqlCommand cmd = new SqlCommand("select Count(*) from StockManagement_Tbl", con);
            int r = Convert.ToInt32(cmd.ExecuteScalar());
            ViewBag.TotalMeter = r;

            SqlCommand cmd2 = new SqlCommand("select Count(*) from DispatchedMeters_Tbl", con);
            int r2 = Convert.ToInt32(cmd2.ExecuteScalar());
            ViewBag.TotalDispatched = r2;

            SqlCommand cmd3 = new SqlCommand("select Count(*) from Installation_Tbl", con);
            int r3 = Convert.ToInt32(cmd3.ExecuteScalar());
            ViewBag.Installation = r3;

            ViewBag.AvailableMeters = r - r2;
            return View();
        }
    }
}