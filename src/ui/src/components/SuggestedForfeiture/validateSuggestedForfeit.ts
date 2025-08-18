export const validateSuggestedForfeit = (value: number, maxForfeit: number): string | null => {
  if (value !== 0 && value !== maxForfeit) {
    return `Entered unforfeiture or forfeiture amount ${value} does not match suggested ${new Intl.NumberFormat("en-US", { style: "currency", currency: "USD" }).format(maxForfeit)}`;
  }
  if (value === 0) {
    return null;
  }
  return null;
};
