using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TenantMNG.Models;
using TenantMNG.ViewModel;

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
                            new SqlParameter("@int_meter_id", meter.int_meter_id),
                            new SqlParameter("@int_id",SqlDbType.Int){
                             Direction = System.Data.ParameterDirection.Output
                            }


                };

                    dbcnx.Database.ExecuteSqlCommand("usp_insert_tenant_meter  @int_tenant_id,@int_meter_id,@int_id out", param);

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

                            new SqlParameter("@dec_custome_charges", invoice.dec_custome_charges==null ?0:invoice.dec_custome_charges),

                            new SqlParameter("@dec_demad", invoice.dec_demad==null?0:invoice.dec_demad),
                            new SqlParameter("@dec_total",invoice.dec_total==null?0:invoice.dec_total),
                            new SqlParameter("@dec_tax_amt",invoice.dec_tax_amt==null?0:invoice.dec_tax_amt),
                            new SqlParameter("@bit_is_editable",invoice.bit_is_editable),

                            new SqlParameter("@date_pay_date",invoice.date_pay_date),
                            new SqlParameter("@str_custome_charge_desc",invoice.str_custome_charge_desc==null?"":invoice.str_custome_charge_desc),
                            new SqlParameter("@dec_prev_peack_energy",invoice.dec_prev_peak_energy==null?0:invoice.dec_prev_peak_energy),
                            new SqlParameter("@dec_prev_inter_energy",invoice.dec_prev_inter_energy==null?0:invoice.dec_prev_inter_energy),
                            new SqlParameter("@dec_current_peack_energy",invoice.dec_current_peak_energy==null?0:invoice.dec_current_peak_energy),
                            new SqlParameter("@dec_current_inter_energy",invoice.dec_current_inter_energy==null ?0 : invoice.dec_current_inter_energy),

                            new SqlParameter("@dec_demanda_facturable",invoice.dec_demanda_facturable),
                            new SqlParameter("@dec_total_ene",invoice.dec_total_ene),
                             new SqlParameter("@dec_demanda_facturable_amount",invoice.dec_demanda_facturable_amount),
                };

                    dbcnx.Database.ExecuteSqlCommand("usp_insert_invoice @int_id out,@int_tenant_id,@bit_tenant_active,@date_s_bill_date,@date_e_bill_date,@dec_custome_charges,@dec_demad,@dec_total,@dec_tax_amt,@bit_is_editable,@date_pay_date,@str_custome_charge_desc,@dec_prev_peack_energy,@dec_prev_inter_energy,@dec_current_peack_energy,@dec_current_inter_energy,@dec_demanda_facturable,@dec_total_ene,@dec_demanda_facturable_amount", param);

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


                         new SqlParameter("@int_meter_id", invoice.int_meter_id),
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


                };

                    dbcnx.Database.ExecuteSqlCommand("usp_insert_invoice_details @int_meter_id,@int_invoice_id,@dec_peak_energy,@dec_peak_energy_rate,@dec_peak_energy_amt,@dec_inter_energy,@dec_inter_energy_rate,@dec_inter_energy_amt,@dec_base_energy,@dec_base_rate,@dec_base_amt", param);

                    _lVal = Convert.ToInt32(param[0].Value);
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
                            new SqlParameter("@int_meter_id", meter.int_meter_id),
                            new SqlParameter("@int_val",SqlDbType.Int){
                             Direction = System.Data.ParameterDirection.Output
                            }


                };

                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_update_tenant_meter  @int_id,@int_tenant_id,@int_meter_id,@int_val out", param);
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

        public int tenant_detach_meter_tenant(int? int_id,int ?int_meter_id)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param =
                {
                            new SqlParameter("@int_id", int_id),
                             new SqlParameter("@int_meter_id", int_meter_id),
                };

                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_detach_tenant_meter @int_id,@int_meter_id", param);
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

                            new SqlParameter("@dec_custome_charges", invoice.dec_custome_charges==null ?0:invoice.dec_custome_charges),

                            new SqlParameter("@dec_demad", invoice.dec_demad==null?0:invoice.dec_demad),
                            new SqlParameter("@dec_total",invoice.dec_total==null?0:invoice.dec_total),
                            new SqlParameter("@dec_tax_amt",invoice.dec_total==null?0:invoice.dec_tax_amt),
                            new SqlParameter("@date_pay_date",invoice.date_pay_date),
                            new SqlParameter("@str_custome_charge_desc",invoice.str_custome_charge_desc==null?"":invoice.str_custome_charge_desc),
                            new SqlParameter("@dec_prev_peack_energy",invoice.dec_prev_peak_energy),
                            new SqlParameter("@dec_prev_inter_energy",invoice.dec_prev_inter_energy),
                            new SqlParameter("@dec_current_peack_energy",invoice.dec_current_peak_energy),
                            new SqlParameter("@dec_current_inter_energy",invoice.dec_current_inter_energy),
                            new SqlParameter("@int_tenant_id",invoice.int_tenant_id),

                            new SqlParameter("@dec_demanda_facturable",invoice.dec_demanda_facturable),
                            new SqlParameter("@dec_total_ene",invoice.dec_total_ene),
                            new SqlParameter("@dec_demanda_facturable_amount",invoice.dec_demanda_facturable_amount),

                };

                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_update_invoice @int_id,@bit_tenant_active,@date_s_bill_date,@date_e_bill_date,@dec_custome_charges,@dec_demad,@dec_total,@dec_tax_amt,@date_pay_date,@str_custome_charge_desc,@dec_prev_peack_energy,@dec_prev_inter_energy,@dec_current_peack_energy,@dec_current_inter_energy,@int_tenant_id,@dec_demanda_facturable,@dec_total_ene,@dec_demanda_facturable_amount", param);


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