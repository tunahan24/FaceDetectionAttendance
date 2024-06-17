using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceDetectionAttendance.MVVM.Model
{
    class Request
    {
        public int id { get; set; }
        public int id_attendance { get; set; }
        public string detail { get; set; }
        public string states { get; set; }
        public string usernamesent { get; set; }
    }
}
