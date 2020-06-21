using System.Collections.Generic;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    /// <summary>
    /// Holds details about a country.
    /// </summary>

    public class Country
    {

        /// <summary>
        /// Country name. 500 characters allowed.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Country regions list.
        /// </summary>
        public List<Region> Regions { get; set; }
        
    }
}
