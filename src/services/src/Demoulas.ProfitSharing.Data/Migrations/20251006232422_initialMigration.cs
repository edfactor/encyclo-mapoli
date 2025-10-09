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
            migrationBuilder.CreateSequence<int>(
                name: "FAKE_SSN_SEQ",
                startValue: 666000000L,
                minValue: 666000000L,
                maxValue: 666999999L);

            migrationBuilder.CreateTable(
                name: "_HEALTH_CHECK_STATUS_HISTORY",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    KEY = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(24)", maxLength: 24, nullable: false),
                    DESCRIPTION = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    EXCEPTION = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    DURATION = table.Column<TimeSpan>(type: "INTERVAL DAY(8) TO SECOND(7)", nullable: false),
                    CREATED_AT = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HEALTH_CHECK_STATUS_HISTORY", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ANNUITY_RATE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    YEAR = table.Column<short>(type: "NUMBER(4)", precision: 4, nullable: false),
                    AGE = table.Column<byte>(type: "NUMBER(3)", precision: 3, nullable: false),
                    SINGLE_RATE = table.Column<decimal>(type: "DECIMAL(6,4)", precision: 6, scale: 4, nullable: false),
                    JOINT_RATE = table.Column<decimal>(type: "DECIMAL(6,4)", precision: 6, scale: 4, nullable: false),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ANNUITY_RATE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AUDIT_EVENT",
                columns: table => new
                {
                    AUDIT_EVENT_ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TABLE_NAME = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: true),
                    OPERATION = table.Column<string>(type: "NVARCHAR2(24)", maxLength: 24, nullable: false),
                    PRIMARY_KEY = table.Column<string>(type: "NVARCHAR2(512)", maxLength: 512, nullable: true),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: false, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    CREATED_AT = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    CHANGES_JSON = table.Column<string>(type: "CLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_EVENT", x => x.AUDIT_EVENT_ID);
                });

            migrationBuilder.CreateTable(
                name: "BENEFICIARY_ARCHIVE",
                columns: table => new
                {
                    ARCHIVE_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    BADGE_NUMBER = table.Column<int>(type: "NUMBER(7)", precision: 7, nullable: false),
                    PSN_SUFFIX = table.Column<short>(type: "NUMBER(5)", precision: 5, nullable: false),
                    DEMOGRAPHIC_ID = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    BENEFICIARY_CONTACT_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    RELATIONSHIP = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: true),
                    KIND_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: true),
                    PERCENT = table.Column<decimal>(type: "numeric(3,0)", precision: 3, nullable: false),
                    DELETE_DATE = table.Column<string>(type: "NVARCHAR2(10)", nullable: false),
                    DELETED_BY = table.Column<string>(type: "NVARCHAR2(24)", maxLength: 24, nullable: false, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BENEFICIARY_ARCHIVE", x => x.ARCHIVE_ID);
                });

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
                    ACC_WKEND2_N = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ACC_APWKEND = table.Column<int>(type: "NUMBER(6)", precision: 6, nullable: false),
                    ACC_WEEKN = table.Column<byte>(type: "NUMBER(2)", precision: 2, nullable: false),
                    ACC_PERIOD = table.Column<byte>(type: "NUMBER(2)", precision: 2, nullable: false),
                    ACC_QUARTER = table.Column<byte>(type: "NUMBER(2)", precision: 2, nullable: false),
                    ACC_CALPERIOD = table.Column<byte>(type: "NUMBER(2)", precision: 2, nullable: false),
                    ACC_CLN60_WEEK = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ACC_CLN60_PERIOD = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ACC_CLN61_WEEK = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ACC_CLN61_PERIOD = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ACC_CLN7X_WEEK = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ACC_CLN7X_PERIOD = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ACC_CLN6X_WEEK = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ACC_CLN6X_PERIOD = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ACC_ALT_KEY_NUM = table.Column<long>(type: "NUMBER(19)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CALDAR_RECORD", x => x.ACC_WKEND2_N);
                });

            migrationBuilder.CreateTable(
                name: "COMMENT_TYPE",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COMMENT_TYPE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "COUNTRY",
                columns: table => new
                {
                    ISO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false),
                    ID = table.Column<byte>(type: "NUMBER(3)", precision: 3, nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    TELEPHONE_CODE = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COUNTRY", x => x.ISO);
                });

            migrationBuilder.CreateTable(
                name: "DATA_IMPORT_RECORD",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    SOURCE_SCHEMA = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    IMPORT_DATE_TIME_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DATA_IMPORT_RECORD", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DEMOGRAPHIC_HISTORY",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(18)", precision: 18, nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DEMOGRAPHIC_ID = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    VALID_FROM = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false),
                    VALID_TO = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false),
                    ORACLE_HCM_ID = table.Column<long>(type: "NUMBER(15)", precision: 15, nullable: false),
                    BADGE_NUMBER = table.Column<int>(type: "NUMBER(7)", precision: 7, nullable: false),
                    STORE_NUMBER = table.Column<short>(type: "NUMBER(4)", precision: 4, nullable: false, comment: "StoreNumber"),
                    PAY_CLASSIFICATION_ID = table.Column<string>(type: "NVARCHAR2(2000)", precision: 2, nullable: false, comment: "PayClassification"),
                    DATE_OF_BIRTH = table.Column<DateTime>(type: "DATE", nullable: false, comment: "DateOfBirth"),
                    HIRE_DATE = table.Column<DateTime>(type: "DATE", nullable: false, comment: "HireDate"),
                    REHIRE_DATE = table.Column<DateTime>(type: "DATE", nullable: true, comment: "ReHireDate"),
                    TERMINATION_DATE = table.Column<DateTime>(type: "DATE", nullable: true, comment: "TerminationDate"),
                    DEPARTMENT = table.Column<byte>(type: "NUMBER(1)", precision: 1, nullable: false, comment: "Department"),
                    EMPLOYMENT_TYPE_ID = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false, comment: "EmploymentType"),
                    PAY_FREQUENCY_ID = table.Column<byte>(type: "NUMBER(3)", maxLength: 1, nullable: false, comment: "PayFrequency"),
                    TERMINATION_CODE_ID = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: true, comment: "TerminationCode"),
                    EMPLOYMENT_STATUS_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    CREATED_DATETIME = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DEMOGRAPHIC_HISTORY", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DEMOGRAPHIC_SYNC_AUDIT",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    BADGE_NUMBER = table.Column<int>(type: "NUMBER(7)", precision: 7, nullable: false),
                    ORACLE_HCM_ID = table.Column<long>(type: "NUMBER(15)", precision: 15, nullable: false),
                    INVALID_VALUE = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: true),
                    USERNAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true),
                    PROPERTY_NAME = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: true),
                    MESSAGE = table.Column<string>(type: "NVARCHAR2(512)", maxLength: 512, nullable: false),
                    CREATED = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DEMOGRAPHIC_SYNC_AUDIT", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DEPARTMENT",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(2)", precision: 2, nullable: false),
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
                name: "DISTRIBUTION_REQUEST_REASON",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(2)", precision: 2, nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DISTRIBUTION_REQUEST_REASON", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DISTRIBUTION_REQUEST_STATUS",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DISTRIBUTION_REQUEST_STATUS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DISTRIBUTION_REQUEST_TYPE",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DISTRIBUTION_REQUEST_TYPE", x => x.ID);
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
                    NAME = table.Column<string>(type: "NVARCHAR2(84)", maxLength: 84, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ENROLLMENT", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EXCLUDED_ID_TYPE",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EXCLUDED_ID_TYPE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FAKE_SSNS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    SSN = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAKE_SSNS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FROZEN_STATE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PROFIT_YEAR = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    IS_ACTIVE = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    FROZEN_BY = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false),
                    AS_OF_DATETIME = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false),
                    CREATED_DATETIME = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FROZEN_STATE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "GENDER",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(32)", maxLength: 32, nullable: false)
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
                name: "MISSIVES",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MESSAGE = table.Column<string>(type: "NVARCHAR2(60)", maxLength: 60, nullable: false),
                    DESCRIPTION = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: false),
                    SEVERITY = table.Column<string>(type: "NVARCHAR2(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MISSIVES", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NAVIGATION_ROLE",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(65)", maxLength: 65, nullable: false),
                    IS_READ_ONLY = table.Column<bool>(type: "NUMBER(1)", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NAVIGATION_ROLE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NAVIGATION_STATUS",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NAVIGATION_STATUS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PAY_CLASSIFICATION",
                columns: table => new
                {
                    ID = table.Column<string>(type: "NVARCHAR2(4)", maxLength: 4, nullable: false),
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
                    ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    FREQUENCY = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROFIT_CODE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "REPORT_CHECKSUM",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    REPORT_TYPE = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    PROFIT_YEAR = table.Column<short>(type: "NUMBER(4)", precision: 4, nullable: false),
                    REQUEST_JSON = table.Column<string>(type: "CLOB", nullable: false),
                    REPORT_JSON = table.Column<string>(type: "CLOB", nullable: false),
                    KEYFIELDS_CHECKSUM_JSON = table.Column<string>(type: "CLOB", nullable: false),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REPORT_CHECKSUM", x => x.ID);
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
                    ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TAX_CODE", x => x.ID);
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
                name: "YE_UPDATE_STATUS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PROFIT_YEAR = table.Column<short>(type: "NUMBER(4)", precision: 4, nullable: false),
                    BENEFICIARIES_EFFECTED = table.Column<int>(type: "NUMBER(6)", precision: 6, nullable: false),
                    EMPLOYEES_EFFECTED = table.Column<int>(type: "NUMBER(6)", precision: 6, nullable: false),
                    ETVAS_EFFECTED = table.Column<int>(type: "NUMBER(6)", precision: 6, nullable: false),
                    CONTRIBUTION_PERCENT = table.Column<decimal>(type: "DECIMAL(9,6)", precision: 9, scale: 6, nullable: false),
                    INCOMING_FORFEIT_PERCENT = table.Column<decimal>(type: "DECIMAL(9,6)", precision: 9, scale: 6, nullable: false),
                    EARNINGS_PERCENT = table.Column<decimal>(type: "DECIMAL(9,6)", precision: 9, scale: 6, nullable: false),
                    SECONDARY_EARNINGS_PERCENT = table.Column<decimal>(type: "DECIMAL(9,6)", precision: 9, scale: 6, nullable: false),
                    MAX_ALLOWED_CONTRIBUTIONS = table.Column<long>(type: "NUMBER(6)", precision: 6, nullable: false),
                    BADGE_ADJUSTED = table.Column<long>(type: "NUMBER(7)", precision: 7, nullable: false),
                    BADGE_ADJUSTED2 = table.Column<long>(type: "NUMBER(7)", precision: 7, nullable: false),
                    ADJUST_CONTRIBUTION_AMOUNT = table.Column<decimal>(type: "DECIMAL(5,2)", precision: 5, scale: 2, nullable: false),
                    ADJUST_EARNINGS_AMOUNT = table.Column<decimal>(type: "DECIMAL(5,2)", precision: 5, scale: 2, nullable: false),
                    ADJUST_INCOMING_FORFEIT_AMOUNT = table.Column<decimal>(type: "DECIMAL(5,2)", precision: 5, scale: 2, nullable: false),
                    ADJUST_EARNINGS_SECONDARY_AMOUNT = table.Column<decimal>(type: "DECIMAL(5,2)", precision: 5, scale: 2, nullable: false),
                    IS_YEAR_END_COMPLETED = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YE_UPDATE_STATUS", x => x.ID);
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
                name: "BENEFICIARY_CONTACT",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    SSN = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    DATE_OF_BIRTH = table.Column<DateTime>(type: "DATE", nullable: false),
                    STREET = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: false, comment: "Street"),
                    STREET2 = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: true, comment: "Street2"),
                    STREET3 = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: true, comment: "Street3"),
                    STREET4 = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: true, comment: "Street4"),
                    CITY = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: false, comment: "City"),
                    STATE = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: false, comment: "State"),
                    POSTAL_CODE = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: false, comment: "Postal Code"),
                    COUNTRY_ISO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: true, defaultValue: "US"),
                    FULL_NAME = table.Column<string>(type: "NVARCHAR2(84)", maxLength: 84, nullable: false, comment: "FullName"),
                    LAST_NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false, comment: "LastName"),
                    FIRST_NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false, comment: "FirstName"),
                    MIDDLE_NAME = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: true, comment: "MiddleName"),
                    PHONE_NUMBER = table.Column<string>(type: "NVARCHAR2(16)", maxLength: 16, nullable: true),
                    MOBILE_NUMBER = table.Column<string>(type: "NVARCHAR2(16)", maxLength: 16, nullable: true),
                    EMAIL_ADDRESS = table.Column<string>(type: "NVARCHAR2(84)", maxLength: 84, nullable: true),
                    CREATED_DATE = table.Column<DateTime>(type: "DATE", nullable: false, defaultValueSql: "SYSDATE"),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BENEFICIARY_CONTACT", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BENEFICIARY_CONTACT_COUNTRY_COUNTRY_ISO",
                        column: x => x.COUNTRY_ISO,
                        principalTable: "COUNTRY",
                        principalColumn: "ISO");
                });

            migrationBuilder.CreateTable(
                name: "BENEFICIARY_CONTACT_ARCHIVE",
                columns: table => new
                {
                    ARCHIVE_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    SSN = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    DATE_OF_BIRTH = table.Column<DateTime>(type: "DATE", nullable: false),
                    STREET = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: false, comment: "Street"),
                    STREET2 = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: true, comment: "Street2"),
                    STREET3 = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: true, comment: "Street3"),
                    STREET4 = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: true, comment: "Street4"),
                    CITY = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: false, comment: "City"),
                    STATE = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: false, comment: "State"),
                    POSTAL_CODE = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: false, comment: "Postal Code"),
                    COUNTRY_ISO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: true, defaultValue: "US"),
                    FULL_NAME = table.Column<string>(type: "NVARCHAR2(84)", maxLength: 84, nullable: false, comment: "FullName"),
                    LAST_NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false, comment: "LastName"),
                    FIRST_NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false, comment: "FirstName"),
                    MIDDLE_NAME = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: true, comment: "MiddleName"),
                    PHONE_NUMBER = table.Column<string>(type: "NVARCHAR2(16)", maxLength: 16, nullable: true),
                    MOBILE_NUMBER = table.Column<string>(type: "NVARCHAR2(16)", maxLength: 16, nullable: true),
                    EMAIL_ADDRESS = table.Column<string>(type: "NVARCHAR2(84)", maxLength: 84, nullable: true),
                    CREATED_DATE = table.Column<DateTime>(type: "DATE", nullable: false, defaultValueSql: "SYSDATE"),
                    DELETE_DATE = table.Column<string>(type: "NVARCHAR2(10)", nullable: false),
                    DELETED_BY = table.Column<string>(type: "NVARCHAR2(24)", maxLength: 24, nullable: false, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BENEFICIARY_CONTACT_ARCHIVE", x => x.ARCHIVE_ID);
                    table.ForeignKey(
                        name: "FK_BENEFICIARY_CONTACT_ARCHIVE_COUNTRY_COUNTRY_ISO",
                        column: x => x.COUNTRY_ISO,
                        principalTable: "COUNTRY",
                        principalColumn: "ISO");
                });

            migrationBuilder.CreateTable(
                name: "DISTRIBUTION_PAYEE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    SSN = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(84)", maxLength: 84, nullable: false),
                    STREET = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: false),
                    STREET2 = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: true),
                    STREET3 = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: true),
                    STREET4 = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: true),
                    CITY = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: true),
                    STATE = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: true),
                    POSTAL_CODE = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    COUNTRY_ISO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: true, defaultValue: "US"),
                    MEMO = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DISTRIBUTION_PAYEE", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_PAYEE_COUNTRY_COUNTRY_ISO",
                        column: x => x.COUNTRY_ISO,
                        principalTable: "COUNTRY",
                        principalColumn: "ISO");
                });

            migrationBuilder.CreateTable(
                name: "DISTRIBUTION_THIRDPARTY_PAYEE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PAYEE = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: true),
                    NAME = table.Column<string>(type: "NVARCHAR2(84)", maxLength: 84, nullable: true),
                    STREET = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: false),
                    STREET2 = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: true),
                    STREET3 = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: true),
                    STREET4 = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: true),
                    CITY = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: true),
                    STATE = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: true),
                    POSTAL_CODE = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    COUNTRY_ISO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: true, defaultValue: "US"),
                    MEMO = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DISTRIBUTION_THIRDPARTY_PAYEE", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_THIRDPARTY_PAYEE_COUNTRY_COUNTRY_ISO",
                        column: x => x.COUNTRY_ISO,
                        principalTable: "COUNTRY",
                        principalColumn: "ISO");
                });

            migrationBuilder.CreateTable(
                name: "EXCLUDED_ID",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    EXCLUDED_ID_TYPE_ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    EXCLUDED_ID_VALUE = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EXCLUDED_ID", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EXCLUDED_ID_EXCLUDEDIDTYPE_EXCLUDEDIDTYPEID",
                        column: x => x.EXCLUDED_ID_TYPE_ID,
                        principalTable: "EXCLUDED_ID_TYPE",
                        principalColumn: "ID");
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
                    STARTED = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false),
                    COMPLETED = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true)
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
                name: "NAVIGATION",
                columns: table => new
                {
                    ID = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    PARENT_ID = table.Column<short>(type: "NUMBER(5)", nullable: true),
                    TITLE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    SUB_TITLE = table.Column<string>(type: "NVARCHAR2(70)", maxLength: 70, nullable: true),
                    URL = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    STATUS_ID = table.Column<byte>(type: "NUMBER(3)", nullable: true),
                    ORDER_NUMBER = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    DISABLED = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    IS_NAVIGABLE = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    ICON = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NAVIGATION", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NAVIGATION_NAVIGATIONSTATUSES_STATUS_ID",
                        column: x => x.STATUS_ID,
                        principalTable: "NAVIGATION_STATUS",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NAVIGATION_NAVIGATION_PARENT_ID",
                        column: x => x.PARENT_ID,
                        principalTable: "NAVIGATION",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "DEMOGRAPHIC",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ORACLE_HCM_ID = table.Column<long>(type: "NUMBER(15)", precision: 15, nullable: false),
                    SSN = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    STORE_NUMBER = table.Column<short>(type: "NUMBER(4)", precision: 4, nullable: false, comment: "StoreNumber"),
                    PAY_CLASSIFICATION_ID = table.Column<string>(type: "NVARCHAR2(4)", maxLength: 4, nullable: false, comment: "PayClassification"),
                    FULL_NAME = table.Column<string>(type: "NVARCHAR2(84)", maxLength: 84, nullable: false, comment: "FullName"),
                    LAST_NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false, comment: "LastName"),
                    FIRST_NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false, comment: "FirstName"),
                    MIDDLE_NAME = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: true, comment: "MiddleName"),
                    PHONE_NUMBER = table.Column<string>(type: "NVARCHAR2(16)", maxLength: 16, nullable: true),
                    MOBILE_NUMBER = table.Column<string>(type: "NVARCHAR2(16)", maxLength: 16, nullable: true),
                    EMAIL_ADDRESS = table.Column<string>(type: "NVARCHAR2(84)", maxLength: 84, nullable: true),
                    STREET = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: false, comment: "Street"),
                    STREET2 = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: true, comment: "Street2"),
                    STREET3 = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: true, comment: "Street3"),
                    STREET4 = table.Column<string>(type: "NVARCHAR2(56)", maxLength: 56, nullable: true, comment: "Street4"),
                    CITY = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: false, comment: "City"),
                    STATE = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: false, comment: "State"),
                    POSTAL_CODE = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: false, comment: "Postal Code"),
                    COUNTRY_ISO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: true, defaultValue: "US"),
                    DATE_OF_BIRTH = table.Column<DateTime>(type: "DATE", nullable: false, comment: "DateOfBirth"),
                    FULL_TIME_DATE = table.Column<DateTime>(type: "DATE", nullable: true, comment: "FullTimeDate"),
                    HIRE_DATE = table.Column<DateTime>(type: "DATE", nullable: false, comment: "HireDate"),
                    REHIRE_DATE = table.Column<DateTime>(type: "DATE", nullable: true, comment: "ReHireDate"),
                    TERMINATION_DATE = table.Column<DateTime>(type: "DATE", nullable: true, comment: "TerminationDate"),
                    DEPARTMENT = table.Column<byte>(type: "NUMBER(1)", precision: 1, nullable: false, comment: "Department"),
                    EMPLOYMENT_TYPE_ID = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false, comment: "EmploymentType"),
                    GENDER_ID = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false, comment: "Gender"),
                    PAY_FREQUENCY_ID = table.Column<byte>(type: "NUMBER(3)", maxLength: 1, nullable: false, comment: "PayFrequency"),
                    TERMINATION_CODE_ID = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: true, comment: "TerminationCode"),
                    EMPLOYMENT_STATUS_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true, defaultValueSql: "SYSTIMESTAMP"),
                    BADGE_NUMBER = table.Column<int>(type: "NUMBER(7)", precision: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DEMOGRAPHIC", x => x.ID);
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
                        name: "FK_DEMOGRAPHIC_EMPLOYMENTTYPES_EMPLOYMENTTYPEID",
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
                name: "PROFIT_DETAIL",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    SSN = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    PROFIT_YEAR = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    PROFIT_YEAR_ITERATION = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    DISTRIBUTION_SEQUENCE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PROFIT_CODE_ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    CONTRIBUTION = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false, comment: "Contribution to plan from DMB"),
                    EARNINGS = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    FORFEITURE = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    MONTH_TO_DATE = table.Column<byte>(type: "NUMBER(2,0)", precision: 2, scale: 0, nullable: false),
                    YEAR_TO_DATE = table.Column<short>(type: "NUMBER(4,0)", precision: 4, scale: 0, nullable: false),
                    REMARK = table.Column<string>(type: "NVARCHAR2(32)", maxLength: 32, nullable: true),
                    ZERO_CONTRIBUTION_REASON_ID = table.Column<byte>(type: "NUMBER(3)", nullable: true),
                    FEDERAL_TAXES = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    STATE_TAXES = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    TAX_CODE_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: true),
                    COMMENT_TYPE_ID = table.Column<byte>(type: "NUMBER(3)", nullable: true),
                    COMMENT_RELATED_CHECK_NUMBER = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    COMMENT_RELATED_STATE = table.Column<string>(type: "NVARCHAR2(24)", maxLength: 24, nullable: true),
                    COMMENT_RELATED_ORACLE_HCM_ID = table.Column<long>(type: "NUMBER(19)", nullable: true),
                    COMMENT_RELATED_PSN_SUFFIX = table.Column<short>(type: "NUMBER(5)", nullable: true),
                    COMMENT_IS_PARTIAL_TRANSACTION = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    YEARS_OF_SERVICE_CREDIT = table.Column<byte>(type: "NUMBER(3)", nullable: false, defaultValue: (byte)0),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROFIT_DETAIL", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PROFIT_DETAIL_COMMENT_TYPE_COMMENTTYPEID",
                        column: x => x.COMMENT_TYPE_ID,
                        principalTable: "COMMENT_TYPE",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_PROFIT_DETAIL_PROFIT_CODE_PROFITCODEID",
                        column: x => x.PROFIT_CODE_ID,
                        principalTable: "PROFIT_CODE",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_PROFIT_DETAIL_TAXCODES_TAXCODEID",
                        column: x => x.TAX_CODE_ID,
                        principalTable: "TAX_CODE",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_PROFIT_DETAIL_ZEROCONTRIBUTIONREASON_ZEROCONTRIBUTIONREASONID",
                        column: x => x.ZERO_CONTRIBUTION_REASON_ID,
                        principalTable: "ZERO_CONTRIBUTION_REASON",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "BENEFICIARY_SSN_CHANGE_HISTORY",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(18)", precision: 18, nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    BENEFICIARY_CONTACT_ID = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true, defaultValueSql: "SYSTIMESTAMP"),
                    OLD_SSN = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    NEW_SSN = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BENEFICIARY_SSN_CHANGE_HISTORY", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BENEFICIARY_SSN_CHANGE_HISTORY_BENEFICIARY_CONTACT_BENEFICIARYCONTACTID",
                        column: x => x.BENEFICIARY_CONTACT_ID,
                        principalTable: "BENEFICIARY_CONTACT",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "DISTRIBUTION",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    SSN = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    PAYMENT_SEQUENCE = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    EMPLOYEE_NAME = table.Column<string>(type: "NVARCHAR2(84)", maxLength: 84, nullable: false),
                    FREQUENCY_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    STATUS_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    PAYEE_ID = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    THIRD_PARTY_PAYEE_ID = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    FORTHEBENEFITOF_PAYEE = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    FORTHEBENEFITOF_ACCOUNT_TYPE = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    TAX1099_FOR_EMPLOYEE = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TAX1099_FOR_BENEFICIARY = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    GROSS_AMOUNT = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    FEDERAL_TAX_AMOUNT = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    STATE_TAX_AMOUNT = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    TAX_CODE_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    DECEASED = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    GENDER_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: true),
                    QDRO = table.Column<bool>(type: "NUMBER(1)", nullable: false, comment: "Qualified Domestic Relations Order"),
                    MEMO = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: true),
                    ROTH_IRA = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    THIRD_PARTY_PAYEE_ACCOUNT = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DISTRIBUTION", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_DISTRIBUTIONPAYEES_PAYEEID",
                        column: x => x.PAYEE_ID,
                        principalTable: "DISTRIBUTION_PAYEE",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_DISTRIBUTIONSTATUSES_STATUSID",
                        column: x => x.STATUS_ID,
                        principalTable: "DISTRIBUTION_STATUS",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_DISTRIBUTIONTHIRDPARTYPAYEE_THIRDPARTYPAYEEID",
                        column: x => x.THIRD_PARTY_PAYEE_ID,
                        principalTable: "DISTRIBUTION_THIRDPARTY_PAYEE",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_DISTRIBUTION_FREQUENCY_FREQUENCYID",
                        column: x => x.FREQUENCY_ID,
                        principalTable: "DISTRIBUTION_FREQUENCY",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_GENDER_GENDERID",
                        column: x => x.GENDER_ID,
                        principalTable: "GENDER",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_TAXCODES_TAXCODEID",
                        column: x => x.TAX_CODE_ID,
                        principalTable: "TAX_CODE",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NAVIGATION_ASSIGNED_ROLES",
                columns: table => new
                {
                    NAVIGATIONID = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    REQUIREDROLESID = table.Column<byte>(type: "NUMBER(3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NAVIGATION_ASSIGNED_ROLES", x => new { x.NAVIGATIONID, x.REQUIREDROLESID });
                    table.ForeignKey(
                        name: "FK_NAVIGATION_ASSIGNED_ROLES_NAVIGATIONROLES_REQUIREDROLESID",
                        column: x => x.REQUIREDROLESID,
                        principalTable: "NAVIGATION_ROLE",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NAVIGATION_ASSIGNED_ROLES_NAVIGATION_NAVIGATIONID",
                        column: x => x.NAVIGATIONID,
                        principalTable: "NAVIGATION",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NAVIGATION_PREREQUISITES",
                columns: table => new
                {
                    NAVIGATION_ID = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    PREREQUISITE_ID = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NAVIGATION_PREREQUISITES", x => new { x.NAVIGATION_ID, x.PREREQUISITE_ID });
                    table.ForeignKey(
                        name: "FK_NAV_PREREQ_DEPENDENT",
                        column: x => x.NAVIGATION_ID,
                        principalTable: "NAVIGATION",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NAV_PREREQ_PREREQUISITE",
                        column: x => x.PREREQUISITE_ID,
                        principalTable: "NAVIGATION",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "NAVIGATION_TRACKING",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NAVIGATION_ID = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    STATUS_ID = table.Column<byte>(type: "NUMBER(3)", nullable: true),
                    USERNAME = table.Column<string>(type: "NVARCHAR2(60)", maxLength: 60, nullable: true),
                    LAST_MODIFIED = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NAVIGATION_TRACKING", x => x.ID);
                    table.ForeignKey(
                        name: "FK_NAVIGATION_TRACKING_NAVIGATION_NAVIGATIONID",
                        column: x => x.NAVIGATION_ID,
                        principalTable: "NAVIGATION",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_NAVIGATION_TRACKING_NAVIGATION_STATUS_STATUS_ID",
                        column: x => x.STATUS_ID,
                        principalTable: "NAVIGATION_STATUS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "BENEFICIARY",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PSN_SUFFIX = table.Column<short>(type: "NUMBER(5)", precision: 5, nullable: false),
                    DEMOGRAPHIC_ID = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    BENEFICIARY_CONTACT_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    RELATIONSHIP = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: true),
                    KIND_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: true),
                    PERCENT = table.Column<decimal>(type: "numeric(3,0)", precision: 3, nullable: false),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true, defaultValueSql: "SYSTIMESTAMP"),
                    BADGE_NUMBER = table.Column<int>(type: "NUMBER(7)", precision: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BENEFICIARY", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BENEFICIARY_BENEFICIARY_CONTACT_BENEFICIARYCONTACTID",
                        column: x => x.BENEFICIARY_CONTACT_ID,
                        principalTable: "BENEFICIARY_CONTACT",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_BENEFICIARY_BENEFICIARY_KIND_KINDID",
                        column: x => x.KIND_ID,
                        principalTable: "BENEFICIARY_KIND",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_BENEFICIARY_DEMOGRAPHICS_DEMOGRAPHICID",
                        column: x => x.DEMOGRAPHIC_ID,
                        principalTable: "DEMOGRAPHIC",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "DEMOGRAPHIC_SSN_CHANGE_HISTORY",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(18)", precision: 18, nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DEMOGRAPHIC_ID = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true, defaultValueSql: "SYSTIMESTAMP"),
                    OLD_SSN = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    NEW_SSN = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DEMOGRAPHIC_SSN_CHANGE_HISTORY", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DEMOGRAPHIC_SSN_CHANGE_HISTORY_DEMOGRAPHIC_DEMOGRAPHICID",
                        column: x => x.DEMOGRAPHIC_ID,
                        principalTable: "DEMOGRAPHIC",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "DISTRIBUTION_REQUEST",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DEMOGRAPHIC_ID = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    REASON_ID = table.Column<byte>(type: "NUMBER(2)", nullable: false),
                    STATUS_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    TYPE_ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    REASON_TEXT = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: true),
                    REASON_OTHER = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    AMOUNT_REQUESTED = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false),
                    AMOUNT_AUTHORIZED = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: true),
                    DATE_REQUESTED = table.Column<DateTime>(type: "Date", nullable: false),
                    DATE_DECIDED = table.Column<DateTime>(type: "Date", nullable: true),
                    TAX_CODE_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DISTRIBUTION_REQUEST", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_REQUEST_DEMOGRAPHIC_DEMOGRAPHICID",
                        column: x => x.DEMOGRAPHIC_ID,
                        principalTable: "DEMOGRAPHIC",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_REQUEST_DISTRIBUTIONREQUESTREASON_REASONID",
                        column: x => x.REASON_ID,
                        principalTable: "DISTRIBUTION_REQUEST_REASON",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_REQUEST_DISTRIBUTIONREQUESTSTATUS_STATUSID",
                        column: x => x.STATUS_ID,
                        principalTable: "DISTRIBUTION_REQUEST_STATUS",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_REQUEST_DISTRIBUTIONREQUESTTYPE_TYPEID",
                        column: x => x.TYPE_ID,
                        principalTable: "DISTRIBUTION_REQUEST_TYPE",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DISTRIBUTION_REQUEST_TAXCODES_TAXCODEID",
                        column: x => x.TAX_CODE_ID,
                        principalTable: "TAX_CODE",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PAY_PROFIT",
                columns: table => new
                {
                    DEMOGRAPHIC_ID = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    PROFIT_YEAR = table.Column<short>(type: "NUMBER(4)", precision: 4, nullable: false),
                    CURRENT_HOURS_YEAR = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: false),
                    CURRENT_INCOME_YEAR = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    ETVA = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    WEEKS_WORKED_YEAR = table.Column<byte>(type: "NUMBER(2)", precision: 2, nullable: false),
                    PS_CERTIFICATE_ISSUED_DATE = table.Column<DateTime>(type: "DATE", nullable: true),
                    ENROLLMENT_ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    BENEFICIARY_TYPE_ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    EMPLOYEE_TYPE_ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    ZERO_CONTRIBUTION_REASON_ID = table.Column<byte>(type: "NUMBER(3)", nullable: true),
                    HOURS_EXECUTIVE = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: false),
                    INCOME_EXECUTIVE = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    POINTS_EARNED = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: true),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAY_PROFIT", x => new { x.DEMOGRAPHIC_ID, x.PROFIT_YEAR });
                    table.ForeignKey(
                        name: "FK_PAY_PROFIT_BENEFICIARY_TYPE_BENEFICIARYTYPEID",
                        column: x => x.BENEFICIARY_TYPE_ID,
                        principalTable: "BENEFICIARY_TYPE",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_PAY_PROFIT_DEMOGRAPHIC_DEMOGRAPHICID",
                        column: x => x.DEMOGRAPHIC_ID,
                        principalTable: "DEMOGRAPHIC",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_PAY_PROFIT_EMPLOYEE_TYPE_EMPLOYEETYPEID",
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
                name: "PROFIT_SHARE_CHECK",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(15)", precision: 15, nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    CHECK_NUMBER = table.Column<int>(type: "NUMBER(15)", precision: 15, nullable: false),
                    SSN = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    DEMOGRAPHIC_ID = table.Column<int>(type: "NUMBER(9)", precision: 9, nullable: false),
                    PAYABLE_NAME = table.Column<string>(type: "NVARCHAR2(84)", maxLength: 84, nullable: false),
                    CHECK_AMOUNT = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    TAX_CODE_ID = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    CHECK_DATE = table.Column<DateTime>(type: "DATE", nullable: true),
                    VOID_FLAG = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    VOID_CHECK_DATE = table.Column<DateTime>(type: "DATE", nullable: true),
                    VOID_RECON_DATE = table.Column<DateTime>(type: "DATE", nullable: true),
                    CLEAR_DATE = table.Column<DateTime>(type: "DATE", nullable: true),
                    CLEAR_DATE_LOADED = table.Column<DateTime>(type: "DATE", nullable: true),
                    REF_NUMBER = table.Column<int>(type: "NUMBER(10)", maxLength: 36, nullable: true),
                    FLOAT_DAYS = table.Column<short>(type: "NUMBER(6)", precision: 6, nullable: true),
                    CHECK_RUN_DATE = table.Column<DateTime>(type: "DATE", nullable: true),
                    DATE_LOADED = table.Column<DateTime>(type: "DATE", nullable: true),
                    OTHER_BENEFICIARY = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    MANUAL_CHECK = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    REPLACE_CHECK = table.Column<string>(type: "NVARCHAR2(24)", maxLength: 24, nullable: true),
                    PSC_CHECK_ID = table.Column<int>(type: "NUMBER(15)", precision: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROFIT_SHARE_CHECK", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PROFIT_SHARE_CHECK_DEMOGRAPHIC_DEMOGRAPHICID",
                        column: x => x.DEMOGRAPHIC_ID,
                        principalTable: "DEMOGRAPHIC",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_PROFIT_SHARE_CHECK_TAXCODES_TAXCODEID",
                        column: x => x.TAX_CODE_ID,
                        principalTable: "TAX_CODE",
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
                columns: new[] { "ACC_WKEND2_N", "ACC_ALT_KEY_NUM", "ACC_CLN60_PERIOD", "ACC_CLN60_WEEK", "ACC_CLN61_PERIOD", "ACC_CLN61_WEEK", "ACC_CLN6X_PERIOD", "ACC_CLN6X_WEEK", "ACC_CLN7X_PERIOD", "ACC_CLN7X_WEEK", "ACC_APWKEND", "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[,]
                {
                    { 19990109, -407L, 1, 1, 1, 1, 1, 1, 1, 1, 990116, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 19990116, -406L, 1, 2, 1, 2, 1, 2, 1, 2, 990123, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 19990123, -405L, 1, 3, 1, 3, 1, 3, 1, 3, 990130, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 19990130, -404L, 1, 4, 1, 4, 1, 4, 1, 4, 990206, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 19990206, -403L, 2, 5, 2, 5, 2, 5, 2, 5, 990213, (byte)2, (byte)2, (byte)1, (byte)5 },
                    { 19990213, -402L, 2, 6, 2, 6, 2, 6, 2, 6, 990220, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 19990220, -401L, 2, 7, 2, 7, 2, 7, 2, 7, 990227, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 19990227, -400L, 2, 8, 2, 8, 2, 8, 2, 8, 990306, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 19990306, -399L, 3, 9, 3, 9, 3, 9, 3, 9, 990313, (byte)3, (byte)3, (byte)1, (byte)9 },
                    { 19990313, -398L, 3, 10, 3, 10, 3, 10, 3, 10, 990320, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 19990320, -397L, 3, 11, 3, 11, 3, 11, 3, 11, 990327, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 19990327, -396L, 3, 12, 3, 12, 3, 12, 3, 12, 990403, (byte)3, (byte)3, (byte)2, (byte)12 },
                    { 19990403, -395L, 4, 13, 4, 13, 4, 13, 4, 13, 990410, (byte)4, (byte)4, (byte)2, (byte)13 },
                    { 19990410, -394L, 4, 14, 4, 14, 4, 14, 4, 14, 990417, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 19990417, -393L, 4, 15, 4, 15, 4, 15, 4, 15, 990424, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 19990424, -392L, 4, 16, 4, 16, 4, 16, 4, 16, 990501, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 19990501, -391L, 4, 17, 4, 17, 4, 17, 4, 17, 990508, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 19990508, -390L, 5, 18, 5, 18, 5, 18, 5, 18, 990515, (byte)5, (byte)5, (byte)2, (byte)18 },
                    { 19990515, -389L, 5, 19, 5, 19, 5, 19, 5, 19, 990522, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 19990522, -388L, 5, 20, 5, 20, 5, 20, 5, 20, 990529, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 19990529, -387L, 5, 21, 5, 21, 5, 21, 5, 21, 990605, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 19990605, -386L, 6, 22, 6, 22, 6, 22, 6, 22, 990612, (byte)6, (byte)6, (byte)2, (byte)22 },
                    { 19990612, -385L, 6, 23, 6, 23, 6, 23, 6, 23, 990619, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 19990619, -384L, 6, 24, 6, 24, 6, 24, 6, 24, 990626, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 19990626, -383L, 6, 25, 6, 25, 6, 25, 6, 25, 990703, (byte)6, (byte)6, (byte)3, (byte)25 },
                    { 19990703, -382L, 6, 26, 6, 26, 6, 26, 6, 26, 990710, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 19990710, -381L, 7, 27, 7, 27, 7, 27, 7, 27, 990717, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 19990717, -380L, 7, 28, 7, 28, 7, 28, 7, 28, 990724, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 19990724, -379L, 7, 29, 7, 29, 7, 29, 7, 29, 990731, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 19990731, -378L, 7, 30, 7, 30, 7, 30, 7, 30, 990807, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 19990807, -377L, 8, 31, 8, 31, 8, 31, 8, 31, 990814, (byte)8, (byte)8, (byte)3, (byte)31 },
                    { 19990814, -376L, 8, 32, 8, 32, 8, 32, 8, 32, 990821, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 19990821, -375L, 8, 33, 8, 33, 8, 33, 8, 33, 990828, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 19990828, -374L, 8, 34, 8, 34, 8, 34, 8, 34, 990904, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 19990904, -373L, 9, 35, 9, 35, 9, 35, 9, 35, 990911, (byte)9, (byte)9, (byte)3, (byte)35 },
                    { 19990911, -372L, 9, 36, 9, 36, 9, 36, 9, 36, 990918, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 19990918, -371L, 9, 37, 9, 37, 9, 37, 9, 37, 990925, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 19990925, -370L, 9, 38, 9, 38, 9, 38, 9, 38, 991002, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 19991002, -369L, 9, 39, 9, 39, 9, 39, 9, 39, 991009, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 19991009, -368L, 10, 40, 10, 40, 10, 40, 10, 40, 991016, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 19991016, -367L, 10, 41, 10, 41, 10, 41, 10, 41, 991023, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 19991023, -366L, 10, 42, 10, 42, 10, 42, 10, 42, 991030, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 19991030, -365L, 10, 43, 10, 43, 10, 43, 10, 43, 991106, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 19991106, -364L, 11, 44, 11, 44, 11, 44, 11, 44, 991113, (byte)11, (byte)11, (byte)4, (byte)44 },
                    { 19991113, -363L, 11, 45, 11, 45, 11, 45, 11, 45, 991120, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 19991120, -362L, 11, 46, 11, 46, 11, 46, 11, 46, 991127, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 19991127, -361L, 11, 47, 11, 47, 11, 47, 11, 47, 991204, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 19991204, -360L, 12, 48, 12, 48, 12, 48, 12, 48, 991211, (byte)12, (byte)12, (byte)4, (byte)48 },
                    { 19991211, -359L, 12, 49, 12, 49, 12, 49, 12, 49, 991218, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 19991218, -358L, 12, 50, 12, 50, 12, 50, 12, 50, 991225, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 19991225, -357L, 12, 51, 12, 51, 12, 51, 12, 51, 101, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20000101, -356L, 12, 52, 12, 52, 12, 52, 12, 52, 108, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20000108, -347L, 1, 1, 1, 1, 1, 1, 1, 1, 115, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20000115, -346L, 1, 2, 1, 2, 1, 2, 1, 2, 122, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20000122, -345L, 1, 3, 1, 3, 1, 3, 1, 3, 129, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20000129, -344L, 1, 4, 1, 4, 1, 4, 1, 4, 205, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20000205, -343L, 2, 5, 2, 5, 2, 5, 2, 5, 212, (byte)2, (byte)2, (byte)1, (byte)5 },
                    { 20000212, -342L, 2, 6, 2, 6, 2, 6, 2, 6, 219, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20000219, -341L, 2, 7, 2, 7, 2, 7, 2, 7, 226, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20000226, -340L, 2, 8, 2, 8, 2, 8, 2, 8, 304, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20000304, -339L, 3, 9, 3, 9, 3, 9, 3, 9, 311, (byte)3, (byte)3, (byte)1, (byte)9 },
                    { 20000311, -338L, 3, 10, 3, 10, 3, 10, 3, 10, 318, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20000318, -337L, 3, 11, 3, 11, 3, 11, 3, 11, 325, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20000325, -336L, 3, 12, 3, 12, 3, 12, 3, 12, 401, (byte)3, (byte)3, (byte)1, (byte)12 },
                    { 20000401, -335L, 3, 13, 3, 13, 3, 13, 3, 13, 408, (byte)3, (byte)3, (byte)2, (byte)13 },
                    { 20000408, -334L, 4, 14, 4, 14, 4, 14, 4, 14, 415, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20000415, -333L, 4, 15, 4, 15, 4, 15, 4, 15, 422, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20000422, -332L, 4, 16, 4, 16, 4, 16, 4, 16, 429, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20000429, -331L, 4, 17, 4, 17, 4, 17, 4, 17, 506, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20000506, -330L, 5, 18, 5, 18, 5, 18, 5, 18, 513, (byte)5, (byte)5, (byte)2, (byte)18 },
                    { 20000513, -329L, 5, 19, 5, 19, 5, 19, 5, 19, 520, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20000520, -328L, 5, 20, 5, 20, 5, 20, 5, 20, 527, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20000527, -327L, 5, 21, 5, 21, 5, 21, 5, 21, 603, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20000603, -326L, 5, 22, 5, 22, 5, 22, 5, 22, 610, (byte)5, (byte)5, (byte)2, (byte)22 },
                    { 20000610, -325L, 6, 23, 6, 23, 6, 23, 6, 23, 617, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20000617, -324L, 6, 24, 6, 24, 6, 24, 6, 24, 624, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20000624, -323L, 6, 25, 6, 25, 6, 25, 6, 25, 701, (byte)6, (byte)6, (byte)2, (byte)25 },
                    { 20000701, -322L, 6, 26, 6, 26, 6, 26, 6, 26, 708, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20000708, -321L, 7, 27, 7, 27, 7, 27, 7, 27, 715, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20000715, -320L, 7, 28, 7, 28, 7, 28, 7, 28, 722, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20000722, -319L, 7, 29, 7, 29, 7, 29, 7, 29, 729, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20000729, -318L, 7, 30, 7, 30, 7, 30, 7, 30, 805, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20000805, -317L, 8, 31, 8, 31, 8, 31, 8, 31, 812, (byte)8, (byte)8, (byte)3, (byte)31 },
                    { 20000812, -316L, 8, 32, 8, 32, 8, 32, 8, 32, 819, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20000819, -315L, 8, 33, 8, 33, 8, 33, 8, 33, 826, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20000826, -314L, 8, 34, 8, 34, 8, 34, 8, 34, 902, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20000902, -313L, 8, 35, 8, 35, 8, 35, 8, 35, 909, (byte)8, (byte)8, (byte)3, (byte)35 },
                    { 20000909, -312L, 9, 36, 9, 36, 9, 36, 9, 36, 916, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20000916, -311L, 9, 37, 9, 37, 9, 37, 9, 37, 923, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20000923, -310L, 9, 38, 9, 38, 9, 38, 9, 38, 930, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20000930, -309L, 9, 39, 9, 39, 9, 39, 9, 39, 1007, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20001007, -308L, 10, 40, 10, 40, 10, 40, 10, 40, 1014, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20001014, -307L, 10, 41, 10, 41, 10, 41, 10, 41, 1021, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20001021, -306L, 10, 42, 10, 42, 10, 42, 10, 42, 1028, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20001028, -305L, 10, 43, 10, 43, 10, 43, 10, 43, 1104, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20001104, -304L, 11, 44, 11, 44, 11, 44, 11, 44, 1111, (byte)11, (byte)11, (byte)4, (byte)44 },
                    { 20001111, -303L, 11, 45, 11, 45, 11, 45, 11, 45, 1118, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20001118, -302L, 11, 46, 11, 46, 11, 46, 11, 46, 1125, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20001125, -301L, 11, 47, 11, 47, 11, 47, 11, 47, 1202, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20001202, -300L, 11, 48, 11, 48, 11, 48, 11, 48, 1209, (byte)11, (byte)11, (byte)4, (byte)48 },
                    { 20001209, -299L, 12, 49, 12, 49, 12, 49, 12, 49, 1216, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20001216, -298L, 12, 50, 12, 50, 12, 50, 12, 50, 1223, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20001223, -297L, 12, 51, 12, 51, 12, 51, 12, 51, 1230, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20001230, -296L, 12, 52, 12, 52, 12, 52, 12, 52, 10106, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20010106, -287L, 1, 1, 1, 1, 1, 1, 1, 1, 10113, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20010113, -286L, 1, 2, 1, 2, 1, 2, 1, 2, 10120, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20010120, -285L, 1, 3, 1, 3, 1, 3, 1, 3, 10127, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20010127, -284L, 1, 4, 1, 4, 1, 4, 1, 4, 10203, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20010203, -283L, 1, 5, 1, 5, 1, 5, 1, 5, 10210, (byte)1, (byte)1, (byte)1, (byte)5 },
                    { 20010210, -282L, 2, 6, 2, 6, 2, 6, 2, 6, 10217, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20010217, -281L, 2, 7, 2, 7, 2, 7, 2, 7, 10224, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20010224, -280L, 2, 8, 2, 8, 2, 8, 2, 8, 10303, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20010303, -279L, 2, 9, 2, 9, 2, 9, 2, 9, 10310, (byte)2, (byte)2, (byte)1, (byte)9 },
                    { 20010310, -278L, 3, 10, 3, 10, 3, 10, 3, 10, 10317, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20010317, -277L, 3, 11, 3, 11, 3, 11, 3, 11, 10324, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20010324, -276L, 3, 12, 3, 12, 3, 12, 3, 12, 10331, (byte)3, (byte)3, (byte)1, (byte)12 },
                    { 20010331, -275L, 3, 13, 3, 13, 3, 13, 3, 13, 10407, (byte)3, (byte)3, (byte)2, (byte)13 },
                    { 20010407, -274L, 4, 14, 4, 14, 4, 14, 4, 14, 10414, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20010414, -273L, 4, 15, 4, 15, 4, 15, 4, 15, 10421, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20010421, -272L, 4, 16, 4, 16, 4, 16, 4, 16, 10428, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20010428, -271L, 4, 17, 4, 17, 4, 17, 4, 17, 10505, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20010505, -270L, 5, 18, 5, 18, 5, 18, 5, 18, 10512, (byte)5, (byte)5, (byte)2, (byte)18 },
                    { 20010512, -269L, 5, 19, 5, 19, 5, 19, 5, 19, 10519, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20010519, -268L, 5, 20, 5, 20, 5, 20, 5, 20, 10526, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20010526, -267L, 5, 21, 5, 21, 5, 21, 5, 21, 10602, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20010602, -266L, 5, 22, 5, 22, 5, 22, 5, 22, 10609, (byte)5, (byte)5, (byte)2, (byte)22 },
                    { 20010609, -265L, 6, 23, 6, 23, 6, 23, 6, 23, 10616, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20010616, -264L, 6, 24, 6, 24, 6, 24, 6, 24, 10623, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20010623, -263L, 6, 25, 6, 25, 6, 25, 6, 25, 10630, (byte)6, (byte)6, (byte)2, (byte)25 },
                    { 20010630, -262L, 6, 26, 6, 26, 6, 26, 6, 26, 10707, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20010707, -261L, 7, 27, 7, 27, 7, 27, 7, 27, 10714, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20010714, -260L, 7, 28, 7, 28, 7, 28, 7, 28, 10721, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20010721, -259L, 7, 29, 7, 29, 7, 29, 7, 29, 10728, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20010728, -258L, 7, 30, 7, 30, 7, 30, 7, 30, 10804, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20010804, -257L, 8, 31, 8, 31, 8, 31, 8, 31, 10811, (byte)8, (byte)8, (byte)3, (byte)31 },
                    { 20010811, -256L, 8, 32, 8, 32, 8, 32, 8, 32, 10818, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20010818, -255L, 8, 33, 8, 33, 8, 33, 8, 33, 10825, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20010825, -254L, 8, 34, 8, 34, 8, 34, 8, 34, 10901, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20010901, -253L, 8, 35, 8, 35, 8, 35, 8, 35, 10908, (byte)8, (byte)8, (byte)3, (byte)35 },
                    { 20010908, -252L, 9, 36, 9, 36, 9, 36, 9, 36, 10915, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20010915, -251L, 9, 37, 9, 37, 9, 37, 9, 37, 10922, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20010922, -250L, 9, 38, 9, 38, 9, 38, 9, 38, 10929, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20010929, -249L, 9, 39, 9, 39, 9, 39, 9, 39, 11006, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20011006, -248L, 10, 40, 10, 40, 10, 40, 10, 40, 11013, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20011013, -247L, 10, 41, 10, 41, 10, 41, 10, 41, 11020, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20011020, -246L, 10, 42, 10, 42, 10, 42, 10, 42, 11027, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20011027, -245L, 10, 43, 10, 43, 10, 43, 10, 43, 11103, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20011103, -244L, 10, 44, 10, 44, 10, 44, 10, 44, 11110, (byte)10, (byte)10, (byte)4, (byte)44 },
                    { 20011110, -243L, 11, 45, 11, 45, 11, 45, 11, 45, 11117, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20011117, -242L, 11, 46, 11, 46, 11, 46, 11, 46, 11124, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20011124, -241L, 11, 47, 11, 47, 11, 47, 11, 47, 11201, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20011201, -240L, 11, 48, 11, 48, 11, 48, 11, 48, 11208, (byte)11, (byte)11, (byte)4, (byte)48 },
                    { 20011208, -239L, 12, 49, 12, 49, 12, 49, 12, 49, 11215, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20011215, -238L, 12, 50, 12, 50, 12, 50, 12, 50, 11222, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20011222, -237L, 12, 51, 12, 51, 12, 51, 12, 51, 11229, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20011229, -236L, 12, 52, 12, 52, 12, 52, 12, 52, 20105, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20020105, -227L, 1, 1, 1, 1, 1, 1, 1, 1, 20112, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20020112, -226L, 1, 2, 1, 2, 1, 2, 1, 2, 20119, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20020119, -225L, 1, 3, 1, 3, 1, 3, 1, 3, 20126, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20020126, -224L, 1, 4, 1, 4, 1, 4, 1, 4, 20202, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20020202, -223L, 1, 5, 1, 5, 1, 5, 1, 5, 20209, (byte)1, (byte)1, (byte)1, (byte)5 },
                    { 20020209, -222L, 2, 6, 2, 6, 2, 6, 2, 6, 20216, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20020216, -221L, 2, 7, 2, 7, 2, 7, 2, 7, 20223, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20020223, -220L, 2, 8, 2, 8, 2, 8, 2, 8, 20302, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20020302, -219L, 2, 9, 2, 9, 2, 9, 2, 9, 20309, (byte)2, (byte)2, (byte)1, (byte)9 },
                    { 20020309, -218L, 3, 10, 3, 10, 3, 10, 3, 10, 20316, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20020316, -217L, 3, 11, 3, 11, 3, 11, 3, 11, 20323, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20020323, -216L, 3, 12, 3, 12, 3, 12, 3, 12, 20330, (byte)3, (byte)3, (byte)1, (byte)12 },
                    { 20020330, -215L, 3, 13, 3, 13, 3, 13, 3, 13, 20406, (byte)3, (byte)3, (byte)2, (byte)13 },
                    { 20020406, -214L, 4, 14, 4, 14, 4, 14, 4, 14, 20413, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20020413, -213L, 4, 15, 4, 15, 4, 15, 4, 15, 20420, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20020420, -212L, 4, 16, 4, 16, 4, 16, 4, 16, 20427, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20020427, -211L, 4, 17, 4, 17, 4, 17, 4, 17, 20504, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20020504, -210L, 5, 18, 5, 18, 5, 18, 5, 18, 20511, (byte)5, (byte)5, (byte)2, (byte)18 },
                    { 20020511, -209L, 5, 19, 5, 19, 5, 19, 5, 19, 20518, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20020518, -208L, 5, 20, 5, 20, 5, 20, 5, 20, 20525, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20020525, -207L, 5, 21, 5, 21, 5, 21, 5, 21, 20601, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20020601, -206L, 5, 22, 5, 22, 5, 22, 5, 22, 20608, (byte)5, (byte)5, (byte)2, (byte)22 },
                    { 20020608, -205L, 6, 23, 6, 23, 6, 23, 6, 23, 20615, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20020615, -204L, 6, 24, 6, 24, 6, 24, 6, 24, 20622, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20020622, -203L, 6, 25, 6, 25, 6, 25, 6, 25, 20629, (byte)6, (byte)6, (byte)2, (byte)25 },
                    { 20020629, -202L, 6, 26, 6, 26, 6, 26, 6, 26, 20706, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20020706, -201L, 7, 27, 7, 27, 7, 27, 7, 27, 20713, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20020713, -200L, 7, 28, 7, 28, 7, 28, 7, 28, 20720, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20020720, -199L, 7, 29, 7, 29, 7, 29, 7, 29, 20727, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20020727, -198L, 7, 30, 7, 30, 7, 30, 7, 30, 20803, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20020803, -197L, 8, 31, 8, 31, 8, 31, 8, 31, 20810, (byte)8, (byte)8, (byte)3, (byte)31 },
                    { 20020810, -196L, 8, 32, 8, 32, 8, 32, 8, 32, 20817, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20020817, -195L, 8, 33, 8, 33, 8, 33, 8, 33, 20824, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20020824, -194L, 8, 34, 8, 34, 8, 34, 8, 34, 20831, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20020831, -193L, 8, 35, 8, 35, 8, 35, 8, 35, 20907, (byte)8, (byte)8, (byte)3, (byte)35 },
                    { 20020907, -192L, 9, 36, 9, 36, 9, 36, 9, 36, 20914, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20020914, -191L, 9, 37, 9, 37, 9, 37, 9, 37, 20921, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20020921, -190L, 9, 38, 9, 38, 9, 38, 9, 38, 20928, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20020928, -189L, 9, 39, 9, 39, 9, 39, 9, 39, 21005, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20021005, -188L, 10, 40, 10, 40, 10, 40, 10, 40, 21012, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20021012, -187L, 10, 41, 10, 41, 10, 41, 10, 41, 21019, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20021019, -186L, 10, 42, 10, 42, 10, 42, 10, 42, 21026, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20021026, -185L, 10, 43, 10, 43, 10, 43, 10, 43, 21102, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20021102, -184L, 10, 44, 10, 44, 10, 44, 10, 44, 21109, (byte)10, (byte)10, (byte)4, (byte)44 },
                    { 20021109, -183L, 11, 45, 11, 45, 11, 45, 11, 45, 21116, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20021116, -182L, 11, 46, 11, 46, 11, 46, 11, 46, 21123, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20021123, -181L, 11, 47, 11, 47, 11, 47, 11, 47, 21130, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20021130, -180L, 11, 48, 11, 48, 11, 48, 11, 48, 21207, (byte)11, (byte)11, (byte)4, (byte)48 },
                    { 20021207, -179L, 12, 49, 12, 49, 12, 49, 12, 49, 21214, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20021214, -178L, 12, 50, 12, 50, 12, 50, 12, 50, 21221, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20021221, -177L, 12, 51, 12, 51, 12, 51, 12, 51, 21228, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20021228, -176L, 12, 52, 12, 52, 12, 52, 12, 52, 30104, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20030104, -167L, 1, 1, 1, 1, 1, 1, 1, 1, 30111, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20030111, -166L, 1, 2, 1, 2, 1, 2, 1, 2, 30118, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20030118, -165L, 1, 3, 1, 3, 1, 3, 1, 3, 30125, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20030125, -164L, 1, 4, 1, 4, 1, 4, 1, 4, 30201, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20030201, -163L, 1, 5, 1, 5, 1, 5, 1, 5, 30208, (byte)1, (byte)1, (byte)1, (byte)5 },
                    { 20030208, -162L, 2, 6, 2, 6, 2, 6, 2, 6, 30215, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20030215, -161L, 2, 7, 2, 7, 2, 7, 2, 7, 30222, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20030222, -160L, 2, 8, 2, 8, 2, 8, 2, 8, 30301, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20030301, -159L, 2, 9, 2, 9, 2, 9, 2, 9, 30308, (byte)2, (byte)2, (byte)1, (byte)9 },
                    { 20030308, -158L, 3, 10, 3, 10, 3, 10, 3, 10, 30315, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20030315, -157L, 3, 11, 3, 11, 3, 11, 3, 11, 30322, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20030322, -156L, 3, 12, 3, 12, 3, 12, 3, 12, 30329, (byte)3, (byte)3, (byte)1, (byte)12 },
                    { 20030329, -155L, 3, 13, 3, 13, 3, 13, 3, 13, 30405, (byte)3, (byte)3, (byte)2, (byte)13 },
                    { 20030405, -154L, 4, 14, 4, 14, 4, 14, 4, 14, 30412, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20030412, -153L, 4, 15, 4, 15, 4, 15, 4, 15, 30419, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20030419, -152L, 4, 16, 4, 16, 4, 16, 4, 16, 30426, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20030426, -151L, 4, 17, 4, 17, 4, 17, 4, 17, 30503, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20030503, -150L, 4, 18, 4, 18, 4, 18, 4, 18, 30510, (byte)4, (byte)4, (byte)2, (byte)18 },
                    { 20030510, -149L, 5, 19, 5, 19, 5, 19, 5, 19, 30517, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20030517, -148L, 5, 20, 5, 20, 5, 20, 5, 20, 30524, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20030524, -147L, 5, 21, 5, 21, 5, 21, 5, 21, 30531, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20030531, -146L, 5, 22, 5, 22, 5, 22, 5, 22, 30607, (byte)5, (byte)5, (byte)2, (byte)22 },
                    { 20030607, -145L, 6, 23, 6, 23, 6, 23, 6, 23, 30614, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20030614, -144L, 6, 24, 6, 24, 6, 24, 6, 24, 30621, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20030621, -143L, 6, 25, 6, 25, 6, 25, 6, 25, 30628, (byte)6, (byte)6, (byte)2, (byte)25 },
                    { 20030628, -142L, 6, 26, 6, 26, 6, 26, 6, 26, 30705, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20030705, -141L, 7, 27, 7, 27, 7, 27, 7, 27, 30712, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20030712, -140L, 7, 28, 7, 28, 7, 28, 7, 28, 30719, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20030719, -139L, 7, 29, 7, 29, 7, 29, 7, 29, 30726, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20030726, -138L, 7, 30, 7, 30, 7, 30, 7, 30, 30802, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20030802, -137L, 7, 31, 7, 31, 7, 31, 7, 31, 30809, (byte)7, (byte)7, (byte)3, (byte)31 },
                    { 20030809, -136L, 8, 32, 8, 32, 8, 32, 8, 32, 30816, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20030816, -135L, 8, 33, 8, 33, 8, 33, 8, 33, 30823, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20030823, -134L, 8, 34, 8, 34, 8, 34, 8, 34, 30830, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20030830, -133L, 8, 35, 8, 35, 8, 35, 8, 35, 30906, (byte)8, (byte)8, (byte)3, (byte)35 },
                    { 20030906, -132L, 9, 36, 9, 36, 9, 36, 9, 36, 30913, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20030913, -131L, 9, 37, 9, 37, 9, 37, 9, 37, 30920, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20030920, -130L, 9, 38, 9, 38, 9, 38, 9, 38, 30927, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20030927, -129L, 9, 39, 9, 39, 9, 39, 9, 39, 31004, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20031004, -128L, 10, 40, 10, 40, 10, 40, 10, 40, 31011, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20031011, -127L, 10, 41, 10, 41, 10, 41, 10, 41, 31018, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20031018, -126L, 10, 42, 10, 42, 10, 42, 10, 42, 31025, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20031025, -125L, 10, 43, 10, 43, 10, 43, 10, 43, 31101, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20031101, -124L, 10, 44, 10, 44, 10, 44, 10, 44, 31108, (byte)10, (byte)10, (byte)4, (byte)44 },
                    { 20031108, -123L, 11, 45, 11, 45, 11, 45, 11, 45, 31115, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20031115, -122L, 11, 46, 11, 46, 11, 46, 11, 46, 31122, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20031122, -121L, 11, 47, 11, 47, 11, 47, 11, 47, 31129, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20031129, -120L, 11, 48, 11, 48, 11, 48, 11, 48, 31206, (byte)11, (byte)11, (byte)4, (byte)48 },
                    { 20031206, -119L, 12, 49, 12, 49, 12, 49, 12, 49, 31213, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20031213, -118L, 12, 50, 12, 50, 12, 50, 12, 50, 31220, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20031220, -117L, 12, 51, 12, 51, 12, 51, 12, 51, 31227, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20031227, -116L, 12, 52, 12, 52, 12, 52, 12, 52, 40103, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20040103, -115L, 12, 53, 12, 53, 12, 53, 12, 53, 40110, (byte)12, (byte)12, (byte)1, (byte)53 },
                    { 20040110, -107L, 1, 1, 1, 1, 1, 1, 1, 1, 40117, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20040117, -106L, 1, 2, 1, 2, 1, 2, 1, 2, 40124, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20040124, -105L, 1, 3, 1, 3, 1, 3, 1, 3, 40131, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20040131, -104L, 1, 4, 1, 4, 1, 4, 1, 4, 40207, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20040207, -103L, 2, 5, 2, 5, 2, 5, 2, 5, 40214, (byte)2, (byte)2, (byte)1, (byte)5 },
                    { 20040214, -102L, 2, 6, 2, 6, 2, 6, 2, 6, 40221, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20040221, -101L, 2, 7, 2, 7, 2, 7, 2, 7, 40228, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20040228, -100L, 2, 8, 2, 8, 2, 8, 2, 8, 40306, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20040306, -99L, 3, 9, 3, 9, 3, 9, 3, 9, 40313, (byte)3, (byte)3, (byte)1, (byte)9 },
                    { 20040313, -98L, 3, 10, 3, 10, 3, 10, 3, 10, 40320, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20040320, -97L, 3, 11, 3, 11, 3, 11, 3, 11, 40327, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20040327, -96L, 3, 12, 3, 12, 3, 12, 3, 12, 40403, (byte)3, (byte)3, (byte)2, (byte)12 },
                    { 20040403, -95L, 3, 13, 3, 13, 3, 13, 3, 13, 40410, (byte)3, (byte)3, (byte)2, (byte)13 },
                    { 20040410, -94L, 4, 14, 4, 14, 4, 14, 4, 14, 40417, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20040417, -93L, 4, 15, 4, 15, 4, 15, 4, 15, 40424, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20040424, -92L, 4, 16, 4, 16, 4, 16, 4, 16, 40501, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20040501, -91L, 4, 17, 4, 17, 4, 17, 4, 17, 40508, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20040508, -90L, 5, 18, 5, 18, 5, 18, 5, 18, 40515, (byte)5, (byte)5, (byte)2, (byte)18 },
                    { 20040515, -89L, 5, 19, 5, 19, 5, 19, 5, 19, 40522, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20040522, -88L, 5, 20, 5, 20, 5, 20, 5, 20, 40529, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20040529, -87L, 5, 21, 5, 21, 5, 21, 5, 21, 40605, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20040605, -86L, 6, 22, 6, 22, 6, 22, 6, 22, 40612, (byte)6, (byte)6, (byte)2, (byte)22 },
                    { 20040612, -85L, 6, 23, 6, 23, 6, 23, 6, 23, 40619, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20040619, -84L, 6, 24, 6, 24, 6, 24, 6, 24, 40626, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20040626, -83L, 6, 25, 6, 25, 6, 25, 6, 25, 40703, (byte)6, (byte)6, (byte)3, (byte)25 },
                    { 20040703, -82L, 6, 26, 6, 26, 6, 26, 6, 26, 40710, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20040710, -81L, 7, 27, 7, 27, 7, 27, 7, 27, 40717, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20040717, -80L, 7, 28, 7, 28, 7, 28, 7, 28, 40724, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20040724, -79L, 7, 29, 7, 29, 7, 29, 7, 29, 40731, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20040731, -78L, 7, 30, 7, 30, 7, 30, 7, 30, 40807, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20040807, -77L, 8, 31, 8, 31, 8, 31, 8, 31, 40814, (byte)8, (byte)8, (byte)3, (byte)31 },
                    { 20040814, -76L, 8, 32, 8, 32, 8, 32, 8, 32, 40821, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20040821, -75L, 8, 33, 8, 33, 8, 33, 8, 33, 40828, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20040828, -74L, 8, 34, 8, 34, 8, 34, 8, 34, 40904, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20040904, -73L, 9, 35, 9, 35, 9, 35, 9, 35, 40911, (byte)9, (byte)9, (byte)3, (byte)35 },
                    { 20040911, -72L, 9, 36, 9, 36, 9, 36, 9, 36, 40918, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20040918, -71L, 9, 37, 9, 37, 9, 37, 9, 37, 40925, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20040925, -70L, 9, 38, 9, 38, 9, 38, 9, 38, 41002, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20041002, -69L, 9, 39, 9, 39, 9, 39, 9, 39, 41009, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20041009, -68L, 10, 40, 10, 40, 10, 40, 10, 40, 41016, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20041016, -67L, 10, 41, 10, 41, 10, 41, 10, 41, 41023, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20041023, -66L, 10, 42, 10, 42, 10, 42, 10, 42, 41030, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20041030, -65L, 10, 43, 10, 43, 10, 43, 10, 43, 41106, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20041106, -64L, 11, 44, 11, 44, 11, 44, 11, 44, 41113, (byte)11, (byte)11, (byte)4, (byte)44 },
                    { 20041113, -63L, 11, 45, 11, 45, 11, 45, 11, 45, 41120, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20041120, -62L, 11, 46, 11, 46, 11, 46, 11, 46, 41127, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20041127, -61L, 11, 47, 11, 47, 11, 47, 11, 47, 41204, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20041204, -60L, 12, 48, 12, 48, 12, 48, 12, 48, 41211, (byte)12, (byte)12, (byte)4, (byte)48 },
                    { 20041211, -59L, 12, 49, 12, 49, 12, 49, 12, 49, 41218, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20041218, -58L, 12, 50, 12, 50, 12, 50, 12, 50, 41225, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20041225, -57L, 12, 51, 12, 51, 12, 51, 12, 51, 50101, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20050101, -56L, 12, 52, 12, 52, 12, 52, 12, 52, 50108, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20050108, -47L, 1, 1, 1, 1, 1, 1, 1, 1, 50115, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20050115, -46L, 1, 2, 1, 2, 1, 2, 1, 2, 50122, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20050122, -45L, 1, 3, 1, 3, 1, 3, 1, 3, 50129, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20050129, -44L, 1, 4, 1, 4, 1, 4, 1, 4, 50205, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20050205, -43L, 2, 5, 2, 5, 2, 5, 2, 5, 50212, (byte)2, (byte)2, (byte)1, (byte)5 },
                    { 20050212, -42L, 2, 6, 2, 6, 2, 6, 2, 6, 50219, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20050219, -41L, 2, 7, 2, 7, 2, 7, 2, 7, 50226, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20050226, -40L, 2, 8, 2, 8, 2, 8, 2, 8, 50305, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20050305, -39L, 3, 9, 3, 9, 3, 9, 3, 9, 50312, (byte)3, (byte)3, (byte)1, (byte)9 },
                    { 20050312, -38L, 3, 10, 3, 10, 3, 10, 3, 10, 50319, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20050319, -37L, 3, 11, 3, 11, 3, 11, 3, 11, 50326, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20050326, -36L, 3, 12, 3, 12, 3, 12, 3, 12, 50402, (byte)3, (byte)3, (byte)1, (byte)12 },
                    { 20050402, -35L, 3, 13, 3, 13, 3, 13, 3, 13, 50409, (byte)3, (byte)3, (byte)2, (byte)13 },
                    { 20050409, -34L, 4, 14, 4, 14, 4, 14, 4, 14, 50416, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20050416, -33L, 4, 15, 4, 15, 4, 15, 4, 15, 50423, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20050423, -32L, 4, 16, 4, 16, 4, 16, 4, 16, 50430, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20050430, -31L, 4, 17, 4, 17, 4, 17, 4, 17, 50507, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20050507, -30L, 5, 18, 5, 18, 5, 18, 5, 18, 50514, (byte)5, (byte)5, (byte)2, (byte)18 },
                    { 20050514, -29L, 5, 19, 5, 19, 5, 19, 5, 19, 50521, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20050521, -28L, 5, 20, 5, 20, 5, 20, 5, 20, 50528, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20050528, -27L, 5, 21, 5, 21, 5, 21, 5, 21, 50604, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20050604, -26L, 6, 22, 6, 22, 6, 22, 6, 22, 50611, (byte)6, (byte)6, (byte)2, (byte)22 },
                    { 20050611, -25L, 6, 23, 6, 23, 6, 23, 6, 23, 50618, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20050618, -24L, 6, 24, 6, 24, 6, 24, 6, 24, 50625, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20050625, -23L, 6, 25, 6, 25, 6, 25, 6, 25, 50702, (byte)6, (byte)6, (byte)2, (byte)25 },
                    { 20050702, -22L, 6, 26, 6, 26, 6, 26, 6, 26, 50709, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20050709, -21L, 7, 27, 7, 27, 7, 27, 7, 27, 50716, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20050716, -20L, 7, 28, 7, 28, 7, 28, 7, 28, 50723, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20050723, -19L, 7, 29, 7, 29, 7, 29, 7, 29, 50730, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20050730, -18L, 7, 30, 7, 30, 7, 30, 7, 30, 50806, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20050806, -17L, 8, 31, 8, 31, 8, 31, 8, 31, 50813, (byte)8, (byte)8, (byte)3, (byte)31 },
                    { 20050813, -16L, 8, 32, 8, 32, 8, 32, 8, 32, 50820, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20050820, -15L, 8, 33, 8, 33, 8, 33, 8, 33, 50827, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20050827, -14L, 8, 34, 8, 34, 8, 34, 8, 34, 50903, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20050903, -13L, 8, 35, 8, 35, 8, 35, 8, 35, 50910, (byte)8, (byte)8, (byte)3, (byte)35 },
                    { 20050910, -12L, 9, 36, 9, 36, 9, 36, 9, 36, 50917, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20050917, -11L, 9, 37, 9, 37, 9, 37, 9, 37, 50924, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20050924, -10L, 9, 38, 9, 38, 9, 38, 9, 38, 51001, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20051001, -9L, 9, 39, 9, 39, 9, 39, 9, 39, 51008, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20051008, -8L, 10, 40, 10, 40, 10, 40, 10, 40, 51015, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20051015, -7L, 10, 41, 10, 41, 10, 41, 10, 41, 51022, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20051022, -6L, 10, 42, 10, 42, 10, 42, 10, 42, 51029, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20051029, -5L, 10, 43, 10, 43, 10, 43, 10, 43, 51105, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20051105, -4L, 11, 44, 11, 44, 11, 44, 11, 44, 51112, (byte)11, (byte)11, (byte)4, (byte)44 },
                    { 20051112, -3L, 11, 45, 11, 45, 11, 45, 11, 45, 51119, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20051119, -2L, 11, 46, 11, 46, 11, 46, 11, 46, 51126, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20051126, -1L, 11, 47, 11, 47, 11, 47, 11, 47, 51203, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20051203, 0L, 11, 48, 11, 48, 11, 48, 11, 48, 51210, (byte)11, (byte)11, (byte)4, (byte)48 },
                    { 20051210, 1L, 12, 49, 12, 49, 12, 49, 12, 49, 51217, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20051217, 2L, 12, 50, 12, 50, 12, 50, 12, 50, 51224, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20051224, 3L, 12, 51, 12, 51, 12, 51, 12, 51, 51231, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20051231, 4L, 12, 52, 12, 52, 12, 52, 12, 52, 60107, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20060107, 13L, 1, 1, 1, 1, 1, 1, 1, 1, 60114, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20060114, 14L, 1, 2, 1, 2, 1, 2, 1, 2, 60121, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20060121, 15L, 1, 3, 1, 3, 1, 3, 1, 3, 60128, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20060128, 16L, 1, 4, 1, 4, 1, 4, 1, 4, 60204, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20060204, 17L, 2, 5, 2, 5, 2, 5, 2, 5, 60211, (byte)2, (byte)2, (byte)1, (byte)5 },
                    { 20060211, 18L, 2, 6, 2, 6, 2, 6, 2, 6, 60218, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20060218, 19L, 2, 7, 2, 7, 2, 7, 2, 7, 60225, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20060225, 20L, 2, 8, 2, 8, 2, 8, 2, 8, 60304, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20060304, 21L, 3, 9, 3, 9, 3, 9, 3, 9, 60311, (byte)3, (byte)3, (byte)1, (byte)9 },
                    { 20060311, 22L, 3, 10, 3, 10, 3, 10, 3, 10, 60318, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20060318, 23L, 3, 11, 3, 11, 3, 11, 3, 11, 60325, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20060325, 24L, 3, 12, 3, 12, 3, 12, 3, 12, 60401, (byte)3, (byte)3, (byte)1, (byte)12 },
                    { 20060401, 25L, 3, 13, 3, 13, 3, 13, 3, 13, 60408, (byte)3, (byte)3, (byte)2, (byte)13 },
                    { 20060408, 26L, 4, 14, 4, 14, 4, 14, 4, 14, 60415, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20060415, 27L, 4, 15, 4, 15, 4, 15, 4, 15, 60422, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20060422, 28L, 4, 16, 4, 16, 4, 16, 4, 16, 60429, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20060429, 29L, 4, 17, 4, 17, 4, 17, 4, 17, 60506, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20060506, 30L, 5, 18, 5, 18, 5, 18, 5, 18, 60513, (byte)5, (byte)5, (byte)2, (byte)18 },
                    { 20060513, 31L, 5, 19, 5, 19, 5, 19, 5, 19, 60520, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20060520, 32L, 5, 20, 5, 20, 5, 20, 5, 20, 60527, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20060527, 33L, 5, 21, 5, 21, 5, 21, 5, 21, 60603, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20060603, 34L, 5, 22, 5, 22, 5, 22, 5, 22, 60610, (byte)5, (byte)5, (byte)2, (byte)22 },
                    { 20060610, 35L, 6, 23, 6, 23, 6, 23, 6, 23, 60617, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20060617, 36L, 6, 24, 6, 24, 6, 24, 6, 24, 60624, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20060624, 37L, 6, 25, 6, 25, 6, 25, 6, 25, 60701, (byte)6, (byte)6, (byte)2, (byte)25 },
                    { 20060701, 38L, 6, 26, 6, 26, 6, 26, 6, 26, 60708, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20060708, 39L, 7, 27, 7, 27, 7, 27, 7, 27, 60715, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20060715, 40L, 7, 28, 7, 28, 7, 28, 7, 28, 60722, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20060722, 41L, 7, 29, 7, 29, 7, 29, 7, 29, 60729, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20060729, 42L, 7, 30, 7, 30, 7, 30, 7, 30, 60805, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20060805, 43L, 8, 31, 8, 31, 8, 31, 8, 31, 60812, (byte)8, (byte)8, (byte)3, (byte)31 },
                    { 20060812, 44L, 8, 32, 8, 32, 8, 32, 8, 32, 60819, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20060819, 45L, 8, 33, 8, 33, 8, 33, 8, 33, 60826, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20060826, 46L, 8, 34, 8, 34, 8, 34, 8, 34, 60902, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20060902, 47L, 8, 35, 8, 35, 8, 35, 8, 35, 60909, (byte)8, (byte)8, (byte)3, (byte)35 },
                    { 20060909, 48L, 9, 36, 9, 36, 9, 36, 9, 36, 60916, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20060916, 49L, 9, 37, 9, 37, 9, 37, 9, 37, 60923, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20060923, 50L, 9, 38, 9, 38, 9, 38, 9, 38, 60930, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20060930, 51L, 9, 39, 9, 39, 9, 39, 9, 39, 61007, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20061007, 52L, 10, 40, 10, 40, 10, 40, 10, 40, 61014, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20061014, 53L, 10, 41, 10, 41, 10, 41, 10, 41, 61021, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20061021, 54L, 10, 42, 10, 42, 10, 42, 10, 42, 61028, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20061028, 55L, 10, 43, 10, 43, 10, 43, 10, 43, 61104, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20061104, 56L, 11, 44, 11, 44, 11, 44, 11, 44, 61111, (byte)11, (byte)11, (byte)4, (byte)44 },
                    { 20061111, 57L, 11, 45, 11, 45, 11, 45, 11, 45, 61118, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20061118, 58L, 11, 46, 11, 46, 11, 46, 11, 46, 61125, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20061125, 59L, 11, 47, 11, 47, 11, 47, 11, 47, 61202, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20061202, 60L, 11, 48, 11, 48, 11, 48, 11, 48, 61209, (byte)11, (byte)11, (byte)4, (byte)48 },
                    { 20061209, 61L, 12, 49, 12, 49, 12, 49, 12, 49, 61216, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20061216, 62L, 12, 50, 12, 50, 12, 50, 12, 50, 61223, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20061223, 63L, 12, 51, 12, 51, 12, 51, 12, 51, 61230, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20061230, 64L, 12, 52, 12, 52, 12, 52, 12, 52, 70106, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20070106, 73L, 1, 1, 1, 1, 1, 1, 1, 1, 70113, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20070113, 74L, 1, 2, 1, 2, 1, 2, 1, 2, 70120, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20070120, 75L, 1, 3, 1, 3, 1, 3, 1, 3, 70127, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20070127, 76L, 1, 4, 1, 4, 1, 4, 1, 4, 70203, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20070203, 77L, 1, 5, 1, 5, 1, 5, 1, 5, 70210, (byte)1, (byte)1, (byte)1, (byte)5 },
                    { 20070210, 78L, 2, 6, 2, 6, 2, 6, 2, 6, 70217, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20070217, 79L, 2, 7, 2, 7, 2, 7, 2, 7, 70224, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20070224, 80L, 2, 8, 2, 8, 2, 8, 2, 8, 70303, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20070303, 81L, 2, 9, 2, 9, 2, 9, 2, 9, 70310, (byte)2, (byte)2, (byte)1, (byte)9 },
                    { 20070310, 82L, 3, 10, 3, 10, 3, 10, 3, 10, 70317, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20070317, 83L, 3, 11, 3, 11, 3, 11, 3, 11, 70324, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20070324, 84L, 3, 12, 3, 12, 3, 12, 3, 12, 70331, (byte)3, (byte)3, (byte)1, (byte)12 },
                    { 20070331, 85L, 3, 13, 3, 13, 3, 13, 3, 13, 70407, (byte)3, (byte)3, (byte)2, (byte)13 },
                    { 20070407, 86L, 4, 14, 4, 14, 4, 14, 4, 14, 70414, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20070414, 87L, 4, 15, 4, 15, 4, 15, 4, 15, 70421, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20070421, 88L, 4, 16, 4, 16, 4, 16, 4, 16, 70428, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20070428, 89L, 4, 17, 4, 17, 4, 17, 4, 17, 70505, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20070505, 90L, 5, 18, 5, 18, 5, 18, 5, 18, 70512, (byte)5, (byte)5, (byte)2, (byte)18 },
                    { 20070512, 91L, 5, 19, 5, 19, 5, 19, 5, 19, 70519, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20070519, 92L, 5, 20, 5, 20, 5, 20, 5, 20, 70526, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20070526, 93L, 5, 21, 5, 21, 5, 21, 5, 21, 70602, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20070602, 94L, 5, 22, 5, 22, 5, 22, 5, 22, 70609, (byte)5, (byte)5, (byte)2, (byte)22 },
                    { 20070609, 95L, 6, 23, 6, 23, 6, 23, 6, 23, 70616, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20070616, 96L, 6, 24, 6, 24, 6, 24, 6, 24, 70623, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20070623, 97L, 6, 25, 6, 25, 6, 25, 6, 25, 70630, (byte)6, (byte)6, (byte)2, (byte)25 },
                    { 20070630, 98L, 6, 26, 6, 26, 6, 26, 6, 26, 70707, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20070707, 99L, 7, 27, 7, 27, 7, 27, 7, 27, 70714, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20070714, 100L, 7, 28, 7, 28, 7, 28, 7, 28, 70721, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20070721, 101L, 7, 29, 7, 29, 7, 29, 7, 29, 70728, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20070728, 102L, 7, 30, 7, 30, 7, 30, 7, 30, 70804, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20070804, 103L, 8, 31, 8, 31, 8, 31, 8, 31, 70811, (byte)8, (byte)8, (byte)3, (byte)31 },
                    { 20070811, 104L, 8, 32, 8, 32, 8, 32, 8, 32, 70818, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20070818, 105L, 8, 33, 8, 33, 8, 33, 8, 33, 70825, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20070825, 106L, 8, 34, 8, 34, 8, 34, 8, 34, 70901, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20070901, 107L, 8, 35, 8, 35, 8, 35, 8, 35, 70908, (byte)8, (byte)8, (byte)3, (byte)35 },
                    { 20070908, 108L, 9, 36, 9, 36, 9, 36, 9, 36, 70915, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20070915, 109L, 9, 37, 9, 37, 9, 37, 9, 37, 70922, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20070922, 110L, 9, 38, 9, 38, 9, 38, 9, 38, 70929, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20070929, 111L, 9, 39, 9, 39, 9, 39, 9, 39, 71006, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20071006, 112L, 10, 40, 10, 40, 10, 40, 10, 40, 71013, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20071013, 113L, 10, 41, 10, 41, 10, 41, 10, 41, 71020, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20071020, 114L, 10, 42, 10, 42, 10, 42, 10, 42, 71027, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20071027, 115L, 10, 43, 10, 43, 10, 43, 10, 43, 71103, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20071103, 116L, 10, 44, 10, 44, 10, 44, 10, 44, 71110, (byte)10, (byte)10, (byte)4, (byte)44 },
                    { 20071110, 117L, 11, 45, 11, 45, 11, 45, 11, 45, 71117, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20071117, 118L, 11, 46, 11, 46, 11, 46, 11, 46, 71124, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20071124, 119L, 11, 47, 11, 47, 11, 47, 11, 47, 71201, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20071201, 120L, 11, 48, 11, 48, 11, 48, 11, 48, 71208, (byte)11, (byte)11, (byte)4, (byte)48 },
                    { 20071208, 121L, 12, 49, 12, 49, 12, 49, 12, 49, 71215, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20071215, 122L, 12, 50, 12, 50, 12, 50, 12, 50, 71222, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20071222, 123L, 12, 51, 12, 51, 12, 51, 12, 51, 71229, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20071229, 124L, 12, 52, 12, 52, 12, 52, 12, 52, 80105, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20080105, 133L, 1, 1, 1, 1, 1, 1, 1, 1, 80112, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20080112, 134L, 1, 2, 1, 2, 1, 2, 1, 2, 80119, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20080119, 135L, 1, 3, 1, 3, 1, 3, 1, 3, 80126, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20080126, 136L, 1, 4, 1, 4, 1, 4, 1, 4, 80202, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20080202, 137L, 1, 5, 1, 5, 1, 5, 1, 5, 80209, (byte)1, (byte)1, (byte)1, (byte)5 },
                    { 20080209, 138L, 2, 6, 2, 6, 2, 6, 2, 6, 80216, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20080216, 139L, 2, 7, 2, 7, 2, 7, 2, 7, 80223, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20080223, 140L, 2, 8, 2, 8, 2, 8, 2, 8, 80301, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20080301, 141L, 2, 9, 2, 9, 2, 9, 2, 9, 80308, (byte)2, (byte)2, (byte)1, (byte)9 },
                    { 20080308, 142L, 3, 10, 3, 10, 3, 10, 3, 10, 80315, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20080315, 143L, 3, 11, 3, 11, 3, 11, 3, 11, 80322, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20080322, 144L, 3, 12, 3, 12, 3, 12, 3, 12, 80329, (byte)3, (byte)3, (byte)1, (byte)12 },
                    { 20080329, 145L, 3, 13, 3, 13, 3, 13, 3, 13, 80405, (byte)3, (byte)3, (byte)2, (byte)13 },
                    { 20080405, 146L, 4, 14, 4, 14, 4, 14, 4, 14, 80412, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20080412, 147L, 4, 15, 4, 15, 4, 15, 4, 15, 80419, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20080419, 148L, 4, 16, 4, 16, 4, 16, 4, 16, 80426, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20080426, 149L, 4, 17, 4, 17, 4, 17, 4, 17, 80503, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20080503, 150L, 4, 18, 4, 18, 4, 18, 4, 18, 80510, (byte)4, (byte)4, (byte)2, (byte)18 },
                    { 20080510, 151L, 5, 19, 5, 19, 5, 19, 5, 19, 80517, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20080517, 152L, 5, 20, 5, 20, 5, 20, 5, 20, 80524, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20080524, 153L, 5, 21, 5, 21, 5, 21, 5, 21, 80531, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20080531, 154L, 5, 22, 5, 22, 5, 22, 5, 22, 80607, (byte)5, (byte)5, (byte)2, (byte)22 },
                    { 20080607, 155L, 6, 23, 6, 23, 6, 23, 6, 23, 80614, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20080614, 156L, 6, 24, 6, 24, 6, 24, 6, 24, 80621, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20080621, 157L, 6, 25, 6, 25, 6, 25, 6, 25, 80628, (byte)6, (byte)6, (byte)2, (byte)25 },
                    { 20080628, 158L, 6, 26, 6, 26, 6, 26, 6, 26, 80705, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20080705, 159L, 7, 27, 7, 27, 7, 27, 7, 27, 80712, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20080712, 160L, 7, 28, 7, 28, 7, 28, 7, 28, 80719, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20080719, 161L, 7, 29, 7, 29, 7, 29, 7, 29, 80726, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20080726, 162L, 7, 30, 7, 30, 7, 30, 7, 30, 80802, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20080802, 163L, 7, 31, 7, 31, 7, 31, 7, 31, 80809, (byte)7, (byte)7, (byte)3, (byte)31 },
                    { 20080809, 164L, 8, 32, 8, 32, 8, 32, 8, 32, 80816, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20080816, 165L, 8, 33, 8, 33, 8, 33, 8, 33, 80823, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20080823, 166L, 8, 34, 8, 34, 8, 34, 8, 34, 80830, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20080830, 167L, 8, 35, 8, 35, 8, 35, 8, 35, 80906, (byte)8, (byte)8, (byte)3, (byte)35 },
                    { 20080906, 168L, 9, 36, 9, 36, 9, 36, 9, 36, 80913, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20080913, 169L, 9, 37, 9, 37, 9, 37, 9, 37, 80920, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20080920, 170L, 9, 38, 9, 38, 9, 38, 9, 38, 80927, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20080927, 171L, 9, 39, 9, 39, 9, 39, 9, 39, 81004, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20081004, 172L, 10, 40, 10, 40, 10, 40, 10, 40, 81011, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20081011, 173L, 10, 41, 10, 41, 10, 41, 10, 41, 81018, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20081018, 174L, 10, 42, 10, 42, 10, 42, 10, 42, 81025, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20081025, 175L, 10, 43, 10, 43, 10, 43, 10, 43, 81101, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20081101, 176L, 10, 44, 10, 44, 10, 44, 10, 44, 81108, (byte)10, (byte)10, (byte)4, (byte)44 },
                    { 20081108, 177L, 11, 45, 11, 45, 11, 45, 11, 45, 81115, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20081115, 178L, 11, 46, 11, 46, 11, 46, 11, 46, 81122, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20081122, 179L, 11, 47, 11, 47, 11, 47, 11, 47, 81129, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20081129, 180L, 11, 48, 11, 48, 11, 48, 11, 48, 81206, (byte)11, (byte)11, (byte)4, (byte)48 },
                    { 20081206, 181L, 12, 49, 12, 49, 12, 49, 12, 49, 81213, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20081213, 182L, 12, 50, 12, 50, 12, 50, 12, 50, 81220, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20081220, 183L, 12, 51, 12, 51, 12, 51, 12, 51, 81227, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20081227, 184L, 12, 52, 12, 52, 12, 52, 12, 52, 90103, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20090103, 185L, 12, 53, 12, 53, 12, 53, 12, 53, 90110, (byte)12, (byte)12, (byte)1, (byte)53 },
                    { 20090110, 193L, 1, 1, 1, 1, 1, 1, 1, 1, 90117, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20090117, 194L, 1, 2, 1, 2, 1, 2, 1, 2, 90124, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20090124, 195L, 1, 3, 1, 3, 1, 3, 1, 3, 90131, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20090131, 196L, 1, 4, 1, 4, 1, 4, 1, 4, 90207, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20090207, 197L, 2, 5, 2, 5, 2, 5, 2, 5, 90214, (byte)2, (byte)2, (byte)1, (byte)5 },
                    { 20090214, 198L, 2, 6, 2, 6, 2, 6, 2, 6, 90221, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20090221, 199L, 2, 7, 2, 7, 2, 7, 2, 7, 90228, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20090228, 200L, 2, 8, 2, 8, 2, 8, 2, 8, 90307, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20090307, 201L, 3, 9, 3, 9, 3, 9, 3, 9, 90314, (byte)3, (byte)3, (byte)1, (byte)9 },
                    { 20090314, 202L, 3, 10, 3, 10, 3, 10, 3, 10, 90321, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20090321, 203L, 3, 11, 3, 11, 3, 11, 3, 11, 90328, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20090328, 204L, 3, 12, 3, 12, 3, 12, 3, 12, 90404, (byte)3, (byte)3, (byte)2, (byte)12 },
                    { 20090404, 205L, 4, 13, 4, 13, 4, 13, 4, 13, 90411, (byte)4, (byte)4, (byte)2, (byte)13 },
                    { 20090411, 206L, 4, 14, 4, 14, 4, 14, 4, 14, 90418, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20090418, 207L, 4, 15, 4, 15, 4, 15, 4, 15, 90425, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20090425, 208L, 4, 16, 4, 16, 4, 16, 4, 16, 90502, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20090502, 209L, 4, 17, 4, 17, 4, 17, 4, 17, 90509, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20090509, 210L, 5, 18, 5, 18, 5, 18, 5, 18, 90516, (byte)5, (byte)5, (byte)2, (byte)18 },
                    { 20090516, 211L, 5, 19, 5, 19, 5, 19, 5, 19, 90523, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20090523, 212L, 5, 20, 5, 20, 5, 20, 5, 20, 90530, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20090530, 213L, 5, 21, 5, 21, 5, 21, 5, 21, 90606, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20090606, 214L, 6, 22, 6, 22, 6, 22, 6, 22, 90613, (byte)6, (byte)6, (byte)2, (byte)22 },
                    { 20090613, 215L, 6, 23, 6, 23, 6, 23, 6, 23, 90620, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20090620, 216L, 6, 24, 6, 24, 6, 24, 6, 24, 90627, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20090627, 217L, 6, 25, 6, 25, 6, 25, 6, 25, 90704, (byte)6, (byte)6, (byte)3, (byte)25 },
                    { 20090704, 218L, 7, 26, 7, 26, 7, 26, 7, 26, 90711, (byte)7, (byte)7, (byte)3, (byte)26 },
                    { 20090711, 219L, 7, 27, 7, 27, 7, 27, 7, 27, 90718, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20090718, 220L, 7, 28, 7, 28, 7, 28, 7, 28, 90725, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20090725, 221L, 7, 29, 7, 29, 7, 29, 7, 29, 90801, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20090801, 222L, 7, 30, 7, 30, 7, 30, 7, 30, 90808, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20090808, 223L, 8, 31, 8, 31, 8, 31, 8, 31, 90815, (byte)8, (byte)8, (byte)3, (byte)31 },
                    { 20090815, 224L, 8, 32, 8, 32, 8, 32, 8, 32, 90822, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20090822, 225L, 8, 33, 8, 33, 8, 33, 8, 33, 90829, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20090829, 226L, 8, 34, 8, 34, 8, 34, 8, 34, 90905, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20090905, 227L, 9, 35, 9, 35, 9, 35, 9, 35, 90912, (byte)9, (byte)9, (byte)3, (byte)35 },
                    { 20090912, 228L, 9, 36, 9, 36, 9, 36, 9, 36, 90919, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20090919, 229L, 9, 37, 9, 37, 9, 37, 9, 37, 90926, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20090926, 230L, 9, 38, 9, 38, 9, 38, 9, 38, 91003, (byte)9, (byte)9, (byte)4, (byte)38 },
                    { 20091003, 231L, 9, 39, 9, 39, 9, 39, 9, 39, 91010, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20091010, 232L, 10, 40, 10, 40, 10, 40, 10, 40, 91017, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20091017, 233L, 10, 41, 10, 41, 10, 41, 10, 41, 91024, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20091024, 234L, 10, 42, 10, 42, 10, 42, 10, 42, 91031, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20091031, 235L, 10, 43, 10, 43, 10, 43, 10, 43, 91107, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20091107, 236L, 11, 44, 11, 44, 11, 44, 11, 44, 91114, (byte)11, (byte)11, (byte)4, (byte)44 },
                    { 20091114, 237L, 11, 45, 11, 45, 11, 45, 11, 45, 91121, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20091121, 238L, 11, 46, 11, 46, 11, 46, 11, 46, 91128, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20091128, 239L, 11, 47, 11, 47, 11, 47, 11, 47, 91205, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20091205, 240L, 12, 48, 12, 48, 12, 48, 12, 48, 91212, (byte)12, (byte)12, (byte)4, (byte)48 },
                    { 20091212, 241L, 12, 49, 12, 49, 12, 49, 12, 49, 91219, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20091219, 242L, 12, 50, 12, 50, 12, 50, 12, 50, 91226, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20091226, 243L, 12, 51, 12, 51, 12, 51, 12, 51, 100102, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20100102, 244L, 12, 52, 12, 52, 12, 52, 12, 52, 100109, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20100109, 253L, 1, 1, 1, 1, 1, 1, 1, 1, 100116, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20100116, 254L, 1, 2, 1, 2, 1, 2, 1, 2, 100123, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20100123, 255L, 1, 3, 1, 3, 1, 3, 1, 3, 100130, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20100130, 256L, 1, 4, 1, 4, 1, 4, 1, 4, 100206, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20100206, 257L, 2, 5, 2, 5, 2, 5, 2, 5, 100213, (byte)2, (byte)2, (byte)1, (byte)5 },
                    { 20100213, 258L, 2, 6, 2, 6, 2, 6, 2, 6, 100220, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20100220, 259L, 2, 7, 2, 7, 2, 7, 2, 7, 100227, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20100227, 260L, 2, 8, 2, 8, 2, 8, 2, 8, 100306, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20100306, 261L, 3, 9, 3, 9, 3, 9, 3, 9, 100313, (byte)3, (byte)3, (byte)1, (byte)9 },
                    { 20100313, 262L, 3, 10, 3, 10, 3, 10, 3, 10, 100320, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20100320, 263L, 3, 11, 3, 11, 3, 11, 3, 11, 100327, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20100327, 264L, 3, 12, 3, 12, 3, 12, 3, 12, 100403, (byte)3, (byte)3, (byte)2, (byte)12 },
                    { 20100403, 265L, 4, 13, 4, 13, 4, 13, 4, 13, 100410, (byte)4, (byte)4, (byte)2, (byte)13 },
                    { 20100410, 266L, 4, 14, 4, 14, 4, 14, 4, 14, 100417, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20100417, 267L, 4, 15, 4, 15, 4, 15, 4, 15, 100424, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20100424, 268L, 4, 16, 4, 16, 4, 16, 4, 16, 100501, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20100501, 269L, 4, 17, 4, 17, 4, 17, 4, 17, 100508, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20100508, 270L, 5, 18, 5, 18, 5, 18, 5, 18, 100515, (byte)5, (byte)5, (byte)2, (byte)18 },
                    { 20100515, 271L, 5, 19, 5, 19, 5, 19, 5, 19, 100522, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20100522, 272L, 5, 20, 5, 20, 5, 20, 5, 20, 100529, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20100529, 273L, 5, 21, 5, 21, 5, 21, 5, 21, 100605, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20100605, 274L, 6, 22, 6, 22, 6, 22, 6, 22, 100612, (byte)6, (byte)6, (byte)2, (byte)22 },
                    { 20100612, 275L, 6, 23, 6, 23, 6, 23, 6, 23, 100619, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20100619, 276L, 6, 24, 6, 24, 6, 24, 6, 24, 100626, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20100626, 277L, 6, 25, 6, 25, 6, 25, 6, 25, 100703, (byte)6, (byte)6, (byte)3, (byte)25 },
                    { 20100703, 278L, 6, 26, 6, 26, 6, 26, 6, 26, 100710, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20100710, 279L, 7, 27, 7, 27, 7, 27, 7, 27, 100717, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20100717, 280L, 7, 28, 7, 28, 7, 28, 7, 28, 100724, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20100724, 281L, 7, 29, 7, 29, 7, 29, 7, 29, 100731, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20100731, 282L, 7, 30, 7, 30, 7, 30, 7, 30, 100807, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20100807, 283L, 8, 31, 8, 31, 8, 31, 8, 31, 100814, (byte)8, (byte)8, (byte)3, (byte)31 },
                    { 20100814, 284L, 8, 32, 8, 32, 8, 32, 8, 32, 100821, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20100821, 285L, 8, 33, 8, 33, 8, 33, 8, 33, 100828, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20100828, 286L, 8, 34, 8, 34, 8, 34, 8, 34, 100904, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20100904, 287L, 9, 35, 9, 35, 9, 35, 9, 35, 100911, (byte)9, (byte)9, (byte)3, (byte)35 },
                    { 20100911, 288L, 9, 36, 9, 36, 9, 36, 9, 36, 100918, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20100918, 289L, 9, 37, 9, 37, 9, 37, 9, 37, 100925, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20100925, 290L, 9, 38, 9, 38, 9, 38, 9, 38, 101002, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20101002, 291L, 9, 39, 9, 39, 9, 39, 9, 39, 101009, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20101009, 292L, 10, 40, 10, 40, 10, 40, 10, 40, 101016, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20101016, 293L, 10, 41, 10, 41, 10, 41, 10, 41, 101023, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20101023, 294L, 10, 42, 10, 42, 10, 42, 10, 42, 101030, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20101030, 295L, 10, 43, 10, 43, 10, 43, 10, 43, 101106, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20101106, 296L, 11, 44, 11, 44, 11, 44, 11, 44, 101113, (byte)11, (byte)11, (byte)4, (byte)44 },
                    { 20101113, 297L, 11, 45, 11, 45, 11, 45, 11, 45, 101120, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20101120, 298L, 11, 46, 11, 46, 11, 46, 11, 46, 101127, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20101127, 299L, 11, 47, 11, 47, 11, 47, 11, 47, 101204, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20101204, 300L, 12, 48, 12, 48, 12, 48, 12, 48, 101211, (byte)12, (byte)12, (byte)4, (byte)48 },
                    { 20101211, 301L, 12, 49, 12, 49, 12, 49, 12, 49, 101218, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20101218, 302L, 12, 50, 12, 50, 12, 50, 12, 50, 101225, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20101225, 303L, 12, 51, 12, 51, 12, 51, 12, 51, 110101, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20110101, 304L, 12, 52, 12, 52, 12, 52, 12, 52, 110108, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20110108, 313L, 1, 1, 1, 1, 1, 1, 1, 1, 110115, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20110115, 314L, 1, 2, 1, 2, 1, 2, 1, 2, 110122, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20110122, 315L, 1, 3, 1, 3, 1, 3, 1, 3, 110129, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20110129, 316L, 1, 4, 1, 4, 1, 4, 1, 4, 110205, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20110205, 317L, 2, 5, 2, 5, 2, 5, 2, 5, 110212, (byte)2, (byte)2, (byte)1, (byte)5 },
                    { 20110212, 318L, 2, 6, 2, 6, 2, 6, 2, 6, 110219, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20110219, 319L, 2, 7, 2, 7, 2, 7, 2, 7, 110226, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20110226, 320L, 2, 8, 2, 8, 2, 8, 2, 8, 110305, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20110305, 321L, 3, 9, 3, 9, 3, 9, 3, 9, 110312, (byte)3, (byte)3, (byte)1, (byte)9 },
                    { 20110312, 322L, 3, 10, 3, 10, 3, 10, 3, 10, 110319, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20110319, 323L, 3, 11, 3, 11, 3, 11, 3, 11, 110326, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20110326, 324L, 3, 12, 3, 12, 3, 12, 3, 12, 110402, (byte)3, (byte)3, (byte)1, (byte)12 },
                    { 20110402, 325L, 3, 13, 3, 13, 3, 13, 3, 13, 110409, (byte)3, (byte)3, (byte)2, (byte)13 },
                    { 20110409, 326L, 4, 14, 4, 14, 4, 14, 4, 14, 110416, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20110416, 327L, 4, 15, 4, 15, 4, 15, 4, 15, 110423, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20110423, 328L, 4, 16, 4, 16, 4, 16, 4, 16, 110430, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20110430, 329L, 4, 17, 4, 17, 4, 17, 4, 17, 110507, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20110507, 330L, 5, 18, 5, 18, 5, 18, 5, 18, 110514, (byte)5, (byte)5, (byte)2, (byte)18 },
                    { 20110514, 331L, 5, 19, 5, 19, 5, 19, 5, 19, 110521, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20110521, 332L, 5, 20, 5, 20, 5, 20, 5, 20, 110528, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20110528, 333L, 5, 21, 5, 21, 5, 21, 5, 21, 110604, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20110604, 334L, 6, 22, 6, 22, 6, 22, 6, 22, 110611, (byte)6, (byte)6, (byte)2, (byte)22 },
                    { 20110611, 335L, 6, 23, 6, 23, 6, 23, 6, 23, 110618, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20110618, 336L, 6, 24, 6, 24, 6, 24, 6, 24, 110625, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20110625, 337L, 6, 25, 6, 25, 6, 25, 6, 25, 110702, (byte)6, (byte)6, (byte)2, (byte)25 },
                    { 20110702, 338L, 6, 26, 6, 26, 6, 26, 6, 26, 110709, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20110709, 339L, 7, 27, 7, 27, 7, 27, 7, 27, 110716, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20110716, 340L, 7, 28, 7, 28, 7, 28, 7, 28, 110723, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20110723, 341L, 7, 29, 7, 29, 7, 29, 7, 29, 110730, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20110730, 342L, 7, 30, 7, 30, 7, 30, 7, 30, 110806, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20110806, 343L, 8, 31, 8, 31, 8, 31, 8, 31, 110813, (byte)8, (byte)8, (byte)3, (byte)31 },
                    { 20110813, 344L, 8, 32, 8, 32, 8, 32, 8, 32, 110820, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20110820, 345L, 8, 33, 8, 33, 8, 33, 8, 33, 110827, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20110827, 346L, 8, 34, 8, 34, 8, 34, 8, 34, 110903, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20110903, 347L, 8, 35, 8, 35, 8, 35, 8, 35, 110910, (byte)8, (byte)8, (byte)3, (byte)35 },
                    { 20110910, 348L, 9, 36, 9, 36, 9, 36, 9, 36, 110917, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20110917, 349L, 9, 37, 9, 37, 9, 37, 9, 37, 110924, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20110924, 350L, 9, 38, 9, 38, 9, 38, 9, 38, 111001, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20111001, 351L, 9, 39, 9, 39, 9, 39, 9, 39, 111008, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20111008, 352L, 10, 40, 10, 40, 10, 40, 10, 40, 111015, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20111015, 353L, 10, 41, 10, 41, 10, 41, 10, 41, 111022, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20111022, 354L, 10, 42, 10, 42, 10, 42, 10, 42, 111029, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20111029, 355L, 10, 43, 10, 43, 10, 43, 10, 43, 111105, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20111105, 356L, 11, 44, 11, 44, 11, 44, 11, 44, 111112, (byte)11, (byte)11, (byte)4, (byte)44 },
                    { 20111112, 357L, 11, 45, 11, 45, 11, 45, 11, 45, 111119, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20111119, 358L, 11, 46, 11, 46, 11, 46, 11, 46, 111126, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20111126, 359L, 11, 47, 11, 47, 11, 47, 11, 47, 111203, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20111203, 360L, 11, 48, 11, 48, 11, 48, 11, 48, 111210, (byte)11, (byte)11, (byte)4, (byte)48 },
                    { 20111210, 361L, 12, 49, 12, 49, 12, 49, 12, 49, 111217, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20111217, 362L, 12, 50, 12, 50, 12, 50, 12, 50, 111224, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20111224, 363L, 12, 51, 12, 51, 12, 51, 12, 51, 111231, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20111231, 364L, 12, 52, 12, 52, 12, 52, 12, 52, 120107, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20120107, 373L, 1, 1, 1, 1, 1, 1, 1, 1, 120114, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20120114, 374L, 1, 2, 1, 2, 1, 2, 1, 2, 120121, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20120121, 375L, 1, 3, 1, 3, 1, 3, 1, 3, 120128, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20120128, 376L, 1, 4, 1, 4, 1, 4, 1, 4, 120204, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20120204, 377L, 2, 5, 2, 5, 2, 5, 2, 5, 120211, (byte)2, (byte)2, (byte)1, (byte)5 },
                    { 20120211, 378L, 2, 6, 2, 6, 2, 6, 2, 6, 120218, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20120218, 379L, 2, 7, 2, 7, 2, 7, 2, 7, 120225, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20120225, 380L, 2, 8, 2, 8, 2, 8, 2, 8, 120303, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20120303, 381L, 2, 9, 2, 9, 2, 9, 2, 9, 120310, (byte)2, (byte)2, (byte)1, (byte)9 },
                    { 20120310, 382L, 3, 10, 3, 10, 3, 10, 3, 10, 120317, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20120317, 383L, 3, 11, 3, 11, 3, 11, 3, 11, 120324, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20120324, 384L, 3, 12, 3, 12, 3, 12, 3, 12, 120331, (byte)3, (byte)3, (byte)1, (byte)12 },
                    { 20120331, 385L, 3, 13, 3, 13, 3, 13, 3, 13, 120407, (byte)3, (byte)3, (byte)2, (byte)13 },
                    { 20120407, 386L, 4, 14, 4, 14, 4, 14, 4, 14, 120414, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20120414, 387L, 4, 15, 4, 15, 4, 15, 4, 15, 120421, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20120421, 388L, 4, 16, 4, 16, 4, 16, 4, 16, 120428, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20120428, 389L, 4, 17, 4, 17, 4, 17, 4, 17, 120505, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20120505, 390L, 5, 18, 5, 18, 5, 18, 5, 18, 120512, (byte)5, (byte)5, (byte)2, (byte)18 },
                    { 20120512, 391L, 5, 19, 5, 19, 5, 19, 5, 19, 120519, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20120519, 392L, 5, 20, 5, 20, 5, 20, 5, 20, 120526, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20120526, 393L, 5, 21, 5, 21, 5, 21, 5, 21, 120602, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20120602, 394L, 5, 22, 5, 22, 5, 22, 5, 22, 120609, (byte)5, (byte)5, (byte)2, (byte)22 },
                    { 20120609, 395L, 6, 23, 6, 23, 6, 23, 6, 23, 120616, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20120616, 396L, 6, 24, 6, 24, 6, 24, 6, 24, 120623, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20120623, 397L, 6, 25, 6, 25, 6, 25, 6, 25, 120630, (byte)6, (byte)6, (byte)2, (byte)25 },
                    { 20120630, 398L, 6, 26, 6, 26, 6, 26, 6, 26, 120707, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20120707, 399L, 7, 27, 7, 27, 7, 27, 7, 27, 120714, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20120714, 400L, 7, 28, 7, 28, 7, 28, 7, 28, 120721, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20120721, 401L, 7, 29, 7, 29, 7, 29, 7, 29, 120728, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20120728, 402L, 7, 30, 7, 30, 7, 30, 7, 30, 120804, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20120804, 403L, 8, 31, 8, 31, 8, 31, 8, 31, 120811, (byte)8, (byte)8, (byte)3, (byte)31 },
                    { 20120811, 404L, 8, 32, 8, 32, 8, 32, 8, 32, 120818, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20120818, 405L, 8, 33, 8, 33, 8, 33, 8, 33, 120825, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20120825, 406L, 8, 34, 8, 34, 8, 34, 8, 34, 120901, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20120901, 407L, 8, 35, 8, 35, 8, 35, 8, 35, 120908, (byte)8, (byte)8, (byte)3, (byte)35 },
                    { 20120908, 408L, 9, 36, 9, 36, 9, 36, 9, 36, 120915, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20120915, 409L, 9, 37, 9, 37, 9, 37, 9, 37, 120922, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20120922, 410L, 9, 38, 9, 38, 9, 38, 9, 38, 120929, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20120929, 411L, 9, 39, 9, 39, 9, 39, 9, 39, 121006, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20121006, 412L, 10, 40, 10, 40, 10, 40, 10, 40, 121013, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20121013, 413L, 10, 41, 10, 41, 10, 41, 10, 41, 121020, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20121020, 414L, 10, 42, 10, 42, 10, 42, 10, 42, 121027, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20121027, 415L, 10, 43, 10, 43, 10, 43, 10, 43, 121103, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20121103, 416L, 10, 44, 10, 44, 10, 44, 10, 44, 121110, (byte)10, (byte)10, (byte)4, (byte)44 },
                    { 20121110, 417L, 11, 45, 11, 45, 11, 45, 11, 45, 121117, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20121117, 418L, 11, 46, 11, 46, 11, 46, 11, 46, 121124, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20121124, 419L, 11, 47, 11, 47, 11, 47, 11, 47, 121201, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20121201, 420L, 11, 48, 11, 48, 11, 48, 11, 48, 121208, (byte)11, (byte)11, (byte)4, (byte)48 },
                    { 20121208, 421L, 12, 49, 12, 49, 12, 49, 12, 49, 121215, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20121215, 422L, 12, 50, 12, 50, 12, 50, 12, 50, 121222, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20121222, 423L, 12, 51, 12, 51, 12, 51, 12, 51, 121229, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20121229, 424L, 12, 52, 12, 52, 12, 52, 12, 52, 130105, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20130105, 433L, 1, 1, 1, 1, 1, 1, 1, 1, 130112, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20130112, 434L, 1, 2, 1, 2, 1, 2, 1, 2, 130119, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20130119, 435L, 1, 3, 1, 3, 1, 3, 1, 3, 130126, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20130126, 436L, 1, 4, 1, 4, 1, 4, 1, 4, 130202, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20130202, 437L, 1, 5, 1, 5, 1, 5, 1, 5, 130209, (byte)1, (byte)1, (byte)1, (byte)5 },
                    { 20130209, 438L, 2, 6, 2, 6, 2, 6, 2, 6, 130216, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20130216, 439L, 2, 7, 2, 7, 2, 7, 2, 7, 130223, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20130223, 440L, 2, 8, 2, 8, 2, 8, 2, 8, 130302, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20130302, 441L, 2, 9, 2, 9, 2, 9, 2, 9, 130309, (byte)2, (byte)2, (byte)1, (byte)9 },
                    { 20130309, 442L, 3, 10, 3, 10, 3, 10, 3, 10, 130316, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20130316, 443L, 3, 11, 3, 11, 3, 11, 3, 11, 130323, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20130323, 444L, 3, 12, 3, 12, 3, 12, 3, 12, 130330, (byte)3, (byte)3, (byte)1, (byte)12 },
                    { 20130330, 445L, 3, 13, 3, 13, 3, 13, 3, 13, 130406, (byte)3, (byte)3, (byte)2, (byte)13 },
                    { 20130406, 446L, 4, 14, 4, 14, 4, 14, 4, 14, 130413, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20130413, 447L, 4, 15, 4, 15, 4, 15, 4, 15, 130420, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20130420, 448L, 4, 16, 4, 16, 4, 16, 4, 16, 130427, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20130427, 449L, 4, 17, 4, 17, 4, 17, 4, 17, 130504, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20130504, 450L, 5, 18, 5, 18, 5, 18, 5, 18, 130511, (byte)5, (byte)5, (byte)2, (byte)18 },
                    { 20130511, 451L, 5, 19, 5, 19, 5, 19, 5, 19, 130518, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20130518, 452L, 5, 20, 5, 20, 5, 20, 5, 20, 130525, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20130525, 453L, 5, 21, 5, 21, 5, 21, 5, 21, 130601, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20130601, 454L, 5, 22, 5, 22, 5, 22, 5, 22, 130608, (byte)5, (byte)5, (byte)2, (byte)22 },
                    { 20130608, 455L, 6, 23, 6, 23, 6, 23, 6, 23, 130615, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20130615, 456L, 6, 24, 6, 24, 6, 24, 6, 24, 130622, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20130622, 457L, 6, 25, 6, 25, 6, 25, 6, 25, 130629, (byte)6, (byte)6, (byte)2, (byte)25 },
                    { 20130629, 458L, 6, 26, 6, 26, 6, 26, 6, 26, 130706, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20130706, 459L, 7, 27, 7, 27, 7, 27, 7, 27, 130713, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20130713, 460L, 7, 28, 7, 28, 7, 28, 7, 28, 130720, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20130720, 461L, 7, 29, 7, 29, 7, 29, 7, 29, 130727, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20130727, 462L, 7, 30, 7, 30, 7, 30, 7, 30, 130803, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20130803, 463L, 7, 31, 7, 31, 7, 31, 7, 31, 130810, (byte)7, (byte)7, (byte)3, (byte)31 },
                    { 20130810, 464L, 8, 32, 8, 32, 8, 32, 8, 32, 130817, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20130817, 465L, 8, 33, 8, 33, 8, 33, 8, 33, 130824, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20130824, 466L, 8, 34, 8, 34, 8, 34, 8, 34, 130831, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20130831, 467L, 8, 35, 8, 35, 8, 35, 8, 35, 130907, (byte)8, (byte)8, (byte)3, (byte)35 },
                    { 20130907, 468L, 9, 36, 9, 36, 9, 36, 9, 36, 130914, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20130914, 469L, 9, 37, 9, 37, 9, 37, 9, 37, 130921, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20130921, 470L, 9, 38, 9, 38, 9, 38, 9, 38, 130928, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20130928, 471L, 9, 39, 9, 39, 9, 39, 9, 39, 131005, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20131005, 472L, 10, 40, 10, 40, 10, 40, 10, 40, 131012, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20131012, 473L, 10, 41, 10, 41, 10, 41, 10, 41, 131019, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20131019, 474L, 10, 42, 10, 42, 10, 42, 10, 42, 131026, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20131026, 475L, 10, 43, 10, 43, 10, 43, 10, 43, 131102, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20131102, 476L, 10, 44, 10, 44, 10, 44, 10, 44, 131109, (byte)10, (byte)10, (byte)4, (byte)44 },
                    { 20131109, 477L, 11, 45, 11, 45, 11, 45, 11, 45, 131116, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20131116, 478L, 11, 46, 11, 46, 11, 46, 11, 46, 131123, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20131123, 479L, 11, 47, 11, 47, 11, 47, 11, 47, 131130, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20131130, 480L, 11, 48, 11, 48, 11, 48, 11, 48, 131207, (byte)11, (byte)11, (byte)4, (byte)48 },
                    { 20131207, 481L, 12, 49, 12, 49, 12, 49, 12, 49, 131214, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20131214, 482L, 12, 50, 12, 50, 12, 50, 12, 50, 131221, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20131221, 483L, 12, 51, 12, 51, 12, 51, 12, 51, 131228, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20131228, 484L, 12, 52, 12, 52, 12, 52, 12, 52, 140104, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20140104, 493L, 1, 1, 1, 1, 1, 1, 1, 1, 140111, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20140111, 494L, 1, 2, 1, 2, 1, 2, 1, 2, 140118, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20140118, 495L, 1, 3, 1, 3, 1, 3, 1, 3, 140125, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20140125, 496L, 1, 4, 1, 4, 1, 4, 1, 4, 140201, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20140201, 497L, 1, 5, 1, 5, 1, 5, 1, 5, 140208, (byte)1, (byte)1, (byte)1, (byte)5 },
                    { 20140208, 498L, 2, 6, 2, 6, 2, 6, 2, 6, 140215, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20140215, 499L, 2, 7, 2, 7, 2, 7, 2, 7, 140222, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20140222, 500L, 2, 8, 2, 8, 2, 8, 2, 8, 140301, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20140301, 501L, 2, 9, 2, 9, 2, 9, 2, 9, 140308, (byte)2, (byte)2, (byte)1, (byte)9 },
                    { 20140308, 502L, 3, 10, 3, 10, 3, 10, 3, 10, 140315, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20140315, 503L, 3, 11, 3, 11, 3, 11, 3, 11, 140322, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20140322, 504L, 3, 12, 3, 12, 3, 12, 3, 12, 140329, (byte)3, (byte)3, (byte)1, (byte)12 },
                    { 20140329, 505L, 3, 13, 3, 13, 3, 13, 3, 13, 140405, (byte)3, (byte)3, (byte)2, (byte)13 },
                    { 20140405, 506L, 4, 14, 4, 14, 4, 14, 4, 14, 140412, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20140412, 507L, 4, 15, 4, 15, 4, 15, 4, 15, 140419, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20140419, 508L, 4, 16, 4, 16, 4, 16, 4, 16, 140426, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20140426, 509L, 4, 17, 4, 17, 4, 17, 4, 17, 140503, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20140503, 510L, 4, 18, 4, 18, 4, 18, 4, 18, 140510, (byte)4, (byte)4, (byte)2, (byte)18 },
                    { 20140510, 511L, 5, 19, 5, 19, 5, 19, 5, 19, 140517, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20140517, 512L, 5, 20, 5, 20, 5, 20, 5, 20, 140524, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20140524, 513L, 5, 21, 5, 21, 5, 21, 5, 21, 140531, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20140531, 514L, 5, 22, 5, 22, 5, 22, 5, 22, 140607, (byte)5, (byte)5, (byte)2, (byte)22 },
                    { 20140607, 515L, 6, 23, 6, 23, 6, 23, 6, 23, 140614, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20140614, 516L, 6, 24, 6, 24, 6, 24, 6, 24, 140621, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20140621, 517L, 6, 25, 6, 25, 6, 25, 6, 25, 140628, (byte)6, (byte)6, (byte)2, (byte)25 },
                    { 20140628, 518L, 6, 26, 6, 26, 6, 26, 6, 26, 140705, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20140705, 519L, 7, 27, 7, 27, 7, 27, 7, 27, 140712, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20140712, 520L, 7, 28, 7, 28, 7, 28, 7, 28, 140719, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20140719, 521L, 7, 29, 7, 29, 7, 29, 7, 29, 140726, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20140726, 522L, 7, 30, 7, 30, 7, 30, 7, 30, 140802, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20140802, 523L, 7, 31, 7, 31, 7, 31, 7, 31, 140809, (byte)7, (byte)7, (byte)3, (byte)31 },
                    { 20140809, 524L, 8, 32, 8, 32, 8, 32, 8, 32, 140816, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20140816, 525L, 8, 33, 8, 33, 8, 33, 8, 33, 140823, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20140823, 526L, 8, 34, 8, 34, 8, 34, 8, 34, 140830, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20140830, 527L, 8, 35, 8, 35, 8, 35, 8, 35, 140906, (byte)8, (byte)8, (byte)3, (byte)35 },
                    { 20140906, 528L, 9, 36, 9, 36, 9, 36, 9, 36, 140913, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20140913, 529L, 9, 37, 9, 37, 9, 37, 9, 37, 140920, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20140920, 530L, 9, 38, 9, 38, 9, 38, 9, 38, 140927, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20140927, 531L, 9, 39, 9, 39, 9, 39, 9, 39, 141004, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20141004, 532L, 10, 40, 10, 40, 10, 40, 10, 40, 141011, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20141011, 533L, 10, 41, 10, 41, 10, 41, 10, 41, 141018, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20141018, 534L, 10, 42, 10, 42, 10, 42, 10, 42, 141025, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20141025, 535L, 10, 43, 10, 43, 10, 43, 10, 43, 141101, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20141101, 536L, 10, 44, 10, 44, 10, 44, 10, 44, 141108, (byte)10, (byte)10, (byte)4, (byte)44 },
                    { 20141108, 537L, 11, 45, 11, 45, 11, 45, 11, 45, 141115, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20141115, 538L, 11, 46, 11, 46, 11, 46, 11, 46, 141122, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20141122, 539L, 11, 47, 11, 47, 11, 47, 11, 47, 141129, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20141129, 540L, 11, 48, 11, 48, 11, 48, 11, 48, 141206, (byte)11, (byte)11, (byte)4, (byte)48 },
                    { 20141206, 541L, 12, 49, 12, 49, 12, 49, 12, 49, 141213, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20141213, 542L, 12, 50, 12, 50, 12, 50, 12, 50, 141220, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20141220, 543L, 12, 51, 12, 51, 12, 51, 12, 51, 141227, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20141227, 544L, 12, 52, 12, 52, 12, 52, 12, 52, 150103, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20150103, 545L, 12, 53, 12, 53, 12, 53, 12, 53, 150110, (byte)12, (byte)12, (byte)1, (byte)53 },
                    { 20150110, 553L, 1, 1, 1, 1, 1, 1, 1, 1, 150117, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20150117, 554L, 1, 2, 1, 2, 1, 2, 1, 2, 150124, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20150124, 555L, 1, 3, 1, 3, 1, 3, 1, 3, 150131, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20150131, 556L, 1, 4, 1, 4, 1, 4, 1, 4, 150207, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20150207, 557L, 2, 5, 2, 5, 2, 5, 2, 5, 150214, (byte)2, (byte)2, (byte)1, (byte)5 },
                    { 20150214, 558L, 2, 6, 2, 6, 2, 6, 2, 6, 150221, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20150221, 559L, 2, 7, 2, 7, 2, 7, 2, 7, 150228, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20150228, 560L, 2, 8, 2, 8, 2, 8, 2, 8, 150307, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20150307, 561L, 3, 9, 3, 9, 3, 9, 3, 9, 150314, (byte)3, (byte)3, (byte)1, (byte)9 },
                    { 20150314, 562L, 3, 10, 3, 10, 3, 10, 3, 10, 150321, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20150321, 563L, 3, 11, 3, 11, 3, 11, 3, 11, 150328, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20150328, 564L, 3, 12, 3, 12, 3, 12, 3, 12, 150404, (byte)3, (byte)3, (byte)2, (byte)12 },
                    { 20150404, 565L, 4, 13, 4, 13, 4, 13, 4, 13, 150411, (byte)4, (byte)4, (byte)2, (byte)13 },
                    { 20150411, 566L, 4, 14, 4, 14, 4, 14, 4, 14, 150418, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20150418, 567L, 4, 15, 4, 15, 4, 15, 4, 15, 150425, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20150425, 568L, 4, 16, 4, 16, 4, 16, 4, 16, 150502, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20150502, 569L, 4, 17, 4, 17, 4, 17, 4, 17, 150509, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20150509, 570L, 5, 18, 5, 18, 5, 18, 5, 18, 150516, (byte)5, (byte)5, (byte)2, (byte)18 },
                    { 20150516, 571L, 5, 19, 5, 19, 5, 19, 5, 19, 150523, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20150523, 572L, 5, 20, 5, 20, 5, 20, 5, 20, 150530, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20150530, 573L, 5, 21, 5, 21, 5, 21, 5, 21, 150606, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20150606, 574L, 6, 22, 6, 22, 6, 22, 6, 22, 150613, (byte)6, (byte)6, (byte)2, (byte)22 },
                    { 20150613, 575L, 6, 23, 6, 23, 6, 23, 6, 23, 150620, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20150620, 576L, 6, 24, 6, 24, 6, 24, 6, 24, 150627, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20150627, 577L, 6, 25, 6, 25, 6, 25, 6, 25, 150704, (byte)6, (byte)6, (byte)3, (byte)25 },
                    { 20150704, 578L, 7, 26, 7, 26, 7, 26, 7, 26, 150711, (byte)7, (byte)7, (byte)3, (byte)26 },
                    { 20150711, 579L, 7, 27, 7, 27, 7, 27, 7, 27, 150718, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20150718, 580L, 7, 28, 7, 28, 7, 28, 7, 28, 150725, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20150725, 581L, 7, 29, 7, 29, 7, 29, 7, 29, 150801, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20150801, 582L, 7, 30, 7, 30, 7, 30, 7, 30, 150808, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20150808, 583L, 8, 31, 8, 31, 8, 31, 8, 31, 150815, (byte)8, (byte)8, (byte)3, (byte)31 },
                    { 20150815, 584L, 8, 32, 8, 32, 8, 32, 8, 32, 150822, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20150822, 585L, 8, 33, 8, 33, 8, 33, 8, 33, 150829, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20150829, 586L, 8, 34, 8, 34, 8, 34, 8, 34, 150905, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20150905, 587L, 9, 35, 9, 35, 9, 35, 9, 35, 150912, (byte)9, (byte)9, (byte)3, (byte)35 },
                    { 20150912, 588L, 9, 36, 9, 36, 9, 36, 9, 36, 150919, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20150919, 589L, 9, 37, 9, 37, 9, 37, 9, 37, 150926, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20150926, 590L, 9, 38, 9, 38, 9, 38, 9, 38, 151003, (byte)9, (byte)9, (byte)4, (byte)38 },
                    { 20151003, 591L, 9, 39, 9, 39, 9, 39, 9, 39, 151010, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20151010, 592L, 10, 40, 10, 40, 10, 40, 10, 40, 151017, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20151017, 593L, 10, 41, 10, 41, 10, 41, 10, 41, 151024, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20151024, 594L, 10, 42, 10, 42, 10, 42, 10, 42, 151031, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20151031, 595L, 10, 43, 10, 43, 10, 43, 10, 43, 151107, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20151107, 596L, 11, 44, 11, 44, 11, 44, 11, 44, 151114, (byte)11, (byte)11, (byte)4, (byte)44 },
                    { 20151114, 597L, 11, 45, 11, 45, 11, 45, 11, 45, 151121, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20151121, 598L, 11, 46, 11, 46, 11, 46, 11, 46, 151128, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20151128, 599L, 11, 47, 11, 47, 11, 47, 11, 47, 151205, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20151205, 600L, 12, 48, 12, 48, 12, 48, 12, 48, 151212, (byte)12, (byte)12, (byte)4, (byte)48 },
                    { 20151212, 601L, 12, 49, 12, 49, 12, 49, 12, 49, 151219, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20151219, 602L, 12, 50, 12, 50, 12, 50, 12, 50, 151226, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20151226, 603L, 12, 51, 12, 51, 12, 51, 12, 51, 160102, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20160102, 604L, 12, 52, 12, 52, 12, 52, 12, 52, 160109, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20160109, 613L, 1, 1, 1, 1, 1, 1, 1, 1, 160116, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20160116, 614L, 1, 2, 1, 2, 1, 2, 1, 2, 160123, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20160123, 615L, 1, 3, 1, 3, 1, 3, 1, 3, 160130, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20160130, 616L, 1, 4, 1, 4, 1, 4, 1, 4, 160206, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20160206, 617L, 2, 5, 2, 5, 2, 5, 2, 5, 160213, (byte)2, (byte)2, (byte)1, (byte)5 },
                    { 20160213, 618L, 2, 6, 2, 6, 2, 6, 2, 6, 160220, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20160220, 619L, 2, 7, 2, 7, 2, 7, 2, 7, 160227, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20160227, 620L, 2, 8, 2, 8, 2, 8, 2, 8, 160305, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20160305, 621L, 3, 9, 3, 9, 3, 9, 3, 9, 160312, (byte)3, (byte)3, (byte)1, (byte)9 },
                    { 20160312, 622L, 3, 10, 3, 10, 3, 10, 3, 10, 160319, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20160319, 623L, 3, 11, 3, 11, 3, 11, 3, 11, 160326, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20160326, 624L, 3, 12, 3, 12, 3, 12, 3, 12, 160402, (byte)3, (byte)3, (byte)1, (byte)12 },
                    { 20160402, 625L, 3, 13, 3, 13, 3, 13, 3, 13, 160409, (byte)3, (byte)3, (byte)2, (byte)13 },
                    { 20160409, 626L, 4, 14, 4, 14, 4, 14, 4, 14, 160416, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20160416, 627L, 4, 15, 4, 15, 4, 15, 4, 15, 160423, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20160423, 628L, 4, 16, 4, 16, 4, 16, 4, 16, 160430, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20160430, 629L, 4, 17, 4, 17, 4, 17, 4, 17, 160507, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20160507, 630L, 5, 18, 5, 18, 5, 18, 5, 18, 160514, (byte)5, (byte)5, (byte)2, (byte)18 },
                    { 20160514, 631L, 5, 19, 5, 19, 5, 19, 5, 19, 160521, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20160521, 632L, 5, 20, 5, 20, 5, 20, 5, 20, 160528, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20160528, 633L, 5, 21, 5, 21, 5, 21, 5, 21, 160604, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20160604, 634L, 6, 22, 6, 22, 6, 22, 6, 22, 160611, (byte)6, (byte)6, (byte)2, (byte)22 },
                    { 20160611, 635L, 6, 23, 6, 23, 6, 23, 6, 23, 160618, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20160618, 636L, 6, 24, 6, 24, 6, 24, 6, 24, 160625, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20160625, 637L, 6, 25, 6, 25, 6, 25, 6, 25, 160702, (byte)6, (byte)6, (byte)2, (byte)25 },
                    { 20160702, 638L, 6, 26, 6, 26, 6, 26, 6, 26, 160709, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20160709, 639L, 7, 27, 7, 27, 7, 27, 7, 27, 160716, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20160716, 640L, 7, 28, 7, 28, 7, 28, 7, 28, 160723, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20160723, 641L, 7, 29, 7, 29, 7, 29, 7, 29, 160730, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20160730, 642L, 7, 30, 7, 30, 7, 30, 7, 30, 160806, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20160806, 643L, 8, 31, 8, 31, 8, 31, 8, 31, 160813, (byte)8, (byte)8, (byte)3, (byte)31 },
                    { 20160813, 644L, 8, 32, 8, 32, 8, 32, 8, 32, 160820, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20160820, 645L, 8, 33, 8, 33, 8, 33, 8, 33, 160827, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20160827, 646L, 8, 34, 8, 34, 8, 34, 8, 34, 160903, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20160903, 647L, 8, 35, 8, 35, 8, 35, 8, 35, 160910, (byte)8, (byte)8, (byte)3, (byte)35 },
                    { 20160910, 648L, 9, 36, 9, 36, 9, 36, 9, 36, 160917, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20160917, 649L, 9, 37, 9, 37, 9, 37, 9, 37, 160924, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20160924, 650L, 9, 38, 9, 38, 9, 38, 9, 38, 161001, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20161001, 651L, 9, 39, 9, 39, 9, 39, 9, 39, 161008, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20161008, 652L, 10, 40, 10, 40, 10, 40, 10, 40, 161015, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20161015, 653L, 10, 41, 10, 41, 10, 41, 10, 41, 161022, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20161022, 654L, 10, 42, 10, 42, 10, 42, 10, 42, 161029, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20161029, 655L, 10, 43, 10, 43, 10, 43, 10, 43, 161105, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20161105, 656L, 11, 44, 11, 44, 11, 44, 11, 44, 161112, (byte)11, (byte)11, (byte)4, (byte)44 },
                    { 20161112, 657L, 11, 45, 11, 45, 11, 45, 11, 45, 161119, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20161119, 658L, 11, 46, 11, 46, 11, 46, 11, 46, 161126, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20161126, 659L, 11, 47, 11, 47, 11, 47, 11, 47, 161203, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20161203, 660L, 11, 48, 11, 48, 11, 48, 11, 48, 161210, (byte)11, (byte)11, (byte)4, (byte)48 },
                    { 20161210, 661L, 12, 49, 12, 49, 12, 49, 12, 49, 161217, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20161217, 662L, 12, 50, 12, 50, 12, 50, 12, 50, 161224, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20161224, 663L, 12, 51, 12, 51, 12, 51, 12, 51, 161231, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20161231, 664L, 12, 52, 12, 52, 12, 52, 12, 52, 170107, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20170107, 673L, 1, 1, 1, 1, 1, 1, 1, 1, 170114, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20170114, 674L, 1, 2, 1, 2, 1, 2, 1, 2, 170121, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20170121, 675L, 1, 3, 1, 3, 1, 3, 1, 3, 170128, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20170128, 676L, 1, 4, 1, 4, 1, 4, 1, 4, 170204, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20170204, 677L, 2, 5, 2, 5, 2, 5, 2, 5, 170211, (byte)2, (byte)2, (byte)1, (byte)5 },
                    { 20170211, 678L, 2, 6, 2, 6, 2, 6, 2, 6, 170218, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20170218, 679L, 2, 7, 2, 7, 2, 7, 2, 7, 170225, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20170225, 680L, 2, 8, 2, 8, 2, 8, 2, 8, 170304, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20170304, 681L, 3, 9, 3, 9, 3, 9, 3, 9, 170311, (byte)3, (byte)3, (byte)1, (byte)9 },
                    { 20170311, 682L, 3, 10, 3, 10, 3, 10, 3, 10, 170318, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20170318, 683L, 3, 11, 3, 11, 3, 11, 3, 11, 170325, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20170325, 684L, 3, 12, 3, 12, 3, 12, 3, 12, 170401, (byte)3, (byte)3, (byte)1, (byte)12 },
                    { 20170401, 685L, 3, 13, 3, 13, 3, 13, 3, 13, 170408, (byte)3, (byte)3, (byte)2, (byte)13 },
                    { 20170408, 686L, 4, 14, 4, 14, 4, 14, 4, 14, 170415, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20170415, 687L, 4, 15, 4, 15, 4, 15, 4, 15, 170422, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20170422, 688L, 4, 16, 4, 16, 4, 16, 4, 16, 170429, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20170429, 689L, 4, 17, 4, 17, 4, 17, 4, 17, 170506, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20170506, 690L, 5, 18, 5, 18, 5, 18, 5, 18, 170513, (byte)5, (byte)5, (byte)2, (byte)18 },
                    { 20170513, 691L, 5, 19, 5, 19, 5, 19, 5, 19, 170520, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20170520, 692L, 5, 20, 5, 20, 5, 20, 5, 20, 170527, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20170527, 693L, 5, 21, 5, 21, 5, 21, 5, 21, 170603, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20170603, 694L, 5, 22, 5, 22, 5, 22, 5, 22, 170610, (byte)5, (byte)5, (byte)2, (byte)22 },
                    { 20170610, 695L, 6, 23, 6, 23, 6, 23, 6, 23, 170617, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20170617, 696L, 6, 24, 6, 24, 6, 24, 6, 24, 170624, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20170624, 697L, 6, 25, 6, 25, 6, 25, 6, 25, 170701, (byte)6, (byte)6, (byte)2, (byte)25 },
                    { 20170701, 698L, 6, 26, 6, 26, 6, 26, 6, 26, 170708, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20170708, 699L, 7, 27, 7, 27, 7, 27, 7, 27, 170715, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20170715, 700L, 7, 28, 7, 28, 7, 28, 7, 28, 170722, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20170722, 701L, 7, 29, 7, 29, 7, 29, 7, 29, 170729, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20170729, 702L, 7, 30, 7, 30, 7, 30, 7, 30, 170805, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20170805, 703L, 8, 31, 8, 31, 8, 31, 8, 31, 170812, (byte)8, (byte)8, (byte)3, (byte)31 },
                    { 20170812, 704L, 8, 32, 8, 32, 8, 32, 8, 32, 170819, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20170819, 705L, 8, 33, 8, 33, 8, 33, 8, 33, 170826, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20170826, 706L, 8, 34, 8, 34, 8, 34, 8, 34, 170902, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20170902, 707L, 8, 35, 8, 35, 8, 35, 8, 35, 170909, (byte)8, (byte)8, (byte)3, (byte)35 },
                    { 20170909, 708L, 9, 36, 9, 36, 9, 36, 9, 36, 170916, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20170916, 709L, 9, 37, 9, 37, 9, 37, 9, 37, 170923, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20170923, 710L, 9, 38, 9, 38, 9, 38, 9, 38, 170930, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20170930, 711L, 9, 39, 9, 39, 9, 39, 9, 39, 171007, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20171007, 712L, 10, 40, 10, 40, 10, 40, 10, 40, 171014, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20171014, 713L, 10, 41, 10, 41, 10, 41, 10, 41, 171021, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20171021, 714L, 10, 42, 10, 42, 10, 42, 10, 42, 171028, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20171028, 715L, 10, 43, 10, 43, 10, 43, 10, 43, 171104, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20171104, 716L, 11, 44, 11, 44, 11, 44, 11, 44, 171111, (byte)11, (byte)11, (byte)4, (byte)44 },
                    { 20171111, 717L, 11, 45, 11, 45, 11, 45, 11, 45, 171118, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20171118, 718L, 11, 46, 11, 46, 11, 46, 11, 46, 171125, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20171125, 719L, 11, 47, 11, 47, 11, 47, 11, 47, 171202, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20171202, 720L, 11, 48, 11, 48, 11, 48, 11, 48, 171209, (byte)11, (byte)11, (byte)4, (byte)48 },
                    { 20171209, 721L, 12, 49, 12, 49, 12, 49, 12, 49, 171216, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20171216, 722L, 12, 50, 12, 50, 12, 50, 12, 50, 171223, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20171223, 723L, 12, 51, 12, 51, 12, 51, 12, 51, 171230, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20171230, 724L, 12, 52, 12, 52, 12, 52, 12, 52, 180106, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20180106, 733L, 1, 1, 1, 1, 1, 1, 1, 1, 180113, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20180113, 734L, 1, 2, 1, 2, 1, 2, 1, 2, 180120, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20180120, 735L, 1, 3, 1, 3, 1, 3, 1, 3, 180127, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20180127, 736L, 1, 4, 1, 4, 1, 4, 1, 4, 180203, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20180203, 737L, 1, 5, 1, 5, 1, 5, 1, 5, 180210, (byte)1, (byte)1, (byte)1, (byte)5 },
                    { 20180210, 738L, 2, 6, 2, 6, 2, 6, 2, 6, 180217, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20180217, 739L, 2, 7, 2, 7, 2, 7, 2, 7, 180224, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20180224, 740L, 2, 8, 2, 8, 2, 8, 2, 8, 180303, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20180303, 741L, 2, 9, 2, 9, 2, 9, 2, 9, 180310, (byte)2, (byte)2, (byte)1, (byte)9 },
                    { 20180310, 742L, 3, 10, 3, 10, 3, 10, 3, 10, 180317, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20180317, 743L, 3, 11, 3, 11, 3, 11, 3, 11, 180324, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20180324, 744L, 3, 12, 3, 12, 3, 12, 3, 12, 180331, (byte)3, (byte)3, (byte)1, (byte)12 },
                    { 20180331, 745L, 3, 13, 3, 13, 3, 13, 3, 13, 180407, (byte)3, (byte)3, (byte)2, (byte)13 },
                    { 20180407, 746L, 4, 14, 4, 14, 4, 14, 4, 14, 180414, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20180414, 747L, 4, 15, 4, 15, 4, 15, 4, 15, 180421, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20180421, 748L, 4, 16, 4, 16, 4, 16, 4, 16, 180428, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20180428, 749L, 4, 17, 4, 17, 4, 17, 4, 17, 180505, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20180505, 750L, 5, 18, 5, 18, 5, 18, 5, 18, 180512, (byte)5, (byte)5, (byte)2, (byte)18 },
                    { 20180512, 751L, 5, 19, 5, 19, 5, 19, 5, 19, 180519, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20180519, 752L, 5, 20, 5, 20, 5, 20, 5, 20, 180526, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20180526, 753L, 5, 21, 5, 21, 5, 21, 5, 21, 180602, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20180602, 754L, 5, 22, 5, 22, 5, 22, 5, 22, 180609, (byte)5, (byte)5, (byte)2, (byte)22 },
                    { 20180609, 755L, 6, 23, 6, 23, 6, 23, 6, 23, 180616, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20180616, 756L, 6, 24, 6, 24, 6, 24, 6, 24, 180623, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20180623, 757L, 6, 25, 6, 25, 6, 25, 6, 25, 180630, (byte)6, (byte)6, (byte)2, (byte)25 },
                    { 20180630, 758L, 6, 26, 6, 26, 6, 26, 6, 26, 180707, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20180707, 759L, 7, 27, 7, 27, 7, 27, 7, 27, 180714, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20180714, 760L, 7, 28, 7, 28, 7, 28, 7, 28, 180721, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20180721, 761L, 7, 29, 7, 29, 7, 29, 7, 29, 180728, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20180728, 762L, 7, 30, 7, 30, 7, 30, 7, 30, 180804, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20180804, 763L, 8, 31, 8, 31, 8, 31, 8, 31, 180811, (byte)8, (byte)8, (byte)3, (byte)31 },
                    { 20180811, 764L, 8, 32, 8, 32, 8, 32, 8, 32, 180818, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20180818, 765L, 8, 33, 8, 33, 8, 33, 8, 33, 180825, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20180825, 766L, 8, 34, 8, 34, 8, 34, 8, 34, 180901, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20180901, 767L, 8, 35, 8, 35, 8, 35, 8, 35, 180908, (byte)8, (byte)8, (byte)3, (byte)35 },
                    { 20180908, 768L, 9, 36, 9, 36, 9, 36, 9, 36, 180915, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20180915, 769L, 9, 37, 9, 37, 9, 37, 9, 37, 180922, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20180922, 770L, 9, 38, 9, 38, 9, 38, 9, 38, 180929, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20180929, 771L, 9, 39, 9, 39, 9, 39, 9, 39, 181006, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20181006, 772L, 10, 40, 10, 40, 10, 40, 10, 40, 181013, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20181013, 773L, 10, 41, 10, 41, 10, 41, 10, 41, 181020, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20181020, 774L, 10, 42, 10, 42, 10, 42, 10, 42, 181027, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20181027, 775L, 10, 43, 10, 43, 10, 43, 10, 43, 181103, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20181103, 776L, 10, 44, 10, 44, 10, 44, 10, 44, 181110, (byte)10, (byte)10, (byte)4, (byte)44 },
                    { 20181110, 777L, 11, 45, 11, 45, 11, 45, 11, 45, 181117, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20181117, 778L, 11, 46, 11, 46, 11, 46, 11, 46, 181124, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20181124, 779L, 11, 47, 11, 47, 11, 47, 11, 47, 181201, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20181201, 780L, 11, 48, 11, 48, 11, 48, 11, 48, 181208, (byte)11, (byte)11, (byte)4, (byte)48 },
                    { 20181208, 781L, 12, 49, 12, 49, 12, 49, 12, 49, 181215, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20181215, 782L, 12, 50, 12, 50, 12, 50, 12, 50, 181222, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20181222, 783L, 12, 51, 12, 51, 12, 51, 12, 51, 181229, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20181229, 784L, 12, 52, 12, 52, 12, 52, 12, 52, 190105, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20190105, 793L, 1, 1, 1, 1, 1, 1, 1, 1, 190112, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20190112, 794L, 1, 2, 1, 2, 1, 2, 1, 2, 190119, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20190119, 795L, 1, 3, 1, 3, 1, 3, 1, 3, 190126, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20190126, 796L, 1, 4, 1, 4, 1, 4, 1, 4, 190202, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20190202, 797L, 1, 5, 1, 5, 1, 5, 1, 5, 190209, (byte)1, (byte)1, (byte)1, (byte)5 },
                    { 20190209, 798L, 2, 6, 2, 6, 2, 6, 2, 6, 190216, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20190216, 799L, 2, 7, 2, 7, 2, 7, 2, 7, 190223, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20190223, 800L, 2, 8, 2, 8, 2, 8, 2, 8, 190302, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20190302, 801L, 2, 9, 2, 9, 2, 9, 2, 9, 190309, (byte)2, (byte)2, (byte)1, (byte)9 },
                    { 20190309, 802L, 3, 10, 3, 10, 3, 10, 3, 10, 190316, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20190316, 803L, 3, 11, 3, 11, 3, 11, 3, 11, 190323, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20190323, 804L, 3, 12, 3, 12, 3, 12, 3, 12, 190330, (byte)3, (byte)3, (byte)1, (byte)12 },
                    { 20190330, 805L, 3, 13, 3, 13, 3, 13, 3, 13, 190406, (byte)3, (byte)3, (byte)2, (byte)13 },
                    { 20190406, 806L, 4, 14, 4, 14, 4, 14, 4, 14, 190413, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20190413, 807L, 4, 15, 4, 15, 4, 15, 4, 15, 190420, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20190420, 808L, 4, 16, 4, 16, 4, 16, 4, 16, 190427, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20190427, 809L, 4, 17, 4, 17, 4, 17, 4, 17, 190504, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20190504, 810L, 5, 18, 5, 18, 5, 18, 5, 18, 190511, (byte)5, (byte)5, (byte)2, (byte)18 },
                    { 20190511, 811L, 5, 19, 5, 19, 5, 19, 5, 19, 190518, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20190518, 812L, 5, 20, 5, 20, 5, 20, 5, 20, 190525, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20190525, 813L, 5, 21, 5, 21, 5, 21, 5, 21, 190601, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20190601, 814L, 5, 22, 5, 22, 5, 22, 5, 22, 190608, (byte)5, (byte)5, (byte)2, (byte)22 },
                    { 20190608, 815L, 6, 23, 6, 23, 6, 23, 6, 23, 190615, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20190615, 816L, 6, 24, 6, 24, 6, 24, 6, 24, 190622, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20190622, 817L, 6, 25, 6, 25, 6, 25, 6, 25, 190629, (byte)6, (byte)6, (byte)2, (byte)25 },
                    { 20190629, 818L, 6, 26, 6, 26, 6, 26, 6, 26, 190706, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20190706, 819L, 7, 27, 7, 27, 7, 27, 7, 27, 190713, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20190713, 820L, 7, 28, 7, 28, 7, 28, 7, 28, 190720, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20190720, 821L, 7, 29, 7, 29, 7, 29, 7, 29, 190727, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20190727, 822L, 7, 30, 7, 30, 7, 30, 7, 30, 190803, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20190803, 823L, 7, 31, 7, 31, 7, 31, 7, 31, 190810, (byte)7, (byte)7, (byte)3, (byte)31 },
                    { 20190810, 824L, 8, 32, 8, 32, 8, 32, 8, 32, 190817, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20190817, 825L, 8, 33, 8, 33, 8, 33, 8, 33, 190824, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20190824, 826L, 8, 34, 8, 34, 8, 34, 8, 34, 190831, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20190831, 827L, 8, 35, 8, 35, 8, 35, 8, 35, 190907, (byte)8, (byte)8, (byte)3, (byte)35 },
                    { 20190907, 828L, 9, 36, 9, 36, 9, 36, 9, 36, 190914, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20190914, 829L, 9, 37, 9, 37, 9, 37, 9, 37, 190921, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20190921, 830L, 9, 38, 9, 38, 9, 38, 9, 38, 190928, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20190928, 831L, 9, 39, 9, 39, 9, 39, 9, 39, 191005, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20191005, 832L, 10, 40, 10, 40, 10, 40, 10, 40, 191012, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20191012, 833L, 10, 41, 10, 41, 10, 41, 10, 41, 191019, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20191019, 834L, 10, 42, 10, 42, 10, 42, 10, 42, 191026, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20191026, 835L, 10, 43, 10, 43, 10, 43, 10, 43, 191102, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20191102, 836L, 10, 44, 10, 44, 10, 44, 10, 44, 191109, (byte)10, (byte)10, (byte)4, (byte)44 },
                    { 20191109, 837L, 11, 45, 11, 45, 11, 45, 11, 45, 191116, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20191116, 838L, 11, 46, 11, 46, 11, 46, 11, 46, 191123, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20191123, 839L, 11, 47, 11, 47, 11, 47, 11, 47, 191130, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20191130, 840L, 11, 48, 11, 48, 11, 48, 11, 48, 191207, (byte)11, (byte)11, (byte)4, (byte)48 },
                    { 20191207, 841L, 12, 49, 12, 49, 12, 49, 12, 49, 191214, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20191214, 842L, 12, 50, 12, 50, 12, 50, 12, 50, 191221, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20191221, 843L, 12, 51, 12, 51, 12, 51, 12, 51, 191228, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20191228, 844L, 12, 52, 12, 52, 12, 52, 12, 52, 200104, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20200104, 853L, 1, 1, 1, 1, 1, 1, 1, 1, 200111, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20200111, 854L, 1, 2, 1, 2, 1, 2, 1, 2, 200118, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20200118, 855L, 1, 3, 1, 3, 1, 3, 1, 3, 200125, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20200125, 856L, 1, 4, 1, 4, 1, 4, 1, 4, 200201, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20200201, 857L, 1, 5, 1, 5, 1, 5, 1, 5, 200208, (byte)1, (byte)1, (byte)1, (byte)5 },
                    { 20200208, 858L, 2, 6, 2, 6, 2, 6, 2, 6, 200215, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20200215, 859L, 2, 7, 2, 7, 2, 7, 2, 7, 200222, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20200222, 860L, 2, 8, 2, 8, 2, 8, 2, 8, 200229, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20200229, 861L, 2, 9, 2, 9, 2, 9, 2, 9, 200307, (byte)2, (byte)2, (byte)1, (byte)9 },
                    { 20200307, 862L, 3, 10, 3, 10, 3, 10, 3, 10, 200314, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20200314, 863L, 3, 11, 3, 11, 3, 11, 3, 11, 200321, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20200321, 864L, 3, 12, 3, 12, 3, 12, 3, 12, 200328, (byte)3, (byte)3, (byte)1, (byte)12 },
                    { 20200328, 865L, 3, 13, 3, 13, 3, 13, 3, 13, 200404, (byte)3, (byte)3, (byte)2, (byte)13 },
                    { 20200404, 866L, 4, 14, 4, 14, 4, 14, 4, 14, 200411, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20200411, 867L, 4, 15, 4, 15, 4, 15, 4, 15, 200418, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20200418, 868L, 4, 16, 4, 16, 4, 16, 4, 16, 200425, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20200425, 869L, 4, 17, 4, 17, 4, 17, 4, 17, 200502, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20200502, 870L, 4, 18, 4, 18, 4, 18, 4, 18, 200509, (byte)4, (byte)4, (byte)2, (byte)18 },
                    { 20200509, 871L, 5, 19, 5, 19, 5, 19, 5, 19, 200516, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20200516, 872L, 5, 20, 5, 20, 5, 20, 5, 20, 200523, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20200523, 873L, 5, 21, 5, 21, 5, 21, 5, 21, 200530, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20200530, 874L, 5, 22, 5, 22, 5, 22, 5, 22, 200606, (byte)5, (byte)5, (byte)2, (byte)22 },
                    { 20200606, 875L, 6, 23, 6, 23, 6, 23, 6, 23, 200613, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20200613, 876L, 6, 24, 6, 24, 6, 24, 6, 24, 200620, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20200620, 877L, 6, 25, 6, 25, 6, 25, 6, 25, 200627, (byte)6, (byte)6, (byte)2, (byte)25 },
                    { 20200627, 878L, 6, 26, 6, 26, 6, 26, 6, 26, 200704, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20200704, 879L, 7, 27, 7, 27, 7, 27, 7, 27, 200711, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20200711, 880L, 7, 28, 7, 28, 7, 28, 7, 28, 200718, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20200718, 881L, 7, 29, 7, 29, 7, 29, 7, 29, 200725, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20200725, 882L, 7, 30, 7, 30, 7, 30, 7, 30, 200801, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20200801, 883L, 7, 31, 7, 31, 7, 31, 7, 31, 200808, (byte)7, (byte)7, (byte)3, (byte)31 },
                    { 20200808, 884L, 8, 32, 8, 32, 8, 32, 8, 32, 200815, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20200815, 885L, 8, 33, 8, 33, 8, 33, 8, 33, 200822, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20200822, 886L, 8, 34, 8, 34, 8, 34, 8, 34, 200829, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20200829, 887L, 8, 35, 8, 35, 8, 35, 8, 35, 200905, (byte)8, (byte)8, (byte)3, (byte)35 },
                    { 20200905, 888L, 9, 36, 9, 36, 9, 36, 9, 36, 200912, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20200912, 889L, 9, 37, 9, 37, 9, 37, 9, 37, 200919, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20200919, 890L, 9, 38, 9, 38, 9, 38, 9, 38, 200926, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20200926, 891L, 9, 39, 9, 39, 9, 39, 9, 39, 201003, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20201003, 892L, 9, 40, 9, 40, 9, 40, 9, 40, 201010, (byte)9, (byte)9, (byte)4, (byte)40 },
                    { 20201010, 893L, 10, 41, 10, 41, 10, 41, 10, 41, 201017, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20201017, 894L, 10, 42, 10, 42, 10, 42, 10, 42, 201024, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20201024, 895L, 10, 43, 10, 43, 10, 43, 10, 43, 201031, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20201031, 896L, 10, 44, 10, 44, 10, 44, 10, 44, 201107, (byte)10, (byte)10, (byte)4, (byte)44 },
                    { 20201107, 897L, 11, 45, 11, 45, 11, 45, 11, 45, 201114, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20201114, 898L, 11, 46, 11, 46, 11, 46, 11, 46, 201121, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20201121, 899L, 11, 47, 11, 47, 11, 47, 11, 47, 201128, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20201128, 900L, 11, 48, 11, 48, 11, 48, 11, 48, 201205, (byte)11, (byte)11, (byte)4, (byte)48 },
                    { 20201205, 901L, 12, 49, 12, 49, 12, 49, 12, 49, 201212, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20201212, 902L, 12, 50, 12, 50, 12, 50, 12, 50, 201219, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20201219, 903L, 12, 51, 12, 51, 12, 51, 12, 51, 201226, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20201226, 904L, 12, 52, 12, 52, 12, 52, 12, 52, 210102, (byte)12, (byte)12, (byte)4, (byte)52 },
                    { 20210102, 905L, 12, 53, 12, 53, 12, 53, 12, 53, 210109, (byte)12, (byte)12, (byte)1, (byte)53 },
                    { 20210109, 913L, 1, 1, 1, 1, 1, 1, 1, 1, 210116, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20210116, 914L, 1, 2, 1, 2, 1, 2, 1, 2, 210123, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20210123, 915L, 1, 3, 1, 3, 1, 3, 1, 3, 210130, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20210130, 916L, 1, 4, 1, 4, 1, 4, 1, 4, 210206, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20210206, 917L, 2, 5, 2, 5, 2, 5, 2, 5, 210213, (byte)2, (byte)2, (byte)1, (byte)5 },
                    { 20210213, 918L, 2, 6, 2, 6, 2, 6, 2, 6, 210220, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20210220, 919L, 2, 7, 2, 7, 2, 7, 2, 7, 210227, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20210227, 920L, 2, 8, 2, 8, 2, 8, 2, 8, 210306, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20210306, 921L, 3, 9, 3, 9, 3, 9, 3, 9, 210313, (byte)3, (byte)3, (byte)1, (byte)9 },
                    { 20210313, 922L, 3, 10, 3, 10, 3, 10, 3, 10, 210320, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20210320, 923L, 3, 11, 3, 11, 3, 11, 3, 11, 210327, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20210327, 924L, 3, 12, 3, 12, 3, 12, 3, 12, 210403, (byte)3, (byte)3, (byte)2, (byte)12 },
                    { 20210403, 925L, 3, 13, 3, 13, 3, 13, 3, 13, 210410, (byte)3, (byte)3, (byte)2, (byte)13 },
                    { 20210410, 926L, 4, 14, 4, 14, 4, 14, 4, 14, 210417, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20210417, 927L, 4, 15, 4, 15, 4, 15, 4, 15, 210424, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20210424, 928L, 4, 16, 4, 16, 4, 16, 4, 16, 210501, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20210501, 929L, 4, 17, 4, 17, 4, 17, 4, 17, 210508, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20210508, 930L, 5, 18, 5, 18, 5, 18, 5, 18, 210515, (byte)5, (byte)5, (byte)2, (byte)18 },
                    { 20210515, 931L, 5, 19, 5, 19, 5, 19, 5, 19, 210522, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20210522, 932L, 5, 20, 5, 20, 5, 20, 5, 20, 210529, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20210529, 933L, 5, 21, 5, 21, 5, 21, 5, 21, 210605, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20210605, 934L, 6, 22, 6, 22, 6, 22, 6, 22, 210612, (byte)6, (byte)6, (byte)2, (byte)22 },
                    { 20210612, 935L, 6, 23, 6, 23, 6, 23, 6, 23, 210619, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20210619, 936L, 6, 24, 6, 24, 6, 24, 6, 24, 210626, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20210626, 937L, 6, 25, 6, 25, 6, 25, 6, 25, 210703, (byte)6, (byte)6, (byte)3, (byte)25 },
                    { 20210703, 938L, 6, 26, 6, 26, 6, 26, 6, 26, 210710, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20210710, 939L, 7, 27, 7, 27, 7, 27, 7, 27, 210717, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20210717, 940L, 7, 28, 7, 28, 7, 28, 7, 28, 210724, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20210724, 941L, 7, 29, 7, 29, 7, 29, 7, 29, 210731, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20210731, 942L, 7, 30, 7, 30, 7, 30, 7, 30, 210807, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20210807, 943L, 8, 31, 8, 31, 8, 31, 8, 31, 210814, (byte)8, (byte)8, (byte)3, (byte)31 },
                    { 20210814, 944L, 8, 32, 8, 32, 8, 32, 8, 32, 210821, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20210821, 945L, 8, 33, 8, 33, 8, 33, 8, 33, 210828, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20210828, 946L, 8, 34, 8, 34, 8, 34, 8, 34, 210904, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20210904, 947L, 9, 35, 9, 35, 9, 35, 9, 35, 210911, (byte)9, (byte)9, (byte)3, (byte)35 },
                    { 20210911, 948L, 9, 36, 9, 36, 9, 36, 9, 36, 210918, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20210918, 949L, 9, 37, 9, 37, 9, 37, 9, 37, 210925, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20210925, 950L, 9, 38, 9, 38, 9, 38, 9, 38, 211002, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20211002, 951L, 9, 39, 9, 39, 9, 39, 9, 39, 211009, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20211009, 952L, 10, 40, 10, 40, 10, 40, 10, 40, 211016, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20211016, 953L, 10, 41, 10, 41, 10, 41, 10, 41, 211023, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20211023, 954L, 10, 42, 10, 42, 10, 42, 10, 42, 211030, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20211030, 955L, 10, 43, 10, 43, 10, 43, 10, 43, 211106, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20211106, 956L, 11, 44, 11, 44, 11, 44, 11, 44, 211113, (byte)11, (byte)11, (byte)4, (byte)44 },
                    { 20211113, 957L, 11, 45, 11, 45, 11, 45, 11, 45, 211120, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20211120, 958L, 11, 46, 11, 46, 11, 46, 11, 46, 211127, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20211127, 959L, 11, 47, 11, 47, 11, 47, 11, 47, 211204, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20211204, 960L, 12, 48, 12, 48, 12, 48, 12, 48, 211211, (byte)12, (byte)12, (byte)4, (byte)48 },
                    { 20211211, 961L, 12, 49, 12, 49, 12, 49, 12, 49, 211218, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20211218, 962L, 12, 50, 12, 50, 12, 50, 12, 50, 211225, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20211225, 963L, 12, 51, 12, 51, 12, 51, 12, 51, 220101, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20220101, 964L, 12, 52, 12, 52, 12, 52, 12, 52, 220108, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20220108, 973L, 1, 1, 1, 1, 1, 1, 1, 1, 220115, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20220115, 974L, 1, 2, 1, 2, 1, 2, 1, 2, 220122, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20220122, 975L, 1, 3, 1, 3, 1, 3, 1, 3, 220129, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20220129, 976L, 1, 4, 1, 4, 1, 4, 1, 4, 220205, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20220205, 977L, 2, 5, 2, 5, 2, 5, 2, 5, 220212, (byte)2, (byte)2, (byte)1, (byte)5 },
                    { 20220212, 978L, 2, 6, 2, 6, 2, 6, 2, 6, 220219, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20220219, 979L, 2, 7, 2, 7, 2, 7, 2, 7, 220226, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20220226, 980L, 2, 8, 2, 8, 2, 8, 2, 8, 220305, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20220305, 981L, 3, 9, 3, 9, 3, 9, 3, 9, 220312, (byte)3, (byte)3, (byte)1, (byte)9 },
                    { 20220312, 982L, 3, 10, 3, 10, 3, 10, 3, 10, 220319, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20220319, 983L, 3, 11, 3, 11, 3, 11, 3, 11, 220326, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20220326, 984L, 3, 12, 3, 12, 3, 12, 3, 12, 220402, (byte)3, (byte)3, (byte)1, (byte)12 },
                    { 20220402, 985L, 3, 13, 3, 13, 3, 13, 3, 13, 220409, (byte)3, (byte)3, (byte)2, (byte)13 },
                    { 20220409, 986L, 4, 14, 4, 14, 4, 14, 4, 14, 220416, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20220416, 987L, 4, 15, 4, 15, 4, 15, 4, 15, 220423, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20220423, 988L, 4, 16, 4, 16, 4, 16, 4, 16, 220430, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20220430, 989L, 4, 17, 4, 17, 4, 17, 4, 17, 220507, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20220507, 990L, 5, 18, 5, 18, 5, 18, 5, 18, 220514, (byte)5, (byte)5, (byte)2, (byte)18 },
                    { 20220514, 991L, 5, 19, 5, 19, 5, 19, 5, 19, 220521, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20220521, 992L, 5, 20, 5, 20, 5, 20, 5, 20, 220528, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20220528, 993L, 5, 21, 5, 21, 5, 21, 5, 21, 220604, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20220604, 994L, 6, 22, 6, 22, 6, 22, 6, 22, 220611, (byte)6, (byte)6, (byte)2, (byte)22 },
                    { 20220611, 995L, 6, 23, 6, 23, 6, 23, 6, 23, 220618, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20220618, 996L, 6, 24, 6, 24, 6, 24, 6, 24, 220625, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20220625, 997L, 6, 25, 6, 25, 6, 25, 6, 25, 220702, (byte)6, (byte)6, (byte)2, (byte)25 },
                    { 20220702, 998L, 6, 26, 6, 26, 6, 26, 6, 26, 220709, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20220709, 999L, 7, 27, 7, 27, 7, 27, 7, 27, 220716, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20220716, 1000L, 7, 28, 7, 28, 7, 28, 7, 28, 220723, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20220723, 1001L, 7, 29, 7, 29, 7, 29, 7, 29, 220730, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20220730, 1002L, 7, 30, 7, 30, 7, 30, 7, 30, 220806, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20220806, 1003L, 8, 31, 8, 31, 8, 31, 8, 31, 220813, (byte)8, (byte)8, (byte)3, (byte)31 },
                    { 20220813, 1004L, 8, 32, 8, 32, 8, 32, 8, 32, 220820, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20220820, 1005L, 8, 33, 8, 33, 8, 33, 8, 33, 220827, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20220827, 1006L, 8, 34, 8, 34, 8, 34, 8, 34, 220903, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20220903, 1007L, 8, 35, 8, 35, 8, 35, 8, 35, 220910, (byte)8, (byte)8, (byte)3, (byte)35 },
                    { 20220910, 1008L, 9, 36, 9, 36, 9, 36, 9, 36, 220917, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20220917, 1009L, 9, 37, 9, 37, 9, 37, 9, 37, 220924, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20220924, 1010L, 9, 38, 9, 38, 9, 38, 9, 38, 221001, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20221001, 1011L, 9, 39, 9, 39, 9, 39, 9, 39, 221008, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20221008, 1012L, 10, 40, 10, 40, 10, 40, 10, 40, 221015, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20221015, 1013L, 10, 41, 10, 41, 10, 41, 10, 41, 221022, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20221022, 1014L, 10, 42, 10, 42, 10, 42, 10, 42, 221029, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20221029, 1015L, 10, 43, 10, 43, 10, 43, 10, 43, 221105, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20221105, 1016L, 11, 44, 11, 44, 11, 44, 11, 44, 221112, (byte)11, (byte)11, (byte)4, (byte)44 },
                    { 20221112, 1017L, 11, 45, 11, 45, 11, 45, 11, 45, 221119, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20221119, 1018L, 11, 46, 11, 46, 11, 46, 11, 46, 221126, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20221126, 1019L, 11, 47, 11, 47, 11, 47, 11, 47, 221203, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20221203, 1020L, 11, 48, 11, 48, 11, 48, 11, 48, 221210, (byte)11, (byte)11, (byte)4, (byte)48 },
                    { 20221210, 1021L, 12, 49, 12, 49, 12, 49, 12, 49, 221217, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20221217, 1022L, 12, 50, 12, 50, 12, 50, 12, 50, 221224, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20221224, 1023L, 12, 51, 12, 51, 12, 51, 12, 51, 221231, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20221231, 1024L, 12, 52, 12, 52, 12, 52, 12, 52, 230107, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20230107, 1033L, 1, 1, 1, 1, 1, 1, 1, 1, 230114, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20230114, 1034L, 1, 2, 1, 2, 1, 2, 1, 2, 230121, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20230121, 1035L, 1, 3, 1, 3, 1, 3, 1, 3, 230128, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20230128, 1036L, 1, 4, 1, 4, 1, 4, 1, 4, 230204, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20230204, 1037L, 2, 5, 2, 5, 2, 5, 2, 5, 230211, (byte)2, (byte)2, (byte)1, (byte)5 },
                    { 20230211, 1038L, 2, 6, 2, 6, 2, 6, 2, 6, 230218, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20230218, 1039L, 2, 7, 2, 7, 2, 7, 2, 7, 230225, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20230225, 1040L, 2, 8, 2, 8, 2, 8, 2, 8, 230304, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20230304, 1041L, 3, 9, 3, 9, 3, 9, 3, 9, 230311, (byte)3, (byte)3, (byte)1, (byte)9 },
                    { 20230311, 1042L, 3, 10, 3, 10, 3, 10, 3, 10, 230318, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20230318, 1043L, 3, 11, 3, 11, 3, 11, 3, 11, 230325, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20230325, 1044L, 3, 12, 3, 12, 3, 12, 3, 12, 230401, (byte)3, (byte)3, (byte)1, (byte)12 },
                    { 20230401, 1045L, 3, 13, 3, 13, 3, 13, 3, 13, 230408, (byte)3, (byte)3, (byte)2, (byte)13 },
                    { 20230408, 1046L, 4, 14, 4, 14, 4, 14, 4, 14, 230415, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20230415, 1047L, 4, 15, 4, 15, 4, 15, 4, 15, 230422, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20230422, 1048L, 4, 16, 4, 16, 4, 16, 4, 16, 230429, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20230429, 1049L, 4, 17, 4, 17, 4, 17, 4, 17, 230506, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20230506, 1050L, 5, 18, 5, 18, 5, 18, 5, 18, 230513, (byte)5, (byte)5, (byte)2, (byte)18 },
                    { 20230513, 1051L, 5, 19, 5, 19, 5, 19, 5, 19, 230520, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20230520, 1052L, 5, 20, 5, 20, 5, 20, 5, 20, 230527, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20230527, 1053L, 5, 21, 5, 21, 5, 21, 5, 21, 230603, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20230603, 1054L, 5, 22, 5, 22, 5, 22, 5, 22, 230610, (byte)5, (byte)5, (byte)2, (byte)22 },
                    { 20230610, 1055L, 6, 23, 6, 23, 6, 23, 6, 23, 230617, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20230617, 1056L, 6, 24, 6, 24, 6, 24, 6, 24, 230624, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20230624, 1057L, 6, 25, 6, 25, 6, 25, 6, 25, 230701, (byte)6, (byte)6, (byte)2, (byte)25 },
                    { 20230701, 1058L, 6, 26, 6, 26, 6, 26, 6, 26, 230708, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20230708, 1059L, 7, 27, 7, 27, 7, 27, 7, 27, 230715, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20230715, 1060L, 7, 28, 7, 28, 7, 28, 7, 28, 230722, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20230722, 1061L, 7, 29, 7, 29, 7, 29, 7, 29, 230729, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20230729, 1062L, 7, 30, 7, 30, 7, 30, 7, 30, 230805, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20230805, 1063L, 8, 31, 8, 31, 8, 31, 8, 31, 230812, (byte)8, (byte)8, (byte)3, (byte)31 },
                    { 20230812, 1064L, 8, 32, 8, 32, 8, 32, 8, 32, 230819, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20230819, 1065L, 8, 33, 8, 33, 8, 33, 8, 33, 230826, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20230826, 1066L, 8, 34, 8, 34, 8, 34, 8, 34, 230902, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20230902, 1067L, 8, 35, 8, 35, 8, 35, 8, 35, 230909, (byte)8, (byte)8, (byte)3, (byte)35 },
                    { 20230909, 1068L, 9, 36, 9, 36, 9, 36, 9, 36, 230916, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20230916, 1069L, 9, 37, 9, 37, 9, 37, 9, 37, 230923, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20230923, 1070L, 9, 38, 9, 38, 9, 38, 9, 38, 230930, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20230930, 1071L, 9, 39, 9, 39, 9, 39, 9, 39, 231007, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20231007, 1072L, 10, 40, 10, 40, 10, 40, 10, 40, 231014, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20231014, 1073L, 10, 41, 10, 41, 10, 41, 10, 41, 231021, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20231021, 1074L, 10, 42, 10, 42, 10, 42, 10, 42, 231028, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20231028, 1075L, 10, 43, 10, 43, 10, 43, 10, 43, 231104, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20231104, 1076L, 11, 44, 11, 44, 11, 44, 11, 44, 231111, (byte)11, (byte)11, (byte)4, (byte)44 },
                    { 20231111, 1077L, 11, 45, 11, 45, 11, 45, 11, 45, 231118, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20231118, 1078L, 11, 46, 11, 46, 11, 46, 11, 46, 231125, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20231125, 1079L, 11, 47, 11, 47, 11, 47, 11, 47, 231202, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20231202, 1080L, 11, 48, 11, 48, 11, 48, 11, 48, 231209, (byte)11, (byte)11, (byte)4, (byte)48 },
                    { 20231209, 1081L, 12, 49, 12, 49, 12, 49, 12, 49, 231216, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20231216, 1082L, 12, 50, 12, 50, 12, 50, 12, 50, 231223, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20231223, 1083L, 12, 51, 12, 51, 12, 51, 12, 51, 231230, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20231230, 1084L, 12, 52, 12, 52, 12, 52, 12, 52, 240106, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20240106, 1093L, 1, 1, 1, 1, 1, 1, 1, 1, 240113, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20240113, 1094L, 1, 2, 1, 2, 1, 2, 1, 2, 240120, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20240120, 1095L, 1, 3, 1, 3, 1, 3, 1, 3, 240127, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20240127, 1096L, 1, 4, 1, 4, 1, 4, 1, 4, 240203, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20240203, 1097L, 1, 5, 1, 5, 1, 5, 1, 5, 240210, (byte)1, (byte)1, (byte)1, (byte)5 },
                    { 20240210, 1098L, 2, 6, 2, 6, 2, 6, 2, 6, 240217, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20240217, 1099L, 2, 7, 2, 7, 2, 7, 2, 7, 240224, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20240224, 1100L, 2, 8, 2, 8, 2, 8, 2, 8, 240302, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20240302, 1101L, 2, 9, 2, 9, 2, 9, 2, 9, 240309, (byte)2, (byte)2, (byte)1, (byte)9 },
                    { 20240309, 1102L, 3, 10, 3, 10, 3, 10, 3, 10, 240316, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20240316, 1103L, 3, 11, 3, 11, 3, 11, 3, 11, 240323, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20240323, 1104L, 3, 12, 3, 12, 3, 12, 3, 12, 240330, (byte)3, (byte)3, (byte)1, (byte)12 },
                    { 20240330, 1105L, 3, 13, 3, 13, 3, 13, 3, 13, 240406, (byte)3, (byte)3, (byte)2, (byte)13 },
                    { 20240406, 1106L, 4, 14, 4, 14, 4, 14, 4, 14, 240413, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20240413, 1107L, 5, 15, 4, 15, 4, 15, 4, 15, 240420, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20240420, 1108L, 4, 16, 4, 16, 4, 16, 4, 16, 240427, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20240427, 1109L, 4, 17, 4, 17, 4, 17, 4, 17, 240504, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20240504, 1110L, 5, 18, 5, 18, 5, 18, 5, 18, 240511, (byte)5, (byte)5, (byte)2, (byte)18 },
                    { 20240511, 1111L, 5, 19, 5, 19, 5, 19, 5, 19, 240518, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20240518, 1112L, 5, 20, 5, 20, 5, 20, 5, 20, 240525, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20240525, 1113L, 5, 21, 5, 21, 5, 21, 5, 21, 240601, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20240601, 1114L, 5, 22, 5, 22, 5, 22, 5, 22, 240608, (byte)5, (byte)5, (byte)2, (byte)22 },
                    { 20240608, 1115L, 6, 23, 6, 23, 6, 23, 6, 23, 240615, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20240615, 1116L, 6, 24, 6, 24, 6, 24, 6, 24, 240622, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20240622, 1117L, 6, 25, 6, 25, 6, 25, 6, 25, 240629, (byte)6, (byte)6, (byte)2, (byte)25 },
                    { 20240629, 1145L, 6, 26, 6, 26, 6, 26, 6, 26, 240706, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20240706, 1119L, 7, 27, 7, 27, 7, 27, 7, 27, 240713, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20240713, 1120L, 3, 28, 3, 28, 3, 28, 3, 28, 240720, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20240720, 1121L, 3, 29, 3, 29, 3, 29, 3, 29, 240727, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20240727, 1122L, 7, 30, 7, 30, 7, 30, 7, 30, 240803, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20240803, 1123L, 7, 31, 7, 31, 7, 31, 7, 31, 240810, (byte)7, (byte)7, (byte)3, (byte)31 },
                    { 20240810, 1124L, 8, 32, 8, 32, 8, 32, 8, 32, 240817, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20240817, 1125L, 8, 33, 8, 33, 8, 33, 8, 33, 240824, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20240824, 1126L, 8, 34, 8, 34, 8, 34, 8, 34, 240831, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20240831, 1127L, 8, 35, 8, 35, 8, 35, 8, 35, 240907, (byte)8, (byte)8, (byte)3, (byte)35 },
                    { 20240907, 1128L, 9, 36, 9, 36, 9, 36, 9, 36, 240914, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20240914, 1129L, 9, 37, 9, 37, 9, 37, 9, 37, 240921, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20240921, 1130L, 9, 38, 9, 38, 9, 38, 9, 38, 240928, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20240928, 1131L, 9, 39, 9, 39, 9, 39, 9, 39, 241005, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20241005, 1132L, 10, 40, 10, 40, 10, 40, 10, 40, 241012, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20241012, 1133L, 10, 41, 10, 41, 10, 41, 10, 41, 241019, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20241019, 1134L, 10, 42, 10, 42, 10, 42, 10, 42, 241026, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20241026, 1135L, 10, 43, 10, 43, 10, 43, 10, 43, 241102, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20241102, 1136L, 10, 44, 10, 44, 10, 44, 10, 44, 241109, (byte)10, (byte)10, (byte)4, (byte)44 },
                    { 20241109, 1137L, 11, 45, 11, 45, 11, 45, 11, 45, 241116, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20241116, 1138L, 11, 46, 11, 46, 11, 46, 11, 46, 241123, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20241123, 1139L, 11, 47, 11, 47, 11, 47, 11, 47, 241130, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20241130, 1140L, 11, 48, 11, 48, 11, 48, 11, 48, 241207, (byte)11, (byte)11, (byte)4, (byte)48 },
                    { 20241207, 1141L, 12, 49, 12, 49, 12, 49, 12, 49, 241214, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20241214, 1142L, 12, 50, 12, 50, 12, 50, 12, 50, 241221, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20241221, 1143L, 12, 51, 12, 51, 12, 51, 12, 51, 241228, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20241228, 1144L, 12, 52, 12, 52, 12, 52, 12, 51, 250104, (byte)12, (byte)12, (byte)1, (byte)52 },
                    { 20250104, 157L, 1, 1, 1, 1, 1, 1, 1, 1, 250111, (byte)1, (byte)1, (byte)1, (byte)1 },
                    { 20250111, 158L, 1, 2, 1, 2, 1, 2, 1, 2, 250118, (byte)1, (byte)1, (byte)1, (byte)2 },
                    { 20250118, 159L, 1, 3, 1, 3, 1, 3, 1, 3, 250125, (byte)1, (byte)1, (byte)1, (byte)3 },
                    { 20250125, 160L, 1, 4, 1, 4, 1, 4, 1, 4, 250201, (byte)1, (byte)1, (byte)1, (byte)4 },
                    { 20250201, 161L, 1, 5, 1, 5, 1, 5, 1, 5, 250208, (byte)1, (byte)1, (byte)1, (byte)5 },
                    { 20250208, 162L, 2, 6, 2, 6, 2, 6, 2, 6, 250215, (byte)2, (byte)2, (byte)1, (byte)6 },
                    { 20250215, 163L, 2, 7, 2, 7, 2, 7, 2, 7, 250222, (byte)2, (byte)2, (byte)1, (byte)7 },
                    { 20250222, 164L, 2, 8, 2, 8, 2, 8, 2, 8, 250301, (byte)2, (byte)2, (byte)1, (byte)8 },
                    { 20250301, 165L, 2, 9, 2, 9, 2, 9, 2, 9, 250308, (byte)2, (byte)2, (byte)1, (byte)9 },
                    { 20250308, 166L, 3, 10, 3, 10, 3, 10, 3, 10, 250315, (byte)3, (byte)3, (byte)1, (byte)10 },
                    { 20250315, 167L, 3, 11, 3, 11, 3, 11, 3, 11, 250322, (byte)3, (byte)3, (byte)1, (byte)11 },
                    { 20250322, 168L, 3, 12, 3, 12, 3, 12, 3, 12, 250329, (byte)3, (byte)3, (byte)1, (byte)12 },
                    { 20250329, 169L, 3, 13, 3, 13, 3, 13, 3, 13, 250405, (byte)3, (byte)3, (byte)2, (byte)13 },
                    { 20250405, 170L, 4, 14, 4, 14, 4, 14, 4, 14, 250412, (byte)4, (byte)4, (byte)2, (byte)14 },
                    { 20250412, 171L, 4, 15, 4, 15, 4, 15, 4, 15, 250419, (byte)4, (byte)4, (byte)2, (byte)15 },
                    { 20250419, 172L, 4, 16, 4, 16, 4, 16, 4, 16, 250426, (byte)4, (byte)4, (byte)2, (byte)16 },
                    { 20250426, 173L, 4, 17, 4, 17, 4, 17, 4, 17, 250503, (byte)4, (byte)4, (byte)2, (byte)17 },
                    { 20250503, 174L, 4, 18, 4, 18, 4, 18, 4, 18, 250510, (byte)4, (byte)4, (byte)2, (byte)18 },
                    { 20250510, 175L, 5, 19, 5, 19, 5, 19, 5, 19, 250517, (byte)5, (byte)5, (byte)2, (byte)19 },
                    { 20250517, 176L, 5, 20, 5, 20, 5, 20, 5, 20, 250524, (byte)5, (byte)5, (byte)2, (byte)20 },
                    { 20250524, 177L, 5, 21, 5, 21, 5, 21, 5, 21, 250531, (byte)5, (byte)5, (byte)2, (byte)21 },
                    { 20250531, 178L, 5, 22, 5, 22, 5, 22, 5, 22, 250607, (byte)5, (byte)5, (byte)2, (byte)22 },
                    { 20250607, 179L, 6, 23, 6, 23, 6, 23, 6, 23, 250614, (byte)6, (byte)6, (byte)2, (byte)23 },
                    { 20250614, 180L, 6, 24, 6, 24, 6, 24, 6, 24, 250621, (byte)6, (byte)6, (byte)2, (byte)24 },
                    { 20250621, 181L, 6, 25, 6, 25, 6, 25, 6, 25, 250628, (byte)6, (byte)6, (byte)2, (byte)25 },
                    { 20250628, 182L, 6, 26, 6, 26, 6, 26, 6, 26, 250705, (byte)6, (byte)6, (byte)3, (byte)26 },
                    { 20250705, 183L, 7, 27, 7, 27, 7, 27, 7, 27, 250712, (byte)7, (byte)7, (byte)3, (byte)27 },
                    { 20250712, 184L, 7, 28, 7, 28, 7, 28, 7, 28, 250719, (byte)7, (byte)7, (byte)3, (byte)28 },
                    { 20250719, 185L, 7, 29, 7, 29, 9, 29, 7, 29, 250726, (byte)7, (byte)7, (byte)3, (byte)29 },
                    { 20250726, 186L, 7, 30, 7, 30, 7, 30, 7, 30, 250802, (byte)7, (byte)7, (byte)3, (byte)30 },
                    { 20250802, 187L, 7, 31, 7, 31, 7, 31, 7, 31, 250809, (byte)7, (byte)7, (byte)3, (byte)31 },
                    { 20250809, 188L, 8, 32, 8, 32, 8, 32, 8, 32, 250816, (byte)8, (byte)8, (byte)3, (byte)32 },
                    { 20250816, 189L, 8, 33, 8, 33, 8, 33, 8, 33, 250823, (byte)8, (byte)8, (byte)3, (byte)33 },
                    { 20250823, 190L, 8, 34, 8, 34, 8, 34, 8, 34, 250830, (byte)8, (byte)8, (byte)3, (byte)34 },
                    { 20250830, 191L, 8, 35, 8, 35, 8, 35, 8, 35, 250906, (byte)8, (byte)8, (byte)3, (byte)35 },
                    { 20250906, 192L, 9, 36, 9, 36, 9, 36, 9, 36, 250913, (byte)9, (byte)9, (byte)3, (byte)36 },
                    { 20250913, 193L, 9, 37, 9, 37, 9, 37, 9, 37, 250920, (byte)9, (byte)9, (byte)3, (byte)37 },
                    { 20250920, 194L, 9, 38, 9, 38, 9, 38, 9, 38, 250927, (byte)9, (byte)9, (byte)3, (byte)38 },
                    { 20250927, 195L, 9, 39, 9, 39, 9, 39, 9, 39, 251004, (byte)9, (byte)9, (byte)4, (byte)39 },
                    { 20251004, 196L, 10, 40, 10, 40, 10, 40, 10, 40, 251011, (byte)10, (byte)10, (byte)4, (byte)40 },
                    { 20251011, 197L, 10, 41, 10, 41, 10, 41, 10, 41, 251018, (byte)10, (byte)10, (byte)4, (byte)41 },
                    { 20251018, 198L, 10, 42, 10, 42, 10, 42, 10, 42, 251025, (byte)10, (byte)10, (byte)4, (byte)42 },
                    { 20251025, 199L, 10, 43, 10, 43, 10, 43, 10, 43, 251101, (byte)10, (byte)10, (byte)4, (byte)43 },
                    { 20251101, 200L, 10, 44, 10, 44, 10, 44, 10, 44, 251108, (byte)10, (byte)10, (byte)4, (byte)44 },
                    { 20251108, 201L, 11, 45, 11, 45, 11, 45, 11, 45, 251115, (byte)11, (byte)11, (byte)4, (byte)45 },
                    { 20251115, 202L, 11, 46, 11, 46, 11, 46, 11, 46, 251122, (byte)11, (byte)11, (byte)4, (byte)46 },
                    { 20251122, 203L, 11, 47, 11, 47, 11, 47, 11, 47, 251129, (byte)11, (byte)11, (byte)4, (byte)47 },
                    { 20251129, 204L, 11, 48, 11, 48, 11, 48, 11, 48, 251206, (byte)11, (byte)11, (byte)4, (byte)48 },
                    { 20251206, 205L, 12, 49, 12, 49, 12, 49, 12, 49, 251213, (byte)12, (byte)12, (byte)4, (byte)49 },
                    { 20251213, 206L, 12, 50, 12, 50, 12, 50, 12, 50, 251220, (byte)12, (byte)12, (byte)4, (byte)50 },
                    { 20251220, 207L, 12, 51, 12, 51, 12, 51, 12, 51, 251227, (byte)12, (byte)12, (byte)4, (byte)51 },
                    { 20251227, 208L, 12, 52, 12, 52, 12, 52, 12, 52, 260103, (byte)12, (byte)12, (byte)1, (byte)52 }
                });

            migrationBuilder.InsertData(
                table: "COMMENT_TYPE",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { (byte)1, "Transfer Out" },
                    { (byte)2, "Transfer In" },
                    { (byte)3, "QDRO Out" },
                    { (byte)4, "QDRO In" },
                    { (byte)5, "V-Only" },
                    { (byte)6, "Forfeit" },
                    { (byte)7, "Un-Forfeit" },
                    { (byte)8, "Class Action" },
                    { (byte)9, "Voided" },
                    { (byte)10, "Hardship" },
                    { (byte)11, "Distribution" },
                    { (byte)12, "Payoff" },
                    { (byte)13, "Dirpay" },
                    { (byte)14, "Rollover" },
                    { (byte)15, "Roth IRA" },
                    { (byte)16, "> 64 - 1 Year Vested" },
                    { (byte)17, "> 64 - 2 Year Vested" },
                    { (byte)18, "> 64 - 3 Year Vested" },
                    { (byte)19, "Military" },
                    { (byte)20, "Other" },
                    { (byte)21, "Rev" },
                    { (byte)22, "Unrev" },
                    { (byte)23, "100% Earnings" },
                    { (byte)24, ">64 & >5 100%" },
                    { (byte)25, "Forfeit Class Action" }
                });

            migrationBuilder.InsertData(
                table: "COUNTRY",
                columns: new[] { "ISO", "ID", "NAME", "TELEPHONE_CODE" },
                values: new object[,]
                {
                    { "AD", (byte)4, "Andorra", "+376" },
                    { "AE", (byte)184, "United Arab Emirates", "+971" },
                    { "AF", (byte)1, "Afghanistan", "+93" },
                    { "AG", (byte)6, "Antigua and Barbuda", "+1-268" },
                    { "AL", (byte)2, "Albania", "+355" },
                    { "AM", (byte)8, "Armenia", "+374" },
                    { "AO", (byte)5, "Angola", "+244" },
                    { "AR", (byte)7, "Argentina", "+54" },
                    { "AT", (byte)10, "Austria", "+43" },
                    { "AU", (byte)9, "Australia", "+61" },
                    { "AZ", (byte)11, "Azerbaijan", "+994" },
                    { "BA", (byte)22, "Bosnia and Herzegovina", "+387" },
                    { "BB", (byte)15, "Barbados", "+1-246" },
                    { "BD", (byte)14, "Bangladesh", "+880" },
                    { "BE", (byte)17, "Belgium", "+32" },
                    { "BF", (byte)27, "Burkina Faso", "+226" },
                    { "BG", (byte)26, "Bulgaria", "+359" },
                    { "BH", (byte)13, "Bahrain", "+973" },
                    { "BI", (byte)28, "Burundi", "+257" },
                    { "BJ", (byte)19, "Benin", "+229" },
                    { "BN", (byte)25, "Brunei", "+673" },
                    { "BO", (byte)21, "Bolivia", "+591" },
                    { "BR", (byte)24, "Brazil", "+55" },
                    { "BS", (byte)12, "Bahamas", "+1-242" },
                    { "BT", (byte)20, "Bhutan", "+975" },
                    { "BW", (byte)23, "Botswana", "+267" },
                    { "BY", (byte)16, "Belarus", "+375" },
                    { "BZ", (byte)18, "Belize", "+501" },
                    { "CA", (byte)32, "Canada", "+1" },
                    { "CD", (byte)45, "Democratic Republic of the Congo", "+243" },
                    { "CF", (byte)33, "Central African Republic", "+236" },
                    { "CG", (byte)39, "Congo (Congo-Brazzaville)", "+242" },
                    { "CH", (byte)168, "Switzerland", "+41" },
                    { "CL", (byte)35, "Chile", "+56" },
                    { "CM", (byte)31, "Cameroon", "+237" },
                    { "CN", (byte)36, "China", "+86" },
                    { "CO", (byte)37, "Colombia", "+57" },
                    { "CR", (byte)40, "Costa Rica", "+506" },
                    { "CU", (byte)42, "Cuba", "+53" },
                    { "CV", (byte)29, "Cabo Verde", "+238" },
                    { "CY", (byte)43, "Cyprus", "+357" },
                    { "CZ", (byte)44, "Czechia (Czech Republic)", "+420" },
                    { "DE", (byte)64, "Germany", "+49" },
                    { "DJ", (byte)47, "Djibouti", "+253" },
                    { "DK", (byte)46, "Denmark", "+45" },
                    { "DM", (byte)48, "Dominica", "+1-767" },
                    { "DO", (byte)49, "Dominican Republic", "+1-809" },
                    { "DZ", (byte)3, "Algeria", "+213" },
                    { "EC", (byte)50, "Ecuador", "+593" },
                    { "EE", (byte)55, "Estonia", "+372" },
                    { "EG", (byte)51, "Egypt", "+20" },
                    { "ER", (byte)54, "Eritrea", "+291" },
                    { "ES", (byte)163, "Spain", "+34" },
                    { "ET", (byte)57, "Ethiopia", "+251" },
                    { "FI", (byte)59, "Finland", "+358" },
                    { "FJ", (byte)58, "Fiji", "+679" },
                    { "FM", (byte)110, "Micronesia", "+691" },
                    { "FR", (byte)60, "France", "+33" },
                    { "GA", (byte)61, "Gabon", "+241" },
                    { "GB", (byte)185, "United Kingdom", "+44" },
                    { "GD", (byte)67, "Grenada", "+1-473" },
                    { "GE", (byte)63, "Georgia", "+995" },
                    { "GH", (byte)65, "Ghana", "+233" },
                    { "GM", (byte)62, "Gambia", "+220" },
                    { "GN", (byte)69, "Guinea", "+224" },
                    { "GQ", (byte)53, "Equatorial Guinea", "+240" },
                    { "GR", (byte)66, "Greece", "+30" },
                    { "GT", (byte)68, "Guatemala", "+502" },
                    { "GW", (byte)70, "Guinea-Bissau", "+245" },
                    { "GY", (byte)71, "Guyana", "+592" },
                    { "HN", (byte)73, "Honduras", "+504" },
                    { "HR", (byte)41, "Croatia", "+385" },
                    { "HT", (byte)72, "Haiti", "+509" },
                    { "HU", (byte)74, "Hungary", "+36" },
                    { "ID", (byte)77, "Indonesia", "+62" },
                    { "IE", (byte)80, "Ireland", "+353" },
                    { "IL", (byte)81, "Israel", "+972" },
                    { "IN", (byte)76, "India", "+91" },
                    { "IQ", (byte)79, "Iraq", "+964" },
                    { "IR", (byte)78, "Iran", "+98" },
                    { "IS", (byte)75, "Iceland", "+354" },
                    { "IT", (byte)82, "Italy", "+39" },
                    { "JM", (byte)83, "Jamaica", "+1-876" },
                    { "JO", (byte)85, "Jordan", "+962" },
                    { "JP", (byte)84, "Japan", "+81" },
                    { "KE", (byte)87, "Kenya", "+254" },
                    { "KG", (byte)90, "Kyrgyzstan", "+996" },
                    { "KH", (byte)30, "Cambodia", "+855" },
                    { "KI", (byte)88, "Kiribati", "+686" },
                    { "KM", (byte)38, "Comoros", "+269" },
                    { "KN", (byte)144, "Saint Kitts and Nevis", "+1-869" },
                    { "KP", (byte)126, "North Korea", "+850" },
                    { "KR", (byte)161, "South Korea", "+82" },
                    { "KW", (byte)89, "Kuwait", "+965" },
                    { "KZ", (byte)86, "Kazakhstan", "+7" },
                    { "LA", (byte)91, "Laos", "+856" },
                    { "LB", (byte)93, "Lebanon", "+961" },
                    { "LC", (byte)145, "Saint Lucia", "+1-758" },
                    { "LI", (byte)97, "Liechtenstein", "+423" },
                    { "LK", (byte)164, "Sri Lanka", "+94" },
                    { "LR", (byte)95, "Liberia", "+231" },
                    { "LS", (byte)94, "Lesotho", "+266" },
                    { "LT", (byte)98, "Lithuania", "+370" },
                    { "LU", (byte)99, "Luxembourg", "+352" },
                    { "LV", (byte)92, "Latvia", "+371" },
                    { "LY", (byte)96, "Libya", "+218" },
                    { "MA", (byte)115, "Morocco", "+212" },
                    { "MC", (byte)112, "Monaco", "+377" },
                    { "MD", (byte)111, "Moldova", "+373" },
                    { "ME", (byte)114, "Montenegro", "+382" },
                    { "MG", (byte)100, "Madagascar", "+261" },
                    { "MH", (byte)106, "Marshall Islands", "+692" },
                    { "MK", (byte)127, "North Macedonia", "+389" },
                    { "ML", (byte)104, "Mali", "+223" },
                    { "MM", (byte)117, "Myanmar (Burma)", "+95" },
                    { "MN", (byte)113, "Mongolia", "+976" },
                    { "MR", (byte)107, "Mauritania", "+222" },
                    { "MT", (byte)105, "Malta", "+356" },
                    { "MU", (byte)108, "Mauritius", "+230" },
                    { "MV", (byte)103, "Maldives", "+960" },
                    { "MW", (byte)101, "Malawi", "+265" },
                    { "MX", (byte)109, "Mexico", "+52" },
                    { "MY", (byte)102, "Malaysia", "+60" },
                    { "MZ", (byte)116, "Mozambique", "+258" },
                    { "NA", (byte)118, "Namibia", "+264" },
                    { "NE", (byte)124, "Niger", "+227" },
                    { "NG", (byte)125, "Nigeria", "+234" },
                    { "NI", (byte)123, "Nicaragua", "+505" },
                    { "NL", (byte)121, "Netherlands", "+31" },
                    { "NO", (byte)128, "Norway", "+47" },
                    { "NP", (byte)120, "Nepal", "+977" },
                    { "NR", (byte)119, "Nauru", "+674" },
                    { "NZ", (byte)122, "New Zealand", "+64" },
                    { "OM", (byte)129, "Oman", "+968" },
                    { "PA", (byte)133, "Panama", "+507" },
                    { "PE", (byte)136, "Peru", "+51" },
                    { "PG", (byte)134, "Papua New Guinea", "+675" },
                    { "PH", (byte)137, "Philippines", "+63" },
                    { "PK", (byte)130, "Pakistan", "+92" },
                    { "PL", (byte)138, "Poland", "+48" },
                    { "PS", (byte)132, "Palestine State", "+970" },
                    { "PT", (byte)139, "Portugal", "+351" },
                    { "PW", (byte)131, "Palau", "+680" },
                    { "PY", (byte)135, "Paraguay", "+595" },
                    { "QA", (byte)140, "Qatar", "+974" },
                    { "RO", (byte)141, "Romania", "+40" },
                    { "RS", (byte)152, "Serbia", "+381" },
                    { "RU", (byte)142, "Russia", "+7" },
                    { "RW", (byte)143, "Rwanda", "+250" },
                    { "SA", (byte)150, "Saudi Arabia", "+966" },
                    { "SB", (byte)158, "Solomon Islands", "+677" },
                    { "SC", (byte)153, "Seychelles", "+248" },
                    { "SD", (byte)165, "Sudan", "+249" },
                    { "SE", (byte)167, "Sweden", "+46" },
                    { "SG", (byte)155, "Singapore", "+65" },
                    { "SI", (byte)157, "Slovenia", "+386" },
                    { "SK", (byte)156, "Slovakia", "+421" },
                    { "SL", (byte)154, "Sierra Leone", "+232" },
                    { "SM", (byte)148, "San Marino", "+378" },
                    { "SN", (byte)151, "Senegal", "+221" },
                    { "SO", (byte)159, "Somalia", "+252" },
                    { "SR", (byte)166, "Suriname", "+597" },
                    { "SS", (byte)162, "South Sudan", "+211" },
                    { "ST", (byte)149, "Sao Tome and Principe", "+239" },
                    { "SV", (byte)52, "El Salvador", "+503" },
                    { "SY", (byte)169, "Syria", "+963" },
                    { "SZ", (byte)56, "Eswatini (Swaziland)", "+268" },
                    { "TD", (byte)34, "Chad", "+235" },
                    { "TG", (byte)175, "Togo", "+228" },
                    { "TH", (byte)173, "Thailand", "+66" },
                    { "TJ", (byte)171, "Tajikistan", "+992" },
                    { "TL", (byte)174, "Timor-Leste", "+670" },
                    { "TM", (byte)180, "Turkmenistan", "+993" },
                    { "TN", (byte)178, "Tunisia", "+216" },
                    { "TO", (byte)176, "Tonga", "+676" },
                    { "TR", (byte)179, "Turkey", "+90" },
                    { "TT", (byte)177, "Trinidad and Tobago", "+1-868" },
                    { "TV", (byte)181, "Tuvalu", "+688" },
                    { "TW", (byte)170, "Taiwan", "+886" },
                    { "TZ", (byte)172, "Tanzania", "+255" },
                    { "UA", (byte)183, "Ukraine", "+380" },
                    { "UG", (byte)182, "Uganda", "+256" },
                    { "US", (byte)186, "United States of America", "+1" },
                    { "UY", (byte)187, "Uruguay", "+598" },
                    { "UZ", (byte)188, "Uzbekistan", "+998" },
                    { "VC", (byte)146, "Saint Vincent and the Grenadines", "+1-784" },
                    { "VE", (byte)190, "Venezuela", "+58" },
                    { "VN", (byte)191, "Vietnam", "+84" },
                    { "VU", (byte)189, "Vanuatu", "+678" },
                    { "WS", (byte)147, "Samoa", "+685" },
                    { "YE", (byte)192, "Yemen", "+967" },
                    { "ZA", (byte)160, "South Africa", "+27" },
                    { "ZM", (byte)193, "Zambia", "+260" },
                    { "ZW", (byte)194, "Zimbabwe", "+263" }
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
                    { (byte)6, "Beer/Wine" },
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
                table: "DISTRIBUTION_REQUEST_REASON",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { (byte)1, "CAR" },
                    { (byte)2, "EDUCATION_EXP" },
                    { (byte)3, "EVICTION_OR_FORECLOSE" },
                    { (byte)4, "FUNERAL_EXP" },
                    { (byte)5, "HOME_PURCHASE" },
                    { (byte)6, "HOME_REPAIR" },
                    { (byte)7, "MEDICAL_DENTAL" },
                    { (byte)8, "OTHER" }
                });

            migrationBuilder.InsertData(
                table: "DISTRIBUTION_REQUEST_STATUS",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { "A", "APPROVED" },
                    { "C", "IN_COMMITTEE_REVIEW" },
                    { "D", "DECLINED" },
                    { "N", "NEW_ENTRY" },
                    { "P", "PROCESSED" },
                    { "R", "READY_FOR_REVIEW" }
                });

            migrationBuilder.InsertData(
                table: "DISTRIBUTION_REQUEST_TYPE",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { (byte)0, "HARDSHIP" },
                    { (byte)1, "YEARY" },
                    { (byte)2, "ONE_TIME" },
                    { (byte)3, "PAYOUT" },
                    { (byte)4, "ROLLOVER" }
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
                    { (byte)4, "New vesting plan has Forfeiture records" },
                    { (byte)9, "Previous years enrollment is unknown. (History not previously tracked)" }
                });

            migrationBuilder.InsertData(
                table: "EXCLUDED_ID_TYPE",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { (byte)1, "QPay066TA Exclusions" },
                    { (byte)2, "QPay066I Exclusions" }
                });

            migrationBuilder.InsertData(
                table: "GENDER",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { "F", "Female" },
                    { "M", "Male" },
                    { "U", "Unknown" },
                    { "X", "Nonbinary" }
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
                    { (byte)0, "Employee Sync Full" },
                    { (byte)1, "Payroll Sync Full" },
                    { (byte)2, "Employee Sync Delta" }
                });

            migrationBuilder.InsertData(
                table: "MISSIVES",
                columns: new[] { "ID", "DESCRIPTION", "MESSAGE", "SEVERITY" },
                values: new object[,]
                {
                    { 1, "The employee has between 2 and 7 years in Profit Sharing, has 1000+ plus hours towards Profit Sharing in the fiscal year, and has company contribution records under the new vesting schedule.", "** VESTING INCREASED ON   CURRENT BALANCE ( > 1000 HRS) **", "Information" },
                    { 2, "The Employee's Zero Contribution Flag is set at 6", "VEST IS NOW 100%, 65+/5 YRS", "Information" },
                    { 3, "Employee is a beneficiary of another employee", "Employee is also a Beneficiary", "Information" },
                    { 4, "The PSN you have entered was not found.  Re-enter using a valid PSN", "Beneficiary not on file", "Error" },
                    { 5, "The Employee Badge Number you have entered is not found.  Re-enter using a valid badge number", "Employee badge not on file", "Error" },
                    { 6, "The Employee SSN you have entered is not on file or you don't have access.  Re-enter using a valid SSN", "Employee SSN not on file", "Error" },
                    { 7, "The Employee's Zero Contribution Flag is set at 7", "*** EMPLOYEE MAY BE 100% - CHECK DATES ***", "Information" }
                });

            migrationBuilder.InsertData(
                table: "NAVIGATION_ROLE",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { (byte)1, "System-Administrator" },
                    { (byte)2, "Finance-Manager" },
                    { (byte)3, "Distributions-Clerk" },
                    { (byte)4, "Hardship-Administrator" },
                    { (byte)5, "Impersonation" }
                });

            migrationBuilder.InsertData(
                table: "NAVIGATION_ROLE",
                columns: new[] { "ID", "IS_READ_ONLY", "NAME" },
                values: new object[] { (byte)6, true, "IT-DevOps" });

            migrationBuilder.InsertData(
                table: "NAVIGATION_ROLE",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { (byte)7, "IT-Operations" },
                    { (byte)8, "Executive-Administrator" }
                });

            migrationBuilder.InsertData(
                table: "NAVIGATION_ROLE",
                columns: new[] { "ID", "IS_READ_ONLY", "NAME" },
                values: new object[] { (byte)9, true, "Auditor" });

            migrationBuilder.InsertData(
                table: "NAVIGATION_ROLE",
                columns: new[] { "ID", "NAME" },
                values: new object[] { (byte)10, "Beneficiary-Administrator" });

            migrationBuilder.InsertData(
                table: "NAVIGATION_STATUS",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { (byte)1, "Not Started" },
                    { (byte)2, "In Progress" },
                    { (byte)3, "On Hold" },
                    { (byte)4, "Complete" }
                });

            migrationBuilder.InsertData(
                table: "PAY_CLASSIFICATION",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { "0", "01" },
                    { "1", "MANAGER" },
                    { "10", "FRONT END MANAGER" },
                    { "11", "ASST HEAD CASHIER" },
                    { "13", "CASHIERS - AM" },
                    { "14", "CASHIERS - PM" },
                    { "15", "CASHIER 14-15" },
                    { "16", "SACKERS - AM" },
                    { "17", "SACKERS - PM" },
                    { "18", "SACKERS 14-15" },
                    { "19", "AVAILABLE" },
                    { "2", "ASSISTANT MANAGER" },
                    { "20", "OFFICE MANAGER" },
                    { "21", "ASST OFFICE MANAGER" },
                    { "22", "COURTESY BOOTH - FT" },
                    { "23", "COURTESY BOOTH - PT" },
                    { "24", "POS - FULL TIME" },
                    { "25", "CLERK - FULL TIME AP" },
                    { "26", "CLERKS - FULL TIME AR" },
                    { "27", "CLERKS - FULL TIME GROC" },
                    { "28", "CLERKS - FULL TIME PERSH" },
                    { "29", "CLERKS F-T WAREHOUSE" },
                    { "30", "MERCHANDISER" },
                    { "31", "GROCERY MANAGER" },
                    { "32", "ENDS - PART TIME" },
                    { "33", "FIRST MEAT CUTTER" },
                    { "35", "FT BAKER/BENCH" },
                    { "36", "MARKETS KITCHEN PT 16-17" },
                    { "37", "CAFE PART TIME" },
                    { "38", "RECEIVER" },
                    { "39", "LOSS PREVENTION" },
                    { "4", "SPIRITS MANAGER" },
                    { "40", "MEAT CUTTERS" },
                    { "41", "APPR MEAT CUTTERS" },
                    { "42", "MEAT CUTTER (PART TIME)" },
                    { "44", "PART TIME SUBSHOP" },
                    { "45", "ASST SUB SHOP MANAGER" },
                    { "46", "SERVICE CASE - FULL TIME" },
                    { "47", "WRAPPERS - FULL TIME" },
                    { "48", "WRAPPERS - PART TIME AM" },
                    { "49", "WRAPPERS - PART TIME PM" },
                    { "5", "ASST SPIRITS MANAGER" },
                    { "50", "HEAD CLERK" },
                    { "51", "SUB SHOP MANAGER" },
                    { "52", "CLERKS - FULL TIME AM" },
                    { "53", "CLERKS - PART TIME AM" },
                    { "54", "CLERK - PART TIME PM" },
                    { "55", "POS - PART TIME" },
                    { "56", "MARKETS KITCHEN-ASST MGR" },
                    { "57", "MARKETS KITCHEN FT" },
                    { "58", "MARKETS KITCHEN PT" },
                    { "59", "KITCHEN MANAGER" },
                    { "6", "SPIRITS CLERK - FT" },
                    { "62", "FT CAKE" },
                    { "63", "PT CAKE" },
                    { "64", "OVEN WORKER PT" },
                    { "65", "BENCH WORKER PT" },
                    { "66", "FORK LIFT OPR (REC)- AM" },
                    { "67", "FORK LIFT OPR (REC)- PM" },
                    { "68", "FORK LIFT OPR (SHIP)- AM" },
                    { "69", "FORK LIFT OPR (SHIP)- PM" },
                    { "7", "SPIRITS CLERK - PT" },
                    { "70", "FORK LIFT OPR (MISC)- AM" },
                    { "71", "FORK LIFT OPR (MISC)- PM" },
                    { "72", "LOADER - AM" },
                    { "73", "LOADER - PM" },
                    { "74", "GENERAL WAREHOUSE - FT - AM" },
                    { "75", "GENERAL WAREHOUSE - PT - AM" },
                    { "77", "SELECTOR (PART-TIME) AM" },
                    { "78", "SELECTOR (PART-TIME) PM" },
                    { "79", "SELECTOR FULL TIME-AM" },
                    { "81", "SELECTOR (FULL-TIME) PM" },
                    { "82", "INSPECTOR" },
                    { "83", "GENERAL WAREHOUSE - FT - PM" },
                    { "84", "GENERAL WAREHOUSE - PT - PM" },
                    { "85", "DRIVER (TRAILER)" },
                    { "86", "EXCEL" },
                    { "87", "MECHANIC" },
                    { "89", "FACILITY OPERATIONS" },
                    { "90", "COMPUTER OPERATIONS" },
                    { "91", "SIGN SHOP" },
                    { "92", "INVENTORY" },
                    { "93", "PROGRAMMING" },
                    { "94", "HELP DESK" },
                    { "96", "TECHNICAL SUPPORT" },
                    { "97", "EXECUTIVE OFFICE" },
                    { "98", "TRAINING" },
                    { "99", "Indian Ridge" },
                    { "AD1", "AD1-MANAGER" },
                    { "AD2", "AD2-RECEPTIONIST" },
                    { "DR1", "DR1-BARTENDER" },
                    { "DR2", "DR2-BUSSER" },
                    { "DR3", "DR3-HOSTESS" },
                    { "DR4", "DR4-MANAGER" },
                    { "DR5", "DR5-SERVER" },
                    { "DR6", "DR6-SERVER" },
                    { "FM1", "FM1-MAINTENANCE ATTENDANT" },
                    { "FM2", "FM2-MAINTENANCE ATTENDANT" },
                    { "FM3", "FM3-MANAGER-FACILITY MAINTENANCE" },
                    { "FM4", "FM4-MAINT ATTEND" },
                    { "FM5", "FM5-MANAGER" },
                    { "FT1", "FT1-BARTENDER" },
                    { "FT2", "FT2-MANAGER" },
                    { "FT3", "FT3-SERVER" },
                    { "GM1", "GM1-GOLF CART MAINT" },
                    { "GM2", "GM2-GOLF CART MAINT" },
                    { "GM3", "GM3-GROUNDS MAINTENANCE" },
                    { "GM4", "GM4-GROUNDS MAINTENANCE" },
                    { "GM5", "GM5-MANAGER" },
                    { "GM6", "GM6-MECHANIC" },
                    { "GR1", "GR1-BUSSER" },
                    { "GR2", "GR2-MANAGER" },
                    { "GR3", "GR3-SERVER" },
                    { "GR4", "GR4-SNACK SHACK" },
                    { "GR5", "GR5-POOLSIDE-GRILLE ROOM" },
                    { "KT1", "KT1-MANAGER" },
                    { "KT2", "KT2-CHEF" },
                    { "KT3", "KT3-CHEF-KITCHEN" },
                    { "KT4", "KT4-DISHWASHER" },
                    { "KT5", "KT5-DISHWASHER-KITCHEN" },
                    { "KT6", "KT6-PREP CHEF" },
                    { "LG1", "LG1-MANAGER" },
                    { "LG2", "LG2-LIFEGUARD" },
                    { "PS1", "PS1-PRO SHOP SERVICES" },
                    { "PS2", "PS2-PRO SHOP SERVICES" },
                    { "PS3", "PS3-MANAGER" },
                    { "PS4", "PS4-OUTSIDE SERVICES" },
                    { "PS5", "PS5-OUTSIDE SERVICES" },
                    { "PS6", "PS6-STARTER" },
                    { "PS7", "PS7-STARTER" },
                    { "TN1", "TN1-MANAGER" },
                    { "TN2", "TN2-TENNIS SERVICES" }
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
                columns: new[] { "ID", "FREQUENCY", "NAME" },
                values: new object[,]
                {
                    { (byte)0, "Year-end only", "Incoming contributions, forfeitures, earnings" },
                    { (byte)1, "Multiple Times", "Outgoing payments (not rollovers or direct payments) - Partial withdrawal" },
                    { (byte)2, "Multiple Times", "Outgoing forfeitures" },
                    { (byte)3, "Multiple Times", "Outgoing direct payments / rollover payments" },
                    { (byte)5, "Once", "Outgoing XFER beneficiary / QDRO allocation (beneficiary payment)" },
                    { (byte)6, "Once", "Incoming QDRO beneficiary allocation (beneficiary receipt)" },
                    { (byte)8, "Usually year-end, unless there is special processing – i.e. Class Action settlement. Earnings are 100% vested.", "Incoming \"100% vested\" earnings" },
                    { (byte)9, "Multiple Times", "Outgoing payment from 100% vesting amount (payment of ETVA funds)" }
                });

            migrationBuilder.InsertData(
                table: "TAX_CODE",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { "0", "Unknown - not legal tax code, yet 24 records in the obfuscated set have this value." },
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
                    { "Z", "IsDeceased" }
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
                name: "IX_ANNUITY_RATE_YEAR_AGE",
                table: "ANNUITY_RATE",
                columns: new[] { "YEAR", "AGE" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_EVENT_TABLENAME",
                table: "AUDIT_EVENT",
                column: "TABLE_NAME");

            migrationBuilder.CreateIndex(
                name: "IX_BENEFICIARY_BADGENUMBER",
                table: "BENEFICIARY",
                column: "BADGE_NUMBER");

            migrationBuilder.CreateIndex(
                name: "IX_BENEFICIARY_BENEFICIARYCONTACTID",
                table: "BENEFICIARY",
                column: "BENEFICIARY_CONTACT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_BENEFICIARY_DEMOGRAPHICID",
                table: "BENEFICIARY",
                column: "DEMOGRAPHIC_ID");

            migrationBuilder.CreateIndex(
                name: "IX_BENEFICIARY_KINDID",
                table: "BENEFICIARY",
                column: "KIND_ID");

            migrationBuilder.CreateIndex(
                name: "IX_BENEFICIARY_PSNSUFFIX",
                table: "BENEFICIARY",
                column: "PSN_SUFFIX");

            migrationBuilder.CreateIndex(
                name: "IX_BENEFICIARY_CONTACT_COUNTRY_ISO",
                table: "BENEFICIARY_CONTACT",
                column: "COUNTRY_ISO");

            migrationBuilder.CreateIndex(
                name: "IX_BENEFICIARY_CONTACT_SSN",
                table: "BENEFICIARY_CONTACT",
                column: "SSN");

            migrationBuilder.CreateIndex(
                name: "IX_BENEFICIARY_CONTACT_ARCHIVE_COUNTRY_ISO",
                table: "BENEFICIARY_CONTACT_ARCHIVE",
                column: "COUNTRY_ISO");

            migrationBuilder.CreateIndex(
                name: "IX_BENEFICIARY_SSN_CHANGE_HISTORY_BENEFICIARYCONTACTID",
                table: "BENEFICIARY_SSN_CHANGE_HISTORY",
                column: "BENEFICIARY_CONTACT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CALDAR_RECORD_WEEKNO_PERIOD",
                table: "CALDAR_RECORD",
                columns: new[] { "ACC_WEEKN", "ACC_PERIOD" });

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_BADGENUMBER",
                table: "DEMOGRAPHIC",
                column: "BADGE_NUMBER",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_COUNTRY_ISO",
                table: "DEMOGRAPHIC",
                column: "COUNTRY_ISO");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_DATEOFBIRTH",
                table: "DEMOGRAPHIC",
                column: "DATE_OF_BIRTH");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_DEPARTMENTID",
                table: "DEMOGRAPHIC",
                column: "DEPARTMENT");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_EMPLOYMENTSTATUSID",
                table: "DEMOGRAPHIC",
                column: "EMPLOYMENT_STATUS_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_EMPLOYMENTSTATUSID_HIREDATE_TERMINATIONDATE",
                table: "DEMOGRAPHIC",
                columns: new[] { "EMPLOYMENT_STATUS_ID", "HIRE_DATE", "TERMINATION_DATE" });

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_EMPLOYMENTSTATUSID_TERMINATIONDATE",
                table: "DEMOGRAPHIC",
                columns: new[] { "EMPLOYMENT_STATUS_ID", "TERMINATION_DATE" });

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_EMPLOYMENTTYPEID",
                table: "DEMOGRAPHIC",
                column: "EMPLOYMENT_TYPE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_FULL_NAME",
                table: "DEMOGRAPHIC",
                column: "FULL_NAME");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_GENDERID",
                table: "DEMOGRAPHIC",
                column: "GENDER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_HIREDATE",
                table: "DEMOGRAPHIC",
                column: "HIRE_DATE");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_ORACLEHCMID",
                table: "DEMOGRAPHIC",
                column: "ORACLE_HCM_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_PAYCLASSIFICATIONID",
                table: "DEMOGRAPHIC",
                column: "PAY_CLASSIFICATION_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_PAYFREQUENCYID",
                table: "DEMOGRAPHIC",
                column: "PAY_FREQUENCY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_SSN",
                table: "DEMOGRAPHIC",
                column: "SSN");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_SSN_BADGENUMBER",
                table: "DEMOGRAPHIC",
                columns: new[] { "SSN", "BADGE_NUMBER" });

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_SSN_ORACLEHCMID",
                table: "DEMOGRAPHIC",
                columns: new[] { "SSN", "ORACLE_HCM_ID" });

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_TERMINATIONCODEID",
                table: "DEMOGRAPHIC",
                column: "TERMINATION_CODE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_TERMINATIONDATE",
                table: "DEMOGRAPHIC",
                column: "TERMINATION_DATE");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_HISTORY_BADGENUMBER",
                table: "DEMOGRAPHIC_HISTORY",
                column: "BADGE_NUMBER");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_HISTORY_DEMOGRAPHICID",
                table: "DEMOGRAPHIC_HISTORY",
                column: "DEMOGRAPHIC_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_SSN_CHANGE_HISTORY_DEMOGRAPHICID",
                table: "DEMOGRAPHIC_SSN_CHANGE_HISTORY",
                column: "DEMOGRAPHIC_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DEMOGRAPHIC_SYNC_AUDIT_BADGENUMBER",
                table: "DEMOGRAPHIC_SYNC_AUDIT",
                column: "BADGE_NUMBER");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_FREQUENCYID",
                table: "DISTRIBUTION",
                column: "FREQUENCY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_GENDERID",
                table: "DISTRIBUTION",
                column: "GENDER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_PAYEEID",
                table: "DISTRIBUTION",
                column: "PAYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_SSN_PAYMENTSEQUENCE",
                table: "DISTRIBUTION",
                columns: new[] { "SSN", "PAYMENT_SEQUENCE" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_STATUSID",
                table: "DISTRIBUTION",
                column: "STATUS_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_TAXCODEID",
                table: "DISTRIBUTION",
                column: "TAX_CODE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_THIRDPARTYPAYEEID",
                table: "DISTRIBUTION",
                column: "THIRD_PARTY_PAYEE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_PAYEE_COUNTRY_ISO",
                table: "DISTRIBUTION_PAYEE",
                column: "COUNTRY_ISO");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_PAYEE_SSN",
                table: "DISTRIBUTION_PAYEE",
                column: "SSN");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_REQUEST_DEMOGRAPHICID",
                table: "DISTRIBUTION_REQUEST",
                column: "DEMOGRAPHIC_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_REQUEST_REASONID",
                table: "DISTRIBUTION_REQUEST",
                column: "REASON_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_REQUEST_STATUSID",
                table: "DISTRIBUTION_REQUEST",
                column: "STATUS_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_REQUEST_TAXCODEID",
                table: "DISTRIBUTION_REQUEST",
                column: "TAX_CODE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_REQUEST_TYPEID",
                table: "DISTRIBUTION_REQUEST",
                column: "TYPE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_DISTRIBUTION_THIRDPARTY_PAYEE_COUNTRY_ISO",
                table: "DISTRIBUTION_THIRDPARTY_PAYEE",
                column: "COUNTRY_ISO");

            migrationBuilder.CreateIndex(
                name: "IX_EXCLUDED_ID_EXCLUDEDIDTYPEID",
                table: "EXCLUDED_ID",
                column: "EXCLUDED_ID_TYPE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_FAKE_SSNS_SSN",
                table: "FAKE_SSNS",
                column: "SSN",
                unique: true);

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
                name: "IX_NAVIGATION_PARENT_ID",
                table: "NAVIGATION",
                column: "PARENT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_NAVIGATION_STATUS_ID",
                table: "NAVIGATION",
                column: "STATUS_ID");

            migrationBuilder.CreateIndex(
                name: "IX_NAVIGATION_ASSIGNED_ROLES_REQUIREDROLESID",
                table: "NAVIGATION_ASSIGNED_ROLES",
                column: "REQUIREDROLESID");

            migrationBuilder.CreateIndex(
                name: "IX_NAVIGATION_PREREQUISITES_PREREQUISITE_ID",
                table: "NAVIGATION_PREREQUISITES",
                column: "PREREQUISITE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_NAVIGATION_TRACKING_NAVIGATIONID",
                table: "NAVIGATION_TRACKING",
                column: "NAVIGATION_ID");

            migrationBuilder.CreateIndex(
                name: "IX_NAVIGATION_TRACKING_STATUS_ID",
                table: "NAVIGATION_TRACKING",
                column: "STATUS_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PAY_PROFIT_BENEFICIARYTYPEID",
                table: "PAY_PROFIT",
                column: "BENEFICIARY_TYPE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PAY_PROFIT_EMPLOYEETYPEID",
                table: "PAY_PROFIT",
                column: "EMPLOYEE_TYPE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PAY_PROFIT_ENROLLMENTID",
                table: "PAY_PROFIT",
                column: "ENROLLMENT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PAY_PROFIT_PROFITYEAR",
                table: "PAY_PROFIT",
                column: "PROFIT_YEAR");

            migrationBuilder.CreateIndex(
                name: "IX_PAY_PROFIT_PROFITYEAR_DEMOGRAPHICID",
                table: "PAY_PROFIT",
                columns: new[] { "PROFIT_YEAR", "DEMOGRAPHIC_ID" });

            migrationBuilder.CreateIndex(
                name: "IX_PAY_PROFIT_ZEROCONTRIBUTIONREASONID",
                table: "PAY_PROFIT",
                column: "ZERO_CONTRIBUTION_REASON_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_DETAIL_COMMENTTYPEID",
                table: "PROFIT_DETAIL",
                column: "COMMENT_TYPE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_DETAIL_PROFITCODEID",
                table: "PROFIT_DETAIL",
                column: "PROFIT_CODE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_DETAIL_PROFITYEAR_MONTHTODATE",
                table: "PROFIT_DETAIL",
                columns: new[] { "PROFIT_YEAR", "MONTH_TO_DATE" });

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_DETAIL_PROFITYEAR_PROFITCODEID",
                table: "PROFIT_DETAIL",
                columns: new[] { "PROFIT_YEAR", "PROFIT_CODE_ID" });

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_DETAIL_SSN",
                table: "PROFIT_DETAIL",
                column: "SSN");

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_DETAIL_SSN_PROFITYEAR",
                table: "PROFIT_DETAIL",
                columns: new[] { "SSN", "PROFIT_YEAR" });

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_DETAIL_SSN_PROFITYEAR_PROFITCODEID",
                table: "PROFIT_DETAIL",
                columns: new[] { "SSN", "PROFIT_YEAR", "PROFIT_CODE_ID" });

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_DETAIL_TAXCODEID",
                table: "PROFIT_DETAIL",
                column: "TAX_CODE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_DETAIL_ZEROCONTRIBUTIONREASONID",
                table: "PROFIT_DETAIL",
                column: "ZERO_CONTRIBUTION_REASON_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_SHARE_CHECK_CHECKNUMBER_ISVOIDED",
                table: "PROFIT_SHARE_CHECK",
                columns: new[] { "CHECK_NUMBER", "VOID_FLAG" });

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_SHARE_CHECK_DEMOGRAPHICID",
                table: "PROFIT_SHARE_CHECK",
                column: "DEMOGRAPHIC_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_SHARE_CHECK_PSCCHECKID",
                table: "PROFIT_SHARE_CHECK",
                column: "PSC_CHECK_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_SHARE_CHECK_SSN",
                table: "PROFIT_SHARE_CHECK",
                column: "SSN");

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_SHARE_CHECK_TAXCODEID",
                table: "PROFIT_SHARE_CHECK",
                column: "TAX_CODE_ID");

            migrationBuilder.CreateIndex(
                name: "IDX_REPORT_CHECKSUM_LOOKUP",
                table: "REPORT_CHECKSUM",
                columns: new[] { "PROFIT_YEAR", "REPORT_TYPE", "CREATED_AT_UTC" },
                descending: new[] { false, false, true });

            migrationBuilder.CreateIndex(
                name: "IX_YE_UPDATE_STATUS_PROFIT_YEAR",
                table: "YE_UPDATE_STATUS",
                column: "PROFIT_YEAR",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "_HEALTH_CHECK_STATUS_HISTORY");

            migrationBuilder.DropTable(
                name: "ANNUITY_RATE");

            migrationBuilder.DropTable(
                name: "AUDIT_EVENT");

            migrationBuilder.DropTable(
                name: "BENEFICIARY");

            migrationBuilder.DropTable(
                name: "BENEFICIARY_ARCHIVE");

            migrationBuilder.DropTable(
                name: "BENEFICIARY_CONTACT_ARCHIVE");

            migrationBuilder.DropTable(
                name: "BENEFICIARY_SSN_CHANGE_HISTORY");

            migrationBuilder.DropTable(
                name: "CALDAR_RECORD");

            migrationBuilder.DropTable(
                name: "DATA_IMPORT_RECORD");

            migrationBuilder.DropTable(
                name: "DEMOGRAPHIC_HISTORY");

            migrationBuilder.DropTable(
                name: "DEMOGRAPHIC_SSN_CHANGE_HISTORY");

            migrationBuilder.DropTable(
                name: "DEMOGRAPHIC_SYNC_AUDIT");

            migrationBuilder.DropTable(
                name: "DISTRIBUTION");

            migrationBuilder.DropTable(
                name: "DISTRIBUTION_REQUEST");

            migrationBuilder.DropTable(
                name: "EXCLUDED_ID");

            migrationBuilder.DropTable(
                name: "FAKE_SSNS");

            migrationBuilder.DropTable(
                name: "FROZEN_STATE");

            migrationBuilder.DropTable(
                name: "JOB");

            migrationBuilder.DropTable(
                name: "MISSIVES");

            migrationBuilder.DropTable(
                name: "NAVIGATION_ASSIGNED_ROLES");

            migrationBuilder.DropTable(
                name: "NAVIGATION_PREREQUISITES");

            migrationBuilder.DropTable(
                name: "NAVIGATION_TRACKING");

            migrationBuilder.DropTable(
                name: "PAY_PROFIT");

            migrationBuilder.DropTable(
                name: "PROFIT_DETAIL");

            migrationBuilder.DropTable(
                name: "PROFIT_SHARE_CHECK");

            migrationBuilder.DropTable(
                name: "REPORT_CHECKSUM");

            migrationBuilder.DropTable(
                name: "STATE_TAX");

            migrationBuilder.DropTable(
                name: "YE_UPDATE_STATUS");

            migrationBuilder.DropTable(
                name: "BENEFICIARY_KIND");

            migrationBuilder.DropTable(
                name: "BENEFICIARY_CONTACT");

            migrationBuilder.DropTable(
                name: "DISTRIBUTION_PAYEE");

            migrationBuilder.DropTable(
                name: "DISTRIBUTION_STATUS");

            migrationBuilder.DropTable(
                name: "DISTRIBUTION_THIRDPARTY_PAYEE");

            migrationBuilder.DropTable(
                name: "DISTRIBUTION_FREQUENCY");

            migrationBuilder.DropTable(
                name: "DISTRIBUTION_REQUEST_REASON");

            migrationBuilder.DropTable(
                name: "DISTRIBUTION_REQUEST_STATUS");

            migrationBuilder.DropTable(
                name: "DISTRIBUTION_REQUEST_TYPE");

            migrationBuilder.DropTable(
                name: "EXCLUDED_ID_TYPE");

            migrationBuilder.DropTable(
                name: "JOBSTARTMETHOD");

            migrationBuilder.DropTable(
                name: "JOBSTATUS");

            migrationBuilder.DropTable(
                name: "JOBTYPE");

            migrationBuilder.DropTable(
                name: "NAVIGATION_ROLE");

            migrationBuilder.DropTable(
                name: "NAVIGATION");

            migrationBuilder.DropTable(
                name: "BENEFICIARY_TYPE");

            migrationBuilder.DropTable(
                name: "EMPLOYEE_TYPE");

            migrationBuilder.DropTable(
                name: "ENROLLMENT");

            migrationBuilder.DropTable(
                name: "COMMENT_TYPE");

            migrationBuilder.DropTable(
                name: "PROFIT_CODE");

            migrationBuilder.DropTable(
                name: "ZERO_CONTRIBUTION_REASON");

            migrationBuilder.DropTable(
                name: "DEMOGRAPHIC");

            migrationBuilder.DropTable(
                name: "TAX_CODE");

            migrationBuilder.DropTable(
                name: "NAVIGATION_STATUS");

            migrationBuilder.DropTable(
                name: "COUNTRY");

            migrationBuilder.DropTable(
                name: "DEPARTMENT");

            migrationBuilder.DropTable(
                name: "EMPLOYMENT_STATUS");

            migrationBuilder.DropTable(
                name: "EMPLOYMENT_TYPE");

            migrationBuilder.DropTable(
                name: "GENDER");

            migrationBuilder.DropTable(
                name: "PAY_CLASSIFICATION");

            migrationBuilder.DropTable(
                name: "PAY_FREQUENCY");

            migrationBuilder.DropTable(
                name: "TERMINATION_CODE");

            migrationBuilder.DropSequence(
                name: "FAKE_SSN_SEQ");
        }
    }
}
