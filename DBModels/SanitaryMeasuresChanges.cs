using System;
using System.Collections.Generic;

namespace CoTEC_Server.DBModels
{
    public  class SanitaryMeasuresChanges
    {
        public string CountryName { get; set; }
        public string MeasureName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public  Country CountryNameNavigation { get; set; }
        public  SanitaryMeasure MeasureNameNavigation { get; set; }
    }
}
