using System;
using System.Collections.Generic;

namespace CoTEC_Server.DBModels
{
    public  class Continent
    {
        public Continent()
        {
            Country = new HashSet<Country>();
        }

        public string Name { get; set; }

        public  ICollection<Country> Country { get; set; }
    }
}
