import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { Page, DSMAccordion } from "smart-ui-library";
import { useState, useEffect } from "react";
import { useSelector } from "react-redux";
import { CAPTIONS } from "../../constants";
import Pay450SearchFilters from "./Pay450SearchFilters";
import Pay450Grid from "./Pay450Grid";
import LabelValueSection from "components/LabelValueSection";
import { RootState } from "reduxstore/store";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";

interface ProfitYearSearch {
    profitYear: number;
}

// Sample data for demo purposes (to be removed when endpoint is fixed)
const sampleData = {
    totalNumberOfEmployees: 0,
    totalNumberOfBeneficiaries: 1
};

const Pay450Summary = () => {
    const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
    const fiscalCloseProfitYear = useFiscalCloseProfitYear();
    const { updateSummary } = useSelector((state: RootState) => state.yearsEnd);
    const [demoMode, setDemoMode] = useState(true);

    // Set initialSearchLoaded to true on mount to trigger the API call
    useEffect(() => {
        setInitialSearchLoaded(true);
    }, []);

    // Dynamic data for the summary section
    const updateSummarySection = [
        { 
            label: "Employees Updated", 
            // Use demo data if in demo mode, otherwise use real data
            value: demoMode ? sampleData.totalNumberOfEmployees.toString() 
                : (updateSummary ? updateSummary.totalNumberOfEmployees.toString() : "-")
        },
        { 
            label: "Beneficiaries Updated", 
            // Use demo data if in demo mode, otherwise use real data
            value: demoMode ? sampleData.totalNumberOfBeneficiaries.toString()
                : (updateSummary ? updateSummary.totalNumberOfBeneficiaries.toString() : "-")
        }
    ];

    const onSearch = (data: ProfitYearSearch) => {
        setInitialSearchLoaded(true);
    };

    return (
        <Page label={CAPTIONS.PAY450_SUMMARY}>
            <Grid2
                container
                rowSpacing="24px">
                <Grid2 width={"100%"}>
                    <Divider />
                </Grid2>
                <Grid2
                    width={"100%"}>
                    <DSMAccordion title="Filter">
                        <Pay450SearchFilters onSearch={onSearch} />
                    </DSMAccordion>
                </Grid2>

                <Grid2 size={{ xs: 2 }} paddingX="24px" >
                    <LabelValueSection
                        data={updateSummarySection}
                    />
                </Grid2>
                <Grid2 width="100%">
                    <Pay450Grid 
                        initialSearchLoaded={initialSearchLoaded}
                        setInitialSearchLoaded={setInitialSearchLoaded}
                        profitYear={fiscalCloseProfitYear}
                    />
                </Grid2>
            </Grid2>
        </Page>
    );
};

export default Pay450Summary;