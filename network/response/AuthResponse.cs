using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle_Verification_System.network.response
{
    class AuthResponse
    {
        public long id { get; set; }
        public string fullname { get; set; }
        public bool is_admin { get; set; }

    }
}
