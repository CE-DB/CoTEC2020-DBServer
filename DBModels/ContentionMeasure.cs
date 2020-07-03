using System;
using System.Collections.Generic;

namespace CoTEC_Server.DBModels
{
    public  class ContentionMeasure
    {
        public ContentionMeasure()
        {
            ContentionMeasuresChanges = new HashSet<ContentionMeasuresChanges>();
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public  ICollection<ContentionMeasuresChanges> ContentionMeasuresChanges { get; set; }
    }
}
