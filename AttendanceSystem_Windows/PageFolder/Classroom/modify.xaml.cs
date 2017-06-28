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

namespace AttendanceSystem_Windows.PageFolder.Classroom {
    /// <summary>
    /// modify.xaml 的交互逻辑
    /// </summary>
    public partial class modify : BasePage {
        public modify() {
            InitializeComponent();
            default_auth = new AuthSetting[] {
                new AuthSetting(Authority.ClassroomManage)
            };
            template = new InstanceTemplate();
            var args = new InstanceArgs("api/classrooms/basic/", null, User.Api, Title, Onlywrite: true);
            args.items.Add(new InstanceArgs.Item("id", "教室编号", "text"));
            args.items.Add(new InstanceArgs.Item("name", "教室名称", "text"));
            args.items.Add(new InstanceArgs.Item("size", "教室大小", "text"));
            template.Construct(args);
            ContentGrid.Children.Add(template);

            pw_temp = new InstanceTemplate();
            var pargs = new InstanceArgs("api/classrooms/manage/", null, User.Api, "教室验证码", Deleteable: false);
            pargs.items.Add(new InstanceArgs.Item("password", "验证码", "text", Editable:EditAbleStatus.Writeable));
            pw_temp.Construct(pargs);
            PwGrid.Children.Add(pw_temp);
        }
        private string ID = null;
        private InstanceTemplate template, pw_temp;
        public override void NavigatedToEvent(object sender, IDictionary<string, object> kwargs) {
            base.NavigatedToEvent(sender, kwargs);
            if (kwargs != null) ID = kwargs["id"] as string;
            if (ID != null) {
                template.Args.instance = ID;
                template.UpdateData();
                pw_temp.Args.instance = ID;
                pw_temp.UpdateData();
            }
        }
    }
}
