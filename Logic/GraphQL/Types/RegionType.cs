using HotChocolate.Types;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class RegionType : ObjectType<Region>
    {
        protected override void Configure(IObjectTypeDescriptor<Region> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Field(t => t.name).Type<NonNullType<StringType>>();

            descriptor.Field(t => t.country).Type<NonNullType<CountryType>>();
        }

    }
}
