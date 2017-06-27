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
    /// teacher_list.xaml 的交互逻辑
    /// </summary>
    public partial class instructor_list : BasePage {
        public instructor_list() {
            InitializeComponent();
            default_auth = new AuthSetting[] {
                new AuthSetting(Authority.InstructorManage)
            };
            template = new ListTemplate();

            var cargs = new CreateArgs("api/auth/instructors/", User.Api, "创建辅导员用户", Margin: new Thickness(0, 10, 0, 10));
            cargs.items.Add(new CreateArgs.Item("username", "工号", "text"));
            cargs.items.Add(new CreateArgs.Item("password", "密码", "text"));
            cargs.items.Add(new CreateArgs.Item("name", "姓名", "text"));
            cargs.items.Add(new CreateArgs.Item("gender", "性别", "choice", data: gender_choices.ToArray()));
            var args = new ListArgs("api/auth/instructors/", User.Api, Title, Searchable: true,Createable:true, Createargs:cargs);
            args.columns.Add(new ListArgs.Column("id", "工号", ordering: "id",
                hyperlink: ListArgs.autohyperlink("instructor-other-document").add("id").lambda));
            args.columns.Add(new ListArgs.Column("user", "姓名", ordering: "username",
                hyperlink: ListArgs.autohyperlink("instructor-other-document").add("id").lambda));
            args.columns.Add(new ListArgs.Column("gender_related", "性别", type: "choice", data: gender_choices));

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
