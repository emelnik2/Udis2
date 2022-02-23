using log4net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TenantMNG.Models;
using TenantMNG.ViewModel;

namespace TenantMNG.BAL
{
    public class PMBAL
    {
        DB_TenantMNGEntities _dbc = new DB_TenantMNGEntities();
        ILog log = log4net.LogManager.GetLogger(typeof(UserBAL));

        #region Insert Method
        public int pm_insert_billing_hours(PMBillingHoursVM user)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param =
                {
                  
                           // new SqlParameter("@str_comp_name", user.tb),
                            new SqlParameter("@int_pm_id", user.int_pm_id),
                            new SqlParameter("@str_peak_s_time_m", user.str_peak_s_time_m),
                            new SqlParameter("@str_peak_e_time_m", user.str_peak_e_time_m),
                            new SqlParameter("@str_inter_s_time_1_m", user.str_inter_s_time_1_m),
                            new SqlParameter("@str_inter_e_time_1_m", user.str_inter_e_time_1_m),

                            new SqlParameter("@str_inter_s_time_2_m", user.str_inter_s_time_2_m),
                            new SqlParameter("@str_inter_e_time_2_m", user.str_inter_e_time_2_m),
                            new SqlParameter("@str_base_s_time_m", user.str_base_s_time_m),
                            new SqlParameter("@str_base_e_time_m", user.str_base_e_time_m),

                             new SqlParameter("@str_base_s_time_sat", user.str_base_s_time_sat),
                            new SqlParameter("@str_base_e_time_sat", user.str_base_e_time_sat),
                            new SqlParameter("@str_inter_s_time_sat", user.str_inter_s_time_sat),
                            new SqlParameter("@str_inter_e_time_sat", user.str_inter_e_time_sat),

                             new SqlParameter("@str_base_s_time_sun", user.str_base_s_time_sun),
                            new SqlParameter("@str_base_e_time_sun", user.str_base_e_time_sun),
                            new SqlParameter("@str_inter_s_time_sun", user.str_inter_s_time_sun),
                            new SqlParameter("@str_inter_e_time_sun", user.str_inter_e_time_sun),
                                           };



                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_insert_pm_billing_hours @int_pm_id,@str_peak_s_time_m,@str_peak_e_time_m,@str_inter_s_time_1_m,@str_inter_e_time_1_m,@str_inter_s_time_2_m,@str_inter_e_time_2_m,@str_base_s_time_m,@str_base_e_time_m," +
                        "@str_base_s_time_sat,@str_base_e_time_sat,@str_inter_s_time_sat,@str_inter_e_time_sat," +
                        "@str_base_s_time_sun,@str_base_e_time_sun,@str_inter_s_time_sun,@str_inter_e_time_sun", param);

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
        public int pm_update_billing_hours(PMBillingHoursVM user)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param =
                {

                            new SqlParameter("@int_pm_id", user.int_pm_id),
                            new SqlParameter("@str_peak_s_time_m", user.str_peak_s_time_m),
                            new SqlParameter("@str_peak_e_time_m", user.str_peak_e_time_m),
                            new SqlParameter("@str_inter_s_time_1_m", user.str_inter_s_time_1_m),
                            new SqlParameter("@str_inter_e_time_1_m", user.str_inter_e_time_1_m),

                            new SqlParameter("@str_inter_s_time_2_m", user.str_inter_s_time_2_m),
                            new SqlParameter("@str_inter_e_time_2_m", user.str_inter_e_time_2_m),
                            new SqlParameter("@str_base_s_time_m", user.str_base_s_time_m),
                            new SqlParameter("@str_base_e_time_m", user.str_base_e_time_m),

                             new SqlParameter("@str_base_s_time_sat", user.str_base_s_time_sat),
                            new SqlParameter("@str_base_e_time_sat", user.str_base_e_time_sat),
                            new SqlParameter("@str_inter_s_time_sat", user.str_inter_s_time_sat),
                            new SqlParameter("@str_inter_e_time_sat", user.str_inter_e_time_sat),

                             new SqlParameter("@str_base_s_time_sun", user.str_base_s_time_sun),
                            new SqlParameter("@str_base_e_time_sun", user.str_base_e_time_sun),
                            new SqlParameter("@str_inter_s_time_sun", user.str_inter_s_time_sun),
                            new SqlParameter("@str_inter_e_time_sun", user.str_inter_e_time_sun),
                                           };



                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_update_pm_billing_hours  @int_pm_id,@str_peak_s_time_m,@str_peak_e_time_m,@str_inter_s_time_1_m,@str_inter_e_time_1_m,@str_inter_s_time_2_m,@str_inter_e_time_2_m,@str_base_s_time_m,@str_base_e_time_m," +
                        "@str_base_s_time_sat,@str_base_e_time_sat,@str_inter_s_time_sat,@str_inter_e_time_sat," +
                        "@str_base_s_time_sun,@str_base_e_time_sun,@str_inter_s_time_sun,@str_inter_e_time_sun", param);

                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return _lVal;
        }
        #endregion

    }
}