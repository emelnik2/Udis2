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
    
    public partial class tbl_tenant_email_setup
    {
        public int int_email_id { get; set; }
        public Nullable<int> int_tenant_id { get; set; }
        public string str_from_email { get; set; }
        public string str_cc_email { get; set; }
        public string str_bcc_email { get; set; }
        public string str_subject { get; set; }
        public string str_body { get; set; }
    
        public virtual tbl_user_master tbl_user_master { get; set; }
    }
}
