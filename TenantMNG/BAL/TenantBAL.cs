using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TenantMNG.Models;
using TenantMNG.ViewModel;
using TenantMNG.ADO.NET;

namespace TenantMNG.BAL
{
    public class TenantBAL
    {
        DB_TenantMNGEntities _dbc = new DB_TenantMNGEntities();
        ILog log = log4net.LogManager.GetLogger(typeof(TenantBAL));

        #region Insert Method
        public int tenant_billing_insert(TenantVM tenant)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param =
                {
                  
                           // new SqlParameter("@str_comp_name", user.tb),
                            new SqlParameter("@int_tenant_id", tenant.int_tenant_id),
                            new SqlParameter("@int_template_id", tenant.int_template_id),
                            new SqlParameter("@dec_rate", tenant.dec_rate),
                            new SqlParameter("@bit_is_seasonal_rate", tenant.bit_is_seasonal_rate),
                            new SqlParameter("@dec_seasonal_multi_rate", tenant.dec_seasonal_multi_rate),
                            new SqlParameter("@bit_is_surchare",tenant.bit_is_surchare),
                            new SqlParameter("@dec_surcharge_amt", tenant.dec_surcharge_amt==null ?"0":tenant.dec_surcharge_amt),
                            new SqlParameter("@str_min_billable_over",tenant.str_min_billable_over==null?"0:00":tenant.str_min_billable_over ),
                            new SqlParameter("@str_charge_tenant_min", tenant.str_charge_tenant_min==null?"0:00":tenant.str_charge_tenant_min),

                            new SqlParameter("@str_charge_tenant_max",tenant.str_charge_tenant_max==null?"0:00":tenant.str_charge_tenant_max),
                            new SqlParameter("@bit_is_consolidate_zone", tenant.bit_is_consolidate_zone),

                            new SqlParameter("@bit_is_print",tenant.bit_is_print),

                            new SqlParameter("@bit_is_file",tenant.bit_is_file),
                            new SqlParameter("@int_type", tenant.int_type),

                            new SqlParameter("@str_email",tenant.str_email==null?"":tenant.str_email)

                };

                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_insert_tenant_billing_info @int_tenant_id,@int_template_id,@dec_rate,@bit_is_seasonal_rate,@dec_seasonal_multi_rate,@bit_is_surchare,@dec_surcharge_amt,@str_min_billable_over,@str_charge_tenant_min,@str_charge_tenant_max,@bit_is_consolidate_zone,@bit_is_print,@bit_is_file,@int_type,@str_email", param);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return _lVal;
        }

        public int tenant_setting_insert(TenantSettingVM tenant)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param =
                {
                  
                           // new SqlParameter("@str_comp_name", user.tb),
                            new SqlParameter("@int_tenant_id", tenant.int_tenant_id),
                            new SqlParameter("@dec_demanda_facturable", tenant.dec_demanda_facturable),
                            new SqlParameter("@dec_total_ene", tenant.dec_total_ene),


                };

                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_insert_tenant_setting @int_tenant_id,@dec_demanda_facturable,@dec_total_ene", param);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return _lVal;
        }

        public int tenant_email_setup_insert(EmailSetupVM email)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param =
                {
                  
                           // new SqlParameter("@str_comp_name", user.tb),
                            new SqlParameter("@int_tenant_id", email.int_tenant_id),
                            new SqlParameter("@str_from_email", email.str_from_email),
                            new SqlParameter("@str_cc_email", email.str_cc_email==null?"":email.str_cc_email),
                            new SqlParameter("@str_bcc_email", email.str_bcc_email==null?"":email.str_bcc_email),
                            new SqlParameter("@str_subject", email.str_subject==null ?"":email.str_subject ),
                            new SqlParameter("@str_body",email.str_body==null?"":email.str_body),


                };

                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_insert_tbl_tenant_email  @int_tenant_id,@str_from_email,@str_cc_email,@str_bcc_email,@str_subject,@str_body", param);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return _lVal;
        }

        public int tenant_meter_insert(TenantMeterVM meter)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param =
                {
                  
                           // new SqlParameter("@str_comp_name", user.tb),
                            new SqlParameter("@int_tenant_id", meter.int_tenant_id),
                            new SqlParameter("@str_meter_id", meter.str_meter_id),
                            new SqlParameter("@int_id",SqlDbType.Int){
                             Direction = System.Data.ParameterDirection.Output
                            }


                };

                    dbcnx.Database.ExecuteSqlCommand("usp_insert_tenant_meter @int_tenant_id,@str_meter_id,@int_id out", param);

                    _lVal = Convert.ToInt32(param[2].Value);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return _lVal;
        }


        public int tenant_invoice_insert(InvoiceVM invoice)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param =
                {

                            new SqlParameter("@int_id", SqlDbType.Int){Direction=System.Data.ParameterDirection.Output},

                            new SqlParameter("@int_tenant_id", invoice.int_tenant_id),
                            new SqlParameter("@bit_tenant_active", invoice.bit_tenant_active),
                            new SqlParameter("@date_s_bill_date", invoice.date_s_bill_date),
                            new SqlParameter("@date_e_bill_date", invoice.date_e_bill_date),

                            new SqlParameter("@dec_total",invoice.dec_total==null?0:invoice.dec_total),
                            new SqlParameter("@dec_tax_amt",invoice.dec_tax_amt==null?0:invoice.dec_tax_amt),
                            new SqlParameter("@bit_is_editable",invoice.bit_is_editable),

                            new SqlParameter("@date_pay_date",invoice.date_pay_date),
                            new SqlParameter("@suministro",invoice.suministro),
                            new SqlParameter("@distribucion",invoice.distribucion),
                            new SqlParameter("@tarifa_transmision",invoice.tarifa_transmision),
                            new SqlParameter("@operacion_cenace",invoice.operacion_cenace),
                            new SqlParameter("@capacidad",invoice.capacidad),
                            new SqlParameter("@cre_servicios_conexos",invoice.cre_servicios_conexos),
                            new SqlParameter("@precio_suministro",invoice.precio_suministro),
                            new SqlParameter("@precio_distribucion",invoice.precio_distribucion),
                            new SqlParameter("@precio_transmision",invoice.precio_transmision),
                            new SqlParameter("@precio_cenace",invoice.precio_cenace),
                            new SqlParameter("@precio_energia",invoice.precio_energia),
                            new SqlParameter("@precio_capacidad",invoice.precio_capacidad),
                            new SqlParameter("@precio_cre_servicios_conexos",invoice.precio_cre_servicios_conexos),
                            new SqlParameter("@precio_dos_porciento_baja_tension",invoice.precio_dos_porciento_baja_tension),
                            new SqlParameter("@precio_decuento_bonificacion",invoice.precio_decuento_bonificacion),

                };

                    dbcnx.Database.ExecuteSqlCommand("usp_insert_invoice @int_id out,@int_tenant_id,@bit_tenant_active,@date_s_bill_date,@date_e_bill_date,@dec_total,@dec_tax_amt,@bit_is_editable,@date_pay_date,@suministro,@distribucion,@tarifa_transmision,@operacion_cenace,@capacidad,@cre_servicios_conexos", param);

                    _lVal = Convert.ToInt32(param[0].Value);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return _lVal;
        }

        public int tenant_invoice_details_insert(InvoiceVM invoice)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param =
                {


                         new SqlParameter("@str_meter_id", invoice.str_meter_id),
                            new SqlParameter("@int_invoice_id", invoice.int_invoice_id),
                            new SqlParameter("@dec_peak_energy", invoice.dec_peak_energy),
                            new SqlParameter("@dec_peak_energy_rate", invoice.dec_peak_energy_rate),
                            new SqlParameter("@dec_peak_energy_amt", invoice.dec_peak_energy_amt),

                            new SqlParameter("@dec_inter_energy", invoice.dec_inter_energy),
                            new SqlParameter("@dec_inter_energy_rate", invoice.dec_inter_energy_rate),
                            new SqlParameter("@dec_inter_energy_amt", invoice.dec_inter_energy_amt),

                            new SqlParameter("@dec_base_energy", invoice.dec_base_energy),
                            new SqlParameter("@dec_base_rate", invoice.dec_base_rate),
                            new SqlParameter("@dec_base_amt", invoice.dec_base_amt),

                            new SqlParameter("@@demanda_base", invoice.demanda_base),
                            new SqlParameter("@demanda_intermedia", invoice.demanda_intermedia),
                            new SqlParameter("@demanda_punta", invoice.demanda_punta),
                            new SqlParameter("@energia_activa", invoice.energia_activa),
                            new SqlParameter("@energia_reactiva", invoice.energia_reactiva),


                };

                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_insert_invoice_details @str_meter_id,@int_invoice_id,@dec_peak_energy,@dec_peak_energy_rate,@dec_peak_energy_amt,@dec_inter_energy,@dec_inter_energy_rate,@dec_inter_energy_amt,@dec_base_energy,@dec_base_rate,@dec_base_amt", param);

                    //_lVal = Convert.ToInt32(param[0].Value);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return _lVal;
        }

        public int tenant_contract_insert(TenantContratVM tenantcontract)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param =
                {

                            new SqlParameter("@int_tenant_id",tenantcontract.int_tenant_id),
                            new SqlParameter("@s_date", tenantcontract.s_date),
                            new SqlParameter("@e_date", tenantcontract.e_date),


                };

                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_insert_tenant_contract @int_tenant_id,@s_date,@e_date", param);


                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return _lVal;
        }
        #endregion

        #region Update Method
        public int tenant_billing_update(TenantVM tenant)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param =
                {
                  
                           // new SqlParameter("@str_comp_name", user.tb),
                            new SqlParameter("@int_tenant_id", tenant.int_tenant_id),
                            new SqlParameter("@int_template_id", tenant.int_template_id),
                            new SqlParameter("@dec_rate", tenant.dec_rate),
                            new SqlParameter("@bit_is_seasonal_rate", tenant.bit_is_seasonal_rate),
                            new SqlParameter("@dec_seasonal_multi_rate", tenant.dec_seasonal_multi_rate),
                            new SqlParameter("@bit_is_surchare",tenant.bit_is_surchare),
                            new SqlParameter("@dec_surcharge_amt", tenant.dec_surcharge_amt==null ?"0":tenant.dec_surcharge_amt),
                            new SqlParameter("@str_min_billable_over",tenant.str_min_billable_over==null?"0.00":tenant.str_min_billable_over ),
                            new SqlParameter("@str_charge_tenant_min", tenant.str_charge_tenant_min==null?"0.00":tenant.str_charge_tenant_min),

                            new SqlParameter("@str_charge_tenant_max",tenant.str_charge_tenant_max==null?"0.00":tenant.str_charge_tenant_max),
                            new SqlParameter("@bit_is_consolidate_zone", tenant.bit_is_consolidate_zone),

                            new SqlParameter("@bit_is_print",tenant.bit_is_print),

                            new SqlParameter("@bit_is_file",tenant.bit_is_file),
                            new SqlParameter("@int_type", tenant.int_type),

                            new SqlParameter("@str_email",tenant.str_email==null?"":tenant.str_email),
                            new SqlParameter("@int_id",tenant.int_id)


                };

                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_update_tenant_billing_info @int_tenant_id,@int_template_id,@dec_rate,@bit_is_seasonal_rate,@dec_seasonal_multi_rate,@bit_is_surchare,@dec_surcharge_amt,@str_min_billable_over,@str_charge_tenant_min,@str_charge_tenant_max,@bit_is_consolidate_zone,@bit_is_print,@bit_is_file,@int_type,@str_email,@int_id", param);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return _lVal;
        }

        public int tenant_billing_rates_update(TenantEnergyVM tenant)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param =
                {
                  
                           // new SqlParameter("@str_comp_name", user.tb),
                            new SqlParameter("@int_tenant_id", tenant.int_tenant_id),
                            new SqlParameter("@dec_rate", tenant.dec_base_rate),
                            new SqlParameter("@dec_seasonal_multi_rate", tenant.dec_peak_energy_rate),
                            new SqlParameter("@dec_surcharge_amt", tenant.dec_inter_energy_rate)
                };

                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_update_tenant_billing_info_rates @int_tenant_id,@dec_rate,@dec_seasonal_multi_rate,@dec_surcharge_amt", param);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return _lVal;
        }


        public int tenant_email_setup_update(EmailSetupVM email)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param =
                {
                  
                           // new SqlParameter("@str_comp_name", user.tb),
                            new SqlParameter("@int_tenant_id", email.int_tenant_id),
                            new SqlParameter("@str_from_email", email.str_from_email),
                            new SqlParameter("@str_cc_email", email.str_cc_email==null?"":email.str_cc_email),
                            new SqlParameter("@str_bcc_email", email.str_bcc_email==null?"":email.str_bcc_email),
                            new SqlParameter("@str_subject", email.str_subject),
                            new SqlParameter("@str_body",email.str_body),
                            new SqlParameter("@int_email_id",email.int_email_id),

                };

                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_update_tbl_tenant_email  @int_tenant_id,@str_from_email,@str_cc_email,@str_bcc_email,@str_subject,@str_body,@int_email_id", param);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return _lVal;
        }

        public DataTable getEnergyConsumption(TenantEnergyVM objvm)
        {
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
                        _meterid = _meterid + "," + m.str_meter_id.ToString();

                    }
                }

                UdisEntities _dbc = new UdisEntities();

                string[] _meteridarray = _meterid.ToString().Trim(',').Split(',');


                foreach (var m in _meteridarray)
                {
                    var _tenantmetername = _dbc.UDIS.Where(x => x.CFE_MeterID == m).OrderByDescending(p => p.fecha_ocurrencia).FirstOrDefault(); 
                    
                    _metername = _metername + "," + _tenantmetername.CFE_MeterID;
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
                            new DataColumn("str_meter_id",typeof(string)),});



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

                        DataSet _ds = obj.getTenantEnergy(m, objvm.date_s_bill_date.ToString(), objvm.date_e_bill_date.ToString());

                        if (_ds.Tables[0].Rows.Count > 0)
                        {
                            decimal peckenergy = Convert.ToDecimal(_ds.Tables[0].Rows[0][0].ToString());
                            decimal interenergy = Convert.ToDecimal(_ds.Tables[2].Rows[0][0].ToString());
                            decimal baseenergy = Convert.ToDecimal(_ds.Tables[1].Rows[0][0].ToString());

                            dt.Rows.Add(m, peckenergy, objvm.dec_peak_energy_rate, (peckenergy * objvm.dec_peak_energy_rate), interenergy,
                                objvm.dec_inter_energy_rate, (interenergy * objvm.dec_inter_energy_rate), baseenergy, objvm.dec_base_rate, (baseenergy * objvm.dec_base_rate), _meteridarray[i]);
                        }

                        i++;

                    }

                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return dt;
        }

        public int tenant_meter_update(TenantMeterVM meter)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param =
                {

                            new SqlParameter("@int_id", meter.int_id),
                            new SqlParameter("@int_tenant_id", meter.int_tenant_id),
                            new SqlParameter("@str_meter_id", meter.str_meter_id),
                            new SqlParameter("@int_val",SqlDbType.Int){
                             Direction = System.Data.ParameterDirection.Output
                            }


                };

                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_update_tenant_meter @int_id,@int_tenant_id,@str_meter_id,@int_val out", param);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return _lVal;
        }

        public int tenant_bill_rate_update(TenantVM tenant)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param =
                {

                            new SqlParameter("@int_id", tenant.int_id),
                            new SqlParameter("@dec_rate", tenant.dec_rate),
                            new SqlParameter("@dec_seasonal_rate", tenant.dec_seasonal_multi_rate),
                            new SqlParameter("@dec_surcharge_amt",tenant.dec_surcharge_amt)


                };

                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_update_bill_rate  @int_id,@dec_rate,@dec_seasonal_rate,@dec_surcharge_amt", param);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return _lVal;
        }

        public int tenant_detach_meter(int? int_id)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param =
                {
                            new SqlParameter("@int_id", int_id),
                };

                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_detach_meter @int_id", param);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return _lVal;
        }

        public int tenant_detach_meter_tenant(int? int_id, string str_meter_id)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param =
                {
                            new SqlParameter("@int_id", int_id),
                             new SqlParameter("@str_meter_id", str_meter_id),
                };

                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_detach_tenant_meter @int_id,@str_meter_id", param);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return _lVal;
        }

        public int tenant_invoice_update(InvoiceVM invoice)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param =
                {

                            new SqlParameter("@int_id", invoice.int_invoice_id),

                            new SqlParameter("@bit_tenant_active", invoice.bit_tenant_active),
                            new SqlParameter("@date_s_bill_date", invoice.date_s_bill_date),
                            new SqlParameter("@date_e_bill_date", invoice.date_e_bill_date),


                            new SqlParameter("@dec_total",invoice.dec_total==null?0:invoice.dec_total),
                            new SqlParameter("@dec_tax_amt",invoice.dec_tax_amt==null?0:invoice.dec_tax_amt),
                            new SqlParameter("@date_pay_date",invoice.date_pay_date),

                            new SqlParameter("@suministro",invoice.suministro),
                            new SqlParameter("@distribucion",invoice.distribucion),
                            new SqlParameter("@tarifa_transmision",invoice.tarifa_transmision),
                            new SqlParameter("@operacion_cenace",invoice.operacion_cenace),
                            new SqlParameter("@capacidad",invoice.capacidad),
                            new SqlParameter("@cre_servicios_conexos",invoice.cre_servicios_conexos),

                            new SqlParameter("@int_tenant_id",invoice.int_tenant_id),
                };

                            _lVal = dbcnx.Database.ExecuteSqlCommand("usp_update_invoice @int_id,@bit_tenant_active,@date_s_bill_date,@date_e_bill_date,@dec_total,@dec_tax_amt,@date_pay_date,@suministro,@distribucion,@tarifa_transmision,@operacion_cenace,@capacidad,@cre_servicios_conexos,@int_tenant_id", param);


                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return _lVal;
        }

        public int tenant_contract_update(TenantContratVM tenantcontract)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param =
                {

                            new SqlParameter("@int_contract_id",tenantcontract.int_contract_id),
                            new SqlParameter("@s_date", tenantcontract.s_date),
                            new SqlParameter("@e_date", tenantcontract.e_date),


                };

                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_update_tenant_contract @int_contract_id,@s_date,@e_date", param);


                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return _lVal;
        }

        public int tenant_setting_update(TenantSettingVM tenant)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param =
                {

                            new SqlParameter("@int_id", tenant.int_id),
                            new SqlParameter("@int_tenant_id", tenant.int_tenant_id),
                            new SqlParameter("@dec_demanda_facturable", tenant.dec_demanda_facturable),
                            new SqlParameter("@dec_total_ene", tenant.dec_total_ene),


                };

                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_update_tenant_setting @int_id,@int_tenant_id,@dec_demanda_facturable,@dec_total_ene", param);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return _lVal;
        }
        #endregion

        public int tenant_delete_meter(int? int_id)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param =
                {
                            new SqlParameter("@int_id", int_id),
                };

                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_delete_tenant_meter @int_id", param);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return _lVal;
        }


    }
}