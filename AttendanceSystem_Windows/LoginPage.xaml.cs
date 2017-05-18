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
using System.Net;
using AttendanceSystem_Windows.HttpConnection;
using AttendanceSystem_Windows.LocalData;
using AttendanceSystem_Windows.Services;
using Newtonsoft.Json.Linq;
using EasyHttp.Http;

namespace AttendanceSystem_Windows {
    /// <summary>
    /// LoginPage.xaml 的交互逻辑
    /// </summary>
    public partial class LoginPage : Page {
        public static LoginPage Get() => reference == null ? reference = new LoginPage() : reference;
        private static LoginPage reference = null;

        private LoginPage() {
            InitializeComponent();
            CheckConnection();
            
        }
        /// <summary>
        /// 检执行UI操作，查对服务器的链接。
        /// </summary>
        public void CheckConnection() {
            SetContent(ContentStatus.Connecting);
            Task.Run(() => {
                OptionClass option = OptionClass.Get();
                HttpConnect con = new HttpConnect(option.GetURL(""), "GET");
                con.Connect();
                var code = con.status;
                Content.Dispatcher.Invoke(() => {
                    if (code == HttpStatusCode.OK) {
                        SetContent(ContentStatus.Login);
                    } else {
                        SetContent(ContentStatus.Disconnect);
                    }
                });
            });
        }
        /// <summary>
        /// 登陆指令。
        /// </summary>
        public void DoLogin() {
            SetLoginOperate(true);
            var username = UsernameTxt.Text;
            var password = PasswordTxt.Password;
            Task.Run(() => {
                OptionClass option = OptionClass.Get();
                JObject loginjson = new JObject();
                loginjson.Add("username", new JValue(username));
                loginjson.Add("password", new JValue(password));
                HttpConnect con = new HttpConnect(option.GetURL(""), "POST", username: username, password: password,data:loginjson.ToString());
                con.Connect();
                var code = con.status;
                Content.Dispatcher.Invoke(() => {
                    if (code == HttpStatusCode.OK) {
                        //登陆成功
                        User user = User.Get();
                        user.authentication = new Auth(con.authentication.username, con.authentication.password);
                        NavigateToMain();
                    }else if (code == HttpStatusCode.Unauthorized) {
                        SetNotice("非法账户或者错误的登录信息");
                        SetLoginOperate(false);
                    }else {
                        SetNotice(con.response);
                        SetLoginOperate(false);
                    }
                });
            });
        }

        public void SetNotice(string text) {
            if (text == null || text == "") {
                NoticeText.Visibility = Visibility.Collapsed;
            }else {
                NoticeText.Text = text;
                NoticeText.Visibility = Visibility.Visible;
            }
        }

        public void SetLoginOperate(bool logining) {
            LoginBtn.IsEnabled = !logining;
            LoginBtn.Content = logining ? "登陆中……" : "登录";
            UsernameTxt.IsEnabled = PasswordTxt.IsEnabled = !logining;
        }

        public void SetContent(ContentStatus status) {
            LoginContent.Visibility = status == ContentStatus.Login ? Visibility.Visible : Visibility.Collapsed;
            ConnectingContent.Visibility = status == ContentStatus.Connecting ? Visibility.Visible : Visibility.Collapsed;
            DisconnectContent.Visibility = status == ContentStatus.Disconnect ? Visibility.Visible : Visibility.Collapsed;
        }

        private void NavigateToMain() {
            NavigationService.Navigate(NavigatorPage.Get());
        }

        public enum ContentStatus {
            Connecting,
            Login,
            Disconnect
        }


        /// <summary>
        /// 重试连接服务器指令。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RetryToConnectBtn_Click(object sender, RoutedEventArgs e) {
            CheckConnection();
        }
        /// <summary>
        /// 发起登陆指令。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginBtn_Click(object sender, RoutedEventArgs e) {
            DoLogin();
        }

    }
}
