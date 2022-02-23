using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TenantMNG.ViewModel
{
    public class EmailSetupVM
    {


        public int int_email_id { get; set; }
        public Nullable<int> int_tenant_id { get; set; }

        [Display(Name = "str_from_email",ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource),ErrorMessageResourceName = "val_form_email")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$",  ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_invalid_email")]
        public string str_from_email { get; set; }

        [Display(Name = "str_cc_email", ResourceType = typeof(Resource))]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$",  ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_invalid_email")]
        public string str_cc_email { get; set; }

        [Display(Name = "str_bcc_email", ResourceType = typeof(Resource))]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$",  ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_invalid_email")]
        public string str_bcc_email { get; set; }

        [Display(Name = "str_subject", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_sub")]
        public string str_subject { get; set; }

        [Display(Name = "str_body", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_message")]
        public string str_body { get; set; }

    }
}