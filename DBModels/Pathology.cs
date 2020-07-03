using System;
using System.Collections.Generic;

namespace CoTEC_Server.DBModels
{
    public  class Pathology
    {
        public Pathology()
        {
            ContactPathology = new HashSet<ContactPathology>();
            PathologySymptoms = new HashSet<PathologySymptoms>();
            PatientPathology = new HashSet<PatientPathology>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Treatment { get; set; }

        public  ICollection<ContactPathology> ContactPathology { get; set; }
        public  ICollection<PathologySymptoms> PathologySymptoms { get; set; }
        public  ICollection<PatientPathology> PatientPathology { get; set; }
    }
}
