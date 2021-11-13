using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Odbc;

namespace maintenanceIsertec
{
    class DBMODEL
    {
        public static OdbcConnection connectionResult()
        {
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            return cnx;
        }
    }
}