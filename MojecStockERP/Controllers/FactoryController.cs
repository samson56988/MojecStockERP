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
    public class FactoryController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["StockManagementERP"].ConnectionString);
        OleDbConnection Econ;
        List<MetersProduced> _produced = new List<MetersProduced>();
        private void ExcelConn(string FilePath)
        {
            string constr = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;""", FilePath);
            Econ = new OleDbConnection(constr);
        }

        public ActionResult FactoryDashboard()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }

            string Disco = (string)Session["Disco"];

            con.Open();
            SqlCommand cmd = new SqlCommand("select Count(*) from Production_Tbl", con);
            int r = Convert.ToInt32(cmd.ExecuteScalar());
            ViewBag.Produced = r;
            return View();
        }
        // GET: Factory
        public ActionResult UploadMetersProduced()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }
            return View();
        }

        [HttpPost]
        public ActionResult UploadMetersProduced(HttpPostedFileBase file)
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
                objbulk.DestinationTableName = "Production_Tbl";
                objbulk.ColumnMappings.Add("MeterNo", "MeterNo");
                objbulk.ColumnMappings.Add("MeterType", "MeterType");
                objbulk.ColumnMappings.Add("Model", "Model");
                objbulk.ColumnMappings.Add("SoftwareVersion", "SoftwareVersion");
                objbulk.ColumnMappings.Add("HardwareVersion", "HardwareVersion");
                objbulk.ColumnMappings.Add("Date", "DateOfProduction");
                objbulk.ColumnMappings.Add("Partners", "Partners");
                objbulk.ColumnMappings.Add("SGC", "SGC");
                objbulk.ColumnMappings.Add("TarrifIndex", "TarrifIndex");
                objbulk.ColumnMappings.Add("Disco", "Disco");
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

        public ActionResult MetersProduced()
        {
            string Username = (string)Session["Username"];

            //if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            //{
            //    return RedirectToAction("Login", "Authentication");
            //}
            _produced = new List<MetersProduced>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("select * from Production_Tbl", con);
                cmd.CommandType = System.Data.CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    MetersProduced produced = new MetersProduced();
                    produced.MeterNo = rdr["MeterNO"].ToString();
                    produced.MeterType = rdr["MeterType"].ToString();
                    produced.Model = rdr["Model"].ToString();
                    produced.SGC = rdr["SGC"].ToString();
                    produced.DateOfSupply = rdr["DateofProduction"].ToString();
                    produced.Partners = rdr["Partners"].ToString();
                    produced.Disco = rdr["Disco"].ToString();
                    _produced.Add(produced);
                }
                rdr.Close();
            }
            return View(_produced);
        }
    }
}

