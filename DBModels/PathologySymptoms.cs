using System;
using System.Collections.Generic;

namespace CoTEC_Server.DBModels
{
    public  class PathologySymptoms
    {
        public string Pathology { get; set; }
        public string Symptom { get; set; }

        public  Pathology PathologyNavigation { get; set; }
    }
}
