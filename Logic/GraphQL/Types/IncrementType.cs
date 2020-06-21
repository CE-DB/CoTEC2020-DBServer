using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class IncrementType : ObjectType<Increment>
    {

        protected override void Configure(IObjectTypeDescriptor<Increment> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Field(t => t.day).Type<NonNullType<DateType>>();

            descriptor.Field(t => t.totalActive).Type<NonNullType<IntType>>();

            descriptor.Field(t => t.totalDeseased).Type<NonNullType<IntType>>();

            descriptor.Field(t => t.totalInfected).Type<NonNullType<IntType>>();

            descriptor.Field(t => t.totalRecovered).Type<NonNullType<IntType>>();
        }

    }
}
