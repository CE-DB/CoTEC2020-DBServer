using CoTEC_Server.DBModels;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    internal class ContactType : ObjectType<Contact>
    {

        protected override void Configure(IObjectTypeDescriptor<Contact> descriptor)
        {
            base.Configure(descriptor);

            descriptor.BindFieldsExplicitly();

            descriptor.Field(t => t.FirstName).Type<NonNullType<StringType>>();

            descriptor.Field(t => t.LastName).Type<NonNullType<StringType>>();

            descriptor.Field(t => t.Identification).Type<NonNullType<StringType>>();

            descriptor.Field(t => t.Age).Type<IntType>();

            descriptor.Field(t => t.Nationality).Type<StringType>();

            descriptor.Field(t => t.Address).Type<NonNullType<StringType>>();

            descriptor.Field(t => t.Email).Type<NonNullType<StringType>>();

            descriptor.Field(t => t.RegionNavigation)
                .Type<NonNullType<RegionType>>()
                .Name("origin");

            descriptor.Field("pathologies")
                .Type<NonNullType<ListType<NonNullType<PathologyType>>>>()
                .Resolver(ctx => {

                    return ctx.Service<CoTEC_DBContext>().Pathology
                    .FromSqlRaw("SELECT Name, description, treatment " +
                    "FROM admin.Pathology " +
                    "WHERE EXISTS(SELECT Name " +
                                "FROM healthcare.Contact_Pathology " +
                                "WHERE Contact_Id = {0})", ctx.Parent<Contact>().Identification)
                    .ToList();
                });

        }
    }
}