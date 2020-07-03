using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoTEC_Server.DBModels
{
    public class PatientsReport
    {
        public string Country { get; set; }
        public int Infected { get; set; }
        public int Cured { get; set; }
        public int Dead { get; set; }
        public int Active { get; set; }
    }
}
