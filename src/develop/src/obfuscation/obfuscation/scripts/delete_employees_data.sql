/* 
    This script deletes the test data that was created, 
    not the tables that were used in the obfuscation process,
    which can be deleted using delete_framework_data.sql
*/

DELETE FROM DEMOGRAPHICS;
DELETE FROM PAYBEN;
DELETE FROM PAYPROFIT;
DELETE FROM PAYREL;
DELETE FROM PROFDIST;
DELETE FROM PROFIT_DIST_REQ;
DELETE FROM PROFIT_DETAIL;
DELETE FROM PROFIT_SHARE_CHECKS;
DELETE FROM PROFIT_SS_DETAIL;
DELETE FROM SOC_SEC_REC;

COMMIT;
