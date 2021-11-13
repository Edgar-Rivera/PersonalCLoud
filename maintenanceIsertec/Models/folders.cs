using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace maintenanceIsertec.Models
{
    public class folders
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string propietario { get; set; }
        public string compartido { get; set; }
        public DateTime regdate { get; set; }
    }
}