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
    
    public partial class tbl_invoice_details
    {
        public int int_id { get; set; }
        public string str_meter_id { get; set; }
        public Nullable<int> int_invoice_id { get; set; }
        public Nullable<decimal> dec_peak_energy { get; set; }
        public Nullable<decimal> dec_peak_energy_rate { get; set; }
        public Nullable<decimal> dec_peak_energy_amt { get; set; }
        public Nullable<decimal> dec_inter_energy { get; set; }
        public Nullable<decimal> dec_inter_energy_rate { get; set; }
        public Nullable<decimal> dec_inter_energy_amt { get; set; }
        public Nullable<decimal> dec_base_energy { get; set; }
        public Nullable<decimal> dec_base_rate { get; set; }
        public Nullable<decimal> dec_base_amt { get; set; }
        public Nullable<decimal> demanda_base { get; set; }
        public Nullable<decimal> demanda_intermedia { get; set; }
        public Nullable<decimal> demanda_punta { get; set; }
        public Nullable<decimal> energia_activa { get; set; }
        public Nullable<decimal> energia_reactiva { get; set; }
    
        public virtual tbl_invoice tbl_invoice { get; set; }
    }
}
