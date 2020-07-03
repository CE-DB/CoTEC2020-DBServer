using CoTEC_Server.DBModels;
using CoTEC_Server.Logic.GraphQL.Types;
using CoTEC_Server.Logic.GraphQL.Types.Input;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace CoTEC_Server.Logic.GraphQL
{
    public class Mutation
    {
        public Mutation()
        { }

        private QueryException ErrorQueryBuilder( 
            string code, 
            string message, 
            params object[]? args)
        {
            return new QueryException(
                       ErrorBuilder.New()
                           .SetMessage(message, args)
                           .SetCode(code)
                           .SetExtension("DatabaseMessage", "NO ERROR")
                           .Build());
        }

        private QueryException ErrorQueryBuilder(SqlException sqlException,
            string message,
            params object[] args)
        {
            return new QueryException(
                       ErrorBuilder.New()
                           .SetMessage(message, args)
                           .SetCode(sqlException.Number.ToString())
                           .SetExtension("DatabaseMessage", sqlException.Message)
                           .Build());
        }

        private IError CustomErrorBuilder(
            string code,
            string message,
            params object[]? args)
        {
            return ErrorBuilder.New()
                           .SetMessage(message, args)
                           .SetCode(code)
                           .SetExtension("DatabaseMessage", "NO ERROR")
                           .Build();
        }

        private IError CustomErrorBuilder(SqlException sqlException,
            string message,
            params object[] args)
        {
            return ErrorBuilder.New()
                           .SetMessage(message, args)
                           .SetCode(sqlException.Number.ToString())
                           .SetExtension("DatabaseMessage", sqlException.Message)
                           .Build();
        }

        [GraphQLType(typeof(RegionType))]
        public async Task<Region> addRegion(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string name,
            [GraphQLNonNullType] string country)
        {

            if (string.IsNullOrEmpty(name))
            {

                throw ErrorQueryBuilder(
                    "NAME_EMPTY",
                    "The name can't be empty."
                    );
                
            }

            if (string.IsNullOrEmpty(country))
            {
                throw ErrorQueryBuilder(
                    "COUNTRY_NAME_EMPTY",
                    "The country name can't be empty."
                    );
            }


            var region = new Region
            {

                Name = name,
                Country = country

            };

            dBContext.Region.Add(region);

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
                        throw ErrorQueryBuilder(
                            sqlException,
                            "The region '{0}', with country '{1}' already exists.", name, country);

                    } 
                    else if (sqlException.Number.Equals(547))
                    {

                        throw ErrorQueryBuilder(
                            sqlException,
                            "The country with name '{0}' doesn't exists", country);
                    }
                    else if (sqlException.Number.Equals(2628))
                    {

                        throw ErrorQueryBuilder(
                            sqlException,
                            "The name specified is too long, try to use a name under 100 chars long.");
                    }
                    else
                    {
                        throw ErrorQueryBuilder(
                            sqlException,
                            "Unknown Error");
                    }

                }


            }
            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return await dBContext.Region
                .Where(s => s.Name.Contains(name.Trim()))
                .Include(s => s.CountryNavigation)
                        .ThenInclude(c => c.ContinentNavigation)
                .FirstOrDefaultAsync();
        }
        
        [GraphQLType(typeof(RegionType))]
        public async Task<Region> updateRegion(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string oldName,
            [GraphQLNonNullType] string oldCountry,
            string newName,
            string newCountry)
        {

            if (string.IsNullOrEmpty(oldName))
            {
                throw ErrorQueryBuilder(
                    "NAME_EMPTY",
                    "The name can't be empty.");
            }

            if (string.IsNullOrEmpty(oldCountry))
            {
                throw ErrorQueryBuilder(
                    "COUNTRY_NAME_EMPTY",
                    "The country name can't be empty.");
            }

            if (string.IsNullOrEmpty(newCountry) && string.IsNullOrEmpty(newName))
            {
                throw ErrorQueryBuilder(
                    "NO_CHANGES_AVAILABLE",
                    "Nothing to change.");
            }

            var region = new Region
            {
                Name = newName,
                Country = newCountry
            };

            try
            {
                int rows = await dBContext.Database.ExecuteSqlRawAsync(
                    "UPDATE admin.region " +
                    "SET Name = {0}, country = {1} " +
                    "WHERE Name = {2} AND country = {3}", newName, newCountry, oldName, oldCountry);

                if (rows < 1)
                {
                    throw ErrorQueryBuilder(
                        "REGION_NOT_FOUND",
                        "The region with name '{0}' and country '{1}' doesn't exists.", oldName, oldCountry);
                }

                await dBContext.SaveChangesAsync();
            }

            catch (SqlException e)
            {
                if (e.Number.Equals(547))
                {
                    throw ErrorQueryBuilder(
                            e,
                            "You tried to change a region with a related hospital, contact or patient. " +
                            "Please delete all elements related before deleting this region. " +
                            "¿Or maybe you inserted a wrong new country name?");
                }
                else if (e.Number.Equals(2627))
                {
                    throw ErrorQueryBuilder(
                            e,
                            "The region name '{0}' and country name '{1}' already exists.", newName, newCountry);
                }
                else if (e.Number.Equals(515))
                {
                    throw ErrorQueryBuilder(
                            e,
                            "The region needs to have a name and a country, please insert both values.");
                }
                else
                {
                    throw ErrorQueryBuilder(
                            e,
                            "Unknown error.");
                }

            }
            catch (QueryException e)
            {
                throw e;
            }

            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return await dBContext.Region
                .Where(s => s.Name.Contains(string.IsNullOrEmpty(newName) ? oldName : newName))
                .Include(s => s.CountryNavigation)
                        .ThenInclude(c => c.ContinentNavigation)
                .FirstOrDefaultAsync();
        }
        
        [GraphQLType(typeof(RegionType))]
        public async Task<Region> deleteRegion(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string name,
            [GraphQLNonNullType] string country)
        {

            if (string.IsNullOrEmpty(name))
            {
                throw ErrorQueryBuilder(
                    "NAME_EMPTY",
                    "The name can't be empty.");
            }

            if (string.IsNullOrEmpty(country))
            {
                throw ErrorQueryBuilder(
                    "COUNTRY_NAME_EMPTY",
                    "The country name can't be empty.");
            }

            try
            {
                int rows = await dBContext.Database.ExecuteSqlRawAsync(
                    "DELETE FROM admin.region " +
                    "WHERE Name = {0} AND country = {1}", name, country);

                if (rows < 1)
                {
                    throw ErrorQueryBuilder(
                        "REGION_NOT_FOUND",
                        "The region with name '{0}' and country '{1}' doesn't exists.", name, country);
                }

                await dBContext.SaveChangesAsync();
            }
            catch (SqlException e)
            {
                if (e.Number.Equals(547))
                {
                    throw ErrorQueryBuilder(
                            e,
                            "You tried to delete a region with a related hospital, contact or patient. " +
                            "Please delete all elements related before deleting this region. " +
                            "¿Or maybe you inserted a wrong new country name?");
                }

                else
                {
                    throw ErrorQueryBuilder(
                            e,
                            "Unknown error.");
                }

            }
            catch (QueryException e)
            {
                throw e;
            }

            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return new Region {
                Name = name,
                CountryNavigation = dBContext.Country
                .Where(c => c.Name.Equals(country))
                .Include(c => c.ContinentNavigation)
                .FirstOrDefault()
            };

        }
        
        [GraphQLType(typeof(PatientType))]
        public async Task<Patient> addPatient(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] CreatePatient patient)
        {

            if (string.IsNullOrEmpty(patient.identification))
            {
                throw ErrorQueryBuilder(
                    "IDENTIFICATION_EMPTY",
                    "The identification code can't be empty."
                    );
            }

            try
            {
                if (string.IsNullOrEmpty(patient.nationality))
                {
                    await dBContext.Database.ExecuteSqlRawAsync(
                    "EXECUTE healthcare.Patient_Existence_Checker @identification = {0}, " +
                    " @firstName = {1}," +
                    " @lastName = {2}," +
                    " @region = {3}," +
                    " @country = {4}," +
                    " @age = {5}," +
                    " @intensiveCareUnite = {6}," +
                    " @hospitalized = {7}," +
                    " @state = {8}," +
                    " @Entrance_Date = {9}",
                    patient.identification,
                    string.IsNullOrEmpty(patient.firstName) ? null : patient.firstName,
                    string.IsNullOrEmpty(patient.lastName) ? null : patient.lastName,
                    string.IsNullOrEmpty(patient.region) ? null : patient.region,
                    string.IsNullOrEmpty(patient.country) ? null : patient.country,
                    patient.age,
                    patient.intensiveCareUnite,
                    patient.hospitalized,
                    string.IsNullOrEmpty(patient.state) ? null : patient.state,
                    patient.dateEntrance);

                    /*await dBContext.Database.ExecuteSqlRawAsync(
                    "INSERT INTO healthcare.Patient ([identification]" +
                    ",[firstName]" +
                    ",[lastName]" +
                    ",[region]" +
                    ",[country]" +
                    ",[age]" +
                    ",[intensiveCareUnite]" +
                    ",[hospitalized]" +
                    ",[state]" +
                    ",[Date_Entrance]) " +
                    "VALUES (" +
                    "{0}," +
                    "{1}," +
                    "{2}," +
                    "{3}," +
                    "{4}," +
                    "{5}," +
                    "{6}," +
                    "{7}," +
                    "{8}," +
                    "{9})",
                    patient.identification,
                    string.IsNullOrEmpty(patient.firstName) ? null : patient.firstName,
                    string.IsNullOrEmpty(patient.lastName) ? null : patient.lastName,
                    patient.region,
                    patient.country,
                    patient.age,
                    patient.intensiveCareUnite,
                    patient.hospitalized,
                    string.IsNullOrEmpty(patient.state) ? null : patient.state,
                    patient.dateEntrance);*/
                } 
                else
                {
                    await dBContext.Database.ExecuteSqlRawAsync(
                    "EXECUTE healthcare.Patient_Existence_Checker @identification = {0}, " +
                    " @firstName = {1}," +
                    " @lastName = {2}," +
                    " @region = {3}," +
                    " @nationality = {4}," +
                    " @country = {5}," +
                    " @age = {6}," +
                    " @intensiveCareUnite = {7}," +
                    " @hospitalized = {8}," +
                    " @state = {9}," +
                    " @Entrance_Date = {10}",
                    patient.identification,
                    string.IsNullOrEmpty(patient.firstName) ? null : patient.firstName,
                    string.IsNullOrEmpty(patient.lastName) ? null : patient.lastName,
                    string.IsNullOrEmpty(patient.region) ? null : patient.region,
                    string.IsNullOrEmpty(patient.nationality) ? null : patient.nationality,
                    string.IsNullOrEmpty(patient.country) ? null : patient.country,
                    patient.age,
                    patient.intensiveCareUnite,
                    patient.hospitalized,
                    string.IsNullOrEmpty(patient.state) ? null : patient.state,
                    patient.dateEntrance);


                    /*await dBContext.Database.ExecuteSqlRawAsync(
                    "INSERT INTO healthcare.Patient ([identification]" +
                    ",[firstName]" +
                    ",[lastName]" +
                    ",[region]" +
                    ",[nationality]" +
                    ",[country]" +
                    ",[age]" +
                    ",[intensiveCareUnite]" +
                    ",[hospitalized]" +
                    ",[state]" +
                    ",[Date_Entrance]) " +
                    "VALUES (" +
                    "{0}," +
                    "{1}," +
                    "{2}," +
                    "{3}," +
                    "{4}," +
                    "{5}," +
                    "{6}," +
                    "{7}," +
                    "{8}," +
                    "{9}," +
                    "{10})",
                    patient.identification,
                    string.IsNullOrEmpty(patient.firstName) ? null : patient.firstName,
                    string.IsNullOrEmpty(patient.lastName) ? null : patient.lastName,
                    patient.region,
                    string.IsNullOrEmpty(patient.nationality) ? null : patient.nationality,
                    patient.country,
                    patient.age,
                    patient.intensiveCareUnite,
                    patient.hospitalized,
                    string.IsNullOrEmpty(patient.state) ? null : patient.state,
                    patient.dateEntrance);*/
                }
            }
            catch (SqlException sqlException)
            {
                if (sqlException.Number.Equals(2627))
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        "The patient with identification '{0}' already exists.", patient.identification);

                }
                else if (sqlException.Number.Equals(547))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The patient has a country, region or state that is invalid.");
                }
                else if (sqlException.Number.Equals(2628))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The identification specified is too long, try to use one under 50 chars long.");
                }
                else if (sqlException.Number.Equals(515))
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        "You must enter a value for all fields.");
                }
                else if (sqlException.Number.Equals(50006))
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        sqlException.Message);
                }
                else
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        "Unknown Error");
                }

            }
            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return await dBContext.Patient
                .Where(t => t.Identification.Contains(patient.identification.Trim()))
                .Include(s => s.RegionNavigation)
                    .ThenInclude(r => r.CountryNavigation)
                        .ThenInclude(r => r.ContinentNavigation)
                .FirstOrDefaultAsync();
        }
        
        [GraphQLType(typeof(PatientType))]
        public async Task<Patient> updatePatient(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string identification,
            [GraphQLNonNullType] UpdatePatient input)
        {

            List<IError> errors = new List<IError>();

            try
            {
                int rows = await dBContext.Database.ExecuteSqlRawAsync(
                    "EXECUTE healthcare.Patients_updater @identification = {0}, " +
                    " @newIdentification = {1}," +
                    " @firstName = {2}," +
                    " @lastName = {3}," +
                    " @region = {4}," +
                    " @nationality = {5}," +
                    " @country = {6}," +
                    " @age = {7}," +
                    " @intensiveCareUnite = {8}," +
                    " @hospitalized = {9}," +
                    " @state = {10}," +
                    " @Entrance_Date = {11}",
                    string.IsNullOrEmpty(identification) ? null : identification,
                    string.IsNullOrEmpty(input.identification) ? null : input.identification,
                    string.IsNullOrEmpty(input.firstName) ? null : input.firstName,
                    string.IsNullOrEmpty(input.lastName) ? null : input.lastName,
                    string.IsNullOrEmpty(input.region) ? null : input.region,
                    string.IsNullOrEmpty(input.nationality) ? null : input.nationality,
                    string.IsNullOrEmpty(input.country) ? null : input.country,
                    input.age.HasValue ? input.age.ToString() : null,
                    input.intensiveCareUnite.HasValue ? input.intensiveCareUnite.ToString() : null,
                    input.Hospitalized.HasValue ? input.Hospitalized.ToString() : null,
                    string.IsNullOrEmpty(input.state) ? null : input.state,
                    input.dateEntrance.HasValue ? input.dateEntrance.ToString() : null);

                if (rows < 1)
                {
                    throw ErrorQueryBuilder(
                        "NOTHING_TO_CHANGE",
                        "The identification code '{0}' doesn't match any patient."
                        , identification
                        );
                }
            }

            catch (SqlException e)
            {
                if (e.Number.Equals(547))
                {
                    errors.Add(CustomErrorBuilder(
                            e,
                            "You tried to change the identification code of patient with a related medication, contact or pathology. " +
                            "Please delete all elements related before deleting this patient. " +
                            "¿Or maybe you inserted the wrong country, patient state or region?"));
                }
                else if (e.Number.Equals(2627))
                {
                    errors.Add(CustomErrorBuilder(
                            e,
                            "You tried to insert an identification code that already exists."));
                }
                else if (e.Number.Equals(15600))
                {
                    throw ErrorQueryBuilder(
                            e,
                            "You must insert an identification code.");
                }
                else
                {
                    errors.Add(CustomErrorBuilder(
                            e,
                            "Unknown error."));
                }

            }
            catch (QueryException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new QueryException(ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            if (input.pathologies != null)
            {
                string currentPathology = "";

                /*dbPath - input > 0 => delete dbPath => loop no errors
                dbPath - input = 0 & dbPath.Count = input.Count => null => null
                dbPath - input = 0 & dbPath.Count < input.Count => insert input => loop with errors
                dbPath - input > 0 & dbPath.Count = input.Count => updatePatient dbPath*/


                ICollection<string> currentDB = dBContext.PatientPathology
                    .Where(p => p.PatientId.Equals(string.IsNullOrEmpty(input.identification) ? identification : input.identification))
                    .Select(s => new string(s.PathologyName))
                    .ToList();
                    //.ConvertAll<string>(s => s.PathologyName);

                ICollection<string> fromDB = currentDB.Except(input.pathologies).ToList();

                ICollection<string> inputPath = input.pathologies.Except(currentDB).ToList();

                if (fromDB.Count > 0 && currentDB.Count > input.pathologies.Count)
                {
                    foreach (var path in fromDB)
                    {
                        currentPathology = path;
                        try
                        {
                            await dBContext.Database.ExecuteSqlRawAsync(
                                "DELETE FROM healthcare.Patient_Pathology " +
                                "WHERE Pathology_Name = {0} " +
                                "AND Patient_Id = {1}"
                                ,path
                                ,string.IsNullOrEmpty(input.identification) ? identification : input.identification);
                        }
                        catch (SqlException sqlException)
                        {
                            if (sqlException.Number.Equals(547))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The pathology with name '{0}', doesn't exists.", currentPathology));
                            }
                            else if (sqlException.Number.Equals(2628))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The pathology name specified is too long, try to use one under 150 chars long."));
                            }
                            else if (sqlException.Number.Equals(515))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "You must enter a value for pathology name."));
                            }
                            else
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "Unknown Error"));
                            }

                        }
                        catch (Exception e)
                        {
                            errors.Add(ErrorBuilder.New()
                               .SetMessage("Unknown error")
                               .SetCode(e.GetType().FullName)
                               .SetExtension("DatabaseMessage", e.Message)
                               .Build());
                        }
                    }
                } 
                else if (fromDB.Count.Equals(0) && currentDB.Count < input.pathologies.Count)
                {
                    foreach (var path in inputPath)
                    {
                        currentPathology = path;
                        try
                        {
                            await dBContext.Database.ExecuteSqlRawAsync(
                                "INSERT INTO healthcare.Patient_Pathology(Pathology_Name, Patient_Id) " +
                                "VALUES (" +
                                "{0}," +
                                "{1})"
                                , path
                                , string.IsNullOrEmpty(input.identification) ? identification : input.identification);
                        }
                        catch (SqlException sqlException)
                        {
                            if (sqlException.Number.Equals(547))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The pathology with name '{0}', doesn't exists.", currentPathology));
                            }
                            else if (sqlException.Number.Equals(2628))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The pathology name specified is too long, try to use one under 150 chars long."));
                            }
                            else if (sqlException.Number.Equals(2627))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The patient already has the pathology '{0}' assigned.", currentPathology));
                            }
                            else if (sqlException.Number.Equals(515))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "You must enter a value for pathology name."));
                            }
                            else
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "Unknown Error"));
                            }

                        }
                        catch (Exception e)
                        {
                            errors.Add(ErrorBuilder.New()
                               .SetMessage("Unknown error")
                               .SetCode(e.GetType().FullName)
                               .SetExtension("DatabaseMessage", e.Message)
                               .Build());
                        }
                    }
                }
                else if (fromDB.Count > 0 && currentDB.Count.Equals(input.pathologies.Count))
                {
                    foreach (var path in fromDB.Zip(inputPath, Tuple.Create))
                    {
                        currentPathology = path.Item2;
                        try
                        {
                            await dBContext.Database.ExecuteSqlRawAsync(
                                "UPDATE healthcare.Patient_Pathology " +
                                "SET Pathology_Name = {0} " +
                                "WHERE Pathology_Name = {1} " +
                                "AND Patient_Id = {2}"
                                , path.Item2
                                , path.Item1
                                , string.IsNullOrEmpty(input.identification) ? identification : input.identification);
                        }
                        catch (SqlException sqlException)
                        {
                            if (sqlException.Number.Equals(547))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The pathology with name '{0}', doesn't exists.", currentPathology));
                            }
                            else if (sqlException.Number.Equals(2628))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The pathology name specified is too long, try to use one under 150 chars long."));
                            }
                            else if (sqlException.Number.Equals(515))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "You must enter a value for pathology name."));
                            }
                            else
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "Unknown Error"));
                            }

                        }
                        catch (Exception e)
                        {
                            errors.Add(ErrorBuilder.New()
                               .SetMessage("Unknown error")
                               .SetCode(e.GetType().FullName)
                               .SetExtension("DatabaseMessage", e.Message)
                               .Build());
                        }
                    }
                }
            }

            if (input.medication != null)
            {
                string currentMedication = "";

                /*dbPath - input > 0 => delete dbPath => loop no errors
                dbPath - input = 0 & dbPath.Count = input.Count => null => null
                dbPath - input = 0 & dbPath.Count < input.Count => insert input => loop with errors
                dbPath - input > 0 & dbPath.Count = input.Count => updatePatient dbPath*/


                ICollection<string> currentDB = dBContext.PatientMedication
                    .Where(p => p.PatientId.Equals(string.IsNullOrEmpty(input.identification) ? identification : input.identification))
                    .Select(s => new string(s.Medication))
                    .ToList();
                //.ConvertAll<string>(s => s.PathologyName);

                ICollection<string> fromDB = currentDB.Except(input.medication).ToList();

                ICollection<string> inputMed = input.medication.Except(currentDB).ToList();

                if (fromDB.Count > 0 && currentDB.Count > input.medication.Count)
                {
                    foreach (var med in fromDB)
                    {
                        currentMedication = med;
                        try
                        {
                            await dBContext.Database.ExecuteSqlRawAsync(
                                "DELETE FROM healthcare.Patient_Medication " +
                                "WHERE Medication = {0} " +
                                "AND Patient_Id = {1}"
                                , med
                                , string.IsNullOrEmpty(input.identification) ? identification : input.identification);
                        }
                        catch (SqlException sqlException)
                        {
                            if (sqlException.Number.Equals(547))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The medication with name '{0}', doesn't exists.", currentMedication));
                            }
                            else if (sqlException.Number.Equals(2628))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The medication name specified is too long, try to use one under 80 chars long."));
                            }
                            else if (sqlException.Number.Equals(515))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "You must enter a value for medication name."));
                            }
                            else
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "Unknown Error"));
                            }

                        }
                        catch (Exception e)
                        {
                            errors.Add(ErrorBuilder.New()
                               .SetMessage("Unknown error")
                               .SetCode(e.GetType().FullName)
                               .SetExtension("DatabaseMessage", e.Message)
                               .Build());
                        }
                    }
                }
                else if (fromDB.Count.Equals(0) && currentDB.Count < input.medication.Count)
                {
                    foreach (var med in inputMed)
                    {
                        currentMedication = med;
                        try
                        {
                            await dBContext.Database.ExecuteSqlRawAsync(
                                "INSERT INTO healthcare.Patient_Medication(Medication, Patient_Id) " +
                                "VALUES (" +
                                "{0}," +
                                "{1})"
                                , med
                                , string.IsNullOrEmpty(input.identification) ? identification : input.identification);
                        }
                        catch (SqlException sqlException)
                        {
                            if (sqlException.Number.Equals(547))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The medication with name '{0}', doesn't exists.", currentMedication));
                            }
                            else if (sqlException.Number.Equals(2628))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The medication name specified is too long, try to use one under 80 chars long."));
                            }
                            else if (sqlException.Number.Equals(2627))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The patient already has the medication '{0}' assigned.", currentMedication));
                            }
                            else if (sqlException.Number.Equals(515))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "You must enter a value for medication name."));
                            }
                            else
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "Unknown Error"));
                            }

                        }
                        catch (Exception e)
                        {
                            errors.Add(ErrorBuilder.New()
                               .SetMessage("Unknown error")
                               .SetCode(e.GetType().FullName)
                               .SetExtension("DatabaseMessage", e.Message)
                               .Build());
                        }
                    }
                }
                else if (fromDB.Count > 0 && currentDB.Count.Equals(input.medication.Count))
                {
                    foreach (var path in fromDB.Zip(inputMed, Tuple.Create))
                    {
                        currentMedication = path.Item2;
                        try
                        {
                            await dBContext.Database.ExecuteSqlRawAsync(
                                "UPDATE healthcare.Patient_Medication " +
                                "SET Medication = {0} " +
                                "WHERE Medication = {1} " +
                                "AND Patient_Id = {2}"
                                , path.Item2
                                , path.Item1
                                , string.IsNullOrEmpty(input.identification) ? identification : input.identification);
                        }
                        catch (SqlException sqlException)
                        {
                            if (sqlException.Number.Equals(547))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The medication with name '{0}', doesn't exists.", currentMedication));
                            }
                            else if (sqlException.Number.Equals(2628))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The medication name specified is too long, try to use one under 80 chars long."));
                            }
                            else if (sqlException.Number.Equals(515))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "You must enter a value for pathology name."));
                            }
                            else
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "Unknown Error"));
                            }

                        }
                        catch (Exception e)
                        {
                            errors.Add(ErrorBuilder.New()
                               .SetMessage("Unknown error")
                               .SetCode(e.GetType().FullName)
                               .SetExtension("DatabaseMessage", e.Message)
                               .Build());
                        }
                    }
                }
            }

            if (errors.Count > 0)
            {
                throw new QueryException(errors.AsEnumerable());
            }

            return await dBContext.Patient
                .Where(t => t.Identification.Equals(string.IsNullOrEmpty(input.identification) ? identification : input.identification))
                .Include(s => s.RegionNavigation)
                    .ThenInclude(r => r.CountryNavigation)
                        .ThenInclude(r => r.ContinentNavigation)
                .FirstOrDefaultAsync();
        }

       
        [GraphQLType(typeof(PatientType))]
        public async Task<Patient> deletePatient(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string identification)
        {
            if (string.IsNullOrEmpty(identification))
            {
                throw ErrorQueryBuilder(
                    "NAME_EMPTY",
                    "The name can't be empty.");
            }
            Patient p;

            try
            {
                p = await dBContext.Patient
                .Where(t => t.Identification.Equals(identification.Trim()))
                .Include(s => s.RegionNavigation)
                    .ThenInclude(r => r.CountryNavigation)
                        .ThenInclude(r => r.ContinentNavigation)
                .FirstOrDefaultAsync();

                int rows = await dBContext.Database.ExecuteSqlRawAsync(
                    "DELETE FROM healthcare.Patient " +
                    "WHERE identification = {0}", identification);

                if (rows < 1)
                {
                    throw ErrorQueryBuilder(
                        "PATIENT_NOT_FOUND",
                        "The patient with identification code '{0}' doesn't exists.", identification);
                }
            }
            catch (SqlException e)
            {
                if (e.Number.Equals(547))
                {
                    throw ErrorQueryBuilder(
                            e,
                            "You tried to delete a patient with a related medication, contact or pathology. " +
                            "Please delete all elements related before deleting this patient.");
                }

                else
                {
                    throw ErrorQueryBuilder(
                            e,
                            "Unknown error.");
                }

            }
            catch (QueryException e)
            {
                throw e;
            }

            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }
            
            return p;

        }

        
        [GraphQLType(typeof(ContactVisitType))]
        public async Task<PatientContact> addContactVisit(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string patientId,
            [GraphQLNonNullType] string contactId,
            [GraphQLNonNullType] DateTime visitDate)
        {

            if (string.IsNullOrEmpty(patientId))
            {

                throw ErrorQueryBuilder(
                    "PATIENT_ID_EMPTY",
                    "The patient id can't be empty."
                    );

            }

            if (string.IsNullOrEmpty(contactId))
            {
                throw ErrorQueryBuilder(
                    "CONTACT_ID_EMPTY",
                    "The contact id can't be empty."
                    );
            }

            var newVisit = new PatientContact
            {

                ContactId = contactId,
                 PatientId = patientId,
                 LastVisit = visitDate

            };

            dBContext.PatientContact.Add(newVisit);

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
                        throw ErrorQueryBuilder(
                            sqlException,
                            "The visit of contact '{0}' to patient '{1}', with date '{2}' already exists.", contactId, patientId, visitDate);

                    }
                    else if (sqlException.Number.Equals(547))
                    {

                        throw ErrorQueryBuilder(
                            sqlException,
                            "The patient (id: {0}) or contact (id: {1}) id doesn't exists", patientId, contactId);
                    }
                    else if (sqlException.Number.Equals(2628))
                    {

                        throw ErrorQueryBuilder(
                            sqlException,
                            "The identification code specified is too long, try to use one under 50 chars long.");
                    }
                    else
                    {
                        throw ErrorQueryBuilder(
                            sqlException,
                            "Unknown Error");
                    }

                }


            }
            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return await dBContext.PatientContact
                .Where(s => s.PatientId.Contains(patientId.Trim())
                            && s.ContactId.Equals(contactId.Trim())
                            && s.LastVisit.Equals(visitDate))
                .Include(s => s.Contact)
                .Include(s => s.Patient)
                .FirstOrDefaultAsync();
        }
        
        [GraphQLType(typeof(ContactVisitType))]
        public async Task<PatientContact> updateContactVisit(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string patientId,
            [GraphQLNonNullType] string contactId,
            [GraphQLNonNullType] DateTime visitDate,
            [GraphQLNonNullType] UpdateContactVisit input)
        {

            if (string.IsNullOrEmpty(patientId))
            {
                throw ErrorQueryBuilder(
                    "PATIENT_ID_EMPTY",
                    "The patient id can't be empty."
                    );
            }

            if (string.IsNullOrEmpty(contactId))
            {
                throw ErrorQueryBuilder(
                    "CONTACT_ID_EMPTY",
                    "The contact id can't be empty."
                    );
            }

            if (string.IsNullOrEmpty(input.contactId) && string.IsNullOrEmpty(input.patientId) && input.visitDate.HasValue)
            {
                throw ErrorQueryBuilder(
                    "NO_CHANGES_AVAILABLE",
                    "Nothing to change.");
            }

            try
            {
                int rows = await dBContext.Database.ExecuteSqlRawAsync(
                    "UPDATE healthcare.Patient_Contact " +
                    "SET Patient_Id = (CASE" +
                                    " WHEN {0} is null THEN Patient_Id " +
                                    " ELSE {0} END), " +
                    "Contact_Id = (CASE" +
                                    " WHEN {1} is null THEN Contact_Id " +
                                    " ELSE {1} END), " +
                    "last_visit = (CASE" +
                                    " WHEN {2} is null THEN last_visit " +
                                    " ELSE {2} END) " +
                    "WHERE Patient_Id = {3} AND Contact_Id = {4} AND last_visit = {5}",
                    string.IsNullOrEmpty(input.patientId) ? null : input.patientId,
                    string.IsNullOrEmpty(input.contactId) ? null : input.contactId,
                    input.visitDate.HasValue ? input.visitDate : null,
                    patientId, contactId, visitDate);

                if (rows < 1)
                {
                    throw ErrorQueryBuilder(
                        "CONTACT_VISIT_NOT_FOUND",
                        "The visit with patient ID '{0}', ccontact ID '{1}' and date '{2}' doesn't exists.", patientId, contactId, visitDate);
                }

                await dBContext.SaveChangesAsync();
            }

            catch (SqlException sqlException)
            {
                if (sqlException.Number.Equals(2627))
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        "The visit of contact '{0}' to patient '{1}', with date '{2}' already exists.", contactId, patientId, visitDate);

                }
                else if (sqlException.Number.Equals(547))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The patient (id: {0}) or contact (id: {1}) id doesn't exists", patientId, contactId);
                }
                else if (sqlException.Number.Equals(2628))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The identification code specified is too long, try to use one under 50 chars long.");
                }
                else
                {
                    throw ErrorQueryBuilder(
                            sqlException,
                            "Unknown error.");
                }

            }
            catch (QueryException e)
            {
                throw e;
            }

            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return await dBContext.PatientContact
                .Where(s => s.PatientId.Contains(string.IsNullOrEmpty(input.patientId) ? patientId : input.patientId)
                            && s.ContactId.Equals(string.IsNullOrEmpty(input.contactId) ? contactId : input.contactId)
                            && s.LastVisit.Equals(input.visitDate.HasValue ? input.visitDate : visitDate))
                .Include(s => s.Contact)
                .Include(s => s.Patient)
                .FirstOrDefaultAsync();
        }
        
        [GraphQLType(typeof(ContactVisitType))]
        public async Task<PatientContact> deleteContactVisit(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string patientId,
            [GraphQLNonNullType] string contactId,
            [GraphQLNonNullType] DateTime visitDate)
        {

            PatientContact p = await dBContext.PatientContact
                .Where(s => s.PatientId.Equals(patientId) && s.ContactId.Equals(contactId))
                .Include(s => s.Contact)
                .Include(s => s.Patient)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(patientId))
            {
                throw ErrorQueryBuilder(
                    "PATIENT_ID_EMPTY",
                    "The patient id can't be empty."
                    );
            }

            if (string.IsNullOrEmpty(contactId))
            {
                throw ErrorQueryBuilder(
                    "CONTACT_ID_EMPTY",
                    "The contact id can't be empty."
                    );
            }

            try
            {
                int rows = await dBContext.Database.ExecuteSqlRawAsync(
                    "DELETE FROM healthcare.Patient_Contact " +
                    "WHERE Patient_Id = {0} AND Contact_Id = {1} AND last_visit = {2}", patientId, contactId, visitDate);

                if (rows < 1)
                {
                    throw ErrorQueryBuilder(
                        "CONTACT_VISIT_NOT_FOUND",
                        "The visit with patient ID '{0}', ccontact ID '{1}' and date '{2}' doesn't exists.", patientId, contactId, visitDate);
                }

                await dBContext.SaveChangesAsync();
            }
            catch (SqlException e)
            {
                if (e.Number.Equals(547))
                {

                    throw ErrorQueryBuilder(
                        e,
                        "The patient (id: {0}) or contact (id: {1}) id doesn't exists", patientId, contactId);
                }

                else
                {
                    throw ErrorQueryBuilder(
                            e,
                            "Unknown error.");
                }

            }
            catch (QueryException e)
            {
                throw e;
            }

            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return p;

        }
        
        [GraphQLType(typeof(StringType))]
        public async Task<string> addPatientState(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string name)
        {

            if (string.IsNullOrEmpty(name))
            {

                throw ErrorQueryBuilder(
                    "STATE_NAME_EMPTY",
                    "The name can't be empty."
                    );

            }

            try
            {
                await dBContext.Database.ExecuteSqlRawAsync(
                    "INSERT INTO admin.Patient_State " +
                    "VALUES ({0})", name);
            }
            catch (SqlException sqlException)
            {
                if (sqlException.Number.Equals(2627))
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        "The patient state '{0}' already exists.", name);

                }
                else if (sqlException.Number.Equals(2628))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The name specified is too long, try to use one under 50 chars long.");
                }
                else
                {
                    throw ErrorQueryBuilder(
                            sqlException,
                            "Unknown error.");
                }

            }
            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return (await dBContext.PatientState.Where(p => p.Name.Equals(name)).FirstOrDefaultAsync()).Name;
        }
        
        [GraphQLType(typeof(StringType))]
        public async Task<string> updatePatientState(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string oldName,
            [GraphQLNonNullType] string newName)
        {

            if (string.IsNullOrEmpty(oldName))
            {
                throw ErrorQueryBuilder(
                    "STATE_NAME_EMPTY",
                    "I can't identifiy a state without a name."
                    );
            }

            if (string.IsNullOrEmpty(newName))
            {
                throw ErrorQueryBuilder(
                    "NO_CHANGES_AVAILABLE",
                    "Nothing to change.");
            }

            try
            {
                int rows = await dBContext.Database.ExecuteSqlRawAsync(
                    "UPDATE admin.Patient_State " +
                    "SET name = {0} " +
                    "WHERE name = {1}",
                    newName, oldName);

                if (rows < 1)
                {
                    throw ErrorQueryBuilder(
                        "PATIENT_STATE_NOT_FOUND",
                        "The patient state with name '{0}' doesn't exists.", oldName);
                }
            }

            catch (SqlException sqlException)
            {
                if (sqlException.Number.Equals(547))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The patient state has patients realted, please assign them another state before updating this one");
                }
                else if (sqlException.Number.Equals(2627))
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        "The patient state with name '{0}' already exists.", newName);

                }
                else if (sqlException.Number.Equals(2628))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The name specified is too long, try to use one under 50 chars long.");
                }
                else if (sqlException.Number.Equals(50005))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        sqlException.Message);
                }
                else
                {
                    throw ErrorQueryBuilder(
                            sqlException,
                            "Unknown error.");
                }

            }
            catch (QueryException e)
            {
                throw e;
            }

            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return await dBContext.PatientState
                .Where(p => p.Name.Equals(newName))
                .Select(s => new string(s.Name))
                .FirstOrDefaultAsync();
        }

        [GraphQLType(typeof(StringType))]
        public async Task<string> deletePatientState(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string name)
        {

            string p;

            if (string.IsNullOrEmpty(name))
            {
                throw ErrorQueryBuilder(
                    "STATE_NAME_EMPTY",
                    "I can't identifiy a state without a name."
                    );
            }

            try
            {
                p = await dBContext.PatientState
                .Where(p => p.Name.Equals(name))
                .Select(s => new string(s.Name))
                .FirstOrDefaultAsync();

                int rows = await dBContext.Database.ExecuteSqlRawAsync(
                    "DELETE FROM admin.Patient_State " +
                    "WHERE name = {0}", name);

                if (rows < 1)
                {
                    throw ErrorQueryBuilder(
                        "PATIENT_STATE_NOT_FOUND",
                        "The patient state with name '{0}' doesn't exists.", name);
                }

                await dBContext.SaveChangesAsync();
            }
            catch (SqlException sqlException)
            {
                if (sqlException.Number.Equals(547))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The patient state has patients related, please assign them another state before deleting this one");
                }
                else if (sqlException.Number.Equals(2628))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The name specified is too long, try to use one under 50 chars long.");
                }
                else if (sqlException.Number.Equals(50005))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        sqlException.Message);
                }
                else
                {
                    throw ErrorQueryBuilder(
                            sqlException,
                            "Unknown error.");
                }

            }
            catch (QueryException e)
            {
                throw e;
            }

            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return p;

        }

        [GraphQLType(typeof(MedicationType))]
        public async Task<Medication> addMedication(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string name,
            [GraphQLNonNullType] string pharmaceutical)
        {

            if (string.IsNullOrEmpty(name))
            {
                throw ErrorQueryBuilder(
                    "MEDICATION_NAME_EMPTY",
                    "The name can't be empty."
                    );
            }

            if (string.IsNullOrEmpty(pharmaceutical))
            {
                throw ErrorQueryBuilder(
                    "MEDICATION_PHARMACEUTICAL_EMPTY",
                    "The name of the pharmaceutical house can't be empty."
                    );
            }

            try
            {
                await dBContext.Database.ExecuteSqlRawAsync(
                    "INSERT INTO admin.Medication(Medicine, Pharmacist) " +
                    "VALUES ({0}, {1})", name, pharmaceutical);
            }
            catch (SqlException sqlException)
            {
                if (sqlException.Number.Equals(2627))
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        "The medication with name '{0}' and pharmaceutical '{1}' already exists.", name, pharmaceutical);

                }
                else if (sqlException.Number.Equals(2628))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The values specified are too long, try to use values under 80 chars long.");
                }
                else
                {
                    throw ErrorQueryBuilder(
                            sqlException,
                            "Unknown error.");
                }

            }
            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return await dBContext.Medication
                .Where(s => s.Medicine.Equals(name.Trim()))
                .FirstOrDefaultAsync();
        }

        [GraphQLType(typeof(MedicationType))]
        public async Task<Medication> updateMedication(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string oldName,
            string newName,
            string pharmaceutical)
        {

            if (string.IsNullOrEmpty(oldName))
            {
                throw ErrorQueryBuilder(
                    "MEDICATION_NAME_EMPTY",
                    "I can't identifiy the medication without a name."
                    );
            }

            if (string.IsNullOrEmpty(newName) && string.IsNullOrEmpty(pharmaceutical))
            {
                throw ErrorQueryBuilder(
                    "NO_CHANGES_AVAILABLE",
                    "Nothing to change.");
            }

            try
            {
                int rows = await dBContext.Database.ExecuteSqlRawAsync(
                    "UPDATE admin.Medication " +
                    "SET Medicine = (CASE" +
                                    " WHEN {0} is null THEN Medicine " +
                                    " ELSE {0} END), " +
                    "Pharmacist = (CASE" +
                                    " WHEN {1} is null THEN Pharmacist " +
                                    " ELSE {1} END) " +
                    "WHERE Medicine = {2}",
                    string.IsNullOrEmpty(newName.Trim()) ? null : newName.Trim(),
                    string.IsNullOrEmpty(pharmaceutical.Trim()) ? null : pharmaceutical.Trim(),
                    oldName.Trim());

                if (rows < 1)
                {
                    throw ErrorQueryBuilder(
                        "MEDICATION_NOT_FOUND",
                        "The medication with name '{0}' doesn't exists.", oldName.Trim());
                }
            }

            catch (SqlException sqlException)
            {
                if (sqlException.Number.Equals(547))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The medication has patients related, please assign them another medications before updating this one");
                }
                else if (sqlException.Number.Equals(2627))
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        "The medication with name '{0}' already exists.", newName.Trim());

                }
                else if (sqlException.Number.Equals(2628))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The values specified is too long, try to use another under 80 chars long.");
                }
                else
                {
                    throw ErrorQueryBuilder(
                            sqlException,
                            "Unknown error.");
                }

            }
            catch (QueryException e)
            {
                throw e;
            }

            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return await dBContext.Medication
                .Where(s => s.Medicine.Equals(string.IsNullOrEmpty(newName) ? oldName.Trim() : newName.Trim()))
                .FirstOrDefaultAsync();
        }

        [GraphQLType(typeof(MedicationType))]
        public async Task<Medication> deleteMedication(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw ErrorQueryBuilder(
                    "MEDICATION_NAME_EMPTY",
                    "The name can't be empty."
                    );
            }

            Medication p;

            try
            {
                p = await dBContext.Medication
                .Where(s => s.Medicine.Equals(name.Trim()))
                .FirstOrDefaultAsync();

                int rows = await dBContext.Database.ExecuteSqlRawAsync(
                    "DELETE FROM admin.Medication " +
                    "WHERE name = {0}", name);

                if (rows < 1)
                {
                    throw ErrorQueryBuilder(
                        "MEDICATION_NOT_FOUND",
                        "The medication with name '{0}' doesn't exists.", name.Trim());
                }

                await dBContext.SaveChangesAsync();
            }
            catch (SqlException sqlException)
            {
                if (sqlException.Number.Equals(547))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The medication has patients related, please assign them another medications before deleting this one");
                }
                else if (sqlException.Number.Equals(2628))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The name specified is too long, try to use another under 80 chars long.");
                }
                else
                {
                    throw ErrorQueryBuilder(
                            sqlException,
                            "Unknown error.");
                }

            }
            catch (QueryException e)
            {
                throw e;
            }

            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return p;

        }

        [GraphQLType(typeof(ContentionMeasureType))]
        public async Task<ContentionMeasure> addContentionMeasure(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string name,
            [GraphQLNonNullType] string description)
        {

            if (string.IsNullOrEmpty(name))
            {
                throw ErrorQueryBuilder(
                    "CONTENTION_MEASURE_NAME_EMPTY",
                    "The name can't be empty."
                    );
            }

            if (string.IsNullOrEmpty(description))
            {
                throw ErrorQueryBuilder(
                    "CONTENTION_MEASURE_DESCRIPTION_EMPTY",
                    "The description can't be empty."
                    );
            }

            try
            {
                await dBContext.Database.ExecuteSqlRawAsync(
                    "INSERT INTO admin.Contention_Measure(Name, Description) " +
                    "VALUES ({0}, {1})", name, description);
            }
            catch (SqlException sqlException)
            {
                if (sqlException.Number.Equals(2627))
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        "The contentio measure with name '{0}' already exists.", name);

                }
                else if (sqlException.Number.Equals(2628))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The name specified are too long, try to use another under 80 chars long.");
                }
                else
                {
                    throw ErrorQueryBuilder(
                            sqlException,
                            "Unknown error.");
                }

            }
            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return await dBContext.ContentionMeasure
                .Where(t => t.Name.Equals(name.Trim()))
                .FirstOrDefaultAsync();
        }

        [GraphQLType(typeof(ContentionMeasureType))]
        public async Task<ContentionMeasure> updateContentionMeasure(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string oldName,
            string newName,
            string description)
        {
            if (string.IsNullOrEmpty(oldName))
            {
                throw ErrorQueryBuilder(
                    "CONTENTION_MEASURE_NAME_EMPTY",
                    "I can't identifiy the medication without a name."
                    );
            }

            if (string.IsNullOrEmpty(newName) && string.IsNullOrEmpty(description))
            {
                throw ErrorQueryBuilder(
                    "NO_CHANGES_AVAILABLE",
                    "Nothing to change.");
            }

            try
            {
                int rows = await dBContext.Database.ExecuteSqlRawAsync(
                    "UPDATE admin.Contention_Measure " +
                    "SET Name = (CASE" +
                                    " WHEN {0} is null THEN Name " +
                                    " ELSE {0} END), " +
                    "Description = (CASE" +
                                    " WHEN {1} is null THEN Description " +
                                    " ELSE {1} END) " +
                    "WHERE Name = {2}",
                    string.IsNullOrEmpty(newName.Trim()) ? null : newName.Trim(),
                    string.IsNullOrEmpty(description) ? null : description,
                    oldName.Trim());

                if (rows < 1)
                {
                    throw ErrorQueryBuilder(
                        "CONTENTION_MEASURE_NOT_FOUND",
                        "The contention measure with name '{0}' doesn't exists.", oldName.Trim());
                }
            }

            catch (SqlException sqlException)
            {
                if (sqlException.Number.Equals(547))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The measure has countries related, please delete them before updating this one.");
                }
                else if (sqlException.Number.Equals(2627))
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        "The contention measure with name '{0}' already exists.", newName.Trim());

                }
                else if (sqlException.Number.Equals(2628))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The values specified is too long, try to use a name under 80 chars long and description under 5000 chars.");
                }
                else
                {
                    throw ErrorQueryBuilder(
                            sqlException,
                            "Unknown error.");
                }

            }
            catch (QueryException e)
            {
                throw e;
            }

            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return await dBContext.ContentionMeasure
                .Where(t => t.Name.Equals(string.IsNullOrEmpty(newName.Trim()) ? null : newName.Trim()))
                .FirstOrDefaultAsync();
        }

        [GraphQLType(typeof(ContentionMeasureType))]
        public async Task<ContentionMeasure> deleteContentionMeasure(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw ErrorQueryBuilder(
                    "CONTENTION_MEASURE_NAME_EMPTY",
                    "The name can't be empty."
                    );
            }

            ContentionMeasure p;

            try
            {
                p = await dBContext.ContentionMeasure
                .Where(t => t.Name.Equals(name))
                .FirstOrDefaultAsync();

                int rows = await dBContext.Database.ExecuteSqlRawAsync(
                    "DELETE FROM admin.Contention_Measure " +
                    "WHERE Name = {0}", name);

                if (rows < 1)
                {
                    throw ErrorQueryBuilder(
                        "CONTENTION_MEASURE_NOT_FOUND",
                        "The contention measure with name '{0}' doesn't exists.", name.Trim());
                }
            }
            catch (SqlException sqlException)
            {
                if (sqlException.Number.Equals(547))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The measure has countries related, please delete them before updating this one.");
                }
                else if (sqlException.Number.Equals(2628))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The name specified is too long, try to use a name under 80 chars long.");
                }
                else
                {
                    throw ErrorQueryBuilder(
                            sqlException,
                            "Unknown error.");
                }

            }
            catch (QueryException e)
            {
                throw e;
            }

            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return p;

        }

        [GraphQLType(typeof(SanitaryMeasureType))]
        public async Task<SanitaryMeasure> addSanitaryMeasure(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string name,
            [GraphQLNonNullType] string description)
        {

            if (string.IsNullOrEmpty(name))
            {
                throw ErrorQueryBuilder(
                    "Sanitary_MEASURE_NAME_EMPTY",
                    "The name can't be empty."
                    );
            }

            if (string.IsNullOrEmpty(description))
            {
                throw ErrorQueryBuilder(
                    "Sanitary_MEASURE_DESCRIPTION_EMPTY",
                    "The description can't be empty."
                    );
            }

            try
            {
                await dBContext.Database.ExecuteSqlRawAsync(
                    "INSERT INTO admin.Sanitary_Measure(Name, Description) " +
                    "VALUES ({0}, {1})", name, description);
            }
            catch (SqlException sqlException)
            {
                if (sqlException.Number.Equals(2627))
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        "The contentio measure with name '{0}' already exists.", name);

                }
                else if (sqlException.Number.Equals(2628))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The name specified are too long, try to use another under 80 chars long.");
                }
                else
                {
                    throw ErrorQueryBuilder(
                            sqlException,
                            "Unknown error.");
                }

            }
            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return await dBContext.SanitaryMeasure
                .Where(t => t.Name.Equals(name.Trim()))
                .FirstOrDefaultAsync();
        }

        [GraphQLType(typeof(SanitaryMeasureType))]
        public async Task<SanitaryMeasure> updateSanitaryMeasure(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string oldName,
            string newName,
            string description)
        {
            if (string.IsNullOrEmpty(oldName))
            {
                throw ErrorQueryBuilder(
                    "Sanitary_MEASURE_NAME_EMPTY",
                    "I can't identifiy the medication without a name."
                    );
            }

            if (string.IsNullOrEmpty(newName) && string.IsNullOrEmpty(description))
            {
                throw ErrorQueryBuilder(
                    "NO_CHANGES_AVAILABLE",
                    "Nothing to change.");
            }

            try
            {
                int rows = await dBContext.Database.ExecuteSqlRawAsync(
                    "UPDATE admin.Sanitary_Measure " +
                    "SET Name = (CASE" +
                                    " WHEN {0} is null THEN Name " +
                                    " ELSE {0} END), " +
                    "Description = (CASE" +
                                    " WHEN {1} is null THEN Description " +
                                    " ELSE {1} END) " +
                    "WHERE Name = {2}",
                    string.IsNullOrEmpty(newName.Trim()) ? null : newName.Trim(),
                    string.IsNullOrEmpty(description) ? null : description,
                    oldName.Trim());

                if (rows < 1)
                {
                    throw ErrorQueryBuilder(
                        "Sanitary_MEASURE_NOT_FOUND",
                        "The Sanitary measure with name '{0}' doesn't exists.", oldName.Trim());
                }
            }

            catch (SqlException sqlException)
            {
                if (sqlException.Number.Equals(547))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The measure has countries related, please delete them before updating this one.");
                }
                else if (sqlException.Number.Equals(2627))
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        "The Sanitary measure with name '{0}' already exists.", newName.Trim());

                }
                else if (sqlException.Number.Equals(2628))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The values specified is too long, try to use a name under 80 chars long and description under 5000 chars.");
                }
                else
                {
                    throw ErrorQueryBuilder(
                            sqlException,
                            "Unknown error.");
                }

            }
            catch (QueryException e)
            {
                throw e;
            }

            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return await dBContext.SanitaryMeasure
                .Where(t => t.Name.Equals(string.IsNullOrEmpty(newName.Trim()) ? null : newName.Trim()))
                .FirstOrDefaultAsync();
        }

        [GraphQLType(typeof(SanitaryMeasureType))]
        public async Task<SanitaryMeasure> deleteSanitaryMeasure(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw ErrorQueryBuilder(
                    "Sanitary_MEASURE_NAME_EMPTY",
                    "The name can't be empty."
                    );
            }

            SanitaryMeasure p;

            try
            {
                p = await dBContext.SanitaryMeasure
                .Where(t => t.Name.Equals(name))
                .FirstOrDefaultAsync();

                int rows = await dBContext.Database.ExecuteSqlRawAsync(
                    "DELETE FROM admin.Sanitary_Measure " +
                    "WHERE Name = {0}", name);

                if (rows < 1)
                {
                    throw ErrorQueryBuilder(
                        "Sanitary_MEASURE_NOT_FOUND",
                        "The Sanitary measure with name '{0}' doesn't exists.", name.Trim());
                }
            }
            catch (SqlException sqlException)
            {
                if (sqlException.Number.Equals(547))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The measure has countries related, please delete them before updating this one.");
                }
                else if (sqlException.Number.Equals(2628))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The name specified is too long, try to use a name under 80 chars long.");
                }
                else
                {
                    throw ErrorQueryBuilder(
                            sqlException,
                            "Unknown error.");
                }

            }
            catch (QueryException e)
            {
                throw e;
            }

            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return p;

        }

        [GraphQLType(typeof(PathologyType))]
        public async Task<Pathology> addPathology(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] CreatePathology input)
        {

            if (string.IsNullOrEmpty(input.name))
            {
                throw ErrorQueryBuilder(
                    "PATHOLOGY_NAME_EMPTY",
                    "The name can't be empty."
                    );
            }

            try
            {
                if (!string.IsNullOrEmpty(input.description) && string.IsNullOrEmpty(input.treatment))
                {
                    await dBContext.Database.ExecuteSqlRawAsync(
                    "INSERT INTO admin.Pathology ([name], [description]) " +
                    "VALUES (" +
                    "{0}," +
                    "{1})",
                    input.name,
                    input.description);
                }
                else if (!string.IsNullOrEmpty(input.treatment) && string.IsNullOrEmpty(input.description))
                {
                    await dBContext.Database.ExecuteSqlRawAsync(
                    "INSERT INTO admin.Pathology ([name], [treatment]) " +
                    "VALUES (" +
                    "{0}," +
                    "{1})",
                    input.name,
                    input.treatment);
                }
                else if (!string.IsNullOrEmpty(input.treatment) && !string.IsNullOrEmpty(input.description))
                {
                    await dBContext.Database.ExecuteSqlRawAsync(
                    "INSERT INTO admin.Pathology ([name], [treatment], [description]) " +
                    "VALUES (" +
                    "{0}," +
                    "{1}," +
                    "{2})",
                    input.name,
                    input.treatment,
                    input.description);
                }
                else
                {
                    await dBContext.Database.ExecuteSqlRawAsync(
                    "INSERT INTO admin.Pathology ([name]) " +
                    "VALUES (" +
                    "{0})",
                    input.name);
                }
            }
            catch (SqlException sqlException)
            {
                if (sqlException.Number.Equals(2627))
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        "The pathology with name '{0}' already exists.", input.name);

                }
                else if (sqlException.Number.Equals(2628))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The name specified is too long, try to use one under 150 chars long.");
                }
                else if (sqlException.Number.Equals(515))
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        "You must enter a value for name field.");
                }
                else
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        "Unknown Error");
                }

            }
            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return await dBContext.Pathology
                .Where(s => s.Name.Contains(input.name))
                .FirstOrDefaultAsync();
        }

        [GraphQLType(typeof(PathologyType))]
        public async Task<Pathology> updatePathology(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string name,
            [GraphQLNonNullType] UpdatePathology input)
        {

            List<IError> errors = new List<IError>();

            try
            {
                int rows = await dBContext.Database.ExecuteSqlRawAsync(
                    "UPDATE admin.Pathology " +
                    "SET Name = (CASE" +
                                    " WHEN {0} is null THEN Name " +
                                    " ELSE {0} END), " +
                    "Description = (CASE" +
                                    " WHEN {1} is null THEN Description " +
                                    " ELSE {1} END) " +
                    "Treatment = (CASE" +
                                    " WHEN {2} is null THEN Treatment " +
                                    " ELSE {2} END), " +
                    "WHERE Name = {3}",
                    string.IsNullOrEmpty(input.name.Trim()) ? null : input.name.Trim(),
                    string.IsNullOrEmpty(input.description) ? null : input.description,
                    string.IsNullOrEmpty(input.treatment) ? null : input.treatment,
                    name.Trim());

                if (rows < 1)
                {
                    throw ErrorQueryBuilder(
                        "NOTHING_TO_CHANGE",
                        "The name '{0}' doesn't match any pathology."
                        , name
                        );
                }
            }

            catch (SqlException e)
            {
                if (e.Number.Equals(547))
                {
                    errors.Add(CustomErrorBuilder(
                            e,
                            "You tried to change the name of a pathology with a related symptoms, contact or patient. " +
                            "Please delete all elements related before deleting this pathology."));
                }
                else if (e.Number.Equals(2627))
                {
                    errors.Add(CustomErrorBuilder(
                            e,
                            "You tried to insert a name that already exists."));
                }
                else
                {
                    errors.Add(CustomErrorBuilder(
                            e,
                            "Unknown error."));
                }

            }
            catch (QueryException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new QueryException(ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            if (input.symptoms != null)
            {
                string currentPathology = "";
                StringBuilder s = new StringBuilder();

                /*dbPath - input > 0 => delete dbPath => loop no errors
                dbPath - input = 0 & dbPath.Count = input.Count => null => null
                dbPath - input = 0 & dbPath.Count < input.Count => insert input => loop with errors
                dbPath - input > 0 & dbPath.Count = input.Count => updatePatient dbPath*/


                ICollection<string> currentDB = dBContext.PathologySymptoms
                    .Where(p => p.Pathology.Equals(string.IsNullOrEmpty(input.name) ? name : input.name))
                    .Select(s => new string(s.Symptom))
                    .ToList();
                //.ConvertAll<string>(s => s.PathologyName);

                ICollection<string> fromDB = currentDB.Except(input.symptoms).ToList();

                ICollection<string> inputPath = input.symptoms.Except(currentDB).ToList();

                if (fromDB.Count > 0 && currentDB.Count > input.symptoms.Count)
                {
                    foreach (var path in fromDB)
                    {
                        currentPathology = path;
                        try
                        {
                            await dBContext.Database.ExecuteSqlRawAsync(
                                "DELETE FROM healthcare.Pathology_Symptoms " +
                                "WHERE Symptom = {0} " +
                                "AND Pathology = {1}"
                                , path
                                , string.IsNullOrEmpty(input.name) ? name : input.name);
                        }
                        catch (SqlException sqlException)
                        {
                            if (sqlException.Number.Equals(547))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The symptom with name '{0}', doesn't exists.", currentPathology));
                            }
                            else if (sqlException.Number.Equals(2628))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The symptom name specified is too long, try to use one under 100 chars long."));
                            }
                            else if (sqlException.Number.Equals(515))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "You must enter a value for symptom name."));
                            }
                            else
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "Unknown Error"));
                            }

                        }
                        catch (Exception e)
                        {
                            errors.Add(ErrorBuilder.New()
                               .SetMessage("Unknown error")
                               .SetCode(e.GetType().FullName)
                               .SetExtension("DatabaseMessage", e.Message)
                               .Build());
                        }
                    }
                }
                else if (fromDB.Count.Equals(0) && currentDB.Count < input.symptoms.Count)
                {
                    foreach (var path in inputPath)
                    {
                        currentPathology = path;
                        try
                        {
                            await dBContext.Database.ExecuteSqlRawAsync(
                                "INSERT INTO healthcare.Pathology_Symptoms(Symptom, Pathology) " +
                                "VALUES (" +
                                "{0}," +
                                "{1})"
                                , path
                                , string.IsNullOrEmpty(input.name) ? name : input.name);
                        }
                        catch (SqlException sqlException)
                        {
                            if (sqlException.Number.Equals(547))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The symptom with name '{0}', doesn't exists.", currentPathology));
                            }
                            else if (sqlException.Number.Equals(2628))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The symptom name specified is too long, try to use one under 100 chars long."));
                            }
                            else if (sqlException.Number.Equals(2627))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The pathology already has the symptom '{0}' assigned.", currentPathology));
                            }
                            else if (sqlException.Number.Equals(515))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "You must enter a value for symptom name."));
                            }
                            else
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "Unknown Error"));
                            }

                        }
                        catch (Exception e)
                        {
                            errors.Add(ErrorBuilder.New()
                               .SetMessage("Unknown error")
                               .SetCode(e.GetType().FullName)
                               .SetExtension("DatabaseMessage", e.Message)
                               .Build());
                        }
                    }
                }
                else if (fromDB.Count > 0 && currentDB.Count.Equals(input.symptoms.Count))
                {
                    foreach (var path in fromDB.Zip(inputPath, Tuple.Create))
                    {
                        currentPathology = path.Item2;
                        try
                        {
                            await dBContext.Database.ExecuteSqlRawAsync(
                                "UPDATE healthcare.Pathology_Symptoms " +
                                "SET Symptom = {0} " +
                                "WHERE Symptom = {1} " +
                                "AND Pathology = {2}"
                                , path.Item2
                                , path.Item1
                                , string.IsNullOrEmpty(input.name) ? name : input.name);
                        }
                        catch (SqlException sqlException)
                        {
                            if (sqlException.Number.Equals(547))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The symptom with name '{0}', doesn't exists.", currentPathology));
                            }
                            else if (sqlException.Number.Equals(2628))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The symptom name specified is too long, try to use one under 100 chars long."));
                            }
                            else if (sqlException.Number.Equals(515))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "You must enter a value for symptom name."));
                            }
                            else
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "Unknown Error"));
                            }

                        }
                        catch (Exception e)
                        {
                            errors.Add(ErrorBuilder.New()
                               .SetMessage("Unknown error")
                               .SetCode(e.GetType().FullName)
                               .SetExtension("DatabaseMessage", e.Message)
                               .Build());
                        }
                    }
                }
            }

            if (errors.Count > 0)
            {
                throw new QueryException(errors.AsEnumerable());
            }

            return await dBContext.Pathology
                .Where(s => s.Name.Equals(string.IsNullOrEmpty(input.name) ? name : input.name))
                .FirstOrDefaultAsync();
        }


        [GraphQLType(typeof(PathologyType))]
        public async Task<Pathology> deletePathology(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw ErrorQueryBuilder(
                    "NAME_EMPTY",
                    "The name can't be empty.");
            }
            Pathology p;

            try
            {
                p = await dBContext.Pathology
                .Where(s => s.Name.Equals(name))
                .FirstOrDefaultAsync();

                int rows = await dBContext.Database.ExecuteSqlRawAsync(
                    "DELETE FROM healthcare.Pathology " +
                    "WHERE Name = {0}", name);

                if (rows < 1)
                {
                    throw ErrorQueryBuilder(
                        "PATHOLOGY_NOT_FOUND",
                        "The pathology with name '{0}' doesn't exists.", name);
                }
            }
            catch (SqlException e)
            {
                if (e.Number.Equals(547))
                {
                    throw ErrorQueryBuilder(
                            e,
                            "You tried to delete a pathology with a related symptom, contact or patient. " +
                            "Please delete all elements related before deleting this patient.");
                }

                else
                {
                    throw ErrorQueryBuilder(
                            e,
                            "Unknown error.");
                }

            }
            catch (QueryException e)
            {
                throw e;
            }

            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return p;

        }

        [GraphQLType(typeof(SanitaryEventType))]
        public async Task<SanitaryMeasuresChanges> addSanitaryMeasureActive(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string measure,
            [GraphQLNonNullType] DateTime startDate,
            DateTime endDate,
            [GraphQLNonNullType] string country)
        {

            if (string.IsNullOrEmpty(measure))
            {

                throw ErrorQueryBuilder(
                    "MEASURE_NAME_EMPTY",
                    "The measure name can't be empty."
                    );

            }

            if (string.IsNullOrEmpty(country))
            {
                throw ErrorQueryBuilder(
                    "COUNTRY_NAME_EMPTY",
                    "The name of the country can't be empty."
                    );
            }

            var newVisit = new SanitaryMeasuresChanges
            {

                MeasureName = measure,
                StartDate = startDate,
                CountryName = country,
                EndDate = endDate

            };

            dBContext.SanitaryMeasuresChanges.Add(newVisit);

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
                        throw ErrorQueryBuilder(
                            sqlException,
                            "The Sanitary measure '{0}' in country '{1}', with start date '{2}' already exists.", measure, country, startDate);

                    }
                    else if (sqlException.Number.Equals(547))
                    {

                        throw ErrorQueryBuilder(
                            sqlException,
                            "The country (name: {0}) or measure (name: {1}) doesn't exists.", country, measure);
                    }
                    else if (sqlException.Number.Equals(2628))
                    {

                        throw ErrorQueryBuilder(
                            sqlException,
                            "The measure name or country specified is too long, try to use one under 80 or 100 chars long.");
                    }
                    else
                    {
                        throw ErrorQueryBuilder(
                            sqlException,
                            "Unknown Error");
                    }

                }


            }
            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return await dBContext.SanitaryMeasuresChanges
                        .Where(s => s.CountryName.Equals(country)
                                 && s.MeasureName.Equals(measure) 
                                 && s.StartDate.Equals(startDate))
                        .Include(s => s.CountryNameNavigation)
                        .Include(s => s.MeasureNameNavigation)
                        .FirstOrDefaultAsync();
        }

        [GraphQLType(typeof(SanitaryEventType))]
        public async Task<SanitaryMeasuresChanges> updateSanitaryMeasureActive(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string measure,
            [GraphQLNonNullType] DateTime startDate,
            [GraphQLNonNullType] string country,
            [GraphQLNonNullType] UpdateSanitaryEvent input)
        {

            if (string.IsNullOrEmpty(measure))
            {

                throw ErrorQueryBuilder(
                    "MEASURE_NAME_EMPTY",
                    "The measure name can't be empty."
                    );

            }

            if (string.IsNullOrEmpty(country))
            {
                throw ErrorQueryBuilder(
                    "COUNTRY_NAME_EMPTY",
                    "The name of the country can't be empty."
                    );
            }

            if (string.IsNullOrEmpty(input.country) 
                && string.IsNullOrEmpty(input.measure) 
                && input.startDate.HasValue 
                && input.endDate.HasValue)
            {
                throw ErrorQueryBuilder(
                    "NO_CHANGES_AVAILABLE",
                    "Nothing to change.");
            }

            try
            {
                int rows = await dBContext.Database.ExecuteSqlRawAsync(
                    "UPDATE healthcare.Sanitary_Measures_Changes " +
                    "SET country_Name = (CASE" +
                                    " WHEN {0} is null THEN country_Name " +
                                    " ELSE {0} END), " +
                    "Measure_Name = (CASE" +
                                    " WHEN {1} is null THEN Measure_Name " +
                                    " ELSE {1} END), " +
                    "Start_date = (CASE" +
                                    " WHEN {2} is null THEN Start_date " +
                                    " ELSE {2} END), " +
                    "End_date = (CASE" +
                                    " WHEN {3} is null THEN End_date " +
                                    " ELSE {3} END) " +
                    "WHERE Measure_Name = {4} AND Start_date = {5} AND country_Name = {6}",
                    string.IsNullOrEmpty(input.country) ? null : input.country,
                    string.IsNullOrEmpty(input.measure) ? null : input.measure,
                    input.startDate.HasValue ? input.startDate : null,
                    input.endDate.HasValue ? input.endDate : null,
                    measure, startDate, country);

                if (rows < 1)
                {
                    throw ErrorQueryBuilder(
                        "MEASURE_NOT_FOUND",
                        "The Sanitary event with name '{0}', in country '{1}' and start date '{2}' doesn't exists.", measure, country, startDate);
                }
            }

            catch (SqlException sqlException)
            {
                if (sqlException.Number.Equals(2627))
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        "The Sanitary event with name '{0}', in country '{1}' and start date '{2}' already exists.", input.measure, input.country, input.startDate);

                }
                else if (sqlException.Number.Equals(547))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The country (name: {0}) or measure (name: {1}) doesn't exists.", input.country, input.measure);
                }
                else if (sqlException.Number.Equals(2628))
                {

                    throw ErrorQueryBuilder(
                            sqlException,
                            "The measure name or country specified is too long, try to use one under 80 or 100 chars long.");
                }
                else
                {
                    throw ErrorQueryBuilder(
                            sqlException,
                            "Unknown error.");
                }

            }
            catch (QueryException e)
            {
                throw e;
            }

            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return await dBContext.SanitaryMeasuresChanges
                        .Where(s => s.CountryName.Equals(string.IsNullOrEmpty(input.country) ? country : input.country)
                                 && s.MeasureName.Equals(string.IsNullOrEmpty(input.measure) ? measure : input.measure)
                                 && s.StartDate.Equals(input.startDate.HasValue ? input.startDate : startDate))
                        .Include(s => s.CountryNameNavigation)
                        .Include(s => s.MeasureNameNavigation)
                        .FirstOrDefaultAsync();
        }

        [GraphQLType(typeof(SanitaryEventType))]
        public async Task<SanitaryMeasuresChanges> deleteSanitaryMeasureActive(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string measure,
            [GraphQLNonNullType] DateTime startDate,
            [GraphQLNonNullType] string country)
        {

            SanitaryMeasuresChanges p;

            if (string.IsNullOrEmpty(measure))
            {

                throw ErrorQueryBuilder(
                    "MEASURE_NAME_EMPTY",
                    "The measure name can't be empty."
                    );

            }

            if (string.IsNullOrEmpty(country))
            {
                throw ErrorQueryBuilder(
                    "COUNTRY_NAME_EMPTY",
                    "The name of the country can't be empty."
                    );
            }

            try
            {
                p = await dBContext.SanitaryMeasuresChanges
                        .Where(s => s.CountryName.Equals(country)
                                 && s.MeasureName.Equals(measure)
                                 && s.StartDate.Equals(startDate))
                        .Include(s => s.CountryNameNavigation)
                        .Include(s => s.MeasureNameNavigation)
                        .FirstOrDefaultAsync();

                int rows = await dBContext.Database.ExecuteSqlRawAsync(
                    "DELETE FROM healthcare.Sanitary_Measures_Changes " +
                    "WHERE Measure_Name = {0} AND country_Name = {1} AND Start_Date = {2}", measure, country, startDate);

                if (rows < 1)
                {
                    throw ErrorQueryBuilder(
                        "MEASURE_NOT_FOUND",
                        "The Sanitary event with name '{0}', in country '{1}' and start date '{2}' doesn't exists.", measure, country, startDate);
                }
            }
            catch (SqlException sqlException)
            {
                if (sqlException.Number.Equals(2628))
                {

                    throw ErrorQueryBuilder(
                            sqlException,
                            "The measure name or country specified is too long, try to use one under 80 or 100 chars long.");
                }
                else
                {
                    throw ErrorQueryBuilder(
                            sqlException,
                            "Unknown error.");
                }

            }
            catch (QueryException e)
            {
                throw e;
            }

            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return p;

        }

        [GraphQLType(typeof(ContactType))]
        public async Task<Contact> addContact(
           [Service] CoTEC_DBContext dBContext,
           [GraphQLNonNullType] CreateContact input)
        {

            if (string.IsNullOrEmpty(input.identification))
            {
                throw ErrorQueryBuilder(
                    "IDENTIFICATION_EMPTY",
                    "The identification code can't be empty."
                    );
            }

            try
            {
                if (string.IsNullOrEmpty(input.nationality))
                {
                    await dBContext.Database.ExecuteSqlRawAsync(
                    "EXECUTE healthcare.Contact_Existence_Checker @identification = {0}, " +
                    " @firstName = {1}," +
                    " @lastName = {2}," +
                    " @region = {3}," +
                    " @country = {4}," +
                    " @age = {5}," +
                    " @address = {6}," +
                    " @email = {7}",
                    input.identification,
                    string.IsNullOrEmpty(input.firstName) ? null : input.firstName,
                    string.IsNullOrEmpty(input.lastName) ? null : input.lastName,
                    string.IsNullOrEmpty(input.region) ? null : input.region,
                    string.IsNullOrEmpty(input.country) ? null : input.country,
                    input.age,
                    input.address,
                    input.email);

                    /*await dBContext.Database.ExecuteSqlRawAsync(
                    "INSERT INTO healthcare.input ([identification]" +
                    ",[firstName]" +
                    ",[lastName]" +
                    ",[region]" +
                    ",[country]" +
                    ",[age]" +
                    ",[intensiveCareUnite]" +
                    ",[hospitalized]" +
                    ",[state]" +
                    ",[Date_Entrance]) " +
                    "VALUES (" +
                    "{0}," +
                    "{1}," +
                    "{2}," +
                    "{3}," +
                    "{4}," +
                    "{5}," +
                    "{6}," +
                    "{7}," +
                    "{8}," +
                    "{9})",
                    input.identification,
                    string.IsNullOrEmpty(input.firstName) ? null : input.firstName,
                    string.IsNullOrEmpty(input.lastName) ? null : input.lastName,
                    input.region,
                    input.country,
                    input.age,
                    input.intensiveCareUnite,
                    input.hospitalized,
                    string.IsNullOrEmpty(input.state) ? null : input.state,
                    input.dateEntrance);*/
                }
                else
                {
                    await dBContext.Database.ExecuteSqlRawAsync(
                    "EXECUTE healthcare.Contact_Existence_Checker @identification = {0}, " +
                    " @firstName = {1}," +
                    " @lastName = {2}," +
                    " @region = {3}," +
                    " @country = {4}," +
                    " @age = {5}," +
                    " @address = {6}," +
                    " @email = {7}" +
                    " @nationality = {8}",
                    input.identification,
                    string.IsNullOrEmpty(input.firstName) ? null : input.firstName,
                    string.IsNullOrEmpty(input.lastName) ? null : input.lastName,
                    string.IsNullOrEmpty(input.region) ? null : input.region,
                    string.IsNullOrEmpty(input.country) ? null : input.country,
                    input.age,
                    input.address,
                    input.email,
                    input.nationality);


                    /*await dBContext.Database.ExecuteSqlRawAsync(
                    "INSERT INTO healthcare.input ([identification]" +
                    ",[firstName]" +
                    ",[lastName]" +
                    ",[region]" +
                    ",[nationality]" +
                    ",[country]" +
                    ",[age]" +
                    ",[intensiveCareUnite]" +
                    ",[hospitalized]" +
                    ",[state]" +
                    ",[Date_Entrance]) " +
                    "VALUES (" +
                    "{0}," +
                    "{1}," +
                    "{2}," +
                    "{3}," +
                    "{4}," +
                    "{5}," +
                    "{6}," +
                    "{7}," +
                    "{8}," +
                    "{9}," +
                    "{10})",
                    input.identification,
                    string.IsNullOrEmpty(input.firstName) ? null : input.firstName,
                    string.IsNullOrEmpty(input.lastName) ? null : input.lastName,
                    input.region,
                    string.IsNullOrEmpty(input.nationality) ? null : input.nationality,
                    input.country,
                    input.age,
                    input.intensiveCareUnite,
                    input.hospitalized,
                    string.IsNullOrEmpty(input.state) ? null : input.state,
                    input.dateEntrance);*/
                }
            }
            catch (SqlException sqlException)
            {
                if (sqlException.Number.Equals(2627))
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        "The contact with identification '{0}' already exists.", input.identification);

                }
                else if (sqlException.Number.Equals(547))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The contact has a country, region or state that is invalid.");
                }
                else if (sqlException.Number.Equals(2628))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The identification specified is too long, try to use one under 50 chars long.");
                }
                else if (sqlException.Number.Equals(515))
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        "You must enter a value for all fields.");
                }
                else if (sqlException.Number.Equals(50006))
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        sqlException.Message);
                }
                else
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        "Unknown Error");
                }

            }
            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return await dBContext.Contact
                .Where(t => t.Identification.Equals(input.identification.Trim()))
                .Include(s => s.RegionNavigation)
                    .ThenInclude(r => r.CountryNavigation)
                        .ThenInclude(r => r.ContinentNavigation)
                .FirstOrDefaultAsync();
        }

        [GraphQLType(typeof(ContactType))]
        public async Task<Contact> updateContact(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string identification,
            [GraphQLNonNullType] UpdateContact input)
        {

            List<IError> errors = new List<IError>();

            try
            {
                int rows = await dBContext.Database.ExecuteSqlRawAsync(
                    "UPDATE healthcare.Contact " +
                    "SET identification = (CASE" +
                                    " WHEN {0} is null THEN identification " +
                                    " ELSE {0} END), " +
                    "firstName = (CASE" +
                                    " WHEN {1} is null THEN firstName " +
                                    " ELSE {1} END), " +
                    "lastName = (CASE" +
                                    " WHEN {2} is null THEN lastName " +
                                    " ELSE {2} END), " +
                    "region = (CASE" +
                                    " WHEN {2} is null THEN region " +
                                    " ELSE {2} END), " +
                    "nationality = (CASE" +
                                    " WHEN {2} is null THEN nationality " +
                                    " ELSE {2} END), " +
                    "country = (CASE" +
                                    " WHEN {2} is null THEN country " +
                                    " ELSE {2} END), " +
                    "address = (CASE" +
                                    " WHEN {2} is null THEN address " +
                                    " ELSE {2} END), " +
                    "age = (CASE" +
                                    " WHEN {2} is null THEN age " +
                                    " ELSE {2} END), " +
                    "email = (CASE" +
                                    " WHEN {3} is null THEN email " +
                                    " ELSE {3} END) " +
                    "WHERE identification = {4}",
                    string.IsNullOrEmpty(input.identification) ? null : input.identification,
                    string.IsNullOrEmpty(input.firstName) ? null : input.firstName,
                    string.IsNullOrEmpty(input.lastName) ? null : input.lastName,
                    string.IsNullOrEmpty(input.region) ? null : input.region,
                    string.IsNullOrEmpty(input.nationality) ? null : input.nationality,
                    string.IsNullOrEmpty(input.country) ? null : input.country,
                    string.IsNullOrEmpty(input.address) ? null : input.address,
                    input.age.HasValue ? input.age : null,
                    string.IsNullOrEmpty(input.email) ? null : input.email,
                    string.IsNullOrEmpty(input.address) ? null : input.address,
                    string.IsNullOrEmpty(input.email) ? null : input.email,
                    identification);

                if (rows < 1)
                {
                    throw ErrorQueryBuilder(
                        "NOTHING_TO_CHANGE",
                        "The identification code '{0}' doesn't match any contact."
                        , identification
                        );
                }
            }

            catch (SqlException e)
            {
                if (e.Number.Equals(547))
                {
                    errors.Add(CustomErrorBuilder(
                            e,
                            "You tried to change the identification code of contact with a related pathology, patient or hospital. " +
                            "Please delete all elements related before updating this patient. " +
                            "¿Or maybe you inserted the wrong country, or region?"));
                }
                else if (e.Number.Equals(2627))
                {
                    errors.Add(CustomErrorBuilder(
                            e,
                            "You tried to insert an identification code that already exists."));
                }
                else
                {
                    errors.Add(CustomErrorBuilder(
                            e,
                            "Unknown error."));
                }

            }
            catch (QueryException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new QueryException(ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            if (input.pathologies != null)
            {
                string currentPathology = "";

                /*dbPath - input > 0 => delete dbPath => loop no errors
                dbPath - input = 0 & dbPath.Count = input.Count => null => null
                dbPath - input = 0 & dbPath.Count < input.Count => insert input => loop with errors
                dbPath - input > 0 & dbPath.Count = input.Count => updatePatient dbPath*/


                ICollection<string> currentDB = dBContext.ContactPathology
                    .Where(p => p.ContactId.Equals(string.IsNullOrEmpty(input.identification) ? identification : input.identification))
                    .Select(s => new string(s.PathologyName))
                    .ToList();
                //.ConvertAll<string>(s => s.PathologyName);

                ICollection<string> fromDB = currentDB.Except(input.pathologies).ToList();

                ICollection<string> inputPath = input.pathologies.Except(currentDB).ToList();

                if (fromDB.Count > 0 && currentDB.Count > input.pathologies.Count)
                {
                    foreach (var path in fromDB)
                    {
                        currentPathology = path;
                        try
                        {
                            await dBContext.Database.ExecuteSqlRawAsync(
                                "DELETE FROM healthcare.Contact_Pathology " +
                                "WHERE Pathology_Name = {0} " +
                                "AND Contact_Id = {1}"
                                , path
                                , string.IsNullOrEmpty(input.identification) ? identification : input.identification);
                        }
                        catch (SqlException sqlException)
                        {
                            if (sqlException.Number.Equals(547))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The pathology with name '{0}', doesn't exists.", currentPathology));
                            }
                            else if (sqlException.Number.Equals(2628))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The pathology name specified is too long, try to use one under 150 chars long."));
                            }
                            else if (sqlException.Number.Equals(515))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "You must enter a value for pathology name."));
                            }
                            else
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "Unknown Error"));
                            }

                        }
                        catch (Exception e)
                        {
                            errors.Add(ErrorBuilder.New()
                               .SetMessage("Unknown error")
                               .SetCode(e.GetType().FullName)
                               .SetExtension("DatabaseMessage", e.Message)
                               .Build());
                        }
                    }
                }
                else if (fromDB.Count.Equals(0) && currentDB.Count < input.pathologies.Count)
                {
                    foreach (var path in inputPath)
                    {
                        currentPathology = path;
                        try
                        {
                            await dBContext.Database.ExecuteSqlRawAsync(
                                "INSERT INTO healthcare.Contact_Pathology(Pathology_Name, Contact_Id) " +
                                "VALUES (" +
                                "{0}," +
                                "{1})"
                                , path
                                , string.IsNullOrEmpty(input.identification) ? identification : input.identification);
                        }
                        catch (SqlException sqlException)
                        {
                            if (sqlException.Number.Equals(547))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The pathology with name '{0}', doesn't exists.", currentPathology));
                            }
                            else if (sqlException.Number.Equals(2628))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The pathology name specified is too long, try to use one under 150 chars long."));
                            }
                            else if (sqlException.Number.Equals(2627))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The Contact already has the pathology '{0}' assigned.", currentPathology));
                            }
                            else if (sqlException.Number.Equals(515))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "You must enter a value for pathology name."));
                            }
                            else
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "Unknown Error"));
                            }

                        }
                        catch (Exception e)
                        {
                            errors.Add(ErrorBuilder.New()
                               .SetMessage("Unknown error")
                               .SetCode(e.GetType().FullName)
                               .SetExtension("DatabaseMessage", e.Message)
                               .Build());
                        }
                    }
                }
                else if (fromDB.Count > 0 && currentDB.Count.Equals(input.pathologies.Count))
                {
                    foreach (var path in fromDB.Zip(inputPath, Tuple.Create))
                    {
                        currentPathology = path.Item2;
                        try
                        {
                            await dBContext.Database.ExecuteSqlRawAsync(
                                "UPDATE healthcare.Contact_Pathology " +
                                "SET Pathology_Name = {0} " +
                                "WHERE Pathology_Name = {1} " +
                                "AND Contact_Id = {2}"
                                , path.Item2
                                , path.Item1
                                , string.IsNullOrEmpty(input.identification) ? identification : input.identification);
                        }
                        catch (SqlException sqlException)
                        {
                            if (sqlException.Number.Equals(547))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The pathology with name '{0}', doesn't exists.", currentPathology));
                            }
                            else if (sqlException.Number.Equals(2628))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "The pathology name specified is too long, try to use one under 150 chars long."));
                            }
                            else if (sqlException.Number.Equals(515))
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "You must enter a value for pathology name."));
                            }
                            else
                            {
                                errors.Add(CustomErrorBuilder(
                                    sqlException,
                                    "Unknown Error"));
                            }

                        }
                        catch (Exception e)
                        {
                            errors.Add(ErrorBuilder.New()
                               .SetMessage("Unknown error")
                               .SetCode(e.GetType().FullName)
                               .SetExtension("DatabaseMessage", e.Message)
                               .Build());
                        }
                    }
                }
            }

            if (errors.Count > 0)
            {
                throw new QueryException(errors.AsEnumerable());
            }

            return await dBContext.Contact
                .Where(t => t.Identification.Equals(string.IsNullOrEmpty(input.identification) ? identification : input.identification))
                .Include(s => s.RegionNavigation)
                    .ThenInclude(r => r.CountryNavigation)
                        .ThenInclude(r => r.ContinentNavigation)
                .FirstOrDefaultAsync();
        }

        [GraphQLType(typeof(ContactType))]
        public async Task<Contact> deleteContact(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string identification)
        {
            if (string.IsNullOrEmpty(identification))
            {
                throw ErrorQueryBuilder(
                    "NAME_EMPTY",
                    "The name can't be empty.");
            }
            Contact p;

            try
            {
                p = await dBContext.Contact
                .Where(t => t.Identification.Equals(identification.Trim()))
                .Include(s => s.RegionNavigation)
                    .ThenInclude(r => r.CountryNavigation)
                        .ThenInclude(r => r.ContinentNavigation)
                .FirstOrDefaultAsync();

                int rows = await dBContext.Database.ExecuteSqlRawAsync(
                    "DELETE FROM healthcare.Contact " +
                    "WHERE identification = {0}", identification);

                if (rows < 1)
                {
                    throw ErrorQueryBuilder(
                        "CONTACT_NOT_FOUND",
                        "The contact with identification code '{0}' doesn't exists.", identification);
                }
            }
            catch (SqlException e)
            {
                if (e.Number.Equals(547))
                {
                    throw ErrorQueryBuilder(
                            e,
                            "You tried to delete a contact with a related pathology, patient or hospital. " +
                            "Please delete all elements related before deleting this contact.");
                }

                else
                {
                    throw ErrorQueryBuilder(
                            e,
                            "Unknown error.");
                }

            }
            catch (QueryException e)
            {
                throw e;
            }

            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return p;

        }

        [GraphQLType(typeof(HealthCenterType))]
        public async Task<Hospital> addHealthCenter(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] CreateHealthcenter input)
        {

            if (string.IsNullOrEmpty(input.name.Trim()))
            {
                throw ErrorQueryBuilder(
                    "HEALTHCENTER_KEY_INCOMPLETE",
                    "The name can't be empty."
                    );
            }

            if (string.IsNullOrEmpty(input.country.Trim()))
            {
                throw ErrorQueryBuilder(
                    "HEALTHCENTER_KEY_INCOMPLETE",
                    "The country can't be empty."
                    );
            }

            if (string.IsNullOrEmpty(input.region.Trim()))
            {
                throw ErrorQueryBuilder(
                    "HEALTHCENTER_KEY_INCOMPLETE",
                    "The region can't be empty."
                    );
            }

            try
            {
                await dBContext.Database.ExecuteSqlRawAsync(
                    "INSERT INTO admin.Hospital " +
                    "(Name," +
                    "region," +
                    "country," +
                    "Capacity," +
                    "ICU_Capacity," +
                    "Manager) " +
                    "VALUES (" +
                    "{0}," +
                    "{1}," +
                    "{2}," +
                    "{3}," +
                    "{4}," +
                    "{5})",
                    input.name,
                    input.region,
                    input.country,
                    input.capacity,
                    input.totalICUBeds,
                    input.directorContact);
                /*if (!string.IsNullOrEmpty(input.description) && string.IsNullOrEmpty(input.treatment))
                {
                    
                }
                else if (!string.IsNullOrEmpty(input.treatment) && string.IsNullOrEmpty(input.description))
                {
                    await dBContext.Database.ExecuteSqlRawAsync(
                    "INSERT INTO admin.Pathology ([name], [treatment]) " +
                    "VALUES (" +
                    "{0}," +
                    "{1})",
                    input.name,
                    input.treatment);
                }
                else if (!string.IsNullOrEmpty(input.treatment) && !string.IsNullOrEmpty(input.description))
                {
                    await dBContext.Database.ExecuteSqlRawAsync(
                    "INSERT INTO admin.Pathology ([name], [treatment], [description]) " +
                    "VALUES (" +
                    "{0}," +
                    "{1}," +
                    "{2})",
                    input.name,
                    input.treatment,
                    input.description);
                }
                else
                {
                    await dBContext.Database.ExecuteSqlRawAsync(
                    "INSERT INTO admin.Pathology ([name]) " +
                    "VALUES (" +
                    "{0})",
                    input.name);
                }*/
            }
            catch (SqlException sqlException)
            {
                if (sqlException.Number.Equals(2627))
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        "The hospital with name '{0}', country '{1}' and region '{2}' already exists.", input.name, input.country, input.region);

                }
                else if (sqlException.Number.Equals(2628))
                {

                    throw ErrorQueryBuilder(
                        sqlException,
                        "The name, country or region specified is too long, try to use one under 100 or 80 chars long.");
                }
                else if (sqlException.Number.Equals(515))
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        "You must enter a value for name, country and region field.");
                }
                else
                {
                    throw ErrorQueryBuilder(
                        sqlException,
                        "Unknown Error");
                }

            }
            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return await dBContext.Hospital
                .Where(s => s.Name.Contains(input.name.Trim()) &&
                s.Region.Contains(input.region.Trim()) &&
                s.Country.Contains(input.country.Trim()))
                .Include(s => s.ManagerNavigation)
                .Include(s => s.RegionNavigation)
                    .ThenInclude(r => r.CountryNavigation)
                        .ThenInclude(r => r.ContinentNavigation)
                .FirstOrDefaultAsync();
        }

        [GraphQLType(typeof(HealthCenterType))]
        public async Task<Hospital> updateHealthCenter(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string name,
            [GraphQLNonNullType] string region,
            [GraphQLNonNullType] string country,
            [GraphQLNonNullType] UpdateHealthcenter input)
        {

            List<IError> errors = new List<IError>();

            try
            {
                int rows = await dBContext.Database.ExecuteSqlRawAsync(
                    "UPDATE admin.Hospital " +
                    "SET Name = (CASE" +
                                    " WHEN {0} is null THEN Name " +
                                    " ELSE {0} END), " +
                    "region = (CASE" +
                                    " WHEN {1} is null THEN region " +
                                    " ELSE {1} END) " +
                    "country = (CASE" +
                                    " WHEN {2} is null THEN country " +
                                    " ELSE {2} END), " +
                    "Capacity = (CASE" +
                                    " WHEN {3} is null THEN Capacity " +
                                    " ELSE {3} END), " +
                    "ICU_Capacity = (CASE" +
                                    " WHEN {4} is null THEN ICU_Capacity " +
                                    " ELSE {4} END), " +
                    "Manager = (CASE" +
                                    " WHEN {5} is null THEN Manager " +
                                    " ELSE {5} END), " +
                    "WHERE Name = {6}, country = {7}, region = {8}",
                    string.IsNullOrEmpty(input.name.Trim()) ? null : input.name.Trim(),
                    string.IsNullOrEmpty(input.region) ? null : input.region,
                    string.IsNullOrEmpty(input.country) ? null : input.country,
                    input.capacity.HasValue ? input.capacity : null,
                    input.totalICUBeds.HasValue ? input.capacity : null,
                    string.IsNullOrEmpty(input.directorContact) ? null : input.directorContact);

                if (rows < 1)
                {
                    throw ErrorQueryBuilder(
                        "NOTHING_TO_CHANGE",
                        "The name '{0}', country '{1}' and region '{2}' doesn't match any hospital."
                        , name
                        , country
                        , region);
                }
            }

            catch (SqlException e)
            {
                if (e.Number.Equals(547))
                {
                    errors.Add(CustomErrorBuilder(
                            e,
                            "You tried to change the name, country or region of a hospital with a related worker. " +
                            "Please delete all elements related before deleting this hospital."));
                }
                else if (e.Number.Equals(2627))
                {
                    errors.Add(CustomErrorBuilder(
                            e,
                            "You tried to insert a hospital with a name, country and region that already exists."));
                }
                else
                {
                    errors.Add(CustomErrorBuilder(
                            e,
                            "Unknown error."));
                }

            }
            catch (QueryException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new QueryException(ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            if (errors.Count > 0)
            {
                throw new QueryException(errors.AsEnumerable());
            }

            return await dBContext.Hospital
                .Where(s => s.Name.Contains(string.IsNullOrEmpty(input.name.Trim()) ? name : input.name.Trim()) &&
                s.Region.Contains(string.IsNullOrEmpty(input.region.Trim()) ? region : input.name.Trim()) &&
                s.Country.Contains(string.IsNullOrEmpty(input.country.Trim()) ? country : input.name.Trim()))
                .Include(s => s.ManagerNavigation)
                .Include(s => s.RegionNavigation)
                    .ThenInclude(r => r.CountryNavigation)
                        .ThenInclude(r => r.ContinentNavigation)
                .FirstOrDefaultAsync();
        }


        [GraphQLType(typeof(HealthCenterType))]
        public async Task<Hospital> deleteHealthCenter(
            [Service] CoTEC_DBContext dBContext,
            [GraphQLNonNullType] string name,
            [GraphQLNonNullType] string region,
            [GraphQLNonNullType] string country)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw ErrorQueryBuilder(
                    "NAME_EMPTY",
                    "The name can't be empty.");
            }

            if (string.IsNullOrEmpty(region))
            {
                throw ErrorQueryBuilder(
                    "REGION_EMPTY",
                    "The region can't be empty.");
            }

            if (string.IsNullOrEmpty(country))
            {
                throw ErrorQueryBuilder(
                    "COUNTRY_EMPTY",
                    "The country can't be empty.");
            }

            Hospital p;

            try
            {
                p = await dBContext.Hospital
                .Where(s => s.Name.Contains(name) &&
                s.Region.Contains(region) &&
                s.Country.Contains(country))
                .Include(s => s.ManagerNavigation)
                .Include(s => s.RegionNavigation)
                    .ThenInclude(r => r.CountryNavigation)
                        .ThenInclude(r => r.ContinentNavigation)
                .FirstOrDefaultAsync();

                int rows = await dBContext.Database.ExecuteSqlRawAsync(
                    "DELETE FROM healthcare.Hospital " +
                    "WHERE Name = {0}, country = {1}, region = {2}", name, country, region);

                if (rows < 1)
                {
                    throw ErrorQueryBuilder(
                        "HOSPITAL_NOT_FOUND",
                        "The hospital with name '{0}', country '{1}' and region '{2}' doesn't exists.", name, country, region);
                }
            }
            catch (SqlException e)
            {
                if (e.Number.Equals(547))
                {
                    throw ErrorQueryBuilder(
                            e,
                            "You tried to delete a hospital with a related worker. " +
                            "Please delete all elements related before deleting this hospital.");
                }

                else
                {
                    throw ErrorQueryBuilder(
                            e,
                            "Unknown error.");
                }

            }
            catch (QueryException e)
            {
                throw e;
            }

            catch (Exception e)
            {
                throw new QueryException(
                       ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }

            return p;

        }
    }
}
