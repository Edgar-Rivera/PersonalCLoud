using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace maintenanceIsertec.Models
{
    public class Cuenta
    {
        public string codusuario { get; set; }
        public string password_user { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string reg_date { get; set; }
        public string tipocuenta { get; set; }
        public string espacio { get; set; }
        public string rol { get; set; }
    }
}