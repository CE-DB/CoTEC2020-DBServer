using HotChocolate.Types;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class CountryType : ObjectType<Country>
    {
        protected override void Configure(IObjectTypeDescriptor<Country> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Field(t => t.name).Type<NonNullType<StringType>>();

            descriptor.Field(t => t.Regions).Type<NonNullType<ListType<NonNullType<RegionType>>>>();
        }

    }
}
