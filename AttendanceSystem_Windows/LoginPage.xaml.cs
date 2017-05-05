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

namespace AttendanceSystem_Windows {
    /// <summary>
    /// LoginPage.xaml 的交互逻辑
    /// </summary>
    public partial class LoginPage : Page {
        public static LoginPage Get() => reference == null ? reference = new LoginPage() : reference;
        private static LoginPage reference = null;

        private LoginPage() {
            InitializeComponent();
            SetContent(ContentStatus.Login);
        }

        public void SetNotice(string text) {
            if (text == null || text == "") {
                NoticeText.Visibility = Visibility.Collapsed;
            }else {
                NoticeText.Text = text;
                NoticeText.Visibility = Visibility.Visible;
            }
        }

        public void SetContent(ContentStatus status) {
            LoginContent.Visibility = status == ContentStatus.Login ? Visibility.Visible : Visibility.Collapsed;
            ConnectingContent.Visibility = status == ContentStatus.Connecting ? Visibility.Visible : Visibility.Collapsed;
            DisconnectContent.Visibility = status == ContentStatus.Disconnect ? Visibility.Visible : Visibility.Collapsed;
        }

        public enum ContentStatus {
            Connecting,
            Login,
            Disconnect
        }
    }
}
