using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using TenantMNG.Models;

namespace TenantMNG.Core
{
    public static class CommonCls
    {

        //static variable for page size
        public static int _pagesize = 10;

        //static variable for user type admin
        public static int _usertypeAdmin = 1;

        //static variable for user type Property Manager
        public static int _usertypePM = 2;

        //static variable for user type Tenant
        public static int _usertypeTenant = 3;

        //static variable for application name
        public static string _applicationname = "Tenant Managemant";

        //static variable for application url
        public static string _applicationurl = "http://www.tecnobuildings.com/";

        //send mail function

        public static bool sendMail(string toemail, string sub, string mess, string path, string cc = null, string bcc = null)
        {
            try
            {

                string senderID = ConfigurationSettings.AppSettings["femail"].ToString();
                //string senderID = "noreply@appraisalhouseusa.com";

                string senderPassword = ConfigurationSettings.AppSettings["pass"].ToString();
                //string senderPassword = "MLay2016!";
                SmtpClient smtp = new SmtpClient
                {
                    // Host = "smtp.office365.com",// smtp server address here…
                    Host = ConfigurationSettings.AppSettings["smtserver"].ToString(),
                    // Port = 587,
                    Port = Convert.ToInt16(ConfigurationSettings.AppSettings["port"].ToString()),
                    EnableSsl = true,

                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new System.Net.NetworkCredential(senderID, senderPassword),
                    Timeout = 50000,
                };

                using (MailMessage message = new MailMessage(senderID, toemail, sub, mess))
                {

                    message.IsBodyHtml = true;
                    if (!string.IsNullOrEmpty(path))
                    {
                        System.Net.Mail.Attachment attachment;
                        attachment = new System.Net.Mail.Attachment(path);
                        message.Attachments.Add(attachment);

                    }
                    if (!string.IsNullOrEmpty(cc))
                    {
                        message.CC.Add(cc);
                    }
                    if (!string.IsNullOrEmpty(bcc))
                    {
                        message.CC.Add(bcc);
                    }

                    //Add this line to bypass the certificate validation
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
                            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                            System.Security.Cryptography.X509Certificates.X509Chain chain,
                            System.Net.Security.SslPolicyErrors sslPolicyErrors)
                    {
                        return true;
                    };

                    smtp.Send(message);
                }

                return true;
            }
            catch (Exception ex)
            {
                ILog log = log4net.LogManager.GetLogger(typeof(CommonCls));
                log.Error(ex.Message);
                return false;

            }
        }

        //get metername for Tenant
        public static string getMeterName(int tenantid)
        {
            DB_TenantMNGEntities _dbc = new DB_TenantMNGEntities();
            var _tenantmeter = _dbc.tbl_tenant_meter.Where(x => x.int_tenant_id == tenantid && x.bit_is_assign == true).ToList();

            if (_tenantmeter != null && _tenantmeter.Count > 0)
            {
                string _metername = "";
                UdisEntities _dbmeter = new UdisEntities();

                UDI _meter = null;

                foreach (var meterid in _tenantmeter)
                {
                    _meter = new UDI();
                    _meter = _dbmeter.UDIS.OrderByDescending(x => x.CFE_MeterID == meterid.str_meter_id).FirstOrDefault();
                    _metername = _metername + ",\n" + _meter.CFE_MeterID;
                }


                return _metername.TrimStart(',');
            }
            else
                return "Not Assign";

        }

        //get metername for Tenant
        public static string getTenantNameforMeter(string meterid)
        {
            DB_TenantMNGEntities _dbc = new DB_TenantMNGEntities();
            var _tenantmeter = _dbc.tbl_tenant_meter.Where(x => x.str_meter_id == meterid && x.bit_is_assign == true).SingleOrDefault();

            if (_tenantmeter != null)
            {
                return _tenantmeter.tbl_user_master.str_comp_name;
            }
            else
                return Resource.not_assign;

        }


        //get metername from Meter id
        public static string getMeterNamefromId(string meterid)
        {


            if (!string.IsNullOrEmpty(meterid))
            {
               UdisEntities _dbmeter = new UdisEntities();
                var _meter = _dbmeter.UDIS.OrderByDescending(x => x.CFE_MeterID == meterid).FirstOrDefault();

                 return _meter.CFE_MeterID;
            }
            else
                return "NA";

        }


        public static string edate(int? meterid)
        {


            if (meterid != 0)
            {
                DB_TenantMNGEntities dB_Tenant = new DB_TenantMNGEntities();
                var _meter = dB_Tenant.tbl_invoice.Where(x => x.int_invoice_id == meterid).SingleOrDefault();

                return _meter.date_e_bill_date.Value.ToString("dd/MM/yyyy"); 
            }
            else
                return "NA";

        }
        public static string sdate(int? meterid)
        {


            if (meterid != 0)
            {
                DB_TenantMNGEntities dB_Tenant = new DB_TenantMNGEntities();
                var _meter = dB_Tenant.tbl_invoice.Where(x => x.int_invoice_id == meterid).SingleOrDefault();

                return _meter.date_s_bill_date.Value.ToString("dd/MM/yyyy"); ;
            }
            else
                return "NA";

        }



        //get Valuefact from Meter id
        public static string getaluefactfromId(int? meterid)
        {


            if (meterid != 0)
            {
                NiagaraEntities _valuefact = new NiagaraEntities();
                var _meter = _valuefact.HISTORY_CONFIG.Where(x => x.ID == meterid).SingleOrDefault();

                return _meter.VALUEFACETS;
            }
            else
                return "NA";

        }

        //check tenant have meter or not
        public static bool isTenantMeter(int tenantid)
        {
            DB_TenantMNGEntities _dbc = new DB_TenantMNGEntities();
            var _tenantmeter = _dbc.tbl_tenant_meter.Where(x => x.int_tenant_id == tenantid && x.bit_is_assign == true).ToList();

            if (_tenantmeter.Count != 0)
                return true;
            else
                return false;

        }

        //check tenant have meter or not
        public static bool isMeterTenant(string id)
        {
            DB_TenantMNGEntities _dbc = new DB_TenantMNGEntities();
            var _tenantmeter = _dbc.tbl_tenant_meter.Where(x => x.str_meter_id == id && x.bit_is_assign == true).SingleOrDefault();

            if (_tenantmeter != null)
                return true;
            else
                return false;

        }

        public static bool isEditbillRate(int tenantid)
        {
            DB_TenantMNGEntities _dbc = new DB_TenantMNGEntities();
            var _tenantmeter = _dbc.tbl_tenant_billing_info.Where(x => x.int_tenant_id == tenantid).SingleOrDefault();

            if (_tenantmeter != null)
                return true;
            else
                return false;

        }

        public static string DoFormat(decimal? myNumber)
        {
            var s = string.Format("{0:0.00}", myNumber);

            return s;
        }


    }
}