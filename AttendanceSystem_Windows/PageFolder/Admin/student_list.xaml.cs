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
using static AttendanceSystem_Windows.PageFolder.Template.ListArgs;

namespace AttendanceSystem_Windows.PageFolder.Admin {
    /// <summary>
    /// student_list.xaml 的交互逻辑
    /// </summary>
    public partial class student_list : BasePage {
        public student_list() {
            InitializeComponent();
            default_auth = new AuthSetting[] {
                new AuthSetting(Authority.StudentManage)
            };
            template = new ListTemplate();
            var cargs = new CreateArgs("api/auth/students/", User.Api, "创建学生用户",Margin:new Thickness(0,10,0,10));
            cargs.items.Add(new CreateArgs.Item("username", "学号", "text"));
            cargs.items.Add(new CreateArgs.Item("password", "密码", "text"));
            cargs.items.Add(new CreateArgs.Item("name", "姓名", "text"));
            cargs.items.Add(new CreateArgs.Item("gender", "性别", "choice", data: gender_choices.ToArray()));

            var args = new ListArgs("api/auth/students/", User.Api, Title, Searchable: true, Createable:true,Createargs:cargs);
            args.columns.Add(new ListArgs.Column("id", "学号", ordering: "id",
                hyperlink: ListArgs.autohyperlink("student-other-document").add("id").lambda));
            args.columns.Add(new ListArgs.Column("user", "姓名", ordering: "username",
                hyperlink: ListArgs.autohyperlink("student-other-document").add("id").lambda));
            args.columns.Add(new ListArgs.Column("gender_related", "性别", type: "choice", data: gender_choices));
            args.columns.Add(new ListArgs.Column("classs", "班级",
                hyperlink:ListArgs.autohyperlink("class-instance").add("id","classs").lambda));



            template.Construct(args);
            ContentGrid.Children.Add(template);
        }

        public static Dictionary<string, string> gender_choices = new Dictionary<string, string>() {
            { "MALE","男" },
            { "FEMALE","女" }
        };
        private ListTemplate template;
        public override void NavigatedToEvent(object sender, IDictionary<string, object> kwargs) {
            base.NavigatedToEvent(sender, kwargs);
            template.UpdateData();
        }
    }
}
