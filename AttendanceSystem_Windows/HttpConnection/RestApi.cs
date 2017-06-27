using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Collections;
using AttendanceSystem_Windows.Services;

namespace AttendanceSystem_Windows.HttpConnection {
    /// <summary>
    /// 专门用于统一处理REST风格的API的类。
    /// </summary>
    public class RestApi {
        public RestApi(string Server, string username = null, string password = null) {
            resources = new Dictionary<string, RestResource>();
            ServerURL = Server;
            this.username = username;
            this.password = password;
        }
        /// <summary>
        /// 用户名和密码。
        /// </summary>
        public string username, password;
        /// <summary>
        /// 服务器的URL地址。末尾请带上反斜线。
        /// </summary>
        public string ServerURL { get; set; }
        /// <summary>
        /// 缓存的资源地址。
        /// </summary>
        private Dictionary<string, RestResource> resources;
        /// <summary>
        /// 定位一项资源。
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public RestResource this[string url] {
            get {
                if (resources.ContainsKey(url)) {
                    return resources[url];
                }else {
                    var result = new RestResource(this, url);
                    resources.Add(url, result);
                    return result;
                }
            }
        }

    }
    /// <summary>
    /// 定位一项资源。
    /// </summary>
    public class RestResource {
        public RestResource(RestApi Server, string URL) {
            ResourceURL = URL;
            server = Server;
            con = new HttpConnect(server.ServerURL + ResourceURL, "GET");
            con.autoclean = true;
        }

        /// <summary>
        /// 资源的URL地址。
        /// </summary>
        public string ResourceURL { get; set; }
        /// <summary>
        /// 定位其服务器资源。
        /// </summary>
        public RestApi server { get; private set; }
        
        internal HttpConnect con = null;

        /// <summary>
        /// 获取单个资源。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RestInstance this[string id] {
            get {
                return id != null ? new RestInstance(this, id) : null;
            }
        }
        /// <summary>
        /// 执行POST操作。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Response Create(string data) {
            if (data != null) {
                UpdateConnect(ResourceURL, "POST");
                con.content = data;
                con.Connect();
                return new Response(con.status, con.response);
            } else return null;
        }
        /// <summary>
        /// 执行GET操作。
        /// </summary>
        /// <returns></returns>
        public Response List(IDictionary<string,string> filter = null, string[] ordering = null, string search = null) {
            UpdateConnect(ResourceURL, "GET");
            var p = con.param;
            if (ordering != null) p["ordering"] = ordering.join(",");
            if (search != null) p.Add("search", search);
            if (filter != null)
                foreach (var i in filter)
                    p.Add(i.Key, i.Value);
            con.Connect();
            return new Response(con.status, con.response);
        }

        public Task<Response> CreateAsync(string data) {
            return Task.Run(() => Create(data));
        }
        public Task<Response> ListAsync(IDictionary<string, string> filter = null, string[] ordering = null, string search = null) {
            return Task.Run(() => List(filter, ordering, search));
        }
        /// <summary>
        /// 更新HttpConnect的基础信息，包括url,method,auth。
        /// </summary>
        internal void UpdateConnect(string url, string method) {
            con.authentication = new Auth(server.username, server.password);
            con.url = server.ServerURL + url;
            con.method = method;
        }
    }
    /// <summary>
    /// 定位一项资源的单个实例。
    /// </summary>
    public class RestInstance {
        public RestInstance(RestResource Resource, string Instance) {
            resource = Resource;
            instance = Instance;
        }
        
        /// <summary>
        /// 定位其资源路径。
        /// </summary>
        public RestResource resource { get; private set; } 

        public string instance { get; private set; }
        /// <summary>
        /// 执行GET操作。
        /// </summary>
        /// <returns></returns>
        public Response Retrieve() {
            resource.UpdateConnect(resource.ResourceURL + instance + "/", "GET");
            resource.con.Connect();
            return new Response(resource.con.status, resource.con.response);
        }
        /// <summary>
        /// 执行PATCH操作。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Response Update(string data, string method = "PATCH") {
            if (data != null && (method == "PATCH" || method == "PUT")) {
                resource.UpdateConnect(resource.ResourceURL + instance + "/", method);
                resource.con.content = data;
                resource.con.Connect();
                return new Response(resource.con.status, resource.con.response);
            } else return null;
        }
        /// <summary>
        /// 执行DELETE操作。
        /// </summary>
        /// <returns></returns>
        public Response Destroy() {
            resource.UpdateConnect(resource.ResourceURL + instance + "/", "DELETE");
            resource.con.Connect();
            return new Response(resource.con.status, resource.con.response);
        }

        public Task<Response> RetrieveAsync() {
            return Task.Run(() => Retrieve());
        }

        public Task<Response> UpdateAsync(string data, string method = "PATCH") {
            return Task.Run(() => Update(data, method));
        }

        public Task<Response> DestroyAsync() {
            return Task.Run(() => Destroy());
        }
    }
    /// <summary>
    /// 表示执行一次操作之后的回执内容。
    /// </summary>
    public class Response {
        public Response(HttpStatusCode status, string content, bool isJson = true) {
            this.StatusCode = status;
            this.status = (int)StatusCode;
            this.content = content;
            if (isJson) {
                try {
                    jtoken = JToken.Parse(content);
                }catch(Exception e) {
                    jtoken = null;
                }
            }
        }

        public HttpStatusCode StatusCode;
        public int status;
        public string content;
        /// <summary>
        /// 检查状态码是否符合指定的配对模式。
        /// </summary>
        /// <param name="like">一个数字字符串。使用确定的数字表示位，并使用*表示任意数字。</param>
        /// <returns></returns>
        public bool statuslike(string like) {
            string statuses = status.ToString();
            if (statuses.Length != like.Length) return false;
            for(int i=0;i<like.Length;++i)
                if(like[i]!='*' && like[i]>='0' && like[i] <= '9') {
                    if (statuses[i] != like[i]) return false;
                }
            return true;
        }
        public JToken token { get { return jtoken; } }
        public JArray list { get { return jtoken == null ? null : jtoken.Type == JTokenType.Array ? jtoken as JArray : null; } }
        public JObject instance { get { return jtoken == null ? null : jtoken.Type == JTokenType.Object ? jtoken as JObject : null; } }

        private JToken jtoken = null;

    }
}
