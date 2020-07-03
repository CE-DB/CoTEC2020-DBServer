using HotChocolate;
using System.Collections.Generic;

namespace CoTEC_Server.Logic.GraphQL.Types.Input
{
    public class UpdateContact
    {
        [GraphQLNonNullType(IsNullable = true)]
        public string firstName { get; set; }
        [GraphQLNonNullType(IsNullable = true)]
        public string lastName { get; set; }
        [GraphQLNonNullType(IsNullable = true)]
        public string identification { get; set; }
        [GraphQLNonNullType(IsNullable = true)]
        public byte? age { get; set; } = null;
        [GraphQLNonNullType(IsNullable = true)]
        public string? nationality { get; set; }
        [GraphQLNonNullType(IsNullable = true)]
        public string region { get; set; }
        [GraphQLNonNullType(IsNullable = true)]
        public string country { get; set; }
        [GraphQLNonNullType(IsElementNullable = false, IsNullable = true)]
        public ICollection<string> pathologies { get; set; } = null;
        [GraphQLNonNullType(IsNullable = true)]
        public string address { get; set; }
        [GraphQLNonNullType(IsNullable = true)]
        public string email { get; set; }
    }
}
