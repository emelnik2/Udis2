﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DB_TenantMNGEntities : DbContext
    {
        public DB_TenantMNGEntities()
            : base("name=DB_TenantMNGEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tbl_template> tbl_template { get; set; }
        public virtual DbSet<tbl_user_master> tbl_user_master { get; set; }
        public virtual DbSet<tbl_user_type> tbl_user_type { get; set; }
        public virtual DbSet<tbl_tenant_email_setup> tbl_tenant_email_setup { get; set; }
        public virtual DbSet<tbl_zone_master> tbl_zone_master { get; set; }
        public virtual DbSet<tblMVCChart> tblMVCCharts { get; set; }
        public virtual DbSet<tbl_tenant_meter> tbl_tenant_meter { get; set; }
        public virtual DbSet<tbl_tenant_billing_info> tbl_tenant_billing_info { get; set; }
        public virtual DbSet<tbl_tenant_contract> tbl_tenant_contract { get; set; }
        public virtual DbSet<tbl_pm_billing_hours> tbl_pm_billing_hours { get; set; }
        public virtual DbSet<tbl_tenant_settings> tbl_tenant_settings { get; set; }
        public virtual DbSet<tbl_invoice> tbl_invoice { get; set; }
        public virtual DbSet<tbl_invoice_details> tbl_invoice_details { get; set; }
    }
}
