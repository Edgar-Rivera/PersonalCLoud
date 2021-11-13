using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sap.Data.Hana;

namespace maintenanceIsertec.Connection
{
    public class connectionHana
    {
        /* Metodo principal de conexion de la libreria SAP.DATA.HANA, prporcioan una conexion abierta para los metodos que lo utilicen, cada metodo debe de cerrar la conexion al finalizar 
         * sus operaciones*/
        public static HanaConnection connectionResult()
        {
            HanaConnection conn = new HanaConnection();
            conn.ConnectionString = "Server=192.168.1.221:30015;UID=SAPDBA;PWD=manag3RS; Current Schema=SBO_ISERTEC_GT";
            conn.Open();
            return conn;
        }
    }
}