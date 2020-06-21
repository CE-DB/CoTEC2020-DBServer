using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class ContentionMeasureType : ObjectType<ContentionMeasure>
    {

        protected override void Configure(IObjectTypeDescriptor<ContentionMeasure> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Field(t => t.country).Type<CountryType>();

            descriptor.Field(t => t.description).Type<NonNullType<StringType>>();

            descriptor.Field(t => t.name).Type<NonNullType<StringType>>();

            descriptor.Field(t => t.startDate).Type<DateType>();

            descriptor.Field(t => t.endDate).Type<DateType>();
        }

    }
}
