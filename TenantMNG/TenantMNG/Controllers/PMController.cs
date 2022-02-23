using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TenantMNG.ADO.NET;
using TenantMNG.BAL;
using TenantMNG.Core;
using TenantMNG.Models;
using TenantMNG.ViewModel;
using PagedList;
using System.Data;
using System.Web.UI.WebControls;
using System.Collections;
using System.Web.Helpers;
using System.Web.Script.Serialization;
using System.Globalization;
using System.Web;
using System.IO;
using System.Web.UI;
using Ionic.Zip;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TenantMNG.Controllers
{
    public class PMController : MyController
    {
        // GET: User
        DB_TenantMNGEntities _dbc = new DB_TenantMNGEntities();
        ILog log = log4net.LogManager.GetLogger(typeof(PMController));
        NiagaraEntities _dmeter = new NiagaraEntities();



        // GET: PM
        [SessionCheck]
        public ActionResult Dashboard()
        {
            int pmid = Convert.ToInt32(Session["uid"].ToString());

            var _user = _dbc.tbl_user_master.Where(x => x.int_user_type_id == 3 && x.int_pm_id == pmid).ToList().Take(5).OrderByDescending(x => x.int_id);

            var _invoice = _dbc.tbl_invoice.Where(x => x.tbl_user_master.int_pm_id == pmid).ToList().Take(5).OrderByDescending(x => x.int_invoice_id);

            PMDashboardVM _objvm = new PMDashboardVM();

            _objvm.tbl_invoice = _invoice.ToList();

            _objvm.tbl_user_master = _user.ToList();

            bindDropBox();
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

        [SessionCheck]
        public ActionResult Tenant(int? page)
        {
            int pmid = Convert.ToInt32(Session["uid"].ToString());
            return View(_dbc.tbl_user_master.Where(x => x.int_user_type_id == 3 && x.int_pm_id == pmid).ToList().ToPagedList(page ?? 1, CommonCls._pagesize));
        }

        [SessionCheck]
        [HttpGet] // this action result returns the partial containing the modal
        public ActionResult AssignMeter(int id)
        {
            try
            {

                var _tenant = _dbc.tbl_tenant_contract.Where(x => x.int_tenant_id == id).SingleOrDefault();

                //if (_tenant == null)
                //    return Json("1", JsonRequestBehavior.AllowGet);


                NiagaraEntities _dbmeter = new NiagaraEntities();

                var _meter = _dbmeter.HISTORY_CONFIG.ToList();

                var _tenantmeter = _dbc.tbl_tenant_meter.Where(x => x.int_tenant_id == id && x.bit_is_assign == true).ToList();




                TenantMeterVM _objvm = new TenantMeterVM();

                _objvm.Meters = getMeters();


                if (_tenantmeter != null)
                {
                    foreach (var meter in _objvm.Meters)
                    {
                        var test = _tenantmeter.Where(x => x.int_meter_id.Value.ToString() == meter.Value).SingleOrDefault();

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
                        int mid = 0;
                        objbal = new TenantBAL();
                        _objvm.bit_is_assign = true;
                        _objvm.int_meter_id = Int32.TryParse(meterid.Value, out mid) ? Int32.Parse(meterid.Value) : (int?)null;
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

        [HttpGet] // this action result returns the partial containing the modal
        public ActionResult DetachMeter(int id)
        {
            Session["tenant_id"] = id;
            Session["meter_id"] = id;
            return new EmptyResult();
        }

        [HttpPost] // this action result returns the partial containing the modal
        public ActionResult DetachMeter()
        {
            TenantBAL objbal = new TenantBAL();

            int _id = Convert.ToInt32(Session["tenant_id"].ToString());

            int _lVal = objbal.tenant_detach_meter(_id);


            return RedirectToAction("Tenant");
        }

        [HttpPost] // this action result returns the partial containing the modal
        public ActionResult DetachMeterTenant()
        {
            TenantBAL objbal = new TenantBAL();

            int _meterid = Convert.ToInt32(Session["meter_id"].ToString());
            var _meter = _dbc.tbl_tenant_meter.Where(x => x.int_meter_id == _meterid && x.bit_is_assign == true).SingleOrDefault();
            int _lVal = objbal.tenant_detach_meter_tenant(_meter.int_tenant_id, _meterid);
            return RedirectToAction("Meters");
        }

        
        [HttpGet]
        public ActionResult CreateInvoice(string tenantid)
        {
            InvoiceVM _objvm = new InvoiceVM();
            try
            {
                int id = Convert.ToInt32(tenantid);
                var _tenantmeter = _dbc.tbl_tenant_meter.Where(x => x.int_tenant_id == id && x.bit_is_assign == true).FirstOrDefault();
                if (_tenantmeter != null)
                {

                    _objvm.int_invoice_id = 0;

                    var _tenantbillinginfo = _dbc.tbl_tenant_billing_info.Where(x => x.int_tenant_id == id).SingleOrDefault();

                    if (_tenantbillinginfo != null)
                    {
                        _objvm.dec_peak_energy_rate = _tenantbillinginfo.dec_seasonal_multi_rate;
                        _objvm.dec_inter_energy_rate = _tenantbillinginfo.dec_surcharge_amt;
                        _objvm.dec_base_rate = _tenantbillinginfo.dec_rate;
                    }
                    int _uid = Convert.ToInt32(Session["uid"].ToString());
                    var _pmbillingtime = _dbc.tbl_pm_billing_hours.Where(x => x.int_pm_id == _uid).SingleOrDefault();
                    if (_pmbillingtime != null)
                    {

                        _objvm.str_inter_e_time = _pmbillingtime.str_inter_e_time_1_m;
                        _objvm.str_inter_s_time = _pmbillingtime.str_inter_s_time_2_m;

                        _objvm.str_peak_e_time = _pmbillingtime.str_peak_e_time_m;
                        _objvm.str_peak_s_time = _pmbillingtime.str_peak_s_time_m;

                        _objvm.int_invoice_period = _tenantbillinginfo.tbl_user_master.int_invoice_period;
                    }

                    var _pmsetting = _dbc.tbl_tenant_settings.Where(x => x.int_tenant_id == id).SingleOrDefault();

                    if (_pmsetting != null)
                    {
                        _objvm.dec_demanda_facturable = _pmsetting.dec_demanda_facturable;
                        _objvm.dec_total_ene = _pmsetting.dec_total_ene;
                    }
                    else
                    {
                        _objvm.dec_demanda_facturable = 0;
                        _objvm.dec_total_ene = 0;
                    }

                }
                ViewBag._erromsg = -1;
                _objvm.int_tenant_id = id;
                return View(_objvm);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateInvoice(InvoiceVM objvm)
        {
            int _lVal = 0;
            ViewBag._erromsg = 0;
            try
            {
                TenantBAL objbal = new TenantBAL();

                objvm.dec_prev_inter_energy = 0;
                objvm.dec_prev_peak_energy = 0;

                if (objvm.int_invoice_id == 0)
                {
                    objvm.bit_tenant_active = true;
                    objvm.bit_is_editable = true;


                    _lVal = objbal.tenant_invoice_insert(objvm);
                }
                else
                {
                    _lVal = objbal.tenant_invoice_update(objvm);
                }

                if (_lVal > 0)
                {
                    DataTable dt = (DataTable)Session["tenantDT"];
                    objvm = new InvoiceVM();
                    objbal = new TenantBAL();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        objvm.int_invoice_id = _lVal;
                        objvm.dec_base_amt = Convert.ToDecimal(dt.Rows[i]["dec_base_amt"].ToString());
                        objvm.dec_base_energy = Convert.ToDecimal(dt.Rows[i]["dec_base_energy"].ToString());
                        objvm.dec_base_rate = Convert.ToDecimal(dt.Rows[i]["dec_base_rate"].ToString());

                        objvm.dec_inter_energy = Convert.ToDecimal(dt.Rows[i]["dec_inter_energy"].ToString());
                        objvm.dec_inter_energy_amt = Convert.ToDecimal(dt.Rows[i]["dec_inter_energy_amt"].ToString());
                        objvm.dec_inter_energy_rate = Convert.ToDecimal(dt.Rows[i]["dec_inter_energy_rate"].ToString());

                        objvm.dec_peak_energy = Convert.ToDecimal(dt.Rows[i]["dec_peak_energy"].ToString());
                        objvm.dec_peak_energy_amt = Convert.ToDecimal(dt.Rows[i]["dec_peak_energy_amt"].ToString());
                        objvm.dec_peak_energy_rate = Convert.ToDecimal(dt.Rows[i]["dec_peak_energy_rate"].ToString());
                        objvm.int_meter_id = Convert.ToInt32(dt.Rows[i]["int_meter_id"].ToString());

                        objbal.tenant_invoice_details_insert(objvm);
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            ViewBag._erromsg = _lVal;
            return View();
        }

      
        [HttpGet]
        public ActionResult CreateMultipleInvoice(int tenantid)
        {
            try
            {
                string _meter = CommonCls.getMeterName(tenantid);

                var _tenantinvoice = _dbc.tbl_invoice.Where(x => x.int_tenant_id == tenantid).OrderByDescending(x => x.int_invoice_id).FirstOrDefault();

                DateTime? _sdate = null;
                decimal? _last_peak_energy = 0, _last_inter_energy = 0;
                int? _period = 0;

                if (_tenantinvoice != null)
                {
                    _sdate = _tenantinvoice.date_e_bill_date.Value.AddDays(1);
                    //_last_inter_energy = _tenantinvoice.dec_inter_energy;
                    //_last_peak_energy = _tenantinvoice.dec_peak_energy;
                    _period = _tenantinvoice.tbl_user_master.int_invoice_period;
                }
                else
                {
                    var _tenantcontract = _dbc.tbl_tenant_contract.Where(x => x.int_tenant_id == tenantid).SingleOrDefault();

                    if (_tenantcontract != null)
                    {
                        _sdate = _tenantcontract.s_date;
                        _period = _tenantcontract.tbl_user_master.int_invoice_period;
                    }

                }

                int _number = 0;
                if (_sdate != null)
                    _number = getNumbers(_sdate, _period);


                int _uid = Convert.ToInt32(Session["uid"].ToString());

                var _pmbillingtime = _dbc.tbl_pm_billing_hours.Where(x => x.int_pm_id == _uid).SingleOrDefault();

                var _tenantbillinginfo = _dbc.tbl_tenant_billing_info.Where(x => x.int_tenant_id == tenantid).SingleOrDefault();

                TenantEnergyVM objvam = new TenantEnergyVM();
                objvam.int_tenant_id = tenantid;
                objvam.str_meter_name = _meter;
                objvam.int_invoice_id = _number;
                objvam.str_inter_s_time = _sdate.Value.ToString("yyyy/MM/dd");
                objvam.int_meter_id = _period;
                objvam.str_peak_e_time = _pmbillingtime.str_peak_e_time_m;
                objvam.str_peak_s_time = _pmbillingtime.str_peak_s_time_m;
                objvam.dec_inter_energy_rate = _tenantbillinginfo.dec_surcharge_amt.Value;
                objvam.dec_peak_energy_rate = _tenantbillinginfo.dec_seasonal_multi_rate;
                objvam.dec_base_rate = _tenantbillinginfo.dec_rate.Value;

                ViewBag.callfunction = "call";

                return View(objvam);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }

        public int getNumbers(DateTime? e_date, int? period)
        {

            int preioddays = 0;

            if (period == 1)
                preioddays = 7;
            else if (period == 2)
                preioddays = 15;
            else if (period == 3)
                preioddays = 30;

            double days = (System.DateTime.Now - e_date.Value).TotalDays;

            int _result = 0;

            _result = (int)days / preioddays;

            if (days % preioddays > 0)
                return _result - 1;
            else
                return _result;
        }

        [HttpPost]
        public string getEnerge(TenantEnergyVM objvm)
        {
            string _energy = "0";
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string json = string.Empty;
            string _meterid = "", _metername = "";
            DataTable dt = new DataTable();
            try
            {

                DB_TenantMNGEntities _dbctenant = new DB_TenantMNGEntities();


                var _tenantmeter = _dbctenant.tbl_tenant_meter.Where(x => x.int_tenant_id == objvm.int_tenant_id).ToList();

                if (_tenantmeter != null && _tenantmeter.Count > 0)
                {
                    foreach (var m in _tenantmeter)
                    {
                        _meterid = _meterid + "," + m.int_meter_id.ToString();

                    }
                }

                NiagaraEntities _dbc = new NiagaraEntities();

                string[] _meteridarray = _meterid.ToString().Trim(',').Split(',');


                foreach (var m in _meteridarray)
                {
                    var _tenantmetername = _dbc.HISTORY_CONFIG.Where(x => x.ID.ToString() == m).SingleOrDefault();

                    _metername = _metername + "," + _tenantmetername.TABLE_NAME;
                }



                dt.Columns.AddRange(new DataColumn[11] { new DataColumn("metername", typeof(string)),
                            new DataColumn("dec_peak_energy", typeof(decimal)),
                            new DataColumn("dec_peak_energy_rate",typeof(decimal)),
                            new DataColumn("dec_peak_energy_amt",typeof(decimal)),
                            new DataColumn("dec_inter_energy",typeof(decimal)),
                            new DataColumn("dec_inter_energy_rate",typeof(decimal)),
                            new DataColumn("dec_inter_energy_amt",typeof(decimal)),
                            new DataColumn("dec_base_energy",typeof(decimal)),
                            new DataColumn("dec_base_rate",typeof(decimal)),
                            new DataColumn("dec_base_amt",typeof(decimal)),
                            new DataColumn("int_meter_id",typeof(int)),});



                if (string.IsNullOrEmpty(_metername) == false)
                {



                    var _hours = (from t1 in _dbctenant.tbl_pm_billing_hours
                                  let t2s = from t2 in _dbctenant.tbl_user_master
                                            where t2.int_id == objvm.int_tenant_id
                                            select t2.int_pm_id

                                  where t2s.Contains(t1.int_pm_id)

                                  select t1).FirstOrDefault();

                    MeterCLS obj = new MeterCLS();


                    string[] _mname = _metername.Trim(',').Split(',');

                    int i = 0;
                    foreach (var m in _mname)
                    {

                        DataSet _ds = obj.getTenantEnergy(m, objvm.date_s_bill_date.ToString(), objvm.date_e_bill_date.ToString(), _hours.str_peak_s_time_m,
                        _hours.str_peak_e_time_m, _hours.str_inter_s_time_1_m, _hours.str_inter_e_time_1_m, _hours.str_inter_s_time_2_m, _hours.str_inter_e_time_2_m,
                        _hours.str_base_s_time_m, _hours.str_base_e_time_m, _hours.str_base_s_time_sat, _hours.str_base_e_time_sat, _hours.str_inter_s_time_sat,
                        _hours.str_inter_e_time_sat, _hours.str_base_s_time_sun, _hours.str_base_e_time_sun, _hours.str_inter_s_time_sun, _hours.str_inter_e_time_sun);

                        if (_ds.Tables[0].Rows.Count > 0)
                        {
                            decimal peckenergy = Convert.ToDecimal(_ds.Tables[0].Rows[0][0].ToString());
                            decimal interenergy = Convert.ToDecimal(_ds.Tables[2].Rows[0][0].ToString());
                            decimal baseenergy = Convert.ToDecimal(_ds.Tables[1].Rows[0][0].ToString());

                            dt.Rows.Add(m, peckenergy, objvm.dec_peak_energy_rate, (peckenergy * objvm.dec_peak_energy_rate), interenergy,
                                objvm.dec_inter_energy_rate, (interenergy * objvm.dec_inter_energy_rate), baseenergy, objvm.dec_base_rate, (baseenergy * objvm.dec_base_rate), Convert.ToInt32(_meteridarray[i]));
                        }

                        i++;

                    }

                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }

            Session["tenantDT"] = dt;

            return serializer.Serialize(rows);
            //return Json(dt, JsonRequestBehavior.AllowGet);
        }

      
        [HttpGet]
        public ActionResult Invoice(int tenantid, int? page, string sortBy)
        {
            try
            {

                ViewBag.SortDateParameter = sortBy == "Date" ? "Date Desc" : "Date";
                ViewBag.SortPeackEnergyParameter = sortBy == "PE" ? "PE Desc" : "PE";
                ViewBag.SortIntermediateEnergyParameter = sortBy == "IE" ? "IE Desc" : "IE";
                ViewBag.SortAmountParameter = sortBy == "Amount" ? "Amount Desc" : "Amount";
                ViewBag.SortCustomChargeParameter = sortBy == "CC" ? "CC Desc" : "CC";


                var _invoice = _dbc.tbl_invoice.AsQueryable();

                _invoice = _invoice.Where(x => x.int_tenant_id == tenantid);

                switch (sortBy)
                {
                    case "Date Desc":
                        _invoice = _invoice.OrderByDescending(x => x.date_invoice_date);
                        break;

                    case "Date":
                        _invoice = _invoice.OrderBy(x => x.date_invoice_date);
                        break;



                    case "Amount Desc":
                        _invoice = _invoice.OrderByDescending(x => x.dec_total);
                        break;

                    case "Amount":
                        _invoice = _invoice.OrderBy(x => x.dec_total);
                        break;

                    case "CC Desc":
                        _invoice = _invoice.OrderByDescending(x => x.dec_custome_charges);
                        break;

                    case "CC":
                        _invoice = _invoice.OrderBy(x => x.dec_custome_charges);
                        break;


                    default:
                        _invoice = _invoice.OrderByDescending(x => x.date_invoice_date);
                        break;


                }

                return View(_invoice.ToList().ToPagedList(page ?? 1, CommonCls._pagesize));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }


        [SessionCheck]
        [HttpGet]
        public ActionResult TenantActivity(string int_id, int? page, string sortBy, string s_date, string e_date)
        {
            try
            {

                ViewBag.SortDateParameter = sortBy == "Date" ? "Date Desc" : "Date";
                ViewBag.SortPeackEnergyParameter = sortBy == "PE" ? "PE Desc" : "PE";
                ViewBag.SortIntermediateEnergyParameter = sortBy == "IE" ? "IE Desc" : "IE";
                ViewBag.SortAmountParameter = sortBy == "Amount" ? "Amount Desc" : "Amount";


                var _invoice = _dbc.tbl_invoice.AsQueryable();

                if (!string.IsNullOrEmpty(int_id))
                {
                    int _id = Convert.ToInt32(int_id);
                    _invoice = _invoice.Where(x => x.int_tenant_id == _id);
                }

                if (!string.IsNullOrEmpty(s_date) && !string.IsNullOrEmpty(e_date))
                {
                    DateTime _sdate = Convert.ToDateTime(s_date);
                    DateTime _edate = Convert.ToDateTime(e_date);

                    _invoice = _invoice.Where(x => x.date_invoice_date >= _sdate && x.date_invoice_date <= _edate);
                }
                switch (sortBy)
                {
                    case "Date Desc":
                        _invoice = _invoice.OrderByDescending(x => x.date_invoice_date);
                        break;

                    case "Date":
                        _invoice = _invoice.OrderBy(x => x.date_invoice_date);
                        break;

                    case "Amount Desc":
                        _invoice = _invoice.OrderByDescending(x => x.dec_total);
                        break;

                    case "Amount":
                        _invoice = _invoice.OrderBy(x => x.dec_total);
                        break;


                    default:
                        _invoice = _invoice.OrderByDescending(x => x.date_invoice_date);
                        break;
                }

                Session["_exportData"] = _invoice.ToList();
                if (TempData["modeldisplay"] != null)
                    ViewBag.Message = TempData["modeldisplay"].ToString();

                if (!string.IsNullOrEmpty(int_id) || !string.IsNullOrEmpty(s_date) || !string.IsNullOrEmpty(e_date))
                    ViewBag._iscleardisplay = true;



                bindDropBox();

                return View(_invoice.ToList().ToPagedList(page ?? 1, CommonCls._pagesize));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }

        public void bindmeterDropBox()
        {
            int meterid = Convert.ToInt32(Session["uid"].ToString());
            NiagaraEntities _dbmeter = new NiagaraEntities();

            var _meter = _dbmeter.HISTORY_CONFIG.Where(x => x.ID == x.ID).ToList();


            ViewBag.meterDropDown = new SelectList(_meter, "ID", "TABLE_NAME");
        }


        public void bindDropBox()
        {
            int _pm_id = Convert.ToInt32(Session["uid"].ToString());

            var _tenant = _dbc.tbl_user_master.Where(x => x.int_pm_id == _pm_id).ToList();

            ViewBag.TenantDropDown = new SelectList(_tenant, "int_id", "str_comp_name");
        }


        public ActionResult EditInvoice(int id)
        {
            var _invoice = _dbc.tbl_invoice.Where(x => x.int_invoice_id == id).FirstOrDefault();

            InvoiceVM _objvm = new InvoiceVM();

            if (_invoice != null)
            {

                _objvm.int_invoice_id = _invoice.int_invoice_id;
                _objvm.bit_tenant_active = _invoice.bit_tenant_active;
                _objvm.date_e_bill_date = _invoice.date_e_bill_date;
                _objvm.date_s_bill_date = _invoice.date_s_bill_date;
                _objvm.dec_custome_charges = _invoice.dec_custome_charges;
                _objvm.dec_demad = _invoice.dec_demad;

                _objvm.dec_tax_amt = _invoice.dec_tax_amt;
                _objvm.dec_total = _invoice.dec_total;

                _objvm.dec_current_inter_energy = _invoice.dec_current_inter_energy;
                _objvm.dec_current_peak_energy = _invoice.dec_current_peack_energy;
                _objvm.dec_prev_inter_energy = _invoice.dec_prev_inter_energy;
                _objvm.dec_prev_peak_energy = _invoice.dec_prev_peack_energy;
                _objvm.date_pay_date = _invoice.date_pay_date;


                _objvm.dec_total_ene = _invoice.dec_total_ene;
                _objvm.dec_demanda_facturable = _invoice.dec_demanda_facturable;
                _objvm.dec_demanda_facturable_amount = _invoice.dec_demanda_facturable_amount;



                _objvm.int_tenant_id = _invoice.int_tenant_id;
            }
            return View("CreateInvoice", _objvm);
        }


        public ActionResult SummaryExportToExcel(int? page)
        {
            var gv = new GridView();
            IList<tbl_invoice> list = (IList<tbl_invoice>)Session["_exportData"];

            try
            {
                DataTable dt = new DataTable();
                if (list.Count > 0)
                {

                    dt.Columns.AddRange(new DataColumn[9] {
                            new DataColumn("Tenant", typeof(string)),
                            new DataColumn("Meter Name", typeof(string)),
                            new DataColumn("Bill Period", typeof(string)),
                            new DataColumn("Peak Energy",typeof(decimal)),
                            new DataColumn("Intermediate Energy", typeof(decimal)),
                            new DataColumn("Custome Charge",typeof(decimal)),
                            new DataColumn("Demand",typeof(decimal)),
                            new DataColumn("Tax",typeof(decimal)),
                            new DataColumn("Total Amount",typeof(decimal))

                    });

                    for (int i = 0; i < list.Count; i++)
                    {
                        dt.Rows.Add(list[i].tbl_user_master.str_comp_name,
                            @TenantMNG.Core.CommonCls.getMeterNamefromId(list[i].tbl_invoice_details.ElementAt(0).int_meter_id),
                           //list[i]@TenantMNG.Core.CommonCls.getMeterNamefromId(dt.meterid).getMeterName(int tenantid),
                           list[i].date_s_bill_date.Value.ToString("dd/MM/yyyy") + " To " + list[i].date_e_bill_date.Value.ToString("dd/MM/yyyy"),
                           string.Format("{0:0.00}", list[i].tbl_invoice_details.ElementAt(0).dec_peak_energy),
                          string.Format("{0:0.00}", list[i].tbl_invoice_details.ElementAt(0).dec_inter_energy),
                           string.Format("{0:0.00}", list[i].dec_custome_charges),
                          string.Format("{0:0.00}", list[i].dec_demad),
                           string.Format("{0:0.00}", list[i].dec_tax_amt),
                            string.Format("{0:0.00}", list[i].dec_total));


                    }
                }


                gv.DataSource = dt;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=DemoExcel.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();


                //Response.Charset = "utf-8";
                //Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1250");
                ////sets font
                //Response.Write("<font style='font-size:12.0pt; font-family:Arial;'>");
                //Response.Write("<BR><BR><BR>");
                ////sets the table border, cell spacing, border color, font of the text, background, foreground, font height
                //Response.Write("<Table border='1' bgColor='#ffffff' " +
                //  "borderColor='#000000' cellSpacing='0' cellPadding='0' " +
                //  "style='font-size:10.0pt; font-family:Calibri; background:white;'><tr ><td colspan=8 style='font-size:18.0pt; font-family:Arial;'><b> Resumen de Facturacion de Inquilinos</b> </td></tr> " +
                //  "<tr ><td colspan=8 style='font-size:10.0pt; font-family:Arial;'>&nbsp;Dirección de Facturación: " + Session["address"].ToString() + " </td></tr> " +
                //  "<tr ><td colspan=8 style='font-size:10.0pt; font-family:Arial;'>&nbsp; </td></tr><TR >");
                ////am getting my grid's column headers
                //int columnscount = dt.Columns.Count;

                //for (int j = 0; j < columnscount; j++)
                //{      //write in new column
                //    Response.Write("<Td style='font-size:10.0pt; font-family:Arial;background-color:#5e78a3;color:white'>");
                //    //Get column headers  and make it as bold in excel columns
                //    Response.Write("<B>");
                //    Response.Write(dt.Columns[j].ToString());
                //    Response.Write("</B>");
                //    Response.Write("</Td>");
                //}
                //Response.Write("</TR>");
                //foreach (DataRow row in dt.Rows)
                //{//write in new row
                //    Response.Write("<TR>");
                //    for (int i = 0; i < dt.Columns.Count; i++)
                //    {
                //        Response.Write("<Td>");
                //        Response.Write(row[i].ToString());
                //        Response.Write("</Td>");
                //    }

                //    Response.Write("</TR>");
                //}
                //Response.Write("</Table>");
                //Response.Write("</font>");
                //Response.Flush();
                //Response.End();

                bindDropBox();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return View("TenantActivity", list.ToList().ToPagedList(page ?? 1, CommonCls._pagesize));
        }
        // PreviewMeterExcel
        public ActionResult PreviewExportToExcel(int? page)
        {
            var gv = new GridView();
            IList<tbl_invoice> list = (IList<tbl_invoice>)Session["_exportData"];

            try
            {
                DataTable dt = new DataTable();

                if (list.Count > 0)
                {

                    dt.Columns.AddRange(new DataColumn[11] {
                            new DataColumn("Bill Date", typeof(string)),
                            new DataColumn("Metering Point", typeof(string)),
                            new DataColumn("Tenant", typeof(string)),
                             new DataColumn("Tenant Satus",typeof(string)),
                             new DataColumn("Bill Period",typeof(string)),
                            //new DataColumn("Base Energy",typeof(decimal)),
                           /* new DataColumn("Peak Energy", typeof(decimal))*/
                            new DataColumn("Energy(kWh/Lectura)", typeof(decimal)),
                            new DataColumn("Personalizods custome charges",typeof(decimal)),
                            new DataColumn("Tarifa intermedia Rate",typeof(decimal)),
                             new DataColumn("Gas",typeof(decimal)),
                             new DataColumn("Agua Water",typeof(decimal)),
                              new DataColumn("Total Amount",typeof(decimal))

                    });

                    for (int i = 0; i < list.Count; i++)
                    {

                        dt.Rows.Add(list[i].date_invoice_date,
                              null,
                               //@TenantMNG.Core.CommonCls.getMeterNamefromId(list[i].tbl_invoice_details.ElementAt(0).int_meter_id),
                               list[i].tbl_user_master.str_comp_name,
                              list[i].bit_tenant_active == true ? "Active" : "InActive",
                            list[i].date_s_bill_date.Value.ToString("dd/MM/yyyy") + " To " + list[i].date_e_bill_date.Value.ToString("dd/MM/yyyy"),
                           //string.Format("{0:0.00}", list[i].tbl_invoice_details.ElementAt(0).dec_base_energy),
                           string.Format("{0:0.00}", list[i].tbl_invoice_details.ElementAt(0).dec_peak_energy),
                           null,
                           null,
                           null,
                           null,
                           //string.Format("{0:0.00}", list[i].tbl_invoice_details.ElementAt(0).dec_inter_energy),
                           //string.Format("{0:0.00}", list[i].dec_demanda_facturable),
                           string.Format("{0:0.00}", list[i].dec_total_ene));
                    }


                }


                gv.DataSource = dt;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=DemoExcel.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();

                bindDropBox();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return View("TenantActivity", list.ToList().ToPagedList(page ?? 1, CommonCls._pagesize));
        }

        public ActionResult ExportToExcel(int? page)
        {
            var gv = new GridView();
            IList<tbl_invoice> list = (IList<tbl_invoice>)Session["_exportData"];

            try
            {
                DataTable dt = new DataTable();
                if (list.Count > 0)
                {

                    dt.Columns.AddRange(new DataColumn[6] {
                            new DataColumn("Fecha de Factura", typeof(string)),
                            new DataColumn("Inquilino", typeof(string)),
                            new DataColumn("Nombre del Medidor", typeof(string)),
                            new DataColumn("Periodo de Facturacion",typeof(string)),
                            new DataColumn("Impuesto",typeof(decimal)),
                            new DataColumn("Total",typeof(decimal))


                    });

                    for (int i = 0; i < list.Count; i++)
                    {
                        dt.Rows.Add(list[i].date_invoice_date.Value.ToString("dd/MM/yyyy"),
                            list[i].tbl_user_master.str_comp_name,
                            list[i].bit_tenant_active,
                            list[i].date_s_bill_date.Value.ToString("dd/MM/yyyy") + " To " + list[i].date_e_bill_date.Value.ToString("dd/MM/yyyy"),
                            string.Format("{0:0.00}", list[i].dec_tax_amt),
                            string.Format("{0:0.00}", list[i].dec_total));

                    }
                }


                gv.DataSource = dt;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=DemoExcel.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                bindDropBox();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return View("PreviewMeter", list.ToList().ToPagedList(page ?? 1, CommonCls._pagesize));
        }
        //MultiSelect Download
        public ActionResult GenerateMultiPDF(string forpdf)
        {

            var pdid = forpdf.Split(',');
            for (int i = 0; i < pdid.Length - 1; i++)
            {

                biData(int.Parse(pdid[i]));

            }

            TempData["modeldisplay"] = "Yes";

            return RedirectToAction("TenantActivity");
        }

        public ActionResult GeneratePDF(int id)
        {

            bindData(id);

            TempData["modeldisplay"] = "Yes";

            return RedirectToAction("TenantActivity");
        }

        //MultiDownload 
        [HttpPost]
        public ActionResult MultiDownload(string forpdf)
        {


            var pdid = forpdf.Split(',');
            for (int i = 0; i < pdid.Length - 1; i++)
            {

                biData(int.Parse(pdid[i]));
                // DownloadbyID(int.Parse(pdid[i]));
                DownloadPdf(int.Parse(pdid[i]));


            }
            return RedirectToAction("TenantActivity");
        }

        [HttpPost]
        public string DownloadFiles(string forpdf)
        {


            string fileName = "Invoice_" + forpdf + ".pdf";
            string ZipFilePath = Server.MapPath("~/PDF/Zip/");
            System.Random rand = new System.Random((int)System.DateTime.Now.Ticks);
            int random = rand.Next(1, 100000000);
            string ZipFileName = "Invoices" + random.ToString() + ".zip";

            var pdid = forpdf.Split(',');

            using (ZipFile zip = new ZipFile())
            {
                for (int i = 0; i < pdid.Length - 1; i++)
                {

                    biData(int.Parse(pdid[i]));


                    fileName = "Invoice_" + pdid[i] + ".pdf";
                    //string filenamepath = Server.MapPath("~/PDF/" + fileName);
                    var saveZipFilePath = Server.MapPath("~/PDF/Zip/" + fileName);
                    zip.AddFile(saveZipFilePath, "files");
                }

                zip.Save(ZipFilePath + $@"\{ZipFileName}");


            }

            var fullPath = ZipFilePath + $@"\{ZipFileName}";


            return ZipFileName;

        }

        public FileResult DownloadPdf(int forpdf)
        {
            string fileName = "Invoice_" + forpdf + ".pdf";
            string filenamepath = Server.MapPath("~/PDF/" + fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filenamepath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            //var result = new FileContentResult(System.IO.File.ReadAllBytes(filenamepath), "application/pdf")
            //{
            //    FileDownloadName = fileName
            //};

            //return result;
        }

        //get oerder multiple invoice report
        private void biData(int forpdf)
        {
            try
            {

                Session["invid"] = forpdf;
                string path = Server.MapPath("~/Template/NInvoice.html");
                string html = System.IO.File.ReadAllText(path);
                string filename = "Invoice_" + forpdf + ".PDF";

                path = HttpContext.Server.MapPath("~/PDF/Zip/");

                string cdnFilePath = path + filename;
                DataSet ds = new DataSet();
                string ConnectionString = ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("[spProcEnergyLog]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    List<ProEnergyLog> userlist = new List<ProEnergyLog>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        ProEnergyLog uobj = new ProEnergyLog();
                        uobj.TENETID = int.Parse(ds.Tables[0].Rows[i]["TENETID"].ToString());
                        uobj.TenentName = Convert.ToString(ds.Tables[0].Rows[i]["COMPNAME"]);
                        uobj.TABLE_NAME = Convert.ToString(ds.Tables[0].Rows[i]["TABLE_NAME"]);
                        uobj.TIMESTAMP = Convert.ToDateTime(ds.Tables[0].Rows[i]["TIMESTAMP"].ToString());
                        uobj.VALUE = Convert.ToDecimal(ds.Tables[0].Rows[i]["VALUE"].ToString());
                        userlist.Add(uobj);
                    }

                    var _invoice = _dbc.tbl_invoice.Where(x => x.int_invoice_id == forpdf).FirstOrDefault();
                    var objuser = userlist.Where(x => x.TENETID == _invoice.int_tenant_id).ToList();
                    var chartgroup = from ins in objuser
                                     group ins by ins.TABLE_NAME into empg
                                     select new
                                     {
                                         Name = empg.Key,
                                         maxObject = empg.OrderByDescending(item => item.VALUE).First(),
                                         minObject = empg.OrderBy(item => item.VALUE).Skip(2).First(),

                                     };
                    var objuser1 = chartgroup.ToList();
                    var a = Convert.ToInt32(objuser1.ElementAt(0).maxObject.VALUE) - Convert.ToInt32(objuser1.ElementAt(0).minObject.VALUE);
                    TimeSpan timeSpan = _invoice.date_e_bill_date.Value.Subtract(_invoice.date_s_bill_date.Value);

                    if (_invoice != null)
                    {
                        CharterColumn(_invoice.int_tenant_id, forpdf);
                        //string address = _invoice.tbl_user_master.str_add_1 + "<br>"+"<br>"+"<br>" + _invoice.tbl_user_master.str_add_2 + "<br>"+ "<br>"+"<br>"+ _invoice.tbl_user_master.str_city + "<br>"+ "<br>"+"<br>" + _invoice.tbl_user_master.str_state + "<br>" +"<br>"+ "<br>" + _invoice.tbl_user_master.str_country;
                        //html = html.Replace("#address", address);
                        html = html.Replace("#edate", _invoice.date_e_bill_date.Value.ToString("dd/MM/yyyy"));
                        html = html.Replace("#sdate", _invoice.date_s_bill_date.Value.ToString("dd/MM/yyyy"));
                        html = html.Replace("#Statement", _invoice.date_pay_date.Value.ToString("dd/MM/yyyy"));
                        html = html.Replace("#Statementmonth", _invoice.date_pay_date.Value.Month.ToString());
                        html = html.Replace("#Tenantname", _invoice.tbl_user_master.str_contact_name);
                        html = html.Replace("#TenantAdd1", _invoice.tbl_user_master.str_add_1);
                        html = html.Replace("#TenantAdd2", _invoice.tbl_user_master.str_add_2);
                        html = html.Replace("#TCity", _invoice.tbl_user_master.str_city);
                        html = html.Replace("#TS", _invoice.tbl_user_master.str_state);
                        html = html.Replace("#TC", _invoice.tbl_user_master.str_country);
                        html = html.Replace("#VaueMax", string.Format("{0:0}", objuser1.ElementAt(0).maxObject.VALUE));
                        html = html.Replace("#VaueMin", string.Format("{0:0 }", objuser1.ElementAt(0).minObject.VALUE));
                        html = html.Replace("#usese", a.ToString());
                        html = html.Replace("#days", _invoice.date_e_bill_date.Value.Date.Subtract(_invoice.date_s_bill_date.Value.Date).TotalDays.ToString());
                        html = html.Replace("#energyusage1", CommonCls.DoFormat(_invoice.tbl_invoice_details.ElementAt(0).dec_peak_energy) + " kWh");
                        html = html.Replace("#energyrate1", CommonCls.DoFormat(_invoice.tbl_invoice_details.ElementAt(0).dec_peak_energy_rate));
                        html = html.Replace("#energyamt1", CommonCls.DoFormat(_invoice.tbl_invoice_details.ElementAt(0).dec_peak_energy * _invoice.tbl_invoice_details.ElementAt(0).dec_peak_energy_rate));
                        html = html.Replace("#energyusage2", CommonCls.DoFormat(_invoice.tbl_invoice_details.ElementAt(0).dec_inter_energy) + " kWh");
                        html = html.Replace("#energyrate2", CommonCls.DoFormat(_invoice.tbl_invoice_details.ElementAt(0).dec_inter_energy_rate));

                        html = html.Replace("#energyamt2", CommonCls.DoFormat(_invoice.tbl_invoice_details.ElementAt(0).dec_inter_energy * _invoice.tbl_invoice_details.ElementAt(0).dec_inter_energy_rate));

                        html = html.Replace("#energyusage3", CommonCls.DoFormat(_invoice.tbl_invoice_details.ElementAt(0).dec_base_energy) + " kWh");
                        html = html.Replace("#energyrate3", CommonCls.DoFormat(_invoice.tbl_invoice_details.ElementAt(0).dec_base_rate));
                        html = html.Replace("#energyamt3", CommonCls.DoFormat(_invoice.tbl_invoice_details.ElementAt(0).dec_base_energy * _invoice.tbl_invoice_details.ElementAt(0).dec_base_rate));

                        html = html.Replace("#metername", CommonCls.getMeterNamefromId(_invoice.tbl_invoice_details.ElementAt(0).int_meter_id));
                        html = html.Replace("#name", _invoice.tbl_user_master.str_comp_name);
                        html = html.Replace("#billperiod", _invoice.date_s_bill_date.Value.ToString("dd/MM/yyyy") + " AL " + _invoice.date_e_bill_date.Value.ToString("dd/MM/yyyy"));
                        html = html.Replace("#billdate", _invoice.date_invoice_date.Value.ToString("dd/MM/yyyy"));
                        html = html.Replace("#paydate", _invoice.date_pay_date.Value.ToString("dd/MM/yyyy"));
                        html = html.Replace("#invno", _invoice.int_invoice_id.ToString());
                        html = html.Replace("#customcharge", "$" + CommonCls.DoFormat(_invoice.dec_custome_charges));
                        html = html.Replace("#customtitle", string.IsNullOrEmpty(_invoice.str_custome_charge_desc) ? "Cargo personalizado" : _invoice.str_custome_charge_desc);
                        html = html.Replace("#opingpeakkw", CommonCls.DoFormat(_invoice.dec_prev_peack_energy) + " kWh");
                        html = html.Replace("#closingpeakkw", CommonCls.DoFormat(_invoice.dec_current_peack_energy) + " kWh");
                        html = html.Replace("#opingintermediatekWh", CommonCls.DoFormat(_invoice.dec_prev_peack_energy) + " kWh");
                        html = html.Replace("#closingintermediatekWh", CommonCls.DoFormat(_invoice.dec_current_inter_energy) + " kWh");
                        html = html.Replace("#energytype1", "Peak Energy");
                        html = html.Replace("#demandtier1", "Tier 1");
                        html = html.Replace("#demandusage", CommonCls.DoFormat(_invoice.dec_demad) + " kWh");
                        html = html.Replace("#demandrate", "$0");
                        html = html.Replace("#demandamt", "$0");

                        html = html.Replace("#chargedesc1", _invoice.str_custome_charge_desc);
                        html = html.Replace("#chargetype1", "Flat Type");
                        html = html.Replace("#customeunit1", "$" + CommonCls.DoFormat(_invoice.dec_custome_charges));
                        html = html.Replace("#customerate1", "$0");
                        html = html.Replace("#customamt1", "$" + CommonCls.DoFormat(_invoice.dec_custome_charges));

                        //   html = html.Replace("#chargedesc2", "IVA 16%");
                        html = html.Replace("#chargetype2", "Percentage");
                        html = html.Replace("#customamt2", "$" + CommonCls.DoFormat(_invoice.dec_tax_amt));
                        html = html.Replace("#charturl", "I");


                        //var _setting = _dbc.tbl_tenant_settings.Where(x => x.int_tenant_id == _invoice.int_tenant_id).FirstOrDefault();
                        //decimal _totalfacturable = 0;
                        //if (_setting != null)
                        //{
                        html = html.Replace("#demanfacturabledamt", "$" + _invoice.dec_demanda_facturable.ToString());
                        html = html.Replace("#buildingtotal", "$" + _invoice.dec_total_ene.ToString());
                        html = html.Replace("#demandfinalamt", "$" + CommonCls.DoFormat(_invoice.dec_demanda_facturable_amount));


                        //decimal _finaltotal = _totalenergyamt + _totalfacturable + _invoice.dec_tax_amt.Value;


                        html = html.Replace("#totalamt", "$" + CommonCls.DoFormat(_invoice.dec_total));

                        path = Server.MapPath("~/Template/meterlist.html");
                        string replacemeterlist = string.Empty;
                        string meterlisthtml = string.Empty;

                        decimal _totalenergyamt = 0, _totalenergy = 0;

                        var _invoicedetails = _dbc.tbl_invoice_details.Where(x => x.int_invoice_id == forpdf).ToList();

                        if (_invoicedetails != null)
                        {

                            foreach (var details in _invoicedetails)
                            {
                                meterlisthtml = System.IO.File.ReadAllText(path);

                                meterlisthtml = meterlisthtml.Replace("#metername", CommonCls.getMeterNamefromId(details.int_meter_id));

                                meterlisthtml = meterlisthtml.Replace("#energyusage1", CommonCls.DoFormat(details.dec_peak_energy) + " kWh");
                                meterlisthtml = meterlisthtml.Replace("#energyrate1", details.dec_peak_energy_rate.ToString());
                                meterlisthtml = meterlisthtml.Replace("#energyamt1", CommonCls.DoFormat(details.dec_peak_energy_amt));
                                meterlisthtml = meterlisthtml.Replace("#energyusage2", CommonCls.DoFormat(details.dec_inter_energy) + " kWh");
                                meterlisthtml = meterlisthtml.Replace("#energyrate2", details.dec_inter_energy_rate.ToString());
                                meterlisthtml = meterlisthtml.Replace("#energyamt2", CommonCls.DoFormat(details.dec_inter_energy_amt));
                                meterlisthtml = meterlisthtml.Replace("#energyusage3", CommonCls.DoFormat(details.dec_base_energy) + " kWh");
                                meterlisthtml = meterlisthtml.Replace("#energyrate3", details.dec_base_rate.ToString());
                                meterlisthtml = meterlisthtml.Replace("#energyamt3", CommonCls.DoFormat(details.dec_base_amt));
                                meterlisthtml = meterlisthtml.Replace("#totaluse", CommonCls.DoFormat(details.dec_peak_energy.Value + details.dec_inter_energy.Value + details.dec_base_energy.Value));
                                meterlisthtml = meterlisthtml.Replace("#toalamt", CommonCls.DoFormat(details.dec_peak_energy_amt.Value + details.dec_inter_energy_amt.Value + details.dec_base_amt.Value));
                                replacemeterlist = replacemeterlist + "<br/>" + meterlisthtml;
                                _totalenergyamt = _totalenergyamt + (details.dec_peak_energy_amt.Value + details.dec_inter_energy_amt.Value + details.dec_base_amt.Value);
                                _totalenergy = _totalenergy + (details.dec_peak_energy.Value + details.dec_inter_energy.Value + details.dec_base_energy.Value);

                            }
                        }

                        html = html.Replace("#meterid", replacemeterlist);

                        html = html.Replace("#sumofamount", "$" + CommonCls.DoFormat(_totalenergyamt));

                        html = html.Replace("#totalenergy", CommonCls.DoFormat(_totalenergy) + " kWh");

                    }

                    path = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Content("~/");


                    html = html.Replace("#FooterImg", path + "/PDF/charts.jpg");
                    html = html.Replace("#place", path + "/PDF/place.png");
                    html = html.Replace("#hlogo", path + "/PDF/hlogo.png");
                    html = html.Replace("#mail", path + "/PDF/mail.png");
                    html = html.Replace("#web", path + "/PDF/web.png");
                    html = html.Replace("#fone", path + "/PDF/fone.png");
                    html = html.Replace("#graph", path + "/PDF/graph.png");
                    html = html.Replace("#qr", path + "/PDF/qr.png");
                    html = html.Replace("#barimg", path + "/PDF/barimg.png");
                    html = html.Replace("#barc", path + "/PDF/barcode.jpg");

                    if (System.IO.File.Exists(cdnFilePath))
                    {
                        System.IO.File.Delete(cdnFilePath);
                    }

                (new NReco.PdfGenerator.HtmlToPdfConverter()).GeneratePdf(html.ToString(), null, cdnFilePath);

                }

            }
            catch (Exception ex)
            {

                log.Error(ex.Message.ToString());
            }

        }

        //get order inoice report 
        private void bindData(int id)
        {

            try
            {

                Session["invid"] = id;
                string path = Server.MapPath("~/Template/NInvoice.html");
                string html = System.IO.File.ReadAllText(path);
                string filename = "Invoice_" + id.ToString() + ".PDF";

                path = HttpContext.Server.MapPath("~/PDF/");
                string cdnFilePath = path + filename;




                DataSet ds = new DataSet();
                string ConnectionString = ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("[spProcEnergyLog]", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    List<ProEnergyLog> userlist = new List<ProEnergyLog>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        ProEnergyLog uobj = new ProEnergyLog();
                        uobj.TENETID = int.Parse(ds.Tables[0].Rows[i]["TENETID"].ToString());
                        uobj.TenentName = Convert.ToString(ds.Tables[0].Rows[i]["COMPNAME"]);
                        uobj.TABLE_NAME = Convert.ToString(ds.Tables[0].Rows[i]["TABLE_NAME"]);
                        uobj.TIMESTAMP = Convert.ToDateTime(ds.Tables[0].Rows[i]["TIMESTAMP"].ToString());
                        uobj.VALUE = Convert.ToDecimal(ds.Tables[0].Rows[i]["VALUE"].ToString());
                        userlist.Add(uobj);
                    }
                    var _invoice = _dbc.tbl_invoice.Where(x => x.int_invoice_id == id).FirstOrDefault();
                    var objuser = userlist.Where(x => x.TENETID == _invoice.int_tenant_id).ToList();
                    var chartgroup = from ins in objuser
                                     group ins by ins.TABLE_NAME into empg
                                     select new
                                     {
                                         Name = empg.Key,
                                         maxObject = empg.OrderByDescending(item => item.VALUE).First(),
                                         minObject = empg.OrderBy(item => item.VALUE).Skip(2).First(),

                                     };
                    var objuser1 = chartgroup.ToList();
                    //   var a = Convert.ToInt32(objuser1.ElementAt(0).maxObject.VALUE) - Convert.ToInt32(objuser1.ElementAt(0).minObject.VALUE);
                    TimeSpan timeSpan = _invoice.date_e_bill_date.Value.Subtract(_invoice.date_s_bill_date.Value);
                    if (_invoice != null)
                    {

                        #region  Dynamic table
                        var _invoicedet = objuser1.ToList();
                        var allmet = "";
                        if (_invoicedet != null)
                        {

                            foreach (var abcd in _invoicedet)
                            {
                                var a = Convert.ToInt32(abcd.maxObject.VALUE) - Convert.ToInt32(abcd.minObject.VALUE);
                                allmet += "<tr><td style ='font-size:10px;text-align:center;'colspan='4'>" + abcd.Name + "</td>" +
                                  "<td style='font-size:10px; text-align:center;'colspan='4'>" + string.Format("{0:0}", abcd.maxObject.VALUE) + "</td>" +
                                  "<td style ='font-size:10px; text-align:center;'colspan='4'>" + string.Format("{0:0 }", abcd.minObject.VALUE) + "</td>" +
                                  "<td style ='font-size:10px; text-align:center;'colspan='4'>" + a.ToString() + "</td>" +
                                  "<td style ='font-size:10px; text-align:center;'colspan='4'>" + "KWH" + "</td></tr>";

                            }
                        }
                        html = html.Replace("#read", allmet);

                        #endregion

                        CharterColumn(_invoice.int_tenant_id, id);
                        string address = _invoice.tbl_user_master.str_add_1 + "<br>" + _invoice.tbl_user_master.str_add_2 + "<br>" + _invoice.tbl_user_master.str_city + "<br>" + _invoice.tbl_user_master.str_state + "<br>" + _invoice.tbl_user_master.str_country;
                        html = html.Replace("#address", address);
                        html = html.Replace("#edate", _invoice.date_e_bill_date.Value.ToString("dd/MM/yyyy"));
                        html = html.Replace("#sdate", _invoice.date_s_bill_date.Value.ToString("dd/MM/yyyy"));
                        html = html.Replace("#Statement", _invoice.date_pay_date.Value.ToString("dd/MM/yyyy"));
                        html = html.Replace("#monthname", _invoice.date_pay_date.Value.ToString("MMM"));
                        html = html.Replace("#Tenantname", _invoice.tbl_user_master.str_contact_name);
                        html = html.Replace("#VaueMax", string.Format("{0:0}", objuser1.ElementAt(0).maxObject.VALUE));
                        html = html.Replace("#VaueMin", string.Format("{0:0 }", objuser1.ElementAt(0).minObject.VALUE));
                        //html = html.Replace("#usese", a.ToString());
                        //html = html.Replace("#VaueMax",objuser.ToString());
                        //html = html.Replace("#VaueMin", objuser1.ToString());
                        html = html.Replace("#TenantAdd1", _invoice.tbl_user_master.str_add_1);
                        html = html.Replace("#TenantAdd2", _invoice.tbl_user_master.str_add_2);
                        html = html.Replace("#TCity", _invoice.tbl_user_master.str_city);
                        html = html.Replace("#TS", _invoice.tbl_user_master.str_state);
                        html = html.Replace("#TC", _invoice.tbl_user_master.str_country);
                        html = html.Replace("#days", _invoice.date_e_bill_date.Value.Date.Subtract(_invoice.date_s_bill_date.Value.Date).TotalDays.ToString());
                        html = html.Replace("#energyusage1", CommonCls.DoFormat(_invoice.tbl_invoice_details.ElementAt(0).dec_peak_energy) + " kWh");
                        html = html.Replace("#energyrate1", CommonCls.DoFormat(_invoice.tbl_invoice_details.ElementAt(0).dec_peak_energy_rate));
                        html = html.Replace("#energyamt1", CommonCls.DoFormat(_invoice.tbl_invoice_details.ElementAt(0).dec_peak_energy * _invoice.tbl_invoice_details.ElementAt(0).dec_peak_energy_rate));
                        html = html.Replace("#energyusage2", CommonCls.DoFormat(_invoice.tbl_invoice_details.ElementAt(0).dec_inter_energy) + " kWh");
                        html = html.Replace("#energyrate2", CommonCls.DoFormat(_invoice.tbl_invoice_details.ElementAt(0).dec_inter_energy_rate));

                        html = html.Replace("#energyamt2", CommonCls.DoFormat(_invoice.tbl_invoice_details.ElementAt(0).dec_inter_energy * _invoice.tbl_invoice_details.ElementAt(0).dec_inter_energy_rate));
                        html = html.Replace("#meternam", CommonCls.getMeterNamefromId(_invoice.tbl_invoice_details.ElementAt(0).int_meter_id));
                        html = html.Replace("#energyusage3", CommonCls.DoFormat(_invoice.tbl_invoice_details.ElementAt(0).dec_base_energy) + " kWh");
                        html = html.Replace("#energyrate3", CommonCls.DoFormat(_invoice.tbl_invoice_details.ElementAt(0).dec_base_rate));
                        html = html.Replace("#energyamt3", CommonCls.DoFormat(_invoice.tbl_invoice_details.ElementAt(0).dec_base_energy * _invoice.tbl_invoice_details.ElementAt(0).dec_base_rate));

                        html = html.Replace("#name", _invoice.tbl_user_master.str_comp_name);
                        html = html.Replace("#billperiod", _invoice.date_s_bill_date.Value.ToString("dd/MM/yyyy") + " AL " + _invoice.date_e_bill_date.Value.ToString("dd/MM/yyyy"));
                        html = html.Replace("#billdate", _invoice.date_invoice_date.Value.ToString("dd/MM/yyyy"));
                        html = html.Replace("#paydate", _invoice.date_pay_date.Value.ToString("dd/MM/yyyy"));
                        html = html.Replace("#invno", _invoice.int_invoice_id.ToString());
                        html = html.Replace("#customcharge", "$" + CommonCls.DoFormat(_invoice.dec_custome_charges));
                        html = html.Replace("#customtitle", string.IsNullOrEmpty(_invoice.str_custome_charge_desc) ? "Cargo personalizado" : _invoice.str_custome_charge_desc);

                        html = html.Replace("#opingpeakkw", CommonCls.DoFormat(_invoice.dec_prev_peack_energy) + " kWh");
                        html = html.Replace("#closingpeakkw", CommonCls.DoFormat(_invoice.dec_current_peack_energy) + " kWh");
                        html = html.Replace("#opingintermediatekWh", CommonCls.DoFormat(_invoice.dec_prev_peack_energy) + " kWh");
                        html = html.Replace("#closingintermediatekWh", CommonCls.DoFormat(_invoice.dec_current_inter_energy) + " kWh");
                        html = html.Replace("#energytype1", "Peak Energy");

                        html = html.Replace("#demandtier1", "Tier 1");
                        html = html.Replace("#demandusage", CommonCls.DoFormat(_invoice.dec_demad) + " kWh");
                        html = html.Replace("#demandrate", "$0");
                        html = html.Replace("#demandamt", "$0");

                        html = html.Replace("#chargedesc1", _invoice.str_custome_charge_desc);
                        html = html.Replace("#chargetype1", "Flat Type");
                        html = html.Replace("#customeunit1", "$" + CommonCls.DoFormat(_invoice.dec_custome_charges));
                        html = html.Replace("#customerate1", "$0");
                        html = html.Replace("#customamt1", "$" + CommonCls.DoFormat(_invoice.dec_custome_charges));

                        //html = html.Replace("#chargedesc2", "IVA 16%");
                        html = html.Replace("#chargetype2", "Percentage");
                        html = html.Replace("#customamt2", "$" + CommonCls.DoFormat(_invoice.dec_tax_amt));
                        html = html.Replace("#charturl", "I");
                        html = html.Replace("#demanfacturabledamt", "$" + _invoice.dec_demanda_facturable.ToString());
                        html = html.Replace("#buildingtotal", "$" + _invoice.dec_total_ene.ToString());
                        html = html.Replace("#demandfinalamt", "$" + CommonCls.DoFormat(_invoice.dec_demanda_facturable_amount));

                        //decimal _finaltotal = _totalenergyamt + _totalfacturable + _invoice.dec_tax_amt.Value;

                        html = html.Replace("#totalamt", "$" + CommonCls.DoFormat(_invoice.dec_total));

                        path = Server.MapPath("~/Template/meterlist.html");
                        string replacemeterlist = string.Empty;
                        string meterlisthtml = string.Empty;

                        decimal _totalenergyamt = 0, _totalenergy = 0, _totalwithtax = 0; ;

                        var _invoicedetails = _dbc.tbl_invoice_details.Where(x => x.int_invoice_id == id).ToList();
                        var invoicecarge = "";
                        if (_invoicedetails != null)
                        {

                            foreach (var details in _invoicedetails)
                            {
                                meterlisthtml = System.IO.File.ReadAllText(path);

                                meterlisthtml = meterlisthtml.Replace("#metername", CommonCls.getMeterNamefromId(details.int_meter_id));
                                //html = html.Replace("#edate", _invoice.date_e_bill_date.Value.ToString("dd/MM/yyyy"));
                                meterlisthtml = meterlisthtml.Replace("#energyusage1", CommonCls.DoFormat(details.dec_peak_energy) + " kWh");
                                meterlisthtml = meterlisthtml.Replace("#energyrate1", details.dec_peak_energy_rate.ToString());
                                meterlisthtml = meterlisthtml.Replace("#energyamt1", CommonCls.DoFormat(details.dec_peak_energy_amt));
                                meterlisthtml = meterlisthtml.Replace("#energyusage2", CommonCls.DoFormat(details.dec_inter_energy) + " kWh");
                                meterlisthtml = meterlisthtml.Replace("#energyrate2", details.dec_inter_energy_rate.ToString());
                                meterlisthtml = meterlisthtml.Replace("#energyamt2", CommonCls.DoFormat(details.dec_inter_energy_amt));
                                meterlisthtml = meterlisthtml.Replace("#energyusage3", CommonCls.DoFormat(details.dec_base_energy) + " kWh");
                                meterlisthtml = meterlisthtml.Replace("#energyrate3", details.dec_base_rate.ToString());
                                meterlisthtml = meterlisthtml.Replace("#energyamt3", CommonCls.DoFormat(details.dec_base_amt));
                                meterlisthtml = meterlisthtml.Replace("#totaluse", CommonCls.DoFormat(details.dec_peak_energy.Value + details.dec_inter_energy.Value + details.dec_base_energy.Value));
                                meterlisthtml = meterlisthtml.Replace("#toalamt", CommonCls.DoFormat(details.dec_peak_energy_amt.Value + details.dec_inter_energy_amt.Value + details.dec_base_amt.Value));
                                replacemeterlist = replacemeterlist + "<br/>" + meterlisthtml;
                                _totalenergyamt = _totalenergyamt + (details.dec_peak_energy_amt.Value + details.dec_inter_energy_amt.Value + details.dec_base_amt.Value);
                                _totalenergy = _totalenergy + (details.dec_peak_energy.Value + details.dec_inter_energy.Value + details.dec_base_energy.Value);

                            }
                        }

                        html = html.Replace("#meteridd", replacemeterlist);

                        html = html.Replace("#sumofamount", "$" + CommonCls.DoFormat(_totalenergyamt));
                        //  html = html.Replace("#customamt1", "$" + CommonCls.DoFormat(_invoice.dec_demanda_facturable));
                        html = html.Replace("#Charge5", "$" + CommonCls.DoFormat(5 * _totalenergyamt / 100));
                        html = html.Replace("#chargedesc2", "16");
                        html = html.Replace("#customamt3", "$" + CommonCls.DoFormat(_invoice.dec_tax_amt));
                        html = html.Replace("#totalenergy", CommonCls.DoFormat(_totalenergy) + " kWh");
                        _totalwithtax = _totalwithtax + (Convert.ToDecimal(_totalenergyamt) + _invoice.dec_tax_amt.Value + _invoice.dec_custome_charges + (5 * _totalenergyamt / 100) + 16);
                        //  html = html.Replace("#ToPayAmount", CommonCls.DoFormat(5 *_totalenergyamt/100)+_invoice.dec_demanda_facturable + _totalenergyamt + "16");

                        html = html.Replace("#ToPayAmount", "$" + CommonCls.DoFormat(_totalwithtax));
                        path = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Content("~/");

                        html = html.Replace("#FooterImg", path + "/PDF/charts.jpg");
                        html = html.Replace("#place", path + "/PDF/place.png");
                        html = html.Replace("#hlogo", path + "/PDF/hlogo.png");
                        html = html.Replace("#mail", path + "/PDF/mail.png");
                        html = html.Replace("#web", path + "/PDF/web.png");
                        html = html.Replace("#fone", path + "/PDF/fone.png");
                        html = html.Replace("#graph", path + "/PDF/graph.png");
                        html = html.Replace("#qr", path + "/PDF/qr.png");
                        html = html.Replace("#barimg", path + "/PDF/barimg.png");
                        html = html.Replace("#barc", path + "/PDF/barcode.jpg");


                        if (System.IO.File.Exists(cdnFilePath))
                        {
                            System.IO.File.Delete(cdnFilePath);
                        }

                    (new NReco.PdfGenerator.HtmlToPdfConverter()).GeneratePdf(html.ToString(), null, cdnFilePath);



                    }
                }
            }

            catch (Exception ex)
            {

                log.Error(ex.Message.ToString());
            }

        }

        [SessionCheck]
        [HttpGet]
        public ActionResult Alarm(int? page, string sortBy)
        {
            try
            {

                ViewBag.SortDateParameter = sortBy == "Date" ? "Date Desc" : "Date";
                ViewBag.SortPeackEnergyParameter = sortBy == "PE" ? "PE Desc" : "PE";
                ViewBag.SortIntermediateEnergyParameter = sortBy == "IE" ? "IE Desc" : "IE";
                ViewBag.SortAmountParameter = sortBy == "Amount" ? "Amount Desc" : "Amount";
                ViewBag.SortCustomChargeParameter = sortBy == "CC" ? "CC Desc" : "CC";


                var _invoice = _dbc.tbl_invoice.AsQueryable();
                int _pmid = Convert.ToInt32(Session["uid"].ToString());

                _invoice = _invoice.Where(x => x.tbl_user_master.int_pm_id == _pmid && x.date_pay_date < System.DateTime.Now);

                switch (sortBy)
                {
                    case "Date Desc":
                        _invoice = _invoice.OrderByDescending(x => x.date_invoice_date);
                        break;

                    case "Date":
                        _invoice = _invoice.OrderBy(x => x.date_invoice_date);
                        break;

                    //case "PE Desc":
                    //    _invoice = _invoice.OrderByDescending(x => x.dec_peak_energy);
                    //    break;

                    //case "PE":
                    //    _invoice = _invoice.OrderBy(x => x.dec_peak_energy);
                    //    break;

                    //case "IE Desc":
                    //    _invoice = _invoice.OrderByDescending(x => x.dec_inter_energy);
                    //    break;

                    //case "IE":
                    //    _invoice = _invoice.OrderBy(x => x.dec_inter_energy);
                    //    break;

                    case "Amount Desc":
                        _invoice = _invoice.OrderByDescending(x => x.dec_total);
                        break;

                    case "Amount":
                        _invoice = _invoice.OrderBy(x => x.dec_total);
                        break;

                    case "CC Desc":
                        _invoice = _invoice.OrderByDescending(x => x.dec_custome_charges);
                        break;

                    case "CC":
                        _invoice = _invoice.OrderBy(x => x.dec_custome_charges);
                        break;


                    default:
                        _invoice = _invoice.OrderByDescending(x => x.date_invoice_date);
                        break;


                }

                return View(_invoice.ToList().ToPagedList(page ?? 1, CommonCls._pagesize));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }

        public void CharterColumn(int? tenantid, int invoiceid)
        {


            ArrayList xValue = new ArrayList();
            ArrayList yValue = new ArrayList();

            var res = _dbc.tbl_invoice_details.Where(x => x.int_invoice_id == invoiceid).ToList();
            var results = (from c in _dbc.tbl_invoice where c.int_tenant_id == tenantid && c.int_invoice_id <= invoiceid orderby c.int_invoice_id descending select c).ToList();
            var data = res.ToList();
            foreach (var Details in data)
            {

                var interenergy = Details.dec_inter_energy;
                var baseenergy = Details.dec_base_energy;
                var penergy = Details.dec_peak_energy;
                var penergy1 = "Punta";
                var baseenergy1 = "Intermedia";
                var interenergy1 = "Base";


                yValue.Add(interenergy);
                yValue.Add(baseenergy);
                yValue.Add(penergy);
                xValue.Add(penergy1);
                xValue.Add(baseenergy1);
                xValue.Add(interenergy1);
            }
            var a = xValue;
            var b = yValue;
            string _path = Server.MapPath("~/PDF/charts.jpg");
            var chart = new Chart(width: 400, height: 300, theme: ChartTheme.Vanilla)
            .AddSeries(
            name: "Employee",
           xValue: a,
           yValues: b)
           .Save(_path, "jpg");
        }



        public void PdfCharterColumn(int? tenantid, int invoiceid)
        {


            ArrayList xValue = new ArrayList();
            ArrayList yValue = new ArrayList();

            var results = (from c in _dbc.tbl_invoice where c.int_tenant_id == tenantid && c.int_invoice_id <= invoiceid orderby c.int_invoice_id descending select c).ToList().Take(5);
            results.ToList().ForEach(rs => xValue.Add(rs.date_invoice_date.Value.ToString("MMM")));
            results.ToList().ForEach(rs => yValue.Add(rs.dec_total));

            string _path = Server.MapPath("~/PDF/chart.jpg");

            new Chart(width: 600, height: 300, theme: ChartTheme.Blue)
            .AddTitle(Resource.prev_chart)
           .AddSeries("Default", chartType: "column", xValue: xValue, yValues: yValue)
                  .Save(_path, "jpg");


        }

        public FileResult Download()
        {


            try
            {
                string fileName = "Invoice_" + Session["invid"].ToString() + ".pdf";
                string filenamepath = Server.MapPath("~/PDF/" + fileName);
                byte[] fileBytes = System.IO.File.ReadAllBytes(filenamepath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception ex)
            {
                return null;

            }



        }
        public FileResult DownloadbyID(int forpdf)
        {

            string fileName = "Invoice_" + forpdf + ".pdf";
            if (fileName != null)
            {
                try
                {
                    string filenamepath = Server.MapPath("~/PDF/" + fileName);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filenamepath);

                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return null;
        }

        [SessionCheck]
        [HttpGet]
        public ActionResult Meters(int? page)
        {
            try
            {


                MeterCLS _meter = new MeterCLS();
                DataSet ds = _meter.getMeter();

                var _meterlist = ds.Tables[0].AsEnumerable().Select(x => new meter { ID = x.Field<int>("ID"), name = x.Field<string>("TABLE_NAME") });

                return View(_meterlist.ToList().ToPagedList(page ?? 1, CommonCls._pagesize));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }

        [SessionCheck]
        [HttpGet] // this action result returns the partial containing the modal
        public ActionResult AssignMeterToTenant(int id)
        {
            try
            {
                int pmid = Convert.ToInt32(Session["uid"].ToString());


                var _tenant = _dbc.tbl_user_master.Where(x => x.int_user_type_id == 3 && x.int_pm_id == pmid);

                var _tenantmeter = _dbc.tbl_tenant_meter.Where(x => x.int_meter_id == id & x.bit_is_assign == true).SingleOrDefault();

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
                _objvm.int_meter_id = id;



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

                _lval = objbal.tenant_meter_insert(_objvm);

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            ViewBag.issuccess = _lval;

            return Json(_lval, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddTextbox(string[] fromdate, string[] todate, string[] paydate, string[] custome, string[] demand, TenantEnergyVM objvm)
        {
            ViewBag.flag = "True";
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                ViewBag.Values = serializer.Serialize(fromdate);

                string message = "";
                MeterCLS obj;
                DataSet _ds = null;
                InvoiceVM objinv;

                DB_TenantMNGEntities _dbctenant = new DB_TenantMNGEntities();

                var _hours = (from t1 in _dbctenant.tbl_pm_billing_hours
                              let t2s = from t2 in _dbctenant.tbl_user_master
                                        where t2.int_id == objvm.int_tenant_id
                                        select t2.int_pm_id

                              where t2s.Contains(t1.int_pm_id)

                              select t1).FirstOrDefault();





                for (int i = 0; i <= fromdate.Length - 1; i++)
                {
                    obj = new MeterCLS();

                    //_ds = obj.getTenantEnergy(objvm.str_meter_name, fromdate[i], todate[i], objvm.str_peak_s_time,
                    //   objvm.str_peak_e_time);

                    _ds = obj.getTenantEnergy(objvm.str_meter_name, fromdate[i], todate[i], _hours.str_peak_s_time_m,
                  _hours.str_peak_e_time_m, _hours.str_inter_s_time_1_m, _hours.str_inter_e_time_1_m, _hours.str_inter_s_time_2_m, _hours.str_inter_e_time_2_m,
                  _hours.str_base_s_time_m, _hours.str_base_e_time_m, _hours.str_base_s_time_sat, _hours.str_base_e_time_sat, _hours.str_inter_s_time_sat,
                  _hours.str_inter_e_time_sat, _hours.str_base_s_time_sun, _hours.str_base_e_time_sun, _hours.str_inter_s_time_sun, _hours.str_inter_e_time_sun);

                    string _energy = getPickandIntermediatEnergy(objvm.int_tenant_id, _ds);
                    message = _energy + "\n";

                    objinv = new InvoiceVM();

                    objinv.bit_is_editable = true;
                    objinv.bit_tenant_active = true;
                    objinv.date_e_bill_date = Convert.ToDateTime(todate[i]);
                    objinv.date_invoice_date = System.DateTime.Now;
                    objinv.date_pay_date = Convert.ToDateTime(paydate[i]);
                    objinv.date_s_bill_date = Convert.ToDateTime(fromdate[i]);
                    objinv.dec_current_inter_energy = Convert.ToDecimal(_energy.Split(',')[1]);
                    objinv.dec_current_peak_energy = Convert.ToDecimal(_energy.Split(',')[0]);
                    objinv.dec_custome_charges = Convert.ToDecimal(string.IsNullOrEmpty(custome[i]) ? "0" : custome[i]);
                    objinv.dec_demad = Convert.ToDecimal(string.IsNullOrEmpty(demand[i]) ? "0" : demand[i]);

                    objinv.dec_prev_inter_energy = 0;
                    objinv.dec_prev_peak_energy = 0;



                    // decimal total = (objinv.dec_peak_energy.Value * objinv.dec_peak_energy_rate.Value) + (objinv.dec_inter_energy.Value * objinv.dec_inter_energy_rate.Value);
                    decimal total = 1;
                    decimal txtamt = (total * 16) / 100;



                    objinv.dec_tax_amt = txtamt;
                    objinv.dec_total = total + txtamt;
                    //objinv.int_meter_id = objvm.int_meter_id;
                    objinv.int_tenant_id = objvm.int_tenant_id;

                    TenantBAL objbal = new TenantBAL();

                    objbal.tenant_invoice_insert(objinv);


                }


                return View("CreateMultipleInvoice");
            }
            catch (Exception ex)
            {
                ViewBag.flag = "False";
                log.Error(ex.Message);
            }
            return View("CreateMultipleInvoice");
        }


        public string getPickandIntermediatEnergy(int tenantid, DataSet ds)
        {

            var _tenant = _dbc.tbl_user_master.Where(x => x.int_id == tenantid).FirstOrDefault();
            string _energy = "0,0,0";

            if (_tenant != null)
            {
                decimal pickenergy = Convert.ToDecimal(ds.Tables[0].Rows[0][0].ToString());
                decimal interenergy = Convert.ToDecimal(ds.Tables[2].Rows[0][0].ToString());
                decimal baseprice = Convert.ToDecimal(ds.Tables[1].Rows[0][0].ToString());


                _energy = (pickenergy).ToString() + "," + (interenergy).ToString() + "," + baseprice.ToString();
            }

            return _energy;
        }


        [SessionCheck]
        [HttpGet]
        public ActionResult Summary(string int_id, int? page, string sortBy, string s_date, string e_date)
        {
            try
            {
                ViewBag.SortMeterParameter = sortBy == "Meter" ? "Meter Desc" : "Meter";
                ViewBag.SortDateParameter = sortBy == "Date" ? "Date Desc" : "Date";
                ViewBag.SortPeackEnergyParameter = sortBy == "PE" ? "PE Desc" : "PE";
                ViewBag.SortIntermediateEnergyParameter = sortBy == "IE" ? "IE Desc" : "IE";


                var _invoice = _dbc.tbl_invoice.AsQueryable();
                NiagaraEntities _dbmeter = new NiagaraEntities();

                var _meter = _dbmeter.HISTORY_CONFIG.Where(x => x.ID == x.ID).ToList();

                if (!string.IsNullOrEmpty(int_id))
                {
                    int _id = Convert.ToInt32(int_id);
                    _invoice = _invoice.Where(x => x.int_tenant_id == _id);
                }

                if (!string.IsNullOrEmpty(s_date) && !string.IsNullOrEmpty(e_date))
                {
                    DateTime _sdate = Convert.ToDateTime(s_date);
                    DateTime _edate = Convert.ToDateTime(e_date);
                    _invoice = _invoice.Where(x => x.date_s_bill_date >= _sdate && x.date_e_bill_date <= _edate);
                }
                switch (sortBy)
                {
                    case "Meter Desc":
                        _meter = _meter.OrderByDescending(x => x.ID).ToList();
                        break;
                    case "Date Desc":
                        _invoice = _invoice.OrderByDescending(x => x.date_invoice_date);
                        break;

                    case "Date":
                        _invoice = _invoice.OrderBy(x => x.date_invoice_date);
                        break;

                    //case "PE Desc":
                    //    invoice = invoice.OrderByDescending(x => x.dec_peak_energy);
                    //    break;

                    //case "PE":
                    //    invoice = invoice.OrderBy(x => x.dec_peak_energy);
                    //    break;

                    //case "IE Desc":
                    //    invoice = invoice.OrderByDescending(x => x.dec_inter_energy);
                    //    break;

                    //case "IE":
                    //    invoice = invoice.OrderBy(x => x.dec_inter_energy);
                    //    break;

                    case "Amount Desc":
                        _invoice = _invoice.OrderByDescending(x => x.dec_total);
                        break;

                    case "Amount":
                        _invoice = _invoice.OrderBy(x => x.dec_total);
                        break;


                    default:
                        _invoice = _invoice.OrderByDescending(x => x.date_invoice_date);
                        break;
                }

                var invoice = _invoice.ToList();
                var bygroup = from ins in invoice
                              group ins by ins.tbl_user_master.str_comp_name into empg
                              select new
                              {

                                  Name = empg.Key,
                                  meterid = empg.Max(x => x.tbl_invoice_details.ElementAt(0).int_meter_id),
                                  fromdate = empg.Min(x => x.date_s_bill_date),
                                  todate = empg.Max(x => x.date_e_bill_date),
                                  peakenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_peak_energy),
                                  interenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_inter_energy),
                                  customecharges = empg.Sum(x => x.dec_custome_charges),
                                  decdemad = empg.Sum(x => x.dec_demad),
                                  dectaxamt = empg.Sum(x => x.dec_tax_amt),
                                  dectotal = empg.Sum(x => x.dec_total),
                              };
                var data = bygroup.ToList();

                List<SummaryViewModel> models = new List<SummaryViewModel>();
                SummaryViewModel abc;
                foreach (var dt in data)
                {
                    abc = new SummaryViewModel();
                    abc.Name = dt.Name;
                    abc.meterid = dt.meterid;
                    abc.fromdate = dt.fromdate;
                    abc.todate = dt.todate;
                    abc.peakenergy = dt.peakenergy;
                    abc.interenergy = dt.interenergy;
                    abc.customecharges = dt.customecharges;
                    abc.decdemad = dt.decdemad;
                    abc.dectaxamt = dt.dectaxamt;
                    abc.dectotal = dt.dectotal;
                    models.Add(abc);
                }


                ViewBag._abc = models;
                Session["_exportData"] = _invoice.ToList();
                if (TempData["modeldisplay"] != null)
                    ViewBag.Message = TempData["modeldisplay"].ToString();

                if (!string.IsNullOrEmpty(int_id) || !string.IsNullOrEmpty(s_date) || !string.IsNullOrEmpty(e_date))
                    ViewBag._iscleardisplay = true;


                //bindmeterDropBox();
                bindDropBox();
                return View(_invoice.ToPagedList(page ?? 1, CommonCls._pagesize));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }


        public ActionResult Chartsformonth()
        {
            bindDropBox();
            return View();
        }
        [HttpPost]
        public JsonResult CharterEnergyConsumption(string s_date, string tenantid)
        {
            if (Session["utypeid"].ToString() == CommonCls._usertypeTenant.ToString() || !string.IsNullOrEmpty(tenantid))
            {
                int id = string.IsNullOrEmpty(tenantid) ? Convert.ToInt32(Session["uid"].ToString()) : Convert.ToInt32(tenantid);

                return this.Json(
    (from obj in _dbc.tbl_invoice where obj.int_tenant_id == id orderby obj.int_invoice_id descending select new { Invoice_No = obj.int_invoice_id.ToString(), Energy = obj.dec_total, InvoiceMonth = obj.date_invoice_date.Value.Month }).Take(5)
    , JsonRequestBehavior.AllowGet
    );
            }
            else
            {
                return this.Json(
   (from obj in _dbc.tbl_invoice orderby obj.int_invoice_id descending select new { Invoice_No = obj.int_invoice_id.ToString(), Energy = obj.dec_total, InvoiceMonth = obj.date_invoice_date.Value.Month }).Take(5)
   , JsonRequestBehavior.AllowGet
   );
            }
        }

        [SessionCheck]
        [HttpPost]
        public JsonResult Tenantconsumption(string s_date, string tenantid)
        {
            if (Session["utypeid"].ToString() == CommonCls._usertypeTenant.ToString() || !string.IsNullOrEmpty(tenantid))
            {
                int id = string.IsNullOrEmpty(tenantid) ? Convert.ToInt32(Session["uid"].ToString()) : Convert.ToInt32(tenantid);
                var Consumedenergy = _dbc.tbl_invoice.Where(u => u.int_tenant_id == id).ToList();
                var JsonResult = new
                {
                    success = true,
                    data = Consumedenergy
                };
                return Json(JsonResult);


                //            return this.Json(
                //(from obj in _dbc.tbl_invoice where obj.int_tenant_id == id orderby obj.int_invoice_id descending select new { Invoice_No = obj.int_invoice_id.ToString(), Energy = obj.dec_total, InvoiceMonth = obj.date_invoice_date.Value.Month }).ToList()
                //, JsonRequestBehavior.AllowGet
                //);
            }
            else
            {
                return this.Json(
   (from obj in _dbc.tbl_invoice orderby obj.int_invoice_id descending select new { Invoice_No = obj.int_invoice_id.ToString(), Energy = obj.dec_total, InvoiceMonth = obj.date_invoice_date.Value.Month }).Take(5)
   , JsonRequestBehavior.AllowGet
   );
            }
        }

        [HttpGet]
        public JsonResult Tenantconsbydropdown(string s_date, int tenantid)
        {
            var Consumedenergy = _dbc.tbl_invoice.Where(u => u.int_tenant_id == tenantid).ToList();
            var JsonResult = new
            {
                success = true,
                data = Consumedenergy
            };
            return Json(JsonResult);
        }

        public ActionResult TenantconsumptionReport(string int_id, int? page, string sortBy, string s_date, string tenantid)
        {
            try
            {

                ViewBag.SortDateParameter = sortBy == "Date" ? "Date Desc" : "Date";
                ViewBag.SortPeackEnergyParameter = sortBy == "PE" ? "PE Desc" : "PE";
                ViewBag.SortIntermediateEnergyParameter = sortBy == "IE" ? "IE Desc" : "IE";
                ViewBag.SortAmountParameter = sortBy == "Amount" ? "Amount Desc" : "Amount";


                //var _Invoice = _dbc.tbl_invoice.Where(x => x.int_invoice_id == x.int_invoice_id && x.dec_total == x.dec_total && x.date_invoice_date.Value.Month == x.date_invoice_date.Value.Month).Take(6); 
                var _invoice = _dbc.tbl_invoice.AsQueryable();
                NiagaraEntities _dbmeter = new NiagaraEntities();
                _invoice = _dbc.tbl_invoice.Where(x => x.date_invoice_date.Value.Month == x.date_invoice_date.Value.Month);

                if (!string.IsNullOrEmpty(int_id))
                {
                    int _id = Convert.ToInt32(int_id);
                    _invoice = _invoice.Where(x => x.int_tenant_id == _id);

                }


                var invoice = _invoice.ToList();
                var chartgroup = from ins in invoice

                                 group ins by ins.tbl_user_master.str_comp_name into empg
                                 select new
                                 {
                                     Name = empg.Key,
                                     peakenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_peak_energy),
                                     interenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_inter_energy),
                                     baseenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_base_energy),
                                     invoicedate = empg.Max(x => x.date_invoice_date.Value.Month.ToString()),
                                 };


                var data = chartgroup.ToList();
                List<SummaryViewModel> models = new List<SummaryViewModel>();
                SummaryViewModel abc;
                foreach (var dt in data)
                {
                    abc = new SummaryViewModel();
                    abc.Name = dt.Name;
                    abc.totalenergy = dt.interenergy + dt.peakenergy + dt.baseenergy;
                    if (dt.invoicedate == "5")
                        abc.dateinvoice = "May";
                    if (dt.invoicedate == "1")
                        abc.dateinvoice = "january";
                    if (dt.invoicedate == "2")
                        abc.dateinvoice = "February";
                    if (dt.invoicedate == "3")
                        abc.dateinvoice = "March";
                    if (dt.invoicedate == "4")
                        abc.dateinvoice = "April";
                    if (dt.invoicedate == "6")
                        abc.dateinvoice = "June";
                    if (dt.invoicedate == "7")
                        abc.dateinvoice = "July";
                    if (dt.invoicedate == "8")
                        abc.dateinvoice = "August";
                    if (dt.invoicedate == "9")
                        abc.dateinvoice = "September";
                    if (dt.invoicedate == "10")
                        abc.dateinvoice = "October";
                    if (dt.invoicedate == "11")
                        abc.dateinvoice = "November";
                    if (dt.invoicedate == "12")
                        abc.dateinvoice = "December";
                    models.Add(abc);
                }

                ViewBag._abc = models;

                bindDropBox();
                if (!string.IsNullOrEmpty(int_id))
                    ViewBag._iscleardisplay = true;

                return View(_invoice.ToList().ToPagedList(page ?? 1, CommonCls._pagesize));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }

        public JsonResult TenantconsumptionReportNEW(string int_id, int? page, string sortBy, string s_date, string tenantid)
        {
            try
            {

                ViewBag.SortDateParameter = sortBy == "Date" ? "Date Desc" : "Date";
                ViewBag.SortPeackEnergyParameter = sortBy == "PE" ? "PE Desc" : "PE";
                ViewBag.SortIntermediateEnergyParameter = sortBy == "IE" ? "IE Desc" : "IE";
                ViewBag.SortAmountParameter = sortBy == "Amount" ? "Amount Desc" : "Amount";
                var _invoice = _dbc.tbl_invoice.AsQueryable();
                NiagaraEntities _dbmeter = new NiagaraEntities();
                //_invoice = _dbc.tbl_invoice.Where(x => x.date_invoice_date.Value.Month == x.date_invoice_date.Value.Month).Take(6);

                if (!string.IsNullOrEmpty(int_id))
                {
                    int _id = Convert.ToInt32(int_id);
                    _invoice = _invoice.Where(x => x.int_tenant_id == _id);

                }


                var invoice = _invoice.ToList();
                var chartgroup = from ins in invoice

                                 group ins by ins.tbl_user_master.str_comp_name into empg
                                 select new
                                 {
                                     Name = empg.Key,
                                     peakenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_peak_energy),
                                     interenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_inter_energy),
                                     baseenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_base_energy),
                                     invoicedate = empg.Max(x => x.date_invoice_date.Value.Month.ToString()),

                                     //invoicedate = empg.Max(x => x.date_e_bill_date),
                                     //< td > @strig.Format("{0:MM-dd-yyyy}", dt.fromdate) TO @string.Format("{0:MM-dd-yyyy}", dt.todate) </ td >

                                 };


                var data = chartgroup.ToList();
                List<SummaryViewModel> models = new List<SummaryViewModel>();
                SummaryViewModel abc;
                foreach (var dt in data)
                {
                    abc = new SummaryViewModel();
                    abc.Name = dt.Name;
                    abc.totalenergy = dt.interenergy + dt.peakenergy + dt.baseenergy;
                    if (dt.invoicedate == "5")
                        abc.dateinvoice = "May";
                    if (dt.invoicedate == "1")
                        abc.dateinvoice = "january";
                    if (dt.invoicedate == "2")
                        abc.dateinvoice = "February";
                    if (dt.invoicedate == "3")
                        abc.dateinvoice = "March";
                    if (dt.invoicedate == "4")
                        abc.dateinvoice = "April";
                    if (dt.invoicedate == "6")
                        abc.dateinvoice = "June";
                    if (dt.invoicedate == "7")
                        abc.dateinvoice = "July";
                    if (dt.invoicedate == "8")
                        abc.dateinvoice = "August";
                    if (dt.invoicedate == "9")
                        abc.dateinvoice = "September";
                    if (dt.invoicedate == "10")
                        abc.dateinvoice = "October";
                    if (dt.invoicedate == "11")
                        abc.dateinvoice = "November";
                    if (dt.invoicedate == "12")
                        abc.dateinvoice = "December";
                    models.Add(abc);
                }

                ViewBag._abc = models;

                bindDropBox();
                if (!string.IsNullOrEmpty(int_id))
                    ViewBag._iscleardisplay = true;

                return Json(models, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return Json(null);
        }

        [SessionCheck]
        [HttpGet]
        public ActionResult PreviewMeter(string int_id, int? page, string sortBy, string s_date, string e_date)
        {
            try
            {
                ViewBag.SortDateParameter = sortBy == "Date" ? "Date Desc" : "Date";
                ViewBag.SortPeackEnergyParameter = sortBy == "PE" ? "PE Desc" : "PE";
                ViewBag.SortIntermediateEnergyParameter = sortBy == "IE" ? "IE Desc" : "IE";
                ViewBag.SortAmountParameter = sortBy == "Amount" ? "Amount Desc" : "Amount";


                var _invoice = _dbc.tbl_invoice.AsQueryable();
                NiagaraEntities _dbmeter = new NiagaraEntities();

                var _meter = _dbmeter.HISTORY_CONFIG.Where(x => x.ID == x.ID).ToList();

                if (!string.IsNullOrEmpty(int_id))
                {
                    int _id = Convert.ToInt32(int_id);
                    _invoice = _invoice.Where(x => x.int_tenant_id == _id);
                }

                if (!string.IsNullOrEmpty(s_date) && !string.IsNullOrEmpty(e_date))
                {
                    DateTime _sdate = Convert.ToDateTime(s_date);
                    DateTime _edate = Convert.ToDateTime(e_date);

                    _invoice = _invoice.Where(x => x.date_s_bill_date >= _sdate && x.date_e_bill_date <= _edate);
                }
                switch (sortBy)
                {
                    case "Date Desc":
                        _invoice = _invoice.OrderByDescending(x => x.date_invoice_date);
                        break;

                    case "Date":
                        _invoice = _invoice.OrderBy(x => x.date_invoice_date);
                        break;

                    case "Amount Desc":
                        _invoice = _invoice.OrderByDescending(x => x.dec_total);
                        break;

                    case "Amount":
                        _invoice = _invoice.OrderBy(x => x.dec_total);
                        break;


                    default:
                        _invoice = _invoice.OrderByDescending(x => x.date_invoice_date);
                        break;


                }
                var invoice = _invoice.ToList();
                var bygroup = from ins in invoice
                              group ins by ins.tbl_user_master.str_comp_name into empg
                              select new
                              {

                                  Name = empg.Key,
                                  meterid = empg.Max(x => x.tbl_invoice_details.ElementAt(0).int_meter_id),
                                  fromdate = empg.Min(x => x.date_s_bill_date),
                                  todate = empg.Max(x => x.date_e_bill_date),
                                  peakenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_peak_energy),
                                  interenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_inter_energy),
                                  customecharges = empg.Sum(x => x.dec_custome_charges),
                                  decdemad = empg.Sum(x => x.dec_demad),
                                  dectaxamt = empg.Sum(x => x.dec_tax_amt),
                                  dectotal = empg.Sum(x => x.dec_total),
                              };
                var data = bygroup.ToList();
                List<SummaryViewModel> models = new List<SummaryViewModel>();
                SummaryViewModel abc;
                foreach (var dt in data)
                {
                    abc = new SummaryViewModel();
                    abc.Name = dt.Name;
                    abc.meterid = dt.meterid;
                    abc.fromdate = dt.fromdate;
                    abc.todate = dt.todate;
                    abc.peakenergy = dt.peakenergy;
                    abc.interenergy = dt.interenergy;
                    abc.customecharges = dt.customecharges;
                    abc.decdemad = dt.decdemad;
                    abc.dectaxamt = dt.dectaxamt;
                    abc.dectotal = dt.dectotal;
                    models.Add(abc);
                }


                ViewBag._abc = models;
                Session["_exportData"] = _invoice.ToList();
                if (TempData["modeldisplay"] != null)
                    ViewBag.Message = TempData["modeldisplay"].ToString();

                if (!string.IsNullOrEmpty(int_id) || !string.IsNullOrEmpty(s_date) || !string.IsNullOrEmpty(e_date))
                    ViewBag._iscleardisplay = true;


                //bindmeterDropBox();
                bindDropBox();
                return View(_invoice.ToList().ToPagedList(page ?? 1, CommonCls._pagesize));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }

        public ActionResult ViewPDF()
        {
            string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"800px\">";
            embed += "If you are unable to view file, you can download from <a href = \"{0}\">here</a>";
            embed += " or download <a target = \"_blank\" href = \"http://get.adobe.com/reader/\">Adobe PDF Reader</a> to view the file.";
            embed += "</object>";

            string fileName = "Invoice_" + Session["invid"].ToString() + ".pdf";
            TempData["Embed"] = string.Format(embed, VirtualPathUtility.ToAbsolute("~/PDF/" + fileName));

            return RedirectToAction("invoicepreview", "PM");
        }

        public ActionResult invoicepreview()
        {
            return View();
        }

        private static List<SelectListItem> getMeters()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            NiagaraEntities _dbmeter = new NiagaraEntities();
            var _meter = _dbmeter.HISTORY_CONFIG.ToList();
            if (_meter != null)
            {
                foreach (var m in _meter)
                {
                    items.Add(new SelectListItem
                    {
                        Text = m.TABLE_NAME,
                        Value = m.ID.ToString(),
                    });
                }
            }

            return items;
        }




        //Energy Log

        public ActionResult GetEnergy(int? page, string int_id, string s_date, string e_date)
        {

            List<ProEnergyLog> objuser = new List<ProEnergyLog>();

            var id = Request.QueryString["int_id"];
            var st_date = Request.QueryString["int_id"];
            var ed_date = Request.QueryString["e_date"];

            DataSet ds = new DataSet();
            string ConnectionString = ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("[spProcEnergyLog]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                List<ProEnergyLog> userlist = new List<ProEnergyLog>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ProEnergyLog uobj = new ProEnergyLog();
                    uobj.TENETID = int.Parse(ds.Tables[0].Rows[i]["TENETID"].ToString());
                    uobj.TenentName = Convert.ToString(ds.Tables[0].Rows[i]["COMPNAME"]);
                    uobj.TABLE_NAME = Convert.ToString(ds.Tables[0].Rows[i]["TABLE_NAME"]);
                    uobj.TIMESTAMP = Convert.ToDateTime(ds.Tables[0].Rows[i]["TIMESTAMP"].ToString());
                    uobj.VALUE = Convert.ToDecimal(ds.Tables[0].Rows[i]["VALUE"].ToString());
                    userlist.Add(uobj);
                }
                objuser = userlist;
                if (!string.IsNullOrEmpty(int_id))
                {
                    int _id = Convert.ToInt32(int_id);
                    objuser = objuser.Where(x => x.TENETID == _id).OrderBy(x => x.TIMESTAMP).ToList();
                }

                if (!string.IsNullOrEmpty(s_date) && !string.IsNullOrEmpty(e_date))
                {
                    DateTime _sdate = Convert.ToDateTime(s_date);
                    DateTime _edate = Convert.ToDateTime(e_date);
                    objuser = objuser.Where(x => x.TIMESTAMP >= _sdate && x.TIMESTAMP <= _edate).OrderBy(x => x.TIMESTAMP).ToList();
                }
                if (string.IsNullOrEmpty(int_id) && string.IsNullOrEmpty(e_date) && string.IsNullOrEmpty(s_date))
                {
                    objuser = objuser.Where(x => x.TIMESTAMP >= DateTime.Now).OrderBy(x => x.TIMESTAMP).ToList();
                }
                // objuser = userlist.Where(P=>P.TENETID==int.Parse(int_id)).ToList();
                ViewBag._abc = objuser.OrderBy(x => x.TIMESTAMP).ToPagedList(page ?? 1, CommonCls._pagesize);
                con.Close();

            }

            if (!string.IsNullOrEmpty(int_id))
                ViewBag._iscleardisplay = true;
            bindDropBox();
            return View(objuser.ToList().ToPagedList(page ?? 1, CommonCls._pagesize));

        }

        // Chart

        public ActionResult Chart(string int_id, int? page, string sortBy, string s_date, string tenantid)
        {
            try
            {

                ViewBag.SortDateParameter = sortBy == "Date" ? "Date Desc" : "Date";
                ViewBag.SortPeackEnergyParameter = sortBy == "PE" ? "PE Desc" : "PE";
                ViewBag.SortIntermediateEnergyParameter = sortBy == "IE" ? "IE Desc" : "IE";
                ViewBag.SortAmountParameter = sortBy == "Amount" ? "Amount Desc" : "Amount";


                //var _Invoice = _dbc.tbl_invoice.Where(x => x.int_invoice_id == x.int_invoice_id && x.dec_total == x.dec_total && x.date_invoice_date.Value.Month == x.date_invoice_date.Value.Month).Take(6); 
                var _invoice = _dbc.tbl_invoice.AsQueryable();
                NiagaraEntities _dbmeter = new NiagaraEntities();
                //_invoice = _dbc.tbl_invoice.Where(x => x.date_invoice_date.Value.Month == x.date_invoice_date.Value.Month);

                if (!string.IsNullOrEmpty(int_id))
                {
                    int _id = Convert.ToInt32(int_id);
                    _invoice = _invoice.Where(x => x.int_tenant_id == _id);

                }


                var invoice = _invoice.ToList();
                var chartgroup = from ins in invoice

                                 group ins by ins.tbl_user_master.str_comp_name into empg
                                 select new
                                 {
                                     Name = empg.Key,
                                     peakenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_peak_energy),
                                     interenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_inter_energy),
                                     baseenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_base_energy),
                                     invoicedate = empg.Max(x => x.date_invoice_date.Value.Month.ToString()),
                                 };


                var data = chartgroup.ToList();
                List<SummaryViewModel> models = new List<SummaryViewModel>();
                SummaryViewModel abc;
                foreach (var dt in data)
                {
                    abc = new SummaryViewModel();
                    abc.Name = dt.Name;
                    abc.totalenergy = dt.interenergy + dt.peakenergy + dt.baseenergy;
                    if (dt.invoicedate == "5")
                        abc.dateinvoice = "May";
                    if (dt.invoicedate == "1")
                        abc.dateinvoice = "january";
                    if (dt.invoicedate == "2")
                        abc.dateinvoice = "February";
                    if (dt.invoicedate == "3")
                        abc.dateinvoice = "March";
                    if (dt.invoicedate == "4")
                        abc.dateinvoice = "April";
                    if (dt.invoicedate == "6")
                        abc.dateinvoice = "June";
                    if (dt.invoicedate == "7")
                        abc.dateinvoice = "July";
                    if (dt.invoicedate == "8")
                        abc.dateinvoice = "August";
                    if (dt.invoicedate == "9")
                        abc.dateinvoice = "September";
                    if (dt.invoicedate == "10")
                        abc.dateinvoice = "October";
                    if (dt.invoicedate == "11")
                        abc.dateinvoice = "November";
                    if (dt.invoicedate == "12")
                        abc.dateinvoice = "December";
                    models.Add(abc);
                }

                ViewBag._abc = models;

                bindDropBox();
                if (!string.IsNullOrEmpty(int_id))
                    ViewBag._iscleardisplay = true;

                return View(_invoice.ToList().ToPagedList(page ?? 1, CommonCls._pagesize));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }

        public JsonResult ChartNew(string int_id, int? page, string sortBy, string s_date, string tenantid)
        {
            try
            {

                ViewBag.SortDateParameter = sortBy == "Date" ? "Date Desc" : "Date";
                ViewBag.SortPeackEnergyParameter = sortBy == "PE" ? "PE Desc" : "PE";
                ViewBag.SortIntermediateEnergyParameter = sortBy == "IE" ? "IE Desc" : "IE";
                ViewBag.SortAmountParameter = sortBy == "Amount" ? "Amount Desc" : "Amount";
                var _invoice = _dbc.tbl_invoice.AsQueryable();
                NiagaraEntities _dbmeter = new NiagaraEntities();
                if (!string.IsNullOrEmpty(int_id))
                {
                    int _id = Convert.ToInt32(int_id);
                    _invoice = _invoice.Where(x => x.int_tenant_id == _id);

                }
                var invoice = _invoice.ToList();
                var chartgroup = from ins in invoice

                                 group ins by new { ins.date_invoice_date, ins.int_tenant_id, ins.tbl_user_master.str_comp_name }
                                     into empg
                                 select new
                                 {
                                     Name = empg.Key.str_comp_name,
                                     peakenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_peak_energy),
                                     interenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_inter_energy),
                                     baseenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_base_energy),
                                     invoicedate = empg.Key.date_invoice_date.Value.Month.ToString()
                                 };


                var data = chartgroup.ToList();
                List<SummaryViewModel> models = new List<SummaryViewModel>();
                SummaryViewModel abc;
                for (int i = 1; i <= 12; i++)
                {
                    abc = new SummaryViewModel();
                    var fltrdata = data.Where(P => P.invoicedate == i.ToString()).FirstOrDefault();
                    if (fltrdata != null)
                    {
                        abc.Name = fltrdata.Name;
                        abc.totalenergy = fltrdata.interenergy + fltrdata.peakenergy + fltrdata.baseenergy;
                        abc.dateinvoice = fltrdata.invoicedate;
                        models.Add(abc);
                    }
                    else
                    {
                        abc.Name = null;
                        abc.totalenergy = 0;
                        abc.dateinvoice = i.ToString();
                        models.Add(abc);
                    }
                }

                ViewBag._abc = models.OrderBy(P => P.dateinvoice);

                bindDropBox();
                if (!string.IsNullOrEmpty(int_id))
                    ViewBag._iscleardisplay = true;

                return Json(models, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return Json(null);
        }

        public ActionResult Piechart(string int_id, int? page, string sortBy, string s_date, string tenantid)
        {
            try
            {

                ViewBag.SortDateParameter = sortBy == "Date" ? "Date Desc" : "Date";
                ViewBag.SortPeackEnergyParameter = sortBy == "PE" ? "PE Desc" : "PE";
                ViewBag.SortIntermediateEnergyParameter = sortBy == "IE" ? "IE Desc" : "IE";
                ViewBag.SortAmountParameter = sortBy == "Amount" ? "Amount Desc" : "Amount";


                //var _Invoice = _dbc.tbl_invoice.Where(x => x.int_invoice_id == x.int_invoice_id && x.dec_total == x.dec_total && x.date_invoice_date.Value.Month == x.date_invoice_date.Value.Month).Take(6); 
                var _invoice = _dbc.tbl_invoice.AsQueryable();
                NiagaraEntities _dbmeter = new NiagaraEntities();
                //_invoice = _dbc.tbl_invoice.Where(x => x.date_invoice_date.Value.Month == x.date_invoice_date.Value.Month);

                if (!string.IsNullOrEmpty(int_id))
                {
                    int _id = Convert.ToInt32(int_id);
                    _invoice = _invoice.Where(x => x.int_tenant_id == _id);

                }


                var invoice = _invoice.ToList();
                var chartgroup = from ins in invoice

                                 group ins by ins.tbl_user_master.str_comp_name into empg
                                 select new
                                 {
                                     Name = empg.Key,
                                     peakenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_peak_energy),
                                     interenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_inter_energy),
                                     baseenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_base_energy),
                                     invoicedate = empg.Max(x => x.date_invoice_date.Value.Month.ToString()),
                                 };


                var data = chartgroup.ToList();
                List<SummaryViewModel> models = new List<SummaryViewModel>();
                SummaryViewModel abc;
                foreach (var dt in data)
                {
                    abc = new SummaryViewModel();
                    abc.Name = dt.Name;
                    abc.totalenergy = dt.interenergy + dt.peakenergy + dt.baseenergy;
                    if (dt.invoicedate == "5")
                        abc.dateinvoice = "May";
                    if (dt.invoicedate == "1")
                        abc.dateinvoice = "january";
                    if (dt.invoicedate == "2")
                        abc.dateinvoice = "February";
                    if (dt.invoicedate == "3")
                        abc.dateinvoice = "March";
                    if (dt.invoicedate == "4")
                        abc.dateinvoice = "April";
                    if (dt.invoicedate == "6")
                        abc.dateinvoice = "June";
                    if (dt.invoicedate == "7")
                        abc.dateinvoice = "July";
                    if (dt.invoicedate == "8")
                        abc.dateinvoice = "August";
                    if (dt.invoicedate == "9")
                        abc.dateinvoice = "September";
                    if (dt.invoicedate == "10")
                        abc.dateinvoice = "October";
                    if (dt.invoicedate == "11")
                        abc.dateinvoice = "November";
                    if (dt.invoicedate == "12")
                        abc.dateinvoice = "December";
                    models.Add(abc);
                }

                ViewBag._abc = models;

                bindDropBox();
                if (!string.IsNullOrEmpty(int_id))
                    ViewBag._iscleardisplay = true;

                return View(_invoice.ToList().ToPagedList(page ?? 1, CommonCls._pagesize));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }

        public JsonResult PiechartNew(string int_id, int? page, string sortBy, string s_date, string tenantid)
        {
            try
            {

                ViewBag.SortDateParameter = sortBy == "Date" ? "Date Desc" : "Date";
                ViewBag.SortPeackEnergyParameter = sortBy == "PE" ? "PE Desc" : "PE";
                ViewBag.SortIntermediateEnergyParameter = sortBy == "IE" ? "IE Desc" : "IE";
                ViewBag.SortAmountParameter = sortBy == "Amount" ? "Amount Desc" : "Amount";
                var _invoice = _dbc.tbl_invoice.AsQueryable();
                NiagaraEntities _dbmeter = new NiagaraEntities();
                if (!string.IsNullOrEmpty(int_id))
                {
                    int _id = Convert.ToInt32(int_id);
                    _invoice = _invoice.Where(x => x.int_tenant_id == _id);

                }
                var invoice = _invoice.ToList();
                var chartgroup = from ins in invoice

                                 group ins by new { ins.date_invoice_date, ins.int_tenant_id, ins.tbl_user_master.str_comp_name }
                                     into empg
                                 select new
                                 {
                                     Name = empg.Key.str_comp_name,
                                     peakenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_peak_energy),
                                     interenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_inter_energy),
                                     baseenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_base_energy),
                                     invoicedate = empg.Key.date_invoice_date.Value.Month.ToString()
                                 };


                var data = chartgroup.ToList();
                List<SummaryViewModel> models = new List<SummaryViewModel>();
                SummaryViewModel abc;
                for (int i = 1; i <= 12; i++)
                {
                    abc = new SummaryViewModel();
                    var fltrdata = data.Where(P => P.invoicedate == i.ToString()).FirstOrDefault();
                    if (fltrdata != null)
                    {
                        abc.Name = fltrdata.Name;
                        abc.totalenergy = fltrdata.interenergy + fltrdata.peakenergy + fltrdata.baseenergy;
                        abc.dateinvoice = fltrdata.invoicedate;
                        models.Add(abc);
                    }
                    else
                    {
                        abc.Name = null;
                        abc.totalenergy = 0;
                        abc.dateinvoice = i.ToString();
                        models.Add(abc);
                    }
                }

                ViewBag._abc = models.OrderBy(P => P.dateinvoice);

                bindDropBox();
                if (!string.IsNullOrEmpty(int_id))
                    ViewBag._iscleardisplay = true;

                return Json(models, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return Json(null);
        }
    }
}
