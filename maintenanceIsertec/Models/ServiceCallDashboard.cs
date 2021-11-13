using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace maintenanceIsertec.Models
{
    public class ServiceCallDashboard
    {
        public string callID { get; set; }
        public string DocNum { get; set; }
        public string Line { get; set; }
        public string U_Estado { get; set; }
        public string HandledBy { get; set; }
        public string U_RutinaEquipo { get; set; }
        public string U_RutinaMantenimiento { get; set; }

    }
}