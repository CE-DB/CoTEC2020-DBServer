using CoTEC_Server.DBModels;
using HotChocolate;
using HotChocolate.Execution;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CoTEC_Server.Logic.GraphQL
{
    public class Mutation
    {
        public Mutation()
        { }

        public async Task<Region> addRegion(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string name,
            [GraphQLNonNullType] string country)
        {

            if (string.IsNullOrEmpty(name))
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage("The name can't be empty.")
                        .SetCode("NAME_EMPTY")
                        .Build());
            }

            if (string.IsNullOrEmpty(country))
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage("The country name can't be empty.")
                        .SetCode("COUNTRY_NAME_EMPTY")
                        .Build());
            }


            var region = new Region
            {

                Name = name,
                Country = country

            };


            var countryRef = await dBContext.Country.FirstOrDefaultAsync(c => c.Name.Equals(country));

            if (countryRef is null)
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage("The specified country name doesn't exist.")
                        .SetCode("INVALID_COUNTRY")
                        .Build());
            }

            var regionRef = await dBContext.Region.FirstOrDefaultAsync(c => c.Name.Equals(name));

            if ( regionRef != null)
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage("The specified region already exists.")
                        .SetCode("REGION_DUPLICATION")
                        .Build());
            }

            dBContext.Region.Add(region);

            await dBContext.SaveChangesAsync();

            return region;


        }


    }
}
