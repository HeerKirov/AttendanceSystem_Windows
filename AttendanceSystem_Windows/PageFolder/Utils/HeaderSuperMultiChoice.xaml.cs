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
    /// HeaderSuperMultiChoice.xaml 的交互逻辑
    /// </summary>
    public partial class HeaderSuperMultiChoice : UserControl {
        public HeaderSuperMultiChoice() {
            InitializeComponent();
            ColumnWidth = 2;
            Header = "";
            Editable = EditAbleStatus.Readonly;
            Editstatus = false;
            AllFontSize = 16;

            SizeChanged += (s, e) => { // 修改该控件的大小时，使edit按钮的宽高自适应。
                EditButton.Width = EditButton.Height;
            };
        }

        public void ClearContent() {
            SetSelects(new bool[itemsource?.Length??0]);
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

        public EditAbleStatus Editable {
            get { return editable; }
            set {
                editable = value;
                Editstatus = Editstatus;
            }
        }
        private EditAbleStatus editable = EditAbleStatus.Readonly;

        public double AllFontSize {
            get { return allfontsize; }
            set {
                TitleBlock.FontSize = value;
                for(int i = 0; i < (readcontents?.Length??0) ; ++i) {
                    readcontents[i].FontSize = value;
                    writecontents[i].FontSize = value;
                }
                allfontsize = value;
            }
        }
        private double allfontsize = 16;

        public bool Editstatus {
            get { return editstatus; }
            set {
                editstatus = value;
                if (Editable == EditAbleStatus.Writeonly) {
                    ReadScroll.Visibility = Visibility.Hidden;
                    WriteScroll.Visibility = Visibility.Visible;
                    EditButton.Visibility = Visibility.Collapsed;
                    SaveButton.Visibility = Visibility.Collapsed;
                } else if (Editable == EditAbleStatus.Readonly) {
                    ReadScroll.Visibility = Visibility.Visible;
                    WriteScroll.Visibility = Visibility.Hidden;
                    EditButton.Visibility = Visibility.Collapsed;
                    SaveButton.Visibility = Visibility.Collapsed;
                } else {
                    ReadScroll.Visibility = !value ? Visibility.Visible : Visibility.Hidden;
                    WriteScroll.Visibility = value ? Visibility.Visible : Visibility.Hidden;
                    EditButton.Visibility = !value ? Visibility.Visible : Visibility.Collapsed;
                    SaveButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }
        private bool editstatus = false;

        public delegate bool[] SubmitEventHandler(object sender, SubmitEventArgs e);

        /// <summary>
        /// 提交更改时发生的事件。
        /// </summary>
        public event SubmitEventHandler Submit;

        public class SubmitEventArgs : EventArgs {
            public readonly bool[] value;
            public SubmitEventArgs(bool[] value) {
                this.value = value;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e) {
            Editstatus = true;
        }
        /// <summary>
        /// 提交更改。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e) {
            //提交更改会发送包含被选择内容的布尔列表。需要回执一个列表表示实际内容。
            bool[] newselected = new bool[selected.Length];
            for (int i = 0; i < newselected.Length; ++i)
                newselected[i] = writecontents[i].IsChecked ?? false;
            SetSelects(Submit?.Invoke(this, new SubmitEventArgs(newselected)));
            Editstatus = false;
        }

        //下面是内容控制模块。
        /// <summary>
        /// 控制能够显示的内容。设置此项目将重置选择项。
        /// </summary>
        public KeyValuePair<string, string>[] ItemSource {
            get { return itemsource; }
            set {
                ClearContent();
                ReadContent.Children.Clear();
                WriteContent.Children.Clear();
                if (value != null) {
                    itemsource = value;
                    selected = new bool[value.Length];
                    readcontents = new TextBlock[value.Length];
                    writecontents = new CheckBox[value.Length];
                    for (int i = 0; i < value.Length; ++i) {
                        selected[i] = false;
                        readcontents[i] = new TextBlock();
                        readcontents[i].Text = value[i].Value;
                        readcontents[i].FontSize = AllFontSize;
                        readcontents[i].Visibility = Visibility.Collapsed;
                        ReadContent.Children.Add(readcontents[i]);
                        writecontents[i] = new CheckBox();
                        writecontents[i].Content = value[i].Value;
                        writecontents[i].FontSize = AllFontSize;
                        writecontents[i].IsChecked = false;
                        WriteContent.Children.Add(writecontents[i]);
                    }
                }else {
                    selected = new bool[0];
                    readcontents = new TextBlock[0];
                    writecontents = new CheckBox[0];
                }
            }
        }
        private KeyValuePair<string, string>[] itemsource;
        public bool setSelected(int index,bool value) {
            if (index < 0 || index >= selected.Length) throw new IndexOutOfRangeException();
            selected[index] = value;
            //无论在什么模式下，执行这个操作都会刷新readcontent的显示和writecontent中对应checkbox的显示。
            readcontents[index].Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            writecontents[index].IsChecked = value;
            return selected[index];
        }
        /// <summary>
        /// 批量地设置显示状态。
        /// </summary>
        /// <param name="value"></param>
        public void SetSelects(bool[] value) {
            if (value != null) {
                for(int i = 0; i < value.Length && i < selected.Length; ++i) {
                    selected[i] = value[i];
                    readcontents[i].Visibility = value[i] ? Visibility.Visible : Visibility.Collapsed;
                    writecontents[i].IsChecked = value[i];
                }
            }
        }
        public bool getSelected(int index) {
            if (index < 0 || index >= selected.Length) throw new IndexOutOfRangeException();
            return Editable == EditAbleStatus.Writeonly ? (writecontents[index].IsChecked ?? false) : selected[index];
        }
        private bool[] selected;
        /// <summary>
        /// 转换为json数据格式输出。
        /// </summary>
        /// <returns></returns>
        public string toJsonString() {
            StringBuilder sb = new StringBuilder("{");
            bool first = true;
            for(int i = 0; i < itemsource.Length; ++i) {
                if (getSelected(i)) {
                    if (first) first = false;
                    else sb.Append(",");
                    sb.Append($@" ""${itemsource[i].Key}"" ");
                }
            }
            sb.Append("}");
            return sb.ToString();
        }
        /// <summary>
        /// 按照拥有项目格式的数据设置内容。
        /// </summary>
        /// <param name="json"></param>
        public void setByItem(string[] item) {
            ClearContent();
            foreach(var it in item) {
                for(int i = 0; i < itemsource.Length; ++i) {
                    if (itemsource[i].Key == it) {
                        setSelected(i, true);
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// 将选择的结果输出为数组。
        /// </summary>
        /// <returns></returns>
        public string[] toArray() {
            List<string> ans = new List<string>();
            for(int i = 0; i < itemsource.Length; ++i) {
                if (getSelected(i)) ans.Add(itemsource[i].Key);
            }
            return ans.ToArray();
        }

        //UI显示记录模块。
        private TextBlock[] readcontents;
        private CheckBox[] writecontents;
    }
}
