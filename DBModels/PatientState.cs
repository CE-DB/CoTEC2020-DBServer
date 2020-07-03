using System;
using System.Collections.Generic;

namespace CoTEC_Server.DBModels
{
    public  class PatientState
    {
        public PatientState()
        {
            Patient = new HashSet<Patient>();
        }

        public string Name { get; set; }

        public  ICollection<Patient> Patient { get; set; }
    }
}
