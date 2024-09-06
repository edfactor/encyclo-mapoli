/*

This script does three things to clear out identifying field data outside
of DEMOGRAPHICS and PAYBEN:

- Clear out all Employee IDs in the PROF_DIST_REQ table as the field is 
the wrong size and is unlikely to be used

- Change all non-null "other reason" free text fields in the PROF_DIST_REQ 
table to a generic value for all rows 

- Clear out all third-party field data in PROFDIST

- Also in PROFDIST, where the PROFDIST_SEX field is not M or F, change that
to F. 

Note: These are big, simple operations. They do not need the types of 
performance patterns seen elsewhere in this application

*/

DECLARE

    L_SQL_ERROR NUMBER;
    L_SQL_ERRM VARCHAR2(512);
    
    -- Variables for performance timing
    l_start_time NUMBER;
    l_completed_time NUMBER;
    l_elapsed_minutes NUMBER;
    l_elapsed_seconds NUMBER;


    BATCH_READ_SIZE PLS_INTEGER := 100;

    l_last_ssnno NUMBER;

    -- We just need the references so we can loop through for updates
    CURSOR GET_PROFIT_SS_DETAIL_CUR IS
    SELECT
        NULL AS PROFIT_SS_YEAR,
        NULL AS PROFIT_SS_CLIENT,
        NULL AS PROFIT_SS_CODE,
        NULL AS PROFIT_SS_CONT,
        NULL AS PROFIT_SS_EARN,
        NULL AS PROFIT_SS_FORT,
        NULL AS PROFIT_SS_MDTE,
        NULL AS PROFIT_SS_YDTE,
        GET_SPACES_FOR_COLUMN('PROFIT_SS_DETAIL', 'PROFIT_SS_CMNT') AS PROFIT_SS_CMNT,
        NULL AS PROFIT_SS_ZEROCONT,
        NULL AS PROFIT_SS_FED_TAXES,
        NULL AS PROFIT_SS_STATE_TAXES,
        NULL AS PROFIT_SS_TAX_CODE,
        PDSS.PR_SS_D_S_SEC_NUMBER AS PROFIT_SS_SSNO,
        GET_SPACES_FOR_COLUMN('PROFIT_SS_DETAIL', 'PROFIT_SS_NAME') AS PROFIT_SS_NAME,
        GET_SPACES_FOR_COLUMN('PROFIT_SS_DETAIL', 'PROFIT_SS_ADDRESS') AS PROFIT_SS_ADDRESS,
        GET_SPACES_FOR_COLUMN('PROFIT_SS_DETAIL', 'PROFIT_SS_CITY') AS PROFIT_SS_CITY,
        GET_SPACES_FOR_COLUMN('PROFIT_SS_DETAIL', 'PROFIT_SS_STATE') AS PROFIT_SS_STATE,
        0 AS PROFIT_SS_ZIP,
        PDSS.PROFIT_SS_DET_RECNO,
        PDSS.PROFIT_SS_DET_PR_SS_D_S_SEQNUM,
        PDSS.PR_SS_D_S_SEC_NUMBER       
    FROM PROFIT_SS_DETAIL PDSS ORDER BY PDSS.PR_SS_D_S_SEC_NUMBER;

    -- 
    TYPE PROFIT_SS_NT IS TABLE OF PROFIT_SS_DETAIL%ROWTYPE;
    L_PROFIT_SS_NT PROFIT_SS_NT;

    l_profit_ss_detail_record PROFIT_SS_DETAIL_RECORD;

BEGIN
    l_start_time := DBMS_UTILITY.get_time;
    DBMS_OUTPUT.PUT_LINE(chr(10) || '- - - Begin obfuscation of other data fields - - -');
    DBMS_OUTPUT.PUT_LINE('Clearing out unused profit share checks employee IDs...');
    UPDATE PROFIT_SHARE_CHECKS
    SET
        EMPLOYEE_NUMBER = NULL;
    DBMS_OUTPUT.PUT_LINE('...done');
    DBMS_OUTPUT.PUT_LINE('Clearing out hardship free-text reason fields...');
    UPDATE PROFIT_DIST_REQ
    SET
        PROFIT_DIST_REQ_REASON_OTHER = 'Other reason'
    WHERE
        PROFIT_DIST_REQ_REASON_OTHER IS NOT NULL;
    DBMS_OUTPUT.PUT_LINE('...done');
    
    DBMS_OUTPUT.PUT_LINE('Clearing out third party distribution fields...');
    
    UPDATE PROFDIST
    SET
        PROFDIST_3RDPAYTO = GET_SPACES_FOR_COLUMN('PROFDIST', 'PROFDIST_3RDPAYTO'),
        PROFDIST_3RDNAME = GET_SPACES_FOR_COLUMN('PROFDIST', 'PROFDIST_3RDNAME'),
        PROFDIST_3RDADDR1 = GET_SPACES_FOR_COLUMN('PROFDIST', 'PROFDIST_3RDADDR1'),
        PROFDIST_3RDADDR2 = GET_SPACES_FOR_COLUMN('PROFDIST', 'PROFDIST_3RDADDR2'),
        PROFDIST_3RDCITY = GET_SPACES_FOR_COLUMN('PROFDIST', 'PROFDIST_3RDCITY'),
        PROFDIST_3RDSTATE = GET_SPACES_FOR_COLUMN('PROFDIST', 'PROFDIST_3RDSTATE'),
        PROFDIST_3RDACCT = GET_SPACES_FOR_COLUMN('PROFDIST', 'PROFDIST_3RDACCT'),
        PROFDIST_FBOPAYTO = GET_SPACES_FOR_COLUMN('PROFDIST', 'PROFDIST_FBOPAYTO'),
        PROFDIST_FBOTYPE = GET_SPACES_FOR_COLUMN('PROFDIST', 'PROFDIST_FBOTYPE'),
        PROFDIST_3RDZIP1 = 0;

        
    DBMS_OUTPUT.PUT_LINE('...done');

    DBMS_OUTPUT.PUT_LINE('Clearing out distribution gender outlier fields...');

    -- Remove PROFDIST_SEX outlier cases
    UPDATE PROFDIST
    SET PROFDIST_SEX = 'F'
    WHERE PROFDIST_SEX NOT IN ('M', 'F');
    
    DBMS_OUTPUT.PUT_LINE('...done');


    -- Obfuscate small number of PROFIT_SS_DETAIL records. We aren't doing the main employee SSN here,
    -- just all the PROFIT_DETAIL-ish fields and the PROFIT_SS_SECNO - which is an unknown legacy
    -- SSN that is neither from DEMOGRAPHICS, PAYBEN, nor PAYREL.

    -- We need to loop through all of them 

    DBMS_OUTPUT.PUT_LINE('Obfuscating some PROFIT_DETAIL_SS record fields...');

    l_last_ssnno := 0;


    L_PROFIT_SS_NT := PROFIT_SS_NT();

    OPEN GET_PROFIT_SS_DETAIL_CUR;
    LOOP FETCH GET_PROFIT_SS_DETAIL_CUR BULK COLLECT INTO L_PROFIT_SS_NT LIMIT BATCH_READ_SIZE;
    EXIT WHEN L_PROFIT_SS_NT.COUNT = 0;                   

        -- We are going to obscure the SSN and randomize the name and address info.
        FOR SS_INDEX IN 1..L_PROFIT_SS_NT.COUNT
        LOOP
            -- If this is another record for the same person, don't generate new
            -- random fields, but re-use
            IF (L_PROFIT_SS_NT(SS_INDEX).PROFIT_SS_SSNO = l_last_ssnno) THEN
                -- So reuse the last record
                L_PROFIT_SS_NT(SS_INDEX).PROFIT_SS_SSNO := l_profit_ss_detail_record.PROFIT_SS_SSNO;
                L_PROFIT_SS_NT(SS_INDEX).PROFIT_SS_NAME := l_profit_ss_detail_record.PROFIT_SS_NAME;
                L_PROFIT_SS_NT(SS_INDEX).PROFIT_SS_ADDRESS := l_profit_ss_detail_record.PROFIT_SS_ADDRESS;
                L_PROFIT_SS_NT(SS_INDEX).PROFIT_SS_CITY := l_profit_ss_detail_record.PROFIT_SS_CITY;
                L_PROFIT_SS_NT(SS_INDEX).PROFIT_SS_STATE := l_profit_ss_detail_record.PROFIT_SS_STATE;
                L_PROFIT_SS_NT(SS_INDEX).PROFIT_SS_ZIP := l_profit_ss_detail_record.PROFIT_SS_ZIP; 
            ELSE
                -- We need to get a new record
                l_last_ssnno :=  L_PROFIT_SS_NT(SS_INDEX).PROFIT_SS_SSNO;
                l_profit_ss_detail_record := GET_PROFIT_SS_DETAIL_RECORD(1234567);
                L_PROFIT_SS_NT(SS_INDEX).PROFIT_SS_SSNO := l_profit_ss_detail_record.PROFIT_SS_SSNO;
                L_PROFIT_SS_NT(SS_INDEX).PROFIT_SS_NAME := l_profit_ss_detail_record.PROFIT_SS_NAME;
                L_PROFIT_SS_NT(SS_INDEX).PROFIT_SS_ADDRESS := l_profit_ss_detail_record.PROFIT_SS_ADDRESS;
                L_PROFIT_SS_NT(SS_INDEX).PROFIT_SS_CITY := l_profit_ss_detail_record.PROFIT_SS_CITY;
                L_PROFIT_SS_NT(SS_INDEX).PROFIT_SS_STATE := l_profit_ss_detail_record.PROFIT_SS_STATE;
                L_PROFIT_SS_NT(SS_INDEX).PROFIT_SS_ZIP := l_profit_ss_detail_record.PROFIT_SS_ZIP;
            END IF;
        END LOOP;

        -- Now we have all the updates we need. Time to process them in batches

        FORALL L_INDEX IN 1 .. L_PROFIT_SS_NT.COUNT SAVE EXCEPTIONS
        UPDATE PROFIT_SS_DETAIL 
        SET 
            PROFIT_SS_SSNO = L_PROFIT_SS_NT(L_INDEX).PROFIT_SS_SSNO,
            PROFIT_SS_NAME = L_PROFIT_SS_NT(L_INDEX).PROFIT_SS_NAME,
            PROFIT_SS_ADDRESS = L_PROFIT_SS_NT(L_INDEX).PROFIT_SS_ADDRESS,
            PROFIT_SS_CITY = L_PROFIT_SS_NT(L_INDEX).PROFIT_SS_CITY,
            PROFIT_SS_STATE = L_PROFIT_SS_NT(L_INDEX).PROFIT_SS_STATE,
            PROFIT_SS_ZIP = L_PROFIT_SS_NT(L_INDEX).PROFIT_SS_ZIP
   
        WHERE
            PROFIT_SS_DET_RECNO = L_PROFIT_SS_NT(L_INDEX).PROFIT_SS_DET_RECNO;

    END LOOP;
    
    CLOSE GET_PROFIT_SS_DETAIL_CUR;

    DBMS_OUTPUT.PUT_LINE('...done');

    DBMS_OUTPUT.PUT_LINE('- - - Other data field obfuscation completed successfully. - - -');
    
    l_completed_time := DBMS_UTILITY.get_time;
    l_elapsed_seconds := (l_completed_time - l_start_time) / 100;
    l_elapsed_minutes := trunc(l_elapsed_seconds / 60, 0);
    
    dbms_output.put_line(chr(10) || '[TIME] Other Field Obfuscation duration: ' 
    || l_elapsed_minutes || ' minutes + ' || mod(l_elapsed_seconds, 60) || ' seconds.'); 

EXCEPTION
    WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('error in other data field obfuscation '
                             || SQLCODE);
         DBMS_OUTPUT.PUT_LINE('message '
         || SQLERRM);
         
        INSERT INTO log_messages (CALLER,  SQL_ERROR_CODE, ERROR_MESSAGE, CREATED_ON) 
            VALUES ('Obfuscate other data fields', L_SQL_ERROR, L_SQL_ERRM, sysdate); 
    
END;