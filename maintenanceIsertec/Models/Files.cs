using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace maintenanceIsertec.Models
{
    public class Files
    {
        public int id_file {get;set;}
        public string nombre {get;set;}
        public string tipo {get;set;}
        public string peso {get;set;}
        public string propietario {get;set;}
        public string compartido {get;set;}       
        public string metadata { get; set; }
        public string username { get; set; }
        public DateTime fecha_creacion {get;set;}
    }
}