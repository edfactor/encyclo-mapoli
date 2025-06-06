import { useEffect, useMemo } from "react";
import { useLazyGetProfitMasterInquiryGroupingQuery } from "reduxstore/api/InquiryApi";
import { GetMasterInquiryGridColumns } from "./MasterInquiryGridColumns";
import { Typography } from "@mui/material";
import { RootState } from "reduxstore/store";
import { useSelector } from "react-redux";
import { DSMGrid } from "smart-ui-library";
import { MasterInquiryRequest } from "reduxstore/types";

const MasterInquiryGroupingGrid = ({ searchParams }: { searchParams: MasterInquiryRequest }) => {
    const [getProfitMasterInquiryGrouping, { isLoading: isGroupingLoading }] = useLazyGetProfitMasterInquiryGroupingQuery();

    useEffect(() => {
        getProfitMasterInquiryGrouping({
            ...searchParams,
            pagination: {
                skip: 0,
                take: 10,
                sortBy: "profitYear",
                isSortDescending: true
            }
        });
    }, [getProfitMasterInquiryGrouping]);

    const columnDefs = useMemo(() => GetMasterInquiryGridColumns(), []);

    const { masterInquiryGroupingData } = useSelector((state: RootState) => state.inquiry);
    const preferenceKey = "MasterInquiryGrouping";

    return (
        <div>
            {isGroupingLoading && <div>Loading...</div>}
            {masterInquiryGroupingData && <div>{masterInquiryGroupingData.length}</div>}

                {!!masterInquiryGroupingData && (
                    <>
                        <div style={{ padding: "0 24px 0 24px" }}>
                            <Typography variant="h2" sx={{ color: "#0258A5" }}>
                                {`Profit Details (${masterInquiryGroupingData.length || 0} ${masterInquiryGroupingData.length === 1 ? "Record" : "Records"})`}
                            </Typography>
                        </div>
                        <DSMGrid
                            preferenceKey={preferenceKey}
                            isLoading={isGroupingLoading}
                            providedOptions={{
                                rowData: masterInquiryGroupingData,
                                columnDefs: columnDefs,
                                suppressMultiSort: true
                            }}
                        />
                    </>
                )}
            
        </div>
    )
}

export default MasterInquiryGroupingGrid;