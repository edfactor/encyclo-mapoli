/*
    This script handles the problem of SSNs changing over time. But it 
    will also handle any records that got disconnected from employees 
    and beneficiaries, and delete them.

    There can be records outside of PAYBEN and DEMOGRAPHICS with SSNs 
    and real names and addresses, that were not handled by the obfuscation 
    process up until this point, which involved starting with DEMOGRAPHICS 
    and PAYBEN rows, and then changing references to them in other tables.
    
    At this point, we have already created "index tables" that just 
    have the SSN and PSN values for every table in scope, as well as 
    SSN and PSN task tables, which know what old values turned into 
    which new values.
    
    This means we can get a list of SSN orphans for each table, and then delete those rows.

    Note: The orphan SSN and PSN values will remain in the lookup tables. This script merely deletes rows containing them out of the output data set.

*/
DECLARE

    L_SQL_ERROR NUMBER;
    L_SQL_ERRM  VARCHAR2(512);

    
     -- Variables for performance timing
    l_start_time NUMBER;
    l_completed_time NUMBER;
    l_elapsed_minutes NUMBER;
    l_elapsed_seconds NUMBER;
    
    --- We need a nested table that can store what we can know about an employee or beneficiary record

    TYPE ORPHAN_SSN_NT IS TABLE OF NUMBER(9); 
    TYPE ORPHAN_PSN_NT IS TABLE OF NUMBER(11); 
        
    L_ORPHAN_SSN_NT ORPHAN_SSN_NT;
    L_ORPHAN_PSN_NT ORPHAN_PSN_NT;

    -- This is needed as a temp variable when orphan counts are high and 
    -- we do not wish to overwhelm the log by logging individual ophans
    l_orphan_temp PLS_INTEGER;

    BATCH_READ_SIZE            PLS_INTEGER := 100; 

    CURSOR GET_PROFIT_CMNT_PSN_CUR IS
    SELECT
        *
    FROM PROFIT_CMNT_PSN WHERE DONE = 0;

    CURSOR GET_PROFIT_SS_CMNT_PSN_CUR IS
    SELECT
        *
    FROM PROFIT_SS_CMNT_PSN WHERE DONE = 0;

    -- Our Nested Table types
    TYPE CMNT_PSN_RECORDS_NT IS TABLE OF PROFIT_CMNT_PSN%ROWTYPE INDEX BY PLS_INTEGER;
    L_CMNT_PSN_RECORDS_NT CMNT_PSN_RECORDS_NT;

    TYPE CMNT_SS_PSN_RECORDS_NT IS TABLE OF PROFIT_SS_CMNT_PSN%ROWTYPE INDEX BY PLS_INTEGER;
    L_CMNT_SS_PSN_RECORDS_NT CMNT_SS_PSN_RECORDS_NT;

    TYPE PSN_STORAGE_NT IS TABLE OF NUMBER(11);
    L_PSN_STORAGE_NT PSN_STORAGE_NT;

    -- Number of records
    NUM_RECORDS NUMBER := 0;

    L_ERROR_COUNT NUMBER;

    -- This is the record type that will represent what we 
    -- gather
    TYPE PROF_CMNT_REC IS RECORD ( 
        PROFIT_DET_RECNO NUMBER(9),
        PROF_CMNT VARCHAR(16)
    );
    TYPE PROF_CMNT_NT IS TABLE OF PROF_CMNT_REC;
    L_PROF_CMNT_NT PROF_CMNT_NT;

    L_LAST_INDEX NUMBER;

    -- We need these for PSN orphan processing
    TYPE PSN_BASE_REC IS RECORD ( 
        PSN NUMBER(11),
        PSN_BASE NUMBER(7)
    );
    TYPE PSN_BASE_NT IS TABLE OF PSN_BASE_REC;
    L_PSN_BASE_NT PSN_BASE_NT;
    
BEGIN

    L_PSN_BASE_NT := PSN_BASE_NT();
    L_CMNT_PSN_RECORDS_NT := CMNT_PSN_RECORDS_NT();
    L_CMNT_SS_PSN_RECORDS_NT := CMNT_SS_PSN_RECORDS_NT();
    L_PROF_CMNT_NT := PROF_CMNT_NT();
    L_PSN_STORAGE_NT := PSN_STORAGE_NT();
    L_ORPHAN_PSN_NT := ORPHAN_PSN_NT();
    
    l_start_time := DBMS_UTILITY.get_time;
    DBMS_OUTPUT.put_line('***************************************');
    DBMS_OUTPUT.put_line('***** PSN/SSN Orphan Detection Report ******');
    DBMS_OUTPUT.put_line('***************************************');
    DBMS_OUTPUT.put_line(' ');
    DBMS_OUTPUT.put_line('Note: Rows WILL be deleted after they are found, but re-running this script');
    DBMS_OUTPUT.put_line('may still show those rows because it uses temporary lookup tables. If you check');
    DBMS_OUTPUT.put_line('the tables themselves, the rows will be gone after the first run. ');
    DBMS_OUTPUT.put_line(' ');
    
    
    -- First we will do PROFIT_DETAIL
    
    SELECT SSN
    BULK COLLECT INTO L_ORPHAN_SSN_NT
    FROM 
    (
    SELECT t1.SSN FROM PROFIT_DETAIL_SSN t1
    MINUS
    SELECT t2.SSN FROM DEMOGRAPHICS_SSN t2
    MINUS
    SELECT t3.SSN FROM PYBEN_PAYSSN t3
    );
    
    -- Log the original SSNs

    DBMS_OUTPUT.put_line('***************************************');
    DBMS_OUTPUT.put_line('PROFIT_DETAIL SSN Orphans:');
    DBMS_OUTPUT.put_line('There were ' || L_ORPHAN_SSN_NT.COUNT || ' PROFIT_DETAIL SSN orphans');

    IF (L_ORPHAN_SSN_NT.COUNT > 10) THEN
        l_orphan_temp := 10;
        DBMS_OUTPUT.PUT_LINE('More than 10 PROFIT_DETAIL SSN Orphans: ' || L_ORPHAN_SSN_NT.COUNT);
    ELSE
        l_orphan_temp := L_ORPHAN_SSN_NT.COUNT;
    END IF;
    
    FOR L_INDEX IN 1..l_orphan_temp
    LOOP
        DBMS_OUTPUT.PUT_LINE('PROFIT_DETAIL SSN Orphan: ' || L_ORPHAN_SSN_NT(L_INDEX));
    END LOOP;
    
    FORALL L_INDEX IN 1 .. L_ORPHAN_SSN_NT.COUNT SAVE EXCEPTIONS
        DELETE FROM PROFIT_DETAIL 
        WHERE
            PR_DET_S_SEC_NUMBER = L_ORPHAN_SSN_NT(L_INDEX);
    
   
    L_ORPHAN_SSN_NT.DELETE;
    
    -- Now PROFIT_SS_DETAIL Employee SSN
    
    SELECT SSN BULK COLLECT INTO L_ORPHAN_SSN_NT
    FROM 
    (
    SELECT t1.SSN FROM PROFIT_SS_DETAIL_SSN t1
    MINUS
    SELECT t2.SSN FROM DEMOGRAPHICS_SSN t2 
    MINUS
    SELECT t3.SSN FROM PYBEN_PAYSSN t3
    );

    DBMS_OUTPUT.put_line('***************************************');
    DBMS_OUTPUT.put_line('PROFIT_SS_DETAIL SSN Orphans:');
    DBMS_OUTPUT.put_line('There were ' || L_ORPHAN_SSN_NT.COUNT || ' PROFIT_SS_DETAIL SSN orphans');
    
    -- Log the orphans

    IF (L_ORPHAN_SSN_NT.COUNT > 10) THEN
        l_orphan_temp := 10;
        DBMS_OUTPUT.PUT_LINE('More than 10 PROFIT_SS_DETAIL SSN Orphans: ' || L_ORPHAN_SSN_NT.COUNT);
    ELSE
        l_orphan_temp := L_ORPHAN_SSN_NT.COUNT;
    END IF;
    
    FOR L_INDEX IN 1..L_orphan_temp
    LOOP
        DBMS_OUTPUT.PUT_LINE('PROFIT_SS_Detail Employee SSN Orphan: ' || L_ORPHAN_SSN_NT(L_INDEX));
    END LOOP;
    
    FORALL L_INDEX IN 1 .. L_ORPHAN_SSN_NT.COUNT SAVE EXCEPTIONS
        DELETE FROM PROFIT_SS_DETAIL 
        WHERE
            PR_SS_D_S_SEC_NUMBER = L_ORPHAN_SSN_NT(L_INDEX);
            
    L_ORPHAN_SSN_NT.DELETE;

    -- Now PROFDIST Employee SSN
    
    SELECT SSN BULK COLLECT INTO L_ORPHAN_SSN_NT
    FROM 
    (
    SELECT t1.SSN FROM PROFDIST_SSN t1
    MINUS
    SELECT t2.SSN FROM DEMOGRAPHICS_SSN t2 
    );
    
    DBMS_OUTPUT.put_line('***************************************');
    DBMS_OUTPUT.put_line('PROFDIST SSN Orphans:');
    DBMS_OUTPUT.put_line('There were ' || L_ORPHAN_SSN_NT.COUNT || ' PROFDIST SSN Orphans');
    -- Log the orphans

    IF (L_ORPHAN_SSN_NT.COUNT > 10) THEN
        l_orphan_temp := 10;
        DBMS_OUTPUT.PUT_LINE('More than 10 PROFDIST SSN Orphans: ' || L_ORPHAN_SSN_NT.COUNT);
    ELSE
        l_orphan_temp := L_ORPHAN_SSN_NT.COUNT;
    END IF;
    
    FOR L_INDEX IN 1..l_orphan_temp
    LOOP
        DBMS_OUTPUT.PUT_LINE('PROFDIST Employee SSN Orphan: ' || L_ORPHAN_SSN_NT(L_INDEX));
    END LOOP;
    
    FORALL L_INDEX IN 1 .. L_ORPHAN_SSN_NT.COUNT SAVE EXCEPTIONS
        DELETE FROM PROFDIST 
        WHERE
            PROFDIST_SSN = L_ORPHAN_SSN_NT(L_INDEX);
            
    L_ORPHAN_SSN_NT.DELETE;

    -- Now SOC_SEC_REC Employee SSN
    
    SELECT SSN BULK COLLECT INTO L_ORPHAN_SSN_NT
    FROM 
    (
    SELECT t1.SSN FROM SOC_SEC_SSN t1
    MINUS
    SELECT t2.SSN FROM DEMOGRAPHICS_SSN t2 
    MINUS
    SELECT t3.SSN FROM PYBEN_PAYSSN t3
    );

    DBMS_OUTPUT.put_line('***************************************');
    DBMS_OUTPUT.put_line('SOC_SEC_REC SSN Orphans:');
    DBMS_OUTPUT.put_line('There were ' || L_ORPHAN_SSN_NT.COUNT || ' SOC_SEC_REC SSN Orphans');
    
    -- Log the orphans

    IF (L_ORPHAN_SSN_NT.COUNT > 10) THEN
        l_orphan_temp := 10;
        DBMS_OUTPUT.PUT_LINE('More than 10 SOC_SEC_REC SSN Orphans: ' || L_ORPHAN_SSN_NT.COUNT);
    ELSE
        l_orphan_temp := L_ORPHAN_SSN_NT.COUNT;
    END IF;
    
    FOR L_INDEX IN 1..l_orphan_temp
    LOOP
        DBMS_OUTPUT.PUT_LINE('SOC SEC_REC_SSN Orphan: ' || L_ORPHAN_SSN_NT(L_INDEX));
    END LOOP;

    FORALL L_INDEX IN 1 .. L_ORPHAN_SSN_NT.COUNT SAVE EXCEPTIONS
        DELETE FROM SOC_SEC_REC
        WHERE
            SOC_SEC_NUMBER = L_ORPHAN_SSN_NT(L_INDEX);

    L_ORPHAN_SSN_NT.DELETE;

    -- Now PROFDIST Beneficiary SSN
    
    /*

    This orphan code can be used if you want any PAYSSN field
    that does not match beneficiary SSNs to be an orphan 
    and deleted. But it is more likely that you'd want to 
    mark something an orphan if this did not match on records
    where there was no PROFDIST_SSN field. To do that, you'd 
    have to create a new lookup table like PROFDIST_SOLO_PAYSSN
    that would only have PAYSSNs from rows where there was no 
    employee SSN.

    SELECT SSN
    BULK COLLECT INTO L_ORPHAN_SSN_NT
    FROM 
    (
    SELECT t1.SSN FROM PROFDIST_PAYSSN t1
    MINUS
    SELECT t2.SSN FROM PYBEN_PAYSSN t2 
    );
    
    -- Log the orphans

    IF (L_ORPHAN_SSN_NT.COUNT > 10) THEN
        l_orphan_temp := 10;
        DBMS_OUTPUT.PUT_LINE('More than 10 PROFDIST Beneficiary SSN Orphans: ' || L_ORPHAN_SSN_NT.COUNT);
    ELSE
        l_orphan_temp := L_ORPHAN_SSN_NT;
    END IF;
    
    FOR L_INDEX IN 1..l_orphan_temp
    LOOP
        DBMS_OUTPUT.PUT_LINE('PROFDIST Beneficiary SSN Orphan: ' || L_ORPHAN_SSN_NT(L_INDEX));
    END LOOP;
    
    
    FORALL L_INDEX IN 1 .. L_ORPHAN_SSN_NT.COUNT SAVE EXCEPTIONS
        DELETE FROM PROFDIST 
        WHERE
            PROFDIST_PAYSSN = L_ORPHAN_SSN_NT(L_INDEX);
            
          
     L_ORPHAN_SSN_NT.DELETE;

     */
     
     -- There are now two kinds of Profit Share Checks records to be sought and deleted
    SELECT SSN
    BULK COLLECT INTO L_ORPHAN_SSN_NT
    FROM 
    (
    SELECT t1.SSN FROM PS_CHECKS_EMP_SSN t1
    MINUS
    SELECT t2.SSN FROM DEMOGRAPHICS_SSN t2 
    );

    DBMS_OUTPUT.put_line('***************************************');
    DBMS_OUTPUT.put_line('PROFIT_SHARE_CHECKS Employee SSN Orphans:');
    DBMS_OUTPUT.put_line('There were ' || L_ORPHAN_SSN_NT.COUNT || ' PROFIT_SHARE_CHECKS SSN Orphans');
    
    -- Log the orphans

    IF (L_ORPHAN_SSN_NT.COUNT > 10) THEN
        l_orphan_temp := 10;
        DBMS_OUTPUT.PUT_LINE('More than 10 PROFIT_SHARE_CHECKS SSN Orphans: ' || L_ORPHAN_SSN_NT.COUNT);
    ELSE
        l_orphan_temp := L_ORPHAN_SSN_NT.COUNT;
    END IF;

    FOR L_INDEX IN 1..l_orphan_temp
    LOOP
        DBMS_OUTPUT.PUT_LINE('PROFIT_SHARE_CHECKS Employee SSN Orphan: ' || L_ORPHAN_SSN_NT(L_INDEX));
    END LOOP;
    
    
    FORALL L_INDEX IN 1 .. L_ORPHAN_SSN_NT.COUNT SAVE EXCEPTIONS
        DELETE FROM PROFIT_SHARE_CHECKS
        WHERE
            EMPLOYEE_SSN = L_ORPHAN_SSN_NT(L_INDEX);
            
          
     L_ORPHAN_SSN_NT.DELETE;


    -- Now PAYREL SSN for  
    SELECT SSN
    BULK COLLECT INTO L_ORPHAN_SSN_NT
    FROM 
    (
    SELECT t1.SSN FROM PYREL_PAYSSN t1
    MINUS
    SELECT t2.SSN FROM PYBEN_PAYSSN t2
    );

    DBMS_OUTPUT.put_line('***************************************');
    DBMS_OUTPUT.put_line('PAYREL SSN Orphans:');
    DBMS_OUTPUT.put_line('There were ' || L_ORPHAN_SSN_NT.COUNT || ' PAYREL SSN Orphans');
    
    -- Log the orphans

    IF (L_ORPHAN_SSN_NT.COUNT > 10) THEN
        l_orphan_temp := 10;
        DBMS_OUTPUT.PUT_LINE('More than 10 PAYREL SSN Orphans: ' || L_ORPHAN_SSN_NT.COUNT);
    ELSE
        l_orphan_temp := L_ORPHAN_SSN_NT.COUNT;
    END IF;

    FOR L_INDEX IN 1..l_orphan_temp
    LOOP
        DBMS_OUTPUT.PUT_LINE('PAYREL Beneficiary SSN Orphan: ' || L_ORPHAN_SSN_NT(L_INDEX));
    END LOOP;
    
    
    FORALL L_INDEX IN 1 .. L_ORPHAN_SSN_NT.COUNT SAVE EXCEPTIONS
        DELETE FROM PAYREL
        WHERE
            PYREL_PAYSSN = L_ORPHAN_SSN_NT(L_INDEX);
            
          
     L_ORPHAN_SSN_NT.DELETE;

     /*
     
    -- Now beneficiary SSN for checks 
    SELECT SSN
    BULK COLLECT INTO L_ORPHAN_SSN_NT
    FROM 
    (
    SELECT t1.SSN FROM PS_CHECKS_SSN t1
    MINUS
    SELECT t2.SSN FROM PYBEN_PAYSSN t2 
    );
    
    -- Log the orphans

    IF (L_ORPHAN_SSN_NT.COUNT > 10) THEN
        l_orphan_temp := 10;
        DBMS_OUTPUT.PUT_LINE('More than 10 PROFIT_SHARE_CHECKS SSN Orphans: ' || L_ORPHAN_SSN_NT.COUNT);
    ELSE
        l_orphan_temp := L_ORPHAN_SSN_NT.COUNT;
    END IF;

    FOR L_INDEX IN 1..l_orphan_temp
    LOOP
        DBMS_OUTPUT.PUT_LINE('PROFIT_SHARE_CHECKS Beneficiary SSN Orphan: ' || L_ORPHAN_SSN_NT(L_INDEX));
    END LOOP;
    
    
    FORALL L_INDEX IN 1 .. L_ORPHAN_SSN_NT.COUNT SAVE EXCEPTIONS
        DELETE FROM PROFIT_SHARE_CHECKS
        WHERE
            SSN_NUMBER = L_ORPHAN_SSN_NT(L_INDEX);
            
          
     L_ORPHAN_SSN_NT.DELETE;

     */
    
    -- Now we need to do PSN orphans for distribution requests
    -- This is for the employee id part

    L_ORPHAN_PSN_NT.DELETE;

    SELECT PSN
    BULK COLLECT INTO L_ORPHAN_PSN_NT
    FROM 
    (
    SELECT t1.PSN FROM PROF_DIST_REQ_EMP t1
    MINUS
    SELECT t2.PSN FROM DEMOGRAPHICS_PSN t2
    );

    DBMS_OUTPUT.put_line('***************************************');
    DBMS_OUTPUT.put_line('PROF_DIST_REQ Employee Badge orphans:');
    DBMS_OUTPUT.put_line('There were ' || L_ORPHAN_PSN_NT.COUNT || ' PROF_DIST_REQ Employee Badge orphans');

    -- Here we do not need to get other values as we
    -- are just dealing with the 7-digit badges 

    FOR L_INDEX IN 1..L_ORPHAN_PSN_NT.COUNT 
    LOOP
        DBMS_OUTPUT.put_line('Missing DEMOGRAPHICS badge: ' ||  L_ORPHAN_PSN_NT(L_INDEX));
    END LOOP;

    FORALL M_INDEX IN 1..L_ORPHAN_PSN_NT.COUNT SAVE EXCEPTIONS
        DELETE FROM PROFIT_DIST_REQ
        WHERE
            PROFIT_DIST_REQ_EMP = L_ORPHAN_PSN_NT(M_INDEX);
            
            
     L_ORPHAN_PSN_NT.DELETE;
     L_PSN_BASE_NT.DELETE;

    -- This is the PSN orphans in PROFIT_DIST_REQ for beneficiary

    SELECT PSN_BASE
    BULK COLLECT INTO L_ORPHAN_PSN_NT
    FROM 
    (
    SELECT t1.PSN_BASE FROM PROF_DIST_PSN_BASES t1
    MINUS
    SELECT t2.PSN FROM DEMOGRAPHICS_PSN t2
    );

    DBMS_OUTPUT.put_line('***************************************');
    DBMS_OUTPUT.put_line('PROF_DIST_REQ PSN orphans:');
    DBMS_OUTPUT.put_line('There were ' || L_ORPHAN_PSN_NT.COUNT || ' PROF_DIST_REQ PSN orphans');

    -- Now we need to go get all the extended values

    -- Loop through the bases
    FOR L_INDEX IN 1..L_ORPHAN_PSN_NT.COUNT 
    LOOP

        -- Now loop through matching PSNs
        SELECT DISTINCT PSN
        BULK COLLECT INTO L_PSN_STORAGE_NT FROM PROF_DIST_PSN_BASES 
        WHERE PSN_BASE = L_ORPHAN_PSN_NT(L_INDEX);

        FOR O_INDEX IN 1..L_PSN_STORAGE_NT.COUNT
        LOOP

            L_PSN_BASE_NT.EXTEND;
            L_LAST_INDEX := L_PSN_BASE_NT.LAST;

            L_PSN_BASE_NT(L_LAST_INDEX).PSN_BASE := L_ORPHAN_PSN_NT(L_INDEX);
            L_PSN_BASE_NT(L_LAST_INDEX).PSN := L_PSN_STORAGE_NT(O_INDEX);

            DBMS_OUTPUT.put_line('Missing DEMOGRAPHICS badge: ' || 
            L_ORPHAN_PSN_NT(L_INDEX) || 
            ' for Beneficiary PSN: ' || L_PSN_STORAGE_NT(O_INDEX));
        END LOOP;
    END LOOP;
    
    
    FORALL L_INDEX IN 1 .. L_PSN_BASE_NT.COUNT SAVE EXCEPTIONS
        DELETE FROM PROFIT_DIST_REQ
        WHERE
            PROFIT_DIST_REQ_PSN = L_PSN_BASE_NT(L_INDEX).PSN;
            
            
     L_ORPHAN_PSN_NT.DELETE;
     L_PSN_BASE_NT.DELETE;
     L_PSN_STORAGE_NT.DELETE;

    -- Now we need to do PSN orphans for PAYREL
    
   SELECT PSN
    BULK COLLECT INTO L_ORPHAN_PSN_NT
    FROM 
    (
        SELECT t1.PSN FROM PYREL_PSN t1
        MINUS
        SELECT t2.PSN FROM PAYBEN_PSN t2
    );

    DBMS_OUTPUT.put_line('***************************************');
    DBMS_OUTPUT.put_line('PAYREL PSN orphans:');
    DBMS_OUTPUT.put_line('There were ' || L_ORPHAN_PSN_NT.COUNT || ' PAYREL PSN orphans');

    -- Now we need to go get all the extended values

    FOR L_INDEX IN 1..L_ORPHAN_PSN_NT.COUNT 
    LOOP
           
            DBMS_OUTPUT.put_line('Missing PAYREL PSN from PAYBEN: ' || 
            L_ORPHAN_PSN_NT(L_INDEX));
       
    END LOOP;
    
    FORALL L_INDEX IN 1 .. L_ORPHAN_PSN_NT.COUNT SAVE EXCEPTIONS
        DELETE FROM PAYREL
        WHERE
            PYREL_PSN= L_ORPHAN_PSN_NT(L_INDEX);
            
    L_PSN_BASE_NT.DELETE;
    L_ORPHAN_PSN_NT.DELETE;
    L_PSN_STORAGE_NT.DELETE;
     
     
-- Now we need to do PSN orphans for PAYBEN
    
   SELECT PSN_BASE
    BULK COLLECT INTO L_ORPHAN_PSN_NT
    FROM 
    (
    SELECT t1.PSN_BASE FROM PAYBEN_PSN_BASES t1
    MINUS
    SELECT t2.PSN FROM DEMOGRAPHICS_PSN t2
    );

    DBMS_OUTPUT.put_line('***************************************');
    DBMS_OUTPUT.put_line('PAYBEN PSN orphans:');
    DBMS_OUTPUT.put_line('There were ' || L_ORPHAN_PSN_NT.COUNT || ' PAYBEN PSN orphans');

    -- Now we need to go get all the extended values

    FOR L_INDEX IN 1..L_ORPHAN_PSN_NT.COUNT 
    LOOP

        -- Now loop through matching PSNs
        SELECT DISTINCT PSN
        BULK COLLECT INTO L_PSN_STORAGE_NT FROM PAYBEN_PSN_BASES 
        WHERE PSN_BASE = L_ORPHAN_PSN_NT(L_INDEX);

        FOR O_INDEX IN 1..L_PSN_STORAGE_NT.COUNT
        LOOP

            L_PSN_BASE_NT.EXTEND;
            L_LAST_INDEX := L_PSN_BASE_NT.LAST;

            L_PSN_BASE_NT(L_LAST_INDEX).PSN_BASE := L_ORPHAN_PSN_NT(L_INDEX);
            L_PSN_BASE_NT(L_LAST_INDEX).PSN := L_PSN_STORAGE_NT(O_INDEX);

            DBMS_OUTPUT.put_line('Missing DEMOGRAPHICS badge: ' || 
            L_ORPHAN_PSN_NT(L_INDEX) || 
            ' PAYBEN PSN: ' || L_PSN_STORAGE_NT(O_INDEX));
        END LOOP;

       
    END LOOP;
    
    FORALL L_INDEX IN 1 .. L_PSN_BASE_NT.COUNT SAVE EXCEPTIONS
        DELETE FROM PAYBEN
        WHERE
            PYBEN_PSN= L_PSN_BASE_NT(L_INDEX).PSN;
            
    L_PSN_BASE_NT.DELETE;
    L_ORPHAN_PSN_NT.DELETE;
    L_PSN_STORAGE_NT.DELETE;

SELECT PSN
    BULK COLLECT INTO L_ORPHAN_PSN_NT
    FROM 
    (
    SELECT t1.PSN FROM PAYPROFIT_PSN t1
    MINUS
    SELECT t2.PSN FROM DEMOGRAPHICS_PSN t2
    );

    DBMS_OUTPUT.put_line('***************************************');
    DBMS_OUTPUT.put_line('PAYPROFIT Employee PSN orphans:');
    DBMS_OUTPUT.put_line('There were ' || L_ORPHAN_PSN_NT.COUNT || ' PAYPROFIT PSN orphans');

    -- Now we need to go get all the extended values

    FOR L_INDEX IN 1..L_ORPHAN_PSN_NT.COUNT 
    LOOP

        -- Now loop through matching PSNs
        SELECT DISTINCT PSN
        BULK COLLECT INTO L_PSN_STORAGE_NT FROM PAYPROFIT_PSN 
        WHERE PSN = L_ORPHAN_PSN_NT(L_INDEX);

        FOR O_INDEX IN 1..L_PSN_STORAGE_NT.COUNT
        LOOP

            L_PSN_BASE_NT.EXTEND;
            L_LAST_INDEX := L_PSN_BASE_NT.LAST;

            L_PSN_BASE_NT(L_LAST_INDEX).PSN_BASE := L_ORPHAN_PSN_NT(L_INDEX);
            L_PSN_BASE_NT(L_LAST_INDEX).PSN := L_PSN_STORAGE_NT(O_INDEX);

            DBMS_OUTPUT.put_line('Missing DEMOGRAPHICS badge: ' || 
            L_ORPHAN_PSN_NT(L_INDEX) || 
            ' PAYPROFIT PSN: ' || L_PSN_STORAGE_NT(O_INDEX));
        END LOOP;

    END LOOP;
    
    FORALL L_INDEX IN 1 .. L_PSN_BASE_NT.COUNT SAVE EXCEPTIONS
        DELETE FROM PAYPROFIT
        WHERE
            PAYPROF_BADGE= L_PSN_BASE_NT(L_INDEX).PSN;
            
     L_ORPHAN_PSN_NT.DELETE;
     L_PSN_BASE_NT.DELETE;
     L_PSN_STORAGE_NT.DELETE;
     
     
    -- PAYPROFIT Orphan SSN
    
    SELECT SSN
    BULK COLLECT INTO L_ORPHAN_SSN_NT
    FROM 
    (
    SELECT t1.SSN FROM PAYPROFIT_SSN t1
    MINUS
    SELECT t2.SSN FROM DEMOGRAPHICS_SSN t2 
    MINUS
    SELECT t3.SSN FROM PYBEN_PAYSSN t3
    );

    DBMS_OUTPUT.put_line('***************************************');
    DBMS_OUTPUT.put_line('PAYPROFIT Orphans:');
    DBMS_OUTPUT.put_line('There were ' || L_ORPHAN_SSN_NT.COUNT || ' PAYPROFIT Orphans');
    
    -- Log the orphans

    IF (L_ORPHAN_SSN_NT.COUNT > 10) THEN
        l_orphan_temp := 10;
        DBMS_OUTPUT.PUT_LINE('More than 10 PAYPROFIT SSN Orphans: ' || L_ORPHAN_SSN_NT.COUNT);
    ELSE
        l_orphan_temp := L_ORPHAN_SSN_NT.COUNT;
    END IF;
    
    FOR L_INDEX IN 1..l_orphan_temp
    LOOP
        DBMS_OUTPUT.PUT_LINE('PAYPROFIT Employee SSN Orphan: ' || L_ORPHAN_SSN_NT(L_INDEX));
    END LOOP;
    
    FORALL L_INDEX IN 1 .. L_ORPHAN_SSN_NT.COUNT SAVE EXCEPTIONS
        DELETE FROM PAYPROFIT
        WHERE
            PAYPROF_SSN= L_ORPHAN_SSN_NT(L_INDEX);
            
    L_ORPHAN_SSN_NT.DELETE; 

    -- Orphan PSNs inside of PROFIT_DETAIL and PROFIT_SS_DETAIL can happen 
    -- in the PROFIT_CMNT and PROFIT_SS_CMNT fields where there was QDROo
    -- or XFER values that did not match a PSN, either because it was a 
    -- check number, or because the PSN was not there. Either way, we should
    -- replace them with a randomized number

    -- First, let us see how many we have

    -- First, PROFIT_DETAIL

    DBMS_OUTPUT.put_line('***************************************');
    DBMS_OUTPUT.put_line('PROFIT_DETAIL PROFIT_CMNT PSN Unknown Values:');

    SELECT COUNT(*) INTO NUM_RECORDS FROM PROFIT_CMNT_PSN WHERE DONE = 0;
    DBMS_OUTPUT.put_line('Number of Unknown PROFIT_DETAIL PROFIT_CMNT PSN/check numbers: ' || NUM_RECORDS);

    OPEN GET_PROFIT_CMNT_PSN_CUR;
    LOOP FETCH GET_PROFIT_CMNT_PSN_CUR BULK COLLECT INTO L_CMNT_PSN_RECORDS_NT LIMIT BATCH_READ_SIZE;
    EXIT WHEN L_CMNT_PSN_RECORDS_NT.COUNT = 0;  

        -- So in here, we will just loop through, add each to a change records NT,
        -- and log each bad one along the way (up to 10)

        L_PROF_CMNT_NT.DELETE;

        FOR L_INDEX in 1..L_CMNT_PSN_RECORDS_NT.COUNT
        LOOP

            L_PROF_CMNT_NT.EXTEND;
            L_LAST_INDEX := L_PROF_CMNT_NT.LAST;
            L_PROF_CMNT_NT(L_LAST_INDEX).PROFIT_DET_RECNO := L_CMNT_PSN_RECORDS_NT(L_INDEX).PROFIT_DET_RECNO;
            
            IF (IS_PROBABLY_PSN(L_CMNT_PSN_RECORDS_NT(L_INDEX).PROFIT_PSN) = 1) THEN
                DBMS_OUTPUT.put_line('Comment will be:|' || 
                ASSEMBLE_COMMENT_TRANSFER(TRIM(L_CMNT_PSN_RECORDS_NT(L_INDEX).PROFIT_CMNT_PREFIX),
                TO_CHAR(PSN_RANDOM.NEXTVAL)) || '|');
                L_PROF_CMNT_NT(L_LAST_INDEX).PROF_CMNT := 
                ASSEMBLE_COMMENT_TRANSFER(TRIM(L_CMNT_PSN_RECORDS_NT(L_INDEX).PROFIT_CMNT_PREFIX),
                TO_CHAR(PSN_RANDOM.NEXTVAL));
            ELSE
                L_PROF_CMNT_NT(L_LAST_INDEX).PROF_CMNT := 
                ASSEMBLE_COMMENT_TRANSFER(TRIM(L_CMNT_PSN_RECORDS_NT(L_INDEX).PROFIT_CMNT_PREFIX),
                TO_CHAR(CHECK_REPL.NEXTVAL));
            END IF;
            
        END LOOP;

        -- Now we need to do the FORALL to do the bulk updates to PROFIT_DETAIL

        -- PROFIT_DETAIL
        FORALL L_INDEX IN 1 .. L_PROF_CMNT_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_DETAIL 
            SET 
                PROFIT_CMNT = L_PROF_CMNT_NT(L_INDEX).PROF_CMNT
            WHERE
                PROFIT_DET_RECNO = L_PROF_CMNT_NT(L_INDEX).PROFIT_DET_RECNO;


        -- And now we need to mark those tasks as done

        FORALL L_INDEX IN 1 .. L_PROF_CMNT_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_CMNT_PSN 
            SET 
                DONE = 1
            WHERE
                PROFIT_DET_RECNO = L_PROF_CMNT_NT(L_INDEX).PROFIT_DET_RECNO;


    END LOOP;

    CLOSE GET_PROFIT_CMNT_PSN_CUR;

    -- Now we have to do the same thing for PROFIT_SS_DETAIL

    DBMS_OUTPUT.put_line('***************************************');
    DBMS_OUTPUT.put_line('PROFIT_SS_DETAIL PROFIT_SS_CMNT PSN unknown values:');

    SELECT COUNT(*) INTO NUM_RECORDS FROM PROFIT_SS_CMNT_PSN WHERE DONE = 0;
    DBMS_OUTPUT.put_line('Number of unknown PROFIT_SS_DETAIL PROFIT_CMNT values: ' || NUM_RECORDS);

    OPEN GET_PROFIT_SS_CMNT_PSN_CUR;
    LOOP FETCH GET_PROFIT_SS_CMNT_PSN_CUR BULK COLLECT INTO L_CMNT_SS_PSN_RECORDS_NT LIMIT BATCH_READ_SIZE;
        EXIT WHEN L_CMNT_SS_PSN_RECORDS_NT.COUNT = 0;  

        -- So in here, we will just loop through, add each to a change records NT,
        -- and log each bad one along the way (up to 10)

        L_PROF_CMNT_NT.DELETE;

        FOR L_INDEX in 1..L_CMNT_SS_PSN_RECORDS_NT.COUNT
        LOOP

            L_PROF_CMNT_NT.EXTEND;
            L_LAST_INDEX := L_PROF_CMNT_NT.LAST;
            L_PROF_CMNT_NT(L_LAST_INDEX).PROFIT_DET_RECNO := L_CMNT_SS_PSN_RECORDS_NT(L_INDEX).PROFIT_SS_DET_RECNO;
            
            IF (IS_PROBABLY_PSN(L_CMNT_SS_PSN_RECORDS_NT(L_INDEX).PROFIT_SS_PSN) = 1) THEN
                L_PROF_CMNT_NT(L_LAST_INDEX).PROF_CMNT := 
                ASSEMBLE_COMMENT_TRANSFER(TRIM(L_CMNT_SS_PSN_RECORDS_NT(L_INDEX).PROFIT_SS_CMNT_PREFIX),
                TO_CHAR(PSN_RANDOM.NEXTVAL));
            ELSE
                L_PROF_CMNT_NT(L_LAST_INDEX).PROF_CMNT := 
                ASSEMBLE_COMMENT_TRANSFER(TRIM(L_CMNT_SS_PSN_RECORDS_NT(L_INDEX).PROFIT_SS_CMNT_PREFIX),
                TO_CHAR(CHECK_REPL.NEXTVAL));
            END IF;
            
            
            L_PROF_CMNT_NT(L_LAST_INDEX).PROF_CMNT := TRIM(L_CMNT_SS_PSN_RECORDS_NT(L_INDEX).PROFIT_SS_CMNT_PREFIX) ||
                TO_CHAR(PSN_REPL.NEXTVAL);

        END LOOP;

        -- Now we need to do the FORALL to do the bulk updates to PROFIT_DETAIL

        -- PROFIT_SS_DETAIL
        FORALL L_INDEX IN 1 .. L_PROF_CMNT_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_SS_DETAIL 
            SET 
                PROFIT_SS_CMNT = L_PROF_CMNT_NT(L_INDEX).PROF_CMNT
            WHERE
                PROFIT_SS_DET_RECNO = L_PROF_CMNT_NT(L_INDEX).PROFIT_DET_RECNO;

        -- And now we need to mark those tasks as done

        FORALL L_INDEX IN 1 .. L_PROF_CMNT_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_SS_CMNT_PSN 
            SET 
                DONE = 1
            WHERE
                PROFIT_SS_DET_RECNO = L_PROF_CMNT_NT(L_INDEX).PROFIT_DET_RECNO;

    END LOOP;

    CLOSE GET_PROFIT_SS_CMNT_PSN_CUR;
        
    COMMIT;    
    
    DBMS_OUTPUT.PUT_LINE('- - - Completed deletion of orphan IDs - - -');
    l_completed_time := DBMS_UTILITY.get_time;
    l_elapsed_seconds := (l_completed_time - l_start_time) / 100;
    l_elapsed_minutes := trunc(l_elapsed_seconds / 60, 0);
    
    dbms_output.put_line(chr(10) || '[TIME] TOTAL Deletion of Orphan IDs duration: ' 
    || l_elapsed_minutes || ' minutes + ' || mod(l_elapsed_seconds, 60) || ' seconds.'); 
    
EXCEPTION
        WHEN OTHERS THEN
            L_SQL_ERROR := SQLCODE;
            L_SQL_ERRM := SQLERRM;
            dbms_output.put_line('ERROR_STACK: ' || DBMS_UTILITY.FORMAT_ERROR_STACK);
            dbms_output.put_line('ERROR_BACKTRACE: ' || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE);
            DBMS_OUTPUT.PUT_LINE('There was an unhandled exeception in obfuscating orphan ids. ');
            DBMS_OUTPUT.PUT_LINE('Error number: '
                                 || L_SQL_ERROR
                                 || ' Message: '
                                 || L_SQL_ERRM);
            INSERT INTO log_messages (CALLER,  SQL_ERROR_CODE, ERROR_MESSAGE, CREATED_ON) 
            VALUES ('Orphan ID Obfuscation script', L_SQL_ERROR, L_SQL_ERRM, sysdate);
END;