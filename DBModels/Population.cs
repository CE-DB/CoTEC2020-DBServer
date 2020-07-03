using System;
using System.Collections.Generic;

namespace CoTEC_Server.DBModels
{
    public  class Population
    {
        public DateTime Day { get; set; }
        public string CountryName { get; set; }
        public int Infected { get; set; }
        public int Cured { get; set; }
        public int Dead { get; set; }
        public int Active { get; set; }

        public  Country CountryNameNavigation { get; set; }
    }
}
