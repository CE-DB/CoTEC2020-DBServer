using CoTEC_Server.DBModels;
using HotChocolate.Types;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class SanitaryEventType : ObjectType<SanitaryMeasuresChanges>
    {

        protected override void Configure(IObjectTypeDescriptor<SanitaryMeasuresChanges> descriptor)
        {
            base.Configure(descriptor);

            descriptor.BindFieldsExplicitly();

            descriptor.Name("SanitaryEvent");

            descriptor.Field(f => f.MeasureNameNavigation)
                .Type<NonNullType<SanitaryMeasureType>>()
                .Name("measure");

            descriptor.Field(f => f.StartDate)
                .Type<NonNullType<DateType>>();

            descriptor.Field(f => f.EndDate)
                .Type<DateType>();

            descriptor.Field(f => f.CountryNameNavigation)
                .Type<NonNullType<CountryType>>()
                .Name("country");
        }

    }
}
