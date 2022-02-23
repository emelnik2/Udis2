using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TenantMNG.Models;

namespace TenantMNG.ViewModel
{
    public class AdminDashboard
    {
        public IEnumerable<tbl_user_master> tbl_PM { get; set; }

        public IEnumerable<tbl_user_master> tbl_Tenant { get; set; }
    }
}