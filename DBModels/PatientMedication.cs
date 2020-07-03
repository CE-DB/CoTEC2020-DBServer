using System;
using System.Collections.Generic;

namespace CoTEC_Server.DBModels
{
    public  class PatientMedication
    {
        public string PatientId { get; set; }
        public string Medication { get; set; }

        public  Medication MedicationNavigation { get; set; }
        public  Patient Patient { get; set; }
    }
}
