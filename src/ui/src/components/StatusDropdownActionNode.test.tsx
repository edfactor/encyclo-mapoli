import { configureStore } from "@reduxjs/toolkit";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { Provider } from "react-redux";
import { beforeEach, describe, expect, it, vi } from "vitest";
import StatusDropdownActionNode from "./StatusDropdownActionNode";

// Mock the API hooks
const mockTriggerUpdate = vi.fn();
const mockTriggerGetNavigation = vi.fn();
const mockTriggerGetNavigationStatus = vi.fn();

vi.mock("../reduxstore/api/NavigationApi", () => ({
  useLazyGetNavigationQuery: () => [mockTriggerGetNavigation, {}]
}));

vi.mock("../reduxstore/api/NavigationStatusApi", () => ({
  useLazyGetNavigationStatusQuery: () => [
    mockTriggerGetNavigationStatus,
    {
      data: {
        navigationStatusList: [
          { id: 1, name: "Not Started" },
          { id: 2, name: "In Progress" },
          { id: 3, name: "Approved" },
          { id: 4, name: "Complete" }
        ]
      },
      isSuccess: true
    }
  ],
  useLazyUpdateNavigationStatusQuery: () => [mockTriggerUpdate, {}]
}));

// Mock StatusDropdown component
vi.mock("./StatusDropdown", () => ({
  default: vi.fn(({ onStatusChange, initialStatus }) => (
    <div data-testid="status-dropdown">
      <div data-testid="current-status">{initialStatus}</div>
      <button
        data-testid="change-to-1"
        onClick={() => onStatusChange("1")}>
        Not Started
      </button>
      <button
        data-testid="change-to-2"
        onClick={() => onStatusChange("2")}>
        In Progress
      </button>
      <button
        data-testid="change-to-4"
        onClick={() => onStatusChange("4")}>
        Complete
      </button>
    </div>
  ))
}));

describe("StatusDropdownActionNode", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    localStorage.setItem("navigationId", "123");
    mockTriggerUpdate.mockResolvedValue({ data: { isSuccessful: true } });
  });

  const createMockStore = (navigationObj = { id: 123, statusId: 1, isReadOnly: false }) => {
    return configureStore({
      reducer: {
        security: () => ({ token: "mock-token" }),
        navigation: () => ({
          navigationData: {
            navigation: [navigationObj]
          }
        })
      }
    });
  };

  const wrapper =
    (store: ReturnType<typeof configureStore>) =>
    ({ children }: { children: React.ReactNode }) => <Provider store={store}>{children}</Provider>;

  it("should render StatusDropdownActionNode", () => {
    const mockStore = createMockStore();
    render(<StatusDropdownActionNode />, { wrapper: wrapper(mockStore) });

    expect(screen.getByTestId("status-dropdown")).toBeInTheDocument();
  });

  it("should call onStatusChange callback when status changes", async () => {
    const mockStore = createMockStore();
    const onStatusChange = vi.fn();

    render(<StatusDropdownActionNode onStatusChange={onStatusChange} />, {
      wrapper: wrapper(mockStore)
    });

    const completeButton = screen.getByTestId("change-to-4");
    fireEvent.click(completeButton);

    await waitFor(() => {
      expect(onStatusChange).toHaveBeenCalledWith("4", "Complete");
    });
  });

  it("should update navigation status via API when status changes", async () => {
    const mockStore = createMockStore();
    render(<StatusDropdownActionNode />, { wrapper: wrapper(mockStore) });

    const inProgressButton = screen.getByTestId("change-to-2");
    fireEvent.click(inProgressButton);

    await waitFor(() => {
      expect(mockTriggerUpdate).toHaveBeenCalledWith({
        navigationId: 123,
        statusId: 2
      });
    });
  });

  it("should call triggerGetNavigation after successful status update", async () => {
    const mockStore = createMockStore();
    render(<StatusDropdownActionNode />, { wrapper: wrapper(mockStore) });

    const completeButton = screen.getByTestId("change-to-4");
    fireEvent.click(completeButton);

    await waitFor(() => {
      expect(mockTriggerGetNavigation).toHaveBeenCalledWith({ navigationId: undefined });
    });
  });

  it("should auto-change from Not Started to In Progress when onSearchClicked is provided", async () => {
    const mockStore = createMockStore({ id: 123, statusId: 1, isReadOnly: false });
    const onStatusChange = vi.fn();

    const { rerender } = render(<StatusDropdownActionNode onStatusChange={onStatusChange} />, {
      wrapper: wrapper(mockStore)
    });

    // Initially no auto-change
    expect(mockTriggerUpdate).not.toHaveBeenCalled();

    // Simulate search clicked by providing onSearchClicked callback
    rerender(
      <StatusDropdownActionNode
        onStatusChange={onStatusChange}
        onSearchClicked={() => {}}
      />
    );

    await waitFor(() => {
      expect(mockTriggerUpdate).toHaveBeenCalledWith({
        navigationId: 123,
        statusId: 2
      });
    });
  });

  it("should NOT auto-change if status is not Not Started", async () => {
    const mockStore = createMockStore({ id: 123, statusId: 2, isReadOnly: false });
    const onStatusChange = vi.fn();

    render(
      <StatusDropdownActionNode
        onStatusChange={onStatusChange}
        onSearchClicked={() => {}}
      />,
      {
        wrapper: wrapper(mockStore)
      }
    );

    // Should not trigger update because current status is already "In Progress" (2)
    await waitFor(
      () => {
        expect(mockTriggerUpdate).not.toHaveBeenCalled();
      },
      { timeout: 500 }
    );
  });

  it("should handle API failure gracefully", async () => {
    mockTriggerUpdate.mockResolvedValue({ data: { isSuccessful: false } });
    const mockStore = createMockStore();

    render(<StatusDropdownActionNode />, { wrapper: wrapper(mockStore) });

    const completeButton = screen.getByTestId("change-to-4");
    fireEvent.click(completeButton);

    await waitFor(() => {
      expect(mockTriggerUpdate).toHaveBeenCalled();
    });

    // Should not call triggerGetNavigation if update was not successful
    expect(mockTriggerGetNavigation).not.toHaveBeenCalled();
  });

  it("should pass correct status name to onStatusChange callback", async () => {
    const mockStore = createMockStore();
    const onStatusChange = vi.fn();

    render(<StatusDropdownActionNode onStatusChange={onStatusChange} />, {
      wrapper: wrapper(mockStore)
    });

    // Test each status transition
    const notStartedButton = screen.getByTestId("change-to-1");
    fireEvent.click(notStartedButton);
    await waitFor(() => {
      expect(onStatusChange).toHaveBeenCalledWith("1", "Not Started");
    });

    const inProgressButton = screen.getByTestId("change-to-2");
    fireEvent.click(inProgressButton);
    await waitFor(() => {
      expect(onStatusChange).toHaveBeenCalledWith("2", "In Progress");
    });

    const completeButton = screen.getByTestId("change-to-4");
    fireEvent.click(completeButton);
    await waitFor(() => {
      expect(onStatusChange).toHaveBeenCalledWith("4", "Complete");
    });
  });
});
