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
        UdisEntities _dmeter = new UdisEntities();



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

        [HttpGet] // this action result returns the partial containing the modal
        public ActionResult DetachMeter(string id)
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

        [HttpGet] // this action result returns the partial containing the modal
        public ActionResult DeleteInvoice(string id)
        {
            Session["invoice_id"] = id;
            return new EmptyResult();
        }

        [HttpPost] // this action result returns the partial containing the modal
        public ActionResult DeleteInvoice()
        {
            UserBAL usrbal = new UserBAL();

            int _id = Convert.ToInt32(Session["invoice_id"].ToString());

            int _lVal = usrbal.invoice_delete(_id);


            return RedirectToAction("TenantActivity");
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
        [HttpGet]
        public ActionResult CreateInvoice(string strmeterid)
        {
            InvoiceVM _objvm = new InvoiceVM();
            try
            {
                int id = 0;

                var _tenantid = _dbc.tbl_tenant_meter.Where(x => x.str_meter_id == strmeterid && x.bit_is_assign == true).FirstOrDefault();
                var tarifas = _dbc.tbl_tarifas.FirstOrDefault();

                if (_tenantid != null)
                {
                    _objvm.int_invoice_id = 0;
                    id = Convert.ToInt32(Convert.ToInt32(_tenantid.int_tenant_id));

                    //var _tenantbillinginfo = _dbc.tbl_tenant_billing_info.Where(x => x.int_tenant_id == id).SingleOrDefault();
                    /*var _ratesinfo = _dbc.tbl_tarifas.FirstOrDefault();

                    if (_ratesinfo != null)
                    {
                        _objvm.suministro = _ratesinfo.suministro;
                        _objvm.distribucion = _ratesinfo.distribucion;
                        _objvm.tarifa_transmision = _ratesinfo.tarifa_transmision;
                        _objvm.operacion_cenace = _ratesinfo.operacion_cenace;
                        _objvm.dec_base_rate = _ratesinfo.dec_base_rate;
                        _objvm.dec_inter_energy_rate = _ratesinfo.dec_inter_energy_rate;
                        _objvm.dec_peak_energy_rate = _ratesinfo.dec_peak_energy_rate;
                        _objvm.capacidad = _ratesinfo.capacidad;
                        _objvm.cre_servicios_conexos = _ratesinfo.cre_servicios_conexos;

                    }*/
                    int _uid = Convert.ToInt32(Session["uid"].ToString());
                    var _pmbillingtime = _dbc.tbl_pm_billing_hours.Where(x => x.int_pm_id == _uid).SingleOrDefault();
                    if (_pmbillingtime != null /*&& _ratesinfo != null*/)
                    {

                        _objvm.str_inter_e_time = _pmbillingtime.str_inter_e_time_1_m;
                        _objvm.str_inter_s_time = _pmbillingtime.str_inter_s_time_2_m;

                        _objvm.str_peak_e_time = _pmbillingtime.str_peak_e_time_m;
                        _objvm.str_peak_s_time = _pmbillingtime.str_peak_s_time_m;

                        //_objvm.int_invoice_period = _tenantbillinginfo.tbl_user_master.int_invoice_period;
                        _objvm.int_invoice_period = 3;
                    }

                    if (tarifas != null)
                    {
                        var _tarifasvm = new TarifasVM();

                        _tarifasvm.dec_base_rate = tarifas.dec_base_rate;
                        _tarifasvm.dec_inter_energy_rate = tarifas.dec_inter_energy_rate;
                        _tarifasvm.dec_peak_energy_rate = tarifas.dec_peak_energy_rate;
                        _tarifasvm.suministro = tarifas.suministro;
                        _tarifasvm.distribucion = tarifas.distribucion;
                        _tarifasvm.tarifa_transmision = tarifas.tarifa_transmision;
                        _tarifasvm.operacion_cenace = tarifas.operacion_cenace;
                        _tarifasvm.capacidad = tarifas.capacidad;
                        _tarifasvm.cre_servicios_conexos = tarifas.cre_servicios_conexos;

                        var meses = new SelectList(new List<SelectListItem>()
                        {
                                new SelectListItem(){ Value="Enero", Text="Enero"},
                                new SelectListItem(){ Value="Febrero", Text="Febrero"},
                                new SelectListItem(){ Value="Marzo", Text="Marzo"},
                                new SelectListItem(){ Value="Abril", Text="Abril"},
                                new SelectListItem(){ Value="Mayo", Text="Mayo"},
                                new SelectListItem(){ Value="Junio", Text="Junio"},
                                new SelectListItem(){ Value="Julio", Text="Julio"},
                                new SelectListItem(){ Value="Agosto", Text="Agosto"},
                                new SelectListItem(){ Value="Septiembre", Text="Septiembre"},
                                new SelectListItem(){ Value="Octubre", Text="Octubre"},
                                new SelectListItem(){ Value="Noviembre", Text="Noviembre"},
                                new SelectListItem(){ Value="Diciembre", Text="Diciembre"},
                        },
                            "Value",
                            "Text");

                       
                        var anios = new SelectList(new List<SelectListItem>()
                        { 
                            new SelectListItem() { Value = Convert.ToString(DateTime.Now.Year), Text = Convert.ToString(DateTime.Now.Year) },
                            new SelectListItem() { Value = Convert.ToString(DateTime.Now.Year - 1), Text = Convert.ToString(DateTime.Now.Year - 1) },
                        },
                            "Value",
                            "Text");

                    _tarifasvm.Meses = meses;
                    _tarifasvm.Anios = anios;

                    _tarifasvm.mes_tarifas = tarifas.mes_tarifas;
                    _tarifasvm.ano_tarifas = Convert.ToInt32(tarifas.ano_tarifas);


                        _objvm.tarifassetting = _tarifasvm;
                    }

                    //var _pmsetting = _dbc.tbl_tenant_settings.Where(x => x.int_tenant_id == id).SingleOrDefault();

                    // This section not used anymore in current version
                    /* if (_pmsetting != null)
                    {
                        _objvm.dec_demanda_facturable = _pmsetting.dec_demanda_facturable;
                        _objvm.dec_total_ene = _pmsetting.dec_total_ene;
                    }
                    else
                    {
                        _objvm.dec_demanda_facturable = 0;
                        _objvm.dec_total_ene = 0;
                    }*/

                }
                ViewBag._erromsg = -1;
                _objvm.int_tenant_id = id;
                _objvm.str_meter_id = strmeterid;
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

                DataTable dt = (DataTable)Session["tenantDT"];
                //objvm = new InvoiceVM();
                objbal = new TenantBAL();

                var tarifas = _dbc.tbl_tarifas.FirstOrDefault();

                if (tarifas != null)
                {
                    var _tarifasvm = new TarifasVM();

                    _tarifasvm.dec_base_rate = tarifas.dec_base_rate;
                    _tarifasvm.dec_inter_energy_rate = tarifas.dec_inter_energy_rate;
                    _tarifasvm.dec_peak_energy_rate = tarifas.dec_peak_energy_rate;
                    _tarifasvm.suministro = tarifas.suministro;
                    _tarifasvm.distribucion = tarifas.distribucion;
                    _tarifasvm.tarifa_transmision = tarifas.tarifa_transmision;
                    _tarifasvm.operacion_cenace = tarifas.operacion_cenace;
                    _tarifasvm.capacidad = tarifas.capacidad;
                    _tarifasvm.cre_servicios_conexos = tarifas.cre_servicios_conexos;

                    var meses = new SelectList(new List<SelectListItem>()
                        {
                                new SelectListItem(){ Value="Enero", Text="Enero"},
                                new SelectListItem(){ Value="Febrero", Text="Febrero"},
                                new SelectListItem(){ Value="Marzo", Text="Marzo"},
                                new SelectListItem(){ Value="Abril", Text="Abril"},
                                new SelectListItem(){ Value="Mayo", Text="Mayo"},
                                new SelectListItem(){ Value="Junio", Text="Junio"},
                                new SelectListItem(){ Value="Julio", Text="Julio"},
                                new SelectListItem(){ Value="Agosto", Text="Agosto"},
                                new SelectListItem(){ Value="Septiembre", Text="Septiembre"},
                                new SelectListItem(){ Value="Octubre", Text="Octubre"},
                                new SelectListItem(){ Value="Noviembre", Text="Noviembre"},
                                new SelectListItem(){ Value="Diciembre", Text="Diciembre"},
                        },
                        "Value",
                        "Text");


                    var anios = new SelectList(new List<SelectListItem>()
                        {
                            new SelectListItem() { Value = Convert.ToString(DateTime.Now.Year), Text = Convert.ToString(DateTime.Now.Year) },
                            new SelectListItem() { Value = Convert.ToString(DateTime.Now.Year - 1), Text = Convert.ToString(DateTime.Now.Year - 1) },
                        },
                        "Value",
                        "Text");

                    _tarifasvm.Meses = meses;
                    _tarifasvm.Anios = anios;

                    _tarifasvm.mes_tarifas = tarifas.mes_tarifas;
                    _tarifasvm.ano_tarifas = Convert.ToInt32(tarifas.ano_tarifas);


                    objvm.tarifassetting = _tarifasvm;
                }


                /*for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //objvm.int_invoice_id = _lVal;
                    objvm.dec_base_amt = Convert.ToDecimal(dt.Rows[i]["dec_base_amt"].ToString());
                    objvm.dec_base_energy = Convert.ToDecimal(dt.Rows[i]["dec_base_energy"].ToString());
                    objvm.dec_base_rate = Convert.ToDecimal(dt.Rows[i]["dec_base_rate"].ToString());

                    objvm.dec_inter_energy = Convert.ToDecimal(dt.Rows[i]["dec_inter_energy"].ToString());
                    objvm.dec_inter_energy_amt = Convert.ToDecimal(dt.Rows[i]["dec_inter_energy_amt"].ToString());
                    objvm.dec_inter_energy_rate = Convert.ToDecimal(dt.Rows[i]["dec_inter_energy_rate"].ToString());

                    objvm.dec_peak_energy = Convert.ToDecimal(dt.Rows[i]["dec_peak_energy"].ToString());
                    objvm.dec_peak_energy_amt = Convert.ToDecimal(dt.Rows[i]["dec_peak_energy_amt"].ToString());
                    objvm.dec_peak_energy_rate = Convert.ToDecimal(dt.Rows[i]["dec_peak_energy_rate"].ToString());
                    objvm.str_meter_id = dt.Rows[i]["str_meter_id"].ToString();

                    objvm.demanda_base = Convert.ToDecimal(dt.Rows[i]["demanda_base"].ToString());
                    objvm.demanda_intermedia = Convert.ToDecimal(dt.Rows[i]["demanda_intermedia"].ToString());
                    objvm.demanda_punta = Convert.ToDecimal(dt.Rows[i]["demanda_punta"].ToString());
                    objvm.energia_activa = Convert.ToDecimal(dt.Rows[i]["energia_activa"].ToString());
                    objvm.energia_reactiva = Convert.ToDecimal(dt.Rows[i]["energia_reactiva"].ToString());

                    objbal.tenant_invoice_details_insert(objvm);
                }*/

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

                
                /*if (_lVal > 0)
                {

                }*/

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            ViewBag._erromsg = _lVal;
            return View(objvm);
        }

        [SessionCheck]
        [HttpGet]
        public ActionResult ModifyInvoice(int invoice_id)
        {

            InvoiceVM _objvm = new InvoiceVM();
           
            try
            {
                int id = 0;

                var _invoice = _dbc.tbl_invoice.Where(x => x.int_invoice_id == invoice_id).FirstOrDefault();
                var tarifas = _dbc.tbl_tarifas.FirstOrDefault();

                if (_invoice != null)
                {
                    _objvm.int_invoice_id = invoice_id;
                    id = Convert.ToInt32(Convert.ToInt32(_invoice.int_tenant_id));

                    int _uid = Convert.ToInt32(Session["uid"].ToString());
                    var _pmbillingtime = _dbc.tbl_pm_billing_hours.Where(x => x.int_pm_id == _uid).SingleOrDefault();
                    if (_pmbillingtime != null /*&& _ratesinfo != null*/)
                    {

                        _objvm.str_inter_e_time = _pmbillingtime.str_inter_e_time_1_m;
                        _objvm.str_inter_s_time = _pmbillingtime.str_inter_s_time_2_m;

                        _objvm.str_peak_e_time = _pmbillingtime.str_peak_e_time_m;
                        _objvm.str_peak_s_time = _pmbillingtime.str_peak_s_time_m;

                        _objvm.int_invoice_period = 3;
                    }

                    if (tarifas != null)
                    {
                        var _tarifasvm = new TarifasVM();

                        _tarifasvm.dec_base_rate = tarifas.dec_base_rate;
                        _tarifasvm.dec_inter_energy_rate = tarifas.dec_inter_energy_rate;
                        _tarifasvm.dec_peak_energy_rate = tarifas.dec_peak_energy_rate;
                        _tarifasvm.suministro = tarifas.suministro;
                        _tarifasvm.distribucion = tarifas.distribucion;
                        _tarifasvm.tarifa_transmision = tarifas.tarifa_transmision;
                        _tarifasvm.operacion_cenace = tarifas.operacion_cenace;
                        _tarifasvm.capacidad = tarifas.capacidad;
                        _tarifasvm.cre_servicios_conexos = tarifas.cre_servicios_conexos;

                        var meses = new SelectList(new List<SelectListItem>()
                        {
                                new SelectListItem(){ Value="Enero", Text="Enero"},
                                new SelectListItem(){ Value="Febrero", Text="Febrero"},
                                new SelectListItem(){ Value="Marzo", Text="Marzo"},
                                new SelectListItem(){ Value="Abril", Text="Abril"},
                                new SelectListItem(){ Value="Mayo", Text="Mayo"},
                                new SelectListItem(){ Value="Junio", Text="Junio"},
                                new SelectListItem(){ Value="Julio", Text="Julio"},
                                new SelectListItem(){ Value="Agosto", Text="Agosto"},
                                new SelectListItem(){ Value="Septiembre", Text="Septiembre"},
                                new SelectListItem(){ Value="Octubre", Text="Octubre"},
                                new SelectListItem(){ Value="Noviembre", Text="Noviembre"},
                                new SelectListItem(){ Value="Diciembre", Text="Diciembre"},
                        },
                            "Value",
                            "Text");


                        var anios = new SelectList(new List<SelectListItem>()
                        {
                            new SelectListItem() { Value = Convert.ToString(DateTime.Now.Year), Text = Convert.ToString(DateTime.Now.Year) },
                            new SelectListItem() { Value = Convert.ToString(DateTime.Now.Year - 1), Text = Convert.ToString(DateTime.Now.Year - 1) },
                        },
                            "Value",
                            "Text");

                        _tarifasvm.Meses = meses;
                        _tarifasvm.Anios = anios;

                        _tarifasvm.mes_tarifas = tarifas.mes_tarifas;
                        _tarifasvm.ano_tarifas = Convert.ToInt32(tarifas.ano_tarifas);


                        _objvm.tarifassetting = _tarifasvm;
                    }
                }
                
                ViewBag._erromsg = -1;
                
                _objvm.int_tenant_id = id;
                _objvm.str_meter_id = _invoice.str_meter_id;
                _objvm.date_s_bill_date = _invoice.date_s_bill_date;
                _objvm.date_e_bill_date = _invoice.date_e_bill_date;
                _objvm.date_pay_date = _invoice.date_pay_date;
                _objvm.dec_base_energy = decimal.Round((decimal)_invoice.dec_base_energy, 2);
                _objvm.dec_inter_energy = decimal.Round((decimal)_invoice.dec_inter_energy, 2);
                _objvm.dec_peak_energy = decimal.Round((decimal)_invoice.dec_peak_energy, 2);
                _objvm.dec_base_rate = _invoice.dec_base_rate;
                _objvm.dec_inter_energy_rate = _invoice.dec_inter_energy_rate;
                _objvm.dec_peak_energy_rate = _invoice.dec_peak_energy_rate;
                _objvm.demanda_base = decimal.Round((decimal)_invoice.demanda_base, 2);
                _objvm.demanda_intermedia = decimal.Round((decimal)_invoice.demanda_intermedia, 2);
                _objvm.demanda_punta = decimal.Round((decimal)_invoice.demanda_punta, 2);
                _objvm.suministro = decimal.Round((decimal)_invoice.suministro, 2);
                _objvm.distribucion = decimal.Round((decimal)_invoice.distribucion, 2);
                _objvm.tarifa_transmision = _invoice.tarifa_transmision;
                _objvm.operacion_cenace = _invoice.operacion_cenace;
                _objvm.cre_servicios_conexos = _invoice.cre_servicios_conexos;
                _objvm.capacidad = decimal.Round((decimal)_invoice.capacidad, 2);
                _objvm.energia_reactiva = decimal.Round((decimal)_invoice.energia_reactiva, 2);
                _objvm.lectura_energia_base_anterior = (_invoice.lectura_energia_base_anterior != null) ? decimal.Round((decimal)_invoice.lectura_energia_base_anterior, 2) : 0;
                _objvm.lectura_energia_base_actual = (_invoice.lectura_energia_base_actual != null) ? decimal.Round((decimal)_invoice.lectura_energia_base_actual, 2) : 0;
                _objvm.lectura_energia_intermedia_anterior = (_invoice.lectura_energia_intermedia_anterior != null) ? decimal.Round((decimal)_invoice.lectura_energia_intermedia_anterior, 2) : 0;
                _objvm.lectura_energia_intermedia_actual = (_invoice.lectura_energia_intermedia_actual != null) ? decimal.Round((decimal)_invoice.lectura_energia_intermedia_actual, 2) : 0;
                _objvm.lectura_energia_punta_anterior = (_invoice.lectura_energia_punta_anterior != null) ? decimal.Round((decimal)_invoice.lectura_energia_punta_anterior, 2) : 0;
                _objvm.lectura_energia_punta_actual = (_invoice.lectura_energia_punta_actual != null) ? decimal.Round((decimal)_invoice.lectura_energia_punta_actual, 2) : 0;
                _objvm.lectura_energia_reactiva_anterior = (_invoice.lectura_energia_reactiva_anterior != null) ? decimal.Round((decimal)_invoice.lectura_energia_reactiva_anterior, 2) : 0;
                _objvm.lectura_energia_reactiva_actual = (_invoice.lectura_energia_reactiva_actual != null) ? decimal.Round((decimal)_invoice.lectura_energia_reactiva_actual, 2) : 0;
                _objvm.precio_suministro = decimal.Round((decimal)_invoice.precio_suministro, 2);
                _objvm.precio_distribucion = decimal.Round((decimal)_invoice.precio_distribucion, 2);
                _objvm.precio_transmision = decimal.Round((decimal)_invoice.precio_transmision, 2);
                _objvm.precio_cenace = decimal.Round((decimal)_invoice.precio_cenace, 2);
                _objvm.precio_energia = decimal.Round((decimal)_invoice.precio_energia, 2);
                _objvm.precio_capacidad = decimal.Round((decimal)_invoice.precio_capacidad, 2);
                _objvm.precio_cre_servicios_conexos = decimal.Round((decimal)_invoice.precio_cre_servicios_conexos, 2);
                _objvm.precio_dos_porciento_baja_tension = decimal.Round((decimal)_invoice.precio_dos_porciento_baja_tension, 2);
                _objvm.precio_decuento_bonificacion = decimal.Round((decimal)_invoice.precio_decuento_bonificacion, 2);
                _objvm.dec_tax_amt = decimal.Round((decimal)_invoice.dec_tax_amt, 2);
                _objvm.dec_total = decimal.Round((decimal)_invoice.dec_total, 2);
                _objvm.int_tenant_id = _invoice.int_tenant_id;
                _objvm.bit_tenant_active = _invoice.bit_tenant_active;
                _objvm.dec_base_amt = _invoice.dec_base_amt;
                _objvm.dec_inter_energy_amt = _invoice.dec_inter_energy_amt;
                _objvm.dec_peak_energy_amt = _invoice.dec_peak_energy_amt;

                return View(_objvm);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }

        [HttpPost]
        public ActionResult ModifyInvoice(InvoiceVM objvm)
        {
            int _lVal = 0;
            ViewBag._erromsg = 0;
            try
            {
                TenantBAL objbal = new TenantBAL();

                DataTable dt = (DataTable)Session["tenantDT"];
                objbal = new TenantBAL();

                var tarifas = _dbc.tbl_tarifas.FirstOrDefault();

                if (tarifas != null)
                {
                    var _tarifasvm = new TarifasVM();

                    _tarifasvm.dec_base_rate = tarifas.dec_base_rate;
                    _tarifasvm.dec_inter_energy_rate = tarifas.dec_inter_energy_rate;
                    _tarifasvm.dec_peak_energy_rate = tarifas.dec_peak_energy_rate;
                    _tarifasvm.suministro = tarifas.suministro;
                    _tarifasvm.distribucion = tarifas.distribucion;
                    _tarifasvm.tarifa_transmision = tarifas.tarifa_transmision;
                    _tarifasvm.operacion_cenace = tarifas.operacion_cenace;
                    _tarifasvm.capacidad = tarifas.capacidad;
                    _tarifasvm.cre_servicios_conexos = tarifas.cre_servicios_conexos;

                    var meses = new SelectList(new List<SelectListItem>()
                        {
                                new SelectListItem(){ Value="Enero", Text="Enero"},
                                new SelectListItem(){ Value="Febrero", Text="Febrero"},
                                new SelectListItem(){ Value="Marzo", Text="Marzo"},
                                new SelectListItem(){ Value="Abril", Text="Abril"},
                                new SelectListItem(){ Value="Mayo", Text="Mayo"},
                                new SelectListItem(){ Value="Junio", Text="Junio"},
                                new SelectListItem(){ Value="Julio", Text="Julio"},
                                new SelectListItem(){ Value="Agosto", Text="Agosto"},
                                new SelectListItem(){ Value="Septiembre", Text="Septiembre"},
                                new SelectListItem(){ Value="Octubre", Text="Octubre"},
                                new SelectListItem(){ Value="Noviembre", Text="Noviembre"},
                                new SelectListItem(){ Value="Diciembre", Text="Diciembre"},
                        },
                        "Value",
                        "Text");


                    var anios = new SelectList(new List<SelectListItem>()
                        {
                            new SelectListItem() { Value = Convert.ToString(DateTime.Now.Year), Text = Convert.ToString(DateTime.Now.Year) },
                            new SelectListItem() { Value = Convert.ToString(DateTime.Now.Year - 1), Text = Convert.ToString(DateTime.Now.Year - 1) },
                        },
                        "Value",
                        "Text");

                    _tarifasvm.Meses = meses;
                    _tarifasvm.Anios = anios;

                    _tarifasvm.mes_tarifas = tarifas.mes_tarifas;
                    _tarifasvm.ano_tarifas = Convert.ToInt32(tarifas.ano_tarifas);


                    objvm.tarifassetting = _tarifasvm;
                }

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


                /*if (_lVal > 0)
                {

                }*/

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            ViewBag._erromsg = _lVal;
            return View(objvm);
        }

        [HttpGet]
        public ActionResult _ModifyRates()
        {
            TarifasVM _tarifasvm = new TarifasVM();

            var tarifas = _dbc.tbl_tarifas.FirstOrDefault();

            try
            {

                if (tarifas != null)
                {
                    _tarifasvm.dec_base_rate = tarifas.dec_base_rate;
                    _tarifasvm.dec_inter_energy_rate = tarifas.dec_inter_energy_rate;
                    _tarifasvm.dec_peak_energy_rate = tarifas.dec_peak_energy_rate;
                    _tarifasvm.suministro = tarifas.suministro;
                    _tarifasvm.distribucion = tarifas.distribucion;
                    _tarifasvm.tarifa_transmision = tarifas.tarifa_transmision;
                    _tarifasvm.operacion_cenace = tarifas.operacion_cenace;
                    _tarifasvm.capacidad = tarifas.capacidad;
                    _tarifasvm.cre_servicios_conexos = tarifas.cre_servicios_conexos;
                    _tarifasvm.mes_tarifas = _tarifasvm.mes_tarifas;
                    _tarifasvm.ano_tarifas = _tarifasvm.ano_tarifas;
                }
                ViewBag._erromsg = -1;
                return View(_tarifasvm);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }

        [HttpPost]
        public ActionResult _ModifyRates(TarifasVM _tarifasvm)
        {
            TenantBAL objbal = new TenantBAL();

            int _lVal = 0;
            ViewBag._erromsg = 0;

            try
            {
                if (_tarifasvm.int_tarifas_id == 0)
                {
                    _lVal = objbal.rates_insert_update(_tarifasvm);
                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            //ViewBag._erromsg = _lVal;
            return Json(_lVal.ToString(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string getRates(TarifasVM _tarifasvm)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            try
            {
                var tarifas = _dbc.tbl_tarifas.Where(x => x.mes_tarifas == _tarifasvm.mes_tarifas && x.ano_tarifas == _tarifasvm.ano_tarifas).ToList();

                return serializer.Serialize(tarifas);

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return serializer.Serialize(null);
        }

        [SessionCheck]
        [HttpGet]
        public ActionResult CreateInvoices()
        {
            InvoiceVM _objvm = new InvoiceVM();

            int _uid = Convert.ToInt32(Session["uid"].ToString());

            var _tenants = _dbc.tbl_user_master.Where(x => x.int_user_type_id == 3 && x.int_pm_id == _uid).ToList();
            var _tenantID = _tenants.FirstOrDefault().int_id;

            try
            {

                var _tenantmeter = _dbc.tbl_tenant_meter.Where(x => x.int_tenant_id == _tenantID && x.bit_is_assign == true).FirstOrDefault();

                if (_tenantmeter != null)
                {

                    _objvm.int_invoice_id = 0;

                    var _tenantbillinginfo = _dbc.tbl_tenant_billing_info.Where(x => x.int_tenant_id == _tenantID).SingleOrDefault();

                    if (_tenantbillinginfo != null)
                    {
                        _objvm.dec_peak_energy_rate = _tenantbillinginfo.dec_seasonal_multi_rate;
                        _objvm.dec_inter_energy_rate = _tenantbillinginfo.dec_surcharge_amt;
                        _objvm.dec_base_rate = _tenantbillinginfo.dec_rate;
                    }

                    var _pmbillingtime = _dbc.tbl_pm_billing_hours.Where(x => x.int_pm_id == _uid).SingleOrDefault();
                    if (_pmbillingtime != null)
                    {

                        _objvm.str_inter_e_time = _pmbillingtime.str_inter_e_time_1_m;
                        _objvm.str_inter_s_time = _pmbillingtime.str_inter_s_time_2_m;

                        _objvm.str_peak_e_time = _pmbillingtime.str_peak_e_time_m;
                        _objvm.str_peak_s_time = _pmbillingtime.str_peak_s_time_m;

                        _objvm.int_invoice_period = _tenantbillinginfo.tbl_user_master.int_invoice_period;
                    }

                    var _pmsetting = _dbc.tbl_tenant_settings.Where(x => x.int_tenant_id == _tenantID).SingleOrDefault();

                    // This section not used anymore in current version
                    /*if (_pmsetting != null)
                    {
                        _objvm.dec_demanda_facturable = _pmsetting.dec_demanda_facturable;
                        _objvm.dec_total_ene = _pmsetting.dec_total_ene;
                    }
                    else
                    {
                        _objvm.dec_demanda_facturable = 0;
                        _objvm.dec_total_ene = 0;
                    }*/

                }
                ViewBag._erromsg = -1;
                _objvm.int_tenant_id = _tenantID;

                return View(_objvm);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateInvoices(InvoiceVM objvm1)
        {
            int _lresult = 0;
            int _lVal = 0;
            decimal _totalamt = 0;
            ViewBag._erromsg = 0;


            TenantEnergyVM _tenant = new TenantEnergyVM();
            TenantBAL objbal = new TenantBAL();
            InvoiceVM objvm = new InvoiceVM();



            try
            {

                int pmid = Convert.ToInt32(Session["uid"].ToString());

                var _tenants = _dbc.tbl_user_master.Where(x => x.int_user_type_id == 3 && x.int_pm_id == pmid).ToList();

                foreach (var user in _tenants)
                {

                    // Tengo que hacer una copia del TenantVM (manualmente) porque hay que crear uno nuevo con cada iteración para poder actualizar
                    // El que se pasa como parámetro no permite actualizarlo

                    objvm.bit_tenant_active = true;
                    objvm.bit_is_editable = true;
                    objvm.date_s_bill_date = objvm1.date_s_bill_date;
                    objvm.date_e_bill_date = objvm1.date_e_bill_date;
                    objvm.date_pay_date = objvm1.date_pay_date;
                    objvm.dec_base_rate = objvm1.dec_base_rate;
                    objvm.dec_peak_energy_rate = objvm1.dec_peak_energy_rate;
                    objvm.dec_inter_energy_rate = objvm1.dec_inter_energy_rate;
                    objvm.suministro = objvm1.suministro;
                    objvm.distribucion = objvm1.distribucion;
                    objvm.tarifa_transmision = objvm1.tarifa_transmision;
                    objvm.operacion_cenace = objvm1.operacion_cenace;
                    objvm.capacidad = objvm1.capacidad;
                    objvm.cre_servicios_conexos = objvm1.cre_servicios_conexos;
                    objvm.demanda_base = objvm1.demanda_base;
                    objvm.demanda_intermedia = objvm1.demanda_intermedia;
                    objvm.demanda_punta = objvm1.demanda_punta;
                    objvm.energia_reactiva = objvm1.energia_reactiva;
                    objvm.energia_activa = objvm1.energia_activa;


                    _totalamt = 0;

                    var _tenant_billing_info = _dbc.tbl_tenant_billing_info.Where(x => x.int_tenant_id == user.int_id).FirstOrDefault();

                    _tenant.int_tenant_id = Convert.ToInt32(_tenant_billing_info.int_tenant_id);
                    _tenant.dec_base_rate = Convert.ToDecimal(objvm.dec_base_rate);
                    _tenant.dec_peak_energy_rate = Convert.ToDecimal(objvm.dec_peak_energy_rate);
                    _tenant.dec_inter_energy_rate = Convert.ToDecimal(objvm.dec_inter_energy_rate);
                    _tenant.date_s_bill_date = Convert.ToDateTime(objvm.date_s_bill_date);
                    _tenant.date_e_bill_date = Convert.ToDateTime(objvm.date_e_bill_date);

                    _lresult = objbal.tenant_billing_rates_update(_tenant);

                    if ((_lresult == 0) || (_lresult == -2))
                            break;

                    DataTable dt = objbal.getEnergyConsumption(_tenant);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow row = dt.Rows[i];
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            switch (dt.Columns[j].ColumnName)
                            {
                                case "dec_base_amt":
                                    _totalamt = _totalamt + Convert.ToInt32(row.ItemArray[j]);
                                    break;

                                case "dec_inter_energy_amt":
                                    _totalamt = _totalamt + Convert.ToInt32(row.ItemArray[j]);
                                    break;

                                case "dec_peak_energy_amt":
                                    _totalamt = _totalamt + Convert.ToInt32(row.ItemArray[j]);
                                    break;
                            }
                        }
                    }

                    //decimal demandfact = (_totalamt /(decimal)objvm.dec_total_ene) * (decimal)objvm.dec_demanda_facturable;
                    //decimal cervantescustomcharges = (_totalamt + demandfact) * (decimal)0.15;
                    decimal taxval = ((_totalamt) * 16) / 100;
                    decimal finaltotal = _totalamt + taxval;

                    objvm.int_tenant_id = _tenant_billing_info.int_tenant_id;
                    objvm.dec_total = finaltotal;
                    objvm.dec_tax_amt = taxval;

                    _lVal = objbal.tenant_invoice_insert(objvm);

                    if ((_lresult == 0) || (_lresult == -2))
                        break;

                    objvm.int_invoice_id = _lVal;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow row = dt.Rows[i];
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            switch (dt.Columns[j].ColumnName)
                            {
                                case "dec_base_amt":
                                    objvm.dec_base_amt = Convert.ToDecimal(row.ItemArray[j]);
                                    break;

                                case "dec_base_energy":
                                    objvm.dec_base_energy = Convert.ToDecimal(row.ItemArray[j]);
                                    break;

                                case "dec_base_rate":
                                    objvm.dec_base_rate = Convert.ToDecimal(row.ItemArray[j]);
                                    break;

                                case "dec_inter_energy_amt":
                                    objvm.dec_inter_energy_amt = Convert.ToDecimal(row.ItemArray[j]);
                                    break;

                                case "dec_inter_energy":
                                    objvm.dec_inter_energy = Convert.ToDecimal(row.ItemArray[j]);
                                    break;

                                case "dec_inter_energy_rate":
                                    objvm.dec_inter_energy_rate = Convert.ToDecimal(row.ItemArray[j]);
                                    break;

                                case "dec_peak_energy_amt":
                                    objvm.dec_peak_energy_amt = Convert.ToDecimal(row.ItemArray[j]);
                                    break;

                                case "dec_peak_energy":
                                    objvm.dec_peak_energy = Convert.ToDecimal(row.ItemArray[j]);
                                    break;

                                case "dec_peak_energy_rate":
                                    objvm.dec_peak_energy_rate = Convert.ToDecimal(row.ItemArray[j]);
                                    break;

                                case "str_meter_id":
                                    objvm.str_meter_id = Convert.ToString(row.ItemArray[j]);
                                    break;

                            }
                        }
                        _lresult = objbal.tenant_invoice_details_insert(objvm);
                        if ((_lresult == 0) || (_lresult == -2))
                            break;
                        objvm = new InvoiceVM();
                        objvm.int_invoice_id = _lVal;
                    }
                }
             }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            ViewBag._erromsg = _lresult;
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
                //decimal? _last_peak_energy = 0, _last_inter_energy = 0;
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
                objvam.str_meter_id = _period.ToString();
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
                        
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //string json = string.Empty;
            string _meterid = "";
            DataTable dt = new DataTable();
            
            try
            {

                
                //DB_TenantMNGEntities _dbctenant = new DB_TenantMNGEntities();
                /*var _tenantmeter = _dbctenant.tbl_tenant_meter.Where(x => x.int_tenant_id == objvm.int_tenant_id).ToList();

                if (_tenantmeter != null && _tenantmeter.Count > 0)
                {
                    foreach (var m in _tenantmeter)
                    {
                        _meterid = _meterid + "," + m.str_meter_id.ToString();

                    }
                }*/

                _meterid = objvm.str_meter_id;

               UdisEntities _dbc = new UdisEntities();

                string[] _meteridarray = _meterid.ToString().Trim(',').Split(',');


                /*foreach (var m in _meteridarray)
                {
                    var _tenantmetername = _dbc.UDIS.Where(x => x.CFE_MeterID == m).OrderByDescending(p => p.fecha_ocurrencia).FirstOrDefault();

                    _metername = _metername + "," + _tenantmetername.CFE_MeterID;
                }*/



                dt.Columns.AddRange(new DataColumn[17] { new DataColumn("metername", typeof(string)),
                            new DataColumn("energia_punta_lectura_anterior", typeof(decimal)),
                            new DataColumn("energia_punta_lectura_actual", typeof(decimal)),
                            new DataColumn("dec_peak_energy_rate",typeof(decimal)),
                            new DataColumn("energia_intermedia_lectura_anterior",typeof(decimal)),
                            new DataColumn("energia_intermedia_lectura_actual",typeof(decimal)),
                            new DataColumn("dec_inter_energy_rate",typeof(decimal)),
                            new DataColumn("energia_base_lectura_anterior", typeof(decimal)),
                            new DataColumn("energia_base_lectura_actual", typeof(decimal)),
                            new DataColumn("dec_base_rate",typeof(decimal)),
                            new DataColumn("demanda_base",typeof(decimal)),
                            new DataColumn("demanda_intermedia",typeof(decimal)),
                            new DataColumn("demanda_punta",typeof(decimal)),
                            new DataColumn("energia_activa",typeof(decimal)),
                            new DataColumn("energia_reactiva_lectura_anterior",typeof(decimal)),
                            new DataColumn("energia_reactiva_lectura_actual",typeof(decimal)),
                            new DataColumn("str_meter_id",typeof(string)),});



                if (string.IsNullOrEmpty(_meterid) == false)
                {



                    /*var _hours = (from t1 in _dbctenant.tbl_pm_billing_hours
                                  let t2s = from t2 in _dbctenant.tbl_user_master
                                            where t2.int_id == objvm.int_tenant_id
                                            select t2.int_pm_id

                                  where t2s.Contains(t1.int_pm_id)

                                  select t1).FirstOrDefault();*/

                    MeterCLS obj = new MeterCLS();


                    //string[] _mname = _metername.Trim(',').Split(',');

                    //int i = 0;
                    //foreach (var m in _mname)
                    //{

                    var multiplier = obj.getMeterMultiplier(_meterid);

                    DataSet _ds = obj.getTenantEnergy(_meterid, objvm.date_s_bill_date.ToString(), objvm.date_e_bill_date.ToString(), multiplier);

                    if (_ds.Tables[0].Rows.Count > 0)
                    {
                        decimal EnergiaBaseLecturaAnterior = (!(_ds.Tables[0].Rows[0][0] == DBNull.Value)) ? Convert.ToDecimal(_ds.Tables[0].Rows[0][0]) : 0;
                        decimal EnergiaBaseLecturaActual = (!(_ds.Tables[1].Rows[0][0] == DBNull.Value)) ? Convert.ToDecimal(_ds.Tables[1].Rows[0][0]) : 0;
                        decimal EnergiaIntermediaLecturaAnterior = (!(_ds.Tables[2].Rows[0][0] == DBNull.Value)) ? Convert.ToDecimal(_ds.Tables[2].Rows[0][0]) : 0;
                        decimal EnergiaIntermediaLecturaActual = (!(_ds.Tables[3].Rows[0][0] == DBNull.Value)) ? Convert.ToDecimal(_ds.Tables[3].Rows[0][0]) : 0;
                        decimal EnergiaPuntaLecturaAnterior = (!(_ds.Tables[4].Rows[0][0] == DBNull.Value)) ? Convert.ToDecimal(_ds.Tables[4].Rows[0][0]) : 0;
                        decimal EnergiaPuntaLecturaActual = (!(_ds.Tables[5].Rows[0][0] == DBNull.Value)) ? Convert.ToDecimal(_ds.Tables[5].Rows[0][0]) : 0;
                        decimal EnergiaReactivaLecturaAnterior = (!(_ds.Tables[6].Rows[0][0] == DBNull.Value)) ? Convert.ToDecimal(_ds.Tables[6].Rows[0][0]) : 0;
                        decimal EnergiaReactivaLecturaActual = (!(_ds.Tables[7].Rows[0][0] == DBNull.Value)) ? Convert.ToDecimal(_ds.Tables[7].Rows[0][0]) : 0;
                        decimal EnergiaActiva = (!(_ds.Tables[8].Rows[0][0] == DBNull.Value)) ? Convert.ToDecimal(_ds.Tables[8].Rows[0][0]) : 0;
                        decimal DemandaBase = (!(_ds.Tables[9].Rows[0][0] == DBNull.Value)) ? Convert.ToDecimal(_ds.Tables[9].Rows[0][0]) : 0;
                        decimal DemandaIntermedia = (!(_ds.Tables[10].Rows[0][0] == DBNull.Value)) ? Convert.ToDecimal(_ds.Tables[10].Rows[0][0]) : 0;
                        decimal DemandaPunta = (!(_ds.Tables[11].Rows[0][0] == DBNull.Value)) ? Convert.ToDecimal(_ds.Tables[11].Rows[0][0]) : 0;

                        dt.Rows.Add(_meterid, EnergiaPuntaLecturaAnterior, EnergiaPuntaLecturaActual, objvm.dec_peak_energy_rate, EnergiaIntermediaLecturaAnterior, EnergiaIntermediaLecturaActual,
                            objvm.dec_inter_energy_rate, EnergiaBaseLecturaAnterior, EnergiaBaseLecturaActual, objvm.dec_base_rate,
                            DemandaBase, DemandaIntermedia, DemandaPunta, EnergiaActiva, EnergiaReactivaLecturaAnterior, EnergiaReactivaLecturaActual, _meteridarray[0]);
                     }

                     //i++;

                    

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

                /*    case "CC Desc":
                        _invoice = _invoice.OrderByDescending(x => x.dec_custome_charges);
                        break;

                    case "CC":
                        _invoice = _invoice.OrderBy(x => x.dec_custome_charges);
                        break; */


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
            UdisEntities _dbmeter = new UdisEntities();

            var _meter = _dbmeter.UDIS.Where(x => x.CFE_MeterID == x.CFE_MeterID).ToList();

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

                _objvm.dec_tax_amt = _invoice.dec_tax_amt;
                _objvm.dec_total = _invoice.dec_total;

                _objvm.date_pay_date = _invoice.date_pay_date;

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
                            @TenantMNG.Core.CommonCls.getMeterNamefromId(list[i].str_meter_id),
                           //list[i]@TenantMNG.Core.CommonCls.getMeterNamefromId(dt.meterid).getMeterName(int tenantid),
                           list[i].date_s_bill_date.Value.ToString("dd/MM/yyyy") + " To " + list[i].date_e_bill_date.Value.ToString("dd/MM/yyyy"),
                           string.Format("{0:0.00}", list[i].dec_peak_energy),
                          string.Format("{0:0.00}", list[i].dec_inter_energy),
                           //string.Format("{0:0.00}", list[i].dec_custome_charges),
                          //string.Format("{0:0.00}", list[i].dec_demad),
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
                           string.Format("{0:0.00}", list[i].dec_peak_energy),
                           null,
                           null,
                           null,
                           null,
                           //string.Format("{0:0.00}", list[i].tbl_invoice_details.ElementAt(0).dec_inter_energy),
                           //string.Format("{0:0.00}", list[i].dec_demanda_facturable),
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
                        html = html.Replace("#Tenantname", _invoice.tbl_user_master.str_comp_name);
                        html = html.Replace("#TenantAdd1", _invoice.tbl_user_master.str_add_1);
                        html = html.Replace("#TenantAdd2", _invoice.tbl_user_master.str_add_2);
                        html = html.Replace("#TCity", _invoice.tbl_user_master.str_city);
                        html = html.Replace("#TS", _invoice.tbl_user_master.str_state);
                        html = html.Replace("#TC", _invoice.tbl_user_master.str_country);
                        html = html.Replace("#VaueMax", string.Format("{0:0}", objuser1.ElementAt(0).maxObject.VALUE));
                        html = html.Replace("#VaueMin", string.Format("{0:0 }", objuser1.ElementAt(0).minObject.VALUE));
                        html = html.Replace("#usese", a.ToString());
                        html = html.Replace("#days", _invoice.date_e_bill_date.Value.Date.Subtract(_invoice.date_s_bill_date.Value.Date).TotalDays.ToString());
                        html = html.Replace("#energyusage1", CommonCls.DoFormat(_invoice.dec_peak_energy) + " kWh");
                        html = html.Replace("#energyrate1", CommonCls.DoFormat(_invoice.dec_peak_energy_rate));
                        html = html.Replace("#energyamt1", CommonCls.DoFormat(_invoice.dec_peak_energy * _invoice.dec_peak_energy_rate));
                        html = html.Replace("#energyusage2", CommonCls.DoFormat(_invoice.dec_inter_energy) + " kWh");
                        html = html.Replace("#energyrate2", CommonCls.DoFormat(_invoice.dec_inter_energy_rate));

                        html = html.Replace("#energyamt2", CommonCls.DoFormat(_invoice.dec_inter_energy * _invoice.dec_inter_energy_rate));

                        html = html.Replace("#energyusage3", CommonCls.DoFormat(_invoice.dec_base_energy) + " kWh");
                        html = html.Replace("#energyrate3", CommonCls.DoFormat(_invoice.dec_base_rate));
                        html = html.Replace("#energyamt3", CommonCls.DoFormat(_invoice.dec_base_energy * _invoice.dec_base_rate));

                        html = html.Replace("#metername", _invoice.str_meter_id);
                        html = html.Replace("#name", _invoice.tbl_user_master.str_comp_name);
                        html = html.Replace("#billperiod", _invoice.date_s_bill_date.Value.ToString("dd/MM/yyyy") + " AL " + _invoice.date_e_bill_date.Value.ToString("dd/MM/yyyy"));
                        html = html.Replace("#billdate", _invoice.date_invoice_date.Value.ToString("dd/MM/yyyy"));
                        html = html.Replace("#paydate", _invoice.date_pay_date.Value.ToString("dd/MM/yyyy"));
                        html = html.Replace("#invno", _invoice.int_invoice_id.ToString());
                        //html = html.Replace("#customcharge", "$" + CommonCls.DoFormat(_invoice.dec_custome_charges));
                        //html = html.Replace("#customtitle", string.IsNullOrEmpty(_invoice.str_custome_charge_desc) ? "Cargo personalizado" : _invoice.str_custome_charge_desc);
                        //html = html.Replace("#opingpeakkw", CommonCls.DoFormat(_invoice.dec_prev_peack_energy) + " kWh");
                        //html = html.Replace("#closingpeakkw", CommonCls.DoFormat(_invoice.dec_current_peack_energy) + " kWh");
                        //html = html.Replace("#opingintermediatekWh", CommonCls.DoFormat(_invoice.dec_prev_peack_energy) + " kWh");
                        //html = html.Replace("#closingintermediatekWh", CommonCls.DoFormat(_invoice.dec_current_inter_energy) + " kWh");
                        html = html.Replace("#energytype1", "Peak Energy");
                        html = html.Replace("#demandtier1", "Tier 1");
                        //html = html.Replace("#demandusage", CommonCls.DoFormat(_invoice.dec_demad) + " kWh");
                        html = html.Replace("#demandrate", "$0");
                        html = html.Replace("#demandamt", "$0");

                        //html = html.Replace("#chargedesc1", _invoice.str_custome_charge_desc);
                        html = html.Replace("#chargetype1", "Flat Type");
                        //html = html.Replace("#customeunit1", "$" + CommonCls.DoFormat(_invoice.dec_custome_charges));
                        html = html.Replace("#customerate1", "$0");
                        //html = html.Replace("#customamt1", "$" + CommonCls.DoFormat(_invoice.dec_custome_charges));

                        //   html = html.Replace("#chargedesc2", "IVA 16%");
                        html = html.Replace("#chargetype2", "Percentage");
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

                        html = html.Replace("#preciosuministro", "$" + string.Format("{0:n}", CommonCls.DoFormat(_invoice.precio_suministro)));
                        html = html.Replace("#preciodistribucion", "$" + string.Format("{0:n}", CommonCls.DoFormat(_invoice.precio_distribucion)));
                        html = html.Replace("#preciotransmision", "$" + string.Format("{0:n}", CommonCls.DoFormat(_invoice.precio_transmision)));
                        html = html.Replace("#preciocenace", "$" + string.Format("{0:n}", CommonCls.DoFormat(_invoice.precio_cenace)));
                        html = html.Replace("#precioenergia", "$" + string.Format("{0:n}", CommonCls.DoFormat(_invoice.precio_energia)));
                        html = html.Replace("#preciocapacidad", "$" + string.Format("{0:n}", CommonCls.DoFormat(_invoice.precio_capacidad)));
                        html = html.Replace("#preciocreserviciosconexos", "$" + string.Format("{0:n}", CommonCls.DoFormat(_invoice.precio_cre_servicios_conexos)));
                        html = html.Replace("#preciodosporcientobajatension", "$" + string.Format("{0:n}", CommonCls.DoFormat(_invoice.precio_dos_porciento_baja_tension)));
                        html = html.Replace("#preciodecuentobonificacion", "$" + string.Format("{0:n}", CommonCls.DoFormat(_invoice.precio_decuento_bonificacion)));

                        html = html.Replace("#customamt3", "$" + string.Format("{0:n}", CommonCls.DoFormat(_invoice.dec_tax_amt)));
                        html = html.Replace("#ToPayAmount", "$" + string.Format("{0:n}", CommonCls.DoFormat(_invoice.dec_total)));

                        path = Server.MapPath("~/Template/meterlist.html");
                        string replacemeterlist = string.Empty;
                        string meterlisthtml = string.Empty;

                        decimal _totalenergyamt = 0, _totalenergy = 0;

                        //var _invoicedetails = _dbc.tbl_invoice_details.Where(x => x.int_invoice_id == forpdf).ToList();

                        //if (_invoicedetails != null)
                        //{

                        //    foreach (var details in _invoicedetails)
                        //    {
                        meterlisthtml = System.IO.File.ReadAllText(path);

                        meterlisthtml = meterlisthtml.Replace("#metername", _invoice.str_meter_id);

                        meterlisthtml = meterlisthtml.Replace("#energyusage1", CommonCls.DoFormat(_invoice.dec_peak_energy) + " kWh");
                        meterlisthtml = meterlisthtml.Replace("#energyrate1", _invoice.dec_peak_energy_rate.ToString());
                        meterlisthtml = meterlisthtml.Replace("#energyamt1", CommonCls.DoFormat(_invoice.dec_peak_energy_amt));
                        meterlisthtml = meterlisthtml.Replace("#energyusage2", CommonCls.DoFormat(_invoice.dec_inter_energy) + " kWh");
                        meterlisthtml = meterlisthtml.Replace("#energyrate2", _invoice.dec_inter_energy_rate.ToString());
                        meterlisthtml = meterlisthtml.Replace("#energyamt2", CommonCls.DoFormat(_invoice.dec_inter_energy_amt));
                        meterlisthtml = meterlisthtml.Replace("#energyusage3", CommonCls.DoFormat(_invoice.dec_base_energy) + " kWh");
                        meterlisthtml = meterlisthtml.Replace("#energyrate3", _invoice.dec_base_rate.ToString());
                        meterlisthtml = meterlisthtml.Replace("#energyamt3", CommonCls.DoFormat(_invoice.dec_base_amt));
                        meterlisthtml = meterlisthtml.Replace("#totaluse", CommonCls.DoFormat(_invoice.dec_peak_energy.Value + _invoice.dec_inter_energy.Value + _invoice.dec_base_energy.Value));
                        meterlisthtml = meterlisthtml.Replace("#toalamt", CommonCls.DoFormat(_invoice.dec_peak_energy_amt.Value + _invoice.dec_inter_energy_amt.Value + _invoice.dec_base_amt.Value));
                        replacemeterlist = replacemeterlist + "<br/>" + meterlisthtml;
                        _totalenergyamt = _totalenergyamt + (_invoice.dec_peak_energy_amt.Value + _invoice.dec_inter_energy_amt.Value + _invoice.dec_base_amt.Value);
                        _totalenergy = _totalenergy + (_invoice.dec_peak_energy.Value + _invoice.dec_inter_energy.Value + _invoice.dec_base_energy.Value);

                        //    }
                        //}

                        html = html.Replace("#meterid", replacemeterlist);

                        html = html.Replace("#sumofamount", "$" + CommonCls.DoFormat(_totalenergyamt));

                        html = html.Replace("#totalenergy", CommonCls.DoFormat(_totalenergy) + " kWh");

                    }

                    path = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Content("~/");


                    html = html.Replace("#FooterImg", path + "/PDF/charts.jpg");
                    html = html.Replace("#place", path + "/PDF/place.png");
                    html = html.Replace("#hlogo", path + "/PDF/puerta-polanco-logo.png");
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


        private void generateSummaryPDF(InvoiceVM _invoicesvm)
        {
            try
            {
                var invoices = _dbc.tbl_invoice.Where(x => x.int_tenant_id == _invoicesvm.int_tenant_id && x.date_e_bill_date.Value.Month == _invoicesvm.monthnumber).ToList();
                var tenant = _dbc.tbl_user_master.Where(x => x.int_id == _invoicesvm.int_tenant_id).FirstOrDefault();

                string path = Server.MapPath("~/Template/ResumenGeneral.html");
                string html = System.IO.File.ReadAllText(path);
                string filename = "Resumen_" + tenant.str_comp_name + (DateTime.Now).ToString("yyyyMMddmm") + ".PDF";

                Session["summaryfilename"] = filename;
                ViewBag.summaryfilename = filename;

                var date = Convert.ToDateTime(invoices.FirstOrDefault().date_e_bill_date);
                var mes = date.ToString("MMMM", CultureInfo.CreateSpecificCulture("es"));
                var anio = Convert.ToString(date.Year);

                path = HttpContext.Server.MapPath("~/PDF/");
                string cdnFilePath = path + filename;

                var invoicehtmldata = "";

                html = html.Replace("#hlogo", path + "puerta-polanco-logo.png");

                decimal total = 0;

                foreach (var invoice in invoices)
                {
                    tenant = _dbc.tbl_user_master.Where(x => x.int_id == invoice.int_tenant_id).FirstOrDefault();

                    invoicehtmldata += "<tr><td>" + tenant.str_comp_name + "</td>" +
                        "<td>" + CommonCls.getIdforMeter(invoice.str_meter_id) + "</td>" +
                        "<td>" + invoice.str_meter_id + "</td>" +
                        "<td>" + (Convert.ToDouble(invoice.dec_tax_amt) / .16).ToString("N", new CultureInfo("en-US")) + "</td>" +
                        "<td>" + Convert.ToDecimal(invoice.dec_tax_amt).ToString("N", new CultureInfo("en-US")) + "</td>" +
                        "<td>" + Convert.ToDecimal(invoice.dec_total).ToString("N", new CultureInfo("en-US")) + "</td>" + "</tr>";

                    total = total + Convert.ToDecimal(invoice.dec_total);
                }

                html = html.Replace("#invoicedata", invoicehtmldata);

                var totalhtml = "<tr><td></td><td></td><td></td><td></td><td></td><td>" + Convert.ToDecimal(total).ToString("N", new CultureInfo("en-US")) + "</td></tr>";

                html = html.Replace("#totales", totalhtml);

                html = html.Replace("#Mes", mes);
                html = html.Replace("#Anio", anio);

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

        //get order invoice report 
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

                //DataSet ds = new DataSet();
                //string ConnectionString = ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString;
                //using (SqlConnection con = new SqlConnection(ConnectionString))
                //{
                    
                    //Cambiamos toda la basura de código de los hindúes, por algo más eficiente
                    //Todo este código comentado a continuación es de ellos

                    /*SqlCommand cmd = new SqlCommand("[spProcEnergyLog]", con);
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
                    }*/

                var _invoice = _dbc.tbl_invoice.Where(x => x.int_invoice_id == id).FirstOrDefault();
                var _previousinvoice = _dbc.tbl_invoice.Where(x => (x.str_meter_id == _invoice.str_meter_id) && (x.date_invoice_date < _invoice.date_invoice_date)).FirstOrDefault();

                    //var _invoicedet = _dbc.tbl_invoice_details.Where(x => x.int_invoice_id == id).ToList();

                    //con.Open();

                    /*var objuser = userlist.Where(x => x.TENETID == _invoice.int_tenant_id).ToList();
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
                    TimeSpan timeSpan = _invoice.date_e_bill_date.Value.Subtract(_invoice.date_s_bill_date.Value);*/

                if (_invoice != null)
                {

                    #region  Dynamic table

                    var meterReadingsString = "";
                    var currentReading = 0;
                    var previousReading = 0;
                    var demandabase = 0;
                    var demandaintermedia = 0;
                    var demandapunta = 0;
                    var total = 0;
                    /*int currentReading = Convert.ToInt32(_invoice.dec_base_energy + _invoice.dec_inter_energy + _invoice.dec_peak_energy);
                    if (_previousinvoice != null)
                    { 
                         previousReading = Convert.ToInt32(_previousinvoice.dec_base_energy + _invoice.dec_inter_energy + _invoice.dec_peak_energy);
                    }*/


                    //var _invoicedet = objuser1.ToList();
                    /*if (_invoicedet != null)
                    {
                        SqlCommand cmd = new SqlCommand("[getLastMeterReading]", con);

                        cmd.Parameters.Add(new SqlParameter("@meterName", ""));
                        cmd.Parameters.Add(new SqlParameter("@monthDate", null));
                        cmd.Parameters.Add(new SqlParameter("@lastMeterReading", SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output });
                        cmd.CommandType = CommandType.StoredProcedure;

                        foreach (var invoice in _invoicedet)
                        {


                            // Leer valor del medidor del mes en curso
                            //cmd.Parameters["@meterName"].Value = CommonCls.getMeterNamefromId(invoice.str_meter_id);
                            cmd.Parameters["@meterName"].Value = invoice.str_meter_id;
                            cmd.Parameters["@monthDate"].Value = _invoice.date_e_bill_date;
                                
                            cmd.ExecuteNonQuery();

                            int currentReading;
                            if (int.TryParse(Convert.ToString(cmd.Parameters["@lastMeterReading"].Value), out currentReading) == false)
                                currentReading = 0;

                            // Leer valor del medidor del mes anterior
                            cmd.Parameters["@monthDate"].Value = Convert.ToDateTime(_invoice.date_s_bill_date).AddDays(-1);

                            cmd.ExecuteNonQuery();

                            int previousReading;
                            if (int.TryParse(Convert.ToString(cmd.Parameters["@lastMeterReading"].Value), out previousReading) == false)
                                previousReading = 0;*/

                    // Lectura en Energía Base

                    if (_invoice.lectura_energia_base_actual != null)
                        currentReading = Convert.ToInt32(_invoice.lectura_energia_base_actual);
                    if (_invoice.lectura_energia_base_anterior != null)
                        previousReading = Convert.ToInt32(_invoice.lectura_energia_base_anterior);
                    total = currentReading - previousReading;
                    meterReadingsString += "<tr><td style ='font-size:10px;text-align:center;'colspan='4'>" + "Energía Base" + "</td>" +
                        "<td style='font-size:10px; text-align:center;'colspan='4'>" + string.Format("{0:0}", currentReading) + "</td>" +
                        "<td style ='font-size:10px; text-align:center;'colspan='4'>" + string.Format("{0:0 }", previousReading) + "</td>" +
                        "<td style ='font-size:10px; text-align:center;'colspan='4'>" + total.ToString() + "</td>" +
                        "<td style ='font-size:10px; text-align:center;'colspan='4'>" + "kWh" + "</td></tr>";

                    // Lectura en Energía Intermedia

                    if (_invoice.lectura_energia_intermedia_actual != null)
                        currentReading = Convert.ToInt32(_invoice.lectura_energia_intermedia_actual);
                    else
                        currentReading = 0;
                    if (_invoice.lectura_energia_intermedia_anterior != null)
                        previousReading = Convert.ToInt32(_invoice.lectura_energia_intermedia_anterior);
                    else
                        previousReading = 0;
                    total = currentReading - previousReading;
                    meterReadingsString += "<tr><td style ='font-size:10px;text-align:center;'colspan='4'>" + "Energía Intermedia" + "</td>" +
                        "<td style='font-size:10px; text-align:center;'colspan='4'>" + string.Format("{0:0}", currentReading) + "</td>" +
                        "<td style ='font-size:10px; text-align:center;'colspan='4'>" + string.Format("{0:0 }", previousReading) + "</td>" +
                        "<td style ='font-size:10px; text-align:center;'colspan='4'>" + total.ToString() + "</td>" +
                        "<td style ='font-size:10px; text-align:center;'colspan='4'>" + "kWh" + "</td></tr>";

                    // Lectura en Energía Punta

                    if (_invoice.lectura_energia_punta_actual != null)
                        currentReading = Convert.ToInt32(_invoice.lectura_energia_punta_actual);
                    else
                        currentReading = 0;
                    if (_invoice.lectura_energia_punta_anterior != null)
                        previousReading = Convert.ToInt32(_invoice.lectura_energia_punta_anterior);
                    else
                        previousReading = 0;
                    total = currentReading - previousReading;
                    meterReadingsString += "<tr><td style ='font-size:10px;text-align:center;'colspan='4'>" + "Energía Punta" + "</td>" +
                        "<td style='font-size:10px; text-align:center;'colspan='4'>" + string.Format("{0:0}", currentReading) + "</td>" +
                        "<td style ='font-size:10px; text-align:center;'colspan='4'>" + string.Format("{0:0 }", previousReading) + "</td>" +
                        "<td style ='font-size:10px; text-align:center;'colspan='4'>" + total.ToString() + "</td>" +
                        "<td style ='font-size:10px; text-align:center;'colspan='4'>" + "kWh" + "</td></tr>";

                    // Lectura en Energía Reactiva

                    if (_invoice.lectura_energia_reactiva_actual != null)
                        currentReading = Convert.ToInt32(_invoice.lectura_energia_reactiva_actual);
                    else
                        currentReading = 0;
                    if (_invoice.lectura_energia_reactiva_anterior != null)
                        previousReading = Convert.ToInt32(_invoice.lectura_energia_reactiva_anterior);
                    else
                        previousReading = 0;
                    total = currentReading - previousReading;
                    meterReadingsString += "<tr><td style ='font-size:10px;text-align:center;'colspan='4'>" + "Energía Reactiva" + "</td>" +
                        "<td style='font-size:10px; text-align:center;'colspan='4'>" + string.Format("{0:0}", currentReading) + "</td>" +
                        "<td style ='font-size:10px; text-align:center;'colspan='4'>" + string.Format("{0:0 }", previousReading) + "</td>" +
                        "<td style ='font-size:10px; text-align:center;'colspan='4'>" + total.ToString() + "</td>" +
                        "<td style ='font-size:10px; text-align:center;'colspan='4'>" + "kWh" + "</td></tr>";

                    // Demandas

                    if (_invoice.demanda_base != null)
                        demandabase = Convert.ToInt32(_invoice.demanda_base);
                    if (_invoice.demanda_intermedia != null)
                        demandaintermedia = Convert.ToInt32(_invoice.demanda_intermedia);
                    if (_invoice.demanda_punta != null)
                        demandapunta = Convert.ToInt32(_invoice.demanda_punta);
                    meterReadingsString += "<tr><td style ='font-size:10px;text-align:center;'colspan='4'>" + "Demanda Base" + "</td>" +
                        "<td style='font-size:10px; text-align:center;'colspan='4'>" + string.Format("{0:0}", demandabase) + "</td>" +
                        "<td style ='font-size:10px; text-align:center;'colspan='4'></td>" +
                        "<td style ='font-size:10px; text-align:center;'colspan='4'></td>" +
                        "<td style ='font-size:10px; text-align:center;'colspan='4'></td></tr>" +
                        "<tr><td style ='font-size:10px;text-align:center;'colspan='4'>" + "Demanda Int." + "</td>" +
                        "<td style='font-size:10px; text-align:center;'colspan='4'>" + string.Format("{0:0}", demandaintermedia) + "</td>" +
                        "<td style ='font-size:10px; text-align:center;'colspan='4'></td>" +
                        "<td style ='font-size:10px; text-align:center;'colspan='4'></td>" +
                        "<td style ='font-size:10px; text-align:center;'colspan='4'></td></tr>" +
                        "<tr><td style ='font-size:10px;text-align:center;'colspan='4'>" + "Demanda Punta" + "</td>" +
                        "<td style='font-size:10px; text-align:center;'colspan='4'>" + string.Format("{0:0}", demandapunta) + "</td>" +
                        "<td style ='font-size:10px; text-align:center;'colspan='4'></td>" +
                        "<td style ='font-size:10px; text-align:center;'colspan='4'></td>" +
                        "<td style ='font-size:10px; text-align:center;'colspan='4'></td></tr>";

                    html = html.Replace("#read", meterReadingsString);

                    #endregion

                    CharterColumn(_invoice.int_tenant_id, id);
                    string address = _invoice.tbl_user_master.str_add_1 + "<br>" + _invoice.tbl_user_master.str_add_2 + "<br>" + _invoice.tbl_user_master.str_city + "<br>" + _invoice.tbl_user_master.str_state + "<br>" + _invoice.tbl_user_master.str_country;
                    html = html.Replace("#address", address);
                    html = html.Replace("#Piso", _invoice.tbl_user_master.str_add_2);
                    html = html.Replace("#edate", _invoice.date_e_bill_date.Value.ToString("dd/MM/yyyy"));
                    html = html.Replace("#sdate", _invoice.date_s_bill_date.Value.ToString("dd/MM/yyyy"));
                    html = html.Replace("#Statement", _invoice.date_pay_date.Value.ToString("dd/MM/yyyy"));
                    html = html.Replace("#monthname", _invoice.date_pay_date.Value.ToString("MMMM"));
                    html = html.Replace("#Tenantname", _invoice.tbl_user_master.str_comp_name);
                    //html = html.Replace("#VaueMax", string.Format("{0:0}", objuser1.ElementAt(0).maxObject.VALUE));
                    //html = html.Replace("#VaueMin", string.Format("{0:0 }", objuser1.ElementAt(0).minObject.VALUE));
                    //html = html.Replace("#usese", string.Format("{0:0 }", _invoice.dec_total_ene));
                    //html = html.Replace("#VaueMax",objuser.ToString());
                    //html = html.Replace("#VaueMin", objuser1.ToString());
                    html = html.Replace("#TenantAdd1", _invoice.tbl_user_master.str_add_1);
                    html = html.Replace("#TenantAdd2", _invoice.tbl_user_master.str_add_2);
                    html = html.Replace("#TCity", _invoice.tbl_user_master.str_city);
                    html = html.Replace("#TS", _invoice.tbl_user_master.str_state);
                    html = html.Replace("#TC", _invoice.tbl_user_master.str_country);
                    html = html.Replace("#days", _invoice.date_e_bill_date.Value.Date.Subtract(_invoice.date_s_bill_date.Value.Date).TotalDays.ToString());
                    html = html.Replace("#energyusage1", CommonCls.DoFormat(_invoice.dec_peak_energy) + " kWh");
                    html = html.Replace("#energyrate1", CommonCls.DoFormat(_invoice.dec_peak_energy_rate));
                    html = html.Replace("#energyamt1", CommonCls.DoFormat(_invoice.dec_peak_energy * _invoice.dec_peak_energy_rate));
                    html = html.Replace("#energyusage2", CommonCls.DoFormat(_invoice.dec_inter_energy) + " kWh");
                    html = html.Replace("#energyrate2", CommonCls.DoFormat(_invoice.dec_inter_energy_rate));

                    html = html.Replace("#energyamt2", CommonCls.DoFormat(_invoice.dec_inter_energy * _invoice.dec_inter_energy_rate));
                    html = html.Replace("#meternam", CommonCls.getMeterNamefromId(_invoice.str_meter_id));
                    html = html.Replace("#energyusage3", CommonCls.DoFormat(_invoice.dec_base_energy) + " kWh");
                    html = html.Replace("#energyrate3", CommonCls.DoFormat(_invoice.dec_base_rate));
                    html = html.Replace("#energyamt3", CommonCls.DoFormat(_invoice.dec_base_energy * _invoice.dec_base_rate));

                    html = html.Replace("#name", _invoice.tbl_user_master.str_comp_name);
                    html = html.Replace("#billperiod", _invoice.date_s_bill_date.Value.ToString("dd/MM/yyyy") + " AL " + _invoice.date_e_bill_date.Value.ToString("dd/MM/yyyy"));
                    html = html.Replace("#billdate", _invoice.date_invoice_date.Value.ToString("dd/MM/yyyy"));
                    html = html.Replace("#paydate", _invoice.date_pay_date.Value.ToString("dd/MM/yyyy"));
                    html = html.Replace("#invno", _invoice.int_invoice_id.ToString());
                    //html = html.Replace("#customcharge", "$" + CommonCls.DoFormat(_invoice.dec_custome_charges));
                    //html = html.Replace("#customtitle", string.IsNullOrEmpty(_invoice.str_custome_charge_desc) ? "Cargo personalizado" : _invoice.str_custome_charge_desc);

                    //html = html.Replace("#opingpeakkw", CommonCls.DoFormat(_invoice.dec_prev_peack_energy) + " kWh");
                    //html = html.Replace("#closingpeakkw", CommonCls.DoFormat(_invoice.dec_current_peack_energy) + " kWh");
                    //html = html.Replace("#opingintermediatekWh", CommonCls.DoFormat(_invoice.dec_prev_peack_energy) + " kWh");
                    //html = html.Replace("#closingintermediatekWh", CommonCls.DoFormat(_invoice.dec_current_inter_energy) + " kWh");
                    html = html.Replace("#energytype1", "Peak Energy");

                    html = html.Replace("#demandtier1", "Tier 1");
                    //html = html.Replace("#demandusage", CommonCls.DoFormat(_invoice.dec_demad) + " kWh");
                    html = html.Replace("#demandrate", "$0");
                    html = html.Replace("#demandamt", "$0");

                    //html = html.Replace("#chargedesc1", _invoice.str_custome_charge_desc);
                    html = html.Replace("#chargetype1", "Flat Type");
                    //html = html.Replace("#customeunit1", "$" + CommonCls.DoFormat(_invoice.dec_custome_charges));
                    html = html.Replace("#customerate1", "$0");
                    //html = html.Replace("#customamt1", "$" + CommonCls.DoFormat(_invoice.dec_custome_charges));

                    //html = html.Replace("#chargedesc2", "IVA 16%");
                    html = html.Replace("#chargetype2", "Percentage");
                    html = html.Replace("#customamt2", "$" + CommonCls.DoFormat(_invoice.dec_tax_amt));
                    html = html.Replace("#charturl", "I");
                    //html = html.Replace("#demanfacturabledamt", "$" + CommonCls.DoFormat(_invoice.dec_demanda_facturable_amount));
                    //html = html.Replace("#buildingtotal", "$" + _invoice.dec_total_ene.ToString());
                    //html = html.Replace("#demandfinalamt", "$" + CommonCls.DoFormat(_invoice.dec_demanda_facturable_amount));

                    //decimal _finaltotal = _totalenergyamt + _totalfacturable + _invoice.dec_tax_amt.Value;

                    html = html.Replace("#preciosuministro", "$" + Convert.ToDecimal(_invoice.precio_suministro).ToString("N", new CultureInfo("en-US")));
                    html = html.Replace("#preciodistribucion", "$" + Convert.ToDecimal(_invoice.precio_distribucion).ToString("N", new CultureInfo("en-US")));
                    html = html.Replace("#preciotransmision", "$" + Convert.ToDecimal(_invoice.precio_transmision).ToString("N", new CultureInfo("en-US")));
                    html = html.Replace("#preciocenace", "$" + Convert.ToDecimal(_invoice.precio_cenace).ToString("N", new CultureInfo("en-US")));
                    html = html.Replace("#precioenergia", "$" + Convert.ToDecimal(_invoice.precio_energia).ToString("N", new CultureInfo("en-US")));
                    html = html.Replace("#preciocapacidad", "$" + Convert.ToDecimal(_invoice.precio_capacidad).ToString("N", new CultureInfo("en-US")));
                    html = html.Replace("#preciocreserviciosconexos", "$" + Convert.ToDecimal(_invoice.precio_cre_servicios_conexos).ToString("N", new CultureInfo("en-US")));
                    html = html.Replace("#preciodosporcientobajatension", "$" + Convert.ToDecimal(_invoice.precio_dos_porciento_baja_tension).ToString("N", new CultureInfo("en-US")));
                    html = html.Replace("#preciodecuentobonificacion", "$" + Convert.ToDecimal(_invoice.precio_decuento_bonificacion).ToString("N", new CultureInfo("en-US")));

                    html = html.Replace("#customamt3", "$" + Convert.ToDecimal(_invoice.dec_tax_amt).ToString("N", new CultureInfo("en-US")));
                    html = html.Replace("#ToPayAmount", "$" + Convert.ToDecimal(_invoice.dec_total).ToString("N", new CultureInfo("en-US")));

                    html = html.Replace("#totalamt", "$" + Convert.ToDecimal(_invoice.dec_total).ToString("N", new CultureInfo("en-US")));

                    path = Server.MapPath("~/Template/meterlist.html");
                    string replacemeterlist = string.Empty;
                    string meterlisthtml = string.Empty;

                    decimal _totalenergyamt = 0, _totalenergy = 0;

                    //var _invoicedetails = _dbc.tbl_invoice_details.Where(x => x.int_invoice_id == id).ToList();
                    //var invoicecarge = "";
                    //if (_invoicedetails != null)
                    //{

                    //        foreach (var details in _invoicedetails)
                    //        {
                    
                    meterlisthtml = System.IO.File.ReadAllText(path);

                    meterlisthtml = meterlisthtml.Replace("#metername", CommonCls.getIdforMeter(_invoice.str_meter_id));
                    //html = html.Replace("#edate", _invoice.date_e_bill_date.Value.ToString("dd/MM/yyyy"));
                    meterlisthtml = meterlisthtml.Replace("#energyusage1", CommonCls.DoFormat(_invoice.dec_peak_energy) + " kWh");
                    meterlisthtml = meterlisthtml.Replace("#energyrate1", _invoice.dec_peak_energy_rate.ToString());
                    meterlisthtml = meterlisthtml.Replace("#energyamt1", CommonCls.DoFormat(_invoice.dec_peak_energy_amt));
                    meterlisthtml = meterlisthtml.Replace("#energyusage2", CommonCls.DoFormat(_invoice.dec_inter_energy) + " kWh");
                    meterlisthtml = meterlisthtml.Replace("#energyrate2", _invoice.dec_inter_energy_rate.ToString());
                    meterlisthtml = meterlisthtml.Replace("#energyamt2", CommonCls.DoFormat(_invoice.dec_inter_energy_amt));
                    meterlisthtml = meterlisthtml.Replace("#energyusage3", CommonCls.DoFormat(_invoice.dec_base_energy) + " kWh");
                    meterlisthtml = meterlisthtml.Replace("#energyrate3", _invoice.dec_base_rate.ToString());
                    meterlisthtml = meterlisthtml.Replace("#energyamt3", CommonCls.DoFormat(_invoice.dec_base_amt));
                    meterlisthtml = meterlisthtml.Replace("#totaluse", CommonCls.DoFormat(_invoice.dec_peak_energy.Value + _invoice.dec_inter_energy.Value + _invoice.dec_base_energy.Value));
                    meterlisthtml = meterlisthtml.Replace("#toalamt", CommonCls.DoFormat(_invoice.dec_peak_energy_amt.Value + _invoice.dec_inter_energy_amt.Value + _invoice.dec_base_amt.Value));
                    replacemeterlist = replacemeterlist + "<br/>" + meterlisthtml;
                    _totalenergyamt = _totalenergyamt + (_invoice.dec_peak_energy_amt.Value + _invoice.dec_inter_energy_amt.Value + _invoice.dec_base_amt.Value);
                    _totalenergy = _totalenergy + (_invoice.dec_peak_energy.Value + _invoice.dec_inter_energy.Value + _invoice.dec_base_energy.Value);

                    //        }
                    //    }

                    html = html.Replace("#meteridd", replacemeterlist);

                    html = html.Replace("#Meterid", _invoice.str_meter_id);

                    html = html.Replace("#suministrorate", Convert.ToString(_invoice.suministro));

                    html = html.Replace("#distribucionrate", Convert.ToString(_invoice.distribucion));

                    html = html.Replace("#transmisionrate", Convert.ToString(_invoice.tarifa_transmision));

                    html = html.Replace("#ocenacerate", Convert.ToString(_invoice.operacion_cenace));

                    html = html.Replace("#energybase", CommonCls.DoFormat(_invoice.dec_base_energy));
                    html = html.Replace("#energybrate", Convert.ToString(_invoice.dec_base_rate));
                    html = html.Replace("#benergytotal", String.Format("{0:n}", CommonCls.DoFormat(_invoice.dec_base_amt)));

                    html = html.Replace("#energyintermedia", CommonCls.DoFormat(_invoice.dec_inter_energy));
                    html = html.Replace("#energyirate", Convert.ToString(_invoice.dec_inter_energy_rate));
                    html = html.Replace("#ienergytotal", String.Format("{0:n}", CommonCls.DoFormat(_invoice.dec_inter_energy_amt)));

                    html = html.Replace("#energypunta", CommonCls.DoFormat(_invoice.dec_peak_energy));
                    html = html.Replace("#energyprate", Convert.ToString(_invoice.dec_peak_energy_rate));
                    html = html.Replace("#penergytotal", String.Format("{0:n}", CommonCls.DoFormat(_invoice.dec_peak_energy_amt)));

                    html = html.Replace("#capacidadrate", Convert.ToString(_invoice.capacidad));

                    html = html.Replace("#serviciosconexosrate", Convert.ToString(_invoice.cre_servicios_conexos));
                    
                    html = html.Replace("#sumofamount", "$" + CommonCls.DoFormat(_totalenergyamt));
                    //html = html.Replace("#customamt1", "$" + CommonCls.DoFormat(_invoice.dec_demanda_facturable));
                    //html = html.Replace("#Charge5", "$" + CommonCls.DoFormat(5 * _totalenergyamt / 100));
                    //html = html.Replace("#strcustomcharges", string.IsNullOrEmpty(_invoice.str_custome_charge_desc) ? "Cargos personalizados" : _invoice.str_custome_charge_desc);
                    //html = html.Replace("#Charge5", "$" + CommonCls.DoFormat(_invoice.dec_custome_charges));
                        

                    html = html.Replace("#chargedesc2", "16");
                    html = html.Replace("#customamt3", "$" + CommonCls.DoFormat(_invoice.dec_tax_amt));
                        

                    html = html.Replace("#totalenergy", CommonCls.DoFormat(_totalenergy) + " kWh");
                    //_totalwithtax = _totalwithtax + (Convert.ToDecimal(_totalenergyamt) + _invoice.dec_tax_amt.Value + _invoice.dec_custome_charges + (5 * _totalenergyamt / 100) + 16);
                    //  html = html.Replace("#ToPayAmount", CommonCls.DoFormat(5 *_totalenergyamt/100)+_invoice.dec_demanda_facturable + _totalenergyamt + "16");

                    //html = html.Replace("#ToPayAmount", "$" + CommonCls.DoFormat(_total   withtax));

                    html = html.Replace("#ToPayAmount", "$" + String.Format("{0:n}", CommonCls.DoFormat(_invoice.dec_total)));

                    path = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Content("~/");

                    html = html.Replace("#FooterImg", path + "/PDF/charts.jpg");
                    html = html.Replace("#place", path + "/PDF/place.png");
                    html = html.Replace("#hlogo", path + "/PDF/puerta-polanco-logo.png");
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

                    //case "CC Desc":
                    //    _invoice = _invoice.OrderByDescending(x => x.dec_custome_charges);
                    //    break;

                    //case "CC":
                    //    _invoice = _invoice.OrderBy(x => x.dec_custome_charges);
                    //    break;


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

            var results = (from c in _dbc.tbl_invoice where c.int_tenant_id == tenantid && c.int_invoice_id <= invoiceid orderby c.int_invoice_id descending select c).ToList().Take(5);

            results.ToList().ForEach(rs => xValue.Add(rs.date_invoice_date.Value.ToString("MMM")));
            results.ToList().ForEach(rs => yValue.Add(rs.dec_total));

            string _path = Server.MapPath("~/PDF/charts.jpg");

            new Chart(width: 600, height: 300, theme: ChartTheme.Blue)
            .AddTitle(Resource.prev_chart)
           .AddSeries("Default", chartType: "column", xValue: xValue, yValues: yValue)
                  .Save(_path, "jpg");


            //Basura de los hindúes
            /*ArrayList xValue = new ArrayList();
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
           .Save(_path, "jpg");*/
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

        public FileResult DownloadSummary()
        {
            try
            {
                string fileName = Session["summaryfilename"].ToString();
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

                //var _meterlist = ds.Tables[0].AsEnumerable().Select(x => new meter { name = x.Field<string>("CFE_MeterID") });
                var _meterlist = ds.Tables[0].AsEnumerable().Select(x => new meter { name = x.Field<string>("str_meter_id"), multiplier = x.Field<int>("multiplicador") });

                return View(_meterlist.ToList().ToPagedList(page ?? 1, CommonCls._pagesize));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }

        [SessionCheck]
        [HttpGet]
        public ActionResult InvoiceMeters(int? page)
        {
            try
            {


                MeterCLS _meter = new MeterCLS();
                DataSet ds = _meter.getMeter();

                var _meterlist = ds.Tables[0].AsEnumerable().Select(x => new meter { name = x.Field<string>("str_meter_id") });

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
        public ActionResult AssignMeterToTenant(string id)
        {
            try
            {
                int pmid = Convert.ToInt32(Session["uid"].ToString());


                var _tenant = _dbc.tbl_user_master.Where(x => x.int_user_type_id == 3 && x.int_pm_id == pmid);

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

        [HttpGet]
        public ActionResult InvoiceSummary(int id)
        {
            InvoiceVM _invoices = new InvoiceVM();

            _invoices.int_tenant_id = id;

            ViewBag.MesesDropDown = new SelectList(new List<SelectListItem>()
                            {
                                new SelectListItem(){ Value="1", Text="Enero"},
                                new SelectListItem(){ Value="2", Text="Febrero"},
                                new SelectListItem(){ Value="3", Text="Marzo"},
                                new SelectListItem(){ Value="4", Text="Abril"},
                                new SelectListItem(){ Value="5", Text="Mayo"},
                                new SelectListItem(){ Value="6", Text="Junio"},
                                new SelectListItem(){ Value="7", Text="Julio"},
                                new SelectListItem(){ Value="8", Text="Agosto"},
                                new SelectListItem(){ Value="9", Text="Septiembre"},
                                new SelectListItem(){ Value="10", Text="Octubre"},
                                new SelectListItem(){ Value="11", Text="Noviembre"},
                                new SelectListItem(){ Value="12", Text="Diciembre"},
                            },
                            "Value",
                            "Text");

            var _invoice = _dbc.tbl_invoice.AsQueryable();

            var invoice = _invoice.ToList();
            
            var anios = (from yrs in invoice

                             group yrs by new { anio = yrs.date_invoice_date.Value.Year } into empg
                             select new
                             {
                                 invoicedate = empg.Key.anio,
                                 count = empg.Count()
                             }).OrderByDescending (a => a.invoicedate);

            List<SelectListItem> _anios = new List<SelectListItem>();

            foreach (var a in anios)
            {
                _anios.Add(new SelectListItem { Value = Convert.ToString(a.invoicedate), Text = Convert.ToString(a.invoicedate) });
            }

            ViewBag.AniosDropDown = _anios;

            return PartialView("_Invoicesummary", _invoices);
        }

        [HttpPost]
        public ActionResult InvoiceSummary (InvoiceVM _invoicevm)
        {
            int _lval = 1;

            try
            {
                var invoices = _dbc.tbl_invoice.Where(x => x.int_tenant_id == _invoicevm.int_tenant_id && x.date_e_bill_date.Value.Month == _invoicevm.monthnumber && x.date_e_bill_date.Value.Year == _invoicevm.anio);

                if (invoices.ToList().Count > 0)
                { 
                    generateSummaryPDF(_invoicevm);
                }
                else
                {
                    _lval = -1;
                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
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

                    var multiplier = obj.getMeterMultiplier(objvm.str_meter_name);

                    _ds = obj.getTenantEnergy(objvm.str_meter_name, fromdate[i], todate[i], multiplier);

                    string _energy = getPickandIntermediatEnergy(objvm.int_tenant_id, _ds);
                    message = _energy + "\n";

                    objinv = new InvoiceVM();

                    objinv.bit_is_editable = true;
                    objinv.bit_tenant_active = true;
                    objinv.date_e_bill_date = Convert.ToDateTime(todate[i]);
                    objinv.date_invoice_date = System.DateTime.Now;
                    objinv.date_pay_date = Convert.ToDateTime(paydate[i]);
                    objinv.date_s_bill_date = Convert.ToDateTime(fromdate[i]);
                    //objinv.dec_current_inter_energy = Convert.ToDecimal(_energy.Split(',')[1]);
                    //objinv.dec_current_peak_energy = Convert.ToDecimal(_energy.Split(',')[0]);
                    //objinv.dec_custome_charges = Convert.ToDecimal(string.IsNullOrEmpty(custome[i]) ? "0" : custome[i]);
                    //objinv.dec_demad = Convert.ToDecimal(string.IsNullOrEmpty(demand[i]) ? "0" : demand[i]);

                    //objinv.dec_prev_inter_energy = 0;
                    //objinv.dec_prev_peak_energy = 0;



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
                UdisEntities _dbmeter = new UdisEntities();

                var _meter = _dbmeter.UDIS.Where(x => x.CFE_MeterID == x.CFE_MeterID).ToList();

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
                        _meter = _meter.OrderByDescending(x => x.CFE_MeterID).ToList();
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
                                  meterid = empg.Max(x => x.str_meter_id),
                                  fromdate = empg.Min(x => x.date_s_bill_date),
                                  todate = empg.Max(x => x.date_e_bill_date),
                                  peakenergy = empg.Sum(x => x.dec_peak_energy),
                                  interenergy = empg.Sum(x => x.dec_inter_energy),
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
                UdisEntities _dbmeter = new UdisEntities();
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
                                     peakenergy = empg.Sum(x => x.dec_peak_energy),
                                     interenergy = empg.Sum(x => x.dec_inter_energy),
                                     baseenergy = empg.Sum(x => x.dec_base_energy),
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

        public JsonResult TenantConsumptionReportPreviousMonth(string int_id, int? page, string sortBy, string s_date, string tenantid)
        {
            try
            {
                var today = DateTime.Today;
                var max = new DateTime(today.Year, today.Month, 1); // first of this month
                var min = max.AddMonths(-1); // first of last month

                var _invoice = (from inv in _dbc.tbl_invoice
                                where inv.date_e_bill_date >= min && inv.date_e_bill_date < max
                                select inv).AsQueryable();

                if (_invoice.Count() == 0)
                {
                    max = max.AddMonths(-1);
                    min = max.AddMonths(-1);

                    _invoice = (from inv in _dbc.tbl_invoice
                                where inv.date_e_bill_date >= min && inv.date_e_bill_date < max
                                select inv).AsQueryable();
                }

                UdisEntities _dbmeter = new UdisEntities();

                if (!string.IsNullOrEmpty(int_id))
                {
                    int _id = Convert.ToInt32(int_id);
                    _invoice = _invoice.Where(x => x.int_tenant_id == _id);

                }

                var invoice = _invoice.ToList();
                var chartgroup = from ins in invoice

                                 group ins by new { invoicedate = ins.date_e_bill_date.Value.Month, ins.tbl_user_master.str_comp_name } into empg
                                 orderby empg.Key.str_comp_name ascending
                                 //group ins by ins.tbl_user_master.str_comp_name into empg
                                 select new
                                 {
                                     Name = empg.Key.str_comp_name,
                                     invoice_total = empg.Sum(x => x.dec_total),
                                     invoicedate = empg.Key.invoicedate.ToString(),
                                 };


                var data = chartgroup.ToList();
                List<SummaryViewModel> models = new List<SummaryViewModel>();
                SummaryViewModel abc;
                foreach (var dt in data)
                {
                    abc = new SummaryViewModel();
                    abc.Name = dt.Name;
                    abc.totalenergy = dt.invoice_total;
                    abc.dateinvoice = getMonthName(Convert.ToInt32(dt.invoicedate));
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

        public JsonResult TenantconsumptionReportNEW(string int_id, int? page, string sortBy, string s_date, string tenantid)
        {
            try
            {

                var today = DateTime.Today;
                var max = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month)); // last day of this month
                var min = max.AddMonths(-3); // previous 3 months
                var previousYear = today.Year;
                string currentMonth = getMonthName(today.Month);
                string previousMonth = getMonthName(today.Month -1);
                string prepreviousMonth = getMonthName(today.Month - 2);


                if (((today.Month - 1) < 1) || ((today.Month - 2) < 1))
                        previousYear = today.Year - 1;

                var _invoice = (from inv in _dbc.tbl_invoice
                                where inv.date_e_bill_date >= min && inv.date_e_bill_date < max
                                select inv).AsQueryable();

                var _cfeAmounts = (from cfe in _dbc.tbl_tarifas
                                  where (cfe.mes_tarifas == currentMonth || cfe.mes_tarifas == previousMonth
                                            || cfe.mes_tarifas == prepreviousMonth) && ((cfe.ano_tarifas == today.Year) && 
                                            (cfe.ano_tarifas == previousYear))
                                  select cfe).AsQueryable();

                UdisEntities _dbmeter = new UdisEntities();
                //_invoice = _dbc.tbl_invoice.Where(x => x.date_invoice_date.Value.Month == x.date_invoice_date.Value.Month).Take(6);

                if (!string.IsNullOrEmpty(int_id))
                {
                    int _id = Convert.ToInt32(int_id);
                    _invoice = _invoice.Where(x => x.int_tenant_id == _id);

                }

                var invoice = _invoice.ToList();
                var cfeAmounts = _cfeAmounts.ToList();
                var chartgroup = from ins in invoice

                                 group ins by new { invoicedate = ins.date_e_bill_date.Value.Month, ins.tbl_user_master.str_comp_name } into empg
                                 orderby empg.Key.invoicedate ascending, empg.Key.str_comp_name ascending
                                 //group ins by ins.tbl_user_master.str_comp_name into empg
                                 select new
                                 {
                                     Name = empg.Key.str_comp_name,
                                     invoice_total = empg.Sum(x => x.dec_total),
                                     invoicedate = empg.Key.invoicedate.ToString(),
                                 };
                

                var data = chartgroup.ToList();
                List<SummaryViewModel> models = new List<SummaryViewModel>();
                SummaryViewModel abc;
                foreach (var dt in data)
                {
                    var cfe = cfeAmounts.Where(x => x.mes_tarifas == getMonthName(Convert.ToInt32(dt.invoicedate))).FirstOrDefault();
                    if ((cfe != null) && (!models.Any(y => y.dateinvoice == "CFE " + cfe.mes_tarifas)))
                    {
                        abc = new SummaryViewModel();
                        abc.dateinvoice = "CFE " + cfe.mes_tarifas;
                        abc.Name = chartgroup.First().Name;
                        abc.totalenergy = cfe.total_cantidad_cfe;
                        models.Add(abc);
                    }

                    abc = new SummaryViewModel();
                    abc.Name = dt.Name;
                    abc.totalenergy = dt.invoice_total;
                    abc.dateinvoice = getMonthName(Convert.ToInt32(dt.invoicedate));
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

        public int getMonthNumber (string monthname)
        {
            int monthnumber = 1;

            if (monthname == "Mayo")
                monthnumber = 5;
            if (monthname == "Enero")
                monthnumber = 1;
            if (monthname == "Febrero")
                monthnumber = 2;
            if (monthname == "Marzo")
                monthnumber = 3;
            if (monthname == "Abril")
                monthnumber = 4;
            if (monthname == "Junio")
                monthnumber = 6;
            if (monthname == "Julio")
                monthnumber = 7;
            if (monthname == "Agosto")
                monthnumber = 8;
            if (monthname == "Septiembre")
                monthnumber = 9;
            if (monthname == "Octubre")
                monthnumber = 10;
            if (monthname == "Noviembre")
                monthnumber = 11;
            if (monthname == "Diciembre")
                monthnumber = 12;

            return monthnumber;
        }

        public string getMonthName(int monthnumber)
        {
            string monthname = "";

            if (monthnumber.ToString() == "5")
                monthname = "Mayo";
            if (monthnumber.ToString() == "1")
                monthname = "Enero";
            if (monthnumber.ToString() == "2")
                monthname = "Febrero";
            if (monthnumber.ToString() == "3")
                monthname = "Marzo";
            if (monthnumber.ToString() == "4")
                monthname = "Abril";
            if (monthnumber.ToString() == "6")
                monthname = "Junio";
            if (monthnumber.ToString() == "7")
                monthname = "Julio";
            if (monthnumber.ToString() == "8")
                monthname = "Agosto";
            if (monthnumber.ToString() == "9")
                monthname = "Septiembre";
            if (monthnumber.ToString() == "10")
                monthname = "Octubre";
            if (monthnumber.ToString() == "11")
                monthname = "Noviembre";
            if (monthnumber.ToString() == "12")
                monthname = "Diciembre";
            if (monthnumber.ToString() == "0")
                monthname = "Diciembre";
            if (monthnumber.ToString() == "-1")
                monthname = "Noviembre";

            return monthname;
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
                UdisEntities _dbmeter = new UdisEntities();

                var _meter = _dbmeter.UDIS.Where(x => x.CFE_MeterID == x.CFE_MeterID).ToList();

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
                                  meterid = empg.Max(x => x.str_meter_id),
                                  fromdate = empg.Min(x => x.date_s_bill_date),
                                  todate = empg.Max(x => x.date_e_bill_date),
                                  peakenergy = empg.Sum(x => x.dec_peak_energy),
                                  interenergy = empg.Sum(x => x.dec_inter_energy),
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

        public ActionResult ViewSummaryPDF()
        {
            string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"100%\" height=\"800px\">";
            embed += "If you are unable to view file, you can download from <a href = \"{0}\">here</a>";
            embed += " or download <a target = \"_blank\" href = \"http://get.adobe.com/reader/\">Adobe PDF Reader</a> to view the file.";
            embed += "</object>";

            string fileName = Session["summaryfilename"].ToString();
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
            UdisEntities _dbmeter = new UdisEntities();
            var _meter = _dbmeter.UDIS.Select(x => new {x.CFE_MeterID}).Distinct().ToList();
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


                var _invoice = _dbc.tbl_invoice.AsQueryable();
                UdisEntities _dbmeter = new UdisEntities();

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
                                     peakenergy = empg.Sum(x => x.dec_peak_energy),
                                     interenergy = empg.Sum(x => x.dec_inter_energy),
                                     baseenergy = empg.Sum(x => x.dec_base_energy),
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
                UdisEntities _dbmeter = new UdisEntities();
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
                                     peakenergy = empg.Sum(x => x.dec_peak_energy),
                                     interenergy = empg.Sum(x => x.dec_inter_energy),
                                     baseenergy = empg.Sum(x => x.dec_base_energy),
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
                UdisEntities _dbmeter = new UdisEntities();
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
                                     peakenergy = empg.Sum(x => x.dec_peak_energy),
                                     interenergy = empg.Sum(x => x.dec_inter_energy),
                                     baseenergy = empg.Sum(x => x.dec_base_energy),
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
                UdisEntities _dbmeter = new UdisEntities();
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
                                     peakenergy = empg.Sum(x => x.dec_peak_energy),
                                     interenergy = empg.Sum(x => x.dec_inter_energy),
                                     baseenergy = empg.Sum(x => x.dec_base_energy),
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
