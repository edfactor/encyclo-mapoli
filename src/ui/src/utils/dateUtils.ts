import { format, isValid, parse, parseISO, startOfDay } from "date-fns";

export const DATE_TIME_FORMAT_MMDDYYYY_HHMMSS = "MM/dd/yyyy HH:mm:ss";
export const DATE_FORMAT_YYYYMMDD = "yyyy-MM-dd";

//This is all over the UI, not the API
export const dateMMDDYYYY = (date: Date | undefined): string => {
  if (!date) {
    return format(new Date(), "MM/dd/yyyy");
  }
  return format(date, "MM/dd/yyyy");
};

//Takes in date and returns it in MM/dd/yyyy format if date exists.
export const mmDDYYFormat = (date: string | Date | undefined | null) => {
  const dateForm = "MM/dd/yyyy";
  const parsedDate = tryddmmyyyyToDate(date);
  return parsedDate ? format(parsedDate, dateForm) : "";
};

export const mmDDYYYY_HHMMSS_Format = (date: string | Date | undefined) => {
  if (!date) return "";

  // Handle Date objects directly
  if (date instanceof Date) {
    return format(date, DATE_TIME_FORMAT_MMDDYYYY_HHMMSS);
  }

  // Handle ISO date strings (preserve time)
  if (typeof date === "string") {
    const parsedDate = parseISO(date);
    if (isValid(parsedDate)) {
      return format(parsedDate, DATE_TIME_FORMAT_MMDDYYYY_HHMMSS);
    }
  }

  return "";
};

export const tryddmmyyyyToDate = (date?: string | Date | null): Date | null => {
  if (!date) return null;
  if (date === DATE_FORMAT_YYYYMMDD) return null;

  // If date is already a Date object, just apply startOfDay
  if (date instanceof Date) {
    return startOfDay(date);
  }

  // Handle C# DateOnly: yyyy-MM-dd or yyyy-MM-ddTHH:mm:ss (no timezone adjustment)
  if (typeof date === "string") {
    // yyyy-MM-dd
    const dateOnlyMatch = date.match(/^\d{4}-\d{2}-\d{2}$/);
    if (dateOnlyMatch) {
      const [year, month, day] = date.split("-").map(Number);
      return new Date(year, month - 1, day); // JS months are 0-based
    }
    // yyyy-MM-ddTHH:mm:ss (no Z, no offset)
    const dateTimeMatch = date.match(/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}$/);
    if (dateTimeMatch) {
      const [datePart] = date.split("T");
      const [year, month, day] = datePart.split("-").map(Number);
      return new Date(year, month - 1, day);
    }
  }

  // Try to parse the date
  try {
    let parsedDate: Date | null = null;

    // Case 1: Try parsing as MM/DD/YYYY format
    if (typeof date === "string" && date.includes("/") && date.split("/").length === 3) {
      const tempDate = parse(date, "MM/dd/yyyy", new Date());
      if (isValid(tempDate)) {
        parsedDate = tempDate;
      }
    }

    // Case 2: Try parsing as ISO format
    if (!parsedDate && typeof date === "string") {
      const tempDate = parseISO(date);
      if (isValid(tempDate)) {
        parsedDate = tempDate;
      }
    }

    // Case 3: Try parsing JavaScript's toString format
    // (e.g., "Sat Jan 13 2024 00:00:00 GMT-0500 (Eastern Standard Time)")
    if (!parsedDate && typeof date === "string" && date.includes("GMT")) {
      const tempDate = new Date(date);
      if (isValid(tempDate)) {
        parsedDate = tempDate;
      }
    }

    // Case 4: Last resort - try generic Date constructor
    if (!parsedDate && typeof date === "string") {
      const tempDate = new Date(date);
      if (isValid(tempDate)) {
        parsedDate = tempDate;
      }
    }

    // Return parsed date with start of day, or null if parsing failed
    return parsedDate ? startOfDay(parsedDate) : null;
  } catch (error) {
    console.warn("Error parsing date:", error);
    return null;
  }
};
