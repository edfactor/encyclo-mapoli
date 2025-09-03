import { MessageUpdate } from "smart-ui-library";

export enum MessageKeys {
  ForfeituresAdjustment = "ForfeituresAdjustment",
  ProfitShareEditUpdate = "ProfitShareEditUpdate"
}

export class Messages {
  static readonly ForfeituresSaveSuccess: MessageUpdate = {
    key: MessageKeys.ForfeituresAdjustment,
    message: {
      type: "success",
      title: "Forfeiture saved successfully"
    }
  };
  static readonly ProfitShareApplySuccess: MessageUpdate = {
    key: MessageKeys.ProfitShareEditUpdate,
    message: {
      type: "success",
      title: "Changes Applied",
      message: `Employees affected: x | Beneficiaries: x, | ETVAs: x `
    }
  };
  static readonly ProfitShareApplyFail: MessageUpdate = {
    key: MessageKeys.ProfitShareEditUpdate,
    message: {
      type: "error",
      title: "Changes Were Not Applied",
      message: `Employees affected: 0 | Beneficiaries: 0, | ETVAs: 0 `
    }
  };
  static readonly ProfitShareRevertSuccess: MessageUpdate = {
    key: MessageKeys.ProfitShareEditUpdate,
    message: {
      type: "success",
      title: "Changes Reverted",
      message: `Employees affected: x | Beneficiaries: x, | ETVAs: x `
    }
  };
  static readonly ProfitShareRevertFail: MessageUpdate = {
    key: MessageKeys.ProfitShareEditUpdate,
    message: {
      type: "error",
      title: "Changes Were Not Reverted",
      message: `Employees affected: 0 | Beneficiaries: 0, | ETVAs: 0 `
    }
  };
  static readonly ProfitShareMasterUpdated: MessageUpdate = {
    key: MessageKeys.ProfitShareEditUpdate,
    message: {
      type: "success",
      title: "Changes Already Applied",
      message: `Updated By: x | Date: x `
    }
  };
}
