using AttendanceSystem_Windows.HttpConnection;
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
        public  MainWindow() {
            InitializeComponent();

            Navigator.Navigate(LoginPage.Get());
            //HttpConnect con = new HttpConnect("http://127.0.0.1:8000/api/auth/users.json", "GET",username:"10000",password:"furandouru");
            //con.args.Add("ordering", "-id");
            //con.Connect();

            //JArray ja = JArray.Parse(con.response);
            //txt.Text = ja.ToString();
        }
    }
}
