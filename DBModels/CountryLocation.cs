using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoTEC_Server.DBModels
{
    public class CountryLocation
    {

        public int totalInfected { get; set; }
        public int totalRecovered { get; set; }
        public int totalDeceased { get; set; }
        public int totalActive { get; set; }
        public string name { get; set; }

    }
}
