using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net.Mime;
using Core.Logger;

namespace Core.Net
{
    public class MailClient
    {
        public bool SendMail(MailMessage mail)
        {
            bool b = false;
            try
            {
                System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage();//mail.From, mail.To, mail.Subject, mail.Body);//(mail.From, mail.To, mail.Subject, mail.Body);
                m.From = new MailAddress(mail.From);
                m.Subject = mail.Subject;
                m.Body = mail.Body;
                
                m.IsBodyHtml = true;

                if (!String.IsNullOrEmpty(mail.To))
                {
                    string[] ar = mail.To.Split(',');
                    foreach (string address in ar)
                        m.To.Add(address);
                }

                if (!String.IsNullOrEmpty(mail.CC))
                {
                    string[] ar = mail.CC.Split(',');
                    foreach (string address in ar)
                        m.CC.Add(address);
                }

                if (!String.IsNullOrEmpty(mail.BCC))
                {
                    string[] ar = mail.BCC.Split(',');
                    foreach (string address in ar)
                        m.Bcc.Add(address);
                }

                if (!String.IsNullOrEmpty(mail.Attachment))
                {
                    string[] ar = mail.Attachment.Split(',');
                    foreach (string file in ar)
                    {
                        Attachment item = new Attachment(file);
                        m.Attachments.Add(item);
                    }
                }

                System.Net.Mail.SmtpClient emailClient = new System.Net.Mail.SmtpClient(mail.SmtpClient);
                System.Net.NetworkCredential SMTPUserInfo = new System.Net.NetworkCredential(mail.Username, mail.Password);
                emailClient.UseDefaultCredentials = true;
                emailClient.Credentials = SMTPUserInfo;
                //emailClient.Port = 587;
                emailClient.Send(m);
                b = true;
            }
            catch (Exception e1)
            {
                Logger.LoggerFactory.CreateLogger().Log("Error::Send Mail::" + e1.ToString(),LogType.Error);
                b = false;
            }
            return b;
             
        }
    }

    public class MailMessage
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SmtpClient { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string Attachment { get; set; }
    }
}