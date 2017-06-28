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
    /// <summary>
    /// user_list.xaml 的交互逻辑
    /// </summary>
    public partial class class_list : BasePage {
        public class_list() {
            InitializeComponent();
            default_auth = new AuthSetting[] {
                new AuthSetting(Authority.Office),
                new AuthSetting(Authority.Instructor)
            };
            template = new ListTemplate();

            var args = new ListArgs("api/classes/", User.Api, Title, Searchable: true);
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
            if (User.Get().HasAuthorityOrRoot(Authority.Instructor)) {
                template.Args.filter("as_instructor_set__username", User.Get().authentication.username);
            } else template.Args.getFilter().Clear();
            template.UpdateData();
        }
    }
}
