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
    public partial class classroom_list : BasePage {
        public classroom_list() {
            InitializeComponent();
            default_auth = new AuthSetting[] {
                new AuthSetting(Authority.ClassroomManage)
            };
            template = new ListTemplate();

            var cargs = new CreateArgs("api/classrooms/basic/", User.Api, "创建教室", Margin: new Thickness(0, 10, 0, 10));
            cargs.items.Add(new CreateArgs.Item("id", "教室编号", "text"));
            cargs.items.Add(new CreateArgs.Item("name", "教室名称", "text"));
            cargs.items.Add(new CreateArgs.Item("size", "教室大小", "text"));
            cargs.items.Add(new CreateArgs.Item("password", "教室密码", "text"));

            var args = new ListArgs("api/classrooms/basic/", User.Api, Title, Searchable: true, Createable:true, Createargs:cargs);
            args.columns.Add(new ListArgs.Column("id", "教室编号",
                hyperlink: ListArgs.autohyperlink("classroom-instance").add("id").lambda));
            args.columns.Add(new ListArgs.Column("name", "教室名称", ordering: "name",
                hyperlink: ListArgs.autohyperlink("classroom-instance").add("id").lambda));
            args.columns.Add(new ListArgs.Column("size", "教室大小", ordering: "size"));

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
