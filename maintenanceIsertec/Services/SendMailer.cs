using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.IO;
using System.Security.Cryptography;

namespace maintenanceIsertec.Services
{
    public interface IsendMailer
    {
        void sendMail(string from, string to, string subject, string path, string customer, string workorder);
    }
    public class SendMailer : IsendMailer
    {
        // path from controller
        public void sendMail(string from, string to, string subject, string path, string customer, string workorder)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Personal Cloud", from));
            email.To.AddRange(address(to));
            //crea libro electronico de direcciones para la copia del correo
            email.Cc.AddRange(copyAddress("riveraambrocio.edgar@gmail.com"));
            email.Subject = subject;
            var mensaje = new BodyBuilder();
            string mytemplate = string.Empty;
            using (StreamReader reader = System.IO.File.OpenText("C:\\Template\\response.html"))
            {
                mytemplate = reader.ReadToEnd();
            }
            mytemplate = mytemplate.Replace("{customer}", customer);
            mytemplate = mytemplate.Replace("{ot}", workorder);
          
            mensaje.HtmlBody = mytemplate;
            //mensaje.Attachments.Add(path);
            email.Body = mensaje.ToMessageBody();
            //email.Headers.Add("Disposition-Notification-To", "ticket@isertec.com");
            var smtp = new SmtpClient();
            smtp.Connect(Mailer.Server, Mailer.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(Mailer.SenderEmail, Mailer.Password);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        //Salida de correo unicamente de OT de emergencia de Energia y Aplicaciones
        public void sendMailEmergency(string from, string to, string subject, string path, string customer, string workorder)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Mantenimientos Isertec", from));
            email.To.AddRange(address(to));
            //crea libro electronico de direcciones para la copia del correo
            email.Cc.AddRange(copyAddress("servicios@isertec.com,erodriguez@isertec.com"));
            email.Subject = subject;
            var mensaje = new BodyBuilder();
            string mytemplate = string.Empty;
            using (StreamReader reader = System.IO.File.OpenText("C:\\Template\\response.html"))
            {
                mytemplate = reader.ReadToEnd();
            }
            mytemplate = mytemplate.Replace("{customer}", customer);
            mytemplate = mytemplate.Replace("{ot}", workorder);
            mytemplate = mytemplate.Replace("{link-confirm}", getServerPath(workorder));
            mensaje.HtmlBody = mytemplate;
            mensaje.Attachments.Add(path);
            email.Body = mensaje.ToMessageBody();
            email.Headers.Add("Disposition-Notification-To", "ticket@isertec.com");
            var smtp = new SmtpClient();
            smtp.Connect(Mailer.Server, Mailer.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(Mailer.SenderEmail, Mailer.Password);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
        // Controlador de correo electronico de RA
        public void sendMailOtherUnit(string from, string to, string subject, string path, string customer, string workorder)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Mantenimientos Isertec", from));
            email.To.AddRange(address(to));
            //crea libro electronico de direcciones para la copia del correo 
            //email.Cc.AddRange(copyAddress("imejia@isertec.com,wmolina@isertec.com,ygamez@isertec.com,dcastillo@isertec.com,erodriguez@isertec.com"));
            email.Subject = subject;
            var mensaje = new BodyBuilder();
            string mytemplate = string.Empty;
            using (StreamReader reader = System.IO.File.OpenText("C:\\Template\\response.html"))
            {
                mytemplate = reader.ReadToEnd();
            }
            mytemplate = mytemplate.Replace("{customer}", customer);
            mytemplate = mytemplate.Replace("{ot}", workorder);
            mytemplate = mytemplate.Replace("{link-confirm}", getServerPath(workorder));
            mensaje.HtmlBody = mytemplate;
            mensaje.Attachments.Add(path);
            email.Body = mensaje.ToMessageBody();
            email.Headers.Add("Disposition-Notification-To", "ticket@isertec.com");
            var smtp = new SmtpClient();
            smtp.Connect(Mailer.Server, Mailer.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(Mailer.SenderEmail, Mailer.Password);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
        public string getServerPath(string regID)
        {
            string temp = "https://cmms.isertec.com/ServiceVerification/Confirm?serviceCall=" + Cypher.Encrypt(regID);
            temp = temp.Replace("+", "%2B");
            return temp;
        }
        public string encryptCypher(string workOrder)
        {
            byte[] encrypted;
            using (AesManaged aes = new AesManaged())
            {
                encrypted = Encrypt(workOrder, aes.Key, aes.IV);

            }
            return System.Text.Encoding.UTF8.GetString(encrypted);
        }

        static byte[] Encrypt(string workOrder, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            using (AesManaged aes = new AesManaged())
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(Key, IV);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter SW = new StreamWriter(cs))
                            SW.WriteLine(workOrder);
                        encrypted = ms.ToArray();
                    }
                }
            }
            return encrypted;
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

        public InternetAddressList address(string direcciones)
        {
            InternetAddressList temp = new InternetAddressList();
            string[] address_ = direcciones.Split(' ', ',');
            foreach (string aux in address_)
            {
                temp.Add(MailboxAddress.Parse(aux));
            }
            return temp;
        }

        public InternetAddressList copyAddress(string direcciones)
        {
            InternetAddressList temp = new InternetAddressList();
            string[] address_ = direcciones.Split(' ', ',');
            foreach (string aux in address_)
            {
                temp.Add(MailboxAddress.Parse(aux));
            }
            return temp;
        }

     
    }
}