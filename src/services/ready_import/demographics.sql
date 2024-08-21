
truncate TABLE DEMOGRAPHIC;
    INSERT INTO demographic (
        oracle_hcm_id,
        ssn,
        badge_number,
        full_name,
        last_name,
        first_name,
        middle_name,
        store_number,
        pay_classification_id,
        phone_number,
        street,
        street2,
        city,
        state,
        postal_code,
        date_of_birth,
        full_time_date,
        hire_date,
        rehire_date,
        termination_date,
        department,
        employment_type_id,
        gender_id,
        pay_frequency_id,
        termination_code_id,
        employment_status_id
    )
        SELECT
            ROWNUM AS oraclehcmid,
            dem_ssn,
            dem_badge,
            py_nam,
            py_lname,
            py_fname,
            py_mname,
            py_stor,
            nvl((
                SELECT
                    id
                FROM
                    pay_classification
                WHERE
                    id = TO_NUMBER(py_cla)
            ),
                1),  --- PR-232 will 
            py_emp_telno,
            py_add,
            py_add2,
            py_city,
            py_state,
            py_zip,
            CASE
                WHEN length(py_dob) = 8
                     AND TO_NUMBER(substr(py_dob, 5, 2)) BETWEEN 1 AND 12
                     AND ( ( TO_NUMBER(substr(py_dob, 5, 2)) IN ( 1, 3, 5, 7, 8,
                                                                  10, 12 )
                             AND TO_NUMBER(substr(py_dob, 7, 2)) BETWEEN 1 AND 31 )
                           OR ( TO_NUMBER(substr(py_dob, 5, 2)) IN ( 4, 6, 9, 11 )
                                AND TO_NUMBER(substr(py_dob, 7, 2)) BETWEEN 1 AND 30 )
                           OR ( TO_NUMBER(substr(py_dob, 5, 2)) = 2
                                AND TO_NUMBER(substr(py_dob, 7, 2)) BETWEEN 1 AND CASE
                                                                                      WHEN mod(TO_NUMBER(substr(py_dob, 1, 4)),
                                                                                               400) = 0
                                                                                           OR ( mod(TO_NUMBER(substr(py_dob, 1, 4)),
                                                                                                        4) = 0
                                                                                                AND mod(TO_NUMBER(substr(py_dob, 1, 4
                                                                                                )),
                                                                                                        100) != 0 ) THEN
                                                                                          29
                                                                                      ELSE
                                                                                          28
                                                                                  END ) ) THEN
                    TO_DATE(py_dob, 'YYYYMMDD')
                ELSE
                    DATE '1900-01-01'
            END    AS converted_date,
            CASE
                WHEN py_full_dt = 0 THEN
                    NULL
                WHEN length(py_full_dt) = 8
                     AND TO_NUMBER(substr(py_full_dt, 5, 2)) BETWEEN 1 AND 12
                     AND ( ( TO_NUMBER(substr(py_full_dt, 5, 2)) IN ( 1, 3, 5, 7, 8,
                                                                      10, 12 )
                             AND TO_NUMBER(substr(py_full_dt, 7, 2)) BETWEEN 1 AND 31 )
                           OR ( TO_NUMBER(substr(py_full_dt, 5, 2)) IN ( 4, 6, 9, 11 )
                                AND TO_NUMBER(substr(py_full_dt, 7, 2)) BETWEEN 1 AND 30 )
                           OR ( TO_NUMBER(substr(py_full_dt, 5, 2)) = 2
                                AND TO_NUMBER(substr(py_full_dt, 7, 2)) BETWEEN 1 AND CASE
                                                                                          WHEN mod(TO_NUMBER(substr(py_full_dt, 1, 4)
                                                                                          ),
                                                                                                   400) = 0
                                                                                               OR ( mod(TO_NUMBER(substr(py_full_dt, 1
                                                                                               , 4)),
                                                                                                            4) = 0
                                                                                                    AND mod(TO_NUMBER(substr(py_full_dt
                                                                                                    , 1, 4)),
                                                                                                            100) != 0 ) THEN
                                                                                              29
                                                                                          ELSE
                                                                                              28
                                                                                      END ) ) THEN
                    TO_DATE(py_full_dt, 'YYYYMMDD')
                ELSE
                    DATE '1900-01-01'
            END    AS converted_date,
            CASE
                WHEN length(py_hire_dt) = 8
                     AND TO_NUMBER(substr(py_hire_dt, 5, 2)) BETWEEN 1 AND 12
                     AND ( ( TO_NUMBER(substr(py_hire_dt, 5, 2)) IN ( 1, 3, 5, 7, 8,
                                                                      10, 12 )
                             AND TO_NUMBER(substr(py_hire_dt, 7, 2)) BETWEEN 1 AND 31 )
                           OR ( TO_NUMBER(substr(py_hire_dt, 5, 2)) IN ( 4, 6, 9, 11 )
                                AND TO_NUMBER(substr(py_hire_dt, 7, 2)) BETWEEN 1 AND 30 )
                           OR ( TO_NUMBER(substr(py_hire_dt, 5, 2)) = 2
                                AND TO_NUMBER(substr(py_hire_dt, 7, 2)) BETWEEN 1 AND CASE
                                                                                          WHEN mod(TO_NUMBER(substr(py_hire_dt, 1, 4)
                                                                                          ),
                                                                                                   400) = 0
                                                                                               OR ( mod(TO_NUMBER(substr(py_hire_dt, 1
                                                                                               , 4)),
                                                                                                            4) = 0
                                                                                                    AND mod(TO_NUMBER(substr(py_hire_dt
                                                                                                    , 1, 4)),
                                                                                                            100) != 0 ) THEN
                                                                                              29
                                                                                          ELSE
                                                                                              28
                                                                                      END ) ) THEN
                    TO_DATE(py_hire_dt, 'YYYYMMDD')
                ELSE
                    DATE '1900-01-01'
            END    AS converted_date,
            CASE
                WHEN py_rehire_dt = 0 THEN
                    NULL
                WHEN length(py_rehire_dt) = 8
                     AND TO_NUMBER(substr(py_rehire_dt, 5, 2)) BETWEEN 1 AND 12
                     AND ( ( TO_NUMBER(substr(py_rehire_dt, 5, 2)) IN ( 1, 3, 5, 7, 8,
                                                                        10, 12 )
                             AND TO_NUMBER(substr(py_rehire_dt, 7, 2)) BETWEEN 1 AND 31 )
                           OR ( TO_NUMBER(substr(py_rehire_dt, 5, 2)) IN ( 4, 6, 9, 11 )
                                AND TO_NUMBER(substr(py_rehire_dt, 7, 2)) BETWEEN 1 AND 30 )
                           OR ( TO_NUMBER(substr(py_rehire_dt, 5, 2)) = 2
                                AND TO_NUMBER(substr(py_rehire_dt, 7, 2)) BETWEEN 1 AND CASE
                                                                                            WHEN mod(TO_NUMBER(substr(py_rehire_dt, 1
                                                                                            , 4)),
                                                                                                     400) = 0
                                                                                                 OR ( mod(TO_NUMBER(substr(py_rehire_dt
                                                                                                 , 1, 4)),
                                                                                                              4) = 0
                                                                                                      AND mod(TO_NUMBER(substr(py_rehire_dt
                                                                                                      , 1, 4)),
                                                                                                              100) != 0 ) THEN
                                                                                                29
                                                                                            ELSE
                                                                                                28
                                                                                        END ) ) THEN
                    TO_DATE(py_rehire_dt, 'YYYYMMDD')
                ELSE
                    DATE '1900-01-01'
            END    AS converted_date,
            CASE
                WHEN py_term_dt = 0 THEN
                    NULL
                WHEN length(py_term_dt) = 8
                     AND TO_NUMBER(substr(py_term_dt, 5, 2)) BETWEEN 1 AND 12
                     AND ( ( TO_NUMBER(substr(py_term_dt, 5, 2)) IN ( 1, 3, 5, 7, 8,
                                                                      10, 12 )
                             AND TO_NUMBER(substr(py_term_dt, 7, 2)) BETWEEN 1 AND 31 )
                           OR ( TO_NUMBER(substr(py_term_dt, 5, 2)) IN ( 4, 6, 9, 11 )
                                AND TO_NUMBER(substr(py_term_dt, 7, 2)) BETWEEN 1 AND 30 )
                           OR ( TO_NUMBER(substr(py_term_dt, 5, 2)) = 2
                                AND TO_NUMBER(substr(py_term_dt, 7, 2)) BETWEEN 1 AND CASE
                                                                                          WHEN mod(TO_NUMBER(substr(py_term_dt, 1, 4)
                                                                                          ),
                                                                                                   400) = 0
                                                                                               OR ( mod(TO_NUMBER(substr(py_term_dt, 1
                                                                                               , 4)),
                                                                                                            4) = 0
                                                                                                    AND mod(TO_NUMBER(substr(py_term_dt
                                                                                                    , 1, 4)),
                                                                                                            100) != 0 ) THEN
                                                                                              29
                                                                                          ELSE
                                                                                              28
                                                                                      END ) ) THEN
                    TO_DATE(py_term_dt, 'YYYYMMDD')
                ELSE
                    DATE '1900-01-01'
            END    AS converted_date,
            py_dp,
            TRIM(py_ful),
            py_gender,
            py_freq,
            CASE
                WHEN py_term = ' ' THEN
                    NULL
                ELSE
                    py_term
            END,
            (
                SELECT
                    id
                FROM
                    employment_status
                WHERE
                    id = lower(py_scod)
            )
        FROM
            profitshare.demographics 
