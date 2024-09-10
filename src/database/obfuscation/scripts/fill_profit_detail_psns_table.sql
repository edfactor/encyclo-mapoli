/*

This script analyzes the PROFIT_CMNT fields of the PROFIT_DETAIL
table. It looks for rows with profit code 5 or 6, for account
transfers that will have PSNs in it.

It then takes the RECNO key and the PROFIT_CMNT field (which it
breaks into two pieces) and puts the rows into PROFIT_CMNT_PSN 
for later processing in the functions OBSCURE_EMPLOYEE_PSN 
and OBSCURE_BENEFICIARY_PSN. (The need to do this task will be
stored in PSN_TASKS and detected and saved by the functions 
FILL_PSN_TABLES_EMPLOYEE and FILL_PSN_TABLES_BENEFICIARY)

*/

DECLARE

    -- This is for batch reading of tables into nested tables
    BATCH_READ_SIZE  PLS_INTEGER := 200;

    -- This is the record type that will represent what we 
    -- gather
    TYPE PROF_CMNT_REC IS RECORD ( 
        PROFIT_DET_RECNO NUMBER(9),
        PROF_CMNT VARCHAR2(16)
    );
    TYPE PROF_CMNT_NT IS TABLE OF PROF_CMNT_REC;
    L_PROF_CMNT_NT PROF_CMNT_NT;
    
    -- This is the record type of what we will be sending
    -- to the PROFIT_CMNT_PSN table to be used in a bulk
    -- FORALL bulk insert.

    TYPE PROF_CMNT_PSN_NT IS TABLE OF PROFIT_CMNT_PSN%ROWTYPE;
    L_PROF_CMNT_PSN_NT PROF_CMNT_PSN_NT;

    L_TRIM_CMNT VARCHAR(16);
    L_FIRST_PART VARCHAR(16);
    L_PSN NUMBER;
    L_PART_LENGTH NUMBER;
    L_LAST_INDEX NUMBER;
    
    L_SQL_ERROR NUMBER;
    L_SQL_ERRM  VARCHAR2(512);
    
     -- Variables for performance timing
    l_start_time NUMBER;
    l_completed_time NUMBER;
    l_elapsed_minutes NUMBER;
    l_elapsed_seconds NUMBER;

    -- We are going to get only rows with 
    -- profit codes of 5 or 6, which can
    -- have the XFER > or QDRO > comments
    CURSOR GET_PROF_DETAIL_CMNT_CUR IS
    SELECT
        PROFIT_DET_RECNO, PROFIT_CMNT
    FROM PROFIT_DETAIL
    WHERE 
        PROFIT_CODE IN ('5', '6', '9');
    
BEGIN
    l_start_time := DBMS_UTILITY.get_time;

    DBMS_OUTPUT.PUT_LINE(chr(10) || '- - - Begin preparation for PROFIT DETAIL CMNT PSNs - - -');

    L_PROF_CMNT_NT := PROF_CMNT_NT();
    L_PROF_CMNT_PSN_NT := PROF_CMNT_PSN_NT();
    
    OPEN GET_PROF_DETAIL_CMNT_CUR;
    LOOP FETCH GET_PROF_DETAIL_CMNT_CUR BULK COLLECT INTO L_PROF_CMNT_NT LIMIT BATCH_READ_SIZE;
    EXIT WHEN L_PROF_CMNT_NT.COUNT = 0;  

        -- Initialize the bulk write NT between loops as it is not tied
        -- to the cursor loop
        L_PROF_CMNT_PSN_NT.DELETE;
        
        FOR L_INDEX IN 1..L_PROF_CMNT_NT.COUNT 
        LOOP

            EXTRACT_COMMENT_PARTS(L_PROF_CMNT_NT(L_INDEX).PROF_CMNT, L_FIRST_PART, L_PSN);
            
            IF (L_PSN != 0) THEN
                -- Now we need to build up our collection for the bulk insert
                L_PROF_CMNT_PSN_NT.EXTEND;
                L_LAST_INDEX := L_PROF_CMNT_PSN_NT.LAST;
                L_PROF_CMNT_PSN_NT(L_LAST_INDEX).PROFIT_DET_RECNO := L_PROF_CMNT_NT(L_INDEX).PROFIT_DET_RECNO;
                L_PROF_CMNT_PSN_NT(L_LAST_INDEX).PROFIT_CMNT_PREFIX := L_FIRST_PART;
                L_PROF_CMNT_PSN_NT(L_LAST_INDEX).PROFIT_PSN := L_PSN;
            ELSE
                DBMS_OUTPUT.put_line('Could not process PROFIT_DETAIL PROFIT_CMNT for rec no: ' || L_PROF_CMNT_NT(L_INDEX).PROFIT_DET_RECNO);
                DBMS_OUTPUT.put_line('Comment was: ' || L_PROF_CMNT_NT(L_INDEX).PROF_CMNT);
            END IF;
          
        END LOOP;
          
        
        FORALL L_INDEX IN 1 .. L_PROF_CMNT_PSN_NT.COUNT SAVE EXCEPTIONS
            INSERT INTO PROFIT_CMNT_PSN (
                PROFIT_DET_RECNO,
                PROFIT_CMNT_PREFIX,
                PROFIT_PSN
            ) VALUES (
                L_PROF_CMNT_PSN_NT(L_INDEX).PROFIT_DET_RECNO,
                L_PROF_CMNT_PSN_NT(L_INDEX).PROFIT_CMNT_PREFIX,
                L_PROF_CMNT_PSN_NT(L_INDEX).PROFIT_PSN
            );

            
    -- End cursor loop
    END LOOP;

    CLOSE GET_PROF_DETAIL_CMNT_CUR;

    COMMIT;
    
    l_completed_time := DBMS_UTILITY.get_time;
    l_elapsed_seconds := (l_completed_time - l_start_time) / 100;
    l_elapsed_minutes := trunc(l_elapsed_seconds / 60, 0);

    dbms_output.put_line(chr(10) || '[TIME] PROFIT_CMNT PSN prep duration: ' 
    || l_elapsed_minutes || ' minutes + ' || mod(l_elapsed_seconds, 60) || ' seconds.'); 
    
    
EXCEPTION
        
        WHEN OTHERS THEN
            L_SQL_ERROR := SQLCODE;
            L_SQL_ERRM := SQLERRM;
            dbms_output.put_line('ERROR_STACK: ' || DBMS_UTILITY.FORMAT_ERROR_STACK);
            dbms_output.put_line('ERROR_BACKTRACE: ' || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE);
            DBMS_OUTPUT.PUT_LINE('There was an unhandled exeception in Preparing for processing of PROFIT_SS_CMNT PSN. ');
            DBMS_OUTPUT.PUT_LINE('Error number: '
                                 || L_SQL_ERROR
                                 || ' Message: '
                                 || L_SQL_ERRM);
            INSERT INTO log_messages (CALLER,  SQL_ERROR_CODE, ERROR_MESSAGE, CREATED_ON) 
            VALUES ('Obscure PROFIT_CMNT PSN', L_SQL_ERROR, L_SQL_ERRM, sysdate);

END;