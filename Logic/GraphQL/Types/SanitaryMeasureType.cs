using CoTEC_Server.DBModels;
using HotChocolate.Types;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class SanitaryMeasureType : ObjectType<SanitaryMeasure>
    {

        protected override void Configure(IObjectTypeDescriptor<SanitaryMeasure> descriptor)
        {
            base.Configure(descriptor);

            descriptor.BindFieldsExplicitly();

            descriptor.Field(t => t.Name).
                Type<NonNullType<StringType>>();

            descriptor.Field(t => t.Description).
                Type<StringType>();


        }


    }
}
