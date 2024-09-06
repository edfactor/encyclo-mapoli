/*

This block obscures distribution state of residence. Most people
will get distributions in New England, or perhaps in Florida,
if they are retired.

However, for people that get distributions in other states, this
location could be used to help identify them. In these cases,
their distribution state will be set to Massachusetts.

This code will handle this in the following way:

It will bulk read in a couple of columns for every row that ends with
a space and two more characters - into a "raw" nested table.
(Columns will be record number and the raw profit comment string)

Then, it wil go through the raw table to create a second, processed
table that has the record number, and the comment field with the 
state changed to MA (we will not keep track of non-outlier fields)

Then, there will be a FORALL statement that updates the comment 
fields for rows (by record number) with the new comment value.

*/

DECLARE

    -- This is for batch reading of tables into nested tables
    BATCH_READ_SIZE  PLS_INTEGER := 200;

    -- Nested table type for two PROFIT DETAIL columns we want
    TYPE STATE_DIST IS RECORD ( 
        REC_NO NUMBER(9),
        PROF_COMMENT VARCHAR2(16)
    );
    
    TYPE OLD_STATE_NT IS TABLE OF STATE_DIST; 
    TYPE NEW_STATE_NT IS TABLE OF STATE_DIST;
    
    L_OLD_STATE_NT OLD_STATE_NT;
    L_NEW_STATE_NT NEW_STATE_NT;
    
    L_FIRST_PART VARCHAR(16);
    L_SECOND_PART VARCHAR(2);
    
    L_LAST_INDEX NUMBER;
    
    L_SQL_ERROR NUMBER;
    L_SQL_ERRM  VARCHAR2(512);
    
     -- Variables for performance timing
    l_start_time NUMBER;
    l_completed_time NUMBER;
    l_elapsed_minutes NUMBER;
    l_elapsed_seconds NUMBER;

    -- Ending with a space and two characters is what we want
    -- Note: Oracle is not letting us use regex_like. We are 
    -- hardcoding the beginning of the strings with ORs 
    -- as they aids the index in performance for lookups
    CURSOR GET_DETAIL_DIST_STATES_CUR IS
    SELECT
        PROFIT_DET_RECNO, PROFIT_CMNT
    FROM PROFIT_DETAIL
    WHERE 
        PROFIT_CMNT LIKE 'VOIDED % __' OR 
        PROFIT_CMNT LIKE 'DIRPAY % __' OR 
        PROFIT_CMNT LIKE 'ROLOVR % __';
    
BEGIN
    l_start_time := DBMS_UTILITY.get_time;
    L_OLD_STATE_NT := OLD_STATE_NT();
    L_NEW_STATE_NT := NEW_STATE_NT();
    
    OPEN GET_DETAIL_DIST_STATES_CUR;
    LOOP FETCH GET_DETAIL_DIST_STATES_CUR BULK COLLECT INTO L_OLD_STATE_NT LIMIT BATCH_READ_SIZE;
    EXIT WHEN L_OLD_STATE_NT.COUNT = 0;  

        -- Initialize between loops
        L_NEW_STATE_NT.DELETE;
        
        FOR L_INDEX IN 1..L_OLD_STATE_NT.COUNT 
        LOOP

            L_FIRST_PART := SUBSTR(L_OLD_STATE_NT(L_INDEX).PROF_COMMENT, 1, LENGTH(L_OLD_STATE_NT(L_INDEX).PROF_COMMENT) - 2);
            L_SECOND_PART := SUBSTR(L_OLD_STATE_NT(L_INDEX).PROF_COMMENT, -2);
    
            -- If a state outside of New England and not Florida (retirees there) then replace it
            IF L_SECOND_PART IN (
                'AK', 'AL', 'AR', 'AZ', 'CA', 'CO', 'DC', 'DE', 'GA',
                'HI', 'IA', 'ID', 'IL', 'IN', 'KS', 'KY', 'LA', 'MD', 
                'MI', 'MN', 'MO', 'MS', 'MT', 'NC', 'ND', 'NE', 'NJ', 'NM',
                'NV', 'NY', 'OH', 'OK', 'OR', 'PA', 'SC', 'SD', 'TN', 'TX',
                'UT', 'VA', 'WA', 'WI', 'WV', 'WY') THEN
                
                L_NEW_STATE_NT.EXTEND;
                L_LAST_INDEX := L_NEW_STATE_NT.LAST;
                L_NEW_STATE_NT(L_LAST_INDEX).REC_NO := L_OLD_STATE_NT(L_INDEX).REC_NO;
                L_NEW_STATE_NT(L_LAST_INDEX).PROF_COMMENT := (L_FIRST_PART || 'MA');
                
            END IF;
        END LOOP;
          
        FORALL L_INDEX IN 1 .. L_NEW_STATE_NT.COUNT SAVE EXCEPTIONS
            UPDATE PROFIT_DETAIL 
            SET
                PROFIT_CMNT = L_NEW_STATE_NT(L_INDEX).PROF_COMMENT    
            WHERE
                PROFIT_DET_RECNO = L_NEW_STATE_NT(L_INDEX).REC_NO;
        
    -- End cursor loop
    END LOOP;

    CLOSE GET_DETAIL_DIST_STATES_CUR;

    COMMIT;
    
    l_completed_time := DBMS_UTILITY.get_time;
    l_elapsed_seconds := (l_completed_time - l_start_time) / 100;
    l_elapsed_minutes := trunc(l_elapsed_seconds / 60, 0);

    dbms_output.put_line(chr(10) || '[TIME] Obfuscate outlier distribution state duration: ' 
    || l_elapsed_minutes || ' minutes + ' || mod(l_elapsed_seconds, 60) || ' seconds.'); 
    
    
EXCEPTION
        
        WHEN OTHERS THEN
            L_SQL_ERROR := SQLCODE;
            L_SQL_ERRM := SQLERRM;
            dbms_output.put_line('ERROR_STACK: ' || DBMS_UTILITY.FORMAT_ERROR_STACK);
            dbms_output.put_line('ERROR_BACKTRACE: ' || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE);
            DBMS_OUTPUT.PUT_LINE('There was an unhandled exeception in obfuscating distribution tax states. ');
            DBMS_OUTPUT.PUT_LINE('Error number: '
                                 || L_SQL_ERROR
                                 || ' Message: '
                                 || L_SQL_ERRM);
            INSERT INTO log_messages (CALLER,  SQL_ERROR_CODE, ERROR_MESSAGE, CREATED_ON) 
            VALUES ('Obscure distribution state', L_SQL_ERROR, L_SQL_ERRM, sysdate);

END;