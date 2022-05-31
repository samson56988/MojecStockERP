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
            catch (Exception ex)
            {
                TempData["delete"] = "Error occured";
            }
           

            return View();
          
        }

        public ActionResult DiscoUser()
        {
            _users = new List<UserLogin>();
            using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
            {
                SqlCommand cmd = new SqlCommand("select * from UserLogin_Tbl where Role = 'Disco'", con);
                cmd.CommandType = System.Data.CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UserLogin user = new UserLogin();
                    user.Fullname = rdr["FullName"].ToString();
                    user.Username = rdr["UserName"].ToString();
                    user.Role = rdr["Role"].ToString();
                    _users.Add(user);
                }
                rdr.Close();
            }
            return View(_users);
        }

        public ActionResult CreateDiscoUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateDiscoUser(DiscoUser disco)
        {
               try
                {

                    if (disco.Password != disco.Confirmpassword)
                    {
                        TempData["delete"] = "Password do not match";
                    }
                    else
                    {
                        using (SqlConnection con = new SqlConnection(StoreConnection.GetConnection()))
                        {
                            using (SqlCommand cmd = new SqlCommand("createDiscoUser", con))
                            {

                                disco.Role = "Disco";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@Fullname", disco.Name);
                                cmd.Parameters.AddWithValue("@Username", disco.Username);
                                cmd.Parameters.AddWithValue("@Password", disco.Password);
                                cmd.Parameters.AddWithValue("@Role", disco.Role);
                                cmd.Parameters.AddWithValue("@Disco", disco.Disco);
                                if (con.State != System.Data.ConnectionState.Open)

                                    con.Open();
                                cmd.ExecuteNonQuery();
                            }

                            con.Close();
                        }
                        TempData["save"] = "User has been added to list";
                        return RedirectToAction("DiscoUsers");
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
                    user.Fullname = rdr["FullName"].ToString();
                    user.Username = rdr["UserName"].ToString();
                    user.Role = rdr["Role"].ToString();
                    _users.Add(user);
                }
                rdr.Close();
            }
            return View(_users);
        }
    }
}