using System;
using System.Collections.Generic;

namespace FaceDetectionAttendance.MVVM.Model;

public partial class Attendance
{
    public int Id { get; set; }

    public int IdWorker { get; set; }

    public DateTime DM { get; set; }

    public string? IdFaculty { get; set; }

    public int? ShiftWorked { get; set; }

    //public virtual Faculty? IdFacultyNavigation { get; set; }

    //public virtual WorkerList IdWorkerNavigation { get; set; } = null!;
}
