/*
    This script empties the message log table and inserts an entry
    indicating the run and the version of the scripts that
    were used.
*/

DECLARE

BEGIN

    DELETE FROM LOG_MESSAGES;

    INSERT INTO log_messages (CALLER,  SQL_ERROR_CODE, ERROR_MESSAGE, CREATED_ON) 
        VALUES ('Begin Run using Version 8.4', NULL, NULL, sysdate);

    DBMS_OUTPUT.PUT_LINE('Begin Obfuscation Run. Version 8.4');

    COMMIT;

END;