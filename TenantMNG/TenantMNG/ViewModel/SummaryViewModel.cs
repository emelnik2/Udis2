using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TenantMNG.ViewModel
{
    public class SummaryViewModel
    {
      

        public int invoiceid { get; set; }

        public string Name { get; set; }
        public int? meterid { get; set; }


        public DateTime? fromdate { get; set; }


        public DateTime? todate { get; set; }


        public Decimal? peakenergy { get; set; }


        public Decimal? interenergy { get; set; }

        public Decimal customecharges { get; set; }

        public Decimal? decdemad { get; set; }
        public Decimal? dectaxamt { get; set; }
        public Decimal? dectotal { get; set; }

        public Decimal? totalenergy { get; set; }
        public Decimal? baseenergy { get; set; }

         public string dateinvoice { get; set; }
        public decimal? Value { get; set; }


    }
}