using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TenantMNG.ViewModel
{
    public class TenantContratVM
    {

        public int int_contract_id { get; set; }


        public Nullable<int> int_tenant_id { get; set; }

        [Display(Name = "s_date", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_s_date")]

        public System.DateTime s_date { get; set; }

        [Display(Name = "e_date", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_e_date")]

        public System.DateTime e_date { get; set; }
    }
}