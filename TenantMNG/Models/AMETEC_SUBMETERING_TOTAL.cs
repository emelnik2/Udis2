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
    
    public partial class AMETEC_SUBMETERING_TOTAL
    {
        public int ID { get; set; }
        public Nullable<System.DateTime> TIMESTAMP { get; set; }
        public Nullable<int> TRENDFLAGS { get; set; }
        public Nullable<int> STATUS { get; set; }
        public Nullable<double> VALUE { get; set; }
        public string TRENDFLAGS_TAG { get; set; }
        public string STATUS_TAG { get; set; }
    }
}
