import { Typography } from "@mui/material";
import LabelValueSection from "../../../components/LabelValueSection";
import Grid2 from '@mui/material/Grid2';

interface TotalsContentProps {
  store: string;
}

const TotalsContent: React.FC<TotalsContentProps> = ({ store }) => {
  const data = [
    { label: "Total Number of Employees:", value: "999,999" },
    { label: "Total Beginning Balances:", value: "1,111,111,111.11" },
    { label: "Total Earnings:", value: "222,222,222.22" },
    { label: "Total Contributions:", value: "333,333,333.33" },
    { label: "Total Forfeitures:", value: "0.00" },
    { label: "Total Disbursements:", value: "444,444,444.44" },
    { label: "Total End Balances:", value: "5,555,555,555.55" },
    { label: "Total Vested Balance:", value: "999,999,999.99" }
  ];

  return (
    <Grid2 container direction="column" width="100%">
      <Grid2 paddingX="24px">
        <Typography
          variant="h2"
          sx={{ color: "#0258A5", marginBottom: "16px" }}>
          {`Totals`}
        </Typography>
      </Grid2>
      <Grid2 width="100%" paddingX="24px">
        <LabelValueSection data={data} />
      </Grid2>
    </Grid2>
  );
};

export default TotalsContent; 