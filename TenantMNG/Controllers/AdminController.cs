using log4net;

using System;

using System.Linq;

using System.Web.Mvc;
using TenantMNG.Models;
using TenantMNG.ViewModel;

namespace TenantMNG.Controllers
{
    public class AdminController : MyController
    {
        DB_TenantMNGEntities _dbc = new DB_TenantMNGEntities();
        ILog log = log4net.LogManager.GetLogger(typeof(AdminController));
        // GET: Admin
        [SessionCheck]
        public ActionResult Dashboard()
        {
            int pmid = Convert.ToInt32(Session["uid"].ToString());

            var _userTenant = _dbc.tbl_user_master.Where(x => x.int_user_type_id == 3).ToList().Take(5).OrderByDescending(x => x.int_id);

            var _userPM = _dbc.tbl_user_master.Where(x => x.int_user_type_id == 2).ToList().Take(5).OrderByDescending(x => x.int_id);

            AdminDashboard _objvm = new AdminDashboard();

            _objvm.tbl_PM = _userPM.ToList();

            _objvm.tbl_Tenant = _userTenant.ToList();

            return View(_objvm);
        }


        public ActionResult ChangePassword()
        {
            int _id = Convert.ToInt32(Session["uid"].ToString());
            var _user = _dbc.tbl_user_master.Where(x => x.int_id == _id).FirstOrDefault();

            UserMasterVM vmobj = new UserMasterVM()
            {

                int_id = _id,
                str_password = _user.str_password,
            };


            return View(vmobj);
        }

//        [HttpPost]
//        public JsonResult CharterColumn()
//        {
//            return this.Json(
//(from obj in _dbc.tbl_invoice select new { Growth_Year = obj.date_s_bill_date, Growth_Value = obj.dec_inter_energy + obj.dec_peak_energy })
//, JsonRequestBehavior.AllowGet
//);
//        }

        public ActionResult ChangeLanguage(string lang, string cnt, string act, string qry = null)
        {
            Session["_lang"] = lang;

            new LanguageMang().SetLanguage(lang);

            var query = Request.QueryString.AllKeys.Count();

            if (act.ToLower() == "edittenant" || act.ToLower() == "invoice")
                return RedirectToAction(act, cnt, new { tenantid = qry });
            else
                return RedirectToAction(act, cnt);



            //string _cntrl = obj.RouteData.Values["controller"].ToString();


        }



    }
}