import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import AddForfeitureModal from "../AddForfeitureModal";

// Mock dependencies
vi.mock("hooks/useFiscalCloseProfitYear", () => ({
  default: () => 2024
}));

const mockUpdateForfeiture = vi.fn();

vi.mock("reduxstore/api/YearsEndApi", () => ({
  useUpdateForfeitureAdjustmentMutation: () => [mockUpdateForfeiture, { isLoading: false }]
}));

describe("AddForfeitureModal - Class Action Checkbox Behavior", () => {
  const mockOnClose = vi.fn();
  const mockOnSave = vi.fn();
  const mockSuggestedForfeitResponse = {
    badgeNumber: 12345,
    suggestedForfeitAmount: 1000,
    demographicId: 1
  };

  beforeEach(() => {
    vi.clearAllMocks();
    mockUpdateForfeiture.mockResolvedValue({ data: {} });
  });

  it("should enable Class Action checkbox when forfeiture amount is positive", () => {
    render(
      <AddForfeitureModal
        open={true}
        onClose={mockOnClose}
        onSave={mockOnSave}
        suggestedForfeitResponse={mockSuggestedForfeitResponse}
      />
    );

    const forfeitureAmountInput = screen.getByRole("spinbutton") as HTMLInputElement;
    const classActionCheckbox = screen.getByRole("checkbox", { name: /class action/i }) as HTMLInputElement;

    // Enter positive amount
    fireEvent.change(forfeitureAmountInput, { target: { value: "500" } });

    expect(classActionCheckbox.disabled).toBe(false);
  });

  it("should disable Class Action checkbox when forfeiture amount is negative", () => {
    render(
      <AddForfeitureModal
        open={true}
        onClose={mockOnClose}
        onSave={mockOnSave}
        suggestedForfeitResponse={mockSuggestedForfeitResponse}
      />
    );

    const forfeitureAmountInput = screen.getByRole("spinbutton") as HTMLInputElement;
    const classActionCheckbox = screen.getByRole("checkbox", { name: /class action/i }) as HTMLInputElement;

    // Checkbox should be enabled initially
    expect(classActionCheckbox.disabled).toBe(false);

    // Enter negative amount (unforfeit)
    fireEvent.change(forfeitureAmountInput, { target: { value: "-500" } });

    // Checkbox should now be disabled
    expect(classActionCheckbox.disabled).toBe(true);
  });

  it("should uncheck Class Action checkbox when changing from positive to negative amount", () => {
    render(
      <AddForfeitureModal
        open={true}
        onClose={mockOnClose}
        onSave={mockOnSave}
        suggestedForfeitResponse={mockSuggestedForfeitResponse}
      />
    );

    const forfeitureAmountInput = screen.getByRole("spinbutton") as HTMLInputElement;
    const classActionCheckbox = screen.getByRole("checkbox", { name: /class action/i }) as HTMLInputElement;

    // Check the checkbox with positive amount
    fireEvent.click(classActionCheckbox);
    expect(classActionCheckbox.checked).toBe(true);

    // Change to negative amount
    fireEvent.change(forfeitureAmountInput, { target: { value: "-500" } });

    // Checkbox should be unchecked and disabled
    expect(classActionCheckbox.checked).toBe(false);
    expect(classActionCheckbox.disabled).toBe(true);
  });

  it("should re-enable Class Action checkbox when changing from negative back to positive", () => {
    render(
      <AddForfeitureModal
        open={true}
        onClose={mockOnClose}
        onSave={mockOnSave}
        suggestedForfeitResponse={mockSuggestedForfeitResponse}
      />
    );

    const forfeitureAmountInput = screen.getByRole("spinbutton") as HTMLInputElement;
    const classActionCheckbox = screen.getByRole("checkbox", { name: /class action/i }) as HTMLInputElement;

    // Start with negative amount
    fireEvent.change(forfeitureAmountInput, { target: { value: "-500" } });
    expect(classActionCheckbox.disabled).toBe(true);

    // Change back to positive
    fireEvent.change(forfeitureAmountInput, { target: { value: "300" } });
    expect(classActionCheckbox.disabled).toBe(false);
  });

  it("should enable Class Action checkbox when amount is zero", () => {
    render(
      <AddForfeitureModal
        open={true}
        onClose={mockOnClose}
        onSave={mockOnSave}
        suggestedForfeitResponse={mockSuggestedForfeitResponse}
      />
    );

    const forfeitureAmountInput = screen.getByRole("spinbutton") as HTMLInputElement;
    const classActionCheckbox = screen.getByRole("checkbox", { name: /class action/i }) as HTMLInputElement;

    // Set to zero
    fireEvent.change(forfeitureAmountInput, { target: { value: "0" } });

    expect(classActionCheckbox.disabled).toBe(false);
  });
});
