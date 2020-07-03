using CoTEC_Server.DBModels;
using HotChocolate.Types;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class ContactVisitType : ObjectType<PatientContact>
    {

        protected override void Configure(IObjectTypeDescriptor<PatientContact> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Name("ContactVisit");

            descriptor.BindFieldsExplicitly();

            descriptor.Field(f => f.Patient).Type<NonNullType<PatientType>>();

            descriptor.Field(f => f.Contact).Type<NonNullType<ContactType>>();

            descriptor.Field(f => f.LastVisit)
                .Type<NonNullType<DateType>>()
                .Name("visitDate");

        }


    }
}
