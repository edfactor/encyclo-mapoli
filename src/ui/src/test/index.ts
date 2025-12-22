/**
 * Test Utilities Index
 *
 * Centralized export of all testing utilities and mock factories
 *
 * Usage:
 *   import {
 *     createMockStore,
 *     createRTKQueryLazyMock,
 *     clickAndWait,
 *     waitForText
 *   } from "../../test";
 */

// Mock factories
export {
  createRTKQueryLazyMock,
  createRTKQueryMock,
  createRTKQueryMutationMock,
  RTKQueryMockBuilder
} from "./mocks/rtkQueryMockFactory";

export { createReactHookFormMock, ReactHookFormMockBuilder } from "./mocks/reactHookFormMock";

// Redux store utilities
export {
  createMockStore,
  createProviderWrapper,
  createMockStoreAndWrapper,
  createTestWrapper,
  createSelectorMock
} from "./mocks/createMockStore";

// Async helpers
export {
  clickAndWait,
  clickButtonAndWait,
  fillField,
  fillFieldByPlaceholder,
  selectOption,
  waitForText,
  waitForTextToDisappear,
  waitForElement,
  waitForElementToDisappear,
  triggerSearchAndWait,
  submitForm,
  getElementText,
  waitForElementToBeEnabled,
  waitForElementToBeDisabled,
  waitForRole,
  waitForAsync,
  pause
} from "./utils/asyncHelpers";
