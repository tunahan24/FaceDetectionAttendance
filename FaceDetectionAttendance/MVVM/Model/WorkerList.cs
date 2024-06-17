using System;
using System.Collections.Generic;

namespace FaceDetectionAttendance.MVVM.Model;

public partial class WorkerList
{
    public int Id { get; set; }

    public string Fullname { get; set; }

    public DateTime Birth { get; set; }
    public int Salary { get;set; }

    public string Images { get; set; } = null!;

    public string? Fid { get; set; }

   //public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

   //public virtual Faculty? FidNavigation { get; set; }
}
