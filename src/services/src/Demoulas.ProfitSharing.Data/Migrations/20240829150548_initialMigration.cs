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
                name: "BENEFICIARY_KIND",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BENEFICIARY_KIND", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BENEFICIARY_TYPE",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ID", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CALDAR_RECORD",
                columns: table => new
                {
                    ACC_WKEND_N = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ACC_APWKEND = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ACC_WEEKN = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ACC_PERIOD = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ACC_QUARTER = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ACC_CALPERIOD = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ACC_CLN60_WEEK = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ACC_CLN60_PERIOD = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ACC_CLN61_WEEK = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ACC_CLN61_PERIOD = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ACC_CLN7X_WEEK = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ACC_CLN7X_PERIOD = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ACC_CLN6X_WEEK = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ACC_CLN6X_PERIOD = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ACC_WKEND2_N = table.Column<long>(type: "NUMBER(19)", nullable: true),
                    ACC_ALT_KEY_NUM = table.Column<long>(type: "NUMBER(19)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CALDAR_RECORD", x => x.ACC_WKEND_N);
                });

            migrationBuilder.CreateTable(
                name: "COUNTRY",
                columns: table => new
                {
                    ISO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false),
                    ID = table.Column<short>(type: "NUMBER(3)", precision: 3, nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    TELEPHONE_CODE = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COUNTRY", x => x.ISO);
                });

            migrationBuilder.CreateTable(
                name: "DEMOGRAPHIC_SYNC_AUDIT",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    BADGE_NUMBER = table.Column<int>(type: "NUMBER(7)", precision: 7, nullable: false),
                    INVALID_VALUE = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    USERNAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true),
                    PROPERTY_NAME = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: true),
                    MESSAGE = table.Column<string>(type: "NVARCHAR2(512)", maxLength: 512, nullable: false),
                    CREATED = table.Column<DateTime>(type: "DATE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DEMOGRAPHIC_SYNC_AUDIT", x => x.ID);
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
                name: "DISTRIBUTION_FREQUENCY",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DISTRIBUTION_FREQUENCY", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DISTRIBUTION_STATUS",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DISTRIBUTION_STATUS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYEE_TYPE",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYEE_TYPE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYMENT_STATUS",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYMENT_STATUS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYMENT_TYPE",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYMENT_TYPE", x => x.ID);
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
                name: "PAY_CLASSIFICATION",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(2)", precision: 2, nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false, comment: "Pay Classification")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAY_CLASSIFICATION", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PAY_FREQUENCY",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAY_FREQUENCY", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PROFIT_CODE",
                columns: table => new
                {
                    CODE = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    DEFINITION = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    FREQUENCY = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROFIT_CODE", x => x.CODE);
                });

            migrationBuilder.CreateTable(
                name: "STATE_TAX",
                columns: table => new
                {
                    ABBREVIATION = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false),
                    RATE = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    USER_MODIFIED = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    DATE_MODIFIED = table.Column<DateTime>(type: "DATE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STATE_TAX", x => x.ABBREVIATION);
                });

            migrationBuilder.CreateTable(
                name: "TAX_CODE",
                columns: table => new
                {
                    CODE = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    DESCRIPTION = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TAX_CODE", x => x.CODE);
                });

            migrationBuilder.CreateTable(
                name: "TERMINATION_CODE",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TERMINATION_CODE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ZERO_CONTRIBUTION_REASON",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZERO_CONTRIBUTION_REASON", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BENEFICIARY",
                columns: table => new
                {
                    PSN = table.Column<long>(type: "NUMBER(11)", precision: 11, nullable: false),
                    SSN = table.Column<long>(type: "NUMBER(9)", precision: 9, nullable: false),
                    FIRST_NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    MIDDLE_NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    LAST_NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    DATE_OF_BIRTH = table.Column<DateTime>(type: "DATE", nullable: false),
                    STREET = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false, comment: "Street"),
                    STREET2 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true, comment: "Street2"),
                    STREET3 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true, comment: "Street3"),
                    STREET4 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true, comment: "Street4"),
                    CITY = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: false, comment: "City"),
                    STATE = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: false, comment: "State"),
                    POSTAL_CODE = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: false, comment: "Postal Code"),
                    COUNTRY_ISO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: true, defaultValue: "US"),
                    PHONE_NUMBER = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: true),
                    MOBILE_NUMBER = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: true),
                    EMAIL_ADDRESS = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    KIND_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: true),
                    DISTRIBUTION = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    AMOUNT = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    EARNINGS = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    SECONDARY_EARNINGS = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    PERCENT = table.Column<decimal>(type: "numeric(3,0)", precision: 3, nullable: false),
                    RELATIONSHIP = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BENEFICIARY", x => x.PSN);
                    table.ForeignKey(
                        name: "FK_BENEFICIARY_BENEFICIARYKINDS_KINDID",
                        column: x => x.KIND_ID,
                        principalTable: "BENEFICIARY_KIND",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_BENEFICIARY_COUNTRY_COUNTRY_ISO",
                        column: x => x.COUNTRY_ISO,
                        principalTable: "COUNTRY",
                        principalColumn: "ISO");
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
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_JOB_JOBSTATUS_JOBSTATUSID",
                        column: x => x.JOBSTATUSID,
                        principalTable: "JOBSTATUS",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_JOB_JOBTYPE_JOBTYPEID",
                        column: x => x.JOBTYPEID,
                        principalTable: "JOBTYPE",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "DISTRIBUTION",
                columns: table => new
                {
                    SSN = table.Column<long>(type: "NUMBER(9)", precision: 9, nullable: false),
                    SEQUENCE_NUMBER = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    EMPLOYEE_NAME = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: false),
                    FREQUENCY_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    STATUS_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    PAYEE_SSN = table.Column<long>(type: "NUMBER(9)", precision: 9, nullable: false),
                    PAYEE_NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    STREET = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    STREET2 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    STREET3 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    STREET4 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    CITY = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: true),
                    STATE = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: true),
                    POSTAL_CODE = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    COUNTRY_ISO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: true, defaultValue: "US"),
                    THIRD_PARTY_PAYEEE = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    THIRD_PARTY_NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    THIRD_PARTY_ACCOUNT = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    THIRD_PARTY_STREET = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    THIRD_PARTY_STREET2 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    THIRD_PARTY_STREET3 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    THIRD_PARTY_STREET4 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    THIRD_PARTY_CITY = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: true),
                    THIRD_PARTY_STATE = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: true),
                    THIRD_PARTY_POSTAL_CODE = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    THIRD_PARTY_COUNTRY_ISO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: true, defaultValue: "US"),
                    FORTHEBENEFITOF_PAYEE = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    FORTHEBENEFITOF_ACCOUNT_TYPE = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    TAX1099_FOR_EMPLOYEE = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TAX1099_FOR_BENEFICIARY = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    FEDERAL_TAX_PERCENTAGE = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    STATE_TAX_PERCENTAGE = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    GROSS_AMOUNT = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    FEDERAL_TAX_AMOUNT = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    STATE_TAX_AMOUNT = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    CHECK_AMOUNT = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    TAX_CODE_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    DECEASED = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    GENDER_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: true),
                    QDRO = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    MEMO = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: true),
                    ROTH_IRA = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DISTRIBUTION", x => new { x.SSN, x.SEQUENCE_NUMBER });
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_COUNTRY_COUNTRY_ISO",
                        column: x => x.COUNTRY_ISO,
                        principalTable: "COUNTRY",
                        principalColumn: "ISO");
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_COUNTRY_THIRD_PARTY_COUNTRY_ISO",
                        column: x => x.THIRD_PARTY_COUNTRY_ISO,
                        principalTable: "COUNTRY",
                        principalColumn: "ISO");
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_DISTRIBUTION_FREQUENCY_FREQUENCYID",
                        column: x => x.FREQUENCY_ID,
                        principalTable: "DISTRIBUTION_FREQUENCY",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_DISTRIBUTION_STATUS_STATUSID",
                        column: x => x.STATUS_ID,
                        principalTable: "DISTRIBUTION_STATUS",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_GENDER_GENDERID",
                        column: x => x.GENDER_ID,
                        principalTable: "GENDER",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_TAX_CODE_TAXCODEID",
                        column: x => x.TAX_CODE_ID,
                        principalTable: "TAX_CODE",
                        principalColumn: "CODE");
                });

            migrationBuilder.CreateTable(
                name: "DEMOGRAPHIC",
                columns: table => new
                {
                    ORACLE_HCM_ID = table.Column<long>(type: "NUMBER(15)", precision: 15, nullable: false),
                    SSN = table.Column<long>(type: "NUMBER(9)", precision: 9, nullable: false),
                    BADGE_NUMBER = table.Column<int>(type: "NUMBER(7)", precision: 7, nullable: false),
                    LAST_MODIFIED_DATE = table.Column<DateTime>(type: "DATE", nullable: false, defaultValueSql: "SYSDATE"),
                    FULL_NAME = table.Column<string>(type: "NVARCHAR2(60)", maxLength: 60, nullable: false, comment: "FullName"),
                    LAST_NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false, comment: "LastName"),
                    FIRST_NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false, comment: "FirstName"),
                    MIDDLE_NAME = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: true, comment: "MiddleName"),
                    STORE_NUMBER = table.Column<short>(type: "NUMBER(3)", precision: 3, nullable: false, comment: "StoreNumber"),
                    PAY_CLASSIFICATION_ID = table.Column<byte>(type: "NUMBER(2)", precision: 2, nullable: false, comment: "PayClassification"),
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
                    COUNTRY_ISO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: true, defaultValue: "US"),
                    DATE_OF_BIRTH = table.Column<DateTime>(type: "DATE", nullable: false, comment: "DateOfBirth"),
                    FULL_TIME_DATE = table.Column<DateTime>(type: "DATE", nullable: true, comment: "FullTimeDate"),
                    HIRE_DATE = table.Column<DateTime>(type: "DATE", nullable: false, comment: "HireDate"),
                    REHIRE_DATE = table.Column<DateTime>(type: "DATE", nullable: true, comment: "ReHireDate"),
                    TERMINATION_DATE = table.Column<DateTime>(type: "DATE", nullable: true, comment: "TerminationDate"),
                    DEPARTMENT = table.Column<byte>(type: "NUMBER(1)", precision: 1, nullable: false, comment: "Department"),
                    EMPLOYMENT_TYPE_ID = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false, comment: "EmploymentType"),
                    GENDER_ID = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false, comment: "Gender"),
                    PAY_FREQUENCY_ID = table.Column<byte>(type: "NUMBER(3)", maxLength: 1, nullable: false, comment: "PayFrequency"),
                    TERMINATION_CODE_ID = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: true, comment: "TerminationCode"),
                    EMPLOYMENT_STATUS_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DEMOGRAPHIC", x => x.ORACLE_HCM_ID);
                    table.ForeignKey(
                        name: "FK_DEMOGRAPHIC_COUNTRY_COUNTRY_ISO",
                        column: x => x.COUNTRY_ISO,
                        principalTable: "COUNTRY",
                        principalColumn: "ISO");
                    table.ForeignKey(
                        name: "FK_DEMOGRAPHIC_DEPARTMENT_DEPARTMENTID",
                        column: x => x.DEPARTMENT,
                        principalTable: "DEPARTMENT",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DEMOGRAPHIC_EMPLOYMENTSTATUS_EMPLOYMENTSTATUSID",
                        column: x => x.EMPLOYMENT_STATUS_ID,
                        principalTable: "EMPLOYMENT_STATUS",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DEMOGRAPHIC_EMPLOYMENTTYPE_EMPLOYMENTTYPEID",
                        column: x => x.EMPLOYMENT_TYPE_ID,
                        principalTable: "EMPLOYMENT_TYPE",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DEMOGRAPHIC_GENDER_GENDERID",
                        column: x => x.GENDER_ID,
                        principalTable: "GENDER",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DEMOGRAPHIC_PAYCLASSIFICATIONS_PAYCLASSIFICATIONID",
                        column: x => x.PAY_CLASSIFICATION_ID,
                        principalTable: "PAY_CLASSIFICATION",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DEMOGRAPHIC_PAYFREQUENCY_PAYFREQUENCYID",
                        column: x => x.PAY_FREQUENCY_ID,
                        principalTable: "PAY_FREQUENCY",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DEMOGRAPHIC_TERMINATIONCODE_TERMINATIONCODEID",
                        column: x => x.TERMINATION_CODE_ID,
                        principalTable: "TERMINATION_CODE",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PAY_PROFIT",
                columns: table => new
                {
                    BADGE_NUMBER = table.Column<int>(type: "NUMBER(7)", precision: 7, nullable: false),
                    SSN = table.Column<long>(type: "NUMBER(9)", precision: 9, nullable: false),
                    HOURS_CURRENT_YEAR = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: false),
                    HOURS_LAST_YEAR = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: false),
                    INCOME_CURRENT_YEAR = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    INCOME_LAST_YEAR = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    EARNINGS_AFTER_APPLYING_VESTING_RULES = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    EARNINGS_ETVA_VALUE = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    SECONDARY_EARNINGS = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: true),
                    SECONDARY_ETVA_EARNINGS = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: true),
                    EARNINGS_PRIOR_ETVA_VALUE = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: true),
                    WEEKS_WORKED_YEAR = table.Column<byte>(type: "NUMBER(2)", precision: 2, nullable: false),
                    WEEKS_WORKED_LAST_YEAR = table.Column<byte>(type: "NUMBER(2)", precision: 2, nullable: false),
                    COMPANY_CONTRIBUTION_YEARS = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    CERTIFICATE_ISSUED_LAST_YEAR = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PS_CERTIFICATE_ISSUED_DATE = table.Column<DateTime>(type: "DATE", nullable: true),
                    INITIAL_CONTRIBUTION_YEAR = table.Column<short>(type: "NUMBER(4)", precision: 4, nullable: false),
                    NET_BALANCE_LAST_YEAR = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    EARNINGS_LAST_YEAR = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    POINTS_EARNED_LAST_YEAR = table.Column<int>(type: "NUMBER(5)", precision: 5, nullable: false),
                    VESTED_BALANCE_LAST_YEAR = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    CONTRIBUTION_AMOUNT_LAST_YEAR = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    FORFEITURE_AMOUNT_LAST_YEAR = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    ENROLLMENT_ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    BENEFICIARY_ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    EMPLOYEE_TYPE_ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    ZERO_CONTRIBUTION_REASON_ID = table.Column<byte>(type: "NUMBER(3)", nullable: true),
                    HOURS_EXECUTIVE = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: false),
                    INCOME_EXECUTIVE = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAY_PROFIT", x => x.BADGE_NUMBER);
                    table.ForeignKey(
                        name: "FK_PAY_PROFIT_BENEFICIARYTYPES_BENEFICIARYTYPEID",
                        column: x => x.BENEFICIARY_ID,
                        principalTable: "BENEFICIARY_TYPE",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_PAY_PROFIT_EMPLOYEETYPES_EMPLOYEETYPEID",
                        column: x => x.EMPLOYEE_TYPE_ID,
                        principalTable: "EMPLOYEE_TYPE",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_PAY_PROFIT_ENROLLMENT_ENROLLMENTID",
                        column: x => x.ENROLLMENT_ID,
                        principalTable: "ENROLLMENT",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_PAY_PROFIT_ZEROCONTRIBUTIONREASON_ZEROCONTRIBUTIONREASONID",
                        column: x => x.ZERO_CONTRIBUTION_REASON_ID,
                        principalTable: "ZERO_CONTRIBUTION_REASON",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PROFIT_DETAIL",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PROFIT_YEAR = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    PROFIT_YEAR_ITERATION = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    PROFIT_CODE_ID = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    CONTRIBUTION = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    EARNINGS = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    FORFEITURE = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    MONTH_TO_DATE = table.Column<byte>(type: "NUMBER(2,0)", precision: 2, scale: 0, nullable: false),
                    YEAR_TO_DATE = table.Column<short>(type: "NUMBER(4,0)", precision: 4, scale: 0, nullable: false),
                    REMARK = table.Column<string>(type: "NVARCHAR2(32)", maxLength: 32, nullable: true),
                    ZERO_CONTRIBUTION_REASON_ID = table.Column<byte>(type: "NUMBER(3)", nullable: true),
                    FEDERAL_TAXES = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    STATE_TAXES = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    TAX_CODE_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: true),
                    SSN = table.Column<long>(type: "NUMBER(9)", precision: 9, nullable: false),
                    DISTRIBUTION_SEQUENCE = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROFIT_DETAIL", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PROFIT_DETAIL_PROFITCODES_PROFIT_CODE_ID",
                        column: x => x.PROFIT_CODE_ID,
                        principalTable: "PROFIT_CODE",
                        principalColumn: "CODE");
                    table.ForeignKey(
                        name: "FK_PROFIT_DETAIL_TAXCODES_TAX_CODE_ID",
                        column: x => x.TAX_CODE_ID,
                        principalTable: "TAX_CODE",
                        principalColumn: "CODE");
                    table.ForeignKey(
                        name: "FK_PROFIT_DETAIL_ZEROCONTRIBUTIONREASON_ZEROCONTRIBUTIONREASONID",
                        column: x => x.ZERO_CONTRIBUTION_REASON_ID,
                        principalTable: "ZERO_CONTRIBUTION_REASON",
                        principalColumn: "ID");
                });

            migrationBuilder.InsertData(
                table: "BENEFICIARY_KIND",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { "P", "Primary" },
                    { "S", "Secondary" }
                });

            migrationBuilder.InsertData(
                table: "BENEFICIARY_TYPE",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { (byte)0, "Employee" },
                    { (byte)1, "Beneficiary" }
                });

            migrationBuilder.InsertData(
                table: "CALDAR_RECORD",
                columns: new[] { "ACC_WKEND_N", "ACC_ALT_KEY_NUM", "ACC_APWKEND", "ACC_CALPERIOD", "ACC_CLN60_PERIOD", "ACC_CLN60_WEEK", "ACC_CLN61_PERIOD", "ACC_CLN61_WEEK", "ACC_CLN6X_PERIOD", "ACC_CLN6X_WEEK", "ACC_CLN7X_PERIOD", "ACC_CLN7X_WEEK", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN", "ACC_WKEND2_N" },
                values: new object[,]
                {
                    { 101, -356L, 108, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20000101L },
                    { 108, -347L, 115, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20000108L },
                    { 115, -346L, 122, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20000115L },
                    { 122, -345L, 129, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20000122L },
                    { 129, -344L, 205, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20000129L },
                    { 205, -343L, 212, 2, 2, 5, 2, 5, 2, 5, 2, 5, 2, 1, 5, 20000205L },
                    { 212, -342L, 219, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20000212L },
                    { 219, -341L, 226, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20000219L },
                    { 226, -340L, 304, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20000226L },
                    { 304, -339L, 311, 3, 3, 9, 3, 9, 3, 9, 3, 9, 3, 1, 9, 20000304L },
                    { 311, -338L, 318, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20000311L },
                    { 318, -337L, 325, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20000318L },
                    { 325, -336L, 401, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 1, 12, 20000325L },
                    { 401, -335L, 408, 3, 3, 13, 3, 13, 3, 13, 3, 13, 3, 2, 13, 20000401L },
                    { 408, -334L, 415, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20000408L },
                    { 415, -333L, 422, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20000415L },
                    { 422, -332L, 429, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20000422L },
                    { 429, -331L, 506, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20000429L },
                    { 506, -330L, 513, 5, 5, 18, 5, 18, 5, 18, 5, 18, 5, 2, 18, 20000506L },
                    { 513, -329L, 520, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20000513L },
                    { 520, -328L, 527, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20000520L },
                    { 527, -327L, 603, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20000527L },
                    { 603, -326L, 610, 5, 5, 22, 5, 22, 5, 22, 5, 22, 5, 2, 22, 20000603L },
                    { 610, -325L, 617, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20000610L },
                    { 617, -324L, 624, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20000617L },
                    { 624, -323L, 701, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 2, 25, 20000624L },
                    { 701, -322L, 708, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 20000701L },
                    { 708, -321L, 715, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20000708L },
                    { 715, -320L, 722, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20000715L },
                    { 722, -319L, 729, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20000722L },
                    { 729, -318L, 805, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20000729L },
                    { 805, -317L, 812, 8, 8, 31, 8, 31, 8, 31, 8, 31, 8, 3, 31, 20000805L },
                    { 812, -316L, 819, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20000812L },
                    { 819, -315L, 826, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20000819L },
                    { 826, -314L, 902, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20000826L },
                    { 902, -313L, 909, 8, 8, 35, 8, 35, 8, 35, 8, 35, 8, 3, 35, 20000902L },
                    { 909, -312L, 916, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20000909L },
                    { 916, -311L, 923, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20000916L },
                    { 923, -310L, 930, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 20000923L },
                    { 930, -309L, 1007, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20000930L },
                    { 1007, -308L, 1014, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20001007L },
                    { 1014, -307L, 1021, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20001014L },
                    { 1021, -306L, 1028, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20001021L },
                    { 1028, -305L, 1104, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20001028L },
                    { 1104, -304L, 1111, 11, 11, 44, 11, 44, 11, 44, 11, 44, 11, 4, 44, 20001104L },
                    { 1111, -303L, 1118, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20001111L },
                    { 1118, -302L, 1125, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20001118L },
                    { 1125, -301L, 1202, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20001125L },
                    { 1202, -300L, 1209, 11, 11, 48, 11, 48, 11, 48, 11, 48, 11, 4, 48, 20001202L },
                    { 1209, -299L, 1216, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20001209L },
                    { 1216, -298L, 1223, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20001216L },
                    { 1223, -297L, 1230, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20001223L },
                    { 1230, -296L, 10106, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20001230L },
                    { 10106, -287L, 10113, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20010106L },
                    { 10113, -286L, 10120, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20010113L },
                    { 10120, -285L, 10127, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20010120L },
                    { 10127, -284L, 10203, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20010127L },
                    { 10203, -283L, 10210, 1, 1, 5, 1, 5, 1, 5, 1, 5, 1, 1, 5, 20010203L },
                    { 10210, -282L, 10217, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20010210L },
                    { 10217, -281L, 10224, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20010217L },
                    { 10224, -280L, 10303, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20010224L },
                    { 10303, -279L, 10310, 2, 2, 9, 2, 9, 2, 9, 2, 9, 2, 1, 9, 20010303L },
                    { 10310, -278L, 10317, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20010310L },
                    { 10317, -277L, 10324, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20010317L },
                    { 10324, -276L, 10331, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 1, 12, 20010324L },
                    { 10331, -275L, 10407, 3, 3, 13, 3, 13, 3, 13, 3, 13, 3, 2, 13, 20010331L },
                    { 10407, -274L, 10414, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20010407L },
                    { 10414, -273L, 10421, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20010414L },
                    { 10421, -272L, 10428, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20010421L },
                    { 10428, -271L, 10505, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20010428L },
                    { 10505, -270L, 10512, 5, 5, 18, 5, 18, 5, 18, 5, 18, 5, 2, 18, 20010505L },
                    { 10512, -269L, 10519, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20010512L },
                    { 10519, -268L, 10526, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20010519L },
                    { 10526, -267L, 10602, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20010526L },
                    { 10602, -266L, 10609, 5, 5, 22, 5, 22, 5, 22, 5, 22, 5, 2, 22, 20010602L },
                    { 10609, -265L, 10616, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20010609L },
                    { 10616, -264L, 10623, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20010616L },
                    { 10623, -263L, 10630, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 2, 25, 20010623L },
                    { 10630, -262L, 10707, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 20010630L },
                    { 10707, -261L, 10714, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20010707L },
                    { 10714, -260L, 10721, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20010714L },
                    { 10721, -259L, 10728, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20010721L },
                    { 10728, -258L, 10804, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20010728L },
                    { 10804, -257L, 10811, 8, 8, 31, 8, 31, 8, 31, 8, 31, 8, 3, 31, 20010804L },
                    { 10811, -256L, 10818, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20010811L },
                    { 10818, -255L, 10825, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20010818L },
                    { 10825, -254L, 10901, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20010825L },
                    { 10901, -253L, 10908, 8, 8, 35, 8, 35, 8, 35, 8, 35, 8, 3, 35, 20010901L },
                    { 10908, -252L, 10915, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20010908L },
                    { 10915, -251L, 10922, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20010915L },
                    { 10922, -250L, 10929, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 20010922L },
                    { 10929, -249L, 11006, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20010929L },
                    { 11006, -248L, 11013, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20011006L },
                    { 11013, -247L, 11020, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20011013L },
                    { 11020, -246L, 11027, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20011020L },
                    { 11027, -245L, 11103, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20011027L },
                    { 11103, -244L, 11110, 10, 10, 44, 10, 44, 10, 44, 10, 44, 10, 4, 44, 20011103L },
                    { 11110, -243L, 11117, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20011110L },
                    { 11117, -242L, 11124, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20011117L },
                    { 11124, -241L, 11201, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20011124L },
                    { 11201, -240L, 11208, 11, 11, 48, 11, 48, 11, 48, 11, 48, 11, 4, 48, 20011201L },
                    { 11208, -239L, 11215, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20011208L },
                    { 11215, -238L, 11222, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20011215L },
                    { 11222, -237L, 11229, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20011222L },
                    { 11229, -236L, 20105, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20011229L },
                    { 20105, -227L, 20112, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20020105L },
                    { 20112, -226L, 20119, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20020112L },
                    { 20119, -225L, 20126, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20020119L },
                    { 20126, -224L, 20202, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20020126L },
                    { 20202, -223L, 20209, 1, 1, 5, 1, 5, 1, 5, 1, 5, 1, 1, 5, 20020202L },
                    { 20209, -222L, 20216, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20020209L },
                    { 20216, -221L, 20223, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20020216L },
                    { 20223, -220L, 20302, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20020223L },
                    { 20302, -219L, 20309, 2, 2, 9, 2, 9, 2, 9, 2, 9, 2, 1, 9, 20020302L },
                    { 20309, -218L, 20316, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20020309L },
                    { 20316, -217L, 20323, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20020316L },
                    { 20323, -216L, 20330, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 1, 12, 20020323L },
                    { 20330, -215L, 20406, 3, 3, 13, 3, 13, 3, 13, 3, 13, 3, 2, 13, 20020330L },
                    { 20406, -214L, 20413, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20020406L },
                    { 20413, -213L, 20420, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20020413L },
                    { 20420, -212L, 20427, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20020420L },
                    { 20427, -211L, 20504, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20020427L },
                    { 20504, -210L, 20511, 5, 5, 18, 5, 18, 5, 18, 5, 18, 5, 2, 18, 20020504L },
                    { 20511, -209L, 20518, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20020511L },
                    { 20518, -208L, 20525, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20020518L },
                    { 20525, -207L, 20601, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20020525L },
                    { 20601, -206L, 20608, 5, 5, 22, 5, 22, 5, 22, 5, 22, 5, 2, 22, 20020601L },
                    { 20608, -205L, 20615, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20020608L },
                    { 20615, -204L, 20622, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20020615L },
                    { 20622, -203L, 20629, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 2, 25, 20020622L },
                    { 20629, -202L, 20706, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 20020629L },
                    { 20706, -201L, 20713, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20020706L },
                    { 20713, -200L, 20720, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20020713L },
                    { 20720, -199L, 20727, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20020720L },
                    { 20727, -198L, 20803, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20020727L },
                    { 20803, -197L, 20810, 8, 8, 31, 8, 31, 8, 31, 8, 31, 8, 3, 31, 20020803L },
                    { 20810, -196L, 20817, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20020810L },
                    { 20817, -195L, 20824, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20020817L },
                    { 20824, -194L, 20831, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20020824L },
                    { 20831, -193L, 20907, 8, 8, 35, 8, 35, 8, 35, 8, 35, 8, 3, 35, 20020831L },
                    { 20907, -192L, 20914, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20020907L },
                    { 20914, -191L, 20921, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20020914L },
                    { 20921, -190L, 20928, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 20020921L },
                    { 20928, -189L, 21005, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20020928L },
                    { 21005, -188L, 21012, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20021005L },
                    { 21012, -187L, 21019, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20021012L },
                    { 21019, -186L, 21026, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20021019L },
                    { 21026, -185L, 21102, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20021026L },
                    { 21102, -184L, 21109, 10, 10, 44, 10, 44, 10, 44, 10, 44, 10, 4, 44, 20021102L },
                    { 21109, -183L, 21116, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20021109L },
                    { 21116, -182L, 21123, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20021116L },
                    { 21123, -181L, 21130, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20021123L },
                    { 21130, -180L, 21207, 11, 11, 48, 11, 48, 11, 48, 11, 48, 11, 4, 48, 20021130L },
                    { 21207, -179L, 21214, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20021207L },
                    { 21214, -178L, 21221, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20021214L },
                    { 21221, -177L, 21228, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20021221L },
                    { 21228, -176L, 30104, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20021228L },
                    { 30104, -167L, 30111, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20030104L },
                    { 30111, -166L, 30118, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20030111L },
                    { 30118, -165L, 30125, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20030118L },
                    { 30125, -164L, 30201, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20030125L },
                    { 30201, -163L, 30208, 1, 1, 5, 1, 5, 1, 5, 1, 5, 1, 1, 5, 20030201L },
                    { 30208, -162L, 30215, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20030208L },
                    { 30215, -161L, 30222, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20030215L },
                    { 30222, -160L, 30301, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20030222L },
                    { 30301, -159L, 30308, 2, 2, 9, 2, 9, 2, 9, 2, 9, 2, 1, 9, 20030301L },
                    { 30308, -158L, 30315, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20030308L },
                    { 30315, -157L, 30322, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20030315L },
                    { 30322, -156L, 30329, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 1, 12, 20030322L },
                    { 30329, -155L, 30405, 3, 3, 13, 3, 13, 3, 13, 3, 13, 3, 2, 13, 20030329L },
                    { 30405, -154L, 30412, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20030405L },
                    { 30412, -153L, 30419, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20030412L },
                    { 30419, -152L, 30426, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20030419L },
                    { 30426, -151L, 30503, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20030426L },
                    { 30503, -150L, 30510, 4, 4, 18, 4, 18, 4, 18, 4, 18, 4, 2, 18, 20030503L },
                    { 30510, -149L, 30517, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20030510L },
                    { 30517, -148L, 30524, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20030517L },
                    { 30524, -147L, 30531, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20030524L },
                    { 30531, -146L, 30607, 5, 5, 22, 5, 22, 5, 22, 5, 22, 5, 2, 22, 20030531L },
                    { 30607, -145L, 30614, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20030607L },
                    { 30614, -144L, 30621, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20030614L },
                    { 30621, -143L, 30628, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 2, 25, 20030621L },
                    { 30628, -142L, 30705, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 20030628L },
                    { 30705, -141L, 30712, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20030705L },
                    { 30712, -140L, 30719, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20030712L },
                    { 30719, -139L, 30726, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20030719L },
                    { 30726, -138L, 30802, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20030726L },
                    { 30802, -137L, 30809, 7, 7, 31, 7, 31, 7, 31, 7, 31, 7, 3, 31, 20030802L },
                    { 30809, -136L, 30816, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20030809L },
                    { 30816, -135L, 30823, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20030816L },
                    { 30823, -134L, 30830, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20030823L },
                    { 30830, -133L, 30906, 8, 8, 35, 8, 35, 8, 35, 8, 35, 8, 3, 35, 20030830L },
                    { 30906, -132L, 30913, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20030906L },
                    { 30913, -131L, 30920, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20030913L },
                    { 30920, -130L, 30927, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 20030920L },
                    { 30927, -129L, 31004, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20030927L },
                    { 31004, -128L, 31011, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20031004L },
                    { 31011, -127L, 31018, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20031011L },
                    { 31018, -126L, 31025, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20031018L },
                    { 31025, -125L, 31101, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20031025L },
                    { 31101, -124L, 31108, 10, 10, 44, 10, 44, 10, 44, 10, 44, 10, 4, 44, 20031101L },
                    { 31108, -123L, 31115, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20031108L },
                    { 31115, -122L, 31122, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20031115L },
                    { 31122, -121L, 31129, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20031122L },
                    { 31129, -120L, 31206, 11, 11, 48, 11, 48, 11, 48, 11, 48, 11, 4, 48, 20031129L },
                    { 31206, -119L, 31213, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20031206L },
                    { 31213, -118L, 31220, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20031213L },
                    { 31220, -117L, 31227, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20031220L },
                    { 31227, -116L, 40103, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20031227L },
                    { 40103, -115L, 40110, 12, 12, 53, 12, 53, 12, 53, 12, 53, 12, 1, 53, 20040103L },
                    { 40110, -107L, 40117, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20040110L },
                    { 40117, -106L, 40124, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20040117L },
                    { 40124, -105L, 40131, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20040124L },
                    { 40131, -104L, 40207, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20040131L },
                    { 40207, -103L, 40214, 2, 2, 5, 2, 5, 2, 5, 2, 5, 2, 1, 5, 20040207L },
                    { 40214, -102L, 40221, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20040214L },
                    { 40221, -101L, 40228, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20040221L },
                    { 40228, -100L, 40306, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20040228L },
                    { 40306, -99L, 40313, 3, 3, 9, 3, 9, 3, 9, 3, 9, 3, 1, 9, 20040306L },
                    { 40313, -98L, 40320, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20040313L },
                    { 40320, -97L, 40327, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20040320L },
                    { 40327, -96L, 40403, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 2, 12, 20040327L },
                    { 40403, -95L, 40410, 3, 3, 13, 3, 13, 3, 13, 3, 13, 3, 2, 13, 20040403L },
                    { 40410, -94L, 40417, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20040410L },
                    { 40417, -93L, 40424, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20040417L },
                    { 40424, -92L, 40501, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20040424L },
                    { 40501, -91L, 40508, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20040501L },
                    { 40508, -90L, 40515, 5, 5, 18, 5, 18, 5, 18, 5, 18, 5, 2, 18, 20040508L },
                    { 40515, -89L, 40522, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20040515L },
                    { 40522, -88L, 40529, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20040522L },
                    { 40529, -87L, 40605, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20040529L },
                    { 40605, -86L, 40612, 6, 6, 22, 6, 22, 6, 22, 6, 22, 6, 2, 22, 20040605L },
                    { 40612, -85L, 40619, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20040612L },
                    { 40619, -84L, 40626, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20040619L },
                    { 40626, -83L, 40703, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 3, 25, 20040626L },
                    { 40703, -82L, 40710, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 20040703L },
                    { 40710, -81L, 40717, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20040710L },
                    { 40717, -80L, 40724, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20040717L },
                    { 40724, -79L, 40731, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20040724L },
                    { 40731, -78L, 40807, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20040731L },
                    { 40807, -77L, 40814, 8, 8, 31, 8, 31, 8, 31, 8, 31, 8, 3, 31, 20040807L },
                    { 40814, -76L, 40821, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20040814L },
                    { 40821, -75L, 40828, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20040821L },
                    { 40828, -74L, 40904, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20040828L },
                    { 40904, -73L, 40911, 9, 9, 35, 9, 35, 9, 35, 9, 35, 9, 3, 35, 20040904L },
                    { 40911, -72L, 40918, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20040911L },
                    { 40918, -71L, 40925, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20040918L },
                    { 40925, -70L, 41002, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 20040925L },
                    { 41002, -69L, 41009, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20041002L },
                    { 41009, -68L, 41016, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20041009L },
                    { 41016, -67L, 41023, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20041016L },
                    { 41023, -66L, 41030, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20041023L },
                    { 41030, -65L, 41106, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20041030L },
                    { 41106, -64L, 41113, 11, 11, 44, 11, 44, 11, 44, 11, 44, 11, 4, 44, 20041106L },
                    { 41113, -63L, 41120, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20041113L },
                    { 41120, -62L, 41127, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20041120L },
                    { 41127, -61L, 41204, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20041127L },
                    { 41204, -60L, 41211, 12, 12, 48, 12, 48, 12, 48, 12, 48, 12, 4, 48, 20041204L },
                    { 41211, -59L, 41218, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20041211L },
                    { 41218, -58L, 41225, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20041218L },
                    { 41225, -57L, 50101, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20041225L },
                    { 50101, -56L, 50108, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20050101L },
                    { 50108, -47L, 50115, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20050108L },
                    { 50115, -46L, 50122, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20050115L },
                    { 50122, -45L, 50129, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20050122L },
                    { 50129, -44L, 50205, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20050129L },
                    { 50205, -43L, 50212, 2, 2, 5, 2, 5, 2, 5, 2, 5, 2, 1, 5, 20050205L },
                    { 50212, -42L, 50219, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20050212L },
                    { 50219, -41L, 50226, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20050219L },
                    { 50226, -40L, 50305, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20050226L },
                    { 50305, -39L, 50312, 3, 3, 9, 3, 9, 3, 9, 3, 9, 3, 1, 9, 20050305L },
                    { 50312, -38L, 50319, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20050312L },
                    { 50319, -37L, 50326, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20050319L },
                    { 50326, -36L, 50402, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 1, 12, 20050326L },
                    { 50402, -35L, 50409, 3, 3, 13, 3, 13, 3, 13, 3, 13, 3, 2, 13, 20050402L },
                    { 50409, -34L, 50416, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20050409L },
                    { 50416, -33L, 50423, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20050416L },
                    { 50423, -32L, 50430, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20050423L },
                    { 50430, -31L, 50507, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20050430L },
                    { 50507, -30L, 50514, 5, 5, 18, 5, 18, 5, 18, 5, 18, 5, 2, 18, 20050507L },
                    { 50514, -29L, 50521, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20050514L },
                    { 50521, -28L, 50528, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20050521L },
                    { 50528, -27L, 50604, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20050528L },
                    { 50604, -26L, 50611, 6, 6, 22, 6, 22, 6, 22, 6, 22, 6, 2, 22, 20050604L },
                    { 50611, -25L, 50618, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20050611L },
                    { 50618, -24L, 50625, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20050618L },
                    { 50625, -23L, 50702, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 2, 25, 20050625L },
                    { 50702, -22L, 50709, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 20050702L },
                    { 50709, -21L, 50716, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20050709L },
                    { 50716, -20L, 50723, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20050716L },
                    { 50723, -19L, 50730, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20050723L },
                    { 50730, -18L, 50806, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20050730L },
                    { 50806, -17L, 50813, 8, 8, 31, 8, 31, 8, 31, 8, 31, 8, 3, 31, 20050806L },
                    { 50813, -16L, 50820, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20050813L },
                    { 50820, -15L, 50827, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20050820L },
                    { 50827, -14L, 50903, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20050827L },
                    { 50903, -13L, 50910, 8, 8, 35, 8, 35, 8, 35, 8, 35, 8, 3, 35, 20050903L },
                    { 50910, -12L, 50917, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20050910L },
                    { 50917, -11L, 50924, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20050917L },
                    { 50924, -10L, 51001, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 20050924L },
                    { 51001, -9L, 51008, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20051001L },
                    { 51008, -8L, 51015, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20051008L },
                    { 51015, -7L, 51022, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20051015L },
                    { 51022, -6L, 51029, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20051022L },
                    { 51029, -5L, 51105, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20051029L },
                    { 51105, -4L, 51112, 11, 11, 44, 11, 44, 11, 44, 11, 44, 11, 4, 44, 20051105L },
                    { 51112, -3L, 51119, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20051112L },
                    { 51119, -2L, 51126, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20051119L },
                    { 51126, -1L, 51203, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20051126L },
                    { 51203, 0L, 51210, 11, 11, 48, 11, 48, 11, 48, 11, 48, 11, 4, 48, 20051203L },
                    { 51210, 1L, 51217, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20051210L },
                    { 51217, 2L, 51224, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20051217L },
                    { 51224, 3L, 51231, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20051224L },
                    { 51231, 4L, 60107, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20051231L },
                    { 60107, 13L, 60114, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20060107L },
                    { 60114, 14L, 60121, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20060114L },
                    { 60121, 15L, 60128, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20060121L },
                    { 60128, 16L, 60204, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20060128L },
                    { 60204, 17L, 60211, 2, 2, 5, 2, 5, 2, 5, 2, 5, 2, 1, 5, 20060204L },
                    { 60211, 18L, 60218, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20060211L },
                    { 60218, 19L, 60225, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20060218L },
                    { 60225, 20L, 60304, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20060225L },
                    { 60304, 21L, 60311, 3, 3, 9, 3, 9, 3, 9, 3, 9, 3, 1, 9, 20060304L },
                    { 60311, 22L, 60318, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20060311L },
                    { 60318, 23L, 60325, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20060318L },
                    { 60325, 24L, 60401, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 1, 12, 20060325L },
                    { 60401, 25L, 60408, 3, 3, 13, 3, 13, 3, 13, 3, 13, 3, 2, 13, 20060401L },
                    { 60408, 26L, 60415, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20060408L },
                    { 60415, 27L, 60422, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20060415L },
                    { 60422, 28L, 60429, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20060422L },
                    { 60429, 29L, 60506, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20060429L },
                    { 60506, 30L, 60513, 5, 5, 18, 5, 18, 5, 18, 5, 18, 5, 2, 18, 20060506L },
                    { 60513, 31L, 60520, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20060513L },
                    { 60520, 32L, 60527, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20060520L },
                    { 60527, 33L, 60603, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20060527L },
                    { 60603, 34L, 60610, 5, 5, 22, 5, 22, 5, 22, 5, 22, 5, 2, 22, 20060603L },
                    { 60610, 35L, 60617, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20060610L },
                    { 60617, 36L, 60624, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20060617L },
                    { 60624, 37L, 60701, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 2, 25, 20060624L },
                    { 60701, 38L, 60708, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 20060701L },
                    { 60708, 39L, 60715, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20060708L },
                    { 60715, 40L, 60722, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20060715L },
                    { 60722, 41L, 60729, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20060722L },
                    { 60729, 42L, 60805, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20060729L },
                    { 60805, 43L, 60812, 8, 8, 31, 8, 31, 8, 31, 8, 31, 8, 3, 31, 20060805L },
                    { 60812, 44L, 60819, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20060812L },
                    { 60819, 45L, 60826, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20060819L },
                    { 60826, 46L, 60902, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20060826L },
                    { 60902, 47L, 60909, 8, 8, 35, 8, 35, 8, 35, 8, 35, 8, 3, 35, 20060902L },
                    { 60909, 48L, 60916, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20060909L },
                    { 60916, 49L, 60923, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20060916L },
                    { 60923, 50L, 60930, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 20060923L },
                    { 60930, 51L, 61007, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20060930L },
                    { 61007, 52L, 61014, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20061007L },
                    { 61014, 53L, 61021, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20061014L },
                    { 61021, 54L, 61028, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20061021L },
                    { 61028, 55L, 61104, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20061028L },
                    { 61104, 56L, 61111, 11, 11, 44, 11, 44, 11, 44, 11, 44, 11, 4, 44, 20061104L },
                    { 61111, 57L, 61118, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20061111L },
                    { 61118, 58L, 61125, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20061118L },
                    { 61125, 59L, 61202, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20061125L },
                    { 61202, 60L, 61209, 11, 11, 48, 11, 48, 11, 48, 11, 48, 11, 4, 48, 20061202L },
                    { 61209, 61L, 61216, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20061209L },
                    { 61216, 62L, 61223, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20061216L },
                    { 61223, 63L, 61230, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20061223L },
                    { 61230, 64L, 70106, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20061230L },
                    { 70106, 73L, 70113, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20070106L },
                    { 70113, 74L, 70120, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20070113L },
                    { 70120, 75L, 70127, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20070120L },
                    { 70127, 76L, 70203, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20070127L },
                    { 70203, 77L, 70210, 1, 1, 5, 1, 5, 1, 5, 1, 5, 1, 1, 5, 20070203L },
                    { 70210, 78L, 70217, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20070210L },
                    { 70217, 79L, 70224, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20070217L },
                    { 70224, 80L, 70303, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20070224L },
                    { 70303, 81L, 70310, 2, 2, 9, 2, 9, 2, 9, 2, 9, 2, 1, 9, 20070303L },
                    { 70310, 82L, 70317, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20070310L },
                    { 70317, 83L, 70324, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20070317L },
                    { 70324, 84L, 70331, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 1, 12, 20070324L },
                    { 70331, 85L, 70407, 3, 3, 13, 3, 13, 3, 13, 3, 13, 3, 2, 13, 20070331L },
                    { 70407, 86L, 70414, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20070407L },
                    { 70414, 87L, 70421, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20070414L },
                    { 70421, 88L, 70428, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20070421L },
                    { 70428, 89L, 70505, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20070428L },
                    { 70505, 90L, 70512, 5, 5, 18, 5, 18, 5, 18, 5, 18, 5, 2, 18, 20070505L },
                    { 70512, 91L, 70519, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20070512L },
                    { 70519, 92L, 70526, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20070519L },
                    { 70526, 93L, 70602, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20070526L },
                    { 70602, 94L, 70609, 5, 5, 22, 5, 22, 5, 22, 5, 22, 5, 2, 22, 20070602L },
                    { 70609, 95L, 70616, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20070609L },
                    { 70616, 96L, 70623, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20070616L },
                    { 70623, 97L, 70630, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 2, 25, 20070623L },
                    { 70630, 98L, 70707, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 20070630L },
                    { 70707, 99L, 70714, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20070707L },
                    { 70714, 100L, 70721, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20070714L },
                    { 70721, 101L, 70728, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20070721L },
                    { 70728, 102L, 70804, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20070728L },
                    { 70804, 103L, 70811, 8, 8, 31, 8, 31, 8, 31, 8, 31, 8, 3, 31, 20070804L },
                    { 70811, 104L, 70818, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20070811L },
                    { 70818, 105L, 70825, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20070818L },
                    { 70825, 106L, 70901, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20070825L },
                    { 70901, 107L, 70908, 8, 8, 35, 8, 35, 8, 35, 8, 35, 8, 3, 35, 20070901L },
                    { 70908, 108L, 70915, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20070908L },
                    { 70915, 109L, 70922, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20070915L },
                    { 70922, 110L, 70929, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 20070922L },
                    { 70929, 111L, 71006, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20070929L },
                    { 71006, 112L, 71013, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20071006L },
                    { 71013, 113L, 71020, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20071013L },
                    { 71020, 114L, 71027, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20071020L },
                    { 71027, 115L, 71103, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20071027L },
                    { 71103, 116L, 71110, 10, 10, 44, 10, 44, 10, 44, 10, 44, 10, 4, 44, 20071103L },
                    { 71110, 117L, 71117, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20071110L },
                    { 71117, 118L, 71124, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20071117L },
                    { 71124, 119L, 71201, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20071124L },
                    { 71201, 120L, 71208, 11, 11, 48, 11, 48, 11, 48, 11, 48, 11, 4, 48, 20071201L },
                    { 71208, 121L, 71215, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20071208L },
                    { 71215, 122L, 71222, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20071215L },
                    { 71222, 123L, 71229, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20071222L },
                    { 71229, 124L, 80105, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20071229L },
                    { 80105, 133L, 80112, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20080105L },
                    { 80112, 134L, 80119, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20080112L },
                    { 80119, 135L, 80126, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20080119L },
                    { 80126, 136L, 80202, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20080126L },
                    { 80202, 137L, 80209, 1, 1, 5, 1, 5, 1, 5, 1, 5, 1, 1, 5, 20080202L },
                    { 80209, 138L, 80216, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20080209L },
                    { 80216, 139L, 80223, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20080216L },
                    { 80223, 140L, 80301, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20080223L },
                    { 80301, 141L, 80308, 2, 2, 9, 2, 9, 2, 9, 2, 9, 2, 1, 9, 20080301L },
                    { 80308, 142L, 80315, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20080308L },
                    { 80315, 143L, 80322, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20080315L },
                    { 80322, 144L, 80329, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 1, 12, 20080322L },
                    { 80329, 145L, 80405, 3, 3, 13, 3, 13, 3, 13, 3, 13, 3, 2, 13, 20080329L },
                    { 80405, 146L, 80412, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20080405L },
                    { 80412, 147L, 80419, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20080412L },
                    { 80419, 148L, 80426, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20080419L },
                    { 80426, 149L, 80503, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20080426L },
                    { 80503, 150L, 80510, 4, 4, 18, 4, 18, 4, 18, 4, 18, 4, 2, 18, 20080503L },
                    { 80510, 151L, 80517, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20080510L },
                    { 80517, 152L, 80524, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20080517L },
                    { 80524, 153L, 80531, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20080524L },
                    { 80531, 154L, 80607, 5, 5, 22, 5, 22, 5, 22, 5, 22, 5, 2, 22, 20080531L },
                    { 80607, 155L, 80614, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20080607L },
                    { 80614, 156L, 80621, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20080614L },
                    { 80621, 157L, 80628, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 2, 25, 20080621L },
                    { 80628, 158L, 80705, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 20080628L },
                    { 80705, 159L, 80712, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20080705L },
                    { 80712, 160L, 80719, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20080712L },
                    { 80719, 161L, 80726, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20080719L },
                    { 80726, 162L, 80802, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20080726L },
                    { 80802, 163L, 80809, 7, 7, 31, 7, 31, 7, 31, 7, 31, 7, 3, 31, 20080802L },
                    { 80809, 164L, 80816, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20080809L },
                    { 80816, 165L, 80823, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20080816L },
                    { 80823, 166L, 80830, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20080823L },
                    { 80830, 167L, 80906, 8, 8, 35, 8, 35, 8, 35, 8, 35, 8, 3, 35, 20080830L },
                    { 80906, 168L, 80913, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20080906L },
                    { 80913, 169L, 80920, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20080913L },
                    { 80920, 170L, 80927, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 20080920L },
                    { 80927, 171L, 81004, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20080927L },
                    { 81004, 172L, 81011, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20081004L },
                    { 81011, 173L, 81018, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20081011L },
                    { 81018, 174L, 81025, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20081018L },
                    { 81025, 175L, 81101, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20081025L },
                    { 81101, 176L, 81108, 10, 10, 44, 10, 44, 10, 44, 10, 44, 10, 4, 44, 20081101L },
                    { 81108, 177L, 81115, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20081108L },
                    { 81115, 178L, 81122, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20081115L },
                    { 81122, 179L, 81129, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20081122L },
                    { 81129, 180L, 81206, 11, 11, 48, 11, 48, 11, 48, 11, 48, 11, 4, 48, 20081129L },
                    { 81206, 181L, 81213, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20081206L },
                    { 81213, 182L, 81220, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20081213L },
                    { 81220, 183L, 81227, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20081220L },
                    { 81227, 184L, 90103, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20081227L },
                    { 90103, 185L, 90110, 12, 12, 53, 12, 53, 12, 53, 12, 53, 12, 1, 53, 20090103L },
                    { 90110, 193L, 90117, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20090110L },
                    { 90117, 194L, 90124, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20090117L },
                    { 90124, 195L, 90131, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20090124L },
                    { 90131, 196L, 90207, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20090131L },
                    { 90207, 197L, 90214, 2, 2, 5, 2, 5, 2, 5, 2, 5, 2, 1, 5, 20090207L },
                    { 90214, 198L, 90221, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20090214L },
                    { 90221, 199L, 90228, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20090221L },
                    { 90228, 200L, 90307, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20090228L },
                    { 90307, 201L, 90314, 3, 3, 9, 3, 9, 3, 9, 3, 9, 3, 1, 9, 20090307L },
                    { 90314, 202L, 90321, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20090314L },
                    { 90321, 203L, 90328, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20090321L },
                    { 90328, 204L, 90404, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 2, 12, 20090328L },
                    { 90404, 205L, 90411, 4, 4, 13, 4, 13, 4, 13, 4, 13, 4, 2, 13, 20090404L },
                    { 90411, 206L, 90418, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20090411L },
                    { 90418, 207L, 90425, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20090418L },
                    { 90425, 208L, 90502, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20090425L },
                    { 90502, 209L, 90509, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20090502L },
                    { 90509, 210L, 90516, 5, 5, 18, 5, 18, 5, 18, 5, 18, 5, 2, 18, 20090509L },
                    { 90516, 211L, 90523, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20090516L },
                    { 90523, 212L, 90530, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20090523L },
                    { 90530, 213L, 90606, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20090530L },
                    { 90606, 214L, 90613, 6, 6, 22, 6, 22, 6, 22, 6, 22, 6, 2, 22, 20090606L },
                    { 90613, 215L, 90620, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20090613L },
                    { 90620, 216L, 90627, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20090620L },
                    { 90627, 217L, 90704, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 3, 25, 20090627L },
                    { 90704, 218L, 90711, 7, 7, 26, 7, 26, 7, 26, 7, 26, 7, 3, 26, 20090704L },
                    { 90711, 219L, 90718, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20090711L },
                    { 90718, 220L, 90725, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20090718L },
                    { 90725, 221L, 90801, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20090725L },
                    { 90801, 222L, 90808, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20090801L },
                    { 90808, 223L, 90815, 8, 8, 31, 8, 31, 8, 31, 8, 31, 8, 3, 31, 20090808L },
                    { 90815, 224L, 90822, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20090815L },
                    { 90822, 225L, 90829, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20090822L },
                    { 90829, 226L, 90905, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20090829L },
                    { 90905, 227L, 90912, 9, 9, 35, 9, 35, 9, 35, 9, 35, 9, 3, 35, 20090905L },
                    { 90912, 228L, 90919, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20090912L },
                    { 90919, 229L, 90926, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20090919L },
                    { 90926, 230L, 91003, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 4, 38, 20090926L },
                    { 91003, 231L, 91010, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20091003L },
                    { 91010, 232L, 91017, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20091010L },
                    { 91017, 233L, 91024, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20091017L },
                    { 91024, 234L, 91031, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20091024L },
                    { 91031, 235L, 91107, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20091031L },
                    { 91107, 236L, 91114, 11, 11, 44, 11, 44, 11, 44, 11, 44, 11, 4, 44, 20091107L },
                    { 91114, 237L, 91121, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20091114L },
                    { 91121, 238L, 91128, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20091121L },
                    { 91128, 239L, 91205, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20091128L },
                    { 91205, 240L, 91212, 12, 12, 48, 12, 48, 12, 48, 12, 48, 12, 4, 48, 20091205L },
                    { 91212, 241L, 91219, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20091212L },
                    { 91219, 242L, 91226, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20091219L },
                    { 91226, 243L, 100102, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20091226L },
                    { 100102, 244L, 100109, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20100102L },
                    { 100109, 253L, 100116, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20100109L },
                    { 100116, 254L, 100123, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20100116L },
                    { 100123, 255L, 100130, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20100123L },
                    { 100130, 256L, 100206, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20100130L },
                    { 100206, 257L, 100213, 2, 2, 5, 2, 5, 2, 5, 2, 5, 2, 1, 5, 20100206L },
                    { 100213, 258L, 100220, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20100213L },
                    { 100220, 259L, 100227, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20100220L },
                    { 100227, 260L, 100306, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20100227L },
                    { 100306, 261L, 100313, 3, 3, 9, 3, 9, 3, 9, 3, 9, 3, 1, 9, 20100306L },
                    { 100313, 262L, 100320, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20100313L },
                    { 100320, 263L, 100327, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20100320L },
                    { 100327, 264L, 100403, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 2, 12, 20100327L },
                    { 100403, 265L, 100410, 4, 4, 13, 4, 13, 4, 13, 4, 13, 4, 2, 13, 20100403L },
                    { 100410, 266L, 100417, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20100410L },
                    { 100417, 267L, 100424, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20100417L },
                    { 100424, 268L, 100501, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20100424L },
                    { 100501, 269L, 100508, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20100501L },
                    { 100508, 270L, 100515, 5, 5, 18, 5, 18, 5, 18, 5, 18, 5, 2, 18, 20100508L },
                    { 100515, 271L, 100522, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20100515L },
                    { 100522, 272L, 100529, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20100522L },
                    { 100529, 273L, 100605, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20100529L },
                    { 100605, 274L, 100612, 6, 6, 22, 6, 22, 6, 22, 6, 22, 6, 2, 22, 20100605L },
                    { 100612, 275L, 100619, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20100612L },
                    { 100619, 276L, 100626, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20100619L },
                    { 100626, 277L, 100703, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 3, 25, 20100626L },
                    { 100703, 278L, 100710, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 20100703L },
                    { 100710, 279L, 100717, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20100710L },
                    { 100717, 280L, 100724, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20100717L },
                    { 100724, 281L, 100731, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20100724L },
                    { 100731, 282L, 100807, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20100731L },
                    { 100807, 283L, 100814, 8, 8, 31, 8, 31, 8, 31, 8, 31, 8, 3, 31, 20100807L },
                    { 100814, 284L, 100821, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20100814L },
                    { 100821, 285L, 100828, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20100821L },
                    { 100828, 286L, 100904, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20100828L },
                    { 100904, 287L, 100911, 9, 9, 35, 9, 35, 9, 35, 9, 35, 9, 3, 35, 20100904L },
                    { 100911, 288L, 100918, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20100911L },
                    { 100918, 289L, 100925, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20100918L },
                    { 100925, 290L, 101002, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 20100925L },
                    { 101002, 291L, 101009, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20101002L },
                    { 101009, 292L, 101016, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20101009L },
                    { 101016, 293L, 101023, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20101016L },
                    { 101023, 294L, 101030, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20101023L },
                    { 101030, 295L, 101106, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20101030L },
                    { 101106, 296L, 101113, 11, 11, 44, 11, 44, 11, 44, 11, 44, 11, 4, 44, 20101106L },
                    { 101113, 297L, 101120, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20101113L },
                    { 101120, 298L, 101127, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20101120L },
                    { 101127, 299L, 101204, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20101127L },
                    { 101204, 300L, 101211, 12, 12, 48, 12, 48, 12, 48, 12, 48, 12, 4, 48, 20101204L },
                    { 101211, 301L, 101218, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20101211L },
                    { 101218, 302L, 101225, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20101218L },
                    { 101225, 303L, 110101, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20101225L },
                    { 110101, 304L, 110108, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20110101L },
                    { 110108, 313L, 110115, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20110108L },
                    { 110115, 314L, 110122, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20110115L },
                    { 110122, 315L, 110129, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20110122L },
                    { 110129, 316L, 110205, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20110129L },
                    { 110205, 317L, 110212, 2, 2, 5, 2, 5, 2, 5, 2, 5, 2, 1, 5, 20110205L },
                    { 110212, 318L, 110219, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20110212L },
                    { 110219, 319L, 110226, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20110219L },
                    { 110226, 320L, 110305, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20110226L },
                    { 110305, 321L, 110312, 3, 3, 9, 3, 9, 3, 9, 3, 9, 3, 1, 9, 20110305L },
                    { 110312, 322L, 110319, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20110312L },
                    { 110319, 323L, 110326, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20110319L },
                    { 110326, 324L, 110402, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 1, 12, 20110326L },
                    { 110402, 325L, 110409, 3, 3, 13, 3, 13, 3, 13, 3, 13, 3, 2, 13, 20110402L },
                    { 110409, 326L, 110416, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20110409L },
                    { 110416, 327L, 110423, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20110416L },
                    { 110423, 328L, 110430, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20110423L },
                    { 110430, 329L, 110507, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20110430L },
                    { 110507, 330L, 110514, 5, 5, 18, 5, 18, 5, 18, 5, 18, 5, 2, 18, 20110507L },
                    { 110514, 331L, 110521, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20110514L },
                    { 110521, 332L, 110528, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20110521L },
                    { 110528, 333L, 110604, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20110528L },
                    { 110604, 334L, 110611, 6, 6, 22, 6, 22, 6, 22, 6, 22, 6, 2, 22, 20110604L },
                    { 110611, 335L, 110618, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20110611L },
                    { 110618, 336L, 110625, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20110618L },
                    { 110625, 337L, 110702, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 2, 25, 20110625L },
                    { 110702, 338L, 110709, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 20110702L },
                    { 110709, 339L, 110716, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20110709L },
                    { 110716, 340L, 110723, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20110716L },
                    { 110723, 341L, 110730, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20110723L },
                    { 110730, 342L, 110806, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20110730L },
                    { 110806, 343L, 110813, 8, 8, 31, 8, 31, 8, 31, 8, 31, 8, 3, 31, 20110806L },
                    { 110813, 344L, 110820, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20110813L },
                    { 110820, 345L, 110827, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20110820L },
                    { 110827, 346L, 110903, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20110827L },
                    { 110903, 347L, 110910, 8, 8, 35, 8, 35, 8, 35, 8, 35, 8, 3, 35, 20110903L },
                    { 110910, 348L, 110917, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20110910L },
                    { 110917, 349L, 110924, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20110917L },
                    { 110924, 350L, 111001, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 20110924L },
                    { 111001, 351L, 111008, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20111001L },
                    { 111008, 352L, 111015, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20111008L },
                    { 111015, 353L, 111022, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20111015L },
                    { 111022, 354L, 111029, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20111022L },
                    { 111029, 355L, 111105, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20111029L },
                    { 111105, 356L, 111112, 11, 11, 44, 11, 44, 11, 44, 11, 44, 11, 4, 44, 20111105L },
                    { 111112, 357L, 111119, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20111112L },
                    { 111119, 358L, 111126, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20111119L },
                    { 111126, 359L, 111203, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20111126L },
                    { 111203, 360L, 111210, 11, 11, 48, 11, 48, 11, 48, 11, 48, 11, 4, 48, 20111203L },
                    { 111210, 361L, 111217, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20111210L },
                    { 111217, 362L, 111224, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20111217L },
                    { 111224, 363L, 111231, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20111224L },
                    { 111231, 364L, 120107, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20111231L },
                    { 120107, 373L, 120114, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20120107L },
                    { 120114, 374L, 120121, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20120114L },
                    { 120121, 375L, 120128, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20120121L },
                    { 120128, 376L, 120204, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20120128L },
                    { 120204, 377L, 120211, 2, 2, 5, 2, 5, 2, 5, 2, 5, 2, 1, 5, 20120204L },
                    { 120211, 378L, 120218, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20120211L },
                    { 120218, 379L, 120225, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20120218L },
                    { 120225, 380L, 120303, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20120225L },
                    { 120303, 381L, 120310, 2, 2, 9, 2, 9, 2, 9, 2, 9, 2, 1, 9, 20120303L },
                    { 120310, 382L, 120317, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20120310L },
                    { 120317, 383L, 120324, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20120317L },
                    { 120324, 384L, 120331, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 1, 12, 20120324L },
                    { 120331, 385L, 120407, 3, 3, 13, 3, 13, 3, 13, 3, 13, 3, 2, 13, 20120331L },
                    { 120407, 386L, 120414, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20120407L },
                    { 120414, 387L, 120421, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20120414L },
                    { 120421, 388L, 120428, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20120421L },
                    { 120428, 389L, 120505, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20120428L },
                    { 120505, 390L, 120512, 5, 5, 18, 5, 18, 5, 18, 5, 18, 5, 2, 18, 20120505L },
                    { 120512, 391L, 120519, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20120512L },
                    { 120519, 392L, 120526, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20120519L },
                    { 120526, 393L, 120602, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20120526L },
                    { 120602, 394L, 120609, 5, 5, 22, 5, 22, 5, 22, 5, 22, 5, 2, 22, 20120602L },
                    { 120609, 395L, 120616, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20120609L },
                    { 120616, 396L, 120623, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20120616L },
                    { 120623, 397L, 120630, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 2, 25, 20120623L },
                    { 120630, 398L, 120707, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 20120630L },
                    { 120707, 399L, 120714, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20120707L },
                    { 120714, 400L, 120721, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20120714L },
                    { 120721, 401L, 120728, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20120721L },
                    { 120728, 402L, 120804, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20120728L },
                    { 120804, 403L, 120811, 8, 8, 31, 8, 31, 8, 31, 8, 31, 8, 3, 31, 20120804L },
                    { 120811, 404L, 120818, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20120811L },
                    { 120818, 405L, 120825, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20120818L },
                    { 120825, 406L, 120901, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20120825L },
                    { 120901, 407L, 120908, 8, 8, 35, 8, 35, 8, 35, 8, 35, 8, 3, 35, 20120901L },
                    { 120908, 408L, 120915, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20120908L },
                    { 120915, 409L, 120922, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20120915L },
                    { 120922, 410L, 120929, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 20120922L },
                    { 120929, 411L, 121006, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20120929L },
                    { 121006, 412L, 121013, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20121006L },
                    { 121013, 413L, 121020, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20121013L },
                    { 121020, 414L, 121027, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20121020L },
                    { 121027, 415L, 121103, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20121027L },
                    { 121103, 416L, 121110, 10, 10, 44, 10, 44, 10, 44, 10, 44, 10, 4, 44, 20121103L },
                    { 121110, 417L, 121117, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20121110L },
                    { 121117, 418L, 121124, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20121117L },
                    { 121124, 419L, 121201, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20121124L },
                    { 121201, 420L, 121208, 11, 11, 48, 11, 48, 11, 48, 11, 48, 11, 4, 48, 20121201L },
                    { 121208, 421L, 121215, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20121208L },
                    { 121215, 422L, 121222, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20121215L },
                    { 121222, 423L, 121229, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20121222L },
                    { 121229, 424L, 130105, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20121229L },
                    { 130105, 433L, 130112, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20130105L },
                    { 130112, 434L, 130119, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20130112L },
                    { 130119, 435L, 130126, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20130119L },
                    { 130126, 436L, 130202, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20130126L },
                    { 130202, 437L, 130209, 1, 1, 5, 1, 5, 1, 5, 1, 5, 1, 1, 5, 20130202L },
                    { 130209, 438L, 130216, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20130209L },
                    { 130216, 439L, 130223, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20130216L },
                    { 130223, 440L, 130302, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20130223L },
                    { 130302, 441L, 130309, 2, 2, 9, 2, 9, 2, 9, 2, 9, 2, 1, 9, 20130302L },
                    { 130309, 442L, 130316, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20130309L },
                    { 130316, 443L, 130323, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20130316L },
                    { 130323, 444L, 130330, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 1, 12, 20130323L },
                    { 130330, 445L, 130406, 3, 3, 13, 3, 13, 3, 13, 3, 13, 3, 2, 13, 20130330L },
                    { 130406, 446L, 130413, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20130406L },
                    { 130413, 447L, 130420, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20130413L },
                    { 130420, 448L, 130427, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20130420L },
                    { 130427, 449L, 130504, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20130427L },
                    { 130504, 450L, 130511, 5, 5, 18, 5, 18, 5, 18, 5, 18, 5, 2, 18, 20130504L },
                    { 130511, 451L, 130518, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20130511L },
                    { 130518, 452L, 130525, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20130518L },
                    { 130525, 453L, 130601, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20130525L },
                    { 130601, 454L, 130608, 5, 5, 22, 5, 22, 5, 22, 5, 22, 5, 2, 22, 20130601L },
                    { 130608, 455L, 130615, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20130608L },
                    { 130615, 456L, 130622, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20130615L },
                    { 130622, 457L, 130629, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 2, 25, 20130622L },
                    { 130629, 458L, 130706, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 20130629L },
                    { 130706, 459L, 130713, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20130706L },
                    { 130713, 460L, 130720, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20130713L },
                    { 130720, 461L, 130727, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20130720L },
                    { 130727, 462L, 130803, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20130727L },
                    { 130803, 463L, 130810, 7, 7, 31, 7, 31, 7, 31, 7, 31, 7, 3, 31, 20130803L },
                    { 130810, 464L, 130817, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20130810L },
                    { 130817, 465L, 130824, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20130817L },
                    { 130824, 466L, 130831, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20130824L },
                    { 130831, 467L, 130907, 8, 8, 35, 8, 35, 8, 35, 8, 35, 8, 3, 35, 20130831L },
                    { 130907, 468L, 130914, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20130907L },
                    { 130914, 469L, 130921, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20130914L },
                    { 130921, 470L, 130928, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 20130921L },
                    { 130928, 471L, 131005, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20130928L },
                    { 131005, 472L, 131012, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20131005L },
                    { 131012, 473L, 131019, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20131012L },
                    { 131019, 474L, 131026, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20131019L },
                    { 131026, 475L, 131102, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20131026L },
                    { 131102, 476L, 131109, 10, 10, 44, 10, 44, 10, 44, 10, 44, 10, 4, 44, 20131102L },
                    { 131109, 477L, 131116, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20131109L },
                    { 131116, 478L, 131123, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20131116L },
                    { 131123, 479L, 131130, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20131123L },
                    { 131130, 480L, 131207, 11, 11, 48, 11, 48, 11, 48, 11, 48, 11, 4, 48, 20131130L },
                    { 131207, 481L, 131214, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20131207L },
                    { 131214, 482L, 131221, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20131214L },
                    { 131221, 483L, 131228, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20131221L },
                    { 131228, 484L, 140104, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20131228L },
                    { 140104, 493L, 140111, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20140104L },
                    { 140111, 494L, 140118, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20140111L },
                    { 140118, 495L, 140125, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20140118L },
                    { 140125, 496L, 140201, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20140125L },
                    { 140201, 497L, 140208, 1, 1, 5, 1, 5, 1, 5, 1, 5, 1, 1, 5, 20140201L },
                    { 140208, 498L, 140215, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20140208L },
                    { 140215, 499L, 140222, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20140215L },
                    { 140222, 500L, 140301, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20140222L },
                    { 140301, 501L, 140308, 2, 2, 9, 2, 9, 2, 9, 2, 9, 2, 1, 9, 20140301L },
                    { 140308, 502L, 140315, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20140308L },
                    { 140315, 503L, 140322, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20140315L },
                    { 140322, 504L, 140329, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 1, 12, 20140322L },
                    { 140329, 505L, 140405, 3, 3, 13, 3, 13, 3, 13, 3, 13, 3, 2, 13, 20140329L },
                    { 140405, 506L, 140412, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20140405L },
                    { 140412, 507L, 140419, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20140412L },
                    { 140419, 508L, 140426, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20140419L },
                    { 140426, 509L, 140503, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20140426L },
                    { 140503, 510L, 140510, 4, 4, 18, 4, 18, 4, 18, 4, 18, 4, 2, 18, 20140503L },
                    { 140510, 511L, 140517, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20140510L },
                    { 140517, 512L, 140524, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20140517L },
                    { 140524, 513L, 140531, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20140524L },
                    { 140531, 514L, 140607, 5, 5, 22, 5, 22, 5, 22, 5, 22, 5, 2, 22, 20140531L },
                    { 140607, 515L, 140614, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20140607L },
                    { 140614, 516L, 140621, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20140614L },
                    { 140621, 517L, 140628, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 2, 25, 20140621L },
                    { 140628, 518L, 140705, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 20140628L },
                    { 140705, 519L, 140712, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20140705L },
                    { 140712, 520L, 140719, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20140712L },
                    { 140719, 521L, 140726, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20140719L },
                    { 140726, 522L, 140802, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20140726L },
                    { 140802, 523L, 140809, 7, 7, 31, 7, 31, 7, 31, 7, 31, 7, 3, 31, 20140802L },
                    { 140809, 524L, 140816, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20140809L },
                    { 140816, 525L, 140823, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20140816L },
                    { 140823, 526L, 140830, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20140823L },
                    { 140830, 527L, 140906, 8, 8, 35, 8, 35, 8, 35, 8, 35, 8, 3, 35, 20140830L },
                    { 140906, 528L, 140913, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20140906L },
                    { 140913, 529L, 140920, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20140913L },
                    { 140920, 530L, 140927, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 20140920L },
                    { 140927, 531L, 141004, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20140927L },
                    { 141004, 532L, 141011, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20141004L },
                    { 141011, 533L, 141018, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20141011L },
                    { 141018, 534L, 141025, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20141018L },
                    { 141025, 535L, 141101, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20141025L },
                    { 141101, 536L, 141108, 10, 10, 44, 10, 44, 10, 44, 10, 44, 10, 4, 44, 20141101L },
                    { 141108, 537L, 141115, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20141108L },
                    { 141115, 538L, 141122, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20141115L },
                    { 141122, 539L, 141129, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20141122L },
                    { 141129, 540L, 141206, 11, 11, 48, 11, 48, 11, 48, 11, 48, 11, 4, 48, 20141129L },
                    { 141206, 541L, 141213, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20141206L },
                    { 141213, 542L, 141220, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20141213L },
                    { 141220, 543L, 141227, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20141220L },
                    { 141227, 544L, 150103, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20141227L },
                    { 150103, 545L, 150110, 12, 12, 53, 12, 53, 12, 53, 12, 53, 12, 1, 53, 20150103L },
                    { 150110, 553L, 150117, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20150110L },
                    { 150117, 554L, 150124, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20150117L },
                    { 150124, 555L, 150131, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20150124L },
                    { 150131, 556L, 150207, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20150131L },
                    { 150207, 557L, 150214, 2, 2, 5, 2, 5, 2, 5, 2, 5, 2, 1, 5, 20150207L },
                    { 150214, 558L, 150221, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20150214L },
                    { 150221, 559L, 150228, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20150221L },
                    { 150228, 560L, 150307, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20150228L },
                    { 150307, 561L, 150314, 3, 3, 9, 3, 9, 3, 9, 3, 9, 3, 1, 9, 20150307L },
                    { 150314, 562L, 150321, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20150314L },
                    { 150321, 563L, 150328, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20150321L },
                    { 150328, 564L, 150404, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 2, 12, 20150328L },
                    { 150404, 565L, 150411, 4, 4, 13, 4, 13, 4, 13, 4, 13, 4, 2, 13, 20150404L },
                    { 150411, 566L, 150418, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20150411L },
                    { 150418, 567L, 150425, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20150418L },
                    { 150425, 568L, 150502, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20150425L },
                    { 150502, 569L, 150509, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20150502L },
                    { 150509, 570L, 150516, 5, 5, 18, 5, 18, 5, 18, 5, 18, 5, 2, 18, 20150509L },
                    { 150516, 571L, 150523, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20150516L },
                    { 150523, 572L, 150530, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20150523L },
                    { 150530, 573L, 150606, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20150530L },
                    { 150606, 574L, 150613, 6, 6, 22, 6, 22, 6, 22, 6, 22, 6, 2, 22, 20150606L },
                    { 150613, 575L, 150620, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20150613L },
                    { 150620, 576L, 150627, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20150620L },
                    { 150627, 577L, 150704, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 3, 25, 20150627L },
                    { 150704, 578L, 150711, 7, 7, 26, 7, 26, 7, 26, 7, 26, 7, 3, 26, 20150704L },
                    { 150711, 579L, 150718, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20150711L },
                    { 150718, 580L, 150725, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20150718L },
                    { 150725, 581L, 150801, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20150725L },
                    { 150801, 582L, 150808, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20150801L },
                    { 150808, 583L, 150815, 8, 8, 31, 8, 31, 8, 31, 8, 31, 8, 3, 31, 20150808L },
                    { 150815, 584L, 150822, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20150815L },
                    { 150822, 585L, 150829, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20150822L },
                    { 150829, 586L, 150905, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20150829L },
                    { 150905, 587L, 150912, 9, 9, 35, 9, 35, 9, 35, 9, 35, 9, 3, 35, 20150905L },
                    { 150912, 588L, 150919, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20150912L },
                    { 150919, 589L, 150926, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20150919L },
                    { 150926, 590L, 151003, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 4, 38, 20150926L },
                    { 151003, 591L, 151010, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20151003L },
                    { 151010, 592L, 151017, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20151010L },
                    { 151017, 593L, 151024, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20151017L },
                    { 151024, 594L, 151031, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20151024L },
                    { 151031, 595L, 151107, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20151031L },
                    { 151107, 596L, 151114, 11, 11, 44, 11, 44, 11, 44, 11, 44, 11, 4, 44, 20151107L },
                    { 151114, 597L, 151121, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20151114L },
                    { 151121, 598L, 151128, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20151121L },
                    { 151128, 599L, 151205, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20151128L },
                    { 151205, 600L, 151212, 12, 12, 48, 12, 48, 12, 48, 12, 48, 12, 4, 48, 20151205L },
                    { 151212, 601L, 151219, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20151212L },
                    { 151219, 602L, 151226, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20151219L },
                    { 151226, 603L, 160102, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20151226L },
                    { 160102, 604L, 160109, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20160102L },
                    { 160109, 613L, 160116, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20160109L },
                    { 160116, 614L, 160123, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20160116L },
                    { 160123, 615L, 160130, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20160123L },
                    { 160130, 616L, 160206, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20160130L },
                    { 160206, 617L, 160213, 2, 2, 5, 2, 5, 2, 5, 2, 5, 2, 1, 5, 20160206L },
                    { 160213, 618L, 160220, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20160213L },
                    { 160220, 619L, 160227, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20160220L },
                    { 160227, 620L, 160305, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20160227L },
                    { 160305, 621L, 160312, 3, 3, 9, 3, 9, 3, 9, 3, 9, 3, 1, 9, 20160305L },
                    { 160312, 622L, 160319, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20160312L },
                    { 160319, 623L, 160326, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20160319L },
                    { 160326, 624L, 160402, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 1, 12, 20160326L },
                    { 160402, 625L, 160409, 3, 3, 13, 3, 13, 3, 13, 3, 13, 3, 2, 13, 20160402L },
                    { 160409, 626L, 160416, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20160409L },
                    { 160416, 627L, 160423, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20160416L },
                    { 160423, 628L, 160430, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20160423L },
                    { 160430, 629L, 160507, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20160430L },
                    { 160507, 630L, 160514, 5, 5, 18, 5, 18, 5, 18, 5, 18, 5, 2, 18, 20160507L },
                    { 160514, 631L, 160521, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20160514L },
                    { 160521, 632L, 160528, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20160521L },
                    { 160528, 633L, 160604, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20160528L },
                    { 160604, 634L, 160611, 6, 6, 22, 6, 22, 6, 22, 6, 22, 6, 2, 22, 20160604L },
                    { 160611, 635L, 160618, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20160611L },
                    { 160618, 636L, 160625, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20160618L },
                    { 160625, 637L, 160702, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 2, 25, 20160625L },
                    { 160702, 638L, 160709, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 20160702L },
                    { 160709, 639L, 160716, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20160709L },
                    { 160716, 640L, 160723, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20160716L },
                    { 160723, 641L, 160730, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20160723L },
                    { 160730, 642L, 160806, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20160730L },
                    { 160806, 643L, 160813, 8, 8, 31, 8, 31, 8, 31, 8, 31, 8, 3, 31, 20160806L },
                    { 160813, 644L, 160820, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20160813L },
                    { 160820, 645L, 160827, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20160820L },
                    { 160827, 646L, 160903, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20160827L },
                    { 160903, 647L, 160910, 8, 8, 35, 8, 35, 8, 35, 8, 35, 8, 3, 35, 20160903L },
                    { 160910, 648L, 160917, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20160910L },
                    { 160917, 649L, 160924, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20160917L },
                    { 160924, 650L, 161001, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 20160924L },
                    { 161001, 651L, 161008, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20161001L },
                    { 161008, 652L, 161015, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20161008L },
                    { 161015, 653L, 161022, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20161015L },
                    { 161022, 654L, 161029, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20161022L },
                    { 161029, 655L, 161105, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20161029L },
                    { 161105, 656L, 161112, 11, 11, 44, 11, 44, 11, 44, 11, 44, 11, 4, 44, 20161105L },
                    { 161112, 657L, 161119, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20161112L },
                    { 161119, 658L, 161126, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20161119L },
                    { 161126, 659L, 161203, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20161126L },
                    { 161203, 660L, 161210, 11, 11, 48, 11, 48, 11, 48, 11, 48, 11, 4, 48, 20161203L },
                    { 161210, 661L, 161217, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20161210L },
                    { 161217, 662L, 161224, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20161217L },
                    { 161224, 663L, 161231, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20161224L },
                    { 161231, 664L, 170107, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20161231L },
                    { 170107, 673L, 170114, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20170107L },
                    { 170114, 674L, 170121, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20170114L },
                    { 170121, 675L, 170128, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20170121L },
                    { 170128, 676L, 170204, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20170128L },
                    { 170204, 677L, 170211, 2, 2, 5, 2, 5, 2, 5, 2, 5, 2, 1, 5, 20170204L },
                    { 170211, 678L, 170218, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20170211L },
                    { 170218, 679L, 170225, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20170218L },
                    { 170225, 680L, 170304, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20170225L },
                    { 170304, 681L, 170311, 3, 3, 9, 3, 9, 3, 9, 3, 9, 3, 1, 9, 20170304L },
                    { 170311, 682L, 170318, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20170311L },
                    { 170318, 683L, 170325, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20170318L },
                    { 170325, 684L, 170401, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 1, 12, 20170325L },
                    { 170401, 685L, 170408, 3, 3, 13, 3, 13, 3, 13, 3, 13, 3, 2, 13, 20170401L },
                    { 170408, 686L, 170415, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20170408L },
                    { 170415, 687L, 170422, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20170415L },
                    { 170422, 688L, 170429, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20170422L },
                    { 170429, 689L, 170506, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20170429L },
                    { 170506, 690L, 170513, 5, 5, 18, 5, 18, 5, 18, 5, 18, 5, 2, 18, 20170506L },
                    { 170513, 691L, 170520, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20170513L },
                    { 170520, 692L, 170527, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20170520L },
                    { 170527, 693L, 170603, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20170527L },
                    { 170603, 694L, 170610, 5, 5, 22, 5, 22, 5, 22, 5, 22, 5, 2, 22, 20170603L },
                    { 170610, 695L, 170617, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20170610L },
                    { 170617, 696L, 170624, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20170617L },
                    { 170624, 697L, 170701, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 2, 25, 20170624L },
                    { 170701, 698L, 170708, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 20170701L },
                    { 170708, 699L, 170715, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20170708L },
                    { 170715, 700L, 170722, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20170715L },
                    { 170722, 701L, 170729, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20170722L },
                    { 170729, 702L, 170805, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20170729L },
                    { 170805, 703L, 170812, 8, 8, 31, 8, 31, 8, 31, 8, 31, 8, 3, 31, 20170805L },
                    { 170812, 704L, 170819, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20170812L },
                    { 170819, 705L, 170826, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20170819L },
                    { 170826, 706L, 170902, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20170826L },
                    { 170902, 707L, 170909, 8, 8, 35, 8, 35, 8, 35, 8, 35, 8, 3, 35, 20170902L },
                    { 170909, 708L, 170916, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20170909L },
                    { 170916, 709L, 170923, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20170916L },
                    { 170923, 710L, 170930, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 20170923L },
                    { 170930, 711L, 171007, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20170930L },
                    { 171007, 712L, 171014, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20171007L },
                    { 171014, 713L, 171021, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20171014L },
                    { 171021, 714L, 171028, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20171021L },
                    { 171028, 715L, 171104, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20171028L },
                    { 171104, 716L, 171111, 11, 11, 44, 11, 44, 11, 44, 11, 44, 11, 4, 44, 20171104L },
                    { 171111, 717L, 171118, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20171111L },
                    { 171118, 718L, 171125, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20171118L },
                    { 171125, 719L, 171202, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20171125L },
                    { 171202, 720L, 171209, 11, 11, 48, 11, 48, 11, 48, 11, 48, 11, 4, 48, 20171202L },
                    { 171209, 721L, 171216, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20171209L },
                    { 171216, 722L, 171223, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20171216L },
                    { 171223, 723L, 171230, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20171223L },
                    { 171230, 724L, 180106, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20171230L },
                    { 180106, 733L, 180113, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20180106L },
                    { 180113, 734L, 180120, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20180113L },
                    { 180120, 735L, 180127, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20180120L },
                    { 180127, 736L, 180203, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20180127L },
                    { 180203, 737L, 180210, 1, 1, 5, 1, 5, 1, 5, 1, 5, 1, 1, 5, 20180203L },
                    { 180210, 738L, 180217, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20180210L },
                    { 180217, 739L, 180224, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20180217L },
                    { 180224, 740L, 180303, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20180224L },
                    { 180303, 741L, 180310, 2, 2, 9, 2, 9, 2, 9, 2, 9, 2, 1, 9, 20180303L },
                    { 180310, 742L, 180317, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20180310L },
                    { 180317, 743L, 180324, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20180317L },
                    { 180324, 744L, 180331, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 1, 12, 20180324L },
                    { 180331, 745L, 180407, 3, 3, 13, 3, 13, 3, 13, 3, 13, 3, 2, 13, 20180331L },
                    { 180407, 746L, 180414, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20180407L },
                    { 180414, 747L, 180421, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20180414L },
                    { 180421, 748L, 180428, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20180421L },
                    { 180428, 749L, 180505, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20180428L },
                    { 180505, 750L, 180512, 5, 5, 18, 5, 18, 5, 18, 5, 18, 5, 2, 18, 20180505L },
                    { 180512, 751L, 180519, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20180512L },
                    { 180519, 752L, 180526, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20180519L },
                    { 180526, 753L, 180602, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20180526L },
                    { 180602, 754L, 180609, 5, 5, 22, 5, 22, 5, 22, 5, 22, 5, 2, 22, 20180602L },
                    { 180609, 755L, 180616, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20180609L },
                    { 180616, 756L, 180623, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20180616L },
                    { 180623, 757L, 180630, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 2, 25, 20180623L },
                    { 180630, 758L, 180707, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 20180630L },
                    { 180707, 759L, 180714, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20180707L },
                    { 180714, 760L, 180721, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20180714L },
                    { 180721, 761L, 180728, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20180721L },
                    { 180728, 762L, 180804, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20180728L },
                    { 180804, 763L, 180811, 8, 8, 31, 8, 31, 8, 31, 8, 31, 8, 3, 31, 20180804L },
                    { 180811, 764L, 180818, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20180811L },
                    { 180818, 765L, 180825, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20180818L },
                    { 180825, 766L, 180901, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20180825L },
                    { 180901, 767L, 180908, 8, 8, 35, 8, 35, 8, 35, 8, 35, 8, 3, 35, 20180901L },
                    { 180908, 768L, 180915, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20180908L },
                    { 180915, 769L, 180922, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20180915L },
                    { 180922, 770L, 180929, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 20180922L },
                    { 180929, 771L, 181006, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20180929L },
                    { 181006, 772L, 181013, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20181006L },
                    { 181013, 773L, 181020, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20181013L },
                    { 181020, 774L, 181027, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20181020L },
                    { 181027, 775L, 181103, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20181027L },
                    { 181103, 776L, 181110, 10, 10, 44, 10, 44, 10, 44, 10, 44, 10, 4, 44, 20181103L },
                    { 181110, 777L, 181117, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20181110L },
                    { 181117, 778L, 181124, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20181117L },
                    { 181124, 779L, 181201, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20181124L },
                    { 181201, 780L, 181208, 11, 11, 48, 11, 48, 11, 48, 11, 48, 11, 4, 48, 20181201L },
                    { 181208, 781L, 181215, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20181208L },
                    { 181215, 782L, 181222, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20181215L },
                    { 181222, 783L, 181229, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20181222L },
                    { 181229, 784L, 190105, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20181229L },
                    { 190105, 793L, 190112, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20190105L },
                    { 190112, 794L, 190119, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20190112L },
                    { 190119, 795L, 190126, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20190119L },
                    { 190126, 796L, 190202, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20190126L },
                    { 190202, 797L, 190209, 1, 1, 5, 1, 5, 1, 5, 1, 5, 1, 1, 5, 20190202L },
                    { 190209, 798L, 190216, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20190209L },
                    { 190216, 799L, 190223, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20190216L },
                    { 190223, 800L, 190302, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20190223L },
                    { 190302, 801L, 190309, 2, 2, 9, 2, 9, 2, 9, 2, 9, 2, 1, 9, 20190302L },
                    { 190309, 802L, 190316, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20190309L },
                    { 190316, 803L, 190323, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20190316L },
                    { 190323, 804L, 190330, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 1, 12, 20190323L },
                    { 190330, 805L, 190406, 3, 3, 13, 3, 13, 3, 13, 3, 13, 3, 2, 13, 20190330L },
                    { 190406, 806L, 190413, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20190406L },
                    { 190413, 807L, 190420, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20190413L },
                    { 190420, 808L, 190427, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20190420L },
                    { 190427, 809L, 190504, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20190427L },
                    { 190504, 810L, 190511, 5, 5, 18, 5, 18, 5, 18, 5, 18, 5, 2, 18, 20190504L },
                    { 190511, 811L, 190518, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20190511L },
                    { 190518, 812L, 190525, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20190518L },
                    { 190525, 813L, 190601, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20190525L },
                    { 190601, 814L, 190608, 5, 5, 22, 5, 22, 5, 22, 5, 22, 5, 2, 22, 20190601L },
                    { 190608, 815L, 190615, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20190608L },
                    { 190615, 816L, 190622, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20190615L },
                    { 190622, 817L, 190629, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 2, 25, 20190622L },
                    { 190629, 818L, 190706, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 20190629L },
                    { 190706, 819L, 190713, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20190706L },
                    { 190713, 820L, 190720, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20190713L },
                    { 190720, 821L, 190727, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20190720L },
                    { 190727, 822L, 190803, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20190727L },
                    { 190803, 823L, 190810, 7, 7, 31, 7, 31, 7, 31, 7, 31, 7, 3, 31, 20190803L },
                    { 190810, 824L, 190817, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20190810L },
                    { 190817, 825L, 190824, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20190817L },
                    { 190824, 826L, 190831, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20190824L },
                    { 190831, 827L, 190907, 8, 8, 35, 8, 35, 8, 35, 8, 35, 8, 3, 35, 20190831L },
                    { 190907, 828L, 190914, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20190907L },
                    { 190914, 829L, 190921, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20190914L },
                    { 190921, 830L, 190928, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 20190921L },
                    { 190928, 831L, 191005, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20190928L },
                    { 191005, 832L, 191012, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20191005L },
                    { 191012, 833L, 191019, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20191012L },
                    { 191019, 834L, 191026, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20191019L },
                    { 191026, 835L, 191102, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20191026L },
                    { 191102, 836L, 191109, 10, 10, 44, 10, 44, 10, 44, 10, 44, 10, 4, 44, 20191102L },
                    { 191109, 837L, 191116, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20191109L },
                    { 191116, 838L, 191123, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20191116L },
                    { 191123, 839L, 191130, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20191123L },
                    { 191130, 840L, 191207, 11, 11, 48, 11, 48, 11, 48, 11, 48, 11, 4, 48, 20191130L },
                    { 191207, 841L, 191214, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20191207L },
                    { 191214, 842L, 191221, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20191214L },
                    { 191221, 843L, 191228, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20191221L },
                    { 191228, 844L, 200104, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20191228L },
                    { 200104, 853L, 200111, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20200104L },
                    { 200111, 854L, 200118, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20200111L },
                    { 200118, 855L, 200125, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20200118L },
                    { 200125, 856L, 200201, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20200125L },
                    { 200201, 857L, 200208, 1, 1, 5, 1, 5, 1, 5, 1, 5, 1, 1, 5, 20200201L },
                    { 200208, 858L, 200215, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20200208L },
                    { 200215, 859L, 200222, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20200215L },
                    { 200222, 860L, 200229, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20200222L },
                    { 200229, 861L, 200307, 2, 2, 9, 2, 9, 2, 9, 2, 9, 2, 1, 9, 20200229L },
                    { 200307, 862L, 200314, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20200307L },
                    { 200314, 863L, 200321, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20200314L },
                    { 200321, 864L, 200328, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 1, 12, 20200321L },
                    { 200328, 865L, 200404, 3, 3, 13, 3, 13, 3, 13, 3, 13, 3, 2, 13, 20200328L },
                    { 200404, 866L, 200411, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20200404L },
                    { 200411, 867L, 200418, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20200411L },
                    { 200418, 868L, 200425, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20200418L },
                    { 200425, 869L, 200502, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20200425L },
                    { 200502, 870L, 200509, 4, 4, 18, 4, 18, 4, 18, 4, 18, 4, 2, 18, 20200502L },
                    { 200509, 871L, 200516, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20200509L },
                    { 200516, 872L, 200523, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20200516L },
                    { 200523, 873L, 200530, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20200523L },
                    { 200530, 874L, 200606, 5, 5, 22, 5, 22, 5, 22, 5, 22, 5, 2, 22, 20200530L },
                    { 200606, 875L, 200613, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20200606L },
                    { 200613, 876L, 200620, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20200613L },
                    { 200620, 877L, 200627, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 2, 25, 20200620L },
                    { 200627, 878L, 200704, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 20200627L },
                    { 200704, 879L, 200711, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20200704L },
                    { 200711, 880L, 200718, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20200711L },
                    { 200718, 881L, 200725, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20200718L },
                    { 200725, 882L, 200801, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20200725L },
                    { 200801, 883L, 200808, 7, 7, 31, 7, 31, 7, 31, 7, 31, 7, 3, 31, 20200801L },
                    { 200808, 884L, 200815, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20200808L },
                    { 200815, 885L, 200822, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20200815L },
                    { 200822, 886L, 200829, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20200822L },
                    { 200829, 887L, 200905, 8, 8, 35, 8, 35, 8, 35, 8, 35, 8, 3, 35, 20200829L },
                    { 200905, 888L, 200912, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20200905L },
                    { 200912, 889L, 200919, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20200912L },
                    { 200919, 890L, 200926, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 20200919L },
                    { 200926, 891L, 201003, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20200926L },
                    { 201003, 892L, 201010, 9, 9, 40, 9, 40, 9, 40, 9, 40, 9, 4, 40, 20201003L },
                    { 201010, 893L, 201017, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20201010L },
                    { 201017, 894L, 201024, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20201017L },
                    { 201024, 895L, 201031, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20201024L },
                    { 201031, 896L, 201107, 10, 10, 44, 10, 44, 10, 44, 10, 44, 10, 4, 44, 20201031L },
                    { 201107, 897L, 201114, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20201107L },
                    { 201114, 898L, 201121, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20201114L },
                    { 201121, 899L, 201128, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20201121L },
                    { 201128, 900L, 201205, 11, 11, 48, 11, 48, 11, 48, 11, 48, 11, 4, 48, 20201128L },
                    { 201205, 901L, 201212, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20201205L },
                    { 201212, 902L, 201219, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20201212L },
                    { 201219, 903L, 201226, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20201219L },
                    { 201226, 904L, 210102, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 4, 52, 20201226L },
                    { 210102, 905L, 210109, 12, 12, 53, 12, 53, 12, 53, 12, 53, 12, 1, 53, 20210102L },
                    { 210109, 913L, 210116, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20210109L },
                    { 210116, 914L, 210123, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20210116L },
                    { 210123, 915L, 210130, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20210123L },
                    { 210130, 916L, 210206, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20210130L },
                    { 210206, 917L, 210213, 2, 2, 5, 2, 5, 2, 5, 2, 5, 2, 1, 5, 20210206L },
                    { 210213, 918L, 210220, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20210213L },
                    { 210220, 919L, 210227, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20210220L },
                    { 210227, 920L, 210306, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20210227L },
                    { 210306, 921L, 210313, 3, 3, 9, 3, 9, 3, 9, 3, 9, 3, 1, 9, 20210306L },
                    { 210313, 922L, 210320, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20210313L },
                    { 210320, 923L, 210327, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20210320L },
                    { 210327, 924L, 210403, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 2, 12, 20210327L },
                    { 210403, 925L, 210410, 3, 3, 13, 3, 13, 3, 13, 3, 13, 3, 2, 13, 20210403L },
                    { 210410, 926L, 210417, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20210410L },
                    { 210417, 927L, 210424, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20210417L },
                    { 210424, 928L, 210501, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20210424L },
                    { 210501, 929L, 210508, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20210501L },
                    { 210508, 930L, 210515, 5, 5, 18, 5, 18, 5, 18, 5, 18, 5, 2, 18, 20210508L },
                    { 210515, 931L, 210522, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20210515L },
                    { 210522, 932L, 210529, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20210522L },
                    { 210529, 933L, 210605, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20210529L },
                    { 210605, 934L, 210612, 6, 6, 22, 6, 22, 6, 22, 6, 22, 6, 2, 22, 20210605L },
                    { 210612, 935L, 210619, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20210612L },
                    { 210619, 936L, 210626, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20210619L },
                    { 210626, 937L, 210703, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 3, 25, 20210626L },
                    { 210703, 938L, 210710, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 20210703L },
                    { 210710, 939L, 210717, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20210710L },
                    { 210717, 940L, 210724, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20210717L },
                    { 210724, 941L, 210731, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20210724L },
                    { 210731, 942L, 210807, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20210731L },
                    { 210807, 943L, 210814, 8, 8, 31, 8, 31, 8, 31, 8, 31, 8, 3, 31, 20210807L },
                    { 210814, 944L, 210821, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20210814L },
                    { 210821, 945L, 210828, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20210821L },
                    { 210828, 946L, 210904, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20210828L },
                    { 210904, 947L, 210911, 9, 9, 35, 9, 35, 9, 35, 9, 35, 9, 3, 35, 20210904L },
                    { 210911, 948L, 210918, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20210911L },
                    { 210918, 949L, 210925, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20210918L },
                    { 210925, 950L, 211002, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 20210925L },
                    { 211002, 951L, 211009, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20211002L },
                    { 211009, 952L, 211016, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20211009L },
                    { 211016, 953L, 211023, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20211016L },
                    { 211023, 954L, 211030, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20211023L },
                    { 211030, 955L, 211106, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20211030L },
                    { 211106, 956L, 211113, 11, 11, 44, 11, 44, 11, 44, 11, 44, 11, 4, 44, 20211106L },
                    { 211113, 957L, 211120, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20211113L },
                    { 211120, 958L, 211127, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20211120L },
                    { 211127, 959L, 211204, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20211127L },
                    { 211204, 960L, 211211, 12, 12, 48, 12, 48, 12, 48, 12, 48, 12, 4, 48, 20211204L },
                    { 211211, 961L, 211218, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20211211L },
                    { 211218, 962L, 211225, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20211218L },
                    { 211225, 963L, 220101, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20211225L },
                    { 220101, 964L, 220108, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20220101L },
                    { 220108, 973L, 220115, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20220108L },
                    { 220115, 974L, 220122, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20220115L },
                    { 220122, 975L, 220129, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20220122L },
                    { 220129, 976L, 220205, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20220129L },
                    { 220205, 977L, 220212, 2, 2, 5, 2, 5, 2, 5, 2, 5, 2, 1, 5, 20220205L },
                    { 220212, 978L, 220219, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20220212L },
                    { 220219, 979L, 220226, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20220219L },
                    { 220226, 980L, 220305, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20220226L },
                    { 220305, 981L, 220312, 3, 3, 9, 3, 9, 3, 9, 3, 9, 3, 1, 9, 20220305L },
                    { 220312, 982L, 220319, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20220312L },
                    { 220319, 983L, 220326, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20220319L },
                    { 220326, 984L, 220402, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 1, 12, 20220326L },
                    { 220402, 985L, 220409, 3, 3, 13, 3, 13, 3, 13, 3, 13, 3, 2, 13, 20220402L },
                    { 220409, 986L, 220416, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20220409L },
                    { 220416, 987L, 220423, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20220416L },
                    { 220423, 988L, 220430, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20220423L },
                    { 220430, 989L, 220507, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20220430L },
                    { 220507, 990L, 220514, 5, 5, 18, 5, 18, 5, 18, 5, 18, 5, 2, 18, 20220507L },
                    { 220514, 991L, 220521, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20220514L },
                    { 220521, 992L, 220528, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20220521L },
                    { 220528, 993L, 220604, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20220528L },
                    { 220604, 994L, 220611, 6, 6, 22, 6, 22, 6, 22, 6, 22, 6, 2, 22, 20220604L },
                    { 220611, 995L, 220618, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20220611L },
                    { 220618, 996L, 220625, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20220618L },
                    { 220625, 997L, 220702, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 2, 25, 20220625L },
                    { 220702, 998L, 220709, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 20220702L },
                    { 220709, 999L, 220716, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20220709L },
                    { 220716, 1000L, 220723, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20220716L },
                    { 220723, 1001L, 220730, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20220723L },
                    { 220730, 1002L, 220806, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20220730L },
                    { 220806, 1003L, 220813, 8, 8, 31, 8, 31, 8, 31, 8, 31, 8, 3, 31, 20220806L },
                    { 220813, 1004L, 220820, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20220813L },
                    { 220820, 1005L, 220827, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20220820L },
                    { 220827, 1006L, 220903, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20220827L },
                    { 220903, 1007L, 220910, 8, 8, 35, 8, 35, 8, 35, 8, 35, 8, 3, 35, 20220903L },
                    { 220910, 1008L, 220917, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20220910L },
                    { 220917, 1009L, 220924, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20220917L },
                    { 220924, 1010L, 221001, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 20220924L },
                    { 221001, 1011L, 221008, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20221001L },
                    { 221008, 1012L, 221015, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20221008L },
                    { 221015, 1013L, 221022, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20221015L },
                    { 221022, 1014L, 221029, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20221022L },
                    { 221029, 1015L, 221105, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20221029L },
                    { 221105, 1016L, 221112, 11, 11, 44, 11, 44, 11, 44, 11, 44, 11, 4, 44, 20221105L },
                    { 221112, 1017L, 221119, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20221112L },
                    { 221119, 1018L, 221126, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20221119L },
                    { 221126, 1019L, 221203, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20221126L },
                    { 221203, 1020L, 221210, 11, 11, 48, 11, 48, 11, 48, 11, 48, 11, 4, 48, 20221203L },
                    { 221210, 1021L, 221217, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20221210L },
                    { 221217, 1022L, 221224, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20221217L },
                    { 221224, 1023L, 221231, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20221224L },
                    { 221231, 1024L, 230107, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20221231L },
                    { 230107, 1033L, 230114, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20230107L },
                    { 230114, 1034L, 230121, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20230114L },
                    { 230121, 1035L, 230128, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20230121L },
                    { 230128, 1036L, 230204, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20230128L },
                    { 230204, 1037L, 230211, 2, 2, 5, 2, 5, 2, 5, 2, 5, 2, 1, 5, 20230204L },
                    { 230211, 1038L, 230218, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20230211L },
                    { 230218, 1039L, 230225, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20230218L },
                    { 230225, 1040L, 230304, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20230225L },
                    { 230304, 1041L, 230311, 3, 3, 9, 3, 9, 3, 9, 3, 9, 3, 1, 9, 20230304L },
                    { 230311, 1042L, 230318, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20230311L },
                    { 230318, 1043L, 230325, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20230318L },
                    { 230325, 1044L, 230401, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 1, 12, 20230325L },
                    { 230401, 1045L, 230408, 3, 3, 13, 3, 13, 3, 13, 3, 13, 3, 2, 13, 20230401L },
                    { 230408, 1046L, 230415, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20230408L },
                    { 230415, 1047L, 230422, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20230415L },
                    { 230422, 1048L, 230429, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20230422L },
                    { 230429, 1049L, 230506, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20230429L },
                    { 230506, 1050L, 230513, 5, 5, 18, 5, 18, 5, 18, 5, 18, 5, 2, 18, 20230506L },
                    { 230513, 1051L, 230520, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20230513L },
                    { 230520, 1052L, 230527, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20230520L },
                    { 230527, 1053L, 230603, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20230527L },
                    { 230603, 1054L, 230610, 5, 5, 22, 5, 22, 5, 22, 5, 22, 5, 2, 22, 20230603L },
                    { 230610, 1055L, 230617, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20230610L },
                    { 230617, 1056L, 230624, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20230617L },
                    { 230624, 1057L, 230701, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 2, 25, 20230624L },
                    { 230701, 1058L, 230708, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 20230701L },
                    { 230708, 1059L, 230715, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20230708L },
                    { 230715, 1060L, 230722, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 20230715L },
                    { 230722, 1061L, 230729, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 20230722L },
                    { 230729, 1062L, 230805, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20230729L },
                    { 230805, 1063L, 230812, 8, 8, 31, 8, 31, 8, 31, 8, 31, 8, 3, 31, 20230805L },
                    { 230812, 1064L, 230819, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20230812L },
                    { 230819, 1065L, 230826, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20230819L },
                    { 230826, 1066L, 230902, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20230826L },
                    { 230902, 1067L, 230909, 8, 8, 35, 8, 35, 8, 35, 8, 35, 8, 3, 35, 20230902L },
                    { 230909, 1068L, 230916, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20230909L },
                    { 230916, 1069L, 230923, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20230916L },
                    { 230923, 1070L, 230930, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 20230923L },
                    { 230930, 1071L, 231007, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20230930L },
                    { 231007, 1072L, 231014, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20231007L },
                    { 231014, 1073L, 231021, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20231014L },
                    { 231021, 1074L, 231028, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20231021L },
                    { 231028, 1075L, 231104, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20231028L },
                    { 231104, 1076L, 231111, 11, 11, 44, 11, 44, 11, 44, 11, 44, 11, 4, 44, 20231104L },
                    { 231111, 1077L, 231118, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20231111L },
                    { 231118, 1078L, 231125, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20231118L },
                    { 231125, 1079L, 231202, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20231125L },
                    { 231202, 1080L, 231209, 11, 11, 48, 11, 48, 11, 48, 11, 48, 11, 4, 48, 20231202L },
                    { 231209, 1081L, 231216, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20231209L },
                    { 231216, 1082L, 231223, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20231216L },
                    { 231223, 1083L, 231230, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20231223L },
                    { 231230, 1084L, 240106, 12, 12, 52, 12, 52, 12, 52, 12, 52, 12, 1, 52, 20231230L },
                    { 240106, 1093L, 240113, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 20240106L },
                    { 240113, 1094L, 240120, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 20240113L },
                    { 240120, 1095L, 240127, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 20240120L },
                    { 240127, 1096L, 240203, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 20240127L },
                    { 240203, 1097L, 240210, 1, 1, 5, 1, 5, 1, 5, 1, 5, 1, 1, 5, 20240203L },
                    { 240210, 1098L, 240217, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 20240210L },
                    { 240217, 1099L, 240224, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 20240217L },
                    { 240224, 1100L, 240302, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 20240224L },
                    { 240302, 1101L, 240309, 2, 2, 9, 2, 9, 2, 9, 2, 9, 2, 1, 9, 20240302L },
                    { 240309, 1102L, 240316, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 20240309L },
                    { 240316, 1103L, 240323, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 20240316L },
                    { 240323, 1104L, 240330, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 1, 12, 20240323L },
                    { 240330, 1105L, 240406, 3, 3, 13, 3, 13, 3, 13, 3, 13, 3, 2, 13, 20240330L },
                    { 240406, 1106L, 240413, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 20240406L },
                    { 240413, 1107L, 240420, 4, 5, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 20240413L },
                    { 240420, 1108L, 240427, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 20240420L },
                    { 240427, 1109L, 240504, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 20240427L },
                    { 240504, 1110L, 240511, 5, 5, 18, 5, 18, 5, 18, 5, 18, 5, 2, 18, 20240504L },
                    { 240511, 1111L, 240518, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 20240511L },
                    { 240518, 1112L, 240525, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 20240518L },
                    { 240525, 1113L, 240601, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 20240525L },
                    { 240601, 1114L, 240608, 5, 5, 22, 5, 22, 5, 22, 5, 22, 5, 2, 22, 20240601L },
                    { 240608, 1115L, 240615, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 20240608L },
                    { 240615, 1116L, 240622, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 20240615L },
                    { 240622, 1117L, 240629, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 2, 25, 20240622L },
                    { 240629, 1145L, 240706, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 20240629L },
                    { 240706, 1119L, 240713, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 20240706L },
                    { 240713, 1120L, 240720, 7, 3, 28, 3, 28, 3, 28, 3, 28, 7, 3, 28, 20240713L },
                    { 240720, 1121L, 240727, 7, 3, 29, 3, 29, 3, 29, 3, 29, 7, 3, 29, 20240720L },
                    { 240727, 1122L, 240803, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 20240727L },
                    { 240803, 1123L, 240810, 7, 7, 31, 7, 31, 7, 31, 7, 31, 7, 3, 31, 20240803L },
                    { 240810, 1124L, 240817, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 20240810L },
                    { 240817, 1125L, 240824, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 20240817L },
                    { 240824, 1126L, 240831, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 20240824L },
                    { 240831, 1127L, 240907, 8, 8, 35, 8, 35, 8, 35, 8, 35, 8, 3, 35, 20240831L },
                    { 240907, 1128L, 240914, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 20240907L },
                    { 240914, 1129L, 240921, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 20240914L },
                    { 240921, 1130L, 240928, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 20240921L },
                    { 240928, 1131L, 241005, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 20240928L },
                    { 241005, 1132L, 241012, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 20241005L },
                    { 241012, 1133L, 241019, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 20241012L },
                    { 241019, 1134L, 241026, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 20241019L },
                    { 241026, 1135L, 241102, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 20241026L },
                    { 241102, 1136L, 241109, 10, 10, 44, 10, 44, 10, 44, 10, 44, 10, 4, 44, 20241102L },
                    { 241109, 1137L, 241116, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 20241109L },
                    { 241116, 1138L, 241123, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 20241116L },
                    { 241123, 1139L, 241130, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 20241123L },
                    { 241130, 1140L, 241207, 11, 11, 48, 11, 48, 11, 48, 11, 48, 11, 4, 48, 20241130L },
                    { 241207, 1141L, 241214, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 20241207L },
                    { 241214, 1142L, 241221, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 20241214L },
                    { 241221, 1143L, 241228, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 20241221L },
                    { 241228, 1144L, 250104, 12, 12, 52, 12, 52, 12, 52, 12, 51, 12, 1, 52, 20241228L },
                    { 990109, -407L, 990116, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 19990109L },
                    { 990116, -406L, 990123, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 19990116L },
                    { 990123, -405L, 990130, 1, 1, 3, 1, 3, 1, 3, 1, 3, 1, 1, 3, 19990123L },
                    { 990130, -404L, 990206, 1, 1, 4, 1, 4, 1, 4, 1, 4, 1, 1, 4, 19990130L },
                    { 990206, -403L, 990213, 2, 2, 5, 2, 5, 2, 5, 2, 5, 2, 1, 5, 19990206L },
                    { 990213, -402L, 990220, 2, 2, 6, 2, 6, 2, 6, 2, 6, 2, 1, 6, 19990213L },
                    { 990220, -401L, 990227, 2, 2, 7, 2, 7, 2, 7, 2, 7, 2, 1, 7, 19990220L },
                    { 990227, -400L, 990306, 2, 2, 8, 2, 8, 2, 8, 2, 8, 2, 1, 8, 19990227L },
                    { 990306, -399L, 990313, 3, 3, 9, 3, 9, 3, 9, 3, 9, 3, 1, 9, 19990306L },
                    { 990313, -398L, 990320, 3, 3, 10, 3, 10, 3, 10, 3, 10, 3, 1, 10, 19990313L },
                    { 990320, -397L, 990327, 3, 3, 11, 3, 11, 3, 11, 3, 11, 3, 1, 11, 19990320L },
                    { 990327, -396L, 990403, 3, 3, 12, 3, 12, 3, 12, 3, 12, 3, 2, 12, 19990327L },
                    { 990403, -395L, 990410, 4, 4, 13, 4, 13, 4, 13, 4, 13, 4, 2, 13, 19990403L },
                    { 990410, -394L, 990417, 4, 4, 14, 4, 14, 4, 14, 4, 14, 4, 2, 14, 19990410L },
                    { 990417, -393L, 990424, 4, 4, 15, 4, 15, 4, 15, 4, 15, 4, 2, 15, 19990417L },
                    { 990424, -392L, 990501, 4, 4, 16, 4, 16, 4, 16, 4, 16, 4, 2, 16, 19990424L },
                    { 990501, -391L, 990508, 4, 4, 17, 4, 17, 4, 17, 4, 17, 4, 2, 17, 19990501L },
                    { 990508, -390L, 990515, 5, 5, 18, 5, 18, 5, 18, 5, 18, 5, 2, 18, 19990508L },
                    { 990515, -389L, 990522, 5, 5, 19, 5, 19, 5, 19, 5, 19, 5, 2, 19, 19990515L },
                    { 990522, -388L, 990529, 5, 5, 20, 5, 20, 5, 20, 5, 20, 5, 2, 20, 19990522L },
                    { 990529, -387L, 990605, 5, 5, 21, 5, 21, 5, 21, 5, 21, 5, 2, 21, 19990529L },
                    { 990605, -386L, 990612, 6, 6, 22, 6, 22, 6, 22, 6, 22, 6, 2, 22, 19990605L },
                    { 990612, -385L, 990619, 6, 6, 23, 6, 23, 6, 23, 6, 23, 6, 2, 23, 19990612L },
                    { 990619, -384L, 990626, 6, 6, 24, 6, 24, 6, 24, 6, 24, 6, 2, 24, 19990619L },
                    { 990626, -383L, 990703, 6, 6, 25, 6, 25, 6, 25, 6, 25, 6, 3, 25, 19990626L },
                    { 990703, -382L, 990710, 6, 6, 26, 6, 26, 6, 26, 6, 26, 6, 3, 26, 19990703L },
                    { 990710, -381L, 990717, 7, 7, 27, 7, 27, 7, 27, 7, 27, 7, 3, 27, 19990710L },
                    { 990717, -380L, 990724, 7, 7, 28, 7, 28, 7, 28, 7, 28, 7, 3, 28, 19990717L },
                    { 990724, -379L, 990731, 7, 7, 29, 7, 29, 7, 29, 7, 29, 7, 3, 29, 19990724L },
                    { 990731, -378L, 990807, 7, 7, 30, 7, 30, 7, 30, 7, 30, 7, 3, 30, 19990731L },
                    { 990807, -377L, 990814, 8, 8, 31, 8, 31, 8, 31, 8, 31, 8, 3, 31, 19990807L },
                    { 990814, -376L, 990821, 8, 8, 32, 8, 32, 8, 32, 8, 32, 8, 3, 32, 19990814L },
                    { 990821, -375L, 990828, 8, 8, 33, 8, 33, 8, 33, 8, 33, 8, 3, 33, 19990821L },
                    { 990828, -374L, 990904, 8, 8, 34, 8, 34, 8, 34, 8, 34, 8, 3, 34, 19990828L },
                    { 990904, -373L, 990911, 9, 9, 35, 9, 35, 9, 35, 9, 35, 9, 3, 35, 19990904L },
                    { 990911, -372L, 990918, 9, 9, 36, 9, 36, 9, 36, 9, 36, 9, 3, 36, 19990911L },
                    { 990918, -371L, 990925, 9, 9, 37, 9, 37, 9, 37, 9, 37, 9, 3, 37, 19990918L },
                    { 990925, -370L, 991002, 9, 9, 38, 9, 38, 9, 38, 9, 38, 9, 3, 38, 19990925L },
                    { 991002, -369L, 991009, 9, 9, 39, 9, 39, 9, 39, 9, 39, 9, 4, 39, 19991002L },
                    { 991009, -368L, 991016, 10, 10, 40, 10, 40, 10, 40, 10, 40, 10, 4, 40, 19991009L },
                    { 991016, -367L, 991023, 10, 10, 41, 10, 41, 10, 41, 10, 41, 10, 4, 41, 19991016L },
                    { 991023, -366L, 991030, 10, 10, 42, 10, 42, 10, 42, 10, 42, 10, 4, 42, 19991023L },
                    { 991030, -365L, 991106, 10, 10, 43, 10, 43, 10, 43, 10, 43, 10, 4, 43, 19991030L },
                    { 991106, -364L, 991113, 11, 11, 44, 11, 44, 11, 44, 11, 44, 11, 4, 44, 19991106L },
                    { 991113, -363L, 991120, 11, 11, 45, 11, 45, 11, 45, 11, 45, 11, 4, 45, 19991113L },
                    { 991120, -362L, 991127, 11, 11, 46, 11, 46, 11, 46, 11, 46, 11, 4, 46, 19991120L },
                    { 991127, -361L, 991204, 11, 11, 47, 11, 47, 11, 47, 11, 47, 11, 4, 47, 19991127L },
                    { 991204, -360L, 991211, 12, 12, 48, 12, 48, 12, 48, 12, 48, 12, 4, 48, 19991204L },
                    { 991211, -359L, 991218, 12, 12, 49, 12, 49, 12, 49, 12, 49, 12, 4, 49, 19991211L },
                    { 991218, -358L, 991225, 12, 12, 50, 12, 50, 12, 50, 12, 50, 12, 4, 50, 19991218L },
                    { 991225, -357L, 101, 12, 12, 51, 12, 51, 12, 51, 12, 51, 12, 4, 51, 19991225L }
                });

            migrationBuilder.InsertData(
                table: "COUNTRY",
                columns: new[] { "ISO", "ID", "NAME", "TELEPHONE_CODE" },
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
                table: "DISTRIBUTION_FREQUENCY",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { "A", "Annually" },
                    { "H", "Hardship" },
                    { "M", "Monthly" },
                    { "P", "Pay Direct" },
                    { "Q", "Quarterly" },
                    { "R", "Rollover Direct" }
                });

            migrationBuilder.InsertData(
                table: "DISTRIBUTION_STATUS",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { "C", "Manual Check" },
                    { "D", "Purge this Record" },
                    { "H", "Request is on hold" },
                    { "O", "Override vested amount in check (death or > 64 and 5 years vested" },
                    { "P", "Payment as been made" },
                    { "X", "Purge all records for the SSN" },
                    { "Y", "Request is OK to pay" },
                    { "Z", "Purge all records for the SSN" }
                });

            migrationBuilder.InsertData(
                table: "EMPLOYEE_TYPE",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { (byte)0, "NOT New in plan last year" },
                    { (byte)1, "New last year" }
                });

            migrationBuilder.InsertData(
                table: "EMPLOYMENT_STATUS",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { "a", "Active" },
                    { "d", "Delete" },
                    { "i", "Inactive" },
                    { "t", "Terminated" }
                });

            migrationBuilder.InsertData(
                table: "EMPLOYMENT_TYPE",
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
                table: "PAY_CLASSIFICATION",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { (byte)0, "01" },
                    { (byte)1, "MANAGER" },
                    { (byte)2, "ASSISTANT MANAGER" },
                    { (byte)7, "SPIRITS CLERK - PT" },
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
                table: "PAY_FREQUENCY",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { (byte)1, "Weekly" },
                    { (byte)2, "Monthly" }
                });

            migrationBuilder.InsertData(
                table: "PROFIT_CODE",
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
                table: "TAX_CODE",
                columns: new[] { "CODE", "DESCRIPTION" },
                values: new object[,]
                {
                    { "0", "Unknown - not legal tax code, yet 24 records in the ofuscated set have this value." },
                    { "1", "Early (Premature) dist no known exception" },
                    { "2", "Early (Premature) dist exception applies" },
                    { "3", "Disability" },
                    { "4", "Death" },
                    { "5", "Prohibited transaction" },
                    { "6", "Section 1035 exchange" },
                    { "7", "Normal distribution" },
                    { "8", "Excess contributions + earnings/deferrals" },
                    { "9", "PS 58 cost" },
                    { "A", "Qualifies for 5- or 10-year averaging" },
                    { "B", "Qualifies for death benefit exclusion" },
                    { "C", "Qualifies for both A and B" },
                    { "D", "Excess contributions + earnings deferrals" },
                    { "E", "Excess annual additions under section 415" },
                    { "F", "Charitable gift annuity" },
                    { "G", "Direct rollover to IRA" },
                    { "H", "Direct rollover to plan/tax sheltered annuity" },
                    { "P", "Excess contributions + earnings/deferrals" }
                });

            migrationBuilder.InsertData(
                table: "TERMINATION_CODE",
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
                    { "L", "Move Out of Area" },
                    { "M", "Poor Performance" },
                    { "N", "Off For Summer" },
                    { "O", "Workmans Compensation" },
                    { "P", "Injured" },
                    { "Q", "Transferred" },
                    { "R", "Retired" },
                    { "S", "Competition" },
                    { "T", "Another Job" },
                    { "U", "Would Not Rehire" },
                    { "V", "Never Reported" },
                    { "W", "Retired Receiving Pension" },
                    { "X", "Military" },
                    { "Y", "FMLA Approved" },
                    { "Z", "Deceased" }
                });

            migrationBuilder.InsertData(
                table: "ZERO_CONTRIBUTION_REASON",
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
                    { (byte)7, "=64 AND 1ST CONTRIBUTION >=5 YEARS AGO GETS 100% VESTED ON THEIR BIRTHDAY" },
                    { (byte)8, "Unknown" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BENEFICIARY_COUNTRY_ISO",
                table: "BENEFICIARY",
                column: "COUNTRY_ISO");

            migrationBuilder.CreateIndex(
                name: "IX_BENEFICIARY_KINDID",
                table: "BENEFICIARY",
                column: "KIND_ID");

            migrationBuilder.CreateIndex(
                name: "CALDAR_RECORD_ACC_APWKEND_N",
                table: "CALDAR_RECORD",
                column: "ACC_APWKEND",
                unique: true,
                filter: "\"ACC_APWKEND\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "CALDAR_RECORD_ACC_WEDATE2",
                table: "CALDAR_RECORD",
                column: "ACC_WKEND2_N",
                unique: true,
                filter: "\"ACC_WKEND2_N\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_COUNTRY_ISO",
                table: "DEMOGRAPHIC",
                column: "COUNTRY_ISO");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_DEPARTMENTID",
                table: "DEMOGRAPHIC",
                column: "DEPARTMENT");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_EMPLOYMENTSTATUSID",
                table: "DEMOGRAPHIC",
                column: "EMPLOYMENT_STATUS_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_EMPLOYMENTTYPEID",
                table: "DEMOGRAPHIC",
                column: "EMPLOYMENT_TYPE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_GENDERID",
                table: "DEMOGRAPHIC",
                column: "GENDER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_PAYCLASSIFICATIONID",
                table: "DEMOGRAPHIC",
                column: "PAY_CLASSIFICATION_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_PAYFREQUENCYID",
                table: "DEMOGRAPHIC",
                column: "PAY_FREQUENCY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_TERMINATIONCODEID",
                table: "DEMOGRAPHIC",
                column: "TERMINATION_CODE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_SYNC_AUDIT_BADGENUMBER",
                table: "DEMOGRAPHIC_SYNC_AUDIT",
                column: "BADGE_NUMBER");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_COUNTRY_ISO",
                table: "DISTRIBUTION",
                column: "COUNTRY_ISO");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_FREQUENCYID",
                table: "DISTRIBUTION",
                column: "FREQUENCY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_GENDERID",
                table: "DISTRIBUTION",
                column: "GENDER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_STATUSID",
                table: "DISTRIBUTION",
                column: "STATUS_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_TAXCODEID",
                table: "DISTRIBUTION",
                column: "TAX_CODE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_THIRD_PARTY_COUNTRY_ISO",
                table: "DISTRIBUTION",
                column: "THIRD_PARTY_COUNTRY_ISO");

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
                name: "IX_PAY_PROFIT_BENEFICIARYTYPEID",
                table: "PAY_PROFIT",
                column: "BENEFICIARY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PAY_PROFIT_EMPLOYEETYPEID",
                table: "PAY_PROFIT",
                column: "EMPLOYEE_TYPE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PAY_PROFIT_ENROLLMENTID",
                table: "PAY_PROFIT",
                column: "ENROLLMENT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PAY_PROFIT_ZEROCONTRIBUTIONREASONID",
                table: "PAY_PROFIT",
                column: "ZERO_CONTRIBUTION_REASON_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_DETAIL_PROFIT_CODE_ID",
                table: "PROFIT_DETAIL",
                column: "PROFIT_CODE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_DETAIL_TAXCODEID",
                table: "PROFIT_DETAIL",
                column: "TAX_CODE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_DETAIL_ZEROCONTRIBUTIONREASONID",
                table: "PROFIT_DETAIL",
                column: "ZERO_CONTRIBUTION_REASON_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BENEFICIARY");

            migrationBuilder.DropTable(
                name: "CALDAR_RECORD");

            migrationBuilder.DropTable(
                name: "DEMOGRAPHIC");

            migrationBuilder.DropTable(
                name: "DEMOGRAPHIC_SYNC_AUDIT");

            migrationBuilder.DropTable(
                name: "DISTRIBUTION");

            migrationBuilder.DropTable(
                name: "JOB");

            migrationBuilder.DropTable(
                name: "PAY_PROFIT");

            migrationBuilder.DropTable(
                name: "PROFIT_DETAIL");

            migrationBuilder.DropTable(
                name: "STATE_TAX");

            migrationBuilder.DropTable(
                name: "BENEFICIARY_KIND");

            migrationBuilder.DropTable(
                name: "DEPARTMENT");

            migrationBuilder.DropTable(
                name: "EMPLOYMENT_STATUS");

            migrationBuilder.DropTable(
                name: "EMPLOYMENT_TYPE");

            migrationBuilder.DropTable(
                name: "PAY_CLASSIFICATION");

            migrationBuilder.DropTable(
                name: "PAY_FREQUENCY");

            migrationBuilder.DropTable(
                name: "TERMINATION_CODE");

            migrationBuilder.DropTable(
                name: "COUNTRY");

            migrationBuilder.DropTable(
                name: "DISTRIBUTION_FREQUENCY");

            migrationBuilder.DropTable(
                name: "DISTRIBUTION_STATUS");

            migrationBuilder.DropTable(
                name: "GENDER");

            migrationBuilder.DropTable(
                name: "JOBSTARTMETHOD");

            migrationBuilder.DropTable(
                name: "JOBSTATUS");

            migrationBuilder.DropTable(
                name: "JOBTYPE");

            migrationBuilder.DropTable(
                name: "BENEFICIARY_TYPE");

            migrationBuilder.DropTable(
                name: "EMPLOYEE_TYPE");

            migrationBuilder.DropTable(
                name: "ENROLLMENT");

            migrationBuilder.DropTable(
                name: "PROFIT_CODE");

            migrationBuilder.DropTable(
                name: "TAX_CODE");

            migrationBuilder.DropTable(
                name: "ZERO_CONTRIBUTION_REASON");
        }
    }
}
