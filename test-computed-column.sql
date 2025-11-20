-- Test 1: Simple concatenation with plain literals (should fail with ORA-12704)
-- This is what's in the code now
CREATE TABLE test_fullname_bad AS
SELECT 
    LAST_NAME || ', ' || FIRST_NAME AS full_name_bad
FROM DEMOGRAPHIC
WHERE ROWNUM = 1;

-- Test 2: Concatenation with quote operator (should work)
-- This is what we need
SELECT 
    LAST_NAME || q'[, ]' || FIRST_NAME AS full_name_good
FROM DEMOGRAPHIC
WHERE ROWNUM = 1;

-- Test 3: Computed column with quote operator and NULL (correct approach)
-- Drop if exists
BEGIN
   EXECUTE IMMEDIATE 'DROP TABLE test_computed_col';
EXCEPTION
   WHEN OTHERS THEN NULL;
END;
/

CREATE TABLE test_computed_col (
    id          NUMBER,
    last_name   VARCHAR2(30),
    first_name  VARCHAR2(30),
    middle_name VARCHAR2(25),
    full_name   VARCHAR2(128) GENERATED ALWAYS AS (
        LAST_NAME || q'[, ]' || FIRST_NAME || 
        CASE WHEN MIDDLE_NAME IS NOT NULL 
            THEN q'[ ]' || SUBSTR(MIDDLE_NAME, 1, 1) 
            ELSE NULL 
        END
    ) STORED
);

-- Test 4: Insert test data
INSERT INTO test_computed_col (id, last_name, first_name, middle_name) 
VALUES (1, 'Smith', 'John', 'Michael');

INSERT INTO test_computed_col (id, last_name, first_name, middle_name) 
VALUES (2, 'Johnson', 'Mary', NULL);

COMMIT;

-- Test 5: Query the computed column (this should work)
SELECT id, last_name, first_name, middle_name, full_name 
FROM test_computed_col;

-- Test 6: The actual problem query - selecting FULL_NAME from DEMOGRAPHIC
-- Run this to see if it fails:
SELECT DISTINCT d.FULL_NAME
FROM DEMOGRAPHIC d
WHERE ROWNUM <= 10;

-- Cleanup
BEGIN
   EXECUTE IMMEDIATE 'DROP TABLE test_computed_col';
   EXECUTE IMMEDIATE 'DROP TABLE test_fullname_bad';
EXCEPTION
   WHEN OTHERS THEN NULL;
END;
/
