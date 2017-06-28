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

namespace AttendanceSystem_Windows.PageFolder.Student {
    /// <summary>
    /// self_attendances.xaml 的交互逻辑
    /// </summary>
    public partial class self_attendances : BasePage {
        public self_attendances() {
            InitializeComponent();
            default_auth = new AuthSetting[] {
                new AuthSetting(Authority.Student)
            };

            template = new ListTemplate();
            var args = new ListArgs("api/record/attendance-records/", User.Api, Title, Searchable: true);
            args.columns.Add(new ListArgs.Column("id", "编号"));
            args.columns.Add(new ListArgs.Column("date", "日期"));
            args.columns.Add(new ListArgs.Column("course_number", "节次"));
            args.columns.Add(new ListArgs.Column("status", "状态"));
            args.columns.Add(new ListArgs.Column("course_manage", "课程编号",
                hyperlink: ListArgs.autohyperlink("course-instance").add("id","course_manage").lambda));
            template.Construct(args);
            ContentGrid.Children.Add(template);
        }

        private ListTemplate template;
        public override void NavigatedToEvent(object sender, IDictionary<string, object> kwargs) {
            base.NavigatedToEvent(sender, kwargs);
            template.Args.filter("student__username", User.Get().authentication.username);
            template.UpdateData();
        }
    }
}
