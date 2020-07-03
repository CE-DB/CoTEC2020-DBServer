using System;
using System.Collections.Generic;

namespace CoTEC_Server.DBModels
{
    public  class Country
    {
        public Country()
        {
            ContentionMeasuresChanges = new HashSet<ContentionMeasuresChanges>();
            Population = new HashSet<Population>();
            Region = new HashSet<Region>();
            SanitaryMeasuresChanges = new HashSet<SanitaryMeasuresChanges>();
        }

        public string Name { get; set; }
        public string Continent { get; set; }

        public  Continent ContinentNavigation { get; set; }
        public  ICollection<ContentionMeasuresChanges> ContentionMeasuresChanges { get; set; }
        public  ICollection<Population> Population { get; set; }
        public  ICollection<Region> Region { get; set; }
        public  ICollection<SanitaryMeasuresChanges> SanitaryMeasuresChanges { get; set; }
    }
}
