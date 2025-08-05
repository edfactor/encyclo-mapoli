CREATE or replace VIEW transaction_totals AS
    SELECT
        pr_det_s_sec_number ssn,
        SUM(
            CASE
                WHEN profit_code IN(0) THEN
                    profit_cont + profit_earn + profit_fort
                ELSE
                    0
            END
        )                   code0,    --- IN: All the contributions coming into the plan.  profit_cont is from the plan, 
                                      ---      profit_earn is earnings from the balance, profit_fort is other peoples money coming in
        SUM(
            CASE
                WHEN profit_code IN(1, 2, 3, 5) THEN
                    profit_fort
                ELSE
                    0
            END
        )                   code1235,  --- OUT: various ways money goes out of the plan
        SUM(
            CASE
                WHEN profit_code = 6 THEN
                    profit_cont
                ELSE
                    0
            END
        )                   code6,    -- IN: Beneficiary Money (ie. from your uncle Vinnie)
        SUM(
            CASE
                WHEN profit_code = 8 THEN
                    profit_earn
                ELSE
                    0
            END
        )                   code8,    -- IN: Earnings which come directly to the member (ie dont pass through the vesting percent.) 
        SUM(
            CASE
                WHEN profit_code = 9 THEN
                    profit_fort
                ELSE
                    0
            END
        )                   code9      -- OUT: Money sent via check or directly to a retirement account
    FROM
        (
            SELECT
                profit_code,
                profit_cont,
                profit_earn,
                profit_fort,
                pr_det_s_sec_number
            FROM
                profit_detail
            UNION ALL
            SELECT
                profit_ss_code,
                profit_ss_cont,
                profit_ss_earn,
                profit_ss_fort,
                pr_ss_d_s_sec_number
            FROM
                profit_ss_detail
        )
    GROUP BY
        pr_det_s_sec_number
