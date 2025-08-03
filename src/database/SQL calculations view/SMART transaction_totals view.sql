CREATE OR REPLACE VIEW transaction_totals AS
SELECT
    ssn,
    SUM(
            CASE
                WHEN profit_code_id IN(0) THEN
                    contribution + earnings + forfeiture
                ELSE
                    0
                END
    ) code0,  --- IN: All the contributions coming into the plan.  "contribution" is from the plan,
              --- "earnings" from the plan gaining value, "forfeiture" is other members leaving money behind and that money being distributed to this member
    SUM(
            CASE
                WHEN profit_code_id IN(1, 2, 3, 5) THEN
                    forfeiture
                ELSE
                    0
                END
    ) code1235,  --- OUT: various ways money goes out of the plan (Hardship, money give to a bene, direct payment)
    SUM(
            CASE
                WHEN profit_code_id = 6 THEN
                    contribution
                ELSE
                    0
                END
    ) code6,    -- IN: Beneficiary Money (ie. from your uncle Vinnie)
    SUM(
            CASE
                WHEN profit_code_id = 8 THEN
                    earnings
                ELSE
                    0
                END
    ) code8,    -- IN: Non ETVA money coming in.   100% Earnings which come directly to the member (ie dont pass through the vesting percent.) 
    SUM(
            CASE
                WHEN profit_code_id = 9 THEN
                    forfeiture
                ELSE
                    0
                END
    ) code9      -- OUT: Money sent via check or directly to a retirement account
FROM
    profit_detail
GROUP BY
    ssn