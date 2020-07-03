using System;
using System.Collections.Generic;

namespace CoTEC_Server.DBModels
{
    public  class PatientContact
    {
        public string PatientId { get; set; }
        public string ContactId { get; set; }
        public DateTime LastVisit { get; set; }

        public  Contact Contact { get; set; }
        public  Patient Patient { get; set; }
    }
}
