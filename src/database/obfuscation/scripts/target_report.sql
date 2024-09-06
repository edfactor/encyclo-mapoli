-- This file creates a report about the state of the reduced data

DECLARE
    -- Target inventory
    l_number_of_target_emp_ssns NUMBER;
    l_number_of_target_ben_ssns NUMBER;
    l_number_of_target_all_ssns NUMBER;

    l_number_of_ssr_ssns NUMBER;

    l_number_of_target_emp_psns NUMBER;
    l_number_of_target_ben_psns NUMBER;
    l_number_of_target_all_psns NUMBER;

    l_number_of_dem_ssns NUMBER;
    l_number_of_dem_psns NUMBER;
    l_number_of_ben_ssns NUMBER;
    l_number_of_ben_psns NUMBER;

    l_number_of_payrel_ssns NUMBER;
    l_number_of_payrel_psns NUMBER;

    l_number_of_payprofit_ssns NUMBER;
    l_number_of_payprofit_psns NUMBER;

    l_number_of_profdist_ssns NUMBER;
    l_number_of_profdist_pay_ssns NUMBER;  

    l_number_of_profdetail_ssns NUMBER;
    l_number_of_ss_profdetail_ssns NUMBER;

    l_number_of_profchecks_emp_ssns NUMBER;
    l_number_of_profchecks_pay_ssns NUMBER;

    l_number_of_profchecks_emp_psns NUMBER;

    -- These are for the employee-as-beneficiary situation
    l_number_of_empben_ssns NUMBER;
    l_number_of_empben_payben_ssns NUMBER;
    l_number_of_empben_payrel_ssns NUMBER;
    l_number_of_empben_payprofit_ssns NUMBER;
    l_number_of_empben_profdist_ssns NUMBER;
    l_number_of_empben_profdist_pay_ssns NUMBER;
    l_number_of_empben_profdetail_ssns NUMBER;
    l_number_of_empben_profchecks_ssns NUMBER;
    l_number_of_empben_profchecks_pay_ssns NUMBER;

    l_number_of_empben_psns NUMBER;
    l_number_of_empben_payben_psns NUMBER;
    l_number_of_empben_payrel_psns NUMBER;
    l_number_of_empben_payprofit_psns NUMBER;



BEGIN

    l_number_of_target_emp_ssns := 0;
    l_number_of_target_ben_ssns := 0;
    l_number_of_target_all_ssns := 0;

    l_number_of_ssr_ssns := 0;

    l_number_of_target_emp_psns := 0;
    l_number_of_target_ben_psns := 0;
    l_number_of_target_all_psns := 0;

    l_number_of_dem_ssns := 0;
    l_number_of_dem_psns := 0;
    l_number_of_ben_ssns := 0;
    l_number_of_ben_psns := 0;

    l_number_of_payrel_ssns := 0;
    l_number_of_payrel_psns := 0;

    l_number_of_payprofit_ssns := 0;
    l_number_of_payprofit_psns := 0;

    l_number_of_profdist_ssns := 0;
    l_number_of_profdist_pay_ssns := 0;  

    l_number_of_profdetail_ssns := 0;
    l_number_of_ss_profdetail_ssns := 0;

    l_number_of_profchecks_emp_ssns := 0;
    l_number_of_profchecks_pay_ssns := 0;

    l_number_of_profchecks_emp_psns := 0;

    -- These are for the employee-as-beneficiary situation
    l_number_of_empben_ssns := 0;
    l_number_of_empben_payben_ssns := 0;
    l_number_of_empben_payrel_ssns := 0;
    l_number_of_empben_payprofit_ssns := 0;
    l_number_of_empben_profdist_ssns := 0;
    l_number_of_empben_profdist_pay_ssns := 0;
    l_number_of_empben_profdetail_ssns := 0;
    l_number_of_empben_profchecks_ssns := 0;
    l_number_of_empben_profchecks_pay_ssns := 0;

    l_number_of_empben_psns := 0;
    l_number_of_empben_payben_psns := 0;
    l_number_of_empben_payrel_psns := 0;
    l_number_of_empben_payprofit_psns := 0;

    DBMS_OUTPUT.PUT_LINE('PRE-OBFUSCATION REPORT');
    DBMS_OUTPUT.PUT_LINE('-----------------------');

    DBMS_OUTPUT.PUT_LINE('SSN ANALYSIS');
    DBMS_OUTPUT.PUT_LINE(' ');

    SELECT COUNT(DISTINCT SSN) into l_number_of_target_all_ssns FROM TARGET_ALL_SSN;
    SELECT COUNT(DISTINCT SSN) into l_number_of_target_emp_ssns FROM TARGET_EMP_SSN;
    SELECT COUNT(DISTINCT SSN) into l_number_of_target_ben_ssns FROM TARGET_BEN_SSN;

    DBMS_OUTPUT.PUT_LINE('Unique SSNs in TARGET_ALL_SSN: ' || l_number_of_target_all_ssns);
    DBMS_OUTPUT.PUT_LINE('Unique SSNs in TARGET_EMP_SSN: ' || l_number_of_target_emp_ssns);
    DBMS_OUTPUT.PUT_LINE('Unique SSNs in TARGET_BEN_SSN: ' || l_number_of_target_ben_ssns);

    DBMS_OUTPUT.PUT_LINE('Extra rows in TARGET_ALL_SSN: ' || 
    (l_number_of_target_all_ssns - l_number_of_target_emp_ssns - l_number_of_target_ben_ssns));


    SELECT COUNT(DISTINCT DEM_SSN) into l_number_of_dem_ssns FROM DEMOGRAPHICS;
    SELECT COUNT(DISTINCT PYBEN_PAYSSN) into l_number_of_ben_ssns FROM PAYBEN;
    SELECT COUNT(DISTINCT PYREL_PAYSSN) into l_number_of_payrel_ssns FROM PAYREL;

    SELECT COUNT(DISTINCT SOC_SEC_NUMBER) into l_number_of_ssr_ssns FROM SOC_SEC_REC;
    SELECT COUNT(DISTINCT EMPLOYEE_SSN) into l_number_of_profchecks_emp_ssns FROM PROFIT_SHARE_CHECKS;
    SELECT COUNT(DISTINCT SSN_NUMBER) into l_number_of_profchecks_pay_ssns FROM PROFIT_SHARE_CHECKS;
    SELECT COUNT(DISTINCT PR_DET_S_SEC_NUMBER) into l_number_of_profdetail_ssns FROM PROFIT_DETAIL;
    SELECT COUNT(DISTINCT PR_SS_DET_S_SEC_NUMBER) into l_number_of_ss_profdetail_ssns FROM PROFIT_SS_DETAIL;
    SELECT COUNT(DISTINCT PROFDIST_SSN) into l_number_of_profdist_ssns FROM PROFDIST;
    SELECT COUNT(DISTINCT PROFDIST_PAYSSN) into l_number_of_profdist_pay_ssns FROM PROFDIST;
    SELECT COUNT(DISTINCT PAYPROF_SSN) into l_number_of_payprofit_ssns FROM PAYPROFIT;
    
    DBMS_OUTPUT.PUT_LINE(' ');
    DBMS_OUTPUT.PUT_LINE('Unique SSNs in DEMOGRAPHICS: ' || l_number_of_dem_ssns);
    DBMS_OUTPUT.PUT_LINE('Unique SSNs in PAYBEN: ' || l_number_of_ben_ssns);
    DBMS_OUTPUT.PUT_LINE('Unique SSNs in PAYREL: ' || l_number_of_payrel_ssns);
    DBMS_OUTPUT.PUT_LINE('Unique SSNs in SOC_SEC_REC: ' || l_number_of_ssr_ssns);
    DBMS_OUTPUT.PUT_LINE('Unique Employee SSNs in PROFIT_SHARE_CHECKS: ' || l_number_of_profchecks_emp_ssns);
    DBMS_OUTPUT.PUT_LINE('Unique Pay SSNs in PROFIT_SHARE_CHECKS: ' || l_number_of_profchecks_pay_ssns);
    DBMS_OUTPUT.PUT_LINE('Unique SSNs in PROFIT_DETAIL: ' || l_number_of_profdetail_ssns);
    DBMS_OUTPUT.PUT_LINE('Unique SSNs in PROFIT_SS_DETAIL: ' || l_number_of_ss_profdetail_ssns);
    DBMS_OUTPUT.PUT_LINE('Unique SSNs in PROFDIST: ' || l_number_of_profdist_ssns);
    DBMS_OUTPUT.PUT_LINE('Unique Pay SSNs in PROFDIST: ' || l_number_of_profdist_pay_ssns);
    DBMS_OUTPUT.PUT_LINE('Unique SSNs in PAYPROFIT: ' || l_number_of_payprofit_ssns);

    DBMS_OUTPUT.PUT_LINE(' ');
    DBMS_OUTPUT.PUT_LINE('***Employees as Beneficiaries***');

    -- We need to store the employees as beneficiaries SSNs

    DELETE FROM EMP_AS_BEN_SSN;

    COMMIT;

    INSERT INTO EMP_AS_BEN_SSN (SSN) 
    (
    SELECT DISTINCT DEM_SSN FROM DEMOGRAPHICS
    INTERSECT
    SELECT DISTINCT PYBEN_PAYSSN FROM PAYBEN
    );

    SELECT COUNT(*) INTO l_number_of_empben_ssns FROM EMP_AS_BEN_SSN;

    -- Now we need to figure out where these emp-as-ben ssns appear
    SELECT COUNT(*) INTO l_number_of_empben_payben_ssns
    FROM (
        SELECT DISTINCT PYBEN_PAYSSN FROM PAYBEN
        INTERSECT
        SELECT DISTINCT SSN FROM EMP_AS_BEN_SSN
    );

    SELECT COUNT(*) INTO l_number_of_empben_payrel_ssns
    FROM (
        SELECT DISTINCT PYREL_PAYSSN FROM PAYREL
        INTERSECT
        SELECT DISTINCT SSN FROM EMP_AS_BEN_SSN
    );

    SELECT COUNT(*) INTO l_number_of_empben_payprofit_ssns
    FROM (
        SELECT DISTINCT PAYPROF_SSN FROM PAYPROFIT
        INTERSECT
        SELECT DISTINCT SSN FROM EMP_AS_BEN_SSN
    );

    SELECT COUNT(*) INTO l_number_of_empben_profdist_ssns
    FROM (
        SELECT DISTINCT PROFDIST_SSN FROM PROFDIST
        INTERSECT
        SELECT DISTINCT SSN FROM EMP_AS_BEN_SSN
    );

    SELECT COUNT(*) INTO l_number_of_empben_profdist_pay_ssns
    FROM (
        SELECT DISTINCT PROFDIST_PAYSSN FROM PROFDIST
        INTERSECT
        SELECT DISTINCT SSN FROM EMP_AS_BEN_SSN
    );

    SELECT COUNT(*) INTO l_number_of_empben_profdetail_ssns
    FROM (
        SELECT DISTINCT PR_DET_S_SEC_NUMBER FROM PROFIT_DETAIL
        INTERSECT
        SELECT DISTINCT SSN FROM EMP_AS_BEN_SSN
    );

    SELECT COUNT(*) INTO l_number_of_empben_profchecks_ssns
    FROM (
        SELECT DISTINCT EMPLOYEE_SSN FROM PROFIT_SHARE_CHECKS
        INTERSECT
        SELECT DISTINCT SSN FROM EMP_AS_BEN_SSN
    );

    SELECT COUNT(*) INTO l_number_of_empben_profchecks_pay_ssns
    FROM (
        SELECT DISTINCT SSN_NUMBER FROM PROFIT_SHARE_CHECKS
        INTERSECT
        SELECT DISTINCT SSN FROM EMP_AS_BEN_SSN
    );

    DBMS_OUTPUT.PUT_LINE('Number of Employee as Beneficiary SSNs: ' || l_number_of_empben_ssns);
    DBMS_OUTPUT.PUT_LINE('PAYBEN | # Employee as Beneficiary SSNs: ' || l_number_of_empben_payben_ssns);
    DBMS_OUTPUT.PUT_LINE('PAYREL | # Employee as Beneficiary SSNs: ' || l_number_of_empben_payrel_ssns);
    DBMS_OUTPUT.PUT_LINE('PAYPROFIT | # Employee as Beneficiary SSNs: ' || l_number_of_empben_payprofit_ssns);
    DBMS_OUTPUT.PUT_LINE('PROFDIST | # Employee as Beneficiary SSNs: ' || l_number_of_empben_profdist_ssns);
    DBMS_OUTPUT.PUT_LINE('PROFDIST | # Employee as Beneficiary PAY SSNs: ' || l_number_of_empben_profdist_pay_ssns); 
    DBMS_OUTPUT.PUT_LINE('PROFIT_DETAIL | # Employee as Beneficiary SSNs: ' || l_number_of_empben_profdetail_ssns);
    DBMS_OUTPUT.PUT_LINE('PROFIT_SHARE_CHECKS | # Employee as Beneficiary SSNs: ' || l_number_of_empben_profchecks_ssns);
    DBMS_OUTPUT.PUT_LINE('PROFIT_SHARE_CHECKS | # Employee as Beneficiary Pay SSNs: ' || l_number_of_empben_profchecks_pay_ssns);

END;