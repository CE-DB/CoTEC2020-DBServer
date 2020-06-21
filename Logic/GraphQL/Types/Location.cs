using System.Collections.Generic;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    /// <summary>
    /// holds details about country specific data (cases increment, measures activated, ...)
    /// </summary>

    public class Location
    {

        /// <summary>
        /// Name of country.
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// Number of infected people.
        /// </summary>
        public int TotalInfected { set; get; }

        /// <summary>
        /// Number of recovered people.
        /// </summary>
        public int Totalrecovered { set; get; }

        /// <summary>
        /// Number of deseased people.
        /// </summary>
        public int TotalDeseased { set; get; }

        /// <summary>
        /// Number of active people.
        /// </summary>
        public int TotalActive { set; get; }

        /// <summary>
        /// List of all daily increments of cases.
        /// </summary>
        public List<Increment> DailyIncrement { set; get; }

        /// <summary>
        /// List of all sanitary measures activated in country.
        /// </summary>
        public List<SanitaryMeasure> SanitaryMeasures { set; get; }

        /// <summary>
        /// List of all contention measures activated in country.
        /// </summary>
        public List<ContentionMeasure> ContentionMeasures { set; get; }

    }
}
