using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.Utility
{
    public class EmailHelper
    {
        private static string MailFromAddress = ConfigurationManager.AppSettings["MailFromAddress"];
        private static string MailFromDisplayName = ConfigurationManager.AppSettings["MailFromDisplayName"];
        private static string MailHost = ConfigurationManager.AppSettings["MailHost"];
        private static string MailPort = ConfigurationManager.AppSettings["MailPort"];
        private static string MailUserName = ConfigurationManager.AppSettings["MailUserName"];
        private static string MailPassword = ConfigurationManager.AppSettings["MailPassword"];

        public static void SendMailAsync(string mailSubject, string mailBody, string mailTo)
        {
            new Task(() => {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(MailFromAddress, MailFromDisplayName),
                    Subject = mailSubject,
                    BodyEncoding = Encoding.Default,
                    Priority = MailPriority.High,
                    Body = mailBody,
                    IsBodyHtml = false
                };
                mailTo.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(r => mailMessage.To.Add(r));
                var smtpClient = new SmtpClient
                {
                    Host = MailHost,
                    Port = Convert.ToInt32(MailPort),
                    UseDefaultCredentials = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(MailUserName, MailPassword),
                    EnableSsl = true,
                    Timeout = 10000
                };
                smtpClient.Send(mailMessage);
            }).Start();
        }
    }
}
