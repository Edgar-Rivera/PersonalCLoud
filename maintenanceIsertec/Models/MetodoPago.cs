using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace maintenanceIsertec.Models
{
    public class MetodoPago
    {
        public int id { get; set; }
        public string usuario { get; set; }
        public string codigo { get; set; }

        public string pin { get; set; }
        public string fecha_vencimiento { get; set; }
        public string fecha_vencimiento_v { get; set; }
    }
}