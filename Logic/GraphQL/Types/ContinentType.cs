using CoTEC_Server.DBModels;
using HotChocolate.Types;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class ContinentType : ObjectType<Continent>
    {

        protected override void Configure(IObjectTypeDescriptor<Continent> descriptor)
        {
            base.Configure(descriptor);

            descriptor.BindFieldsExplicitly();

            descriptor.Field(f => f.Name).Type<NonNullType<StringType>>();

            descriptor.Field(f => f.Country).Type<NonNullType<ListType<NonNullType<CountryType>>>>();


        }

    }
}
