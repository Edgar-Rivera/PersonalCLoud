using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace maintenanceIsertec.Models
{
    public class UserNameData
    {

        public string InternalKey { get; set; }
        public string UserName { get; set; }
        // CAMPO PARA OBTENER LA UNIDAD PERTENECIENTE EN BASE A DEPARTAMENTO
        public string Department { get; set; }   
    }
}