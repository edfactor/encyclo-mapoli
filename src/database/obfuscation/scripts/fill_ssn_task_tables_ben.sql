/*
    This script calls the functions to fill the temporary 
    tables for SSNs for beneficiaries. The function that 
    does the work is in functions_fill_task_tables.sql
*/
DECLARE
    RESULT_CODE NUMBER(2);
    L_SQL_ERROR NUMBER;
    L_SQL_ERRM  VARCHAR2(512);
    -- We need a custom exception to halt execution and explain why
    ex_custom EXCEPTION;
    PRAGMA EXCEPTION_INIT( ex_custom, -20001 );
    
    -- Variables for performance timing
    l_start_time NUMBER;
    l_completed_time NUMBER;
    l_elapsed_minutes NUMBER;
    l_elapsed_seconds NUMBER;
    
BEGIN
    l_start_time := DBMS_UTILITY.get_time;
    
    
    DBMS_OUTPUT.PUT_LINE('Proceeding to fill SSN temp tables for Beneficiaries... ');
    RESULT_CODE := FILL_SSN_TABLES_BENEFICIARY;
    IF (RESULT_CODE <> 0) THEN
        raise_application_error( -20001, 'Error in filling SSNs for Beneficiaries' );        
    END IF;
    DBMS_OUTPUT.PUT_LINE('...done. ');
    
    COMMIT;
    
    l_completed_time := DBMS_UTILITY.get_time;
    l_elapsed_seconds := (l_completed_time - l_start_time) / 100;
    l_elapsed_minutes := trunc(l_elapsed_seconds / 60, 0);
    
    dbms_output.put_line(chr(10) || '[TIME] Fill Beneficiary SSN Temp Tables duration: ' 
    || l_elapsed_minutes || ' minutes + ' || mod(l_elapsed_seconds, 60) || ' seconds.'); 
    
    
EXCEPTION
        WHEN ex_custom THEN
            L_SQL_ERROR := SQLCODE;
            L_SQL_ERRM := SQLERRM;
            DBMS_OUTPUT.PUT_LINE('Execution of fill benefciary SSN temp tables script halted. ');
            DBMS_OUTPUT.PUT_LINE('Error number: '
                                 || L_SQL_ERROR
                                 || ' Message: '
                                 || L_SQL_ERRM);
            INSERT INTO log_messages (CALLER,  SQL_ERROR_CODE, ERROR_MESSAGE, CREATED_ON) 
            VALUES ('Main ben fill SSN temp tables script', L_SQL_ERROR, L_SQL_ERRM, sysdate);
        WHEN OTHERS THEN
            L_SQL_ERROR := SQLCODE;
            L_SQL_ERRM := SQLERRM;
            DBMS_OUTPUT.PUT_LINE('There was an unhandled exeception in filling beneficiary ssn temporary tables. ');
            DBMS_OUTPUT.PUT_LINE('Error number: '
                                 || L_SQL_ERROR
                                 || ' Message: '
                                 || L_SQL_ERRM);
            INSERT INTO log_messages (CALLER,  SQL_ERROR_CODE, ERROR_MESSAGE, CREATED_ON) 
            VALUES ('Main beneficiary fill SSN temp tables script', L_SQL_ERROR, L_SQL_ERRM, sysdate);
END;