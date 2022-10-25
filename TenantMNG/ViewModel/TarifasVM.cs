using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TenantMNG.ViewModel
{
    public class TarifasVM
    {
        public int int_tarifas_id { get; set; }

        [Display(Name = "mes_tarifas", ResourceType = typeof(Resource))]
        public String mes_tarifas { get; set; }

        [Display(Name = "ano_tarifas", ResourceType = typeof(Resource))]
        public int ano_tarifas { get; set; }


        [Display(Name = "dec_base_rate", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_base_rate { get; set; }

        [Display(Name = "dec_peak_energy_rate", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_peak_energy_rate { get; set; }

        [Display(Name = "dec_inter_energy_rate", ResourceType = typeof(Resource))]
        public Nullable<decimal> dec_inter_energy_rate { get; set; }

        [Display(Name = "suministro", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "suministro_enter_amount")]
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

        public IEnumerable<SelectListItem> Meses { get; set; }

        public IEnumerable<SelectListItem> Anios { get; set; }

        [Display(Name = "total_cantidad_cfe", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "total_cantidad_cfe_enter_amount")]
        public Nullable<decimal> total_cantidad_cfe { get; set; }


    }

}