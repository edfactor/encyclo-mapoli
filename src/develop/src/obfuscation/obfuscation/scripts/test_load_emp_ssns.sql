/*
    This is a utility file to generate target employee SSNs when
    testing the target features using bulk data from the bulk data 
    loader.

    The example here is the first 1,000 employees of a large set.
    It goes by two in order to allow testing of employee-as-beneficiary
    situations.
*/
DECLARE
    L_RESULT NUMBER;

BEGIN        

    FOR L_INDEX IN 1 .. 2000
    LOOP
        L_RESULT := MOD(L_INDEX, 2); 
        IF (L_RESULT = 0) THEN
            INSERT INTO TARGET_EMP_SSN (SSN) VALUES (-L_INDEX);
        END IF;
    END LOOP;    
END;