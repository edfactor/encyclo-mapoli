/*
  This is the master script to work from a list of targeted employee
  SSNs to deleting all unrelated records.

  First, it uses the SSNs in TARGET_EMP_SSNs and traverses the tables
  to come up with keys to populate five other lookup tables.

  Second, it uses those lookup tables to delete unwanted records across the profit sharing data tables.

*/


-- In case any values have been added to these tables 
-- before the target run: TARGET_EMP_PSN, TARGET_BEN_PSN,
-- TARGET_BEN_SSN - then we need to add them to the two
-- 'all' tables for operations there
@@target_seed_all

-- These two scripts build the six lookup tables:
-- TARGET_EMP_SSN, TARGET_EMP_PSN, TARGET_BEN_PSN,
-- TARGET_BEN_SSN, TARGET_ALL_PSN, TARGET_ALL_SSN
@@target_build_emp_lookups
@@target_build_ben_lookups

-- This script deletes PROFIT_SHARE_CHECKS
@@target_delete_checks

-- This scripts deletes PROFIT_DETAIL
@@target_delete_details

-- This script does deletes across all other tables
@@target_delete_others


