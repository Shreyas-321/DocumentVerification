using Microsoft.EntityFrameworkCore;
using VerificationDLL.Models;

namespace VerificationDLL
{
    public class VerificationDbContext : DbContext
    {
        public VerificationDbContext(DbContextOptions<VerificationDbContext> options) : base(options)
        {
        }

        public DbSet<Models.ExtractedData> ExtractedData { get; set; }
        public DbSet<Models.OriginalAadhaarData> OriginalAadhaarData { get; set; }
        public DbSet<Models.OriginalPANData> OriginalPANData { get; set; }
        public DbSet<Models.OriginalECData> OriginalECData { get; set; }
        public DbSet<Models.VerificationResults> VerificationResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure ExtractedData
            modelBuilder.Entity<Models.ExtractedData>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.ApplicationNumber).HasMaxLength(100);
                entity.Property(e => e.ApplicantName).HasMaxLength(150);
                entity.Property(e => e.ApplicantAddress).HasMaxLength(500);
                entity.Property(e => e.SurveyNo).HasMaxLength(50);
                entity.Property(e => e.MeasuringArea).HasMaxLength(50);
                entity.Property(e => e.Village).HasMaxLength(100);
                entity.Property(e => e.Hobli).HasMaxLength(100);
                entity.Property(e => e.Taluk).HasMaxLength(100);
                entity.Property(e => e.District).HasMaxLength(100);
                entity.Property(e => e.AadhaarName).HasMaxLength(150);
                entity.Property(e => e.AadhaarNo).HasMaxLength(50);
                entity.Property(e => e.DOB).HasMaxLength(100);
                entity.Property(e => e.PANName).HasMaxLength(150);
                entity.Property(e => e.PANNo).HasMaxLength(20);
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");
            });

            // Configure OriginalAadhaarData
            modelBuilder.Entity<Models.OriginalAadhaarData>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.AadhaarName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.DOB).IsRequired().HasMaxLength(50);
                entity.Property(e => e.AadhaarNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");
            });

            // Configure OriginalPANData
            modelBuilder.Entity<Models.OriginalPANData>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.PANName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.PANNo).IsRequired().HasMaxLength(20);
                entity.Property(e => e.DOB).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");
            });

            // Configure OriginalECData
            modelBuilder.Entity<Models.OriginalECData>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.SurveyNo).HasMaxLength(50);
                entity.Property(e => e.MeasuringArea).HasMaxLength(50);
                entity.Property(e => e.Village).HasMaxLength(100);
                entity.Property(e => e.Hobli).HasMaxLength(100);
                entity.Property(e => e.Taluk).HasMaxLength(100);
                entity.Property(e => e.District).HasMaxLength(100);
                //entity.Property(e => e.Latitude).HasColumnType("decimal(9,6)");
                //entity.Property(e => e.Longitude).HasColumnType("decimal(9,6)");
                entity.Property(e => e.Checked).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");
            });

            // Configure VerificationResults
            modelBuilder.Entity<Models.VerificationResults>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50).HasDefaultValue("Pending");
                entity.Property(e => e.RiskScore).HasColumnType("decimal(5,2)").HasDefaultValue(0.00m);
                entity.Property(e => e.VerifiedAt).IsRequired().HasDefaultValueSql("GETDATE()");
            });
        }
    }
}