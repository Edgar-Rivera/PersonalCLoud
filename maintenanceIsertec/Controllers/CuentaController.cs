using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using maintenanceIsertec.Models;

namespace maintenanceIsertec.Controllers
{
    public class CuentaController : Controller
    {
        // GET: Cuenta
        public ActionResult Configuracion()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult CrearCuenta()
        {
            return View();
        }
        public bool ingresaMetodoPago(string usuario, string codigo, string pin, string f1, string f2 )
        {
             OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
             cnx.Open();
             string sql = string.Format("INSERT INTO Pago VALUES (null,'{0}','{1}','{2}','{3}','{4}');", usuario,codigo,pin,f1,f2);
             OdbcCommand cmd = new OdbcCommand(sql, cnx);
             if (cmd.ExecuteNonQuery() == 1)
             {
                 return true;
             }
             else
             {
                 return false;
             }
           
        }

        public void creaDirectorioROOT(string nombre)
        {
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("insert into folders values (null, 'root', '{0}','S',NOW());", nombre);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            cmd.ExecuteNonQuery();
           
        }

        [HttpPost]
        public ActionResult CrearCuenta(Models.Cuenta micuenta, string test, string test2, string test3, string test4 )
        {
            string temp = string.Empty;
            if(micuenta.tipocuenta == "Estandar")
            {
                temp = "15 GB";
            } else if(micuenta.tipocuenta == "Avanzada")
            {
                temp = "50 GB";
            }
            else
            {
                temp = "100 GB";
            }
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("INSERT INTO USUARIO VALUES ('{0}','{1}','{2}','{3}','{4}',now(),'{5}','{6}','user');", micuenta.codusuario, micuenta.password_user,
                micuenta.firstname,micuenta.lastname,micuenta.email,micuenta.tipocuenta,temp);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            if (cmd.ExecuteNonQuery() == 1)
            {
                ingresaMetodoPago(micuenta.codusuario,test,test2,test3,test4);
                creaDirectorioROOT(micuenta.codusuario);
                return View("CuentaCreada");
            }
            else
            {
                return View("Error");
            }
          
        }
    }
}