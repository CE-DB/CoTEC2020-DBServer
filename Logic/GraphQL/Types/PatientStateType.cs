using HotChocolate.Types;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class PatientStateType : ObjectType<PatientState>
    {
        protected override void Configure(IObjectTypeDescriptor<PatientState> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Field(t => t.name).Type<NonNullType<StringType>>();

        }


    }
}
