using System;
using System.Collections.Generic;

namespace CoTEC_Server.DBModels
{
    public  class HospitalWorkers
    {
        public string IdCode { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }

        public  Hospital Hospital { get; set; }
        public  Staff IdCodeNavigation { get; set; }
    }
}
