using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace maintenanceIsertec.Models
{
    public class shared_files
    {
        public int id { get; set; }
        public int id_file { get; set; }
        public string nombre { get; set; }
        public string detalles { get; set; }
        public string propietario { get; set; }
        public DateTime reg_date { get; set; }
        public string usuario { get; set; }
    }
}