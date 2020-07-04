using CoTEC_Server.DBModels;
using CoTEC_Server.Logic.GraphQL.Types;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Execution;
using HotChocolate.Types;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
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
        //[Authorize(Policy = Constants.AdminPolicyName)]
        public async Task<List<Continent>> continents(
            [Service] CoTEC_DBContext dbCont,
            string name)
        {
            try
            {
                if (name == null)
                {
                    return await dbCont.Continent.Include(s => s.Country).ToListAsync();
                }

                return await dbCont.Continent.Where(s => s.Name.Contains(name.Trim())).Include(s => s.Country).ToListAsync();
            }
            catch (Exception e)
            {
                throw new QueryException(ErrorBuilder.New()
                           .SetMessage("Unknown error")
                           .SetCode(e.GetType().FullName)
                           .SetExtension("DatabaseMessage", e.Message)
                           .Build());
            }
        }


        /// <summary>
        /// Get the continents available
        /// </summary>
        /// <param name="name">Filter results by name.</param>
        [GraphQLType(typeof(NonNullType<ListType<NonNullType<RegionType>>>))]
        //[Authorize(Policy = Constants.HealthCenterPolicyName)]
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
        //[Authorize(Policy = Constants.HealthCenterPolicyName)]
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

        /// <summary>
        /// Get the patient states available
        /// </summary>
        [GraphQLType(typeof(StringType))]
        public async Task<string> newCasesReport([Service] CoTEC_DBContext dbCont)
        {
            List<CasesAndDeaths> patients = await dbCont.CasesAndDeaths
                .FromSqlRaw("EXECUTE user_public.deaths_and_cases").ToListAsync();

            MemoryStream m = new MemoryStream();

            PdfWriter writer = new PdfWriter(m);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf, PageSize.A4, false);


            Paragraph header = new Paragraph("New cases and deaths report")
               .SetTextAlignment(TextAlignment.CENTER)
               .SetFontSize(30);

            document.Add(header);

            LineSeparator ls = new LineSeparator(new SolidLine());
            document.Add(ls);

            Table table = new Table(9, false);
            Cell h1 = new Cell(1, 1)
               .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .SetTextAlignment(TextAlignment.CENTER)
               .Add(new Paragraph("Country"));

            Cell h2 = new Cell(1, 1)
               .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .SetTextAlignment(TextAlignment.CENTER)
               .Add(new Paragraph("Type"));

            Cell h3 = new Cell(1, 1)
               .SetTextAlignment(TextAlignment.CENTER)
               .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .Add(new Paragraph(DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd")));

            Cell h4 = new Cell(1, 1)
               .SetTextAlignment(TextAlignment.CENTER)
               .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .Add(new Paragraph(DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd")));

            Cell h5 = new Cell(1, 1)
               .SetTextAlignment(TextAlignment.CENTER)
               .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .Add(new Paragraph(DateTime.Now.AddDays(-4).ToString("yyyy-MM-dd")));

            Cell h6 = new Cell(1, 1)
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .SetTextAlignment(TextAlignment.CENTER)
               .Add(new Paragraph(DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd")));

            Cell h7 = new Cell(1, 1)
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .SetTextAlignment(TextAlignment.CENTER)
               .Add(new Paragraph(DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd")));

            Cell h8 = new Cell(1, 1)
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .SetTextAlignment(TextAlignment.CENTER)
               .Add(new Paragraph(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")));

            Cell h9 = new Cell(1, 1)
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .SetTextAlignment(TextAlignment.CENTER)
               .Add(new Paragraph(DateTime.Now.ToString("yyyy-MM-dd")));

            table.AddHeaderCell(h1);
            table.AddHeaderCell(h2);
            table.AddHeaderCell(h3);
            table.AddHeaderCell(h4);
            table.AddHeaderCell(h5);
            table.AddHeaderCell(h6);
            table.AddHeaderCell(h7);
            table.AddHeaderCell(h8);
            table.AddHeaderCell(h9);


            foreach (var p in patients)
            {
                Cell r1 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Country));

                Cell r2 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Type));

                Cell r3 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Fecha1.ToString()));

                Cell r4 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Fecha2.ToString()));

                Cell r5 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Fecha3.ToString()));

                Cell r6 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Fecha4.ToString()));

                Cell r7 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Fecha5.ToString()));

                Cell r8 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Fecha6.ToString()));

                Cell r9 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Fecha7.ToString()));

                table.AddCell(r1);
                table.AddCell(r2);
                table.AddCell(r3);
                table.AddCell(r4);
                table.AddCell(r5);
                table.AddCell(r6);
                table.AddCell(r7);
                table.AddCell(r8);
                table.AddCell(r9);
            }

            Paragraph newline = new Paragraph(new Text("\n"));

            table.UseAllAvailableWidth();

            document.Add(newline);
            document.Add(table);


            int n = pdf.GetNumberOfPages();
            for (int i = 1; i <= n; i++)
            {
                document.ShowTextAligned(new Paragraph(string
                   .Format("page" + i + " of " + n)),
                   559, 806, i, TextAlignment.RIGHT,
                   VerticalAlignment.TOP, 0);
            }

            document.Close();

            return Convert.ToBase64String(m.ToArray());
        }

        /// <summary>
        /// Get the patient states available
        /// </summary>
        [GraphQLType(typeof(StringType))]
        public async Task<string> patientsReport([Service] CoTEC_DBContext dbCont)
        {
            List<PatientsReport> patients = await dbCont.PatientsReport
                .FromSqlRaw("EXECUTE user_public.patients_by_country").ToListAsync();

            MemoryStream m = new MemoryStream();

            PdfWriter writer = new PdfWriter(m);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf, PageSize.A4, false);


            Paragraph header = new Paragraph("Patients report")
               .SetTextAlignment(TextAlignment.CENTER)
               .SetFontSize(30);

            document.Add(header);

            LineSeparator ls = new LineSeparator(new SolidLine());
            document.Add(ls);

            Table table = new Table(5, false);
            Cell cell11 = new Cell(1, 1)
               .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .SetTextAlignment(TextAlignment.CENTER)
               .Add(new Paragraph("Country"));

            Cell cell12 = new Cell(1, 1)
               .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .SetTextAlignment(TextAlignment.CENTER)
               .Add(new Paragraph("Infected"));

            Cell cell21 = new Cell(1, 1)
               .SetTextAlignment(TextAlignment.CENTER)
               .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .Add(new Paragraph("Recovered"));

            Cell cell22 = new Cell(1, 1)
               .SetTextAlignment(TextAlignment.CENTER)
               .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .Add(new Paragraph("Dead"));

            Cell cell31 = new Cell(1, 1)
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .SetTextAlignment(TextAlignment.CENTER)
               .Add(new Paragraph("Active"));

            table.AddCell(cell11);
            table.AddCell(cell12);
            table.AddCell(cell21);
            table.AddCell(cell22);
            table.AddCell(cell31);

            foreach (var p in patients)
            {
                Cell r1 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Country));

                Cell r2 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Infected.ToString()));

                Cell r3 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Cured.ToString()));

                Cell r4 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Dead.ToString()));

                Cell r5 = new Cell(1, 1)
                   .SetTextAlignment(TextAlignment.CENTER)
                   .Add(new Paragraph(p.Active.ToString()));

                table.AddCell(r1);
                table.AddCell(r2);
                table.AddCell(r3);
                table.AddCell(r4);
                table.AddCell(r5);
            }

            Paragraph newline = new Paragraph(new Text("\n"));

            table.UseAllAvailableWidth();

            document.Add(newline);
            document.Add(table);


            int n = pdf.GetNumberOfPages();
            for (int i = 1; i <= n; i++)
            {
                document.ShowTextAligned(new Paragraph(string
                   .Format("page" + i + " of " + n)),
                   559, 806, i, TextAlignment.RIGHT,
                   VerticalAlignment.TOP, 0);
            }

            document.Close();

            return Convert.ToBase64String(m.ToArray());

        }

        [GraphQLType(typeof(NonNullType<ListType<NonNullType<HospitalWorkerType>>>))]
        public async Task<List<Staff>> hospitalWorkers([Service] CoTEC_DBContext dbCont)
        {
            return await dbCont.Staff
                .Where(s => s.Role.Equals("hospitalworker"))
                .ToListAsync();
        }



        

    }
}
