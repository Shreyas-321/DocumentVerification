using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webApitest.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OriginalAadhaarData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AadhaarName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DOB = table.Column<DateTime>(type: "date", nullable: false),
                    AadhaarNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OriginalAadhaarData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OriginalECData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SurveyNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MeasuringArea = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Village = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Hobli = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Taluk = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                    Checked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OriginalECData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OriginalPANData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PANName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PANNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DOB = table.Column<DateTime>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OriginalPANData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Pincode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserActivityLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Activity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RelatedEntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RelatedEntityId = table.Column<int>(type: "int", nullable: true),
                    ActionResult = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AdditionalData = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserActivityLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserUploadedDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ECPath = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    AadhaarPath = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    PANPath = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserUploadedDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserUploadedDocuments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExtractedData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UploadId = table.Column<int>(type: "int", nullable: false),
                    AadhaarName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AadhaarNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DOB = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PANName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PANNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ApplicationNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ApplicantName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ApplicantAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SurveyNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MeasuringArea = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Village = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Hobli = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Taluk = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtractedData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtractedData_UserUploadedDocuments_UploadId",
                        column: x => x.UploadId,
                        principalTable: "UserUploadedDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VerificationResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UploadId = table.Column<int>(type: "int", nullable: false),
                    AadhaarNameMatch = table.Column<bool>(type: "bit", nullable: true),
                    AadhaarNoMatch = table.Column<bool>(type: "bit", nullable: true),
                    DOBMatch = table.Column<bool>(type: "bit", nullable: true),
                    PANNameMatch = table.Column<bool>(type: "bit", nullable: true),
                    PANNoMatch = table.Column<bool>(type: "bit", nullable: true),
                    ApplicationNumberMatch = table.Column<bool>(type: "bit", nullable: true),
                    ApplicantNameMatch = table.Column<bool>(type: "bit", nullable: true),
                    ApplicantAddressMatch = table.Column<bool>(type: "bit", nullable: true),
                    SurveyNoMatch = table.Column<bool>(type: "bit", nullable: true),
                    MeasuringAreaMatch = table.Column<bool>(type: "bit", nullable: true),
                    VillageMatch = table.Column<bool>(type: "bit", nullable: true),
                    HobliMatch = table.Column<bool>(type: "bit", nullable: true),
                    TalukMatch = table.Column<bool>(type: "bit", nullable: true),
                    DistrictMatch = table.Column<bool>(type: "bit", nullable: true),
                    OverallMatch = table.Column<bool>(type: "bit", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    RiskScore = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 0.00m),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerificationResults_UserUploadedDocuments_UploadId",
                        column: x => x.UploadId,
                        principalTable: "UserUploadedDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "City", "CreatedAt", "Email", "FullName", "PasswordHash", "Phone", "Pincode", "Role", "State", "UpdatedAt" },
                values: new object[] { 1, "System", new DateTime(2025, 9, 28, 14, 13, 48, 409, DateTimeKind.Utc).AddTicks(5392), "admin@contactmanager.com", "System Administrator", "$2a$11$JoAg0VaOk0El8/N.GXgfKuvhGdvgqpLUHuD1Tl3fx61NSb8J.s1Km", "0000000000", "000000", "Admin", "System", null });

            migrationBuilder.CreateIndex(
                name: "IX_ExtractedData_UploadId",
                table: "ExtractedData",
                column: "UploadId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityLogs_UserId",
                table: "UserActivityLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserUploadedDocuments_UserId",
                table: "UserUploadedDocuments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationResults_UploadId",
                table: "VerificationResults",
                column: "UploadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExtractedData");

            migrationBuilder.DropTable(
                name: "OriginalAadhaarData");

            migrationBuilder.DropTable(
                name: "OriginalECData");

            migrationBuilder.DropTable(
                name: "OriginalPANData");

            migrationBuilder.DropTable(
                name: "UserActivityLogs");

            migrationBuilder.DropTable(
                name: "VerificationResults");

            migrationBuilder.DropTable(
                name: "UserUploadedDocuments");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
