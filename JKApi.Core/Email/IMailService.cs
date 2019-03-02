using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using MimeKit;

namespace JKApi.Core.Email
{
    public interface IMailService
    {
        Task<bool> SendEmailAsync(string emailTo, string mailbody, string subject, List<Attachment> attachment = null);
        
        string SetMailValues(string html, List<KeyValuePair<string, string>> keyValuePair);

        List<MimeMessage> GetNewEmail();
    }
}
