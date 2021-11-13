using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using maintenanceIsertec.Connection.Consult;
using maintenanceIsertec.Models;

namespace maintenanceIsertec.Controllers
{
    /* Realizado por: Edgar Rivera 
     * Noviembre del 2020 
     * Ultimo Deployment - 05 de Junio del 2021*/
    [Authorize]
    public class ConsultController : Controller
    {
        // GET: Consult
        public ActionResult list(int? page, string findString)
        {
            
            return View();
        }
        [HttpGet]
        public ActionResult Arranque(int CallID, string Line, string DocNum)
        {
           
                return View("dataFound");
            
        }
        [HttpGet]
        public ActionResult UPSMonofásico(int CallID, string Line, string DocNum)
        {
            
                return View("dataFound");
            
        }
        [HttpGet]
        public ActionResult UPSTrifásico(int CallID, string Line, string DocNum)
        {
           
                return View("dataFound");
            
        }
        [HttpGet]
        public ActionResult MotoGenerador(int CallID, string Line, string DocNum)
        {
           
                return View("dataFound");
            
        }
        [HttpGet]
        public ActionResult ACAPCSCSeries(int CallID, string Line, string DocNum)
        {
           
                return View("dataFound");
            
        }
        [HttpGet]
        public ActionResult ACAPCRDRPSeries(int CallID, string Line, string DocNum)
        {
           
                return View("dataFound");
            
        }
        [HttpGet]
        public ActionResult ACUniflairAmico(int CallID, string Line, string DocNum)
        {
           
                return View("dataFound");
            
        }
        [HttpGet]
        public ActionResult ACComfort(int CallID, string Line, string DocNum)
        {
            
                return View("dataFound");
            
        }
        [HttpGet]
        public ActionResult AtencióndeEmergencia(int CallID, string Line, string DocNum)
        {
            
                return View("dataFound");
            
        }
        [HttpGet]
        public ActionResult AtenciónGeneral(int CallID, string Line, string DocNum)
        {
            
                return View("dataFound");
            
        }
        [HttpGet]
        public ActionResult AtencióndeEmergenciaRA(int CallID, string Line, string DocNum)
        {
           
                return View("dataFound");
            
        }
        [HttpGet]
        public ActionResult AtenciónGeneralRA(int CallID, string Line, string DocNum)
        {
           
                return View("dataFound");
            
        }
        [HttpGet]
        public ActionResult DetecciónySupresión(int CallID, string Line, string DocNum)
        {
           
                return View("dataFound");
            
        }
    }
}