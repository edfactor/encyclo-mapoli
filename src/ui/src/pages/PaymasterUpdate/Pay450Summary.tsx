import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { Page, DSMAccordion, numberToCurrency } from "smart-ui-library";
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

const Pay450Summary = () => {
    const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
    const fiscalCloseProfitYear = useFiscalCloseProfitYear();
    const { updateSummary } = useSelector((state: RootState) => state.yearsEnd);

    useEffect(() => {
        setInitialSearchLoaded(true);
    }, []);

    const updateSummarySection = [
        { 
            label: "Employees Updated", 
            value: updateSummary ? updateSummary.totalNumberOfEmployees.toString() : "-"
        },
        { 
            label: "Beneficiaries Updated", 
            value: updateSummary ? updateSummary.totalNumberOfBeneficiaries.toString() : "-"
        },
        { 
            label: "Before Profit Sharing Amount", 
            value: updateSummary ? numberToCurrency(updateSummary.totalBeforeProfitSharingAmount) : "-"
        },
        { 
            label: "Before Vested Amount", 
            value: updateSummary ? numberToCurrency(updateSummary.totalBeforeVestedAmount) : "-"
        },
        { 
            label: "After Profit Sharing Amount", 
            value: updateSummary ? numberToCurrency(updateSummary.totalAfterProfitSharingAmount) : "-"
        },
        { 
            label: "After Vested Amount", 
            value: updateSummary ? numberToCurrency(updateSummary.totalAfterVestedAmount) : "-"
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

                <Grid2 size={{ xs: 12 }} paddingX="24px" >
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