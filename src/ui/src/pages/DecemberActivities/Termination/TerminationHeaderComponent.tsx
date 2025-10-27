import { IHeaderParams } from "ag-grid-community";
import { SharedForfeitHeaderComponent } from "../../../components/ForfeitActivities";
import { ForfeitureAdjustmentUpdateRequest } from "../../../types";

interface HeaderComponentProps extends IHeaderParams {
  addRowToSelectedRows: (id: number) => void;
  removeRowFromSelectedRows: (id: number) => void;
  onBulkSave?: (requests: ForfeitureAdjustmentUpdateRequest[], names: string[]) => Promise<void>;
  isBulkSaving?: boolean;
  isReadOnly?: boolean;
}

export const HeaderComponent: React.FC<HeaderComponentProps> = (params: HeaderComponentProps) => {
  return (
    <SharedForfeitHeaderComponent
      {...params}
      config={{
        activityType: "termination"
      }}
    />
  );
};
