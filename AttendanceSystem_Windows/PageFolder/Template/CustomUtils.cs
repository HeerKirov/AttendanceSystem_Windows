using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystem_Windows.PageFolder.Template {
    public struct CustomButton {
        public CustomButton(string Title, Action Action_, string Fontfamily = "Segoe MDL2 Assets", string Tips = null) {
            title = Title;
            action = Action_;
            fontfamily = Fontfamily;
            tips = Tips;
        }
        public string title;
        public string fontfamily;
        public string tips;
        public Action action;
    }
}
