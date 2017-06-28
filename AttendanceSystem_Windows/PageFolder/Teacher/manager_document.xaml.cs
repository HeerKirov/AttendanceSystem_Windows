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

namespace AttendanceSystem_Windows.PageFolder.Student {

    public partial class manager_document : BasePage {
        public manager_document() {
            InitializeComponent();
            default_auth = new AuthSetting[] {
                new AuthSetting(Authority.StudentManage)
            };

            //txt_gender.Choices = gender_choices;
            template = new InstanceTemplate();

            var args = new InstanceArgs("api/auth/users/", null, User.Api, Title, Deleteable: true, Onlywrite:true);
            args.margin = new Thickness(0, 10, 0, 10);
            args.items.Add(new InstanceArgs.Item("name", "姓名", "text"));
            args.items.Add(new InstanceArgs.Item("gender", "性别", "choice",data:gender_choices.ToArray()));
            template.Construct(args);
            ContentGrid.Children.Add(template);
        }
        public KeyValuePair<string, string>[] gender_choices = new KeyValuePair<string, string>[] {
            new KeyValuePair<string, string>("MALE","男"),
            new KeyValuePair<string, string>("FEMALE","女")
        };
        private InstanceTemplate template;
        private string ID = null;

        public override void NavigatedToEvent(object sender, IDictionary<string, object> kwargs) {
            base.NavigatedToEvent(sender, kwargs);
            if (kwargs != null) ID = kwargs["id"] as string;
            if (ID != null) {
                template.Args.instance = ID;
                template.UpdateData();
                
            }
        }

    }
}
