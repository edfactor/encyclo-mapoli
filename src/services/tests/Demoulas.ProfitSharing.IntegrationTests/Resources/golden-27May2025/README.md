

This directory contains the golden master outputs from READY generated on May 27, 2025.    It used the obfuscated data
in the PROFSHARE schema to create a new READY schema.     The PROFSHARE schema is already moved up to the Frozen Step
(aka the SHIFT has happended), so we start from Activity 15 (first step in frozen.)

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

