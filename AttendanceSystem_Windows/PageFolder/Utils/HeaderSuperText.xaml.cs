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
using AttendanceSystem_Windows.PageFolder;
using AttendanceSystem_Windows.Services;

namespace AttendanceSystem_Windows.PageFolder.Utils {
    /// <summary>
    /// HeaderSuperText.xaml 的交互逻辑
    /// </summary>
    public partial class HeaderSuperText : UserControl {
        public HeaderSuperText() {
            InitializeComponent();
            ColumnWidth = 2;
            Header = "";
            Text = "";
            Multiable = false;
            Editable = EditAbleStatus.Readonly;
            Editstatus = false;
            AllFontSize = 16;
            
            this.SizeChanged += (s, e) => { // 修改该控件的大小时，使edit按钮的宽高自适应。
                EditButton.Width = EditButton.Height;
            };
        }

        public HorizontalAlignment HeaderHorizontalAlignment {
            get { return TitleBlock.HorizontalAlignment; }
            set { TitleBlock.HorizontalAlignment = value; }
        }

        public double ColumnWidth {
            get { return columnwidth; }
            set {
                columnwidth = value;
                MiddleColumn.Width = new GridLength(columnwidth, GridUnitType.Star);
            }
        }
        private double columnwidth = 2;

        public string Header {
            get { return TitleBlock.Text; }
            set { TitleBlock.Text = value; }
        }

        public void ClearContent() {
            Text = "";
        }

        public bool Multiable {
            get { return multiable; }
            set {
                multiable = value;
                ContentText.TextWrapping = value ? TextWrapping.Wrap : TextWrapping.NoWrap;
                ContentText.AcceptsReturn = value;
                ContentText.VerticalScrollBarVisibility = value ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto;
                ContentText.VerticalAlignment = value ? VerticalAlignment.Stretch : VerticalAlignment.Center;
            }
        }
        private bool multiable = false;

        public string Text {
            get { return Editable == EditAbleStatus.Writeonly ? ContentText.Text : text; }
            set {
                text = value;
                ContentBlock.Text = value;
                ContentText.Text = value;
            }
        }
        private string text = "";

        public string[] MultiText {
            get {
                return Text.Trim().Length > 0 ? Text.Split('\n').map(s => s.Trim()) as string[] : new string[0];
            }
            set {
                Text = value?.join("\n") ?? "";
            }
        }

        public double AllFontSize {
            get { return allfontsize ; }
            set {
                TitleBlock.FontSize = value;
                ContentBlock.FontSize = value;
                ContentText.FontSize = value;
                allfontsize = value;
            }
        }
        private double allfontsize = 16;

        public EditAbleStatus Editable {
            get { return editable; }
            set {
                editable = value;
                Editstatus = Editstatus;
            }
        }
        private EditAbleStatus editable = EditAbleStatus.Readonly;

        public bool Editstatus {
            get { return editstatus; }
            set {
                editstatus = value;
                if (Editable == EditAbleStatus.Writeonly) {
                    ContentBlock.Visibility = Visibility.Hidden;
                    ContentText.Visibility = Visibility.Visible;
                    EditButton.Visibility = Visibility.Collapsed;
                    SaveButton.Visibility = Visibility.Collapsed;
                } else if (Editable == EditAbleStatus.Readonly) {
                    ContentBlock.Visibility = Visibility.Visible;
                    ContentText.Visibility = Visibility.Hidden;
                    EditButton.Visibility = Visibility.Collapsed;
                    SaveButton.Visibility = Visibility.Collapsed;
                } else {
                    ContentBlock.Visibility = !value ? Visibility.Visible : Visibility.Hidden;
                    ContentText.Visibility =value ? Visibility.Visible : Visibility.Hidden;
                    EditButton.Visibility = !value ? Visibility.Visible : Visibility.Collapsed;
                    SaveButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }
        private bool editstatus = false;

        public TextBlock Title => TitleBlock;
        public TextBlock ContentField => ContentBlock;
        public TextBox ContentEdit => ContentText;

        private void EditButton_Click(object sender, RoutedEventArgs e) {
            Editstatus = true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e) {
            Text = Submit?.Invoke(this, new SubmitEventArgs(ContentText.Text)) ?? ContentText.Text;
            Editstatus = false;
        }

        public delegate string SubmitEventHandler(object sender, SubmitEventArgs e);

        /// <summary>
        /// 提交更改时发生的事件。
        /// </summary>
        public event SubmitEventHandler Submit;

        public class SubmitEventArgs : EventArgs {
            public readonly string text;
            public SubmitEventArgs(string text) {
                this.text = text;
            }
        }

        public string toJsonString() {
            if (Multiable) {
                var sb = new StringBuilder();
                sb.Append("[");
                bool first = true;
                foreach(var i in MultiText) {
                    if (first) first = false;
                    else sb.Append(",");
                    sb.Append($@" ""{i}"" ");
                }
                sb.Append("]");
                return sb.ToString();
            } else return Text;
        }
    }
}
