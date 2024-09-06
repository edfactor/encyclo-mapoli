
/*
    This script loads in records across tables with the minimum amount of field 
    data for the scripts to be fully exercised.
    
    You can change the parameters in the local variables to determine the amount of employees and
    related objects.
    
    Note: the SSN and PSN values will be negative instead of positive. This will allow all records to be 
    deleted cleanly BEFORE obfuscation, even if in the same schema as non-obfuscated data.
    
*/
DECLARE
    -- These numbers are the levers of how large the data set is
    -- They are set after the BEGIN statement, and should be 
    -- changed there

    l_employees NUMBER;
    -- How many beneficiaries per employee
    l_bens_per_employee NUMBER;
    l_dists_per_employee NUMBER;
    l_requests_per_employee NUMBER;

    l_start_year_details NUMBER;
    l_end_year_details NUMBER;
    
    l_detail_amount NUMBER;
    l_start_year_checks NUMBER;
    l_end_year_checks NUMBER;
    l_checks_per_year NUMBER;

    -- The number for the starting range of beneficiary SSNs
    l_ben_ssn NUMBER;

    -- These variables manage the database writes
    -- as we cannot add more than 100000 employees at once
    -- witout exceeding memory limits
    l_batch_size PLS_INTEGER;
    l_last PLS_INTEGER;   
    l_start PLS_INTEGER;   
    l_end PLS_INTEGER;
    
    l_employees_written PLS_INTEGER;
    l_beneficiaries_written PLS_INTEGER;
    l_distributions_written PLS_INTEGER;
    l_requests_written PLS_INTEGER;
    l_details_written PLS_INTEGER;
    l_ss_details_written PLS_INTEGER;
    l_checks_written PLS_INTEGER;
    l_soc_secs_written PLS_INTEGER;
    l_payprofits_written PLS_INTEGER;
    l_payrels_written PLS_INTEGER;
    l_emp_as_ben_written PLS_INTEGER;


    -- This one means "create one of these for every X records"
    l_profit_ss_chance PLS_INTEGER;
    l_emp_as_beneficiary_chance PLS_INTEGER;
    l_relative_chance PLS_INTEGER;

    -- This one will be the flag to know a ben as emp has occurred
    l_beneficiary_as_emp NUMBER;

    l_random_result NUMBER;

    -- These numbers below are just variables used to keep
    -- track of things
    l_error_count NUMBER;
    l_number_beneficiaries NUMBER;
    l_number_distributions NUMBER;
    l_number_requests NUMBER;
    l_number_details NUMBER;
    l_number_relatives NUMBER;
    l_number_checks NUMBER;
    l_number_payprofits NUMBER;
    l_details_per_year NUMBER;
    l_month_string VARCHAR(12);
    l_employee_dates employee_dates;
    l_emp_first_name VARCHAR(30);
    l_emp_last_name VARCHAR(30);
    l_emp_mid_name CHAR(1);
    l_emp_state CHAR(2);
    l_ben_state CHAR(2);
    
    l_last_index NUMBER;
    l_current_employee_index NUMBER;
    l_aux_last_index NUMBER;
    l_soc_sec_rec_no NUMBER;

    l_profit_ss_recno NUMBER;
    l_profit_ss_ssno NUMBER;
    
    -- These records and nested tables below are for keeping track
    -- of what has been created to do the bulk inserts that create
    -- all the records

    TYPE employees_nt IS TABLE of DEMOGRAPHICS%ROWTYPE;
    l_employees_nt employees_nt;

    TYPE beneficiaries_nt IS TABLE of PAYBEN%ROWTYPE;
    l_beneficiaries_nt beneficiaries_nt;

    TYPE soc_sec_nt IS TABLE of SOC_SEC_REC%ROWTYPE;
    l_soc_sec_nt soc_sec_nt;

    TYPE distributions_nt IS TABLE of PROFDIST%ROWTYPE;
    l_distributions_nt distributions_nt;
    
    TYPE distrib_requests_nt IS TABLE of PROFIT_DIST_REQ%ROWTYPE;
    l_dist_reqs_nt distrib_requests_nt;

    TYPE details_nt IS TABLE of PROFIT_DETAIL%ROWTYPE;
    l_details_nt details_nt;

    TYPE ss_details_nt IS TABLE of PROFIT_SS_DETAIL%ROWTYPE;
    l_ss_details_nt ss_details_nt;

    TYPE checks_nt IS TABLE of PROFIT_SHARE_CHECKS%ROWTYPE;
    l_checks_nt checks_nt;

    TYPE payprofits_nt IS TABLE of PAYPROFIT%ROWTYPE;
    l_payprofits_nt payprofits_nt;

    TYPE payrel_nt IS TABLE of PAYREL%ROWTYPE;
    l_payrel_nt payrel_nt;
    
    l_payrel_record PAYREL_RECORD;
    l_payprofit_record PAYPROFIT_RECORD;
    l_profit_detail_record PROFIT_DETAIL_RECORD;
    l_profit_ss_detail_record PROFIT_SS_DETAIL_RECORD;
    l_distribution_request_record DISTRIBUTION_REQUEST_RECORD;
    l_profit_distribution_record PROFIT_DISTRIBUTION_RECORD;

BEGIN

    l_employees :=  500;
    l_bens_per_employee := 1;

    l_soc_sec_rec_no := 1;
   
    -- Cannot be more than 9 'distributions' per employee for PROFDIST table
    l_dists_per_employee := 2;
    l_requests_per_employee := 4;
    l_details_per_year := 4;
    l_start_year_details := 2018;
    l_end_year_details := 2023;
    l_detail_amount := 50;
    l_checks_per_year := 2;
    l_start_year_checks := 2018;
    l_end_year_checks := 2023;

    l_ben_ssn := 200000000;

    -- So every 50 or so records create a PROFIT_SS_DETAIL record
    l_profit_ss_chance := 4;
    l_relative_chance := 20;
    l_emp_as_beneficiary_chance := 50;
    
    -- These are just working variables
    l_number_beneficiaries := 0;
    l_number_distributions := 0;
    l_number_requests := 0;
    l_number_details := 0;
    l_number_checks := 0;
    l_number_payprofits := 0;
    l_number_relatives := 0;

    l_profit_ss_recno := 0;
    l_profit_ss_ssno := 0;


    l_employees_written := 0;
    l_beneficiaries_written := 0;
    l_distributions_written := 0;
    l_requests_written := 0;
    l_details_written := 0;
    l_ss_details_written := 0;
    l_checks_written := 0;
    l_soc_secs_written := 0;
    l_payprofits_written := 0;
    l_payrels_written := 0;
    l_emp_as_ben_written := 0;

    -- This is a database processing parameter and should not change
    -- alongside the employee number
    l_batch_size := 10000;
    l_start := 1;
    l_last := l_employees;
    l_current_employee_index := 0;
    
    l_employees_nt := employees_nt();
    l_beneficiaries_nt := beneficiaries_nt();
    l_soc_sec_nt := soc_sec_nt();
    l_distributions_nt := distributions_nt();
    l_dist_reqs_nt := distrib_requests_nt();
    l_details_nt := details_nt();
    l_ss_details_nt := ss_details_nt();
    l_checks_nt := checks_nt();
    l_payprofits_nt := payprofits_nt();
    l_payrel_nt := payrel_nt();

    dbms_output.put_line('CREATING LARGE SYNTHETIC DATA SET OF ' || l_employees || ' employees and ' || (l_employees * l_bens_per_employee) || ' beneficiaries.');
    dbms_output.put_line('*********************************************');
    
    LOOP
        EXIT WHEN l_start > l_employees;
        
        l_end := LEAST (l_start + l_batch_size - 1, l_last); 
        dbms_output.put_line('Writing Batch of ' || l_batch_size || '| Start: ' || l_start || ' End: ' || l_end);
        for emp_counter IN l_start..l_end
        LOOP

        -- False as default
        l_beneficiary_as_emp := 0;

        l_random_result:= FLOOR(DBMS_RANDOM.VALUE(1, l_emp_as_beneficiary_chance));
        
        IF ((l_random_result = (l_emp_as_beneficiary_chance - 1)) and (emp_counter > 2)) THEN
            l_beneficiary_as_emp := 1;
        END IF;

        -- Create SOC_SEC_REC for employee
        l_soc_sec_nt.EXTEND;
        l_aux_last_index := l_soc_sec_nt.LAST;
        l_soc_sec_nt(l_aux_last_index).SOC_SEC_NUMBER := -emp_counter;
        l_soc_sec_nt(l_aux_last_index).SOC_SEC_RECNO := l_soc_sec_rec_no;
        l_soc_sec_rec_no := l_soc_sec_rec_no + 1;
        
        
        -- Create an employee
        l_employees_nt.EXTEND;
        l_current_employee_index := l_employees_nt.LAST;

        
        l_emp_first_name := get_a_first_name();
        l_emp_mid_name := get_a_middle_name();
        l_emp_last_name := get_a_last_name();
        l_employee_dates := get_employee_dates();
        l_emp_state := get_a_state();
        
        l_employees_nt(l_current_employee_index).DEM_SSN := -emp_counter;
        l_employees_nt(l_current_employee_index).DEM_BADGE := -emp_counter;
        l_employees_nt(l_current_employee_index).PY_FNAME := l_emp_first_name;
        l_employees_nt(l_current_employee_index).PY_LNAME := l_emp_last_name;
        l_employees_nt(l_current_employee_index).PY_MNAME := l_emp_mid_name;
        l_employees_nt(l_current_employee_index).PY_NAM := SUBSTR((l_emp_last_name || ', ' || l_emp_first_name), 1, 40);
        l_employees_nt(l_current_employee_index).PY_DOB := l_employee_dates.BIRTH_DATE;
        l_employees_nt(l_current_employee_index).PY_HIRE_DT := l_employee_dates.HIRE_DATE;
        l_employees_nt(l_current_employee_index).PY_REHIRE_DT := l_employee_dates.REHIRE_DATE;
        l_employees_nt(l_current_employee_index).PY_FULL_DT := l_employee_dates.FULL_TIME_DATE;
        l_employees_nt(l_current_employee_index).PY_TERM_DT := l_employee_dates.TERMINATION_DATE;
        -- If someone has been terminated, we need to set a flag
        IF (l_employee_dates.TERMINATION_DATE != 0) THEN
            l_employees_nt(l_current_employee_index).PY_TERM := substr('ABCDEFGHIJKLMNOPQRSTUVWXYZ', dbms_random.value(1,27), 1);
        ELSE
            l_employees_nt(l_current_employee_index).PY_TERM := ' ';
        END IF;
        l_employees_nt(l_current_employee_index).PY_ADD := get_an_address();
        l_employees_nt(l_current_employee_index).PY_STOR := get_a_store_number();
        l_employees_nt(l_current_employee_index).PY_CITY := get_a_city();
        l_employees_nt(l_current_employee_index).PY_GENDER := get_a_gender();
        l_employees_nt(l_current_employee_index).PY_STATE := l_emp_state;
        l_employees_nt(l_current_employee_index).PY_EMP_TELNO := get_a_phone_number();
        l_employees_nt(l_current_employee_index).PY_ZIP := get_zip_for_state(l_emp_state);

        l_employees_nt(l_current_employee_index).PY_SET_PWD := 0;
        l_employees_nt(l_current_employee_index).PY_SET_PWD_DT := NULL;

        l_employees_nt(l_current_employee_index).PY_SHOUR := get_hours();
        l_employees_nt(l_current_employee_index).PY_CLASS_DT := l_employee_dates.CLASS_DATE;
        l_employees_nt(l_current_employee_index).PY_GUID := 8;

        -- For each employee, do beneficiaries
      
        for ben_counter IN 1..l_bens_per_employee
        LOOP
            
            l_ben_state := get_a_state();
            l_number_beneficiaries := l_number_beneficiaries + 1;
            
            -- Create SOC_SEC_REC for beneficiary
            l_soc_sec_nt.EXTEND;
            l_aux_last_index := l_soc_sec_nt.LAST;
            l_soc_sec_nt(l_aux_last_index).SOC_SEC_NUMBER := -l_ben_ssn;
            l_soc_sec_nt(l_aux_last_index).SOC_SEC_RECNO := l_soc_sec_rec_no;
            l_soc_sec_rec_no := l_soc_sec_rec_no + 1;

            -- For each beneficiary
            l_beneficiaries_nt.EXTEND;
            l_last_index := l_beneficiaries_nt.LAST;

            l_beneficiaries_nt(l_last_index).PYBEN_NAME := SUBSTR((get_a_last_name() || ', ' || get_a_first_name()), 1, 40);
            l_beneficiaries_nt(l_last_index).PYBEN_CITY := get_a_city();
            l_beneficiaries_nt(l_last_index).PYBEN_ZIP := get_zip_for_state(l_ben_state);
            l_beneficiaries_nt(l_last_index).PYBEN_STATE := l_ben_state;
            l_beneficiaries_nt(l_last_index).PYBEN_DOBIRTH:= get_a_birth_date();
            l_beneficiaries_nt(l_last_index).PYBEN_ADD:= get_an_address();
            l_beneficiaries_nt(l_last_index).PYBEN_PAYSSN := -l_ben_ssn;

            -- Calculating the number with math only worked up to 214,700 then had an error. Interesting.
            --l_beneficiaries_nt(l_last_index).PYBEN_PSN := -((emp_counter * 10000) + (ben_counter * 1000));
            l_beneficiaries_nt(l_last_index).PYBEN_PSN := -TO_NUMBER( TO_CHAR(emp_counter) || TO_CHAR(ben_counter * 1000) );
            l_beneficiaries_nt(l_last_index).PYBEN_PERCENT := ROUND(100 / l_bens_per_employee);

            -- Do we make this a relative also
            l_random_result:= FLOOR(DBMS_RANDOM.VALUE(1, l_relative_chance));
            
            IF (l_random_result = (l_relative_chance - 1)) THEN
                l_payrel_record := GET_PAYREL_RECORD();
                l_payrel_nt.EXTEND;
                l_last_index := l_payrel_nt.LAST;

                -- Most of the time let us create the proper ben-matching PSN, but 
                -- once in a while, create an orphan PSN
                IF (FLOOR(DBMS_RANDOM.VALUE(1, 10)) = 2) THEN
                    l_payrel_nt(l_last_index).PYREL_PSN := ((-emp_counter * 10000) - (2000 * ben_counter));
                ELSE
                    l_payrel_nt(l_last_index).PYREL_PSN := -TO_NUMBER( TO_CHAR(emp_counter) || TO_CHAR(ben_counter * 1000) );
                END IF;
                
                l_payrel_nt(l_last_index).PYREL_PAYSSN:= -l_ben_ssn;
                l_payrel_nt(l_last_index).PYREL_TYPE := l_payrel_record.PYREL_TYPE;
                l_payrel_nt(l_last_index).PYREL_PERCENT := l_payrel_record.PYREL_PERCENT;
                l_payrel_nt(l_last_index).PYREL_PSAMT := l_payrel_record.PYREL_PSAMT;
                l_payrel_nt(l_last_index).PYREL_STATUS := l_payrel_record.PYREL_STATUS;
                l_payrel_nt(l_last_index).PYREL_REASON := l_payrel_record.PYREL_REASON;
                l_payrel_nt(l_last_index).PYREL_RELATION := l_payrel_record.PYREL_RELATION;

            END IF;

            l_ben_ssn := l_ben_ssn + 1;

        END LOOP; -- end of create all beneficiaries

        -- Now, if we should create an employee as a beneficiary, do so
        IF (l_beneficiary_as_emp = 1) THEN

            l_number_beneficiaries := l_number_beneficiaries + 1;

            -- Note: We don't need a SOC_SEC_REC entry as the
            -- emp we already have got one already

            -- For each beneficiary
            l_beneficiaries_nt.EXTEND;
            l_last_index := l_beneficiaries_nt.LAST;
            
            l_beneficiaries_nt(l_last_index).PYBEN_NAME := l_employees_nt(l_current_employee_index - 1).PY_NAM;
            l_beneficiaries_nt(l_last_index).PYBEN_CITY := l_employees_nt(l_current_employee_index - 1).PY_CITY;
            l_beneficiaries_nt(l_last_index).PYBEN_ZIP := l_employees_nt(l_current_employee_index - 1).PY_ZIP;
            l_beneficiaries_nt(l_last_index).PYBEN_STATE := l_employees_nt(l_current_employee_index - 1).PY_STATE;
            l_beneficiaries_nt(l_last_index).PYBEN_DOBIRTH:= l_employees_nt(l_current_employee_index - 1).PY_DOB;
            l_beneficiaries_nt(l_last_index).PYBEN_ADD:= l_employees_nt(l_current_employee_index - 1).PY_ADD;
            l_beneficiaries_nt(l_last_index).PYBEN_PAYSSN := l_employees_nt(l_current_employee_index - 1).DEM_SSN;
            -- This is just there to make testing easier
            l_beneficiaries_nt(l_last_index).PYBEN_PSDISB := 999;
            
            -- Calculating the number with math only worked up to 214,700 then had an error. Interesting.
            --l_beneficiaries_nt(l_last_index).PYBEN_PSN := -((emp_counter * 10000) + (ben_counter * 1000));
            l_beneficiaries_nt(l_last_index).PYBEN_PSN := -TO_NUMBER( TO_CHAR(emp_counter) || TO_CHAR((l_bens_per_employee + 1) * 1000) );
            l_beneficiaries_nt(l_last_index).PYBEN_PERCENT := 0; -- Yes, zero percent

            /*
            dbms_output.put_line('Got one! PYBEN_PAYSSN: ' || l_beneficiaries_nt(l_last_index).PYBEN_PAYSSN);
            dbms_output.put_line('Current employeee SSN: ' || l_employees_nt(emp_counter).DEM_SSN);
            dbms_output.put_line('Our employee SSN: ' || l_employees_nt(emp_counter - 1).DEM_SSN);
            */


            -- Now let us put them in the PAYREL table for testing purposes

            l_payrel_record := GET_PAYREL_RECORD();
            l_payrel_nt.EXTEND;
            l_last_index := l_payrel_nt.LAST;

            -- For PSN, just the badge number
            l_payrel_nt(l_last_index).PYREL_PSN := ((-emp_counter * 10000) - 6000);
            l_payrel_nt(l_last_index).PYREL_PAYSSN := l_employees_nt(l_current_employee_index - 1).DEM_SSN;
            l_payrel_nt(l_last_index).PYREL_TYPE := l_payrel_record.PYREL_TYPE;
            l_payrel_nt(l_last_index).PYREL_PERCENT := l_payrel_record.PYREL_PERCENT;
            l_payrel_nt(l_last_index).PYREL_PSAMT := l_payrel_record.PYREL_PSAMT;
            l_payrel_nt(l_last_index).PYREL_STATUS := l_payrel_record.PYREL_STATUS;
            -- Going to hardcode this to make the rows easier to see in testing
            l_payrel_nt(l_last_index).PYREL_REASON := 'Employee';
            l_payrel_nt(l_last_index).PYREL_RELATION := l_payrel_record.PYREL_RELATION;

            -- Let us give them a PROFIT_DETAIL
            l_number_details := l_number_details + 1;
            l_details_nt.EXTEND;
            l_last_index := l_details_nt.LAST;
            l_profit_detail_record := GET_PROFIT_DETAIL_RECORD((l_employees_nt(l_current_employee_index - 1).DEM_BADGE *10000) - 1000);
            
            l_details_nt(l_last_index).PROFIT_DET_PR_DET_S_SEQNUM := l_number_details;
            l_details_nt(l_last_index).PR_DET_S_SEC_NUMBER := l_employees_nt(l_current_employee_index - 1).DEM_SSN;
            l_details_nt(l_last_index).PROFIT_YEAR := l_start_year_details;
            l_details_nt(l_last_index).PROFIT_EARN := 33;
            l_details_nt(l_last_index).PROFIT_CODE := l_profit_detail_record.CODE;
            l_details_nt(l_last_index).PROFIT_CMNT := l_profit_detail_record.CMNT;
            l_details_nt(l_last_index).PROFIT_YDTE := substr(l_start_year_details, -2);
            l_details_nt(l_last_index).PROFIT_DET_RECNO := l_number_details;
            -- Setting this just to make these rows easier to see in database
            l_details_nt(l_last_index).PROFIT_TAX_CODE := 'X';


            -- And a PAYPROFIT
            l_payprofit_record := get_payprofit_record();
            l_payprofits_nt.EXTEND;
            l_last_index := l_payprofits_nt.LAST;
                
            l_payprofits_nt(l_last_index).PAYPROF_BADGE := l_employees_nt(emp_counter - 1).DEM_BADGE;
            l_payprofits_nt(l_last_index).PAYPROF_SSN := l_employees_nt(emp_counter - 1).DEM_SSN;
            l_payprofits_nt(l_last_index).PY_PH := l_payprofit_record.PY_PH;
            l_payprofits_nt(l_last_index).PY_PD := l_payprofit_record.PY_PD;
            l_payprofits_nt(l_last_index).PY_WEEKS_WORK := l_payprofit_record.PY_WEEKS_WORK;
            l_payprofits_nt(l_last_index).PY_PROF_CERT := l_payprofit_record.PY_PROF_CERT;
            l_payprofits_nt(l_last_index).PY_PS_ENROLLED := l_payprofit_record.PY_PS_ENROLLED;
            l_payprofits_nt(l_last_index).PY_PS_YEARS := l_payprofit_record.PY_PS_YEARS;
            l_payprofits_nt(l_last_index).PY_PROF_BENEFICIARY := 1;
                  
            -- Reset
            l_beneficiary_as_emp := 0;

            l_ben_ssn := l_ben_ssn + 1;
            l_emp_as_ben_written := l_emp_as_ben_written + 1;
            --DBMS_OUTPUT.PUT_LINE('EMP as BEN SSN was: ' || l_employees_nt(emp_counter - 1).DEM_SSN);

        END IF;
        
        -- create distributions
        for dist_counter IN 1..l_dists_per_employee
        LOOP
            
            l_number_distributions := l_number_distributions + 1;
            l_distributions_nt.EXTEND;
            l_profit_distribution_record := GET_PROFDIST_RECORD();
            l_last_index := l_distributions_nt.LAST;
            l_distributions_nt(l_last_index).PROFDIST_PAYSEQ := dist_counter;
            l_distributions_nt(l_last_index).PROFDIST_SSN := -emp_counter;
            l_distributions_nt(l_last_index).PROFDIST_PAYFREQ := l_profit_distribution_record.PAYFREQ;
            l_distributions_nt(l_last_index).PROFDIST_CHECKAMT := l_profit_distribution_record.CHECKAMT;
            l_distributions_nt(l_last_index).PROFDIST_PAYFLAG := l_profit_distribution_record.PAYFLAG;
            l_distributions_nt(l_last_index).PROFDIST_FBOPAYTO := l_profit_distribution_record.FBOPAYTO;
            l_distributions_nt(l_last_index).PROFDIST_SEX := l_profit_distribution_record.SEX;

        END LOOP;

        -- Now we need distribution requests
        for dist_req_counter IN 1..l_requests_per_employee
        LOOP
        
            l_number_requests := l_number_requests + 1;
            l_dist_reqs_nt.EXTEND;
            l_last_index := l_dist_reqs_nt.LAST;
            l_distribution_request_record := GET_PROF_DIST_REQ_RECORD();
            l_dist_reqs_nt(l_last_index).PROFIT_DIST_REQ_SEQ_NUM := l_number_requests;
            l_dist_reqs_nt(l_last_index).PROFIT_DIST_REQ_EMP := -emp_counter;
            -- Let us create some orphan PSNS
            IF (FLOOR(DBMS_RANDOM.VALUE(1, 50)) = 2) THEN
                l_dist_reqs_nt(l_last_index).PROFIT_DIST_REQ_PSN := (((-90999 + -emp_counter) * 10000) - 1000);
            END IF;
            l_dist_reqs_nt(l_last_index).PROFIT_DIST_REQ_AMT_REQ := l_distribution_request_record.REQ_AMT_REQ;
            l_dist_reqs_nt(l_last_index).PROFIT_DIST_REQ_TYPE := l_distribution_request_record.REQ_TYPE;
            l_dist_reqs_nt(l_last_index).PROFIT_DIST_REQ_REASON := l_distribution_request_record.REQ_REASON;
            
        
        END LOOP;

        -- Now we need profit sharing details for the year range dictated by the variables
        for detail_year_counter IN l_start_year_details..l_end_year_details
        LOOP
            for inner_counter IN 1..l_details_per_year
            LOOP
                l_number_details := l_number_details + 1;
                l_details_nt.EXTEND;
                l_last_index := l_details_nt.LAST;
                -- Here we will use employee badge, but must add 0000 to end
                l_profit_detail_record := GET_PROFIT_DETAIL_RECORD(-emp_counter * 10000);
                
                l_details_nt(l_last_index).PROFIT_DET_PR_DET_S_SEQNUM := l_number_details;
                l_details_nt(l_last_index).PR_DET_S_SEC_NUMBER := -emp_counter;
                l_details_nt(l_last_index).PROFIT_YEAR := detail_year_counter;
                l_details_nt(l_last_index).PROFIT_EARN := l_profit_detail_record.EARN;
                l_details_nt(l_last_index).PROFIT_CMNT := l_profit_detail_record.CMNT;
                l_details_nt(l_last_index).PROFIT_CODE := l_profit_detail_record.CODE;
                l_details_nt(l_last_index).PROFIT_YDTE := substr(detail_year_counter, -2);
                l_details_nt(l_last_index).PROFIT_DET_RECNO := l_number_details;
            END LOOP;
        END LOOP;

        -- We do a PROFIT_SS_DETAIL

        l_profit_ss_recno := l_profit_ss_recno + emp_counter; 
        l_ss_details_nt.EXTEND;
        l_last_index := l_ss_details_nt.LAST;
        l_profit_ss_detail_record := GET_PROFIT_SS_DETAIL_RECORD(-emp_counter * 10000);

        l_ss_details_nt(l_last_index).PROFIT_SS_YEAR := l_profit_ss_detail_record.PROFIT_SS_YEAR;
        l_ss_details_nt(l_last_index).PROFIT_SS_CLIENT := l_profit_ss_detail_record.PROFIT_SS_CLIENT;
        l_ss_details_nt(l_last_index).PROFIT_SS_CODE := l_profit_ss_detail_record.PROFIT_SS_CODE;
        l_ss_details_nt(l_last_index).PROFIT_SS_CONT := l_profit_ss_detail_record.PROFIT_SS_CONT;
        l_ss_details_nt(l_last_index).PROFIT_SS_EARN := 25;
        l_ss_details_nt(l_last_index).PROFIT_SS_FORT := l_profit_ss_detail_record.PROFIT_SS_FORT;
        l_ss_details_nt(l_last_index).PROFIT_SS_MDTE := l_profit_ss_detail_record.PROFIT_SS_MDTE;
        l_ss_details_nt(l_last_index).PROFIT_SS_YDTE := l_profit_ss_detail_record.PROFIT_SS_YDTE;
        l_ss_details_nt(l_last_index).PROFIT_SS_CMNT := l_profit_ss_detail_record.PROFIT_SS_CMNT;
        l_ss_details_nt(l_last_index).PROFIT_SS_ZEROCONT := l_profit_ss_detail_record.PROFIT_SS_ZEROCONT;
        l_ss_details_nt(l_last_index).PROFIT_SS_FED_TAXES := l_profit_ss_detail_record.PROFIT_SS_FED_TAXES;
        l_ss_details_nt(l_last_index).PROFIT_SS_STATE_TAXES := l_profit_ss_detail_record.PROFIT_SS_STATE_TAXES;
        l_ss_details_nt(l_last_index).PROFIT_SS_TAX_CODE := l_profit_ss_detail_record.PROFIT_SS_TAX_CODE;
        l_ss_details_nt(l_last_index).PROFIT_SS_SSNO := -l_profit_ss_detail_record.PROFIT_SS_SSNO;

        -- Saving this in case we do a duplicate
        l_profit_ss_ssno := -l_profit_ss_detail_record.PROFIT_SS_SSNO;
        
        l_ss_details_nt(l_last_index).PROFIT_SS_NAME := SUBSTR(l_employees_nt(l_current_employee_index).PY_NAM, 1, 25);
        l_ss_details_nt(l_last_index).PROFIT_SS_ADDRESS := SUBSTR(l_employees_nt(l_current_employee_index).PY_STATE, 1, 20);
        l_ss_details_nt(l_last_index).PROFIT_SS_CITY := l_employees_nt(l_current_employee_index).PY_CITY;
        l_ss_details_nt(l_last_index).PROFIT_SS_STATE := l_employees_nt(l_current_employee_index).PY_STATE;
        l_ss_details_nt(l_last_index).PROFIT_SS_ZIP := l_employees_nt(l_current_employee_index).PY_ZIP;
        
        l_ss_details_nt(l_last_index).PROFIT_SS_DET_RECNO := l_profit_ss_recno;
        l_ss_details_nt(l_last_index).PROFIT_SS_DET_PR_SS_D_S_SEQNUM := l_profit_ss_recno;
        l_ss_details_nt(l_last_index).PR_SS_D_S_SEC_NUMBER := -emp_counter;

        -- Perhaps we do two PROFIT_SS_DETAIL RECORDs
        l_random_result:= FLOOR(DBMS_RANDOM.VALUE(1, l_profit_ss_chance));
        
       IF (l_random_result = 3) THEN
            l_profit_ss_recno := l_profit_ss_recno + 1;
            l_ss_details_nt.EXTEND;
            l_last_index := l_ss_details_nt.LAST;
            l_profit_ss_detail_record := GET_PROFIT_SS_DETAIL_RECORD(-emp_counter * 10000);

            l_ss_details_nt(l_last_index).PROFIT_SS_YEAR := l_profit_ss_detail_record.PROFIT_SS_YEAR;
            l_ss_details_nt(l_last_index).PROFIT_SS_CLIENT := l_profit_ss_detail_record.PROFIT_SS_CLIENT;
            l_ss_details_nt(l_last_index).PROFIT_SS_CODE := l_profit_ss_detail_record.PROFIT_SS_CODE;
            l_ss_details_nt(l_last_index).PROFIT_SS_CONT := l_profit_ss_detail_record.PROFIT_SS_CONT;
            l_ss_details_nt(l_last_index).PROFIT_SS_EARN := 25;
            l_ss_details_nt(l_last_index).PROFIT_SS_FORT := l_profit_ss_detail_record.PROFIT_SS_FORT;
            l_ss_details_nt(l_last_index).PROFIT_SS_MDTE := l_profit_ss_detail_record.PROFIT_SS_MDTE;
            l_ss_details_nt(l_last_index).PROFIT_SS_YDTE := l_profit_ss_detail_record.PROFIT_SS_YDTE;
            l_ss_details_nt(l_last_index).PROFIT_SS_CMNT := l_profit_ss_detail_record.PROFIT_SS_CMNT;
            l_ss_details_nt(l_last_index).PROFIT_SS_ZEROCONT := l_profit_ss_detail_record.PROFIT_SS_ZEROCONT;
            l_ss_details_nt(l_last_index).PROFIT_SS_FED_TAXES := l_profit_ss_detail_record.PROFIT_SS_FED_TAXES;
            l_ss_details_nt(l_last_index).PROFIT_SS_STATE_TAXES := l_profit_ss_detail_record.PROFIT_SS_STATE_TAXES;
            l_ss_details_nt(l_last_index).PROFIT_SS_TAX_CODE := l_profit_ss_detail_record.PROFIT_SS_TAX_CODE;
            l_ss_details_nt(l_last_index).PROFIT_SS_SSNO := l_profit_ss_ssno;
            
            l_ss_details_nt(l_last_index).PROFIT_SS_NAME := SUBSTR(l_employees_nt(l_current_employee_index).PY_NAM, 1, 25);
            l_ss_details_nt(l_last_index).PROFIT_SS_ADDRESS := SUBSTR(l_employees_nt(l_current_employee_index).PY_STATE, 1, 20);
            l_ss_details_nt(l_last_index).PROFIT_SS_CITY := l_employees_nt(l_current_employee_index).PY_CITY;
            l_ss_details_nt(l_last_index).PROFIT_SS_STATE := l_employees_nt(l_current_employee_index).PY_STATE;
            l_ss_details_nt(l_last_index).PROFIT_SS_ZIP := l_employees_nt(l_current_employee_index).PY_ZIP;
            
            l_ss_details_nt(l_last_index).PROFIT_SS_DET_RECNO := l_profit_ss_recno;
            l_ss_details_nt(l_last_index).PROFIT_SS_DET_PR_SS_D_S_SEQNUM := l_profit_ss_recno;
            l_ss_details_nt(l_last_index).PR_SS_D_S_SEC_NUMBER := -emp_counter;


       END IF;


        -- Now we need checks 
        for check_counter IN l_start_year_details..l_end_year_details
            LOOP
                for inner_check_counter IN 1..l_checks_per_year
                    LOOP
                        IF inner_check_counter < 10 THEN
                            l_month_string := '0' || inner_check_counter;
                        ELSE
                            l_month_string := '' || inner_check_counter;
                        END IF;
                        
                        l_number_checks := l_number_checks + 1;
                        l_checks_nt.EXTEND;
                        l_last_index := l_checks_nt.LAST;
                        l_checks_nt(l_last_index).CHECK_NUMBER := (100000 + l_number_checks);
                        l_checks_nt(l_last_index).PAYABLE_NAME := SUBSTR((l_emp_first_name || ' ' || l_emp_last_name), 1, 40);
                        l_checks_nt(l_last_index).SSN_NUMBER := -emp_counter;
                        l_checks_nt(l_last_index).EMPLOYEE_SSN := -emp_counter;
                        l_checks_nt(l_last_index).CHECK_DATE := 
                        TO_DATE(('' || check_counter || l_month_string || '01'), 'yyyymmdd');   
                    END LOOP;
            END LOOP;
            
        -- PAYPROFIT - add one dummy record per employee

        l_payprofit_record := get_payprofit_record();
        l_payprofits_nt.EXTEND;
        l_last_index := l_payprofits_nt.LAST;
              
        l_payprofits_nt(l_last_index).PAYPROF_BADGE := -emp_counter;
        l_payprofits_nt(l_last_index).PAYPROF_SSN := -emp_counter;
        l_payprofits_nt(l_last_index).PY_PH := l_payprofit_record.PY_PH;
        l_payprofits_nt(l_last_index).PY_PD := l_payprofit_record.PY_PD;
        l_payprofits_nt(l_last_index).PY_WEEKS_WORK := l_payprofit_record.PY_WEEKS_WORK;
        l_payprofits_nt(l_last_index).PY_PROF_CERT := l_payprofit_record.PY_PROF_CERT;
        l_payprofits_nt(l_last_index).PY_PS_ENROLLED := l_payprofit_record.PY_PS_ENROLLED;
        l_payprofits_nt(l_last_index).PY_PS_YEARS := l_payprofit_record.PY_PS_YEARS;
        l_payprofits_nt(l_last_index).PY_PROF_BENEFICIARY := 0;
    
    END LOOP;
    -- Now we need a FORALL to add all employees
    
    FORALL L_INDEX IN 1 .. l_employees_nt.COUNT 
        INSERT INTO DEMOGRAPHICS (
            DEM_BADGE,
            DEM_SSN,
            PY_NAM,
            PY_LNAME,
            PY_FNAME,
            PY_MNAME,
            PY_ADD,
            PY_CITY,
            PY_STATE,
            PY_ZIP,
            PY_DOB,
            PY_HIRE_DT,
            PY_FULL_DT,
            PY_REHIRE_DT,
            PY_TERM_DT,
            PY_TERM,
            PY_GENDER,
            PY_ASSIGN_ID,
            PY_ASSIGN_DESC,
            PY_STOR,
            PY_EMP_TELNO,
            PY_SHOUR,
            PY_CLASS_DT,
            PY_GUID
        )
        VALUES (
            l_employees_nt(L_INDEX).DEM_BADGE,
            l_employees_nt(L_INDEX).DEM_SSN,
            l_employees_nt(L_INDEX).PY_NAM,
            l_employees_nt(L_INDEX).PY_LNAME,
            l_employees_nt(L_INDEX).PY_FNAME,
            l_employees_nt(L_INDEX).PY_MNAME,
            l_employees_nt(L_INDEX).PY_ADD,
            l_employees_nt(L_INDEX).PY_CITY,
            l_employees_nt(L_INDEX).PY_STATE,
            l_employees_nt(L_INDEX).PY_ZIP,
            l_employees_nt(L_INDEX).PY_DOB,
            l_employees_nt(L_INDEX).PY_HIRE_DT,
            l_employees_nt(L_INDEX).PY_FULL_DT,
            l_employees_nt(L_INDEX).PY_REHIRE_DT,
            l_employees_nt(L_INDEX).PY_TERM_DT,
            l_employees_nt(L_INDEX).PY_TERM,
            l_employees_nt(L_INDEX).PY_GENDER,
            37382,
            'PASSPHRASE',
            l_employees_nt(L_INDEX).PY_STOR,
            l_employees_nt(L_INDEX).PY_EMP_TELNO,
            l_employees_nt(L_INDEX).PY_SHOUR,
            l_employees_nt(L_INDEX).PY_CLASS_DT,
            l_employees_nt(L_INDEX).PY_GUID
        );
        
    DBMS_OUTPUT.PUT_LINE('Number of employees inserted: ' || l_employees_nt.COUNT);
    l_employees_written := l_employees_written +  l_employees_nt.COUNT;
    l_employees_nt.DELETE;
    
    -- Now we need a FORALL to add all beneficiaries
    
    FORALL L_INDEX IN 1 .. l_beneficiaries_nt.COUNT 
        INSERT INTO PAYBEN (
            PYBEN_PSN, 
            PYBEN_PAYSSN, 
            PYBEN_NAME,
            PYBEN_ADD,
            PYBEN_CITY,
            PYBEN_ZIP,
            PYBEN_STATE,
            PYBEN_DOBIRTH,
            PYBEN_PERCENT,
            PYBEN_PSDISB
        )
        VALUES (
            l_beneficiaries_nt(L_INDEX).PYBEN_PSN, 
            l_beneficiaries_nt(L_INDEX).PYBEN_PAYSSN, 
            l_beneficiaries_nt(L_INDEX).PYBEN_NAME,
            l_beneficiaries_nt(L_INDEX).PYBEN_ADD,
            l_beneficiaries_nt(L_INDEX).PYBEN_CITY,
            l_beneficiaries_nt(L_INDEX).PYBEN_ZIP,
            l_beneficiaries_nt(L_INDEX).PYBEN_STATE,
            l_beneficiaries_nt(L_INDEX).PYBEN_DOBIRTH,
            l_beneficiaries_nt(L_INDEX).PYBEN_PERCENT,
            l_beneficiaries_nt(L_INDEX).PYBEN_PSDISB
        );
        
    DBMS_OUTPUT.PUT_LINE('Number of beneficiaries inserted: ' || l_beneficiaries_nt.COUNT);
    l_beneficiaries_written := l_beneficiaries_written +  l_beneficiaries_nt.COUNT;
    l_beneficiaries_nt.DELETE;

    -- Now we need a FORALL to add all relatives
    
    FORALL L_INDEX IN 1 .. l_payrel_nt.COUNT 
        INSERT INTO PAYREL (
            PYREL_PSN,
            PYREL_PAYSSN,
            PYREL_TYPE,
            PYREL_PERCENT,
            PYREL_PSAMT,
            PYREL_STATUS,
            PYREL_REASON,
            PYREL_RELATION
        )
        VALUES (
            l_payrel_nt(L_INDEX).PYREL_PSN,
            l_payrel_nt(L_INDEX).PYREL_PAYSSN,
            l_payrel_nt(L_INDEX).PYREL_TYPE,
            l_payrel_nt(L_INDEX).PYREL_PERCENT,
            l_payrel_nt(L_INDEX).PYREL_PSAMT,
            l_payrel_nt(L_INDEX).PYREL_STATUS,
            l_payrel_nt(L_INDEX).PYREL_REASON,
            l_payrel_nt(L_INDEX).PYREL_RELATION
        );
        
    DBMS_OUTPUT.PUT_LINE('Number of relatives inserted: ' || l_payrel_nt.COUNT);
    l_payrels_written := l_payrels_written +  l_payrel_nt.COUNT;
    l_payrel_nt.DELETE;
    
    -- Add all PAYPROFITS
    
    FORALL L_INDEX IN 1 .. l_payprofits_nt.COUNT 
    
        INSERT INTO PAYPROFIT (
            PAYPROF_BADGE,
            PAYPROF_SSN, 
            PY_PH, 
            PY_PD, 
            PY_WEEKS_WORK, 
            PY_PROF_CERT,
            PY_PS_ENROLLED,
            PY_PS_YEARS,
            PY_PROF_BENEFICIARY
            )
        VALUES (
            l_payprofits_nt(L_INDEX).PAYPROF_BADGE,
            l_payprofits_nt(L_INDEX).PAYPROF_SSN,
            l_payprofits_nt(L_INDEX).PY_PH,
            l_payprofits_nt(L_INDEX).PY_PD,
            l_payprofits_nt(L_INDEX).PY_WEEKS_WORK,
            l_payprofits_nt(L_INDEX).PY_PROF_CERT,
            l_payprofits_nt(L_INDEX).PY_PS_ENROLLED,
            l_payprofits_nt(L_INDEX).PY_PS_YEARS,
            l_payprofits_nt(L_INDEX).PY_PROF_BENEFICIARY
        );
    
    DBMS_OUTPUT.PUT_LINE('Number of payprofits inserted: ' || l_payprofits_nt.COUNT);
    l_payprofits_written := l_payprofits_written +  l_payprofits_nt.COUNT;
    l_payprofits_nt.DELETE;

    -- Add all distributions
    
    FORALL L_INDEX IN 1 .. l_distributions_nt.COUNT 
    
        INSERT INTO PROFDIST (
            PROFDIST_PAYSEQ, 
            PROFDIST_SSN, 
            PROFDIST_PAYSSN, 
            PROFDIST_PAYNAME, 
            PROFDIST_EMPNAME,
            PROFDIST_PAYFREQ,
            PROFDIST_PAYFLAG,
            PROFDIST_CHECKAMT,
            PROFDIST_FBOPAYTO)
        VALUES (
            l_distributions_nt(L_INDEX).PROFDIST_PAYSEQ, 
            l_distributions_nt(L_INDEX).PROFDIST_SSN, 
            l_distributions_nt(L_INDEX).PROFDIST_SSN, 
            'Jane Smith', -- Doesn't really matter who the payable is before obfuscation
            (l_emp_first_name || ' ' || l_emp_last_name),
            l_distributions_nt(L_INDEX).PROFDIST_PAYFREQ,
            l_distributions_nt(L_INDEX).PROFDIST_PAYFLAG,
            l_distributions_nt(L_INDEX).PROFDIST_CHECKAMT,
            l_distributions_nt(L_INDEX).PROFDIST_FBOPAYTO
        );
    
    DBMS_OUTPUT.PUT_LINE('Number of distributions inserted: ' || l_distributions_nt.COUNT);
    l_distributions_written := l_distributions_written +  l_distributions_nt.COUNT;
    l_distributions_nt.DELETE;

    -- Add all distribution requests
    
    FORALL L_INDEX IN 1 .. l_dist_reqs_nt.COUNT 
        INSERT INTO PROFIT_DIST_REQ (
            PROFIT_DIST_REQ_SEQ_NUM, 
            PROFIT_DIST_REQ_PSN,
            PROFIT_DIST_REQ_EMP, 
            PROFIT_DIST_REQ_TYPE,
            PROFIT_DIST_REQ_REASON,
            PROFIT_DIST_REQ_AMT_REQ
        )
        VALUES (
                l_dist_reqs_nt(L_INDEX).PROFIT_DIST_REQ_SEQ_NUM, 
                l_dist_reqs_nt(L_INDEX).PROFIT_DIST_REQ_PSN,
                l_dist_reqs_nt(L_INDEX).PROFIT_DIST_REQ_EMP,
                l_dist_reqs_nt(L_INDEX).PROFIT_DIST_REQ_TYPE,
                l_dist_reqs_nt(L_INDEX).PROFIT_DIST_REQ_REASON,
                l_dist_reqs_nt(L_INDEX).PROFIT_DIST_REQ_AMT_REQ

        );
    
    DBMS_OUTPUT.PUT_LINE('Number of distribution requests inserted: '  || l_dist_reqs_nt.COUNT);
    l_requests_written := l_requests_written +  l_dist_reqs_nt.COUNT;
    l_dist_reqs_nt.DELETE;

    -- Transaction details
    
    FORALL L_INDEX IN 1 .. l_details_nt.COUNT 
        INSERT INTO PROFIT_DETAIL (
                    PROFIT_DET_PR_DET_S_SEQNUM,
                    PR_DET_S_SEC_NUMBER, 
                    PROFIT_YEAR,
                    PROFIT_CODE,
                    PROFIT_YDTE, 
                    PROFIT_CMNT,
                    PROFIT_EARN,
                    PROFIT_DET_RECNO,
                    PROFIT_TAX_CODE)
        VALUES (l_details_nt(L_INDEX).PROFIT_DET_PR_DET_S_SEQNUM, 
                l_details_nt(L_INDEX).PR_DET_S_SEC_NUMBER,
                l_details_nt(L_INDEX).PROFIT_YEAR,
                l_details_nt(L_INDEX).PROFIT_CODE,
                l_details_nt(L_INDEX).PROFIT_YDTE,
                l_details_nt(L_INDEX).PROFIT_CMNT,
                l_details_nt(L_INDEX).PROFIT_EARN,
                l_details_nt(L_INDEX).PROFIT_DET_RECNO,
                l_details_nt(L_INDEX).PROFIT_TAX_CODE);

    
    DBMS_OUTPUT.PUT_LINE('Number of profit details inserted: ' || l_details_nt.COUNT);
    l_details_written := l_details_written +  l_details_nt.COUNT;
    l_details_nt.DELETE;

    -- PROFIT_SS_DETAILS

    FORALL L_INDEX IN 1 .. l_ss_details_nt.COUNT 
    INSERT INTO PROFIT_SS_DETAIL (
        PROFIT_SS_YEAR,
        PROFIT_SS_CLIENT,
        PROFIT_SS_CODE,
        PROFIT_SS_CONT,
        PROFIT_SS_EARN,
        PROFIT_SS_FORT,
        PROFIT_SS_MDTE,
        PROFIT_SS_YDTE,
        PROFIT_SS_CMNT,
        PROFIT_SS_ZEROCONT,
        PROFIT_SS_FED_TAXES,
        PROFIT_SS_STATE_TAXES,
        PROFIT_SS_TAX_CODE,
        PROFIT_SS_SSNO,
        PROFIT_SS_NAME,
        PROFIT_SS_ADDRESS,
        PROFIT_SS_CITY,
        PROFIT_SS_STATE,
        PROFIT_SS_ZIP,
        PROFIT_SS_DET_RECNO,
        PROFIT_SS_DET_PR_SS_D_S_SEQNUM,
        PR_SS_D_S_SEC_NUMBER)
    VALUES (
        l_ss_details_nt(L_INDEX).PROFIT_SS_YEAR,
        l_ss_details_nt(L_INDEX).PROFIT_SS_CLIENT,
        l_ss_details_nt(L_INDEX).PROFIT_SS_CODE,
        l_ss_details_nt(L_INDEX).PROFIT_SS_CONT,
        l_ss_details_nt(L_INDEX).PROFIT_SS_EARN,
        l_ss_details_nt(L_INDEX).PROFIT_SS_FORT,
        l_ss_details_nt(L_INDEX).PROFIT_SS_MDTE,
        l_ss_details_nt(L_INDEX).PROFIT_SS_YDTE,
        l_ss_details_nt(L_INDEX).PROFIT_SS_CMNT,
        l_ss_details_nt(L_INDEX).PROFIT_SS_ZEROCONT,
        l_ss_details_nt(L_INDEX).PROFIT_SS_FED_TAXES,
        l_ss_details_nt(L_INDEX).PROFIT_SS_STATE_TAXES,
        l_ss_details_nt(L_INDEX).PROFIT_SS_TAX_CODE,
        l_ss_details_nt(L_INDEX).PROFIT_SS_SSNO,
        l_ss_details_nt(L_INDEX).PROFIT_SS_NAME,
        l_ss_details_nt(L_INDEX).PROFIT_SS_ADDRESS,
        l_ss_details_nt(L_INDEX).PROFIT_SS_CITY,
        l_ss_details_nt(L_INDEX).PROFIT_SS_STATE,
        l_ss_details_nt(L_INDEX).PROFIT_SS_ZIP,
        l_ss_details_nt(L_INDEX).PROFIT_SS_DET_RECNO,
        l_ss_details_nt(L_INDEX).PROFIT_SS_DET_PR_SS_D_S_SEQNUM,
        l_ss_details_nt(L_INDEX).PR_SS_D_S_SEC_NUMBER
    );

    
    DBMS_OUTPUT.PUT_LINE('Number of profit ss details inserted: ' || l_ss_details_nt.COUNT);
    l_ss_details_written := l_ss_details_written +  l_ss_details_nt.COUNT;
    l_ss_details_nt.DELETE;

    -- Checks
    
    FORALL L_INDEX IN 1 .. l_checks_nt.COUNT 
        INSERT INTO PROFIT_SHARE_CHECKS (
                    CHECK_NUMBER, 
                    PAYABLE_NAME,
                    --EMPLOYEE_NUMBER,  (This field is too small in DB - should be 7 not 5 - and is unused
                    SSN_NUMBER,
                    EMPLOYEE_SSN,
                    CHECK_DATE)
        VALUES (
                l_checks_nt(L_INDEX).CHECK_NUMBER,
                l_checks_nt(L_INDEX).PAYABLE_NAME,
                --l_checks_nt(L_INDEX).EMPLOYEE_NUMBER,
                l_checks_nt(L_INDEX).SSN_NUMBER,
                l_checks_nt(L_INDEX).EMPLOYEE_SSN,
                l_checks_nt(L_INDEX).CHECK_DATE
                );
    
    DBMS_OUTPUT.PUT_LINE('Number of checks inserted: ' || l_checks_nt.COUNT );
    l_checks_written := l_checks_written +  l_checks_nt.COUNT;
    l_checks_nt.DELETE;

    -- Social Security Numbers
    
    FORALL L_INDEX IN 1 .. l_soc_sec_nt.COUNT 
        INSERT INTO SOC_SEC_REC (
                    SOC_SEC_NUMBER, 
                    SOC_SEC_RECNO)
        VALUES (
                l_soc_sec_nt(L_INDEX).SOC_SEC_NUMBER,
                l_soc_sec_nt(L_INDEX).SOC_SEC_RECNO
                );
    
    DBMS_OUTPUT.PUT_LINE('Number of SSNs for SOC SEC REC inserted: ' || l_soc_sec_nt.COUNT );
    l_soc_secs_written := l_soc_secs_written +  l_soc_sec_nt.COUNT;
    l_soc_sec_nt.DELETE;
    
    -- This is a commit after each batch of 10,000 employees,
    -- If we have more than 200,000, this becomes necessary
    IF (l_employees > 200000) THEN
        COMMIT;
    END IF;
    
    l_start := l_end + 1;
    dbms_output.put_line('---------------');
        
    END LOOP;
    COMMIT;
    
    DBMS_OUTPUT.PUT_LINE('***************************');
    DBMS_OUTPUT.PUT_LINE('Final Tallies');
    DBMS_OUTPUT.PUT_LINE('***************************');
    DBMS_OUTPUT.PUT_LINE('Total employees inserted: ' || l_employees_written );
    DBMS_OUTPUT.PUT_LINE('Total beneficiaries inserted: ' || l_beneficiaries_written );
    DBMS_OUTPUT.PUT_LINE('Total employees-as-bens inserted: ' || l_emp_as_ben_written );
    DBMS_OUTPUT.PUT_LINE('Total distributions inserted: ' || l_distributions_written );
    DBMS_OUTPUT.PUT_LINE('Total requests inserted: ' || l_requests_written );
    DBMS_OUTPUT.PUT_LINE('Total details inserted: ' || l_details_written );
    DBMS_OUTPUT.PUT_LINE('Total profit ss details inserted: ' || l_ss_details_written );
    DBMS_OUTPUT.PUT_LINE('Total checks inserted: ' || l_checks_written );
    DBMS_OUTPUT.PUT_LINE('Total payprofits inserted: ' || l_payprofits_written );
    DBMS_OUTPUT.PUT_LINE('Total relatives inserted: ' || l_payrels_written );
    DBMS_OUTPUT.PUT_LINE('Total SSNs inserted: ' || l_soc_secs_written );
    DBMS_OUTPUT.PUT_LINE('***************************');

EXCEPTION
    WHEN OTHERS
    THEN
        -- DBMS_OUTPUT.put_line ('Rows inserted : ' || SQL%ROWCOUNT);
        l_error_count := SQL%BULK_EXCEPTIONS.count;
        DBMS_OUTPUT.put_line('Number of failures: ' || l_error_count);
        FOR indx IN 1 .. SQL%BULK_EXCEPTIONS.COUNT
          LOOP
             DBMS_OUTPUT.put_line (
                   'Error '
                || indx
                || ' occurred on index '
                || SQL%BULK_EXCEPTIONS (indx).ERROR_INDEX
                || ' attempting to create bulk data. ');
             DBMS_OUTPUT.put_line (
                   'Oracle error is : '
                || SQLERRM(-SQL%BULK_EXCEPTIONS(indx).ERROR_CODE));
                   
          END LOOP;
        
          ROLLBACK; 
        RAISE;

END;
