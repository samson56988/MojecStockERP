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
    public class StoreController : Controller
    {
        List<MetersDispatched> _dispatched = new List<MetersDispatched>();
        List<StoredMeters> _store = new List<StoredMeters>();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["StockManagementERP"].ConnectionString);
        OleDbConnection Econ;
        private void ExcelConn(string FilePath)
        {
            string constr = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;""", FilePath);
            Econ = new OleDbConnection(constr);
        }
        // GET: Store
        public ActionResult StoreDashboard()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }

            string Disco = (string)Session["Disco"];

            con.Open();
            SqlCommand cmd = new SqlCommand("select Count(*) from StoreRecieved_Tbl", con);
            int r = Convert.ToInt32(cmd.ExecuteScalar());
            ViewBag.TotalRecieved = r;

            SqlCommand cmd2 = new SqlCommand("select Count(*) from DispatchedMeters_Tbl", con);
            int r2 = Convert.ToInt32(cmd2.ExecuteScalar());
            ViewBag.TotalDispatched = r2;

            ViewBag.AvailableMeters = r - r2;
            return View();
        }
        public ActionResult UploadMetersRecieved()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }
            return View();
        }
        [HttpPost]
        public ActionResult UploadMetersRecieved(HttpPostedFileBase file)
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
                objbulk.DestinationTableName = "StoreRecieved_Tbl";
                objbulk.ColumnMappings.Add("MeterNo", "MeterNo");
                objbulk.ColumnMappings.Add("MeterType", "MeterType");
                objbulk.ColumnMappings.Add("Model", "Model");
                objbulk.ColumnMappings.Add("HardwareVersion", "Hardwareversion");
                objbulk.ColumnMappings.Add("SoftwareVersion", "Softwareversion");
                objbulk.ColumnMappings.Add("Date", "Date");
                objbulk.ColumnMappings.Add("Partners", "Partners");
                objbulk.ColumnMappings.Add("SGC", "SGC");
                objbulk.ColumnMappings.Add("TarrifIndex", "TarrifIndex");
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
        public ActionResult MetersRecieved()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }
            _store = new List<StoredMeters>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetAllMetersinStore", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    StoredMeters store = new StoredMeters();
                    store.MeterNo = rdr["MeterNo"].ToString();
                    store.MeterType = rdr["MeterType"].ToString();
                    store.Model = rdr["Model"].ToString();
                    store.SGC = rdr["SGC"].ToString();
                    store.Partners = rdr["Partners"].ToString();
                    store.Date = rdr["Date"].ToString();
                    _store.Add(store);
                }
                rdr.Close();
            }
            return View(_store);
        }
        public ActionResult UploadDispatchedMeters()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }
            return View();
        }
        [HttpPost]
        public ActionResult UploadDispatchedMeters(HttpPostedFileBase file)
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
                objbulk.DestinationTableName = "DispatchedMeters_Tbl";
                objbulk.ColumnMappings.Add("MeterNo", "MeterNO");
                objbulk.ColumnMappings.Add("MeterType", "MeterType");
                objbulk.ColumnMappings.Add("Model", "Model");
                objbulk.ColumnMappings.Add("SoftwareVersion", "SoftwareVersion");
                objbulk.ColumnMappings.Add("HardwareVersion", "HardwareVersion");
                objbulk.ColumnMappings.Add("Date", "DateofDispatched");
                objbulk.ColumnMappings.Add("Partners", "Partners");
                objbulk.ColumnMappings.Add("Disco", "Disco");
                objbulk.ColumnMappings.Add("SGC", "SGC");
                objbulk.ColumnMappings.Add("TarrifIndex", "TarrifIndex");
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
        public ActionResult DispatchedMeters()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }
            _dispatched = new List<MetersDispatched>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("select * from DispatchedMeters_Tbl", con);
                cmd.CommandType = System.Data.CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    MetersDispatched dispatched = new MetersDispatched();
                    dispatched.MeterNo = rdr["MeterNO"].ToString();
                    dispatched.MeterType = rdr["MeterType"].ToString();
                    dispatched.Model = rdr["Model"].ToString();
                    dispatched.DateOfDispatch = rdr["DateofDispatched"].ToString();
                    dispatched.Partners = rdr["Partners"].ToString();
                    dispatched.Disco = rdr["Disco"].ToString();
                    _dispatched.Add(dispatched);
                }
                rdr.Close();
            }
            return View(_dispatched);
        }
        public FileResult DownloadFile(int? fileId)
        {
            fileId = 1;
            Files model = PopulateDispatchFiles().Find(x => x.Id == Convert.ToInt32(fileId));
            string fileName = model.Name;
            string contentType = model.ContentType;
            byte[] bytes = model.Data;
            return File(bytes, contentType, fileName);
        }
        private static List<Files> PopulateDispatchFiles()
        {
            List<Files> files = new List<Files>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {

                using (SqlCommand cmd = new SqlCommand("Select * from DispatchTemplate", con))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            files.Add(new Files
                            {
                                Id = Convert.ToInt32(sdr["Id"]),
                                Name = sdr["Name"].ToString(),
                                ContentType = sdr["ContentType"].ToString(),
                                Data = (byte[])sdr["Data"]
                            });
                        }
                    }
                    con.Close();
                }
            }

            return files;
        }
        public FileResult DownloadFilerecieved(int? fileId)
        {
            fileId = 1;
            Files model = PopulaterecievedFiles().Find(x => x.Id == Convert.ToInt32(fileId));
            string fileName = model.Name;
            string contentType = model.ContentType;
            byte[] bytes = model.Data;
            return File(bytes, contentType, fileName);
        }
        private static List<Files> PopulaterecievedFiles()
        {
            List<Files> files = new List<Files>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {

                using (SqlCommand cmd = new SqlCommand("Select * from RecievedGoodsTemplate", con))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            files.Add(new Files
                            {
                                Id = Convert.ToInt32(sdr["Id"]),
                                Name = sdr["Name"].ToString(),
                                ContentType = sdr["ContentType"].ToString(),
                                Data = (byte[])sdr["Data"]
                            });
                        }
                    }
                    con.Close();
                }
            }

            return files;
        }
    }
}