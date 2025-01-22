import Balance from "./FrozenTabContent/Balance";
import Contributions from "./FrozenTabContent/Contributions";
import Distributions from "./FrozenTabContent/Distributions";
import Forfeitures from "./FrozenTabContent/Forfeitures";

const distributions: string = "Distributions";
const contributions: string = "Contributions";
const forfeitures: string = "Forfeitures";
const balance: string = "Balance";
const tabs: string[] = ["Summary", distributions, contributions, forfeitures, balance];

interface FrozenSummaryGridProps {
  tabIndex: number;
}

const FrozenSummaryGrids: React.FC<FrozenSummaryGridProps> = ({ tabIndex }) => {
  switch (tabIndex) {
    case tabs.indexOf(distributions):
      return <Distributions />;
    case tabs.indexOf(contributions):
      return <Contributions />;
    case tabs.indexOf(forfeitures):
      return <Forfeitures />;
    case tabs.indexOf(balance):
      return <Balance />;
    default:
      return <h1>Grid</h1>;
  }
};

export default FrozenSummaryGrids;
