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
    public partial class class_list : BasePage {
        public class_list() {
            InitializeComponent();
            default_auth = new AuthSetting[] {
                new AuthSetting(Authority.ClassManage)
            };
            template = new ListTemplate();

            var cargs = new CreateArgs("api/classes/", User.Api, "创建班级", Margin: new Thickness(0, 10, 0, 10));
            cargs.items.Add(new CreateArgs.Item("id", "班级编号", "text"));
            cargs.items.Add(new CreateArgs.Item("grade", "年级", "text"));
            cargs.items.Add(new CreateArgs.Item("college", "学院", "text"));
            cargs.items.Add(new CreateArgs.Item("major", "专业", "text"));
            cargs.items.Add(new CreateArgs.Item("number", "班级", "text"));
            cargs.items.Add(new CreateArgs.Item("as_instructor_set", "辅导员列表(工号)", "multitext"));
            cargs.items.Add(new CreateArgs.Item("as_student_set", "学生列表(学号)", "multitext"));

            var args = new ListArgs("api/classes/", User.Api, Title, Searchable: true, Createable: true, Createargs: cargs);
            args.columns.Add(new ListArgs.Column("id", "班级编号",
                hyperlink: ListArgs.autohyperlink("class-instance").add("id").lambda));
            args.columns.Add(new ListArgs.Column("grade", "年级", ordering: "grade"));
            args.columns.Add(new ListArgs.Column("college", "学院", ordering: "college"));
            args.columns.Add(new ListArgs.Column("major", "专业", ordering: "major"));
            args.columns.Add(new ListArgs.Column("number", "班级", ordering: "number"));

            template.Construct(args);
            ContentGrid.Children.Add(template);
        }
        private ListTemplate template;
        public override void NavigatedToEvent(object sender, IDictionary<string, object> kwargs) {
            base.NavigatedToEvent(sender, kwargs);
            template.UpdateData();
        }
    }
}
