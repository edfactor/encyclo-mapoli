/**
 * RTK Query Mock Factory
 *
 * Creates properly-structured RTK Query mock returns that support .unwrap() chaining.
 * This is essential for components that use async patterns like:
 *
 *   const result = await trigger(params).unwrap();
 *
 * Usage:
 *   vi.mocked(useLazyGetSomeQuery).mockReturnValue(
 *     createRTKQueryLazyMock({ results: [...], total: 10 })
 *   );
 */

/**
 * Creates a mock return value for RTK Query lazy hooks
 *
 * Returns a tuple of [trigger, metadata] that matches RTK Query's return type
 * The trigger function supports .unwrap() chaining for error handling
 *
 * @param successData - The data to return on success
 * @param options - Configuration options
 * @returns Tuple of [triggerFn, metadata] matching RTK Query return type
 *
 * @example
 * // Basic usage
 * vi.mocked(useLazyGetData).mockReturnValue(
 *   createRTKQueryLazyMock({ results: mockData, total: 100 })
 * );
 *
 * // With error simulation
 * vi.mocked(useLazyGetData).mockReturnValue(
 *   createRTKQueryLazyMock(null, {
 *     shouldFail: true,
 *     error: new Error("API Error")
 *   })
 * );
 *
 * // With delay (simulate network latency)
 * vi.mocked(useLazyGetData).mockReturnValue(
 *   createRTKQueryLazyMock({ results: mockData }, { delay: 100 })
 * );
 */
export const createRTKQueryLazyMock = <T, E = Error>(
  successData: T,
  options?: {
    shouldFail?: boolean;
    error?: E;
    delay?: number;
  }
) => {
  const trigger = vi.fn(async (..._args: unknown[]) => {
    // Simulate network delay if specified
    if (options?.delay) {
      await new Promise((resolve) => setTimeout(resolve, options.delay));
    }

    const result = {
      data: successData,
      isLoading: false,
      isFetching: false,
      isSuccess: !options?.shouldFail,
      isError: options?.shouldFail ?? false,
      error: options?.error ?? null,
      status: options?.shouldFail ? "rejected" : "fulfilled" as const
    };

    return {
      ...result,
      // RTK Query thunk actions have .unwrap() method for error handling
      // This allows components to do: const data = await trigger(...).unwrap();
      unwrap: async () => {
        if (options?.shouldFail) {
          throw options.error ?? new Error("RTK Query Error");
        }
        return successData;
      }
    };
  });

  return [trigger, { isFetching: false, isLoading: false, status: "uninitialized" }] as const;
};

/**
 * Creates a mock return value for RTK Query eager (non-lazy) hooks
 *
 * These hooks are called automatically and don't need the trigger function
 *
 * @param data - The data to return
 * @param options - Additional query state (loading, error, etc.)
 * @returns Query state object matching RTK Query query hook return type
 *
 * @example
 * vi.mocked(useGetStatesQuery).mockReturnValue(
 *   createRTKQueryMock([{ code: "MA", name: "Massachusetts" }])
 * );
 */
export const createRTKQueryMock = <T, E = Error>(
  data: T,
  options?: {
    isLoading?: boolean;
    isError?: boolean;
    error?: E;
    isSuccess?: boolean;
  }
) => {
  return {
    data,
    isLoading: options?.isLoading ?? false,
    isFetching: false,
    isError: options?.isError ?? false,
    isSuccess: options?.isSuccess ?? true,
    error: options?.error ?? null,
    status: options?.isError ? "rejected" : "fulfilled" as const,
    refetch: vi.fn()
  };
};

/**
 * Creates a mock mutation function that supports .unwrap()
 *
 * Used for RTK Query mutations like createDistribution, updateEmployee, etc.
 *
 * @param successData - Data returned on success
 * @param options - Configuration options
 * @returns Tuple of [mutationFn, metadata] matching RTK Query mutation return type
 *
 * @example
 * vi.mocked(useCreateDistributionMutation).mockReturnValue([
 *   createRTKQueryMutationMock({ id: 123 }).fn,
 *   { isLoading: false }
 * ]);
 */
export const createRTKQueryMutationMock = <T, E = Error>(
  successData: T,
  options?: {
    shouldFail?: boolean;
    error?: E;
    delay?: number;
  }
) => {
  const mutationFn = vi.fn(async (..._args: unknown[]) => {
    if (options?.delay) {
      await new Promise((resolve) => setTimeout(resolve, options.delay));
    }

    if (options?.shouldFail) {
      throw options.error ?? new Error("RTK Mutation Error");
    }

    return {
      data: successData,
      isLoading: false,
      isSuccess: true,
      isError: false,
      unwrap: async () => successData
    };
  });

  return [
    mutationFn,
    { isLoading: false, isSuccess: false, isError: false }
  ] as const;
};

/**
 * Type-safe wrapper for creating typed RTK Query mocks
 *
 * Helps maintain type safety when mocking complex data structures
 */
export class RTKQueryMockBuilder<T> {
  private data: T;
  private shouldFail: boolean = false;
  private error?: Error;
  private delay: number = 0;

  constructor(data: T) {
    this.data = data;
  }

  withError(error: Error): this {
    this.shouldFail = true;
    this.error = error;
    return this;
  }

  withDelay(ms: number): this {
    this.delay = ms;
    return this;
  }

  buildLazy() {
    return createRTKQueryLazyMock(this.data, {
      shouldFail: this.shouldFail,
      error: this.error,
      delay: this.delay
    });
  }

  buildEager() {
    return createRTKQueryMock(this.data, {
      isError: this.shouldFail,
      error: this.error
    });
  }

  buildMutation() {
    return createRTKQueryMutationMock(this.data, {
      shouldFail: this.shouldFail,
      error: this.error,
      delay: this.delay
    });
  }
}

// Import vi for use in module
import { vi } from "vitest";
