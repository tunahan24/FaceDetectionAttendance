using System;
using System.Collections.Generic;

namespace FaceDetectionAttendance.MVVM.Model;

public partial class Account
{
    public string username { get; set; } = null!;

    public string? password { get; set; }

    public int roles { get; set; }

    public string? gmail { get; set; }

    public string? images { get; set; }

    public string? fid { get; set; }

    //public virtual Faculty? FidNavigation { get; set; }
}
