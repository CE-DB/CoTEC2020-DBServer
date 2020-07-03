using System.Collections.Generic;

namespace CoTEC_Server.Logic.GraphQL.Types.Input
{
    public class UpdatePathology
    {
        public string name { get; set; }
        public string description { get; set; }
        public ICollection<string> symptoms { get; set; }
        public string treatment { get; set; }
    }
}
