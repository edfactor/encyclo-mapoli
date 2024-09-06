/*
    This script makes sure that all tasks have run, 
    and that all remaining SSN and PSN keys have been 
    obfuscated. It will print out counts and samples of
    records that were not obfuscated.

    Lastly, it will count up SSNs so you can see if all 
    the ones you started with in TARGET tables (if you 
    used them), how many made it into the task tables, 
    and then how many were in the resulting tables. This 
    can help you see if any didn't make it all the way 
    through.
*/
DECLARE
    L_SQL_ERROR NUMBER;
    L_SQL_ERRM  VARCHAR2(512);

    
    -- Variables for performance timing
    l_start_time NUMBER;
    l_completed_time NUMBER;
    l_elapsed_minutes NUMBER;
    l_elapsed_seconds NUMBER;
    
    l_ssn_start NUMBER;
    l_ssn_end NUMBER;
    l_psn_start NUMBER;
    l_psn_end NUMBER;

    -- these are the count numbers
    l_remaining_tasks NUMBER;
    l_remaining_rows NUMBER;
    
BEGIN

    l_ssn_start := 700000000;
    l_ssn_end := 750000000;
    l_psn_start := 700000;
    l_psn_end  := 999999;
    
    l_start_time := DBMS_UTILITY.get_time;
    l_remaining_tasks := 0;
    l_remaining_rows := 0;
    
    DBMS_OUTPUT.PUT_LINE('Begin post-validation.');
    DBMS_OUTPUT.PUT_LINE('  ');
    DBMS_OUTPUT.PUT_LINE('Check for unobfuscated rows.');
    DBMS_OUTPUT.PUT_LINE('-----------------------------');
    DBMS_OUTPUT.PUT_LINE('  ');
    SELECT count(*) INTO l_remaining_tasks FROM SSN_TASKS WHERE DONE = 0;
    DBMS_OUTPUT.PUT_LINE('Number of SSN Tasks undone: ' || l_remaining_tasks);
    
    SELECT count(*) INTO l_remaining_tasks FROM PSN_TASKS WHERE DONE = 0;
    DBMS_OUTPUT.PUT_LINE('Number of PSN Tasks undone: ' || l_remaining_tasks);

    SELECT count(*) INTO l_remaining_rows FROM DEMOGRAPHICS WHERE DEM_SSN IS NOT NULL and DEM_SSN < l_ssn_start and DEM_SSN < l_ssn_end;
    DBMS_OUTPUT.PUT_LINE('Number of DEMOGRAPHICS SSNs unobfuscated: ' || l_remaining_rows);
    
    SELECT count(*) INTO l_remaining_rows FROM DEMOGRAPHICS WHERE DEM_BADGE IS NOT NULL and DEM_BADGE < l_psn_start and DEM_BADGE < l_psn_end;
    DBMS_OUTPUT.PUT_LINE('Number of DEMOGRAPHICS PSNs unobfuscated: ' || l_remaining_rows);
    
    SELECT count(*) INTO l_remaining_rows FROM PAYBEN WHERE PYBEN_PSN IS NOT NULL and PYBEN_PSN < l_psn_start and PYBEN_PSN < l_psn_end;
    DBMS_OUTPUT.PUT_LINE('Number of PAYBEN PSNs unobfuscated: ' || l_remaining_rows);
        
    SELECT count(*) INTO l_remaining_rows FROM PAYPROFIT WHERE PAYPROF_SSN IS NOT NULL and PAYPROF_SSN < l_ssn_start and PAYPROF_SSN < l_ssn_end;
    DBMS_OUTPUT.PUT_LINE('Number of PAYPROFIT SSNs unobfuscated: ' || l_remaining_rows);
    
    SELECT count(*) INTO l_remaining_rows FROM PAYPROFIT WHERE PAYPROF_BADGE IS NOT NULL and PAYPROF_BADGE < l_psn_start and PAYPROF_BADGE < l_psn_end;
    DBMS_OUTPUT.PUT_LINE('Number of PAYPROFIT PSNs unobfuscated: ' || l_remaining_rows);
    
    SELECT count(*) INTO l_remaining_rows FROM PROFIT_DETAIL WHERE PR_DET_S_SEC_NUMBER IS NOT NULL and PR_DET_S_SEC_NUMBER < l_ssn_start and PR_DET_S_SEC_NUMBER < l_ssn_end;
    DBMS_OUTPUT.PUT_LINE('Number of PROFIT_DETAIL SSNs unobfuscated: ' || l_remaining_rows);
    
    SELECT count(*) INTO l_remaining_rows FROM PROFIT_SS_DETAIL WHERE PR_SS_D_S_SEC_NUMBER IS NOT NULL and PR_SS_D_S_SEC_NUMBER < l_ssn_start and PR_SS_D_S_SEC_NUMBER < l_ssn_end;
    DBMS_OUTPUT.PUT_LINE('Number of PROFIT_SS_DETAIL Primary SSNs unobfuscated: ' || l_remaining_rows);

    SELECT count(*) INTO l_remaining_rows FROM PROFIT_SS_DETAIL WHERE PROFIT_SS_SSNO IS NOT NULL and PROFIT_SS_SSNO < l_ssn_start and PROFIT_SS_SSNO < l_ssn_end;
    DBMS_OUTPUT.PUT_LINE('Number of PROFIT_SS_DETAIL Secondary SSNs unobfuscated: ' || l_remaining_rows);

    SELECT count(*) INTO l_remaining_rows FROM SOC_SEC_REC WHERE SOC_SEC_NUMBER IS NOT NULL and SOC_SEC_NUMBER < l_ssn_start and SOC_SEC_NUMBER < l_ssn_end;
    DBMS_OUTPUT.PUT_LINE('Number of SOC_SEC_REC Secondary SSNs unobfuscated: ' || l_remaining_rows);

    SELECT count(*) INTO l_remaining_rows FROM PROFIT_DIST_REQ WHERE PROFIT_DIST_REQ_PSN IS NOT NULL and PROFIT_DIST_REQ_PSN < l_psn_start and PROFIT_DIST_REQ_PSN < l_psn_end;
    DBMS_OUTPUT.PUT_LINE('Number of PROFIT_DIST_REQ PSNs unobfuscated: ' || l_remaining_rows);
   
    SELECT count(*) INTO l_remaining_rows FROM PROFIT_DIST_REQ WHERE PROFIT_DIST_REQ_EMP IS NOT NULL and PROFIT_DIST_REQ_EMP < l_psn_start and PROFIT_DIST_REQ_EMP < l_psn_end;
    DBMS_OUTPUT.PUT_LINE('Number of PROFIT_DIST_REQ Employee IDs unobfuscated: ' || l_remaining_rows);
    
    SELECT count(*) INTO l_remaining_rows FROM PROFDIST WHERE PROFDIST_SSN IS NOT NULL and PROFDIST_SSN < l_ssn_start and PROFDIST_SSN < l_ssn_end;
    DBMS_OUTPUT.PUT_LINE('Number of PROFDIST SSNs unobfuscated: ' || l_remaining_rows);
    
    SELECT count(*) INTO l_remaining_rows FROM PROFDIST WHERE PROFDIST_PAYSSN IS NOT NULL and PROFDIST_PAYSSN < l_ssn_start and PROFDIST_PAYSSN < l_ssn_end;
    DBMS_OUTPUT.PUT_LINE('Number of PROFDIST PaySSNs unobfuscated: ' || l_remaining_rows);
    
    SELECT count(*) INTO l_remaining_rows FROM PROFIT_SHARE_CHECKS WHERE EMPLOYEE_SSN IS NOT NULL and EMPLOYEE_SSN < l_ssn_start and EMPLOYEE_SSN < l_ssn_end;
    DBMS_OUTPUT.PUT_LINE('Number of PROFIT_SHARE_CHECKS SSNs unobfuscated: ' || l_remaining_rows);
    
    SELECT count(*) INTO l_remaining_rows FROM PROFIT_SHARE_CHECKS WHERE EMPLOYEE_NUMBER IS NOT NULL and EMPLOYEE_NUMBER < l_psn_start and EMPLOYEE_NUMBER < l_psn_end;
    DBMS_OUTPUT.PUT_LINE('Number of PROFIT_SHARE_CHECKS Employee Numbers unobfuscated: ' || l_remaining_rows);
    DBMS_OUTPUT.PUT_LINE('  ');
    DBMS_OUTPUT.PUT_LINE('Checking counts of unique SSNs across phases.');
    DBMS_OUTPUT.PUT_LINE('-----------------------------');

    SELECT COUNT(DISTINCT SSN) into l_remaining_rows FROM TARGET_EMP_SSN;
    DBMS_OUTPUT.PUT_LINE('Number of TARGET_EMP_SSNs: ' || l_remaining_rows);

    SELECT COUNT(DISTINCT DEMOGRAPHICS_SSN) into l_remaining_rows FROM SSN_TASKS;
    DBMS_OUTPUT.PUT_LINE('Number of DEMOGRAPHICS SSNs in SSN_TASKS: ' || l_remaining_rows);

    SELECT COUNT(DISTINCT DEM_SSN) into l_remaining_rows FROM DEMOGRAPHICS;
    DBMS_OUTPUT.PUT_LINE('Number of DEMOGRAPHICS SSNs: ' || l_remaining_rows);

    SELECT COUNT(DISTINCT SSN) into l_remaining_rows FROM TARGET_BEN_SSN;
    DBMS_OUTPUT.PUT_LINE('Number of TARGET_BEN_SSNs: ' || l_remaining_rows);

    SELECT COUNT(DISTINCT PYBEN_PAYSSN) into l_remaining_rows FROM SSN_TASKS;
    DBMS_OUTPUT.PUT_LINE('Number of PAYBEN_PAYSSNs in SSN_TASKS ' || l_remaining_rows);

    SELECT COUNT(DISTINCT PYBEN_PAYSSN) into l_remaining_rows FROM PAYBEN;
    DBMS_OUTPUT.PUT_LINE('Number of PAYBEN SSNs: ' || l_remaining_rows);

    SELECT COUNT(DISTINCT SSN) into l_remaining_rows FROM TARGET_ALL_SSN;
    DBMS_OUTPUT.PUT_LINE('Number of TARGET_ALL_SSNs: ' || l_remaining_rows);
    DBMS_OUTPUT.PUT_LINE('  ');
    DBMS_OUTPUT.PUT_LINE('Post validation complete. ');
    

    
    l_completed_time := DBMS_UTILITY.get_time;
    l_elapsed_seconds := (l_completed_time - l_start_time) / 100;
    l_elapsed_minutes := trunc(l_elapsed_seconds / 60, 0);
    
    dbms_output.put_line(chr(10) || '[TIME] Post validation duration: ' 
    || l_elapsed_minutes || ' minutes + ' || mod(l_elapsed_seconds, 60) || ' seconds.'); 
    
    
EXCEPTION
    WHEN OTHERS THEN
        L_SQL_ERROR := SQLCODE;
        L_SQL_ERRM := SQLERRM;
        DBMS_OUTPUT.PUT_LINE('There was an unhandled exeception in post-validation.');
        DBMS_OUTPUT.PUT_LINE('Error number: '
                                || L_SQL_ERROR
                                || ' Message: '
                                || L_SQL_ERRM);
        INSERT INTO log_messages (CALLER,  SQL_ERROR_CODE, ERROR_MESSAGE, CREATED_ON) 
        VALUES ('Post validation script', L_SQL_ERROR, L_SQL_ERRM, sysdate);
END;