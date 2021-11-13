using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sap.Data.Hana;
using maintenanceIsertec.Models;

namespace maintenanceIsertec.Connection
{
    /* Todos los metodos de esta clase utilizan, la libreria SAP.DATA.HANA, y no se realiza por medio de la capa de servicio Service Layer */
    public class ServiceCallsUser
    {
        public List<ServiceCallDashboard> getUserDashboard()
        {
            /* Metodo que contabiliza el total de ordenes de un tecnico asignado, para mostrar en el dashboard */
            var data = new List<ServiceCallDashboard>();
            HanaConnection conn = new HanaConnection();
            conn = connectionHana.connectionResult();
            HanaCommand cmd = new HanaCommand("SELECT A.\"callID\", A.\"DocNum\", B.\"Line\", B.\"U_Estado\", B.\"HandledBy\", B.\"U_RutinaEquipo\", B.\"U_RutinaMantenimiento\"" +
                " FROM OSCL A " +
                "INNER JOIN SCL6 B ON B.\"SrcvCallID\" = A.\"callID\"" +
                "INNER JOIN OUSR C ON B.\"HandledBy\" = C.\"USERID\"" +
                "WHERE B.\"U_Estado\" = '1' ORDER BY 1 DESC LIMIT 1000", conn);
            HanaDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                data.Add(new ServiceCallDashboard()
                {
                    callID = reader.GetString(0),
                    DocNum = reader.GetString(1),
                    Line = reader.GetString(2),
                    U_Estado = reader.GetString(3),
                    HandledBy = reader.GetString(4),
                    U_RutinaEquipo = reader.GetString(5),
                    U_RutinaMantenimiento = reader.GetString(6)
                });
            }
            conn.Close();
            return data;
        }
    }
}