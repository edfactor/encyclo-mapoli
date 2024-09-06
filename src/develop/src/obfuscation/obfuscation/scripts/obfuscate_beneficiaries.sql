DECLARE
    /*
    This file will obfuscate non-identifier data for all beneficiaries, who are in the PAYBEN table.
    Those fields are: name, address, city, zip, and date of birth.

    NOTE: We also have to access a table, EMPLOYEES_OBFUSCATED, to know if a 
    beneficiary we have is also an employee. If so, the beneficiary must use
    some values from the already obfuscated employee instead of creating new
    ones. These are name, address, date of birth.

    */


    /* 
    This variable is the batch size for reading beneficiary records into the script. So, at 100, 
    they will all be added 100 at a time into the collection.
    */
    BATCH_READ_SIZE            PLS_INTEGER := 100; 

    /*
    We need a cursor to read in all the PAYBEN records while creating random values for each row. 
    Note that all but the first in the select are 'hard coded' values - in this case randomized values. 
    The randomized fields come from functions that are in another file, such as get_a_first_name()

    The next step is to create a nested table that has fields corresponding to those in the cursor.

    Then, we will open the cursor, and then bulk collect all the PAYBEN PSNs into 
    the nested table with randomzed data.

    The final step is do do a bulk update with the FORALL statement, sending the contents of the 
    nested table to the database for changes to all those rows.
    */
    CURSOR GET_PAYBEN_INFO_CUR IS
    SELECT
        PAYBEN.PYBEN_PSN, 
        PAYBEN.PYBEN_PAYSSN AS PYBEN_PAYSSN,
        GET_SPACES_FOR_COLUMN('PAYBEN','PYBEN_TYPE') AS PYBEN_TYPE,
        NULL AS PYBEN_PERCENT,
        GET_SPACES_FOR_COLUMN('PAYBEN','PYBEN_NAME') AS PYBEN_NAME,
        GET_SPACES_FOR_COLUMN('PAYBEN', 'PYBEN_ADD') AS PYBEN_ADD,
        GET_SPACES_FOR_COLUMN('PAYBEN', 'PYBEN_CITY') AS PYBEN_CITY,
        PAYBEN.PYBEN_STATE AS PYBEN_STATE,
        0 AS PYBEN_ZIP,
        GET_SPACES_FOR_COLUMN('PAYBEN','PYBEN_RELATION') AS PYBEN_RELATION,
        PAYBEN.PYBEN_DOBIRTH AS PYBEN_DOBIRTH,
        NULL AS PYBEN_PSDISB,
        NULL AS PYBEN_PSAMT,
        NULL AS PYBEN_PROF_EARN,
        NULL AS PYBEN_PROF_EARN2

        FROM PAYBEN PAYBEN;

    /*
        This cursor is for reading records of already-obufuscated 
        employee data that we might need
    */
    CURSOR GET_EMPLOYEES_OBF_CUR IS
    SELECT
        DEM_SSN,
        PY_NAM,
        PY_ADD,
        PY_ADD2,
        PY_CITY,
        PY_STATE,
        PY_ZIP,
        PY_DOB

        FROM EMPLOYEES_OBFUSCATED;
    
        
        -- Our Nested Table types
        TYPE BENEFICIARY_IDS_NT IS TABLE OF PAYBEN%ROWTYPE INDEX BY PLS_INTEGER;
        L_BENEFICIARY_IDS_NT BENEFICIARY_IDS_NT;

        TYPE EMPLOYEES_OBFUSCATED_NT IS TABLE OF EMPLOYEES_OBFUSCATED%ROWTYPE INDEX BY PLS_INTEGER;
        L_EMPLOYEES_OBFUSCATED_NT EMPLOYEES_OBFUSCATED_NT;

        TYPE EMPLOYEE_SSN_LOOKUP IS TABLE OF NUMBER;
        L_EMPLOYEE_SSN_LOOKUP EMPLOYEE_SSN_LOOKUP;

        -- Number of records
        NUM_RECORDS NUMBER := 0;
        L_SQL_ERROR NUMBER;
        L_SQL_ERRM VARCHAR2(512);
        L_ERROR_COUNT NUMBER;

        -- Variables for performance timing
        l_start_time NUMBER;
        l_completed_time NUMBER;
        l_elapsed_minutes NUMBER;
        l_elapsed_seconds NUMBER;
        
        L_FIRST_NAME VARCHAR(30);
        L_MIDDLE_NAME VARCHAR(25);
        L_LAST_NAME VARCHAR(30);
        L_FULL_NAME VARCHAR(40);

    
BEGIN 
        l_start_time := DBMS_UTILITY.get_time;
        DBMS_OUTPUT.PUT_LINE(chr(10) || '- - - Begin obfuscation of non-SSN/PSN beneficiary data - - -');
        DBMS_OUTPUT.PUT_LINE('Obfuscating beneficiary data fields...');

        -- We need to load in the lookup table rows for a few items for all employees, in case
        -- we need to use those fields for an employee who is already a beneficiary

        -- We are going to get all the records from EMPLOYEES_OBFUSCATED one time, and store them
        -- in two different ways. First, we will store the full records in a nested table. 
        -- But we are also going to have a quick lookup array that is just the SSNs. Getting a hit
        -- for when an employee is a beneficiary is rare, so we don't want to have to do a loop
        -- through all records unless we know one is there. So we will do a MEMBER OF search 
        -- on a simple nested table and only if we get a hit do we do the loop of all records
        -- in a different nested table to get all the fields
        
        L_EMPLOYEES_OBFUSCATED_NT := EMPLOYEES_OBFUSCATED_NT();
        L_EMPLOYEE_SSN_LOOKUP := EMPLOYEE_SSN_LOOKUP();


        -- So first we read in all the entire records

        OPEN GET_EMPLOYEES_OBF_CUR;

        FETCH GET_EMPLOYEES_OBF_CUR BULK COLLECT INTO L_EMPLOYEES_OBFUSCATED_NT;

        CLOSE GET_EMPLOYEES_OBF_CUR;

        -- Then we bulk collect all of the DEM_SSNs
        SELECT DEM_SSN BULK COLLECT INTO L_EMPLOYEE_SSN_LOOKUP FROM EMPLOYEES_OBFUSCATED;
        
        L_BENEFICIARY_IDS_NT := BENEFICIARY_IDS_NT();

        OPEN GET_PAYBEN_INFO_CUR;
        LOOP FETCH GET_PAYBEN_INFO_CUR BULK COLLECT INTO L_BENEFICIARY_IDS_NT LIMIT BATCH_READ_SIZE;
        EXIT WHEN L_BENEFICIARY_IDS_NT.COUNT = 0;  
        
        -- Change the beneficiaries
        
        FOR BEN_INDEX IN 1..L_BENEFICIARY_IDS_NT.COUNT
            LOOP

            IF (L_BENEFICIARY_IDS_NT(BEN_INDEX).PYBEN_PAYSSN MEMBER OF L_EMPLOYEE_SSN_LOOKUP) THEN 
                -- So we loop through to find the whole record for the SSN
                FOR L_INDEX in 1..L_EMPLOYEES_OBFUSCATED_NT.COUNT
                LOOP
                    IF (L_EMPLOYEES_OBFUSCATED_NT(L_INDEX).DEM_SSN = L_BENEFICIARY_IDS_NT(BEN_INDEX).PYBEN_PAYSSN) THEN
                        L_BENEFICIARY_IDS_NT(BEN_INDEX).PYBEN_NAME := SUBSTR(L_EMPLOYEES_OBFUSCATED_NT(L_INDEX).PY_NAM, 1, 25);
                        L_BENEFICIARY_IDS_NT(BEN_INDEX).PYBEN_ADD := L_EMPLOYEES_OBFUSCATED_NT(L_INDEX).PY_ADD;
                        L_BENEFICIARY_IDS_NT(BEN_INDEX).PYBEN_CITY := L_EMPLOYEES_OBFUSCATED_NT(L_INDEX).PY_CITY;
                        L_BENEFICIARY_IDS_NT(BEN_INDEX).PYBEN_STATE := L_EMPLOYEES_OBFUSCATED_NT(L_INDEX).PY_STATE;
                        L_BENEFICIARY_IDS_NT(BEN_INDEX).PYBEN_ZIP := L_EMPLOYEES_OBFUSCATED_NT(L_INDEX).PY_ZIP;
                        L_BENEFICIARY_IDS_NT(BEN_INDEX).PYBEN_DOBIRTH := L_EMPLOYEES_OBFUSCATED_NT(L_INDEX).PY_DOB;
                        EXIT;
                    END IF;
                END LOOP;  
            ELSE
                L_FIRST_NAME := get_a_first_name();
                L_MIDDLE_NAME := get_a_middle_name();
                L_LAST_NAME := get_a_last_name();
                L_FULL_NAME := L_LAST_NAME || ', ' || L_FIRST_NAME;
                L_BENEFICIARY_IDS_NT(BEN_INDEX).PYBEN_NAME := SUBSTR(L_FULL_NAME, 1, 40);
                L_BENEFICIARY_IDS_NT(BEN_INDEX).PYBEN_ADD := get_an_address();
                L_BENEFICIARY_IDS_NT(BEN_INDEX).PYBEN_CITY := get_a_city();
                L_BENEFICIARY_IDS_NT(BEN_INDEX).PYBEN_ZIP := get_zip_for_state(L_BENEFICIARY_IDS_NT(BEN_INDEX).PYBEN_STATE);
                L_BENEFICIARY_IDS_NT(BEN_INDEX).PYBEN_DOBIRTH := ADJUST_DATE(L_BENEFICIARY_IDS_NT(BEN_INDEX).PYBEN_DOBIRTH);
            END IF;
                   
            END LOOP;
        
        FORALL L_INDEX IN 1 .. L_BENEFICIARY_IDS_NT.COUNT SAVE EXCEPTIONS
            UPDATE PAYBEN 
            SET 
                PYBEN_NAME = L_BENEFICIARY_IDS_NT(L_INDEX).PYBEN_NAME,
                PYBEN_ADD = L_BENEFICIARY_IDS_NT(L_INDEX).PYBEN_ADD,
                PYBEN_CITY = L_BENEFICIARY_IDS_NT(L_INDEX).PYBEN_CITY,
                PYBEN_ZIP = L_BENEFICIARY_IDS_NT(L_INDEX).PYBEN_ZIP,
                PYBEN_DOBIRTH = L_BENEFICIARY_IDS_NT(L_INDEX).PYBEN_DOBIRTH
            WHERE
                PYBEN_PSN = L_BENEFICIARY_IDS_NT(L_INDEX).PYBEN_PSN;
                
        END LOOP;

        CLOSE GET_PAYBEN_INFO_CUR;
        
      
        -- This is the end of PAYBEN field changes

        DBMS_OUTPUT.PUT_LINE('...done.');
        DBMS_OUTPUT.PUT_LINE('- - - Beneficiary non-SSN/PSN data field obfuscation completed successfully. - - -');
        
        l_completed_time := DBMS_UTILITY.get_time;
        l_elapsed_seconds := (l_completed_time - l_start_time) / 100;
        l_elapsed_minutes := trunc(l_elapsed_seconds / 60, 0);

        dbms_output.put_line(chr(10) || '[TIME] Beneficiary Field Obfuscation duration: ' 
        || l_elapsed_minutes || ' minutes + ' || mod(l_elapsed_seconds, 60) || ' seconds.'); 

EXCEPTION
            WHEN OTHERS
            THEN
            DBMS_OUTPUT.PUT_LINE('Error number: '
                                 || L_SQL_ERROR
                                 || ' Message: '
                                 || L_SQL_ERRM);
                DBMS_OUTPUT.put_line ('Rows inserted : ' || SQL%ROWCOUNT);
                l_error_count := SQL%BULK_EXCEPTIONS.count;
                DBMS_OUTPUT.put_line('Number of failures: ' || l_error_count);
                FOR indx IN 1 .. SQL%BULK_EXCEPTIONS.COUNT
                  LOOP
                     DBMS_OUTPUT.put_line (
                           'Error '
                        || indx
                        || ' occurred on index '
                        || SQL%BULK_EXCEPTIONS (indx).ERROR_INDEX
                        || ' attempting to update PAYBEN fields. ');
                     DBMS_OUTPUT.put_line (
                           'Oracle error is : '
                        || SQLERRM(-SQL%BULK_EXCEPTIONS(indx).ERROR_CODE));
                           
                  END LOOP;
                
                  ROLLBACK;
                             
END;