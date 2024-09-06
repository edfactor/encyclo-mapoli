/*

This script bulk-deletes rows that do not corespond to the employee SSNs in
the TARGET_EMP_SSN table for the following related tables:

DEMOGRAPHICS
PAYBEN
PAYREL
PROFIT_DIST_REQ
PAYPROFIT

The two other profit sharing tables, PROFIT_SHARE_CHECKS and PROFIT_DETAIL
are handled in separate scripts as those tables will have many more rows
than these five do.

*/


DECLARE

    L_SQL_ERROR NUMBER;
    L_SQL_ERRM VARCHAR2(512);

    L_ERROR_COUNT NUMBER;

    -- Variables for performance timing
    l_section_time NUMBER;
    l_completed_time NUMBER;
    l_elapsed_minutes NUMBER;
    l_elapsed_seconds NUMBER;

    L_DELETE_COUNT NUMBER;
    V_DONE BOOLEAN;
    v_RECORDS_DELETED NUMBER;


BEGIN
    
    DBMS_OUTPUT.PUT_LINE('Proceeding to delete all unrelated records. ' || chr(10));
    DBMS_OUTPUT.PUT_LINE('Preparing to delete DEMOGRAPHICS records. ');
    l_section_time := DBMS_UTILITY.get_time;

    -- DEMOGRAPHICS
    -- So now we need to bulk collect all non-matching SSNs into our nested table

    L_DELETE_COUNT := 0;
    V_DONE := FALSE;
    v_RECORDS_DELETED := 0;
   
    WHILE NOT V_DONE
        LOOP
            DELETE FROM
                DEMOGRAPHICS
            WHERE
                NOT EXISTS (
                    SELECT
                        NULL
                    FROM
                        TARGET_EMP_SSN
                    WHERE
                        TARGET_EMP_SSN.SSN = DEMOGRAPHICS.DEM_SSN
                );

                v_RECORDS_DELETED := SQL%ROWCOUNT;
                L_DELETE_COUNT := L_DELETE_COUNT + v_RECORDS_DELETED;

                IF (v_RECORDS_DELETED = 0)
                THEN
                    V_DONE := TRUE;
                END IF;
        COMMIT;
    END LOOP;

    DBMS_OUTPUT.PUT_LINE('DEMOGRAPHICS Delete count: ' || L_DELETE_COUNT);

    l_completed_time := DBMS_UTILITY.get_time; 
    l_elapsed_seconds := (l_completed_time - l_section_time) / 100;
    l_elapsed_minutes := trunc(l_elapsed_seconds / 60, 0);
    
    dbms_output.put_line(chr(10) || '[TIME] Delete DEMOGRAPHICS records duration: ' 
    || l_elapsed_minutes || ' minutes + ' || mod(l_elapsed_seconds, 60) || ' seconds.' || chr(10));
   
    -- PAYBEN
    -- So now we need to bulk collect all non-matching PSNs into our nested table

    DBMS_OUTPUT.PUT_LINE('Preparing to delete PAYBEN records. ');
    l_section_time := DBMS_UTILITY.get_time;
    
    L_DELETE_COUNT := 0;
    V_DONE := FALSE;
    v_RECORDS_DELETED := 0;
   
    WHILE NOT V_DONE
        LOOP
            DELETE FROM
                PAYBEN
            WHERE
                NOT EXISTS (
                    SELECT
                        NULL
                    FROM
                        TARGET_ALL_SSN
                    WHERE
                        TARGET_ALL_SSN.SSN = PAYBEN.PYBEN_PAYSSN
                );

                v_RECORDS_DELETED := SQL%ROWCOUNT;
                L_DELETE_COUNT := L_DELETE_COUNT + v_RECORDS_DELETED;

                IF (v_RECORDS_DELETED = 0)
                THEN
                    V_DONE := TRUE;
                END IF;
                COMMIT;
    END LOOP;

    DBMS_OUTPUT.PUT_LINE('PAYBEN Delete count: ' || L_DELETE_COUNT);

    l_completed_time := DBMS_UTILITY.get_time;
    l_elapsed_seconds := (l_completed_time - l_section_time) / 100;
    l_elapsed_minutes := trunc(l_elapsed_seconds / 60, 0);
    
    dbms_output.put_line(chr(10) || '[TIME] Delete PAYBEN records duration: ' 
    || l_elapsed_minutes || ' minutes + ' || mod(l_elapsed_seconds, 60) || ' seconds.' || chr(10)); 
    
    
    -- PAYPROFIT

    DBMS_OUTPUT.PUT_LINE('Preparing to delete PAYPROFIT records. ');
    l_section_time := DBMS_UTILITY.get_time;
    
    L_DELETE_COUNT := 0;

    V_DONE := FALSE;
    v_RECORDS_DELETED := 0;
   
    WHILE NOT V_DONE
        LOOP
            DELETE FROM
                PAYPROFIT
            WHERE
                NOT EXISTS (
                    SELECT
                        NULL
                    FROM
                        TARGET_ALL_SSN
                    WHERE
                        TARGET_ALL_SSN.SSN = PAYPROFIT.PAYPROF_SSN
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
    v_RECORDS_DELETED := 0;
   
    WHILE NOT V_DONE
        LOOP
            DELETE FROM
                PAYPROFIT
            WHERE
                NOT EXISTS (
                    SELECT
                        NULL
                    FROM
                        TARGET_ALL_PSN
                    WHERE
                        TARGET_ALL_PSN.PSN = PAYPROFIT.PAYPROF_BADGE
                );

                v_RECORDS_DELETED := SQL%ROWCOUNT;
                L_DELETE_COUNT := L_DELETE_COUNT + v_RECORDS_DELETED;

                IF (v_RECORDS_DELETED = 0)
                THEN
                    V_DONE := TRUE;
                END IF;
                COMMIT;
    END LOOP;


    DBMS_OUTPUT.PUT_LINE('PAYPROFIT Delete count: ' || L_DELETE_COUNT);

    l_completed_time := DBMS_UTILITY.get_time;
    l_elapsed_seconds := (l_completed_time - l_section_time) / 100;
    l_elapsed_minutes := trunc(l_elapsed_seconds / 60, 0);
    
    dbms_output.put_line(chr(10) || '[TIME] Delete PAYPROFIT records duration: ' 
    || l_elapsed_minutes || ' minutes + ' || mod(l_elapsed_seconds, 60) || ' seconds.' || chr(10)); 

    -- PYREL
    -- So now we need to bulk collect all non-matching SSN/PSNs into our nested table

    DBMS_OUTPUT.PUT_LINE('Preparing to delete PAYREL SSN records. ');
    l_section_time := DBMS_UTILITY.get_time;
    
    L_DELETE_COUNT := 0;
    
    /*

    -- This block would delete PAYREL by PSNs

    V_DONE := FALSE;
    v_RECORDS_DELETED := 0;
   
    WHILE NOT V_DONE
        LOOP
            DELETE FROM
                PAYREL
            WHERE
                NOT EXISTS (
                    SELECT
                        NULL
                    FROM
                        TARGET_ALL_PSN
                    WHERE
                        TARGET_ALL_PSN.PSN = PAYREL.PYREL_PSN
                );

                v_RECORDS_DELETED := SQL%ROWCOUNT;
                L_DELETE_COUNT := L_DELETE_COUNT + v_RECORDS_DELETED;

                IF (v_RECORDS_DELETED = 0)
                THEN
                    V_DONE := TRUE;
                END IF;
                COMMIT;
    END LOOP;
*/
    
    V_DONE := FALSE;
    v_RECORDS_DELETED := 0;
   
    WHILE NOT V_DONE
        LOOP
            DELETE FROM
                PAYREL
            WHERE
                NOT EXISTS (
                    SELECT
                        NULL
                    FROM
                        TARGET_ALL_SSN
                    WHERE
                        TARGET_ALL_SSN.SSN = PAYREL.PYREL_PAYSSN
                );

                v_RECORDS_DELETED := SQL%ROWCOUNT;
                L_DELETE_COUNT := L_DELETE_COUNT + v_RECORDS_DELETED;

                IF (v_RECORDS_DELETED = 0)
                THEN
                    V_DONE := TRUE;
                END IF;
                COMMIT;
    END LOOP;

    DBMS_OUTPUT.PUT_LINE('PAYREL SSN Delete count: ' || L_DELETE_COUNT);

    l_completed_time := DBMS_UTILITY.get_time;
    l_elapsed_seconds := (l_completed_time - l_section_time) / 100;
    l_elapsed_minutes := trunc(l_elapsed_seconds / 60, 0);
    
    dbms_output.put_line(chr(10) || '[TIME] Delete PAYREL records duration: ' 
    || l_elapsed_minutes || ' minutes + ' || mod(l_elapsed_seconds, 60) || ' seconds.' || chr(10)); 
    

    -- PROFDIST
    -- So now we need to bulk collect all non-matching SSNs into our nested table

    DBMS_OUTPUT.PUT_LINE('Preparing to delete PROFDIST records. ');
    l_section_time := DBMS_UTILITY.get_time;
    
    L_DELETE_COUNT := 0;
    V_DONE := FALSE;
    v_RECORDS_DELETED := 0;
   
    WHILE NOT V_DONE
        LOOP
            DELETE FROM
                PROFDIST
            WHERE
                NOT EXISTS (
                    SELECT
                        NULL
                    FROM
                        TARGET_ALL_SSN
                    WHERE
                        TARGET_ALL_SSN.SSN = PROFDIST.PROFDIST_SSN
                );

                v_RECORDS_DELETED := SQL%ROWCOUNT;
                L_DELETE_COUNT := L_DELETE_COUNT + v_RECORDS_DELETED;

                IF (v_RECORDS_DELETED = 0)
                THEN
                    V_DONE := TRUE;
                END IF;
                COMMIT;
    END LOOP;

    DBMS_OUTPUT.PUT_LINE('PROFDIST SSN Delete count: ' || L_DELETE_COUNT);

    l_completed_time := DBMS_UTILITY.get_time;
    l_elapsed_seconds := (l_completed_time - l_section_time) / 100;
    l_elapsed_minutes := trunc(l_elapsed_seconds / 60, 0);
    
    dbms_output.put_line(chr(10) || '[TIME] Delete PROFDIST records duration: ' 
    || l_elapsed_minutes || ' minutes + ' || mod(l_elapsed_seconds, 60) || ' seconds.' || chr(10)); 

    -- PROFIT_SS_DETAIL
    -- There are not a lot of these legacy records

    DBMS_OUTPUT.PUT_LINE('Preparing to delete PROFIT_SS_DETAIL records. ');
    l_section_time := DBMS_UTILITY.get_time;
    
    L_DELETE_COUNT := 0;
    V_DONE := FALSE;
    v_RECORDS_DELETED := 0;
   
    WHILE NOT V_DONE
        LOOP
            DELETE FROM
                PROFIT_SS_DETAIL
            WHERE
                NOT EXISTS (
                    SELECT
                        NULL
                    FROM
                        TARGET_ALL_SSN
                    WHERE
                        TARGET_ALL_SSN.SSN = PROFIT_SS_DETAIL.PR_SS_D_S_SEC_NUMBER
                );

                v_RECORDS_DELETED := SQL%ROWCOUNT;
                L_DELETE_COUNT := L_DELETE_COUNT + v_RECORDS_DELETED;

                IF (v_RECORDS_DELETED = 0)
                THEN
                    V_DONE := TRUE;
                END IF;
                COMMIT;
    END LOOP;

    DBMS_OUTPUT.PUT_LINE('PROFIT_SS_DETAIL Delete count: ' || L_DELETE_COUNT);

    l_completed_time := DBMS_UTILITY.get_time;
    l_elapsed_seconds := (l_completed_time - l_section_time) / 100;
    l_elapsed_minutes := trunc(l_elapsed_seconds / 60, 0);
    
    dbms_output.put_line(chr(10) || '[TIME] Delete PROFIT_SS_DETAIL records duration: ' 
    || l_elapsed_minutes || ' minutes + ' || mod(l_elapsed_seconds, 60) || ' seconds.' || chr(10)); 


    -- SOC_SEC_REC
    -- There will be a lot of these records.

    DBMS_OUTPUT.PUT_LINE('Preparing to delete SOC_SEC_REC records. ');
    l_section_time := DBMS_UTILITY.get_time;
    
    L_DELETE_COUNT := 0;
    V_DONE := FALSE;
    v_RECORDS_DELETED := 0;
   
    WHILE NOT V_DONE
        LOOP
            DELETE FROM
                SOC_SEC_REC
            WHERE
                NOT EXISTS (
                    SELECT
                        NULL
                    FROM
                        TARGET_ALL_SSN
                    WHERE
                        TARGET_ALL_SSN.SSN = SOC_SEC_REC.SOC_SEC_NUMBER
                );

                v_RECORDS_DELETED := SQL%ROWCOUNT;
                L_DELETE_COUNT := L_DELETE_COUNT + v_RECORDS_DELETED;

                IF (v_RECORDS_DELETED = 0)
                THEN
                    V_DONE := TRUE;
                END IF;
                COMMIT;
    END LOOP;

    DBMS_OUTPUT.PUT_LINE('SOC_SEC_REC Delete count: ' || L_DELETE_COUNT);

    l_completed_time := DBMS_UTILITY.get_time;
    l_elapsed_seconds := (l_completed_time - l_section_time) / 100;
    l_elapsed_minutes := trunc(l_elapsed_seconds / 60, 0);
    
    dbms_output.put_line(chr(10) || '[TIME] Delete SOC_SEC_REC records duration: ' 
    || l_elapsed_minutes || ' minutes + ' || mod(l_elapsed_seconds, 60) || ' seconds.' || chr(10)); 


    -- PROFIT_DIST_REQ
    DBMS_OUTPUT.PUT_LINE('Preparing to delete PROFIT_DIST_REQ records. ');
    l_section_time := DBMS_UTILITY.get_time;
    
    L_DELETE_COUNT := 0;
    V_DONE := FALSE;
    v_RECORDS_DELETED := 0;
   
    WHILE NOT V_DONE
        LOOP
            DELETE FROM
                PROFIT_DIST_REQ
            WHERE
                NOT EXISTS (
                    SELECT
                        NULL
                    FROM
                        TARGET_EMP_PSN
                    WHERE
                        TARGET_EMP_PSN.PSN = PROFIT_DIST_REQ.PROFIT_DIST_REQ_EMP
                );

                v_RECORDS_DELETED := SQL%ROWCOUNT;
                L_DELETE_COUNT := L_DELETE_COUNT + v_RECORDS_DELETED;

                IF (v_RECORDS_DELETED = 0)
                THEN
                    V_DONE := TRUE;
                END IF;
                COMMIT;
    END LOOP;

    DBMS_OUTPUT.PUT_LINE('PROFIT_DIST_REQ Delete count: ' || L_DELETE_COUNT);

    l_completed_time := DBMS_UTILITY.get_time;
    l_elapsed_seconds := (l_completed_time - l_section_time) / 100;
    l_elapsed_minutes := trunc(l_elapsed_seconds / 60, 0);
    
    dbms_output.put_line(chr(10) || '[TIME] Delete PROFIT_DIST_REQ records duration: ' 
    || l_elapsed_minutes || ' minutes + ' || mod(l_elapsed_seconds, 60) || ' seconds.' || chr(10)); 

EXCEPTION
        WHEN OTHERS THEN
            L_SQL_ERROR := SQLCODE;
            L_SQL_ERRM := SQLERRM;
            DBMS_OUTPUT.PUT_LINE('Script to delete small-table unrelated rows halted. ');
            DBMS_OUTPUT.PUT_LINE('Error number: '
                                 || L_SQL_ERROR
                                 || ' Message: '
                                 || L_SQL_ERRM);
            INSERT INTO log_messages (CALLER,  SQL_ERROR_CODE, ERROR_MESSAGE, CREATED_ON) 
            VALUES ('Error deleting small-table unrelated records', L_SQL_ERROR, L_SQL_ERRM, sysdate);

END;