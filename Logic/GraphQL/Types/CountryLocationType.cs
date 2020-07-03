using CoTEC_Server.DBModels;
using HotChocolate.Types;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class CountryLocationType : ObjectType<CountryLocation>
    {

        protected override void Configure(IObjectTypeDescriptor<CountryLocation> descriptor)
        {
            base.Configure(descriptor);

            descriptor.BindFieldsExplicitly();

            descriptor.Field(f => f.name)
                .Type<NonNullType<StringType>>();

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
                        "WHERE country_Name = @param " +
                        "GROUP BY day " +
                        "ORDER BY day ASC", new SqlParameter("@param", ctx.Parent<CountryLocation>().name))
                        .ToList();

                });

            descriptor.Field("sanitaryMeasures")
                .Type<NonNullType<ListType<NonNullType<SanitaryEventType>>>>()
                .Resolver(ctx => {

                    return ctx.Service<CoTEC_DBContext>().SanitaryMeasuresChanges
                        .FromSqlRaw("SELECT s.country_Name, s.End_Date, s.Measure_Name, s.Start_Date " +
                        "FROM user_public.Sanitary_Measures_Changes AS s " +
                        "WHERE country_Name = @param", new SqlParameter("@param", ctx.Parent<CountryLocation>().name))
                        .Include(s => s.CountryNameNavigation)
                        .Include(s => s.MeasureNameNavigation)
                        .ToList();

                });


            descriptor.Field("contentionMeasures")
                .Type<NonNullType<ListType<NonNullType<ContentionEventType>>>>()
                .Resolver(ctx => {

                    return ctx.Service<CoTEC_DBContext>().ContentionMeasuresChanges
                        .FromSqlRaw("SELECT s.country_Name, s.End_Date, s.Measure_Name, s.Start_Date " +
                        "FROM user_public.Contention_Measures_Changes AS s " +
                        "WHERE country_Name = @param", new SqlParameter("@param", ctx.Parent<CountryLocation>().name))
                        .Include(s => s.CountryNameNavigation)
                        .Include(s => s.MeasureNameNavigation)
                        .ToList();

                });
        }
    }
}
