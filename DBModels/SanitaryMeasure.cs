using System;
using System.Collections.Generic;

namespace CoTEC_Server.DBModels
{
    public  class SanitaryMeasure
    {
        public SanitaryMeasure()
        {
            SanitaryMeasuresChanges = new HashSet<SanitaryMeasuresChanges>();
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public  ICollection<SanitaryMeasuresChanges> SanitaryMeasuresChanges { get; set; }
    }
}
