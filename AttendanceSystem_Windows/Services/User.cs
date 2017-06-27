using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using AttendanceSystem_Windows.HttpConnection;
using AttendanceSystem_Windows.LocalData;
using Newtonsoft.Json.Linq;

namespace AttendanceSystem_Windows.Services {
    public class User {
        public static User Get() => reference == null ? reference = new User() : reference;
        public static RestApi Api => api;
        public static void SetApiConfig(Auth authentication) {
            api = new RestApi(OptionClass.Get().ServerURL, authentication.username, authentication.password);
        }
        private static RestApi api = null;
        private static User reference = null;
        private User() { }

        public Auth authentication { get; set; } = null;


        public HashSet<Authority> authority { get; private set; } = new HashSet<Authority>();

        /// <summary>
        /// 更新权限信息。如果无法连接无服务器，则会返回异常状态码。
        /// </summary>
        /// <returns></returns>
        public HttpStatusCode UpdateAuthority() {
            HttpConnect con = new HttpConnect(OptionClass.Get().GetURL("action/self-authority"), "GET");
            con.authentication = authentication;
            con.Connect();
            if (con.status == HttpStatusCode.OK) {
                authority.Clear();
                JObject jo = JObject.Parse(con.response);
                if (jo["auth"] != null) {
                    var authnumber = long.Parse((jo["auth"] as JValue).Value.ToString());
                    foreach(var i in AuthorityItem.getAuthorityArray(authnumber)) {
                        authority.Add(i);
                    }
                }
            }
            return con.status;
        }
        public bool HasAuthorityOrRoot(Authority auth) {
            return authority.Contains(auth) || authority.Contains(Authority.Root);
        }
        public bool HasAuthority(Authority auth) {
            return authority.Contains(auth);
        }
        /// <summary>
        /// 是否拥有目标权限列表中的任一？
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public bool HasAnyAuthority(Authority[] arr) {
            if (arr == null) return true;
            foreach (var i in arr)
                if (authority.Contains(i))
                    return true;
            return false;
        }
        /// <summary>
        /// 没有任何权限？
        /// </summary>
        /// <returns></returns>
        public bool HasNoAuthority() {
            return authority.Count <= 0;
        }
        /// <summary>
        /// 代为向服务器发起一次从属关系查询。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public BelongRelation BelongQuery(string type, string id) {
            var param = new Dictionary<string, string>() {
                { "type", type },
                { "id", id }
            };
            HttpConnect con = new HttpConnect(OptionClass.Get().GetURL("action/belong"), "GET",param:param);
            con.authentication = authentication;
            con.Connect();
            if (con.status == HttpStatusCode.OK) {
                authority.Clear();
                JObject jo = JObject.Parse(con.response);
                if (jo["relation"] != null) {
                    var relation = jo["relation"].ToString();
                    switch (relation) {
                    case "other":return BelongRelation.Other;
                    case "self":return BelongRelation.Self;
                    case "sub":return BelongRelation.Sub;
                    case "parent":return BelongRelation.Parent;
                    case "manager":return BelongRelation.Manager;
                    default:return BelongRelation.None;
                    }
                }
            }
            return BelongRelation.None;
        }
    }
    public struct AuthorityItem {
        public AuthorityItem(int binary,int number, Authority english) {
            this.binary = binary;
            this.number = number;
            this.english = english;
        }
        public int binary { get; private set; }
        public int number { get; private set; }
        public Authority english { get; private set; }
        public override string ToString() {
            return english.ToString();
        }

        public static readonly AuthorityItem[] list = new AuthorityItem[] {
            new AuthorityItem(0,1,Authority.Root),
            new AuthorityItem(1,2,Authority.Student),
            new AuthorityItem(2,4,Authority.Teacher),
            new AuthorityItem(3,8,Authority.Instructor),
            new AuthorityItem(4,16,Authority.Admin),
            new AuthorityItem(5,32,Authority.UserManage),
            new AuthorityItem(6,64,Authority.StudentManage),
            new AuthorityItem(7,128,Authority.TeacherManage),
            new AuthorityItem(8,256,Authority.InstructorManage),
            new AuthorityItem(9,512,Authority.CourseManage),
            new AuthorityItem(10,1024,Authority.ClassroomManage),
            new AuthorityItem(11,2048,Authority.ClassManage),
        };

        public static Authority[] getAuthorityArray(long auth_number) {
            List<Authority> ans = new List<Authority>();
            string bin = Convert.ToString(auth_number, 2);
            for(int i = 0; i < bin.Length; ++i) {
                if (bin[i] == '1') {
                    ans.Add(list[bin.Length - i - 1].english);
                }
            }
            return ans.ToArray();
        }

        public static long getAuthorityNumber(Authority[] auths) {
            long ans = 0;
            foreach(var i in auths) ans += list[list.firstAt(a => a.english == i)].number;
            return ans;
        }
    }
    public enum Authority {
        Root,
        Student,Teacher,Instructor,Office,Admin,
       UserManage, StudentManage,TeacherManage,InstructorManage,ClassroomManage,CourseManage,ClassManage
    }

    public enum BelongRelation {
        None,
        Other,
        Self,
        Sub,
        Parent,
        Manager
    }
}
