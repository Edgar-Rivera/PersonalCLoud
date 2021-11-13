using ClosedXML.Excel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace maintenanceIsertec.Controllers
{
    [Authorize]
    public class ToolsController : Controller
    {
        // GET: Tools
        public ActionResult Admin(int? page, string findString)
        {
            var data = new List<maintenanceIsertec.Models.Cuenta>();
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("select * from usuario where codusuario  <> '{0}';", Session["user"]);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            OdbcDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                data.Add(new Models.Cuenta()
                {
                    codusuario = reader.GetString(0),
                    password_user = reader.GetString(1),
                    firstname = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    lastname = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    email = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    reg_date = reader.IsDBNull(5) ? ""+DateTime.Now : ""+reader.GetDateTime(5),
                    tipocuenta = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    espacio = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                    rol = reader.GetString(8)
                });
            }
            cnx.Close();
            ViewBag.findString = findString;
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            if (!String.IsNullOrEmpty(findString))
            {
                var obj = data.Where(s => s.codusuario.IndexOf(findString, StringComparison.OrdinalIgnoreCase) >= 0 || s.firstname.IndexOf(findString, 
                    StringComparison.OrdinalIgnoreCase) >= 0 || s.lastname.IndexOf(findString, StringComparison.OrdinalIgnoreCase) >= 0 || s.email.IndexOf(findString, StringComparison.OrdinalIgnoreCase) >= 0 || s.tipocuenta.IndexOf(findString, StringComparison.OrdinalIgnoreCase) >= 0);
                return View(obj.ToPagedList(pageNumber, pageSize));
            }
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult details(string id)
        {
            var data = new List<maintenanceIsertec.Models.Cuenta>();
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("select * from usuario where codusuario = '{0}';", id);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            OdbcDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                data.Add(new Models.Cuenta()
                {
                    codusuario = reader.GetString(0),
                    password_user = reader.GetString(1),
                    firstname = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    lastname = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    email = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    reg_date = reader.IsDBNull(5) ? "" + DateTime.Now : "" + reader.GetDateTime(5),
                    tipocuenta = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    espacio = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                    rol = reader.GetString(8)
                });
            }
            cnx.Close();
            var pago = new List<maintenanceIsertec.Models.MetodoPago>();
            OdbcConnection cnxs = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnxs.Open();
            string sqls = string.Format("select * from pago where usuario = '{0}';", id);
            OdbcCommand cmds = new OdbcCommand(sqls, cnxs);
            OdbcDataReader readers = cmds.ExecuteReader();
            while (readers.Read())
            {
                pago.Add(new Models.MetodoPago()
                {
                    id = readers.IsDBNull(0) ? 0 : readers.GetInt32(0),
                    usuario = readers.IsDBNull(1) ? string.Empty : readers.GetString(1),
                    codigo = readers.IsDBNull(2) ? string.Empty : readers.GetString(2),
                    pin = readers.IsDBNull(3) ? string.Empty : readers.GetString(3),
                    fecha_vencimiento = readers.IsDBNull(4) ? string.Empty : readers.GetString(4),
                    fecha_vencimiento_v = readers.IsDBNull(5) ? string.Empty : readers.GetString(5)
                });
            }
            ViewBag.pago = pago;
           
            cnxs.Close();
            return View(data);
        }
        public FileResult ExportExcel(string findString)
        {
            var data = new List<maintenanceIsertec.Models.Cuenta>();
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("select * from usuario;");
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            OdbcDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                data.Add(new Models.Cuenta()
                {
                    codusuario = reader.GetString(0),
                    password_user = reader.GetString(1),
                    firstname = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    lastname = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    email = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    reg_date = reader.IsDBNull(5) ? "" + DateTime.Now : "" + reader.GetDateTime(5),
                    tipocuenta = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    espacio = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                    rol = reader.GetString(8)
                });
            }
            cnx.Close();
            DataTable dt = new DataTable("Usuarios Registrados");
            dt.Columns.AddRange(new DataColumn[8] { 
                                            new DataColumn("Usuario"),
                                            new DataColumn("Nombre"),
                                            new DataColumn("Apellido"),
                                            new DataColumn("Email") ,
                                            new DataColumn("Fecha Registro") ,
                                            new DataColumn("Tipo Cuenta") ,
                                            new DataColumn("Espacio") ,
                                            new DataColumn("Rol") });

            foreach (var item in data)
            {
                dt.Rows.Add(item.codusuario, item.firstname, item.lastname, item.email, item.reg_date, item.tipocuenta, item.espacio,item.rol);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte" + Session["nombre"] + "_" + DateTime.Now.ToShortDateString() + ".xlsx");
                }
            }
        }


        [HttpPost]
        public ActionResult UpdateRol(string rol, string usuario)
        {
            ViewBag.shared = usuario;
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("update usuario set rol = '{0}' where codusuario = '{1}';", rol, usuario);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            if (cmd.ExecuteNonQuery() == 1)
            {

                return View();
            }
            else
            {
                return View("Error");
            }

        }

        [HttpPost]
        public ActionResult UpdatePago(string test, string test2, string test3, string test4, string usuario)
        {
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("update pago set codigo = '{0}', pin = '{1}', fecha_vencimiento = '{2}', fecha_vencimiento_y = '{3}' where usuario = '{4}';", test,test2,test3,test4, usuario);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            if (cmd.ExecuteNonQuery() == 1)
            {

                return View();
            }
            else
            {
                return View("Error");
            }

        }

        [HttpPost]
        public ActionResult UpdateCuenta(string usuario, string cuenta)
        {
            string temp = string.Empty;
            if (cuenta == "Estandar")
            {
                temp = "15 GB";
            }
            else if (cuenta == "Avanzada")
            {
                temp = "50 GB";
            }
            else
            {
                temp = "100 GB";
            }
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("update usuario set tipocuenta = '{0}', espacio = '{1}' where codusuario = '{2}';", cuenta,temp, usuario);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            if (cmd.ExecuteNonQuery() == 1)
            {

                return View();
            }
            else
            {
                return View("Error");
            }

        }

        [HttpPost]
        public ActionResult UpdateInfo(string usuario, string nombre, string apellido, string email)
        {    
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("update usuario set firstname = '{0}', lastname = '{1}', email = '{2}' where codusuario = '{3}';", nombre, apellido, email, usuario);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            if (cmd.ExecuteNonQuery() == 1)
            {

                return View();
            }
            else
            {
                return View("Error");
            }

        }

        private void eliminaCompartidos(int id)
        {
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("delete from shared_files where id_file = {0};", id);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            cmd.ExecuteNonQuery();
           
        }
        [HttpPost]
        public ActionResult deleteFile(int id)
        {
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("delete from files where id_file = {0};", id);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            if (cmd.ExecuteNonQuery() == 1)
            {
                eliminaCompartidos(id);
                return View();
            }
            else
            {
                return View("Error");
            }

        }

        [HttpPost]
        public ActionResult deleteFileR(int id, string user)
        {
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("delete from shared_files where id_file = {0} and usuario = '{1}';", id, user);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            if (cmd.ExecuteNonQuery() == 1)
            {
                return View("deleteFile");
            }
            else
            {
                return View("Error");
            }

        }
        public void renameFileDirectory(string nombre,string nuevo)
        {
            string fileName = @"C:\Users\river\Downloads\Telegram Desktop\maintenanceIsertec\maintenanceIsertec\Files\";
            System.IO.File.Move(fileName + nombre, fileName + nuevo);
        }
        [HttpPost]
        public ActionResult renameFile(int id, string nombre, string temp)
        {
            string fileName = "~/Files/" + temp;
            FileInfo fi = new FileInfo(fileName);
            string extension = fi.Extension;
            nombre = nombre + extension;
            renameFileDirectory(temp, nombre);
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("update files set nombre = '{0}' where id_file = {1};", nombre, id);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            if (cmd.ExecuteNonQuery() == 1)
            {
                return View();
            }
            else
            {
                return View("Error");
            }

        }

        [HttpPost]
        public ActionResult editComents(int id, string nombre)
        {
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("update files set metadata = '{0}' where id_file = {1};", nombre, id);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            if (cmd.ExecuteNonQuery() == 1)
            {
                return View();
            }
            else
            {
                return View("Error");
            }

        }

        [HttpPost]
        public ActionResult createDirectorio(string usuario, string nombre)
        {
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("insert into folders values (null,'{0}','{1}','S',NOW());", nombre, usuario );
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            if (cmd.ExecuteNonQuery() == 1)
            {
                return RedirectToAction("folders","Documents");
            }
            else
            {
                return View("Error");
            }

        }

        [HttpPost]
        public ActionResult moveFile(int id, int carpeta)
        {
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("update files set id_folder = {0} where id_file = {1};",carpeta, id);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            cmd.ExecuteNonQuery();
            return View();

        }

    }
}