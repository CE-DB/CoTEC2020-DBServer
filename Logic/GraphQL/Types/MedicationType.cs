using HotChocolate.Types;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class MedicationType : ObjectType<Medication>
    {

        protected override void Configure(IObjectTypeDescriptor<Medication> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Field(t => t.name).Type<NonNullType<StringType>>();

            descriptor.Field(t => t.pharmaceutical).Type<NonNullType<StringType>>();
        }

    }
}
