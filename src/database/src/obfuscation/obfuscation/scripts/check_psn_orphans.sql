-- This file will check for PSN orphans

-- So we just need a nested table of a 
-- custom record type with two values:
-- a base PSN and a longer PSN

DECLARE
    TYPE PSN_BASE_REC IS RECORD ( 
        PSN NUMBER(11),
        PSN_BASE NUMBER(7)
    );
    TYPE PSN_BASE_NT IS TABLE OF PSN_BASE_REC;
    TYPE PSN_COLLECT_NT IS TABLE OF NUMBER(7);

    TYPE PSN_STORAGE_NT IS TABLE OF NUMBER(11);
    L_PSN_STORAGE_NT PSN_STORAGE_NT;

    L_PSN_BASE_NT PSN_BASE_NT;
    L_PSN_COLLECT_NT PSN_COLLECT_NT;



    L_LAST_INDEX NUMBER;

-- ****************
-- PROFIT_DIST_REQ - PSN field
-- ****************

-- So this query will find out what PSNs in 
-- PROFIT_DIST_REQ are not tied to DEMOGRAPHICS
-- We need to take the list of base values
-- in the PAYBEN_PSN_BASEs, use the MINUS
-- operation from DEMOGRAPHICS.DEM_BADGE
-- and see what is not there.

BEGIN
    L_PSN_BASE_NT := PSN_BASE_NT();
    L_PSN_COLLECT_NT := PSN_COLLECT_NT();
    L_PSN_STORAGE_NT := PSN_STORAGE_NT();

SELECT PSN_BASE
    BULK COLLECT INTO L_PSN_COLLECT_NT
    FROM 
    (
    SELECT t1.PSN_BASE FROM PROF_DIST_PSN_BASES t1
    MINUS
    SELECT t2.PSN FROM DEMOGRAPHICS_PSN t2
    );

DBMS_OUTPUT.put_line('***************************************');
DBMS_OUTPUT.put_line('***** PSN Orphan Detection Report ******');
DBMS_OUTPUT.put_line('***************************************');
DBMS_OUTPUT.put_line('Note: rows will not be removed until delete orphans script is run.');
DBMS_OUTPUT.put_line(' ');
DBMS_OUTPUT.put_line('PROF_DIST_REQ PSN orphans:');
DBMS_OUTPUT.put_line('There were ' || L_PSN_COLLECT_NT.COUNT || ' PROF_DIST_REQ PSN orphans');

-- Now we need to go get all the extended values

FOR L_INDEX IN 1..L_PSN_COLLECT_NT.COUNT 
LOOP
    L_PSN_BASE_NT.EXTEND;
    L_LAST_INDEX := L_PSN_BASE_NT.LAST;
    L_PSN_BASE_NT(L_LAST_INDEX).PSN_BASE := L_PSN_COLLECT_NT(L_INDEX);
    SELECT DISTINCT PSN INTO L_PSN_BASE_NT(L_LAST_INDEX).PSN FROM PROF_DIST_PSN_BASES WHERE PSN_BASE = L_PSN_COLLECT_NT(L_INDEX);
    DBMS_OUTPUT.put_line('Missing DEMOGRAPHICS badge: ' || L_PSN_BASE_NT(L_LAST_INDEX).PSN_BASE || ' for PSN: ' || L_PSN_BASE_NT(L_LAST_INDEX).PSN);
END LOOP;


L_PSN_BASE_NT.DELETE;
L_PSN_COLLECT_NT.DELETE;

-- ****************
-- PROFIT_DIST_REQ - Employee Badge field
-- ****************

-- So this query will find out what PSNs in 
-- PROFIT_DIST_REQ are not tied to DEMOGRAPHICS
-- We need to take the list of base values
-- in the PROF_DIST_REQ_EMP, use the MINUS
-- operation from DEMOGRAPHICS.DEM_BADGE
-- and see what is not there.

SELECT PSN
    BULK COLLECT INTO L_PSN_COLLECT_NT
    FROM 
    (
    SELECT t1.PSN FROM PROF_DIST_REQ_EMP t1
    MINUS
    SELECT t2.PSN FROM DEMOGRAPHICS_PSN t2
    );

DBMS_OUTPUT.put_line('***************************************');
DBMS_OUTPUT.put_line('PROF_DIST_REQ Employee Badge orphans:');
DBMS_OUTPUT.put_line('There were ' || L_PSN_COLLECT_NT.COUNT || ' PROF_DIST_REQ Employee Badge orphans');

-- Now we need to go get all the extended values

FOR L_INDEX IN 1..L_PSN_COLLECT_NT.COUNT 
LOOP
    L_PSN_BASE_NT.EXTEND;
    L_LAST_INDEX := L_PSN_BASE_NT.LAST;
    L_PSN_BASE_NT(L_LAST_INDEX).PSN_BASE := L_PSN_COLLECT_NT(L_INDEX);
    SELECT DISTINCT PSN INTO L_PSN_BASE_NT(L_LAST_INDEX).PSN FROM PROF_DIST_REQ_EMP WHERE PSN = L_PSN_COLLECT_NT(L_INDEX);
    DBMS_OUTPUT.put_line('Missing DEMOGRAPHICS badge: ' || L_PSN_BASE_NT(L_LAST_INDEX).PSN_BASE || ' for : ' || L_PSN_BASE_NT(L_LAST_INDEX).PSN);
END LOOP;

L_PSN_BASE_NT.DELETE;
L_PSN_COLLECT_NT.DELETE;

-- ****************
-- PAYBEN
-- ****************

-- So this query will find out what PSNs in 
-- PROFIT_DIST_REQ are not tied to DEMOGRAPHICS
-- We need to take the list of base values
-- in the PAYBEN_PSN_BASEs, use the MINUS
-- operation from DEMOGRAPHICS.DEM_BADGE
-- and see what is not there.

SELECT PSN_BASE
    BULK COLLECT INTO L_PSN_COLLECT_NT
    FROM 
    (
    SELECT t1.PSN_BASE FROM PAYBEN_PSN_BASES t1
    MINUS
    SELECT t2.PSN FROM DEMOGRAPHICS_PSN t2
    );

DBMS_OUTPUT.put_line('***************************************');
DBMS_OUTPUT.put_line('PAYBEN PSN orphans:');
DBMS_OUTPUT.put_line('There were ' || L_PSN_COLLECT_NT.COUNT || ' PAYBEN PSN orphans');

-- Now we need to go get all the extended values

FOR L_INDEX IN 1..L_PSN_COLLECT_NT.COUNT 
LOOP

    -- Now there could be more than one psn per missing badge, so we need a loop
    SELECT DISTINCT PSN
    BULK COLLECT INTO L_PSN_STORAGE_NT FROM PAYBEN_PSN_BASES 
    WHERE PSN_BASE = L_PSN_COLLECT_NT(L_INDEX);

    FOR O_INDEX IN 1..L_PSN_STORAGE_NT.COUNT
    LOOP
        DBMS_OUTPUT.put_line('Missing DEMOGRAPHICS badge: ' || 
        L_PSN_COLLECT_NT(L_INDEX) || 
        ' for PAYBEN PSN: ' || L_PSN_STORAGE_NT(O_INDEX));
    END LOOP;

END LOOP;

L_PSN_STORAGE_NT.DELETE;
L_PSN_BASE_NT.DELETE;
L_PSN_COLLECT_NT.DELETE;


-- ****************
-- PAYREL
-- ****************

-- All PAYREL PSNs must be in PAYBEN. 

SELECT PSN
    BULK COLLECT INTO L_PSN_COLLECT_NT
    FROM 
    (
    SELECT t1.PSN FROM PYREL_PSN t1
    MINUS
    SELECT t2.PSN FROM PAYBEN_PSN t2
    );

DBMS_OUTPUT.put_line('***************************************');
DBMS_OUTPUT.put_line('PAYREL PSN orphans:');
DBMS_OUTPUT.put_line('There were ' || L_PSN_COLLECT_NT.COUNT || ' PAYREL PSN orphans');

-- Now we need to go get all the extended values

FOR L_INDEX IN 1..L_PSN_COLLECT_NT.COUNT 
LOOP
    DBMS_OUTPUT.put_line('Missing PAYBEN PSN for PAYREL PSN: ' || 
    L_PSN_COLLECT_NT(L_INDEX));
END LOOP;

L_PSN_STORAGE_NT.DELETE;
L_PSN_BASE_NT.DELETE;
L_PSN_COLLECT_NT.DELETE;

-- ****************
-- PAYPROFIT 
-- ****************

-- So this query will find out what PSNs in 
-- PAYPROFIT are not tied to DEMOGRAPHICS
-- We need to take the list of base values
-- in the PAYREL_PSN_BASEs, use the MINUS
-- operation from DEMOGRAPHICS.DEM_BADGE
-- and see what is not there.

SELECT PSN
    BULK COLLECT INTO L_PSN_COLLECT_NT
    FROM 
    (
    SELECT t1.PSN FROM PAYPROFIT_PSN t1
    MINUS
    SELECT t2.PSN FROM DEMOGRAPHICS_PSN t2
    );

DBMS_OUTPUT.put_line('***************************************');
DBMS_OUTPUT.put_line('PAYPROFIT PSN orphans:');
DBMS_OUTPUT.put_line('There were ' || L_PSN_COLLECT_NT.COUNT || ' PAYPROFIT PSN orphans');

-- Now we need to go get all the extended values

FOR L_INDEX IN 1..L_PSN_COLLECT_NT.COUNT 
LOOP
    -- Now there could be more than one psn per missing badge, so we need a loop
    SELECT DISTINCT PSN
    BULK COLLECT INTO L_PSN_STORAGE_NT FROM PAYPROFIT_PSN 
    WHERE PSN = L_PSN_COLLECT_NT(L_INDEX);

    FOR O_INDEX IN 1..L_PSN_STORAGE_NT.COUNT
    LOOP
        DBMS_OUTPUT.put_line('Missing DEMOGRAPHICS badge: ' || 
        L_PSN_COLLECT_NT(L_INDEX) || 
        ' for PAYPROFIT PSN: ' || L_PSN_STORAGE_NT(O_INDEX));
    END LOOP;


END LOOP;

L_PSN_STORAGE_NT.DELETE;
L_PSN_BASE_NT.DELETE;
L_PSN_COLLECT_NT.DELETE;

-- ****************
-- PROFIT DETAIL (PSNs inside PROFIT_CMNT)
-- ****************

-- So this query will find out what PSNs in 
-- PROFIT DETAIL  are not tied to DEMOGRAPHICS
-- We need to take the list of base values
-- in the , use the MINUS
-- operation from DEMOGRAPHICS.DEM_BADGE
-- and see what is not there.

SELECT PSN_BASE
    BULK COLLECT INTO L_PSN_COLLECT_NT
    FROM 
    (
    SELECT t1.PSN_BASE FROM PROFIT_DETAIL_PSN_BASES t1
    MINUS
    SELECT t2.PSN FROM DEMOGRAPHICS_PSN t2
    );

DBMS_OUTPUT.put_line('***************************************');
DBMS_OUTPUT.put_line('PROFIT DETAIL Comment PSN orphans:');
DBMS_OUTPUT.put_line('There were ' || L_PSN_COLLECT_NT.COUNT || ' PROFIT DETAIL Comment PSN orphans');

-- Now we need to go get all the extended values

FOR L_INDEX IN 1..L_PSN_COLLECT_NT.COUNT 
LOOP
    -- Now there could be more than one psn per missing badge, so we need a loop
    SELECT DISTINCT PSN
    BULK COLLECT INTO L_PSN_STORAGE_NT FROM PROFIT_DETAIL_PSN_BASES 
    WHERE PSN_BASE = L_PSN_COLLECT_NT(L_INDEX);

    FOR O_INDEX IN 1..L_PSN_STORAGE_NT.COUNT
    LOOP
        DBMS_OUTPUT.put_line('Missing DEMOGRAPHICS badge: ' || 
        L_PSN_COLLECT_NT(L_INDEX) || 
        ' for Comment PSN: ' || L_PSN_STORAGE_NT(O_INDEX));
    END LOOP;
END LOOP;

L_PSN_STORAGE_NT.DELETE;
L_PSN_BASE_NT.DELETE;
L_PSN_COLLECT_NT.DELETE;

-- ****************
-- PROFIT SS DETAIL (PSNs inside PROFIT_CMNT)
-- ****************

-- So this query will find out what PSNs in 
-- PROFIT SS DETAIL  are not tied to DEMOGRAPHICS
-- We need to take the list of base values
-- in the , use the MINUS
-- operation from DEMOGRAPHICS.DEM_BADGE
-- and see what is not there.

SELECT PSN_BASE
    BULK COLLECT INTO L_PSN_COLLECT_NT
    FROM 
    (
    SELECT t1.PSN_BASE FROM PROFIT_SS_DETAIL_PSN_BASES t1
    MINUS
    SELECT t2.PSN FROM DEMOGRAPHICS_PSN t2
    );

DBMS_OUTPUT.put_line('***************************************');
DBMS_OUTPUT.put_line('PROFIT SS DETAIL Comment PSN orphans:');
DBMS_OUTPUT.put_line('There were ' || L_PSN_COLLECT_NT.COUNT || ' PROFIT SS DETAIL Comment PSN orphans');

-- Now we need to go get all the extended values

FOR L_INDEX IN 1..L_PSN_COLLECT_NT.COUNT 
LOOP
   -- Now there could be more than one PSN for each missing badge, so we need a loop
    SELECT DISTINCT PSN
    BULK COLLECT INTO L_PSN_STORAGE_NT FROM PROFIT_SS_DETAIL_PSN_BASES 
    WHERE PSN_BASE = L_PSN_COLLECT_NT(L_INDEX);

    FOR O_INDEX IN 1..L_PSN_STORAGE_NT.COUNT
    LOOP
        DBMS_OUTPUT.put_line('Missing DEMOGRAPHICS badge: ' || 
        L_PSN_COLLECT_NT(L_INDEX) || 
        ' for Comment PSN: ' || L_PSN_STORAGE_NT(O_INDEX));
    END LOOP;
END LOOP;

L_PSN_STORAGE_NT.DELETE;
L_PSN_BASE_NT.DELETE;
L_PSN_COLLECT_NT.DELETE;




END;