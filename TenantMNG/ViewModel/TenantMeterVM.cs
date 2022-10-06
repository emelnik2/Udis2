using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TenantMNG.ViewModel
{
    public class TenantMeterVM
    {
        public int int_id { get; set; }
        public Nullable<int> int_tenant_id { get; set; }

        [Required(ErrorMessage = "Please Select Meter")]
        public string str_meter_id { get; set; }
        public Nullable<System.DateTime> date_assign_date { get; set; }
        public Nullable<System.DateTime> date_detach_date { get; set; }
        public bool bit_is_assign { get; set; }

        public List<SelectListItem> Meters { get; set; }
        public int[] MetersID { get; set; }

        public int multiplier { get; set; }



    }

   
}