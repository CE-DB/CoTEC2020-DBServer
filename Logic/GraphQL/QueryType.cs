using CoTEC_Server.Logic.GraphQL.Types;
using HotChocolate.Types;

namespace CoTEC_Server.Logic.GraphQL
{
    public class QueryType : ObjectType<Query>
    {
        protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Field(q => q.totalCountCases(default, default))
                .Type<NonNullType<LocationType>>();

            descriptor.Field(q => q.contentionMeasuresActive(default, default))
                .Type<NonNullType<LocationType>>()
                .Argument("countryName", a => a.Type<NonNullType<StringType>>());

            descriptor.Field(q => q.countries(default, default))
                .Type<NonNullType<ListType<NonNullType<CountryType>>>>();

            descriptor.Field(q => q.regions(default, default))
                .Type<NonNullType<ListType<NonNullType<RegionType>>>>();

            descriptor.Field(q => q.pathologies(default, default))
                .Type<NonNullType<ListType<NonNullType<PathologyType>>>>();

            descriptor.Field(q => q.patientStates(default, default))
                .Type<NonNullType<ListType<NonNullType<PatientStateType>>>>();

            descriptor.Field(q => q.Healthcenters(default, default))
                .Type<NonNullType<ListType<NonNullType<HealthCenterType>>>>();

            descriptor.Field(q => q.SanitaryMeasures(default, default))
                .Type<NonNullType<ListType<NonNullType<SanitaryMeasureType>>>>();

            descriptor.Field(q => q.ContentionMeasures(default, default))
                .Type<NonNullType<ListType<NonNullType<ContentionMeasureType>>>>();

            descriptor.Field(q => q.medications(default, default))
                .Type<NonNullType<ListType<NonNullType<MedicationType>>>>();

            descriptor.Field(q => q.patients(default, default))
                .Type<NonNullType<ListType<NonNullType<PatientType>>>>();

            descriptor.Field(q => q.contacts(default, default))
                .Type<NonNullType<ListType<NonNullType<ContactType>>>>();

            descriptor.Field(q => q.patientsReport(default))
                .Type<NonNullType<StringType>>();

            descriptor.Field(q => q.newCasesReport(default))
                .Type<NonNullType<StringType>>();
        }
    }
}
