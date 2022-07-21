using MojecStockERP.Config;
using MojecStockERP.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MojecStockERP.Controllers
{
    public class PlasticController : Controller
    {
        List<PlasticProduction> _production = new List<PlasticProduction>();
        // GET: Plastic4
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["StockManagementERP"].ConnectionString);
        public ActionResult AddPlasticProduction()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddPlasticProduction(PlasticProduction plastic)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
                {
                    using (SqlCommand cmd = new SqlCommand("AddPlasticProduction", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ChemicalType", plastic.Chemical);
                        cmd.Parameters.AddWithValue("@Quantity", plastic.Quantity);
                        cmd.Parameters.AddWithValue("@Date", plastic.Date);
                        cmd.Parameters.AddWithValue("@MeterType", plastic.MeterType);
                        if (con.State != System.Data.ConnectionState.Open)

                            con.Open();
                        cmd.ExecuteNonQuery();
                    }

                    con.Close();
                }
                TempData["save"] = "Details Added Successfully";
                return RedirectToAction("PlasticProduction");
            }
            catch (Exception ex)
            {
                TempData["delete"] = "Update Failed";
                return View();
            }
        }
        public ActionResult PlasticProduction()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }
            _production = new List<PlasticProduction>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("select * from PlasticProduction_Tbl", con);
                cmd.CommandType = System.Data.CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    PlasticProduction plastic = new PlasticProduction();
                    plastic.Chemical = rdr["ChemicalType"].ToString();
                    plastic.Quantity = rdr["Quantity"].ToString();
                    plastic.MeterType = rdr["MeterType"].ToString();
                    plastic.Date = rdr["Date"].ToString();
                    _production.Add(plastic);
                }
                rdr.Close();
            }
            return View(_production);
        }
        public ActionResult AddPlasticSupplied()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddPlasticSupplied(PlasticProduction plastic)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
                {
                    using (SqlCommand cmd = new SqlCommand("InsertPlasticSupplied", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Quantity", plastic.Quantity);
                        cmd.Parameters.AddWithValue("@Date", plastic.Date);
                        cmd.Parameters.AddWithValue("@MeterType", plastic.MeterType);
                        cmd.Parameters.AddWithValue("@Disco", plastic.Disco);
                        if (con.State != System.Data.ConnectionState.Open)

                            con.Open();
                        cmd.ExecuteNonQuery();
                    }
                   con.Close();
                }
                TempData["save"] = "Details Added Successfully";
                return RedirectToAction("PlasticSupplied");
            }
            catch (Exception ex)
            {
                TempData["delete"] = "Update Failed";
                return View();
            }
        }
        public ActionResult PlasticSupplied()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }
            _production = new List<PlasticProduction>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("select * from PlasticSupplied_Tbl", con);
                cmd.CommandType = System.Data.CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    PlasticProduction plastic = new PlasticProduction();
                    plastic.Chemical = rdr["Chemical"].ToString();
                    plastic.Quantity = rdr["Quantity"].ToString();
                    plastic.MeterType = rdr["MeterType"].ToString();
                    plastic.Date = rdr["Date"].ToString();
                    plastic.Disco = rdr["Disco"].ToString();
                    _production.Add(plastic);
                }
                rdr.Close();
            }
            return View(_production);
        }
        

       

    }
}