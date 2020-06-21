using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    /// <summary>
    /// Holds hospital information.
    /// </summary>

    public class Healthcenter
    {
        /// <summary>
        /// Hospital name. 500 characters allowed. Only numbers and letters are allowed.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Number of max. spaces available.
        /// </summary>
        public int capacity { get; set; }

        /// <summary>
        /// Number of max. intensive care unit spaces available.
        /// </summary>
        public int totalICUbeds { get; set; }

        /// <summary>
        /// Hospital manager contact information.
        /// </summary>
        public Contact directorcontact { get; set; }

        /// <summary>
        /// Hospital region/state where is located.
        /// </summary>
        public Region ubication { get; set; }

    }
}
