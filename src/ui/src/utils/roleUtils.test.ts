import { describe, expect, it } from "vitest";
import { ImpersonationRoles } from "../types/common/enums";
import { validateImpersonationRoles, validateRoleRemoval } from "./roleUtils";

describe("validateImpersonationRoles", () => {
  describe("Single role selection", () => {
    it("should allow selecting a single role when no roles are selected", () => {
      const result = validateImpersonationRoles([], ImpersonationRoles.Auditor);
      expect(result).toEqual([ImpersonationRoles.Auditor]);
    });

    it("should replace existing role when selecting a non-combinable role", () => {
      const result = validateImpersonationRoles([ImpersonationRoles.Auditor], ImpersonationRoles.ItDevOps);
      expect(result).toEqual([ImpersonationRoles.ItDevOps]);
    });

    it("should replace multiple roles when selecting a non-combinable role", () => {
      const result = validateImpersonationRoles(
        [ImpersonationRoles.ExecutiveAdministrator, ImpersonationRoles.FinanceManager],
        ImpersonationRoles.Auditor
      );
      expect(result).toEqual([ImpersonationRoles.Auditor]);
    });
  });

  describe("Executive-Administrator with combinable roles", () => {
    it("should allow adding Executive-Administrator to Finance-Manager", () => {
      const result = validateImpersonationRoles(
        [ImpersonationRoles.FinanceManager],
        ImpersonationRoles.ExecutiveAdministrator
      );
      expect(result).toEqual([ImpersonationRoles.FinanceManager, ImpersonationRoles.ExecutiveAdministrator]);
    });

    it("should allow adding Executive-Administrator to Distributions-Clerk", () => {
      const result = validateImpersonationRoles(
        [ImpersonationRoles.DistributionsClerk],
        ImpersonationRoles.ExecutiveAdministrator
      );
      expect(result).toEqual([ImpersonationRoles.DistributionsClerk, ImpersonationRoles.ExecutiveAdministrator]);
    });

    it("should allow adding Executive-Administrator to Hardship-Administrator", () => {
      const result = validateImpersonationRoles(
        [ImpersonationRoles.HardshipAdministrator],
        ImpersonationRoles.ExecutiveAdministrator
      );
      expect(result).toEqual([ImpersonationRoles.HardshipAdministrator, ImpersonationRoles.ExecutiveAdministrator]);
    });

    it("should allow adding Executive-Administrator to System-Administrator", () => {
      const result = validateImpersonationRoles(
        [ImpersonationRoles.ProfitSharingAdministrator],
        ImpersonationRoles.ExecutiveAdministrator
      );
      expect(result).toEqual([
        ImpersonationRoles.ProfitSharingAdministrator,
        ImpersonationRoles.ExecutiveAdministrator
      ]);
    });

    it("should allow adding Finance-Manager to Executive-Administrator", () => {
      const result = validateImpersonationRoles(
        [ImpersonationRoles.ExecutiveAdministrator],
        ImpersonationRoles.FinanceManager
      );
      expect(result).toEqual([ImpersonationRoles.ExecutiveAdministrator, ImpersonationRoles.FinanceManager]);
    });

    it("should allow adding Distributions-Clerk to Executive-Administrator", () => {
      const result = validateImpersonationRoles(
        [ImpersonationRoles.ExecutiveAdministrator],
        ImpersonationRoles.DistributionsClerk
      );
      expect(result).toEqual([ImpersonationRoles.ExecutiveAdministrator, ImpersonationRoles.DistributionsClerk]);
    });
  });

  describe("Non-combinable scenarios with Executive-Administrator", () => {
    it("should replace existing role when adding Executive-Administrator to Auditor", () => {
      const result = validateImpersonationRoles(
        [ImpersonationRoles.Auditor],
        ImpersonationRoles.ExecutiveAdministrator
      );
      expect(result).toEqual([ImpersonationRoles.ExecutiveAdministrator]);
    });

    it("should replace existing role when adding Executive-Administrator to IT-DevOps", () => {
      const result = validateImpersonationRoles(
        [ImpersonationRoles.ItDevOps],
        ImpersonationRoles.ExecutiveAdministrator
      );
      expect(result).toEqual([ImpersonationRoles.ExecutiveAdministrator]);
    });

    it("should replace Executive-Administrator when adding non-combinable role", () => {
      const result = validateImpersonationRoles(
        [ImpersonationRoles.ExecutiveAdministrator],
        ImpersonationRoles.Auditor
      );
      expect(result).toEqual([ImpersonationRoles.Auditor]);
    });

    it("should replace combination when adding non-combinable role", () => {
      const result = validateImpersonationRoles(
        [ImpersonationRoles.ExecutiveAdministrator, ImpersonationRoles.FinanceManager],
        ImpersonationRoles.ItOperations
      );
      expect(result).toEqual([ImpersonationRoles.ItOperations]);
    });
  });

  describe("Multiple combinable roles without Executive-Administrator", () => {
    it("should replace when adding second combinable role without Executive-Administrator", () => {
      const result = validateImpersonationRoles(
        [ImpersonationRoles.FinanceManager],
        ImpersonationRoles.DistributionsClerk
      );
      expect(result).toEqual([ImpersonationRoles.DistributionsClerk]);
    });

    it("should replace when adding third combinable role to two existing", () => {
      const result = validateImpersonationRoles(
        [ImpersonationRoles.FinanceManager, ImpersonationRoles.DistributionsClerk],
        ImpersonationRoles.HardshipAdministrator
      );
      expect(result).toEqual([ImpersonationRoles.HardshipAdministrator]);
    });
  });
});

describe("validateRoleRemoval", () => {
  it("should remove single role leaving empty array", () => {
    const result = validateRoleRemoval([ImpersonationRoles.Auditor], ImpersonationRoles.Auditor);
    expect(result).toEqual([]);
  });

  it("should remove role from combination of Executive-Administrator and combinable role", () => {
    const result = validateRoleRemoval(
      [ImpersonationRoles.ExecutiveAdministrator, ImpersonationRoles.FinanceManager],
      ImpersonationRoles.FinanceManager
    );
    expect(result).toEqual([ImpersonationRoles.ExecutiveAdministrator]);
  });

  it("should keep only first combinable role when removing Executive-Administrator from multiple combinable roles", () => {
    const result = validateRoleRemoval(
      [
        ImpersonationRoles.ExecutiveAdministrator,
        ImpersonationRoles.FinanceManager,
        ImpersonationRoles.DistributionsClerk
      ],
      ImpersonationRoles.ExecutiveAdministrator
    );
    expect(result).toEqual([ImpersonationRoles.FinanceManager]);
  });

  it("should keep single combinable role when removing Executive-Administrator", () => {
    const result = validateRoleRemoval(
      [ImpersonationRoles.ExecutiveAdministrator, ImpersonationRoles.FinanceManager],
      ImpersonationRoles.ExecutiveAdministrator
    );
    expect(result).toEqual([ImpersonationRoles.FinanceManager]);
  });

  it("should keep single non-combinable role when removing another non-combinable role", () => {
    const result = validateRoleRemoval(
      [ImpersonationRoles.Auditor, ImpersonationRoles.ItDevOps],
      ImpersonationRoles.ItDevOps
    );
    expect(result).toEqual([ImpersonationRoles.Auditor]);
  });

  describe("SSN-Unmasking removal scenarios", () => {
    it("should prevent selecting SSN-Unmasking when no roles are selected", () => {
      const result = validateImpersonationRoles([], ImpersonationRoles.SsnUnmasking);
      expect(result).toEqual([]);
    });

    it("should allow adding SSN-Unmasking to ProfitSharingAdministrator", () => {
      const result = validateImpersonationRoles(
        [ImpersonationRoles.ProfitSharingAdministrator],
        ImpersonationRoles.SsnUnmasking
      );
      expect(result).toEqual([ImpersonationRoles.ProfitSharingAdministrator, ImpersonationRoles.SsnUnmasking]);
    });

    it("should prevent adding SSN-Unmasking to ExecutiveAdministrator with non-compatible base roles", () => {
      const result = validateImpersonationRoles(
        [ImpersonationRoles.ExecutiveAdministrator, ImpersonationRoles.FinanceManager],
        ImpersonationRoles.SsnUnmasking
      );
      // SSN-Unmasking can only pair with ProfitSharingAdministrator or ExecutiveAdministrator directly
      expect(result).toEqual([]);
    });

    it("should prevent adding SSN-Unmasking to Auditor", () => {
      const result = validateImpersonationRoles([ImpersonationRoles.Auditor], ImpersonationRoles.SsnUnmasking);
      expect(result).toEqual([]);
    });

    it("should prevent adding SSN-Unmasking to ItDevOps", () => {
      const result = validateImpersonationRoles([ImpersonationRoles.ItDevOps], ImpersonationRoles.SsnUnmasking);
      expect(result).toEqual([]);
    });

    it("should prevent adding SSN-Unmasking to Finance-Manager alone (not with Executive-Administrator)", () => {
      const result = validateImpersonationRoles([ImpersonationRoles.FinanceManager], ImpersonationRoles.SsnUnmasking);
      expect(result).toEqual([]);
    });

    it("should replace base role while keeping SSN-Unmasking only if new base is directly compatible", () => {
      const result = validateImpersonationRoles(
        [ImpersonationRoles.SsnUnmasking, ImpersonationRoles.ProfitSharingAdministrator],
        ImpersonationRoles.ExecutiveAdministrator
      );
      // ExecutiveAdministrator can only be added to combinable roles, and SSN-Unmasking cannot pair with ExecutiveAdministrator alone
      expect(result).toEqual([
        ImpersonationRoles.SsnUnmasking,
        ImpersonationRoles.ProfitSharingAdministrator,
        ImpersonationRoles.ExecutiveAdministrator
      ]);
    });

    it("should remove SSN-Unmasking when base role is removed and no Executive-Administrator", () => {
      const result = validateRoleRemoval(
        [ImpersonationRoles.ProfitSharingAdministrator, ImpersonationRoles.SsnUnmasking],
        ImpersonationRoles.ProfitSharingAdministrator
      );
      expect(result).toEqual([]);
    });

    it("should keep SSN-Unmasking when Executive-Administrator remains after base role removal", () => {
      const result = validateRoleRemoval(
        [ImpersonationRoles.ExecutiveAdministrator, ImpersonationRoles.FinanceManager, ImpersonationRoles.SsnUnmasking],
        ImpersonationRoles.FinanceManager
      );
      expect(result).toEqual([ImpersonationRoles.ExecutiveAdministrator, ImpersonationRoles.SsnUnmasking]);
    });

    it("should remove SSN-Unmasking when removing Executive-Administrator from mixed roles", () => {
      const result = validateRoleRemoval(
        [ImpersonationRoles.ExecutiveAdministrator, ImpersonationRoles.FinanceManager, ImpersonationRoles.SsnUnmasking],
        ImpersonationRoles.ExecutiveAdministrator
      );
      // When ExecutiveAdministrator is removed, Finance-Manager remains but SSN-Unmasking is also removed
      // because Finance-Manager alone is not a compatible base for SSN-Unmasking
      expect(result).toEqual([ImpersonationRoles.FinanceManager]);
    });
  });
});
