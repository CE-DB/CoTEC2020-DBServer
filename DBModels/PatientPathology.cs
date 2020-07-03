using System;
using System.Collections.Generic;

namespace CoTEC_Server.DBModels
{
    public  class PatientPathology
    {
        public string PathologyName { get; set; }
        public string PatientId { get; set; }

        public  Pathology PathologyNameNavigation { get; set; }
        public  Patient Patient { get; set; }
    }
}
