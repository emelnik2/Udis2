using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace TenantMNG.Models
{
    [Table("ProEnergyLog")]
    public class ProEnergyLog
    {
        [Display(Name = "TENETID")]
        public int TENETID { get; set; }
        [Display(Name = "TABLE_NAME")]
        public string TABLE_NAME { get; set; }
        [Display(Name = "TIMESTAMP")]
        public DateTime? TIMESTAMP { get; set; }
        [Display(Name = "VALUE")]
        public Decimal? VALUE { get; set; }
        [Display(Name = "Tenent Name")]
        public string TenentName { get; set; }

    }

}



  
   