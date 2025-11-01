/**
 * Async Testing Helpers
 *
 * Utilities for coordinating async operations, user interactions, and state changes
 * in tests. These helpers handle common patterns like:
 *   - Clicking and waiting for results
 *   - Filling forms and validating
 *   - Waiting for loading states
 *   - Coordinating multiple async operations
 *
 * Usage:
 *   await clickAndWait("search-btn");
 *   await fillField("email-input", "test@example.com");
 *   await waitForText("Success!");
 */

import { waitFor, screen, ByRoleOptions } from "@testing-library/react";
import { act } from "@testing-library/react";
import userEvent from "@testing-library/user-event";

/**
 * Wait options with sensible defaults
 */
const defaultWaitOptions = { timeout: 2000 };

/**
 * Clicks an element by test ID and waits for async completion
 *
 * Ensures that any async operations triggered by the click complete
 * before assertions run, preventing race conditions
 *
 * @param testId - Test ID of element to click
 * @param options - Wait options
 *
 * @example
 * await clickAndWait("search-btn");
 * expect(screen.getByTestId("results")).toBeInTheDocument();
 */
export const clickAndWait = async (
  testId: string,
  options?: { timeout?: number }
) => {
  const user = userEvent.setup();
  const element = screen.getByTestId(testId);

  await act(async () => {
    await user.click(element);
  });

  // Wait for element to be enabled again (indicates async completion)
  await waitFor(
    () => {
      expect(element).toBeInTheDocument();
    },
    { timeout: options?.timeout ?? defaultWaitOptions.timeout }
  );
};

/**
 * Clicks a button by role and waits for completion
 *
 * Useful when test IDs aren't available
 *
 * @param buttonName - Button text or role name
 * @param options - Wait options
 *
 * @example
 * await clickButtonAndWait("Search");
 * await clickButtonAndWait("Delete", { timeout: 3000 });
 */
export const clickButtonAndWait = async (
  buttonName: string,
  options?: { timeout?: number }
) => {
  const user = userEvent.setup();
  const button = screen.getByRole("button", { name: new RegExp(buttonName, "i") });

  await act(async () => {
    await user.click(button);
  });

  await waitFor(
    () => {
      expect(button).toBeInTheDocument();
    },
    { timeout: options?.timeout ?? defaultWaitOptions.timeout }
  );
};

/**
 * Fills a text input field and waits for change event
 *
 * @param testId - Test ID of input element
 * @param value - Value to type
 * @param options - Wait options
 *
 * @example
 * await fillField("search-input", "John Doe");
 * await fillField("email-input", "test@example.com");
 */
export const fillField = async (
  testId: string,
  value: string,
  options?: { timeout?: number }
) => {
  const user = userEvent.setup();
  const field = screen.getByTestId(testId) as HTMLInputElement;

  await act(async () => {
    await user.clear(field);
    await user.type(field, value);
  });

  // Wait for value to be set
  await waitFor(
    () => {
      expect(field.value).toBe(value);
    },
    { timeout: options?.timeout ?? defaultWaitOptions.timeout }
  );
};

/**
 * Fills a field by placeholder text
 *
 * Useful when test IDs aren't available
 *
 * @param placeholder - Placeholder text
 * @param value - Value to type
 * @param options - Wait options
 *
 * @example
 * await fillFieldByPlaceholder("Enter SSN", "123456789");
 */
export const fillFieldByPlaceholder = async (
  placeholder: string,
  value: string,
  options?: { timeout?: number }
) => {
  const user = userEvent.setup();
  const field = screen.getByPlaceholderText(
    new RegExp(placeholder, "i")
  ) as HTMLInputElement;

  await act(async () => {
    await user.clear(field);
    await user.type(field, value);
  });

  await waitFor(
    () => {
      expect(field.value).toBe(value);
    },
    { timeout: options?.timeout ?? defaultWaitOptions.timeout }
  );
};

/**
 * Selects a value from a dropdown/select element
 *
 * @param testId - Test ID of select element
 * @param value - Value to select
 * @param options - Wait options
 *
 * @example
 * await selectOption("state-select", "MA");
 */
export const selectOption = async (
  testId: string,
  value: string,
  options?: { timeout?: number }
) => {
  const user = userEvent.setup();
  const select = screen.getByTestId(testId) as HTMLSelectElement;

  await act(async () => {
    await user.selectOptions(select, value);
  });

  await waitFor(
    () => {
      expect(select.value).toBe(value);
    },
    { timeout: options?.timeout ?? defaultWaitOptions.timeout }
  );
};

/**
 * Waits for text to appear in the document
 *
 * @param text - Text to search for (regex supported)
 * @param options - Wait options
 *
 * @example
 * await waitForText("Success!");
 * await waitForText(/Results for John/i);
 * await waitForText("Loading", { timeout: 3000 });
 */
export const waitForText = async (
  text: string | RegExp,
  options?: { timeout?: number }
) => {
  const pattern = typeof text === "string" ? new RegExp(text, "i") : text;

  await waitFor(
    () => {
      expect(screen.getByText(pattern)).toBeInTheDocument();
    },
    { timeout: options?.timeout ?? defaultWaitOptions.timeout }
  );
};

/**
 * Waits for text to disappear from the document
 *
 * Useful for waiting for loading states to complete
 *
 * @param text - Text to wait for disappearance
 * @param options - Wait options
 *
 * @example
 * await waitForTextToDisappear("Loading...");
 */
export const waitForTextToDisappear = async (
  text: string | RegExp,
  options?: { timeout?: number }
) => {
  const pattern = typeof text === "string" ? new RegExp(text, "i") : text;

  await waitFor(
    () => {
      expect(screen.queryByText(pattern)).not.toBeInTheDocument();
    },
    { timeout: options?.timeout ?? defaultWaitOptions.timeout }
  );
};

/**
 * Waits for element with test ID to be in the document
 *
 * @param testId - Test ID to wait for
 * @param options - Wait options
 *
 * @example
 * await waitForElement("results-grid");
 */
export const waitForElement = async (
  testId: string,
  options?: { timeout?: number }
) => {
  await waitFor(
    () => {
      expect(screen.getByTestId(testId)).toBeInTheDocument();
    },
    { timeout: options?.timeout ?? defaultWaitOptions.timeout }
  );
};

/**
 * Waits for element with test ID to disappear
 *
 * Useful for waiting for loading indicators, modals, etc.
 *
 * @param testId - Test ID to wait for disappearance
 * @param options - Wait options
 *
 * @example
 * await waitForElementToDisappear("loading-spinner");
 */
export const waitForElementToDisappear = async (
  testId: string,
  options?: { timeout?: number }
) => {
  await waitFor(
    () => {
      expect(screen.queryByTestId(testId)).not.toBeInTheDocument();
    },
    { timeout: options?.timeout ?? defaultWaitOptions.timeout }
  );
};

/**
 * Simulates a search workflow: click search and wait for results
 *
 * Waits for loading indicator to appear, then disappear
 * Assumes search button has test ID "search-btn" and loading has "loading"
 *
 * @param options - Configuration
 *
 * @example
 * await triggerSearchAndWait();
 * expect(screen.getByTestId("results-grid")).toBeInTheDocument();
 */
export const triggerSearchAndWait = async (options?: {
  searchButtonTestId?: string;
  loadingTestId?: string;
  timeout?: number;
}) => {
  const searchBtnId = options?.searchButtonTestId ?? "search-btn";
  const loadingId = options?.loadingTestId ?? "loading";
  const timeout = options?.timeout ?? defaultWaitOptions.timeout;

  // Click search button
  await clickAndWait(searchBtnId, { timeout });

  // Wait for loading to appear
  try {
    await waitForElement(loadingId, { timeout: 500 });
  } catch {
    // Loading might be too fast to catch, that's OK
  }

  // Wait for loading to disappear
  await waitForElementToDisappear(loadingId, { timeout });
};

/**
 * Simulates a form submission workflow
 *
 * Fills all specified fields, then clicks submit
 *
 * @param fields - Map of test IDs to values
 * @param submitButtonTestId - Test ID of submit button
 * @param options - Wait options
 *
 * @example
 * await submitForm(
 *   {
 *     "email-input": "test@example.com",
 *     "password-input": "password123"
 *   },
 *   "submit-btn"
 * );
 */
export const submitForm = async (
  fields: Record<string, string>,
  submitButtonTestId: string,
  options?: { timeout?: number }
) => {
  // Fill all fields
  for (const [testId, value] of Object.entries(fields)) {
    await fillField(testId, value, options);
  }

  // Submit form
  await clickAndWait(submitButtonTestId, options);
};

/**
 * Gets all text content from an element
 *
 * Useful for assertions on complex content
 *
 * @param testId - Test ID of element
 * @returns Element text content
 *
 * @example
 * const gridContent = getElementText("results-grid");
 * expect(gridContent).toContain("John Doe");
 */
export const getElementText = (testId: string): string => {
  const element = screen.getByTestId(testId);
  return element.textContent ?? "";
};

/**
 * Waits for a specific element to become enabled
 *
 * Useful for waiting for async operations to complete
 *
 * @param testId - Test ID of button/input
 * @param options - Wait options
 *
 * @example
 * await waitForElementToBeEnabled("submit-btn");
 */
export const waitForElementToBeEnabled = async (
  testId: string,
  options?: { timeout?: number }
) => {
  await waitFor(
    () => {
      const element = screen.getByTestId(testId) as HTMLButtonElement;
      expect(element).toBeEnabled();
    },
    { timeout: options?.timeout ?? defaultWaitOptions.timeout }
  );
};

/**
 * Waits for a specific element to become disabled
 *
 * Useful for waiting for async operations to start
 *
 * @param testId - Test ID of button/input
 * @param options - Wait options
 *
 * @example
 * await waitForElementToBeDisabled("submit-btn");
 */
export const waitForElementToBeDisabled = async (
  testId: string,
  options?: { timeout?: number }
) => {
  await waitFor(
    () => {
      const element = screen.getByTestId(testId) as HTMLButtonElement;
      expect(element).toBeDisabled();
    },
    { timeout: options?.timeout ?? defaultWaitOptions.timeout }
  );
};

/**
 * Waits for a role element and returns it
 *
 * Useful for working with accessible components
 *
 * @param role - ARIA role
 * @param options - Role options + wait options
 * @returns The element
 *
 * @example
 * const button = await waitForRole("button", { name: /Submit/i });
 */
export const waitForRole = async (
  role: string,
  options?: ByRoleOptions & { timeout?: number }
) => {
  const { timeout = defaultWaitOptions.timeout, ...roleOptions } = options ?? {};

  let element: HTMLElement | null = null;

  await waitFor(
    () => {
      element = screen.getByRole(
        role as Parameters<typeof screen.getByRole>[0],
        roleOptions
      );
      expect(element).toBeInTheDocument();
    },
    { timeout }
  );

  return element!;
};

/**
 * Waits for async state updates to complete
 *
 * Useful after triggering async operations
 *
 * @param callback - Assertion to verify state completion
 * @param options - Wait options
 *
 * @example
 * await waitForAsync(() => {
 *   expect(store.getState().data).toEqual(expectedData);
 * });
 */
export const waitForAsync = async (
  callback: () => void | Promise<void>,
  options?: { timeout?: number }
) => {
  await waitFor(callback, {
    timeout: options?.timeout ?? defaultWaitOptions.timeout
  });
};

/**
 * Pauses execution for a specified duration
 *
 * Use sparingly - prefer waitFor/waitForElement
 * Only for cases where timing is critical
 *
 * @param ms - Milliseconds to wait
 *
 * @example
 * await pause(500); // Wait 500ms for animation
 */
export const pause = (ms: number): Promise<void> => {
  return new Promise((resolve) => setTimeout(resolve, ms));
};
