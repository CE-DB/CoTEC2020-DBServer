namespace CoTEC_Server.Logic.GraphQL.Types
{
    /// <summary>
    /// Holds Holds information about medications.
    /// </summary>

    public class Medication
    {

        /// <summary>
        /// Medication name. 500 characters allowed.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Corporation or lab that creates medication. 200 characters allowed.
        /// </summary>
        public string pharmaceutical { get; set; }

    }
}
