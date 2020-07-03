using HotChocolate;

namespace CoTEC_Server.Logic.GraphQL.Types.Input
{
    public class UpdateHealthcenter
    {
        [GraphQLNonNullType(IsNullable = true)]
        public string name { get; set; }
        [GraphQLNonNullType(IsNullable = true)]
        public int? capacity { get; set; } = null;
        [GraphQLNonNullType(IsNullable = true)]
        public int? totalICUBeds { get; set; } = null;
        [GraphQLNonNullType(IsNullable = true)]
        public string directorContact { get; set; }
        [GraphQLNonNullType(IsNullable = true)]
        public string region { get; set; }
        [GraphQLNonNullType(IsNullable = true)]
        public string country { get; set; }
    }
}
