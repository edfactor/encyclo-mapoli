export const validateSuggestedForfeit = (value: number, maxForfeit: number): string | null => {
  if (value != 0 && value > maxForfeit * -1) {
    return `Suggested unforfeiture or forfeiture amount ${value} cannot exceed ${new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(maxForfeit * -1)}`;
  }
  return null;
}; 