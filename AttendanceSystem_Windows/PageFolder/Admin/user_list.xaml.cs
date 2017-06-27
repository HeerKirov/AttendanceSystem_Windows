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

namespace AttendanceSystem_Windows.PageFolder.Admin {
    /// <summary>
    /// user_list.xaml 的交互逻辑
    /// </summary>
    public partial class user_list : BasePage {
        public user_list() {
            InitializeComponent();
            default_auth = new AuthSetting[] {
                new AuthSetting(Authority.UserManage)
            };
            template = new ListTemplate();

            var cargs = new CreateArgs("api/auth/users/", User.Api, "创建用户", Margin: new Thickness(0, 10, 0, 10));
            cargs.items.Add(new CreateArgs.Item("username", "账号", "text"));
            cargs.items.Add(new CreateArgs.Item("password", "密码", "text"));
            cargs.items.Add(new CreateArgs.Item("name", "姓名", "text"));
            cargs.items.Add(new CreateArgs.Item("gender", "性别", "choice", data: gender_choices.ToArray()));
            cargs.items.Add(new CreateArgs.Item("authority", "职位", "multi", data: authority_choices.ToArray()));

            var args = new ListArgs("api/auth/users/", User.Api, Title, Searchable:true, Createable:true, Createargs:cargs);
            args.columns.Add(new ListArgs.Column("id", "账号", ordering: "id", 
                hyperlink:ListArgs.autohyperlink("basic-other-document").add("id").lambda));
            args.columns.Add(new ListArgs.Column("name", "姓名", ordering: "name",
                hyperlink: ListArgs.autohyperlink("basic-other-document").add("id").lambda));
            args.columns.Add(new ListArgs.Column("gender", "性别", ordering: "gender", type: "choice", data: gender_choices));
            args.columns.Add(new ListArgs.Column("last_login_time", "最后登录"));

            template.Construct(args);
            ContentGrid.Children.Add(template);
        }
        public static Dictionary<string, string> gender_choices = new Dictionary<string, string>() {
            { "MALE","男" },
            { "FEMALE","女" }
        };
        public static Dictionary<string, string> authority_choices = new Dictionary<string, string>() {
            { "Student","学生"},
            { "Teacher","教师"},
            { "Instructor","辅导员"},
            { "Office","教务处"},
        };
        private ListTemplate template;
        public override void NavigatedToEvent(object sender, IDictionary<string, object> kwargs) {
            base.NavigatedToEvent(sender, kwargs);
            template.UpdateData();
        }
    }
}
