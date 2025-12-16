-- ============================================================================
-- OracleHCM Sync Integration Testing - DEMOGRAPHIC_HISTORY Validation Script
-- ============================================================================
-- Oracle 19 Version
-- Purpose: Show before/after changes to validate DEMOGRAPHIC_HISTORY captures
--          both manual modifications AND sync-driven restores
--
-- Workflow:
--   1. First sync runs → downloads employees to DEMOGRAPHIC + DEMOGRAPHIC_HISTORY
--   2. Run PART 1 (below) → Show BEFORE, modify data, show AFTER
--   3. Manually run OracleHCM sync → restores original data from Oracle
--   4. Run PART 2 (at end) → Show HISTORY + validate sync worked
--
-- ============================================================================

-- Enable output for messages
BEGIN
  DBMS_OUTPUT.ENABLE(NULL);
END;
/

-- ============================================================================
-- PART 1: SHOW BEFORE → MODIFY → SHOW AFTER
-- ============================================================================

-- STEP 1: DISPLAY ORIGINAL DATA (BEFORE MODIFICATION)
BEGIN
  DBMS_OUTPUT.PUT_LINE('');
  DBMS_OUTPUT.PUT_LINE('============================================================================');
  DBMS_OUTPUT.PUT_LINE('STEP 1: ORIGINAL DEMOGRAPHIC DATA (before modification)');
  DBMS_OUTPUT.PUT_LINE('============================================================================');
  DBMS_OUTPUT.PUT_LINE('');
END;
/

SELECT 
    ORACLE_HCM_ID,
    FIRST_NAME,
    LAST_NAME,
    PHONE_NUMBER,
    MOBILE_NUMBER,
    EMAIL_ADDRESS,
    STREET
FROM DEMOGRAPHIC
WHERE ORACLE_HCM_ID > 100000000
ORDER BY ORACLE_HCM_ID
FETCH FIRST 20 ROWS ONLY;

-- STEP 2: MODIFY THE DATA
BEGIN
  DBMS_OUTPUT.PUT_LINE('');
  DBMS_OUTPUT.PUT_LINE('============================================================================');
  DBMS_OUTPUT.PUT_LINE('STEP 2: MODIFYING DATA (updating 6 columns)');
  DBMS_OUTPUT.PUT_LINE('============================================================================');
  DBMS_OUTPUT.PUT_LINE('');
END;
/

DECLARE
  v_rows_modified PLS_INTEGER;
BEGIN
  UPDATE DEMOGRAPHIC
  SET 
      FIRST_NAME = 'MOD_' || SUBSTR(FIRST_NAME, 1, 25),
      LAST_NAME = 'TST_' || SUBSTR(LAST_NAME, 1, 25),
      PHONE_NUMBER = '555-0000',
      MOBILE_NUMBER = '555-0001',
      EMAIL_ADDRESS = 'modified@test.com',
      STREET = '999 TEST STREET'
  WHERE ORACLE_HCM_ID > 100000000;

  v_rows_modified := SQL%ROWCOUNT;
  COMMIT;

  DBMS_OUTPUT.PUT_LINE('✓ Rows modified: ' || v_rows_modified);
  DBMS_OUTPUT.PUT_LINE('');
  DBMS_OUTPUT.PUT_LINE('Columns changed:');
  DBMS_OUTPUT.PUT_LINE('  • FIRST_NAME → prepended with MOD_ (truncated to fit 30-char limit)');
  DBMS_OUTPUT.PUT_LINE('  • LAST_NAME → prepended with TST_ (truncated to fit 30-char limit)');
  DBMS_OUTPUT.PUT_LINE('  • PHONE_NUMBER → 555-0000');
  DBMS_OUTPUT.PUT_LINE('  • MOBILE_NUMBER → 555-0001');
  DBMS_OUTPUT.PUT_LINE('  • EMAIL_ADDRESS → modified@test.com');
  DBMS_OUTPUT.PUT_LINE('  • STREET → 999 TEST STREET');
  DBMS_OUTPUT.PUT_LINE('');

EXCEPTION
  WHEN OTHERS THEN
    DBMS_OUTPUT.PUT_LINE('ERROR: ' || SQLERRM);
    ROLLBACK;
END;
/

-- STEP 3: DISPLAY MODIFIED DATA (AFTER MODIFICATION)
BEGIN
  DBMS_OUTPUT.PUT_LINE('============================================================================');
  DBMS_OUTPUT.PUT_LINE('STEP 3: MODIFIED DEMOGRAPHIC DATA (after modification, before sync)');
  DBMS_OUTPUT.PUT_LINE('============================================================================');
  DBMS_OUTPUT.PUT_LINE('');
END;
/

SELECT 
    ORACLE_HCM_ID,
    FIRST_NAME,
    LAST_NAME,
    PHONE_NUMBER,
    MOBILE_NUMBER,
    EMAIL_ADDRESS,
    STREET
FROM DEMOGRAPHIC
WHERE ORACLE_HCM_ID > 100000000
ORDER BY ORACLE_HCM_ID
FETCH FIRST 20 ROWS ONLY;

-- STEP 4: PAUSE FOR MANUAL SYNC
BEGIN
  DBMS_OUTPUT.PUT_LINE('');
  DBMS_OUTPUT.PUT_LINE('============================================================================');
  DBMS_OUTPUT.PUT_LINE('STEP 4: MANUAL SYNC REQUIRED');
  DBMS_OUTPUT.PUT_LINE('============================================================================');
  DBMS_OUTPUT.PUT_LINE('');
  DBMS_OUTPUT.PUT_LINE('*** STOP HERE ***');
  DBMS_OUTPUT.PUT_LINE('');
  DBMS_OUTPUT.PUT_LINE('Now run the OracleHCM sync process manually');
  DBMS_OUTPUT.PUT_LINE('(this will download original data and update DEMOGRAPHIC_HISTORY)');
  DBMS_OUTPUT.PUT_LINE('');
  DBMS_OUTPUT.PUT_LINE('After sync completes, run PART 2 below to validate history');
  DBMS_OUTPUT.PUT_LINE('============================================================================');
  DBMS_OUTPUT.PUT_LINE('');
END;
/

-- ============================================================================
-- PART 2: VALIDATE SYNC RESULTS (run AFTER manual sync)
-- ============================================================================

BEGIN
  DBMS_OUTPUT.PUT_LINE('');
  DBMS_OUTPUT.PUT_LINE('============================================================================');
  DBMS_OUTPUT.PUT_LINE('PART 2: VALIDATE SYNC (run AFTER manual sync completes)');
  DBMS_OUTPUT.PUT_LINE('============================================================================');
  DBMS_OUTPUT.PUT_LINE('');
END;
/

-- VALIDATION 1: SHOW CURRENT DEMOGRAPHIC (should be restored to original)
BEGIN
  DBMS_OUTPUT.PUT_LINE('VALIDATION 1: Current DEMOGRAPHIC (should be restored to original values)');
  DBMS_OUTPUT.PUT_LINE('');
END;
/

SELECT 
    ORACLE_HCM_ID,
    FIRST_NAME,
    LAST_NAME,
    PHONE_NUMBER,
    MOBILE_NUMBER,
    EMAIL_ADDRESS,
    STREET
FROM DEMOGRAPHIC
WHERE ORACLE_HCM_ID > 100000000
ORDER BY ORACLE_HCM_ID
FETCH FIRST 20 ROWS ONLY;

-- VALIDATION 2: SHOW DEMOGRAPHIC_HISTORY (should have 2 versions per employee)
BEGIN
  DBMS_OUTPUT.PUT_LINE('');
  DBMS_OUTPUT.PUT_LINE('VALIDATION 2: DEMOGRAPHIC_HISTORY (should show manual change + sync restore)');
  DBMS_OUTPUT.PUT_LINE('');
END;
/

SELECT 
    ORACLE_HCM_ID,
    FIRST_NAME,
    LAST_NAME,
    PHONE_NUMBER,
    MOBILE_NUMBER,
    EMAIL_ADDRESS,
    STREET,
    VALID_FROM,
    VALID_TO
FROM DEMOGRAPHIC_HISTORY
WHERE ORACLE_HCM_ID > 100000000
ORDER BY ORACLE_HCM_ID, VALID_FROM DESC
FETCH FIRST 40 ROWS ONLY;

-- VALIDATION 3: SIDE-BY-SIDE COMPARISON
BEGIN
  DBMS_OUTPUT.PUT_LINE('');
  DBMS_OUTPUT.PUT_LINE('VALIDATION 3: Current vs Latest History (should match)');
  DBMS_OUTPUT.PUT_LINE('');
END;
/

SELECT 
    d.ORACLE_HCM_ID,
    'CURRENT' as source,
    d.FIRST_NAME,
    d.LAST_NAME,
    d.PHONE_NUMBER,
    d.MOBILE_NUMBER,
    d.EMAIL_ADDRESS,
    d.STREET,
    NULL as VALID_FROM,
    NULL as VALID_TO
FROM DEMOGRAPHIC d
WHERE d.ORACLE_HCM_ID > 100000000

UNION ALL

SELECT 
    dh.ORACLE_HCM_ID,
    'HISTORY' as source,
    dh.FIRST_NAME,
    dh.LAST_NAME,
    dh.PHONE_NUMBER,
    dh.MOBILE_NUMBER,
    dh.EMAIL_ADDRESS,
    dh.STREET,
    dh.VALID_FROM,
    dh.VALID_TO
FROM DEMOGRAPHIC_HISTORY dh
WHERE dh.ORACLE_HCM_ID > 100000000
  AND dh.VALID_TO = (SELECT MAX(VALID_TO) FROM DEMOGRAPHIC_HISTORY dh2 WHERE dh2.ORACLE_HCM_ID = dh.ORACLE_HCM_ID)

ORDER BY ORACLE_HCM_ID, source DESC
FETCH FIRST 40 ROWS ONLY;
