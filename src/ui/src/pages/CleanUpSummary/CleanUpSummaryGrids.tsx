import DemoBadges from "./CleanUpTabContent/DemoBadges";
import DupeNames from "./CleanUpTabContent/DupeNames";
import DupeSsns from "./CleanUpTabContent/DupeSsns";
import NegativeEtvaTab from "./CleanUpTabContent/NegativeEtvaTab";

const negativeETVA: string = "Negative ETVA";
const dupeSsns: string = "Duplicate SSNs";
const demographicBadges: string = "Demographic Badges";
const duplicateNames: string = "Duplicate Names and Birthdays";
const tabs: string[] = ["Summary", negativeETVA, dupeSsns, demographicBadges, duplicateNames];

interface CleanUpSummaryGridProps {
  tabIndex: number;
}

const CleanUpSummaryGrids: React.FC<CleanUpSummaryGridProps> = ({ tabIndex }) => {
  switch (tabIndex) {
    case tabs.indexOf(negativeETVA):
      return <NegativeEtvaTab />;
    case tabs.indexOf(dupeSsns):
      return <DupeSsns />;
    case tabs.indexOf(demographicBadges):
      return <DemoBadges />;
    case tabs.indexOf(duplicateNames):
      return <DupeNames />;
    default:
      return <h1>Grid</h1>;
  }
};

export default CleanUpSummaryGrids;
