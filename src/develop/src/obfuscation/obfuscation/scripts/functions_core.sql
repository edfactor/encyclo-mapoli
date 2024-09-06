/* 
    This script obfuscates PSN and SSN while maintaining the relationships of records across the following tables:

    DEMOGRAPHICS
    PAYBEN
    PAYREL
    PROFDIST
    PROFIT_DIST_REQ
    PROFIT_DETAIL
    PROFIT_SHARE_CHECKS
    PAYPROFIT
    
    We use temporary tables to manage the tasks, one for SSNs and one for PSNs, that have the following in each row:
    --The original ID
    - The replacement ID
    - True/false columns indicating all instances of the original ID across tables
    - Address information for that ID that can be used in the PROFDIST table
    - A Done (true/false) flag to indicate if the ID has been changed across all tables
    
    The two tables are: SSN_TASKS and PSN_TASKS. These tasks are the source for the
    work in these functions.

    EXECUTION STRATEGY

    For SSNs and PSNs, we are going to have four functions for employee SSN and PSN and beneficiary SSN and PSN. 
    
    Each function will use the same strategy. 
    
    At the top of the function there will be a cursor that BULK COLLECTs the rows into a nested table that holds them all. (There will be batching via LIMIT)

    Then there will be a series of loops that are checks for each table to see if a change is needed for that record for each table type

    Then for each of those table-specific nested tables, there will be a FORALL loop
    that packages up all of the updates for that table into a batch.

    Then there is one more FORALL loop that updates the status flag for the rows in the batch in the tasks table to mark them as complete.

*/
CREATE OR REPLACE FUNCTION OBFUSCATE_EMPLOYEE_SSNS RETURN NUMBER IS

    -- This is for batch reading of tables into nested tables
    BATCH_READ_SIZE  PLS_INTEGER := 200;
    
    -- Our Nested Table type for overall tasks
    TYPE EMPLOYEE_SSN_Tasks_NT IS TABLE OF SSN_TASKS%ROWTYPE INDEX BY PLS_INTEGER;
    -- This is the intialized instance of our nested table
    L_EMPLOYEE_SSN_Tasks_NT EMPLOYEE_SSN_Tasks_NT;


    -- Nested table for general table updates
    TYPE SSN_CHANGE_REC IS RECORD ( 
        OLD_SSN NUMBER(9),
        NEW_SSN NUMBER(9)
    );

    -- Nested table for PROFDIST table updates
    TYPE PROFDIST_ADD_CHANGE_REC IS RECORD ( 
        OLD_SSN NUMBER(9),
        NEW_SSN NUMBER(9),
        EMP_NAME VARCHAR(25),
        PAY_NAME VARCHAR(25),
        NEW_ADDRESS VARCHAR(20),
        NEW_CITY VARCHAR(13),
        NEW_STATE VARCHAR(2),
        NEW_ZIP NUMBER(5)
    );

    TYPE PROFIT_SS_DETAIL_CHANGE_REC IS RECORD ( 
        OLD_SSN NUMBER(9),
        NEW_SSN NUMBER(9),
        NEW_NAME VARCHAR(25),
        NEW_ADDRESS VARCHAR(20),
        NEW_CITY VARCHAR(13),
        NEW_STATE VARCHAR(2),
        NEW_ZIP NUMBER(5)
    );

    TYPE CHECKS_CHANGE_REC IS RECORD ( 
        OLD_SSN NUMBER(9),
        NEW_SSN NUMBER(9),
        PAYABLE_NAME VARCHAR(25)
    );


    TYPE CHECKS_TASKS_NT IS TABLE OF CHECKS_CHANGE_REC; 
    
    TYPE PROFIT_SS_DETAIL_TASKS_NT IS TABLE OF PROFIT_SS_DETAIL_CHANGE_REC;
    
    TYPE SSN_TASKS_NT IS TABLE OF SSN_CHANGE_REC; 
    L_DEMO_CHANGES_NT SSN_TASKS_NT;

    TYPE PROFDIST_ADD_TASKS_NT IS TABLE OF PROFDIST_ADD_CHANGE_REC; --
    
    L_PROFDIST_ADD_TASKS_NT PROFDIST_ADD_TASKS_NT;

    -- This next line looks like a typo, but the checks recods has the
    -- name which we need to use
    L_PROFDIST_SSN_TASKS_NT CHECKS_TASKS_NT;
    L_SOC_SEC_SSN_TASKS_NT SSN_TASKS_NT;
    L_PAYPROFIT_SSN_TASKS_NT SSN_TASKS_NT;

    L_PROF_CHECKS_EMP_TASKS_NT CHECKS_TASKS_NT;
    L_PROF_CHECKS_OTHER_TASKS_NT CHECKS_TASKS_NT;

    L_PROFIT_DETAIL_TASKS_NT SSN_TASKS_NT;
    L_PROFIT_SS_DETAIL_TASKS_NT PROFIT_SS_DETAIL_TASKS_NT;

    L_PAYBEN_SSN_TASKS_NT SSN_TASKS_NT;
    L_PAYREL_SSN_TASKS_NT SSN_TASKS_NT;

    -- This is a hack! If the employee is a beneficiary,
    -- and is in either PAYBEN or PAYREL, we want to 
    -- set the beneficiary SSN task (which will also)
    -- be there, to DONE so that it does not process
    -- the row again, severing the connection to the 
    -- correct employee
    L_CANCEL_BEN_SSN_TASKS_NT SSN_TASKS_NT;


    
    L_HIGH_VOLUME_FLAG BOOLEAN;
    

 /* 
    We need to set up a cursor to read in all the fields
    for employee SSN values (identified by the beneficiary 
    SSN being null)
*/

    CURSOR GET_EMPLOYEE_SSN_TASKS_CUR IS
    SELECT
        *
    FROM SSN_TASKS SSN
    WHERE PYBEN_PAYSSN IS NULL AND DONE = 0;
    
--- Number of records
    L_SQL_ERROR NUMBER;
    L_SQL_ERRM VARCHAR2(512);
    
    L_ERROR_COUNT NUMBER := 0;
    
    -- This is used to track indexes when we extend the specialized 
    -- nested tables
    L_LAST NUMBER := 0;
    
    L_BATCH_NUMBER NUMBER := 0;

BEGIN

        -- Set this to TRUE if you are working with more than 40 million
        -- total records
        L_HIGH_VOLUME_FLAG := FALSE;
        
        L_EMPLOYEE_SSN_Tasks_NT := EMPLOYEE_SSN_Tasks_NT();
        
        L_DEMO_CHANGES_NT := SSN_TASKS_NT();
        L_PROFDIST_SSN_TASKS_NT := CHECKS_TASKS_NT();
        L_PAYPROFIT_SSN_TASKS_NT := SSN_TASKS_NT();
        L_PROFDIST_ADD_TASKS_NT := PROFDIST_ADD_TASKS_NT();
        L_PROF_CHECKS_EMP_TASKS_NT := CHECKS_TASKS_NT();
        L_PROF_CHECKS_OTHER_TASKS_NT := CHECKS_TASKS_NT();
        L_PROFIT_DETAIL_TASKS_NT := SSN_TASKS_NT();
        L_PROFIT_SS_DETAIL_TASKS_NT := PROFIT_SS_DETAIL_TASKS_NT();
        L_SOC_SEC_SSN_TASKS_NT := SSN_TASKS_NT();
        L_PAYBEN_SSN_TASKS_NT := SSN_TASKS_NT();
        L_PAYREL_SSN_TASKS_NT := SSN_TASKS_NT();

        L_CANCEL_BEN_SSN_TASKS_NT := SSN_TASKS_NT();
        
        
        OPEN GET_EMPLOYEE_SSN_TASKS_CUR;
        LOOP FETCH GET_EMPLOYEE_SSN_TASKS_CUR BULK COLLECT INTO L_EMPLOYEE_SSN_Tasks_NT LIMIT BATCH_READ_SIZE;
        EXIT WHEN L_EMPLOYEE_SSN_Tasks_NT.COUNT = 0;  
        L_BATCH_NUMBER := L_BATCH_NUMBER + 1;

        
        L_DEMO_CHANGES_NT.DELETE;
        L_PROFDIST_SSN_TASKS_NT.DELETE;
        L_PAYPROFIT_SSN_TASKS_NT.DELETE;
        L_PROFDIST_ADD_TASKS_NT.DELETE;
        L_PROF_CHECKS_EMP_TASKS_NT.DELETE;
        L_PROF_CHECKS_OTHER_TASKS_NT.DELETE;
        L_PROFIT_DETAIL_TASKS_NT.DELETE;
        L_PROFIT_SS_DETAIL_TASKS_NT.DELETE;
        L_SOC_SEC_SSN_TASKS_NT.DELETE;

        L_PAYBEN_SSN_TASKS_NT.DELETE;
        L_PAYREL_SSN_TASKS_NT.DELETE;

        L_CANCEL_BEN_SSN_TASKS_NT.DELETE;
        
        

        -- Now we need to fill a nested table for each target table
        -- that needs an SSN change
        L_DEMO_CHANGES_NT.EXTEND(L_EMPLOYEE_SSN_Tasks_NT.COUNT);

        FOR rec IN 1 .. L_EMPLOYEE_SSN_Tasks_NT.COUNT
        LOOP
            
            L_DEMO_CHANGES_NT(rec).OLD_SSN := L_EMPLOYEE_SSN_Tasks_NT(rec).DEMOGRAPHICS_SSN;
            L_DEMO_CHANGES_NT(rec).NEW_SSN :=  L_EMPLOYEE_SSN_Tasks_NT(rec).REPLACEMENT_SSN;

            -- Now let us do the PROF_DIST, where we need the address also
            
            IF (L_EMPLOYEE_SSN_Tasks_NT(rec).PROFDIST_SSN = 1 AND
                L_EMPLOYEE_SSN_Tasks_NT(rec).PROFDIST_PAYSSN = 1) THEN
                -- This means an employee is the addressee
                L_PROFDIST_ADD_TASKS_NT.EXTEND;
                L_LAST := L_PROFDIST_ADD_TASKS_NT.LAST;
                L_PROFDIST_ADD_TASKS_NT(L_LAST).OLD_SSN := L_EMPLOYEE_SSN_Tasks_NT(rec).DEMOGRAPHICS_SSN;
                L_PROFDIST_ADD_TASKS_NT(L_LAST).NEW_SSN := L_EMPLOYEE_SSN_Tasks_NT(rec).REPLACEMENT_SSN;
                L_PROFDIST_ADD_TASKS_NT(L_LAST).NEW_CITY := SUBSTR(L_EMPLOYEE_SSN_Tasks_NT(rec).NEW_CITY, 1, 13);
                L_PROFDIST_ADD_TASKS_NT(L_LAST).NEW_STATE := L_EMPLOYEE_SSN_Tasks_NT(rec).NEW_STATE;
                L_PROFDIST_ADD_TASKS_NT(L_LAST).NEW_ZIP := L_EMPLOYEE_SSN_Tasks_NT(rec).NEW_ZIP;
                L_PROFDIST_ADD_TASKS_NT(L_LAST).NEW_ADDRESS := SUBSTR(L_EMPLOYEE_SSN_Tasks_NT(rec).NEW_ADDRESS, 1, 20);
                L_PROFDIST_ADD_TASKS_NT(L_LAST).EMP_NAME := SUBSTR(L_EMPLOYEE_SSN_Tasks_NT(rec).EMP_NAME, 1, 25);
                L_PROFDIST_ADD_TASKS_NT(L_LAST).PAY_NAME := SUBSTR(L_EMPLOYEE_SSN_Tasks_NT(rec).EMP_NAME, 1, 25);
                
            
            ELSIF (L_EMPLOYEE_SSN_Tasks_NT(rec).PROFDIST_SSN = 1 AND
                L_EMPLOYEE_SSN_Tasks_NT(rec).PROFDIST_PAYSSN = 0) THEN
                -- This means an employee but not the addressee
                L_PROFDIST_SSN_TASKS_NT.EXTEND;
                L_LAST := L_PROFDIST_SSN_TASKS_NT.LAST;
                L_PROFDIST_SSN_TASKS_NT(L_LAST).OLD_SSN := L_EMPLOYEE_SSN_Tasks_NT(rec).DEMOGRAPHICS_SSN;
                L_PROFDIST_SSN_TASKS_NT(L_LAST).NEW_SSN := L_EMPLOYEE_SSN_Tasks_NT(rec).REPLACEMENT_SSN;
                L_PROFDIST_SSN_TASKS_NT(L_LAST).PAYABLE_NAME := SUBSTR(L_EMPLOYEE_SSN_Tasks_NT(rec).EMP_NAME, 1, 25);
                
            END IF;
            
            --- Now let us do PROFIT_SHARE_CHECKS for two fields
            IF (L_EMPLOYEE_SSN_Tasks_NT(rec).PROFIT_SHARE_CHECKS_EMPLOYEE_SSN = 1) THEN
                L_PROF_CHECKS_EMP_TASKS_NT.EXTEND;
                L_LAST := L_PROF_CHECKS_EMP_TASKS_NT.LAST;
                L_PROF_CHECKS_EMP_TASKS_NT(L_LAST).OLD_SSN := L_EMPLOYEE_SSN_Tasks_NT(rec).DEMOGRAPHICS_SSN;
                L_PROF_CHECKS_EMP_TASKS_NT(L_LAST).NEW_SSN := L_EMPLOYEE_SSN_Tasks_NT(rec).REPLACEMENT_SSN;
                L_PROF_CHECKS_EMP_TASKS_NT(L_LAST).PAYABLE_NAME := L_EMPLOYEE_SSN_Tasks_NT(rec).EMP_NAME;
            END IF;

            IF (L_EMPLOYEE_SSN_Tasks_NT(rec).PROFIT_SHARE_CHECKS_SSN_NUMBER = 1) THEN
                L_PROF_CHECKS_OTHER_TASKS_NT.EXTEND;
                L_LAST := L_PROF_CHECKS_OTHER_TASKS_NT.LAST;
                L_PROF_CHECKS_OTHER_TASKS_NT(L_LAST).OLD_SSN := L_EMPLOYEE_SSN_Tasks_NT(rec).DEMOGRAPHICS_SSN;
                L_PROF_CHECKS_OTHER_TASKS_NT(L_LAST).NEW_SSN := L_EMPLOYEE_SSN_Tasks_NT(rec).REPLACEMENT_SSN;
                L_PROF_CHECKS_OTHER_TASKS_NT(L_LAST).PAYABLE_NAME := L_EMPLOYEE_SSN_Tasks_NT(rec).EMP_NAME;
                
            END IF;

            --- SOC SEC REC
            IF (L_EMPLOYEE_SSN_Tasks_NT(rec).SOC_SEC_SSN = 1) THEN
                L_SOC_SEC_SSN_TASKS_NT.EXTEND;
                L_LAST := L_SOC_SEC_SSN_TASKS_NT.LAST;
                L_SOC_SEC_SSN_TASKS_NT(L_LAST).OLD_SSN := L_EMPLOYEE_SSN_Tasks_NT(rec).DEMOGRAPHICS_SSN;
                L_SOC_SEC_SSN_TASKS_NT(L_LAST).NEW_SSN := L_EMPLOYEE_SSN_Tasks_NT(rec).REPLACEMENT_SSN;
            END IF;

            -- IF PAYBEN OR PAYREL flag is set, we have an employee as beneficiary and need
            -- keep track of this so we can set the beneficiary SSN task to DONE proactively
            -- (This will be done in the FORALLs below)
            IF (L_EMPLOYEE_SSN_Tasks_NT(rec).PAYBEN_PYBEN_PAYSSN = 1) OR (L_EMPLOYEE_SSN_Tasks_NT(rec).PAYREL_PAYSSN = 1) THEN
                L_CANCEL_BEN_SSN_TASKS_NT.EXTEND;
                L_LAST := L_CANCEL_BEN_SSN_TASKS_NT.LAST;
                L_CANCEL_BEN_SSN_TASKS_NT(L_LAST).OLD_SSN := L_EMPLOYEE_SSN_Tasks_NT(rec).DEMOGRAPHICS_SSN;
            END IF;

            

            -- PAYBEN (for employees as beneficiaries)
            IF (L_EMPLOYEE_SSN_Tasks_NT(rec).PAYBEN_PYBEN_PAYSSN = 1) THEN
                L_PAYBEN_SSN_TASKS_NT.EXTEND;
                L_LAST := L_PAYBEN_SSN_TASKS_NT.LAST;
                L_PAYBEN_SSN_TASKS_NT(L_LAST).OLD_SSN := L_EMPLOYEE_SSN_Tasks_NT(rec).DEMOGRAPHICS_SSN;
                L_PAYBEN_SSN_TASKS_NT(L_LAST).NEW_SSN := L_EMPLOYEE_SSN_Tasks_NT(rec).REPLACEMENT_SSN;
            END IF;

            -- PAYREL (for employees as beneficiaries)
            IF (L_EMPLOYEE_SSN_Tasks_NT(rec).PAYREL_PAYSSN = 1) THEN
                L_PAYREL_SSN_TASKS_NT.EXTEND;
                L_LAST := L_PAYREL_SSN_TASKS_NT.LAST;
                L_PAYREL_SSN_TASKS_NT(L_LAST).OLD_SSN := L_EMPLOYEE_SSN_Tasks_NT(rec).DEMOGRAPHICS_SSN;
                L_PAYREL_SSN_TASKS_NT(L_LAST).NEW_SSN := L_EMPLOYEE_SSN_Tasks_NT(rec).REPLACEMENT_SSN;
            END IF;
            
            -- PAYPROFIT
            IF (L_EMPLOYEE_SSN_Tasks_NT(rec).PAYPROFIT_SSN = 1) THEN
                L_PAYPROFIT_SSN_TASKS_NT.EXTEND;
                L_LAST := L_PAYPROFIT_SSN_TASKS_NT.LAST;
                L_PAYPROFIT_SSN_TASKS_NT(L_LAST).OLD_SSN := L_EMPLOYEE_SSN_Tasks_NT(rec).DEMOGRAPHICS_SSN;
                L_PAYPROFIT_SSN_TASKS_NT(L_LAST).NEW_SSN := L_EMPLOYEE_SSN_Tasks_NT(rec).REPLACEMENT_SSN;
            END IF;

            -- Now one field in PROFIT DETAIL
            IF (L_EMPLOYEE_SSN_Tasks_NT(rec).PROFIT_DETAIL_PR_DET_S_SEC_NUMBER = 1) THEN
                L_PROFIT_DETAIL_TASKS_NT.EXTEND;
                L_LAST := L_PROFIT_DETAIL_TASKS_NT.LAST;
                L_PROFIT_DETAIL_TASKS_NT(L_LAST).OLD_SSN := L_EMPLOYEE_SSN_Tasks_NT(rec).DEMOGRAPHICS_SSN;
                L_PROFIT_DETAIL_TASKS_NT(L_LAST).NEW_SSN := L_EMPLOYEE_SSN_Tasks_NT(rec).REPLACEMENT_SSN;
            END IF;

            -- Now SSN and pull-through stuff in PROFIT SS DETAIL
            IF (L_EMPLOYEE_SSN_Tasks_NT(rec).PR_SS_D_S_SEC_NUMBER = 1) THEN
                L_PROFIT_SS_DETAIL_TASKS_NT.EXTEND;
                L_LAST := L_PROFIT_SS_DETAIL_TASKS_NT.LAST;
                L_PROFIT_SS_DETAIL_TASKS_NT(L_LAST).OLD_SSN := L_EMPLOYEE_SSN_Tasks_NT(rec).DEMOGRAPHICS_SSN;
                L_PROFIT_SS_DETAIL_TASKS_NT(L_LAST).NEW_SSN := L_EMPLOYEE_SSN_Tasks_NT(rec).REPLACEMENT_SSN;
                L_PROFIT_SS_DETAIL_TASKS_NT(L_LAST).NEW_NAME := SUBSTR(L_EMPLOYEE_SSN_Tasks_NT(rec).EMP_NAME, 1, 25);
                L_PROFIT_SS_DETAIL_TASKS_NT(L_LAST).NEW_ADDRESS := SUBSTR(L_EMPLOYEE_SSN_Tasks_NT(rec).NEW_ADDRESS, 1, 20);
                L_PROFIT_SS_DETAIL_TASKS_NT(L_LAST).NEW_CITY := SUBSTR(L_EMPLOYEE_SSN_Tasks_NT(rec).NEW_CITY, 1, 13);
                L_PROFIT_SS_DETAIL_TASKS_NT(L_LAST).NEW_STATE := L_EMPLOYEE_SSN_Tasks_NT(rec).NEW_STATE;
                L_PROFIT_SS_DETAIL_TASKS_NT(L_LAST).NEW_ZIP := L_EMPLOYEE_SSN_Tasks_NT(rec).NEW_ZIP;

            END IF;
                   
        END LOOP;
        
        -- Now we need to do the FORALL bulk updates
        
        
        FORALL L_INDEX IN 1 .. L_DEMO_CHANGES_NT.COUNT SAVE EXCEPTIONS
            UPDATE DEMOGRAPHICS 
            SET 
                DEM_SSN = L_DEMO_CHANGES_NT(L_INDEX).NEW_SSN    
            WHERE
                DEM_SSN = L_DEMO_CHANGES_NT(L_INDEX).OLD_SSN;

        -- We need a forall profdist with and without address
        -- This first one is where the employee is the addressee
        FORALL L_INDEX IN 1 .. L_PROFDIST_ADD_TASKS_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFDIST 
            SET 
                PROFDIST_SSN = L_PROFDIST_ADD_TASKS_NT(L_INDEX).NEW_SSN, 
                PROFDIST_PAYSSN = L_PROFDIST_ADD_TASKS_NT(L_INDEX).NEW_SSN,
                PROFDIST_PAYADDR1 = L_PROFDIST_ADD_TASKS_NT(L_INDEX).NEW_ADDRESS,
                PROFDIST_PAYCITY = L_PROFDIST_ADD_TASKS_NT(L_INDEX).NEW_CITY,
                PROFDIST_PAYSTATE = L_PROFDIST_ADD_TASKS_NT(L_INDEX).NEW_STATE,
                PROFDIST_PAYZIP1 = L_PROFDIST_ADD_TASKS_NT(L_INDEX).NEW_ZIP,   
                PROFDIST_PAYZIP2 = 0,  
                PROFDIST_EMPNAME = L_PROFDIST_ADD_TASKS_NT(L_INDEX).EMP_NAME,
                PROFDIST_PAYNAME = L_PROFDIST_ADD_TASKS_NT(L_INDEX).PAY_NAME
            WHERE
                PROFDIST_SSN = L_PROFDIST_ADD_TASKS_NT(L_INDEX).OLD_SSN;

        FORALL L_INDEX IN 1 .. L_PROFDIST_SSN_TASKS_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFDIST 
            SET 
                PROFDIST_SSN = L_PROFDIST_SSN_TASKS_NT(L_INDEX).NEW_SSN,
                PROFDIST_EMPNAME = L_PROFDIST_SSN_TASKS_NT(L_INDEX).PAYABLE_NAME 
            WHERE
                PROFDIST_SSN = L_PROFDIST_SSN_TASKS_NT(L_INDEX).OLD_SSN;

        -- Time for two PROFIT_SHARE_CHECKS updates
        
        FORALL L_INDEX IN 1 .. L_PROF_CHECKS_EMP_TASKS_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_SHARE_CHECKS 
            SET 
                EMPLOYEE_SSN = L_PROF_CHECKS_EMP_TASKS_NT(L_INDEX).NEW_SSN,
                PAYABLE_NAME = L_PROF_CHECKS_EMP_TASKS_NT(L_INDEX).PAYABLE_NAME
            WHERE
                EMPLOYEE_SSN = L_PROF_CHECKS_EMP_TASKS_NT(L_INDEX).OLD_SSN;
    
        FORALL L_INDEX IN 1 .. L_PROF_CHECKS_OTHER_TASKS_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_SHARE_CHECKS  
            SET 
                SSN_NUMBER = L_PROF_CHECKS_OTHER_TASKS_NT(L_INDEX).NEW_SSN,
                PAYABLE_NAME = L_PROF_CHECKS_OTHER_TASKS_NT(L_INDEX).PAYABLE_NAME
            WHERE
                SSN_NUMBER = L_PROF_CHECKS_OTHER_TASKS_NT(L_INDEX).OLD_SSN;

        -- PROFIT_DETAIL
        
        FORALL L_INDEX IN 1 .. L_PROFIT_DETAIL_TASKS_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_DETAIL 
            SET 
                PR_DET_S_SEC_NUMBER = L_PROFIT_DETAIL_TASKS_NT(L_INDEX).NEW_SSN    
            WHERE
                PR_DET_S_SEC_NUMBER = L_PROFIT_DETAIL_TASKS_NT(L_INDEX).OLD_SSN;

        -- PAYBEN
        
        FORALL L_INDEX IN 1 .. L_PAYBEN_SSN_TASKS_NT.COUNT SAVE EXCEPTIONS
            UPDATE PAYBEN 
            SET 
                PYBEN_PAYSSN = L_PAYBEN_SSN_TASKS_NT(L_INDEX).NEW_SSN    
            WHERE
                PYBEN_PAYSSN = L_PAYBEN_SSN_TASKS_NT(L_INDEX).OLD_SSN;

        -- PAYREL
        
        FORALL L_INDEX IN 1 .. L_PAYREL_SSN_TASKS_NT.COUNT SAVE EXCEPTIONS
            UPDATE PAYREL 
            SET 
                PYREL_PAYSSN = L_PAYREL_SSN_TASKS_NT(L_INDEX).NEW_SSN    
            WHERE
                PYREL_PAYSSN = L_PAYREL_SSN_TASKS_NT(L_INDEX).OLD_SSN;

        -- PROFIT_SS_DETAIL
        
        FORALL L_INDEX IN 1 .. L_PROFIT_SS_DETAIL_TASKS_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_SS_DETAIL 
            SET 
                PR_SS_D_S_SEC_NUMBER = L_PROFIT_SS_DETAIL_TASKS_NT(L_INDEX).NEW_SSN,
                PROFIT_SS_NAME = L_PROFIT_SS_DETAIL_TASKS_NT(L_INDEX).NEW_NAME,
                PROFIT_SS_ADDRESS = L_PROFIT_SS_DETAIL_TASKS_NT(L_INDEX).NEW_ADDRESS,
                PROFIT_SS_CITY = L_PROFIT_SS_DETAIL_TASKS_NT(L_INDEX).NEW_CITY,
                PROFIT_SS_STATE = L_PROFIT_SS_DETAIL_TASKS_NT(L_INDEX).NEW_STATE,
                PROFIT_SS_ZIP = L_PROFIT_SS_DETAIL_TASKS_NT(L_INDEX).NEW_ZIP
            WHERE
                PR_SS_D_S_SEC_NUMBER = L_PROFIT_SS_DETAIL_TASKS_NT(L_INDEX).OLD_SSN;

        -- PREVENT RELATED BENEFICIARY SSN tasks from happening. It does this by
        -- looking for SSN tasks rows for beneficiaries where the employee SSN
        -- we have matches that beneficiary SSN, and we set it to DONE = 1,
        -- as the query for beneficiary SSN obfuscation only gets rows where DONE
        -- is 0
        
        FORALL L_INDEX IN 1 .. L_CANCEL_BEN_SSN_TASKS_NT.COUNT SAVE EXCEPTIONS
            UPDATE SSN_TASKS 
            SET 
                DONE = 1    
            WHERE
                PYBEN_PAYSSN = L_CANCEL_BEN_SSN_TASKS_NT(L_INDEX).OLD_SSN;

        -- SOC_SEC_REC
        
        FORALL L_INDEX IN 1 .. L_SOC_SEC_SSN_TASKS_NT.COUNT SAVE EXCEPTIONS
            UPDATE SOC_SEC_REC 
            SET 
                SOC_SEC_NUMBER = L_SOC_SEC_SSN_TASKS_NT(L_INDEX).NEW_SSN    
            WHERE
                SOC_SEC_NUMBER = L_SOC_SEC_SSN_TASKS_NT(L_INDEX).OLD_SSN;

        -- PAYPROFIT
        FORALL L_INDEX IN 1 .. L_PAYPROFIT_SSN_TASKS_NT.COUNT SAVE EXCEPTIONS
            UPDATE PAYPROFIT 
            SET 
                PAYPROF_SSN = L_PAYPROFIT_SSN_TASKS_NT(L_INDEX).NEW_SSN    
            WHERE
                PAYPROF_SSN = L_PAYPROFIT_SSN_TASKS_NT(L_INDEX).OLD_SSN;
        
        -- And lastly, go through all of the SSN tasks rows we have completed in this batch and
        -- set flags on each row to done
        FORALL L_INDEX IN 1 .. L_DEMO_CHANGES_NT.COUNT SAVE EXCEPTIONS
            UPDATE SSN_TASKS 
            SET 
                DONE = 1
            WHERE
               DEMOGRAPHICS_SSN = L_DEMO_CHANGES_NT(L_INDEX).OLD_SSN;


        -- Our batch size is 200, and we need to commit every 100,000 employees 
        -- or else we will run out of 'undo' (rollback) space
        IF (L_HIGH_VOLUME_FLAG = TRUE AND (MOD((L_BATCH_NUMBER * 200), 100000) = 0)) THEN
            COMMIT;
            dbms_output.put_line('Committed 100,000 records...');
        END IF;

    END LOOP;
    
    RETURN 0;

EXCEPTION  
   WHEN OTHERS
   THEN  
    dbms_output.put_line('ERROR_STACK: ' || DBMS_UTILITY.FORMAT_ERROR_STACK);
    dbms_output.put_line('ERROR_BACKTRACE: ' || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE);
   
      DBMS_OUTPUT.put_line (SQLERRM);  
      DBMS_OUTPUT.put_line (  
         'Updated ' || SQL%ROWCOUNT || ' rows.');  
  
      FOR indx IN 1 .. SQL%BULK_EXCEPTIONS.COUNT  
      LOOP  
         DBMS_OUTPUT.put_line (  
               'Error '  
            || indx  
            || ' occurred on index '  
            || SQL%BULK_EXCEPTIONS (indx).ERROR_INDEX  
            );  
         DBMS_OUTPUT.put_line (  
               'Oracle error is '  
            || SQLERRM (  
                  -1 * SQL%BULK_EXCEPTIONS (indx).ERROR_CODE));  
      END LOOP;  
  
      ROLLBACK;  
END OBFUSCATE_EMPLOYEE_SSNS;
/

CREATE OR REPLACE FUNCTION OBFUSCATE_BENEFICIARY_SSNS RETURN NUMBER IS

 -- This is for batch reading of tables into nested tables
    BATCH_READ_SIZE  PLS_INTEGER := 200;

  -- Our Nested Table type for overall tasks
    TYPE BENEFICIARY_SSN_Tasks_NT IS TABLE OF SSN_TASKS%ROWTYPE INDEX BY PLS_INTEGER;
-- This is the intialized instance of our nested table
    L_BENEFICIARY_SSN_Tasks_NT BENEFICIARY_SSN_Tasks_NT;


    -- Nested table for generic SSN updates
    TYPE SSN_CHANGE_REC IS RECORD ( 
        OLD_SSN NUMBER(9),
        NEW_SSN NUMBER(9)
    );

    
    TYPE PROFDIST_ADD_CHANGE_REC IS RECORD ( 
        OLD_SSN NUMBER(9),
        NEW_SSN NUMBER(9),
        EMP_NAME VARCHAR(25),
        PAY_NAME VARCHAR(25),
        NEW_ADDRESS VARCHAR(20),
        NEW_CITY VARCHAR(13),
        NEW_STATE VARCHAR(2),
        NEW_ZIP NUMBER(5)
    );

    TYPE PROFIT_CHECKS_CHANGE_REC IS RECORD ( 
        OLD_SSN NUMBER(9),
        NEW_SSN NUMBER(9),
        PAY_NAME VARCHAR(30)
    );

    TYPE PROFIT_SS_DETAIL_CHANGE_REC IS RECORD ( 
        OLD_SSN NUMBER(9),
        NEW_SSN NUMBER(9),
        NEW_NAME VARCHAR(25),
        NEW_ADDRESS VARCHAR(20),
        NEW_CITY VARCHAR(13),
        NEW_STATE VARCHAR(2),
        NEW_ZIP NUMBER(5)
    );
    
    TYPE SSN_TASKS_NT IS TABLE OF SSN_CHANGE_REC; 
    L_PAYBEN_CHANGES_NT SSN_TASKS_NT;
    L_PYREL_CHANGES_NT SSN_TASKS_NT;
    L_PAYPROFIT_CHANGES_NT SSN_TASKS_NT;

    -- We need to NTs, one for address change, one not
    TYPE PROFDIST_ADD_TASKS_NT IS TABLE OF PROFDIST_ADD_CHANGE_REC; --
    L_PROFDIST_ADD_TASKS_NT PROFDIST_ADD_TASKS_NT;
    
    L_PROFDIST_SSN_TASKS_NT PROFDIST_ADD_TASKS_NT;
    L_SOC_SEC_SSN_TASKS_NT SSN_TASKS_NT;
    
    TYPE PROF_CHECKS_BEN_TASKS_NT IS TABLE OF PROFIT_CHECKS_CHANGE_REC; 
    L_PROF_CHECKS_BEN_TASKS_NT PROF_CHECKS_BEN_TASKS_NT;

    TYPE PROFIT_SS_DETAIL_TASKS_NT IS TABLE OF PROFIT_SS_DETAIL_CHANGE_REC;
    
    L_PROFIT_SS_DETAIL_TASKS_NT PROFIT_SS_DETAIL_TASKS_NT;
    L_PROFIT_DETAIL_TASKS_NT SSN_TASKS_NT;
    
    L_HIGH_VOLUME_FLAG BOOLEAN;
    

 /* 
    We need to set up a cursor to read in all the fields
    for BENEFICIARY SSN values (identified by the beneficiary 
    SSN being null)
*/

    CURSOR GET_BENEFICIARY_SSN_TASKS_CUR IS
    SELECT
        *
    FROM SSN_TASKS SSN
    WHERE PYBEN_PAYSSN IS NOT NULL AND DONE = 0;


--- Number of records
    L_SQL_ERROR NUMBER;
    L_SQL_ERRM VARCHAR2(512);
    
    L_ERROR_COUNT NUMBER;
    
    -- This is used to track indexes when we extend the specialized 
    -- nested tables
    L_LAST NUMBER := 0;
    
    L_BATCH_NUMBER NUMBER := 0;

BEGIN
    
    -- Set this to true for more than 500,000 beneficiaries
    L_HIGH_VOLUME_FLAG := FALSE;

    L_BENEFICIARY_SSN_Tasks_NT := BENEFICIARY_SSN_Tasks_NT();
    L_PAYBEN_CHANGES_NT := SSN_TASKS_NT();
    L_PYREL_CHANGES_NT := SSN_TASKS_NT();
    
    L_PROFDIST_ADD_TASKS_NT := PROFDIST_ADD_TASKS_NT();
    L_PROFDIST_SSN_TASKS_NT := PROFDIST_ADD_TASKS_NT();
    L_SOC_SEC_SSN_TASKS_NT := SSN_TASKS_NT();
    L_PAYPROFIT_CHANGES_NT := SSN_TASKS_NT();
    
    L_PROF_CHECKS_BEN_TASKS_NT := PROF_CHECKS_BEN_TASKS_NT();
    
    L_PROFIT_DETAIL_TASKS_NT := SSN_TASKS_NT();
    L_PROFIT_SS_DETAIL_TASKS_NT := PROFIT_SS_DETAIL_TASKS_NT();
    
    OPEN GET_BENEFICIARY_SSN_TASKS_CUR;
    LOOP FETCH GET_BENEFICIARY_SSN_TASKS_CUR BULK COLLECT INTO L_BENEFICIARY_SSN_Tasks_NT LIMIT BATCH_READ_SIZE;
    EXIT WHEN L_BENEFICIARY_SSN_Tasks_NT.COUNT = 0;  
    L_BATCH_NUMBER := L_BATCH_NUMBER + 1;
    

    -- These statements clear the collections between batches
    L_PAYBEN_CHANGES_NT.DELETE;
    L_PYREL_CHANGES_NT.DELETE;
    L_PROFDIST_ADD_TASKS_NT.DELETE;
    L_PROFDIST_SSN_TASKS_NT.DELETE;
    L_PROF_CHECKS_BEN_TASKS_NT.DELETE;
    L_PROFIT_DETAIL_TASKS_NT.DELETE;
    L_PROFIT_SS_DETAIL_TASKS_NT.DELETE;
    L_SOC_SEC_SSN_TASKS_NT.DELETE;
    L_PAYPROFIT_CHANGES_NT.DELETE;
    
    -- We know every record will have a change to the PAYBEN table,
    -- so we can extend that NT by the size of the whole thing
    L_PAYBEN_CHANGES_NT.EXTEND(L_BENEFICIARY_SSN_Tasks_NT.COUNT);

    FOR rec IN 1 .. L_BENEFICIARY_SSN_Tasks_NT.COUNT
    LOOP

        L_PAYBEN_CHANGES_NT(rec).OLD_SSN := L_BENEFICIARY_SSN_Tasks_NT(rec).PYBEN_PAYSSN;
        L_PAYBEN_CHANGES_NT(rec).NEW_SSN :=  L_BENEFICIARY_SSN_Tasks_NT(rec).REPLACEMENT_SSN;

            -- Now let us do the PROF_DIST, where we need the address also

            IF (L_BENEFICIARY_SSN_Tasks_NT(rec).PROFDIST_SSN = 0 AND
                L_BENEFICIARY_SSN_Tasks_NT(rec).PROFDIST_PAYSSN = 1) THEN
                -- This means an BENEFICIARY is the addressee
                L_PROFDIST_ADD_TASKS_NT.EXTEND;
                L_LAST := L_PROFDIST_ADD_TASKS_NT.LAST;
                L_PROFDIST_ADD_TASKS_NT(L_LAST).OLD_SSN := L_BENEFICIARY_SSN_Tasks_NT(rec).PYBEN_PAYSSN;
                L_PROFDIST_ADD_TASKS_NT(L_LAST).NEW_SSN := L_BENEFICIARY_SSN_Tasks_NT(rec).REPLACEMENT_SSN;
                L_PROFDIST_ADD_TASKS_NT(L_LAST).NEW_CITY := SUBSTR(L_BENEFICIARY_SSN_Tasks_NT(rec).NEW_CITY, 1, 13);
                L_PROFDIST_ADD_TASKS_NT(L_LAST).NEW_STATE := L_BENEFICIARY_SSN_Tasks_NT(rec).NEW_STATE;
                L_PROFDIST_ADD_TASKS_NT(L_LAST).NEW_ZIP := L_BENEFICIARY_SSN_Tasks_NT(rec).NEW_ZIP;
                L_PROFDIST_ADD_TASKS_NT(L_LAST).NEW_ADDRESS := SUBSTR(L_BENEFICIARY_SSN_Tasks_NT(rec).NEW_ADDRESS, 1, 20);
                L_PROFDIST_ADD_TASKS_NT(L_LAST).EMP_NAME := SUBSTR(L_BENEFICIARY_SSN_Tasks_NT(rec).EMP_NAME, 1, 25);
                L_PROFDIST_ADD_TASKS_NT(L_LAST).PAY_NAME := SUBSTR(L_BENEFICIARY_SSN_Tasks_NT(rec).BEN_NAME, 1, 25);
                
                
            
            ELSE 
                -- This means the BENEFICIARY is the employee and addressee
                L_PROFDIST_SSN_TASKS_NT.EXTEND;
                L_LAST := L_PROFDIST_SSN_TASKS_NT.LAST;
                L_PROFDIST_SSN_TASKS_NT(L_LAST).OLD_SSN := L_BENEFICIARY_SSN_Tasks_NT(rec).PYBEN_PAYSSN;
                L_PROFDIST_SSN_TASKS_NT(L_LAST).NEW_SSN := L_BENEFICIARY_SSN_Tasks_NT(rec).REPLACEMENT_SSN;

                L_PROFDIST_SSN_TASKS_NT(L_LAST).OLD_SSN := L_BENEFICIARY_SSN_Tasks_NT(rec).PYBEN_PAYSSN;
                L_PROFDIST_SSN_TASKS_NT(L_LAST).NEW_SSN := L_BENEFICIARY_SSN_Tasks_NT(rec).REPLACEMENT_SSN;
                L_PROFDIST_SSN_TASKS_NT(L_LAST).NEW_CITY := SUBSTR(L_BENEFICIARY_SSN_Tasks_NT(rec).NEW_CITY, 1, 13);
                L_PROFDIST_SSN_TASKS_NT(L_LAST).NEW_STATE := L_BENEFICIARY_SSN_Tasks_NT(rec).NEW_STATE;
                L_PROFDIST_SSN_TASKS_NT(L_LAST).NEW_ZIP := L_BENEFICIARY_SSN_Tasks_NT(rec).NEW_ZIP;
                L_PROFDIST_SSN_TASKS_NT(L_LAST).NEW_ADDRESS := SUBSTR(L_BENEFICIARY_SSN_Tasks_NT(rec).NEW_ADDRESS, 1, 20);
                L_PROFDIST_SSN_TASKS_NT(L_LAST).EMP_NAME := SUBSTR(L_BENEFICIARY_SSN_Tasks_NT(rec).EMP_NAME, 1, 25);
                L_PROFDIST_SSN_TASKS_NT(L_LAST).PAY_NAME := SUBSTR(L_BENEFICIARY_SSN_Tasks_NT(rec).BEN_NAME, 1, 25);


            END IF;

            --- SOC SEC REC
            IF (L_BENEFICIARY_SSN_Tasks_NT(rec).SOC_SEC_SSN = 1) THEN
                L_SOC_SEC_SSN_TASKS_NT.EXTEND;
                L_LAST := L_SOC_SEC_SSN_TASKS_NT.LAST;
                L_SOC_SEC_SSN_TASKS_NT(L_LAST).OLD_SSN := L_BENEFICIARY_SSN_Tasks_NT(rec).PYBEN_PAYSSN;
                L_SOC_SEC_SSN_TASKS_NT(L_LAST).NEW_SSN := L_BENEFICIARY_SSN_Tasks_NT(rec).REPLACEMENT_SSN;
            END IF;

            --- PYREL
            IF (L_BENEFICIARY_SSN_Tasks_NT(rec).PAYREL_PAYSSN = 1) THEN
                L_PYREL_CHANGES_NT.EXTEND;
                L_LAST := L_PYREL_CHANGES_NT.LAST;
                L_PYREL_CHANGES_NT(L_LAST).OLD_SSN := L_BENEFICIARY_SSN_Tasks_NT(rec).PYBEN_PAYSSN;
                L_PYREL_CHANGES_NT(L_LAST).NEW_SSN := L_BENEFICIARY_SSN_Tasks_NT(rec).REPLACEMENT_SSN;
            END IF;

            --- PAYPROFIT
            IF (L_BENEFICIARY_SSN_Tasks_NT(rec).PAYPROFIT_SSN = 1) THEN
                L_PAYPROFIT_CHANGES_NT.EXTEND;
                L_LAST := L_PAYPROFIT_CHANGES_NT.LAST;
                L_PAYPROFIT_CHANGES_NT(L_LAST).OLD_SSN := L_BENEFICIARY_SSN_Tasks_NT(rec).PYBEN_PAYSSN;
                L_PAYPROFIT_CHANGES_NT(L_LAST).NEW_SSN := L_BENEFICIARY_SSN_Tasks_NT(rec).REPLACEMENT_SSN;
            END IF;
            
            --- Now let us do PROFIT_SHARE_CHECKS for two fields
            IF (L_BENEFICIARY_SSN_Tasks_NT(rec).PROFIT_SHARE_CHECKS_SSN_NUMBER = 1) THEN
                L_PROF_CHECKS_BEN_TASKS_NT.EXTEND;
                L_LAST := L_PROF_CHECKS_BEN_TASKS_NT.LAST;
                L_PROF_CHECKS_BEN_TASKS_NT(L_LAST).OLD_SSN := L_BENEFICIARY_SSN_Tasks_NT(rec).PYBEN_PAYSSN;
                L_PROF_CHECKS_BEN_TASKS_NT(L_LAST).NEW_SSN := L_BENEFICIARY_SSN_Tasks_NT(rec).REPLACEMENT_SSN;
                L_PROF_CHECKS_BEN_TASKS_NT(L_LAST).PAY_NAME := L_BENEFICIARY_SSN_Tasks_NT(rec).BEN_NAME;

            END IF;

            -- Now one field in PROFIT DETAIL
            IF (L_BENEFICIARY_SSN_Tasks_NT(rec).PROFIT_DETAIL_PR_DET_S_SEC_NUMBER = 1) THEN
                
                L_PROFIT_DETAIL_TASKS_NT.EXTEND;
                L_LAST := L_PROFIT_DETAIL_TASKS_NT.LAST;
                L_PROFIT_DETAIL_TASKS_NT(L_LAST).OLD_SSN := L_BENEFICIARY_SSN_Tasks_NT(rec).PYBEN_PAYSSN;
                L_PROFIT_DETAIL_TASKS_NT(L_LAST).NEW_SSN := L_BENEFICIARY_SSN_Tasks_NT(rec).REPLACEMENT_SSN;
            END IF;

            -- One field in PROFIT_DETAIL
            IF (L_BENEFICIARY_SSN_Tasks_NT(rec).PR_SS_D_S_SEC_NUMBER = 1) THEN
                
                L_PROFIT_SS_DETAIL_TASKS_NT.EXTEND;
                L_LAST := L_PROFIT_SS_DETAIL_TASKS_NT.LAST;
                L_PROFIT_SS_DETAIL_TASKS_NT(L_LAST).OLD_SSN := L_BENEFICIARY_SSN_Tasks_NT(rec).PYBEN_PAYSSN;
                L_PROFIT_SS_DETAIL_TASKS_NT(L_LAST).NEW_SSN := L_BENEFICIARY_SSN_Tasks_NT(rec).REPLACEMENT_SSN;
                L_PROFIT_SS_DETAIL_TASKS_NT(L_LAST).NEW_NAME := SUBSTR(L_BENEFICIARY_SSN_Tasks_NT(rec).BEN_NAME, 1, 25);
                L_PROFIT_SS_DETAIL_TASKS_NT(L_LAST).NEW_ADDRESS := SUBSTR(L_BENEFICIARY_SSN_Tasks_NT(rec).NEW_ADDRESS, 1, 20);
                L_PROFIT_SS_DETAIL_TASKS_NT(L_LAST).NEW_CITY := SUBSTR(L_BENEFICIARY_SSN_Tasks_NT(rec).NEW_CITY, 1, 13);
                L_PROFIT_SS_DETAIL_TASKS_NT(L_LAST).NEW_STATE := L_BENEFICIARY_SSN_Tasks_NT(rec).NEW_STATE;
                L_PROFIT_SS_DETAIL_TASKS_NT(L_LAST).NEW_ZIP := L_BENEFICIARY_SSN_Tasks_NT(rec).NEW_ZIP;
            END IF;


                   
        END LOOP;
        
        -- Now we need to do the FORALL updates
        
        FORALL L_INDEX IN 1 .. L_PAYPROFIT_CHANGES_NT.COUNT SAVE EXCEPTIONS
            UPDATE PAYPROFIT 
            SET 
                PAYPROF_SSN = L_PAYPROFIT_CHANGES_NT(L_INDEX).NEW_SSN    
            WHERE
                PAYPROF_SSN = L_PAYPROFIT_CHANGES_NT(L_INDEX).OLD_SSN;

   
        FORALL L_INDEX IN 1 .. L_PAYBEN_CHANGES_NT.COUNT SAVE EXCEPTIONS
            UPDATE PAYBEN 
            SET 
                PYBEN_PAYSSN = L_PAYBEN_CHANGES_NT(L_INDEX).NEW_SSN    
            WHERE
                PYBEN_PAYSSN = L_PAYBEN_CHANGES_NT(L_INDEX).OLD_SSN;

        -- We need a loop for employee as beneficiary and then just beneficiary
        FORALL L_INDEX IN 1 .. L_PROFDIST_ADD_TASKS_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFDIST 
            SET 
                PROFDIST_SSN = L_PROFDIST_ADD_TASKS_NT(L_INDEX).NEW_SSN, 
                PROFDIST_PAYSSN = L_PROFDIST_ADD_TASKS_NT(L_INDEX).NEW_SSN,
                PROFDIST_PAYADDR1 = L_PROFDIST_ADD_TASKS_NT(L_INDEX).NEW_ADDRESS,
                PROFDIST_PAYCITY = L_PROFDIST_ADD_TASKS_NT(L_INDEX).NEW_CITY,
                PROFDIST_PAYSTATE = L_PROFDIST_ADD_TASKS_NT(L_INDEX).NEW_STATE,
                PROFDIST_PAYZIP1 = L_PROFDIST_ADD_TASKS_NT(L_INDEX).NEW_ZIP,   
                PROFDIST_PAYZIP2 = 0,  
                PROFDIST_EMPNAME = L_PROFDIST_ADD_TASKS_NT(L_INDEX).EMP_NAME,
                PROFDIST_PAYNAME = L_PROFDIST_ADD_TASKS_NT(L_INDEX).PAY_NAME
            WHERE
                PROFDIST_SSN = L_PROFDIST_ADD_TASKS_NT(L_INDEX).OLD_SSN;

        FORALL L_INDEX IN 1 .. L_PROFDIST_SSN_TASKS_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFDIST 
            SET 
                PROFDIST_PAYSSN = L_PROFDIST_SSN_TASKS_NT(L_INDEX).NEW_SSN,
                PROFDIST_PAYADDR1 = L_PROFDIST_SSN_TASKS_NT(L_INDEX).NEW_ADDRESS,
                PROFDIST_PAYCITY = L_PROFDIST_SSN_TASKS_NT(L_INDEX).NEW_CITY,
                PROFDIST_PAYSTATE = L_PROFDIST_SSN_TASKS_NT(L_INDEX).NEW_STATE,
                PROFDIST_PAYZIP1 = L_PROFDIST_SSN_TASKS_NT(L_INDEX).NEW_ZIP,   
                PROFDIST_PAYZIP2 = 0,  
                PROFDIST_EMPNAME = L_PROFDIST_SSN_TASKS_NT(L_INDEX).EMP_NAME,
                PROFDIST_PAYNAME = L_PROFDIST_SSN_TASKS_NT(L_INDEX).PAY_NAME    
            WHERE
                PROFDIST_PAYSSN = L_PROFDIST_SSN_TASKS_NT(L_INDEX).OLD_SSN;

        -- PROFIT_SHARE_CHECKS updates
        FORALL L_INDEX IN 1 .. L_PROF_CHECKS_BEN_TASKS_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_SHARE_CHECKS 
            SET 
                SSN_NUMBER = L_PROF_CHECKS_BEN_TASKS_NT(L_INDEX).NEW_SSN,
                PAYABLE_NAME = L_PROF_CHECKS_BEN_TASKS_NT(L_INDEX).PAY_NAME
            WHERE
                SSN_NUMBER = L_PROF_CHECKS_BEN_TASKS_NT(L_INDEX).OLD_SSN;

        -- SOC_SEC_REC
        
        FORALL L_INDEX IN 1 .. L_SOC_SEC_SSN_TASKS_NT.COUNT SAVE EXCEPTIONS
            UPDATE SOC_SEC_REC 
            SET 
                SOC_SEC_NUMBER = L_SOC_SEC_SSN_TASKS_NT(L_INDEX).NEW_SSN    
            WHERE
                SOC_SEC_NUMBER = L_SOC_SEC_SSN_TASKS_NT(L_INDEX).OLD_SSN;

        -- PAYREL SSN
        
        FORALL L_INDEX IN 1 .. L_PYREL_CHANGES_NT.COUNT SAVE EXCEPTIONS
            UPDATE PAYREL 
            SET 
                PYREL_PAYSSN = L_PYREL_CHANGES_NT(L_INDEX).NEW_SSN    
            WHERE
                PYREL_PAYSSN = L_PYREL_CHANGES_NT(L_INDEX).OLD_SSN;

        -- PROFIT_SS_DETAIL

        FORALL L_INDEX IN 1 .. L_PROFIT_SS_DETAIL_TASKS_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_SS_DETAIL 
            SET 
                PR_SS_D_S_SEC_NUMBER = L_PROFIT_SS_DETAIL_TASKS_NT(L_INDEX).NEW_SSN,
                PROFIT_SS_NAME = L_PROFIT_SS_DETAIL_TASKS_NT(L_INDEX).NEW_NAME,
                PROFIT_SS_ADDRESS = L_PROFIT_SS_DETAIL_TASKS_NT(L_INDEX).NEW_ADDRESS,
                PROFIT_SS_CITY = L_PROFIT_SS_DETAIL_TASKS_NT(L_INDEX).NEW_CITY,
                PROFIT_SS_STATE = L_PROFIT_SS_DETAIL_TASKS_NT(L_INDEX).NEW_STATE,
                PROFIT_SS_ZIP = L_PROFIT_SS_DETAIL_TASKS_NT(L_INDEX).NEW_ZIP
            WHERE
                PR_SS_D_S_SEC_NUMBER = L_PROFIT_DETAIL_TASKS_NT(L_INDEX).OLD_SSN;
        
        -- PROFIT_DETAIL

        FORALL L_INDEX IN 1 .. L_PROFIT_DETAIL_TASKS_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_DETAIL 
            SET 
                PR_DET_S_SEC_NUMBER = L_PROFIT_DETAIL_TASKS_NT(L_INDEX).NEW_SSN
            WHERE
                PR_DET_S_SEC_NUMBER = L_PROFIT_DETAIL_TASKS_NT(L_INDEX).OLD_SSN;

        -- And lastly, go through all of the SSN tasks rows we have and
        -- set flags on each row to done
        
        -- SSN TASKS
        FORALL L_INDEX IN 1 .. L_PAYBEN_CHANGES_NT.COUNT SAVE EXCEPTIONS
            UPDATE SSN_TASKS 
            SET 
                DONE = 1
            WHERE
               PYBEN_PAYSSN = L_PAYBEN_CHANGES_NT(L_INDEX).OLD_SSN;
               
            
         -- Our batch size is 200, and we need to commit every 100,000 beneficiaries 
        -- or else we will run out of 'undo' (rollback) space
        IF (L_HIGH_VOLUME_FLAG AND (MOD((L_BATCH_NUMBER * 200), 100000) = 0)) THEN
            COMMIT;
            dbms_output.put_line('Committed 100,000 records...');
        END IF;

    END LOOP;

    RETURN 0;

EXCEPTION
    WHEN OTHERS
    THEN
        -- DBMS_OUTPUT.put_line ('Rows inserted : ' || SQL%ROWCOUNT);
        l_error_count := SQL%BULK_EXCEPTIONS.count;
        DBMS_OUTPUT.put_line('Number of failures: ' || l_error_count);
        FOR indx IN 1 .. SQL%BULK_EXCEPTIONS.COUNT
          LOOP
             DBMS_OUTPUT.put_line (
                   'Error '
                || indx
                || ' occurred on index '
                || SQL%BULK_EXCEPTIONS (indx).ERROR_INDEX
                || ' attempting to update beneficiary SSNs. ');
             DBMS_OUTPUT.put_line (
                   'Oracle error is : '
                || SQLERRM(-SQL%BULK_EXCEPTIONS(indx).ERROR_CODE));
                   
          END LOOP;
        
          ROLLBACK; 
        RAISE;
END OBFUSCATE_BENEFICIARY_SSNS;
/

CREATE OR REPLACE FUNCTION OBFUSCATE_EMPLOYEE_PSNS RETURN NUMBER IS

 -- This is for batch reading of tables into nested tables
    BATCH_READ_SIZE  PLS_INTEGER := 200;

  -- Our Nested Table type for overall tasks
    TYPE EMPLOYEE_PSN_Tasks_NT IS TABLE OF PSN_TASKS%ROWTYPE INDEX BY PLS_INTEGER;
-- This is the intialized instance of our nested table
    L_EMPLOYEE_PSN_Tasks_NT EMPLOYEE_PSN_Tasks_NT;


    -- Nested table for generic PSN updates
    TYPE PSN_CHANGE_REC IS RECORD ( 
        OLD_PSN NUMBER(7),
        NEW_PSN NUMBER(7)
    );

    -- Nested table for rows in PROFIT_CMNT_PSN
    TYPE PROFIT_CMNT_PSN_NT IS TABLE OF PROFIT_CMNT_PSN%ROWTYPE;
    L_PROFIT_CMNT_PSN_NT PROFIT_CMNT_PSN_NT;

    -- Nested table for rows in PROFIT_SS_CMNT_PSN
    TYPE PROFIT_SS_CMNT_PSN_NT IS TABLE OF PROFIT_SS_CMNT_PSN%ROWTYPE;
    L_PROFIT_SS_CMNT_PSN_NT PROFIT_SS_CMNT_PSN_NT;
    
    -- Nested table for updates to PROFIT_DETAIL
    TYPE PROFIT_CMNT_CHANGE_REC IS RECORD ( 
        PROFIT_DET_RECNO NUMBER(9),
        NEW_PROF_COMMENT VARCHAR(16)
    );

    TYPE PROFIT_CMNT_PSN_ENTRIES_NT IS TABLE OF PROFIT_CMNT_CHANGE_REC;
    L_PROFIT_CMNT_PSN_ENTRIES_NT PROFIT_CMNT_PSN_ENTRIES_NT;

    TYPE PROFIT_SS_CMNT_PSN_ENTRIES_NT IS TABLE OF PROFIT_CMNT_CHANGE_REC;
    L_PROFIT_SS_CMNT_PSN_ENTRIES_NT PROFIT_SS_CMNT_PSN_ENTRIES_NT;

    TYPE PSN_TASKS_NT IS TABLE OF PSN_CHANGE_REC; 
    L_DEMO_CHANGES_NT PSN_TASKS_NT;
    --L_PROFIT_SHARE_CHECKS_NT PSN_TASKS_NT;
    L_PROFIT_DIST_REQ_EMP_NT PSN_TASKS_NT;
    L_PROFIT_DIST_REQ_PSN_NT PSN_TASKS_NT;
    L_PAYPROFIT_PSN_NT PSN_TASKS_NT;

    L_PAYREL_PSN_NT PSN_TASKS_NT;

    L_RANDOM_NUMBER NUMBER;
    
    L_HIGH_VOLUME_FLAG BOOLEAN;

 /* 
    We need to set up a cursor to read in all the fields
    for EMPLOYEE PSN values (identified by the EMPLOYEE 
    PSN being null)
*/

    CURSOR GET_EMPLOYEE_PSN_TASKS_CUR IS
    SELECT
        *
    FROM PSN_TASKS PSN
    WHERE DEMOGRAPHICS_PSN IS NOT NULL  AND DONE = 0;
    
--- Number of records
    L_SQL_ERROR NUMBER;
    L_SQL_ERRM VARCHAR2(512);
    
    L_ERROR_COUNT NUMBER;
    -- This is used to track indexes when we extend the specialized 
    -- nested tables
    L_LAST NUMBER := 0;
    
    L_BATCH_NUMBER NUMBER := 0;

BEGIN
        
        L_HIGH_VOLUME_FLAG := FALSE;

        L_PROFIT_CMNT_PSN_NT := PROFIT_CMNT_PSN_NT();
        L_PROFIT_CMNT_PSN_ENTRIES_NT := PROFIT_CMNT_PSN_ENTRIES_NT();

        L_PROFIT_SS_CMNT_PSN_NT := PROFIT_SS_CMNT_PSN_NT();
        L_PROFIT_SS_CMNT_PSN_ENTRIES_NT := PROFIT_SS_CMNT_PSN_ENTRIES_NT();

        L_EMPLOYEE_PSN_Tasks_NT := EMPLOYEE_PSN_Tasks_NT();

        L_DEMO_CHANGES_NT := PSN_TASKS_NT();
        --L_PROFIT_SHARE_CHECKS_NT := PSN_TASKS_NT();
        L_PROFIT_DIST_REQ_EMP_NT := PSN_TASKS_NT();
        L_PROFIT_DIST_REQ_PSN_NT := PSN_TASKS_NT();
        L_PAYPROFIT_PSN_NT := PSN_TASKS_NT();
        L_PAYREL_PSN_NT := PSN_TASKS_NT();
        
        OPEN GET_EMPLOYEE_PSN_TASKS_CUR;
        LOOP FETCH GET_EMPLOYEE_PSN_TASKS_CUR BULK COLLECT INTO L_EMPLOYEE_PSN_Tasks_NT LIMIT BATCH_READ_SIZE;
        EXIT WHEN L_EMPLOYEE_PSN_Tasks_NT.COUNT = 0;  
        L_BATCH_NUMBER := L_BATCH_NUMBER + 1;
        
        
        L_DEMO_CHANGES_NT.DELETE;
        --L_PROFIT_SHARE_CHECKS_NT.DELETE;
        L_PROFIT_DIST_REQ_EMP_NT.DELETE;
        L_PROFIT_DIST_REQ_PSN_NT.DELETE;
        L_PAYPROFIT_PSN_NT.DELETE;
        L_PAYREL_PSN_NT.DELETE;
       
        -- We know every record will have a change to the DEMOGRAPHICS table,
        -- so we can extend that NT by the size of the whole thing
        L_DEMO_CHANGES_NT.EXTEND(L_EMPLOYEE_PSN_Tasks_NT.COUNT);

        FOR rec IN 1 .. L_EMPLOYEE_PSN_Tasks_NT.COUNT
        LOOP

            L_DEMO_CHANGES_NT(rec).OLD_PSN := L_EMPLOYEE_PSN_Tasks_NT(rec).DEMOGRAPHICS_PSN;
            L_DEMO_CHANGES_NT(rec).NEW_PSN :=  L_EMPLOYEE_PSN_Tasks_NT(rec).REPLACEMENT_PSN;
    
            --- PROFIT_SHARE_CHECKS - EMP
            /*
            IF (L_EMPLOYEE_PSN_Tasks_NT(rec).PROFIT_SHARE_CHECKS_EMPLOYEE_NUMBER = 1) THEN
                L_PROFIT_SHARE_CHECKS_NT.EXTEND;
                L_LAST := L_PROFIT_SHARE_CHECKS_NT.LAST;
                L_PROFIT_SHARE_CHECKS_NT(L_LAST).OLD_PSN := L_EMPLOYEE_PSN_Tasks_NT(rec).DEMOGRAPHICS_PSN;
                L_PROFIT_SHARE_CHECKS_NT(L_LAST).NEW_PSN := L_EMPLOYEE_PSN_Tasks_NT(rec).REPLACEMENT_PSN;
                
            END IF;
            */
            
            -- PROFIT DIST REQ EMP
            IF (L_EMPLOYEE_PSN_Tasks_NT(rec).PROF_DIST_REQ_EMP = 1) THEN
                L_PROFIT_DIST_REQ_EMP_NT.EXTEND;
                L_LAST := L_PROFIT_DIST_REQ_EMP_NT.LAST;
                L_PROFIT_DIST_REQ_EMP_NT(L_LAST).OLD_PSN := L_EMPLOYEE_PSN_Tasks_NT(rec).DEMOGRAPHICS_PSN;
                L_PROFIT_DIST_REQ_EMP_NT(L_LAST).NEW_PSN := L_EMPLOYEE_PSN_Tasks_NT(rec).REPLACEMENT_PSN;
                
            END IF;

            -- Employee Badge number appearing in PROFIT_CMNT of PROFIT_DETAIL
            IF (L_EMPLOYEE_PSN_Tasks_NT(rec).PROFIT_CMNT_PSN = 1) THEN

                L_PROFIT_CMNT_PSN_NT.DELETE;

                -- So we have at least one of these records, but we probably have several
                -- So we need to loop through these

                SELECT * BULK COLLECT INTO L_PROFIT_CMNT_PSN_NT 
                FROM PROFIT_CMNT_PSN PCP 
                WHERE PCP.PROFIT_PSN = (L_EMPLOYEE_PSN_Tasks_NT(rec).DEMOGRAPHICS_PSN * 10000);

                FOR L_INDEX IN 1..L_PROFIT_CMNT_PSN_NT.COUNT
                LOOP
                    L_PROFIT_CMNT_PSN_ENTRIES_NT.EXTEND;
                    L_LAST := L_PROFIT_CMNT_PSN_ENTRIES_NT.LAST;
                    L_PROFIT_CMNT_PSN_ENTRIES_NT(L_LAST).PROFIT_DET_RECNO := L_PROFIT_CMNT_PSN_NT(L_INDEX).PROFIT_DET_RECNO;
                   
                    /* Rarely log what we have here
                    L_RANDOM_NUMBER := FLOOR(DBMS_RANDOM.VALUE(1, 25));

                    IF (L_RANDOM_NUMBER = 2) THEN
                   
                        DBMS_OUTPUT.put_line('Prefix was: ' || L_PROFIT_CMNT_PSN_NT(L_INDEX).PROFIT_CMNT_PREFIX ||  
                        ' and PSN was: ' || TO_CHAR(L_EMPLOYEE_PSN_Tasks_NT(rec).REPLACEMENT_PSN * 10000));
                        DBMS_OUTPUT.put_line('Prefix length: ' || LENGTH(L_PROFIT_CMNT_PSN_NT(L_INDEX).PROFIT_CMNT_PREFIX));
                        DBMS_OUTPUT.put_line('PSN length: ' || LENGTH(TO_CHAR(L_EMPLOYEE_PSN_Tasks_NT(rec).REPLACEMENT_PSN * 10000)));
                        DBMS_OUTPUT.put_line('COMBINED length: ' || LENGTH(L_PROFIT_CMNT_PSN_NT(L_INDEX).PROFIT_CMNT_PREFIX || TO_CHAR(L_EMPLOYEE_PSN_Tasks_NT(rec).REPLACEMENT_PSN * 10000)));
                    END IF;
                    */
                    L_PROFIT_CMNT_PSN_ENTRIES_NT(L_LAST).NEW_PROF_COMMENT := 
                        ASSEMBLE_COMMENT_TRANSFER(TRIM(L_PROFIT_CMNT_PSN_NT(L_INDEX).PROFIT_CMNT_PREFIX),
                        TO_CHAR(L_EMPLOYEE_PSN_Tasks_NT(rec).REPLACEMENT_PSN * 10000));
                    
                END LOOP;

            END IF;

            -- We have to do this for PROFIT_SS_DETAIL also
            IF (L_EMPLOYEE_PSN_Tasks_NT(rec).PROFIT_SS_CMNT_PSN = 1) THEN

                L_PROFIT_SS_CMNT_PSN_NT.DELETE;

                -- So we have at least one of these records, but we probably have several
                -- So we need to loop through these

                SELECT * BULK COLLECT INTO L_PROFIT_SS_CMNT_PSN_NT 
                FROM PROFIT_SS_CMNT_PSN PCP 
                WHERE PCP.PROFIT_SS_PSN = (L_EMPLOYEE_PSN_Tasks_NT(rec).DEMOGRAPHICS_PSN * 10000);

                FOR L_INDEX IN 1..L_PROFIT_SS_CMNT_PSN_NT.COUNT
                LOOP
                    L_PROFIT_SS_CMNT_PSN_ENTRIES_NT.EXTEND;
                    L_LAST := L_PROFIT_SS_CMNT_PSN_ENTRIES_NT.LAST;
                    L_PROFIT_SS_CMNT_PSN_ENTRIES_NT(L_LAST).PROFIT_DET_RECNO := L_PROFIT_SS_CMNT_PSN_NT(L_INDEX).PROFIT_SS_DET_RECNO;

                    L_PROFIT_SS_CMNT_PSN_ENTRIES_NT(L_LAST).NEW_PROF_COMMENT := 
                        ASSEMBLE_COMMENT_TRANSFER(TRIM(L_PROFIT_SS_CMNT_PSN_NT(L_INDEX).PROFIT_SS_CMNT_PREFIX),
                        TO_CHAR(L_EMPLOYEE_PSN_Tasks_NT(rec).REPLACEMENT_PSN));
                END LOOP;

            END IF;

            -- PAYREL
            IF (L_EMPLOYEE_PSN_Tasks_NT(rec).PAYREL_PSN = 1) THEN
                L_PAYREL_PSN_NT.EXTEND;
                L_LAST := L_PAYREL_PSN_NT.LAST;
                L_PAYREL_PSN_NT(L_LAST).OLD_PSN := L_EMPLOYEE_PSN_Tasks_NT(rec).DEMOGRAPHICS_PSN;
                L_PAYREL_PSN_NT(L_LAST).NEW_PSN := L_EMPLOYEE_PSN_Tasks_NT(rec).REPLACEMENT_PSN;
                
            END IF;
            
            -- PAYPROFIT
            IF (L_EMPLOYEE_PSN_Tasks_NT(rec).PAYPROFIT_PSN = 1) THEN
                L_PAYPROFIT_PSN_NT.EXTEND;
                L_LAST := L_PAYPROFIT_PSN_NT.LAST;
                L_PAYPROFIT_PSN_NT(L_LAST).OLD_PSN := L_EMPLOYEE_PSN_Tasks_NT(rec).DEMOGRAPHICS_PSN;
                L_PAYPROFIT_PSN_NT(L_LAST).NEW_PSN := L_EMPLOYEE_PSN_Tasks_NT(rec).REPLACEMENT_PSN;
                
            END IF;

            -- PROFIT DIST REQ PSN
            IF (L_EMPLOYEE_PSN_Tasks_NT(rec).PROF_DIST_REQ_PSN = 1) THEN
                L_PROFIT_DIST_REQ_PSN_NT.EXTEND;
                L_LAST := L_PROFIT_DIST_REQ_PSN_NT.LAST;
                L_PROFIT_DIST_REQ_PSN_NT(L_LAST).OLD_PSN := L_EMPLOYEE_PSN_Tasks_NT(rec).DEMOGRAPHICS_PSN;
                L_PROFIT_DIST_REQ_PSN_NT(L_LAST).NEW_PSN := L_EMPLOYEE_PSN_Tasks_NT(rec).REPLACEMENT_PSN;
                
            END IF;
                   
        END LOOP;
        
        -- Now we need to do the FORALL updates
        
   
        -- DEMO
        FORALL L_INDEX IN 1 .. L_DEMO_CHANGES_NT.COUNT SAVE EXCEPTIONS
            UPDATE DEMOGRAPHICS 
            SET 
                DEM_BADGE = L_DEMO_CHANGES_NT(L_INDEX).NEW_PSN    
            WHERE
                DEM_BADGE = L_DEMO_CHANGES_NT(L_INDEX).OLD_PSN;

         -- Time for one PROFIT_SHARE_CHECKS updates
        /*
        
        Note: We should be doing this update, but the employee number on checks is the older
        NUMBER(5) which is no longer valid
        
        FORALL L_INDEX IN 1 .. L_PROFIT_SHARE_CHECKS_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_SHARE_CHECKS
            SET 
                EMPLOYEE_NUMBER = L_PROFIT_SHARE_CHECKS_NT(L_INDEX).NEW_PSN
            WHERE
                EMPLOYEE_NUMBER = L_PROFIT_SHARE_CHECKS_NT(L_INDEX).OLD_PSN;
                
        */

        -- PAYREL
        FORALL L_INDEX IN 1 .. L_PAYREL_PSN_NT.COUNT SAVE EXCEPTIONS
            UPDATE PAYREL 
            SET 
                PYREL_PSN = L_PAYREL_PSN_NT(L_INDEX).NEW_PSN    
            WHERE
                PYREL_PSN = L_PAYREL_PSN_NT(L_INDEX).OLD_PSN;

        -- PROFIT_DETAIL
        FORALL L_INDEX IN 1 .. L_PROFIT_CMNT_PSN_ENTRIES_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_DETAIL 
            SET 
                PROFIT_CMNT = L_PROFIT_CMNT_PSN_ENTRIES_NT(L_INDEX).NEW_PROF_COMMENT
            WHERE
                PROFIT_DET_RECNO = L_PROFIT_CMNT_PSN_ENTRIES_NT(L_INDEX).PROFIT_DET_RECNO;

        -- PROFIT_DETAIL bookkeeping
        -- Now we need to mark those rows as done

        FORALL L_INDEX IN 1 .. L_PROFIT_CMNT_PSN_ENTRIES_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_CMNT_PSN 
            SET 
                DONE = 1
            WHERE
                PROFIT_DET_RECNO = L_PROFIT_CMNT_PSN_ENTRIES_NT(L_INDEX).PROFIT_DET_RECNO;


        -- PROFIT_SS_DETAIL
        FORALL L_INDEX IN 1 .. L_PROFIT_SS_CMNT_PSN_ENTRIES_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_SS_DETAIL 
            SET 
                PROFIT_SS_CMNT = L_PROFIT_SS_CMNT_PSN_ENTRIES_NT(L_INDEX).NEW_PROF_COMMENT
            WHERE
                PROFIT_SS_DET_RECNO = L_PROFIT_SS_CMNT_PSN_ENTRIES_NT(L_INDEX).PROFIT_DET_RECNO;
        
        -- PROFIT_SS_DETAIL bookkeeping
        -- Now we need to mark those rows as done

        FORALL L_INDEX IN 1 .. L_PROFIT_SS_CMNT_PSN_ENTRIES_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_SS_CMNT_PSN 
            SET 
                DONE = 1
            WHERE
                PROFIT_SS_DET_RECNO = L_PROFIT_SS_CMNT_PSN_ENTRIES_NT(L_INDEX).PROFIT_DET_RECNO;


        -- PROFIT DIST REQ EMP
        FORALL L_INDEX IN 1 .. L_PROFIT_DIST_REQ_EMP_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_DIST_REQ 
            SET 
                PROFIT_DIST_REQ_EMP = L_PROFIT_DIST_REQ_EMP_NT(L_INDEX).NEW_PSN    
            WHERE
                PROFIT_DIST_REQ_EMP = L_PROFIT_DIST_REQ_EMP_NT(L_INDEX).OLD_PSN;
                
        
        -- PAYPROFIT
        FORALL L_INDEX IN 1 .. L_PAYPROFIT_PSN_NT.COUNT SAVE EXCEPTIONS
            UPDATE PAYPROFIT
            SET 
                PAYPROF_BADGE = L_PAYPROFIT_PSN_NT(L_INDEX).NEW_PSN    
            WHERE
                PAYPROF_BADGE = L_PAYPROFIT_PSN_NT(L_INDEX).OLD_PSN;

        
        -- PROFIT DIST REQ PSN
        FORALL L_INDEX IN 1 .. L_PROFIT_DIST_REQ_PSN_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_DIST_REQ
            SET 
                PROFIT_DIST_REQ_PSN = L_PROFIT_DIST_REQ_PSN_NT(L_INDEX).NEW_PSN    
            WHERE
                PROFIT_DIST_REQ_PSN = L_PROFIT_DIST_REQ_PSN_NT(L_INDEX).OLD_PSN;

        -- And lastly, go through all of the PSN tasks rows we have and
        -- set flags on each row to done
         -- PSN TASKS
        FORALL L_INDEX IN 1 .. L_DEMO_CHANGES_NT.COUNT SAVE EXCEPTIONS
            UPDATE PSN_TASKS 
            SET 
                DONE = 1
            WHERE
               DEMOGRAPHICS_PSN = L_DEMO_CHANGES_NT(L_INDEX).OLD_PSN;
       
        -- Our batch size is 200, and we need to commit every 100,000 employees 
        -- or else we will run out of 'undo' (rollback) space
        IF (L_HIGH_VOLUME_FLAG AND (MOD((L_BATCH_NUMBER * 200), 100000) = 0)) THEN
            COMMIT;
            dbms_output.put_line('Committed 100,000 records...');
        END IF;

        L_PROFIT_CMNT_PSN_ENTRIES_NT.DELETE;
        L_PROFIT_SS_CMNT_PSN_ENTRIES_NT.DELETE;
        
    END LOOP;

    RETURN 0;

EXCEPTION
    WHEN OTHERS
    THEN
        -- DBMS_OUTPUT.put_line ('Rows inserted : ' || SQL%ROWCOUNT);
        l_error_count := SQL%BULK_EXCEPTIONS.count;
        DBMS_OUTPUT.put_line('Number of failures: ' || l_error_count);
        FOR indx IN 1 .. SQL%BULK_EXCEPTIONS.COUNT
          LOOP
             DBMS_OUTPUT.put_line (
                   'Error '
                || indx
                || ' occurred on index '
                || SQL%BULK_EXCEPTIONS (indx).ERROR_INDEX
                || ' attempting to update employee PSNs. ');
             DBMS_OUTPUT.put_line (
                   'Oracle error is : '
                || SQLERRM(-SQL%BULK_EXCEPTIONS(indx).ERROR_CODE));
                   
          END LOOP;
        
          ROLLBACK; 
        RAISE;
END OBFUSCATE_EMPLOYEE_PSNS;
/

CREATE OR REPLACE FUNCTION OBFUSCATE_BENEFICIARY_PSNS RETURN NUMBER IS

 -- This is for batch reading of tables into nested tables
    BATCH_READ_SIZE  PLS_INTEGER := 200;

  -- Our Nested Table type for overall tasks
    TYPE BENEFICIARY_PSN_Tasks_NT IS TABLE OF PSN_TASKS%ROWTYPE INDEX BY PLS_INTEGER;
-- This is the intialized instance of our nested table
    L_BENEFICIARY_PSN_Tasks_NT BENEFICIARY_PSN_Tasks_NT;

    -- Nested table for rows in PROFIT_CMNT_PSN
    TYPE PROFIT_CMNT_PSN_NT IS TABLE OF PROFIT_CMNT_PSN%ROWTYPE;
    L_PROFIT_CMNT_PSN_NT PROFIT_CMNT_PSN_NT;
    
    -- Nested table for rows in PROFIT_SS_CMNT_PSN
    TYPE PROFIT_SS_CMNT_PSN_NT IS TABLE OF PROFIT_SS_CMNT_PSN%ROWTYPE;
    L_PROFIT_SS_CMNT_PSN_NT PROFIT_SS_CMNT_PSN_NT;

    -- Nested table for updates to PROFIT_DETAIL and PROFIT_SS_DETAIL
    TYPE PROFIT_CMNT_CHANGE_REC IS RECORD ( 
        PROFIT_DET_RECNO NUMBER(9),
        NEW_PROF_COMMENT VARCHAR(16)
    );

    TYPE PROFIT_CMNT_PSN_ENTRIES_NT IS TABLE OF PROFIT_CMNT_CHANGE_REC;
    L_PROFIT_CMNT_PSN_ENTRIES_NT PROFIT_CMNT_PSN_ENTRIES_NT;

    TYPE PROFIT_SS_CMNT_PSN_ENTRIES_NT IS TABLE OF PROFIT_CMNT_CHANGE_REC;
    L_PROFIT_SS_CMNT_PSN_ENTRIES_NT PROFIT_SS_CMNT_PSN_ENTRIES_NT;

    -- Nested table for generic PSN updates
    TYPE PSN_CHANGE_REC IS RECORD ( 
        OLD_PSN NUMBER(11),
        NEW_PSN NUMBER(11)
    );
    
    TYPE PSN_TASKS_NT IS TABLE OF PSN_CHANGE_REC; 
    L_PAYBEN_CHANGES_NT PSN_TASKS_NT;

    L_PROFIT_SHARE_CHECKS_NT PSN_TASKS_NT;
    L_PROFIT_DIST_REQ_EMP_NT PSN_TASKS_NT;
    L_PROFIT_DIST_REQ_PSN_NT PSN_TASKS_NT;
    L_PAYREL_NT PSN_TASKS_NT;
    
    L_HIGH_VOLUME_FLAG BOOLEAN;

 /* 
    We need to set up a cursor to read in all the fields
    for BENEFICIARY PSN values (identified by the BENEFICIARY 
    PSN being null)
*/

    CURSOR GET_BENEFICIARY_PSN_TASKS_CUR IS
    SELECT
        *
    FROM PSN_TASKS PSN
    WHERE PAYBEN_PSN IS NOT NULL AND DONE = 0;
    
--- Number of records
    L_SQL_ERROR NUMBER;
    L_SQL_ERRM VARCHAR2(512);
    
    L_ERROR_COUNT NUMBER;
    -- This is used to track indexes when we extend the specialized 
    -- nested tables
    L_LAST NUMBER := 0;
    
    L_BATCH_NUMBER NUMBER := 0;

BEGIN
    
        -- Set this to true if the number of beneficiaries is greater than 500,000
        L_HIGH_VOLUME_FLAG := FALSE;

        L_BENEFICIARY_PSN_Tasks_NT := BENEFICIARY_PSN_Tasks_NT();
        L_PAYBEN_CHANGES_NT := PSN_TASKS_NT();
        L_PAYREL_NT := PSN_TASKS_NT();
        L_PROFIT_DIST_REQ_PSN_NT := PSN_TASKS_NT();

        L_PROFIT_SS_CMNT_PSN_NT := PROFIT_SS_CMNT_PSN_NT();
        L_PROFIT_SS_CMNT_PSN_ENTRIES_NT := PROFIT_SS_CMNT_PSN_ENTRIES_NT();

        L_PROFIT_CMNT_PSN_NT := PROFIT_CMNT_PSN_NT();
        L_PROFIT_CMNT_PSN_ENTRIES_NT := PROFIT_CMNT_PSN_ENTRIES_NT();
        
        
        OPEN GET_BENEFICIARY_PSN_TASKS_CUR;
        LOOP FETCH GET_BENEFICIARY_PSN_TASKS_CUR BULK COLLECT INTO L_BENEFICIARY_PSN_Tasks_NT LIMIT BATCH_READ_SIZE;
        EXIT WHEN L_BENEFICIARY_PSN_Tasks_NT.COUNT = 0;  
        L_BATCH_NUMBER := L_BATCH_NUMBER + 1;

        
        L_PAYBEN_CHANGES_NT.DELETE;
        L_PAYREL_NT.DELETE;
        L_PROFIT_DIST_REQ_PSN_NT.DELETE;
       
        -- We know every record will have a change to the PAYBEN table,
        -- so we can extend that NT by the size of the whole thing
        L_PAYBEN_CHANGES_NT.EXTEND(L_BENEFICIARY_PSN_Tasks_NT.COUNT);

        FOR rec IN 1 .. L_BENEFICIARY_PSN_Tasks_NT.COUNT
        LOOP

            L_PAYBEN_CHANGES_NT(rec).OLD_PSN := L_BENEFICIARY_PSN_Tasks_NT(rec).PAYBEN_PSN;
            -- Need to use the replacement beneficiary PSN
            L_PAYBEN_CHANGES_NT(rec).NEW_PSN :=  L_BENEFICIARY_PSN_Tasks_NT(rec).REPLACEMENT_BEN_PSN;

            
            -- PAYREL
            IF (L_BENEFICIARY_PSN_Tasks_NT(rec).PAYREL_PSN = 1) THEN
                L_PAYREL_NT.EXTEND;
                L_LAST := L_PAYREL_NT.LAST;
                L_PAYREL_NT(L_LAST).OLD_PSN := L_BENEFICIARY_PSN_Tasks_NT(rec).PAYBEN_PSN;
                L_PAYREL_NT(L_LAST).NEW_PSN := L_BENEFICIARY_PSN_Tasks_NT(rec).REPLACEMENT_BEN_PSN;
                
            END IF;


             -- Employee Badge number appearing in PROFIT_CMNT of PROFIT_DETAIL
            IF (L_BENEFICIARY_PSN_Tasks_NT(rec).PROFIT_CMNT_PSN = 1) THEN

                L_PROFIT_CMNT_PSN_NT.DELETE;

                -- So we have at least one of these records, but we probably have several
                -- So we need to loop through these

                SELECT * BULK COLLECT INTO L_PROFIT_CMNT_PSN_NT 
                FROM PROFIT_CMNT_PSN PCP 
                WHERE PCP.PROFIT_PSN = L_BENEFICIARY_PSN_Tasks_NT(rec).PAYBEN_PSN;

                FOR L_INDEX IN 1..L_PROFIT_CMNT_PSN_NT.COUNT
                LOOP
                    L_PROFIT_CMNT_PSN_ENTRIES_NT.EXTEND;
                    L_LAST := L_PROFIT_CMNT_PSN_ENTRIES_NT.LAST;
                    L_PROFIT_CMNT_PSN_ENTRIES_NT(L_LAST).PROFIT_DET_RECNO := L_PROFIT_CMNT_PSN_NT(L_INDEX).PROFIT_DET_RECNO;
                    L_PROFIT_CMNT_PSN_ENTRIES_NT(L_LAST).NEW_PROF_COMMENT := 
                        ASSEMBLE_COMMENT_TRANSFER(TRIM(L_PROFIT_CMNT_PSN_NT(L_INDEX).PROFIT_CMNT_PREFIX),
                        TO_CHAR(L_BENEFICIARY_PSN_Tasks_NT(rec).REPLACEMENT_BEN_PSN));
                END LOOP;

            END IF;

            -- We have to do this for PROFIT_SS_DETAIL also
            IF (L_BENEFICIARY_PSN_Tasks_NT(rec).PROFIT_SS_CMNT_PSN = 1) THEN

                L_PROFIT_SS_CMNT_PSN_NT.DELETE;

                -- So we have at least one of these records, but we probably have several
                -- So we need to loop through these

                SELECT * BULK COLLECT INTO L_PROFIT_SS_CMNT_PSN_NT 
                FROM PROFIT_SS_CMNT_PSN PCP 
                WHERE PCP.PROFIT_SS_PSN = L_BENEFICIARY_PSN_Tasks_NT(rec).PAYBEN_PSN;

                FOR L_INDEX IN 1..L_PROFIT_SS_CMNT_PSN_NT.COUNT
                LOOP
                    L_PROFIT_SS_CMNT_PSN_ENTRIES_NT.EXTEND;
                    L_LAST := L_PROFIT_SS_CMNT_PSN_ENTRIES_NT.LAST;
                    L_PROFIT_SS_CMNT_PSN_ENTRIES_NT(L_LAST).PROFIT_DET_RECNO := L_PROFIT_SS_CMNT_PSN_NT(L_INDEX).PROFIT_SS_DET_RECNO;
                    L_PROFIT_SS_CMNT_PSN_ENTRIES_NT(L_LAST).NEW_PROF_COMMENT := 
                        ASSEMBLE_COMMENT_TRANSFER(TRIM(L_PROFIT_SS_CMNT_PSN_NT(L_INDEX).PROFIT_SS_CMNT_PREFIX),
                        TO_CHAR(L_BENEFICIARY_PSN_Tasks_NT(rec).REPLACEMENT_BEN_PSN));
                END LOOP;

            END IF;

            -- PROFIT DIST REQ PSN
            IF (L_BENEFICIARY_PSN_Tasks_NT(rec).PROF_DIST_REQ_PSN = 1) THEN
                L_PROFIT_DIST_REQ_PSN_NT.EXTEND;
                L_LAST := L_PROFIT_DIST_REQ_PSN_NT.LAST;
                L_PROFIT_DIST_REQ_PSN_NT(L_LAST).OLD_PSN := L_BENEFICIARY_PSN_Tasks_NT(rec).PAYBEN_PSN;
                L_PROFIT_DIST_REQ_PSN_NT(L_LAST).NEW_PSN := L_BENEFICIARY_PSN_Tasks_NT(rec).REPLACEMENT_BEN_PSN;
                
            END IF;
      
        END LOOP;
        
        -- Now we need to do the FORALL updates
        

        -- PAYBEN
        FORALL L_INDEX IN 1 .. L_PAYBEN_CHANGES_NT.COUNT SAVE EXCEPTIONS
            UPDATE PAYBEN 
            SET 
                PYBEN_PSN = L_PAYBEN_CHANGES_NT(L_INDEX).NEW_PSN    
            WHERE
                PYBEN_PSN = L_PAYBEN_CHANGES_NT(L_INDEX).OLD_PSN;

        -- PROFIT_DETAIL
        FORALL L_INDEX IN 1 .. L_PROFIT_CMNT_PSN_ENTRIES_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_DETAIL 
            SET 
                PROFIT_CMNT = L_PROFIT_CMNT_PSN_ENTRIES_NT(L_INDEX).NEW_PROF_COMMENT
            WHERE
                PROFIT_DET_RECNO = L_PROFIT_CMNT_PSN_ENTRIES_NT(L_INDEX).PROFIT_DET_RECNO;

        -- PROFIT_DETAIL bookkeeping
        -- Now we need to mark those rows as done

        FORALL L_INDEX IN 1 .. L_PROFIT_CMNT_PSN_ENTRIES_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_CMNT_PSN 
            SET 
                DONE = 1
            WHERE
                PROFIT_DET_RECNO = L_PROFIT_CMNT_PSN_ENTRIES_NT(L_INDEX).PROFIT_DET_RECNO;


        -- PROFIT_SS_DETAIL
        FORALL L_INDEX IN 1 .. L_PROFIT_SS_CMNT_PSN_ENTRIES_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_SS_DETAIL 
            SET 
                PROFIT_SS_CMNT = L_PROFIT_SS_CMNT_PSN_ENTRIES_NT(L_INDEX).NEW_PROF_COMMENT
            WHERE
                PROFIT_SS_DET_RECNO = L_PROFIT_SS_CMNT_PSN_ENTRIES_NT(L_INDEX).PROFIT_DET_RECNO;

        -- PROFIT_SS_DETAIL bookkeeping
        -- Now we need to mark those rows as done

        FORALL L_INDEX IN 1 .. L_PROFIT_SS_CMNT_PSN_ENTRIES_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_SS_CMNT_PSN 
            SET 
                DONE = 1
            WHERE
                PROFIT_SS_DET_RECNO = L_PROFIT_SS_CMNT_PSN_ENTRIES_NT(L_INDEX).PROFIT_DET_RECNO;


        -- PAYREL
        FORALL L_INDEX IN 1 .. L_PAYREL_NT.COUNT SAVE EXCEPTIONS
            UPDATE PAYREL 
            SET 
                PYREL_PSN = L_PAYREL_NT(L_INDEX).NEW_PSN    
            WHERE
                PYREL_PSN = L_PAYREL_NT(L_INDEX).OLD_PSN;

        -- PROFIT DIST REQ PSN
        FORALL L_INDEX IN 1 .. L_PROFIT_DIST_REQ_PSN_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_DIST_REQ 
            SET 
                PROFIT_DIST_REQ_PSN = L_PROFIT_DIST_REQ_PSN_NT(L_INDEX).NEW_PSN    
            WHERE
                PROFIT_DIST_REQ_PSN = L_PROFIT_DIST_REQ_PSN_NT(L_INDEX).OLD_PSN;

        

        -- And lastly, go through all of the PSN tasks rows we have and
        -- set flags on each row to done
        -- PSN TASKS
        FORALL L_INDEX IN 1 .. L_PAYBEN_CHANGES_NT.COUNT SAVE EXCEPTIONS
            UPDATE PSN_TASKS 
            SET 
                DONE = 1
            WHERE
                PAYBEN_PSN = L_PAYBEN_CHANGES_NT(L_INDEX).OLD_PSN;
                
                
        -- Our batch size is 200, and we need to commit every 100,000 beneficiaries 
        -- or else we will run out of 'undo' (rollback) space
        IF (L_HIGH_VOLUME_FLAG AND (MOD((L_BATCH_NUMBER * 200), 100000) = 0)) THEN
            COMMIT;
            dbms_output.put_line('Committed 100,000 records.....');
        END IF;

        L_PROFIT_CMNT_PSN_ENTRIES_NT.DELETE;
        L_PROFIT_SS_CMNT_PSN_ENTRIES_NT.DELETE;
        
    END LOOP;

    RETURN 0;

EXCEPTION
    WHEN OTHERS
    THEN
        -- DBMS_OUTPUT.put_line ('Rows inserted : ' || SQL%ROWCOUNT);
        l_error_count := SQL%BULK_EXCEPTIONS.count;
        DBMS_OUTPUT.put_line('Number of failures: ' || l_error_count);
        FOR indx IN 1 .. SQL%BULK_EXCEPTIONS.COUNT
          LOOP
             DBMS_OUTPUT.put_line (
                   'Error '
                || indx
                || ' occurred on index '
                || SQL%BULK_EXCEPTIONS (indx).ERROR_INDEX
                || ' attempting to update beneficiary PSNs. ');
             DBMS_OUTPUT.put_line (
                   'Oracle error is : '
                || SQLERRM(-SQL%BULK_EXCEPTIONS(indx).ERROR_CODE));
                   
          END LOOP;
        
          ROLLBACK; 
        RAISE;
END OBFUSCATE_BENEFICIARY_PSNS;
/