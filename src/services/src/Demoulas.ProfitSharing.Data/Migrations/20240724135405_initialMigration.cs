using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class initialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BeneficiaryType",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeneficiaryType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    ISO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false),
                    Id = table.Column<short>(type: "NUMBER(3)", precision: 3, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    TelephoneCode = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.ISO);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeType",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentStatus",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentType",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Enrollment",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gender",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gender", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayClassification",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "NUMBER(2)", precision: 2, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false, comment: "Pay Classification")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayClassification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayFrequency",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayFrequency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfitCode",
                columns: table => new
                {
                    Code = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Definition = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    Frequency = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfitCode", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TaxCode",
                columns: table => new
                {
                    Code = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxCode", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "TerminationCode",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminationCode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ZeroContributionReason",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZeroContributionReason", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Beneficiary",
                columns: table => new
                {
                    PSN = table.Column<long>(type: "NUMBER(9)", precision: 9, nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    SSN = table.Column<long>(type: "NUMBER(9)", precision: 9, nullable: false),
                    FirstName = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    MiddleName = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    LastName = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "DATE", nullable: false),
                    Street = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false, comment: "Street"),
                    Street2 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true, comment: "Street2"),
                    Street3 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true, comment: "Street3"),
                    Street4 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true, comment: "Street4"),
                    City = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: false, comment: "City"),
                    State = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: false, comment: "State"),
                    PostalCode = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false, comment: "Postal Code"),
                    CountryISO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false, defaultValue: "US"),
                    ContactInfo_PhoneNumber = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: false, comment: "PhoneNumber"),
                    MobileNumber = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: true),
                    EmailAddress = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    TypeId = table.Column<byte>(type: "NUMBER(3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiary", x => x.PSN);
                    table.ForeignKey(
                        name: "FK_Beneficiary_BeneficiaryType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "BeneficiaryType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Beneficiary_Country_CountryISO",
                        column: x => x.CountryISO,
                        principalTable: "Country",
                        principalColumn: "ISO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfitDetail",
                columns: table => new
                {
                    ProfitYear = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    ProfitYearIteration = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    ProfitClient = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    ProfitCodeId = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Contribution = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    Earnings = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    Forfeiture = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    Month = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Year = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Comment = table.Column<string>(type: "NVARCHAR2(16)", maxLength: 16, nullable: false),
                    ZeroCont = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    FederalTaxes = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    StateTaxes = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    TaxCodeId = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    SSN = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ProfDistId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfitDetail", x => new { x.ProfitYear, x.ProfitYearIteration });
                    table.ForeignKey(
                        name: "FK_ProfitDetail_ProfitCode_ProfitCodeId",
                        column: x => x.ProfitCodeId,
                        principalTable: "ProfitCode",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfitDetail_TaxCode_TaxCodeId",
                        column: x => x.TaxCodeId,
                        principalTable: "TaxCode",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Demographics",
                columns: table => new
                {
                    OracleHcmId = table.Column<long>(type: "NUMBER(15)", precision: 15, nullable: false),
                    SSN = table.Column<long>(type: "NUMBER(9)", precision: 9, nullable: false),
                    BadgeNumber = table.Column<int>(type: "NUMBER(7)", precision: 7, nullable: false),
                    FullName = table.Column<string>(type: "NVARCHAR2(60)", maxLength: 60, nullable: false, comment: "FullName"),
                    LastName = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false, comment: "LastName"),
                    FirstName = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false, comment: "FirstName"),
                    PY_MNAME = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: true, comment: "MiddleName"),
                    PY_STOR = table.Column<short>(type: "NUMBER(3)", precision: 3, nullable: false, comment: "StoreNumber"),
                    PY_CLA = table.Column<byte>(type: "NUMBER(2)", precision: 2, nullable: false, comment: "PayClassification"),
                    PY_EMP_TELNO = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: false, comment: "PhoneNumber"),
                    MobileNumber = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: true),
                    EmailAddress = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    Street = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false, comment: "Street"),
                    Street2 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true, comment: "Street2"),
                    Street3 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true, comment: "Street3"),
                    Street4 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true, comment: "Street4"),
                    City = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: false, comment: "City"),
                    State = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: false, comment: "State"),
                    PostalCode = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false, comment: "Postal Code"),
                    CountryISO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false, defaultValue: "US"),
                    DateOfBirth = table.Column<DateTime>(type: "DATE", nullable: false, comment: "DateOfBirth"),
                    FullTimeDate = table.Column<DateTime>(type: "DATE", nullable: false, comment: "FullTimeDate"),
                    HireDate = table.Column<DateTime>(type: "DATE", nullable: false, comment: "HireDate"),
                    ReHireDate = table.Column<DateTime>(type: "DATE", nullable: true, comment: "ReHireDate"),
                    TerminationDate = table.Column<DateTime>(type: "DATE", nullable: true, comment: "TerminationDate"),
                    PY_DP = table.Column<byte>(type: "NUMBER(1)", precision: 1, nullable: false, comment: "Department"),
                    PY_FUL = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false, comment: "EmploymentType"),
                    PY_GENDER = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false, comment: "Gender"),
                    PY_FREQ = table.Column<byte>(type: "NUMBER(3)", maxLength: 1, nullable: false, comment: "PayFrequency"),
                    PY_TERM = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: true, comment: "TerminationCode"),
                    EmploymentStatusId = table.Column<string>(type: "NVARCHAR2(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demographics", x => x.OracleHcmId);
                    table.ForeignKey(
                        name: "FK_Demographics_Country_CountryISO",
                        column: x => x.CountryISO,
                        principalTable: "Country",
                        principalColumn: "ISO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Demographics_Department_PY_DP",
                        column: x => x.PY_DP,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Demographics_EmploymentStatus_EmploymentStatusId",
                        column: x => x.EmploymentStatusId,
                        principalTable: "EmploymentStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Demographics_EmploymentType_PY_FUL",
                        column: x => x.PY_FUL,
                        principalTable: "EmploymentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Demographics_Gender_PY_GENDER",
                        column: x => x.PY_GENDER,
                        principalTable: "Gender",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Demographics_PayClassification_PY_CLA",
                        column: x => x.PY_CLA,
                        principalTable: "PayClassification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Demographics_PayFrequency_PY_FREQ",
                        column: x => x.PY_FREQ,
                        principalTable: "PayFrequency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Demographics_TerminationCode_PY_TERM",
                        column: x => x.PY_TERM,
                        principalTable: "TerminationCode",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PayProfit",
                columns: table => new
                {
                    EmployeeBadge = table.Column<int>(type: "NUMBER(7)", precision: 7, nullable: false),
                    EmployeeSSN = table.Column<long>(type: "NUMBER(15)", maxLength: 9, nullable: false),
                    HoursCurrentYear = table.Column<decimal>(type: "DECIMAL(4,2)", precision: 4, scale: 2, nullable: false),
                    HoursLastYear = table.Column<decimal>(type: "DECIMAL(4,2)", precision: 4, scale: 2, nullable: false),
                    EarningsCurrentYear = table.Column<decimal>(type: "DECIMAL(8,2)", precision: 8, scale: 2, nullable: false),
                    EarningsLastYear = table.Column<decimal>(type: "DECIMAL(8,2)", precision: 8, scale: 2, nullable: false),
                    EarningsAfterApplyingVestingRules = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    EarningsEtvaValue = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    SecondaryEarnings = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: true),
                    SecondaryEtvaEarnings = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: true),
                    EarningsPriorEtvaValue = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: true),
                    WeeksWorkedYear = table.Column<byte>(type: "NUMBER(2)", precision: 2, nullable: false),
                    WeeksWorkedLastYear = table.Column<byte>(type: "NUMBER(2)", precision: 2, nullable: false),
                    CompanyContributionYears = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    PSCertificateIssuedDate = table.Column<string>(type: "NVARCHAR2(10)", nullable: true),
                    InitialContributionYear = table.Column<short>(type: "NUMBER(4)", precision: 4, nullable: false),
                    NetBalanceLastYear = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    NumberOfDollarsEarningLastYear = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    PointsEarnedLastYear = table.Column<int>(type: "NUMBER(5)", precision: 5, nullable: false),
                    VestedBalanceLastYear = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    ContributionAmountLastYear = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    ForfeitureAmountLastYear = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    EnrollmentId = table.Column<byte>(type: "NUMBER(3)", nullable: true),
                    BeneficiaryTypeId = table.Column<byte>(type: "NUMBER(3)", nullable: true),
                    EmployeeTypeId = table.Column<byte>(type: "NUMBER(3)", nullable: true),
                    ZeroContributionReasonId = table.Column<byte>(type: "NUMBER(3)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayProfit", x => x.EmployeeBadge);
                    table.ForeignKey(
                        name: "FK_PayProfit_BeneficiaryType_BeneficiaryTypeId",
                        column: x => x.BeneficiaryTypeId,
                        principalTable: "BeneficiaryType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PayProfit_Demographics_EmployeeSSN",
                        column: x => x.EmployeeSSN,
                        principalTable: "Demographics",
                        principalColumn: "OracleHcmId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PayProfit_EmployeeType_EmployeeTypeId",
                        column: x => x.EmployeeTypeId,
                        principalTable: "EmployeeType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PayProfit_Enrollment_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollment",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PayProfit_ZeroContributionReason_ZeroContributionReasonId",
                        column: x => x.ZeroContributionReasonId,
                        principalTable: "ZeroContributionReason",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "BeneficiaryType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { (byte)0, "Employee" },
                    { (byte)1, "Beneficiary" }
                });

            migrationBuilder.InsertData(
                table: "Country",
                columns: new[] { "ISO", "Id", "Name", "TelephoneCode" },
                values: new object[,]
                {
                    { "AD", (short)4, "Andorra", "+376" },
                    { "AE", (short)184, "United Arab Emirates", "+971" },
                    { "AF", (short)1, "Afghanistan", "+93" },
                    { "AG", (short)6, "Antigua and Barbuda", "+1-268" },
                    { "AL", (short)2, "Albania", "+355" },
                    { "AM", (short)8, "Armenia", "+374" },
                    { "AO", (short)5, "Angola", "+244" },
                    { "AR", (short)7, "Argentina", "+54" },
                    { "AT", (short)10, "Austria", "+43" },
                    { "AU", (short)9, "Australia", "+61" },
                    { "AZ", (short)11, "Azerbaijan", "+994" },
                    { "BA", (short)22, "Bosnia and Herzegovina", "+387" },
                    { "BB", (short)15, "Barbados", "+1-246" },
                    { "BD", (short)14, "Bangladesh", "+880" },
                    { "BE", (short)17, "Belgium", "+32" },
                    { "BF", (short)27, "Burkina Faso", "+226" },
                    { "BG", (short)26, "Bulgaria", "+359" },
                    { "BH", (short)13, "Bahrain", "+973" },
                    { "BI", (short)28, "Burundi", "+257" },
                    { "BJ", (short)19, "Benin", "+229" },
                    { "BN", (short)25, "Brunei", "+673" },
                    { "BO", (short)21, "Bolivia", "+591" },
                    { "BR", (short)24, "Brazil", "+55" },
                    { "BS", (short)12, "Bahamas", "+1-242" },
                    { "BT", (short)20, "Bhutan", "+975" },
                    { "BW", (short)23, "Botswana", "+267" },
                    { "BY", (short)16, "Belarus", "+375" },
                    { "BZ", (short)18, "Belize", "+501" },
                    { "CA", (short)32, "Canada", "+1" },
                    { "CD", (short)45, "Democratic Republic of the Congo", "+243" },
                    { "CF", (short)33, "Central African Republic", "+236" },
                    { "CG", (short)39, "Congo (Congo-Brazzaville)", "+242" },
                    { "CH", (short)168, "Switzerland", "+41" },
                    { "CL", (short)35, "Chile", "+56" },
                    { "CM", (short)31, "Cameroon", "+237" },
                    { "CN", (short)36, "China", "+86" },
                    { "CO", (short)37, "Colombia", "+57" },
                    { "CR", (short)40, "Costa Rica", "+506" },
                    { "CU", (short)42, "Cuba", "+53" },
                    { "CV", (short)29, "Cabo Verde", "+238" },
                    { "CY", (short)43, "Cyprus", "+357" },
                    { "CZ", (short)44, "Czechia (Czech Republic)", "+420" },
                    { "DE", (short)64, "Germany", "+49" },
                    { "DJ", (short)47, "Djibouti", "+253" },
                    { "DK", (short)46, "Denmark", "+45" },
                    { "DM", (short)48, "Dominica", "+1-767" },
                    { "DO", (short)49, "Dominican Republic", "+1-809" },
                    { "DZ", (short)3, "Algeria", "+213" },
                    { "EC", (short)50, "Ecuador", "+593" },
                    { "EE", (short)55, "Estonia", "+372" },
                    { "EG", (short)51, "Egypt", "+20" },
                    { "ER", (short)54, "Eritrea", "+291" },
                    { "ES", (short)163, "Spain", "+34" },
                    { "ET", (short)57, "Ethiopia", "+251" },
                    { "FI", (short)59, "Finland", "+358" },
                    { "FJ", (short)58, "Fiji", "+679" },
                    { "FM", (short)110, "Micronesia", "+691" },
                    { "FR", (short)60, "France", "+33" },
                    { "GA", (short)61, "Gabon", "+241" },
                    { "GB", (short)185, "United Kingdom", "+44" },
                    { "GD", (short)67, "Grenada", "+1-473" },
                    { "GE", (short)63, "Georgia", "+995" },
                    { "GH", (short)65, "Ghana", "+233" },
                    { "GM", (short)62, "Gambia", "+220" },
                    { "GN", (short)69, "Guinea", "+224" },
                    { "GQ", (short)53, "Equatorial Guinea", "+240" },
                    { "GR", (short)66, "Greece", "+30" },
                    { "GT", (short)68, "Guatemala", "+502" },
                    { "GW", (short)70, "Guinea-Bissau", "+245" },
                    { "GY", (short)71, "Guyana", "+592" },
                    { "HN", (short)73, "Honduras", "+504" },
                    { "HR", (short)41, "Croatia", "+385" },
                    { "HT", (short)72, "Haiti", "+509" },
                    { "HU", (short)74, "Hungary", "+36" },
                    { "ID", (short)77, "Indonesia", "+62" },
                    { "IE", (short)80, "Ireland", "+353" },
                    { "IL", (short)81, "Israel", "+972" },
                    { "IN", (short)76, "India", "+91" },
                    { "IQ", (short)79, "Iraq", "+964" },
                    { "IR", (short)78, "Iran", "+98" },
                    { "IS", (short)75, "Iceland", "+354" },
                    { "IT", (short)82, "Italy", "+39" },
                    { "JM", (short)83, "Jamaica", "+1-876" },
                    { "JO", (short)85, "Jordan", "+962" },
                    { "JP", (short)84, "Japan", "+81" },
                    { "KE", (short)87, "Kenya", "+254" },
                    { "KG", (short)90, "Kyrgyzstan", "+996" },
                    { "KH", (short)30, "Cambodia", "+855" },
                    { "KI", (short)88, "Kiribati", "+686" },
                    { "KM", (short)38, "Comoros", "+269" },
                    { "KN", (short)144, "Saint Kitts and Nevis", "+1-869" },
                    { "KP", (short)126, "North Korea", "+850" },
                    { "KR", (short)161, "South Korea", "+82" },
                    { "KW", (short)89, "Kuwait", "+965" },
                    { "KZ", (short)86, "Kazakhstan", "+7" },
                    { "LA", (short)91, "Laos", "+856" },
                    { "LB", (short)93, "Lebanon", "+961" },
                    { "LC", (short)145, "Saint Lucia", "+1-758" },
                    { "LI", (short)97, "Liechtenstein", "+423" },
                    { "LK", (short)164, "Sri Lanka", "+94" },
                    { "LR", (short)95, "Liberia", "+231" },
                    { "LS", (short)94, "Lesotho", "+266" },
                    { "LT", (short)98, "Lithuania", "+370" },
                    { "LU", (short)99, "Luxembourg", "+352" },
                    { "LV", (short)92, "Latvia", "+371" },
                    { "LY", (short)96, "Libya", "+218" },
                    { "MA", (short)115, "Morocco", "+212" },
                    { "MC", (short)112, "Monaco", "+377" },
                    { "MD", (short)111, "Moldova", "+373" },
                    { "ME", (short)114, "Montenegro", "+382" },
                    { "MG", (short)100, "Madagascar", "+261" },
                    { "MH", (short)106, "Marshall Islands", "+692" },
                    { "MK", (short)127, "North Macedonia", "+389" },
                    { "ML", (short)104, "Mali", "+223" },
                    { "MM", (short)117, "Myanmar (Burma)", "+95" },
                    { "MN", (short)113, "Mongolia", "+976" },
                    { "MR", (short)107, "Mauritania", "+222" },
                    { "MT", (short)105, "Malta", "+356" },
                    { "MU", (short)108, "Mauritius", "+230" },
                    { "MV", (short)103, "Maldives", "+960" },
                    { "MW", (short)101, "Malawi", "+265" },
                    { "MX", (short)109, "Mexico", "+52" },
                    { "MY", (short)102, "Malaysia", "+60" },
                    { "MZ", (short)116, "Mozambique", "+258" },
                    { "NA", (short)118, "Namibia", "+264" },
                    { "NE", (short)124, "Niger", "+227" },
                    { "NG", (short)125, "Nigeria", "+234" },
                    { "NI", (short)123, "Nicaragua", "+505" },
                    { "NL", (short)121, "Netherlands", "+31" },
                    { "NO", (short)128, "Norway", "+47" },
                    { "NP", (short)120, "Nepal", "+977" },
                    { "NR", (short)119, "Nauru", "+674" },
                    { "NZ", (short)122, "New Zealand", "+64" },
                    { "OM", (short)129, "Oman", "+968" },
                    { "PA", (short)133, "Panama", "+507" },
                    { "PE", (short)136, "Peru", "+51" },
                    { "PG", (short)134, "Papua New Guinea", "+675" },
                    { "PH", (short)137, "Philippines", "+63" },
                    { "PK", (short)130, "Pakistan", "+92" },
                    { "PL", (short)138, "Poland", "+48" },
                    { "PS", (short)132, "Palestine State", "+970" },
                    { "PT", (short)139, "Portugal", "+351" },
                    { "PW", (short)131, "Palau", "+680" },
                    { "PY", (short)135, "Paraguay", "+595" },
                    { "QA", (short)140, "Qatar", "+974" },
                    { "RO", (short)141, "Romania", "+40" },
                    { "RS", (short)152, "Serbia", "+381" },
                    { "RU", (short)142, "Russia", "+7" },
                    { "RW", (short)143, "Rwanda", "+250" },
                    { "SA", (short)150, "Saudi Arabia", "+966" },
                    { "SB", (short)158, "Solomon Islands", "+677" },
                    { "SC", (short)153, "Seychelles", "+248" },
                    { "SD", (short)165, "Sudan", "+249" },
                    { "SE", (short)167, "Sweden", "+46" },
                    { "SG", (short)155, "Singapore", "+65" },
                    { "SI", (short)157, "Slovenia", "+386" },
                    { "SK", (short)156, "Slovakia", "+421" },
                    { "SL", (short)154, "Sierra Leone", "+232" },
                    { "SM", (short)148, "San Marino", "+378" },
                    { "SN", (short)151, "Senegal", "+221" },
                    { "SO", (short)159, "Somalia", "+252" },
                    { "SR", (short)166, "Suriname", "+597" },
                    { "SS", (short)162, "South Sudan", "+211" },
                    { "ST", (short)149, "Sao Tome and Principe", "+239" },
                    { "SV", (short)52, "El Salvador", "+503" },
                    { "SY", (short)169, "Syria", "+963" },
                    { "SZ", (short)56, "Eswatini (Swaziland)", "+268" },
                    { "TD", (short)34, "Chad", "+235" },
                    { "TG", (short)175, "Togo", "+228" },
                    { "TH", (short)173, "Thailand", "+66" },
                    { "TJ", (short)171, "Tajikistan", "+992" },
                    { "TL", (short)174, "Timor-Leste", "+670" },
                    { "TM", (short)180, "Turkmenistan", "+993" },
                    { "TN", (short)178, "Tunisia", "+216" },
                    { "TO", (short)176, "Tonga", "+676" },
                    { "TR", (short)179, "Turkey", "+90" },
                    { "TT", (short)177, "Trinidad and Tobago", "+1-868" },
                    { "TV", (short)181, "Tuvalu", "+688" },
                    { "TW", (short)170, "Taiwan", "+886" },
                    { "TZ", (short)172, "Tanzania", "+255" },
                    { "UA", (short)183, "Ukraine", "+380" },
                    { "UG", (short)182, "Uganda", "+256" },
                    { "US", (short)186, "United States of America", "+1" },
                    { "UY", (short)187, "Uruguay", "+598" },
                    { "UZ", (short)188, "Uzbekistan", "+998" },
                    { "VC", (short)146, "Saint Vincent and the Grenadines", "+1-784" },
                    { "VE", (short)190, "Venezuela", "+58" },
                    { "VN", (short)191, "Vietnam", "+84" },
                    { "VU", (short)189, "Vanuatu", "+678" },
                    { "WS", (short)147, "Samoa", "+685" },
                    { "YE", (short)192, "Yemen", "+967" },
                    { "ZA", (short)160, "South Africa", "+27" },
                    { "ZM", (short)193, "Zambia", "+260" },
                    { "ZW", (short)194, "Zimbabwe", "+263" }
                });

            migrationBuilder.InsertData(
                table: "Department",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { (byte)1, "Grocery" },
                    { (byte)2, "Meat" },
                    { (byte)3, "Produce" },
                    { (byte)4, "Deli" },
                    { (byte)5, "Dairy" },
                    { (byte)6, "Beer and Wine" },
                    { (byte)7, "Bakery" }
                });

            migrationBuilder.InsertData(
                table: "EmployeeType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { (byte)0, "NOT New in plan last year" },
                    { (byte)1, "New last year" }
                });

            migrationBuilder.InsertData(
                table: "EmploymentStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { "a", "Active" },
                    { "d", "Delete" },
                    { "i", "Inactive" },
                    { "t", "Terminated" }
                });

            migrationBuilder.InsertData(
                table: "EmploymentType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { "F", "FullTimeEightPaidHolidays" },
                    { "G", "FullTimeAccruedPaidHolidays" },
                    { "H", "FullTimeStraightSalary" },
                    { "P", "PartTime" }
                });

            migrationBuilder.InsertData(
                table: "Enrollment",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { (byte)0, "Not Enrolled" },
                    { (byte)1, "Old vesting plan has Contributions (7 years to full vesting)" },
                    { (byte)2, "New vesting plan has Contributions (6 years to full vesting)" },
                    { (byte)3, "Old vesting plan has Forfeiture records" },
                    { (byte)4, "New vesting plan has Forfeiture records" }
                });

            migrationBuilder.InsertData(
                table: "Gender",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { "F", "Female" },
                    { "M", "Male" },
                    { "X", "Other" }
                });

            migrationBuilder.InsertData(
                table: "PayClassification",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { (byte)1, "MANAGER" },
                    { (byte)2, "ASSISTANT MANAGER" },
                    { (byte)10, "FRONT END MANAGER" },
                    { (byte)11, "ASSISTANT HEAD CASHIER" },
                    { (byte)13, "CASHIERS - AM" },
                    { (byte)14, "CASHIERS - PM" },
                    { (byte)15, "CASHIERS 14-15" },
                    { (byte)16, "SACKERS - AM" },
                    { (byte)17, "SACKERS - PM" },
                    { (byte)18, "SACKERS 14-15" },
                    { (byte)19, "STORE MAINTENANCE" },
                    { (byte)20, "OFFICE MANAGER" },
                    { (byte)22, "COURTESY BOOTH - AM" },
                    { (byte)23, "COURTESY BOOTH - PM" },
                    { (byte)24, "POS - FULL TIME" },
                    { (byte)25, "CLERK -FULL TIME AP" },
                    { (byte)26, "CLERKS - FULL TIME AR" },
                    { (byte)27, "CLERKS - FULL TIME GROC" },
                    { (byte)28, "CLERKS - FULL TIME PERISHABLES" },
                    { (byte)29, "CLERKS - FULL TIME WAREHOUSE" },
                    { (byte)30, "MERCHANDISER" },
                    { (byte)31, "GROCERY MANAGER" },
                    { (byte)32, "ENDS - PART TIME" },
                    { (byte)33, "FIRST MEAT CUTTER" },
                    { (byte)35, "NOT USED" },
                    { (byte)37, "CAFE PART TIME" },
                    { (byte)38, "RECEIVER" },
                    { (byte)39, "NOT USED" },
                    { (byte)40, "MEAT CUTTERS" },
                    { (byte)41, "APPR MEAT CUTTERS" },
                    { (byte)42, "MEAT CUTTER PART TIME" },
                    { (byte)43, "TRAINEE MEAT CUTTER" },
                    { (byte)44, "PART TIME SUBSHOP" },
                    { (byte)45, "ASST SUB SHOP MANAGER" },
                    { (byte)46, "SERVICE CASE - FULL TIME" },
                    { (byte)47, "WRAPPERS - FULL TIME" },
                    { (byte)48, "WRAPPERS - PART TIME AM" },
                    { (byte)49, "WRAPPERS - PART TIME PM" },
                    { (byte)50, "HEAD CLERK" },
                    { (byte)51, "SUB SHOP MANAGER" },
                    { (byte)52, "CLERKS - FULL TIME AM" },
                    { (byte)53, "CLERKS - PART TIME AM" },
                    { (byte)54, "CLERKS - PART TIME PM" },
                    { (byte)55, "POS - PART TIME" },
                    { (byte)56, "MARKETS KITCHEN - ASST MGR" },
                    { (byte)57, "MARKETS KITCHEN FT" },
                    { (byte)58, "MARKETS KITCHEN PT" },
                    { (byte)59, "KITCHEN MANAGER" },
                    { (byte)60, "NOT USED" },
                    { (byte)61, "PT BAKERY MERCHANDISER" },
                    { (byte)62, "FT CAKE & CREAMS" },
                    { (byte)63, "CAKE & CREAM PT" },
                    { (byte)64, "OVER WORKER PT" },
                    { (byte)65, "BENCH WORKER PT" },
                    { (byte)66, "FORK LIFT OPR (REC) AM" },
                    { (byte)67, "FORK LIFT OPR (REC) PM" },
                    { (byte)68, "FORK LIFT OPR (SHIP) AM" },
                    { (byte)69, "FORK LIFT OPR (SHIP) PM" },
                    { (byte)70, "FORK LIFT OPR (MISC.) AM" },
                    { (byte)71, "FORK LIFT OPR (MISC.) PM" },
                    { (byte)72, "LOADER - AM" },
                    { (byte)73, "LOADER - PM" },
                    { (byte)74, "WHSE MAINTENANCE - AM" },
                    { (byte)75, "WHSE MAINTENANCE - PM" },
                    { (byte)77, "SELECTOR PART TIME - AM" },
                    { (byte)78, "SELECTOR PART TIME - PM" },
                    { (byte)79, "SELECTOR FULL TIME - AM" },
                    { (byte)80, "TEMP FULLTIME" },
                    { (byte)81, "SELECTOR FULL TIME - PM" },
                    { (byte)82, "INSPECTOR" },
                    { (byte)83, "GENERAL WAREHOUSE - AM" },
                    { (byte)84, "GENERAL WAREHOUSE - PM" },
                    { (byte)85, "DRIVER - TRAILER" },
                    { (byte)86, "DRIVER - STRAIGHT" },
                    { (byte)87, "MECHANIC" },
                    { (byte)88, "GARAGE - PM" },
                    { (byte)89, "FACILITY OPERATIONS" },
                    { (byte)90, "COMPUTER OPERATIONS" },
                    { (byte)91, "SIGN SHOP" },
                    { (byte)92, "INVENTORY" },
                    { (byte)93, "PROGRAMMING" },
                    { (byte)94, "HELP DESK" },
                    { (byte)95, "DEFUNCT" },
                    { (byte)96, "TECHNICAL SUPPORT" },
                    { (byte)98, "TRAINING" }
                });

            migrationBuilder.InsertData(
                table: "PayFrequency",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { (byte)1, "Weekly" },
                    { (byte)2, "Monthly" }
                });

            migrationBuilder.InsertData(
                table: "ProfitCode",
                columns: new[] { "Code", "Definition", "Frequency" },
                values: new object[,]
                {
                    { (short)0, "Incoming contributions, forfeitures, earnings", "Year-end only" },
                    { (short)1, "Outgoing payments (not rollovers or direct payments) - Partial withdrawal", "Multiple Times" },
                    { (short)2, "Outgoing forfeitures", "Multiple Times" },
                    { (short)3, "Outgoing direct payments / rollover payments", "Multiple Times" },
                    { (short)5, "Outgoing XFER beneficiary / QDRO allocation (beneficiary payment)", "Once" },
                    { (short)6, "Incoming QDRO beneficiary allocation  (beneficiary receipt)", "Once" },
                    { (short)8, "Incoming \"100% vested\" earnings", "Usually year-end, unless there is special processing – i.e. Class Action settlement.  Earnings are 100% vested." },
                    { (short)9, "Outgoing payment from 100% vesting amount (payment of ETVA funds)", "Multiple Times" }
                });

            migrationBuilder.InsertData(
                table: "TerminationCode",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { "A", "Left On Own" },
                    { "B", "Personal Or Family Reason" },
                    { "C", "Could Not Work Available Hours" },
                    { "D", "Stealing" },
                    { "E", "Not Following Company Policy" },
                    { "F", "FMLA Expired" },
                    { "G", "Terminated Private" },
                    { "H", "Job Abandonment" },
                    { "I", "Health Reasons Non-FMLA" },
                    { "J", "Layoff No Work" },
                    { "K", "School Or Sports" },
                    { "X", "Military" }
                });

            migrationBuilder.InsertData(
                table: "ZeroContributionReason",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { (byte)0, "Normal" },
                    { (byte)1, "18, 19, OR 20 WITH > 1000 HOURS" },
                    { (byte)2, "TERMINATED EMPLOYEE > 1000 HOURS WORKED GETS YEAR VESTED" },
                    { (byte)3, "OVER 64 AND < 1000 HOURS GETS 1 YEAR VESTING (obsolete 11/20)" },
                    { (byte)4, "OVER 64 AND < 1000 HOURS GETS 2 YEARS VESTING (obsolete 11/20)" },
                    { (byte)5, "OVER 64 AND > 1000 HOURS GETS 3 YEARS VESTING (obsolete 11/20)" },
                    { (byte)6, ">=65 AND 1st CONTRIBUTION >= 5 YEARS AGO GETS 100% VESTED" },
                    { (byte)7, "=64 AND 1ST CONTRIBUTION >=5 YEARS AGO GETS 100% VESTED ON THEIR BIRTHDAY" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiary_CountryISO",
                table: "Beneficiary",
                column: "CountryISO");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiary_TypeId",
                table: "Beneficiary",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SSN",
                table: "Beneficiary",
                column: "SSN");

            migrationBuilder.CreateIndex(
                name: "IX_Demographics_CountryISO",
                table: "Demographics",
                column: "CountryISO");

            migrationBuilder.CreateIndex(
                name: "IX_Demographics_EmploymentStatusId",
                table: "Demographics",
                column: "EmploymentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Demographics_PY_CLA",
                table: "Demographics",
                column: "PY_CLA");

            migrationBuilder.CreateIndex(
                name: "IX_Demographics_PY_DP",
                table: "Demographics",
                column: "PY_DP");

            migrationBuilder.CreateIndex(
                name: "IX_Demographics_PY_FREQ",
                table: "Demographics",
                column: "PY_FREQ");

            migrationBuilder.CreateIndex(
                name: "IX_Demographics_PY_FUL",
                table: "Demographics",
                column: "PY_FUL");

            migrationBuilder.CreateIndex(
                name: "IX_Demographics_PY_GENDER",
                table: "Demographics",
                column: "PY_GENDER");

            migrationBuilder.CreateIndex(
                name: "IX_Demographics_PY_TERM",
                table: "Demographics",
                column: "PY_TERM");

            migrationBuilder.CreateIndex(
                name: "IX_SSN1",
                table: "Demographics",
                column: "SSN");

            migrationBuilder.CreateIndex(
                name: "IX_PayProfit_BeneficiaryTypeId",
                table: "PayProfit",
                column: "BeneficiaryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayProfit_EmployeeSSN",
                table: "PayProfit",
                column: "EmployeeSSN");

            migrationBuilder.CreateIndex(
                name: "IX_PayProfit_EmployeeTypeId",
                table: "PayProfit",
                column: "EmployeeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayProfit_EnrollmentId",
                table: "PayProfit",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PayProfit_ZeroContributionReasonId",
                table: "PayProfit",
                column: "ZeroContributionReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfitDetail_ProfitCodeId",
                table: "ProfitDetail",
                column: "ProfitCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfitDetail_TaxCodeId",
                table: "ProfitDetail",
                column: "TaxCodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Beneficiary");

            migrationBuilder.DropTable(
                name: "PayProfit");

            migrationBuilder.DropTable(
                name: "ProfitDetail");

            migrationBuilder.DropTable(
                name: "BeneficiaryType");

            migrationBuilder.DropTable(
                name: "Demographics");

            migrationBuilder.DropTable(
                name: "EmployeeType");

            migrationBuilder.DropTable(
                name: "Enrollment");

            migrationBuilder.DropTable(
                name: "ZeroContributionReason");

            migrationBuilder.DropTable(
                name: "ProfitCode");

            migrationBuilder.DropTable(
                name: "TaxCode");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "EmploymentStatus");

            migrationBuilder.DropTable(
                name: "EmploymentType");

            migrationBuilder.DropTable(
                name: "Gender");

            migrationBuilder.DropTable(
                name: "PayClassification");

            migrationBuilder.DropTable(
                name: "PayFrequency");

            migrationBuilder.DropTable(
                name: "TerminationCode");
        }
    }
}
