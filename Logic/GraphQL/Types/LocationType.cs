using HotChocolate.Types;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class LocationType : ObjectType<Location>
    {

        protected override void Configure(IObjectTypeDescriptor<Location> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Field(t => t.Name).Type<StringType>();

            descriptor.Field(t => t.TotalInfected).Type<NonNullType<IntType>>();

            descriptor.Field(t => t.Totalrecovered).Type<NonNullType<IntType>>();

            descriptor.Field(t => t.TotalDeseased).Type<NonNullType<IntType>>();

            descriptor.Field(t => t.TotalActive).Type<NonNullType<IntType>>();

            descriptor.Field(t => t.DailyIncrement).Type<NonNullType<ListType<NonNullType<IncrementType>>>>();

            descriptor.Field(t => t.SanitaryMeasures).Type<NonNullType<ListType<NonNullType<SanitaryMeasureType>>>>();

            descriptor.Field(t => t.ContentionMeasures).Type<NonNullType<ListType<NonNullType<ContentionMeasureType>>>>();
        }

    }
}
