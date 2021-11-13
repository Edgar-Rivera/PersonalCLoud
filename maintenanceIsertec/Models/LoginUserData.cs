using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace maintenanceIsertec.Models
{
    public class LoginUserData
    {
        public string CompanyDB { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}