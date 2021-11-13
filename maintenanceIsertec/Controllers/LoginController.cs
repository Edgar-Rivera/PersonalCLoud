using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using maintenanceIsertec.Models;
using System.Web.Security;
using System.Data.Odbc;

namespace maintenanceIsertec.Controllers
{

    public class LoginController : Controller
    {
        [Authorize]
        public ActionResult logoutSession()
        {
            Session.Clear();
            Session.Abandon(); // Cierra la Sesion por Session de ASP.NET
            FormsAuthentication.SignOut();// Cierra sesión de FormsAuthenticationTicket
            return RedirectToAction("Login");
        }
       
        
       
       
        public  ActionResult Login(SiteUser attemp)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
                    cnx.Open();
                    string sql = string.Format("select firstname, lastname, codusuario, rol from usuario where codusuario = '{0}' and password_user = '{1}'", attemp.UserName, attemp.Password);
                    OdbcCommand cmd = new OdbcCommand(sql, cnx);
                    OdbcDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                       
                        Session.Add("nombre", reader.GetString(0)); // Variable agregada a la cookie de sesión de ASP.NET para guardar el nombre del usuario logueado
                        Session.Add("apellido",reader.GetString(1)); // Variable agregada a la cookie de sesión de ASP.NET para guardar el codigo de tecnico del usuario logueado, para posteriormente filtrar sus OT's 
                        Session.Add("user",reader.GetString(2));
                        Session.Add("rol",reader.GetString(3));
                        Session.Timeout = 560;  // Tiempo que ASP.NET guardara la cookie de sesión del Usuario, tambien de debe manejar desde IIS o Apache Tomcat
                        DateTime expire = DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes);
                        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, reader.GetString(0), DateTime.Now, expire, false, attemp.UserName); // se crea un nuevo ticket de Sesión para el usuario
                        string hashTicket = FormsAuthentication.Encrypt(ticket); // Encriptación del ticket para no ser expuesta en el navegador
                        HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hashTicket);
                        cookie.HttpOnly = true;
                        HttpContext.Response.Cookies.Add(cookie);
                        return RedirectToAction("Index", "Home");

                    }
                    else
                    {
                        return View(attemp);
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Usuario o contraseña  Incorrecto!.");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("Pass", "Usuario o Contraseña Incorrectos!");
                return View();
            }

        }
        #region Certificado
        private static bool RemoteSSLTLSCertificateValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors ssl)
        {
            return true;
        }
        #endregion
    }
}
