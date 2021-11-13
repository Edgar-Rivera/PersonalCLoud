using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using Sap.Data.Hana;
using maintenanceIsertec.Connection.Consult;
using maintenanceIsertec.Models;
using maintenanceIsertec.Services;


namespace maintenanceIsertec.Controllers
 {/* Realizado por: Edgar Rivera 
         * Noviembre del 2020 
         * Ultimo Deployment - 24 de Mayo del 2021*/
    public class ServiceVerificationController : Controller
    {
        [HandleError]
        public ActionResult ErrorConfirm()
        {
            return View();
        }
        public string setParameterValues(string paramServiceCall)
        {
            if (paramServiceCall != null)
            {
                string[] deserealized = paramServiceCall.Split('-');
                return deserealized[0];
            }
            else
            {
                return "InternalError";
            }
        }
        public string setTicketValues(string paramServiceCall)
        {
            if (paramServiceCall != null)
            {
                string[] deserealized = paramServiceCall.Split('-');
                return deserealized[1];
            }
            else
            {
                return "InternalError";
            }
        }
        static string Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;
            using (AesManaged aes = new AesManaged())
            {
                ICryptoTransform decryptor = aes.CreateDecryptor(Key, IV);
                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cs))
                            plaintext = reader.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Confirm(string serviceCall)
        {
            
                return View();
            
        }
       
    }
}
