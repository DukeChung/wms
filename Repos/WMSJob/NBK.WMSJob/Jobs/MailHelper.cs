using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Configuration;

namespace NBK.WMSJob
{
    public class MailHelper
    {
        public static string smtpServer = ConfigurationManager.AppSettings["SmtpServer"].ToString();
        public static string userName = ConfigurationManager.AppSettings["UserName"].ToString();
        public static string passWord = ConfigurationManager.AppSettings["PassWord"].ToString();
        public static string emailFrom = ConfigurationManager.AppSettings["EmailFrom"].ToString();

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="Subject">标题</param>
        /// <param name="Body">正文</param>
        /// <param name="attachments">附件</param>
        /// <returns></returns>
        public static string SendMail(string Subject, string Body, string mailTo, List<Attachment> attachments)
        {
            string temp = string.Empty;
            SmtpClient smtp = new SmtpClient();
            MailMessage mail = new MailMessage();

            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Host = smtpServer;
            smtp.Credentials = new System.Net.NetworkCredential(userName, passWord);  //发送邮件，用户名，密码
            mail.From = new MailAddress(emailFrom);

            string[] mails = mailTo.Split(';');
            for (int i = 0; i < mails.Length; i++)
            {
                if (!string.IsNullOrEmpty(mails[i]))
                {
                    mail.To.Add(mails[i]);
                }
            }

            //附件
            if (attachments != null && attachments.Count > 0)
            {
                foreach (var item in attachments)
                {
                    mail.Attachments.Add(item);
                }
            }

            mail.Subject = Subject;
            mail.Body = Body;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.Normal;

            try
            {
                smtp.Send(mail);
                temp = "Y";
            }
            catch (Exception ex)
            {
                temp = ex.Message;
            }

            if(mail.Attachments != null)
            {
                mail.Attachments.Dispose();
            }

            return temp;
        }
    }
}
