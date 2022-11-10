using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TenantMNG.Core;
using TenantMNG.Models;
using TenantMNG.ViewModel;

namespace TenantMNG.Controllers
{
    public class LoginController : MyController
    {
        DB_TenantMNGEntities _dbc = new DB_TenantMNGEntities();
        ILog log = log4net.LogManager.GetLogger(typeof(LoginController));

        // GET: Login
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginVM objuser)
        {
            try
            {
                var parameterDate = DateTime.ParseExact("02/01/2023", "MM/dd/yyyy", CultureInfo.InvariantCulture);
                var todaysDate = DateTime.Today;

                if (ModelState.IsValid)
                {
                    var user = _dbc.tbl_user_master.SingleOrDefault(x => x.str_user_name == objuser.str_user_name);
                    if (user != null && (todaysDate < parameterDate))
                    {
                        if (user.str_password == objuser.str_password)
                        {
                            Session["uid"] = user.int_id;
                            Session["utypeid"] = user.int_user_type_id;

                            if (user.int_user_type_id == CommonCls._usertypeAdmin)
                                return RedirectToAction("Dashboard", "Admin");
                            else if (user.int_user_type_id == CommonCls._usertypePM)
                            {
                                Session["address"] = user.str_add_1 + ", " + user.str_add_2 + ", " + user.str_city + ", " + user.str_state + ", " + user.str_country;
                                return RedirectToAction("Dashboard", "PM");
                            }
                            else if (user.int_user_type_id == CommonCls._usertypeTenant)
                                return RedirectToAction("Dashboard", "Tenant");

                        }
                        else
                        {
                            ViewBag.isvalid = "False";
                        }
                    }
                    else
                    {
                        ViewBag.isvalid = "False";
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }


        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult Forgotpassword(string emailid)
        {
            string _lStatus = "0";
            try
            {
                var _user = _dbc.tbl_user_master.Where(x => x.str_user_name == emailid || x.str_email == emailid).FirstOrDefault();

                if (_user != null)
                {


                    string _path = Server.MapPath("~/EmailTemplate/forgotpassword.html");
                    string html = System.IO.File.ReadAllText(_path);

                    html = html.Replace("#name", _user.str_contact_name);
                    html = html.Replace("#uname", _user.str_user_name);
                    html = html.Replace("#pass", _user.str_password);
                    html = html.Replace("#url", CommonCls._applicationurl);

                    CommonCls.sendMail(_user.str_email, "Login Details for Tenanat Mangment", html, "");
                    _lStatus = "1";
                }

            }
            catch (Exception)
            {

            }

            return Json(_lStatus);
        }

        public ActionResult ChangeLanguage(string lang)
        {
            new LanguageMang().SetLanguage(lang);
            return RedirectToAction("Index", "Login");
        }
    }
}