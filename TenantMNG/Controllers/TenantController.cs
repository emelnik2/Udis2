using System.Linq;
using System.Collections.Generic;
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
using log4net;
using System;
using System.Web.UI;


namespace TenantMNG.Controllers
{
    public class TenantController : MyController
    {
        DB_TenantMNGEntities _dbc = new DB_TenantMNGEntities();
        ILog log = log4net.LogManager.GetLogger(typeof(TenantController));
        // GET: Tenant
        public ActionResult Dashboard()
        {
            int pmid = Convert.ToInt32(Session["uid"].ToString());
            var _user = _dbc.tbl_user_master.Where(x => x.int_user_type_id ==3 && x.int_pm_id == pmid).ToList();
            var _invoice = _dbc.tbl_invoice.Where(x =>x.int_tenant_id == pmid).ToList();

            TenantVM _objvm = new TenantVM();
            _objvm.tbl_invoice = _invoice.ToList();

            _objvm.tbl_user_master = _user.ToList();


            bindDropBox();
            return View(_objvm);
        }

        public void bindDropBox()
        {
            int _pm_id = Convert.ToInt32(Session["uid"].ToString());

            var _tenant = _dbc.tbl_user_master.Where(x => x.int_pm_id == _pm_id).ToList();

            ViewBag.TenantDropDown = new SelectList(_tenant, "int_id", "str_comp_name");
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

        [HttpPost]
        public JsonResult PieCharterEnergyConsumption()
        {
            return this.Json(
(from obj in _dbc.tbl_invoice select new { PeackEnergy = obj.dec_total })
, JsonRequestBehavior.AllowGet
);
        }

        // for meter summary

        [SessionCheck]
        [HttpGet]
        public ActionResult Summary(int? page)
        {
            try
            {
                int _pm_id = Convert.ToInt32(Session["uid"].ToString());

                var _tenant = _dbc.tbl_invoice.Where(x => x.int_tenant_id == _pm_id).ToList();
                var tenant = _tenant.ToList();
                var bygroup = from ins in tenant
                              group ins by ins.tbl_user_master.str_comp_name into empg
                              select new
                              {

                                  Name = empg.Key,
                                  meterid = empg.Max(x => x.tbl_invoice_details.ElementAt(0).str_meter_id),
                                  fromdate = empg.Min(x => x.date_s_bill_date),
                                  todate = empg.Max(x => x.date_e_bill_date),
                                  peakenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_peak_energy),
                                  interenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_inter_energy),
                                  //customecharges = empg.Sum(x => x.dec_custome_charges),
                                  //decdemad = empg.Sum(x => x.dec_demad),
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
                    abc.strmeterid = dt.meterid;
                    abc.fromdate = dt.fromdate;
                    abc.todate = dt.todate;
                    abc.peakenergy = dt.peakenergy;
                    abc.interenergy = dt.interenergy;
                    //abc.customecharges = dt.customecharges;
                    //abc.decdemad = dt.decdemad;
                    abc.dectaxamt = dt.dectaxamt;
                    abc.dectotal = dt.dectotal;
                    models.Add(abc);
                }
                ViewBag._abc = models;
                return View(_dbc.tbl_invoice.Where(x => x.int_tenant_id == _pm_id).ToList().ToPagedList(page ?? 1, CommonCls._pagesize));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }

        //[SessionCheck]
        //[HttpGet]
        //public ActionResult Summary(int? page, string s_date, string e_date, string int_id)
        //{
        //    try
        //    {
        //        var _tenant = _dbc.tbl_invoice.AsQueryable();

        //        int _pm_id = Convert.ToInt32(Session["uid"].ToString());

        //        if (!string.IsNullOrEmpty(int_id))
        //        {

        //            _tenant = _dbc.tbl_invoice.Where(x => x.int_tenant_id == _pm_id);

        //        }
        //        if (!string.IsNullOrEmpty(s_date) && !string.IsNullOrEmpty(e_date))
        //        {
        //            DateTime _sdate = Convert.ToDateTime(s_date);
        //            DateTime _edate = Convert.ToDateTime(e_date);


        //            _tenant = _tenant.Where(x => x.date_s_bill_date >= _sdate && x.date_e_bill_date <= _edate);

        //        }

        //        //_tenant = _tenant.Where(x => x.int_tenant_id == _pm_id).ToList();

        //        return View(_tenant.Where(x => x.int_tenant_id == _pm_id).ToList().ToPagedList(page ?? 1, CommonCls._pagesize));
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.Message);
        //    }
        //    return View();
        //}



        //Meter Reading
        [SessionCheck]
        [HttpGet]
        public ActionResult MeterReading(int? page)
        {
            try
            {
                int _pm_id = Convert.ToInt32(Session["uid"].ToString());

                var _tenant = _dbc.tbl_invoice.Where(x => x.int_tenant_id == _pm_id).ToList();
                var tenant = _tenant.ToList();
                var bygroup = from ins in tenant
                              group ins by ins.tbl_user_master.str_comp_name into empg
                              select new
                              {

                                  Name = empg.Key,
                                  meterid = empg.Max(x => x.tbl_invoice_details.ElementAt(0).str_meter_id),
                                  fromdate = empg.Min(x => x.date_s_bill_date),
                                  todate = empg.Max(x => x.date_e_bill_date),
                                  peakenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_peak_energy),
                                  interenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_inter_energy),
                                  //customecharges = empg.Sum(x => x.dec_custome_charges),
                                  //decdemad = empg.Sum(x => x.dec_demad),
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
                    abc.strmeterid = dt.meterid;
                    abc.fromdate = dt.fromdate;
                    abc.todate = dt.todate;
                    abc.peakenergy = dt.peakenergy;
                    abc.interenergy = dt.interenergy;
                    //abc.customecharges = dt.customecharges;
                    //abc.decdemad = dt.decdemad;
                    abc.dectaxamt = dt.dectaxamt;
                    abc.dectotal = dt.dectotal;
                    models.Add(abc);
                }


                ViewBag._abc = models;
                return View(_dbc.tbl_invoice.Where(x => x.int_tenant_id == _pm_id).ToList().ToPagedList(page ?? 1, CommonCls._pagesize));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }


        public ActionResult TenantconsumptionReport(int? page)
        {
            try
            {
                int _pm_id = Convert.ToInt32(Session["uid"].ToString());

                var _tenant = _dbc.tbl_invoice.Where(x => x.int_tenant_id == _pm_id).ToList();
                var tenant = _tenant.ToList();
                var bygroup = from ins in tenant
                              group ins by ins.tbl_user_master.str_comp_name into empg
                              select new
                              {

                                  Name = empg.Key,
                                  peakenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_peak_energy),
                                  interenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_inter_energy),
                                  baseenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_base_energy),
                                  invoicedate = empg.Max(x => x.date_invoice_date.Value.Month.ToString()),
                              };
                var data = bygroup.ToList();

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
                return View(_dbc.tbl_invoice.Where(x => x.int_tenant_id == _pm_id).ToList().ToPagedList(page ?? 1, CommonCls._pagesize));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }

        public JsonResult TenantconsumptionReportNEW( int? page)
        {
            try
            {

                int _pm_id = Convert.ToInt32(Session["uid"].ToString());

                var _tenant = _dbc.tbl_invoice.Where(x => x.int_tenant_id == _pm_id).ToList();
                var tenant = _tenant.ToList();
                var chartgroup = from ins in tenant

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

                return Json(models, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return Json(null);
        }
        public ActionResult PaiChart(int? page)
        {
            try
            {
                int _pm_id = Convert.ToInt32(Session["uid"].ToString());

                var _tenant = _dbc.tbl_invoice.Where(x => x.int_tenant_id == _pm_id).ToList();
                var tenant = _tenant.ToList();
                var bygroup = from ins in tenant
                              group ins by ins.tbl_user_master.str_comp_name into empg
                              select new
                              {

                                  Name = empg.Key,
                                  peakenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_peak_energy),
                                  interenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_inter_energy),
                                  baseenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_base_energy),
                                  invoicedate = empg.Max(x => x.date_invoice_date.Value.Month.ToString()),
                              };
                var data = bygroup.ToList();

                List<SummaryViewModel> models = new List<SummaryViewModel>();
                SummaryViewModel abc;
                foreach (var dt in data)
                {
                    abc = new SummaryViewModel();
                    abc.Name = dt.Name;
                    abc.totalenergy = dt.interenergy + dt.peakenergy + dt.baseenergy;
                    abc.dateinvoice = dt.invoicedate;
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
                return View(_dbc.tbl_invoice.Where(x => x.int_tenant_id == _pm_id).ToList().ToPagedList(page ?? 1, CommonCls._pagesize));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }





        public ActionResult Chart(int? page)
        {
            try
            {
              int  _pm_id = Convert.ToInt32(Session["uid"].ToString());

                var _tenant = _dbc.tbl_invoice.Where(x => x.int_tenant_id == _pm_id).ToList();
                var tenant = _tenant.ToList();
                var bygroup = from ins in tenant
                              group ins by ins.tbl_user_master.str_comp_name into empg
                              select new
                              {

                                  Name = empg.Key,
                                  peakenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_peak_energy),
                                  interenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_inter_energy),
                                  baseenergy = empg.Sum(x => x.tbl_invoice_details.ElementAt(0).dec_base_energy),
                                  invoicedate = empg.Max(x => x.date_invoice_date.Value.Month.ToString()),
                              };
                var data = bygroup.ToList();

                List<SummaryViewModel> models = new List<SummaryViewModel>();
                SummaryViewModel abc;
                foreach (var dt in data)
                {
                    abc = new SummaryViewModel();
                    abc.Name = dt.Name;
                    abc.totalenergy = dt.interenergy + dt.peakenergy + dt.baseenergy;
                    abc.dateinvoice = dt.invoicedate;
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
                return View(_dbc.tbl_invoice.Where(x => x.int_tenant_id == _pm_id).ToList().ToPagedList(page ?? 1, CommonCls._pagesize));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }

        public JsonResult Chartnew (int? page)
        {
            try
            {
               int _pm_id = Convert.ToInt32(Session["uid"].ToString());

                var _tenant = _dbc.tbl_invoice.Where(x => x.int_tenant_id == _pm_id).ToList();
                var tenant = _tenant.ToList();
                var bygroup = from ins in tenant
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
                var data = bygroup.ToList();
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
                return Json(models, JsonRequestBehavior.AllowGet);
                //return View(_dbc.tbl_invoice.Where(x => x.int_tenant_id == _pm_id).ToList().ToPagedList(page ?? 1, CommonCls._pagesize));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return Json(null);
        }


        [SessionCheck]
        [HttpGet]
        public ActionResult TenantActivity( int? page)
        {
            try
            {
                int _pm_id = Convert.ToInt32(Session["uid"].ToString());

                var _tenant = _dbc.tbl_invoice.Where(x => x.int_tenant_id == _pm_id).ToList();
                var _invoice = _dbc.tbl_invoice.AsQueryable();

               
                Session["_exportData"] = _invoice.ToList();
                if (TempData["modeldisplay"] != null)
                    ViewBag.Message = TempData["modeldisplay"].ToString();

                bindDropBox();
                return View(_dbc.tbl_invoice.Where(x => x.int_tenant_id == _pm_id).ToList().ToPagedList(page ?? 1, CommonCls._pagesize));
               
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }
        public ActionResult GeneratePDF(int id)
        {

            bindData(id);

            TempData["modeldisplay"] = "Yes";

            return RedirectToAction("TenantActivity");
        }

        private void bindData(int id)
        {
            try
            {
                TempData["invid"] = id;
                string path = Server.MapPath("~/Template/invoice.html");
                string html = System.IO.File.ReadAllText(path);
                string filename = "Invoice_" + id.ToString() + ".PDF";

                path = HttpContext.Server.MapPath("~/PDF/");




                string cdnFilePath = path + filename;

                var _invoice = _dbc.tbl_invoice.Where(x => x.int_invoice_id == id).FirstOrDefault();

                if (_invoice != null)
                {
                    CharterColumn(_invoice.int_tenant_id, id);
                    string address = _invoice.tbl_user_master.str_add_1 + "<br>" + _invoice.tbl_user_master.str_add_2 + "<br>" + _invoice.tbl_user_master.str_city + "<br>" + _invoice.tbl_user_master.str_state + "<br>" + _invoice.tbl_user_master.str_country;
                    html = html.Replace("#address", address);
                    html = html.Replace("#name", _invoice.tbl_user_master.str_comp_name);
                    html = html.Replace("#billperiod", _invoice.date_s_bill_date.Value.ToString("dd/MM/yyyy") + " AL " + _invoice.date_e_bill_date.Value.ToString("dd/MM/yyyy"));
                    html = html.Replace("#billdate", _invoice.date_invoice_date.Value.ToString("dd/MM/yyyy"));
                    html = html.Replace("#paydate", _invoice.date_pay_date.Value.ToString("dd/MM/yyyy"));
                    html = html.Replace("#invno", _invoice.int_invoice_id.ToString());
                    //html = html.Replace("#customcharge", "$" + CommonCls.DoFormat(_invoice.dec_custome_charges));
                    //html = html.Replace("#customtitle", string.IsNullOrEmpty(_invoice.str_custome_charge_desc) ? "Cargo personalizado" : _invoice.str_custome_charge_desc);

                    //html = html.Replace("#demandenergy", CommonCls.DoFormat(_invoice.dec_demad) + " kW");


                    //html = html.Replace("#meterpoint", CommonCls.getMeterNamefromId(_invoice.int_meter_id));
                    //html = html.Replace("#consumepeakkw", CommonCls.DoFormat(_invoice.dec_peak_energy) + " kWh");
                    //html = html.Replace("#consumebasekw", CommonCls.DoFormat(_invoice.dec_base_energy) + " kWh");
                    //html = html.Replace("#consumeintermediatekWh", CommonCls.DoFormat(_invoice.dec_inter_energy) + " kWh");


                    //html = html.Replace("#opingpeakkw", CommonCls.DoFormat(_invoice.dec_prev_peack_energy) + " kWh");
                    //html = html.Replace("#closingpeakkw", CommonCls.DoFormat(_invoice.dec_current_peack_energy) + " kWh");

                    //html = html.Replace("#opingintermediatekWh", CommonCls.DoFormat(_invoice.dec_prev_peack_energy) + " kWh");
                    //html = html.Replace("#closingintermediatekWh", CommonCls.DoFormat(_invoice.dec_current_inter_energy) + " kWh");

                    //html = html.Replace("#energytype1", "Peak Energy");
                    //html = html.Replace("#energyusage1", CommonCls.DoFormat(_invoice.dec_peak_energy) + " kWh");
                    //html = html.Replace("#energyrate1", CommonCls.DoFormat(_invoice.dec_peak_energy_rate));
                    //html = html.Replace("#energyamt1", "$" + CommonCls.DoFormat(_invoice.dec_peak_energy_rate * _invoice.dec_peak_energy));

                    //html = html.Replace("#energytype3", "Base Energy");
                    //html = html.Replace("#energyusage3", CommonCls.DoFormat(_invoice.dec_base_energy) + " kWh");
                    //html = html.Replace("#energyrate3", CommonCls.DoFormat(_invoice.dec_base_rate));
                    //html = html.Replace("#energyamt3", "$" + CommonCls.DoFormat(_invoice.dec_base_rate * _invoice.dec_base_energy));

                    //html = html.Replace("#energytype2", "Intermediate Energy");
                    //html = html.Replace("#energyusage2", CommonCls.DoFormat(_invoice.dec_inter_energy).ToString() + " kWh");
                    //html = html.Replace("#energyrate2", CommonCls.DoFormat(_invoice.dec_inter_energy_rate));
                    //html = html.Replace("#energyamt2", "$" + CommonCls.DoFormat(_invoice.dec_inter_energy * _invoice.dec_inter_energy_rate));

                    html = html.Replace("#demandtier1", "Tier 1");
                    //html = html.Replace("#demandusage", CommonCls.DoFormat(_invoice.dec_demad) + " kWh");
                    html = html.Replace("#demandrate", "$0");
                    html = html.Replace("#demandamt", "$0");

                    //html = html.Replace("#chargedesc1", _invoice.str_custome_charge_desc);
                    html = html.Replace("#chargetype1", "Flat Type");
                    //html = html.Replace("#customeunit1", "$" + CommonCls.DoFormat(_invoice.dec_custome_charges));
                    html = html.Replace("#customerate1", "$0");
                    //html = html.Replace("#customamt1", "$" + CommonCls.DoFormat(_invoice.dec_custome_charges));

                    html = html.Replace("#chargedesc2", "IVA 16%");
                    html = html.Replace("#chargetype2", "Percentage");
                    //html = html.Replace("#customeunit2", CommonCls.DoFormat(_invoice.dec_inter_energy + _invoice.dec_peak_energy + _invoice.dec_base_energy) + " kWh");
                    //html = html.Replace("#customerate2", "16");
                    html = html.Replace("#customamt2", "$" + CommonCls.DoFormat(_invoice.dec_tax_amt));





                    html = html.Replace("#charturl", "I");


                    //var _setting = _dbc.tbl_tenant_settings.Where(x => x.int_tenant_id == _invoice.int_tenant_id).FirstOrDefault();
                    //decimal _totalfacturable = 0;
                    //if (_setting != null)
                    //{
                    //html = html.Replace("#demanfacturabledamt", "$" + _invoice.dec_demanda_facturable.ToString());
                    //html = html.Replace("#buildingtotal", "$" + _invoice.dec_total_ene.ToString());




                    //html = html.Replace("#demandfinalamt", "$" + CommonCls.DoFormat(_invoice.dec_demanda_facturable_amount));


                    //decimal _finaltotal = _totalenergyamt + _totalfacturable + _invoice.dec_tax_amt.Value;

                    html = html.Replace("#totalamt", "$" + CommonCls.DoFormat(_invoice.dec_total));

                    path = Server.MapPath("~/Template/meterlist.html");
                    string replacemeterlist = string.Empty;
                    string meterlisthtml = string.Empty;

                    decimal _totalenergyamt = 0, _totalenergy = 0;

                    var _invoicedetails = _dbc.tbl_invoice_details.Where(x => x.int_invoice_id == id).ToList();

                    if (_invoicedetails != null)
                    {

                        foreach (var details in _invoicedetails)
                        {
                            meterlisthtml = System.IO.File.ReadAllText(path);

                            meterlisthtml = meterlisthtml.Replace("#metername", CommonCls.getMeterNamefromId(details.str_meter_id));

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

                html = html.Replace("#FooterImg", path + "/PDF/chart.jpg");

                if (System.IO.File.Exists(cdnFilePath))
                {
                    System.IO.File.Delete(cdnFilePath);
                }

                (new NReco.PdfGenerator.HtmlToPdfConverter()).GeneratePdf(html.ToString(), null, cdnFilePath);



            }
            catch (Exception ex)
            {

                log.Error(ex.Message.ToString());
            }

        }

        public void CharterColumn(int? tenantid, int invoiceid)
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

        public FileResult DownloadPdf(int forpdf)
        {
            string fileName = "Invoice_" + forpdf + ".pdf";
            string filenamepath = Server.MapPath("~/PDF/" + fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filenamepath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public ActionResult ViewPDF()
        {
            string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"800px\">";
            embed += "If you are unable to view file, you can download from <a href = \"{0}\">here</a>";
            embed += " or download <a target = \"_blank\" href = \"http://get.adobe.com/reader/\">Adobe PDF Reader</a> to view the file.";
            embed += "</object>";

            string fileName = "Invoice_" + TempData["invid"].ToString() + ".pdf";
            TempData["Embed"] = string.Format(embed, VirtualPathUtility.ToAbsolute("~/PDF/" + fileName));

            return RedirectToAction("invoicepreview", "PM");
        }


        public FileResult Download()
        {
            string fileName = "Invoice_" + TempData["invid"].ToString() + ".pdf";
            string filenamepath = Server.MapPath("~/PDF/" + fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filenamepath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}