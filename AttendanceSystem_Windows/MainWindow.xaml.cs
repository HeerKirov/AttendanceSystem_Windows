using AttendanceSystem_Windows.HttpConnection;
using AttendanceSystem_Windows.LocalData;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        public static MainWindow reference { get; private set; }
        public  MainWindow() {
            InitializeComponent();
            reference = this;
            Navigator.Navigate(LoginPage.Get());
        }
        /// <summary>
        /// 给出次级标题，并设置窗口标题。
        /// </summary>
        /// <param name="subtitle"></param>
        public void setTitle(string subtitle) {
            Title = "Attendance System Development0.001" + (subtitle != null ? " - " : "") + subtitle;
        }
    }
}
