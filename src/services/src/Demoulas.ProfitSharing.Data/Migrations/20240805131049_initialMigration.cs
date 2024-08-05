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
                name: "BENEFICIARYTYPE",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BENEFICIARYTYPE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "COUNTRY",
                columns: table => new
                {
                    ISO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false),
                    ID = table.Column<short>(type: "NUMBER(3)", precision: 3, nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    TELEPHONECODE = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COUNTRY", x => x.ISO);
                });

            migrationBuilder.CreateTable(
                name: "DEPARTMENT",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DEPARTMENT", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYEETYPE",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYEETYPE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYMENTSTATUS",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYMENTSTATUS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYMENTTYPE",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYMENTTYPE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ENROLLMENT",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ENROLLMENT", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "GENDER",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GENDER", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "JOBSTARTMETHOD",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(3)", precision: 3, nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JOBSTARTMETHOD", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "JOBSTATUS",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(3)", precision: 3, nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JOBSTATUS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "JOBTYPE",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(3)", precision: 3, nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JOBTYPE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PAYCLASSIFICATION",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(2)", precision: 2, nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false, comment: "Pay Classification")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAYCLASSIFICATION", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PAYFREQUENCY",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAYFREQUENCY", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PROFITCODE",
                columns: table => new
                {
                    CODE = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    DEFINITION = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    FREQUENCY = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROFITCODE", x => x.CODE);
                });

            migrationBuilder.CreateTable(
                name: "TAXCODE",
                columns: table => new
                {
                    CODE = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    DESCRIPTION = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TAXCODE", x => x.CODE);
                });

            migrationBuilder.CreateTable(
                name: "TERMINATIONCODE",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TERMINATIONCODE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ZEROCONTRIBUTIONREASON",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZEROCONTRIBUTIONREASON", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BENEFICIARY",
                columns: table => new
                {
                    PSN = table.Column<long>(type: "NUMBER(9)", precision: 9, nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    SSN = table.Column<long>(type: "NUMBER(9)", precision: 9, nullable: false),
                    FIRSTNAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    MIDDLENAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    LASTNAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    DATEOFBIRTH = table.Column<DateTime>(type: "DATE", nullable: false),
                    STREET = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false, comment: "Street"),
                    STREET2 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true, comment: "Street2"),
                    STREET3 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true, comment: "Street3"),
                    STREET4 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true, comment: "Street4"),
                    CITY = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: false, comment: "City"),
                    STATE = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: false, comment: "State"),
                    POSTAL_CODE = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: false, comment: "Postal Code"),
                    COUNTRY_ISO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false, defaultValue: "US"),
                    PHONE_NUMBER = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: true),
                    MOBILE_NUMBER = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: true),
                    EMAIL_ADDRESS = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    TYPEID = table.Column<byte>(type: "NUMBER(3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BENEFICIARY", x => x.PSN);
                    table.ForeignKey(
                        name: "FK_BENEFICIARY_BENEFICIARYTYPE_TYPEID",
                        column: x => x.TYPEID,
                        principalTable: "BENEFICIARYTYPE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BENEFICIARY_COUNTRY_COUNTRY_ISO",
                        column: x => x.COUNTRY_ISO,
                        principalTable: "COUNTRY",
                        principalColumn: "ISO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JOB",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    JOBTYPEID = table.Column<byte>(type: "NUMBER(3)", precision: 3, nullable: false),
                    STARTMETHODID = table.Column<byte>(type: "NUMBER(3)", precision: 3, nullable: false),
                    REQUESTEDBY = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    JOBSTATUSID = table.Column<byte>(type: "NUMBER(3)", precision: 3, nullable: false),
                    STARTED = table.Column<DateTime>(type: "DATE", nullable: false),
                    COMPLETED = table.Column<DateTime>(type: "DATE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JOB", x => x.ID);
                    table.ForeignKey(
                        name: "FK_JOB_JOBSTARTMETHOD_STARTMETHODID",
                        column: x => x.STARTMETHODID,
                        principalTable: "JOBSTARTMETHOD",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JOB_JOBSTATUS_JOBSTATUSID",
                        column: x => x.JOBSTATUSID,
                        principalTable: "JOBSTATUS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JOB_JOBTYPE_JOBTYPEID",
                        column: x => x.JOBTYPEID,
                        principalTable: "JOBTYPE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PROFITDETAIL",
                columns: table => new
                {
                    PROFITYEAR = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    PROFITYEARITERATION = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    PROFITCLIENT = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    PROFITCODEID = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    CONTRIBUTION = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    EARNINGS = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    FORFEITURE = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    MONTH = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    YEAR = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    COMMENT = table.Column<string>(type: "NVARCHAR2(16)", maxLength: 16, nullable: false),
                    ZEROCONT = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    FEDERALTAXES = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    STATETAXES = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    TAXCODEID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    SSN = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    PROFDISTID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROFITDETAIL", x => new { x.PROFITYEAR, x.PROFITYEARITERATION });
                    table.ForeignKey(
                        name: "FK_PROFITDETAIL_PROFITCODE_PROFITCODEID",
                        column: x => x.PROFITCODEID,
                        principalTable: "PROFITCODE",
                        principalColumn: "CODE",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PROFITDETAIL_TAXCODE_TAXCODEID",
                        column: x => x.TAXCODEID,
                        principalTable: "TAXCODE",
                        principalColumn: "CODE",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DEMOGRAPHICS",
                columns: table => new
                {
                    ORACLEHCMID = table.Column<long>(type: "NUMBER(15)", precision: 15, nullable: false),
                    SSN = table.Column<long>(type: "NUMBER(9)", precision: 9, nullable: false),
                    BADGENUMBER = table.Column<int>(type: "NUMBER(7)", precision: 7, nullable: false),
                    LASTMODIFIEDDATE = table.Column<DateTime>(type: "DATE", nullable: false, defaultValue: new DateTime(2024, 8, 5, 9, 10, 48, 780, DateTimeKind.Local).AddTicks(1739)),
                    FULLNAME = table.Column<string>(type: "NVARCHAR2(60)", maxLength: 60, nullable: false, comment: "FullName"),
                    LASTNAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false, comment: "LastName"),
                    FIRSTNAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false, comment: "FirstName"),
                    MIDDLENAME = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: true, comment: "MiddleName"),
                    STORENUMBER = table.Column<short>(type: "NUMBER(3)", precision: 3, nullable: false, comment: "StoreNumber"),
                    PAYCLASSIFICATIONID = table.Column<byte>(type: "NUMBER(2)", precision: 2, nullable: false, comment: "PayClassification"),
                    PHONE_NUMBER = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: true),
                    MOBILE_NUMBER = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: true),
                    EMAIL_ADDRESS = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    STREET = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false, comment: "Street"),
                    STREET2 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true, comment: "Street2"),
                    STREET3 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true, comment: "Street3"),
                    STREET4 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true, comment: "Street4"),
                    CITY = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: false, comment: "City"),
                    STATE = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: false, comment: "State"),
                    POSTAL_CODE = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: false, comment: "Postal Code"),
                    COUNTRY_ISO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false, defaultValue: "US"),
                    DATEOFBIRTH = table.Column<DateTime>(type: "DATE", nullable: false, comment: "DateOfBirth"),
                    FULLTIMEDATE = table.Column<DateTime>(type: "DATE", nullable: true, comment: "FullTimeDate"),
                    HIREDATE = table.Column<DateTime>(type: "DATE", nullable: false, comment: "HireDate"),
                    REHIREDATE = table.Column<DateTime>(type: "DATE", nullable: true, comment: "ReHireDate"),
                    TERMINATIONDATE = table.Column<DateTime>(type: "DATE", nullable: true, comment: "TerminationDate"),
                    DEPARTMENTID = table.Column<byte>(type: "NUMBER(1)", precision: 1, nullable: false, comment: "Department"),
                    EMPLOYMENTTYPEID = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false, comment: "EmploymentType"),
                    GENDERID = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false, comment: "Gender"),
                    PAYFREQUENCYID = table.Column<byte>(type: "NUMBER(3)", maxLength: 1, nullable: false, comment: "PayFrequency"),
                    TERMINATIONCODEID = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: true, comment: "TerminationCode"),
                    EMPLOYMENTSTATUSID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DEMOGRAPHICS", x => x.ORACLEHCMID);
                    table.ForeignKey(
                        name: "FK_DEMOGRAPHICS_COUNTRY_COUNTRY_ISO",
                        column: x => x.COUNTRY_ISO,
                        principalTable: "COUNTRY",
                        principalColumn: "ISO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DEMOGRAPHICS_DEPARTMENT_DEPARTMENTID",
                        column: x => x.DEPARTMENTID,
                        principalTable: "DEPARTMENT",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DEMOGRAPHICS_EMPLOYMENTSTATUS_EMPLOYMENTSTATUSID",
                        column: x => x.EMPLOYMENTSTATUSID,
                        principalTable: "EMPLOYMENTSTATUS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DEMOGRAPHICS_EMPLOYMENTTYPE_EMPLOYMENTTYPEID",
                        column: x => x.EMPLOYMENTTYPEID,
                        principalTable: "EMPLOYMENTTYPE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DEMOGRAPHICS_GENDER_GENDERID",
                        column: x => x.GENDERID,
                        principalTable: "GENDER",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DEMOGRAPHICS_PAYCLASSIFICATION_PAYCLASSIFICATIONID",
                        column: x => x.PAYCLASSIFICATIONID,
                        principalTable: "PAYCLASSIFICATION",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DEMOGRAPHICS_PAYFREQUENCY_PAYFREQUENCYID",
                        column: x => x.PAYFREQUENCYID,
                        principalTable: "PAYFREQUENCY",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DEMOGRAPHICS_TERMINATIONCODE_TERMINATIONCODEID",
                        column: x => x.TERMINATIONCODEID,
                        principalTable: "TERMINATIONCODE",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PAYPROFIT",
                columns: table => new
                {
                    BADGENUMBER = table.Column<int>(type: "NUMBER(7)", precision: 7, nullable: false),
                    SSN = table.Column<long>(type: "NUMBER(19)", maxLength: 9, nullable: false),
                    HOURSCURRENTYEAR = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: false),
                    HOURSLASTYEAR = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: false),
                    EARNINGSCURRENTYEAR = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    EARNINGSLASTYEAR = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    EARNINGSAFTERAPPLYINGVESTINGRULES = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    EARNINGSETVAVALUE = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    SECONDARYEARNINGS = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: true),
                    SECONDARYETVAEARNINGS = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: true),
                    EARNINGSPRIORETVAVALUE = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: true),
                    WEEKSWORKEDYEAR = table.Column<byte>(type: "NUMBER(2)", precision: 2, nullable: false),
                    WEEKSWORKEDLASTYEAR = table.Column<byte>(type: "NUMBER(2)", precision: 2, nullable: false),
                    COMPANYCONTRIBUTIONYEARS = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    PSCERTIFICATEISSUEDDATE = table.Column<string>(type: "NVARCHAR2(10)", nullable: true),
                    INITIALCONTRIBUTIONYEAR = table.Column<short>(type: "NUMBER(4)", precision: 4, nullable: false),
                    NETBALANCELASTYEAR = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    NUMBEROFDOLLARSEARNINGLASTYEAR = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    POINTSEARNEDLASTYEAR = table.Column<int>(type: "NUMBER(5)", precision: 5, nullable: false),
                    VESTEDBALANCELASTYEAR = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    CONTRIBUTIONAMOUNTLASTYEAR = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    FORFEITUREAMOUNTLASTYEAR = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    ENROLLMENTID = table.Column<byte>(type: "NUMBER(3)", nullable: true),
                    BENEFICIARYTYPEID = table.Column<byte>(type: "NUMBER(3)", nullable: true),
                    EMPLOYEETYPEID = table.Column<byte>(type: "NUMBER(3)", nullable: true),
                    ZEROCONTRIBUTIONREASONID = table.Column<byte>(type: "NUMBER(3)", nullable: true),
                    EXECUTIVEHOURS = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: false),
                    EXECUTIVEEARNINGS = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAYPROFIT", x => x.BADGENUMBER);
                    table.ForeignKey(
                        name: "FK_PAYPROFIT_BENEFICIARYTYPE_BENEFICIARYTYPEID",
                        column: x => x.BENEFICIARYTYPEID,
                        principalTable: "BENEFICIARYTYPE",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_PAYPROFIT_EMPLOYEETYPE_EMPLOYEETYPEID",
                        column: x => x.EMPLOYEETYPEID,
                        principalTable: "EMPLOYEETYPE",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_PAYPROFIT_ENROLLMENT_ENROLLMENTID",
                        column: x => x.ENROLLMENTID,
                        principalTable: "ENROLLMENT",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_PAYPROFIT_ZEROCONTRIBUTIONREASON_ZEROCONTRIBUTIONREASONID",
                        column: x => x.ZEROCONTRIBUTIONREASONID,
                        principalTable: "ZEROCONTRIBUTIONREASON",
                        principalColumn: "ID");
                });

            migrationBuilder.InsertData(
                table: "BENEFICIARYTYPE",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { (byte)0, "Employee" },
                    { (byte)1, "Beneficiary" }
                });

            migrationBuilder.InsertData(
                table: "COUNTRY",
                columns: new[] { "ISO", "ID", "NAME", "TELEPHONECODE" },
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
                table: "DEPARTMENT",
                columns: new[] { "ID", "NAME" },
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
                table: "EMPLOYEETYPE",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { (byte)0, "NOT New in plan last year" },
                    { (byte)1, "New last year" }
                });

            migrationBuilder.InsertData(
                table: "EMPLOYMENTSTATUS",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { "a", "Active" },
                    { "d", "Delete" },
                    { "i", "Inactive" },
                    { "t", "Terminated" }
                });

            migrationBuilder.InsertData(
                table: "EMPLOYMENTTYPE",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { "F", "FullTimeEightPaidHolidays" },
                    { "G", "FullTimeAccruedPaidHolidays" },
                    { "H", "FullTimeStraightSalary" },
                    { "P", "PartTime" }
                });

            migrationBuilder.InsertData(
                table: "ENROLLMENT",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { (byte)0, "Not Enrolled" },
                    { (byte)1, "Old vesting plan has Contributions (7 years to full vesting)" },
                    { (byte)2, "New vesting plan has Contributions (6 years to full vesting)" },
                    { (byte)3, "Old vesting plan has Forfeiture records" },
                    { (byte)4, "New vesting plan has Forfeiture records" }
                });

            migrationBuilder.InsertData(
                table: "GENDER",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { "F", "Female" },
                    { "M", "Male" },
                    { "X", "Other" }
                });

            migrationBuilder.InsertData(
                table: "JOBSTARTMETHOD",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { (byte)0, "System" },
                    { (byte)1, "OnDemand" }
                });

            migrationBuilder.InsertData(
                table: "JOBSTATUS",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { (byte)0, "Pending" },
                    { (byte)1, "Running" },
                    { (byte)2, "Completed" },
                    { (byte)99, "Failed" }
                });

            migrationBuilder.InsertData(
                table: "JOBTYPE",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { (byte)0, "Full" },
                    { (byte)1, "Delta" },
                    { (byte)2, "Individual" }
                });

            migrationBuilder.InsertData(
                table: "PAYCLASSIFICATION",
                columns: new[] { "ID", "NAME" },
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
                table: "PAYFREQUENCY",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { (byte)1, "Weekly" },
                    { (byte)2, "Monthly" }
                });

            migrationBuilder.InsertData(
                table: "PROFITCODE",
                columns: new[] { "CODE", "DEFINITION", "FREQUENCY" },
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
                table: "TERMINATIONCODE",
                columns: new[] { "ID", "NAME" },
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
                table: "ZEROCONTRIBUTIONREASON",
                columns: new[] { "ID", "NAME" },
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
                name: "IX_BENEFICIARY_COUNTRY_ISO",
                table: "BENEFICIARY",
                column: "COUNTRY_ISO");

            migrationBuilder.CreateIndex(
                name: "IX_BENEFICIARY_SSN",
                table: "BENEFICIARY",
                column: "SSN");

            migrationBuilder.CreateIndex(
                name: "IX_BENEFICIARY_TYPEID",
                table: "BENEFICIARY",
                column: "TYPEID");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHICS_COUNTRY_ISO",
                table: "DEMOGRAPHICS",
                column: "COUNTRY_ISO");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHICS_DEPARTMENTID",
                table: "DEMOGRAPHICS",
                column: "DEPARTMENTID");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHICS_EMPLOYMENTSTATUSID",
                table: "DEMOGRAPHICS",
                column: "EMPLOYMENTSTATUSID");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHICS_EMPLOYMENTTYPEID",
                table: "DEMOGRAPHICS",
                column: "EMPLOYMENTTYPEID");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHICS_GENDERID",
                table: "DEMOGRAPHICS",
                column: "GENDERID");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHICS_PAYCLASSIFICATIONID",
                table: "DEMOGRAPHICS",
                column: "PAYCLASSIFICATIONID");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHICS_PAYFREQUENCYID",
                table: "DEMOGRAPHICS",
                column: "PAYFREQUENCYID");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHICS_SSN",
                table: "DEMOGRAPHICS",
                column: "SSN");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHICS_TERMINATIONCODEID",
                table: "DEMOGRAPHICS",
                column: "TERMINATIONCODEID");

            migrationBuilder.CreateIndex(
                name: "IX_JOB_JOBSTATUSID",
                table: "JOB",
                column: "JOBSTATUSID");

            migrationBuilder.CreateIndex(
                name: "IX_JOB_JOBTYPEID",
                table: "JOB",
                column: "JOBTYPEID");

            migrationBuilder.CreateIndex(
                name: "IX_JOB_STARTMETHODID",
                table: "JOB",
                column: "STARTMETHODID");

            migrationBuilder.CreateIndex(
                name: "IX_PAYPROFIT_BENEFICIARYTYPEID",
                table: "PAYPROFIT",
                column: "BENEFICIARYTYPEID");

            migrationBuilder.CreateIndex(
                name: "IX_PAYPROFIT_EMPLOYEETYPEID",
                table: "PAYPROFIT",
                column: "EMPLOYEETYPEID");

            migrationBuilder.CreateIndex(
                name: "IX_PAYPROFIT_ENROLLMENTID",
                table: "PAYPROFIT",
                column: "ENROLLMENTID");

            migrationBuilder.CreateIndex(
                name: "IX_PAYPROFIT_ZEROCONTRIBUTIONREASONID",
                table: "PAYPROFIT",
                column: "ZEROCONTRIBUTIONREASONID");

            migrationBuilder.CreateIndex(
                name: "IX_PROFITDETAIL_PROFITCODEID",
                table: "PROFITDETAIL",
                column: "PROFITCODEID");

            migrationBuilder.CreateIndex(
                name: "IX_PROFITDETAIL_TAXCODEID",
                table: "PROFITDETAIL",
                column: "TAXCODEID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BENEFICIARY");

            migrationBuilder.DropTable(
                name: "DEMOGRAPHICS");

            migrationBuilder.DropTable(
                name: "JOB");

            migrationBuilder.DropTable(
                name: "PAYPROFIT");

            migrationBuilder.DropTable(
                name: "PROFITDETAIL");

            migrationBuilder.DropTable(
                name: "COUNTRY");

            migrationBuilder.DropTable(
                name: "DEPARTMENT");

            migrationBuilder.DropTable(
                name: "EMPLOYMENTSTATUS");

            migrationBuilder.DropTable(
                name: "EMPLOYMENTTYPE");

            migrationBuilder.DropTable(
                name: "GENDER");

            migrationBuilder.DropTable(
                name: "PAYCLASSIFICATION");

            migrationBuilder.DropTable(
                name: "PAYFREQUENCY");

            migrationBuilder.DropTable(
                name: "TERMINATIONCODE");

            migrationBuilder.DropTable(
                name: "JOBSTARTMETHOD");

            migrationBuilder.DropTable(
                name: "JOBSTATUS");

            migrationBuilder.DropTable(
                name: "JOBTYPE");

            migrationBuilder.DropTable(
                name: "BENEFICIARYTYPE");

            migrationBuilder.DropTable(
                name: "EMPLOYEETYPE");

            migrationBuilder.DropTable(
                name: "ENROLLMENT");

            migrationBuilder.DropTable(
                name: "ZEROCONTRIBUTIONREASON");

            migrationBuilder.DropTable(
                name: "PROFITCODE");

            migrationBuilder.DropTable(
                name: "TAXCODE");
        }
    }
}
