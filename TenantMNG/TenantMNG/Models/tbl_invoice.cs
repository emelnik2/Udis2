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
    
    public partial class tbl_invoice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_invoice()
        {
            this.tbl_invoice_details = new HashSet<tbl_invoice_details>();
        }
    
        public int int_invoice_id { get; set; }
        public Nullable<System.DateTime> date_invoice_date { get; set; }
        public Nullable<int> int_tenant_id { get; set; }
        public Nullable<bool> bit_tenant_active { get; set; }
        public Nullable<System.DateTime> date_s_bill_date { get; set; }
        public Nullable<System.DateTime> date_e_bill_date { get; set; }
        public decimal dec_custome_charges { get; set; }
        public Nullable<decimal> dec_demad { get; set; }
        public Nullable<decimal> dec_total { get ; set; }
        public Nullable<System.DateTime> date_modify { get; set; }
        public Nullable<decimal> dec_tax_amt { get; set; }
        public Nullable<bool> bit_is_editable { get; set; }
        public Nullable<System.DateTime> date_pay_date { get; set; }
        public string str_custome_charge_desc { get; set; }
        public Nullable<decimal> dec_prev_peack_energy { get; set; }
        public Nullable<decimal> dec_prev_inter_energy { get; set; }
        public Nullable<decimal> dec_current_peack_energy { get; set; }
        public Nullable<decimal> dec_current_inter_energy { get; set; }
        public Nullable<decimal> dec_demanda_facturable { get; set; }
        public Nullable<decimal> dec_total_ene { get; set; }
        public Nullable<decimal> dec_demanda_facturable_amount { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_invoice_details> tbl_invoice_details { get; set; }
        public virtual tbl_user_master tbl_user_master { get; set; }

        internal static DB_TenantMNGEntities Where(Func<object, object> p)
        {
            throw new NotImplementedException();
        }

        internal object getMeterNamefromId(object int_meter_id)
        {
            throw new NotImplementedException();
        }
    }
}
