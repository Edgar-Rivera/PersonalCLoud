using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sap.Data.Hana;
using maintenanceIsertec.Connection;
using maintenanceIsertec.Models;
using PagedList;
using maintenanceIsertec.Connection.Schedulings;
using maintenanceIsertec.Connection.ServiceCalls;

namespace maintenanceIsertec.Controllers
{
    /* Realizado por: Edgar Rivera 
     * Noviembre del 2020 
     * Ultimo Deployment - 24 de Mayo del 2021*/
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            return View();
        }

        public PartialViewResult QuickAcces()
        {
           
            return PartialView();
        }
        public ActionResult ListItems(int? page)
        {
            List<ServiceCallDashboard> data = new ServiceCallsUser().getUserDashboard();
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(data.ToPagedList(pageNumber, pageSize));
        }


       
    }
}