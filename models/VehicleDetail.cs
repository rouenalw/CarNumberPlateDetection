using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Verification_System.models
{
    class VehicleDetail
    {
        public long id { get; set; }
        public string fullname { get; set; }
        public string vehicle_no { get; set; }
        public string licence_no { get; set; }
        public string details { get; set; }
        public string chasis_no { get; set; }
        public string dob { get; set; }
        public string token { get; set; }
        public string address { get; set; }
        public string image { get; set; }
        public int status { get; set; }
        public string image_path { get; set; }

    }
}
