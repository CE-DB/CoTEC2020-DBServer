using System;
using System.Collections.Generic;

namespace CoTEC_Server.DBModels
{
    public  class Staff
    {
        public Staff()
        {
            HospitalWorkers = new HashSet<HospitalWorkers>();
        }

        public string IdCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public  ICollection<HospitalWorkers> HospitalWorkers { get; set; }
    }
}
