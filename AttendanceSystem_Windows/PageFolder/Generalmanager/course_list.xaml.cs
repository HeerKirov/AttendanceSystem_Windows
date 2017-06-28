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

namespace AttendanceSystem_Windows.PageFolder.Generalmanager {
    public partial class course_list : BasePage {
        public course_list() {
            InitializeComponent();
            default_auth = new AuthSetting[] {
                new AuthSetting(Authority.Teacher),
                new AuthSetting(Authority.Office),
            };
            template = new ListTemplate();

            var args = new ListArgs("api/courses/basic/", User.Api, Title, Searchable: true);
            args.columns.Add(new ListArgs.Column("id", "课程编号", ordering: "id",
                hyperlink: ListArgs.autohyperlink("course-instance").add("id").lambda));
            args.columns.Add(new ListArgs.Column("name", "课程名称", ordering: "name",
                hyperlink: ListArgs.autohyperlink("course-instance").add("id").lambda));
            args.columns.Add(new ListArgs.Column("teacher", "任课教师", ordering: "teacher",
                hyperlink: ListArgs.autohyperlink("teacher-other-document").add("id", "teacher").lambda));

            template.Construct(args);
            ContentGrid.Children.Add(template);
        }
        private ListTemplate template;
        public override void NavigatedToEvent(object sender, IDictionary<string, object> kwargs) {
            base.NavigatedToEvent(sender, kwargs);
            if (User.Get().HasAuthorityOrRoot(Authority.Teacher)) {
                template.Args.filter("teacher__username", User.Get().authentication.username);
            } else template.Args.getFilter().Clear();
            template.UpdateData();
        }
    }
}
