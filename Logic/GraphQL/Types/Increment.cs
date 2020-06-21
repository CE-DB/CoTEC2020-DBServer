using System;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    /// <summary>
    /// Holds information about count of infected, recovered, deseased or active (infected - recovered - deseased)
    /// </summary>
    public class Increment
    {

        /// <summary>
        /// Day the increase occurred. The format is YYYY-MM-DD.
        /// </summary>
        public DateTime day { get; set; }

        /// <summary>
        /// The number of people infected.
        /// </summary>
        public int totalInfected { get; set; }

        /// <summary>
        /// The number of people recovered.
        /// </summary>
        public int totalRecovered { get; set; }

        /// <summary>
        /// The number of people deseased.
        /// </summary>
        public int totalDeseased { get; set; }

        /// <summary>
        /// The number of people active.
        /// </summary>
        public int totalActive { get; set; }

    }
}
