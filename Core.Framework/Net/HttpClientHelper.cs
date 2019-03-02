using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Core.Logger;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace Core.Net
{
    public class HttpClientHelper
    {
        public HttpClientResponse HitURL(string url)
        {
            Stopwatch stopwatch = new Stopwatch();
            HttpClientResponse clientResponse = new HttpClientResponse();
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";

            HttpWebResponse httpResponse = null;
            try
            {
                stopwatch.Start();
                WebResponse response = request.GetResponse();
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                Stream dataStream = response.GetResponseStream();
                httpResponse = (HttpWebResponse)response;
                StreamReader reader = new StreamReader(dataStream);
                clientResponse.HttpStatusCode = ((int)httpResponse.StatusCode).ToString();
                clientResponse.ClientResponse = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();
            }
            catch (WebException exception)
            {
                LoggerFactory.CreateLogger().Log("Error:: Hitting URL in Core.HttpClientHelper.HitURL ::" + url + Environment.NewLine + "Response :: " + exception.ToString(), LogType.Error);
                clientResponse.ClientResponse = exception.Message;
                if (exception.Status == WebExceptionStatus.ProtocolError)
                {
                    httpResponse = (HttpWebResponse)exception.Response;
                    clientResponse.HttpStatusCode = ((int)httpResponse.StatusCode).ToString();
                }
                else
                {
                    LoggerFactory.CreateLogger().Log("Error:: Hitting URL in Core.HttpClientHelper.HitURL :: its not ProtocolError. So, error will be thrown :: " + url + Environment.NewLine + "Response :: " + exception.ToString(), LogType.Error);
                    throw;
                }
            }
            catch (Exception ex)
            {
                LoggerFactory.CreateLogger().Log("Error:: Hitting URL in Core.HttpClientHelper.HitURL ::" + url, LogType.Error);
            }

            stopwatch.Stop();
            clientResponse.ProcessingTimeMilliseconds = stopwatch.ElapsedMilliseconds;
            return clientResponse;
        }
        public HttpClientResponse HitURLWithHeader(string url,string username,string password)
        {
            Stopwatch stopwatch = new Stopwatch();
            HttpClientResponse clientResponse = new HttpClientResponse();
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";

            HttpWebResponse httpResponse = null;
            try
            {
                request.Headers.Add("username", username);
                request.Headers.Add("password", password);

                stopwatch.Start();
                WebResponse response = request.GetResponse();
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                Stream dataStream = response.GetResponseStream();
                httpResponse = (HttpWebResponse)response;
                StreamReader reader = new StreamReader(dataStream);
                clientResponse.HttpStatusCode = ((int)httpResponse.StatusCode).ToString();
                clientResponse.ClientResponse = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();
            }
            catch (WebException exception)
            {
                LoggerFactory.CreateLogger().Log("Error:: Hitting URL in Core.HttpClientHelper.HitURL ::" + url + Environment.NewLine + "Response :: " + exception.ToString(), LogType.Error);
                clientResponse.ClientResponse = exception.Message;
                if (exception.Status == WebExceptionStatus.ProtocolError)
                {
                    httpResponse = (HttpWebResponse)exception.Response;
                    clientResponse.HttpStatusCode = ((int)httpResponse.StatusCode).ToString();
                }
                else
                {
                    LoggerFactory.CreateLogger().Log("Error:: Hitting URL in Core.HttpClientHelper.HitURL :: its not ProtocolError. So, error will be thrown :: " + url + Environment.NewLine + "Response :: " + exception.ToString(), LogType.Error);
                    throw;
                }
            }
            catch (Exception ex)
            {
                LoggerFactory.CreateLogger().Log("Error:: Hitting URL in Core.HttpClientHelper.HitURL ::" + url, LogType.Error);
            }

            stopwatch.Stop();
            clientResponse.ProcessingTimeMilliseconds = stopwatch.ElapsedMilliseconds;
            return clientResponse;
        }
        public HttpClientResponse HitURL(string url, HttpPostRequest postRequest = null)
        {
            Stopwatch stopwatch = new Stopwatch();
            HttpClientResponse clientResponse = new HttpClientResponse();
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";

            HttpWebResponse httpResponse = null;
            try
            {
                if (postRequest != null)
                {
                    if (postRequest.Headers != null)
                    {
                        foreach (string key in postRequest.Headers.AllKeys)
                        {
                            request.Headers.Add(key, postRequest.Headers.Get(key));
                        }
                    }
                }
                
                stopwatch.Start();
                WebResponse response = request.GetResponse();
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                Stream dataStream = response.GetResponseStream();
                httpResponse = (HttpWebResponse)response;
                StreamReader reader = new StreamReader(dataStream);
                clientResponse.HttpStatusCode = ((int)httpResponse.StatusCode).ToString();
                clientResponse.ClientResponse = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();
            }
            catch (WebException exception)
            {
                LoggerFactory.CreateLogger().Log("Error:: Hitting URL in Core.HttpClientHelper.HitURL ::" + url + Environment.NewLine + "Response :: " + exception.ToString(), LogType.Error);
                clientResponse.ClientResponse = exception.Message;
                if (exception.Status == WebExceptionStatus.ProtocolError)
                {
                    httpResponse = (HttpWebResponse)exception.Response;
                    clientResponse.HttpStatusCode = ((int)httpResponse.StatusCode).ToString();
                }
                else
                {
                    LoggerFactory.CreateLogger().Log("Error:: Hitting URL in Core.HttpClientHelper.HitURL :: its not ProtocolError. So, error will be thrown :: " + url + Environment.NewLine + "Response :: " + exception.ToString(), LogType.Error);
                    throw;
                }
            }
            catch (Exception ex)
            {
                LoggerFactory.CreateLogger().Log("Error:: Hitting URL in Core.HttpClientHelper.HitURL ::" + url, LogType.Error);
            }

            stopwatch.Stop();
            clientResponse.ProcessingTimeMilliseconds = stopwatch.ElapsedMilliseconds;
            return clientResponse;
        }
       
        public HttpClientResponse PostData(HttpPostRequest postRequest)
        {
            Stopwatch stopwatch = new Stopwatch();
            HttpClientResponse clientResponse = new HttpClientResponse();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postRequest.URL);
            request.Method = "POST";
            request.ContentType = postRequest.ContentType;

            /******************************************************************************/
            if (postRequest.TimeOut != null && postRequest.TimeOut>0)
                 request.Timeout = postRequest.TimeOut;
            /*****************************************************************************/


            HttpWebResponse httpResponse = null;
            try
            {
                foreach (string key in postRequest.Headers.AllKeys)
                {
                    request.Headers.Add(key, postRequest.Headers.Get(key));
                }

                if (!String.IsNullOrEmpty(postRequest.AcceptHeader))
                {
                     request.Accept = postRequest.AcceptHeader;
                }

                if (!String.IsNullOrEmpty(postRequest.CertificatePath))
                {
                    //X509Certificate Cert = X509Certificate.CreateFromCertFile(postRequest.CertificatePath);

                    System.Security.Cryptography.X509Certificates.X509Certificate Cert = null;

                    if(!String.IsNullOrEmpty(postRequest.CertificatePassword))
                        Cert = new X509Certificate(postRequest.CertificatePath, postRequest.CertificatePassword);
                    else
                        Cert = new X509Certificate(postRequest.CertificatePath);

                    ServicePointManager.CertificatePolicy = new CertPolicy();
                    request.ClientCertificates.Add(Cert);
                }

                stopwatch.Start();
                byte[] byteArray = Encoding.UTF8.GetBytes(postRequest.Body);
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse response = request.GetResponse();
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                dataStream = response.GetResponseStream();
                httpResponse = (HttpWebResponse)response;
                StreamReader reader = new StreamReader(dataStream);
                clientResponse.HttpStatusCode = ((int)httpResponse.StatusCode).ToString();
                clientResponse.ClientResponse = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();
            }
            catch (WebException exception)
            {
                LoggerFactory.CreateLogger().Log("Error:: Hitting URL in Core.HttpClientHelper.PostData ::" + exception.ToString(), LogType.Error);
                clientResponse.ClientResponse = exception.Message;
                if (exception.Status == WebExceptionStatus.ProtocolError)
                {
                    httpResponse = (HttpWebResponse)exception.Response;
                    clientResponse.HttpStatusCode = ((int)httpResponse.StatusCode).ToString();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                LoggerFactory.CreateLogger().Log("Error:: Hitting URL in Core.HttpClientHelper.PostData ::" + ex.ToString(), LogType.Error);
                clientResponse.ClientResponse = ex.Message;
            }

            stopwatch.Stop();
            clientResponse.ProcessingTimeMilliseconds = stopwatch.ElapsedMilliseconds;
            return clientResponse;
        }
    }

    public class HttpClientResponse
    {
        public string HttpStatusCode { get; set; }
        public string ClientResponse { get; set; }
        public long ProcessingTimeMilliseconds { get; set; }
    }

    class CertPolicy : ICertificatePolicy
    {
        public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
        {
            return true;
        }
    }
}
