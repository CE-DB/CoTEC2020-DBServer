using System.Collections.Generic;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    /// <summary>
    /// Holds information about pathologies.
    /// </summary>

    public class Pathology
    {
        /// <summary>
        /// Pthology name. 500 characters allowed.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Pathology description. 500 characters allowed.
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Pathology list of symptoms.
        /// </summary>
        public List<string> symptoms { get; set; }

        /// <summary>
        /// Pathology treatment description. 5000 characters allowed.
        /// </summary>
        public string treatment { get; set; }

    }
}
