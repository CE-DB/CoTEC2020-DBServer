using HotChocolate.Types;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class ContactType : ObjectType<Contact>
    {

        protected override void Configure(IObjectTypeDescriptor<Contact> descriptor)
        {
            base.Configure(descriptor);

            
            descriptor.Field(a => a.firstName).Type<NonNullType<StringType>>();

            descriptor.Field(a => a.lastName).Type<NonNullType<StringType>>();

            descriptor.Field(a => a.identification).Type<NonNullType<StringType>>();

            descriptor.Field(a => a.age).Type<NonNullType<IntType>>();

            descriptor.Field(a => a.nationality).Type<NonNullType<StringType>>();

            descriptor.Field(a => a.address).Type<NonNullType<StringType>>();

            descriptor.Field(a => a.pathologies).Type<NonNullType<ListType<NonNullType<PathologyType>>>>();

            descriptor.Field(a => a.email).Type<NonNullType<StringType>>();

            descriptor.Field(a => a.region).Type<NonNullType<RegionType>>();


        }


    }
}
