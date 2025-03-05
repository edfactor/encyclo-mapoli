import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import MasterInquirySearchFilter from "./MasterInquirySearchFilter";
import MasterInquiryGrid from "./MasterInquiryGrid";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import MasterInquiryEmployeeDetails from "./MasterInquiryEmployeeDetails";
import { useState } from "react";

const MasterInquiry = () => {
  const { masterInquiryEmployeeDetails } = useSelector((state: RootState) => state.yearsEnd);
  const [startProfitYear, setStartProfitYear] = useState<Date | null>(null);
  const [endProfitYear, setEndProfitYear] = useState<Date | null>(null);
  const [startProfitMonth, setStartProfitMonth] = useState<number | null>(null);
  const [endProfitMonth, setEndProfitMonth] = useState<number | null>(null);
  const [socialSecurity, setSocialSecurity] = useState<number | null>(null);
  const [name, setName] = useState<string | null>(null);
  const [badgeNumber, setBadgeNumber] = useState<number | null>(null);
  const [comment, setComment] = useState<string | null>(null);
  const [paymentType, setPaymentType] = useState<"all" | "hardship" | "payoffs" | "rollovers">("all");
  const [memberType, setMemberType] = useState<"all" | "employees" | "beneficiaries" | "none">("all");
  const [contribution, setContribution] = useState<number | null>(null);
  const [earnings, setEarnings] = useState<number | null>(null);
  const [forfeiture, setForfeiture] = useState<number | null>(null);
  const [payment, setPayment] = useState<number | null>(null);
  //const [voids, setVoids] = useState<boolean>(false);
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);

  return (
    <Page label="MASTER INQUIRY (008-10)">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <MasterInquirySearchFilter
              setStartProfitYear={setStartProfitYear}
              setEndProfitYear={setEndProfitYear}
              setStartProfitMonth={setStartProfitMonth}
              setEndProfitMonth={setEndProfitMonth}
              setSocialSecurity={setSocialSecurity}
              setName={setName}
              setBadgeNumber={setBadgeNumber}
              setComment={setComment}
              setPaymentType={setPaymentType}
              setMemberType={setMemberType}
              setContribution={setContribution}
              setEarnings={setEarnings}
              setForfeiture={setForfeiture}
              setPayment={setPayment}
              //setVoids={setVoids}
              setInitialSearchLoaded={setInitialSearchLoaded}
            />
          </DSMAccordion>
        </Grid2>

        {masterInquiryEmployeeDetails && <MasterInquiryEmployeeDetails details={masterInquiryEmployeeDetails} />}

        <Grid2 width="100%">
          <MasterInquiryGrid
            startProfitYearCurrent={startProfitYear}
            endProfitYearCurrent={endProfitYear}
            startProfitMonthCurrent={startProfitMonth}
            endProfitMonthCurrent={endProfitMonth}
            socialSecurityCurrent={socialSecurity}
            nameCurrent={name}
            badgeNumberCurrent={badgeNumber}
            commentCurrent={comment}
            paymentTypeCurrent={paymentType}
            memberTypeCurrent={memberType}
            contributionCurrent={contribution}
            earningsCurrent={earnings}
            forfeitureCurrent={forfeiture}
            paymentCurrent={payment}
            //voidsCurrent={voids}
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default MasterInquiry;
