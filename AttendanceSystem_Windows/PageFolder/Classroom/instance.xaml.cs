using AttendanceSystem_Windows.PageFolder.Template;
using AttendanceSystem_Windows.Services;
using Newtonsoft.Json.Linq;
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

namespace AttendanceSystem_Windows.PageFolder.Classroom {
    /// <summary>
    /// instance.xaml 的交互逻辑
    /// </summary>
    public partial class instance : BasePage {
        public instance() {
            InitializeComponent();

            template = new InstanceTemplate();
            var args = new InstanceArgs("api/classrooms/basic/",null,User.Api,Title,
                Margin:new Thickness(0,10,0,10));
            args.items.Add(new InstanceArgs.Item("id", "教室编号", "text"));
            args.items.Add(new InstanceArgs.Item("name", "教室名称", "text"));
            args.items.Add(new InstanceArgs.Item("size", "教室大小", "text"));
            template.Construct(args);
            ContentGrid.Children.Add(template);

            list_temp = new ListTemplate();
            var largs = new ListArgs("api/record/classroom-records/", User.Api, "教室使用记录", Searchable: true);
            largs.columns.Add(new ListArgs.Column("id", "编号"));
            largs.columns.Add(new ListArgs.Column("student_name_related", "学生姓名",
                hyperlink:ListArgs.autohyperlink("student-other-document").add("id","student").lambda));
            largs.columns.Add(new ListArgs.Column("time_in", "进入时间"));
            largs.columns.Add(new ListArgs.Column("time_out", "离开时间"));
            list_temp.Construct(largs);
            //ListGrid.Children.Add(list_temp);
        }
        private string ID = null;
        private InstanceTemplate template;
        private ListTemplate list_temp;

        public override void NavigatedToEvent(object sender, IDictionary<string, object> kwargs) {
            base.NavigatedToEvent(sender, kwargs);
            if (kwargs != null) ID = kwargs["id"] as string;
            if (ID != null) {
                template.Args.instance = ID;
                template.UpdateData();
                //更新按钮。
                template.Args.custombutton = new CustomButton[0];
                if (User.Get().HasAuthorityOrRoot(Authority.ClassroomManage)) {
                    template.Args.custombutton = template.Args.custombutton.append(new CustomButton("", () => {//[pencil]
                        NavigatorPage.NavigatorGoto("classroom-modify", new Dictionary<string, object>() { { "id", ID } });
                    }, Tips: "修改"));
                    list_temp.Args.filter("classroom_manage", ID);
                    //如果是管理者，那么还可以查看到教室使用记录。
                    list_temp.UpdateData();
                    if (ListGrid.Children.Count <= 0) ListGrid.Children.Add(list_temp);
                } else if (ListGrid.Children.Count > 0) ListGrid.Children.Clear();
                template.ConstructCustomButton();

            }
        }
    }
}
