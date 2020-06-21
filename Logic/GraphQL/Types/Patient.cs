using System.Collections.Generic;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    /// <summary>
    /// Holds patient information.
    /// </summary>
    public class Patient {

        /// <summary>
        /// Patient first name. 1000 characters max.
        /// </summary>
        public string firstName { get; set; }

        /// <summary>
        /// Patient last name. 1000 characters max.
        /// </summary>
        public string lastName { get; set; }

        /// <summary>
        /// Patient identification code. 1000 chars max.
        /// </summary>
        public string identification { get; set; }

        /// <summary>
        /// Patient age, 3 digits max. and only positive values are allowed.
        /// </summary>
        public int age { get; set; }

        /// <summary>
        /// Patient nationality, 100 characters max. and only letters and white spaces are allowed.
        /// </summary>
        public string nationality { get; set; }

        /// <summary>
        /// Indicates if patient is in hospital.
        /// </summary>
        public bool hospitalized { get; set; }

        /// <summary>
        /// indicates if patient is in intesive care unit (ICU).
        /// </summary>
        public bool intensiveCareUnite { get; set; }

        /// <summary>
        /// Region of Patient address.
        /// </summary>
        public Region region { get; set; }

        /// <summary>
        /// Patient pathologies list.
        /// </summary>
        public List<Pathology> pathologies { get; set; }

        /// <summary>
        /// Patient state (dead, infected, recovered, ...)
        /// </summary>
        public PatientState state { get; set; }

        /// <summary>
        /// Patient list of last Patients.
        /// </summary>
        public List<Contact> latestContacts { get; set; }

        /// <summary>
        /// Patient list of medications applied.
        /// </summary>
        public List<Medication> medications { get; set; }
    }
}
