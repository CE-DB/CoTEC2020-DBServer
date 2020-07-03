using HotChocolate;
using System;
using System.Collections.Generic;

namespace CoTEC_Server.Logic.GraphQL.Types.Input
{
    public class UpdatePatient
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
        public bool? Hospitalized { get; set; } = null;
        [GraphQLNonNullType(IsNullable = true)]
        public bool? intensiveCareUnite { get; set; } = null;
        [GraphQLNonNullType(IsNullable = true)]
        public string region { get; set; }
        [GraphQLNonNullType(IsNullable = true)]
        public string country { get; set; }
        [GraphQLNonNullType(IsElementNullable = false, IsNullable = true)]
        public ICollection<string> pathologies { get; set; } = null;
        [GraphQLNonNullType(IsNullable = true)]
        public string state { get; set; }
        [GraphQLNonNullType(IsElementNullable = false, IsNullable = true)]
        public ICollection<string> contacts { get; set; } = null;
        [GraphQLNonNullType(IsElementNullable = false, IsNullable = true)]
        public ICollection<string> medication { get; set; } = null;
        [GraphQLNonNullType(IsNullable = true)]
        public DateTime? dateEntrance { get; set; } = null;
    }
}
