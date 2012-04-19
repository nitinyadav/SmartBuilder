using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Web.Script.Serialization;


namespace StackApi
{
    public class apiFunctions
    {
        private const string ApiVersion = "1.1";
        private const string BaseUri = "http://api.stackoverflow.com/" + ApiVersion;
        private readonly string _apiKey;
        public RootObject rootObject;
        public AnswerObject aObject;
        public QuestionObject qObject;

        public apiFunctions()
        {
            this._apiKey = "O979-QMfw06xQjD2MWfnrA";
        }

        public int? MaxRateLimit { get; set; }
        public int? CurrentRateLimit { get; set; }

        private string ComposeUri(string path)
        {
            var uri = String.Format("{0}{1}", BaseUri, path);
            if (!String.IsNullOrWhiteSpace(this._apiKey))
            {
                //var separator = uri.Contains("?") ? "&" : "?";
                var separator = "&";
                uri = String.Format("{0}{1}key={2}", uri, separator, this._apiKey);
            }
            return uri;
        }

        private void ParseHeaders(WebResponse response)
        {
            if (response.Headers == null) return;

            if (response.Headers.AllKeys.Contains("X-RateLimit-Max"))
            {
                this.MaxRateLimit = Int32.Parse(response.Headers["X-RateLimit-Max"]);
            }
            if (response.Headers.AllKeys.Contains("X-RateLimit-Current"))
            {
                this.CurrentRateLimit = Int32.Parse(response.Headers["X-RateLimit-Current"]);
            }
        }

        private string ExtractJsonResponse(WebResponse response)
        {
            ParseHeaders(response);

            string json;
            using (var outStream = new MemoryStream())
            using (var zipStream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
            {
                zipStream.CopyTo(outStream);
                outStream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(outStream, Encoding.UTF8))
                {
                    json = reader.ReadToEnd();
                }
            }
            return json;
        }

        public QuestionObject ParseSearchJson(string json)
        {

            JavaScriptSerializer js = new JavaScriptSerializer();
            QuestionObject rt = js.Deserialize<QuestionObject>(json);
            return rt;
        }

        public AnswerObject ParseAnswerJson(string json)
        {

            JavaScriptSerializer js = new JavaScriptSerializer();
            AnswerObject rt = js.Deserialize<AnswerObject>(json);
            return rt;
        }

        public RootObject ParseSimilarJson(string json)
        {

            JavaScriptSerializer js = new JavaScriptSerializer();
            RootObject rt = js.Deserialize<RootObject>(json);
            return rt;
        }

        public string getAnswer(string answerUrl)
        {
            string answerResponse = GetResponse(answerUrl + "?body=true");
            aObject = ParseAnswerJson(answerResponse);
            return answerResponse;
        }

        public string similar(string searchString, string[] tags)
        {
            StringBuilder parameter = new StringBuilder("/similar?answers=true&body=true&title=");
            StringBuilder searchText = new StringBuilder(searchString);
            //searchText.Replace(" ", "%20");
            parameter.Append(Uri.EscapeDataString(searchText.ToString()));
            if (tags.Length != 0)
            {
                parameter.Append("&tagged=");
                foreach (string s in tags)
                {
                    if (parameter[parameter.Length-1].Equals('='))
                        parameter.Append(Uri.EscapeDataString(s));
                    else
                        parameter.Append(Uri.EscapeDataString(";" + s));
                }
            }
            string url = parameter.ToString();
            string similarResponse = GetResponse(url);
            rootObject = ParseSimilarJson(similarResponse);
            return similarResponse;
        }

        public string search(string searchString)
        {
            StringBuilder searchPrefix = new StringBuilder("/search?intitle=");
            StringBuilder searchTag = new StringBuilder(searchString);
            searchTag.Replace(" ", "%20");
            searchPrefix.Append(searchTag);
            string url = searchPrefix.ToString();
            string searchResponse = GetResponse(url);
            qObject = ParseSearchJson(searchResponse);
            return searchResponse;
        }

        public string GetResponse(string requestUri)
        {
            var request = (HttpWebRequest)WebRequest.Create(ComposeUri(requestUri));
            StreamWriter sr = new StreamWriter("E:\\Data\\Response.txt");
            sr.WriteLine(ComposeUri(requestUri));
            request.Method = WebRequestMethods.Http.Get;
            request.Accept = "application/json";
            //ICredentials cred = new NetworkCredential("296820","nitin123nitin");
            //request.Proxy = new WebProxy("http://10.1.1.19:80/", true,null,cred);
            var json = ExtractJsonResponse(request.GetResponse());
            sr.WriteLine(json);
            sr.Close();
            return json;
        }

        public string cleanText(string text)
        {
            StringBuilder cleaned = new StringBuilder();
            bool flag = false;
            foreach (char c in text)
            {
                if (flag == true)
                {
                    if (c == '>')
                        flag = false;

                    continue;
                }
                if (c == '<')
                {
                    cleaned.Append("\n");
                    flag = true;
                }
                else
                {
                    cleaned.Append(c);
                }
            }
            return cleaned.ToString();
        }
    }
}
