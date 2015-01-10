using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Json;

namespace TwoTurnGameConnector
{

    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                if (args.Length < 2)
                {
                    throw new ArgumentException("One or two arguments required");
                }

                string readAllText = File.ReadAllText("config.json");
                var config = JsonParser.Deserialize(readAllText);

                String host = config.host;
                String port = config.port;

                if (!host.StartsWith("http"))
                {
                    host = string.Format("http://{0}", host);
                }
                string fullUrl = String.Format("{0}:{2}{1}", host, args[1], port);
                if (args[0].ToUpper() == "GET")
                {
  
                    string response = GetHttp(fullUrl);
                    File.WriteAllText("response.txt", response);
                }
                else if (args[0].ToUpper() == "POST")
                {
                    if (args.Length < 3)
                    {
                        throw new ArgumentException("Two arguments required");
                    }
                    string allText = File.ReadAllText(args[2]);
                    PostHttp(fullUrl, allText);
                }
                else
                {
                    throw new ArgumentException("First argument is not GET or POST");
                }

               
            }
            catch (WebException e)
            {
                WebResponse webResponse = e.Response;
                Console.WriteLine(e.Message);
                if (webResponse != null && webResponse.GetResponseStream() != null)
                {
                    var responseString = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                    Console.WriteLine("POST failed because: " + responseString);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
            Console.ReadLine();
        }

        static void PostHttp(String url, String postdata)
        {
            Console.WriteLine("post: " + url);
            Console.WriteLine("data: " + postdata);
            WebRequest webRequest = WebRequest.Create(url);
            var data = Encoding.ASCII.GetBytes(postdata);

            webRequest.Method = "POST";
            webRequest.ContentType = "text/plain";
            webRequest.ContentLength = data.Length;
            using (var stream = webRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)webRequest.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            

            Console.WriteLine(responseString);
        }


        static String GetHttp(String url)
        {
            Console.WriteLine("get: " + url);
            WebRequest webRequest = WebRequest.Create(url);
            WebResponse webResponse = webRequest.GetResponse();
            string result = null;
            using (var reader = new StreamReader(webResponse.GetResponseStream()))
            {
                result = reader.ReadToEnd(); // do something fun...
            }
            Console.WriteLine(result);
            return result;
        }
    }
}
