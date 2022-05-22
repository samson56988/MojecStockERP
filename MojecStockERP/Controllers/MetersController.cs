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
    public class MetersController : Controller
    {
        List<Installation> _installation = new List<Installation>();
        List<MetersDispatched> _dispatchedMeters = new List<MetersDispatched>();
        List<MetersProduced> _producedMeters = new List<MetersProduced>();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["StockManagementERP"].ConnectionString);
        // GET: Meters

        OleDbConnection Econ;
        private void ExcelConn(string FilePath)
        {
            string constr = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;""", FilePath);
            Econ = new OleDbConnection(constr);
        }

        public ActionResult MeterProductionUpload()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }
            return View();
        }

        [HttpPost]
        public ActionResult MeterProductionUpload(HttpPostedFileBase file)
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
            objbulk.DestinationTableName = "StockManagement_Tbl";
            objbulk.ColumnMappings.Add("MeterNo", "MeterNo");
            objbulk.ColumnMappings.Add("MeterType", "MeterType");
            objbulk.ColumnMappings.Add("Model", "Model");
            objbulk.ColumnMappings.Add("Version", "Version");
            objbulk.ColumnMappings.Add("date", "DateofSupply");
            objbulk.ColumnMappings.Add("Partners", "Partners");
            con.Open();
            objbulk.WriteToServer(dt);
            con.Close();
            TempData["save"] = "Upload successful";
            return View();
        }

        public ActionResult MeterDispatchedUpload()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }
            return View();
        }

        [HttpPost]
        public ActionResult MeterDispatchedUpload(HttpPostedFileBase file)
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
            objbulk.ColumnMappings.Add("Version", "Version");
            objbulk.ColumnMappings.Add("date", "DateofDispatched");
            objbulk.ColumnMappings.Add("Partners", "Partners");
            con.Open();
            objbulk.WriteToServer(dt);
            con.Close();
            TempData["save"] = "Upload successful";
            return View();
        }

        public ActionResult Meter()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }
            _producedMeters = new List<MetersProduced>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("select * from StockManagement_Tbl", con);
                cmd.CommandType = System.Data.CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    MetersProduced produced = new MetersProduced();
                    produced.MeterNo = rdr["MeterNO"].ToString();
                    produced.MeterType = rdr["MeterType"].ToString();
                    produced.Model = rdr["Model"].ToString();
                    produced.Version = rdr["Version"].ToString();
                    produced.DateOfSupply = rdr["DateofSupply"].ToString();
                    produced.Partners = rdr["Partners"].ToString();
                    _producedMeters.Add(produced);
                }
                rdr.Close();
            }
            return View(_producedMeters);
        }

        public ActionResult MeterDispatched()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }
            _dispatchedMeters = new List<MetersDispatched>();
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
                    dispatched.Version = rdr["Version"].ToString();
                    dispatched.DateOfDispatch = rdr["DateofDispatched"].ToString();
                    dispatched.Partners = rdr["Partners"].ToString();
                    _dispatchedMeters.Add(dispatched);
                }
                rdr.Close();
            }
            return View(_dispatchedMeters);
        }

        
    }
}