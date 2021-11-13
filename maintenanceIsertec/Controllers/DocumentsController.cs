using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Odbc;
using PagedList;
using System.Data;
using ClosedXML.Excel;
using System.Web.Services;
using maintenanceIsertec.Services;

namespace maintenanceIsertec.Controllers
{
    [Authorize]
    public class DocumentsController : Controller
    {

        public string getRootFolder()
        {
            string idResult = string.Empty;
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("select id from folders where propietario = '{0}' and nombre = 'root';", Session["user"]);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            OdbcDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                idResult = ""+reader.GetInt32(0);
            }
            cnx.Close();
            return idResult;
        }
        // GET: Documents
        [HttpGet]
        public ActionResult list(int? page, string findString)
        {
            string root = getRootFolder();
            ViewBag.id = root;
            // controlador de lista y subida de archivos
            var data = new List<maintenanceIsertec.Models.Files>();
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("select * from files where username = '{0}' and id_folder = {1};", Session["user"], root);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            OdbcDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                data.Add(new Models.Files()
                {
                    id_file = reader.GetInt32(0),
                    nombre = reader.GetString(1),
                    tipo = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    peso = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    propietario = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    compartido = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    metadata = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    username = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                    fecha_creacion = reader.GetDateTime(8),
                });
            }
            cnx.Close();
            ViewBag.findString = findString;
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            if (!String.IsNullOrEmpty(findString))
            {
                var obj = data.Where(s => s.nombre.IndexOf(findString, StringComparison.OrdinalIgnoreCase) >= 0 || s.tipo.IndexOf(findString, StringComparison.OrdinalIgnoreCase) >= 0 || s.metadata.IndexOf(findString, StringComparison.OrdinalIgnoreCase) >= 0 || s.peso.IndexOf(findString, StringComparison.OrdinalIgnoreCase) >= 0 || s.propietario.IndexOf(findString, StringComparison.OrdinalIgnoreCase) >= 0);
                return View(obj.ToPagedList(pageNumber, pageSize));
            }
            return View(data.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult folders()
        {
            // controlador de lista y subida de archivos
            var data = new List<maintenanceIsertec.Models.folders>();
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("select * from folders where propietario = '{0}';", Session["user"]);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            OdbcDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                data.Add(new Models.folders()
                {
                    id = reader.GetInt32(0),
                    nombre = reader.GetString(1),
                    propietario = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    compartido = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    regdate = reader.GetDateTime(4),
                });
            }
            cnx.Close();

            return View(data);
        }

        [HttpPost]
        public ActionResult UploadFilesF(HttpPostedFileBase postedFiled, string comments, string id, string nombre)
        {
            string filePath = "";
            filePath = Server.MapPath("~/Files/");
            if (postedFiled == null)
            {
                postedFiled = Request.Files["userFile"];
            }
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            filePath = filePath + Path.GetFileName(postedFiled.FileName);
            postedFiled.SaveAs(filePath);
            string exttension = System.IO.Path.GetExtension(postedFiled.FileName);
            ViewBag.Message = "Archivo Cargado!";
            // codigo para ingreso a base de datos
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("INSERT INTO FILES VALUES (0,'{0}','{1}','{2}','{3}', 'N','{4}','{5}',now(),{6});", postedFiled.FileName, exttension, postedFiled.ContentLength,
                Session["nombre"] + " " + Session["apellido"], comments, Session["user"], id);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            if (cmd.ExecuteNonQuery() == 1)
            {
            }
            return RedirectToAction("folderDetails", "Documents", new { id = id , nombre = nombre});
        }

        public ActionResult UploadFiles(HttpPostedFileBase postedFiled, string comments, string id)
        {
            string filePath = "";
            filePath = Server.MapPath("~/Files/");
            if (postedFiled == null)
            {
                postedFiled = Request.Files["userFile"];
            }
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            filePath = filePath + Path.GetFileName(postedFiled.FileName);
            postedFiled.SaveAs(filePath);
            string exttension = System.IO.Path.GetExtension(postedFiled.FileName);
            ViewBag.Message = "Archivo Cargado!";
            // codigo para ingreso a base de datos
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("INSERT INTO FILES VALUES (0,'{0}','{1}','{2}','{3}', 'N','{4}','{5}',now(),{6});", postedFiled.FileName, exttension, postedFiled.ContentLength,
                Session["nombre"] + " " + Session["apellido"], comments, Session["user"], id);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            if (cmd.ExecuteNonQuery() == 1)
            {
            }
            return RedirectToAction("list", "Documents");
        }
        public FileResult ExportExcel(string findString)
        {
            var data = new List<maintenanceIsertec.Models.Files>();
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("select * from files;");
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            OdbcDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                data.Add(new Models.Files()
                {
                    id_file = reader.GetInt32(0),
                    nombre = reader.GetString(1),
                    tipo = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    peso = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    propietario = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    compartido = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    fecha_creacion = reader.GetDateTime(8),
                });
            }
            cnx.Close();
            DataTable dt = new DataTable("Mis Archivos");
            dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Nombre Archivo"),
                                            new DataColumn("Tipo"),
                                            new DataColumn("Tamaño (KB)"),
                                            new DataColumn("Propietario"),
                                            new DataColumn("Compartido") ,
                                            new DataColumn("Fecha Creacion") });

            foreach (var item in data)
            {
                dt.Rows.Add(item.nombre, item.tipo, item.peso, item.propietario, item.compartido, item.fecha_creacion);
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

        public FileResult ExportExcelShare(string findString)
        {
            var data = new List<maintenanceIsertec.Models.shared_files>();
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("select * from SHARED_FILES_DETAILS where usuario = '{0}';", Session["user"]);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            OdbcDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                data.Add(new Models.shared_files()
                {
                    id = reader.GetInt32(0),
                    id_file = reader.GetInt32(1),
                    nombre = reader.GetString(2),
                    detalles = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    propietario = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    reg_date = reader.GetDateTime(5),
                    usuario = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                });
            }
            cnx.Close();
            DataTable dt = new DataTable("Mis Archivos Compartidos");
            dt.Columns.AddRange(new DataColumn[5] { new DataColumn("Nombre Archivo"),
                                            new DataColumn("Detalles"),
                                            new DataColumn("Propietario"),
                                            new DataColumn("Fecha Usuario") ,
                                            new DataColumn("Compartido") });

            foreach (var item in data)
            {
                dt.Rows.Add(item.nombre, item.detalles, item.propietario, item.reg_date, item.usuario);
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
        [HttpGet]
        public ActionResult details(string id)
        {
            var data = new List<maintenanceIsertec.Models.Files>();
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("select * from files where id_file = {0};", id);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            OdbcDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                data.Add(new Models.Files()
                {
                    id_file = reader.GetInt32(0),
                    nombre = reader.GetString(1),
                    tipo = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    peso = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    propietario = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    compartido = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    metadata = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    username = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                    fecha_creacion = reader.GetDateTime(8),
                });
            }
            cnx.Close();
            return View(data);
        }

        public FileResult Download(string name)
        {
            string filePath = Server.MapPath("~/Files/");
            filePath += name;
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string fileName = name;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpPost]
        public ActionResult SharedFile(int id, string propietario, string usuario, string detalles)
        {
            ViewBag.shared = usuario;
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("INSERT INTO SHARED_FILES VALUES (null,{0},'{1}','{2}','{3}',now());", id, propietario, usuario, detalles);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            if (cmd.ExecuteNonQuery() == 1)
            {

                updateStatus(id);
                SendMailer message = new SendMailer();
                message.sendMail("eriveraa1@miumg.edu.gt", emailCompartido(usuario), "Archivo Compartido", "", nameCompartido(usuario), (string)Session["nombre"] + " " + (string)Session["apellido"]);
                return View();
            }
            else
            {
                return View("Error");
            }

        }
        public ActionResult shared(int? page, string findString)
        {
            // controlador de lista y subida de archivos
            var data = new List<maintenanceIsertec.Models.shared_files>();
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("select * from SHARED_FILES_DETAILS where usuario = '{0}';", Session["user"]);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            OdbcDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                data.Add(new Models.shared_files()
                {
                    id = reader.GetInt32(0),
                    id_file = reader.GetInt32(1),
                    nombre = reader.GetString(2),
                    detalles = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    propietario = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    reg_date = reader.GetDateTime(5),
                    usuario = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                });
            }
            cnx.Close();
            ViewBag.findString = findString;
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            if (!String.IsNullOrEmpty(findString))
            {
                var obj = data.Where(s => s.nombre.IndexOf(findString, StringComparison.OrdinalIgnoreCase) >= 0 || s.detalles.IndexOf(findString, StringComparison.OrdinalIgnoreCase) >= 0 || s.usuario.IndexOf(findString, StringComparison.OrdinalIgnoreCase) >= 0 || s.detalles.IndexOf(findString, StringComparison.OrdinalIgnoreCase) >= 0 || s.propietario.IndexOf(findString, StringComparison.OrdinalIgnoreCase) >= 0);
                return View(obj.ToPagedList(pageNumber, pageSize));
            }
            return View(data.ToPagedList(pageNumber, pageSize));
        }
        public void updateStatus(int id)
        {
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("UPDATE FILES SET compartido = 'S' WHERE id_file = {0} ;", id);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            if (cmd.ExecuteNonQuery() == 1)
            {

            }
            cnx.Close();
        }

        [HttpPost]
        public ActionResult SaveCapturedImage(string data, string comments, string id, string nombre)
        {
            string temp = comments;
            string fileName = DateTime.Now.ToString("dd-MM-yy hh-mm-ss");
            //Convert Base64 Encoded string to Byte Array.
            byte[] imageBytes = Convert.FromBase64String(data.Split(',')[1]);
            //Save the Byte Array as Image File.
            string filePath = Server.MapPath(string.Format("~/Files/{0}.jpg", fileName));
            System.IO.File.WriteAllBytes(filePath, imageBytes);
            // return true;
            ViewBag.Message = "Archivo Cargado!";
            // codigo para ingreso a base de datos
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("INSERT INTO FILES VALUES (0,'{0}','{1}','{2}','{3}', 'N','{4}','{5}',now(), {6});", fileName + ".jpg", "jpg", "335",
                Session["nombre"] + " " + Session["apellido"], comments, Session["user"], id);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            if (cmd.ExecuteNonQuery() == 1)
            {
            }
            return RedirectToAction("fileDetails","Documents",new { id = id, nombre = nombre});
        }

        [HttpPost]
        public ActionResult SaveCapturedImageR(string data, string comments, string id)
        {
            string temp = comments;
            string fileName = DateTime.Now.ToString("dd-MM-yy hh-mm-ss");
            //Convert Base64 Encoded string to Byte Array.
            byte[] imageBytes = Convert.FromBase64String(data.Split(',')[1]);
            //Save the Byte Array as Image File.
            string filePath = Server.MapPath(string.Format("~/Files/{0}.jpg", fileName));
            System.IO.File.WriteAllBytes(filePath, imageBytes);
            // return true;
            ViewBag.Message = "Archivo Cargado!";
            // codigo para ingreso a base de datos
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("INSERT INTO FILES VALUES (0,'{0}','{1}','{2}','{3}', 'N','{4}','{5}',now(), {6});", fileName + ".jpg", "jpg", "335",
                Session["nombre"] + " " + Session["apellido"], comments, Session["user"], id);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            if (cmd.ExecuteNonQuery() == 1)
            {
            }
            return RedirectToAction("list", "Documents");
        }

        public string getSize()
        {
            string temp = string.Empty;
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("select sum(peso) from files where username = '{0}';", Session["user"]);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            OdbcDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                temp = reader.GetString(0);
            }
            cnx.Close();
            return temp;
        }


        [HttpGet]
        public ActionResult myprofile()
        {
            string id = (string)Session["user"];
            ViewBag.peso = getSize();
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
        public string nameCompartido(string user)
        {
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("select concat(firstname, \" \", lastname) \"name\" from usuario where codusuario = '{0}';", user);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            OdbcDataReader reader = cmd.ExecuteReader();
            string name = "";
            while (reader.Read())
            {
                name = reader.GetString(0);
            }
            cnx.Close();
            return name;
        }
        public string emailCompartido(string user)
        {
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("select email from usuario where codusuario = '{0}';", user);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            OdbcDataReader reader = cmd.ExecuteReader();
            string name = "";
            while (reader.Read())
            {
                name = reader.GetString(0);
            }
            cnx.Close();
            return name;
        }

        public ActionResult folderDetails(int? page, string findString, int id, string nombre)
        {
            // controlador de lista y subida de archivos
            var data = new List<maintenanceIsertec.Models.Files>();
            OdbcConnection cnx = new OdbcConnection("Dsn=PERSONALCLOUD");
            cnx.Open();
            string sql = string.Format("select * from files where username = '{0}' and id_folder = {1};", Session["user"], id);
            OdbcCommand cmd = new OdbcCommand(sql, cnx);
            OdbcDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                data.Add(new Models.Files()
                {
                    id_file = reader.GetInt32(0),
                    nombre = reader.GetString(1),
                    tipo = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    peso = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    propietario = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    compartido = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    metadata = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    username = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                    fecha_creacion = reader.GetDateTime(8),
                });
            }
            cnx.Close();
            ViewBag.findString = findString;
            ViewBag.id = id;
            ViewBag.nombre = nombre;
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            if (!String.IsNullOrEmpty(findString))
            {
                var obj = data.Where(s => s.nombre.IndexOf(findString, StringComparison.OrdinalIgnoreCase) >= 0 || s.tipo.IndexOf(findString, StringComparison.OrdinalIgnoreCase) >= 0 || s.metadata.IndexOf(findString, StringComparison.OrdinalIgnoreCase) >= 0 || s.peso.IndexOf(findString, StringComparison.OrdinalIgnoreCase) >= 0 || s.propietario.IndexOf(findString, StringComparison.OrdinalIgnoreCase) >= 0);
                return View(obj.ToPagedList(pageNumber, pageSize));
            }
            return View(data.ToPagedList(pageNumber, pageSize));
        }
    }
}