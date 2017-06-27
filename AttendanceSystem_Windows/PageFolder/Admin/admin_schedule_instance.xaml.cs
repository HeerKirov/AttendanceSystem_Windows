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

namespace AttendanceSystem_Windows.PageFolder.Admin {
    /// <summary>
    /// admin_schedule_instance.xaml 的交互逻辑
    /// </summary>
    public partial class admin_schedule_instance : BasePage {
        public admin_schedule_instance() {
            InitializeComponent();
            //生成概要信息区域
            template = new InstanceTemplate();
            var args = new InstanceArgs("api/schedule/system-schedules/", null, User.Api, Title);
            args.margin = new Thickness(0, 10, 0, 10);
            args.items.Add(new InstanceArgs.Item("id", "编号", "text"));
            args.items.Add(new InstanceArgs.Item("year", "学年", "text", EditAbleStatus.Writeable));
            args.items.Add(new InstanceArgs.Item("term", "学期", "text", EditAbleStatus.Writeable));
            args.items.Add(new InstanceArgs.Item("begin", "开始时间", "text", EditAbleStatus.Writeable));
            args.items.Add(new InstanceArgs.Item("end", "结束时间", "text", EditAbleStatus.Writeable));
            template.Construct(args);
            ContentGrid.Children.Add(template);
            //生成列表区域
            list_temp = new ListTemplate();
            var clargs = new CreateArgs("api/schedule/system-schedule-items/", User.Api, "创建时间表项");
            clargs.items.Add(new CreateArgs.Item("no", "节次", "text"));
            clargs.items.Add(new CreateArgs.Item("begin", "上课时间", "text"));
            clargs.items.Add(new CreateArgs.Item("end", "下课时间", "text"));
            clargs.items.Add(new CreateArgs.Item("system_schedule", "系统时间表", "hidden", data: "system_schedule"));

            var largs = new ListArgs("api/schedule/system-schedule-items/", User.Api, "时间表项",Createable:true,Createargs:clargs);
            largs.columns.Add(new ListArgs.Column("id", "编号",customaction: UpdateScheduleItemInstance));
            largs.columns.Add(new ListArgs.Column("no", "节次", customaction: UpdateScheduleItemInstance));
            largs.columns.Add(new ListArgs.Column("begin", "上课时间"));
            largs.columns.Add(new ListArgs.Column("end", "下课时间"));

            list_temp.Construct(largs);
            ListGrid.Children.Add(list_temp);
            //生成项的详情区域
            instance_temp = new InstanceTemplate();
            var iargs = new InstanceArgs("api/schedule/system-schedule-items/", null, User.Api, "表项信息");
            iargs.items.Add(new InstanceArgs.Item("id", "编号", "text"));
            iargs.items.Add(new InstanceArgs.Item("no", "节次", "text",EditAbleStatus.Writeable));
            iargs.items.Add(new InstanceArgs.Item("begin", "上课时间", "text",EditAbleStatus.Writeable));
            iargs.items.Add(new InstanceArgs.Item("end", "下课时间", "text",EditAbleStatus.Writeable));
            iargs.deleteaction = () => {//当按下子项的删除按钮时
                InstanceGrid.Children.Clear();//仅仅是不显示该区域。
                list_temp.UpdateData();//刷新列表。
            };

            instance_temp.Construct(iargs);
        }

        private InstanceTemplate template;
        private ListTemplate list_temp;
        private InstanceTemplate instance_temp;
        public override void NavigatedToEvent(object sender, IDictionary<string, object> kwargs) {
            base.NavigatedToEvent(sender, kwargs);
            if (kwargs != null) {
                var ID = kwargs["id"] as string;
                template.Args.instance = ID;
                list_temp.Args.filter("system_schedule", ID);
                list_temp.Args.createargs.param["system_schedule"] = $@"""{ID}""";
            }
            template.UpdateData();
            list_temp.UpdateData();
            InstanceGrid.Children.Clear();
        }

        private void UpdateScheduleItemInstance(JToken j) {
            //执行切换表项详情项的命令。
            var ID = j["id"].ToString();
            instance_temp.Args.instance = ID;
            instance_temp.UpdateData();
            if (InstanceGrid.Children.Count <= 0) {
                InstanceGrid.Children.Add(instance_temp);
            }
        }
    }
}
