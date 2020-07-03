using CoTEC_Server.DBModels;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class LocationType : ObjectType<Increment>
    {

        protected override void Configure(IObjectTypeDescriptor<Increment> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Name("Location");

            descriptor.BindFieldsExplicitly();

            descriptor.Field(f => f.totalInfected)
                .Type<NonNullType<IntType>>();

            descriptor.Field(f => f.totalRecovered)
                .Type<NonNullType<IntType>>();

            descriptor.Field(f => f.totalDeceased)
                .Type<NonNullType<IntType>>();

            descriptor.Field(f => f.totalActive)
                .Type<NonNullType<IntType>>();

            descriptor.Field("dailyIncrement")
                .Type<NonNullType<ListType<NonNullType<IncrementType>>>>()
                .Resolver(ctx => { 

                    return ctx.Service<CoTEC_DBContext>().Population
                        .FromSqlRaw("SELECT day, '' AS country_Name, SUM(Infected) AS Infected, SUM(Cured) AS Cured, SUM(Dead) AS Dead, SUM(Active) AS Active " +
                        "FROM user_public.Population " +
                        "GROUP BY day " +
                        "ORDER BY day ASC")
                        .ToList();
                
                });
        }

    }
}
