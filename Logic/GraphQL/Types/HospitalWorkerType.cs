using CoTEC_Server.DBModels;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class HospitalWorkerType : ObjectType<Staff>
    {
        protected override void Configure(IObjectTypeDescriptor<Staff> descriptor)
        {
            base.Configure(descriptor);

            descriptor.BindFieldsExplicitly();

            descriptor.Field(s => s.IdCode).Type<NonNullType<StringType>>();

            descriptor.Field(s => s.FirstName).Type<NonNullType<StringType>>();

            descriptor.Field(s => s.LastName).Type<NonNullType<StringType>>();

            descriptor.Field(s => s.Password).Type<NonNullType<StringType>>();

            descriptor.Field("hospital")
                .Type<NonNullType<HealthCenterType>>()
                .Resolver(ctx => {

                    var data = ctx.Service<CoTEC_DBContext>()
                        .HospitalWorkers
                        .Where(s => s.IdCode.Equals(ctx.Parent<Staff>().IdCode))
                        .Select(s => new {
                            s.Region,
                            s.Country,
                            s.Name
                        })
                        .FirstOrDefault();

                    return ctx.Service<CoTEC_DBContext>().Hospital
                            .Where(s => s.Name.Equals(data.Name) &&
                            s.Region.Contains(data.Region) &&
                            s.Country.Contains(data.Country))
                            .Include(s => s.ManagerNavigation)
                            .Include(s => s.RegionNavigation)
                                .ThenInclude(r => r.CountryNavigation)
                                    .ThenInclude(r => r.ContinentNavigation)
                            .FirstOrDefault();

                });
        }
    }
}
