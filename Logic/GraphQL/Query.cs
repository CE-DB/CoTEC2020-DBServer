using CoTEC_Server.Database;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace CoTEC_Server.Logic.GraphQL
{
    public class Query
    {
        public Query()
        { }

        /// <summary>
        /// Get the total of cases for a certain country or global count.
        /// </summary>
        /// <param name="countryName">Filter results by country name.</param>
        public async Task<Types.Location> totalCountCases([Service] SQLServerContext dbCont, string countryName)
        {
            if (countryName == null)
            {
                return await dbCont.Locations.FromSqlRaw("").FirstOrDefaultAsync();
            }

            return await dbCont.Locations.FromSqlRaw("").FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get contention measures for a certain country.
        /// </summary>
        /// <param name="countryName">Filter results by country name.</param>
        public async Task<Types.Location> contentionMeasuresActive([Service] SQLServerContext dbCont, string countryName)
        {
            if (countryName == null)
            {
                return await dbCont.Locations.FromSqlRaw("").FirstOrDefaultAsync();
            }

            return await dbCont.Locations.FromSqlRaw("").FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get countries available in database.
        /// </summary>
        /// <param name="countryName">Filter results by country name.</param>
        public async Task<List<Types.Country>> countries([Service] SQLServerContext dbCont, string countryName)
        {
            if (countryName == null)
            {
                return await dbCont.Countries.FromSqlRaw("").ToListAsync();
            }

            return await dbCont.Countries.FromSqlRaw("").ToListAsync();
        }

        /// <summary>
        /// Get regions available in database.
        /// </summary>
        /// <param name="name">Filter results by region name.</param>
        [Authorize(Policy = Constants.AdminPolicyName)]
        public async Task<List<Types.Region>> regions([Service] SQLServerContext dbCont, string name)
        {
            if (name == null)
            {
                return await dbCont.Regions.FromSqlRaw("").ToListAsync();
            }

            return await dbCont.Regions.FromSqlRaw("").ToListAsync();
        }

        /// <summary>
        /// Get pathologies available in database.
        /// </summary>
        /// <param name="name">Filter results by pathology name.</param>
        [Authorize(Policy = Constants.AdminPolicyName)]
        public async Task<List<Types.Pathology>> pathologies([Service] SQLServerContext dbCont, string name)
        {
            if (name == null)
            {
                return await dbCont.Pathologies.FromSqlRaw("").ToListAsync();
            }

            return await dbCont.Pathologies.FromSqlRaw("").ToListAsync();
        }

        /// <summary>
        /// Get patient states available in database.
        /// </summary>
        /// <param name="name">Filter results by patient state name.</param>
        [Authorize(Policy = Constants.AdminPolicyName)]
        public async Task<List<Types.PatientState>> patientStates([Service] SQLServerContext dbCont, string name)
        {
            if (name == null)
            {
                return await dbCont.PatientStates.FromSqlRaw("").ToListAsync();
            }

            return await dbCont.PatientStates.FromSqlRaw("").ToListAsync();
        }

        /// <summary>
        /// Get health centers available in database.
        /// </summary>
        /// <param name="name">Filter results by hospital name.</param>
        [Authorize(Policy = Constants.AdminPolicyName)]
        public async Task<List<Types.Healthcenter>> Healthcenters([Service] SQLServerContext dbCont, string name)
        {
            if (name == null)
            {
                return await dbCont.HealthCenters.FromSqlRaw("").ToListAsync();
            }

            return await dbCont.HealthCenters.FromSqlRaw("").ToListAsync();
        }

        /// <summary>
        /// Get sanitary measures available in database.
        /// </summary>
        /// <param name="name">Filter results by measure name.</param>
        [Authorize(Policy = Constants.AdminPolicyName)]
        public async Task<List<Types.SanitaryMeasure>> SanitaryMeasures([Service] SQLServerContext dbCont, string name)
        {
            if (name == null)
            {
                return await dbCont.SanitaryMeasures.FromSqlRaw("").ToListAsync();
            }

            return await dbCont.SanitaryMeasures.FromSqlRaw("").ToListAsync();
        }

        /// <summary>
        /// Get contaiment measures available in database.
        /// </summary>
        /// <param name="name">Filter results by measure name.</param>
        [Authorize(Policy = Constants.AdminPolicyName)]
        public async Task<List<Types.ContentionMeasure>> ContentionMeasures([Service] SQLServerContext dbCont, string name)
        {
            if (name == null)
            {
                return await dbCont.ContentionMeasures.FromSqlRaw("").ToListAsync();
            }

            return await dbCont.ContentionMeasures.FromSqlRaw("").ToListAsync();
        }

        /// <summary>
        /// Get medications available in database.
        /// </summary>
        /// <param name="name">Filter results by medication name.</param>
        [Authorize(Policy = Constants.AdminPolicyName)]
        public async Task<List<Types.Medication>> medications([Service] SQLServerContext dbCont, string name)
        {
            if (name == null)
            {
                return await dbCont.Medications.FromSqlRaw("").ToListAsync();
            }

            return await dbCont.Medications.FromSqlRaw("").ToListAsync();
        }

        /// <summary>
        /// Get patients available in database.
        /// </summary>
        /// <param name="id">Filter results by identification number of patient.</param>
        [Authorize(Policy = Constants.HealthCenterPolicyName)]
        public async Task<List<Types.Patient>> patients([Service] SQLServerContext dbCont, string? id)
        {
            if (id == null)
            {
                return await dbCont.Patients.FromSqlRaw("").ToListAsync();
            }

            return await dbCont.Patients.FromSqlRaw("").ToListAsync();
        }

        /// <summary>
        /// Get contacts available in database.
        /// </summary>
        /// <param name="id">Filter results by identification number of contact.</param>
        [Authorize(Policy = Constants.HealthCenterPolicyName)]
        public async Task<List<Types.Patient>> contacts([Service] SQLServerContext dbCont, int? id)
        {
            return null;
        }

        /// <summary>
        /// Get patients Report
        /// </summary>
        /// <returns>
        /// Endpoint string for download PDF file.
        /// </returns>
        [Authorize(Policy = Constants.HealthCenterPolicyName)]
        public async Task<string> patientsReport([Service] SQLServerContext dbCont)
        {
            return null;
        }

        /// <summary>
        /// Get new cases and deaths Report
        /// </summary>
        /// <returns>
        /// Endpoint string for download PDF file.
        /// </returns>
        [Authorize(Policy = Constants.HealthCenterPolicyName)]
        public async Task<string> newCasesReport([Service] SQLServerContext dbCont)
        {
            return null;
        }

    }
}
