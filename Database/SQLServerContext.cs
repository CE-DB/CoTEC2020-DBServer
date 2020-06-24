using CoTEC_Server.Logic.GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace CoTEC_Server.Database
{
    public class SQLServerContext : DbContext
    {
        public static string DbConnectionString = "Server=localhost;Database=CoTEC_DB;User Id=CoTEC_ServerApp_Login;Password=2Lfy.KMNwe{{>&@AZ&A3";

        public SQLServerContext(DbContextOptions<SQLServerContext> options) : base(options)
        { }


        public DbSet<Location> Locations { get; set; }

        public DbSet<Contact> Contacts { get; set; }

        public DbSet<ContentionMeasure> ContentionMeasures { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Healthcenter> HealthCenters { get; set; }

        public DbSet<Increment> Increments { get; set; }

        public DbSet<Medication> Medications { get; set; }

        public DbSet<Pathology> Pathologies { get; set; }

        public DbSet<PatientState> PatientStates { get; set; }

        public DbSet<Region> Regions { get; set; }

        public DbSet<SanitaryMeasure> SanitaryMeasures { get; set; }

        public DbSet<Patient> Patients { get; set; }



    }
}