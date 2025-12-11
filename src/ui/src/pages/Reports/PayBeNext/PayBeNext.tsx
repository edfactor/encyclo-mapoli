import { yupResolver } from "@hookform/resolvers/yup";
import { Checkbox, Divider, FormLabel, Grid, MenuItem, Select, Typography } from "@mui/material";
import { CellClickedEvent, ColDef, ICellRendererParams } from "ag-grid-community";
import { useEffect, useMemo, useState } from "react";
import { Controller, Resolver, useForm } from "react-hook-form";
import { DSMAccordion, DSMGrid, ISortParams, Page, Pagination, SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import { CAPTIONS } from "../../../constants";
import { useContentAwareGridHeight } from "../../../hooks/useContentAwareGridHeight";
import useFiscalCloseProfitYear from "../../../hooks/useFiscalCloseProfitYear";
import { useLazyAdhocBeneficiariesReportQuery } from "../../../reduxstore/api/AdhocApi";
import {
  AdhocBeneficiariesReportRequest,
  adhocBeneficiariesReportResponse,
  BeneficiaryReportDto
} from "../../../reduxstore/types";
import { PayBeNextGridColumns, ProfitDetailGridColumns } from "./PayBeNextGridColumns";

const schema = yup.object().shape({
  profitYear: yup.string().notRequired(),
  isAlsoEmployee: yup.boolean().notRequired()
});
interface bRequest {
  profitYear: string;
  isAlsoEmployee: boolean;
}

const PayBeNext = () => {
  const [expandedRows, setExpandedRows] = useState<Record<string, boolean>>({});
  const profitYear = useFiscalCloseProfitYear();
  const [triggerReport, { isFetching, isSuccess }] = useLazyAdhocBeneficiariesReportQuery();
  const [adhocBeneficiariesReport, setAdhocBeneficiariesReport] = useState<adhocBeneficiariesReportResponse>();
  const [pageNumber, setPageNumber] = useState<number>(0);
  const [pageSize, setPageSize] = useState<number>(25);
  const [_sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "psnSuffix",
    isSortDescending: true
  });
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (!isFetching) {
      setIsSubmitting(false);
    }
  }, [isFetching]);

  const mainColumns = useMemo(() => PayBeNextGridColumns(), []);
  const detailColumns = useMemo(() => ProfitDetailGridColumns(), []);

  const {
    control,

    formState: { isValid },

    handleSubmit,
    reset
  } = useForm<bRequest>({
    resolver: yupResolver(schema) as Resolver<bRequest>,
    defaultValues: { isAlsoEmployee: true, profitYear: "2024" }
  });

  // Create the grid data with expandable rows
  const gridData = useMemo(() => {
    if (!adhocBeneficiariesReport?.response?.results) return [];

    const rows = [];

    for (const row of adhocBeneficiariesReport.response.results) {
      const hasDetails = row.profitDetails && row.profitDetails.length > 0;

      // Add main row
      rows.push({
        ...row,
        isExpandable: hasDetails,
        isExpanded: hasDetails && Boolean(expandedRows[row.badgeNumber.toString() + row.beneficiaryId.toString()]),
        isDetail: false
      });

      // Add detail rows if expanded
      if (hasDetails && expandedRows[row.badgeNumber.toString() + row.beneficiaryId.toString()]) {
        for (const detail of row.profitDetails || []) {
          rows.push({
            ...row,
            ...detail,
            isDetail: true,
            isExpandable: false,
            isExpanded: false,
            parentId: parseInt(row.badgeNumber + "" + row.beneficiaryId),
            suggestedForfeit: 0
          });
        }
      }
    }

    return rows;
  }, [adhocBeneficiariesReport, expandedRows]);

  // Use content-aware grid height utility hook
  const gridMaxHeight = useContentAwareGridHeight({
    rowCount: gridData?.length ?? 0
  });

  // Handle row expansion toggle
  const handleRowExpansion = (badgeNumber: string) => {
    setExpandedRows((prev) => ({
      ...prev,
      [badgeNumber]: !prev[badgeNumber]
    }));
  };

  // Create column definitions with expand/collapse functionality
  const columnDefs = useMemo((): ColDef[] => {
    // Add an expansion column
    const expansionColumn: ColDef = {
      headerName: "",
      field: "isExpandable",
      width: 50,
      cellRenderer: (params: ICellRendererParams) => {
        if (params.data && !params.data.isDetail && params.data.isExpandable) {
          return params.data.isExpanded ? "▼" : "►";
        }
        return "";
      },
      onCellClicked: (event: CellClickedEvent) => {
        if (event.data && !event.data.isDetail && event.data.isExpandable) {
          handleRowExpansion(event.data.badgeNumber.toString() + event.data.beneficiaryId.toString());
        }
      },
      suppressSizeToFit: true,
      suppressAutoSize: true,
      lockVisible: true,
      lockPosition: true,
      pinned: "left" as const
    };

    return [expansionColumn, ...mainColumns, ...detailColumns];
  }, [mainColumns, detailColumns]);

  const createAdhocBeneficiariesReportReqeust = (
    skip: number,
    sortBy: string,
    isSortDescending: boolean,
    take: number,
    isAlsoEmployee: boolean,
    _profityear: number
  ): AdhocBeneficiariesReportRequest => {
    const request: AdhocBeneficiariesReportRequest = {
      isAlsoEmployee: isAlsoEmployee,
      profitYear: _profityear,
      isSortDescending: isSortDescending,
      skip: skip,
      sortBy: sortBy,
      take: take
    };
    return request;
  };

  const sortEventHandler = (update: ISortParams) => {
    if (update.sortBy === "") {
      update.sortBy = "psnSuffix";
      update.isSortDescending = true;
    }
    setSortParams(update);
    setPageNumber(0);

    const request = createAdhocBeneficiariesReportReqeust(0, "", false, 50, true, 2024);
    triggerReport(request)
      .unwrap()
      .then((res) => {
        setAdhocBeneficiariesReport(res);
      })
      .catch((err) => console.log(err));
  };

  const onSubmit = (data: bRequest) => {
    if (!isSubmitting) {
      setIsSubmitting(true);
      const request = createAdhocBeneficiariesReportReqeust(
        0,
        "",
        false,
        50,
        data.isAlsoEmployee ?? true,
        parseInt(data.profitYear)
      );
      triggerReport(request)
        .unwrap()
        .then((res) => {
          console.log(res);
          setAdhocBeneficiariesReport(res);
        })
        .catch((err) => console.log(err));
    }
  };

  const validateAndSubmit = handleSubmit(onSubmit);

  const handleReset = () => {
    reset({ isAlsoEmployee: true, profitYear: "2024" });
  };

  return (
    <Page label="PAY BE NEXT">
      <Grid
        container
        rowSpacing="24px">
        <Grid
          size={{ xs: 12 }}
          width={"100%"}>
          <Divider />
        </Grid>
        <Grid
          size={{ xs: 12 }}
          width={"100%"}>
          <DSMAccordion title="Filter">
            <form onSubmit={validateAndSubmit}>
              <Grid
                container
                paddingX="24px">
                <Grid
                  container
                  spacing={2}
                  width="100%">
                  <Grid size={{ xs: 12, sm: 2, md: 2 }}>
                    <FormLabel>Profit Year</FormLabel>
                    <Controller
                      name="profitYear"
                      control={control}
                      render={({ field }) => (
                        <Select
                          {...field}
                          defaultValue="2024"
                          fullWidth
                          size="small"
                          variant="outlined"
                          labelId="profitYear"
                          id="profitYear"
                          value={field.value}
                          label="Profit Year"
                          onChange={(e) => field.onChange(e.target.value)}>
                          <MenuItem value="2024">2024</MenuItem>
                          <MenuItem value="2023">2023</MenuItem>
                          <MenuItem value="2022">2022</MenuItem>
                          <MenuItem value="2021">2021</MenuItem>
                          <MenuItem value="2020">2020</MenuItem>
                        </Select>
                      )}
                    />
                  </Grid>
                  <Grid size={{ xs: 12, sm: 3, md: 3 }}>
                    <FormLabel>&nbsp;</FormLabel>
                    <div
                      className="flex items-center"
                      style={{ marginLeft: "5px" }}>
                      <FormLabel style={{ marginRight: "8px" }}>Is Also Employee</FormLabel>
                      <Controller
                        name="isAlsoEmployee"
                        control={control}
                        render={({ field }) => (
                          <Checkbox
                            {...field}
                            size="small"
                            checked={!!field.value}
                            onChange={(e) => {
                              field.onChange(e.target.checked);
                            }}
                          />
                        )}
                      />
                    </div>
                  </Grid>
                </Grid>

                <Grid
                  container
                  justifyContent="flex-end"
                  paddingY="16px">
                  <Grid size={{ xs: 12 }}>
                    <SearchAndReset
                      handleReset={handleReset}
                      handleSearch={validateAndSubmit}
                      isFetching={isFetching || isSubmitting}
                      disabled={!isValid || isFetching || isSubmitting}
                    />
                  </Grid>
                </Grid>
              </Grid>
            </form>
          </DSMAccordion>
        </Grid>

        <Grid
          size={{ xs: 12 }}
          width="100%">
          {isSuccess && (
            <div className="relative">
              <div>
                <Typography
                  variant="h2"
                  sx={{ color: "#0258A5" }}>
                  {`Employee Beneficiaries Control Sheet Starting at Year ${profitYear}`}
                </Typography>
                <strong>Ending Balance</strong> ${adhocBeneficiariesReport?.totalEndingBalance}
                <DSMGrid
                  preferenceKey={CAPTIONS.BENEFICIARY_INQUIRY}
                  isLoading={isFetching}
                  maxHeight={gridMaxHeight}
                  handleSortChanged={sortEventHandler}
                  providedOptions={{
                    rowData: gridData,
                    columnDefs: columnDefs,
                    suppressMultiSort: true,
                    masterDetail: true,
                    detailCellRenderer: (params: BeneficiaryReportDto) => {
                      const pDetails = params.profitDetails || [];
                      return (
                        <div style={{ padding: "10px" }}>
                          {pDetails.length > 0 ? (
                            <table style={{ width: "100%", marginTop: "8px" }}>
                              <thead>
                                <tr>
                                  <th>Year</th>
                                  <th>Code</th>
                                  <th>Contributions</th>
                                  <th>Earnings</th>
                                  <th>Forfeitures</th>
                                  <th>Date</th>
                                  <th>Comments</th>
                                </tr>
                              </thead>
                              <tbody>
                                {pDetails.map((data, index) => (
                                  <tr key={index}>
                                    <td>{data.year}</td>
                                    <td>{data.code}</td>
                                    <td>{data.contributions}</td>
                                    <td>{data.earnings}</td>
                                    <td>{data.forfeitures}</td>
                                    <td>{data.date.toDateString()}</td>
                                    <td>{data.comments}</td>
                                  </tr>
                                ))}
                              </tbody>
                            </table>
                          ) : (
                            <div>No data found.</div>
                          )}
                        </div>
                      );
                    }
                  }}
                />
                {adhocBeneficiariesReport &&
                  adhocBeneficiariesReport.response &&
                  adhocBeneficiariesReport.response.results.length > 0 && (
                    <Pagination
                      pageNumber={pageNumber}
                      setPageNumber={(value: number) => {
                        setPageNumber(value - 1);
                      }}
                      pageSize={pageSize}
                      setPageSize={(value: number) => {
                        setPageSize(value);
                        setPageNumber(1);
                      }}
                      recordCount={adhocBeneficiariesReport.response.total}
                    />
                  )}
              </div>
            </div>
          )}

          {/**Render Report here! */}
        </Grid>
      </Grid>
    </Page>
  );
};

export default PayBeNext;
