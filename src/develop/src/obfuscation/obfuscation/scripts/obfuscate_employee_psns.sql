/*
    This script uses the temporary tables to change all of the PSNs 
    in the system for employees while maintaining the relationships.
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
    DBMS_OUTPUT.PUT_LINE(chr(10) || '- - - Begin obfuscation of employee PSNs - - -');
    
    RESULT_CODE := OBFUSCATE_EMPLOYEE_PSNS;
    IF (RESULT_CODE <> 0) THEN
        raise_application_error( -20001, 'Error Obfuscating PSNs for Employees' );        
    END IF;
    DBMS_OUTPUT.PUT_LINE('...done.');
    COMMIT;
    
    DBMS_OUTPUT.PUT_LINE('- - - Completed obfuscation of employee PSNs - - -');
    l_completed_time := DBMS_UTILITY.get_time;
    l_elapsed_seconds := (l_completed_time - l_start_time) / 100;
    l_elapsed_minutes := trunc(l_elapsed_seconds / 60, 0);
    
    dbms_output.put_line(chr(10) || '[TIME] Obfuscation of Employee PSNs duration: ' 
    || l_elapsed_minutes || ' minutes + ' || mod(l_elapsed_seconds, 60) || ' seconds.'); 
    
EXCEPTION
        WHEN ex_custom THEN
            L_SQL_ERROR := SQLCODE;
            L_SQL_ERRM := SQLERRM;
            DBMS_OUTPUT.PUT_LINE('Execution of main emp obfuscation PSN script halted. ');
            DBMS_OUTPUT.PUT_LINE('Error number: '
                                 || L_SQL_ERROR
                                 || ' Message: '
                                 || L_SQL_ERRM);
            INSERT INTO log_messages (CALLER,  SQL_ERROR_CODE, ERROR_MESSAGE, CREATED_ON) 
            VALUES ('Main PSN obfuscation script', L_SQL_ERROR, L_SQL_ERRM, sysdate);
        WHEN OTHERS THEN
            L_SQL_ERROR := SQLCODE;
            L_SQL_ERRM := SQLERRM;
            dbms_output.put_line('ERROR_STACK: ' || DBMS_UTILITY.FORMAT_ERROR_STACK);
            dbms_output.put_line('ERROR_BACKTRACE: ' || DBMS_UTILITY.FORMAT_ERROR_BACKTRACE);
            DBMS_OUTPUT.PUT_LINE('There was an unhandled exeception in obfuscating emp PSNs. ');
            DBMS_OUTPUT.PUT_LINE('Error number: '
                                 || L_SQL_ERROR
                                 || ' Message: '
                                 || L_SQL_ERRM);
            INSERT INTO log_messages (CALLER,  SQL_ERROR_CODE, ERROR_MESSAGE, CREATED_ON) 
            VALUES ('Main emp PSN obfuscation ID script', L_SQL_ERROR, L_SQL_ERRM, sysdate);
END;