//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TenantMNG.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_tarifas
    {
        public int int_tarifas_id { get; set; }
        public Nullable<decimal> suministro { get; set; }
        public Nullable<decimal> distribucion { get; set; }
        public Nullable<decimal> tarifa_transmision { get; set; }
        public Nullable<decimal> operacion_cenace { get; set; }
        public Nullable<decimal> dec_base_rate { get; set; }
        public Nullable<decimal> dec_inter_energy_rate { get; set; }
        public Nullable<decimal> dec_peak_energy_rate { get; set; }
        public Nullable<decimal> capacidad { get; set; }
        public Nullable<decimal> cre_servicios_conexos { get; set; }
        public string mes_tarifas { get; set; }
    }
}
