---THIS SCRIPT WILL LOAD ALL SMART PROFIT SHARING TABLES
---TO "YOUR CURRENT SCHEMA" FROM - {SOURCE_PROFITSHARE_SCHEMA}
-------------------------------------------------------------------------------------
DECLARE
    this_year NUMBER := 2025; -- Set this to the current year
    last_year NUMBER := 2024; -- Set this to the previous year
BEGIN

 -- First disable all foreign key constraints
    FOR fk IN (SELECT constraint_name, table_name 
               FROM user_constraints 
               WHERE constraint_type = 'R')
    LOOP
        EXECUTE IMMEDIATE 'ALTER TABLE ' || fk.table_name || 
                         ' DISABLE CONSTRAINT ' || fk.constraint_name;
    END LOOP;

    -- FIRST EMPTY OUT THE TABLES
    EXECUTE IMMEDIATE 'TRUNCATE TABLE JOB';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE DEMOGRAPHIC_SYNC_AUDIT';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE BENEFICIARY_SSN_CHANGE_HISTORY';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE PROFIT_DETAIL';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE BENEFICIARY';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE BENEFICIARY_CONTACT';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE PAY_PROFIT';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE PROFIT_SHARE_CHECK';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE DISTRIBUTION';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE DISTRIBUTION_THIRDPARTY_PAYEE';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE DISTRIBUTION_PAYEE';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE DISTRIBUTION_REQUEST';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE FROZEN_STATE';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE FAKE_SSNS';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE DEMOGRAPHIC_HISTORY';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE DEMOGRAPHIC';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE DEMOGRAPHIC_SSN_CHANGE_HISTORY';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE AUDIT_CHANGE__AUDIT_EVENT';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE AUDIT_CHANGE';
    EXECUTE IMMEDIATE 'TRUNCATE TABLE AUDIT_EVENT';
    
    -- Reset sequence
    EXECUTE IMMEDIATE 'ALTER SEQUENCE FAKE_SSN_SEQ RESTART START WITH 666000000';

     -- Re-enable foreign key constraints at the end
    FOR fk IN (SELECT constraint_name, table_name 
               FROM user_constraints 
               WHERE constraint_type = 'R')
    LOOP
        EXECUTE IMMEDIATE 'ALTER TABLE ' || fk.table_name || 
                         ' ENABLE CONSTRAINT ' || fk.constraint_name;
    END LOOP;

--LOAD DEMOGRAPHIC TABLE TO - "YOUR CURRENT SCHEMA" FROM - {SOURCE_PROFITSHARE_SCHEMA}


    INSERT INTO DEMOGRAPHIC
    (ORACLE_HCM_ID,
     SSN,
     BADGE_NUMBER,
     FULL_NAME,
     LAST_NAME,
     FIRST_NAME,
     MIDDLE_NAME,
     STORE_NUMBER,
     PAY_CLASSIFICATION_ID,
     PHONE_NUMBER,
     STREET,
     STREET2,
     CITY,
     STATE,
     POSTAL_CODE,
     DATE_OF_BIRTH,
     FULL_TIME_DATE,
     HIRE_DATE,
     REHIRE_DATE,
     TERMINATION_DATE,
     DEPARTMENT,
     EMPLOYMENT_TYPE_ID,
     GENDER_ID,
     PAY_FREQUENCY_ID,
     TERMINATION_CODE_ID,
     EMPLOYMENT_STATUS_ID)
    SELECT
                ROWNUM AS ORACLEHCMID,
                DEM_SSN,
                DEM_BADGE,
                PY_NAM,
                PY_LNAME,
                PY_FNAME,
                PY_MNAME,
                PY_STOR,
                PY_CLA,
                PY_EMP_TELNO,
                PY_ADD,
                PY_ADD2,
                PY_CITY,
                PY_STATE,
                PY_ZIP,
                CASE
                    WHEN LENGTH(PY_DOB) = 8
                        AND TO_NUMBER(SUBSTR(PY_DOB, 5, 2)) BETWEEN 1 AND 12
                        AND (
                             (TO_NUMBER(SUBSTR(PY_DOB, 5, 2)) IN (1, 3, 5, 7, 8, 10, 12) AND TO_NUMBER(SUBSTR(PY_DOB, 7, 2)) BETWEEN 1 AND 31) OR
                             (TO_NUMBER(SUBSTR(PY_DOB, 5, 2)) IN (4, 6, 9, 11) AND TO_NUMBER(SUBSTR(PY_DOB, 7, 2)) BETWEEN 1 AND 30) OR
                             (TO_NUMBER(SUBSTR(PY_DOB, 5, 2)) = 2 AND TO_NUMBER(SUBSTR(PY_DOB, 7, 2)) BETWEEN 1 AND
                                 CASE
                                     WHEN MOD(TO_NUMBER(SUBSTR(PY_DOB, 1, 4)), 400) = 0 OR (MOD(TO_NUMBER(SUBSTR(PY_DOB, 1, 4)), 4) = 0 AND MOD(TO_NUMBER(SUBSTR(PY_DOB, 1, 4)), 100) != 0)
                                         THEN 29
                                     ELSE 28
                                     END
                                 )
                             )
                        THEN TO_DATE(PY_DOB, 'YYYYMMDD')
                    ELSE DATE '1900-01-01'
                    END AS converted_date,
                CASE
                    WHEN PY_FULL_DT = 0 THEN NULL
                    WHEN LENGTH(PY_FULL_DT) = 8
                        AND TO_NUMBER(SUBSTR(PY_FULL_DT, 5, 2)) BETWEEN 1 AND 12
                        AND (
                             (TO_NUMBER(SUBSTR(PY_FULL_DT, 5, 2)) IN (1, 3, 5, 7, 8, 10, 12) AND TO_NUMBER(SUBSTR(PY_FULL_DT, 7, 2)) BETWEEN 1 AND 31) OR
                             (TO_NUMBER(SUBSTR(PY_FULL_DT, 5, 2)) IN (4, 6, 9, 11) AND TO_NUMBER(SUBSTR(PY_FULL_DT, 7, 2)) BETWEEN 1 AND 30) OR
                             (TO_NUMBER(SUBSTR(PY_FULL_DT, 5, 2)) = 2 AND TO_NUMBER(SUBSTR(PY_FULL_DT, 7, 2)) BETWEEN 1 AND
                                 CASE
                                     WHEN MOD(TO_NUMBER(SUBSTR(PY_FULL_DT, 1, 4)), 400) = 0 OR (MOD(TO_NUMBER(SUBSTR(PY_FULL_DT, 1, 4)), 4) = 0 AND MOD(TO_NUMBER(SUBSTR(PY_FULL_DT, 1, 4)), 100) != 0)
                                         THEN 29
                                     ELSE 28
                                     END
                                 )
                             )
                        THEN TO_DATE(PY_FULL_DT, 'YYYYMMDD')
                    ELSE DATE '1900-01-01'
                    END AS converted_date,
                CASE
                    WHEN LENGTH(PY_HIRE_DT) = 8
                        AND TO_NUMBER(SUBSTR(PY_HIRE_DT, 5, 2)) BETWEEN 1 AND 12
                        AND (
                             (TO_NUMBER(SUBSTR(PY_HIRE_DT, 5, 2)) IN (1, 3, 5, 7, 8, 10, 12) AND TO_NUMBER(SUBSTR(PY_HIRE_DT, 7, 2)) BETWEEN 1 AND 31) OR
                             (TO_NUMBER(SUBSTR(PY_HIRE_DT, 5, 2)) IN (4, 6, 9, 11) AND TO_NUMBER(SUBSTR(PY_HIRE_DT, 7, 2)) BETWEEN 1 AND 30) OR
                             (TO_NUMBER(SUBSTR(PY_HIRE_DT, 5, 2)) = 2 AND TO_NUMBER(SUBSTR(PY_HIRE_DT, 7, 2)) BETWEEN 1 AND
                                 CASE
                                     WHEN MOD(TO_NUMBER(SUBSTR(PY_HIRE_DT, 1, 4)), 400) = 0 OR (MOD(TO_NUMBER(SUBSTR(PY_HIRE_DT, 1, 4)), 4) = 0 AND MOD(TO_NUMBER(SUBSTR(PY_HIRE_DT, 1, 4)), 100) != 0)
                                         THEN 29
                                     ELSE 28
                                     END
                                 )
                             )
                        THEN TO_DATE(PY_HIRE_DT, 'YYYYMMDD')
                    ELSE DATE '1900-01-01'
                    END AS converted_date,
                CASE
                    WHEN PY_REHIRE_DT = 0 THEN NULL
                    WHEN LENGTH(PY_REHIRE_DT) = 8
                        AND TO_NUMBER(SUBSTR(PY_REHIRE_DT, 5, 2)) BETWEEN 1 AND 12
                        AND (
                             (TO_NUMBER(SUBSTR(PY_REHIRE_DT, 5, 2)) IN (1, 3, 5, 7, 8, 10, 12) AND TO_NUMBER(SUBSTR(PY_REHIRE_DT, 7, 2)) BETWEEN 1 AND 31) OR
                             (TO_NUMBER(SUBSTR(PY_REHIRE_DT, 5, 2)) IN (4, 6, 9, 11) AND TO_NUMBER(SUBSTR(PY_REHIRE_DT, 7, 2)) BETWEEN 1 AND 30) OR
                             (TO_NUMBER(SUBSTR(PY_REHIRE_DT, 5, 2)) = 2 AND TO_NUMBER(SUBSTR(PY_REHIRE_DT, 7, 2)) BETWEEN 1 AND
                                 CASE
                                     WHEN MOD(TO_NUMBER(SUBSTR(PY_REHIRE_DT, 1, 4)), 400) = 0 OR (MOD(TO_NUMBER(SUBSTR(PY_REHIRE_DT, 1, 4)), 4) = 0 AND MOD(TO_NUMBER(SUBSTR(PY_REHIRE_DT, 1, 4)), 100) != 0)
                                         THEN 29
                                     ELSE 28
                                     END
                                 )
                             )
                        THEN TO_DATE(PY_REHIRE_DT, 'YYYYMMDD')
                    ELSE DATE '1900-01-01'
                    END AS converted_date,
                CASE
                    WHEN PY_TERM_DT = 0 THEN NULL
                    WHEN LENGTH(PY_TERM_DT) = 8
                        AND TO_NUMBER(SUBSTR(PY_TERM_DT, 5, 2)) BETWEEN 1 AND 12
                        AND (
                             (TO_NUMBER(SUBSTR(PY_TERM_DT, 5, 2)) IN (1, 3, 5, 7, 8, 10, 12) AND TO_NUMBER(SUBSTR(PY_TERM_DT, 7, 2)) BETWEEN 1 AND 31) OR
                             (TO_NUMBER(SUBSTR(PY_TERM_DT, 5, 2)) IN (4, 6, 9, 11) AND TO_NUMBER(SUBSTR(PY_TERM_DT, 7, 2)) BETWEEN 1 AND 30) OR
                             (TO_NUMBER(SUBSTR(PY_TERM_DT, 5, 2)) = 2 AND TO_NUMBER(SUBSTR(PY_TERM_DT, 7, 2)) BETWEEN 1 AND
                                 CASE
                                     WHEN MOD(TO_NUMBER(SUBSTR(PY_TERM_DT, 1, 4)), 400) = 0 OR (MOD(TO_NUMBER(SUBSTR(PY_TERM_DT, 1, 4)), 4) = 0 AND MOD(TO_NUMBER(SUBSTR(PY_TERM_DT, 1, 4)), 100) != 0)
                                         THEN 29
                                     ELSE 28
                                     END
                                 )
                             )
                        THEN TO_DATE(PY_TERM_DT, 'YYYYMMDD')
                    ELSE DATE '1900-01-01'
                    END AS converted_date,
                PY_DP,
                TRIM(PY_FUL),
                CASE
                    WHEN TRIM(PY_GENDER) = 'O' THEN 'X' -- Nonbinary
                    WHEN TRIM(PY_GENDER) IS NULL then 'U' -- 'U'nknown
                    WHEN TRIM(PY_GENDER) = '' then 'U' -- 'U'nknown
                    ELSE TRIM(PY_GENDER)
                    END AS PY_GENDER,
                PY_FREQ,
                CASE
                    WHEN PY_TERM = ' ' THEN NULL
                    ELSE PY_TERM
                    END as termcd,
                (
                    SELECT ID
                    FROM EMPLOYMENT_STATUS
                    WHERE ID=LOWER(PY_SCOD)
                ) AS EMPLOYMENT_STATUS_ID
    FROM {SOURCE_PROFITSHARE_SCHEMA}.DEMOGRAPHICS;

    INSERT INTO DEMOGRAPHIC_HISTORY(DEMOGRAPHIC_ID, VALID_FROM, VALID_TO, ORACLE_HCM_ID, BADGE_NUMBER, STORE_NUMBER, PAY_CLASSIFICATION_ID, DATE_OF_BIRTH, HIRE_DATE, REHIRE_DATE, TERMINATION_DATE, DEPARTMENT, EMPLOYMENT_TYPE_ID, PAY_FREQUENCY_ID, TERMINATION_CODE_ID, EMPLOYMENT_STATUS_ID, CREATED_DATETIME)
    SELECT ID, TO_TIMESTAMP('01-01-1900 00:00:00','MM-DD-YYYY HH24:MI:SS'), TO_TIMESTAMP('01-01-2100 00:00:00','MM-DD-YYYY HH24:MI:SS'), ORACLE_HCM_ID, BADGE_NUMBER, STORE_NUMBER, PAY_CLASSIFICATION_ID, DATE_OF_BIRTH, HIRE_DATE, REHIRE_DATE, TERMINATION_DATE, DEPARTMENT, EMPLOYMENT_TYPE_ID, PAY_FREQUENCY_ID, TERMINATION_CODE_ID, EMPLOYMENT_STATUS_ID, sys_extract_utc(systimestamp)
    FROM DEMOGRAPHIC;

    --------------------------------------------------------------------------------------


-- Insert contact information into the BENEFICIARY_CONTACT table
    INSERT INTO BENEFICIARY_CONTACT
    (SSN,
     FIRST_NAME,
     LAST_NAME,
     FULL_NAME,
     DATE_OF_BIRTH,
     STREET,
     CITY,
     STATE,
     POSTAL_CODE,
     PHONE_NUMBER,
     MOBILE_NUMBER,
     EMAIL_ADDRESS)
    SELECT
        PYBEN.PYBEN_PAYSSN AS SSN,
        TRIM(SUBSTR(PYBEN.PYBEN_NAME, INSTR(PYBEN.PYBEN_NAME, ', ') + 2)) AS FIRSTNAME,
        TRIM(SUBSTR(PYBEN.PYBEN_NAME, 1, INSTR(PYBEN.PYBEN_NAME, ',') - 1)) AS LASTNAME,
        TRIM(PYBEN.PYBEN_NAME) AS FULL_NAME,
        CASE
            WHEN LENGTH(PYBEN.PYBEN_DOBIRTH) = 8
                AND TO_NUMBER(SUBSTR(PYBEN.PYBEN_DOBIRTH, 5, 2)) BETWEEN 1 AND 12
                AND (
                     (TO_NUMBER(SUBSTR(PYBEN.PYBEN_DOBIRTH, 5, 2)) IN (1, 3, 5, 7, 8, 10, 12) AND TO_NUMBER(SUBSTR(PYBEN.PYBEN_DOBIRTH, 7, 2)) BETWEEN 1 AND 31) OR
                     (TO_NUMBER(SUBSTR(PYBEN.PYBEN_DOBIRTH, 5, 2)) IN (4, 6, 9, 11) AND TO_NUMBER(SUBSTR(PYBEN.PYBEN_DOBIRTH, 7, 2)) BETWEEN 1 AND 30) OR
                     (TO_NUMBER(SUBSTR(PYBEN.PYBEN_DOBIRTH, 5, 2)) = 2 AND TO_NUMBER(SUBSTR(PYBEN.PYBEN_DOBIRTH, 7, 2)) BETWEEN 1 AND
                         CASE
                             WHEN MOD(TO_NUMBER(SUBSTR(PYBEN.PYBEN_DOBIRTH, 1, 4)), 400) = 0 OR (MOD(TO_NUMBER(SUBSTR(PYBEN.PYBEN_DOBIRTH, 1, 4)), 4) = 0 AND MOD(TO_NUMBER(SUBSTR(PYBEN.PYBEN_DOBIRTH, 1, 4)), 100) != 0)
                                 THEN 29
                             ELSE 28
                             END
                         )
                     )
                THEN TO_DATE(PYBEN.PYBEN_DOBIRTH, 'YYYYMMDD')
            ELSE DATE '1900-01-01'
            END AS DATE_OF_BIRTH,
        PYBEN.PYBEN_ADD AS STREET,
        PYBEN.PYBEN_CITY AS CITY,
        PYBEN.PYBEN_STATE AS STATE,
        PYBEN.PYBEN_ZIP AS POSTAL_CODE,
        NULL AS PHONE_NUMBER, -- phone number isn't available
        NULL AS MOBILE_NUMBER,  -- mobile number isn't available
        NULL AS EMAIL_ADDRESS  -- email isn't available
    FROM
        {SOURCE_PROFITSHARE_SCHEMA}.PAYBEN PYBEN;

    INSERT INTO BENEFICIARY
    (PSN_SUFFIX,
     BADGE_NUMBER,
     DEMOGRAPHIC_ID,
     BENEFICIARY_CONTACT_ID,
     RELATIONSHIP,
     KIND_ID,
     PERCENT)
    SELECT
        TO_NUMBER(SUBSTR(LPAD(PYBEN.PYBEN_PSN, 11, 0), 8)) AS PSN_SUFFIX,
        TO_NUMBER(SUBSTR(LPAD(PYBEN_PSN,11,0),1,7)) AS BADGE_NUMBER,
        CASE
            WHEN d.ID IS NULL THEN d2.ID
            ELSE d.ID
            END                                  AS DEMOGRAPHIC_ID,
        BC.ID AS BENEFICIARY_CONTACT_ID,  -- Link to BENEFICIARY_CONTACT
        PAYREL.PYREL_RELATION AS RELATIONSHIP,
        CASE
            WHEN PAYREL.PYREL_TYPE = ' ' THEN NULL
            ELSE PAYREL.PYREL_TYPE
            END AS KIND_ID,

        CASE WHEN PAYREL.PYREL_PERCENT IS NULL THEN 0
             ELSE PAYREL.PYREL_PERCENT
            END AS PERCENT
    FROM {SOURCE_PROFITSHARE_SCHEMA}.PAYBEN PYBEN
             INNER JOIN BENEFICIARY_CONTACT BC ON PYBEN.PYBEN_PAYSSN = BC.SSN
             LEFT join DEMOGRAPHIC d on PYBEN.PYBEN_PAYSSN = d.SSN
             LEFT join DEMOGRAPHIC d2 on TO_NUMBER(SUBSTR(LPAD(PYBEN_PSN,11,0),1,7)) = d2.BADGE_NUMBER
             LEFT JOIN {SOURCE_PROFITSHARE_SCHEMA}.PAYREL ON PYBEN.PYBEN_PSN = PAYREL.PYREL_PSN;

    --During import, the goal is to identify the employee to whom a beneficiary is most closely related.
--Is the beneficiary an employee?
--If yes, the DEMOGRAPHIC_ID is assigned based on the employee's SSN, directly linking to their demographic record.
--If the beneficiary is not an employee
--The DEMOGRAPHIC_ID is assigned to the employee who designated them as a beneficiary.
--This is determined using the badge number extracted from PYBEN_PSN to find the corresponding demographic record.

--------------------------------------------------------------------------------------------------

    -------------------------------------------------------------------------------
    -- Insert THIS YEARS data into the PAY_PROFIT table
    INSERT INTO PAY_PROFIT
    (DEMOGRAPHIC_ID,
     PROFIT_YEAR,
     CURRENT_HOURS_YEAR,
     CURRENT_INCOME_YEAR,
     WEEKS_WORKED_YEAR,
     PS_CERTIFICATE_ISSUED_DATE,
     ENROLLMENT_ID,
     BENEFICIARY_TYPE_ID,
     EMPLOYEE_TYPE_ID,
     ZERO_CONTRIBUTION_REASON_ID,
     HOURS_EXECUTIVE,
     INCOME_EXECUTIVE,
     POINTS_EARNED,
     ETVA)
    SELECT
        (select ID from DEMOGRAPHIC where BADGE_NUMBER = PAYPROF_BADGE) AS DEMOGRAPHIC_ID,
        this_year AS PROFIT_YEAR,
        PY_PH AS CURRENT_HOURS_YEAR,
        PY_PD AS CURRENT_INCOME_YEAR,
        PY_WEEKS_WORK AS WEEKS_WORKED_YEAR,
        null as PS_CERTIFICATE_ISSUED_DATE, -- calculated and set during YE process (is this internal, ie not on any screens?)
        PY_PS_ENROLLED AS ENROLLMENT_ID,
        PY_PROF_BENEFICIARY AS BENEFICIARY_ID,
        PY_PROF_NEWEMP AS EMPLOYEE_TYPE_ID,
        null AS ZERO_CONTRIBUTION_REASON_ID, -- calculated and set during YE process (is this internal, ie not on any screens?)
        0 AS HOURS_EXECUTIVE,
        0 AS INCOME_EXECUTIVE,
        0 as POINTS_EARNED, -- calculated and set during YE process  (is this internal, ie not on any screens?)
        PY_PS_ETVA as ETVA
    FROM {SOURCE_PROFITSHARE_SCHEMA}.PAYPROFIT
    where PAYPROF_BADGE in ( select BADGE_NUMBER from DEMOGRAPHIC  );

    -- Insert LAST YEARS data into the PAY_PROFIT table
    INSERT INTO PAY_PROFIT
    (DEMOGRAPHIC_ID,
     PROFIT_YEAR,
     CURRENT_HOURS_YEAR,
     CURRENT_INCOME_YEAR,
     WEEKS_WORKED_YEAR,
     PS_CERTIFICATE_ISSUED_DATE,
     ENROLLMENT_ID,
     BENEFICIARY_TYPE_ID,
     EMPLOYEE_TYPE_ID,
     ZERO_CONTRIBUTION_REASON_ID,
     HOURS_EXECUTIVE,
     INCOME_EXECUTIVE,
     POINTS_EARNED,
     ETVA)
    SELECT
        (SELECT ID FROM DEMOGRAPHIC WHERE BADGE_NUMBER = PAYPROF_BADGE) AS DEMOGRAPHIC_ID,
        last_year AS PROFIT_YEAR,
        PY_PH_LASTYR AS CURRENT_HOURS_YEAR,
        PY_PD_LASTYR AS CURRENT_INCOME_YEAR,
        PY_WEEKS_WORK_LAST AS WEEKS_WORKED_YEAR,
        NULL AS PS_CERTIFICATE_ISSUED_DATE,
        PY_PS_ENROLLED,
        PY_PROF_BENEFICIARY AS BENEFICIARY_ID,
         0 as EMPLOYEE_TYPE_ID, -- calculated and set during YE process (ie NEW, or Not New employee) 
         -- See PAY456.cbl Lines 152-165
        case when PY_PROF_ZEROCONT < 6 then 0 else PY_PROF_ZEROCONT end as ZERO_CONTRIBUTION_REASON_ID, -- We keep last years value, if it gets to be >= 6 
        NVL(PY_PH_EXEC, 0) AS HOURS_EXECUTIVE,
        NVL(PY_PD_EXEC, 0) AS INCOME_EXECUTIVE,
        0 AS POINTS_EARNED, -- calculated and set during YE process
        PY_PRIOR_ETVA as ETVA 
    FROM
        {SOURCE_PROFITSHARE_SCHEMA}.PAYPROFIT pp
            LEFT JOIN {SOURCE_PROFITSHARE_SCHEMA}.DEMOGRAPHICS d on d.DEM_BADGE = pp.PAYPROF_BADGE
    WHERE
        PAYPROF_BADGE IN (SELECT BADGE_NUMBER FROM DEMOGRAPHIC);

    -- Insert the year before LAST YEARS data into the PAY_PROFIT table
    INSERT INTO PAY_PROFIT
    (DEMOGRAPHIC_ID,
     PROFIT_YEAR,
     CURRENT_HOURS_YEAR,
     CURRENT_INCOME_YEAR,
     WEEKS_WORKED_YEAR,
     PS_CERTIFICATE_ISSUED_DATE,
     ENROLLMENT_ID,
     BENEFICIARY_TYPE_ID,
     EMPLOYEE_TYPE_ID,
     ZERO_CONTRIBUTION_REASON_ID,
     HOURS_EXECUTIVE,
     INCOME_EXECUTIVE,
     POINTS_EARNED,
     ETVA)
    SELECT
        (SELECT ID FROM DEMOGRAPHIC WHERE BADGE_NUMBER = PAYPROF_BADGE) AS DEMOGRAPHIC_ID,
        last_year - 1 AS PROFIT_YEAR,
        0 AS CURRENT_HOURS_YEAR,
        0 AS CURRENT_INCOME_YEAR,
        0 AS WEEKS_WORKED_YEAR,
        NULL AS PS_CERTIFICATE_ISSUED_DATE,
        PY_PS_ENROLLED,
        0 AS BENEFICIARY_ID,
        0 AS EMPLOYEE_TYPE_ID,
        8 AS ZERO_CONTRIBUTION_REASON_ID, -- 8/History not previously tracked (Unknown)
        0 AS HOURS_EXECUTIVE,
        0 AS INCOME_EXECUTIVE,
        0 AS POINTS_EARNED,
        0 as ETVA
    FROM
        {SOURCE_PROFITSHARE_SCHEMA}.PAYPROFIT pp
            LEFT JOIN {SOURCE_PROFITSHARE_SCHEMA}.DEMOGRAPHICS d on d.DEM_BADGE = pp.PAYPROF_BADGE
    WHERE
        PAYPROF_BADGE IN (SELECT BADGE_NUMBER FROM DEMOGRAPHIC);

    ---------------------------------------------------------------

-- Migrate data into DISTRIBUTION_PAYEE table from {SOURCE_PROFITSHARE_SCHEMA}_PROFDIST
-- Ensure that there are no duplicate entries based on unique PAYEE (SSN, NAME, and ADDRESS)
    INSERT INTO DISTRIBUTION_PAYEE (SSN, NAME, STREET, CITY, STATE, POSTAL_CODE, COUNTRY_ISO, MEMO)
    SELECT DISTINCT
        PROFDIST_PAYSSN,
        PROFDIST_PAYNAME,
        PROFDIST_PAYADDR1,
        PROFDIST_PAYCITY,
        PROFDIST_PAYSTATE,
        TO_NCHAR(PROFDIST_PAYZIP1) || PROFDIST_PAYZIP2,
        N'US', -- Default country
        NULL -- No memo available in original data
    FROM {SOURCE_PROFITSHARE_SCHEMA}.PROFDIST
    WHERE PROFDIST_PAYSSN IS NOT NULL;


    -- Migrate data into DISTRIBUTION_THIRDPARTY_PAYEE table from {SOURCE_PROFITSHARE_SCHEMA}_PROFDIST
-- Ensure that there are no duplicate entries based on unique third-party payee (SSN, NAME, and ADDRESS)
    INSERT INTO DISTRIBUTION_THIRDPARTY_PAYEE (PAYEE, NAME, ACCOUNT, STREET, STREET2, CITY, STATE, POSTAL_CODE, COUNTRY_ISO, MEMO)
    SELECT DISTINCT
        PROFDIST_3RDPAYTO,
        PROFDIST_3RDNAME,
        PROFDIST_3RDACCT,
        PROFDIST_3RDADDR1,
        PROFDIST_3RDADDR2,
        PROFDIST_3RDCITY,
        PROFDIST_3RDSTATE,
        TO_NCHAR(PROFDIST_3RDZIP1) || PROFDIST_3RDZIP2,
        N'US', -- Default country
        NULL -- No memo available in original data
    FROM {SOURCE_PROFITSHARE_SCHEMA}.PROFDIST
    WHERE (PROFDIST_3RDPAYTO IS NOT NULL AND TRIM(PROFDIST_3RDPAYTO) != '');


--LOAD DISTRIBUTION TABLE TO - "YOUR CURRENT SCHEMA" FROM - {SOURCE_PROFITSHARE_SCHEMA}
    INSERT INTO DISTRIBUTION
    (SSN,
     PAYMENT_SEQUENCE,
     EMPLOYEE_NAME,
     FREQUENCY_ID,
     STATUS_ID,
     PAYEE_ID,
     THIRD_PARTY_PAYEE_ID,
     FORTHEBENEFITOF_PAYEE,
     FORTHEBENEFITOF_ACCOUNT_TYPE,
     TAX1099_FOR_EMPLOYEE,
     TAX1099_FOR_BENEFICIARY,
     FEDERAL_TAX_PERCENTAGE,
     STATE_TAX_PERCENTAGE,
     GROSS_AMOUNT,
     FEDERAL_TAX_AMOUNT,
     STATE_TAX_AMOUNT,
     CHECK_AMOUNT,
     TAX_CODE_ID,
     DECEASED,
     GENDER_ID,
     QDRO,
     MEMO,
     ROTH_IRA)
    SELECT
        PROFDIST_SSN,
        PROFDIST_PAYSEQ,
        PROFDIST_EMPNAME,
        PROFDIST_PAYFREQ,
        PROFDIST_PAYFLAG,

        -- Map to DISTRIBUTION_PAYEE table (using a subquery to fetch the correct PAYEE_ID)
        (SELECT ID FROM DISTRIBUTION_PAYEE
         WHERE SSN = PROFDIST_PAYSSN
           AND NAME = PROFDIST_PAYNAME
           AND STREET = PROFDIST_PAYADDR1),

        -- Map to DISTRIBUTION_THIRDPARTY_PAYEE table (using a subquery to fetch the correct THIRD_PARTY_PAYEE_ID)
        (SELECT ID FROM DISTRIBUTION_THIRDPARTY_PAYEE
         WHERE ACCOUNT = PROFDIST_3RDACCT
           AND NAME = PROFDIST_3RDNAME
           AND STREET = PROFDIST_3RDADDR1),

        PROFDIST_FBOPAYTO,
        PROFDIST_FBOTYPE,
        CASE
            WHEN PROFDIST_1099_INFO_EMP = 'Y' THEN 1
            ELSE 0
            END as acode,
        CASE
            WHEN PROFDIST_OTHER_BENEFICIARY = 'Y' THEN 1
            ELSE 0
            END as bcode,
        PROFDIST_FEDPCT,
        PROFDIST_STATEPCT,
        PROFDIST_GROSSAMT,
        PROFDIST_FEDTAX,
        PROFDIST_STATETAX,
        PROFDIST_CHECKAMT,
        PROFDIST_TAXCODE,
        CASE
            WHEN PROFDIST_DECEASED = 'Y' THEN 1
            ELSE 0
            END as ccode,
        CASE
            WHEN PROFDIST_SEX = 'M' THEN 'M'
            WHEN PROFDIST_SEX ='F' THEN 'F'
            ELSE NULL
            END as psex,
        CASE
            WHEN PROFDIST_QDRO = 'Y' THEN 1
            ELSE 0
            END as dcode,
        PROFDIST_MEMO,
        CASE
            WHEN PROFDIST_ROTH_IRA = 'Y' THEN 1
            ELSE 0
            END as ecode
    FROM {SOURCE_PROFITSHARE_SCHEMA}.PROFDIST;

--PROFIT DETAIL table
    INSERT INTO PROFIT_DETAIL
    (PROFIT_YEAR,
     PROFIT_YEAR_ITERATION,
     PROFIT_CODE_ID,
     CONTRIBUTION,
     EARNINGS,
     FORFEITURE,
     ZERO_CONTRIBUTION_REASON_ID,
     FEDERAL_TAXES,
     STATE_TAXES,
     TAX_CODE_ID,
     SSN,
     DISTRIBUTION_SEQUENCE,
     MONTH_TO_DATE,
     REMARK,
     YEAR_TO_DATE)
    SELECT
        TRUNC(PROFIT_YEAR),
        MOD(PROFIT_YEAR * 10, 10),
        PROFIT_CODE,
        PROFIT_CONT,
        PROFIT_EARN,
        PROFIT_FORT,
        CASE
            WHEN PROFIT_ZEROCONT = ' ' THEN NULL
            ELSE PROFIT_ZEROCONT
            END as Zcode,
        NVL(PROFIT_FED_TAXES,0),
        NVL(PROFIT_STATE_TAXES,0),
        CASE
            WHEN PROFIT_TAX_CODE = ' ' THEN NULL
            ELSE PROFIT_TAX_CODE
            END as ptcode,
        PR_DET_S_SEC_NUMBER,
        PROFIT_DET_PR_DET_S_SEQNUM,
        PROFIT_MDTE,
        CASE
            WHEN TRIM(PROFIT_CMNT) = '' THEN NULL
            ELSE TRIM(PROFIT_CMNT)
            END as pcmnt,
        CASE 
        WHEN TO_NUMBER(PROFIT_YDTE) = 0 THEN '0'
        WHEN TO_NUMBER(PROFIT_YDTE) >= 59 THEN '19' || PROFIT_YDTE
        ELSE '20' || LPAD(PROFIT_YDTE, 2, '0')
        END AS FOUR_DIGIT_YEAR

    FROM {SOURCE_PROFITSHARE_SCHEMA}.PROFIT_DETAIL;

    -----------------THIS IS THE OTHER TABLE THAT ALSO GOES TO PROFIT_DETAIL----------------
-- DO NOT TRUNCATE TABLE

    INSERT INTO PROFIT_DETAIL
    (PROFIT_YEAR,
     PROFIT_YEAR_ITERATION,
     PROFIT_CODE_ID,
     CONTRIBUTION,
     EARNINGS,
     FORFEITURE,
     ZERO_CONTRIBUTION_REASON_ID,
     FEDERAL_TAXES,
     STATE_TAXES,
     TAX_CODE_ID,
     SSN,
     DISTRIBUTION_SEQUENCE,
     MONTH_TO_DATE,
     REMARK,
     YEAR_TO_DATE)
    SELECT
        TRUNC(PROFIT_SS_YEAR),
        MOD(PROFIT_SS_YEAR * 10, 10),
        PROFIT_SS_CODE,
        PROFIT_SS_CONT,
        PROFIT_SS_EARN,
        PROFIT_SS_FORT,
        CASE
            WHEN PROFIT_SS_ZEROCONT = ' ' THEN NULL
            ELSE PROFIT_SS_ZEROCONT
            END as Zcode,
        NVL(PROFIT_SS_FED_TAXES,0),
        NVL(PROFIT_SS_STATE_TAXES,0),
        CASE
            WHEN PROFIT_SS_TAX_CODE = ' ' THEN NULL
            ELSE PROFIT_SS_TAX_CODE
            END as ptcode,
        PR_SS_D_S_SEC_NUMBER,
        PROFIT_SS_DET_PR_SS_D_S_SEQNUM,
        PROFIT_SS_MDTE,
        CASE
            WHEN TRIM(PROFIT_SS_CMNT) = '' THEN NULL
            ELSE TRIM(PROFIT_SS_CMNT)
            END as pcmnt,
        CASE           
            WHEN TO_NUMBER(PROFIT_SS_YDTE) = 0 THEN '0'
            WHEN TO_NUMBER(PROFIT_SS_YDTE) >= 59 THEN '19' || PROFIT_SS_YDTE
            ELSE '20' || LPAD(PROFIT_SS_YDTE, 2, '0') 
            END AS FOUR_DIGIT_YEAR

    FROM {SOURCE_PROFITSHARE_SCHEMA}.PROFIT_SS_DETAIL;

    ---------------------------------------------------------------------------------------------------------------------

-- Step 2: Insert data from legacy table to new table
    INSERT INTO PROFIT_SHARE_CHECK
    (
        CHECK_NUMBER,
        SSN,
        DEMOGRAPHIC_ID,
        PAYABLE_NAME,
        CHECK_AMOUNT,
        TAX_CODE_ID,
        CHECK_DATE,
        VOID_FLAG,
        VOID_CHECK_DATE,
        VOID_RECON_DATE,
        CLEAR_DATE,
        CLEAR_DATE_LOADED,
        REF_NUMBER,
        FLOAT_DAYS,
        CHECK_RUN_DATE,
        DATE_LOADED,
        OTHER_BENEFICIARY,
        MANUAL_CHECK,
        REPLACE_CHECK,
        PSC_CHECK_ID
    )
    SELECT
        psc.CHECK_NUMBER,
        psc.EMPLOYEE_SSN,  -- Assuming EMPLOYEE_SSN is mapped to SSN
        d.ID AS DEMOGRAPHIC_ID,  -- Lookup ORACLE_HCM_ID from DEMOGRAPHIC table using SSN
        psc.PAYABLE_NAME,
        psc.CHECK_AMOUNT,
        CASE WHEN psc.TAX_CODE IS NULL THEN '0'
             ELSE psc.TAX_CODE
            END AS TAX_CODE_ID,
        psc.CHECK_DATE,
        psc.VOID_FLAG,  -- Store VOID_FLAG as a NUMBER(1) (1 for true, 0 for false)
        psc.VOID_CHECK_DATE,
        psc.VOID_RECON_DATE,
        psc.CLEAR_DATE,
        psc.CLEAR_DATE_LOADED,
        TO_NUMBER(psc.REF_NUMBER) AS REF_NUMBER,  -- Assuming REF_NUMBER can be converted to a NUMBER
        psc.FLOAT_DAYS,
        psc.CHECK_RUN_DATE,
        psc.DATE_LOADED,
        CASE psc.OTHER_BENEFICIARY WHEN '1' THEN 1 ELSE 0 END,  -- Converting VARCHAR2(1) to NUMBER(1) for boolean
        CASE psc.MANUAL_CHECK WHEN '1' THEN 1 ELSE 0 END,  -- Converting VARCHAR2(1) to NUMBER(1) for boolean
        psc.REPLACE_CHECK,
        psc.PSC_CHECK_ID
    FROM
        {SOURCE_PROFITSHARE_SCHEMA}.PROFIT_SHARE_CHECKS psc
            JOIN
        DEMOGRAPHIC d ON psc.EMPLOYEE_SSN = d.SSN;  -- Lookup the DEMOGRAPHIC_ID using the SSN from DEMOGRAPHIC


    INSERT INTO DISTRIBUTION_REQUEST (
        DEMOGRAPHIC_ID,
        REASON_ID,
        STATUS_ID,
        TYPE_ID,
        REASON_TEXT,
        REASON_OTHER,
        AMOUNT_REQUESTED,
        AMOUNT_AUTHORIZED,
        DATE_REQUESTED,
        DATE_DECIDED,
        TAX_CODE_ID
    )
    SELECT
        d.ID,
        DRM.ID AS REASON_ID,
        DSM.ID AS STATUS_ID,
        DTM.ID AS TYPE_ID,
        PDR.PROFIT_DIST_REQ_REASON,
        PDR.PROFIT_DIST_REQ_REASON_OTHER,
        PDR.PROFIT_DIST_REQ_AMT_REQ,
        PDR.PROFIT_DIST_REQ_AMT_AUTH,
        PDR.PROFIT_DIST_REQ_DATE_ENT,
        PDR.PROFIT_DIST_REQ_DATE_AUTH,
        PDR.PROFIT_DIST_REQ_TAXCODE
    FROM
        {SOURCE_PROFITSHARE_SCHEMA}.PROFIT_DIST_REQ PDR
            inner join DEMOGRAPHIC d on d.BADGE_NUMBER = PDR.PROFIT_DIST_REQ_EMP
            LEFT JOIN
        DISTRIBUTION_REQUEST_TYPE DTM ON PDR.PROFIT_DIST_REQ_TYPE = DTM.NAME
            LEFT JOIN
        DISTRIBUTION_REQUEST_REASON DRM ON PDR.PROFIT_DIST_REQ_REASON = DRM.NAME
            LEFT JOIN
        DISTRIBUTION_REQUEST_STATUS DSM ON PDR.PROFIT_DIST_REQ_STATUS = DSM.ID
    WHERE
        PDR.PROFIT_DIST_REQ_TYPE IS NOT NULL
      AND DRM.ID IS NOT NULL
      AND DSM.ID IS NOT NULL;

    BEGIN
        -- Declare variables
        DECLARE
            source_count NUMBER;
            target_count NUMBER;
            integrity_mismatches NUMBER;
        BEGIN
            -- 1. Validate row counts
            SELECT COUNT(*) INTO source_count
            FROM {SOURCE_PROFITSHARE_SCHEMA}.PROFIT_DIST_REQ
            WHERE PROFIT_DIST_REQ_TYPE IS NOT NULL;

            SELECT COUNT(*) INTO target_count
            FROM DISTRIBUTION_REQUEST
            WHERE TYPE_ID IS NOT NULL;

            IF source_count != target_count THEN
                DBMS_OUTPUT.PUT_LINE('Row count mismatch: Source=' || source_count || ', Target=' || target_count);
            ELSE
                DBMS_OUTPUT.PUT_LINE('Row counts match: ' || source_count || ' rows.');
            END IF;

            -- 2. Data integrity validation
            SELECT COUNT(*) INTO integrity_mismatches
            FROM (
                     SELECT
                         PDR.PROFIT_DIST_REQ_EMP,
                         PDR.PROFIT_DIST_REQ_REASON,
                         PDR.PROFIT_DIST_REQ_STATUS,
                         PDR.PROFIT_DIST_REQ_TYPE,
                         DR.DEMOGRAPHIC_ID,
                         DRM.ID AS REASON_ID,
                         DSM.ID AS STATUS_ID,
                         DTM.ID AS TYPE_ID
                     FROM {SOURCE_PROFITSHARE_SCHEMA}.PROFIT_DIST_REQ PDR
                              LEFT JOIN DEMOGRAPHIC D ON D.BADGE_NUMBER = PDR.PROFIT_DIST_REQ_EMP
                              LEFT JOIN DISTRIBUTION_REQUEST_TYPE DTM ON PDR.PROFIT_DIST_REQ_TYPE = DTM.NAME
                              LEFT JOIN DISTRIBUTION_REQUEST_REASON DRM ON PDR.PROFIT_DIST_REQ_REASON = DRM.NAME
                              LEFT JOIN DISTRIBUTION_REQUEST_STATUS DSM ON PDR.PROFIT_DIST_REQ_STATUS = DSM.ID
                              LEFT JOIN DISTRIBUTION_REQUEST DR ON DR.DEMOGRAPHIC_ID = D.ID
                     WHERE (
                               D.ID IS NULL OR
                               DRM.ID IS NULL OR
                               DSM.ID IS NULL OR
                               DTM.ID IS NULL
                               )
                 );

            IF integrity_mismatches > 0 THEN
                DBMS_OUTPUT.PUT_LINE('Data integrity validation failed: ' || integrity_mismatches || ' mismatches found.');
            ELSE
                DBMS_OUTPUT.PUT_LINE('Data integrity validation passed. No mismatches found.');
            END IF;

            -- 3. Null value checks in target table
            SELECT COUNT(*) INTO integrity_mismatches
            FROM DISTRIBUTION_REQUEST
            WHERE DEMOGRAPHIC_ID IS NULL
               OR REASON_ID IS NULL
               OR STATUS_ID IS NULL
               OR TYPE_ID IS NULL;

            IF integrity_mismatches > 0 THEN
                DBMS_OUTPUT.PUT_LINE('Null value validation failed: ' || integrity_mismatches || ' rows with null values found.');
            ELSE
                DBMS_OUTPUT.PUT_LINE('Null value validation passed. No null values found in critical columns.');
            END IF;

            -- Final result
            DBMS_OUTPUT.PUT_LINE('Validation completed successfully.');
        END;
    END;




    --Update Comment types
--Transfer outs XFER>07000711000
    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 1,
        COMMENT_RELATED_ORACLE_HCM_ID = (SELECT ORACLE_HCM_ID FROM DEMOGRAPHIC d WHERE d.BADGE_NUMBER = TO_NUMBER(SUBSTR(pd.REMARK,6,7))),
        COMMENT_RELATED_PSN_SUFFIX  = TO_NUMBER(SUBSTR(PD.REMARK,13,4))
    WHERE REMARK LIKE 'XFER>%';

    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 1,
        COMMENT_RELATED_ORACLE_HCM_ID = (SELECT ORACLE_HCM_ID FROM DEMOGRAPHIC d WHERE d.BADGE_NUMBER = TO_NUMBER(SUBSTR(pd.REMARK,7,7))),
        COMMENT_RELATED_PSN_SUFFIX  = TO_NUMBER(SUBSTR(PD.REMARK,14,4))
    WHERE REMARK LIKE 'XFER >%';

--Tranfer Ins XFER<07000710000
    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 2,
        COMMENT_RELATED_ORACLE_HCM_ID = (SELECT ORACLE_HCM_ID FROM DEMOGRAPHIC d WHERE d.BADGE_NUMBER = TO_NUMBER(SUBSTR(pd.REMARK,6,7))),
        COMMENT_RELATED_PSN_SUFFIX  = TO_NUMBER(SUBSTR(PD.REMARK,13,4))
    WHERE REMARK LIKE 'XFER<%';

    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 2,
        COMMENT_RELATED_ORACLE_HCM_ID = (SELECT ORACLE_HCM_ID FROM DEMOGRAPHIC d WHERE d.BADGE_NUMBER = TO_NUMBER(SUBSTR(pd.REMARK,7,7))),
        COMMENT_RELATED_PSN_SUFFIX  = TO_NUMBER(SUBSTR(PD.REMARK,14,4))
    WHERE REMARK LIKE 'XFER <%';

--QDRO Out QDRO>07002111000
    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 3,
        COMMENT_RELATED_ORACLE_HCM_ID = (SELECT ORACLE_HCM_ID FROM DEMOGRAPHIC d WHERE d.BADGE_NUMBER = TO_NUMBER(SUBSTR(pd.REMARK,6,7))),
        COMMENT_RELATED_PSN_SUFFIX  = TO_NUMBER(SUBSTR(PD.REMARK,13,4))
    WHERE REMARK LIKE 'QDRO>%';

    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 3,
        COMMENT_RELATED_ORACLE_HCM_ID = (SELECT ORACLE_HCM_ID FROM DEMOGRAPHIC d WHERE d.BADGE_NUMBER = TO_NUMBER(SUBSTR(pd.REMARK,7,7))),
        COMMENT_RELATED_PSN_SUFFIX  = TO_NUMBER(SUBSTR(PD.REMARK,14,4))
    WHERE REMARK LIKE 'QDRO >%';

--QDRO In QDRO<07002110000
    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 4,
        COMMENT_RELATED_ORACLE_HCM_ID = (SELECT ORACLE_HCM_ID FROM DEMOGRAPHIC d WHERE d.BADGE_NUMBER = TO_NUMBER(SUBSTR(pd.REMARK,6,7))),
        COMMENT_RELATED_PSN_SUFFIX  = TO_NUMBER(SUBSTR(PD.REMARK,13,4))
    WHERE REMARK LIKE 'QDRO<%';

    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 4,
        COMMENT_RELATED_ORACLE_HCM_ID = (SELECT ORACLE_HCM_ID FROM DEMOGRAPHIC d WHERE d.BADGE_NUMBER = TO_NUMBER(SUBSTR(pd.REMARK,7,7))),
        COMMENT_RELATED_PSN_SUFFIX  = TO_NUMBER(SUBSTR(PD.REMARK,14,4))
    WHERE REMARK LIKE 'QDRO <%';

--V-Only
    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 5
    WHERE REMARK LIKE 'V-ONLY%';

--Forfeit
    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 6
    WHERE REMARK LIKE 'FORFEIT%';

--Un-Forfeit
    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 7
    WHERE REMARK LIKE 'UN-FORFEIT%';

--Class Action
    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 8
    WHERE REMARK LIKE 'CLASS-ACTION'
       OR REMARK LIKE 'CLASS ACTION';

--Voided VOIDED 012257 MA
    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 9,
        COMMENT_RELATED_CHECK_NUMBER = CASE WHEN LENGTH(TRIM(pd.REMARK)) > 14 THEN SUBSTR(pd.REMARK, 8, 6) ELSE NULL END,
        COMMENT_IS_PARTIAL_TRANSACTION = CASE WHEN SUBSTR(pd.REMARK,14,1) = '+' THEN 1 ELSE 0 END,
        COMMENT_RELATED_STATE = SUBSTR(pd.REMARK,15,2)
    WHERE REMARK LIKE 'VOIDED%';

--Hardship HRDSHP 021103 MA
    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 10,
        COMMENT_RELATED_CHECK_NUMBER = CASE WHEN LENGTH(TRIM(pd.REMARK)) > 14 THEN SUBSTR(pd.REMARK, 8, 6) ELSE NULL END,
        COMMENT_IS_PARTIAL_TRANSACTION = CASE WHEN SUBSTR(pd.REMARK,14,1) = '+' THEN 1 ELSE 0 END,
        COMMENT_RELATED_STATE = SUBSTR(pd.REMARK,15,2)
    WHERE REMARK LIKE 'HRDSHP%'
       OR REMARK LIKE 'HARDSHIP%';

--Distribution
    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 11,
        COMMENT_RELATED_CHECK_NUMBER = CASE WHEN LENGTH(TRIM(pd.REMARK)) > 14 THEN SUBSTR(pd.REMARK, 8, 6) ELSE NULL END,
        COMMENT_IS_PARTIAL_TRANSACTION = CASE WHEN SUBSTR(pd.REMARK,14,1) = '+' THEN 1 ELSE 0 END,
        COMMENT_RELATED_STATE = SUBSTR(pd.REMARK,15,2)
    WHERE REMARK LIKE 'DISTRB%';

    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 11
    WHERE REMARK LIKE 'DIST%IBUTION%';

--Payoff
    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 12
    WHERE REMARK LIKE 'PAYOFF'
       OR REMARK LIKE 'PAY OFF';

--DirPay DIRPAY 048465+MA
    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 13,
        COMMENT_RELATED_CHECK_NUMBER = CASE WHEN LENGTH(TRIM(pd.REMARK)) > 14 THEN SUBSTR(pd.REMARK, 8, 6) ELSE NULL END,
        COMMENT_IS_PARTIAL_TRANSACTION = CASE WHEN SUBSTR(pd.REMARK,14,1) = '+' THEN 1 ELSE 0 END,
        COMMENT_RELATED_STATE = SUBSTR(pd.REMARK,15,2)
    WHERE REMARK LIKE 'DIRPAY%'
       OR REMARK LIKE 'DIRECT PAY%';

--Rollover ROLOVR 029663 NH
    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 14,
        COMMENT_RELATED_CHECK_NUMBER = CASE WHEN LENGTH(TRIM(pd.REMARK)) > 14 THEN SUBSTR(pd.REMARK, 8, 6) ELSE NULL END,
        COMMENT_IS_PARTIAL_TRANSACTION = CASE WHEN SUBSTR(pd.REMARK,14,1) = '+' THEN 1 ELSE 0 END,
        COMMENT_RELATED_STATE = SUBSTR(pd.REMARK,15,2)
    WHERE REMARK LIKE 'ROLOVR%'
       OR REMARK LIKE 'ROLLOVER%';

--Rollover ROTHIR 053087+FL
    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 15,
        COMMENT_RELATED_CHECK_NUMBER = CASE WHEN LENGTH(TRIM(pd.REMARK)) > 14 THEN SUBSTR(pd.REMARK, 8, 6) ELSE NULL END,
        COMMENT_IS_PARTIAL_TRANSACTION = CASE WHEN SUBSTR(pd.REMARK,14,1) = '+' THEN 1 ELSE 0 END,
        COMMENT_RELATED_STATE = SUBSTR(pd.REMARK,15,2)
    WHERE REMARK LIKE 'ROTHIR%';

--Over 64, years vested
    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 16
    WHERE REMARK LIKE '>64-1 YR VEST' OR REMARK LIKE '>64 - 1 YR VEST';

    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 17
    WHERE REMARK LIKE '>64-2 YR VEST' OR REMARK LIKE '>64 - 2 YR VEST';

    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 18
    WHERE REMARK LIKE '>64-3 YR VEST' OR REMARK LIKE '>64 - 3 YR VEST';

--Military
    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 19
    WHERE REMARK LIKE 'MILITARY';

--Other
    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 20
    WHERE REMARK LIKE 'OTHER';

--Rev
    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 21
    WHERE REMARK LIKE 'REV%';

--Un-Rev
    UPDATE PROFIT_DETAIL pd
    SET COMMENT_TYPE_ID = 22
    WHERE REMARK LIKE 'UN-REV%';

    UPDATE profit_detail pd
    SET comment_type_id = 23
    WHERE REMARK = '100% EARNINGS';

    UPDATE profit_detail pd
    SET comment_type_id = 24
    WHERE REMARK = '>64 & >5 100%';

    --https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/402817082/008-12+to+forfeit+Class+Action+-+Mockup
    UPDATE profit_detail pd
    SET comment_type_id = 25
    WHERE REMARK LIKE 'FORFEIT CA';

    UPDATE PROFIT_DETAIL pd
       SET COMMENT_TYPE_ID = 25
     WHERE EXISTS 
      (
            SELECT pdF.Id FROM PROFIT_DETAIL pdF
            JOIN PROFIT_DETAIL pdCa ON pdF.SSN = pdCa.SSN AND pdF.PROFIT_YEAR >= pdCa.PROFIT_YEAR
            WHERE pdF.COMMENT_TYPE_ID = 6
            AND pdCa.COMMENT_TYPE_ID = 8
            AND pdF.FORFEITURE  = pdCA.EARNINGS
            AND pd.ID  = pdF.ID
      ) -- Forfeits that match a class action should be categorized as FORFEIT CA 

    -- Set flag on Profit Detail marking a year of elibility for Profit Sharing
-- Comment types indicating years of service other than a contribution include:
-- V-Only
-- > 64 - 1 Year Vested
-- > 64 - 2 Year Vested
-- > 64 - 3 Year Vested
-- >64 & >5 100%
-- Military
-- https://demoulas.atlassian.net/browse/PS-1147
    UPDATE PROFIT_DETAIL pd
       SET pd.YEARS_OF_SERVICE_CREDIT = CASE WHEN (pd.Contribution > 0 OR pd.COMMENT_TYPE_ID IN (5, 16, 17, 18, 19)) AND MONTH_TO_DATE != 20 THEN 1 ELSE 0 END
     WHERE EXISTS (SELECT SSN FROM DEMOGRAPHIC d WHERE d.SSN = pd.SSN);


-- https://demoulas.atlassian.net/browse/PS-1147
Update PROFIT_DETAIL pd
    SET MONTH_TO_DATE = 1 
        WHERE MONTH_TO_DATE = 20;

-- Approimate the creation date to aid in correct sorting of the transactions
UPDATE PROFIT_DETAIL
SET CREATED_UTC = TO_TIMESTAMP(
        YEAR_TO_DATE || '-' ||
        LPAD(MONTH_TO_DATE, 2, '0') || '-01 00:00:00',
        'YYYY-MM-DD HH24:MI:SS'
                     ) AT TIME ZONE 'UTC'
WHERE YEAR_TO_DATE > 1900 AND MONTH_TO_DATE BETWEEN 1 AND 12;

END;
COMMIT ;
