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

namespace AttendanceSystem_Windows.PageFolder.Course {
    /// <summary>
    /// instance.xaml 的交互逻辑
    /// </summary>
    public partial class instance : BasePage {
        public instance() {
            InitializeComponent();

            template = new InstanceTemplate();
            var args = new InstanceArgs("api/courses/basic/",null,User.Api,Title,
                Margin:new Thickness(0,10,0,10));
            args.items.Add(new InstanceArgs.Item("id", "课程编号", "text"));
            args.items.Add(new InstanceArgs.Item("name", "课程名称", "text"));
            args.items.Add(new InstanceArgs.Item("teacher_name_related", "教师", "text"));
            template.Construct(args);
            ContentGrid.Children.Add(template);

            list_temp = new ListTemplate();
            var largs = new ListArgs(null, User.Api, "学生列表");
            largs.columns.Add(new ListArgs.Column("id", "学号", customaction:HyperlinkToStudent));
            largs.columns.Add(new ListArgs.Column("name", "姓名", customaction: HyperlinkToStudent));
            list_temp.Construct(largs);
            ListGrid.Children.Add(list_temp);
        }
        private string ID = null;
        private InstanceTemplate template;
        private ListTemplate list_temp;

        public override bool NavigatingToEvent(object sender, IDictionary<string, object> kwargs) {
            if (kwargs == null && ID == null) return false;
            if (kwargs != null) {
                ID = kwargs["id"] as string;
                default_auth = new AuthSetting[] {
                    new AuthSetting(BelongRelation.Sub,"course",ID),
                    new AuthSetting(BelongRelation.Parent,"course",ID),
                    new AuthSetting(Authority.CourseManage),
                };
            }
            return base.NavigatingToEvent(sender, kwargs);
        }
        public override void NavigatedToEvent(object sender, IDictionary<string, object> kwargs) {
            base.NavigatedToEvent(sender, kwargs);
            if (ID != null) {
                template.Args.instance = ID;
                template.UpdateData();
                //更新按钮。
                template.Args.custombutton = new CustomButton[0];
                if (User.Get().HasAuthorityOrRoot(Authority.CourseManage)) {
                    template.Args.custombutton = template.Args.custombutton.append(new CustomButton("", () => {//[pencil]
                        NavigatorPage.NavigatorGoto("course-modify", new Dictionary<string, object>() { { "id", ID } });
                    }, Tips: "修改"));
                }
                if (User.Get().BelongQuery("course", ID) == BelongRelation.Parent ||
                    User.Get().HasAuthorityOrRoot(Authority.CourseManage)) {
                    template.Args.custombutton = template.Args.custombutton.append(new CustomButton("", () => {//[people]
                        NavigatorPage.NavigatorGoto("course-detail", new Dictionary<string, object>() { { "id", ID } });
                    }, Tips: "出勤记录"));
                }
                template.ConstructCustomButton();

                //更新学生列表。
                var res = User.Api["api/courses/basic/"][ID].Retrieve();
                if (res.statuslike("2**")) {
                    var data = res.instance;
                    //构建有关学生的列表。
                    var datas = new JArray();
                    var idset = data["as_student_set"].map(j => j.ToString()) as string[];
                    var nameset = data["student_name_related"].map(j => j.ToString()) as string[];
                    for(int i = 0; i < idset.Length && i < nameset.Length; ++i) {
                        var jo = new JObject();
                        jo["id"] = new JValue(idset[i]);
                        jo["name"] = new JValue(nameset[i]);
                        datas.Add(jo);
                    }
                    list_temp.UpdateData(datas);
                }
            }
        }

        private void HyperlinkToStudent(JToken j) {
            NavigatorPage.NavigatorGoto("student-other-document", new Dictionary<string, object>() {
                { "id", j["id"].ToString() }
            });
        }
    }
}
