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
    
    public partial class tbl_user_type
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_user_type()
        {
            this.tbl_user_master = new HashSet<tbl_user_master>();
        }
    
        public int int_type_id { get; set; }
        public string str_type { get; set; }
        public Nullable<bool> bit_active { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_user_master> tbl_user_master { get; set; }
    }
}
