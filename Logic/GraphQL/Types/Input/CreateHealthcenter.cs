using HotChocolate;

namespace CoTEC_Server.Logic.GraphQL.Types.Input
{
    public class CreateHealthcenter
    {
        [GraphQLNonNullType]
        public string name { get; set; }
        [GraphQLNonNullType]
        public int capacity { get; set; }
        [GraphQLNonNullType]
        public int totalICUBeds { get; set; }
        [GraphQLNonNullType]
        public string directorContact { get; set; }
        [GraphQLNonNullType]
        public string region { get; set; }
        [GraphQLNonNullType]
        public string country { get; set; }
    }
}
