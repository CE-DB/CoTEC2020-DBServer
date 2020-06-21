using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoTEC_Server.Logic.GraphQL.Types
{

    /// <summary>
    /// Holds details about sanitary measure.
    /// </summary>

    public class SanitaryMeasure
    {

        /// <summary>
        /// Name of measure, 200 characters allowed.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Description of measure. 5000 characters allowed.
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Date measure activation. The format is YYYY-MM-DD
        /// </summary>
        public DateTime startDate { get; set; }

        /// <summary>
        /// Date measure desactivation. The format is YYYY-MM-DD
        /// </summary>
        public DateTime endDate { get; set; }

        /// <summary>
        /// Country where measure is/was active.
        /// </summary>
        public Country country { get; set; }

    }
}
