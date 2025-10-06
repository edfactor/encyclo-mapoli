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
});
