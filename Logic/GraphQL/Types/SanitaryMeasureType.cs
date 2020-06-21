using HotChocolate.Types;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class SanitaryMeasureType : ObjectType<SanitaryMeasure>
    {
        protected override void Configure(IObjectTypeDescriptor<SanitaryMeasure> descriptor)
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
