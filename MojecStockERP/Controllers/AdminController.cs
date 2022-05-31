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
                    meters.MeterNo = rdr["MeterNo"].ToString();
                    meters.MeterType = rdr["MeterType"].ToString();
                    meters.SGC = rdr["SGC"].ToString();
                    meters.Model = rdr["Model"].ToString();
                    meters.Partners = rdr["Partners"].ToString();
                    meters.DateOfSupply = rdr["DateOfProduction"].ToString();
                    _meters.Add(meters);
                }
                rdr.Close();
            }
            return View(_meters);
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

    }
}