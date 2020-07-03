using CoTEC_Server.DBModels;
using HotChocolate.Types;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class RegionType : ObjectType<Region>
    {

        protected override void Configure(IObjectTypeDescriptor<Region> descriptor)
        {
            base.Configure(descriptor);

            descriptor.BindFieldsExplicitly();

            descriptor.Field(f => f.Name).Type<StringType>();

            descriptor.Field(f => f.CountryNavigation)
                .Type<NonNullType<CountryType>>()
                .Name("country");
        }




    }
}