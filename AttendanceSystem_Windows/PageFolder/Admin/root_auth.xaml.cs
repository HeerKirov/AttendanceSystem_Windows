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
    /// root_auth.xaml 的交互逻辑
    /// </summary>
    public partial class root_auth : BasePage {
        public root_auth() {
            InitializeComponent();
            UserAuthChoice.ItemSource = UserAuth_Choices.ToArray();
            ManageAuthChoice.ItemSource = ManageAuth_Choices.ToArray();
        }
        private Dictionary<string, string> UserAuth_Choices = new Dictionary<string, string>() {
            { "Student","学生"},
            { "Teacher","教师"},
            { "Instructor","辅导员"},
            { "Office","教务处"},
            { "Admin","管理员"}
        };
        private Dictionary<string, string> ManageAuth_Choices = new Dictionary<string, string>() {
            { "Root","超级管理员"},
            { "UserManager","用户管理员"},
            { "SutdentManager","学生管理员"},
            { "TeacherManager","教师管理员"},
            { "InstructorManager","辅导员管理员" },
            { "CourseManager","课程管理员"},
            { "ClassroomManager","教室管理员"},
            { "ClassManager","班级管理员"},
        };

        private void SubmitQuery_Click(object sender, RoutedEventArgs e) {
            selected = UserText.Text;
            var res = User.Api["api/user/auth/"][selected].Retrieve();
            if (res.statuslike("2**")) {
                try {
                    var inst = res.instance;
                    var auth_number = long.Parse(inst["auth"].ToString());
                    UserName.Text = inst["name"].ToString();
                    var auths = AuthorityItem.getAuthorityArray(auth_number).map(i => i.ToString()) as string[];
                    UserAuthChoice.setByItem(auths);
                    ManageAuthChoice.setByItem(auths);
                } catch(Exception exception) {
                    NavigatorPage.MsgSystem.Show(null, "应用程序错误", exception.ToString());
                }
            }else {
                NavigatorPage.MsgSystem.Show(null, "错误", res.content);
            }
        }

        private void SubmitChange_Click(object sender, RoutedEventArgs e) {
            //构造jdata
            var auths = UserAuthChoice.toArray().Concat(ManageAuthChoice.toArray()).map(i => (Authority)Enum.Parse(typeof(Authority), i)) as Authority[];
            var auth_number = AuthorityItem.getAuthorityNumber(auths);
            var data = $@"{{
                ""auth"":""${auth_number}""
            }}";
            //发送请求
            var res = User.Api["api/user/auth"][selected].Update(data);
            if (res.statuslike("2**")) {
                NavigatorPage.MsgSystem.Show(null, "提示", "提交成功。");
            } else {
                NavigatorPage.MsgSystem.Show(null, "错误", res.content);
            }
        }
        private string selected;
    }
}
