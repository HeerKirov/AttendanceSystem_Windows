using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AttendanceSystem_Windows.HttpConnection;

namespace AttendanceSystem_Windows.Services {
    public class User {
        public static User Get() => reference == null ? reference = new User() : reference;
        private static User reference = null;
        private User() { }

        public Auth authentication { get; set; } = null;

        public HashSet<Authority> authority { get; private set; } = new HashSet<Authority>();

        
    }
    public enum Authority {
        Root,
        Student,Teacher,Instructor,Office,Admin,
        StudentManage,TeacherManage,InstructorManage,ClassroomManage,CourseManage,ClassManage
    }
}
