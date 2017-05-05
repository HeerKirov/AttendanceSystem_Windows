using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace AttendanceSystem_Windows.HttpConnection {
    public class HttpConnect {
        
        public HttpConnect() { }

        public HttpConnect(string url, string method, string data = "", Dictionary<string,string> args = null, string username = null, string password = null) {
            this.url = url;
            this.method = method;
            this.data = data;
            if (args != null)
                foreach (var item in args)
                    this.args.Add(item.Key, item.Value);
            this.username = username;
            this.password = password;
        }

        public HttpConnect SetProperty(string url=null, string method=null, string data=null,Dictionary<string,string> args=null,string username=null,string password = null) {
            if (url != null) this.url = url;
            if (method != null) this.method = method;
            if (data != null) this.data = data;
            if (args != null) {
                this.args.Clear();
                foreach(var item in args) this.args.Add(item.Key, item.Value);
            }
            if (username != null) this.username = username;
            if (password != null) this.password = password;
            return this;
        }

        public Task ConnectAsync() {
            return Task.Run(() => {
                Connect();
            });
        }

        public void Connect() {
            string strdata = "";
            if (method == "GET") {
                bool first = true;
                foreach (var item in args) {
                    if (first) {
                        strdata += "?";
                        first = false;
                    } else {
                        strdata += "&";
                    }
                    strdata += WebUtility.UrlEncode(item.Key) + "=" + WebUtility.UrlEncode(item.Value);
                }
            }
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url+strdata);
            request.Method = method;
            if (username != null && username != "" && password != null) request.Credentials = new NetworkCredential(username, password);
            request.ContentType = "application/json;charset=UTF-8";
            if (method == "POST" || method == "PUT" || method == "PATCH") {
                byte[] payload = Encoding.UTF8.GetBytes(data);
                request.ContentLength = payload.Length;
                Stream writer = request.GetRequestStream();
                writer.Write(payload, 0, payload.Length);
                writer.Close();
            }
            //接收请求
            try {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                    using (Stream readerstream = response.GetResponseStream()) {
                        using (StreamReader reader = new StreamReader(readerstream, Encoding.UTF8)) {
                            this.response = reader.ReadToEnd();
                        }
                    }
                }
            }catch(WebException e) {
                this.status = (e.Response as HttpWebResponse).StatusCode;
                this.response = "";
            } finally {
                if (autoclean) {
                    data = "";
                    args.Clear();
                }
            }
        }

        public string url { get; set; } = "";

        public string method { get; set; } = "";

        public string data { get; set; } = "";

        public Dictionary<string, string> args { get; private set; } = new Dictionary<string, string>();

        public string response { get; private set; } = "";

        public HttpStatusCode status { get; private set; } = default(HttpStatusCode);

        public bool autoclean { get; set; } = true;

        public string username { get; set; } = null;
        public string password { get; set; } = null;

    }
}
