using HotChocolate;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CoTEC_Server.Logic.GraphQL.Types.Input
{
    public class CreatePatient
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
        public bool hospitalized { get; set; }
        [GraphQLNonNullType]
        public bool intensiveCareUnite { get; set; }
        [GraphQLNonNullType]
        public string region { get; set; }
        [GraphQLNonNullType]
        public string country { get; set; }
        [GraphQLNonNullType( IsElementNullable = false, IsNullable = true)]
        public ICollection<string> pathologies { get; set; }
        [GraphQLNonNullType]
        public string state { get; set; }
        [GraphQLNonNullType(IsElementNullable = false, IsNullable = true)]
        public ICollection<string> contacts { get; set; }
        [GraphQLNonNullType(IsElementNullable = false, IsNullable = true)]
        public ICollection<string> medication{ get; set; }
        [GraphQLNonNullType]
        public DateTime dateEntrance { get; set; }

    }
}
