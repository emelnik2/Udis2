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
    public class UserBAL
    {
        DB_TenantMNGEntities _dbc = new DB_TenantMNGEntities();
        ILog log = log4net.LogManager.GetLogger(typeof(UserBAL));

        #region Insert Method
        public int user_insert(UserMasterVM user)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param = 
                {
                  
                           // new SqlParameter("@str_comp_name", user.tb),
                            new SqlParameter("@str_comp_name", user.str_comp_name),
                            new SqlParameter("@str_contact_name", user.str_contact_name),
                            new SqlParameter("@str_add_1", user.str_add_1==null ? "":user.str_add_1),
                            new SqlParameter("@str_add_2", user.str_add_2==null ? "":user.str_add_2),
                            new SqlParameter("@str_city", user.str_city==null ? "":user.str_city),
                            new SqlParameter("@str_state", user.str_state==null ? "":user.str_state),
                            new SqlParameter("@int_pin_code", user.int_pin_code==null ? 0:user.int_pin_code),
                            new SqlParameter("@str_country",user.str_country==null ? "":user.str_country),
                            new SqlParameter("@str_email",user.str_email==null ? "":user.str_email),
                            new SqlParameter("@int_user_type_id", user.int_user_type_id),
                                                  
                            new SqlParameter("@str_user_name",user.str_user_name),
                            new SqlParameter("@str_password", user.str_password),
                            new SqlParameter("@int_pm_id", user.int_pm_id),
                            new SqlParameter("@int_user_id",SqlDbType.Int){
                             Direction = System.Data.ParameterDirection.Output 
                            },
                            new SqlParameter("@int_invoice_period", user.int_invoice_period),

                };

                    dbcnx.Database.ExecuteSqlCommand("usp_insert_user @str_comp_name,@str_contact_name,@str_add_1,@str_add_2,@str_city,@str_state,@int_pin_code,@str_country,@str_email,@int_user_type_id,@str_user_name,@str_password,@int_pm_id,@int_user_id out,@int_invoice_period", param);

                    _lVal = Convert.ToInt32(param[13].Value);

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
        public int user_update(UserMasterVM user)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param = 
                {
                  
                            new SqlParameter("@int_id", user.int_id),
                            new SqlParameter("@str_comp_name", user.str_comp_name),
                            new SqlParameter("@str_contact_name", user.str_contact_name),
                            new SqlParameter("@str_add_1", user.str_add_1==null ? "":user.str_add_1),
                            new SqlParameter("@str_add_2", user.str_add_2==null ? "":user.str_add_2),
                            new SqlParameter("@str_city", user.str_city==null ? "":user.str_city),
                            new SqlParameter("@str_state", user.str_state==null ? "":user.str_state),
                            new SqlParameter("@int_pin_code", user.int_pin_code==null ? 0:user.int_pin_code),
                            new SqlParameter("@str_country",user.str_country==null ? "":user.str_country),
                            new SqlParameter("@str_email",user.str_email==null ? "":user.str_email),
                            new SqlParameter("@int_user_type_id", user.int_user_type_id),
                                                  
                            new SqlParameter("@str_user_name",user.str_user_name),
                            new SqlParameter("@str_password", user.str_password),
                           
                            new SqlParameter("@int_pm_id",user.int_pm_id),
                            new SqlParameter("@int_invoice_period",user.int_invoice_period)

                };

                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_update_user @int_id,@str_comp_name,@str_contact_name,@str_add_1,@str_add_2,@str_city,@str_state,@int_pin_code,@str_country,@str_email,@int_user_type_id,@str_user_name,@str_password,@int_pm_id,@int_invoice_period", param);
                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return _lVal;
        }

        public int user_profile(UserMasterVM user)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param = 
                {
                  
                            new SqlParameter("@int_id", user.int_id),
                            new SqlParameter("@str_comp_name", user.str_comp_name),
                            new SqlParameter("@str_contact_name", user.str_contact_name),
                            new SqlParameter("@str_add_1", user.str_add_1==null ? "":user.str_add_1),
                            new SqlParameter("@str_add_2", user.str_add_2==null ? "":user.str_add_2),
                            new SqlParameter("@str_city", user.str_city==null ? "":user.str_city),
                            new SqlParameter("@str_state", user.str_state==null ? "":user.str_state),
                            new SqlParameter("@int_pin_code", user.int_pin_code==null ? 0:user.int_pin_code),
                            new SqlParameter("@str_country",user.str_country==null ? "":user.str_country),
                            new SqlParameter("@str_email",user.str_email==null ? "":user.str_email),
                           

                };

                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_change_profile @int_id,@str_comp_name,@str_contact_name,@str_add_1,@str_add_2,@str_city,@str_state,@int_pin_code,@str_country,@str_email", param);
                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return _lVal;
        }
        #endregion

        #region Delete Method
        public int user_delete(int int_id)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param = 
                {
                    new SqlParameter("@int_id", int_id)
                };

                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_delete_user @int_id", param);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return _lVal;
        }

        public int invoice_delete(int int_invoice_id)
        {
            int _lVal = 0;
            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param =
                {
                    new SqlParameter("@int_invoice_id", int_invoice_id)
                };

                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_delete_invocie @int_invoice_id", param);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return _lVal;
        }
        #endregion

        #region ChangePassword Method
        public int user_changepassword(UserMasterVM user)
        {
            int _lVal = 0;

            try
            {
                using (DB_TenantMNGEntities dbcnx = new DB_TenantMNGEntities())
                {
                    SqlParameter[] param = 
                {
                  
                           // new SqlParameter("@str_comp_name", user.tb),
                            new SqlParameter("@int_id", user.int_id),
                            new SqlParameter("@str_new_password", user.str_new_password),
                            
                };



                    _lVal = dbcnx.Database.ExecuteSqlCommand("usp_change_password @int_id,@str_new_password", param);


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