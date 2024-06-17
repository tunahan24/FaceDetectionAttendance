using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace FaceDetectionAttendance.MVVM.Model
{
    public class Dataconnecttion
    {
        private SqlConnection dc = new SqlConnection("Data Source=DESKTOP-AOREIER;Initial Catalog=CCPTPM1;Integrated Security=True;Trust Server Certificate=True");
        public SqlConnection GetConnection()
        {
            return this.dc;
        }
    }
}
