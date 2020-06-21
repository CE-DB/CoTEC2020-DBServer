using CoTEC_Server.Database;
using CoTEC_Server.Logic.GraphQL.Types;
using HotChocolate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoTEC_Server.Logic.GraphQL
{
    public class Mutation
    {
        public Mutation()
        { }

        
        /// <summary>
        /// Add new region.
        /// </summary>
        /// <param name="countryName">Country name where region belongs to.</param>
        /// <param name="name">Name of the country where region belongs to</param>
        [GraphQLNonNullType]
        public async Task<Region> addRegion([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string name, 
            [GraphQLNonNullType] string countryName)
        {

            if (countryName == null)
            {
                return null;
            }

            return null;
        }


        /// <summary>
        /// Update a region.
        /// </summary>
        /// <param name="oldName">Name of the region to update.</param>
        /// <param name="newName">New name for region, send nothing for keeping old value.</param>
        /// <param name="newCountry">New name of country, send nothing for keeping old value.</param>
        /// <param name="oldCountry">Old name of country.</param>
        [GraphQLNonNullType]
        public async Task<Region> updateRegion([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string oldName,
            [GraphQLNonNullType] string oldCountry,
            string newName,
            string newCountry)
        {

            return null;
        }

        /// <summary>
        /// Delete a region.
        /// </summary>
        /// <param name="Name">Name of the region to delete.</param>
        [GraphQLNonNullType]
        public async Task<Region> deleteRegion([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string name)
        {

            return null;
        }


        /// <summary>
        /// Add new patient.
        /// </summary>
        /// <param name="firstName">Name of patient.</param>
        /// <param name="lastName">Last name of patient.</param>
        /// <param name="identification">Identification code.</param>
        /// <param name="age">Age vnumber</param>
        /// <param name="nationality">Nationality of patient.</param>
        /// <param name="hospitalized">Indicates if patient is hospitalized.</param>
        /// <param name="intensiveCareUnite">Indicates if patient is in ICU.</param>
        /// <param name="region">Region where patient lives.</param>
        /// <param name="country">Country where patient lives.</param>
        /// <param name="pathologies">List of pathology names the patient has.</param>
        /// <param name="state">State of patient (dead, infected, recovered...)</param>
        /// <param name="latestContacts">List of identification numbers of visitors of the patient.</param>
        [GraphQLNonNullType]
        public async Task<Patient> addPatient([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string firstName,
            [GraphQLNonNullType] string lastName,
            [GraphQLNonNullType] string identification,
            [GraphQLNonNullType] int age,
            [GraphQLNonNullType] string nationality,
            [GraphQLNonNullType] bool hospitalized,
            [GraphQLNonNullType] bool intensiveCareUnite,
            [GraphQLNonNullType] string region,
            [GraphQLNonNullType] string country,
            [GraphQLNonNullType] List<string> pathologies,
            [GraphQLNonNullType] string state,
            [GraphQLNonNullType] List<string> latestContacts)
        {

            return null;
        }


        /// <summary>
        /// Updates a patient.
        /// </summary>
        /// <param name="oldIdentification">Identification code of patient to update.</param>
        /// <param name="firstName">Name of patient.</param>
        /// <param name="lastName">Last name of patient.</param>
        /// <param name="newIdentification">New identification code.</param>
        /// <param name="age">Age vnumber</param>
        /// <param name="nationality">Nationality of patient.</param>
        /// <param name="hospitalized">Indicates if patient is hospitalized.</param>
        /// <param name="intensiveCareUnite">Indicates if patient is in ICU.</param>
        /// <param name="region">Region where patient lives.</param>
        /// <param name="country">Country where patient lives.</param>
        /// <param name="pathologies">List of pathology names the patient has.</param>
        /// <param name="state">State of patient (dead, infected, recovered...)</param>
        /// <param name="latestContacts">List of identification numbers of visitors of the patient.</param>
        [GraphQLNonNullType]
        public async Task<Patient> updatePatient([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string oldIdentification,
            string firstName,
            string lastName,
            string newIdentification,
            [GraphQLNonNullType(IsElementNullable = true, IsNullable = true)] int age,
            string nationality,
            bool? hospitalized,
            bool? intensiveCareUnite,
            string region,
            string country,
            [GraphQLNonNullType(IsElementNullable = false, IsNullable = true)] List<string> pathologies,
            string state,
            [GraphQLNonNullType(IsElementNullable = false, IsNullable = true)] List<string> latestContacts)
        {

            return null;
        }

        /// <summary>
        /// Delete a patient.
        /// </summary>
        /// <param name="identification">Identification code of patient to delete.</param>
        [GraphQLNonNullType]
        public async Task<Patient> deletePatient([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string identification)
        {

            return null;
        }


        /// <summary>
        /// Add new contact.
        /// </summary>
        /// <param name="firstName">Name of contact.</param>
        /// <param name="lastName">Last name of contact.</param>
        /// <param name="identification">Identification code.</param>
        /// <param name="age">Age vnumber</param>
        /// <param name="nationality">Nationality of contact.</param>
        /// <param name="address">Esact contact address.</param>
        /// <param name="email">Contact email address.</param>
        /// <param name="region">Region where contact lives.</param>
        /// <param name="country">Country where contact lives.</param>
        /// <param name="pathologies">List of pathology names the contact has.</param>
        [GraphQLNonNullType]
        public async Task<Contact> addContact([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string firstName,
            [GraphQLNonNullType] string lastName,
            [GraphQLNonNullType] string identification,
            [GraphQLNonNullType] int age,
            [GraphQLNonNullType] string nationality,
            [GraphQLNonNullType] string address,
            [GraphQLNonNullType] string region,
            [GraphQLNonNullType] string country,
            [GraphQLNonNullType] List<string> pathologies,
            [GraphQLNonNullType] string email)
        {

            return null;
        }


        /// <summary>
        /// Updates a contact.
        /// </summary>
        /// <param name="oldIdentification">Identification code of contact to update.</param>
        /// <param name="firstName">Name of contact.</param>
        /// <param name="lastName">Last name of contact.</param>
        /// <param name="newIdentification">New identification code.</param>
        /// <param name="age">Age number</param>
        /// <param name="nationality">Nationality of contact.</param>
        /// <param name="region">Region where contact lives.</param>
        /// <param name="country">Country where contact lives.</param>
        /// <param name="pathologies">List of pathology names the contact has.</param>
        /// <param name="address">Exact contact address.</param>
        /// <param name="email">Contact email address.</param>
        [GraphQLNonNullType]
        public async Task<Contact> updateContact([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string oldIdentification,
            string firstName,
            string lastName,
            string newIdentification,
            [GraphQLNonNullType(IsNullable = true, IsElementNullable = true)] int age,
            string nationality,
            string address,
            string region,
            string country,
            [GraphQLNonNullType(IsNullable = true, IsElementNullable = false)] List<string> pathologies,
            string email)
        {

            return null;
        }

        /// <summary>
        /// Delete a contact.
        /// </summary>
        /// <param name="identification">Identification code of contact to delete.</param>
        [GraphQLNonNullType]
        public async Task<Contact> deleteContact([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string identification)
        {

            return null;
        }

        /// <summary>
        /// Add new pathology.
        /// </summary>
        /// <param name="name">Name of pathology.</param>
        /// <param name="description">Description of pathology.</param>
        /// <param name="symptoms">List of symptoms name.</param>
        /// <param name="treatment">Treatment description.</param>
        [GraphQLNonNullType]
        public async Task<Pathology> addPathology([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string name,
            [GraphQLNonNullType] string description,
            [GraphQLNonNullType] List<string> symptoms,
            [GraphQLNonNullType] string treatment)
        {

            return null;
        }


        /// <summary>
        /// Update a pathology.
        /// </summary>
        /// <param name="oldName">Name of pathology to update.</param>
        /// <param name="newName">New name of pathology.</param>
        /// <param name="description">Description of pathology.</param>
        /// <param name="symptoms">List of symptoms name.</param>
        /// <param name="treatment">Treatment description.</param>
        [GraphQLNonNullType]
        public async Task<Pathology> updatePathology([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string oldName,
            string newName,
            string description,
            [GraphQLNonNullType(IsElementNullable = false, IsNullable = true)] List<string> symptoms,
            string treatment)
        {

            return null;
        }


        /// <summary>
        /// Delete a pathology.
        /// </summary>
        /// <param name="name">Name of pathology to delete.</param>
        [GraphQLNonNullType]
        public async Task<Pathology> deletePathology([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string name)
        {

            return null;
        }


        /// <summary>
        /// Add new patient state.
        /// </summary>
        /// <param name="name">Name of state.</param>
        [GraphQLNonNullType]
        public async Task<PatientState> addPatientState([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string name)
        {

            return null;
        }

        /// <summary>
        /// Update a patient state.
        /// </summary>
        /// <param name="oldName">Name of state to update.</param>
        /// <param name="newName">New name of state.</param>
        [GraphQLNonNullType]
        public async Task<PatientState> updatePatientState([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string oldName,
            string newName)
        {

            return null;
        }

        /// <summary>
        /// Delete a patient state.
        /// </summary>
        /// <param name="name">Name of patient state to delete.</param>
        [GraphQLNonNullType]
        public async Task<PatientState> deletePatientState([Service] SQLServerContext dbCont,
           [GraphQLNonNullType] string name)
        {

            return null;
        }

        /// <summary>
        /// Add new health center.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="capacity">Total spaces of capacity</param>
        /// <param name="totalICUbeds">Total spaces of ICU.</param>
        /// <param name="directorContact">Identification code of hospital manager</param>
        /// <param name="region">Region name of location</param>
        /// <param name="country">Country name of location</param>
        [GraphQLNonNullType]
        public async Task<Healthcenter> addHealthCenter([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string name,
            [GraphQLNonNullType] int capacity,
            [GraphQLNonNullType] int totalICUbeds,
            [GraphQLNonNullType] string directorContact,
            [GraphQLNonNullType] string region,
            [GraphQLNonNullType] string country)
        {

            return null;
        }


        /// <summary>
        /// Update a health center.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="capacity">Total spaces of capacity</param>
        /// <param name="totalICUbeds">Total spaces of ICU.</param>
        /// <param name="directorContact">Identification code of hospital manager</param>
        /// <param name="region">Region name of location</param>
        /// <param name="country">Country name of location</param>
        /// <param name="oldCountry">Country of hospital to update.</param>
        /// <param name="oldName">Name of hospital to update</param>
        /// <param name="oldRegion">Region of hospital to update</param>
        [GraphQLNonNullType]
        public async Task<Healthcenter> updateHealthCenter([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string oldName,
            [GraphQLNonNullType] string oldRegion,
            [GraphQLNonNullType] string oldCountry,
            string name,
            [GraphQLNonNullType(IsNullable = true, IsElementNullable = true)] int capacity,
            [GraphQLNonNullType(IsNullable = true, IsElementNullable = true)] int totalICUbeds,
            string directorContact,
            string region,
            string country)
        {

            return null;
        }

        /// <summary>
        /// Delete a health center.
        /// </summary>
        /// <param name="name">Name of hospital to delete.</param>
        /// <param name="country">Name of country of hospital to delete.</param>
        /// <param name="region">Name of region of hospital to delete</param>
        [GraphQLNonNullType]
        public async Task<Healthcenter> deleteHealthCenter([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string name,
            [GraphQLNonNullType] string country,
            [GraphQLNonNullType] string region)
        {

            return null;
        }


        /// <summary>
        /// Add new Contention measure
        /// </summary>
        /// <param name="name">Name of contention measure.</param>
        /// <param name="description">Description of contention measure.</param>
        /// <param name="startDate">The date of activation of the measure. Format is YYYY-MM-DD.</param>
        /// <param name="endDate">The date of desactivation of the measure. Format is YYYY-MM-DD</param>
        /// <param name="country">Country name that activate the measure.</param>
        [GraphQLNonNullType]
        public async Task<ContentionMeasure> addContentionMeasure([Service] SQLServerContext dbCont,
           [GraphQLNonNullType] string name,
           [GraphQLNonNullType] string description)
        {

            return null;
        }

        /// <summary>
        /// update a Contention measure
        /// </summary>
        /// <param name="oldName">Name of contention measure to update</param>
        /// <param name="name">Name of contention measure.</param>
        /// <param name="description">Description of contention measure.</param>
        /// <param name="startDate">The date of activation of the measure. Format is YYYY-MM-DD.</param>
        /// <param name="endDate">The date of desactivation of the measure. Format is YYYY-MM-DD</param>
        /// <param name="country">Country name that activate the measure.</param>
        [GraphQLNonNullType]
        public async Task<ContentionMeasure> updateContentionMeasure([Service] SQLServerContext dbCont,
           [GraphQLNonNullType] string oldName,
            string name,
            string description)
        {

            return null;
        }

        /// <summary>
        /// Delete a Contention measure
        /// </summary>
        /// <param name="name">Name of contention measure.</param>
        [GraphQLNonNullType]
        public async Task<ContentionMeasure> deleteContentionMeasure([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string name)
        {

            return null;
        }


        /// <summary>
        /// Add new Sanitary measure
        /// </summary>
        /// <param name="name">Name of Sanitary measure.</param>
        /// <param name="description">Description of Sanitary measure.</param>
        /// <param name="startDate">The date of activation of the measure. Format is YYYY-MM-DD.</param>
        /// <param name="endDate">The date of desactivation of the measure. Format is YYYY-MM-DD</param>
        /// <param name="country">Country name that activate the measure.</param>
        [GraphQLNonNullType]
        public async Task<SanitaryMeasure> addSanitaryMeasure([Service] SQLServerContext dbCont,
           [GraphQLNonNullType] string name,
            [GraphQLNonNullType] string description)
        {

            return null;
        }

        /// <summary>
        /// update a Sanitary measure
        /// </summary>
        /// <param name="oldName">Name of Sanitary measure to update</param>
        /// <param name="name">Name of Sanitary measure.</param>
        /// <param name="description">Description of Sanitary measure.</param>
        /// <param name="startDate">The date of activation of the measure. Format is YYYY-MM-DD.</param>
        /// <param name="endDate">The date of desactivation of the measure. Format is YYYY-MM-DD</param>
        /// <param name="country">Country name that activate the measure.</param>
        [GraphQLNonNullType]
        public async Task<SanitaryMeasure> updateSanitaryMeasure([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string oldName,
            string name,
            string description)
        {

            return null;
        }


        /// <summary>
        /// Delete a Sanitary measure
        /// </summary>
        /// <param name="name">Name of Sanitary measure.</param>
        [GraphQLNonNullType]
        public async Task<SanitaryMeasure> deleteSanitaryMeasure([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string name)
        {

            return null;
        }

        /// <summary>
        /// Add new medication
        /// </summary>
        /// <param name="name">Name of medication.</param>
        /// <param name="pharmaceutical">Pharmaceutical house that made the medication.</param>
        [GraphQLNonNullType]
        public async Task<Medication> addMedication([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string name,
            [GraphQLNonNullType] string pharmaceutical)
        {

            return null;
        }

        /// <summary>
        /// Update a medication
        /// </summary>
        /// <param name="name">Name of medication.</param>
        /// <param name="pharmaceutical">Pharmaceutical house that made the medication.</param>
        /// <param name="oldName">Name of the medication to update.</param>
        [GraphQLNonNullType]
        public async Task<SanitaryMeasure> updateMedication([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string oldName,
            string name,
            string pharmaceutical)
        {

            return null;
        }


        /// <summary>
        /// Delete a medication
        /// </summary>
        /// <param name="name">Name of medication to delete.</param>
        [GraphQLNonNullType]
        public async Task<Medication> deleteMedication([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string name)
        {

            return null;
        }

        /// <summary>
        /// Add new Sanitary measure relation to country.
        /// </summary>
        /// <param name="name">Name of Sanitary measure.</param>
        /// <param name="startDate">The date of activation of the measure. Format is YYYY-MM-DD.</param>
        /// <param name="endDate">The date of desactivation of the measure. Format is YYYY-MM-DD</param>
        /// <param name="country">Country name that activate the measure.</param>
        [GraphQLNonNullType]
        public async Task<SanitaryMeasure> addSanitaryMeasureEvent([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string name,
            [GraphQLNonNullType] string country,
            [GraphQLNonNullType] DateTime startDate,
            [GraphQLNonNullType] DateTime endDate)
        {

            return null;
        }

        /// <summary>
        /// Update a Sanitary measure relation to country.
        /// </summary>
        /// <param name="name">Name of Sanitary measure.</param>
        /// <param name="startDate">The date of activation of the measure. Format is YYYY-MM-DD.</param>
        /// <param name="endDate">The date of desactivation of the measure. Format is YYYY-MM-DD</param>
        /// <param name="country">Country name that activate the measure.</param>
        /// <param name="oldName">Old Name of Sanitary measure.</param>
        /// <param name="oldStartDate">The old date of activation of the measure. Format is YYYY-MM-DD.</param>
        /// <param name="oldCountry">Old Country name that activate the measure.</param>
        [GraphQLNonNullType]
        public async Task<SanitaryMeasure> updateSanitaryMeasureEvent([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string oldName,
            [GraphQLNonNullType] string oldCountry,
            [GraphQLNonNullType] DateTime oldStartDate,
            string name,
            string country,
            DateTime? startDate,
            DateTime? endDate)
        {

            return null;
        }


        /// <summary>
        /// Delete a Sanitary measure event (only marks it like active, they really cannot being deleted.)
        /// </summary>
        /// <param name="Name">Old Name of Sanitary measure.</param>
        /// <param name="StartDate">The old date of activation of the measure. Format is YYYY-MM-DD.</param>
        /// <param name="Country">Old Country name that activate the measure.</param>
        [GraphQLNonNullType]
        public async Task<SanitaryMeasure> deleteSanitaryMeasureEvent([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string Name,
            [GraphQLNonNullType] string Country,
            [GraphQLNonNullType] DateTime StartDate)
        {

            return null;
        }

        /// <summary>
        /// Add new Contention measure relation to country.
        /// </summary>
        /// <param name="name">Name of Contention measure.</param>
        /// <param name="startDate">The date of activation of the measure. Format is YYYY-MM-DD.</param>
        /// <param name="endDate">The date of desactivation of the measure. Format is YYYY-MM-DD</param>
        /// <param name="country">Country name that activate the measure.</param>
        [GraphQLNonNullType]
        public async Task<ContentionMeasure> addContentionMeasureEvent([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string name,
            [GraphQLNonNullType] string country,
            [GraphQLNonNullType] DateTime startDate,
            [GraphQLNonNullType] DateTime endDate)
        {

            return null;
        }

        /// <summary>
        /// Update a Contention measure relation to country.
        /// </summary>
        /// <param name="name">Name of Contention measure.</param>
        /// <param name="startDate">The date of activation of the measure. Format is YYYY-MM-DD.</param>
        /// <param name="endDate">The date of desactivation of the measure. Format is YYYY-MM-DD</param>
        /// <param name="country">Country name that activate the measure.</param>
        /// <param name="oldName">Old Name of Contention measure.</param>
        /// <param name="oldStartDate">The old date of activation of the measure. Format is YYYY-MM-DD.</param>
        /// <param name="oldCountry">Old Country name that activate the measure.</param>
        [GraphQLNonNullType]
        public async Task<ContentionMeasure> updateContentionMeasureEvent([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string oldName,
            [GraphQLNonNullType] string oldCountry,
            [GraphQLNonNullType] DateTime oldStartDate,
            string name,
            string country,
            DateTime? startDate,
            DateTime? endDate)
        {

            return null;
        }


        /// <summary>
        /// Delete a Contention measure event (only marks it like active, they really cannot being deleted.)
        /// </summary>
        /// <param name="Name">Old Name of Contention measure.</param>
        /// <param name="StartDate">The old date of activation of the measure. Format is YYYY-MM-DD.</param>
        /// <param name="Country">Old Country name that activate the measure.</param>
        [GraphQLNonNullType]
        public async Task<ContentionMeasure> deleteContentionMeasureEvent([Service] SQLServerContext dbCont,
            [GraphQLNonNullType] string Name,
            [GraphQLNonNullType] string Country,
            [GraphQLNonNullType] DateTime StartDate)
        {

            return null;
        }
    }
}
