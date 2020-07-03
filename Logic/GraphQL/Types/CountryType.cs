using CoTEC_Server.DBModels;
using HotChocolate.Types;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class CountryType : ObjectType<Country>
    {

        protected override void Configure(IObjectTypeDescriptor<Country> descriptor)
        {
            base.Configure(descriptor);

            descriptor.BindFieldsExplicitly();

            descriptor.Field(f => f.Name).Type<NonNullType<StringType>>();

            descriptor.Field(f => f.ContinentNavigation).Type<NonNullType<ContinentType>>()
                .Name("continent");

            descriptor.Field(f => f.Region).Type<NonNullType<ListType<NonNullType<RegionType>>>>();

        }

    }
}
