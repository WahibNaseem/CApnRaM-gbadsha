using System;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using JKApi.Data.DAL;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using Org.BouncyCastle.Utilities.Collections;

namespace JKApi.Core
{
    public class MailService
    {
        public jkDatabaseEntities JkEntityModel = jkDatabaseEntities.Instance;

        private static readonly Lazy<MailService> Lazy = new Lazy<MailService>(() => new MailService());
        public static MailService Instance => Lazy.Value;

        protected NLogger NLogger = NLogger.Instance;

        private static string SmtpHost => Convert.ToString(ConfigurationManager.AppSettings["emailSmtpHost"]);

        private static int SmtpPort => Convert.ToInt32(ConfigurationManager.AppSettings["emailSmtpPort"]);

        private static string SmtpUsername => Convert.ToString(ConfigurationManager.AppSettings["emailSmtpUsername"]);

        private static string SmtpPassword => Convert.ToString(ConfigurationManager.AppSettings["emailSmtpPassword"]);

        private static int SmtpClientTimeOut
            => Convert.ToInt32(ConfigurationManager.AppSettings["emailsmtpClientTimeOut"]);

        private static string IMapHost => Convert.ToString(ConfigurationManager.AppSettings["emailImapHost"]);

        private static int ImapPort => Convert.ToInt32(ConfigurationManager.AppSettings["emailImapPort"]);

        private static string EmailFromName => Convert.ToString(ConfigurationManager.AppSettings["emailFromName"]);

        private static string EmailFrom => Convert.ToString(ConfigurationManager.AppSettings["emailFrom"]);


        /// <summary>
        /// Send Mail to emailTo via async 
        /// </summary>
        /// <param name="emailTo">Mail Id to sender user</param>
        /// <param name="mailBody">Mail Boby according to Templete</param>
        /// <param name="subject">Mail Subject according to Templete</param>
        /// <param name="attachments">Mail attachments</param>
        /// <param name="cc">CC email</param>
        /// <returns>if mail send its returm true othewise return false</returns>
        public bool SendEmailAsync(string emailTo, string mailBody, string subject, List<Attachment> attachments = null, string emailToDisplayName = "", string cc = "")
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
                    NLogger.Debug($"Try to send email to={emailTo} from={EmailFrom} smtpHost={SmtpHost} smtpPort={SmtpPort} timeOut={SmtpClientTimeOut}");
                    client.Send((MimeMessage)message);

                    // Update the database with notification
                    JkEntityModel.MailNotifications.Add(new MailNotification
                    {
                        MailTo = emailTo,
                        MailFrom = EmailFrom,
                        MailCC = string.Empty,
                        MailBCC = string.Empty,
                        MailSubject = subject,
                        MailBody = mailBody,
                        FromModule = string.Empty,
                        IsSent = false,
                        CreatedBy = -1,
                        CreatedOn = DateTime.Now,
                        IsActive = true
                    });
                    JkEntityModel.SaveChanges();
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                NLogger.Error($"Exception={ex.Message} detial={ex.InnerException?.Message}");
                success = false;
            }

            return success;
        }

        public bool SendEmailAsyncWithFrom(string emailFrom, string emailTo, string mailBody, string subject, List<Attachment> attachments = null)
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
                    var message = new MailMessage { From = (new MailAddress(emailFrom, emailFrom)) };
                    foreach (var address in emailTo.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        message.To.Add(address);
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
                    NLogger.Debug($"Try to send email to={emailTo} from={emailFrom} smtpHost={SmtpHost} smtpPort={SmtpPort} timeOut={SmtpClientTimeOut}");
                    client.Send((MimeMessage)message);

                    // Update the database with notification
                    JkEntityModel.MailNotifications.Add(new MailNotification
                    {
                        MailTo = emailTo,
                        MailFrom = emailFrom,
                        MailCC = string.Empty,
                        MailBCC = string.Empty,
                        MailSubject = subject,
                        MailBody = mailBody,
                        FromModule = string.Empty,
                        IsSent = false,
                        CreatedBy = -1,
                        CreatedOn = DateTime.Now,
                        IsActive = true
                    });
                    JkEntityModel.SaveChanges();
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                NLogger.Error($"Exception={ex.Message} detial={ex.InnerException?.Message}");
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Create Key Pair value for Mail Subject and Body
        /// </summary>
        /// <param name="html">HTML value</param>
        /// <param name="keyValuePair">Replace key value in HTML</param>
        /// <returns>Formatted HTML Value</returns>
        public string SetMailValues(string html, List<KeyValuePair<string, string>> keyValuePair)
        {
            if (keyValuePair != null)
                for (int i = 0; i < keyValuePair.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(keyValuePair[i].Key) &&
                        !string.IsNullOrWhiteSpace(keyValuePair[i].Value))
                        html = html.Replace("<<" + keyValuePair[i].Key.ToLower() + ">>", keyValuePair[i].Value);
                }
            return html;
        }

        public List<MimeMessage> GetNewEmail(int userId)
        {
            var newUnreadEmail = new List<MimeMessage>();

            var user = JkEntityModel.AuthUserLogins.FirstOrDefault(x => x.UserId == userId);
            var imapUsername = user?.OutlookUsername;
            var imapPassword = user?.OutlookPassword;

            if (imapUsername != null && imapPassword != null)
            {
                try
                {
                    using (var client = new ImapClient())
                    {
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        client.Connect(IMapHost, ImapPort);
                        client.AuthenticationMechanisms.Remove("XOAUTH2");
                        client.Authenticate(imapUsername, imapPassword);
                        var inbox = client.Inbox;
                        inbox.Open(FolderAccess.ReadOnly);
                        var query = SearchQuery.NotSeen;
                        if (client.Capabilities.HasFlag(ImapCapabilities.Sort))
                        {
                            var orderBy = new[] { OrderBy.ReverseArrival };
                            foreach (var uid in inbox.Sort(query, orderBy))
                            {
                                var message = inbox.GetMessage(uid);
                                newUnreadEmail.Add(message);
                            }
                        }
                        else
                        {
                            var reversed = new UniqueIdSet(SortOrder.Descending);
                            foreach (var uid in inbox.Search(query))
                            {
                                reversed.Add(uid);
                            }
                            foreach (var uid in reversed)
                            {
                                var message = inbox.GetMessage(uid);
                                newUnreadEmail.Add(message);
                            }
                        }

                        client.Disconnect(true);
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Instance.Error($"Exception when initializing the mail service: @{ex.Message}");
                }
            }

            return newUnreadEmail;
        }
    }

    public static class AsyncHelpers
    {
        /// <summary>
        /// Execute's an async Task<T> method which has a void return value synchronously
        /// </summary>
        /// <param name="task">Task<T> method to execute</param>
        public static void RunSync(Func<Task> task)
        {
            var oldContext = SynchronizationContext.Current;
            var synch = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synch);
            synch.Post(async _ =>
            {
                try
                {
                    await task();
                }
                catch (Exception e)
                {
                    synch.InnerException = e;
                }
                finally
                {
                    synch.EndMessageLoop();
                }
            }, null);
            synch.BeginMessageLoop();

            SynchronizationContext.SetSynchronizationContext(oldContext);
        }

        /// <summary>
        /// Execute's an async Task<T> method which has a T return type synchronously
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="task">Task<T> method to execute</param>
        /// <returns></returns>
        public static T RunSync<T>(Func<Task<T>> task)
        {
            var oldContext = SynchronizationContext.Current;
            var synch = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synch);
            T ret = default(T);
            synch.Post(async _ =>
            {
                try
                {
                    ret = await task();
                }
                catch (Exception e)
                {
                    synch.InnerException = e;
                }
                finally
                {
                    synch.EndMessageLoop();
                }
            }, null);
            synch.BeginMessageLoop();
            SynchronizationContext.SetSynchronizationContext(oldContext);
            return ret;
        }

        private class ExclusiveSynchronizationContext : SynchronizationContext
        {
            private bool done;
            public Exception InnerException { get; set; }
            readonly AutoResetEvent workItemsWaiting = new AutoResetEvent(false);

            readonly Queue<Tuple<SendOrPostCallback, object>> items =
                new Queue<Tuple<SendOrPostCallback, object>>();

            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("We cannot send to our same thread");
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                lock (items)
                {
                    items.Enqueue(Tuple.Create(d, state));
                }
                workItemsWaiting.Set();
            }

            public void EndMessageLoop()
            {
                Post(_ => done = true, null);
            }

            public void BeginMessageLoop()
            {
                while (!done)
                {
                    Tuple<SendOrPostCallback, object> task = null;
                    lock (items)
                    {
                        if (items.Count > 0)
                        {
                            task = items.Dequeue();
                        }
                    }
                    if (task != null)
                    {
                        task.Item1(task.Item2);
                        if (InnerException != null) // the method threw an exception
                        {
                            //throw new AggregateException("AsyncHelpers.Run method threw an exception.", InnerException);
                        }
                    }
                    else
                    {
                        workItemsWaiting.WaitOne();
                    }
                }
            }

            public override SynchronizationContext CreateCopy()
            {
                return this;
            }
        }
    }
}