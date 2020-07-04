using HotChocolate;
using System.Collections.Generic;

namespace CoTEC_Server.Logic.GraphQL.Types.Input
{
    /// <summary>
    /// This class handle the information to create contacts
    /// </summary>
    public class CreateContact
    {

        [GraphQLNonNullType]
        public string firstName { get; set; }
        [GraphQLNonNullType]
        public string lastName { get; set; }
        [GraphQLNonNullType]
        public string identification { get; set; }
        [GraphQLNonNullType]
        public byte age { get; set; }

        public string? nationality { get; set; }
        [GraphQLNonNullType]
        public string region { get; set; }
        [GraphQLNonNullType]
        public string country { get; set; }
        /// <summary>
        /// This is a list of the pathology related names.
        /// </summary>
        [GraphQLNonNullType(IsElementNullable = false, IsNullable = true)]
        public ICollection<string> pathologies { get; set; }
        [GraphQLNonNullType]
        public string address { get; set; }
        [GraphQLNonNullType]
        public string email { get; set; }
    }
}
