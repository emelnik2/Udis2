using log4net;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using TenantMNG.Models;
using TenantMNG.BAL;
using TenantMNG.ViewModel;
using PagedList;
using TenantMNG.Core;
using TenantMNG.ADO.NET;
using System.Collections.Generic;

namespace TenantMNG.Controllers
{
    public class UserController : MyController
    {
        // GET: User
        DB_TenantMNGEntities _dbc = new DB_TenantMNGEntities();
        ILog log = log4net.LogManager.GetLogger(typeof(UserController));

        public ActionResult Index(int id)
        {
            return View(_dbc.tbl_user_master.Where(x => x.int_user_type_id == id).ToList());
        }

        public ActionResult Delete(int id, int utype)
        {
            var user = _dbc.tbl_user_master.Where(x => x.int_pm_id == id).FirstOrDefault();

            //if (user == null)
            //{
            UserBAL objbal = new UserBAL();
            objbal.user_delete(id);
            //}
            //else
            //{
            //    ViewBag.message = "True";

            //}
            if (Session["utypeid"].ToString() == CommonCls._usertypeAdmin.ToString())
            {
                if (utype == CommonCls._usertypePM)
                {
                    return RedirectToAction("PropertyManager", new
                    {
                        page = Request["page"],
                        sortby = Request["sortBy"]
                    });
                }
                else
                {
                    return RedirectToAction("Tenant", new
                    {
                        page = Request["page"],
                        sortby = Request["sortBy"]
                    });
                }
            }
            else
            {


                return RedirectToAction("Tenant","PM", new
                {
                    page = Request["page"],
                    sortby = Request["sortBy"]
                });

            }


        }

        public ActionResult DeleteInvoice(int id, int tenantid)
        {

            UserBAL objbal = new UserBAL();
            objbal.invoice_delete(id);

            return RedirectToAction("Invoice", "PM", new
            {
                tenantid = tenantid,
                page = Request["page"],
                sortby = Request["sortBy"]
            });



        }
       
        public ActionResult Edit(int id)
        {
            var user = _dbc.tbl_user_master.Where(x => x.int_id == id).FirstOrDefault();

            if (user == null)
            {
                return HttpNotFound();
            }
            var uservm = new UserMasterVM
            {
                int_id = user.int_id,
                int_pin_code = user.int_pin_code,
                str_add_1 = user.str_add_1,
                str_add_2 = user.str_add_2,
                str_city = user.str_city,
                str_comp_name = user.str_comp_name,
                str_contact_name = user.str_contact_name,
                str_country = user.str_country,
                str_password = user.str_password,
                str_state = user.str_state,
                str_user_name = user.str_user_name,
                str_email = user.str_email,

            };

            return View("CreatePropertyManager", uservm);

        }

        [SessionCheck]
        public ActionResult PropertyManager(int? page, string sortBy)
        {
            ViewBag.SortNameParameter = string.IsNullOrEmpty(sortBy) ? "CompName Desc" : "";
            ViewBag.SortContactNameParameter = string.IsNullOrEmpty(sortBy) ? "ContactName Desc" : "";
            ViewBag.SortCityParameter = string.IsNullOrEmpty(sortBy) ? "City Desc" : "";

            var user = _dbc.tbl_user_master.AsQueryable();

            switch (sortBy)
            {
                case "CompName Desc":
                    user = user.OrderByDescending(x => x.str_comp_name);
                    break;

                case "ContactName Desc":
                    user = user.OrderByDescending(x => x.str_contact_name);
                    break;

                case "City Desc":
                    user = user.OrderByDescending(x => x.str_city);
                    break;

                default:
                    user = user.OrderBy(x => x.str_comp_name);
                    break;


            }

            return View(user.Where(x => x.int_user_type_id == 2).ToList().ToPagedList(page ?? 1, CommonCls._pagesize));
        }

        public PartialViewResult PropertyManagerPaging(int? page, string sortBy)
        {
            return PartialView("_PMList", _dbc.tbl_user_master.Where(x => x.int_user_type_id == 2).ToList().ToPagedList(page ?? 1, 2));
        }

        [SessionCheck]
        public ActionResult Tenant(int? page, string sortBy)
        {
            ViewBag.SortNameParameter = string.IsNullOrEmpty(sortBy) ? "CompName Desc" : "";
            ViewBag.SortContactNameParameter = string.IsNullOrEmpty(sortBy) ? "ContactName Desc" : "";
            ViewBag.SortCityParameter = string.IsNullOrEmpty(sortBy) ? "City Desc" : "";

            var user = _dbc.tbl_user_master.AsQueryable();

            switch (sortBy)
            {
                case "CompName Desc":
                    user = user.OrderByDescending(x => x.str_comp_name);
                    break;

                case "ContactName Desc":
                    user = user.OrderByDescending(x => x.str_contact_name);
                    break;

                case "City Desc":
                    user = user.OrderByDescending(x => x.str_city);
                    break;

                default:
                    user = user.OrderBy(x => x.str_comp_name);
                    break;


            }



            return View(user.Where(x => x.int_user_type_id == 3).ToList().ToPagedList(page ?? 1, CommonCls._pagesize));



        }

        [SessionCheck]
        public ActionResult CreatePropertyManager()
        {
            UserMasterVM objvm = new UserMasterVM();
            return View(objvm);
        }
        [HttpPost]
        public ActionResult CreatePropertyManager(UserMasterVM objuser)
        {
            //if (ModelState.IsValid)
            //{
            int _lresult = 0;
            if (objuser.int_id == 0)
            {
                if (_dbc.tbl_user_master.Any(x => x.str_user_name == objuser.str_user_name))
                {
                    ModelState.AddModelError("str_user_name", "User Name Already Present into the system");
                    return View(objuser);
                }
            }
            else
            {
                if (_dbc.tbl_user_master.Any(x => x.str_user_name == objuser.str_user_name && x.int_id != objuser.int_id))
                {
                    ModelState.AddModelError("str_user_name", "User Name Already Present into the system");
                    return View(objuser);
                }
            }
            objuser.int_pm_id = 0;
            objuser.int_user_type_id = 2;
            objuser.int_invoice_period = 0;

            UserBAL objbal = new UserBAL();

            if (objuser.int_id == 0)
                _lresult = objbal.user_insert(objuser);
            else
                _lresult = objbal.user_update(objuser);

            if (_lresult > 0)
            {
                return RedirectToAction("PropertyManager");
            }

            // }
            return View(objuser);
        }

        [SessionCheck]
        public ActionResult CreateTenant()
        {

            int _utype = Convert.ToInt32(Session["utypeid"].ToString());

            TenantVM tenant = new TenantVM();
            tenant.property_manager = _dbc.tbl_user_master.Where(x => x.int_user_type_id == 2).ToList();

            tenant.template = _dbc.tbl_template.ToList();

            var _user = new UserMasterVM();
            _user.int_pm_id = 0;

            if (_utype == CommonCls._usertypePM)
            {
                _user.int_pm_id = Convert.ToInt32(Session["uid"].ToString());
            }

            tenant.user_contact_info = _user;
            tenant.tenantcontract = new TenantContratVM();
            var list = new SelectList(new[]
        {
    new { ID = "1", Name = Resource.weekly },
    new { ID = "2", Name = Resource.day15 },
    new { ID = "3", Name = Resource.monthly },
},
        "ID", "Name", 1);

            ViewData["list"] = list;



            if (_utype == CommonCls._usertypeAdmin)
            {

                return View("CreateTenant", tenant);
            }
            else if (_utype == CommonCls._usertypePM)
            {

                return View("../PM/CreateTenant", tenant);
            }
            return View("CreateTenant", tenant);


        }


        [HttpPost]
        public ActionResult CreateTenantBillingInfo(TenantVM objvm)
        {
            int _lresult = 0;

            try
            {
                if (objvm.int_tenant_id == null)
                    objvm.int_tenant_id = Convert.ToInt32(TempData["_tenantid"]);

                TempData["_tenantid"] = objvm.int_tenant_id;

                TenantBAL objbal = new TenantBAL();
                if (objvm.int_id == 0)
                    _lresult = objbal.tenant_billing_insert(objvm);
                else
                    _lresult = objbal.tenant_billing_update(objvm);


            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return Json(_lresult.ToString(), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult CreateTenantContactInfo(UserMasterVM data)
        {
            int _lresult = 0;


            TenantVM tenant = new TenantVM()
            {
                user_contact_info = data
            };

            try
            {

                Random generator = new Random();
                String u = generator.Next(0, 1000000).ToString("D4");
                
                tenant.user_contact_info.int_user_type_id = 3;
                tenant.user_contact_info.str_user_name = "norberto.sanchez" + u;
                tenant.user_contact_info.str_password = "T3cn0bu1ld1ngs!";
                tenant.user_contact_info.int_invoice_period = 3;

                UserBAL objbal = new UserBAL();

                if (data.int_id == 0)
                    _lresult = objbal.user_insert(tenant.user_contact_info);
                else
                    _lresult = objbal.user_update(tenant.user_contact_info);


                if (_lresult > 0)
                {
                    ViewBag.message = "True";
                    TempData["_tenantid"] = _lresult;
                }
                else
                {
                    ViewBag.message = "False";
                }

            }
            catch (Exception ex)
            {

                log.Error(ex.Message);

            }

            //return View("CreateTenant", tenant);
            return Json(_lresult.ToString(), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult CreateTenantContract(TenantContratVM data)
        {
            int _lresult = 0;

            try
            {
                TenantBAL objbal = new TenantBAL();

                if (data.int_tenant_id == 0)
                    data.int_tenant_id = Convert.ToInt32(TempData["_tenantid"]);


                if (data.int_contract_id == 0)
                    _lresult = objbal.tenant_contract_insert(data);
                else
                    _lresult = objbal.tenant_contract_update(data);


                if (_lresult > 0)
                {
                    ViewBag.message = "True";

                }
                else
                {
                    ViewBag.message = "False";
                }

            }
            catch (Exception ex)
            {

                log.Error(ex.Message);

            }

            //return View("CreateTenant", tenant);
            return Json(_lresult.ToString(), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult SetupTenantEmailInfo(EmailSetupVM objvm)
        {
            int _lresult = 0;

            try
            {
                if (objvm.int_tenant_id == null)
                    objvm.int_tenant_id = Convert.ToInt32(TempData["_tenantid"]);

                TempData["_tenantid"] = objvm.int_tenant_id;

                TenantBAL objbal = new TenantBAL();

                if (objvm.int_email_id == 0)
                    _lresult = objbal.tenant_email_setup_insert(objvm);
                else
                    _lresult = objbal.tenant_email_setup_update(objvm);

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            //return View("CreateTenant", tenant);
            return Json(_lresult.ToString(), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult CreateTenantSetting(TenantSettingVM objvm)
        {
            int _lresult = 0;

            try
            {
                if (objvm.int_tenant_id == null)
                    objvm.int_tenant_id = Convert.ToInt32(TempData["_tenantid"]);

                TempData["_tenantid"] = objvm.int_tenant_id;

                TenantBAL objbal = new TenantBAL();

                if (objvm.int_id == 0)
                    _lresult = objbal.tenant_setting_insert(objvm);
                else
                    _lresult = objbal.tenant_setting_update(objvm);

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            //return View("CreateTenant", tenant);
            return Json(_lresult.ToString(), JsonRequestBehavior.AllowGet);
        }

        [SessionCheck]
        public ActionResult EditTenant(int tenantid)
        {


            var _user = _dbc.tbl_user_master.Where(x => x.int_id == tenantid).SingleOrDefault();
            var _bill = _dbc.tbl_tenant_billing_info.Where(x => x.int_tenant_id == tenantid).SingleOrDefault();
            var _email = _dbc.tbl_tenant_email_setup.Where(x => x.int_tenant_id == tenantid).SingleOrDefault();
            var _setting = _dbc.tbl_tenant_settings.Where(x => x.int_tenant_id == tenantid).SingleOrDefault();

            var _contract = _dbc.tbl_tenant_contract.Where(x => x.int_tenant_id == tenantid).SingleOrDefault();

            UserMasterVM _uservm = new UserMasterVM()
            {

                int_id = _user.int_id,
                int_pin_code = _user.int_pin_code,
                int_pm_id = _user.int_pm_id,
                str_add_1 = _user.str_add_1,
                str_add_2 = _user.str_add_2,
                str_city = _user.str_city,
                str_comp_name = _user.str_comp_name,
                str_contact_name = _user.str_contact_name,
                str_password = _user.str_password,
                str_country = _user.str_country,
                str_state = _user.str_state,
                str_user_name = _user.str_user_name,
                str_email = _user.str_email

            };

            TempData["_tenantid"] = _user.int_id;

            EmailSetupVM _emailvm = null;
            if (_email != null)
            {
                _emailvm = new EmailSetupVM()
                {
                    str_bcc_email = _email.str_bcc_email,
                    str_body = _email.str_body,
                    str_cc_email = _email.str_cc_email,
                    str_from_email = _email.str_from_email,
                    str_subject = _email.str_subject,
                    int_email_id = _email.int_email_id
                };
            }

            TenantContratVM _contractvm = null;
            if (_contract != null)
            {
                _contractvm = new TenantContratVM()
                {
                    e_date = _contract.e_date.Value,
                    s_date = _contract.s_date.Value,
                    int_contract_id = _contract.int_contract_id

                };
            }

            TenantSettingVM _settingvm = null;
            if (_setting != null)
            {
                _settingvm = new TenantSettingVM()
                {
                    dec_demanda_facturable = _setting.dec_demanda_facturable,
                    dec_total_ene = _setting.dec_total_ene,
                    int_id = _setting.int_id,
                };
            }

            TenantVM _tenant = new TenantVM()
            {
                user_contact_info = _uservm,
                emailsetup = _emailvm,
                tenantsetting = _settingvm

            };
            if (_bill != null)
            {
                _tenant.bit_is_consolidate_zone = _bill.bit_is_consolidate_zone;
                _tenant.bit_is_file = _bill.bit_is_file;
                _tenant.bit_is_print = _bill.bit_is_print;
                _tenant.bit_is_seasonal_rate = (bool)_bill.bit_is_seasonal_rate;
                _tenant.bit_is_surchare = (bool)_bill.bit_is_surchare;
                _tenant.dec_rate = _bill.dec_rate.Value.ToString();
                _tenant.dec_surcharge_amt = _bill.dec_surcharge_amt.Value.ToString();
                _tenant.dec_seasonal_multi_rate = _bill.dec_seasonal_multi_rate.ToString();
                _tenant.int_template_id = _bill.int_template_id;
                _tenant.int_type = _bill.int_type;
                _tenant.str_charge_tenant_max = _bill.str_charge_tenant_max;
                _tenant.str_email = _bill.str_email;
                _tenant.str_charge_tenant_min = _bill.str_charge_tenant_min;
                _tenant.str_min_billable_over = _bill.str_min_billable_over;
                _tenant.int_id = _bill.int_id;
            }

            _tenant.property_manager = _dbc.tbl_user_master.Where(x => x.int_user_type_id == 2).ToList();
            _tenant.template = _dbc.tbl_template.ToList();
            _tenant.tenantcontract = _contractvm;

            var list = new SelectList(new[]
        {
    new { ID = "1", Name = Resource.weekly },
    new { ID = "2", Name = Resource.day15 },
    new { ID = "3", Name = Resource.monthly },
},
        "ID", "Name", _user.int_invoice_period);

            ViewData["list"] = list;

            int _utype = Convert.ToInt32(Session["utypeid"].ToString());

            if (_utype == CommonCls._usertypeAdmin)
                return View("CreateTenant", _tenant);
            else if (_utype == CommonCls._usertypePM)
                return View("../PM/CreateTenant", _tenant);

            return View("CreateTenant", _tenant);

        }


        [HttpPost]
        public ActionResult ChangePassword(UserMasterVM objvm)
        {
            int _lVal;
            ViewBag._errorcode = 0;

            try
            {
                UserBAL objbal = new UserBAL();
                _lVal = objbal.user_changepassword(objvm);
                if (_lVal > 0)
                    ViewBag._errorcode = 1;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            int _utypeid = Convert.ToInt32(Session["utypeid"].ToString());

            if (_utypeid == CommonCls._usertypePM)
                return View("../PM/ChangePassword", objvm);
            else if (_utypeid == CommonCls._usertypeAdmin)
                return View("../Admin/ChangePassword", objvm);
            else if (_utypeid == CommonCls._usertypeTenant)
                return View("../Tenant/ChangePassword", objvm);

            return View();

        }

        [SessionCheck]
        [HttpGet] // this action result returns the partial containing the modal
        public ActionResult AssignMeter(int id)
        {
            try
            {

                var _tenant = _dbc.tbl_tenant_contract.Where(x => x.int_tenant_id == id).SingleOrDefault();

                UdisEntities _dbmeter = new UdisEntities();

                var _meter = _dbmeter.UDIS.Select(m => m.CFE_MeterID).Distinct().ToList();

                var _tenantmeter = _dbc.tbl_tenant_meter.Where(x => x.int_tenant_id == id && x.bit_is_assign == true).ToList();

                TenantMeterVM _objvm = new TenantMeterVM();

                _objvm.Meters = getMeters();


                if (_tenantmeter != null)
                {
                    foreach (var meter in _objvm.Meters)
                    {
                        var test = _tenantmeter.Where(x => x.str_meter_id == meter.Text).SingleOrDefault();

                        if (test != null)
                            meter.Selected = true;

                    }

                }



                _objvm.bit_is_assign = true;
                _objvm.int_tenant_id = id;

                ViewBag.messgVal = 2;


                return View(_objvm);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();


        }

        [HttpPost] // this action result returns the partial containing the modal
        public ActionResult AssignMeter(TenantMeterVM _objvm)
        {


            int _lval = 0;
            try
            {
                TenantBAL objbal = new TenantBAL();


                objbal.tenant_delete_meter(_objvm.int_tenant_id);

                foreach (var meterid in _objvm.Meters)
                {
                    if (meterid.Selected)
                    {
                        //int mid = 0;
                        objbal = new TenantBAL();
                        _objvm.bit_is_assign = true;
                        //_objvm.str_meter_id = Int32.TryParse(meterid.Value, out mid) ? Int32.Parse(meterid.Value) : (int?)null;
                        _objvm.str_meter_id = meterid.Text;
                        _objvm.multiplier = 1;
                        _lval = objbal.tenant_meter_insert(_objvm);
                    }

                }

                ViewBag.messgVal = _lval;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return RedirectToAction("Tenant");
        }

        private static List<SelectListItem> getMeters()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            UdisEntities _dbmeter = new UdisEntities();
            var _meter = _dbmeter.UDIS.Select(x => new { x.CFE_MeterID }).Distinct().ToList();
            if (_meter != null)
            {
                foreach (var m in _meter)
                {
                    items.Add(new SelectListItem
                    {
                        Text = m.CFE_MeterID,
                    });
                }
            }

            return items;
        }

        [HttpGet] // this action result returns the partial containing the modal
        public ActionResult BillRate(int id)
        {
            try
            {


                var _billrate = _dbc.tbl_tenant_billing_info.Where(x => x.int_tenant_id == id).SingleOrDefault();

                TenantVM _objvm = new TenantVM();

                if (_billrate != null)
                {
                    _objvm.dec_seasonal_multi_rate = _billrate.dec_seasonal_multi_rate.ToString();
                    _objvm.dec_rate = _billrate.dec_rate.Value.ToString();
                    _objvm.dec_surcharge_amt = _billrate.dec_surcharge_amt.Value.ToString();
                    _objvm.int_id = id;

                }


                return PartialView("_billrate", _objvm);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return PartialView("_billrate");


        }

        [HttpPost] // this action result returns the partial containing the modal
        public ActionResult BillRate(TenantVM objvm)
        {
            int _lval = 0;
            try
            {

                TenantBAL objbal = new TenantBAL();
                _lval = objbal.tenant_bill_rate_update(objvm);

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return Json(_lval.ToString(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet] // this action result returns the partial containing the modal
        public ActionResult BillingHours(int id)
        {
            try
            {

                var _billinghours = _dbc.tbl_pm_billing_hours.Where(x => x.int_pm_id == id).SingleOrDefault();
                PMBillingHoursVM _objvm = new PMBillingHoursVM();


                if (_billinghours != null)
                {
                    _objvm.int_rate_id = _billinghours.int_rate_id;
                    _objvm.str_base_e_time_m = _billinghours.str_base_e_time_m;
                    _objvm.str_base_e_time_sat = _billinghours.str_base_e_time_sat;
                    _objvm.str_base_e_time_sun = _billinghours.str_base_e_time_sun;
                    _objvm.str_base_s_time_m = _billinghours.str_base_s_time_m;

                    _objvm.str_base_s_time_sat = _billinghours.str_base_s_time_sat;
                    _objvm.str_base_s_time_sun = _billinghours.str_base_s_time_sun;
                    _objvm.str_inter_e_time_1_m = _billinghours.str_inter_e_time_1_m;
                    _objvm.str_inter_e_time_2_m = _billinghours.str_inter_e_time_2_m;

                    _objvm.str_inter_e_time_sat = _billinghours.str_inter_e_time_sat;
                    _objvm.str_inter_e_time_sun = _billinghours.str_inter_e_time_sun;
                    _objvm.str_inter_s_time_1_m = _billinghours.str_inter_s_time_1_m;
                    _objvm.str_inter_s_time_2_m = _billinghours.str_inter_s_time_2_m;

                    _objvm.str_peak_s_time_sat = _billinghours.str_peak_s_time_sat;
                    _objvm.str_peak_e_time_sat = _billinghours.str_peak_e_time_sat;

                    _objvm.str_inter_s_time_2_sat = _billinghours.str_inter_s_time_2_sat;
                    _objvm.str_inter_e_time_2_sat = _billinghours.str_inter_e_time_2_sat;

                    _objvm.str_inter_s_time_sat = _billinghours.str_inter_s_time_sat;
                    _objvm.str_inter_s_time_sun = _billinghours.str_inter_s_time_sun;
                    _objvm.str_peak_e_time_m = _billinghours.str_peak_e_time_m;
                    _objvm.str_peak_s_time_m = _billinghours.str_peak_s_time_m;
                }

                _objvm.int_pm_id = id;


                return PartialView("_pmbillinghours", _objvm);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return PartialView("_pmbillinghours");


        }

        [HttpPost] // this action result returns the partial containing the modal
        public ActionResult BillingHours(PMBillingHoursVM objvm)
        {
            int _lVal = 0;
            try
            {
                PMBAL objbal = new PMBAL();

                if (objvm.int_rate_id == 0)
                    _lVal = objbal.pm_insert_billing_hours(objvm);
                else
                    _lVal = objbal.pm_update_billing_hours(objvm);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return Json(_lVal, JsonRequestBehavior.AllowGet);
        }

        [SessionCheck]
        [HttpGet]
        public ActionResult Meters(int? page)
        {
            try
            {

                MeterCLS _meter = new MeterCLS();
                DataSet ds = _meter.getMeter();

                var _meterlist = ds.Tables[0].AsEnumerable().Select(x => new meter { name = x.Field<string>("str_meter_id"), multiplier = x.Field<int>("multiplicador") });


                return View(_meterlist.ToList().ToPagedList(page ?? 1, CommonCls._pagesize));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }

        [HttpGet] // this action result returns the partial containing the modal
        public ActionResult DetachMeter(string id)
        {
            Session["tenant_id"] = id;
            Session["meter_id"] = id;
            return new EmptyResult();
        }

        [HttpPost] // this action result returns the partial containing the modal
        public ActionResult DetachMeterTenant()
        {
            TenantBAL objbal = new TenantBAL();

            //int _meterid = Convert.ToInt32(Session["meter_id"].ToString());
            string _meterid = Session["meter_id"].ToString();
            var _meter = _dbc.tbl_tenant_meter.Where(x => x.str_meter_id == _meterid && x.bit_is_assign == true).SingleOrDefault();
            int _lVal = objbal.tenant_detach_meter_tenant(_meter.int_tenant_id, _meterid);
            return RedirectToAction("Meters");
        }

        [SessionCheck]
        [HttpGet] // this action result returns the partial containing the modal
        public ActionResult AssignMeterToTenant(string id)
        {
            try
            {
                int pmid = Convert.ToInt32(Session["uid"].ToString());


                var _tenant = _dbc.tbl_user_master.Where(x => x.int_user_type_id == 3);

                var _tenantmeter = _dbc.tbl_tenant_meter.Where(x => x.str_meter_id == id & x.bit_is_assign == true).SingleOrDefault();

                TenantMeterVM _objvm = new TenantMeterVM();

                if (_tenantmeter == null)
                {
                    ViewBag.TenantDropDown = new SelectList(_tenant, "int_id", "str_comp_name");
                    _objvm.int_id = 0;

                }
                else
                {
                    ViewBag.TenantDropDown = new SelectList(_tenant, "int_id", "str_comp_name", _tenantmeter.int_tenant_id);
                    _objvm.int_id = _tenantmeter.int_id;
                }

                _objvm.bit_is_assign = true;
                _objvm.str_meter_id = id;



                return PartialView("_assigntenant", _objvm);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return PartialView("_assigntenant");


        }

        [HttpPost] // this action result returns the partial containing the modal
        public ActionResult AssignMeterToTenant(TenantMeterVM _objvm)
        {


            int _lval = 0;
            try
            {
                TenantBAL objbal = new TenantBAL();

                _objvm.bit_is_assign = true;
                _objvm.multiplier = 1;

                _lval = objbal.tenant_meter_insert(_objvm);

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            ViewBag.issuccess = _lval;

            return Json(_lval, JsonRequestBehavior.AllowGet);
        }

        [HttpPost] // this action result returns the partial containing the modal
        public ActionResult ChangeMultiplier(TenantMeterVM _objvm)
        {


            int _lval = 0;
            try
            {
                TenantBAL objbal = new TenantBAL();

                _objvm.bit_is_assign = true;

                _lval = objbal.change_meter_multiplier(_objvm);

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            ViewBag.issuccess = _lval;

            return Json(_lval, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Profile()
        {

            int id = Convert.ToInt32(Session["uid"].ToString());

            var user = _dbc.tbl_user_master.Where(x => x.int_id == id).FirstOrDefault();

            if (user == null)
            {
                return HttpNotFound();
            }
            var uservm = new UserMasterVM
            {
                int_id = user.int_id,
                int_pin_code = user.int_pin_code,
                str_add_1 = user.str_add_1,
                str_add_2 = user.str_add_2,
                str_city = user.str_city,
                str_comp_name = user.str_comp_name,
                str_contact_name = user.str_contact_name,
                str_country = user.str_country,
                str_password = user.str_password,
                str_state = user.str_state,
                str_user_name = user.str_user_name,
                str_email = user.str_email,

            };

            int _utypeid = Convert.ToInt32(Session["utypeid"].ToString());

            if (_utypeid == CommonCls._usertypePM)
                return View("../User/Profile", uservm);
            else if (_utypeid == CommonCls._usertypeAdmin)
                return View("../Admin/Profile", uservm);
            else if (_utypeid == CommonCls._usertypeTenant)
                return View("../Tenant/Profile", uservm);

            return View("Profile", uservm);

        }

        [HttpPost]
        public ActionResult Profile(UserMasterVM objvm)
        {
            int _lVal;
            ViewBag._errorcode = 0;

            try
            {
                UserBAL objbal = new UserBAL();
                _lVal = objbal.user_profile(objvm);
                if (_lVal > 0)
                    ViewBag._errorcode = 1;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            int _utypeid = Convert.ToInt32(Session["utypeid"].ToString());

            if (_utypeid == CommonCls._usertypePM)
                return View("../User/Profile", objvm);
            else if (_utypeid == CommonCls._usertypeAdmin)
                return View("../Admin/Profile", objvm);
            else if (_utypeid == CommonCls._usertypeTenant)
                return View("../Tenant/Profile", objvm);

            return View();

        }



    }
}