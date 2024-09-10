/*
    This block does two things when run early in the 
    process:
    
    Get the store numbers of all stores with fewer than 
    50 employees and put those numbers into the TINY_STORES table.
    
    Get the store numbers of the three stores with the most 
    employees and put those numbers into the HUGE_STORES table.

    Then, later it the process, we can notice when an employee 
    is in a tiny store with few employees - which can make it 
    possible to identify them - and then we can put them in 
    one of the three largest stores instead.
*/

DECLARE
    -- Nested table for general table updates
    TYPE STORE_COUNT_REC IS RECORD ( 
        STORE_NUMBER NUMBER(3),
        EMP_COUNT NUMBER(5)
    );
    
    TYPE STORE_COUNT_NT IS TABLE OF STORE_COUNT_REC;
    L_TINY_STORES_NT STORE_COUNT_NT;
    L_HUGE_STORES_NT STORE_COUNT_NT;
    L_ALL_STORES_NT STORE_COUNT_NT;
    L_STORE_COUNTER NUMBER;
    L_SMALL_STORE_SIZE NUMBER;

    L_BIG_STORE_MAX NUMBER;

BEGIN
    
    L_TINY_STORES_NT := STORE_COUNT_NT();
    L_HUGE_STORES_NT := STORE_COUNT_NT();
    L_ALL_STORES_NT := STORE_COUNT_NT();
    L_STORE_COUNTER := 1;
    L_SMALL_STORE_SIZE := 50;
    SELECT DEMO.PY_STOR, COUNT(*) AS employee_total 
    BULK COLLECT INTO L_ALL_STORES_NT FROM DEMOGRAPHICS DEMO GROUP BY DEMO.PY_STOR ORDER BY employee_total;
    
    dbms_output.put_line('Total number of stores in dataset: ' || L_ALL_STORES_NT.COUNT);
    IF (L_ALL_STORES_NT.COUNT < 4) THEN
        dbms_output.put_line('You have only a few stores. Perhaps do not use store number outlier removal.');
    END IF;

    FOR L_INDEX IN 1..L_ALL_STORES_NT.COUNT
    LOOP
        IF (L_ALL_STORES_NT(L_INDEX).EMP_COUNT < L_SMALL_STORE_SIZE) THEN
            L_TINY_STORES_NT.EXTEND;
            L_TINY_STORES_NT(L_INDEX).STORE_NUMBER := L_ALL_STORES_NT(L_INDEX).STORE_NUMBER;
            L_TINY_STORES_NT(L_INDEX).EMP_COUNT := L_ALL_STORES_NT(L_INDEX).EMP_COUNT;
        END IF;
    END LOOP;

    /* If we have more than 3 stores, we only want top three big stores
       but if we have fewer, then we have to use total number of stores instead */
    IF (L_ALL_STORES_NT.COUNT > 3) THEN
        L_BIG_STORE_MAX := 3;
    ELSE 
        L_BIG_STORE_MAX := L_ALL_STORES_NT.COUNT;
    END IF;
    
    FOR COUNTER IN 1..L_BIG_STORE_MAX 
    LOOP
        L_HUGE_STORES_NT.EXTEND;
        L_HUGE_STORES_NT(COUNTER).STORE_NUMBER := L_ALL_STORES_NT(L_ALL_STORES_NT.COUNT - COUNTER).STORE_NUMBER;
        L_HUGE_STORES_NT(COUNTER).EMP_COUNT := L_ALL_STORES_NT(L_ALL_STORES_NT.COUNT - COUNTER).EMP_COUNT;
    END LOOP;
    
    /* For debugging....
    FOR L_INDEX IN 1..L_HUGE_STORES_NT.COUNT
    LOOP
        DBMS_OUTPUT.PUT_LINE('Huge Store # ' || L_HUGE_STORES_NT(L_INDEX).STORE_NUMBER);
        DBMS_OUTPUT.PUT_LINE('Employee Count: ' || L_HUGE_STORES_NT(L_INDEX).EMP_COUNT);
    END LOOP;
    
    
    FOR L_INDEX IN 1..L_TINY_STORES_NT.COUNT
    LOOP
        DBMS_OUTPUT.PUT_LINE('Tiny Store # ' || L_TINY_STORES_NT(L_INDEX).STORE_NUMBER);
        DBMS_OUTPUT.PUT_LINE('Employee Count: ' || L_TINY_STORES_NT(L_INDEX).EMP_COUNT);
    END LOOP;
    */
    
    -- Now we need to insert these into the two tables
    
    FORALL L_INDEX IN 1 .. L_HUGE_STORES_NT.COUNT SAVE EXCEPTIONS
            INSERT INTO HUGE_STORES (STORE_NUMBER) 
            VALUES (L_HUGE_STORES_NT(L_INDEX).STORE_NUMBER);

    FORALL L_INDEX IN 1 .. L_TINY_STORES_NT.COUNT SAVE EXCEPTIONS
            INSERT INTO TINY_STORES (STORE_NUMBER) 
            VALUES ( L_TINY_STORES_NT(L_INDEX).STORE_NUMBER);       
            
    COMMIT;

END;
