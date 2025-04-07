import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetRehireForfeituresQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { GetMilitaryAndRehireForfeituresColumns } from "./RehireForfeituresGridColumns";
import { ColDef, ValueFormatterParams } from "ag-grid-community";
import { agGridNumberToCurrency } from "smart-ui-library";

interface MilitaryAndRehireForfeituresGridSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const RehireForfeituresGrid: React.FC<MilitaryAndRehireForfeituresGridSearchProps> = ({
                                                                                        initialSearchLoaded,
                                                                                        setInitialSearchLoaded
                                                                                      }) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });

  const { rehireForfeitures, rehireForfeituresQueryParams } = useSelector(
    (state: RootState) => state.yearsEnd
  );

  const [triggerSearch, { isFetching }] = useLazyGetRehireForfeituresQuery();

  const onSearch = useCallback(async () => {
    const request = {
      profitYear: rehireForfeituresQueryParams?.profitYear ?? 0,
      beginningDate: rehireForfeituresQueryParams?.beginningDate ?? "",
      endingDate: rehireForfeituresQueryParams?.endingDate ?? "",
      pagination: {
        skip: pageNumber * pageSize,
        take: pageSize,
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending
      }
    };

    await triggerSearch(request, false);
  }, [
    pageNumber,
    pageSize,
    sortParams,
    triggerSearch,
    rehireForfeituresQueryParams?.profitYear,
    rehireForfeituresQueryParams?.beginningDate,
    rehireForfeituresQueryParams?.endingDate
  ]);

  useEffect(() => {
    if (initialSearchLoaded) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, onSearch]);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  // For debugging - log the data structure when it changes
  useEffect(() => {
    if (rehireForfeitures?.response?.results?.length > 0) {
      console.log("First row data:", rehireForfeitures.response.results[0]);
    }
  }, [rehireForfeitures]);

  // Create a flattened view of the data instead of master/detail
  const getFlattenedData = useMemo(() => {
    if (!rehireForfeitures?.response?.results) return [];

    const flattenedRows = [];

    for (const row of rehireForfeitures.response.results) {
      // Add main row
      flattenedRows.push({
        ...row,
        isParent: true,
        rowType: 'parent'
      });

      // Add detail rows if they exist
      if (row.details && row.details.length > 0) {
        for (const detail of row.details) {
          flattenedRows.push({
            badgeNumber: row.badgeNumber,
            fullName: row.fullName,
            ...detail,
            isParent: false,
            rowType: 'child',
            parentId: row.badgeNumber
          });
        }
      }
    }

    return flattenedRows;
  }, [rehireForfeitures]);

  // Updated column defs to handle the flattened structure
  const columnDefs: ColDef[] = useMemo(() => {
    return [
      {
        headerName: "Badge",
        field: "badgeNumber",
        minWidth: 80,
        headerClass: "right-align",
        cellClass: "right-align",
        resizable: true,
        sortable: true
      },
      {
        headerName: "Full Name",
        field: "fullName",
        minWidth: 150,
        headerClass: "left-align",
        cellClass: "left-align",
        resizable: true
      },
      {
        headerName: "SSN",
        field: "ssn",
        minWidth: 90,
        headerClass: "left-align",
        cellClass: "left-align",
        resizable: true
      },
      {
        headerName: "Rehired Date",
        field: "reHiredDate",
        minWidth: 120,
        headerClass: "left-align",
        cellClass: "left-align",
        resizable: true
      },
      {
        headerName: "Contribution Years",
        field: "companyContributionYears",
        minWidth: 150,
        headerClass: "right-align",
        cellClass: "right-align",
        resizable: true
      },
      {
        headerName: "Hours Current Year",
        field: "hoursCurrentYear",
        minWidth: 150,
        headerClass: "right-align",
        cellClass: "right-align",
        resizable: true
      },
      {
        headerName: "Profit Year",
        field: "profitYear",
        minWidth: 100,
        headerClass: "right-align",
        cellClass: "right-align",
        resizable: true
      },
      {
        headerName: "Forfeiture",
        field: "forfeiture",
        minWidth: 150,
        headerClass: "right-align",
        cellClass: "right-align",
        resizable: true,
        valueFormatter: agGridNumberToCurrency
      },
      {
        headerName: "Remark",
        field: "remark",
        minWidth: 150,
        headerClass: "left-align",
        cellClass: "left-align",
        resizable: true
      }
    ];
  }, []);

  // Custom row class to visually distinguish parent and child rows
  const getRowClass = (params) => {
    if (params.data.rowType === 'child') {
      return 'detail-row';
    }
    return '';
  };

  return (
    <>
      {rehireForfeitures?.response && (
        <>
          <div style={{ padding: "0 24px 0 24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`${CAPTIONS.REHIRE_FORFEITURES} (${rehireForfeitures?.response.total || 0} records)`}
            </Typography>
          </div>

          {/* Add some custom CSS for the detail rows */}
          <style>{`
            .detail-row {
              background-color: #f5f5f5;
            }
            .detail-row:hover {
              background-color: #eaeaea;
            }
          `}</style>

          <DSMGrid
            preferenceKey={"QPREV-PROF"}
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: getFlattenedData,
              columnDefs: columnDefs,
              getRowClass: getRowClass
            }}
          />
        </>
      )}
      {!!rehireForfeitures && rehireForfeitures.response.results.length > 0 && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => {
            setPageNumber(value - 1);
            setInitialSearchLoaded(true);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            setPageSize(value);
            setPageNumber(1);
            setInitialSearchLoaded(true);
          }}
          recordCount={rehireForfeitures.response.total}
        />
      )}
    </>
  );
};

export default RehireForfeituresGrid;