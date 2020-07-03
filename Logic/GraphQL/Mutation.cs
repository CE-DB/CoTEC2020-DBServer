using CoTEC_Server.DBModels;
using HotChocolate;
using HotChocolate.Execution;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CoTEC_Server.Logic.GraphQL
{
    public class Mutation
    {
        public Mutation()
        { }

        private QueryException errorQueryBuilder( SqlException sqlException, DbUpdateException e, string message, params object[] args)
        {
            return new QueryException(
                       ErrorBuilder.New()
                           .SetMessage(message, args)
                           .SetCode(sqlException.Number.ToString())
                           .SetExtension("DatabaseMessage", e.InnerException.Message)
                           .Build());
        }

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

            var result = dBContext.Region.Add(region);

            try
            {
                await dBContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {

                var sqlException = e.GetBaseException() as SqlException;

                if (sqlException != null)
                {

                    if (sqlException.Number.Equals(2627))
                    {
                        throw errorQueryBuilder(
                            sqlException,
                            e,
                            "The region '{0}', with country '{1}' already exists.", name, country);

                    } 
                    else if (sqlException.Number.Equals(547))
                    {

                        throw errorQueryBuilder(
                            sqlException,
                            e,
                            "The country with name '{0}' doesn't exists", country);
                    }
                    else if (sqlException.Number.Equals(2628))
                    {

                        throw errorQueryBuilder(
                            sqlException,
                            e,
                            "The name specified is too long, try to use a name under 100 chars long.");
                    }
                    else
                    {
                        throw errorQueryBuilder(
                            sqlException,
                            e,
                            "Unknown Error");
                    }

                }


            }

            return result.Entity;
        }


        public Region updateRegion(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string oldName,
            [GraphQLNonNullType] string oldCountry,
            string newName,
            string newCountry)
        {

            if (string.IsNullOrEmpty(oldName))
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage("The name can't be empty.")
                        .SetCode("NAME_EMPTY")
                        .Build());
            }

            if (string.IsNullOrEmpty(oldCountry))
            {
                throw new QueryException(
                    ErrorBuilder.New()
                        .SetMessage("The country name can't be empty.")
                        .SetCode("COUNTRY_NAME_EMPTY")
                        .Build());
            }

            if (string.IsNullOrEmpty(newCountry) && string.IsNullOrEmpty(newName))
            {
                return null;
            }

            var region = new Region
            {
                Name = "TESTING",
                Country = "TESTING"
            };

            return region;

        }


    }
}
