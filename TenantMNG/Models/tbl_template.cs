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
    
    public partial class tbl_template
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_template()
        {
            this.tbl_tenant_billing_info = new HashSet<tbl_tenant_billing_info>();
        }
    
        public int int_temp_id { get; set; }
        public string str_temp_name { get; set; }
        public string str_body { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_tenant_billing_info> tbl_tenant_billing_info { get; set; }
    }
}
