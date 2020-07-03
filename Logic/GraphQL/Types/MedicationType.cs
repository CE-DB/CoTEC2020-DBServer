using CoTEC_Server.DBModels;
using HotChocolate.Types;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class MedicationType : ObjectType<Medication>
    {

        protected override void Configure(IObjectTypeDescriptor<Medication> descriptor)
        {
            base.Configure(descriptor);

            descriptor.BindFieldsExplicitly();

            descriptor.Field(f => f.Medicine).Type<NonNullType<StringType>>()
                .Name("name");

            descriptor.Field(f => f.Pharmacist).Type<StringType>()
                .Name("pharmaceutical");
        }


    }
}
