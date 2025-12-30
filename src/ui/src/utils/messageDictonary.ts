import { MessageUpdate } from "smart-ui-library";

export enum MessageKeys {
  ForfeituresAdjustment = "ForfeituresAdjustment",
  ProfitShareEditUpdate = "ProfitShareEditUpdate",
  TerminationSave = "TerminationSave",
  TerminationBulkSave = "TerminationBulkSave",
  UnforfeitSave = "UnforfeitSave",
  RmdFactorsSave = "RmdFactorsSave"
  MilitaryContribution = "MilitaryContribution",
  StateTaxRatesSave = "StateTaxRatesSave",
  AnnuityRatesSave = "AnnuityRatesSave"
}

export class Messages {
  static readonly TerminationSaveSuccess: MessageUpdate = {
    key: MessageKeys.TerminationSave,
    message: {
      type: "success",
      title: "Forfeiture adjustment saved successfully"
    }
  };
  static readonly TerminationBulkSaveSuccess: MessageUpdate = {
    key: MessageKeys.TerminationSave,
    message: {
      type: "success",
      title: "All forfeiture adjustments saved successfully"
    }
  };
  static readonly UnforfeitSaveSuccess: MessageUpdate = {
    key: MessageKeys.UnforfeitSave,
    message: {
      type: "success",
      title: "Unforfeiture saved successfully"
    }
  };
  static readonly UnforfeitBulkSaveSuccess: MessageUpdate = {
    key: MessageKeys.UnforfeitSave,
    message: {
      type: "success",
      title: "All unforfeitures saved successfully"
    }
  };
  static readonly ForfeituresSaveSuccess: MessageUpdate = {
    key: MessageKeys.ForfeituresAdjustment,
    message: {
      type: "success",
      title: "Forfeiture saved successfully"
    }
  };
  static readonly MilitaryContributionSaveSuccess: MessageUpdate = {
    key: MessageKeys.MilitaryContribution,
    message: {
      type: "success",
      title: "Military contribution saved successfully"
    }
  };
  static readonly StateTaxRatesSaveSuccess: MessageUpdate = {
    key: MessageKeys.StateTaxRatesSave,
    message: {
      type: "success",
      title: "State tax rates saved successfully"
    }
  };
  static readonly StateTaxRatesSaveError: MessageUpdate = {
    key: MessageKeys.StateTaxRatesSave,
    message: {
      type: "error",
      title: "Failed to save state tax rates",
      message: "Please try again"
    }
  };
  static readonly AnnuityRatesSaveSuccess: MessageUpdate = {
    key: MessageKeys.AnnuityRatesSave,
    message: {
      type: "success",
      title: "Annuity rates saved successfully"
    }
  };
  static readonly AnnuityRatesSaveError: MessageUpdate = {
    key: MessageKeys.AnnuityRatesSave,
    message: {
      type: "error",
      title: "Failed to save annuity rates",
      message: "Please try again"
    }
  };
  static readonly RmdFactorsSaveSuccess: MessageUpdate = {
    key: MessageKeys.RmdFactorsSave,
    message: {
      type: "success",
      title: "RMD factors saved successfully"
    }
  };
    static readonly RmdFactorsSaveError: MessageUpdate = {
        key: MessageKeys.RmdFactorsError,
        message: {
            type: "success",
            title: "Failed to save RMD changes"
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
  static readonly ProfitSharePrerequisiteIncomplete: MessageUpdate = {
    key: MessageKeys.ProfitShareEditUpdate,
    message: {
      type: "error",
      title: "Prerequisite Not Complete",
      message: "" // dynamic message added at dispatch time
    }
  };
}
