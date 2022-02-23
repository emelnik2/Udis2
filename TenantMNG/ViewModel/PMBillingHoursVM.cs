using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TenantMNG.ViewModel
{
    public class PMBillingHoursVM
    {

        public int int_rate_id { get; set; }
        public Nullable<int> int_pm_id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_time")]
        [RegularExpression("^(1[0-2]|0?[1-9]):[0-5][0-9] [APap][mM]$", ErrorMessage = "HH:MM (AM/PM)")]
        public string str_peak_s_time_m { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_time")]
        [RegularExpression("^(1[0-2]|0?[1-9]):[0-5][0-9] [APap][mM]$", ErrorMessage = "HH:MM (AM/PM)")]
        public string str_peak_e_time_m { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_time")]
        [RegularExpression("^(1[0-2]|0?[1-9]):[0-5][0-9] [APap][mM]$", ErrorMessage = "HH:MM (AM/PM)")]
        public string str_inter_s_time_1_m { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_time")]
        [RegularExpression("^(1[0-2]|0?[1-9]):[0-5][0-9] [APap][mM]$", ErrorMessage = "HH:MM (AM/PM)")]
        public string str_inter_e_time_1_m { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_time")]
        [RegularExpression("^(1[0-2]|0?[1-9]):[0-5][0-9] [APap][mM]$", ErrorMessage = "HH:MM (AM/PM)")]
        public string str_inter_s_time_2_m { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_time")]
        [RegularExpression("^(1[0-2]|0?[1-9]):[0-5][0-9] [APap][mM]$", ErrorMessage = "HH:MM (AM/PM)")]
        public string str_inter_e_time_2_m { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_time")]
        [RegularExpression("^(1[0-2]|0?[1-9]):[0-5][0-9] [APap][mM]$", ErrorMessage = "HH:MM (AM/PM)")]
        public string str_base_s_time_m { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_time")]
        [RegularExpression("^(1[0-2]|0?[1-9]):[0-5][0-9] [APap][mM]$", ErrorMessage = "HH:MM (AM/PM)")]
        public string str_base_e_time_m { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_time")]
        [RegularExpression("^(1[0-2]|0?[1-9]):[0-5][0-9] [APap][mM]$", ErrorMessage = "HH:MM (AM/PM)")]
        public string str_base_s_time_sat { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_time")]
        [RegularExpression("^(1[0-2]|0?[1-9]):[0-5][0-9] [APap][mM]$", ErrorMessage = "HH:MM (AM/PM)")]
        public string str_base_e_time_sat { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_time")]
        [RegularExpression("^(1[0-2]|0?[1-9]):[0-5][0-9] [APap][mM]$", ErrorMessage = "HH:MM (AM/PM)")]
        public string str_inter_s_time_sat { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_time")]
        [RegularExpression("^(1[0-2]|0?[1-9]):[0-5][0-9] [APap][mM]$", ErrorMessage = "HH:MM (AM/PM)")]
        public string str_inter_e_time_sat { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_time")]
        [RegularExpression("^(1[0-2]|0?[1-9]):[0-5][0-9] [APap][mM]$", ErrorMessage = "HH:MM (AM/PM)")]
        public string str_inter_s_time_2_sat { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_time")]
        [RegularExpression("^(1[0-2]|0?[1-9]):[0-5][0-9] [APap][mM]$", ErrorMessage = "HH:MM (AM/PM)")]
        public string str_inter_e_time_2_sat { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_time")]
        [RegularExpression("^(1[0-2]|0?[1-9]):[0-5][0-9] [APap][mM]$", ErrorMessage = "HH:MM (AM/PM)")]
        public string str_peak_s_time_sat { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_time")]
        [RegularExpression("^(1[0-2]|0?[1-9]):[0-5][0-9] [APap][mM]$", ErrorMessage = "HH:MM (AM/PM)")]
        public string str_peak_e_time_sat { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_time")]
        [RegularExpression("^(1[0-2]|0?[1-9]):[0-5][0-9] [APap][mM]$", ErrorMessage = "HH:MM (AM/PM)")]
        public string str_base_s_time_sun { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_time")]
        [RegularExpression("^(1[0-2]|0?[1-9]):[0-5][0-9] [APap][mM]$", ErrorMessage = "HH:MM (AM/PM)")]
        public string str_base_e_time_sun { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_time")]
        [RegularExpression("^(1[0-2]|0?[1-9]):[0-5][0-9] [APap][mM]$", ErrorMessage = "HH:MM (AM/PM)")]
        public string str_inter_s_time_sun { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "val_enter_time")]
        [RegularExpression("^(1[0-2]|0?[1-9]):[0-5][0-9] [APap][mM]$", ErrorMessage = "HH:MM (AM/PM)")]
        public string str_inter_e_time_sun { get; set; }

    }
}