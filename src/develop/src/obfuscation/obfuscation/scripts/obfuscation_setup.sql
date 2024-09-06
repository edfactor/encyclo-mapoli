/*
    This is the master script that runs all of the one-time setup
    tasks for the obfuscation process. It creates the tables
    and loads all of the functions.
*/

-- New tables
@@create_task_tables
@@create_log_table
@@create_key_lookup_tables
@@create_employee_obf_table

-- Load all function libraries
@@functions_random
@@functions_fill_task_tables
@@functions_core

-- Indexes added to tables for performance
@@create_indexes

COMMIT;