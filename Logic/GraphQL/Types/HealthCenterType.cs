using HotChocolate.Types;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class HealthCenterType : ObjectType<Healthcenter>
    {

        protected override void Configure(IObjectTypeDescriptor<Healthcenter> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Field(t => t.name).Type<NonNullType<StringType>>();

            descriptor.Field(t => t.capacity).Type<NonNullType<IntType>>();

            descriptor.Field(t => t.totalICUbeds).Type<NonNullType<IntType>>();

            descriptor.Field(t => t.directorcontact).Type<NonNullType<ContactType>>();

            descriptor.Field(t => t.ubication).Type<NonNullType<RegionType>>();
        }

    }
}
