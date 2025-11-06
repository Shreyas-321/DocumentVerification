using Microsoft.EntityFrameworkCore;
using webApitest.Models;

namespace webApitest.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
       
        public DbSet<GeoTable> GeoTable { get; set; } 
        public DbSet<UserUploadedDocuments> UserUploadedDocuments { get; set; }
        public DbSet<ExtractedData> ExtractedData { get; set; }
        public DbSet<OriginalAadhaarData> OriginalAadhaarData { get; set; }
        public DbSet<OriginalECData> OriginalECData { get; set; }
        public DbSet<OriginalPANData> OriginalPANData { get; set; }
        public DbSet<VerificationResults> VerificationResults { get; set; }
        public DbSet<UserActivityLog> UserActivityLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity to match your SQL schema
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100);
                    
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);
                    
                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(255);
                    
                entity.Property(e => e.Phone)
                    .HasMaxLength(20);
                    
                entity.Property(e => e.City)
                    .HasMaxLength(100);
                    
                entity.Property(e => e.State)
                    .HasMaxLength(100);
                    
                entity.Property(e => e.Pincode)
                    .HasMaxLength(20);
                    
                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(50);
                    
                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");
                    
                entity.Property(e => e.UpdatedAt);

                // Create unique index on email
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure UserUploadedDocuments entity to match your SQL schema
            modelBuilder.Entity<UserUploadedDocuments>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.UserId).IsRequired();
                
                entity.Property(e => e.ECPath)
                    .HasColumnType("NVARCHAR(MAX)");
                    
                entity.Property(e => e.AadhaarPath)
                    .HasColumnType("NVARCHAR(MAX)");
                    
                entity.Property(e => e.PANPath)
                    .HasColumnType("NVARCHAR(MAX)");
                    
                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");

                // Foreign key relationship
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ExtractedData entity to match your SQL schema
            modelBuilder.Entity<ExtractedData>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.UploadId).IsRequired();
                
                entity.Property(e => e.AadhaarName)
                    .HasMaxLength(255);
                    
                entity.Property(e => e.AadhaarNo)
                    .HasMaxLength(20);
                    
                entity.Property(e => e.DOB)
                    .HasMaxLength(100);
                    
                entity.Property(e => e.PANName)
                    .HasMaxLength(255);
                    
                entity.Property(e => e.PANNo)
                    .HasMaxLength(20);
                    
                entity.Property(e => e.ApplicationNumber)
                    .HasMaxLength(50);
                    
                entity.Property(e => e.ApplicantName)
                    .HasMaxLength(255);
                    
                entity.Property(e => e.ApplicantAddress)
                    .HasMaxLength(500);
                    
                entity.Property(e => e.SurveyNo)
                    .HasMaxLength(50);
                    
                entity.Property(e => e.MeasuringArea)
                    .HasMaxLength(50);
                    
                entity.Property(e => e.Village)
                    .HasMaxLength(100);
                    
                entity.Property(e => e.Hobli)
                    .HasMaxLength(100);
                    
                entity.Property(e => e.Taluk)
                    .HasMaxLength(100);
                    
                entity.Property(e => e.District)
                    .HasMaxLength(100);
                    
                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");

                // Foreign key relationship
                entity.HasOne(e => e.UserUploadedDocuments)
                      .WithMany()
                      .HasForeignKey(e => e.UploadId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure OriginalAadhaarData entity
            modelBuilder.Entity<OriginalAadhaarData>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.AadhaarName)
                    .IsRequired()
                    .HasMaxLength(150);
                    
                entity.Property(e => e.DOB)
                    .IsRequired()
                    .HasColumnType("date");
                    
                entity.Property(e => e.AadhaarNo)
                    .IsRequired()
                    .HasMaxLength(50);
                    
                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");
            });

            // Configure OriginalECData entity
            modelBuilder.Entity<OriginalECData>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.SurveyNo)
                    .HasMaxLength(50);
                    
                entity.Property(e => e.MeasuringArea)
                    .HasMaxLength(50);
                    
                entity.Property(e => e.Village)
                    .HasMaxLength(100);
                    
                entity.Property(e => e.Hobli)
                    .HasMaxLength(100);
                    
                entity.Property(e => e.Taluk)
                    .HasMaxLength(100);
                    
                entity.Property(e => e.District)
                    .HasMaxLength(100);
                    
                entity.Property(e => e.Latitude)
                    .HasColumnType("decimal(9,6)");
                    
                entity.Property(e => e.Longitude)
                    .HasColumnType("decimal(9,6)");
                    
                entity.Property(e => e.Checked)
                    .HasDefaultValue(false);
                    
                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");
            });

            // Configure OriginalPANData entity
            modelBuilder.Entity<OriginalPANData>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.PANName)
                    .IsRequired()
                    .HasMaxLength(150);
                    
                entity.Property(e => e.PANNo)
                    .IsRequired()
                    .HasMaxLength(20);
                    
                entity.Property(e => e.DOB)
                    .HasColumnType("date");
                    
                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");
            });

            // Configure VerificationResults entity
            modelBuilder.Entity<VerificationResults>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.UploadId).IsRequired();
                
                // Match columns (nullable BIT)
                entity.Property(e => e.AadhaarNameMatch)
                    .HasColumnType("bit");
                    
                entity.Property(e => e.AadhaarNoMatch)
                    .HasColumnType("bit");
                    
                entity.Property(e => e.DOBMatch)
                    .HasColumnType("bit");
                    
                entity.Property(e => e.PANNameMatch)
                    .HasColumnType("bit");
                    
                entity.Property(e => e.PANNoMatch)
                    .HasColumnType("bit");
                    
                entity.Property(e => e.ApplicationNumberMatch)
                    .HasColumnType("bit");
                    
                entity.Property(e => e.ApplicantNameMatch)
                    .HasColumnType("bit");
                    
                entity.Property(e => e.ApplicantAddressMatch)
                    .HasColumnType("bit");
                    
                entity.Property(e => e.SurveyNoMatch)
                    .HasColumnType("bit");
                    
                entity.Property(e => e.MeasuringAreaMatch)
                    .HasColumnType("bit");
                    
                entity.Property(e => e.VillageMatch)
                    .HasColumnType("bit");
                    
                entity.Property(e => e.HobliMatch)
                    .HasColumnType("bit");
                    
                entity.Property(e => e.TalukMatch)
                    .HasColumnType("bit");
                    
                entity.Property(e => e.DistrictMatch)
                    .HasColumnType("bit");
                    
                entity.Property(e => e.OverallMatch)
                    .HasColumnType("bit");
                    
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasDefaultValue("Pending");
                    
                entity.Property(e => e.RiskScore)
                    .HasColumnType("decimal(5,2)")
                    .HasDefaultValue(0.00m);
                    
                entity.Property(e => e.VerifiedAt)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");

                // Foreign key relationship
                entity.HasOne(e => e.UserUploadedDocuments)
                      .WithMany()
                      .HasForeignKey(e => e.UploadId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure UserActivityLog entity
            modelBuilder.Entity<UserActivityLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.UserId).IsRequired();
                
                entity.Property(e => e.Activity)
                    .IsRequired()
                    .HasMaxLength(100);
                    
                entity.Property(e => e.Description)
                    .HasMaxLength(500);
                    
                entity.Property(e => e.IPAddress)
                    .HasMaxLength(50);
                    
                entity.Property(e => e.UserAgent)
                    .HasMaxLength(200);
                    
                entity.Property(e => e.RelatedEntityType)
                    .HasMaxLength(50);
                    
                entity.Property(e => e.ActionResult)
                    .HasMaxLength(100);
                    
                entity.Property(e => e.AdditionalData)
                    .HasMaxLength(1000);
                    
                entity.Property(e => e.Timestamp)
                    .IsRequired()
                    .HasDefaultValueSql("GETDATE()");

                // Foreign key relationship
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed admin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FullName = "System Administrator",
                    Email = "admin@contactmanager.com",
                    Phone = "0000000000",
                    City = "System",
                    State = "System",
                    Pincode = "000000",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}