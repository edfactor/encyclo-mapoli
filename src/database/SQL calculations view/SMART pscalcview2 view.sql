CREATE OR REPLACE VIEW pscalcview2 AS
SELECT
    ssn,
    vested_percent,
    code0 + code6 + code8 - (code9 + code1235) AS total_balance,
    -- if the total_balance is zero, then vested_balance should also be zero
    CASE
        WHEN code0 + code6 + code8 - ( code9 + code1235 ) = 0 THEN
            0
        ELSE
            code0 * vested_percent + code6 + code8 - ( code9 + code1235 )
        END                           AS vested_balance
FROM
    (
        SELECT
            c.ssn,
            vested_percent,
            nvl(code0, 0)    AS code0,  -- the NVL funcitons (treat null as 0) are needed to give the column a specific type, otherwise a view cannot be created
            nvl(code6, 0)    AS code6,
            nvl(code8, 0)    AS code8,
            nvl(code9, 0)    AS code9,
            nvl(code1235, 0) AS code1235
        FROM
            transaction_totals c
                LEFT OUTER JOIN vested_percent     v ON c.ssn = v.ssn
    )