using CoTEC_Server.DBModels;
using HotChocolate.Types;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class ContentionMeasureType : ObjectType<ContentionMeasure>
    {

        protected override void Configure(IObjectTypeDescriptor<ContentionMeasure> descriptor)
        {
            base.Configure(descriptor);

            descriptor.BindFieldsExplicitly();

            descriptor.Field(t => t.Name)
                .Type<NonNullType<StringType>>();

            descriptor.Field(t => t.Description)
                .Type<StringType>();


        }


    }
}
