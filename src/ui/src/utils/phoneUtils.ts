/**
 * Formats a phone number to US format (XXX) XXX-XXXX
 * @param phone - Raw phone number (e.g., "5081234567")
 * @returns Formatted phone number or original if invalid
 */
export const formatPhoneNumber = (phone: string | null | undefined): string => {
  if (!phone) return "N/A";

  // Remove all non-digit characters
  const cleaned = phone.replace(/\D/g, "");

  // Check if we have a valid 10-digit US phone number
  if (cleaned.length === 10) {
    return `(${cleaned.slice(0, 3)}) ${cleaned.slice(3, 6)}-${cleaned.slice(6)}`;
  }

  // If not 10 digits, return original or N/A
  return phone || "N/A";
};
