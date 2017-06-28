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
using AttendanceSystem_Windows.HttpConnection;
using AttendanceSystem_Windows.LocalData;
using AttendanceSystem_Windows.Services;
using Newtonsoft.Json.Linq;
using AttendanceSystem_Windows.PageFolder.Template;

namespace AttendanceSystem_Windows.PageFolder.Basic {

    public partial class other_document : BasePage {
        public other_document() {
            InitializeComponent();

            //txt_gender.Choices = gender_choices;
            template = new InstanceTemplate();

            var args = new InstanceArgs("api/auth/users/", null, User.Api, "个人资料", Deleteable: false);
            args.margin = new Thickness(0, 10, 0, 10);
            args.items.Add(new InstanceArgs.Item("id", "用户名", "text"));
            args.items.Add(new InstanceArgs.Item("name", "姓名", "text"));
            args.items.Add(new InstanceArgs.Item("gender", "性别", "choice", data: gender_choices.ToArray()));
            args.items.Add(new InstanceArgs.Item("register_time", "注册时间", "text"));
            args.items.Add(new InstanceArgs.Item("last_login_time", "最近登录", "text"));

            template.Construct(args);
            ContentGrid.Children.Add(template);
        }
        public KeyValuePair<string, string>[] gender_choices = new KeyValuePair<string, string>[] {
            new KeyValuePair<string, string>("MALE","男"),
            new KeyValuePair<string, string>("FEMALE","女")
        };
        private InstanceTemplate template;
        private ListTemplate list_temp;
        private string ID = null;

        public override bool NavigatingToEvent(object sender, IDictionary<string, object> kwargs) {
            if (kwargs != null) ID = kwargs["id"] as string;
            if (ID != null) {
                default_auth = new AuthSetting[] {
                    new AuthSetting(BelongRelation.Self,"user",ID),
                    new AuthSetting(BelongRelation.Parent,"user",ID),
                    new AuthSetting(Authority.UserManage),
                    new AuthSetting(Authority.Office)
                };
            }
            return base.NavigatingToEvent(sender, kwargs);
        }

        public override void NavigatedToEvent(object sender, IDictionary<string, object> kwargs) {
            base.NavigatedToEvent(sender, kwargs);
            if (kwargs != null) ID = kwargs["id"] as string;
            if (ID != null) {
                template.Args.instance = ID;
                template.UpdateData();

                //修改按钮。
                template.Args.custombutton = new CustomButton[0];
                if (User.Get().HasAuthorityOrRoot(Authority.UserManage)) {
                    template.Args.custombutton = template.Args.custombutton.append(new CustomButton("", () => {
                        NavigatorPage.NavigatorGoto("basic-manager-document", new Dictionary<string, object>() {
                            { "id", ID }
                        });
                    }));
                }
                template.ConstructCustomButton();
            }
        }

    }
}
