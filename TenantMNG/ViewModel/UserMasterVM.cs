using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TenantMNG.Models;

namespace TenantMNG.ViewModel
{
    public class UserMasterVM
    {


        public int int_id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_cmp_name")]
        [Display(Name = "str_comp_name", ResourceType = typeof(Resource))]
        public string str_comp_name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_contact_name")]
        [Display(Name = "str_contact_name", ResourceType = typeof(Resource))]
        public string str_contact_name { get; set; }

        [Display(Name = "str_add_1", ResourceType = typeof(Resource))]
        public string str_add_1 { get; set; }

        [Display(Name = "str_add_2", ResourceType = typeof(Resource))]
        public string str_add_2 { get; set; }

        [Display(Name = "str_city", ResourceType = typeof(Resource))]
        public string str_city { get; set; }

        [Display(Name = "str_state", ResourceType = typeof(Resource))]
        public string str_state { get; set; }

        [Display(Name = "int_pin_code", ResourceType = typeof(Resource))]
        public Nullable<int> int_pin_code { get; set; }


        public Nullable<int> int_invoice_period { get; set; }

        [Display(Name = "str_country", ResourceType = typeof(Resource))]
        public string str_country { get; set; }

        [Display(Name = "int_user_type_id", ResourceType = typeof(Resource))]
        public Nullable<int> int_user_type_id { get; set; }

        [Display(Name = "str_user_name", ResourceType = typeof(Resource))]
        [RegularExpression("^[a-zA-Z0-9]+([._@]?[a-zA-Z0-9]+){9,16}$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_uname")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_uname")]
        public string str_user_name { get; set; }

        [Display(Name = "str_password", ResourceType = typeof(Resource))]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[$@$!%*#?&amp;])[A-Za-z\d$@$!%*#?&amp;]{9,}$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_pass")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_pass")]
        public string str_password { get; set; }

        [Display(Name = "str_new_password", ResourceType = typeof(Resource))]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[$@$!%*#?&amp;])[A-Za-z\d$@$!%*#?&amp;]{9,}$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_pass")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_new_pass")]
        public string str_new_password { get; set; }
        public Nullable<int> int_pm_id { get; set; }

        [Display(Name = "str_email", ResourceType = typeof(Resource))]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_invalid_email")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_email")]
        public string str_email { get; set; }
        public Nullable<System.DateTime> date_created { get; set; }


        public virtual tbl_user_type tbl_user_type { get; set; }
    }
}