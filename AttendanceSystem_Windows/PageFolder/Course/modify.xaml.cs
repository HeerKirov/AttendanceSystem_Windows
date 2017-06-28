using AttendanceSystem_Windows.PageFolder.Template;
using AttendanceSystem_Windows.Services;
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
    /// modify.xaml 的交互逻辑
    /// </summary>
    public partial class modify : BasePage {
        public modify() {
            InitializeComponent();
            default_auth = new AuthSetting[] {
                new AuthSetting(Authority.CourseManage)
            };
            template = new InstanceTemplate();
            var args = new InstanceArgs("api/courses/basic/", null, User.Api, Title, Onlywrite: true);
            args.items.Add(new InstanceArgs.Item("id", "课程编号", "text"));
            args.items.Add(new InstanceArgs.Item("name", "课程名称", "text"));
            args.items.Add(new InstanceArgs.Item("teacher", "教师(工号)", "text"));
            args.items.Add(new InstanceArgs.Item("as_student_set", "学生(工号)", "multitext"));
            template.Construct(args);
            ContentGrid.Children.Add(template);
        }
        private string ID = null;
        private InstanceTemplate template;
        public override void NavigatedToEvent(object sender, IDictionary<string, object> kwargs) {
            base.NavigatedToEvent(sender, kwargs);
            if (kwargs != null) ID = kwargs["id"] as string;
            if(ID != null) {
                template.Args.instance = ID;
                template.UpdateData();
            }
        }
    }
}
