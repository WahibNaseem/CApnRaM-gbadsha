using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Net
{
    public class HttpPostRequest
    {
        public string URL { get; set; }
        public string Body { get; set; }
        public string ContentType { get; set; }
        public string CertificatePath { get; set; }
        public string AcceptHeader { get; set; }
        public WebHeaderCollection Headers { get; set; }
        public string CertificatePassword { get; set; }
        public int TimeOut { get; set; }
    }
}
