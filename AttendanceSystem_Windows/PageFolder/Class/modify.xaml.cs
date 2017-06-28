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

namespace AttendanceSystem_Windows.PageFolder.Class {
    /// <summary>
    /// modify.xaml 的交互逻辑
    /// </summary>
    public partial class modify : BasePage {
        public modify() {
            InitializeComponent();
            default_auth = new AuthSetting[] {
                new AuthSetting(Authority.ClassManage)
            };
            template = new InstanceTemplate();
            var args = new InstanceArgs("api/classes/", null, User.Api, Title, Onlywrite: true);
            args.items.Add(new InstanceArgs.Item("id", "班级编号", "text"));
            args.items.Add(new InstanceArgs.Item("grade", "年级", "text"));
            args.items.Add(new InstanceArgs.Item("college", "学院", "text"));
            args.items.Add(new InstanceArgs.Item("major", "专业", "text"));
            args.items.Add(new InstanceArgs.Item("number", "班号", "text"));
            args.items.Add(new InstanceArgs.Item("as_instructor_set", "辅导员(工号)", "multitext"));
            args.items.Add(new InstanceArgs.Item("as_student_set", "学生(学号)", "multitext"));
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
