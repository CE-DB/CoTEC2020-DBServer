using HotChocolate;
using System.Collections.Generic;

namespace CoTEC_Server.Logic.GraphQL.Types.Input
{
    public class CreatePathology
    {
        [GraphQLNonNullType]
        public string name { get; set; }
        public string description { get; set; }
        [GraphQLNonNullType(IsElementNullable = false, IsNullable = true)]
        public ICollection<string> symptoms { get; set; }
        public string treatment { get; set; }
    }
}
