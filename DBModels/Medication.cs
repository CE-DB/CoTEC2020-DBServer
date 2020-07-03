using System;
using System.Collections.Generic;

namespace CoTEC_Server.DBModels
{
    public  class Medication
    {
        public Medication()
        {
            PatientMedication = new HashSet<PatientMedication>();
        }

        public string Medicine { get; set; }
        public string Pharmacist { get; set; }

        public  ICollection<PatientMedication> PatientMedication { get; set; }
    }
}
