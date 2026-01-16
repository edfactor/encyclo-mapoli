import { useEffect, useRef } from "react";

/**
 * Cache the last non-null value and return current value or the cached one.
 * Keeps UI stable when the current value temporarily becomes null while loading
 */
export function useCachedPrevious<T>(value: T | null | undefined): T | null | undefined {
  const lastRef = useRef<T | null | undefined>(null);

  useEffect(() => {
    if (value !== null && value !== undefined) {
      lastRef.current = value;
    }
  }, [value]);

  return value ?? lastRef.current;
}
