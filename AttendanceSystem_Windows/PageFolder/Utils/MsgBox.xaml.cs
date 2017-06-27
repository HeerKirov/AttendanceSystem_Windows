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

namespace AttendanceSystem_Windows.PageFolder.Utils {
    /// <summary>
    /// MsgBox.xaml 的交互逻辑
    /// </summary>
    public partial class MsgBox : UserControl {
        public MsgBox() {
            InitializeComponent();
            Title = "";
            Text = "";
            Back = Visibility.Visible;
            SetButtons(null);
            btns = new Button[] { Btn1, Btn2, Btn3 };
            for(int i = 0; i < btns.Length; ++i) {
                var package_i = i;
                btns[i].Click += (s, e) => {
                    if (Visibility == Visibility.Visible) {
                        var args = new MsgBoxEventArgs(package_i, btns[package_i].Content.ToString());
                        MsgBoxTrigger?.Invoke(this, args);
                        foreach(var func in OnceEvent) {
                            func?.Invoke(this, args);
                        }
                        OnceEvent.Clear();
                        Dispatcher.Invoke(() => Visibility = Visibility.Collapsed);//注意线程同步的问题。
                    }
                };
            }
        }
        public void Show(MsgBoxEvent func = null, string title = "", string contenttext = "", Visibility? back = null, string[] buttonname = null) {
            Dispatcher.Invoke(() => {
                if (title != null) Title = title;
                if (contenttext != null) Text = contenttext;
                if (back.HasValue) Back = back.Value;
                if (buttonname != null) SetButtons(buttonname);
                Visibility = Visibility.Visible;
            });
            OnceEvent.Add(func);
        }

        private Button[] btns;
        /// <summary>
        /// 是否正处于使用状态。
        /// </summary>
        public bool BeingUsed => Visibility == Visibility.Visible;

        public string Title {
            get { return TitleBox.Text; }
            set { TitleBox.Text = value; }
        }
        public string Text {
            get { return ContentBox.Text; }
            set { ContentBox.Text = value; }
        }
        public Visibility Back {
            get { return BackGrid.Visibility; }
            set { BackGrid.Visibility = value; }
        }
        public MsgBox SetOkButton() {
            return SetButtons(new string[] { "确定" });
        }
        public MsgBox SetOkCancelButton() {
            return SetButtons(new string[] { "确定", "取消" });
        }
        public MsgBox SetYesNoButton() {
            return SetButtons(new string[] { "是", "否" });
        }
        public MsgBox SetYesNoCancelButton() {
            return SetButtons(new string[] { "是", "否", "取消" });
        }
        public MsgBox SetButtons(string[] name) {
            if (name == null) name = new string[] { "确定" };
            Btn2.Visibility = name.Length >= 2 ? Visibility.Visible : Visibility.Collapsed;
            Btn3.Visibility = name.Length >= 3 ? Visibility.Visible : Visibility.Collapsed;
            Btn1.Content = name[0];
            Btn2.Content = name.Length >= 2 ? name[1] : null;
            Btn3.Content = name.Length >= 3 ? name[2] : null;
            return this;
        }
        public MsgBox AddOnceEvent(MsgBoxEvent func){
            OnceEvent.Add(func);
            return this;
        }

        /// <summary>
        /// 该消息框触发了按钮事件时的事件。
        /// </summary>
        public event MsgBoxEvent MsgBoxTrigger;
        public delegate void MsgBoxEvent(object sender, MsgBoxEventArgs e);
        public class MsgBoxEventArgs {
            public readonly int CheckIndex;
            public readonly string CheckText;
            public MsgBoxEventArgs(int index,string text) {
                CheckIndex = index;
                CheckText = text;
            }
        }
        /// <summary>
        /// 一次性使用的事件。
        /// </summary>
        private HashSet<MsgBoxEvent> OnceEvent = new HashSet<MsgBoxEvent>();

    }

    
}
