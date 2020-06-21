using System.Collections.Generic;

namespace CoTEC_Server.Logic.GraphQL.Types
{

    /// <summary>
    /// Holds hospital manager and patient contact info.
    /// </summary>
    public class Contact
    {

        /// <summary>
        /// Contact first name. 1000 characters max.
        /// </summary>
        public string firstName { get; set; }

        /// <summary>
        /// Contact last name. 1000 characters max.
        /// </summary>
        public string lastName { get; set; }

        /// <summary>
        /// Contact identification code. 1000 chars max.
        /// </summary>
        public string identification { get; set; }

        /// <summary>
        /// Contact age, 3 digits max. and only positive values are allowed.
        /// </summary>
        public int age { get; set; }

        /// <summary>
        /// Contact nationality, 100 characters max. and only letters and white spaces are allowed.
        /// </summary>
        public string nationality { get; set; }

        /// <summary>
        /// Exact contact address. 200 chars allowed.
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// Contact pathologies list.
        /// </summary>
        public List<Pathology> pathologies { get; set; }

        /// <summary>
        /// The email address of contact. 100 characters max.
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// Region of contact address.
        /// </summary>
        public Region region { get; set; }

    }
}
