import { Typography } from "@mui/material";
import { useMemo } from "react";
import { useNavigate } from "react-router";
import { ProfallData } from "reduxstore/types";
import { DSMGrid } from "smart-ui-library";
import { GetProfallGridColumns } from "./ProfallGridColumns";

const sampleProfallData: ProfallData[] = [
  {
    badge: 47425,
    employeeName: "SKYWALKER, LUKE",
    address: "1234 Moisture Farm Lane",
    city: "Mos Eisley",
    state: "TS",
    zipCode: "12345"
  },
  {
    badge: 82424,
    employeeName: "SOLO, HAN",
    address: "5678 Millennium Way",
    city: "Coronet City",
    state: "CR",
    zipCode: "67890"
  },
  {
    badge: 85744,
    employeeName: "ORGANA, LEIA",
    address: "9012 Royal Palace Road",
    city: "Aldera",
    state: "AL",
    zipCode: "34567"
  },
  {
    badge: 91532,
    employeeName: "KENOBI, OBI-WAN",
    address: "3456 High Ground Circle",
    city: "Mos Espa",
    state: "TS",
    zipCode: "89012"
  },
  {
    badge: 77889,
    employeeName: "CALRISSIAN, LANDO",
    address: "7890 Cloud City Drive",
    city: "Bespin",
    state: "BS",
    zipCode: "45678"
  }
];

const ProfallGrid = () => {
  const navigate = useNavigate();

  const handleNavigationForButton = useCallback(
    (destination: string | Partial<Path>) => {
      navigate(destination);
    },
    [navigate]
  );

  const columnDefs = useMemo(() => GetProfallGridColumns(handleNavigationForButton), [handleNavigationForButton]);

  return (
    <>
      <div style={{ padding: "0 24px 0 24px" }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          {`PROFALL REPORT (${sampleProfallData.length || 0})`}
        </Typography>
      </div>
      <DSMGrid
        preferenceKey={"PROFALL_REPORT"}
        isLoading={false}
        handleSortChanged={(_params) => {}}
        providedOptions={{
          rowData: sampleProfallData,
          columnDefs: columnDefs
        }}
      />
    </>
  );
};

export default ProfallGrid;
