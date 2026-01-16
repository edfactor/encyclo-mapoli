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
                name: "BANK_ACCOUNT_SEQ",
                minValue: 1L);

            migrationBuilder.CreateSequence<int>(
                name: "BANK_SEQ",
                minValue: 1L);

            migrationBuilder.CreateSequence<int>(
                name: "FAKE_SSN_SEQ",
                startValue: 666000000L,
                minValue: 666000000L,
                maxValue: 666999999L);

            migrationBuilder.CreateSequence<int>(
                name: "PROFIT_SHARE_CHECK_NUMBER_SEQ",
                minValue: 1L);

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
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ANNUITY_RATE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ANNUITY_RATE_CONFIG",
                columns: table => new
                {
                    YEAR = table.Column<short>(type: "NUMBER(4)", precision: 4, nullable: false),
                    MINIMUM_AGE = table.Column<byte>(type: "NUMBER(3)", precision: 3, nullable: false),
                    MAXIMUM_AGE = table.Column<byte>(type: "NUMBER(3)", precision: 3, nullable: false),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ANNUITY_RATE_CONFIG", x => x.YEAR);
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
                    SESSION_ID = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    CREATED_AT = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    CHANGES_JSON = table.Column<string>(type: "CLOB", nullable: true),
                    CHANGES_HASH = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_EVENT", x => x.AUDIT_EVENT_ID);
                });

            migrationBuilder.CreateTable(
                name: "BANK",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValueSql: "BANK_SEQ.NEXTVAL"),
                    ROUTING_NUMBER = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    NAME = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    OFFICE_TYPE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    CITY = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    STATE = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: true),
                    PHONE = table.Column<string>(type: "NVARCHAR2(24)", maxLength: 24, nullable: true),
                    STATUS = table.Column<string>(type: "NVARCHAR2(24)", maxLength: 24, nullable: true),
                    FEDACH_CHANGE_DATE = table.Column<DateTime>(type: "DATE", nullable: true),
                    SERVICING_FED_ROUTING_NUMBER = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    SERVICING_FED_ADDRESS = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    FEDWIRE_TELEGRAPHIC_NAME = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    FEDWIRE_LOCATION = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    FEDWIRE_REVISION_DATE = table.Column<DateTime>(type: "DATE", nullable: true),
                    IS_DISABLED = table.Column<bool>(type: "NUMBER(1)", nullable: false, defaultValue: false),
                    CREATEDBY = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    MODIFIEDBY = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BANK", x => x.ID);
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
                    PERCENT = table.Column<decimal>(type: "numeric(3,0)", precision: 3, nullable: false),
                    DELETE_DATE = table.Column<string>(type: "NVARCHAR2(10)", nullable: false),
                    DELETED_BY = table.Column<string>(type: "NVARCHAR2(24)", maxLength: 24, nullable: false, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BENEFICIARY_ARCHIVE", x => x.ARCHIVE_ID);
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
                name: "CHECK_RUN_WORKFLOW",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PROFITYEAR = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    CHECKRUNDATE = table.Column<string>(type: "NVARCHAR2(10)", nullable: false),
                    STEPNUMBER = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    STEPSTATUS = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CHECKNUMBER = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    REPRINTCOUNT = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValue: 0),
                    MAXREPRINTCOUNT = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValue: 2),
                    CREATEDBYUSERNAME = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    CREATEDDATE = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    MODIFIEDBYUSERNAME = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    MODIFIEDDATE = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHECK_RUN_WORKFLOW", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "COMMENT_TYPE",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    ISPROTECTED = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true)
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
                    FIRST_NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true, comment: "FirstName from ContactInfo"),
                    LAST_NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true, comment: "LastName from ContactInfo"),
                    MIDDLE_NAME = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: true, comment: "MiddleName from ContactInfo"),
                    PHONE_NUMBER = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: true, comment: "PhoneNumber from ContactInfo"),
                    MOBILE_NUMBER = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: true, comment: "MobileNumber from ContactInfo"),
                    EMAIL_ADDRESS = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true, comment: "EmailAddress from ContactInfo"),
                    STREET = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true, comment: "Street from Address"),
                    STREET2 = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true, comment: "Street2 from Address"),
                    CITY = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: true, comment: "City from Address"),
                    STATE = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: true, comment: "State from Address"),
                    POSTAL_CODE = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true, comment: "PostalCode from Address"),
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
                name: "FILE_TRANSFER_AUDIT",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TIMESTAMP = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    FILENAME = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    DESTINATION = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    FILESIZE = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    TRANSFERDURATIONMS = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ISSUCCESS = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ERRORMESSAGE = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    USERNAME = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    CSVCONTENT = table.Column<byte[]>(type: "RAW(2000)", nullable: true),
                    CHECKRUNWORKFLOWID = table.Column<Guid>(type: "RAW(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FILE_TRANSFER_AUDIT", x => x.ID);
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
                name: "FTP_OPERATION_LOG",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CHECKRUNWORKFLOWID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OPERATIONTYPE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    FILENAME = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    DESTINATION = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    ISSUCCESS = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ERRORMESSAGE = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    DURATIONMS = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    TIMESTAMP = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    USERNAME = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FTP_OPERATION_LOG", x => x.ID);
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
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REPORT_CHECKSUM", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RMDS_FACTOR_BY_AGE",
                columns: table => new
                {
                    AGE = table.Column<byte>(type: "NUMBER(3)", precision: 3, nullable: false),
                    FACTOR = table.Column<decimal>(type: "DECIMAL(4,1)", precision: 4, scale: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RMDS_FACTOR_BY_AGE", x => x.AGE);
                });

            migrationBuilder.CreateTable(
                name: "STATE",
                columns: table => new
                {
                    ABBREVIATION = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STATE", x => x.ABBREVIATION);
                });

            migrationBuilder.CreateTable(
                name: "STATE_TAX",
                columns: table => new
                {
                    ABBREVIATION = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false),
                    RATE = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true)
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
                name: "VESTING_SCHEDULE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DESCRIPTION = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    IS_ACTIVE = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CREATED_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    EFFECTIVE_DATE = table.Column<string>(type: "NVARCHAR2(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VESTING_SCHEDULE", x => x.ID);
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
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true)
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
                name: "BANK_ACCOUNT",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValueSql: "BANK_ACCOUNT_SEQ.NEXTVAL"),
                    BANK_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ROUTING_NUMBER = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: false),
                    ACCOUNT_NUMBER = table.Column<string>(type: "NVARCHAR2(34)", maxLength: 34, nullable: false),
                    ACCOUNT_NAME = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    IS_PRIMARY = table.Column<bool>(type: "NUMBER(1)", nullable: false, defaultValue: false),
                    IS_DISABLED = table.Column<bool>(type: "NUMBER(1)", nullable: false, defaultValue: false),
                    SERVICING_FED_ROUTING_NUMBER = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    SERVICING_FED_ADDRESS = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    FEDWIRE_TELEGRAPHIC_NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    FEDWIRE_LOCATION = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    FED_ACH_CHANGE_DATE = table.Column<DateTime>(type: "DATE", nullable: true),
                    FEDWIRE_REVISION_DATE = table.Column<DateTime>(type: "DATE", nullable: true),
                    NOTES = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    EFFECTIVE_DATE = table.Column<DateTime>(type: "DATE", nullable: true),
                    DISCONTINUED_DATE = table.Column<DateTime>(type: "DATE", nullable: true),
                    CREATEDBY = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    MODIFIEDBY = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BANK_ACCOUNT", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BANK_ACCOUNT_BANK_BANKID",
                        column: x => x.BANK_ID,
                        principalTable: "BANK",
                        principalColumn: "ID");
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
                    FULL_NAME = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: true, computedColumnSql: "LAST_NAME || ', ' || FIRST_NAME || CASE WHEN MIDDLE_NAME IS NOT NULL THEN ' ' || SUBSTR(MIDDLE_NAME,1,1) ELSE NULL END", stored: true, comment: "Automatically computed from LastName, FirstName, and MiddleName with middle initial"),
                    LAST_NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false, comment: "LastName"),
                    FIRST_NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false, comment: "FirstName"),
                    MIDDLE_NAME = table.Column<string>(type: "NVARCHAR2(25)", maxLength: 25, nullable: true, comment: "MiddleName"),
                    PHONE_NUMBER = table.Column<string>(type: "NVARCHAR2(16)", maxLength: 16, nullable: true),
                    MOBILE_NUMBER = table.Column<string>(type: "NVARCHAR2(16)", maxLength: 16, nullable: true),
                    EMAIL_ADDRESS = table.Column<string>(type: "NVARCHAR2(84)", maxLength: 84, nullable: true),
                    CREATED_DATE = table.Column<DateTime>(type: "DATE", nullable: false, defaultValueSql: "SYSDATE"),
                    ISDELETED = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true)
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
                    FULL_NAME = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: true, computedColumnSql: "LAST_NAME || ', ' || FIRST_NAME || CASE WHEN MIDDLE_NAME IS NOT NULL THEN ' ' || SUBSTR(MIDDLE_NAME,1,1) ELSE NULL END", stored: true, comment: "Automatically computed from LastName, FirstName, and MiddleName with middle initial"),
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
                    FULL_NAME = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: true, computedColumnSql: "LAST_NAME || ', ' || FIRST_NAME || CASE WHEN MIDDLE_NAME IS NOT NULL THEN ' ' || SUBSTR(MIDDLE_NAME,1,1) ELSE NULL END", stored: true, comment: "Automatically computed from LastName, FirstName, and MiddleName with middle initial"),
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
                    DATE_OF_DEATH = table.Column<DateTime>(type: "DATE", nullable: true, comment: "DateOfDeath"),
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
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true),
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
                        name: "FK_DEMOGRAPHIC_TERMINATIONCODES_TERMINATIONCODEID",
                        column: x => x.TERMINATION_CODE_ID,
                        principalTable: "TERMINATION_CODE",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "VESTING_SCHEDULE_DETAIL",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    VESTING_SCHEDULE_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    YEARS_OF_SERVICE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    VESTING_PERCENT = table.Column<decimal>(type: "DECIMAL(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VESTING_SCHEDULE_DETAIL", x => x.ID);
                    table.ForeignKey(
                        name: "FK_VSD_SCHEDULE",
                        column: x => x.VESTING_SCHEDULE_ID,
                        principalTable: "VESTING_SCHEDULE",
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
                    YEARS_OF_SERVICE_CREDIT = table.Column<short>(type: "NUMBER(3)", nullable: false, defaultValue: (short)0),
                    REVERSED_FROM_PROFIT_DETAIL_ID = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true)
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
                        name: "FK_PROFIT_DETAIL_PROFIT_DETAIL_REVERSEDFROMPROFITDETAILID",
                        column: x => x.REVERSED_FROM_PROFIT_DETAIL_ID,
                        principalTable: "PROFIT_DETAIL",
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
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true),
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
                    MANUAL_CHECK_NUMBER = table.Column<string>(type: "NVARCHAR2(16)", maxLength: 16, nullable: true),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true)
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
                name: "NAVIGATION_CUSTOM_SETTING",
                columns: table => new
                {
                    NAVIGATION_ID = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    KEY = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    VALUE_JSON = table.Column<string>(type: "CLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NAVIGATION_CUSTOM_SETTING", x => new { x.NAVIGATION_ID, x.KEY });
                    table.ForeignKey(
                        name: "FK_NAVIGATION_CUSTOM_SETTING_NAVIGATION_NAVIGATIONID",
                        column: x => x.NAVIGATION_ID,
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
                    PERCENT = table.Column<decimal>(type: "numeric(3,0)", precision: 3, nullable: false),
                    ISDELETED = table.Column<bool>(type: "NUMBER(1)", nullable: false, defaultValue: false),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true),
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
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true),
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
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true)
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
                    VESTING_SCHEDULE_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    HAS_FORFEITED = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    BENEFICIARY_TYPE_ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    EMPLOYEE_TYPE_ID = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    ZERO_CONTRIBUTION_REASON_ID = table.Column<byte>(type: "NUMBER(3)", nullable: true),
                    HOURS_EXECUTIVE = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: false),
                    INCOME_EXECUTIVE = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false),
                    POINTS_EARNED = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: true),
                    TOTAL_HOURS = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: false, computedColumnSql: "HOURS_EXECUTIVE + CURRENT_HOURS_YEAR", stored: true),
                    TOTAL_INCOME = table.Column<decimal>(type: "DECIMAL(9,2)", precision: 9, scale: 2, nullable: false, computedColumnSql: "INCOME_EXECUTIVE + CURRENT_INCOME_YEAR", stored: true),
                    CREATED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    USER_NAME = table.Column<string>(type: "NVARCHAR2(96)", maxLength: 96, nullable: true, defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')"),
                    MODIFIED_AT_UTC = table.Column<DateTimeOffset>(type: "TIMESTAMP WITH TIME ZONE", nullable: true)
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
                    CHECK_RUN_WORKFLOW_ID = table.Column<Guid>(type: "RAW(16)", nullable: true),
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
                        name: "FK_PROFIT_SHARE_CHECK_CHECK_RUN_WORKFLOW_CHECKRUNWORKFLOWID",
                        column: x => x.CHECK_RUN_WORKFLOW_ID,
                        principalTable: "CHECK_RUN_WORKFLOW",
                        principalColumn: "ID");
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
                table: "BANK",
                columns: new[] { "ID", "CITY", "CREATED_AT_UTC", "CREATEDBY", "FEDACH_CHANGE_DATE", "FEDWIRE_LOCATION", "FEDWIRE_REVISION_DATE", "FEDWIRE_TELEGRAPHIC_NAME", "MODIFIED_AT_UTC", "MODIFIEDBY", "NAME", "OFFICE_TYPE", "PHONE", "ROUTING_NUMBER", "SERVICING_FED_ADDRESS", "SERVICING_FED_ROUTING_NUMBER", "STATE", "STATUS" },
                values: new object[] { 1, "Lake Success", new DateTimeOffset(new DateTime(2026, 1, 16, 0, 18, 48, 696, DateTimeKind.Unspecified).AddTicks(9709), new TimeSpan(0, 0, 0, 0, 0)), "SYSTEM", new DateTime(2024, 7, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Miami, FL", new DateTime(2023, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "NEWTEK BANK, NA", null, null, "Newtek Bank, NA", "Main Office", "516-254-7586", "026004297", "100 Orchard Street, East Rutherford, NJ", "021001208", "NY", "Active" });

            migrationBuilder.InsertData(
                table: "BENEFICIARY_TYPE",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { (byte)0, "Employee" },
                    { (byte)1, "Beneficiary" }
                });

            migrationBuilder.InsertData(
                table: "COMMENT_TYPE",
                columns: new[] { "ID", "ISPROTECTED", "MODIFIED_AT_UTC", "NAME" },
                values: new object[,]
                {
                    { (byte)1, true, null, "Transfer Out" },
                    { (byte)2, true, null, "Transfer In" },
                    { (byte)3, true, null, "QDRO Out" },
                    { (byte)4, true, null, "QDRO In" },
                    { (byte)5, true, null, "V-Only" },
                    { (byte)6, true, null, "Forfeit" },
                    { (byte)7, true, null, "Un-Forfeit" },
                    { (byte)8, true, null, "Class Action" },
                    { (byte)9, true, null, "Voided" },
                    { (byte)10, true, null, "Hardship" },
                    { (byte)11, true, null, "Distribution" },
                    { (byte)12, true, null, "Payoff" },
                    { (byte)13, false, null, "Dirpay" },
                    { (byte)14, true, null, "Rollover" },
                    { (byte)15, true, null, "Roth IRA" },
                    { (byte)16, false, null, "> 64 - 1 Year Vested" },
                    { (byte)17, false, null, "> 64 - 2 Year Vested" },
                    { (byte)18, false, null, "> 64 - 3 Year Vested" },
                    { (byte)19, true, null, "Military" },
                    { (byte)20, false, null, "Other" },
                    { (byte)21, true, null, "Rev" },
                    { (byte)22, true, null, "Unrev" },
                    { (byte)23, true, null, "100% Earnings" },
                    { (byte)24, true, null, ">64 & >5 100%" },
                    { (byte)25, true, null, "Forfeit Class Action" },
                    { (byte)26, true, null, "Forfeit Administrative" },
                    { (byte)27, false, null, "Administrative - taking money from under 21" },
                    { (byte)28, false, null, "Forfeiture adjustment for Class Action" }
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
                    { 7, "The Employee's Zero Contribution Flag is set at 7", "*** EMPLOYEE MAY BE 100% - CHECK DATES ***", "Information" },
                    { 8, "Employee is currently under 21 and has a current or vested balance greater than zero.", "Employee under 21 w/ balance > 0", "Information" }
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
                table: "NAVIGATION_ROLE",
                columns: new[] { "ID", "IS_READ_ONLY", "NAME" },
                values: new object[,]
                {
                    { (byte)11, true, "HR-ReadOnly" },
                    { (byte)12, true, "SSN-Unmasking" }
                });

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
                table: "RMDS_FACTOR_BY_AGE",
                columns: new[] { "AGE", "FACTOR" },
                values: new object[,]
                {
                    { (byte)73, 26.5m },
                    { (byte)74, 25.5m },
                    { (byte)75, 24.6m },
                    { (byte)76, 23.7m },
                    { (byte)77, 22.9m },
                    { (byte)78, 22.0m },
                    { (byte)79, 21.1m },
                    { (byte)80, 20.2m },
                    { (byte)81, 19.4m },
                    { (byte)82, 18.5m },
                    { (byte)83, 17.7m },
                    { (byte)84, 16.8m },
                    { (byte)85, 16.0m },
                    { (byte)86, 15.2m },
                    { (byte)87, 14.4m },
                    { (byte)88, 13.7m },
                    { (byte)89, 12.9m },
                    { (byte)90, 12.2m },
                    { (byte)91, 11.5m },
                    { (byte)92, 10.8m },
                    { (byte)93, 10.1m },
                    { (byte)94, 9.5m },
                    { (byte)95, 8.9m },
                    { (byte)96, 8.4m },
                    { (byte)97, 7.8m },
                    { (byte)98, 7.3m },
                    { (byte)99, 6.8m }
                });

            migrationBuilder.InsertData(
                table: "STATE",
                columns: new[] { "ABBREVIATION", "NAME" },
                values: new object[,]
                {
                    { "AK", "Alaska" },
                    { "AL", "Alabama" },
                    { "AR", "Arkansas" },
                    { "AS", "American Samoa" },
                    { "AZ", "Arizona" },
                    { "CA", "California" },
                    { "CO", "Colorado" },
                    { "CT", "Connecticut" },
                    { "DC", "District of Columbia" },
                    { "DE", "Delaware" },
                    { "FL", "Florida" },
                    { "GA", "Georgia" },
                    { "GU", "Guam" },
                    { "HI", "Hawaii" },
                    { "IA", "Iowa" },
                    { "ID", "Idaho" },
                    { "IL", "Illinois" },
                    { "IN", "Indiana" },
                    { "KS", "Kansas" },
                    { "KY", "Kentucky" },
                    { "LA", "Louisiana" },
                    { "MA", "Massachusetts" },
                    { "MD", "Maryland" },
                    { "ME", "Maine" },
                    { "MI", "Michigan" },
                    { "MN", "Minnesota" },
                    { "MO", "Missouri" },
                    { "MP", "Northern Mariana Islands" },
                    { "MS", "Mississippi" },
                    { "MT", "Montana" },
                    { "NC", "North Carolina" },
                    { "ND", "North Dakota" },
                    { "NE", "Nebraska" },
                    { "NH", "New Hampshire" },
                    { "NJ", "New Jersey" },
                    { "NM", "New Mexico" },
                    { "NV", "Nevada" },
                    { "NY", "New York" },
                    { "OH", "Ohio" },
                    { "OK", "Oklahoma" },
                    { "OR", "Oregon" },
                    { "PA", "Pennsylvania" },
                    { "PR", "Puerto Rico" },
                    { "RI", "Rhode Island" },
                    { "SC", "South Carolina" },
                    { "SD", "South Dakota" },
                    { "TN", "Tennessee" },
                    { "TX", "Texas" },
                    { "UM", "United States Minor Outlying Islands" },
                    { "UT", "Utah" },
                    { "VA", "Virginia" },
                    { "VI", "Virgin Islands" },
                    { "VT", "Vermont" },
                    { "WA", "Washington" },
                    { "WI", "Wisconsin" },
                    { "WV", "West Virginia" },
                    { "WY", "Wyoming" }
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
                table: "VESTING_SCHEDULE",
                columns: new[] { "ID", "CREATED_DATE", "DESCRIPTION", "EFFECTIVE_DATE", "IS_ACTIVE", "NAME" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 12, 0, 0, 0, 0, DateTimeKind.Utc), "7-year vesting schedule with vesting starting at year 3. Used for employees whose first contribution was before 2007.", "1917-01-01", true, "Old Plan (Pre-2007)" },
                    { 2, new DateTime(2025, 1, 12, 0, 0, 0, 0, DateTimeKind.Utc), "6-year vesting schedule with vesting starting at year 2. Used for employees whose first contribution was 2007 or later.", "2007-01-01", true, "New Plan (2007+)" }
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

            migrationBuilder.InsertData(
                table: "BANK_ACCOUNT",
                columns: new[] { "ID", "ACCOUNT_NAME", "ACCOUNT_NUMBER", "BANK_ID", "CREATED_AT_UTC", "CREATEDBY", "DISCONTINUED_DATE", "EFFECTIVE_DATE", "FED_ACH_CHANGE_DATE", "FEDWIRE_LOCATION", "FEDWIRE_REVISION_DATE", "FEDWIRE_TELEGRAPHIC_NAME", "IS_PRIMARY", "MODIFIED_AT_UTC", "MODIFIEDBY", "NOTES", "ROUTING_NUMBER", "SERVICING_FED_ADDRESS", "SERVICING_FED_ROUTING_NUMBER" },
                values: new object[] { 1, "Profit Sharing Distribution Account", "PLACEHOLDER", 1, new DateTimeOffset(new DateTime(2026, 1, 16, 0, 18, 48, 701, DateTimeKind.Unspecified).AddTicks(6239), new TimeSpan(0, 0, 0, 0, 0)), "SYSTEM", null, null, null, null, null, null, true, null, null, null, "026004297", null, null });

            migrationBuilder.InsertData(
                table: "VESTING_SCHEDULE_DETAIL",
                columns: new[] { "ID", "VESTING_PERCENT", "VESTING_SCHEDULE_ID", "YEARS_OF_SERVICE" },
                values: new object[,]
                {
                    { 1, 0m, 1, 0 },
                    { 2, 0m, 1, 1 },
                    { 3, 0m, 1, 2 },
                    { 4, 20m, 1, 3 },
                    { 5, 40m, 1, 4 },
                    { 6, 60m, 1, 5 },
                    { 7, 80m, 1, 6 },
                    { 8, 100m, 1, 7 },
                    { 9, 0m, 2, 0 },
                    { 10, 0m, 2, 1 },
                    { 11, 20m, 2, 2 },
                    { 12, 40m, 2, 3 },
                    { 13, 60m, 2, 4 },
                    { 14, 80m, 2, 5 },
                    { 15, 100m, 2, 6 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ANNUITY_RATE_YEAR_AGE",
                table: "ANNUITY_RATE",
                columns: new[] { "YEAR", "AGE" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_EVENT_SESSION_ID_CREATEDAT",
                table: "AUDIT_EVENT",
                columns: new[] { "SESSION_ID", "CREATED_AT" });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_EVENT_TABLENAME",
                table: "AUDIT_EVENT",
                column: "TABLE_NAME");

            migrationBuilder.CreateIndex(
                name: "IX_BANK_IS_DISABLED",
                table: "BANK",
                column: "IS_DISABLED");

            migrationBuilder.CreateIndex(
                name: "IX_BANK_NAME",
                table: "BANK",
                column: "NAME");

            migrationBuilder.CreateIndex(
                name: "IX_BANK_ROUTING_NUMBER",
                table: "BANK",
                column: "ROUTING_NUMBER");

            migrationBuilder.CreateIndex(
                name: "IX_BANK_ACCOUNT_BANK_ID",
                table: "BANK_ACCOUNT",
                column: "BANK_ID");

            migrationBuilder.CreateIndex(
                name: "IX_BANK_ACCOUNT_BANK_PRIMARY",
                table: "BANK_ACCOUNT",
                columns: new[] { "BANK_ID", "IS_PRIMARY" });

            migrationBuilder.CreateIndex(
                name: "IX_BANK_ACCOUNT_IS_DISABLED",
                table: "BANK_ACCOUNT",
                column: "IS_DISABLED");

            migrationBuilder.CreateIndex(
                name: "IX_BANK_ACCOUNT_ROUTING_NUMBER",
                table: "BANK_ACCOUNT",
                column: "ROUTING_NUMBER");

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
                name: "IX_CHECK_RUN_WORKFLOW_PROFIT_YEAR",
                table: "CHECK_RUN_WORKFLOW",
                column: "PROFITYEAR");

            migrationBuilder.CreateIndex(
                name: "IX_CHECK_RUN_WORKFLOW_RUN_DATE",
                table: "CHECK_RUN_WORKFLOW",
                column: "CHECKRUNDATE");

            migrationBuilder.CreateIndex(
                name: "IX_CHECK_RUN_WORKFLOW_YEAR_STATUS",
                table: "CHECK_RUN_WORKFLOW",
                columns: new[] { "PROFITYEAR", "STEPSTATUS" });

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
                name: "IX_DEMOGRAPHIC_HISTORY_VALIDFROM_VALIDTO_DEMOGRAPHICID",
                table: "DEMOGRAPHIC_HISTORY",
                columns: new[] { "VALID_FROM", "VALID_TO", "DEMOGRAPHIC_ID" });

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
                name: "IX_FILE_TRANSFER_AUDIT_FILENAME",
                table: "FILE_TRANSFER_AUDIT",
                column: "FILENAME");

            migrationBuilder.CreateIndex(
                name: "IX_FILE_TRANSFER_AUDIT_TIMESTAMP",
                table: "FILE_TRANSFER_AUDIT",
                column: "TIMESTAMP");

            migrationBuilder.CreateIndex(
                name: "IX_FILE_TRANSFER_AUDIT_WORKFLOW_ID",
                table: "FILE_TRANSFER_AUDIT",
                column: "CHECKRUNWORKFLOWID");

            migrationBuilder.CreateIndex(
                name: "IX_FTP_OPERATION_LOG_TIMESTAMP",
                table: "FTP_OPERATION_LOG",
                column: "TIMESTAMP");

            migrationBuilder.CreateIndex(
                name: "IX_FTP_OPERATION_LOG_WORKFLOW_ID",
                table: "FTP_OPERATION_LOG",
                column: "CHECKRUNWORKFLOWID");

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
                name: "IX_PAY_PROFIT_PROFITYEAR",
                table: "PAY_PROFIT",
                column: "PROFIT_YEAR");

            migrationBuilder.CreateIndex(
                name: "IX_PAY_PROFIT_PROFITYEAR_DEMOGRAPHICID",
                table: "PAY_PROFIT",
                columns: new[] { "PROFIT_YEAR", "DEMOGRAPHIC_ID" });

            migrationBuilder.CreateIndex(
                name: "IX_PAY_PROFIT_PROFITYEAR_TOTALHOURS",
                table: "PAY_PROFIT",
                columns: new[] { "PROFIT_YEAR", "TOTAL_HOURS" });

            migrationBuilder.CreateIndex(
                name: "IX_PAY_PROFIT_ZEROCONTRIBUTIONREASONID",
                table: "PAY_PROFIT",
                column: "ZERO_CONTRIBUTION_REASON_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_DETAIL_COMMENTTYPEID",
                table: "PROFIT_DETAIL",
                column: "COMMENT_TYPE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_DETAIL_PROFITCODEID_SSN",
                table: "PROFIT_DETAIL",
                columns: new[] { "PROFIT_CODE_ID", "SSN" });

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_DETAIL_PROFITYEAR_MONTHTODATE",
                table: "PROFIT_DETAIL",
                columns: new[] { "PROFIT_YEAR", "MONTH_TO_DATE" });

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_DETAIL_PROFITYEAR_PROFITCODEID",
                table: "PROFIT_DETAIL",
                columns: new[] { "PROFIT_YEAR", "PROFIT_CODE_ID" });

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_DETAIL_PROFITYEAR_SSN",
                table: "PROFIT_DETAIL",
                columns: new[] { "PROFIT_YEAR", "SSN" });

            migrationBuilder.CreateIndex(
                name: "IX_PROFIT_DETAIL_REVERSED_FROM_PROFIT_DETAIL_ID",
                table: "PROFIT_DETAIL",
                column: "REVERSED_FROM_PROFIT_DETAIL_ID",
                filter: "REVERSED_FROM_PROFIT_DETAIL_ID IS NOT NULL");

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
                name: "IX_PROFIT_SHARE_CHECK_CHECKRUNWORKFLOWID",
                table: "PROFIT_SHARE_CHECK",
                column: "CHECK_RUN_WORKFLOW_ID");

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
                name: "UK_VESTING_SCHEDULE_NAME",
                table: "VESTING_SCHEDULE",
                column: "NAME",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VESTING_SCHEDULE_DETAIL_VESTING_SCHEDULE_ID",
                table: "VESTING_SCHEDULE_DETAIL",
                column: "VESTING_SCHEDULE_ID");

            migrationBuilder.CreateIndex(
                name: "UK_VSD_SCHEDULE_YEARS",
                table: "VESTING_SCHEDULE_DETAIL",
                columns: new[] { "VESTING_SCHEDULE_ID", "YEARS_OF_SERVICE" },
                unique: true);

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
                name: "ANNUITY_RATE_CONFIG");

            migrationBuilder.DropTable(
                name: "AUDIT_EVENT");

            migrationBuilder.DropTable(
                name: "BANK_ACCOUNT");

            migrationBuilder.DropTable(
                name: "BENEFICIARY");

            migrationBuilder.DropTable(
                name: "BENEFICIARY_ARCHIVE");

            migrationBuilder.DropTable(
                name: "BENEFICIARY_CONTACT_ARCHIVE");

            migrationBuilder.DropTable(
                name: "BENEFICIARY_SSN_CHANGE_HISTORY");

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
                name: "FILE_TRANSFER_AUDIT");

            migrationBuilder.DropTable(
                name: "FROZEN_STATE");

            migrationBuilder.DropTable(
                name: "FTP_OPERATION_LOG");

            migrationBuilder.DropTable(
                name: "JOB");

            migrationBuilder.DropTable(
                name: "MISSIVES");

            migrationBuilder.DropTable(
                name: "NAVIGATION_ASSIGNED_ROLES");

            migrationBuilder.DropTable(
                name: "NAVIGATION_CUSTOM_SETTING");

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
                name: "RMDS_FACTOR_BY_AGE");

            migrationBuilder.DropTable(
                name: "STATE");

            migrationBuilder.DropTable(
                name: "STATE_TAX");

            migrationBuilder.DropTable(
                name: "VESTING_SCHEDULE_DETAIL");

            migrationBuilder.DropTable(
                name: "YE_UPDATE_STATUS");

            migrationBuilder.DropTable(
                name: "BANK");

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
                name: "COMMENT_TYPE");

            migrationBuilder.DropTable(
                name: "PROFIT_CODE");

            migrationBuilder.DropTable(
                name: "ZERO_CONTRIBUTION_REASON");

            migrationBuilder.DropTable(
                name: "CHECK_RUN_WORKFLOW");

            migrationBuilder.DropTable(
                name: "DEMOGRAPHIC");

            migrationBuilder.DropTable(
                name: "TAX_CODE");

            migrationBuilder.DropTable(
                name: "VESTING_SCHEDULE");

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
                name: "BANK_ACCOUNT_SEQ");

            migrationBuilder.DropSequence(
                name: "BANK_SEQ");

            migrationBuilder.DropSequence(
                name: "FAKE_SSN_SEQ");

            migrationBuilder.DropSequence(
                name: "PROFIT_SHARE_CHECK_NUMBER_SEQ");
        }
    }
}
