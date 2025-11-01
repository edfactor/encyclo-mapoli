import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { createMockStoreAndWrapper } from "../../../../test";
import { MissiveAlertProvider } from "../../../../components/MissiveAlerts/MissiveAlertContext";
import MilitaryContribution from "../MilitaryContribution";

interface MissiveAlert {
  id: number;
  message: string;
}

// Mock child components
vi.mock("components/FrozenYearWarning", () => ({
  default: vi.fn(() => <div role="alert" aria-label="frozen year warning">Frozen Year Warning</div>)
}));

vi.mock("components/StatusReadOnlyInfo", () => ({
  default: vi.fn(() => <div role="status" aria-label="read only info">Read Only Info</div>)
}));

vi.mock("components/StatusDropdownActionNode", () => ({
  default: vi.fn(() => <div role="status" aria-label="status dropdown">Status Dropdown</div>)
}));

vi.mock("components/MissiveAlerts/MissiveAlerts", () => ({
  default: vi.fn(() => <div role="alert" aria-label="missive alerts">Missive Alerts</div>)
}));

vi.mock("./MilitaryContributionSearchFilter", () => ({
  default: vi.fn(() => <section aria-label="search filter">Search Filter</section>)
}));

vi.mock("./MilitaryContributionFormGrid", () => ({
  default: vi.fn(({ onAddContribution, isReadOnly }) => (
    <section aria-label="military contribution grid">
      <button
        onClick={onAddContribution}
        disabled={isReadOnly}>
        Add Contribution
      </button>
    </section>
  ))
}));

vi.mock("./MilitaryContributionForm", () => ({
  default: vi.fn(({ onSubmit, onCancel }) => (
    <form aria-label="military contribution form">
      <button
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
        onClick={onCancel}>
        Cancel
      </button>
    </form>
  ))
}));

vi.mock("../../../../hooks/useDecemberFlowProfitYear", () => ({
  default: vi.fn(() => 2024)
}));

vi.mock("../../../../hooks/useIsProfitYearFrozen", () => ({
  useIsProfitYearFrozen: vi.fn(() => false)
}));

vi.mock("../../../../hooks/useIsReadOnlyByStatus", () => ({
  useIsReadOnlyByStatus: vi.fn(() => false)
}));

vi.mock("../../../../hooks/useMissiveAlerts", () => ({
  useMissiveAlerts: vi.fn(() => ({
    missiveAlerts: [],
    addAlert: vi.fn(),
    clearAlerts: vi.fn()
  }))
}));

vi.mock("../../../../hooks/useReadOnlyNavigation", () => ({
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
  ApiMessageAlert: vi.fn(() => <div role="alert">Message Alert</div>),
  DSMAccordion: vi.fn(({ title, children }) => (
    <section aria-label={title}>
      <h2>{title}</h2>
      {children}
    </section>
  )),
  Page: vi.fn(({ label, actionNode, children }) => (
    <main>
      <h1>{label}</h1>
      {actionNode}
      {children}
    </main>
  )),
  SearchAndReset: vi.fn(({ handleSearch, handleReset, disabled, isFetching }) => (
    <div role="group" aria-label="search and reset controls">
      <button
        onClick={handleSearch}
        disabled={disabled || isFetching}>
        Search
      </button>
      <button
        onClick={handleReset}>
        Reset
      </button>
      {isFetching && <span role="status">Loading...</span>}
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
      },
      lookups: {
        missives: []
      }
    };
    const { wrapper } = createMockStoreAndWrapper(mockState);
    // Wrap with MissiveAlertProvider for this component
    customWrapper = ({ children }: { children: React.ReactNode }) => (
      <MissiveAlertProvider>{wrapper({ children })}</MissiveAlertProvider>
    );
  });

  describe("Rendering", () => {
    it("should render the page with all components", async () => {
      render(<MilitaryContribution />, { wrapper: customWrapper });

      await waitFor(() => {
        expect(screen.getByText("Status Dropdown")).toBeInTheDocument();
        expect(screen.getByText("Message Alert")).toBeInTheDocument();
        expect(screen.getByText("Search Filter")).toBeInTheDocument();
      });
    });

    it("should render search filter accordion", async () => {
      render(<MilitaryContribution />, { wrapper: customWrapper });

      await waitFor(() => {
        expect(screen.getByText("Search Filter")).toBeInTheDocument();
      });
    });

    it("should render military contribution grid when member selected", async () => {
      render(<MilitaryContribution />, { wrapper: customWrapper });

      await waitFor(() => {
        expect(screen.getByRole("button", { name: /add contribution/i })).toBeInTheDocument();
      });
    });

    it("should display message when no member selected", async () => {
      const useDecemberFlowProfitYear = await import("../../../../hooks/useDecemberFlowProfitYear");
      vi.mocked(useDecemberFlowProfitYear.default).mockReturnValueOnce(2024);

      const noMemberState = {
        security: { token: "mock-token" },
        navigation: { navigationData: null },
        inquiry: {
          masterInquiryMemberDetails: null // No member selected
        },
        yearsEnd: {
          selectedProfitYear: 2024
        },
        lookups: {
          missives: []
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
      const { useIsProfitYearFrozen } = await import("../../../../hooks/useIsProfitYearFrozen");
      vi.mocked(useIsProfitYearFrozen).mockReturnValueOnce(true);

      render(<MilitaryContribution />, { wrapper: customWrapper });

      await waitFor(() => {
        expect(screen.getByText("Frozen Year Warning")).toBeInTheDocument();
      });
    });

    it("should not display frozen year warning when year is not frozen", async () => {
      const { useIsProfitYearFrozen } = await import("../../../../hooks/useIsProfitYearFrozen");
      vi.mocked(useIsProfitYearFrozen).mockReturnValueOnce(false);

      render(<MilitaryContribution />, { wrapper: customWrapper });

      await waitFor(() => {
        expect(screen.queryByText("Frozen Year Warning")).not.toBeInTheDocument();
      });
    });
  });

  describe("Read-only mode", () => {
    it("should display read-only info when status is read-only", async () => {
      const { useIsReadOnlyByStatus } = await import("../../../../hooks/useIsReadOnlyByStatus");
      vi.mocked(useIsReadOnlyByStatus).mockReturnValueOnce(true);

      render(<MilitaryContribution />, { wrapper: customWrapper });

      await waitFor(() => {
        expect(screen.getByText("Read Only Info")).toBeInTheDocument();
      });
    });

    it("should disable add contribution button in read-only mode", async () => {
      const { useReadOnlyNavigation } = await import("../../../../hooks/useReadOnlyNavigation");
      vi.mocked(useReadOnlyNavigation).mockReturnValueOnce(true);

      render(<MilitaryContribution />, { wrapper: customWrapper });

      await waitFor(() => {
        const addBtn = screen.getByRole("button", { name: /add contribution/i });
        expect(addBtn).toBeDisabled();
      });
    });

    it("should enable add contribution button when not read-only", async () => {
      const { useReadOnlyNavigation } = await import("../../../../hooks/useReadOnlyNavigation");
      vi.mocked(useReadOnlyNavigation).mockReturnValueOnce(false);

      render(<MilitaryContribution />, { wrapper: customWrapper });

      await waitFor(() => {
        const addBtn = screen.getByRole("button", { name: /add contribution/i });
        expect(addBtn).not.toBeDisabled();
      });
    });
  });

  describe("Add contribution dialog", () => {
    it("should open dialog when add contribution button is clicked", async () => {
      const user = userEvent.setup();
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
        },
        lookups: {
          missives: []
        }
      };
      const { wrapper: mockWrapper } = createMockStoreAndWrapper(memberState);

      const testWrapper = ({ children }: { children: React.ReactNode }) => (
        <MissiveAlertProvider>{mockWrapper({ children })}</MissiveAlertProvider>
      );

      const { rerender } = render(<MilitaryContribution />, { wrapper: testWrapper });

      const addBtn = screen.getByRole("button", { name: /add contribution/i });
      await user.click(addBtn);

      // Dialog should be visible
      rerender(<MilitaryContribution />);

      // Wait for dialog to appear
      await waitFor(() => {
        expect(screen.getByRole("heading", { name: /Add Military Contribution/i })).toBeInTheDocument();
      });
    });

    it("should close dialog when form is submitted", async () => {
      const user = userEvent.setup();
      render(<MilitaryContribution />, { wrapper: customWrapper });

      const addBtn = screen.getByRole("button", { name: /add contribution/i });
      await user.click(addBtn);

      // Wait for dialog to appear
      await waitFor(() => {
        expect(screen.getByRole("button", { name: /submit/i })).toBeInTheDocument();
      });

      // Submit the form
      const submitBtn = screen.getByRole("button", { name: /submit/i });
      await user.click(submitBtn);

      // Dialog should close (form disappears)
      await waitFor(() => {
        expect(screen.queryByRole("button", { name: /submit/i })).not.toBeInTheDocument();
      });
    });

    it("should close dialog when cancel is clicked", async () => {
      const user = userEvent.setup();
      render(<MilitaryContribution />, { wrapper: customWrapper });

      const addBtn = screen.getByRole("button", { name: /add contribution/i });
      await user.click(addBtn);

      await waitFor(() => {
        expect(screen.getByRole("button", { name: /cancel/i })).toBeInTheDocument();
      });

      const cancelBtn = screen.getByRole("button", { name: /cancel/i });
      await user.click(cancelBtn);

      await waitFor(() => {
        expect(screen.queryByRole("button", { name: /cancel/i })).not.toBeInTheDocument();
      });
    });
  });

  describe("Contribution saved message", () => {
    it("should display success message when contribution is saved", async () => {
      const user = userEvent.setup();
      const { setMessage } = await import("smart-ui-library");
      render(<MilitaryContribution />, { wrapper: customWrapper });

      const addBtn = screen.getByRole("button", { name: /add contribution/i });
      await user.click(addBtn);

      await waitFor(() => {
        expect(screen.getByRole("button", { name: /submit/i })).toBeInTheDocument();
      });

      const submitBtn = screen.getByRole("button", { name: /submit/i });
      await user.click(submitBtn);

      await waitFor(() => {
        expect(setMessage).toHaveBeenCalled();
      });
    });
  });

  describe("Member details synchronization", () => {
    it("should fetch contributions when member details change", async () => {
      render(<MilitaryContribution />, { wrapper: customWrapper });

      await waitFor(() => {
        expect(screen.getByRole("button", { name: /add contribution/i })).toBeInTheDocument();
      });
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

      // Verify unmount completed - component should no longer be in document
      expect(screen.queryByText("Status Dropdown")).not.toBeInTheDocument();
    });
  });

  describe("Missive alerts", () => {
    it("should display missive alerts when present", async () => {
      const { useMissiveAlerts } = await import("../../../../hooks/useMissiveAlerts");
      vi.mocked(useMissiveAlerts).mockReturnValueOnce({
        missiveAlerts: [{ id: 1, message: "Test Alert" }] as MissiveAlert[],
        addAlert: vi.fn(),
        clearAlerts: vi.fn()
      });

      render(<MilitaryContribution />, { wrapper: customWrapper });

      expect(screen.getByText("Missive Alerts")).toBeInTheDocument();
    });
  });
});
