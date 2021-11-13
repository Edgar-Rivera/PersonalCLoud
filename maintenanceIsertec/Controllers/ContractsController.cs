using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using maintenanceIsertec.Models;
using maintenanceIsertec.Connection.Contracts;
using PagedList;

namespace maintenanceIsertec.Controllers
{
    /* Realizado por: Edgar Rivera 
     * Noviembre del 2020 
     * Ultimo Deployment - 24 de Mayo del 2021*/
    [Authorize]
    public class ContractsController : Controller
    {
        // GET: Contracts
        public ActionResult list(int? page, string findString)
        {
           
            return View();
        }
        public ActionResult details(int id)
        {
           
            return View();
        }
    }
}