using System;
using System.Collections.Generic;

namespace CoTEC_Server.DBModels
{
    public  class Hospital
    {
        public Hospital()
        {
            HospitalWorkers = new HashSet<HospitalWorkers>();
        }

        public string Name { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public short Capacity { get; set; }
        public short IcuCapacity { get; set; }
        public string Manager { get; set; }

        public  Contact ManagerNavigation { get; set; }
        public  Region RegionNavigation { get; set; }
        public  ICollection<HospitalWorkers> HospitalWorkers { get; set; }
    }
}
