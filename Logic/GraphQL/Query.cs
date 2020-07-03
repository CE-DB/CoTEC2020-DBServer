using CoTEC_Server.DBModels;
using CoTEC_Server.Logic.GraphQL.Types;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoTEC_Server.Logic.GraphQL
{
    public class Query
    {
        public Query()
        { }

        /// <summary>
        /// Get the continents available
        /// </summary>
        /// <param name="name">Filter results by name.</param>
        [GraphQLType(typeof(NonNullType<ListType<NonNullType<ContinentType>>>))]
        public async Task<List<Continent>> continents(
            [Service] CoTEC_DBContext dbCont,
            string name)
        {
            if (name == null)
            {
                return await dbCont.Continent.Include(s => s.Country).ToListAsync();
            }

            return await dbCont.Continent.Where(s => s.Name.Contains(name.Trim())).Include(s => s.Country).ToListAsync();
        }


        /// <summary>
        /// Get the continents available
        /// </summary>
        /// <param name="name">Filter results by name.</param>
        [GraphQLType(typeof(NonNullType<ListType<NonNullType<RegionType>>>))]
        public async Task<List<Region>> regions([Service] CoTEC_DBContext dbCont, string name)
        {
            if (name == null)
            {
                return await dbCont.Region
                    .Include(s => s.CountryNavigation)
                        .ThenInclude(c => c.ContinentNavigation)
                    .ToListAsync();
            }

            return await dbCont.Region
                .Where(s => s.Name.Contains(name.Trim()))
                .Include(s => s.CountryNavigation)
                        .ThenInclude(c => c.ContinentNavigation)
                .ToListAsync();
        }


        /// <summary>
        /// Get the countries available
        /// </summary>
        /// <param name="name">Filter results by name.</param>
        [GraphQLType(typeof(NonNullType<ListType<NonNullType<CountryType>>>))]
        public async Task<List<Country>> countries([Service] CoTEC_DBContext dbCont, string name)
        {
            if (name == null)
            {
                return await dbCont.Country
                    .Include(s => s.Region)
                    .Include(s => s.ContinentNavigation)
                    .ToListAsync();
            }

            return await dbCont.Country
                .Where(s => s.Name.Contains(name.Trim()))
                .Include(s => s.Region)
                .Include(s => s.ContinentNavigation)
                .ToListAsync();
        }

        /// <summary>
        /// Get the medications available
        /// </summary>
        /// <param name="name">Filter results by name.</param>
        [GraphQLType(typeof(NonNullType<ListType<NonNullType<MedicationType>>>))]
        //[Authorize(Policy = Constants.AdminPolicyName)]
        public async Task<List<Medication>> medications([Service] CoTEC_DBContext dbCont, string name)
        {
            if (name == null)
            {
                return await dbCont.Medication.ToListAsync();
            }

            return await dbCont.Medication.Where(s => s.Medicine.Contains(name.Trim())).ToListAsync();
        }

        /// <summary>
        /// Get visit events of contacts to patients
        /// </summary>
        /// <param name="patientId">Filter results by identification of patient.</param>
        [GraphQLType(typeof(NonNullType<ListType<NonNullType<ContactVisitType>>>))]
       // [Authorize(Policy = Constants.HealthCenterPolicyName)]
        public async Task<List<PatientContact>> contactVisits([Service] CoTEC_DBContext dbCont, string patientId)
        {
            if (patientId == null)
            {
                return await dbCont.PatientContact
                    .Include(s => s.Contact)
                    .Include(s => s.Patient)
                    .ToListAsync();
            }

            return await dbCont.PatientContact
                .Where(s => s.PatientId.Contains(patientId.Trim()))
                .Include(s => s.Contact)
                .Include(s => s.Patient)
                .ToListAsync();
        }

        /// <summary>
        /// Get the pathologies available
        /// </summary>
        /// <param name="name">Filter results by name.</param>
        [GraphQLType(typeof(NonNullType<ListType<NonNullType<PathologyType>>>))]
       // [Authorize(Policy = Constants.AdminPolicyName)]
        public async Task<List<Pathology>> pathologies([Service] CoTEC_DBContext dbCont, string name)
        {
            if (name == null)
            {
                return await dbCont.Pathology.ToListAsync();
            }

            return await dbCont.Pathology.Where(s => s.Name.Contains(name.Trim())).ToListAsync();
        }

        /// <summary>
        /// Get the contention measures available
        /// </summary>
        /// <param name="name">Filter results by name.</param>
        [GraphQLType(typeof(NonNullType<ListType<NonNullType<ContentionMeasureType>>>))]
       // [Authorize(Policy = Constants.AdminPolicyName)]
        public async Task<List<ContentionMeasure>> contentionMeasures([Service] CoTEC_DBContext dbCont, string name)
        {
            if (name == null)
            {
                return await dbCont.ContentionMeasure.ToListAsync();
            }

            return await dbCont.ContentionMeasure
                .Where(t => t.Name.Contains(name.Trim()))
                .ToListAsync();
        }

        /// <summary>
        /// Get the sanitary measures available
        /// </summary>
        /// <param name="name">Filter results by name.</param>
        [GraphQLType(typeof(NonNullType<ListType<NonNullType<SanitaryMeasureType>>>))]
        //[Authorize(Policy = Constants.AdminPolicyName)]
        public async Task<List<SanitaryMeasure>> sanitaryMeasures([Service] CoTEC_DBContext dbCont, string name)
        {
            if (name == null)
            {
                return await dbCont.SanitaryMeasure.ToListAsync();
            }

            return await dbCont.SanitaryMeasure
                .Where(t => t.Name.Contains(name.Trim()))
                .ToListAsync();
        }

        /// <summary>
        /// Get the patients available
        /// </summary>
        /// <param name="id">Filter results by name.</param>
        [GraphQLType(typeof(NonNullType<ListType<NonNullType<PatientType>>>))]
        //[Authorize(Policy = Constants.HealthCenterPolicyName)]
        public async Task<List<Patient>> patients([Service] CoTEC_DBContext dbCont, string id)
        {
            if (id == null)
            {
                return await dbCont.Patient
                    .Include(s => s.RegionNavigation)
                        .ThenInclude(r => r.CountryNavigation)
                            .ThenInclude(r => r.ContinentNavigation)
                    .ToListAsync();
            }

            return await dbCont.Patient
                .Where(t => t.Identification.Contains(id.Trim()))
                .Include(s => s.RegionNavigation)
                    .ThenInclude(r => r.CountryNavigation)
                        .ThenInclude(r => r.ContinentNavigation)
                .ToListAsync();
        }

        /// <summary>
        /// Get the patients available
        /// </summary>
        /// <param name="id">Filter results by name.</param>
        [GraphQLType(typeof(NonNullType<ListType<NonNullType<ContactType>>>))]
        //[Authorize(Policy = Constants.HealthCenterPolicyName)]
        public async Task<List<Contact>> contacts([Service] CoTEC_DBContext dbCont, string id)
        {
            if (id == null)
            {
                return await dbCont.Contact
                    .Include(s => s.RegionNavigation)
                        .ThenInclude(r => r.CountryNavigation)
                            .ThenInclude(r => r.ContinentNavigation)
                    .ToListAsync();
            }

            return await dbCont.Contact
                .Where(t => t.Identification.Equals(id))
                .Include(s => s.RegionNavigation)
                    .ThenInclude(r => r.CountryNavigation)
                        .ThenInclude(r => r.ContinentNavigation)
                .ToListAsync();
        }

        /// <summary>
        /// Get the patient states available
        /// </summary>
        [GraphQLType(typeof(NonNullType<ListType<NonNullType<StringType>>>))]
        //[Authorize(Policy = Constants.AdminPolicyName)]
        public List<string> patientStates([Service] CoTEC_DBContext dbCont)
        {

            var data = dbCont.PatientState.ToList();

            List<string> result = new List<string>();

            foreach (var elm in data)
            {
                result.Add(elm.Name);

            }

            return result;
        }

        /// <summary>
        /// Get the patient states available
        /// </summary>
        [GraphQLType(typeof(NonNullType<ListType<NonNullType<HealthCenterType>>>))]
        //[Authorize(Policy = Constants.AdminPolicyName)]
        public async Task<List<Hospital>> healthCenters([Service] CoTEC_DBContext dbCont, string name, string region, string country)
        {

            if (name == null || region == null || country == null)
            {
                return await dbCont.Hospital
                .Include(s => s.ManagerNavigation)
                .Include(s => s.RegionNavigation)
                    .ThenInclude(r => r.CountryNavigation)
                        .ThenInclude(r => r.ContinentNavigation)
                .ToListAsync();

            }

            return await dbCont.Hospital
                .Where(s => s.Name.Contains(name.Trim()) && 
                s.Region.Contains(region.Trim()) &&
                s.Country.Contains(country.Trim()))
                .Include(s => s.ManagerNavigation)
                .Include(s => s.RegionNavigation)
                    .ThenInclude(r => r.CountryNavigation)
                        .ThenInclude(r => r.ContinentNavigation)
                .ToListAsync();


        }


        /// <summary>
        /// Get the patient states available
        /// </summary>
        [GraphQLType(typeof(NonNullType<LocationType>))]
        public async Task<Increment> globalGeneralReport([Service] CoTEC_DBContext dbCont)
        {

            return await dbCont.Increment
                .FromSqlRaw("SELECT SUM(Infected) AS totalInfected, SUM(Cured) AS totalRecovered, SUM(Dead) AS totalDeceased, SUM(Active) AS totalActive " +
                "FROM user_public.Population")
                .FirstOrDefaultAsync();
        }


        /// <summary>
        /// Get the patient states available
        /// </summary>
        [GraphQLType(typeof(CountryLocationType))]
        public async Task<CountryLocation> countryGeneralReport([Service] CoTEC_DBContext dbCont, [GraphQLNonNullType] string country)
        {

            return await dbCont.CountryLocation
                .FromSqlRaw("SELECT country_Name AS name, SUM(Infected) AS totalInfected, SUM(Cured) AS totalRecovered, SUM(Dead) AS totalDeceased, SUM(Active) AS totalActive " +
                "FROM user_public.Population " +
                "WHERE country_Name = {0} " +
                "GROUP BY country_Name", country)
                .FirstOrDefaultAsync();
        }
    }
}
