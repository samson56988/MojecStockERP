using MojecStockERP.Config;
using MojecStockERP.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MojecStockERP.Controllers
{
    public class ProcurementController : Controller
    {
        List<MeterOrder> _orders = new List<MeterOrder>();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["StockManagementERP"].ConnectionString);
        OleDbConnection Econ;
        List<MeterOrder> _produced = new List<MeterOrder>();
        List<PlasticOrder> _plastic = new List<PlasticOrder>();

        private void ExcelConn(string FilePath)
        {
            string constr = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;""", FilePath);
            Econ = new OleDbConnection(constr);
        }
        // GET: Procurement
        public ActionResult Dashboard()
        {
            //string Username = (string)Session["Username"];

            //if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            //{
            //    return RedirectToAction("Login", "Authentication");
            //}

            con.Open();
            //SqlCommand cmd = new SqlCommand("select Sum(Quantity) from MeterProcurement_Tbl", con);
            //int r = Convert.ToInt32(cmd.ExecuteScalar());
            //ViewBag.TotalOrders = r;

            return View();
        }
       public ActionResult UploadProcurement()
       {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }
            return View();
       }
        [HttpPost]
        public ActionResult UploadProcurement(HttpPostedFileBase file)
        {
            try
            {
                string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string filepath = "/excelfolder/" + filename;
                file.SaveAs(Path.Combine(Server.MapPath("/excelfolder/"), filename));
                string fullpath = Server.MapPath("/excelfolder/") + filename;
                ExcelConn(fullpath);
                string Query = string.Format("Select * from [{0}]", "Sheet1$");
                OleDbCommand Ecom = new OleDbCommand(Query, Econ);
                Econ.Open();
                OleDbDataReader dr = Ecom.ExecuteReader();
                DataSet ds = new DataSet();
                OleDbDataAdapter oda = new OleDbDataAdapter(Query, Econ);
                Econ.Close();
                oda.Fill(ds);
                DataTable dt = ds.Tables[0];
                SqlBulkCopy objbulk = new SqlBulkCopy(con);
                objbulk.DestinationTableName = "MeterProcurement_Tbl";
                objbulk.ColumnMappings.Add("MeterType ", "MeterType");
                objbulk.ColumnMappings.Add("Model", "Model");
                objbulk.ColumnMappings.Add("Partners", "Partners");
                objbulk.ColumnMappings.Add("Quantity", "Quantity");
                objbulk.ColumnMappings.Add("BillofLaden", "BillofLaden");
                objbulk.ColumnMappings.Add("Date", "Date");
                con.Open();
                objbulk.WriteToServer(dt);
                con.Close();
                TempData["save"] = "Upload successful";
                return View();
            }
            catch (Exception ex)
            {
                TempData["delete"] = "Upload Failed";
            }

            return View();
        }
        public ActionResult MeterProcurement()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }
            _orders = new List<MeterOrder>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("select * from MeterProcurement_Tbl", con);
                cmd.CommandType = System.Data.CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    MeterOrder produced = new MeterOrder();
                    produced.MeterID = Convert.ToInt32(rdr["MeterProcurementID"].ToString());
                    produced.MeterType = rdr["MeterType"].ToString();
                    produced.Model = rdr["Model"].ToString();
                    produced.Partners = rdr["Partners"].ToString();
                    produced.Quantity = rdr["Quantity"].ToString();
                    produced.BillofLaden = rdr["BillofLaden"].ToString();
                    produced.Date = rdr["Date"].ToString();
                    produced.Arrivaldate = rdr["DateArrived"].ToString();
                    _orders.Add(produced);
                }
                rdr.Close();
            }
            return View(_orders);
        }
        public ActionResult UploadPlastic()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadPlastic(HttpPostedFileBase file)
        {
            try
            {
                string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string filepath = "/excelfolder/" + filename;
                file.SaveAs(Path.Combine(Server.MapPath("/excelfolder/"), filename));
                string fullpath = Server.MapPath("/excelfolder/") + filename;
                ExcelConn(fullpath);
                string Query = string.Format("Select * from [{0}]", "Sheet1$");
                OleDbCommand Ecom = new OleDbCommand(Query, Econ);
                Econ.Open();
                OleDbDataReader dr = Ecom.ExecuteReader();
                DataSet ds = new DataSet();
                OleDbDataAdapter oda = new OleDbDataAdapter(Query, Econ);
                Econ.Close();
                oda.Fill(ds);
                DataTable dt = ds.Tables[0];
                SqlBulkCopy objbulk = new SqlBulkCopy(con);
                objbulk.DestinationTableName = "PlasticProcurement_Tbl";
                objbulk.ColumnMappings.Add("Chemicals ", "Chemical");
                objbulk.ColumnMappings.Add("Quantity ", "Quantity");
                objbulk.ColumnMappings.Add("Date", "Date");
                con.Open();
                objbulk.WriteToServer(dt);
                con.Close();
                TempData["save"] = "Upload successful";
                return View();
            }
            catch (Exception ex)
            {
                TempData["delete"] = "Upload Failed";
            }

            return View();
        }
        public ActionResult PlasticOrder()
        {
            _plastic = new List<PlasticOrder>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("select * from PlasticProcurement_Tbl", con);
                cmd.CommandType = System.Data.CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    PlasticOrder produced = new PlasticOrder();
                    produced.Chemicals= rdr["Chemical"].ToString();
                    produced.Quantity = rdr["Quantity"].ToString();
                    produced.Date = rdr["Date"].ToString();
                    produced.ArrivalDate = rdr["DateArrival"].ToString();

                    _plastic.Add(produced);
                }
                rdr.Close();
            }
            return View(_plastic);
        }
        public ActionResult AddMeterProcurement()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddMeterProcurement(MeterOrder meters)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
                {
                    using (SqlCommand cmd = new SqlCommand("AddMeterProcurement", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MeterType", meters.MeterType);
                        cmd.Parameters.AddWithValue("@Model", meters.Model);
                        cmd.Parameters.AddWithValue("@Partners", meters.Partners);
                        cmd.Parameters.AddWithValue("@Quantity", meters.Quantity);
                        cmd.Parameters.AddWithValue("@BillofLaden", meters.BillofLaden);
                        cmd.Parameters.AddWithValue("@Date", meters.Date);
                        cmd.Parameters.AddWithValue("@DateArrived", meters.Arrivaldate);
                        cmd.Parameters.AddWithValue("@CommunicationType", meters.CommunicationTye);
                        if (con.State != System.Data.ConnectionState.Open)

                            con.Open();
                        cmd.ExecuteNonQuery();
                    }

                    con.Close();
                }
                TempData["save"] = "Details Added Successfully";
                return RedirectToAction("MeterProcurement");
            }
            catch (Exception ex)
            {
                TempData["delete"] = "Update Failed";
                return View();
            }
        }
        public ActionResult AddPlasticProcurement()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddPlasticProcurement(PlasticOrder order)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
                {
                    using (SqlCommand cmd = new SqlCommand("AddPlasticProcurement", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Chemical", order.Chemicals);
                        cmd.Parameters.AddWithValue("@Quanity", order.Quantity);
                        cmd.Parameters.AddWithValue("@Date", order.Date);
                        cmd.Parameters.AddWithValue("@DateArrival", order.ArrivalDate);
                        if (con.State != System.Data.ConnectionState.Open)

                            con.Open();
                        cmd.ExecuteNonQuery();
                    }

                    con.Close();
                }
                TempData["save"] = "Details Added Successfully";
                return RedirectToAction("PlasticOrder");
            }
            catch (Exception ex)
            {
                TempData["delete"] = "Update Failed";
                return View();
            }
        }





    }
}