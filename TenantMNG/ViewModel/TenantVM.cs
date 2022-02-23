using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TenantMNG.Models;

namespace TenantMNG.ViewModel
{
    public class TenantVM
    {

        public UserMasterVM user_contact_info { get; set; }

        public EmailSetupVM emailsetup { get; set; }

        public TenantSettingVM tenantsetting { get; set; }

        public TenantContratVM tenantcontract { get; set; }

        public int int_id { get; set; }
        public Nullable<int> int_tenant_id { get; set; }

        [Display(Name = "int_template_id", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_template")]
        public Nullable<int> int_template_id { get; set; }

        [Display(Name = "dec_rate", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_bill_rate")]
        public string dec_rate { get; set; }

        [Display(Name = "dec_seasonal_multi_rate", ResourceType = typeof(Resource))]
        public string dec_seasonal_multi_rate { get; set; }

        [Display(Name = "dec_surcharge_amt", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_amt")]
        public string dec_surcharge_amt { get; set; }

        [Display(Name = "str_min_billable_over", ResourceType = typeof(Resource))]
        [RegularExpression("^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_invalid_time")]
        public string str_min_billable_over { get; set; }

        [Display(Name = "str_charge_tenant_min", ResourceType = typeof(Resource))]
        [RegularExpression("^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_invalid_time")]
        public string str_charge_tenant_min { get; set; }

        [Display(Name = "str_charge_tenant_max", ResourceType = typeof(Resource))]
        [RegularExpression("^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_invalid_time")]
        public string str_charge_tenant_max { get; set; }
        public bool bit_is_consolidate_zone { get; set; }
        public bool bit_is_print { get; set; }
        public bool bit_is_file { get; set; }
        public Nullable<int> int_type { get; set; }

        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_invalid_email")]
        public string str_email { get; set; }
        public bool bit_is_seasonal_rate { get; set; }
        public bool bit_is_surchare { get; set; }

        public IList<tbl_user_master> property_manager { get; set; }

        public IList<tbl_template> template { get; set; }

        public int int_invoice_period { get; set; }


        public IEnumerable<tbl_user_master> tbl_user_master { get; set; }

        public IEnumerable<tbl_invoice> tbl_invoice { get; set; }
    } 
}