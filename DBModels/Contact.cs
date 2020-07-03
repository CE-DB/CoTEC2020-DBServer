using System;
using System.Collections.Generic;

namespace CoTEC_Server.DBModels
{
    public class Contact
    {
        public Contact()
        {
            ContactPathology = new HashSet<ContactPathology>();
            Hospital = new HashSet<Hospital>();
            PatientContact = new HashSet<PatientContact>();
        }

        public string Identification { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Region { get; set; }
        public string Nationality { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public byte? Age { get; set; }

        public Region RegionNavigation { get; set; }
        public ICollection<ContactPathology> ContactPathology { get; set; }
        public ICollection<Hospital> Hospital { get; set; }
        public ICollection<PatientContact> PatientContact { get; set; }
    }
}
