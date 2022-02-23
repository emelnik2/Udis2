using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TenantMNG.ViewModel
{
    public class TenantEnergyVM
    {

        //public DateTime date_s_bill_date { get; set; }


        //public DateTime date_e_bill_date { get; set; }

        public DateTime date_s_bill_date { get; set; }
        
        public DateTime date_e_bill_date { get; set; }

        public string str_meter_id { get; set; }

        public string str_peak_s_time { get; set; }


        public string str_peak_e_time { get; set; }


        public string str_inter_s_time { get; set; }


        public string str_inter_e_time { get; set; }

        public int int_tenant_id { get; set; }
        public int int_invoice_id { get; set; }

        public decimal dec_prev_peak_energy { get; set; }

        public decimal dec_prev_inter_energy { get; set; }

        public string str_meter_name { get; set; }

        public decimal dec_peak_energy_rate { get; set; }

        public decimal dec_inter_energy_rate { get; set; }

        public decimal dec_base_rate { get; set; }

    }
}