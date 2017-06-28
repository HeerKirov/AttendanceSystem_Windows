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
using AttendanceSystem_Windows.HttpConnection;
using AttendanceSystem_Windows.LocalData;
using AttendanceSystem_Windows.Services;
using Newtonsoft.Json.Linq;
using AttendanceSystem_Windows.PageFolder.Template;

namespace AttendanceSystem_Windows.PageFolder.Teacher {
    /// <summary>
    /// self_document.xaml 的交互逻辑
    /// </summary>
    public partial class self_document : BasePage {
        public self_document() {
            InitializeComponent();

            //txt_gender.Choices = gender_choices;
            template = new InstanceTemplate();

            var args = new InstanceArgs("api/auth/users/", User.Get().authentication.username, User.Api, "个人资料", Deleteable: false);
            args.margin = new Thickness(0, 10, 0, 10);
            args.items.Add(new InstanceArgs.Item("id", "用户名", "text"));
            args.items.Add(new InstanceArgs.Item("name", "姓名", "text", EditAbleStatus.Writeable));
            args.items.Add(new InstanceArgs.Item("gender", "性别", "choice", EditAbleStatus.Writeable, data: gender_choices));
            args.items.Add(new InstanceArgs.Item("register_time", "注册时间", "text"));
            args.items.Add(new InstanceArgs.Item("last_login_time", "最近登录", "text"));

            template.Construct(args);
            ContentGrid.Children.Add(template);
        }
        public KeyValuePair<string, string>[] gender_choices = new KeyValuePair<string, string>[] {
            new KeyValuePair<string, string>("MALE","男"),
            new KeyValuePair<string, string>("FEMALE","女")
        };
        private InstanceTemplate template;


        public override void NavigatedToEvent(object sender, IDictionary<string, object> kwargs) {
            base.NavigatedToEvent(sender, kwargs);
            template.UpdateData();
        }

    }
}
