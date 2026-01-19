import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { useCachedPrevious } from "../useCachedPrevious";

type Value = { text: string } | null;

function TestComponent({ value }: { value: Value }) {
  const display = useCachedPrevious<Value>(value);
  if (!display) return <div data-testid="empty">empty</div>;
  return <div>{display.text}</div>;
}

describe("useCachedPrevious", () => {
  it("returns null on initial null value", () => {
    render(<TestComponent value={null} />);
    expect(screen.getByTestId("empty")).toBeInTheDocument();
  });

  it("caches previous value while current value is null and updates when new value arrives", () => {
    const { rerender } = render(<TestComponent value={{ text: "page1" }} />);
    expect(screen.getByText("page1")).toBeInTheDocument();

    // Simulate loading where current value becomes null
    rerender(<TestComponent value={null} />);
    expect(screen.getByText("page1")).toBeInTheDocument();

    // New data arrives
    rerender(<TestComponent value={{ text: "page2" }} />);
    expect(screen.getByText("page2")).toBeInTheDocument();
  });
});
