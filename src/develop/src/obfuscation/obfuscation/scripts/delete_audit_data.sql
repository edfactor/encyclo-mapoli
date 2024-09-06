/*
    As we do not need the audit tables for application development 
    and testing, and because audit rows contain identifying information, 
    we are going to remove all data from them.
*/

DELETE FROM PAYBEN_AUD;

DELETE FROM PROFDIST_AUD;

DELETE FROM PROFIT_DIST_REQ_AUD;

DELETE FROM PROFIT_DETAIL_AUD;

DELETE FROM PROFIT_SHARE_CHECKS_AUD;

COMMIT;