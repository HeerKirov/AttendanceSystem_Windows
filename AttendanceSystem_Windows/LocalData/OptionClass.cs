using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;

namespace AttendanceSystem_Windows.LocalData {
    public class OptionClass {
        public static OptionClass Get() => reference == null ? reference = new OptionClass() : reference;
        private static OptionClass reference = null;

        private const string config_file = "config.data";
        private const string default_server = "http://127.0.0.1:8000/";

        private OptionClass() { }

        public string ServerURL { get; set; } = "http://127.0.0.1:8000/";
        public string GetURL(string url = "") {
            return ServerURL + url;
        }

        public bool RememberPasswd { get; set; } = false;
        public bool AutoLogin { get; set; } = false;

        public string Username { get; set; } = "";
        public string Password {
            get { if (!RememberPasswd) password_ = "";return password_; }
            set { if (RememberPasswd) password_ = value; }
        }
        private string password_ = "";

        public void WriteToConfig() {
            var jdata = new JObject();
            jdata["serverurl"] = new JValue(ServerURL);
            jdata["rememberpasswd"] = new JValue(RememberPasswd);
            jdata["autologin"] = new JValue(AutoLogin);
            jdata["username"] = new JValue(topw(Username));
            jdata["password"] = new JValue(topw(Password));

            File.WriteAllBytes(config_file, Encoding.Default.GetBytes(topw(jdata.ToString())));
        }
        public void ReadFromConfig() {
            if (!File.Exists(config_file)) File.Create(config_file).Close();
            var p = frompw(Encoding.Default.GetString(File.ReadAllBytes(config_file)));
            try {
                var jdata = JObject.Parse(p);
                ServerURL = jdata["serverurl"]?.ToString() ?? default_server;
                RememberPasswd = bool.Parse(jdata["rememberpasswd"]?.ToString() ?? "false");
                AutoLogin = bool.Parse(jdata["autologin"]?.ToString() ?? "false");
                Username = frompw(jdata["username"]?.ToString());
                Password = frompw(jdata["password"]?.ToString());
            }catch(Exception e) {
                ServerURL = default_server;
                RememberPasswd = false;
                AutoLogin = false;
                Username = "";
                password_ = "";
            }
        }
        /// <summary>
        /// 将明文加密。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string topw(string str) {
            if (str == null) return "";
            var bytes = Encoding.Default.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }
        /// <summary>
        /// 解密。
        /// </summary>
        /// <param name="pw"></param>
        /// <returns></returns>
        public string frompw(string pw) {
            if (pw == null) return "";
            var bytes = Convert.FromBase64String(pw);
            return Encoding.Default.GetString(bytes);
        }

    }
}
