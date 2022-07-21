using MojecStockERP.Config;
using MojecStockERP.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
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
        List<MeterOrder> _orders = new List<MeterOrder>();
        List<PlasticOrder> _plastic = new List<PlasticOrder>();
        List<Disco> _disco = new List<Disco>();
        List<Partners> _partners = new List<Partners>();
        List<MeterType> _Metertype = new List<MeterType>();
        List<Model> _model = new List<Model>();
        List<Accessories> _accessories = new List<Accessories>();
        List<Chemicals> _chemical = new List<Chemicals>();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["StockManagementERP"].ConnectionString);
        SqlCommand com = new SqlCommand();
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
                    install.BU = rdr["BU"].ToString();
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
            try 
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
            catch(Exception ex)
            {
                TempData["delete"] = "Failed to update";
            }

            return View();
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
            try 
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
            catch(Exception ex)
            {
                TempData["delete"] = "Update Failed";
                return View();
            }
            
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
            try
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
            catch(Exception ex)
            {
                TempData["delete"] = "Details Deleted Successfully";
                return View();
            }
           
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
            try
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
            catch(Exception ex)
            {
                TempData["delete"] = "Failed to update details";
                return View();
            }
           
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
        public ActionResult ProcurementMeter()
        {
            //string Username = (string)Session["Username"];

            //if (string.IsNullOrEmpty(Convert.ToString(Session["Username"])))
            //{
            //    return RedirectToAction("Login", "Authentication");
            //}
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
                    _orders.Add(produced);
                }
                rdr.Close();
            }
            return View(_orders);
        }      
        public ActionResult UpdateProcuremeter(int Id)
       {
            MeterOrder meter = new MeterOrder();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetMeterProcurement", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MID", Id);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    meter.MeterID = Convert.ToInt32(rdr["MeterProcurementID"].ToString());
                    meter.MeterType = rdr["MeterType"].ToString();
                    meter.Model = rdr["Model"].ToString();
                    meter.Partners = rdr["Partners"].ToString();
                    meter.Quantity = rdr["Quantity"].ToString();
                    meter.BillofLaden = rdr["BillofLaden"].ToString();
                    meter.Date = rdr["Date"].ToString();

                }
                rdr.Close();
            }
            return View(meter);
        }
        [HttpPost]
        public ActionResult UpdateProcuremeter(MeterOrder order)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
                {
                    using (SqlCommand cmd = new SqlCommand("UpdateMeterOrder", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MPID", order.MeterID);
                        cmd.Parameters.AddWithValue("@MeterType", order.MeterType);
                        cmd.Parameters.AddWithValue("@Model", order.Model);
                        cmd.Parameters.AddWithValue("@Partners", order.Partners);
                        cmd.Parameters.AddWithValue("@Quantity", order.Quantity);
                        cmd.Parameters.AddWithValue("@BillOfLaden", order.BillofLaden);
                        cmd.Parameters.AddWithValue("@Date", order.Date);
                        if (con.State != System.Data.ConnectionState.Open)

                            con.Open();
                        cmd.ExecuteNonQuery();
                    }

                    con.Close();
                }
                TempData["save"] = "Details Updated Successfully";
                return RedirectToAction("ProcurementMeter");
            }
            catch (Exception ex)
            {
                TempData["delete"] = "Failed to update details";
                return View();
            }
        }
        public ActionResult DeleteProcurement(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("DeleteMeterOdered", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeterID", Id);
                if (con.State != System.Data.ConnectionState.Open)

                    con.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["save"] = "Details Deleted Successfully";
            return RedirectToAction("ProcurementMeter");
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
                    produced.orderId = Convert.ToInt32(rdr["ID"].ToString());
                    produced.Chemicals = rdr["Chemical"].ToString();
                    produced.Quantity = rdr["Quantity"].ToString();
                    produced.Date = rdr["Date"].ToString();
                   
                    
                    _plastic.Add(produced);
                }
                rdr.Close();
            }
            return View(_plastic);
        }
        public ActionResult UpdatePlasticOrder(int Id)
        {
            PlasticOrder meter = new PlasticOrder();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetPlasticID", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", Id);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    meter.orderId = Convert.ToInt32(rdr["ID"].ToString());
                    meter.Chemicals = rdr["Chemical"].ToString();
                    meter.Quantity = rdr["Quantity"].ToString();
                    meter.Date = rdr["Date"].ToString();

                }
                rdr.Close();
            }
            return View(meter);
        }
        [HttpPost]
        public ActionResult UpdatePlasticOrder(PlasticOrder plastic)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
                {
                    using (SqlCommand cmd = new SqlCommand("UpdatePlastic", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MID", plastic.orderId);
                        cmd.Parameters.AddWithValue("@Chemicals", plastic.Chemicals);
                        cmd.Parameters.AddWithValue("@Quantity", plastic.Quantity);
                        cmd.Parameters.AddWithValue("@Date", plastic.Date);
                        if (con.State != System.Data.ConnectionState.Open)

                            con.Open();
                        cmd.ExecuteNonQuery();
                    }

                    con.Close();
                }
                TempData["save"] = "Details Updated Successfully";
                return RedirectToAction("PlasticOrder");
            }
            catch (Exception ex)
            {
                TempData["delete"] = "Failed to update details";
                return View();
            }
        }
        public ActionResult DeletePlasticOrder(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("DeletePlasticOrdered", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MID", Id);
                if (con.State != System.Data.ConnectionState.Open)

                    con.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["save"] = "Details Deleted Successfully";
            return RedirectToAction("PlasticOrder");
        }
        public ActionResult UploadInstallationTemplate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadInstallationTemplate(HttpPostedFileBase postedFiles)
        {

            SqlDataReader dr;
            con.Open();
            com.Connection = con;
            com.CommandText = "Select * from InstallationTemplate where Id = 1";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                string fileName = Path.GetFileName(postedFiles.FileName);
                string type = postedFiles.ContentType;
                byte[] bytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    postedFiles.InputStream.CopyTo(ms);
                    bytes = ms.ToArray();
                }
                using (SqlConnection con = new SqlConnection((StoreConnection.GetConnection())))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "Update InstallationTemplate set Name = @Name, ContentType = @ContentType, Data = @Data where Id = 1";
                        cmd.Parameters.AddWithValue("@Name", fileName);
                        cmd.Parameters.AddWithValue("@ContentType", type);
                        cmd.Parameters.AddWithValue("@Data", bytes);
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            else
            {
                string fileName = Path.GetFileName(postedFiles.FileName);
                string type = postedFiles.ContentType;
                byte[] bytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    postedFiles.InputStream.CopyTo(ms);
                    bytes = ms.ToArray();
                }
                using (SqlConnection con = new SqlConnection((StoreConnection.GetConnection())))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "INSERT INTO InstallationTemplate(Name, ContentType, Data) VALUES (@Name, @ContentType, @Data)";
                        cmd.Parameters.AddWithValue("@Name", fileName);
                        cmd.Parameters.AddWithValue("@ContentType", type);
                        cmd.Parameters.AddWithValue("@Data", bytes);
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }



            return View();
        }
        public ActionResult UploadKycTemplate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadKYCTemplate(HttpPostedFileBase postedFiles)
        {

            SqlDataReader dr;
            con.Open();
            com.Connection = con;
            com.CommandText = "Select * from KYCTemplate where Id = 1";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                 string fileName = Path.GetFileName(postedFiles.FileName);
            string type = postedFiles.ContentType;
            byte[] bytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                postedFiles.InputStream.CopyTo(ms);
                bytes = ms.ToArray();
            }
            using (SqlConnection con = new SqlConnection((StoreConnection.GetConnection())))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "Update KYCTemplate set Name = @Name, ContentType = @ContenType, Data = @Data where Id = 1";
                    cmd.Parameters.AddWithValue("@Name", fileName);
                    cmd.Parameters.AddWithValue("@ContentType", type);
                    cmd.Parameters.AddWithValue("@Data", bytes);
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            }
            else
            {
                string fileName = Path.GetFileName(postedFiles.FileName);
                string type = postedFiles.ContentType;
                byte[] bytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    postedFiles.InputStream.CopyTo(ms);
                    bytes = ms.ToArray();
                }
                using (SqlConnection con = new SqlConnection((StoreConnection.GetConnection())))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "INSERT INTO KYCTemplate(Name, ContentType, Data) VALUES (@Name, @ContentType, @Data)";
                        cmd.Parameters.AddWithValue("@Name", fileName);
                        cmd.Parameters.AddWithValue("@ContentType", type);
                        cmd.Parameters.AddWithValue("@Data", bytes);
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
           


            return RedirectToAction("UploadInstallationTemplate");
        }
        [HttpPost]
        public ActionResult UploadDispatchedTemplate(HttpPostedFileBase postedFiles)
        {

            SqlDataReader dr;
            con.Open();
            com.Connection = con;
            com.CommandText = "Select * from DispatchTemplate where Id = 1";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                string fileName = Path.GetFileName(postedFiles.FileName);
                string type = postedFiles.ContentType;
                byte[] bytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    postedFiles.InputStream.CopyTo(ms);
                    bytes = ms.ToArray();
                }
                using (SqlConnection con = new SqlConnection((StoreConnection.GetConnection())))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "Update KYCTemplate set Name = @Name, ContentType = @ContentType, Data = @Data where Id = 1";
                        cmd.Parameters.AddWithValue("@Name", fileName);
                        cmd.Parameters.AddWithValue("@ContentType", type);
                        cmd.Parameters.AddWithValue("@Data", bytes);
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            else
            {
                string fileName = Path.GetFileName(postedFiles.FileName);
                string type = postedFiles.ContentType;
                byte[] bytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    postedFiles.InputStream.CopyTo(ms);
                    bytes = ms.ToArray();
                }
                using (SqlConnection con = new SqlConnection((StoreConnection.GetConnection())))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "INSERT INTO DispatchTemplate(Name, ContentType, Data) VALUES (@Name, @ContentType, @Data)";
                        cmd.Parameters.AddWithValue("@Name", fileName);
                        cmd.Parameters.AddWithValue("@ContentType", type);
                        cmd.Parameters.AddWithValue("@Data", bytes);
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }



            return RedirectToAction("UploadInstallationTemplate");
        }
        [HttpPost]
        public ActionResult UploadProductionTemplate(HttpPostedFileBase postedFiles)
        {

            SqlDataReader dr;
            con.Open();
            com.Connection = con;
            com.CommandText = "Select * from ProductionTemplate where Id = 1";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                string fileName = Path.GetFileName(postedFiles.FileName);
                string type = postedFiles.ContentType;
                byte[] bytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    postedFiles.InputStream.CopyTo(ms);
                    bytes = ms.ToArray();
                }
                using (SqlConnection con = new SqlConnection((StoreConnection.GetConnection())))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "Update ProductionTemplate set Name = @Name, ContentType = @ContentType, Data = @Data where Id = 1";
                        cmd.Parameters.AddWithValue("@Name", fileName);
                        cmd.Parameters.AddWithValue("@ContentType", type);
                        cmd.Parameters.AddWithValue("@Data", bytes);
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            else
            {
                string fileName = Path.GetFileName(postedFiles.FileName);
                string type = postedFiles.ContentType;
                byte[] bytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    postedFiles.InputStream.CopyTo(ms);
                    bytes = ms.ToArray();
                }
                using (SqlConnection con = new SqlConnection((StoreConnection.GetConnection())))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "INSERT INTO ProductionTemplate(Name, ContentType, Data) VALUES (@Name, @ContentType, @Data)";
                        cmd.Parameters.AddWithValue("@Name", fileName);
                        cmd.Parameters.AddWithValue("@ContentType", type);
                        cmd.Parameters.AddWithValue("@Data", bytes);
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }



            return RedirectToAction("UploadInstallationTemplate");
        }
        public ActionResult UploadInwardGoodsTemplate(HttpPostedFileBase postedFiles)
        {
            SqlDataReader dr;
            con.Open();
            com.Connection = con;
            com.CommandText = "Select * from RecievedGoodsTemplate where Id = 1";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                string fileName = Path.GetFileName(postedFiles.FileName);
                string type = postedFiles.ContentType;
                byte[] bytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    postedFiles.InputStream.CopyTo(ms);
                    bytes = ms.ToArray();
                }
                using (SqlConnection con = new SqlConnection((StoreConnection.GetConnection())))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "Update RecievedGoodsTemplate set Name = @Name, ContentType = @ContentType, Data = @Data where Id = 1";
                        cmd.Parameters.AddWithValue("@Name", fileName);
                        cmd.Parameters.AddWithValue("@ContentType", type);
                        cmd.Parameters.AddWithValue("@Data", bytes);
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            else
            {
                string fileName = Path.GetFileName(postedFiles.FileName);
                string type = postedFiles.ContentType;
                byte[] bytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    postedFiles.InputStream.CopyTo(ms);
                    bytes = ms.ToArray();
                }
                using (SqlConnection con = new SqlConnection((StoreConnection.GetConnection())))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "INSERT INTO RecievedGoodsTemplate(Name, ContentType, Data) VALUES (@Name, @ContentType, @Data)";
                        cmd.Parameters.AddWithValue("@Name", fileName);
                        cmd.Parameters.AddWithValue("@ContentType", type);
                        cmd.Parameters.AddWithValue("@Data", bytes);
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }



            return RedirectToAction("UploadInstallationTemplate");
        }
        public ActionResult Disco()
        {
            _disco = new List<Disco>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("select * from Disco_Tbl", con);
                cmd.CommandType = System.Data.CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Disco disco = new Disco();
                    disco.Id = Convert.ToInt32(rdr["DiscoID"].ToString());
                    disco.Name = rdr["DiscoName"].ToString();
                    _disco.Add(disco);
                }
                rdr.Close();
            }
            return View(_disco);
        }
        public ActionResult AddDisco()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddDisco (Disco disco)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("AddDisco", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DiscoName", disco.Name);
                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
            TempData["save"] = "Disco has been added to list";
            return RedirectToAction("Disco");
        }
        public ActionResult UpdateDisco(int Id)
        {
            Disco disco = new Disco();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetDiscoByID", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", Id);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    disco.Id = Convert.ToInt32(rdr["DiscoID"].ToString());
                    disco.Name = rdr["DiscoName"].ToString();             
                }
                rdr.Close();
            }
            return View(disco);
        }
        [HttpPost]
        public ActionResult UpdateDisco(Disco disco)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateDisco_Tbl", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@DiscoID", disco.Id);
                    cmd.Parameters.AddWithValue("@DiscoName", disco.Name);

                    if (con.State != System.Data.ConnectionState.Open)

                        con.Open();
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
            TempData["save"] = "Disco has been updated";
            return RedirectToAction("Disco");
        }
        public ActionResult DeleteDisco(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("DeleteDisco", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DiscoID", Id);
                if (con.State != System.Data.ConnectionState.Open)

                    con.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["save"] = "Disco Deleted Successfully";
            return RedirectToAction("KYC");
        }
        public ActionResult Partners()
        {
            _partners = new List<Partners>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("select * from Partners_Tbl", con);
                cmd.CommandType = System.Data.CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Partners partners = new Partners();
                    partners.Id = Convert.ToInt32(rdr["PartnerID"].ToString());
                    partners.Name = rdr["PartnerName"].ToString();
                    _partners.Add(partners);
                }
                rdr.Close();
            }
            return View(_partners);
        }
        public ActionResult AddPartners()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddPartners(Partners partners)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("AddPartners", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PartnerName", partners.Name);
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            TempData["save"] = "Partners has been added to list";
            return RedirectToAction("Partners");
        }
        public ActionResult UpdatePartners(int Id)
        {
            Partners partners = new Partners();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetPartnersByID", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", Id);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    partners.Id = Convert.ToInt32(rdr["PartnerID"].ToString());
                    partners.Name = rdr["PartnerName"].ToString();
                }
                rdr.Close();
            }
            return View(partners);
        }
        [HttpPost]
        public ActionResult UpdatePartners(Partners partners)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("UpdatePartners_Tbl", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PartnerID", partners.Id);
                    cmd.Parameters.AddWithValue("@PartnerName", partners.Name);
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            TempData["save"] = "Partners has been updated";
            return RedirectToAction("Partners");
        }
        public ActionResult MeterType()
        {
            _Metertype = new List<MeterType>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("select * from MeterType_Tbl", con);
                cmd.CommandType = System.Data.CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    MeterType type = new MeterType();
                    type.Id = Convert.ToInt32(rdr["MeterType_ID"].ToString());
                    type.Name = rdr["MeterTypeName"].ToString();
                    _Metertype.Add(type);
                }
                rdr.Close();
            }
            return View(_Metertype);
        }
        public ActionResult AddMeterType()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddMeterType(MeterType meterType)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("AddMeterType", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MeterTypeName", meterType.Name);
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            TempData["save"] = "Meter Type has been added to list";
            return RedirectToAction("MeterType");
        }
        public ActionResult UpdateMeterType(int Id)
        {
            MeterType meter = new MeterType();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetMeterTypeByID", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", Id);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    meter.Id = Convert.ToInt32(rdr["MeterType_ID"].ToString());
                    meter.Name = rdr["MeterTypeName"].ToString();
                }
                rdr.Close();
            }
            return View(meter);
        }           
        [HttpPost]
        public ActionResult UpdateMeterType(MeterType meterType)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateMeterType_Tbl", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MeterType_ID", meterType.Id);
                    cmd.Parameters.AddWithValue("@MeterTypeName", meterType.Name);
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            TempData["save"] = "Meter Type has been updated";
            return RedirectToAction("MeterType");
        }
        public ActionResult Model()
        {
            _model = new List<Model>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("select * from Model_Tbl", con);
                cmd.CommandType = System.Data.CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    MeterType type = new MeterType();
                    type.Id = Convert.ToInt32(rdr["ModelID"].ToString());
                    type.Name = rdr["ModelName"].ToString();
                    _Metertype.Add(type);
                }
                rdr.Close();
            }
            return View(_Metertype);
        }
        public ActionResult AddModel()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddModel(Model model)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("AddMeterModel", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ModelName", model.Name);
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            TempData["save"] = "Model has been added to list";
            return RedirectToAction("Model");
        }
        public ActionResult UpdateModel(int Id)
        {
            Model model = new Model();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetModelByID", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ModelID", Id);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    model.Id = Convert.ToInt32(rdr["ModelID"].ToString());
                    model.Name = rdr["ModelName"].ToString();
                }
                rdr.Close();
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult UpdateModel(Model model)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateModel_Tbl", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ModelID", model.Id);
                    cmd.Parameters.AddWithValue("@ModelName", model.Name);
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            TempData["save"] = "Model has been added to list";
            return RedirectToAction("Model");
        }
        public ActionResult Chemical()
        {
            _chemical = new List<Chemicals>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("select * from Chemicals_Tbl", con);
                cmd.CommandType = System.Data.CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Chemicals type = new Chemicals();
                    type.Id = Convert.ToInt32(rdr["ChemicalID"].ToString());
                    type.Name = rdr["ChemicalName"].ToString();
                    _chemical.Add(type);
                }
                rdr.Close();
            }
            return View(_chemical);
        }
        public ActionResult AddChemical()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddChemical(Chemicals model)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("AddChemicals", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ChemicalName", model.Name);
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            TempData["save"] = "Chemical has been added to list";
            return RedirectToAction("Chemical");
        }
       
        public ActionResult UpdateChemical(int Id)
        {
            Chemicals chemical = new Chemicals();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetChemicalByID", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", Id);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    chemical.Id = Convert.ToInt32(rdr["ChemicalID"].ToString());
                    chemical.Name = rdr["ChemicalName"].ToString();
                }
                rdr.Close();
            }
            return View(chemical);
        }
        [HttpPost]
        public ActionResult UpdateChemical(Chemicals chemical)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateChemicals_Tbl", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ChemicalID", chemical.Id);
                    cmd.Parameters.AddWithValue("@ChemicalName", chemical.Name);
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            TempData["save"] = "Chemical has been added to list";
            return RedirectToAction("Chemical");
        }

        public ActionResult Accessories()
        {
            _accessories = new List<Accessories>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("select * from AccessoryTbl", con);
                cmd.CommandType = System.Data.CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Accessories accessories = new Accessories();
                    accessories.Id = Convert.ToInt32(rdr["ID"].ToString());
                    accessories.Name = rdr["AccessoryName"].ToString();
                    _accessories.Add(accessories);
                }
                rdr.Close();
            }
            return View(_accessories);
        }

        public ActionResult AddAccessories()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddAccessories(Accessories accessories)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("CreateAccessory", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AccessoryName", accessories.Name);
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            TempData["save"] = "Accessory has been added to list";
            return RedirectToAction("Accessories");
        }
        public ActionResult UpdateAccessories(int Id)
        {
            Accessories accessories = new Accessories();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("GetAccessoryByID", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", Id);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    accessories.Id = Convert.ToInt32(rdr["ID"].ToString());
                    accessories.Name = rdr["AccessoryName"].ToString();
                }
                rdr.Close();
            }
            return View(accessories);
        }
        [HttpPost]
        public ActionResult UpdateAccessories(Accessories accessories)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateAccessory", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AccessoryID", accessories.Id);
                    cmd.Parameters.AddWithValue("@AccessoryName", accessories.Name);
                    if (con.State != System.Data.ConnectionState.Open)
                        con.Open();
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            TempData["save"] = "Accessory has been updated";
            return RedirectToAction("Accessories");
        }

        public ActionResult ActivateAccessories(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("ActivateAccessory", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", Id);
                if (con.State != System.Data.ConnectionState.Open)

                    con.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["save"] = "Accessory Activated Successfully";
            return RedirectToAction("Accessories");
        }

        public ActionResult DeactivateAccessories(int Id)
        {
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("DeactivateAccessory", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", Id);
                if (con.State != System.Data.ConnectionState.Open)

                    con.Open();
                cmd.ExecuteNonQuery();
            }
            TempData["save"] = "Accessory Deactivated Successfully";
            return RedirectToAction("Accessories");
        }








    }
}