using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystem_Windows.LocalData {
    public class OptionClass {
        public static OptionClass Get() => reference == null ? reference = new OptionClass() : reference;
        private static OptionClass reference = null;
        private OptionClass() { }

        public string ServerURL { get; private set; } = "http://127.0.0.1:8000/";
        public string GetURL(string url = "") {
            return ServerURL + url;
        }
    }
}
