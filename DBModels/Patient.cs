using System;
using System.Collections.Generic;

namespace CoTEC_Server.DBModels
{
    public  class Patient
    {
        public Patient()
        {
            PatientContact = new HashSet<PatientContact>();
            PatientMedication = new HashSet<PatientMedication>();
            PatientPathology = new HashSet<PatientPathology>();
        }

        public string Identification { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Region { get; set; }
        public string Nationality { get; set; }
        public string Country { get; set; }
        public byte Age { get; set; }
        public bool IntensiveCareUnite { get; set; }
        public bool Hospitalized { get; set; }
        public string State { get; set; }
        public DateTime DateEntrance { get; set; }

        public  Region RegionNavigation { get; set; }
        public  PatientState StateNavigation { get; set; }
        public  ICollection<PatientContact> PatientContact { get; set; }
        public  ICollection<PatientMedication> PatientMedication { get; set; }
        public  ICollection<PatientPathology> PatientPathology { get; set; }
    }
}
