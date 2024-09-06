-- This script will fill up PSN orphan assistance tables

-- The plan here is to run through the lookup tables that have
-- 11-digit PSNs and create new tables with those and the 
-- 7-digit employee badge bases so we can then to MINUS 
-- operations to figure out where the orphans are

-- We need a nested table for the PSNs, and another 
-- one to hold the two-field records for what needs
-- writing
DECLARE

    TYPE PSN_BASE_REC IS RECORD ( 
        PSN NUMBER(11),
        PSN_BASE NUMBER(7)
    );
    TYPE PSN_BASE_NT IS TABLE OF PSN_BASE_REC;
    L_PSN_BASE_NT PSN_BASE_NT;

    TYPE PSN_CACHE_NT IS TABLE OF NUMBER;
    L_PSN_CACHE_NT PSN_CACHE_NT;

    L_LAST_INDEX NUMBER;
    L_PSN_STRING VARCHAR(14);


BEGIN

    L_PSN_BASE_NT := PSN_BASE_NT();
    L_PSN_CACHE_NT := PSN_CACHE_NT();

    -- *****
    -- PAYBEN PSN
    -- *****
    -- Get the original unique PSNs
    SELECT DISTINCT PSN BULK COLLECT INTO L_PSN_CACHE_NT
        FROM PAYBEN_PSN;

    -- Now a quick loop through
    FOR L_INDEX IN 1..L_PSN_CACHE_NT.COUNT
    LOOP
        L_PSN_BASE_NT.EXTEND;
        L_LAST_INDEX := L_PSN_BASE_NT.LAST;
        L_PSN_BASE_NT(L_LAST_INDEX).PSN := L_PSN_CACHE_NT(L_INDEX);
        L_PSN_STRING := TO_CHAR(L_PSN_CACHE_NT(L_INDEX));
        -- Get the badge number base
        IF (LENGTH(L_PSN_STRING) > 4) THEN
            L_PSN_BASE_NT(L_LAST_INDEX).PSN_BASE := TO_NUMBER(SUBSTR(L_PSN_STRING, 1, LENGTH(L_PSN_STRING) - 4));
        ELSE
            -- Abort this row as PSN is invalid
            L_PSN_BASE_NT.DELETE(L_LAST_INDEX);
        END IF;
    END LOOP;

    -- Now do the FORALL

    FORALL L_INDEX IN 1 .. L_PSN_BASE_NT.COUNT SAVE EXCEPTIONS
    INSERT INTO PAYBEN_PSN_BASES (PSN, PSN_BASE) 
    VALUES (L_PSN_BASE_NT(L_INDEX).PSN, L_PSN_BASE_NT(L_INDEX).PSN_BASE);

    L_PSN_BASE_NT.DELETE;
    L_PSN_CACHE_NT.DELETE;

    -- *****
    -- PROFDIST PSN
    -- *****
    -- Get the original unique PSNs
    SELECT DISTINCT PSN BULK COLLECT INTO L_PSN_CACHE_NT
        FROM PROF_DIST_REQ_PSN;

    -- Now a quick loop through
    FOR L_INDEX IN 1..L_PSN_CACHE_NT.COUNT
    LOOP
        L_PSN_BASE_NT.EXTEND;
        L_LAST_INDEX := L_PSN_BASE_NT.LAST;
        L_PSN_BASE_NT(L_LAST_INDEX).PSN := L_PSN_CACHE_NT(L_INDEX);
        L_PSN_STRING := TO_CHAR(L_PSN_CACHE_NT(L_INDEX));
        -- Get the badge number base
        IF (LENGTH(L_PSN_STRING) > 4) THEN
            L_PSN_BASE_NT(L_LAST_INDEX).PSN_BASE := TO_NUMBER(SUBSTR(L_PSN_STRING, 1, LENGTH(L_PSN_STRING) - 4));
        ELSE
            -- Abort this row as PSN is invalid
            L_PSN_BASE_NT.DELETE(L_LAST_INDEX);
        END IF;
    END LOOP;

    -- Now do the FORALL

    FORALL L_INDEX IN 1 .. L_PSN_BASE_NT.COUNT SAVE EXCEPTIONS
    INSERT INTO PROF_DIST_PSN_BASES (PSN, PSN_BASE) 
    VALUES (L_PSN_BASE_NT(L_INDEX).PSN, L_PSN_BASE_NT(L_INDEX).PSN_BASE);

    L_PSN_BASE_NT.DELETE;
    L_PSN_CACHE_NT.DELETE;

    -- *****
    -- PYREL PSN
    -- Note: There can be employee PSNs in here!
    -- *****
    -- Get the original unique PSNs
    SELECT DISTINCT PSN BULK COLLECT INTO L_PSN_CACHE_NT
        FROM PYREL_PSN;

    -- Now a quick loop through
    FOR L_INDEX IN 1..L_PSN_CACHE_NT.COUNT
    LOOP
        L_PSN_BASE_NT.EXTEND;
        L_LAST_INDEX := L_PSN_BASE_NT.LAST;
        L_PSN_BASE_NT(L_LAST_INDEX).PSN := L_PSN_CACHE_NT(L_INDEX);
        L_PSN_STRING := TO_CHAR(L_PSN_CACHE_NT(L_INDEX));
        -- Get the badge number base
        IF (LENGTH(L_PSN_STRING) > 4) THEN
            L_PSN_BASE_NT(L_LAST_INDEX).PSN_BASE := TO_NUMBER(SUBSTR(L_PSN_STRING, 1, LENGTH(L_PSN_STRING) - 4));
        ELSE
            -- Abort this row as PSN is invalid
            L_PSN_BASE_NT.DELETE(L_LAST_INDEX);
        END IF;
    END LOOP;

    -- Now do the FORALL

    FORALL L_INDEX IN 1 .. L_PSN_BASE_NT.COUNT SAVE EXCEPTIONS
    INSERT INTO PYREL_PSN_BASES (PSN, PSN_BASE) 
    VALUES (L_PSN_BASE_NT(L_INDEX).PSN, L_PSN_BASE_NT(L_INDEX).PSN_BASE);

    L_PSN_BASE_NT.DELETE;
    L_PSN_CACHE_NT.DELETE;

    -- *****
    -- PROFIT_DETAIL PROFIT_CMNT
    -- *****
    -- Get the original unique PSNs
    SELECT DISTINCT PROFIT_PSN BULK COLLECT INTO L_PSN_CACHE_NT
        FROM PROFIT_CMNT_PSN;

    -- Now a quick loop through
    FOR L_INDEX IN 1..L_PSN_CACHE_NT.COUNT
    LOOP
        L_PSN_BASE_NT.EXTEND;
        L_LAST_INDEX := L_PSN_BASE_NT.LAST;
        L_PSN_BASE_NT(L_LAST_INDEX).PSN := L_PSN_CACHE_NT(L_INDEX);
        L_PSN_STRING := TO_CHAR(L_PSN_CACHE_NT(L_INDEX));
        -- Get the badge number base
        IF (LENGTH(L_PSN_STRING) > 4) THEN
            L_PSN_BASE_NT(L_LAST_INDEX).PSN_BASE := TO_NUMBER(SUBSTR(L_PSN_STRING, 1, LENGTH(L_PSN_STRING) - 4));
        ELSE
            -- Abort this row as PSN is invalid
            L_PSN_BASE_NT.DELETE(L_LAST_INDEX);
        END IF;
    END LOOP;

    -- Now do the FORALL

    FORALL L_INDEX IN 1 .. L_PSN_BASE_NT.COUNT SAVE EXCEPTIONS
    INSERT INTO PROFIT_DETAIL_PSN_BASES (PSN, PSN_BASE) 
    VALUES (L_PSN_BASE_NT(L_INDEX).PSN, L_PSN_BASE_NT(L_INDEX).PSN_BASE);

    L_PSN_BASE_NT.DELETE;
    L_PSN_CACHE_NT.DELETE;

    -- *****
    -- PROFIT_SS_DETAIL PROFIT_CMNT
    -- *****
    -- Get the original unique PSNs
    SELECT DISTINCT PROFIT_SS_PSN BULK COLLECT INTO L_PSN_CACHE_NT
        FROM PROFIT_SS_CMNT_PSN;

    -- Now a quick loop through
    FOR L_INDEX IN 1..L_PSN_CACHE_NT.COUNT
    LOOP
        L_PSN_BASE_NT.EXTEND;
        L_LAST_INDEX := L_PSN_BASE_NT.LAST;
        L_PSN_BASE_NT(L_LAST_INDEX).PSN := L_PSN_CACHE_NT(L_INDEX);
        L_PSN_STRING := TO_CHAR(L_PSN_CACHE_NT(L_INDEX));
        -- Get the badge number base
        IF (LENGTH(L_PSN_STRING) > 4) THEN
            L_PSN_BASE_NT(L_LAST_INDEX).PSN_BASE := TO_NUMBER(SUBSTR(L_PSN_STRING, 1, LENGTH(L_PSN_STRING) - 4));
        ELSE
            -- Abort this row as PSN is invalid
            L_PSN_BASE_NT.DELETE(L_LAST_INDEX);
        END IF;
    END LOOP;

    -- Now do the FORALL

    FORALL L_INDEX IN 1 .. L_PSN_BASE_NT.COUNT SAVE EXCEPTIONS
    INSERT INTO PROFIT_SS_DETAIL_PSN_BASES (PSN, PSN_BASE) 
    VALUES (L_PSN_BASE_NT(L_INDEX).PSN, L_PSN_BASE_NT(L_INDEX).PSN_BASE);

    L_PSN_BASE_NT.DELETE;
    L_PSN_CACHE_NT.DELETE;


END;


