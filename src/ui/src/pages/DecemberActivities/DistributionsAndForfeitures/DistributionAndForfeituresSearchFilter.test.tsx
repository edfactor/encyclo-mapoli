import { configureStore } from "@reduxjs/toolkit";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { Provider } from "react-redux";
import { beforeEach, describe, expect, it, vi } from "vitest";
import yearsEndReducer from "../../../reduxstore/slices/yearsEndSlice";
import DistributionAndForfeituresSearchFilter from "./DistributionAndForfeituresSearchFilter";

// Mock the API hooks
const mockTriggerSearch = vi.fn();
const mockStatesData = {
  results: [
    { state: "MA", stateName: "Massachusetts" },
    { state: "CT", stateName: "Connecticut" },
    { state: "NH", stateName: "New Hampshire" }
  ]
};
const mockTaxCodesData = {
  results: [
    { taxCodeId: "H", taxCodeName: "Regular" },
    { taxCodeId: "8", taxCodeName: "Overtime" },
    { taxCodeId: "9", taxCodeName: "Bonus" }
  ]
};

vi.mock("../../../reduxstore/api/YearsEndApi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("../../../reduxstore/api/YearsEndApi")>();
  return {
    ...actual,
    useLazyGetDistributionsAndForfeituresQuery: () => [mockTriggerSearch, { isFetching: false }]
  };
});

vi.mock("../../../reduxstore/api/LookupsApi", () => ({
  useGetStatesQuery: () => ({ data: mockStatesData, isLoading: false }),
  useGetTaxCodesQuery: () => ({ data: mockTaxCodesData, isLoading: false })
}));

// Mock the fiscal calendar hooks
vi.mock("../../../hooks/useFiscalCalendarYear", () => ({
  useFiscalCalendarYear: () => ({
    profitYear: 2025,
    fiscalStartDate: new Date(2025, 0, 1),
    fiscalEndDate: new Date(2025, 11, 31)
  })
}));

vi.mock("../../../hooks/useDecemberFlowProfitYear", () => ({
  default: () => 2025
}));

// Simple security reducer for testing
const securityReducer = (state = { token: "test-token" }, action: any) => state;

describe("DistributionAndForfeituresSearchFilter", () => {
  let store: any;
  const mockSetInitialSearchLoaded = vi.fn();

  beforeEach(() => {
    store = configureStore({
      reducer: {
        yearsEnd: yearsEndReducer,
        security: securityReducer
      },
      middleware: (getDefaultMiddleware) =>
        getDefaultMiddleware({
          serializableCheck: false
        })
    });
    mockTriggerSearch.mockClear();
    mockSetInitialSearchLoaded.mockClear();
  });

  const renderComponent = () => {
    return render(
      <Provider store={store}>
        <DistributionAndForfeituresSearchFilter setInitialSearchLoaded={mockSetInitialSearchLoaded} />
      </Provider>
    );
  };

  describe("Initial Render", () => {
    it("should render all filter fields", () => {
      renderComponent();

      expect(screen.getByLabelText(/Start Date/i)).toBeInTheDocument();
      expect(screen.getByLabelText(/End Date/i)).toBeInTheDocument();
      expect(screen.getByLabelText(/State/i)).toBeInTheDocument();
      expect(screen.getByLabelText(/Tax Code/i)).toBeInTheDocument();
      expect(screen.getByRole("button", { name: /Search/i })).toBeInTheDocument();
      expect(screen.getByRole("button", { name: /Reset/i })).toBeInTheDocument();
    });

    it("should initialize state dropdown with 'All' selected", () => {
      renderComponent();
      const stateDropdown = screen.getByLabelText(/State/i);
      expect(stateDropdown).toHaveTextContent("All");
    });

    it("should initialize tax code dropdown with 'All' selected", () => {
      renderComponent();
      const taxCodeDropdown = screen.getByLabelText(/Tax Code/i);
      expect(taxCodeDropdown).toHaveTextContent("All");
    });
  });

  describe("State Multi-Select", () => {
    it("should display 'All' when no states are selected", () => {
      renderComponent();
      const stateDropdown = screen.getByLabelText(/State/i);
      expect(stateDropdown).toHaveTextContent("All");
    });

    it("should allow selecting multiple states", async () => {
      const user = userEvent.setup();
      renderComponent();

      const stateDropdown = screen.getByLabelText(/State/i);
      await user.click(stateDropdown);

      // Wait for dropdown to open and select MA
      await waitFor(() => {
        const maOption = screen.getByRole("option", { name: /Massachusetts/i });
        expect(maOption).toBeInTheDocument();
      });

      const maOption = screen.getByRole("option", { name: /Massachusetts/i });
      await user.click(maOption);

      // Dropdown should now show "MA" instead of "All"
      await waitFor(() => {
        expect(stateDropdown).toHaveTextContent("MA");
      });
    });

    it("should display multiple selected states as comma-separated list", async () => {
      const user = userEvent.setup();
      renderComponent();

      const stateDropdown = screen.getByLabelText(/State/i);
      await user.click(stateDropdown);

      // Select MA
      await waitFor(() => {
        const maOption = screen.getByRole("option", { name: /Massachusetts/i });
        expect(maOption).toBeInTheDocument();
      });
      const maOption = screen.getByRole("option", { name: /Massachusetts/i });
      await user.click(maOption);

      // Re-open dropdown and select CT
      await user.click(stateDropdown);
      const ctOption = screen.getByRole("option", { name: /Connecticut/i });
      await user.click(ctOption);

      // Should show "MA, CT"
      await waitFor(() => {
        const displayText = stateDropdown.textContent;
        expect(displayText).toContain("MA");
        expect(displayText).toContain("CT");
      });
    });

    it("should clear selections when 'All' is clicked after selecting specific states", async () => {
      const user = userEvent.setup();
      renderComponent();

      const stateDropdown = screen.getByLabelText(/State/i);

      // Select MA
      await user.click(stateDropdown);
      const maOption = screen.getByRole("option", { name: /Massachusetts/i });
      await user.click(maOption);

      // Click "All" to clear
      await user.click(stateDropdown);
      const allOption = screen.getByRole("option", { name: "All" });
      await user.click(allOption);

      // Should show "All" again
      await waitFor(() => {
        expect(stateDropdown).toHaveTextContent("All");
      });
    });
  });

  describe("Tax Code Multi-Select", () => {
    it("should display 'All' when no tax codes are selected", () => {
      renderComponent();
      const taxCodeDropdown = screen.getByLabelText(/Tax Code/i);
      expect(taxCodeDropdown).toHaveTextContent("All");
    });

    it("should allow selecting multiple tax codes", async () => {
      const user = userEvent.setup();
      renderComponent();

      const taxCodeDropdown = screen.getByLabelText(/Tax Code/i);
      await user.click(taxCodeDropdown);

      // Wait for dropdown to open and select H
      await waitFor(() => {
        const hOption = screen.getByRole("option", { name: /H - Regular/i });
        expect(hOption).toBeInTheDocument();
      });

      const hOption = screen.getByRole("option", { name: /H - Regular/i });
      await user.click(hOption);

      // Dropdown should now show "H" instead of "All"
      await waitFor(() => {
        expect(taxCodeDropdown).toHaveTextContent("H");
      });
    });

    it("should display multiple selected tax codes as comma-separated list", async () => {
      const user = userEvent.setup();
      renderComponent();

      const taxCodeDropdown = screen.getByLabelText(/Tax Code/i);
      await user.click(taxCodeDropdown);

      // Select H
      await waitFor(() => {
        const hOption = screen.getByRole("option", { name: /H - Regular/i });
        expect(hOption).toBeInTheDocument();
      });
      const hOption = screen.getByRole("option", { name: /H - Regular/i });
      await user.click(hOption);

      // Re-open dropdown and select 8
      await user.click(taxCodeDropdown);
      const eightOption = screen.getByRole("option", { name: /8 - Overtime/i });
      await user.click(eightOption);

      // Should show "H, 8"
      await waitFor(() => {
        const displayText = taxCodeDropdown.textContent;
        expect(displayText).toContain("H");
        expect(displayText).toContain("8");
      });
    });
  });

  describe("Search Functionality", () => {
    it("should dispatch query params with empty arrays when 'All' is selected", async () => {
      const user = userEvent.setup();
      renderComponent();

      const searchButton = screen.getByRole("button", { name: /Search/i });
      await user.click(searchButton);

      await waitFor(() => {
        const state = store.getState().yearsEnd;
        expect(state.distributionsAndForfeituresQueryParams).toBeDefined();
        expect(state.distributionsAndForfeituresQueryParams.states).toEqual([]);
        expect(state.distributionsAndForfeituresQueryParams.taxCodes).toEqual([]);
      });
    });

    it("should dispatch query params with selected states array", async () => {
      const user = userEvent.setup();
      renderComponent();

      // Select MA and CT
      const stateDropdown = screen.getByLabelText(/State/i);
      await user.click(stateDropdown);

      await waitFor(() => {
        expect(screen.getByRole("option", { name: /Massachusetts/i })).toBeInTheDocument();
      });

      await user.click(screen.getByRole("option", { name: /Massachusetts/i }));
      await user.click(stateDropdown);
      await user.click(screen.getByRole("option", { name: /Connecticut/i }));

      const searchButton = screen.getByRole("button", { name: /Search/i });
      await user.click(searchButton);

      await waitFor(() => {
        const state = store.getState().yearsEnd;
        expect(state.distributionsAndForfeituresQueryParams.states).toEqual(expect.arrayContaining(["MA", "CT"]));
      });
    });

    it("should dispatch query params with selected tax codes array", async () => {
      const user = userEvent.setup();
      renderComponent();

      // Select H and 8
      const taxCodeDropdown = screen.getByLabelText(/Tax Code/i);
      await user.click(taxCodeDropdown);

      await waitFor(() => {
        expect(screen.getByRole("option", { name: /H - Regular/i })).toBeInTheDocument();
      });

      await user.click(screen.getByRole("option", { name: /H - Regular/i }));
      await user.click(taxCodeDropdown);
      await user.click(screen.getByRole("option", { name: /8 - Overtime/i }));

      const searchButton = screen.getByRole("button", { name: /Search/i });
      await user.click(searchButton);

      await waitFor(() => {
        const state = store.getState().yearsEnd;
        expect(state.distributionsAndForfeituresQueryParams.taxCodes).toEqual(expect.arrayContaining(["H", "8"]));
      });
    });

    it("should include date range in query params", async () => {
      const user = userEvent.setup();
      renderComponent();

      const searchButton = screen.getByRole("button", { name: /Search/i });
      await user.click(searchButton);

      await waitFor(() => {
        const state = store.getState().yearsEnd;
        expect(state.distributionsAndForfeituresQueryParams.startDate).toBeDefined();
        expect(state.distributionsAndForfeituresQueryParams.endDate).toBeDefined();
      });
    });
  });

  describe("Reset Functionality", () => {
    it("should reset all filters to default values", async () => {
      const user = userEvent.setup();
      renderComponent();

      // Select some filters first
      const stateDropdown = screen.getByLabelText(/State/i);
      await user.click(stateDropdown);
      await waitFor(() => {
        expect(screen.getByRole("option", { name: /Massachusetts/i })).toBeInTheDocument();
      });
      await user.click(screen.getByRole("option", { name: /Massachusetts/i }));

      // Click reset
      const resetButton = screen.getByRole("button", { name: /Reset/i });
      await user.click(resetButton);

      // Verify dropdowns show "All" again
      await waitFor(() => {
        expect(stateDropdown).toHaveTextContent("All");
      });

      const taxCodeDropdown = screen.getByLabelText(/Tax Code/i);
      expect(taxCodeDropdown).toHaveTextContent("All");
    });

    it("should clear Redux query params on reset", async () => {
      const user = userEvent.setup();
      renderComponent();

      // Search first to set query params
      const searchButton = screen.getByRole("button", { name: /Search/i });
      await user.click(searchButton);

      // Verify params are set
      await waitFor(() => {
        const state = store.getState().yearsEnd;
        expect(state.distributionsAndForfeituresQueryParams).toBeDefined();
      });

      // Click reset
      const resetButton = screen.getByRole("button", { name: /Reset/i });
      await user.click(resetButton);

      // Verify params are cleared
      await waitFor(() => {
        const state = store.getState().yearsEnd;
        expect(state.distributionsAndForfeituresQueryParams).toBeNull();
      });
    });
  });

  describe("onChange Logic", () => {
    it("should remove 'ALL' from array when specific state is selected", async () => {
      const user = userEvent.setup();
      renderComponent();

      const stateDropdown = screen.getByLabelText(/State/i);

      // Initial state should be "All" (empty array internally)
      expect(stateDropdown).toHaveTextContent("All");

      // Click to select a specific state
      await user.click(stateDropdown);
      await waitFor(() => {
        expect(screen.getByRole("option", { name: /Massachusetts/i })).toBeInTheDocument();
      });
      await user.click(screen.getByRole("option", { name: /Massachusetts/i }));

      // Should now show only "MA", not "All, MA"
      await waitFor(() => {
        const displayText = stateDropdown.textContent;
        expect(displayText).toBe("MA");
      });
    });

    it("should clear to empty array when 'All' is clicked with selections", async () => {
      const user = userEvent.setup();
      renderComponent();

      const stateDropdown = screen.getByLabelText(/State/i);

      // Select MA first
      await user.click(stateDropdown);
      await waitFor(() => {
        expect(screen.getByRole("option", { name: /Massachusetts/i })).toBeInTheDocument();
      });
      await user.click(screen.getByRole("option", { name: /Massachusetts/i }));

      // Verify MA is selected
      await waitFor(() => {
        expect(stateDropdown).toHaveTextContent("MA");
      });

      // Click "All" to clear
      await user.click(stateDropdown);
      const allOption = screen.getByRole("option", { name: "All" });
      await user.click(allOption);

      // Should show "All" again
      await waitFor(() => {
        expect(stateDropdown).toHaveTextContent("All");
      });
    });
  });
});
