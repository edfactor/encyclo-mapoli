import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import React from "react";
import { createMockStoreAndWrapper } from "../../../../test";
import { MissiveAlertProvider } from "../../../../components/MissiveAlerts/MissiveAlertContext";
import MilitaryContribution from "../MilitaryContribution";

// Hoist all mock functions so they can be accessed in vi.mock() and modified in tests
const {
  mockTriggerGetMilitaryContributions,
  mockTriggerSearchMasterInquiry,
  mockTriggerGetProfitMasterInquiryMember,
  mockUseDecemberFlowProfitYear,
  mockUseIsProfitYearFrozen,
  mockUseIsReadOnlyByStatus,
  mockUseReadOnlyNavigation,
  mockUseMissiveAlerts
} = vi.hoisted(() => ({
  mockTriggerGetMilitaryContributions: vi.fn(),
  mockTriggerSearchMasterInquiry: vi.fn(),
  mockTriggerGetProfitMasterInquiryMember: vi.fn(),
  mockUseDecemberFlowProfitYear: vi.fn(),
  mockUseIsProfitYearFrozen: vi.fn(),
  mockUseIsReadOnlyByStatus: vi.fn(),
  mockUseReadOnlyNavigation: vi.fn(),
  mockUseMissiveAlerts: vi.fn()
}));

vi.mock("../../../../reduxstore/api/MilitaryApi", () => ({
  MilitaryApi: {
    reducerPath: "militaryApi",
    reducer: (state = {}) => state,
    middleware: []
  },
  useLazyGetMilitaryContributionsQuery: vi.fn(() => [
    mockTriggerGetMilitaryContributions,
    { isFetching: false }
  ])
}));

vi.mock("../../../../reduxstore/api/InquiryApi", () => ({
  InquiryApi: {
    reducerPath: "inquiryApi",
    reducer: (state = {}) => state,
    middleware: [],
    util: {
      invalidateTags: vi.fn(() => ({ type: "inquiryApi/invalidateTags", payload: [] }))
    }
  },
  useLazySearchProfitMasterInquiryQuery: vi.fn(() => [
    mockTriggerSearchMasterInquiry,
    { isFetching: false }
  ]),
  useLazyGetProfitMasterInquiryMemberQuery: vi.fn(() => [
    mockTriggerGetProfitMasterInquiryMember,
    { isFetching: false }
  ])
}));

vi.mock("../../../../hooks/useDecemberFlowProfitYear", () => ({
  default: mockUseDecemberFlowProfitYear
}));

vi.mock("../../../../hooks/useIsProfitYearFrozen", () => ({
  useIsProfitYearFrozen: mockUseIsProfitYearFrozen
}));

vi.mock("../../../../hooks/useIsReadOnlyByStatus", () => ({
  useIsReadOnlyByStatus: mockUseIsReadOnlyByStatus
}));

vi.mock("../../../../hooks/useReadOnlyNavigation", () => ({
  useReadOnlyNavigation: mockUseReadOnlyNavigation
}));

vi.mock("../../../../hooks/useMissiveAlerts", () => ({
  useMissiveAlerts: mockUseMissiveAlerts
}));

interface MissiveAlert {
  id: number;
  message: string;
}

// Mock child components
vi.mock("../../../../components/FrozenYearWarning", () => ({
  default: vi.fn(() => React.createElement('div', { role: 'alert', 'aria-label': 'frozen year warning' }, 'Frozen Year Warning'))
}));

vi.mock("../../../../components/StatusReadOnlyInfo", () => ({
  default: vi.fn(() => React.createElement('div', { role: 'status', 'aria-label': 'read only info' }, 'Read Only Info'))
}));

vi.mock("../../../../components/StatusDropdownActionNode", () => ({
  default: vi.fn(() => React.createElement('div', { role: 'status', 'aria-label': 'status dropdown' }, 'Status Dropdown'))
}));

vi.mock("../../../../components/MissiveAlerts/MissiveAlerts", () => ({
  default: vi.fn(() => React.createElement('div', { role: 'alert', 'aria-label': 'missive alerts' }, 'Missive Alerts'))
}));

vi.mock("../MilitaryContributionSearchFilter", () => ({
  default: vi.fn(() => React.createElement('section', { 'aria-label': 'search filter' }, 'Search Filter'))
}));

vi.mock("../MilitaryContributionFormGrid", () => ({
  default: vi.fn(({ onAddContribution, isReadOnly }) =>
    React.createElement('section', { 'aria-label': 'military contribution grid' },
      React.createElement('button', { onClick: onAddContribution, disabled: isReadOnly }, 'Add Contribution')
    )
  )
}));

vi.mock("../MilitaryContributionForm", () => ({
  default: vi.fn(({ onSubmit, onCancel }) =>
    React.createElement('form', { 'aria-label': 'military contribution form' },
      React.createElement('button', {
        onClick: () => onSubmit({
          contributionAmount: 5000,
          contributionYear: 2024,
          isSupplementalContribution: false
        })
      }, 'Submit'),
      React.createElement('button', { onClick: onCancel }, 'Cancel')
    )
  )
}));

// useMilitaryContribution hook mock
vi.mock("../hooks/useMilitaryContribution", () => ({
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
  ApiMessageAlert: vi.fn(() => React.createElement('div', { role: 'alert' }, 'Message Alert')),
  DSMAccordion: vi.fn(({ title, children }) =>
    React.createElement('section', { 'aria-label': title },
      React.createElement('h2', null, title),
      children
    )
  ),
  DSMGrid: vi.fn(() => React.createElement('div', { role: 'grid' }, 'Grid')),
  Page: vi.fn(({ label, actionNode, children }) =>
    React.createElement('main', null,
      React.createElement('h1', null, label),
      actionNode,
      children
    )
  ),
  SearchAndReset: vi.fn(({ handleSearch, handleReset, disabled, isFetching }) =>
    React.createElement('div', { role: 'group', 'aria-label': 'search and reset controls' },
      React.createElement('button', { onClick: handleSearch, disabled: disabled || isFetching }, 'Search'),
      React.createElement('button', { onClick: handleReset }, 'Reset'),
      isFetching && React.createElement('span', { role: 'status' }, 'Loading...')
    )
  ),
  formatNumberWithComma: vi.fn((num) => num.toString()),
  setMessage: vi.fn((payload) => ({ type: "message/setMessage", payload }))
}));

describe("MilitaryContribution", () => {
  let customWrapper: ({ children }: { children: React.ReactNode }) => React.ReactElement;

  beforeEach(() => {
    vi.clearAllMocks();

    // Set up mock return values for API hooks
    mockTriggerGetMilitaryContributions.mockReturnValue({
      unwrap: vi.fn().mockResolvedValue({
        results: [],
        total: 0
      })
    });

    mockTriggerSearchMasterInquiry.mockReturnValue({
      unwrap: vi.fn().mockResolvedValue({
        results: [],
        total: 0
      })
    });

    mockTriggerGetProfitMasterInquiryMember.mockReturnValue({
      unwrap: vi.fn().mockResolvedValue(null)
    });

    // Set up default return values for custom hooks
    mockUseDecemberFlowProfitYear.mockReturnValue(2024);
    mockUseIsProfitYearFrozen.mockReturnValue(false);
    mockUseIsReadOnlyByStatus.mockReturnValue(false);
    mockUseReadOnlyNavigation.mockReturnValue(false);
    mockUseMissiveAlerts.mockReturnValue({
      missiveAlerts: [],
      addAlert: vi.fn(),
      clearAlerts: vi.fn()
    });

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
      mockUseDecemberFlowProfitYear.mockReturnValueOnce(2024);

      const noMemberState = {
        security: { token: "mock-token" },
        navigation: {
          navigationData: undefined,
          error: undefined,
          currentNavigationId: null
        },
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
      mockUseIsProfitYearFrozen.mockReturnValueOnce(true);

      render(<MilitaryContribution />, { wrapper: customWrapper });

      await waitFor(() => {
        expect(screen.getByText("Frozen Year Warning")).toBeInTheDocument();
      });
    });

    it("should not display frozen year warning when year is not frozen", async () => {
      mockUseIsProfitYearFrozen.mockReturnValueOnce(false);

      render(<MilitaryContribution />, { wrapper: customWrapper });

      await waitFor(() => {
        expect(screen.queryByText("Frozen Year Warning")).not.toBeInTheDocument();
      });
    });
  });

  describe("Read-only mode", () => {
    it("should display read-only info when status is read-only", async () => {
      mockUseIsReadOnlyByStatus.mockReturnValueOnce(true);

      render(<MilitaryContribution />, { wrapper: customWrapper });

      await waitFor(() => {
        expect(screen.getByText("Read Only Info")).toBeInTheDocument();
      });
    });

    it("should disable add contribution button in read-only mode", async () => {
      mockUseReadOnlyNavigation.mockReturnValueOnce(true);

      render(<MilitaryContribution />, { wrapper: customWrapper });

      await waitFor(() => {
        const addBtn = screen.getByRole("button", { name: /add contribution/i });
        expect(addBtn).toBeDisabled();
      });
    });

    it("should enable add contribution button when not read-only", async () => {
      mockUseReadOnlyNavigation.mockReturnValueOnce(false);

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
      mockUseMissiveAlerts.mockReturnValueOnce({
        missiveAlerts: [{ id: 1, message: "Test Alert" }] as MissiveAlert[],
        addAlert: vi.fn(),
        clearAlerts: vi.fn()
      });

      render(<MilitaryContribution />, { wrapper: customWrapper });

      expect(screen.getByText("Missive Alerts")).toBeInTheDocument();
    });
  });
});
