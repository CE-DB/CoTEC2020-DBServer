using HotChocolate.Types;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class PathologyType : ObjectType<Pathology>
    {

        protected override void Configure(IObjectTypeDescriptor<Pathology> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Field(t => t.description).Type<NonNullType<StringType>>();

            descriptor.Field(t => t.name).Type<NonNullType<StringType>>();

            descriptor.Field(t => t.symptoms).Type<NonNullType<ListType<NonNullType<StringType>>>>();

            descriptor.Field(t => t.treatment).Type<NonNullType<StringType>>();
        }

    }
}
