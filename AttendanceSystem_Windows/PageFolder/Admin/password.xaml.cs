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
    public partial class password : BasePage {
        public password() {
            InitializeComponent();
            default_auth = new AuthSetting[] {
                new AuthSetting(Authority.UserManage)
            };
        }
        public override void NavigatedToEvent(object sender, IDictionary<string, object> kwargs) {
            base.NavigatedToEvent(sender, kwargs);
            UserText.ClearContent();
            UserName.ClearContent();
            PasswordText.ClearContent();
        }
        private string selected = null;
        private void UserText_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                SubmitQuery_Click(SubmitQuery, new RoutedEventArgs());
            }
        }

        private void SubmitQuery_Click(object sender, RoutedEventArgs e) {
            selected = UserText.Text.Trim();
            var res = User.Api["api/auth/users/"][selected].Retrieve();
            if (res.statuslike("2**")) {
                UserName.Text = res.instance["name"].ToString();
            }else {
                NavigatorPage.MsgSystem.Show(null, "错误", res.content);
                selected = null;
            }
        }

        private void PasswordText_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                SubmitPassword_Click(SubmitQuery, new RoutedEventArgs());
            }
        }

        private void SubmitPassword_Click(object sender, RoutedEventArgs e) {
            if (selected != null) {
                var jdata = $@"{{
                ""new_password"":""{PasswordText.Text}""
            }}";
                var res = User.Api["api/user/password-admin/"][selected].Update(jdata);
                if (res.statuslike("2**")) {
                    NavigatorPage.MsgSystem.Show(null, "提示", "修改成功。");
                } else NavigatorPage.MsgSystem.Show(null, "错误", res.content);
            }
        }
    }
}
