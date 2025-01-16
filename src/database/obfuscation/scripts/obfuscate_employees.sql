DECLARE
/*
    This file will obfuscate non-identifier data for all employeess, who are in the DEMOGRAPHCIS table.
    Those fields are: name, address, city, zip, date of hire, and date of birth. 
    
    It will also replace outlier cases
    for state and gender with common values.

    It will also create rows in EMPLOYEES_OBFUSCATED, a 
    framework table that keeps track of some new obfuscated 
    values so we can re-use them in case an employee is also 
    a beneficiary.
*/

    -- Variables for performance timing
    l_start_time NUMBER;
    l_completed_time NUMBER;
    l_elapsed_minutes NUMBER;
    l_elapsed_seconds NUMBER;
    
    -- Variables to store huge and small store numbers used to remove outliers
    TYPE STORES_NT IS TABLE OF NUMBER(3);
    L_HUGE_STORES  STORES_NT;
    L_TINY_STORES STORES_NT;
    

/* 
    This variable is the batch size for reading demographics records into the script. So, at 100, 
    they will all be added 100 at a time into the collection.
*/
    BATCH_READ_SIZE            PLS_INTEGER := 100;
 /*
    We need a cursor to read in all the DEMOGRAPHCS records while creating random values for each row. 
    Note that all but the first in the select are 'hard coded' values - in this case randomized values. 
    The randomized fields come from functions that are in another file, random_data_functions.sql,
    such as get_a_first_name()
    
    The next step is to create a nested table that has fields corresponding to those in the cursor.
    
    Then, we will open the cursor, and then bulk collect all the employee ids (DEM_BADGE) into 
    th nested table with randomzed data.
    
    The final step is do do a bulk update with the FORALL statement, sending the contents of the 
    nested table to the database for changes to all those rows.
*/
    CURSOR GET_DEMOGRAPHICS_INFO_CUR IS
    SELECT
        DEMO.DEM_BADGE,
        DEMO.DEM_SSN,
        ' ' AS PY_NAM,
        ' ' AS PY_LNAME,
        ' ' AS PY_FNAME,
        ' ' AS PY_MNAME,
        DEMO.PY_STOR AS PY_STOR,
        NULL AS PY_DP,
        NULL AS PY_CLA,
        ' ' AS PY_ADD,
        ' ' AS PY_ADD2,
        ' ' AS PY_CITY,
        DEMO.PY_STATE,
        0 AS PY_ZIP,
        DEMO.PY_DOB,
        NULL AS PY_FUL,
        NULL AS PY_FREQ,
        NULL AS PY_TYPE,
        NULL AS PY_SCOD,
        DEMO.PY_HIRE_DT AS PY_HIRE_DT,
        DEMO.PY_FULL_DT AS PY_FULL_DT,
        DEMO.PY_REHIRE_DT AS PY_REHIRE_DT,
        DEMO.PY_TERM_DT AS PY_TERM_DT,
        DEMO.PY_TERM AS PY_TERM,
        NULL AS PY_ASSIGN_ID,
        '' AS PY_ASSIGN_DESC,
        NULL AS PY_NEW_EMP,
        ' ' AS PY_GENDER,
        NULL AS PY_EMP_TELNO,
        DEMO.PY_SHOUR AS PY_SHOUR,
        0 AS PY_SET_PWD,
        NULL AS PY_SET_PWD_DT,
        DEMO.PY_CLASS_DT AS PY_CLASS_DT,
        ' ' AS PY_GUID
        
        FROM DEMOGRAPHICS DEMO;
        
-- Our Nested Table type
        TYPE BADGE_NUMBERS_NT IS TABLE OF DEMOGRAPHICS%ROWTYPE INDEX BY PLS_INTEGER;
        L_BADGE_NUMBERS_NT BADGE_NUMBERS_NT;
 -- Number of records
        NUM_RECORDS NUMBER := 0;
        L_SQL_ERROR NUMBER;
        L_SQL_ERRM VARCHAR2(512);
        
        L_ERROR_COUNT NUMBER;
        
        L_HUGE_STORE_REPLACEMENT NUMBER;
        L_FIRST_NAME VARCHAR(30);
        L_MIDDLE_NAME VARCHAR(25);
        L_LAST_NAME VARCHAR(30);
        L_FULL_NAME VARCHAR(40);

        
BEGIN 
        DBMS_OUTPUT.PUT_LINE(chr(10) || '- - - Begin obfuscation of non SSN/PSN employee data - - -');
        l_start_time := DBMS_UTILITY.get_time;
        DBMS_OUTPUT.PUT_LINE('Randomizing employee data fields...');
        L_BADGE_NUMBERS_NT := BADGE_NUMBERS_NT();
        
        L_HUGE_STORES := STORES_NT();
        L_TINY_STORES := STORES_NT();

        -- We need to read in the lists of small and huge stores
        SELECT HUGE.STORE_NUMBER BULK COLLECT INTO L_HUGE_STORES FROM HUGE_STORES HUGE;
        SELECT TINY.STORE_NUMBER BULK COLLECT INTO L_TINY_STORES FROM TINY_STORES TINY;

        
        -- Doing another block to catch exceptions
        BEGIN
            
        
            OPEN GET_DEMOGRAPHICS_INFO_CUR;
            LOOP FETCH GET_DEMOGRAPHICS_INFO_CUR BULK COLLECT INTO L_BADGE_NUMBERS_NT LIMIT BATCH_READ_SIZE;
            EXIT WHEN L_BADGE_NUMBERS_NT.COUNT = 0;  
            
            -- Now we need to add in correct fields, dates and state zip
            
            FOR EMP_INDEX IN 1..L_BADGE_NUMBERS_NT.COUNT
            LOOP
            L_FIRST_NAME := get_a_first_name();
            L_MIDDLE_NAME := get_a_middle_name();
            L_LAST_NAME := get_a_last_name();
            L_FULL_NAME := L_LAST_NAME || ', ' || L_FIRST_NAME;
            L_BADGE_NUMBERS_NT(EMP_INDEX).PY_FNAME := L_FIRST_NAME;
            L_BADGE_NUMBERS_NT(EMP_INDEX).PY_MNAME := L_MIDDLE_NAME;
            L_BADGE_NUMBERS_NT(EMP_INDEX).PY_LNAME := L_LAST_NAME;
            L_BADGE_NUMBERS_NT(EMP_INDEX).PY_NAM := L_FULL_NAME;
            L_BADGE_NUMBERS_NT(EMP_INDEX).PY_ADD := get_an_address();
            L_BADGE_NUMBERS_NT(EMP_INDEX).PY_ADD2 := GET_SPACES_FOR_COLUMN('DEMOGRAPHICS', 'PY_ADD2');
            L_BADGE_NUMBERS_NT(EMP_INDEX).PY_ASSIGN_DESC := GET_SPACES_FOR_COLUMN('DEMOGRAPHICS', 'PY_ASSIGN_DESC');
            L_BADGE_NUMBERS_NT(EMP_INDEX).PY_CITY := get_a_city();

            L_BADGE_NUMBERS_NT(EMP_INDEX).PY_DOB := ADJUST_DATE(L_BADGE_NUMBERS_NT(EMP_INDEX).PY_DOB);
            L_BADGE_NUMBERS_NT(EMP_INDEX).PY_HIRE_DT := ADJUST_DATE(L_BADGE_NUMBERS_NT(EMP_INDEX).PY_HIRE_DT);
            L_BADGE_NUMBERS_NT(EMP_INDEX).PY_REHIRE_DT := ADJUST_DATE(L_BADGE_NUMBERS_NT(EMP_INDEX).PY_REHIRE_DT);
            L_BADGE_NUMBERS_NT(EMP_INDEX).PY_FULL_DT := ADJUST_DATE(L_BADGE_NUMBERS_NT(EMP_INDEX).PY_FULL_DT);
            L_BADGE_NUMBERS_NT(EMP_INDEX).PY_TERM_DT := ADJUST_DATE(L_BADGE_NUMBERS_NT(EMP_INDEX).PY_TERM_DT);
            L_BADGE_NUMBERS_NT(EMP_INDEX).PY_CLASS_DT := ADJUST_DATE(L_BADGE_NUMBERS_NT(EMP_INDEX).PY_CLASS_DT);
            L_BADGE_NUMBERS_NT(EMP_INDEX).PY_ZIP := get_zip_for_state(L_BADGE_NUMBERS_NT(EMP_INDEX).PY_STATE);
            L_BADGE_NUMBERS_NT(EMP_INDEX).PY_EMP_TELNO := get_a_phone_number();

            L_BADGE_NUMBERS_NT(EMP_INDEX).PY_SHOUR := get_hours();
            

            
            IF (L_BADGE_NUMBERS_NT(EMP_INDEX).PY_STOR MEMBER OF L_TINY_STORES) THEN
                -- We now need to reassign to one of the three largest stores
                L_HUGE_STORE_REPLACEMENT := FLOOR(DBMS_RANDOM.VALUE(1, L_HUGE_STORES.COUNT +1));
                L_BADGE_NUMBERS_NT(EMP_INDEX).PY_STOR := L_HUGE_STORES(L_HUGE_STORE_REPLACEMENT);
                
            END IF;

            END LOOP;
            
            
            FORALL L_INDEX IN 1 .. L_BADGE_NUMBERS_NT.COUNT SAVE EXCEPTIONS
                UPDATE DEMOGRAPHICS 
                SET 
                    PY_FNAME = L_BADGE_NUMBERS_NT(L_INDEX).PY_FNAME,
                    PY_MNAME = L_BADGE_NUMBERS_NT(L_INDEX).PY_MNAME, 
                    PY_LNAME = L_BADGE_NUMBERS_NT(L_INDEX).PY_LNAME,
                    PY_NAM = L_BADGE_NUMBERS_NT(L_INDEX).PY_NAM, 
                    PY_ADD = L_BADGE_NUMBERS_NT(L_INDEX).PY_ADD,
                    PY_ADD2 = L_BADGE_NUMBERS_NT(L_INDEX).PY_ADD2,
                    PY_CITY = L_BADGE_NUMBERS_NT(L_INDEX).PY_CITY,
                    PY_ZIP = L_BADGE_NUMBERS_NT(L_INDEX).PY_ZIP,
                    PY_DOB = L_BADGE_NUMBERS_NT(L_INDEX).PY_DOB,
                    PY_HIRE_DT = L_BADGE_NUMBERS_NT(L_INDEX).PY_HIRE_DT,
                    PY_FULL_DT = L_BADGE_NUMBERS_NT(L_INDEX).PY_FULL_DT,
                    PY_REHIRE_DT = L_BADGE_NUMBERS_NT(L_INDEX).PY_REHIRE_DT,
                    PY_TERM_DT = L_BADGE_NUMBERS_NT(L_INDEX).PY_TERM_DT,
                    PY_TERM = L_BADGE_NUMBERS_NT(L_INDEX).PY_TERM,
                    PY_EMP_TELNO = L_BADGE_NUMBERS_NT(L_INDEX).PY_EMP_TELNO,
                    PY_STOR = L_BADGE_NUMBERS_NT(L_INDEX).PY_STOR,
                    PY_ASSIGN_ID = 654321,
                    PY_ASSIGN_DESC = L_BADGE_NUMBERS_NT(L_INDEX).PY_ASSIGN_DESC,
                    PY_STATE = L_BADGE_NUMBERS_NT(L_INDEX).PY_STATE,
                    PY_SHOUR = L_BADGE_NUMBERS_NT(L_INDEX).PY_SHOUR,
                    PY_SET_PWD = L_BADGE_NUMBERS_NT(L_INDEX).PY_SET_PWD,
                    PY_SET_PWD_DT = L_BADGE_NUMBERS_NT(L_INDEX).PY_SET_PWD_DT,
                    PY_CLASS_DT = L_BADGE_NUMBERS_NT(L_INDEX).PY_CLASS_DT,
                    PY_GUID = L_BADGE_NUMBERS_NT(L_INDEX).PY_GUID
                WHERE
                    DEM_BADGE = L_BADGE_NUMBERS_NT(L_INDEX).DEM_BADGE;


            -- We need to also put some fields in the EMPLOYEES_OBFUSCATED 
            -- table because we may need them when an employee is also a beneficiary

            FORALL L_INDEX IN 1 .. L_BADGE_NUMBERS_NT.COUNT SAVE EXCEPTIONS
                INSERT INTO EMPLOYEES_OBFUSCATED (
                        DEM_SSN,
                        PY_STATE,
                        PY_ZIP,
                        PY_DOB,
                        PY_CITY,
                        PY_ADD,
                        PY_ADD2,
                        PY_NAM
                ) VALUES (
                    L_BADGE_NUMBERS_NT(L_INDEX).DEM_SSN,
                    L_BADGE_NUMBERS_NT(L_INDEX).PY_STATE,
                    L_BADGE_NUMBERS_NT(L_INDEX).PY_ZIP,
                    L_BADGE_NUMBERS_NT(L_INDEX).PY_DOB,
                    L_BADGE_NUMBERS_NT(L_INDEX).PY_CITY,
                    L_BADGE_NUMBERS_NT(L_INDEX).PY_ADD,
                    GET_SPACES_FOR_COLUMN('DEMOGRAPHICS', 'PY_ADD2'),
                    SUBSTR(L_BADGE_NUMBERS_NT(L_INDEX).PY_NAM, 1, 40)    
                );

            END LOOP;
    
            CLOSE GET_DEMOGRAPHICS_INFO_CUR;
            
            
        EXCEPTION
            WHEN OTHERS
            THEN
                DBMS_OUTPUT.put_line ('ERROR OCCURED');
                ROLLBACK;
                    
        END;
        
        DBMS_OUTPUT.PUT_LINE('...done.');
        DBMS_OUTPUT.PUT_LINE('Eliminating outlier data...');

        -- Now we need to update outlier cases
        UPDATE DEMOGRAPHICS
        SET PY_STATE = 'MA'
        WHERE PY_STATE NOT IN ('MA', 'NH', 'RI', 'ME', 'FL');
        
        UPDATE DEMOGRAPHICS
        SET PY_GENDER = 'F'
        WHERE PY_GENDER NOT IN ('F', 'M');
        
        -- Change second address field, if there, to generic
        UPDATE DEMOGRAPHICS
        SET PY_ADD2 = 'Unit 2'
        WHERE PY_ADD2 NOT IN (NULL, ' ');
        
        DBMS_OUTPUT.PUT_LINE('...done.');
        
        -- This is the end of DEMOGRAPHICS field changes
        COMMIT;
        
        
        
        DBMS_OUTPUT.PUT_LINE('- - - Employee non-SSN/PSN data field obfuscation completed successfully. - - -');
        
        l_completed_time := DBMS_UTILITY.get_time;
        l_elapsed_seconds := (l_completed_time - l_start_time) / 100;
        l_elapsed_minutes := trunc(l_elapsed_seconds / 60, 0);
        
        dbms_output.put_line(chr(10) || '[TIME] Employee Field Obfuscation duration: ' 
        || l_elapsed_minutes || ' minutes + ' || mod(l_elapsed_seconds, 60) || ' seconds.'); 


EXCEPTION
    WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('error in demographics field obfuscation '
                             || SQLCODE);
         DBMS_OUTPUT.PUT_LINE('message '
         || SQLERRM);
         
        INSERT INTO log_messages (CALLER,  SQL_ERROR_CODE, ERROR_MESSAGE, CREATED_ON) 
            VALUES ('Obfuscate data fields main', L_SQL_ERROR, L_SQL_ERRM, sysdate); 
                             
END;



