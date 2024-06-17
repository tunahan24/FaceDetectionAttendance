using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetectionAttendance.MVVM.Model
{
    public class AccountManagement
    {
        public string username { get; set; } = null!;

        public string? password { get; set; }

        public string roles { get; set; }

        public string? gmail { get; set; }

        public string? fid { get; set; }
    }
}
