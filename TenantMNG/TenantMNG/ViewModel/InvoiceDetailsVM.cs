using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TenantMNG.ViewModel
{
    public class InvoiceDetailsVM
    {
        public int int_id { get; set; }

        //public Nullable<int> int_meter_id { get; set; }

        [Display(Name = "dec_peak_energy", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_peak_energy { get; set; }

        [Display(Name = "dec_inter_energy", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_inter_energy { get; set; }



        [Display(Name = "dec_peak_energy_rate", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_peak_energy_rate { get; set; }

        [Display(Name = "dec_inter_energy_rate", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_inter_energy_rate { get; set; }

        [Display(Name = "dec_base_energy", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_base_energy { get; set; }

        [Display(Name = "dec_base_rate", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_base_rate { get; set; }

        [Display(Name = "dec_peak_energy_amt", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_peak_energy_amt { get; set; }

        [Display(Name = "dec_inter_energy_amt", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_inter_energy_amt { get; set; }

    }
}