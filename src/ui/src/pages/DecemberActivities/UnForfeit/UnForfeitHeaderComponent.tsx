import { SharedForfeitHeaderComponent } from "../../../components/ForfeitActivities";
import { UnForfeitHeaderComponentProps } from "../../../reduxstore/types";

export const HeaderComponent: React.FC<UnForfeitHeaderComponentProps> = (params: UnForfeitHeaderComponentProps) => {
  return (
    <SharedForfeitHeaderComponent
      {...params}
      config={{
        activityType: "unforfeit"
      }}
    />
  );
};
