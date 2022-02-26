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
        public string str_meter_id { get; set; }

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

        [Display(Name = "dec_peak_energy_rate", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_peak_energy_rate { get; set; }

        [Display(Name = "dec_inter_energy_rate", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_inter_energy_rate { get; set; }

        [Display(Name = "date_pay_date", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_pay_date")]
        public Nullable<System.DateTime> date_pay_date { get; set; }

        [Display(Name = "dec_total", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_total { get; set; }

        [Display(Name = "dec_tax_amt", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_tax_amt { get; set; }

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

        public Nullable<decimal> dec_peak_energy_amt { get; set; }

        public Nullable<decimal> dec_inter_energy_amt { get; set; }

        public Nullable<decimal> dec_base_amt { get; set; }

        [Display(Name = "demanda_base", ResourceType = typeof(Resource))]
        public Nullable<decimal> demanda_base { get; set; }

        [Display(Name = "demanda_intermedia", ResourceType = typeof(Resource))]
        public Nullable<decimal> demanda_intermedia { get; set; }

        [Display(Name = "demanda_punta", ResourceType = typeof(Resource))]
        public Nullable<decimal> demanda_punta { get; set; }

        public Nullable<decimal> energia_activa { get; set; }

        public Nullable<decimal> energia_reactiva { get; set; }

        [Display(Name = "suministro", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "distribucion_enter_amount")]
        public Nullable<decimal> suministro { get; set; }

        [Display(Name = "distribucion", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "distribucion_enter_amount")]
        public Nullable<decimal> distribucion { get; set; }

        [Display(Name = "tarifa_transmision", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "transmision_enter_amount")]
        public Nullable<decimal> tarifa_transmision { get; set; }

        [Display(Name = "operacion_cenace", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "cencace_enter_amount")]
        public Nullable<decimal> operacion_cenace { get; set; }

        [Display(Name = "capacidad", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "capacidad_enter_amount")]
        public Nullable<decimal> capacidad { get; set; }

        [Display(Name = "cre_servicios_conexos", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "servicios_conexos_enter_amount")]
        public Nullable<decimal> cre_servicios_conexos { get; set; }

        [Display(Name = "precio_suministro", ResourceType = typeof(Resource))]
        public Nullable<decimal> precio_suministro { get; set; }

        [Display(Name = "precio_distribucion", ResourceType = typeof(Resource))]
        public Nullable<decimal> precio_distribucion { get; set; }

        [Display(Name = "precio_transmision", ResourceType = typeof(Resource))]
        public Nullable<decimal> precio_transmision { get; set; }

        [Display(Name = "precio_cenace", ResourceType = typeof(Resource))]
        public Nullable<decimal> precio_cenace { get; set; }

        [Display(Name = "precio_energia", ResourceType = typeof(Resource))]
        public Nullable<decimal> precio_energia { get; set; }

        [Display(Name = "precio_capacidad", ResourceType = typeof(Resource))]
        public Nullable<decimal> precio_capacidad { get; set; }

        [Display(Name = "precio_cre_servicios_conexos", ResourceType = typeof(Resource))]
        public Nullable<decimal> precio_cre_servicios_conexos { get; set; }

        [Display(Name = "precio_dos_porciento_baja_tension", ResourceType = typeof(Resource))]
        public Nullable<decimal> precio_dos_porciento_baja_tension { get; set; }

        [Display(Name = "precio_decuento_bonificacion", ResourceType = typeof(Resource))]
        public Nullable<decimal> precio_decuento_bonificacion { get; set; }

    }
}