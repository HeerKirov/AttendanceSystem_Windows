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
    /// HeaderSuperChoice.xaml 的交互逻辑
    /// </summary>
    public partial class HeaderSuperChoice : UserControl {
        public HeaderSuperChoice() {
            InitializeComponent();
            ColumnWidth = 2;
            Header = "";
            Value = "";
            Editable = EditAbleStatus.Readonly;
            Editstatus = false;
            AllFontSize = 16;

            SizeChanged += (s, e) => { // 修改该控件的大小时，使edit按钮的宽高自适应。
                EditButton.Width = EditButton.Height;
            };
        }

        public KeyValuePair<string, string>[] Choices {
            get { return choices; }
            set {
                choices = value;
                if (choices != null) {
                    contentlist = new string[choices.Length];
                    for (int i = 0; i < choices.Length; ++i) contentlist[i] = choices[i].Value;
                    ContentCombo.ItemsSource = contentlist;
                }else {
                    contentlist = null;
                    ContentCombo.ItemsSource = null;
                }
            }
        }
        private KeyValuePair<string, string>[] choices;
        private string[] contentlist;

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

        public void ClearContent() {
            SelectedIndex = -1;
        }

        public string Header {
            get { return TitleBlock.Text; }
            set { TitleBlock.Text = value; }
        }
        /// <summary>
        /// 当前正选定的项目的索引。
        /// </summary>
        public int SelectedIndex {
            get { return Editable == EditAbleStatus.Writeonly ? ContentCombo.SelectedIndex : selectedindex; }
            set {
                ContentCombo.SelectedIndex = value;
                ContentBlock.Text = value >= 0 ? contentlist[value] : null;
                selectedindex = value;
            }
        }
        private int selectedindex;
        /// <summary>
        /// 正在使用的项目的程序名。
        /// </summary>
        public string Key {
            get { return SelectedIndex >= 0 ? Choices[SelectedIndex].Key : null; }
            set {
                var i = Choices.indexOf(p => p.Key == value);
                SelectedIndex = i;
            }
        }
        /// <summary>
        /// 正在使用的项目的显示名称。
        /// </summary>
        public string Value {
            get { return SelectedIndex >= 0 ? Choices[SelectedIndex].Value : null; }
            set {
                var i = Choices.indexOf(p => p.Value == value);
                SelectedIndex = i;
            }
        }

        public double AllFontSize {
            get { return allfontsize; }
            set {
                TitleBlock.FontSize = value;
                ContentBlock.FontSize = value;
                ContentCombo.FontSize = value;
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
                    ContentCombo.Visibility = Visibility.Visible;
                    EditButton.Visibility = Visibility.Collapsed;
                    SaveButton.Visibility = Visibility.Collapsed;
                } else if (Editable == EditAbleStatus.Readonly) {
                    ContentBlock.Visibility = Visibility.Visible;
                    ContentCombo.Visibility = Visibility.Hidden;
                    EditButton.Visibility = Visibility.Collapsed;
                    SaveButton.Visibility = Visibility.Collapsed;
                } else {
                    ContentBlock.Visibility = !value ? Visibility.Visible : Visibility.Hidden;
                    ContentCombo.Visibility = value ? Visibility.Visible : Visibility.Hidden;
                    EditButton.Visibility = !value ? Visibility.Visible : Visibility.Collapsed;
                    SaveButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }
        private bool editstatus = false;

        public TextBlock Title => TitleBlock;
        public TextBlock ContentField => ContentBlock;
        public ComboBox ContentEdit => ContentCombo;

        private void EditButton_Click(object sender, RoutedEventArgs e) {
            Editstatus = true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e) {
            SelectedIndex = Submit?.Invoke(this, new SubmitEventArgs(ContentCombo.SelectedIndex,
                ContentCombo.SelectedIndex>=0?Choices[ContentCombo.SelectedIndex].Key:null,
                ContentCombo.SelectedIndex>=0?Choices[ContentCombo.SelectedIndex].Value:null
                )) ?? ContentCombo.SelectedIndex;
            Editstatus = false;
        }

        public delegate int SubmitEventHandler(object sender, SubmitEventArgs e);

        /// <summary>
        /// 提交更改时发生的事件。
        /// </summary>
        public event SubmitEventHandler Submit;

        public class SubmitEventArgs : EventArgs {
            public readonly int selected;
            public readonly string key, value;
            public SubmitEventArgs(int selected, string key, string value) {
                this.selected = selected;
                this.key = key;
                this.value = value;
            }
        }

    }
}
