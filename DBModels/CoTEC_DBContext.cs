using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CoTEC_Server.DBModels
{
    public partial class CoTEC_DBContext : DbContext
    {
        public CoTEC_DBContext()
        {
        }

        public CoTEC_DBContext(DbContextOptions<CoTEC_DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Contact> Contact { get; set; }
        public virtual DbSet<ContactPathology> ContactPathology { get; set; }
        public virtual DbSet<ContentionMeasure> ContentionMeasure { get; set; }
        public virtual DbSet<ContentionMeasuresChanges> ContentionMeasuresChanges { get; set; }
        public virtual DbSet<Continent> Continent { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Hospital> Hospital { get; set; }
        public virtual DbSet<HospitalWorkers> HospitalWorkers { get; set; }
        public virtual DbSet<Medication> Medication { get; set; }
        public virtual DbSet<Pathology> Pathology { get; set; }
        public virtual DbSet<PathologySymptoms> PathologySymptoms { get; set; }
        public virtual DbSet<Patient> Patient { get; set; }
        public virtual DbSet<PatientContact> PatientContact { get; set; }
        public virtual DbSet<PatientMedication> PatientMedication { get; set; }
        public virtual DbSet<PatientPathology> PatientPathology { get; set; }
        public virtual DbSet<PatientState> PatientState { get; set; }
        public virtual DbSet<Population> Population { get; set; }
        public virtual DbSet<Region> Region { get; set; }
        public virtual DbSet<SanitaryMeasure> SanitaryMeasure { get; set; }
        public virtual DbSet<SanitaryMeasuresChanges> SanitaryMeasuresChanges { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public DbSet<Increment> Increment { get; set; }
        public DbSet<CountryLocation> CountryLocation { get; set; }
        public DbSet<PatientsReport> PatientsReport { get; set; }
        public DbSet<CasesAndDeaths> CasesAndDeaths { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:DefaultSchema", "admin");

            modelBuilder.Entity<Increment>(entity => entity.HasNoKey());

            modelBuilder.Entity<CountryLocation>(entity => entity.HasNoKey());

            modelBuilder.Entity<PatientsReport>(entity => entity.HasNoKey());

            modelBuilder.Entity<CasesAndDeaths>(entity => entity.HasNoKey());

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.HasKey(e => e.Identification)
                    .HasName("PK__Contact__AAA7C1F49EE13F29");

                entity.ToTable("Contact", "healthcare");

                entity.Property(e => e.Identification)
                    .HasColumnName("identification")
                    .HasMaxLength(50);

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnName("address")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Age)
                    .HasColumnName("age")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasColumnName("country")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("firstName")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("lastName")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Nationality)
                    .HasColumnName("nationality")
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('No nationality')");

                entity.Property(e => e.Region)
                    .IsRequired()
                    .HasColumnName("region")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.RegionNavigation)
                    .WithMany(p => p.Contact)
                    .HasForeignKey(d => new { d.Region, d.Country })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Contact__4D94879B");
            });

            modelBuilder.Entity<ContactPathology>(entity =>
            {
                entity.HasKey(e => new { e.ContactId, e.PathologyName })
                    .HasName("PK__Contact___C105FEF9F9FE16D7");

                entity.ToTable("Contact_Pathology", "healthcare");

                entity.Property(e => e.ContactId)
                    .HasColumnName("Contact_Id")
                    .HasMaxLength(50);

                entity.Property(e => e.PathologyName)
                    .HasColumnName("Pathology_name")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.ContactPathology)
                    .HasForeignKey(d => d.ContactId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Contact_P__Conta__6B24EA82");

                entity.HasOne(d => d.PathologyNameNavigation)
                    .WithMany(p => p.ContactPathology)
                    .HasForeignKey(d => d.PathologyName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Contact_P__Patho__6C190EBB");
            });

            modelBuilder.Entity<ContentionMeasure>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("PK__Contenti__737584F7ADB306A4");

                entity.ToTable("Contention_Measure");

                entity.Property(e => e.Name)
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasMaxLength(5000)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('No Description')");
            });

            modelBuilder.Entity<ContentionMeasuresChanges>(entity =>
            {
                entity.HasKey(e => new { e.MeasureName, e.CountryName, e.StartDate })
                    .HasName("PK__Contenti__13203753EFF34C89");

                entity.ToTable("Contention_Measures_Changes", "user_public");

                entity.Property(e => e.MeasureName)
                    .HasColumnName("Measure_Name")
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.CountryName)
                    .HasColumnName("country_Name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate)
                    .HasColumnName("Start_Date")
                    .HasColumnType("date");

                entity.Property(e => e.EndDate)
                    .HasColumnName("End_Date")
                    .HasColumnType("date");

                entity.HasOne(d => d.CountryNameNavigation)
                    .WithMany(p => p.ContentionMeasuresChanges)
                    .HasForeignKey(d => d.CountryName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Contentio__count__35BCFE0A");

                entity.HasOne(d => d.MeasureNameNavigation)
                    .WithMany(p => p.ContentionMeasuresChanges)
                    .HasForeignKey(d => d.MeasureName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Contentio__Measu__36B12243");
            });

            modelBuilder.Entity<Continent>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("PK__Continen__737584F7EE433B23");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("PK__country__737584F74F3365E7");

                entity.ToTable("country");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Continent)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.ContinentNavigation)
                    .WithMany(p => p.Country)
                    .HasForeignKey(d => d.Continent)
                    .HasConstraintName("FK__country__Contine__267ABA7A");
            });

            modelBuilder.Entity<Hospital>(entity =>
            {
                entity.HasKey(e => new { e.Name, e.Region, e.Country })
                    .HasName("PK__Hospital__92F28AB58763A4C0");

                entity.Property(e => e.Name)
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.Region)
                    .HasColumnName("region")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.IcuCapacity).HasColumnName("ICU_Capacity");

                entity.Property(e => e.Manager)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.ManagerNavigation)
                    .WithMany(p => p.Hospital)
                    .HasForeignKey(d => d.Manager)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Hospital__Manage__534D60F1");

                entity.HasOne(d => d.RegionNavigation)
                    .WithMany(p => p.Hospital)
                    .HasForeignKey(d => new { d.Region, d.Country })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Hospital__52593CB8");
            });

            modelBuilder.Entity<HospitalWorkers>(entity =>
            {
                entity.HasKey(e => new { e.IdCode, e.Name, e.Region, e.Country })
                    .HasName("PK__Hospital__E2F6565E4171624E");

                entity.ToTable("Hospital_Workers");

                entity.Property(e => e.IdCode)
                    .HasColumnName("Id_Code")
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.Region)
                    .HasColumnName("region")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCodeNavigation)
                    .WithMany(p => p.HospitalWorkers)
                    .HasForeignKey(d => d.IdCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Hospital___Id_Co__5DCAEF64");

                entity.HasOne(d => d.Hospital)
                    .WithMany(p => p.HospitalWorkers)
                    .HasForeignKey(d => new { d.Name, d.Region, d.Country })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Hospital_Workers__5CD6CB2B");
            });

            modelBuilder.Entity<Medication>(entity =>
            {
                entity.HasKey(e => e.Medicine)
                    .HasName("PK__Medicati__5CCC71DAF6E6E797");

                entity.Property(e => e.Medicine)
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.Pharmacist)
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Unknown')");
            });

            modelBuilder.Entity<Pathology>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("PK__Patholog__737584F7F741FADA");

                entity.Property(e => e.Name)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasMaxLength(5000)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('No Description')");

                entity.Property(e => e.Treatment)
                    .HasMaxLength(5000)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('No Treatment')");
            });

            modelBuilder.Entity<PathologySymptoms>(entity =>
            {
                entity.HasKey(e => new { e.Pathology, e.Symptom })
                    .HasName("PK__Patholog__CFE9B724779EB169");

                entity.ToTable("Pathology_Symptoms");

                entity.Property(e => e.Pathology)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Symptom)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.PathologyNavigation)
                    .WithMany(p => p.PathologySymptoms)
                    .HasForeignKey(d => d.Pathology)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Pathology__Patho__68487DD7");
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.Identification)
                    .HasName("PK__Patient__AAA7C1F4CA35E522");

                entity.ToTable("Patient", "healthcare");

                entity.Property(e => e.Identification)
                    .HasColumnName("identification")
                    .HasMaxLength(50);

                entity.Property(e => e.Age).HasColumnName("age");

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasColumnName("country")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DateEntrance)
                    .HasColumnName("Date_Entrance")
                    .HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("firstName")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Hospitalized).HasColumnName("hospitalized");

                entity.Property(e => e.IntensiveCareUnite).HasColumnName("intensiveCareUnite");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("lastName")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Nationality)
                    .IsRequired()
                    .HasColumnName("nationality")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Region)
                    .HasColumnName("region")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasColumnName("state")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.StateNavigation)
                    .WithMany(p => p.Patient)
                    .HasForeignKey(d => d.State)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Patient__state__59FA5E80");

                entity.HasOne(d => d.RegionNavigation)
                    .WithMany(p => p.Patient)
                    .HasForeignKey(d => new { d.Region, d.Country })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Patient__59063A47");
            });

            modelBuilder.Entity<PatientContact>(entity =>
            {
                entity.HasKey(e => new { e.ContactId, e.PatientId, e.LastVisit })
                    .HasName("PK__Patient___DA4E0F4091713AE2");

                entity.ToTable("Patient_Contact", "healthcare");

                entity.Property(e => e.ContactId)
                    .HasColumnName("Contact_Id")
                    .HasMaxLength(50);

                entity.Property(e => e.PatientId)
                    .HasColumnName("Patient_Id")
                    .HasMaxLength(50);

                entity.Property(e => e.LastVisit)
                    .HasColumnName("last_visit")
                    .HasColumnType("date")
                    .HasDefaultValueSql("(CONVERT([date],getdate()))");

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.PatientContact)
                    .HasForeignKey(d => d.ContactId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Patient_C__Conta__6FE99F9F");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.PatientContact)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Patient_C__Patie__70DDC3D8");
            });

            modelBuilder.Entity<PatientMedication>(entity =>
            {
                entity.HasKey(e => new { e.PatientId, e.Medication })
                    .HasName("PK__Patient___3A269A75712302A3");

                entity.ToTable("Patient_Medication", "healthcare");

                entity.Property(e => e.PatientId)
                    .HasColumnName("Patient_Id")
                    .HasMaxLength(50);

                entity.Property(e => e.Medication)
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.HasOne(d => d.MedicationNavigation)
                    .WithMany(p => p.PatientMedication)
                    .HasForeignKey(d => d.Medication)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Patient_M__Medic__619B8048");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.PatientMedication)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Patient_M__Patie__60A75C0F");
            });

            modelBuilder.Entity<PatientPathology>(entity =>
            {
                entity.HasKey(e => new { e.PathologyName, e.PatientId })
                    .HasName("PK__Patient___16218E9974B70606");

                entity.ToTable("Patient_Pathology", "healthcare");

                entity.Property(e => e.PathologyName)
                    .HasColumnName("Pathology_Name")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.PatientId)
                    .HasColumnName("Patient_Id")
                    .HasMaxLength(50);

                entity.HasOne(d => d.PathologyNameNavigation)
                    .WithMany(p => p.PatientPathology)
                    .HasForeignKey(d => d.PathologyName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Patient_P__Patho__656C112C");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.PatientPathology)
                    .HasForeignKey(d => d.PatientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Patient_P__Patie__6477ECF3");
            });

            modelBuilder.Entity<PatientState>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("PK__Patient___72E12F1A2F4562E6");

                entity.ToTable("Patient_State");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Population>(entity =>
            {
                entity.HasKey(e => new { e.Day, e.CountryName })
                    .HasName("PK__Populati__757140EFFCBE52F6");

                entity.ToTable("Population", "user_public");

                entity.Property(e => e.Day)
                    .HasColumnName("day")
                    .HasColumnType("date");

                entity.Property(e => e.CountryName)
                    .HasColumnName("country_Name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Active).HasComputedColumnSql("(([Infected]-[Cured])-[Dead])");

                entity.HasOne(d => d.CountryNameNavigation)
                    .WithMany(p => p.Population)
                    .HasForeignKey(d => d.CountryName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Populatio__count__300424B4");
            });

            modelBuilder.Entity<Region>(entity =>
            {
                entity.HasKey(e => new { e.Name, e.Country })
                    .HasName("PK__region__11F6C9DDF7CED572");

                entity.ToTable("region");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.CountryNavigation)
                    .WithMany(p => p.Region)
                    .HasForeignKey(d => d.Country)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__region__country__29572725");
            });

            modelBuilder.Entity<SanitaryMeasure>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("PK__Sanitary__737584F72E595DB1");

                entity.ToTable("Sanitary_Measure");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasMaxLength(5000)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('No Description')");
            });

            modelBuilder.Entity<SanitaryMeasuresChanges>(entity =>
            {
                entity.HasKey(e => new { e.MeasureName, e.CountryName, e.StartDate })
                    .HasName("PK__Sanitary__1320375366BBB906");

                entity.ToTable("Sanitary_Measures_Changes", "user_public");

                entity.Property(e => e.MeasureName)
                    .HasColumnName("Measure_Name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CountryName)
                    .HasColumnName("country_Name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate)
                    .HasColumnName("Start_Date")
                    .HasColumnType("date");

                entity.Property(e => e.EndDate)
                    .HasColumnName("End_Date")
                    .HasColumnType("date");

                entity.HasOne(d => d.CountryNameNavigation)
                    .WithMany(p => p.SanitaryMeasuresChanges)
                    .HasForeignKey(d => d.CountryName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Sanitary___count__3C69FB99");

                entity.HasOne(d => d.MeasureNameNavigation)
                    .WithMany(p => p.SanitaryMeasuresChanges)
                    .HasForeignKey(d => d.MeasureName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Sanitary___Measu__3D5E1FD2");
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.HasKey(e => e.IdCode)
                    .HasName("PK__Staff__ABD97EF5F4758948");

                entity.Property(e => e.IdCode)
                    .HasColumnName("Id_Code")
                    .HasMaxLength(50);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("firstName")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("lastName")
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
