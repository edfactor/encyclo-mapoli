/*

This script will use the contents of TARGET_EMP_PSN in order to get all matching
beneficiary records, and then use those rows to insert into three tables:

TARGET_BEN_PSN
TARGET_BEN_SSN
TARGET_ALL_SSN

*/

DECLARE

    -- This is for batch reading of tables into nested tables
    BATCH_READ_SIZE  PLS_INTEGER := 200;

    CURSOR GET_TARGET_EMP_PSN_CUR IS
        SELECT
        PSN
    FROM TARGET_EMP_PSN;


    TYPE EMPLOYEE_PSN_REC IS RECORD ( 
        PSN NUMBER(7)
    );

    TYPE BENEFICIARY_REC IS RECORD ( 
        SSN NUMBER(9),
        PSN NUMBER(11)
    );

    -- Our Nested Table types
    TYPE EMPLOYEE_PSNs_NT IS TABLE OF EMPLOYEE_PSN_REC INDEX BY PLS_INTEGER;
    TYPE BENEFICIARY_NT IS TABLE OF BENEFICIARY_REC;
    
    -- This is the intialized instance of our nested table
    L_EMPLOYEE_PSNs_NT EMPLOYEE_PSNs_NT;
    L_BENEFICIARY_NT BENEFICIARY_NT;
 
    NUM_RECORDS NUMBER := 0;
    L_SQL_ERROR NUMBER;
    L_SQL_ERRM VARCHAR2(512);

    L_ERROR_COUNT NUMBER;
    L_BATCH_NUMBER NUMBER := 0;
    L_LAST_INDEX NUMBER  := 0;

    -- Variables for performance timing
    l_start_time NUMBER;
    l_completed_time NUMBER;
    l_elapsed_minutes NUMBER;
    l_elapsed_seconds NUMBER;


BEGIN

    l_start_time := DBMS_UTILITY.get_time;
    
    DBMS_OUTPUT.PUT_LINE('Proceeding to fill TARGET_BEN_PSN, TARGET_ALL_PSN, and TARGET_BEN_SSN tables... ');

    L_EMPLOYEE_PSNs_NT := EMPLOYEE_PSNs_NT();
    L_BENEFICIARY_NT := BENEFICIARY_NT();
            
    OPEN GET_TARGET_EMP_PSN_CUR;
    LOOP FETCH GET_TARGET_EMP_PSN_CUR BULK COLLECT INTO L_EMPLOYEE_PSNs_NT LIMIT BATCH_READ_SIZE;
    EXIT WHEN L_EMPLOYEE_PSNs_NT.COUNT = 0;  
    L_BATCH_NUMBER := L_BATCH_NUMBER + 1;
    dbms_output.put_line('Batch of ' || BATCH_READ_SIZE || ' number : ' || L_BATCH_NUMBER);

    -- So now we need to loop through the PSNs to get matching PAYBEN records.
    FOR L_INDEX IN 1..L_EMPLOYEE_PSNs_NT.COUNT
    LOOP
        FOR beneficiary_rec IN (
            SELECT PYBEN_PSN, PYBEN_PAYSSN
                FROM PAYBEN
                WHERE SUBSTR(PYBEN_PSN, 1, LENGTH(PYBEN_PSN) - 4) = L_EMPLOYEE_PSNs_NT(L_INDEX).PSN )
            LOOP
                L_BENEFICIARY_NT.EXTEND;
                L_LAST_INDEX := L_BENEFICIARY_NT.LAST;
                L_BENEFICIARY_NT(L_LAST_INDEX).PSN := beneficiary_rec.PYBEN_PSN;
                L_BENEFICIARY_NT(L_LAST_INDEX).SSN := beneficiary_rec.PYBEN_PAYSSN;
            END LOOP;  -- End select records
        END LOOP; -- End employee loop
        
    -- So now we just need four bulk inserts for four different tables
    FORALL L_INDEX IN 1 ..  L_BENEFICIARY_NT.COUNT SAVE EXCEPTIONS
        INSERT INTO TARGET_BEN_PSN (PSN) 
        VALUES (L_BENEFICIARY_NT(L_INDEX).PSN);

    FORALL L_INDEX IN 1 ..  L_BENEFICIARY_NT.COUNT SAVE EXCEPTIONS
        INSERT INTO TARGET_ALL_PSN (PSN) 
        VALUES (L_BENEFICIARY_NT(L_INDEX).PSN);

    FORALL L_INDEX IN 1 ..  L_BENEFICIARY_NT.COUNT SAVE EXCEPTIONS
        INSERT INTO TARGET_BEN_SSN (SSN) 
        VALUES (L_BENEFICIARY_NT(L_INDEX).SSN);

    FORALL L_INDEX IN 1 ..  L_BENEFICIARY_NT.COUNT SAVE EXCEPTIONS
        INSERT INTO TARGET_ALL_SSN (SSN) 
        VALUES (L_BENEFICIARY_NT(L_INDEX).SSN);

    
    -- Now delete table for next batch
    L_BENEFICIARY_NT.DELETE;
    
    END LOOP; -- End bulk select

    COMMIT;

    l_completed_time := DBMS_UTILITY.get_time;
    l_elapsed_seconds := (l_completed_time - l_start_time) / 100;
    l_elapsed_minutes := trunc(l_elapsed_seconds / 60, 0);
    
    dbms_output.put_line(chr(10) || '[TIME] Create Beneficiary SSN and PSN tables duration: ' 
    || l_elapsed_minutes || ' minutes + ' || mod(l_elapsed_seconds, 60) || ' seconds.'); 

EXCEPTION  
   WHEN OTHERS THEN  
    dbms_output.put_line('ERROR_STACK: ' || DBMS_UTILITY.FORMAT_ERROR_STACK);
    dbms_output.put_line('ERROR_BACKTRACE: ' || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE);
   
      DBMS_OUTPUT.put_line (SQLERRM);  
      DBMS_OUTPUT.put_line (  
         'Inserted ' || SQL%ROWCOUNT || ' rows.');  
  
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

END;

