namespace CoTEC_Server.Logic.GraphQL.Types
{
    /// <summary>
    /// Holds region information.
    /// </summary>

    public class Region
    {
        /// <summary>
        /// Region name. 500 characters allowed.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Region country information.
        /// </summary>
        public Country country { get; set; }

    }
}
