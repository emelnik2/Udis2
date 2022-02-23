using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TenantMNG.Models;

namespace TenantMNG.ViewModel
{
    public class TenantSettingVM
    {
        public int int_id { get; set; }

        public Nullable<int> int_tenant_id { get; set; }


        [Required(ErrorMessageResourceName = "val_enter_val", ErrorMessageResourceType = typeof(Resource))]
        //[Display(Name = "dec_demanda_facturable", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_demanda_facturable { get; set; }

        [Required(ErrorMessageResourceName = "val_enter_val", ErrorMessageResourceType = typeof(Resource))]
        //[Display(Name = "dec_total_ene", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_total_ene { get; set; }

        public virtual tbl_user_master tbl_user_master { get; set; }

    }
}