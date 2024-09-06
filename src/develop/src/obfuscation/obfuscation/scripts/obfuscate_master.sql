/*
    Obfuscation Framework

    This is the main driver script that runs the framework. It assumes that if you wanted to cut down the data set with the target_* scripts, this has already happened.

    This is also the script you run after creating test data with either test_create_data.sql or 
    test_bulk_data_create.sql

    NOTE: It is possible to run these scripts individually, but some need to be run before 
    others. The order cannot be changed much.
*/

-- Optional tasks
--@@test_insert_orphan_records.sql
--@@test_view_data
--@@delete_audit_data

-- STEP 1: These three scripts just set things up. No data is changed
@@obfuscate_start_run
@@fill_store_tables
@@reset_sequences

-- STEP 2: These four scripts to whole-table changes that are not about
-- tracking SSN or PSNs from one table to another

-- Employee data must be done before beneficiary data
@@obfuscate_employees
@@obfuscate_beneficiaries
@@obfuscate_other_fields
@@obfuscate_emp_distribution_state

-- STEP 3: These three scripts are about arranging data so that
-- the filling of "task tables" (per-SSN, per-PSN jobs stored in 
-- SSN_TASKS and PSN_TASKS) can be done in a high-performance way 
@@fill_profit_detail_psns_table
@@fill_profit_ss_detail_psns_table
@@fill_ssn_psn_lookup_tables

-- This is an optional PSN orphan check 
@@fill_psn_orphan_tables
@@check_psn_orphans

-- STEP 4: These three scripts create all the rows in SSN_TASKS
-- and PSN_TASKS. SSN tasks for employees and beneficiaries
-- are in separate scripts as these operations take the longest 
@@fill_ssn_task_tables_emp
@@fill_ssn_task_tables_ben
@@fill_psn_task_tables

-- STEP 5: These four scripts actually change SSN and PSN 
-- values across all tables

@@obfuscate_employee_psns
@@obfuscate_beneficiary_psns
-- Employee SSNs must be obfuscated before beneficiary SSNs
@@obfuscate_employee_ssns
@@obfuscate_beneficiary_ssns

-- STEP 6: These two scripts just clean up any errors that 
-- were not obfuscated and print a report tracing SSNs

@@delete_orphan_records
--@obfuscate_validation
--@@test_view_data
