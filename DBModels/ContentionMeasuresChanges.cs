using System;
using System.Collections.Generic;

namespace CoTEC_Server.DBModels
{
    public  class ContentionMeasuresChanges
    {
        public string CountryName { get; set; }
        public string MeasureName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public  Country CountryNameNavigation { get; set; }
        public  ContentionMeasure MeasureNameNavigation { get; set; }
    }
}
