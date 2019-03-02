using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace JK.FMS.WindowService
{
    class Program
    {
        private static string SmtpHost => Convert.ToString(ConfigurationManager.AppSettings["emailSmtpHost"]);

        private static int SmtpPort => Convert.ToInt32(ConfigurationManager.AppSettings["emailSmtpPort"]);

        private static string SmtpUsername => Convert.ToString(ConfigurationManager.AppSettings["emailSmtpUsername"]);

        private static string SmtpPassword => Convert.ToString(ConfigurationManager.AppSettings["emailSmtpPassword"]);

        private static int SmtpClientTimeOut => Convert.ToInt32(ConfigurationManager.AppSettings["emailsmtpClientTimeOut"]);

        private static string IMapHost => Convert.ToString(ConfigurationManager.AppSettings["emailImapHost"]);

        private static int ImapPort => Convert.ToInt32(ConfigurationManager.AppSettings["emailImapPort"]);

        private static string EmailFromName => Convert.ToString(ConfigurationManager.AppSettings["emailFromName"]);

        private static string EmailFrom => Convert.ToString(ConfigurationManager.AppSettings["emailFrom"]);
        static void Main(string[] args)
        {
            Customer_SuspendedTOActive();

            PostPayment2GeneralLedgerTrx_Checkbook();

            User_LoffOff();
        }


        public static string Customer_SuspendedTOActive()
        {
            string returnId = string.Empty;

            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, "portal_sp_Customer_SuspendedTOActive"))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    return ds.Tables[0].Rows[0][0].ToString();
                }
            }

            return returnId;
        }

        public static string User_LoffOff()
        {
            string returnId = string.Empty;

            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, "Auth_LogOofUserTracking"))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    return ds.Tables[0].Rows[0][0].ToString();
                }
            }

            return returnId;
        }

        public static string PostPayment2GeneralLedgerTrx_Checkbook()
        {
           
            string returnId = string.Empty;
            SqlParameter[] parmList = {
            new SqlParameter("@PostDate",DateTime.Now.ToString("MM/dd/yyy")),
            new SqlParameter("@RegionId",0)
            };
            SendEmailAsync("pnguyen@janiking.com", "Checkbook Insert Start", "WS: Checkbook Insert Start");

            using (DataSet ds = SQLHelper.ExecuteDataset(SQLHelper.ConnectionStringTransaction, CommandType.StoredProcedure, "spCreate_AR_PaymentPostInCheckbookAndGL", parmList))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    return ds.Tables[0].Rows[0][0].ToString();
                }
            }
            SendEmailAsync("pnguyen@janiking.com", "Checkbook Insert Start", "WS: Checkbook Insert End");
            return returnId;
        }

        /// <summary>
        /// Send Mail to emailTo via async 
        /// </summary>
        /// <param name="emailTo">Mail Id to sender user</param>
        /// <param name="mailBody">Mail Boby according to Templete</param>
        /// <param name="subject">Mail Subject according to Templete</param>
        /// <param name="attachments">Mail attachments</param>
        /// <param name="cc">CC email</param>
        /// <returns>if mail send its returm true othewise return false</returns>
        public static bool SendEmailAsync(string emailTo, string mailBody, string subject, List<Attachment> attachments = null, string emailToDisplayName = "", string cc = "")
        {
            var success = true;

            try
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    // client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Timeout = SmtpClientTimeOut;
                    client.Connect(SmtpHost, SmtpPort, SecureSocketOptions.Auto);
                    if (client.Capabilities.HasFlag(SmtpCapabilities.Authentication))
                    {
                        // Remove oauth and use simple authentication
                        client.AuthenticationMechanisms.Remove("XOAUTH2");
                        client.Authenticate(SmtpUsername, SmtpPassword);
                    }

                    // Build the message
                    var message = new MailMessage { From = (new MailAddress(EmailFrom, EmailFromName)) };
                    foreach (var address in emailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (emailToDisplayName != "")
                            message.To.Add(new MailAddress(address, emailToDisplayName));
                        else
                            message.To.Add(address);
                    }

                    /* Added CC functionality - German Sosa 10/5/2018 */
                    foreach (var address in cc.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!string.IsNullOrEmpty(cc))
                            message.CC.Add(new MailAddress(cc));
                        else
                            message.CC.Add(address);
                    }
                    message.Subject = subject;
                    message.Body = mailBody;
                    message.IsBodyHtml = true;
                    if (attachments != null)
                    {
                        foreach (var attachment in attachments)
                        {
                            message.Attachments.Add(attachment);
                        }
                    }
                    message.DeliveryNotificationOptions = DeliveryNotificationOptions.Delay | DeliveryNotificationOptions.OnFailure | DeliveryNotificationOptions.OnSuccess;

                    // Send the message
                    //NLogger.Debug($"Try to send email to={emailTo} from={EmailFrom} smtpHost={SmtpHost} smtpPort={SmtpPort} timeOut={SmtpClientTimeOut}");
                    client.Send((MimeMessage)message);

                    //// Update the database with notification
                    //JkEntityModel.MailNotifications.Add(new MailNotification
                    //{
                    //    MailTo = emailTo,
                    //    MailFrom = EmailFrom,
                    //    MailCC = string.Empty,
                    //    MailBCC = string.Empty,
                    //    MailSubject = subject,
                    //    MailBody = mailBody,
                    //    FromModule = string.Empty,
                    //    IsSent = false,
                    //    CreatedBy = -1,
                    //    CreatedOn = DateTime.Now,
                    //    IsActive = true
                    //});
                    //JkEntityModel.SaveChanges();
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                //NLogger.Error($"Exception={ex.Message} detial={ex.InnerException?.Message}");
                success = false;
            }

            return success;
        }
    }
}
