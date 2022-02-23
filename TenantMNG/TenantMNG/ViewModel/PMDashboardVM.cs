using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TenantMNG.Models;

namespace TenantMNG.ViewModel
{
    public class PMDashboardVM
    {

        public IEnumerable<tbl_user_master> tbl_user_master { get; set; }

        public IEnumerable<tbl_invoice> tbl_invoice { get; set; }
    }
}