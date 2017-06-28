using AttendanceSystem_Windows.PageFolder.Template;
using AttendanceSystem_Windows.Services;
using Newtonsoft.Json.Linq;
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

namespace AttendanceSystem_Windows.PageFolder.Course {
    /// <summary>
    /// detail.xaml 的交互逻辑
    /// </summary>
    public partial class detail : BasePage {
        public detail() {
            InitializeComponent();

            list_temp = new ListTemplate();
            var largs = new ListArgs(null, User.Api, "出勤记录表", Searchable: true);
            largs.columns.Add(new ListArgs.Column("id", "编号"));
            largs.columns.Add(new ListArgs.Column("student", "学生姓名"));
            largs.columns.Add(new ListArgs.Column("course_manage", "课程编号"));
            largs.columns.Add(new ListArgs.Column("date", "日期"));
            largs.columns.Add(new ListArgs.Column("course_number", "节次"));
            largs.columns.Add(new ListArgs.Column("status", "出勤状态",type:"choice",data:AttendanceState.attendance_state));
            list_temp.Construct(largs);
            ListGrid.Children.Add(list_temp);
        }
        private string ID = null;
        private ListTemplate list_temp;
        public override bool NavigatingToEvent(object sender, IDictionary<string, object> kwargs) {
            if (kwargs != null) {
                ID = kwargs["id"] as string;
            }
            if (ID != null) {
                default_auth = new AuthSetting[] {
                    new AuthSetting(Authority.CourseManage),
                    new AuthSetting(BelongRelation.Parent,"course",ID)
                };
            }
            return base.NavigatingToEvent(sender, kwargs);
        }

        public override void NavigatedToEvent(object sender, IDictionary<string, object> kwargs) {
            base.NavigatedToEvent(sender, kwargs);
            if (kwargs != null) {
                ID = kwargs["id"] as string;
            }
            if (ID != null) {
                var res = User.Api["api/courses/manage"][ID].Retrieve();
                if (res.statuslike("2**")) {
                    var datas = res.instance["attendance_record_set"] as JArray;
                    list_temp.UpdateData(datas);
                }
            }
        }
    }
}
