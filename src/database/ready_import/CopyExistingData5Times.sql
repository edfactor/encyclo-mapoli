/*
    Oracle v21 Script to Create 5x Data in Specific Tables
    
    This script should be run AFTER the main "SQL copy all from ready to smart ps.sql" script
    to create 5x the amount of data in the following tables:
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
*/

DECLARE
    v_max_oracle_hcm_id NUMBER;
    v_max_badge_number NUMBER;
    v_max_demographic_id NUMBER;
    v_max_beneficiary_contact_id NUMBER;
    v_max_beneficiary_id NUMBER;
    
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
    
    -- Variable for iteration prefix
    v_iteration_prefix NVARCHAR2(1);
    
BEGIN
    DBMS_OUTPUT.PUT_LINE('Starting data multiplication process (creating 5x original data)...');
    
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
    
    DBMS_OUTPUT.PUT_LINE('Original record counts:');
    DBMS_OUTPUT.PUT_LINE('  DEMOGRAPHIC: ' || v_original_demographic);
    DBMS_OUTPUT.PUT_LINE('  DEMOGRAPHIC_HISTORY: ' || v_original_demographic_history);
    DBMS_OUTPUT.PUT_LINE('  BENEFICIARY_CONTACT: ' || v_original_beneficiary_contact);
    DBMS_OUTPUT.PUT_LINE('  BENEFICIARY: ' || v_original_beneficiary);
    DBMS_OUTPUT.PUT_LINE('  PAY_PROFIT: ' || v_original_pay_profit);
    DBMS_OUTPUT.PUT_LINE('  PROFIT_DETAIL: ' || v_original_profit_detail);
    
    -- Loop 4 times to create 4 additional copies (original + 4 = 5x total)
    FOR v_iteration IN 1..4 LOOP
        -- Set iteration prefix: A, B, C, D for iterations 1, 2, 3, 4
        v_iteration_prefix := CHR(64 + v_iteration); -- 64 + 1 = 65 (A), 64 + 2 = 66 (B), etc.
        
        DBMS_OUTPUT.PUT_LINE('');
        DBMS_OUTPUT.PUT_LINE('=== CREATING COPY SET #' || v_iteration || ' (Prefix: ' || v_iteration_prefix || ') ===');
        
        -- Get current maximum values for key fields (updated each iteration)
        SELECT NVL(MAX(ORACLE_HCM_ID), 0) INTO v_max_oracle_hcm_id FROM DEMOGRAPHIC;
        SELECT NVL(MAX(BADGE_NUMBER), 0) INTO v_max_badge_number FROM DEMOGRAPHIC;
        SELECT NVL(MAX(ID), 0) INTO v_max_demographic_id FROM DEMOGRAPHIC;
        SELECT NVL(MAX(ID), 0) INTO v_max_beneficiary_contact_id FROM BENEFICIARY_CONTACT;
        SELECT NVL(MAX(ID), 0) INTO v_max_beneficiary_id FROM BENEFICIARY;
        
        -- 1. COPY DEMOGRAPHIC TABLE
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
            EMPLOYMENT_STATUS_ID
        )
        SELECT 
            ORACLE_HCM_ID + (v_iteration * 20000) AS ORACLE_HCM_ID, -- Safe offset: 20K per iteration (max 80K)
            SSN + (v_iteration * 1000000) AS SSN, -- 1M increments per iteration (max 4M)
            BADGE_NUMBER + (v_iteration * 20000) AS BADGE_NUMBER, -- 20K badge number increment
            v_iteration_prefix || FULL_NAME AS FULL_NAME,
            v_iteration_prefix || LAST_NAME AS LAST_NAME,
            v_iteration_prefix || FIRST_NAME AS FIRST_NAME,
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
            -- Map to the new demographic records created above
            (SELECT d2.ID 
             FROM DEMOGRAPHIC d2 
             WHERE d2.ORACLE_HCM_ID = dh.ORACLE_HCM_ID + (v_iteration * 20000)) AS DEMOGRAPHIC_ID,
            dh.VALID_FROM,
            dh.VALID_TO,
            dh.ORACLE_HCM_ID + (v_iteration * 20000) AS ORACLE_HCM_ID,
            dh.BADGE_NUMBER + (v_iteration * 20000) AS BADGE_NUMBER,
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
            WHERE d.ORACLE_HCM_ID = dh.ORACLE_HCM_ID + (v_iteration * 20000)
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
            SSN + (v_iteration * 1000000) AS SSN, -- Offset SSN by iteration (1M increments)
            v_iteration_prefix || FIRST_NAME AS FIRST_NAME,
            v_iteration_prefix || LAST_NAME AS LAST_NAME,
            v_iteration_prefix || FULL_NAME AS FULL_NAME,
            DATE_OF_BIRTH,
            STREET,
            CITY,
            STATE,
            POSTAL_CODE,
            PHONE_NUMBER,
            MOBILE_NUMBER,
            EMAIL_ADDRESS
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
            KIND_ID,
            PERCENT
        )
        SELECT 
            b.PSN_SUFFIX + (v_iteration * 20000) AS PSN_SUFFIX, -- Safe increment per iteration
            b.BADGE_NUMBER + (v_iteration * 20000) AS BADGE_NUMBER, -- Match badge number increments
            -- Map to new demographic ID
            (SELECT d2.ID 
             FROM DEMOGRAPHIC d2 
             WHERE d2.BADGE_NUMBER = b.BADGE_NUMBER + (v_iteration * 20000)) AS DEMOGRAPHIC_ID,
            -- Map to new beneficiary contact ID  
            (SELECT bc2.ID 
             FROM BENEFICIARY_CONTACT bc2 
             WHERE bc2.SSN = (SELECT bc.SSN + (v_iteration * 1000000) FROM BENEFICIARY_CONTACT bc WHERE bc.ID = b.BENEFICIARY_CONTACT_ID)) AS BENEFICIARY_CONTACT_ID,
            b.RELATIONSHIP,
            b.KIND_ID,
            b.PERCENT
        FROM BENEFICIARY b
        WHERE b.ID <= v_original_beneficiary -- Only copy original records
        AND EXISTS (
            SELECT 1 FROM DEMOGRAPHIC d 
            WHERE d.BADGE_NUMBER = b.BADGE_NUMBER + (v_iteration * 20000)
        )
        AND EXISTS (
            SELECT 1 FROM BENEFICIARY_CONTACT bc2 
            WHERE bc2.SSN = (SELECT bc.SSN + (v_iteration * 1000000) FROM BENEFICIARY_CONTACT bc WHERE bc.ID = b.BENEFICIARY_CONTACT_ID)
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
            -- Map to new demographic ID using badge number relationship
            (SELECT d2.ID 
             FROM DEMOGRAPHIC d2, DEMOGRAPHIC d1 
             WHERE d1.ID = pp.DEMOGRAPHIC_ID 
             AND d2.BADGE_NUMBER = d1.BADGE_NUMBER + (v_iteration * 20000)) AS DEMOGRAPHIC_ID,
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
            AND d2.BADGE_NUMBER = d1.BADGE_NUMBER + (v_iteration * 20000)
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
            pd.SSN + (v_iteration * 1000000) AS SSN, -- Offset SSN by iteration (1M increments)
            pd.DISTRIBUTION_SEQUENCE,
            pd.MONTH_TO_DATE,
            CASE 
                WHEN pd.REMARK IS NULL THEN v_iteration_prefix || '_DATA'
                ELSE v_iteration_prefix || '_' || pd.REMARK 
            END AS REMARK,
            pd.YEAR_TO_DATE
        FROM PROFIT_DETAIL pd
        WHERE pd.ID <= v_original_profit_detail -- Only copy original records
        AND EXISTS (
            SELECT 1 FROM DEMOGRAPHIC d 
            WHERE d.SSN = pd.SSN + (v_iteration * 1000000)
        );
        
        SELECT COUNT(*) INTO v_counter FROM PROFIT_DETAIL;
        DBMS_OUTPUT.PUT_LINE('PROFIT_DETAIL copy #' || v_iteration || ' complete. Total records: ' || v_counter);
        
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
    DBMS_OUTPUT.PUT_LINE('DEMOGRAPHIC: ' || v_counter || ' records (Original: ' || v_original_demographic || ', Expected: ' || (v_original_demographic * 5) || ')');
    
    SELECT COUNT(*) INTO v_counter FROM DEMOGRAPHIC_HISTORY;
    DBMS_OUTPUT.PUT_LINE('DEMOGRAPHIC_HISTORY: ' || v_counter || ' records (Original: ' || v_original_demographic_history || ', Expected: ' || (v_original_demographic_history * 5) || ')');
    
    SELECT COUNT(*) INTO v_counter FROM BENEFICIARY_CONTACT;
    DBMS_OUTPUT.PUT_LINE('BENEFICIARY_CONTACT: ' || v_counter || ' records (Original: ' || v_original_beneficiary_contact || ', Expected: ' || (v_original_beneficiary_contact * 5) || ')');
    
    SELECT COUNT(*) INTO v_counter FROM BENEFICIARY;
    DBMS_OUTPUT.PUT_LINE('BENEFICIARY: ' || v_counter || ' records (Original: ' || v_original_beneficiary || ', Expected: ' || (v_original_beneficiary * 5) || ')');
    
    SELECT COUNT(*) INTO v_counter FROM PAY_PROFIT;
    DBMS_OUTPUT.PUT_LINE('PAY_PROFIT: ' || v_counter || ' records (Original: ' || v_original_pay_profit || ', Expected: ' || (v_original_pay_profit * 5) || ')');
    
    SELECT COUNT(*) INTO v_counter FROM PROFIT_DETAIL;
    DBMS_OUTPUT.PUT_LINE('PROFIT_DETAIL: ' || v_counter || ' records (Original: ' || v_original_profit_detail || ', Expected: ' || (v_original_profit_detail * 5) || ')');
    
    DBMS_OUTPUT.PUT_LINE('');
    DBMS_OUTPUT.PUT_LINE('Data multiplication process completed successfully! Created 5x original data.');
    
    -- Commit all changes
    COMMIT;
    
EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        DBMS_OUTPUT.PUT_LINE('Error during data multiplication process: ' || SQLERRM);
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
