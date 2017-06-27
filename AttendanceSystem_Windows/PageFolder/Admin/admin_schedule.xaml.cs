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
    /// admin_schedule.xaml 的交互逻辑
    /// </summary>
    public partial class admin_schedule : BasePage {
        public admin_schedule() {
            InitializeComponent();

            default_auth = new AuthSetting[] {
                new AuthSetting(Authority.Admin)
            };
            template = new ListTemplate();

            var cargs = new CreateArgs("api/schedule/system-schedules/", User.Api, "创建时间表", Margin: new Thickness(0, 10, 0, 10));
            cargs.items.Add(new CreateArgs.Item("year", "学年", "text"));
            cargs.items.Add(new CreateArgs.Item("term", "学期", "text"));
            cargs.items.Add(new CreateArgs.Item("begin", "开始时间", "text"));
            cargs.items.Add(new CreateArgs.Item("end", "结束时间", "text"));


            var args = new ListArgs("api/schedule/system-schedules/", User.Api, Title, Searchable: true, Createable: true, Createargs: cargs);
            args.columns.Add(new ListArgs.Column("id", "编号", ordering: "id",
                hyperlink: ListArgs.autohyperlink("admin-admin-schedule-instance").add("id").lambda));
            args.columns.Add(new ListArgs.Column("year", "学年"));
            args.columns.Add(new ListArgs.Column("term", "学期"));
            args.columns.Add(new ListArgs.Column("begin", "开始时间"));
            args.columns.Add(new ListArgs.Column("end", "结束时间"));

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
