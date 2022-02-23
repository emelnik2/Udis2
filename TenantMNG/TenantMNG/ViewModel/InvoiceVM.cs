using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TenantMNG.ViewModel
{
    public class InvoiceVM
    {
        public int int_invoice_id { get; set; }
        public Nullable<System.DateTime> date_invoice_date { get; set; }
        public Nullable<int> int_meter_id { get; set; }

        [Display(Name = "int_tenant_id", ResourceType = typeof(Resource))]
        public Nullable<int> int_tenant_id { get; set; }
        public Nullable<bool> bit_tenant_active { get; set; }

        [Display(Name = "date_s_bill_date", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_from_date")]
        public Nullable<System.DateTime> date_s_bill_date { get; set; }

        [Display(Name = "date_e_bill_date", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_to_date")]
        public Nullable<System.DateTime> date_e_bill_date { get; set; }

        [Display(Name = "dec_peak_energy", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_peak_energy { get; set; }

        [Display(Name = "dec_inter_energy", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_inter_energy { get; set; }


        [Display(Name = "dec_prev_peak_energy", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_prev_peak_energy { get; set; }

        [Display(Name = "dec_prev_inter_energy", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_prev_inter_energy { get; set; }

        [Display(Name = "dec_current_peak_energy", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_current_peak_energy { get; set; }

        [Display(Name = "dec_current_inter_energy", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_current_inter_energy { get; set; }


        [Display(Name = "dec_custome_charges", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_custome_charges { get; set; }

        [Display(Name = "dec_peak_energy_rate", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_peak_energy_rate { get; set; }

        [Display(Name = "dec_inter_energy_rate", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_inter_energy_rate { get; set; }

        [Display(Name = "str_custome_charge_desc", ResourceType = typeof(Resource))]
        public string str_custome_charge_desc { get; set; }

        [Display(Name = "date_pay_date", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_pay_date")]
        public Nullable<System.DateTime> date_pay_date { get; set; }

        [Display(Name = "dec_demad", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_demad { get; set; }


        [Display(Name = "dec_total", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_total { get; set; }

        [Display(Name = "dec_tax_amt", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_tax_amt { get; set; }

        [Display(Name = "demand_fact", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_pay_date")]
        public Nullable<decimal> dec_demanda_facturable { get; set; }

        [Display(Name = "dec_total_ene", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_pay_date")]
        public Nullable<decimal> dec_total_ene { get; set; }

        public Nullable<decimal> date_modify { get; set; }

        public string meter_name { get; set; }


        public string str_peak_s_time { get; set; }


        public string str_peak_e_time { get; set; }


        public string str_inter_s_time { get; set; }


        public string str_inter_e_time { get; set; }

        public bool bit_is_editable { get; set; }

        public Nullable<int> int_invoice_period { get; set; }

        [Display(Name = "dec_base_energy", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_base_energy { get; set; }

        [Display(Name = "dec_base_rate", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_base_rate { get; set; }

        [Display(Name = "dec_demanda_facturable_amount", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_demanda_facturable_amount { get; set; }

        public Nullable<decimal> dec_peak_energy_amt { get; set; }

        public Nullable<decimal> dec_inter_energy_amt { get; set; }

        public Nullable<decimal> dec_base_amt { get; set; }
    }
}