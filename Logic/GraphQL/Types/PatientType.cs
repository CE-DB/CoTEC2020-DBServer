using CoTEC_Server.DBModels;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class PatientType : ObjectType<Patient>
    {

        protected override void Configure(IObjectTypeDescriptor<Patient> descriptor)
        {
            base.Configure(descriptor);

            descriptor.BindFieldsExplicitly();

            descriptor.Field(t => t.FirstName).Type<NonNullType<StringType>>();

            descriptor.Field(t => t.LastName).Type<NonNullType<StringType>>();

            descriptor.Field(t => t.Identification).Type<NonNullType<StringType>>();

            descriptor.Field(t => t.Age).Type<NonNullType<ByteType>>();

            descriptor.Field(t => t.Nationality).Type<NonNullType<StringType>>();

            descriptor.Field(t => t.Hospitalized).Type<NonNullType<BooleanType>>();

            descriptor.Field(t => t.IntensiveCareUnite).Type<NonNullType<BooleanType>>();

            descriptor.Field(t => t.RegionNavigation)
                .Type<RegionType>()
                .Name("region");

            descriptor.Field("country")
                .Type<NonNullType<CountryType>>()
                .Resolver(ctx => {
                    return ctx.Service<CoTEC_DBContext>()
                    .Country
                    .Where(c => c.Name.Equals(ctx.Parent<Patient>().Country))
                        .Include(c => c.ContinentNavigation)
                    .FirstOrDefault();
                });

            descriptor.Field(f => f.State)
                .Type<NonNullType<StringType>>();

            descriptor.Field("pathologies")
                .Type<NonNullType<ListType<NonNullType<PathologyType>>>>()
                .Resolver(ctx => {

                    return ctx.Service<CoTEC_DBContext>().Pathology
                    .FromSqlRaw("SELECT Name, description, treatment " +
                    "FROM admin.Pathology " +
                    "WHERE EXISTS(SELECT Name " +
                                "FROM healthcare.Patient_Pathology " +
                                "WHERE Patient_Id = {0})", ctx.Parent<Patient>().Identification)
                    .ToList();
                
                
                });

            descriptor.Field("medication")
                .Type<NonNullType<ListType<NonNullType<MedicationType>>>>()
                .Resolver(ctx => {

                    return ctx.Service<CoTEC_DBContext>().Medication
                    .FromSqlRaw("SELECT Medicine, Pharmacist " +
                    "FROM admin.Medication " +
                    "WHERE EXISTS(SELECT Medicine " +
                                 "FROM healthcare.Patient_Medication " +
                                 "WHERE Patient_Id = {0})", ctx.Parent<Patient>().Identification)
                    .ToList();
                });

            descriptor.Field("contacts")
                .Type<NonNullType<ListType<NonNullType<ContactVisitType>>>>()
                .Resolver(ctx => {

                    return ctx.Service<CoTEC_DBContext>().PatientContact
                    .FromSqlRaw("SELECT Patient_Id, Contact_Id, last_visit " +
                    "FROM healthcare.Patient_Contact " +
                    "WHERE Patient_Id = {0}", ctx.Parent<Patient>().Identification)
                    .Include(s => s.Contact)
                    .Include(s => s.Patient)
                    .ToList();


                });

            descriptor.Field(f => f.DateEntrance)
                .Type<NonNullType<DateType>>();


        }


    }
}
