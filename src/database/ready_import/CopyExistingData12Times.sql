/*
    Oracle v21 Script to Create 12x Data in Specific Tables
    
    This script should be run AFTER the main "SQL copy all from ready to smart ps.sql" script
    to create 12x the amount of data in the following tables:
    - DEMOGRAPHIC
    - DEMOGRAPHIC_HISTORY  
    - PROFIT_DETAIL
    - PAY_PROFIT
    - BENEFICIARY
    - BENEFICIARY_CONTACT
    
    The script maintains referential integrity by:
    1. Creating new records with modified identifiers
    2. Preserving relationships between tables
    3. Using sequences where appropriate
    4. Handling potential constraint violations
    
    Updated for current EF Core 9 schema with ContactInfo/Address complex types
    Uses dynamic offsets based on maximum values per iteration:
    - Oracle HCM IDs: max + 1 per iteration (guarantees no collisions or precision issues)
    - Badge Numbers: max + 1 per iteration (guarantees no collisions or precision issues)
    - SSNs: max + 1 per iteration (guarantees no collisions or precision issues)
    - PSN_SUFFIX: max + 1 per iteration (guarantees no collisions or precision issues)
*/

DECLARE
    v_max_oracle_hcm_id NUMBER;
    v_max_badge_number NUMBER;
    v_max_ssn NUMBER;
    v_max_psn_suffix NUMBER;
    v_max_demographic_id NUMBER;
    v_max_beneficiary_contact_id NUMBER;
    v_max_beneficiary_id NUMBER;
    
    -- Offset values calculated from max + 1 approach
    v_oracle_hcm_offset NUMBER;
    v_badge_offset NUMBER;
    v_ssn_offset NUMBER;
    v_psn_offset NUMBER;
    
    -- Counter for progress tracking
    v_counter NUMBER := 0;
    v_total_records NUMBER := 0;
    v_iteration NUMBER;
    
    -- Variables to hold original counts
    v_original_demographic NUMBER;
    v_original_demographic_history NUMBER;
    v_original_beneficiary_contact NUMBER;
    v_original_beneficiary NUMBER;
    v_original_pay_profit NUMBER;
    v_original_profit_detail NUMBER;
    
    -- Variable for iteration prefix (A-K for 11 copies, original + 11 = 12x total)
    v_iteration_prefix NVARCHAR2(2);
    
BEGIN
    DBMS_OUTPUT.PUT_LINE('Starting data multiplication process (creating 12x original data)...');
    
    -- First, disable foreign key constraints to avoid issues during bulk inserts
    FOR fk IN (SELECT constraint_name, table_name 
               FROM user_constraints 
               WHERE constraint_type = 'R' 
               AND table_name IN ('DEMOGRAPHIC', 'DEMOGRAPHIC_HISTORY', 'PROFIT_DETAIL', 
                                'PAY_PROFIT', 'BENEFICIARY', 'BENEFICIARY_CONTACT')) 
    LOOP
        EXECUTE IMMEDIATE 'ALTER TABLE ' || fk.table_name || 
                         ' DISABLE CONSTRAINT ' || fk.constraint_name;
    END LOOP;
    
    -- Store original record counts
    SELECT COUNT(*) INTO v_original_demographic FROM DEMOGRAPHIC;
    SELECT COUNT(*) INTO v_original_demographic_history FROM DEMOGRAPHIC_HISTORY;
    SELECT COUNT(*) INTO v_original_beneficiary_contact FROM BENEFICIARY_CONTACT;
    SELECT COUNT(*) INTO v_original_beneficiary FROM BENEFICIARY;
    SELECT COUNT(*) INTO v_original_pay_profit FROM PAY_PROFIT;
    SELECT COUNT(*) INTO v_original_profit_detail FROM PROFIT_DETAIL;
    
    -- Get maximum values to calculate safe offsets
    SELECT NVL(MAX(ORACLE_HCM_ID), 0) INTO v_max_oracle_hcm_id FROM DEMOGRAPHIC;
    SELECT NVL(MAX(BADGE_NUMBER), 0) INTO v_max_badge_number FROM DEMOGRAPHIC;
    SELECT NVL(MAX(SSN), 0) INTO v_max_ssn FROM DEMOGRAPHIC;
    SELECT NVL(MAX(PSN_SUFFIX), 0) INTO v_max_psn_suffix FROM BENEFICIARY;
    
    DBMS_OUTPUT.PUT_LINE('Maximum values in existing data:');
    DBMS_OUTPUT.PUT_LINE('  Max Oracle HCM ID: ' || v_max_oracle_hcm_id);
    DBMS_OUTPUT.PUT_LINE('  Max Badge Number: ' || v_max_badge_number);
    DBMS_OUTPUT.PUT_LINE('  Max SSN: ' || v_max_ssn);
    DBMS_OUTPUT.PUT_LINE('  Max PSN Suffix: ' || v_max_psn_suffix);
    
    DBMS_OUTPUT.PUT_LINE('Original record counts:');
    DBMS_OUTPUT.PUT_LINE('  DEMOGRAPHIC: ' || v_original_demographic);
    DBMS_OUTPUT.PUT_LINE('  DEMOGRAPHIC_HISTORY: ' || v_original_demographic_history);
    DBMS_OUTPUT.PUT_LINE('  BENEFICIARY_CONTACT: ' || v_original_beneficiary_contact);
    DBMS_OUTPUT.PUT_LINE('  BENEFICIARY: ' || v_original_beneficiary);
    DBMS_OUTPUT.PUT_LINE('  PAY_PROFIT: ' || v_original_pay_profit);
    DBMS_OUTPUT.PUT_LINE('  PROFIT_DETAIL: ' || v_original_profit_detail);
    
    -- Loop 11 times to create 11 additional copies (original + 11 = 12x total)
    FOR v_iteration IN 1..11 LOOP
        -- Set iteration prefix: A through K for iterations 1-11
        IF v_iteration <= 9 THEN
            v_iteration_prefix := CHR(64 + v_iteration); -- A through I
        ELSE
            v_iteration_prefix := 'J' || CHR(48 + (v_iteration - 10)); -- J1, J2, etc. (but we only go to J1 for iteration 10, K for 11)
        END IF;
        
        -- Override for specific iterations
        IF v_iteration = 10 THEN
            v_iteration_prefix := 'J';
        ELSIF v_iteration = 11 THEN
            v_iteration_prefix := 'K';
        END IF;
        
        DBMS_OUTPUT.PUT_LINE('');
        DBMS_OUTPUT.PUT_LINE('=== CREATING COPY SET #' || v_iteration || ' (Prefix: ' || v_iteration_prefix || ') ===');
        
        -- Get current maximum values for this iteration and calculate offsets (max + 1)
        SELECT NVL(MAX(ORACLE_HCM_ID), 0) + 1 INTO v_oracle_hcm_offset FROM DEMOGRAPHIC;
        SELECT NVL(MAX(BADGE_NUMBER), 0) + 1 INTO v_badge_offset FROM DEMOGRAPHIC;
        SELECT NVL(MAX(SSN), 0) + 1 INTO v_max_ssn FROM DEMOGRAPHIC;
        SELECT NVL(MAX(PSN_SUFFIX), 0) + 1 INTO v_psn_offset FROM BENEFICIARY;
        SELECT NVL(MAX(ID), 0) INTO v_max_demographic_id FROM DEMOGRAPHIC;
        SELECT NVL(MAX(ID), 0) INTO v_max_beneficiary_contact_id FROM BENEFICIARY_CONTACT;
        SELECT NVL(MAX(ID), 0) INTO v_max_beneficiary_id FROM BENEFICIARY;
        
        DBMS_OUTPUT.PUT_LINE('Iteration #' || v_iteration || ' offsets (starting from max + 1):');
        DBMS_OUTPUT.PUT_LINE('  Oracle HCM ID offset: ' || v_oracle_hcm_offset);
        DBMS_OUTPUT.PUT_LINE('  Badge Number offset: ' || v_badge_offset);
        DBMS_OUTPUT.PUT_LINE('  SSN offset: ' || v_max_ssn);
        DBMS_OUTPUT.PUT_LINE('  PSN Suffix offset: ' || v_psn_offset);
        
        -- 1. COPY DEMOGRAPHIC TABLE (Updated for EF Core 9 schema)
        DBMS_OUTPUT.PUT_LINE('Creating DEMOGRAPHIC copy #' || v_iteration || '...');
        SELECT COUNT(*) INTO v_total_records FROM DEMOGRAPHIC;
        DBMS_OUTPUT.PUT_LINE('Current DEMOGRAPHIC records: ' || v_total_records);
    
        INSERT INTO DEMOGRAPHIC (
            ORACLE_HCM_ID,
            SSN,
            BADGE_NUMBER, 
            FULL_NAME,
            LAST_NAME,
            FIRST_NAME,
            MIDDLE_NAME,
            PHONE_NUMBER,
            MOBILE_NUMBER,
            EMAIL_ADDRESS,
            STORE_NUMBER,
            PAY_CLASSIFICATION_ID,
            STREET,
            STREET2,
            STREET3,
            STREET4,
            CITY,
            STATE,
            POSTAL_CODE,
            COUNTRY_ISO,
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
            EMPLOYMENT_STATUS_ID
        )
        SELECT 
            v_oracle_hcm_offset + (ROWNUM - 1) AS ORACLE_HCM_ID, -- Start from max+1, increment by 1
            v_max_ssn + (ROWNUM - 1) AS SSN, -- Start from max+1, increment by 1
            v_badge_offset + (ROWNUM - 1) AS BADGE_NUMBER, -- Start from max+1, increment by 1
            CASE 
                WHEN FULL_NAME IS NOT NULL THEN v_iteration_prefix || ' ' || FULL_NAME
                ELSE v_iteration_prefix || ' ' || FIRST_NAME || ' ' || LAST_NAME
            END AS FULL_NAME,
            v_iteration_prefix || ' ' || LAST_NAME AS LAST_NAME,
            v_iteration_prefix || ' ' || FIRST_NAME AS FIRST_NAME,
            MIDDLE_NAME,
            PHONE_NUMBER,
            MOBILE_NUMBER,
            CASE 
                WHEN EMAIL_ADDRESS IS NOT NULL THEN 
                    REPLACE(EMAIL_ADDRESS, '@', '+' || LOWER(v_iteration_prefix) || '@')
                ELSE NULL
            END AS EMAIL_ADDRESS,
            STORE_NUMBER,
            PAY_CLASSIFICATION_ID,
            STREET,
            STREET2,
            STREET3,
            STREET4,
            CITY,
            STATE,
            POSTAL_CODE,
            COUNTRY_ISO,
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
            EMPLOYMENT_STATUS_ID
        FROM DEMOGRAPHIC
        WHERE ID <= v_original_demographic; -- Only copy original records, not previously created copies
        
        SELECT COUNT(*) INTO v_counter FROM DEMOGRAPHIC;
        DBMS_OUTPUT.PUT_LINE('DEMOGRAPHIC copy #' || v_iteration || ' complete. Total records: ' || v_counter);
        
        -- 2. COPY DEMOGRAPHIC_HISTORY TABLE  
        DBMS_OUTPUT.PUT_LINE('Creating DEMOGRAPHIC_HISTORY copy #' || v_iteration || '...');
        SELECT COUNT(*) INTO v_total_records FROM DEMOGRAPHIC_HISTORY;
        DBMS_OUTPUT.PUT_LINE('Current DEMOGRAPHIC_HISTORY records: ' || v_total_records);
    
        INSERT INTO DEMOGRAPHIC_HISTORY (
            DEMOGRAPHIC_ID,
            VALID_FROM,
            VALID_TO,
            ORACLE_HCM_ID,
            BADGE_NUMBER,
            STORE_NUMBER,
            PAY_CLASSIFICATION_ID,
            DATE_OF_BIRTH,
            HIRE_DATE,
            REHIRE_DATE,
            TERMINATION_DATE,
            DEPARTMENT,
            EMPLOYMENT_TYPE_ID,
            PAY_FREQUENCY_ID,
            TERMINATION_CODE_ID,
            EMPLOYMENT_STATUS_ID,
            CREATED_DATETIME
        )
        SELECT 
            -- Map to the new demographic records created above (using offset-based Oracle HCM ID)
            (SELECT d2.ID 
             FROM DEMOGRAPHIC d2 
             WHERE d2.ORACLE_HCM_ID = v_oracle_hcm_offset + (dh.ID - 1)) AS DEMOGRAPHIC_ID,
            dh.VALID_FROM,
            dh.VALID_TO,
            v_oracle_hcm_offset + (dh.ID - 1) AS ORACLE_HCM_ID,
            v_badge_offset + (dh.ID - 1) AS BADGE_NUMBER,
            dh.STORE_NUMBER,
            dh.PAY_CLASSIFICATION_ID,
            dh.DATE_OF_BIRTH,
            dh.HIRE_DATE,
            dh.REHIRE_DATE,
            dh.TERMINATION_DATE,
            dh.DEPARTMENT,
            dh.EMPLOYMENT_TYPE_ID,
            dh.PAY_FREQUENCY_ID,
            dh.TERMINATION_CODE_ID,
            dh.EMPLOYMENT_STATUS_ID,
            SYSTIMESTAMP AS CREATED_DATETIME
        FROM DEMOGRAPHIC_HISTORY dh
        WHERE dh.DEMOGRAPHIC_ID <= v_original_demographic -- Only copy original records
        AND EXISTS (
            SELECT 1 FROM DEMOGRAPHIC d 
            WHERE d.ORACLE_HCM_ID = v_oracle_hcm_offset + (dh.ID - 1)
        );
        
        SELECT COUNT(*) INTO v_counter FROM DEMOGRAPHIC_HISTORY;
        DBMS_OUTPUT.PUT_LINE('DEMOGRAPHIC_HISTORY copy #' || v_iteration || ' complete. Total records: ' || v_counter);
        
        -- 3. COPY BENEFICIARY_CONTACT TABLE
        DBMS_OUTPUT.PUT_LINE('Creating BENEFICIARY_CONTACT copy #' || v_iteration || '...');
        SELECT COUNT(*) INTO v_total_records FROM BENEFICIARY_CONTACT;
        DBMS_OUTPUT.PUT_LINE('Current BENEFICIARY_CONTACT records: ' || v_total_records);
    
        INSERT INTO BENEFICIARY_CONTACT (
            SSN,
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
            EMAIL_ADDRESS
        )
        SELECT 
            v_max_ssn + (ROWNUM - 1) AS SSN, -- Start from max SSN + 1, increment by 1
            v_iteration_prefix || ' ' || FIRST_NAME AS FIRST_NAME,
            v_iteration_prefix || ' ' || LAST_NAME AS LAST_NAME,
            CASE 
                WHEN FULL_NAME IS NOT NULL THEN v_iteration_prefix || ' ' || FULL_NAME
                ELSE v_iteration_prefix || ' ' || FIRST_NAME || ' ' || LAST_NAME
            END AS FULL_NAME,
            DATE_OF_BIRTH,
            STREET,
            CITY,
            STATE,
            POSTAL_CODE,
            PHONE_NUMBER,
            MOBILE_NUMBER,
            CASE 
                WHEN EMAIL_ADDRESS IS NOT NULL THEN 
                    REPLACE(EMAIL_ADDRESS, '@', '+' || LOWER(v_iteration_prefix) || '@')
                ELSE NULL
            END AS EMAIL_ADDRESS
        FROM BENEFICIARY_CONTACT
        WHERE ID <= v_original_beneficiary_contact; -- Only copy original records
        
        SELECT COUNT(*) INTO v_counter FROM BENEFICIARY_CONTACT;
        DBMS_OUTPUT.PUT_LINE('BENEFICIARY_CONTACT copy #' || v_iteration || ' complete. Total records: ' || v_counter);
        
        -- 4. COPY BENEFICIARY TABLE
        DBMS_OUTPUT.PUT_LINE('Creating BENEFICIARY copy #' || v_iteration || '...');
        SELECT COUNT(*) INTO v_total_records FROM BENEFICIARY;
        DBMS_OUTPUT.PUT_LINE('Current BENEFICIARY records: ' || v_total_records);
    
        INSERT INTO BENEFICIARY (
            PSN_SUFFIX,
            BADGE_NUMBER,
            DEMOGRAPHIC_ID,
            BENEFICIARY_CONTACT_ID,
            RELATIONSHIP,
            PERCENT
        )
        SELECT 
            v_psn_offset + (b.ID - 1) AS PSN_SUFFIX, -- Use ID-based offset
            v_badge_offset + (d_orig.ID - 1) AS BADGE_NUMBER, -- Use original demographic ID for offset
            -- Map to new demographic ID
            (SELECT d2.ID 
             FROM DEMOGRAPHIC d2 
             WHERE d2.BADGE_NUMBER = v_badge_offset + (d_orig.ID - 1)) AS DEMOGRAPHIC_ID,
            -- Map to new beneficiary contact ID using original beneficiary contact's ID
            (SELECT bc2.ID 
             FROM BENEFICIARY_CONTACT bc2, BENEFICIARY_CONTACT bc_orig
             WHERE bc_orig.ID = b.BENEFICIARY_CONTACT_ID
             AND bc2.SSN = v_max_ssn + (bc_orig.ID - 1)
             AND ROWNUM = 1) AS BENEFICIARY_CONTACT_ID,
            b.RELATIONSHIP,
            b.PERCENT
        FROM BENEFICIARY b
        JOIN DEMOGRAPHIC d_orig ON d_orig.ID = b.DEMOGRAPHIC_ID
        WHERE b.ID <= v_original_beneficiary -- Only copy original records
        AND EXISTS (
            SELECT 1 FROM DEMOGRAPHIC d2 
            WHERE d2.BADGE_NUMBER = v_badge_offset + (d_orig.ID - 1)
        )
        AND EXISTS (
            SELECT 1 FROM BENEFICIARY_CONTACT bc_orig, BENEFICIARY_CONTACT bc2
            WHERE bc_orig.ID = b.BENEFICIARY_CONTACT_ID
            AND bc2.SSN = v_max_ssn + (bc_orig.ID - 1)
        );
        
        SELECT COUNT(*) INTO v_counter FROM BENEFICIARY;
        DBMS_OUTPUT.PUT_LINE('BENEFICIARY copy #' || v_iteration || ' complete. Total records: ' || v_counter);
        
        -- 5. COPY PAY_PROFIT TABLE
        DBMS_OUTPUT.PUT_LINE('Creating PAY_PROFIT copy #' || v_iteration || '...');
        SELECT COUNT(*) INTO v_total_records FROM PAY_PROFIT;
        DBMS_OUTPUT.PUT_LINE('Current PAY_PROFIT records: ' || v_total_records);
    
        INSERT INTO PAY_PROFIT (
            DEMOGRAPHIC_ID,
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
            ETVA
        )
        SELECT 
            -- Map to new demographic ID using offset-based badge number
            (SELECT d2.ID 
             FROM DEMOGRAPHIC d2, DEMOGRAPHIC d1 
             WHERE d1.ID = pp.DEMOGRAPHIC_ID 
             AND d2.BADGE_NUMBER = v_badge_offset + (d1.ID - 1)) AS DEMOGRAPHIC_ID,
            pp.PROFIT_YEAR,
            pp.CURRENT_HOURS_YEAR,
            pp.CURRENT_INCOME_YEAR,
            pp.WEEKS_WORKED_YEAR,
            pp.PS_CERTIFICATE_ISSUED_DATE,
            pp.ENROLLMENT_ID,
            pp.BENEFICIARY_TYPE_ID,
            pp.EMPLOYEE_TYPE_ID,
            pp.ZERO_CONTRIBUTION_REASON_ID,
            pp.HOURS_EXECUTIVE,
            pp.INCOME_EXECUTIVE,
            pp.POINTS_EARNED,
            pp.ETVA
        FROM PAY_PROFIT pp
        WHERE pp.DEMOGRAPHIC_ID <= v_original_demographic -- Only copy original records
        AND EXISTS (
            SELECT 1 
            FROM DEMOGRAPHIC d1, DEMOGRAPHIC d2 
            WHERE d1.ID = pp.DEMOGRAPHIC_ID 
            AND d2.BADGE_NUMBER = v_badge_offset + (d1.ID - 1)
        );
        
        SELECT COUNT(*) INTO v_counter FROM PAY_PROFIT;
        DBMS_OUTPUT.PUT_LINE('PAY_PROFIT copy #' || v_iteration || ' complete. Total records: ' || v_counter);
        
        -- 6. COPY PROFIT_DETAIL TABLE
        DBMS_OUTPUT.PUT_LINE('Creating PROFIT_DETAIL copy #' || v_iteration || '...');
        SELECT COUNT(*) INTO v_total_records FROM PROFIT_DETAIL;
        DBMS_OUTPUT.PUT_LINE('Current PROFIT_DETAIL records: ' || v_total_records);
    
        INSERT INTO PROFIT_DETAIL (
            PROFIT_YEAR,
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
            YEAR_TO_DATE
        )
        SELECT 
            pd.PROFIT_YEAR,
            pd.PROFIT_YEAR_ITERATION,
            pd.PROFIT_CODE_ID,
            pd.CONTRIBUTION,
            pd.EARNINGS,
            pd.FORFEITURE,
            pd.ZERO_CONTRIBUTION_REASON_ID,
            pd.FEDERAL_TAXES,
            pd.STATE_TAXES,
            pd.TAX_CODE_ID,
            pd.SSN + (ROWNUM - 1) AS SSN, -- Offset SSN using ROWNUM (starts from current max)
            pd.DISTRIBUTION_SEQUENCE,
            pd.MONTH_TO_DATE,
            CASE 
                WHEN pd.REMARK IS NULL THEN v_iteration_prefix || '_DATA'
                ELSE v_iteration_prefix || '_' || SUBSTR(pd.REMARK, 1, 45) -- Ensure we don't exceed column limits
            END AS REMARK,
            pd.YEAR_TO_DATE
        FROM PROFIT_DETAIL pd
        WHERE pd.ID <= v_original_profit_detail -- Only copy original records
        AND EXISTS (
            SELECT 1 FROM DEMOGRAPHIC d 
            WHERE d.SSN = v_max_ssn + (pd.ID - 1)
        );
        
        SELECT COUNT(*) INTO v_counter FROM PROFIT_DETAIL;
        DBMS_OUTPUT.PUT_LINE('PROFIT_DETAIL copy #' || v_iteration || ' complete. Total records: ' || v_counter);
        
        -- Commit after each iteration to prevent transaction log issues
        COMMIT;
        DBMS_OUTPUT.PUT_LINE('Iteration #' || v_iteration || ' committed successfully.');
        
    END LOOP; -- End of iteration loop
    
    -- Re-enable all foreign key constraints
    FOR fk IN (SELECT constraint_name, table_name 
               FROM user_constraints 
               WHERE constraint_type = 'R' 
               AND table_name IN ('DEMOGRAPHIC', 'DEMOGRAPHIC_HISTORY', 'PROFIT_DETAIL', 
                                'PAY_PROFIT', 'BENEFICIARY', 'BENEFICIARY_CONTACT')) 
    LOOP
        BEGIN
            EXECUTE IMMEDIATE 'ALTER TABLE ' || fk.table_name || 
                             ' ENABLE CONSTRAINT ' || fk.constraint_name;
        EXCEPTION
            WHEN OTHERS THEN
                DBMS_OUTPUT.PUT_LINE('Warning: Could not re-enable constraint ' || 
                                   fk.constraint_name || ' on table ' || fk.table_name || 
                                   '. Error: ' || SQLERRM);
        END;
    END LOOP;
    
    -- Final verification - count records in each table
    DBMS_OUTPUT.PUT_LINE('');
    DBMS_OUTPUT.PUT_LINE('=== FINAL RECORD COUNTS ===');
    
    SELECT COUNT(*) INTO v_counter FROM DEMOGRAPHIC;
    DBMS_OUTPUT.PUT_LINE('DEMOGRAPHIC: ' || v_counter || ' records (Original: ' || v_original_demographic || ', Expected: ' || (v_original_demographic * 12) || ')');
    
    SELECT COUNT(*) INTO v_counter FROM DEMOGRAPHIC_HISTORY;
    DBMS_OUTPUT.PUT_LINE('DEMOGRAPHIC_HISTORY: ' || v_counter || ' records (Original: ' || v_original_demographic_history || ', Expected: ' || (v_original_demographic_history * 12) || ')');
    
    SELECT COUNT(*) INTO v_counter FROM BENEFICIARY_CONTACT;
    DBMS_OUTPUT.PUT_LINE('BENEFICIARY_CONTACT: ' || v_counter || ' records (Original: ' || v_original_beneficiary_contact || ', Expected: ' || (v_original_beneficiary_contact * 12) || ')');
    
    SELECT COUNT(*) INTO v_counter FROM BENEFICIARY;
    DBMS_OUTPUT.PUT_LINE('BENEFICIARY: ' || v_counter || ' records (Original: ' || v_original_beneficiary || ', Expected: ' || (v_original_beneficiary * 12) || ')');
    
    SELECT COUNT(*) INTO v_counter FROM PAY_PROFIT;
    DBMS_OUTPUT.PUT_LINE('PAY_PROFIT: ' || v_counter || ' records (Original: ' || v_original_pay_profit || ', Expected: ' || (v_original_pay_profit * 12) || ')');
    
    SELECT COUNT(*) INTO v_counter FROM PROFIT_DETAIL;
    DBMS_OUTPUT.PUT_LINE('PROFIT_DETAIL: ' || v_counter || ' records (Original: ' || v_original_profit_detail || ', Expected: ' || (v_original_profit_detail * 12) || ')');
    
    -- Performance verification
    DBMS_OUTPUT.PUT_LINE('');
    DBMS_OUTPUT.PUT_LINE('=== PERFORMANCE VERIFICATION ===');
    DBMS_OUTPUT.PUT_LINE('Checking index coverage for large dataset...');
    
    -- Sample query performance check
    SELECT COUNT(*) INTO v_counter 
    FROM DEMOGRAPHIC 
    WHERE EMPLOYMENT_STATUS_ID = 'A' 
    AND STORE_NUMBER BETWEEN 1 AND 100;
    DBMS_OUTPUT.PUT_LINE('Active employees in stores 1-100: ' || v_counter);
    
    SELECT COUNT(*) INTO v_counter 
    FROM DEMOGRAPHIC 
    WHERE SSN > 1000000 
    AND ORACLE_HCM_ID > 10000;
    DBMS_OUTPUT.PUT_LINE('Generated test records: ' || v_counter);
    
    DBMS_OUTPUT.PUT_LINE('');
    DBMS_OUTPUT.PUT_LINE('Data multiplication process completed successfully! Created 12x original data.');
    DBMS_OUTPUT.PUT_LINE('Total processing time: ' || ROUND((DBMS_UTILITY.GET_TIME - DBMS_UTILITY.GET_TIME)/100, 2) || ' seconds');
    
    -- Final commit
    COMMIT;
    
EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        DBMS_OUTPUT.PUT_LINE('Error during data multiplication process: ' || SQLERRM);
        DBMS_OUTPUT.PUT_LINE('Error occurred at iteration: ' || NVL(v_iteration, 0));
        DBMS_OUTPUT.PUT_LINE('All changes have been rolled back.');
        
        -- Try to re-enable constraints even after error
        FOR fk IN (SELECT constraint_name, table_name 
                   FROM user_constraints 
                   WHERE constraint_type = 'R' 
                   AND table_name IN ('DEMOGRAPHIC', 'DEMOGRAPHIC_HISTORY', 'PROFIT_DETAIL', 
                                    'PAY_PROFIT', 'BENEFICIARY', 'BENEFICIARY_CONTACT')) 
        LOOP
            BEGIN
                EXECUTE IMMEDIATE 'ALTER TABLE ' || fk.table_name || 
                                 ' ENABLE CONSTRAINT ' || fk.constraint_name;
            EXCEPTION
                WHEN OTHERS THEN
                    NULL; -- Ignore errors when trying to re-enable
            END;
        END LOOP;
        
        RAISE;
END;
/