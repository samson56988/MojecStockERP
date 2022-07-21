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
    public class InstallationController : Controller
    {

        List<Installation> _installation = new List<Installation>();
        List<KYC> _kyc = new List<KYC>();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["StockManagementERP"].ConnectionString);
        OleDbConnection Econ;
        public ActionResult KYC()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }
            string Disco = (string)Session["Disco"];
            _kyc = new List<KYC>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("Sp_GetKYCUploadPerDisco", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Disco",Disco);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    KYC kyc = new KYC();
                    kyc.DateShared = rdr["DateShared"].ToString();
                    kyc.CustomerName = rdr["CustomerName"].ToString();
                    kyc.ARN = rdr["ARN"].ToString();
                    kyc.Disco = rdr["Disco"].ToString();
                    kyc.BU = rdr["BU"].ToString();
                    kyc.AdditionalComment = rdr["AdditionalComment"].ToString();
             
                    _kyc.Add(kyc);
                }
                rdr.Close();
            }
            return View(_kyc);
        }
        public ActionResult UploadKYC()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }
            return View();
        }
        [HttpPost]
        public ActionResult UploadKYC(HttpPostedFileBase file)
        {
            try 
            {
                string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string filepath = "/excelfolder/" + filename;
                file.SaveAs(Path.Combine(Server.MapPath("/excelfolder/"), filename));
                string fullpath = Server.MapPath("/excelfolder/") + filename;
                ExcelConn(fullpath);
                string Dateshared = "";
                string ACNo1 = "";
                string ACNo2 = "";
                string SBCsMAIN = "";
                string ARN = "";
                string CustomerName = "";
                string Email = "";
                string PhoneNumber = "";
                string Address = "";
                string BU = "";
                string Tariff = "";
                string MeteredStatus = "";
                string TypeofApartment = "";
                string RecommendedMeterType = "";
                string AdditionalComment = "";
                string Disco = "";
                string Query = string.Format("Select * from [{0}]", "Sheet1$");
                OleDbCommand Ecom = new OleDbCommand(Query, Econ);
                Econ.Open();
                OleDbDataReader dr = Ecom.ExecuteReader();
                while (dr.Read())
                {

                    Dateshared = dr[0].ToString();
                    ACNo1 = dr[1].ToString();
                    ACNo2 = dr[2].ToString();
                    SBCsMAIN = dr[3].ToString();
                    ARN = dr[4].ToString();
                    CustomerName = dr[5].ToString();
                    Email = dr[6].ToString();
                    PhoneNumber = dr[7].ToString();
                    Address = dr[8].ToString();
                    BU = dr[9].ToString();
                    Tariff = dr[10].ToString();
                    MeteredStatus = dr[11].ToString();
                    TypeofApartment = dr[12].ToString();
                    RecommendedMeterType = dr[13].ToString();
                    AdditionalComment = dr[14].ToString();
                    Disco = dr[15].ToString();
                    SqlConnection con = new SqlConnection("Data Source=mojecserver.database.windows.net;Initial Catalog=StockManagementSystemMojec;User ID=mojec;Password=Admin123;");
                    con.Open();
                    string Disconame = (string)Session["Disco"];
                    if (Disco != Disconame)
                    {
                        TempData["delete"] = "Sorry Input your correct Disco name into the Template";
                        return View();
                    }
                    //MySqlCommand cmddelete = new MySqlCommand("truncate table duplicate",con);
                    //cmddelete.ExecuteNonQuery();
                    SqlCommand cmd = new SqlCommand("Select * from KYCUpload_Tbl where ARN = @arn", con);
                    cmd.Parameters.AddWithValue("@arn", ARN);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count >= 1)
                    {
                        SqlCommand icmmd = new SqlCommand("update KYCUpload_Tbl set AdditionalComment = @Additioncomment where ARN = @arn", con);
                        icmmd.Parameters.AddWithValue("@arn", ARN);
                        icmmd.Parameters.AddWithValue("@Additioncomment", AdditionalComment);
                        ViewBag.Result = "Data Imported Successfully";
                        icmmd.ExecuteNonQuery();
                    }
                    else
                    {

                        SqlCommand icmmd = new SqlCommand("INSERT INTO KYCUpload_Tbl(DateShared,ACNo1,ACNo2,SBCsMAIN,ARN,CustomerName,Email,PhoneNumber,Address,BU,Tariff,MeterStatus,TypeofApartment,MeterRecommended,AdditionalComment,Disco)VALUES(@DateShared,@ACNo1,@ACNo2,@SBCsMAIN,@ARN,@CustomerName,@Email,@PhoneNumber,@Address,@BU,@Tariff,@MeteredStatus,@TypeofApartment,@RecommendedMeterType,@AdditionalComment,@Disco)", con);
                        icmmd.Parameters.AddWithValue("@DateShared", Dateshared);
                        icmmd.Parameters.AddWithValue("@email", Email);
                        icmmd.Parameters.AddWithValue("@ACNo1", ACNo1);
                        icmmd.Parameters.AddWithValue("@ACNo2", ACNo2);
                        icmmd.Parameters.AddWithValue("@SBCsMAIN", SBCsMAIN);
                        icmmd.Parameters.AddWithValue("@ARN", ARN);
                        icmmd.Parameters.AddWithValue("@CustomerName", CustomerName);
                        icmmd.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);
                        icmmd.Parameters.AddWithValue("@Address", Address);
                        icmmd.Parameters.AddWithValue("@BU", BU);
                        icmmd.Parameters.AddWithValue("@Tariff", Tariff);
                        icmmd.Parameters.AddWithValue("@MeteredStatus", MeteredStatus);
                        icmmd.Parameters.AddWithValue("@TypeofApartment", TypeofApartment);
                        icmmd.Parameters.AddWithValue("@RecommendedMeterType", RecommendedMeterType);
                        icmmd.Parameters.AddWithValue("@AdditionalComment", AdditionalComment);
                        icmmd.Parameters.AddWithValue("@Disco", Disco);
                        icmmd.ExecuteNonQuery();
                        ViewBag.Result = "Data Imported Successfully";
                    }
                    con.Close();

                }
                TempData["save"] = "Upload Successful";
                return View();
            }
            catch(Exception ex)
            {
                TempData["delete"] = "Upload Failed";
            }
            return View();
        }
        public ActionResult Dashboard()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }

            string Disco = (string)Session["Disco"];

            con.Open();
            SqlCommand cmd = new SqlCommand("select Count(*) from KYCUpload_Tbl where AdditionalComment = 'Ready for metering' and Disco = '"+Disco+"'", con);
            int r = Convert.ToInt32(cmd.ExecuteScalar());
            ViewBag.CompletedKYC = r;

            SqlCommand cmd2 = new SqlCommand("select Count(*) from KYCUpload_Tbl where AdditionalComment Not in ('Ready for metering') and Disco = '" + Disco + "'", con);
            int r2 = Convert.ToInt32(cmd2.ExecuteScalar());
            ViewBag.KYCProgress = r2;

            SqlCommand cmd3 = new SqlCommand("select Count(*) from Installation_Tbl where Disco = '" + Disco + "'", con);
            int r3 = Convert.ToInt32(cmd3.ExecuteScalar());
            ViewBag.Installation = r3;

            ViewBag.AvailableMeters = r - r2;
            return View();
            
        }
        private void ExcelConn(string FilePath)
        {
            string constr = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;""", FilePath);
            Econ = new OleDbConnection(constr);
        }
        // GET: Installation
        public ActionResult InstallationUpload()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }
            return View();
        }
        [HttpPost]
        public ActionResult InstallationUpload(HttpPostedFileBase file)
        {
            try
            {
                string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string filepath = "/excelfolder/" + filename;
                file.SaveAs(Path.Combine(Server.MapPath("/excelfolder/"), filename));
                string Discosession = (string)Session["Disco"];
            string fullpath = Server.MapPath("/excelfolder/") + filename;
            ExcelConn(fullpath);
            string MeterNo = "";
            string MeterType = "";
            string DateInstalled = "";
            string Disco = "";
            string BU = "";
            string Query = string.Format("Select * from [{0}]", "Sheet1$");
            OleDbCommand Ecom = new OleDbCommand(Query, Econ);
            Econ.Open();
            OleDbDataReader dr = Ecom.ExecuteReader();
            while (dr.Read())
            {

                MeterNo = dr[0].ToString();
                MeterType = dr[1].ToString();
                DateInstalled = dr[2].ToString();
                Disco = dr[3].ToString();
                BU = dr[4].ToString();
                SqlConnection con = new SqlConnection("Data Source=mojecserver.database.windows.net;Initial Catalog=StockManagementSystemMojec;User ID=mojec;Password=Admin123;");
                con.Open();
                //MySqlCommand cmddelete = new MySqlCommand("truncate table duplicate",con);
                //cmddelete.ExecuteNonQuery();
                if(Disco != Discosession )
                {
                   TempData["delete"] = "Sorry Input your correct Disco name into the Template";
                        return View();
                }
                    SqlCommand icmmd = new SqlCommand("INSERT INTO Installation_Tbl(MeterNo,MeterType,Disco,DateInstalled,BU)VALUES(@MeterNo,@MeterType,@Disco,@DateInstalled,@BU)", con);
                    icmmd.Parameters.AddWithValue("@MeterNo", MeterNo);
                    icmmd.Parameters.AddWithValue("@MeterType",MeterType );
                    icmmd.Parameters.AddWithValue("@Disco", Disco);
                    icmmd.Parameters.AddWithValue("@DateInstalled", DateInstalled);
                    icmmd.Parameters.AddWithValue("@BU", BU);
                    icmmd.ExecuteNonQuery();
                    ViewBag.Result = "Data Imported Successfully";
                }
                con.Close();
                TempData["save"] = "Upload Successful";
                return View();
            }
            catch (Exception ex)
            {
                TempData["delete"] = "Upload Failed";
            }
            return View();
        }
        private void InsertInstallationdata(string fileepath, string filename)
        {
           

            

        }
        public ActionResult Installation()
        {
            string Disco = (string)Session["Disco"];
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }
            _installation = new List<Installation>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("Sp_GetInstallationPerDisco", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Disco",Disco);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Installation install = new Installation();
                    install.MeterNo = rdr["MeterNO"].ToString();
                    install.MeterType = rdr["MeterType"].ToString();
                    install.Disco = rdr["Disco"].ToString();
                    install.DateInstalled = rdr["DateInstalled"].ToString();
                    install.BU = rdr["BU"].ToString();
                    _installation.Add(install);
                }
                rdr.Close();
            }
            return View(_installation);
        }
        public FileResult DownloadFile(int? fileId)
        {
            fileId = 1;
            Files model = PopulateInstallationFiles().Find(x => x.Id == Convert.ToInt32(fileId));
            string fileName = model.Name;
            string contentType = model.ContentType;
            byte[] bytes = model.Data;
            return File(bytes, contentType, fileName);
        }
        private static List<Files> PopulateInstallationFiles()
        {
            List<Files> files = new List<Files>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                
                using (SqlCommand cmd = new SqlCommand("Select * from InstallationTemplate", con))
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
        public FileResult DownloadFilekyc(int? fileId)
        {
            fileId = 1;
            Files model = PopulateKYCFiles().Find(x => x.Id == Convert.ToInt32(fileId));
            string fileName = model.Name;
            string contentType = model.ContentType;
            byte[] bytes = model.Data;
            return File(bytes, contentType, fileName);
        }
        private static List<Files> PopulateKYCFiles()
        {
            List<Files> files = new List<Files>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {

                using (SqlCommand cmd = new SqlCommand("Select * from KYCTemplate", con))
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