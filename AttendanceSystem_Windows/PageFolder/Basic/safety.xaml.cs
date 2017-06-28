using AttendanceSystem_Windows.HttpConnection;
using AttendanceSystem_Windows.LocalData;
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

namespace AttendanceSystem_Windows.PageFolder.Basic {

    public partial class safety : BasePage {
        public safety() {
            InitializeComponent();
            ServerIP.Submit += (s, e) => {
                option.ServerURL = e.text;
                option.WriteToConfig();
                NavigatorPage.MsgSystem.Show((w,r)=> {
                    User.Get().authentication = null;
                    MainWindow.reference.setContent(LoginPage.Get());
                }, "提示", "服务器地址已经修改。即将被弹出登录。");
                return e.text;
            };
        }

        OptionClass option;

        public override void NavigatedToEvent(object sender, IDictionary<string, object> kwargs) {
            base.NavigatedToEvent(sender, kwargs);
            option = OptionClass.Get();
            RPW.IsChecked = option.RememberPasswd;
            AL.IsChecked = option.AutoLogin;
            ServerIP.Text = option.ServerURL;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            //提交密码修改。
            if (CheckPassword.Text.Trim() != NewPassword.Text.Trim()) {
                NavigatorPage.MsgSystem.Show(null, "错误", "两次输入的新密码不一致。");
            }else {
                var jdata = new JObject();
                jdata["old_password"] = new JValue(OldPassword.Text.Trim());
                jdata["new_password"] = new JValue(NewPassword.Text.Trim());
                RestApi api = User.Api;
                var res = api["api/user/password/"][User.Get().authentication.username].Update(jdata.ToString());
                if (res.statuslike("2**")) {
                    NavigatorPage.MsgSystem.Show((s, r) => {
                        User.Get().authentication = null;
                        MainWindow.reference.setContent(LoginPage.Get());
                    }, "成功", "成功修改为新密码。您即将被弹出登录。");
                }else if (res.statuslike("400")) {
                    NavigatorPage.MsgSystem.Show(null, "错误", "错误的验证信息。您也许输入了错误的旧密码。");
                }else {
                    NavigatorPage.MsgSystem.Show(null, "错误", res.content);
                }
            }
            
        }

        private void RPW_Checked(object sender, RoutedEventArgs e) {
            option.RememberPasswd = RPW.IsChecked ?? false;
            option.WriteToConfig();
        }

        private void AL_Checked(object sender, RoutedEventArgs e) {
            option.AutoLogin = AL.IsChecked ?? false;
            option.WriteToConfig();
        }
    }
}
