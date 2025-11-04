/**
 * React Hook Form Mock Factory
 *
 * Creates properly-structured React Hook Form mocks for testing components
 * that use useForm, Controller, useFormContext, etc.
 *
 * This is essential for testing forms without needing to mock react-hook-form
 * in every test file individually.
 *
 * Usage:
 *   // In test file
 *   vi.mock("react-hook-form", () => createReactHookFormMock());
 *
 *   // Or for global setup (in setup.ts):
 *   vi.mock("react-hook-form", () => createReactHookFormMock());
 */

import { vi } from "vitest";
import React from "react";

/**
 * Creates a complete mock of react-hook-form
 *
 * Provides implementations for all commonly-used hooks and components:
 * - useForm: Returns form methods and state
 * - Controller: Renders form fields with proper field props
 * - useFormContext: Provides access to form context
 * - useController: Hook version of Controller
 *
 * @param options - Configuration options for the mock
 * @returns Mock implementation of react-hook-form
 *
 * @example
 * // Basic usage - mock globally
 * vi.mock("react-hook-form", () => createReactHookFormMock());
 *
 * @example
 * // With default values
 * vi.mock("react-hook-form", () => createReactHookFormMock({
 *   defaultValues: { email: "test@example.com", age: 25 }
 * }));
 *
 * @example
 * // With validation errors
 * vi.mock("react-hook-form", () => createReactHookFormMock({
 *   errors: { email: { type: "required", message: "Email is required" } }
 * }));
 */
export const createReactHookFormMock = (options?: {
  defaultValues?: Record<string, unknown>;
  errors?: Record<string, { type?: string; message?: string }>;
  isValid?: boolean;
  isDirty?: boolean;
  isSubmitting?: boolean;
}) => {
  const defaultValues = options?.defaultValues ?? {};
  const errors = options?.errors ?? {};

  return {
    /**
     * Mock useForm hook
     *
     * Returns all methods and state that components expect from useForm
     */
    useForm: vi.fn((config?: { defaultValues?: Record<string, unknown> }) => {
      const formDefaultValues = config?.defaultValues ?? defaultValues;

      return {
        // Form control object (required by Controller)
        control: {},

        // Form submission handler
        handleSubmit: vi.fn((fn) => async (e?: React.BaseSyntheticEvent) => {
          e?.preventDefault?.();
          // Allow errors to propagate for test handling
          await fn(formDefaultValues);
        }),

        // Form state
        formState: {
          errors: errors,
          isValid: options?.isValid ?? Object.keys(errors).length === 0,
          isDirty: options?.isDirty ?? false,
          isSubmitting: options?.isSubmitting ?? false,
          isValidating: false,
          isSubmitted: false,
          isSubmitSuccessful: false,
          submitCount: 0,
          touchedFields: {},
          dirtyFields: {}
        },

        // Watch form values
        watch: vi.fn((name?: string | string[]) => {
          if (!name) return formDefaultValues;
          if (Array.isArray(name)) {
            return name.map((n) => formDefaultValues[n]);
          }
          return formDefaultValues[name];
        }),

        // Set form value
        setValue: vi.fn(),

        // Reset form
        reset: vi.fn((values?: Record<string, unknown>) => {
          if (values) {
            Object.assign(formDefaultValues, values);
          }
        }),

        // Get form values
        getValues: vi.fn((name?: string | string[]) => {
          if (!name) return formDefaultValues;
          if (Array.isArray(name)) {
            return name.map((n) => formDefaultValues[n]);
          }
          return formDefaultValues[name];
        }),

        // Trigger validation
        trigger: vi.fn(() => Promise.resolve(options?.isValid ?? true)),

        // Set error
        setError: vi.fn(),

        // Clear errors
        clearErrors: vi.fn(),

        // Set focus
        setFocus: vi.fn(),

        // Register field
        register: vi.fn((name: string) => ({
          name,
          onChange: vi.fn(),
          onBlur: vi.fn(),
          ref: vi.fn()
        })),

        // Unregister field
        unregister: vi.fn()
      };
    }),

    /**
     * Mock Controller component
     *
     * Renders the field using the render prop with properly-structured field props
     * This allows controlled components like MUI inputs to work correctly in tests
     */
    Controller: vi.fn(({ render, name, defaultValue }) => {
      const value = defaultValues[name] ?? defaultValue ?? "";
      const error = errors[name];

      // Call the render function with proper field props
      const element = render({
        field: {
          onChange: vi.fn(),
          onBlur: vi.fn(),
          value: value,
          name: name,
          ref: vi.fn()
        },
        fieldState: {
          invalid: !!error,
          error: error,
          isDirty: options?.isDirty ?? false,
          isTouched: false
        },
        formState: {
          errors: errors,
          isValid: options?.isValid ?? Object.keys(errors).length === 0,
          isDirty: options?.isDirty ?? false,
          isSubmitting: options?.isSubmitting ?? false,
          isValidating: false,
          isSubmitted: false,
          isSubmitSuccessful: false,
          submitCount: 0,
          touchedFields: {},
          dirtyFields: {}
        }
      });

      return element;
    }),

    /**
     * Mock useFormContext hook
     *
     * Used by components that need access to form context without having
     * useForm directly (e.g., deeply nested form components)
     */
    useFormContext: vi.fn(() => ({
      control: {},
      formState: {
        errors: errors,
        isValid: options?.isValid ?? Object.keys(errors).length === 0
      },
      watch: vi.fn((name?: string) => {
        if (!name) return defaultValues;
        return defaultValues[name];
      }),
      setValue: vi.fn(),
      getValues: vi.fn((name?: string) => {
        if (!name) return defaultValues;
        return defaultValues[name];
      }),
      trigger: vi.fn(() => Promise.resolve(true)),
      register: vi.fn(() => ({
        name: "",
        onChange: vi.fn(),
        onBlur: vi.fn(),
        ref: vi.fn()
      }))
    })),

    /**
     * Mock useController hook
     *
     * Hook version of Controller for creating custom controlled inputs
     */
    useController: vi.fn(({ name, defaultValue }) => {
      const value = defaultValues[name] ?? defaultValue ?? "";
      const error = errors[name];

      return {
        field: {
          onChange: vi.fn(),
          onBlur: vi.fn(),
          value: value,
          name: name,
          ref: vi.fn()
        },
        fieldState: {
          invalid: !!error,
          error: error,
          isDirty: options?.isDirty ?? false,
          isTouched: false
        },
        formState: {
          errors: errors,
          isValid: options?.isValid ?? Object.keys(errors).length === 0
        }
      };
    }),

    /**
     * Mock FormProvider component
     *
     * Used to provide form context to child components
     */
    FormProvider: vi.fn(({ children }) => {
      return React.createElement(React.Fragment, null, children);
    }),

    /**
     * Mock useWatch hook
     *
     * Watches form values and re-renders on change
     */
    useWatch: vi.fn(({ name, defaultValue }) => {
      if (name && typeof name === "string") {
        return defaultValues[name] ?? defaultValue;
      }
      return defaultValue;
    }),

    /**
     * Mock useFieldArray hook
     *
     * For managing dynamic form fields (arrays)
     */
    useFieldArray: vi.fn(() => ({
      fields: [],
      append: vi.fn(),
      prepend: vi.fn(),
      remove: vi.fn(),
      insert: vi.fn(),
      update: vi.fn(),
      replace: vi.fn(),
      move: vi.fn(),
      swap: vi.fn()
    }))
  };
};

/**
 * Type-safe builder for creating customized React Hook Form mocks
 *
 * Useful for tests that need specific form state (errors, dirty state, etc.)
 *
 * @example
 * const formMock = new ReactHookFormMockBuilder()
 *   .withDefaultValues({ email: "test@test.com" })
 *   .withErrors({ email: { message: "Invalid email" } })
 *   .withValidState(false)
 *   .build();
 *
 * vi.mock("react-hook-form", () => formMock);
 */
export class ReactHookFormMockBuilder {
  private defaultValues: Record<string, unknown> = {};
  private errors: Record<string, { type?: string; message?: string }> = {};
  private isValid: boolean = true;
  private isDirty: boolean = false;
  private isSubmitting: boolean = false;

  withDefaultValues(values: Record<string, unknown>): this {
    this.defaultValues = values;
    return this;
  }

  withErrors(errors: Record<string, { type?: string; message?: string }>): this {
    this.errors = errors;
    return this;
  }

  withValidState(isValid: boolean): this {
    this.isValid = isValid;
    return this;
  }

  withDirtyState(isDirty: boolean): this {
    this.isDirty = isDirty;
    return this;
  }

  withSubmittingState(isSubmitting: boolean): this {
    this.isSubmitting = isSubmitting;
    return this;
  }

  build() {
    return createReactHookFormMock({
      defaultValues: this.defaultValues,
      errors: this.errors,
      isValid: this.isValid,
      isDirty: this.isDirty,
      isSubmitting: this.isSubmitting
    });
  }
}
