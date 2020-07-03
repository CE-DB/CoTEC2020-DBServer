using CoTEC_Server.DBModels;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class IncrementType : ObjectType<Population>
    {

        protected override void Configure(IObjectTypeDescriptor<Population> descriptor)
        {
            base.Configure(descriptor);

            descriptor.BindFieldsExplicitly();

            descriptor.Name("Increment");

            descriptor.Field(f => f.Day)
                .Type<NonNullType<DateType>>();

            descriptor.Field(f => f.Infected)
                .Type<NonNullType<IntType>>();

            descriptor.Field(f => f.Cured)
                .Type<NonNullType<IntType>>()
                .Name("recovered");

            descriptor.Field(f => f.Dead)
                .Type<NonNullType<IntType>>()
                .Name("deceased");

            descriptor.Field(f => f.Active)
                .Type<NonNullType<IntType>>();
        }



    }
}
