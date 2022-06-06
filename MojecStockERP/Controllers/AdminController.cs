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
    public class AdminController : Controller
    {
        List<UserLogin> _users = new List<UserLogin>();
        List<Installation> _installation = new List<Installation>();
        List<KYC> _Kyc = new List<KYC>();
        List<MetersProduced> _meters = new List<MetersProduced>();
        List<MetersDispatched> _dispatched = new List<MetersDispatched>();
        List<StoredMeters> _store = new List<StoredMeters>();
     
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["StockManagementERP"].ConnectionString);
        // GET: Admin
        public ActionResult Admin()
        {
            string Username = (string)Session["Username"];

            if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            {
                return RedirectToAction("Login", "Authentication");
            }
            con.Open();
            SqlCommand cmd = new SqlCommand("select Count(*) from Production_Tbl", con);
            int r = Convert.ToInt32(cmd.ExecuteScalar());
            ViewBag.TotalMeter = r;

            SqlCommand cmd2 = new SqlCommand("select Count(*) from DispatchedMeters_Tbl", con);
            int r2 = Convert.ToInt32(cmd2.ExecuteScalar());
            ViewBag.TotalDispatched = r2;

            SqlCommand cmd3 = new SqlCommand("select Count(*) from Installation_Tbl", con);
            int r3 = Convert.ToInt32(cmd3.ExecuteScalar());
            ViewBag.Installation = r3;

            SqlCommand cmd7 = new SqlCommand("select Count(*) from StoreRecieved_Tbl", con);
            int r7 = Convert.ToInt32(cmd7.ExecuteScalar());
            ViewBag.Store = r7;

            ViewBag.Available = r - r2;

            
            return View();
        }
        public ActionResult AddUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddUser(UserLogin user)
        {
            try
            {
                if(user.Disco == null)
                {
                    if (user.Password != user.Confirmpassword)
                    {
                        TempData["delete"] = "Password do not match";
                    }
                    else
                    {
                        using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
                        {
                            using (SqlCommand cmd = new SqlCommand("AddUsers", con))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@Fullname", user.Fullname);
                                cmd.Parameters.AddWithValue("@Username", user.Username);
                                cmd.Parameters.AddWithValue("@Password", user.Password);
                                cmd.Parameters.AddWithValue("@Role", user.Role);
                                if (con.State != System.Data.ConnectionState.Open)

                                    con.Open();
                                cmd.ExecuteNonQuery();
                            }

                            con.Close();
                        }
                        TempData["save"] = "User has been added to list";
                        return RedirectToAction("Users");
                    }
                }
                else if(user.Disco != null)
                {
                    try
                    {

                        if (user.Password != user.Confirmpassword)
                        {
                            TempData["delete"] = "Password do not match";
                        }
                        else
                        {
                            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
                            {
                                using (SqlCommand cmd = new SqlCommand("createDiscoUser", con))
                                {

                                    user.Role = "Disco";
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@Fullname", user.Fullname);
                                    cmd.Parameters.AddWithValue("@Username", user.Username);
                                    cmd.Parameters.AddWithValue("@Password", user.Password);
                                    cmd.Parameters.AddWithValue("@Role", user.Role);
                                    cmd.Parameters.AddWithValue("@Disco", user.Disco);
                                    if (con.State != System.Data.ConnectionState.Open)

                                        con.Open();
                                    cmd.ExecuteNonQuery();
                                }

                                con.Close();
                            }
                            TempData["save"] = "User has been added to list";
                            return RedirectToAction("Users");
                        }

                    }
                    catch (Exception ex)
                    {
                        TempData["delete"] = "Error occured";
                    }
                }
               

            }
            catch (Exception ex)
            {
                TempData["delete"] = "Error occured";
            }
           

            return View();
          
        }
        public ActionResult Users()
        {
            _users = new List<UserLogin>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("select * from UserLogin_Tbl", con);
                cmd.CommandType = System.Data.CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UserLogin user = new UserLogin();
                    user.UserID = Convert.ToInt32(rdr["UserID"].ToString());
                    user.Fullname = rdr["FullName"].ToString();
                    user.Username = rdr["UserName"].ToString();
                    user.Role = rdr["Role"].ToString();
                    _users.Add(user);
                }
                rdr.Close();
            }
            return View(_users);
        }
        public ActionResult Installation()
        {
            _installation = new List<Installation>();
            using(SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetAllInstalledMeters", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Installation install = new Installation();
                    install.MeterNo = rdr["MeterNo"].ToString();
                    install.MeterType = rdr["MeterType"].ToString();
                    install.Disco = rdr["Disco"].ToString();
                    install.DateInstalled = rdr["DateInstalled"].ToString();
                    _installation.Add(install);
                }
                rdr.Close();
            }
            return View(_installation);
        }
        [HttpGet]
        public ActionResult UpdateInstallation(string Id)
        {
            Installation install = new Installation();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetInstallationByID", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeterNo",Id);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    install.MeterNo = rdr["MeterNo"].ToString();
                    install.MeterType = rdr["MeterType"].ToString();
                    install.DateInstalled = rdr["DateInstalled"].ToString();
                }
            }
            return View(install);
        }
        [HttpPost]
        public ActionResult UpdateInstallation(Installation install)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateInstallationTbl", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MeterNo", install.MeterNo);
                    cmd.Parameters.AddWithValue("@MeterType", install.MeterType);
                    cmd.Parameters.AddWithValue("@DateInstalled", install.DateInstalled);
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
            TempData["save"] = "Details Updated Successfully";
            return RedirectToAction("Installation");
        }
        public ActionResult DeleteInstallation(string Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("DeleteInstalledMetersByMeterNo", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeterNo", Id);
                if (con.State != System.Data.ConnectionState.Open)

                    con.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["save"] = "Installation Deleted Successfully";
            return RedirectToAction("Installation");
        }
        public ActionResult UpdateKYC(string id)
        {
            KYC kyc = new KYC();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetKYCByID", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ARN", id);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    kyc.DateShared = rdr["DateShared"].ToString();
                    kyc.ACno1 = rdr["ACNo1"].ToString();
                    kyc.Acno2 = rdr["ACNo2"].ToString();
                    kyc.SBCsMain = rdr["SBCsMain"].ToString();
                    kyc.ARN = rdr["ARN"].ToString();
                    kyc.CustomerName = rdr["CustomerName"].ToString();
                    kyc.Email = rdr["Email"].ToString();
                    kyc.PhoneNumber = rdr["PhoneNumber"].ToString();
                    kyc.Address = rdr["Address"].ToString();
                    kyc.BU = rdr["BU"].ToString();
                    kyc.Tarriff = rdr["Tariff"].ToString();
                    kyc.MeterStatus = rdr["MeterStatus"].ToString();
                    kyc.TypeOfApartment = rdr["TypeofApartment"].ToString();
                    kyc.MeterRecommended = rdr["MeterRecommended"].ToString();
                    kyc.AdditionalComment = rdr["AdditionalComment"].ToString();

                }
            }
            return View(kyc);
        }
        [HttpPost]
        public ActionResult UpdateKYC(KYC kyc)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateKYCwithARN", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DateShared", kyc.DateShared);
                    cmd.Parameters.AddWithValue("@ACNo1", kyc.ACno1);
                    cmd.Parameters.AddWithValue("@ACNo2", kyc.Acno2);
                    cmd.Parameters.AddWithValue("@SBCsMain", kyc.SBCsMain);
                    cmd.Parameters.AddWithValue("@ARN", kyc.ARN);
                    cmd.Parameters.AddWithValue("@CustomerName ", kyc.CustomerName);
                    cmd.Parameters.AddWithValue("@Email ", kyc.Email);
                    cmd.Parameters.AddWithValue("@PhoneNumber ", kyc.PhoneNumber);
                    cmd.Parameters.AddWithValue("@Address ", kyc.Address);
                    cmd.Parameters.AddWithValue("@BU", kyc.BU);
                    cmd.Parameters.AddWithValue("@Tariff", kyc.Tarriff);
                    cmd.Parameters.AddWithValue("@MeterStatus", kyc.MeterStatus);
                    cmd.Parameters.AddWithValue("@TypeofApartment", kyc.TypeOfApartment);
                    cmd.Parameters.AddWithValue("@MeterRecommended", kyc.MeterRecommended);
                    cmd.Parameters.AddWithValue("@AdditionalComment", kyc.AdditionalComment);
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
            TempData["save"] = "Details Updated Successfully";
            return RedirectToAction("KYC");
        }

        public ActionResult DeleteKYC(string Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("DeleteKYCUpload", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ARN", Id);
                if (con.State != System.Data.ConnectionState.Open)

                    con.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["save"] = "KYC Deleted Successfully";
            return RedirectToAction("KYC");
        }
        public ActionResult KYC()
        {
            _Kyc = new List<KYC>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetAllKYCUpload", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    KYC kyc = new KYC();
                    kyc.DateShared = rdr["DateShared"].ToString();
                    kyc.CustomerName = rdr["CustomerName"].ToString();
                    kyc.ARN = rdr["ARN"].ToString();
                    kyc.AdditionalComment = rdr["AdditionalComment"].ToString();
                    kyc.Disco = rdr["Disco"].ToString();
                     _Kyc.Add(kyc);
                }
                rdr.Close();
            }
            return View(_Kyc);
        }
        public ActionResult MeterProduced()
        {
            _meters = new List<MetersProduced>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetAllProducedMeters", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    MetersProduced meters = new MetersProduced();
                    meters.MeterID =Convert.ToInt32(rdr["MeterID"].ToString());
                    meters.MeterNo = rdr["MeterNo"].ToString();
                    meters.MeterType = rdr["MeterType"].ToString();
                    meters.SGC = rdr["SGC"].ToString();
                    meters.Model = rdr["Model"].ToString();
                    meters.Partners = rdr["Partners"].ToString();
                    meters.DateOfSupply = rdr["DateOfProduction"].ToString();
                    meters.Disco = rdr["Disco"].ToString();
                    _meters.Add(meters);
                }
                rdr.Close();
            }
            return View(_meters);
        }
        public ActionResult UpdateMeterProduced(int Id)
        {
             MetersProduced meter = new MetersProduced();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetProducedByID", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeterID", Id);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    meter.MeterID = Convert.ToInt32(rdr["MeterID"].ToString());
                    meter.MeterNo = rdr["MeterNo"].ToString();
                    meter.MeterType = rdr["MeterType"].ToString();
                    meter.Model = rdr["Model"].ToString();
                    meter.SoftwareVersion = rdr["SoftwareVersion"].ToString();
                    meter.HardwareVersion = rdr["HardwareVersion"].ToString();
                    meter.DateOfSupply = rdr["DateOfProduction"].ToString();
                    meter.Partners = rdr["Partners"].ToString();
                    meter.SGC = rdr["SGC"].ToString();
                    meter.TarrifIndex = rdr["TarrifIndex"].ToString();
                    
                }
                rdr.Close();
            }
            return View(meter);
        }
        [HttpPost]
        public ActionResult UpdateMeterProduced(MetersProduced meters)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateProductionTbl", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MeterID", meters.MeterID);
                    cmd.Parameters.AddWithValue("@MeterNo", meters.MeterNo);
                    cmd.Parameters.AddWithValue("@MeterType", meters.MeterType);
                    cmd.Parameters.AddWithValue("@Model", meters.Model);
                    cmd.Parameters.AddWithValue("@SoftwareVersion", meters.SoftwareVersion);
                    cmd.Parameters.AddWithValue("@HardwareVersion", meters.HardwareVersion);
                    cmd.Parameters.AddWithValue("@DateOfProduction", meters.DateOfSupply);
                    cmd.Parameters.AddWithValue("@Partners", meters.Partners);
                    cmd.Parameters.AddWithValue("@SGC", meters.SGC);
                    cmd.Parameters.AddWithValue("@TarriffIndex", meters.TarrifIndex);
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
            TempData["save"] = "Details Updated Successfully";
            return RedirectToAction("MeterProduced");
        }
        public ActionResult DeleteProducedMeter(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("DeleteProducedMeters", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeterID", Id);
                if (con.State != System.Data.ConnectionState.Open)

                    con.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["save"] = "Details Deleted Successfully";
            return RedirectToAction("MeterProduced");
        }
        public ActionResult UpdateRecievedMeters(int Id)
        {
            StoredMeters meter = new StoredMeters();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetRecievedMetersByID", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeterID", Id);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    meter.MeterID = Convert.ToInt32(rdr["MeterID"].ToString());
                    meter.MeterNo = rdr["MeterNo"].ToString();
                    meter.MeterType = rdr["MeterType"].ToString();
                    meter.Model = rdr["Model"].ToString();
                    meter.SoftwareVersion = rdr["SoftwareVersion"].ToString();
                    meter.HardwareVersion = rdr["HardwareVersion"].ToString();
                    meter.Date = rdr["Date"].ToString();
                    meter.Partners = rdr["Partners"].ToString();
                    meter.SGC = rdr["SGC"].ToString();
                    meter.TarrifIndex = rdr["TarrifIndex"].ToString();

                }
                rdr.Close();
            }
            return View(meter);
        }
        [HttpPost]
        public ActionResult UpdateRecievedMeters(StoredMeters meters)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateStoreRecievedTbl", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MeterID", meters.MeterID);
                    cmd.Parameters.AddWithValue("@MeterNo", meters.MeterNo);
                    cmd.Parameters.AddWithValue("@MeterType", meters.MeterType);
                    cmd.Parameters.AddWithValue("@Model", meters.Model);
                    cmd.Parameters.AddWithValue("@SoftwareVersion", meters.SoftwareVersion);
                    cmd.Parameters.AddWithValue("@HardwareVersion", meters.HardwareVersion);
                    cmd.Parameters.AddWithValue("@Date", meters.Date);
                    cmd.Parameters.AddWithValue("@Partners", meters.Partners);
                    cmd.Parameters.AddWithValue("@SGC", meters.SGC);
                    cmd.Parameters.AddWithValue("@TarriffIndex", meters.TarrifIndex);
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
            TempData["save"] = "Details Updated Successfully";
            return RedirectToAction("StoredMeters");
        }
        public ActionResult DeleteStoredMeters(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("DeleteStoreRecieved", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeterID", Id);
                if (con.State != System.Data.ConnectionState.Open)

                    con.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["save"] = "Details Deleted Successfully";
            return RedirectToAction("StoredMeters");
        }
        public ActionResult MeterDispatched()
        {
            _dispatched = new List<MetersDispatched>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetAllDispatchedMeters", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    MetersDispatched meters = new MetersDispatched();
                    meters.MeterID =Convert.ToInt32(rdr["MeterID"].ToString());
                    meters.MeterNo = rdr["MeterNo"].ToString();
                    meters.MeterType = rdr["MeterType"].ToString();
                    meters.SGC = rdr["SGC"].ToString();
                    meters.Model = rdr["Model"].ToString();
                    meters.Partners = rdr["Partners"].ToString();
                    meters.DateOfDispatch = rdr["DateOfDispatched"].ToString();
                    meters.Disco = rdr["Disco"].ToString();
                    _dispatched.Add(meters);
                }
                rdr.Close();
            }
            return View(_dispatched);
        }
        public ActionResult StoredMeters()
        {
            _store = new List<StoredMeters>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetAllMetersinStore", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    StoredMeters meters = new StoredMeters();
                    meters.MeterID = Convert.ToInt32(rdr["MeterID"].ToString());
                    meters.MeterNo = rdr["MeterNo"].ToString();
                    meters.MeterType = rdr["MeterType"].ToString();
                    meters.SGC = rdr["SGC"].ToString();
                    meters.Model = rdr["Model"].ToString();
                    meters.Partners = rdr["Partners"].ToString();
                    meters.SoftwareVersion = rdr["Softwareversion"].ToString();
                    meters.HardwareVersion = rdr["Hardwareversion"].ToString();
                    _store.Add(meters);
                }
                rdr.Close();
            }
            return View(_store);
        }
        public ActionResult UpdateMetersDispatched(string Id)
        {
            MetersDispatched meter = new MetersDispatched();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetDispatchedByID", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeterID", Id);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    meter.MeterID = Convert.ToInt32(rdr["MeterID"].ToString());
                    meter.MeterNo = rdr["MeterNo"].ToString();
                    meter.MeterType = rdr["MeterType"].ToString();
                    meter.Model = rdr["Model"].ToString();
                    meter.SoftwareVersion = rdr["SoftwareVersion"].ToString();
                    meter.HardwareVersion = rdr["HardwareVersion"].ToString();
                    meter.DateOfDispatch = rdr["DateofDispatched"].ToString();
                    meter.Partners = rdr["Partners"].ToString();
                    meter.SGC = rdr["SGC"].ToString();
                    meter.TarrifIndex = rdr["TarrifIndex"].ToString();

                }
                rdr.Close();
            }
            return View(meter);
        }
        [HttpPost]
        public ActionResult UpdateMetersDispatched(MetersDispatched meters)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateDispatchedTbl", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MeterID", meters.MeterID);
                    cmd.Parameters.AddWithValue("@MeterNo", meters.MeterNo);
                    cmd.Parameters.AddWithValue("@MeterType", meters.MeterType);
                    cmd.Parameters.AddWithValue("@Model", meters.Model);
                    cmd.Parameters.AddWithValue("@SoftwareVersion", meters.SoftwareVersion);
                    cmd.Parameters.AddWithValue("@HardwareVersion", meters.HardwareVersion);
                    cmd.Parameters.AddWithValue("@DateofDispatched", meters.DateOfDispatch);
                    cmd.Parameters.AddWithValue("@Partners", meters.Partners);
                    cmd.Parameters.AddWithValue("@SGC", meters.SGC);
                    cmd.Parameters.AddWithValue("@TarriffIndex", meters.TarrifIndex);
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
            TempData["save"] = "Details Updated Successfully";
            return RedirectToAction("MeterDispatched");
        }
        public ActionResult ActivateUsers(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("ActivateUsers", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", Id);
                if (con.State != System.Data.ConnectionState.Open)

                    con.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["save"] = "User Activated Successfully";
            return RedirectToAction("Users");
        }
        public ActionResult DeactivateUsers(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("DeactivateUsers", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", Id);
                if (con.State != System.Data.ConnectionState.Open)

                    con.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["save"] = "User Deactivated Successfully";
            return RedirectToAction("Users");
        }



    }
}