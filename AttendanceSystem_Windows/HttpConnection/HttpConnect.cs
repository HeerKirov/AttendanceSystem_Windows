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
using EasyHttp.Http;

namespace AttendanceSystem_Windows.HttpConnection {
    public class HttpConnect {
        
        public HttpConnect() {
            client_method = new Dictionary<string, Func<string, object, string, object, HttpResponse>>();
            client_method["POST"] = client.Post;
            client_method["PUT"] = client.Put;
            client_method["PATCH"] = client.Patch;
            client_safe_method = new Dictionary<string, Func<string, object, HttpResponse>>();
            client_safe_method["GET"] = client.Get;
            client_safe_method["DELETE"] = client.Delete;
            client_safe_method["HEAD"] = client.Head;
            client.Request.Accept = HttpContentTypes.ApplicationJson;
        }
        public HttpConnect(string url,string method,string data=null,IDictionary<string,string> param=null,string username=null,string password = null):this() {
            this.url = url;
            this.method = method;
            this.content = data;
            if (param != null) this.param = new Dictionary<string, string>(param);
            if (username != null && password != null) this.authentication = new Auth(username, password);
        }

        public void Connect() {
            try {
                while (!locks) { }
                locks = false;
                HttpResponse res = null;
                if (client_method.Keys.Contains(method)) res = client_method[method](url, content, HttpContentTypes.ApplicationJson, null);
                else if (client_safe_method.Keys.Contains(method)) res = client_safe_method[method](url + getparam(), null);
                else if (method == "OPTIONS") res = client.Options(url + getparam());
                status = res.StatusCode;
                response = res.RawText;
            }catch(WebException e) {
                response = "";
                status = HttpStatusCode.BadRequest;
            } finally {
                if (autoclean) {
                    content = "";
                    param.Clear();
                }
                locks = true;
            }
        }

        private bool locks=true;

        public void Connect(string url=null,string method=null, string data = null, IDictionary<string, string> param = null) {
            this.url = url ?? this.url;
            this.method = method ?? this.method;
            this.content = data ?? this.content;
            if (param != null) this.param = new Dictionary<string, string>(param);
            Connect();
        }
        public Task ConnectAsync() {
            return Task.Run(() => {
                Connect();
            });
        }
        public Task ConnectAsync(string url = null, string method = null, string data = null, IDictionary<string, string> param = null) {
            return Task.Run(() => {
                Connect(url, method, data, param);
            });
        }

        private Dictionary<string, Func<string, object, string, object, HttpResponse>> client_method;
        private Dictionary<string, Func<string, object, HttpResponse>> client_safe_method;

        private string getparam() {
            if (param.Count > 0) {
                StringBuilder sb = new StringBuilder("?");
                bool first = true;
                foreach (var i in param) {
                    sb.Append((first ? "" : "&") + WebUtility.UrlEncode(i.Key) + "=" + WebUtility.UrlEncode(i.Value));
                    if (first) first = false;
                }
                return sb.ToString();
            } else return "";
        }

        public string url { get; set; } = "";

        public string method { get; set; } = "GET";

        public Auth authentication {
            get { return auth; }
            set {
                auth = value;
                if (auth?.valid()??false) {
                    client.Request.ForceBasicAuth = true;
                    client.Request.SetBasicAuthentication(auth.username, auth.password);
                }else {
                    client.Request.ForceBasicAuth = false;
                }
            }
        }

        public string content { get; set; } = "";

        public IDictionary<string, string> param { get; private set; } = new Dictionary<string, string>();

        public bool autoclean { get; set; } = true;

        public string response { get; private set; } = null;

        public HttpStatusCode status { get; private set; } = default(HttpStatusCode);

        private HttpClient client = new HttpClient();

        private Auth auth = null;

    }

    public class Auth {
        public Auth(string username=null,string password = null) {
            this.username = username;
            this.password = password;
        }
        public bool valid() {
            return username != null && password != null && username.Length > 0 && password.Length > 0;
        }
        public string username;
        public string password;
    }
}
