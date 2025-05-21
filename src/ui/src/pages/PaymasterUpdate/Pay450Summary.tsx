import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { Page, DSMAccordion, numberToCurrency, SmartModal } from "smart-ui-library";
import { useState, useEffect } from "react";
import { useSelector } from "react-redux";
import { CAPTIONS } from "../../constants";
import Pay450SearchFilters from "./Pay450SearchFilters";
import Pay450Grid from "./Pay450Grid";
import LabelValueSection from "components/LabelValueSection";
import { RootState } from "reduxstore/store";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useLazyGetUpdateSummaryQuery, useUpdateEnrollmentMutation } from "reduxstore/api/YearsEndApi";
import { Button } from "@mui/material";

interface ProfitYearSearch {
    profitYear: number;
}

const Pay450Summary = () => {
    const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const fiscalCloseProfitYear = useFiscalCloseProfitYear();
    const { updateSummary } = useSelector((state: RootState) => state.yearsEnd);

    const [getUpdateSummary] = useLazyGetUpdateSummaryQuery();
    const [updateEnrollment] = useUpdateEnrollmentMutation();

    useEffect(() => {
        setInitialSearchLoaded(true);
    }, []);

    const handleUpdate = async () => {
        await updateEnrollment({});
        setIsModalOpen(false);
    };

    const handleCancel = () => {
        setIsModalOpen(false);
    };

    const renderActionNode = () => {
        if (!updateSummary) return null;
        
        return (
            <Button
                onClick={() => setIsModalOpen(true)}
                variant="outlined"
                className="h-10 whitespace-nowrap min-w-fit">
                Update
            </Button>
        );
    };

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
        console.log("Huh");
        getUpdateSummary({
            profitYear: data.profitYear, pagination: {
                skip: 0, take: 255,
                sortBy: "",
                isSortDescending: false
            }
        });
    };

    return (
        <Page label={CAPTIONS.PAY450_SUMMARY} actionNode={renderActionNode()}>
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

            <SmartModal
                open={isModalOpen}
                onClose={handleCancel}
                actions={[
                    <Button
                        onClick={handleUpdate}
                        variant="contained"
                        color="primary"
                        className="mr-2">
                        Yes, Update
                    </Button>,
                    <Button
                        onClick={handleCancel}
                        variant="outlined">
                        No, Cancel
                    </Button>
                ]}
                title="Update Enrollment"
            >
                This update will bring new employees into the Profit Sharing System as enrolled members
            </SmartModal>
        </Page>
    );
};

export default Pay450Summary;