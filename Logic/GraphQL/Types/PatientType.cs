using HotChocolate.Types;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class PatientType : ObjectType<Patient>
    {

        protected override void Configure(IObjectTypeDescriptor<Patient> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Field(a => a.firstName).Type<NonNullType<StringType>>();

            descriptor.Field(a => a.lastName).Type<NonNullType<StringType>>();

            descriptor.Field(a => a.identification).Type<NonNullType<StringType>>();

            descriptor.Field(a => a.age).Type<NonNullType<IntType>>();

            descriptor.Field(a => a.nationality).Type<NonNullType<StringType>>();

            descriptor.Field(a => a.pathologies).Type<NonNullType<ListType<NonNullType<PathologyType>>>>();

            descriptor.Field(a => a.region).Type<NonNullType<RegionType>>();

            descriptor.Field(a => a.hospitalized).Type<NonNullType<BooleanType>>();

            descriptor.Field(a => a.intensiveCareUnite).Type<NonNullType<BooleanType>>();

            descriptor.Field(a => a.state).Type<NonNullType<PatientStateType>>();

            descriptor.Field(a => a.latestContacts).Type<NonNullType<ListType<NonNullType<ContactType>>>>();

            descriptor.Field(a => a.medications).Type<NonNullType<ListType<NonNullType<MedicationType>>>>();
        }

    }
}
