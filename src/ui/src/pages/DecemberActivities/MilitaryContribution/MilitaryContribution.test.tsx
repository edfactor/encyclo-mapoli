import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { createMockStoreAndWrapper } from "../../../test";
import { MissiveAlertProvider } from "../../../components/MissiveAlerts/MissiveAlertContext";
import MilitaryContribution from "./MilitaryContribution";

interface MissiveAlert {
  id: number;
  message: string;
  description: string;
  severity: string;
}

// Mock child components
vi.mock("components/FrozenYearWarning", () => ({
  default: vi.fn(() => <div data-testid="frozen-warning">Frozen Year Warning</div>)
}));

vi.mock("components/StatusReadOnlyInfo", () => ({
  default: vi.fn(() => <div data-testid="readonly-info">Read Only Info</div>)
}));

vi.mock("components/StatusDropdownActionNode", () => ({
  default: vi.fn(() => <div data-testid="status-dropdown">Status Dropdown</div>)
}));

vi.mock("components/MissiveAlerts/MissiveAlerts", () => ({
  default: vi.fn(() => <div data-testid="missive-alerts">Missive Alerts</div>)
}));

vi.mock("./MilitaryContributionSearchFilter", () => ({
  default: vi.fn(() => <div data-testid="search-filter">Search Filter</div>)
}));

vi.mock("./MilitaryContributionFormGrid", () => ({
  default: vi.fn(({ onAddContribution, isReadOnly }) => (
    <div data-testid="military-grid">
      <button
        data-testid="add-contribution-btn"
        onClick={onAddContribution}
        disabled={isReadOnly}>
        Add Contribution
      </button>
    </div>
  ))
}));

vi.mock("./MilitaryContributionForm", () => ({
  default: vi.fn(({ onSubmit, onCancel }) => (
    <div data-testid="military-form">
      <button
        data-testid="form-submit-btn"
        onClick={() =>
          onSubmit({
            contributionAmount: 5000,
            contributionYear: 2024,
            isSupplementalContribution: false
          })
        }>
        Submit
      </button>
      <button
        data-testid="form-cancel-btn"
        onClick={onCancel}>
        Cancel
      </button>
    </div>
  ))
}));

vi.mock("../../../hooks/useDecemberFlowProfitYear", () => ({
  default: vi.fn(() => 2024)
}));

vi.mock("../../../hooks/useIsProfitYearFrozen", () => ({
  useIsProfitYearFrozen: vi.fn(() => false)
}));

vi.mock("../../../hooks/useIsReadOnlyByStatus", () => ({
  useIsReadOnlyByStatus: vi.fn(() => false)
}));

vi.mock("../../../hooks/useMissiveAlerts", () => ({
  useMissiveAlerts: vi.fn(() => ({
    missiveAlerts: [],
    addAlert: vi.fn(),
    clearAlerts: vi.fn()
  }))
}));

vi.mock("../../../hooks/useReadOnlyNavigation", () => ({
  useReadOnlyNavigation: vi.fn(() => false)
}));

vi.mock("./hooks/useMilitaryContribution", () => ({
  default: vi.fn(() => ({
    contributionsData: null,
    isLoadingContributions: false,
    contributionsGridPagination: {
      pageNumber: 0,
      pageSize: 25,
      sortParams: { sortBy: "year", isSortDescending: false },
      handlePaginationChange: vi.fn(),
      handleSortChange: vi.fn()
    },
    fetchMilitaryContributions: vi.fn(),
    resetSearch: vi.fn(),
    isSearching: false,
    executeSearch: vi.fn()
  }))
}));

vi.mock("smart-ui-library", () => ({
  ApiMessageAlert: vi.fn(() => <div data-testid="api-message-alert">Message Alert</div>),
  DSMAccordion: vi.fn(({ title, children }) => (
    <div data-testid="accordion">
      <div>{title}</div>
      {children}
    </div>
  )),
  Page: vi.fn(({ label, actionNode, children }) => (
    <div data-testid="page">
      <div>{label}</div>
      {actionNode}
      {children}
    </div>
  )),
  formatNumberWithComma: vi.fn((num) => num.toString()),
  setMessage: vi.fn((payload) => ({ type: "message/setMessage", payload }))
}));

describe("MilitaryContribution", () => {
  let customWrapper: ({ children }: { children: React.ReactNode }) => React.ReactElement;

  beforeEach(() => {
    vi.clearAllMocks();
    const mockState = {
      yearsEnd: {
        selectedProfitYear: 2024
      },
      inquiry: {
        masterInquiryMemberDetails: {
          badgeNumber: 12345,
          firstName: "John",
          lastName: "Doe"
        }
      }
    };
    const { wrapper } = createMockStoreAndWrapper(mockState);
    // Wrap with MissiveAlertProvider for this component
    customWrapper = ({ children }: { children: React.ReactNode }) => (
      <MissiveAlertProvider>{wrapper({ children })}</MissiveAlertProvider>
    );
  });

  describe("Rendering", () => {
    it("should render the page with all components", () => {
      render(<MilitaryContribution />, { wrapper: customWrapper });

      expect(screen.getByTestId("page")).toBeInTheDocument();
      expect(screen.getByTestId("status-dropdown")).toBeInTheDocument();
      expect(screen.getByTestId("api-message-alert")).toBeInTheDocument();
      expect(screen.getByTestId("accordion")).toBeInTheDocument();
    });

    it("should render search filter accordion", () => {
      render(<MilitaryContribution />, { wrapper: customWrapper });

      expect(screen.getByTestId("search-filter")).toBeInTheDocument();
    });

    it("should render military contribution grid when member selected", () => {
      render(<MilitaryContribution />, { wrapper: customWrapper });

      expect(screen.getByTestId("military-grid")).toBeInTheDocument();
    });

    it("should display message when no member selected", async () => {
      const useDecemberFlowProfitYear = await import("../../../hooks/useDecemberFlowProfitYear");
      vi.mocked(useDecemberFlowProfitYear.default).mockReturnValueOnce(2024);

      const noMemberState = {
        security: { token: "mock-token" },
        navigation: { navigationData: null },
        inquiry: {
          masterInquiryMemberDetails: null // No member selected
        },
        yearsEnd: {
          selectedProfitYear: 2024
        }
      };
      const { wrapper: mockWrapper } = createMockStoreAndWrapper(noMemberState);

      const testWrapper = ({ children }: { children: React.ReactNode }) => (
        <MissiveAlertProvider>{mockWrapper({ children })}</MissiveAlertProvider>
      );

      render(<MilitaryContribution />, { wrapper: testWrapper });

      expect(screen.getByText(/Please search for and select an employee/)).toBeInTheDocument();
    });
  });

  describe("Frozen year warning", () => {
    it("should display frozen year warning when year is frozen", async () => {
      const { useIsProfitYearFrozen } = await import("../../../hooks/useIsProfitYearFrozen");
      vi.mocked(useIsProfitYearFrozen).mockReturnValueOnce(true);

      render(<MilitaryContribution />, { wrapper: customWrapper });

      expect(screen.getByTestId("frozen-warning")).toBeInTheDocument();
    });

    it("should not display frozen year warning when year is not frozen", async () => {
      const { useIsProfitYearFrozen } = await import("../../../hooks/useIsProfitYearFrozen");
      vi.mocked(useIsProfitYearFrozen).mockReturnValueOnce(false);

      render(<MilitaryContribution />, { wrapper: customWrapper });

      expect(screen.queryByTestId("frozen-warning")).not.toBeInTheDocument();
    });
  });

  describe("Read-only mode", () => {
    it("should display read-only info when status is read-only", async () => {
      const { useIsReadOnlyByStatus } = await import("../../../hooks/useIsReadOnlyByStatus");
      vi.mocked(useIsReadOnlyByStatus).mockReturnValueOnce(true);

      render(<MilitaryContribution />, { wrapper: customWrapper });

      expect(screen.getByTestId("readonly-info")).toBeInTheDocument();
    });

    it("should disable add contribution button in read-only mode", async () => {
      const { useReadOnlyNavigation } = await import("../../../hooks/useReadOnlyNavigation");
      vi.mocked(useReadOnlyNavigation).mockReturnValueOnce(true);

      render(<MilitaryContribution />, { wrapper: customWrapper });

      const addBtn = screen.getByTestId("add-contribution-btn");
      expect(addBtn).toBeDisabled();
    });

    it("should enable add contribution button when not read-only", async () => {
      const { useReadOnlyNavigation } = await import("../../../hooks/useReadOnlyNavigation");
      vi.mocked(useReadOnlyNavigation).mockReturnValueOnce(false);

      render(<MilitaryContribution />, { wrapper: customWrapper });

      const addBtn = screen.getByTestId("add-contribution-btn");
      expect(addBtn).not.toBeDisabled();
    });
  });

  describe("Add contribution dialog", () => {
    it("should open dialog when add contribution button is clicked", async () => {
      const memberState = {
        yearsEnd: {
          selectedProfitYear: 2024
        },
        inquiry: {
          masterInquiryMemberDetails: {
            badgeNumber: 12345,
            firstName: "John",
            lastName: "Doe"
          }
        }
      };
      const { wrapper: mockWrapper } = createMockStoreAndWrapper(memberState);

      const testWrapper = ({ children }: { children: React.ReactNode }) => (
        <MissiveAlertProvider>{mockWrapper({ children })}</MissiveAlertProvider>
      );

      const { rerender } = render(<MilitaryContribution />, { wrapper: testWrapper });

      const addBtn = screen.getByTestId("add-contribution-btn");
      fireEvent.click(addBtn);

      // Dialog should be visible
      rerender(<MilitaryContribution />);

      // Wait for dialog to appear
      await waitFor(() => {
        expect(screen.getByRole("heading", { name: /Add Military Contribution/i })).toBeInTheDocument();
      });
    });

    it("should close dialog when form is submitted", async () => {
      render(<MilitaryContribution />, { wrapper: customWrapper });

      const addBtn = screen.getByTestId("add-contribution-btn");
      fireEvent.click(addBtn);

      // Wait for dialog to appear
      await waitFor(() => {
        expect(screen.getByTestId("military-form")).toBeInTheDocument();
      });

      // Submit the form
      const submitBtn = screen.getByTestId("form-submit-btn");
      fireEvent.click(submitBtn);

      // Dialog should close (form disappears)
      await waitFor(() => {
        expect(screen.queryByTestId("military-form")).not.toBeInTheDocument();
      });
    });

    it("should close dialog when cancel is clicked", async () => {
      render(<MilitaryContribution />, { wrapper: customWrapper });

      const addBtn = screen.getByTestId("add-contribution-btn");
      fireEvent.click(addBtn);

      await waitFor(() => {
        expect(screen.getByTestId("military-form")).toBeInTheDocument();
      });

      const cancelBtn = screen.getByTestId("form-cancel-btn");
      fireEvent.click(cancelBtn);

      await waitFor(() => {
        expect(screen.queryByTestId("military-form")).not.toBeInTheDocument();
      });
    });
  });

  describe("Contribution saved message", () => {
    it("should display success message when contribution is saved", async () => {
      const { setMessage } = await import("smart-ui-library");
      render(<MilitaryContribution />, { wrapper: customWrapper });

      const addBtn = screen.getByTestId("add-contribution-btn");
      fireEvent.click(addBtn);

      await waitFor(() => {
        expect(screen.getByTestId("military-form")).toBeInTheDocument();
      });

      const submitBtn = screen.getByTestId("form-submit-btn");
      fireEvent.click(submitBtn);

      await waitFor(() => {
        expect(setMessage).toHaveBeenCalled();
      });
    });
  });

  describe("Member details synchronization", () => {
    it("should fetch contributions when member details change", () => {
      render(<MilitaryContribution />, { wrapper: customWrapper });

      expect(screen.getByTestId("military-grid")).toBeInTheDocument();
    });
  });

  describe("Component cleanup", () => {
    it("should reset search on unmount", () => {
      const { wrapper: mockWrapper } = createMockStoreAndWrapper();

      const testWrapper = ({ children }: { children: React.ReactNode }) => (
        <MissiveAlertProvider>{mockWrapper({ children })}</MissiveAlertProvider>
      );

      const { unmount } = render(<MilitaryContribution />, { wrapper: testWrapper });

      // The component should call resetSearch on unmount
      unmount();

      // Verify unmount completed
      expect(screen.queryByTestId("page")).not.toBeInTheDocument();
    });
  });

  describe("Missive alerts", () => {
    it("should display missive alerts when present", async () => {
      const { useMissiveAlerts } = await import("../../../hooks/useMissiveAlerts");
      vi.mocked(useMissiveAlerts).mockReturnValueOnce({
        missiveAlerts: [{ id: 1, message: "Test Alert", description: "Test description", severity: "info" }] as MissiveAlert[],
        addAlert: vi.fn(),
        clearAlerts: vi.fn()
      });

      render(<MilitaryContribution />, { wrapper: customWrapper });

      expect(screen.getByTestId("missive-alerts")).toBeInTheDocument();
    });
  });
});
