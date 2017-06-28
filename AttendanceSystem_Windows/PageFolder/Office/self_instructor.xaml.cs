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

namespace AttendanceSystem_Windows.PageFolder.Office {

    public partial class self_instructor : BasePage {
        public self_instructor() {
            InitializeComponent();
            default_auth = new AuthSetting[] {
                new AuthSetting(Authority.Office)
            };
            template = new ListTemplate();

            var args = new ListArgs("api/auth/instructors/", User.Api, Title, Searchable: true);
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
