

This directory contains the golden master outputs from READY generated on May 27, 2025.    It used the obfuscated data
in the PROFSHARE schema to create a new READY schema.     The PROFSHARE schema is already moved up to the Frozen Step
(aka the SHIFT has happened), so we start from Activity 15 (first step in frozen.)

This wiki page shows which outputs correspond to which kshs.
https://demoulas.atlassian.net/wiki/spaces/NGDS/pages/367296514/YE+READY+schedule+with+jobs+and+files+and+reports+created+and+database+fields+updated

The YERunner was used to execute activities A15-A28, using this specifier;

    await Run(Specify(
            "R0",
            "R15",
            "R16", // second chance to edit exec hours/dollars
            "R17",
            "R18",
            "R19",
            "R20",
            "R21",
            "R22",
            "R24",
            "R25",
            "R26",
            "R27",
            "R28"
        ));

All the output files from /dsmdev/data/PAYROLL/SYS/PVTSYSOUT/ were then gathered into this directory and checked in.

The "LBL" file was manually copied 
    $ scp bherman2@tduapp01:/dsmdev/data/UNDER21/LBL .


##  Matching Data without YERunner

To match the data on SMART, depending on where you are in the YE, you need to run the steps below to move the database
to the correct state.

1) Import the PROFSHARE schema
2) Run A12 (aka create a 1/4/2024 freeze)
3) If you go past A18, run YearEndService.RunFinalYearEndUpdates() to update EarnedPoints, Cert Date, New Employee, ZeroContr
4) If you go past A23, run ProfitMasterUpdate.Update(contribution=15, Earning=5, Forfeit=5, SecondaryEarnings=0, Max=76500)


## Using the YERunner to push SMART to the right state;

Run the YERunnder with the corresponding specifiers
1) Specify("S0")
2) Specify("S0","S12")
3) Specify("S0","S12","S18")
4) Specify("S0","S12","S18","S23")
