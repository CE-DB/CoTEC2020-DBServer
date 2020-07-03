using System;
using System.Collections.Generic;

namespace CoTEC_Server.DBModels
{
    public  class Region
    {
        public Region()
        {
            Contact = new HashSet<Contact>();
            Hospital = new HashSet<Hospital>();
            Patient = new HashSet<Patient>();
        }

        public string Name { get; set; }
        public string Country { get; set; }

        public  Country CountryNavigation { get; set; }
        public  ICollection<Contact> Contact { get; set; }
        public  ICollection<Hospital> Hospital { get; set; }
        public  ICollection<Patient> Patient { get; set; }
    }
}
