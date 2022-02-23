using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TenantMNG.ViewModel
{
    public class LoginVM
    {
        [Display(Name = "str_user_name", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "unamerequire")]

        public string str_user_name { get; set; }

        [Display(Name = "str_password", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "passrequire")]

        public string str_password { get; set; }
    }
}