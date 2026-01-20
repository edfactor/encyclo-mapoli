import { MessageUpdate } from "smart-ui-library";

export enum MessageKeys {
  ForfeituresAdjustment = "ForfeituresAdjustment",
  ProfitShareEditUpdate = "ProfitShareEditUpdate",
  TerminationSave = "TerminationSave",
  TerminationBulkSave = "TerminationBulkSave",
  UnforfeitSave = "UnforfeitSave",
  MilitaryContribution = "MilitaryContribution",
  StateTaxRatesSave = "StateTaxRatesSave",
  TaxCodesSave = "TaxCodesSave",
  AnnuityRatesSave = "AnnuityRatesSave",
  RmdFactorsSave = "RmdFactorsSave",
  BanksSave = "BanksSave",
  BankCreate = "BankCreate",
  BankDisable = "BankDisable",
  BankAccountsSave = "BankAccountsSave",
  BankAccountCreate = "BankAccountCreate",
  BankAccountDisable = "BankAccountDisable",
  BankAccountSetPrimary = "BankAccountSetPrimary"
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
  static readonly TaxCodesSaveSuccess: MessageUpdate = {
    key: MessageKeys.TaxCodesSave,
    message: {
      type: "success",
      title: "Tax codes saved successfully"
    }
  };
  static readonly TaxCodesSaveError: MessageUpdate = {
    key: MessageKeys.TaxCodesSave,
    message: {
      type: "error",
      title: "Failed to save tax codes",
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
  static readonly AnnuityRatesCreateSuccess: MessageUpdate = {
    key: MessageKeys.AnnuityRatesSave,
    message: {
      type: "success",
      title: "Annuity rates created successfully"
    }
  };
  static readonly AnnuityRatesCreateError: MessageUpdate = {
    key: MessageKeys.AnnuityRatesSave,
    message: {
      type: "error",
      title: "Failed to create annuity rates",
      message: "Please try again"
    }
  };
  static readonly RmdFactorsLoadError: MessageUpdate = {
    key: MessageKeys.RmdFactorsSave,
    message: {
      type: "error",
      title: "Failed to load RMD factors",
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
    key: MessageKeys.RmdFactorsSave,
    message: {
      type: "error",
      title: "Failed to save RMD factors",
      message: "Please try again"
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

  // Bank management messages
  static readonly BanksSaveSuccess: MessageUpdate = {
    key: MessageKeys.BanksSave,
    message: {
      type: "success",
      title: "Banks saved successfully"
    }
  };
  static readonly BanksSaveError: MessageUpdate = {
    key: MessageKeys.BanksSave,
    message: {
      type: "error",
      title: "Failed to save banks",
      message: "Please try again"
    }
  };
  static readonly BankCreatedSuccess: MessageUpdate = {
    key: MessageKeys.BankCreate,
    message: {
      type: "success",
      title: "Bank created successfully"
    }
  };
  static readonly BankCreateError: MessageUpdate = {
    key: MessageKeys.BankCreate,
    message: {
      type: "error",
      title: "Failed to create bank",
      message: "Please try again"
    }
  };
  static readonly BankDisabledSuccess: MessageUpdate = {
    key: MessageKeys.BankDisable,
    message: {
      type: "success",
      title: "Bank disabled successfully"
    }
  };
  static readonly BankDisableError: MessageUpdate = {
    key: MessageKeys.BankDisable,
    message: {
      type: "error",
      title: "Failed to disable bank",
      message: "Please try again"
    }
  };

  // Bank account management messages
  static readonly BankAccountsSaveSuccess: MessageUpdate = {
    key: MessageKeys.BankAccountsSave,
    message: {
      type: "success",
      title: "Bank accounts saved successfully"
    }
  };
  static readonly BankAccountsSaveError: MessageUpdate = {
    key: MessageKeys.BankAccountsSave,
    message: {
      type: "error",
      title: "Failed to save bank accounts",
      message: "Please try again"
    }
  };
  static readonly BankAccountCreatedSuccess: MessageUpdate = {
    key: MessageKeys.BankAccountCreate,
    message: {
      type: "success",
      title: "Bank account created successfully"
    }
  };
  static readonly BankAccountCreateError: MessageUpdate = {
    key: MessageKeys.BankAccountCreate,
    message: {
      type: "error",
      title: "Failed to create bank account",
      message: "Please try again"
    }
  };
  static readonly BankAccountDisabledSuccess: MessageUpdate = {
    key: MessageKeys.BankAccountDisable,
    message: {
      type: "success",
      title: "Bank account disabled successfully"
    }
  };
  static readonly BankAccountDisableError: MessageUpdate = {
    key: MessageKeys.BankAccountDisable,
    message: {
      type: "error",
      title: "Failed to disable bank account",
      message: "Please try again"
    }
  };
  static readonly BankAccountSetPrimarySuccess: MessageUpdate = {
    key: MessageKeys.BankAccountSetPrimary,
    message: {
      type: "success",
      title: "Primary account updated successfully"
    }
  };
  static readonly BankAccountSetPrimaryError: MessageUpdate = {
    key: MessageKeys.BankAccountSetPrimary,
    message: {
      type: "error",
      title: "Failed to set primary account",
      message: "Please try again"
    }
  };
}
