/*

This script removes unrelated PROFIT_DETAIL records that do not correspond
to the SSNs in the TARGET_EMP_SSN table.

*/


DECLARE

    -- This is for batch reading of tables into nested tables
    BATCH_READ_SIZE  PLS_INTEGER := 200;

    TYPE EMPLOYEE_SSN_REC IS RECORD ( 
        SSN NUMBER(9)
    );

    -- Our Nested Table types
    TYPE DELETE_SSNs_NT IS TABLE OF EMPLOYEE_SSN_REC;

    -- This is the intialized instance of our nested table
    L_DELETE_SSNs_NT DELETE_SSNs_NT;

 
    NUM_RECORDS NUMBER := 0;
    L_SQL_ERROR NUMBER;
    L_SQL_ERRM VARCHAR2(512);

    L_ERROR_COUNT NUMBER;


    -- Variables for performance timing
    l_start_time NUMBER;
    l_completed_time NUMBER;
    l_elapsed_minutes NUMBER;
    l_elapsed_seconds NUMBER;

    L_HIGH_VOLUME_FLAG BOOLEAN;
    L_DELETE_COUNT NUMBER;
    V_DONE BOOLEAN;
    v_RECORDS_DELETED NUMBER;


BEGIN

    L_DELETE_COUNT := 0;
    v_RECORDS_DELETED := 0;

    -- Set this to TRUE if you are working with more than 10 million
    -- total records
    L_HIGH_VOLUME_FLAG := FALSE;

    l_start_time := DBMS_UTILITY.get_time;

    -- PROFIT_SHARING_CHECKS
    -- So now we need to bulk collect all non-matching SSNs into our nested table

    DBMS_OUTPUT.PUT_LINE('Preparing to delete PROFIT_SHARING_CHECKS records. ');

    V_DONE := FALSE;
    

    WHILE NOT V_DONE
        LOOP
            DELETE FROM
                PROFIT_SHARE_CHECKS
            WHERE
                NOT EXISTS (
                    SELECT
                        NULL
                    FROM
                        TARGET_EMP_SSN
                    WHERE
                        TARGET_EMP_SSN.SSN = PROFIT_SHARE_CHECKS.EMPLOYEE_SSN
                );

                v_RECORDS_DELETED := SQL%ROWCOUNT;
                L_DELETE_COUNT := L_DELETE_COUNT + v_RECORDS_DELETED;

                IF (v_RECORDS_DELETED = 0)
                THEN
                    V_DONE := TRUE;
                END IF;
                COMMIT;
    END LOOP;

    V_DONE := FALSE;
    

    WHILE NOT V_DONE
        LOOP
            DELETE FROM
                PROFIT_SHARE_CHECKS
            WHERE
                NOT EXISTS (
                    SELECT
                        NULL
                    FROM
                        TARGET_ALL_SSN
                    WHERE
                        TARGET_ALL_SSN.SSN = PROFIT_SHARE_CHECKS.SSN_NUMBER
                );

                v_RECORDS_DELETED := SQL%ROWCOUNT;
                L_DELETE_COUNT := L_DELETE_COUNT + v_RECORDS_DELETED;

                IF (v_RECORDS_DELETED = 0)
                THEN
                    V_DONE := TRUE;
                END IF;
                COMMIT;
    END LOOP;

    DBMS_OUTPUT.PUT_LINE('PROFIT_SHARE_CHECKS Employee SSN Delete count: ' || L_DELETE_COUNT);
    --COMMIT;

    l_completed_time := DBMS_UTILITY.get_time; 
    l_elapsed_seconds := (l_completed_time - l_start_time) / 100;
    l_elapsed_minutes := trunc(l_elapsed_seconds / 60, 0);
    
    dbms_output.put_line(chr(10) || '[TIME] Delete PROFIT_SHARE_CHECKS records duration: ' 
    || l_elapsed_minutes || ' minutes + ' || mod(l_elapsed_seconds, 60) || ' seconds.' || chr(10));

EXCEPTION
        WHEN OTHERS THEN
            L_SQL_ERROR := SQLCODE;
            L_SQL_ERRM := SQLERRM;
            DBMS_OUTPUT.PUT_LINE('Script to delete unrelated PROFIT_SHARE_CHECKS rows halted. ');
            DBMS_OUTPUT.PUT_LINE('Error number: '
                                 || L_SQL_ERROR
                                 || ' Message: '
                                 || L_SQL_ERRM);
            INSERT INTO log_messages (CALLER,  SQL_ERROR_CODE, ERROR_MESSAGE, CREATED_ON) 
            VALUES ('Error deleting unrelated PROFIT_SHARE_CHECKS records', L_SQL_ERROR, L_SQL_ERRM, sysdate);

END;