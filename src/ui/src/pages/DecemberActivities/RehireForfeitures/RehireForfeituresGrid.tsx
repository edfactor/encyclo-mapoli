import { Typography } from "@mui/material";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetRehireForfeituresQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { GetMilitaryAndRehireForfeituresColumns, GetDetailColumns } from "./RehireForfeituresGridColumns";

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
  const [expandedRows, setExpandedRows] = useState<Record<string, boolean>>({});

  const { rehireForfeitures, rehireForfeituresQueryParams } = useSelector(
    (state: RootState) => state.yearsEnd
  );

  const [triggerSearch, { isFetching }] = useLazyGetRehireForfeituresQuery();

  const onSearch = useCallback(async () => {
    // ... existing search code
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

  // Initialize expandedRows when data is loaded
  useEffect(() => {
    if (rehireForfeitures?.response?.results) {
      const initialExpandState: Record<string, boolean> = {};

      // Set all rows with details to be expanded by default
      rehireForfeitures.response.results.forEach(row => {
        if (row.details && row.details.length > 0) {
          initialExpandState[row.badgeNumber] = true;
        }
      });

      setExpandedRows(initialExpandState);
    }
  }, [rehireForfeitures?.response?.results]);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  // Handle row expansion toggle
  const handleRowExpansion = (badgeNumber: string) => {
    setExpandedRows(prev => ({
      ...prev,
      [badgeNumber]: !prev[badgeNumber]
    }));
  };

  // Create the grid data with expandable rows
  const gridData = useMemo(() => {
    if (!rehireForfeitures?.response?.results) return [];

    const rows = [];

    for (const row of rehireForfeitures.response.results) {
      const hasDetails = row.details && row.details.length > 0;

      // Add main row
      rows.push({
        ...row,
        isExpandable: hasDetails,
        isExpanded: hasDetails && Boolean(expandedRows[row.badgeNumber])
      });

      // Add detail rows if expanded
      if (hasDetails && expandedRows[row.badgeNumber]) {
        for (const detail of row.details) {
          rows.push({
            badgeNumber: row.badgeNumber,
            fullName: row.fullName,
            ...detail,
            isDetail: true,
            parentId: row.badgeNumber
          });
        }
      }
    }

    return rows;
  }, [rehireForfeitures, expandedRows]);

  // Create column definitions with expand/collapse functionality
  const columnDefs = useMemo(() => {
    const mainColumns = GetMilitaryAndRehireForfeituresColumns();
    const detailColumns = GetDetailColumns();

    // Add an expansion column as the first column
    const expansionColumn = {
      headerName: "",
      field: "isExpandable",
      width: 50,
      cellRenderer: (params: any) => {
        if (!params.data.isExpandable) return "";
        return params.data.isExpanded ? "▼" : "►";
      },
      onCellClicked: (params: any) => {
        if (params.data.isExpandable) {
          handleRowExpansion(params.data.badgeNumber);
        }
      }
    };

    // Add a style column to handle indentation and styling
    const styleColumn = {
      headerName: "",
      field: "isDetail",
      width: 20,
      cellRenderer: (params: any) => {
        return params.data.isDetail ? "└" : "";
      }
    };

    // Combine all columns
    return [expansionColumn, styleColumn, ...mainColumns, ...detailColumns];
  }, []);

  // Custom CSS classes for rows
  const getRowClass = (params: any) => {
    return params.data.isDetail ? "detail-row" : "";
  };

  return (
    <div>
      <Typography
        variant="h2"
        sx={{ color: "#0258A5" }}>
        {`Rehire Forfeitures (QPREV-PROF) (${rehireForfeitures?.response.total || 0} ${rehireForfeitures?.response.total === 1 ? 'Record' : 'Records'})`}
      </Typography>

      <style>
        {`
          .detail-row {
            background-color: #f5f5f5;
            font-style: italic;
          }
        `}
      </style>

      {rehireForfeitures?.response && (
        <>
          <DSMGrid
            preferenceKey={"REHIRE_FORFEITURES"}
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: gridData,
              columnDefs: columnDefs,
              getRowClass: getRowClass,
              suppressRowClickSelection: true,
              rowHeight: 40,
            }}
          />

          {rehireForfeitures.response.results.length > 0 && (
            <Pagination
              pageNumber={pageNumber + 1}
              setPageNumber={(value: number) => {
                setPageNumber(value - 1);
                setInitialSearchLoaded(true);
              }}
              pageSize={pageSize}
              setPageSize={(value: number) => {
                setPageSize(value);
                setPageNumber(0);
                setInitialSearchLoaded(true);
              }}
              recordCount={rehireForfeitures.response.total || 0}
            />
          )}
        </>
      )}
    </div>
  );
};

export default RehireForfeituresGrid;