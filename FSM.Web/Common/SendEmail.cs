using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace FSM.Web.Common
{
    public class SendEmail
    {
        public void Send(string subject, string body, string fromMail, string emailTo)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(System.Configuration.ConfigurationManager.AppSettings["SmtpServer"]);
            mail.From = new MailAddress(fromMail);
            mail.To.Add(emailTo);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SmtpServer.Port = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]); 
            SmtpServer.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Username"], System.Configuration.ConfigurationManager.AppSettings["Password"]);
            SmtpServer.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"]);
            SmtpServer.Send(mail);
        }
    }
}