-- This script makes sure that any values prepopulated in 
-- these tables: TARGET_EMP_PSN, TARGET_BEN_PSN,
-- TARGET_BEN_SSN - get added to the proper "all"
-- table ahead of time
DECLARE

    -- This is for batch reading of tables into nested tables
    BATCH_READ_SIZE  PLS_INTEGER := 200;

    CURSOR GET_TARGET_SSN_CUR IS
        SELECT
        SSN
    FROM TARGET_BEN_SSN;

    CURSOR GET_TARGET_BEN_PSN_CUR IS
        SELECT
        PSN
    FROM TARGET_BEN_PSN;

    CURSOR GET_TARGET_EMP_PSN_CUR IS
        SELECT
        PSN
    FROM TARGET_EMP_PSN;

    TYPE BEN_PSN_REC IS RECORD ( 
        PSN NUMBER(11)
    );

    TYPE EMP_PSN_REC IS RECORD ( 
        PSN NUMBER(7)
    );

    TYPE BEN_SSN_REC IS RECORD ( 
        SSN NUMBER(9)
    );

    -- Our Nested Table types
    TYPE BEN_SSNs_NT IS TABLE OF BEN_SSN_REC INDEX BY PLS_INTEGER;
    TYPE BEN_PSNs_NT IS TABLE OF BEN_PSN_REC INDEX BY PLS_INTEGER;
    TYPE EMP_PSNs_NT IS TABLE OF EMP_PSN_REC INDEX BY PLS_INTEGER;
    
    -- This is the intialized instance of our nested table
    L_BEN_SSNs_NT BEN_SSNs_NT;
    L_BEN_PSNs_NT BEN_PSNs_NT;
    L_EMP_PSNs_NT EMP_PSNs_NT;
 
    NUM_RECORDS NUMBER := 0;
    L_SQL_ERROR NUMBER;
    L_SQL_ERRM VARCHAR2(512);

    L_ERROR_COUNT NUMBER;
    L_BATCH_NUMBER NUMBER := 0;

    -- Variables for performance timing
    l_start_time NUMBER;
    l_completed_time NUMBER;
    l_elapsed_minutes NUMBER;
    l_elapsed_seconds NUMBER;
 


BEGIN

    l_start_time := DBMS_UTILITY.get_time;
    
    DBMS_OUTPUT.PUT_LINE('Proceeding to add seeded values to TARGET_ALL_PSN and TARGET_ALL_SSN tables... ');

    L_BEN_SSNs_NT := BEN_SSNs_NT();
    L_BEN_PSNs_NT := BEN_PSNs_NT();
    L_EMP_PSNs_NT := EMP_PSNs_NT();


    -- First, BEN SSNs
            
    OPEN GET_TARGET_SSN_CUR;
    LOOP FETCH GET_TARGET_SSN_CUR BULK COLLECT INTO L_BEN_SSNs_NT LIMIT BATCH_READ_SIZE;
    EXIT WHEN L_BEN_SSNs_NT.COUNT = 0;  

    -- So now we have a batch of BEN_SSNs, so we need a bulk insert
    FORALL L_INDEX IN 1 .. L_BEN_SSNs_NT.COUNT SAVE EXCEPTIONS
        INSERT INTO TARGET_ALL_SSN (SSN) VALUES
        (
            L_BEN_SSNs_NT(L_INDEX).SSN
        );

    END LOOP;
    CLOSE GET_TARGET_SSN_CUR;
    COMMIT;

    -- Now, EMP PSNs

    OPEN GET_TARGET_EMP_PSN_CUR;
    LOOP FETCH GET_TARGET_EMP_PSN_CUR BULK COLLECT INTO L_EMP_PSNs_NT LIMIT BATCH_READ_SIZE;
    EXIT WHEN L_EMP_PSNs_NT.COUNT = 0;  

    
    FORALL L_INDEX IN 1 .. L_EMP_PSNs_NT.COUNT SAVE EXCEPTIONS
        INSERT INTO TARGET_ALL_PSN (PSN) VALUES
        (
            L_EMP_PSNs_NT(L_INDEX).PSN
        );

    END LOOP;
    CLOSE GET_TARGET_EMP_PSN_CUR;
    COMMIT;

    -- Now BEN PSNs

    OPEN GET_TARGET_BEN_PSN_CUR;
    LOOP FETCH GET_TARGET_BEN_PSN_CUR BULK COLLECT INTO L_BEN_PSNs_NT LIMIT BATCH_READ_SIZE;
    EXIT WHEN L_BEN_PSNs_NT.COUNT = 0;  

    FORALL L_INDEX IN 1 .. L_BEN_PSNs_NT.COUNT SAVE EXCEPTIONS
        INSERT INTO TARGET_ALL_PSN (PSN) VALUES
        (
            L_BEN_PSNs_NT(L_INDEX).PSN
        );

    END LOOP;
    CLOSE GET_TARGET_BEN_PSN_CUR;
    COMMIT;



    l_completed_time := DBMS_UTILITY.get_time;
    l_elapsed_seconds := (l_completed_time - l_start_time) / 100;
    l_elapsed_minutes := trunc(l_elapsed_seconds / 60, 0);
    
    dbms_output.put_line(chr(10) || '[TIME] Fill in TARGET ALL TABLES duration: ' 
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

