using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AttendanceSystem_Windows.PageFolder;
using AttendanceSystem_Windows.HttpConnection;
using AttendanceSystem_Windows.LocalData;
using Newtonsoft.Json.Linq;

namespace AttendanceSystem_Windows.PageFolder.Basic {
    /// <summary>
    /// schedule.xaml 的交互逻辑
    /// </summary>
    public partial class schedule : BasePage {
        public schedule() {
            InitializeComponent();
        }

        public override void NavigatedToEvent(object sender, IDictionary<string, object> kwargs) {
            HttpConnect con = new HttpConnect(OptionClass.Get().GetURL("action/schedule"), "GET");
            con.Connect();
            if (con.status == System.Net.HttpStatusCode.OK) {
                var response = con.response;
                JObject data = JObject.Parse(response);
                TimeRange.Text = string.Format("执行时间:{0} - {1}", data["begin"].ToString(), data["end"].ToString());
                JArray items = data["items"] as JArray;
                foreach(var i in items) {
                    Tuple<int, string, string> itemdata = new Tuple<int, string, string>(int.Parse(i["no"].ToString()), i["begin"].ToString(), i["end"].ToString());
                    var ctrl = new schedule_timeitem();
                    ctrl.Height = 60;
                    ctrl.content = itemdata;
                    TimeList.Children.Add(ctrl);
                }
            }   
        }
    }
}
