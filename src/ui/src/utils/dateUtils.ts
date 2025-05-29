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
export const mmDDYYFormat = (date: string | Date | undefined) => {
  const dateForm = "MM/dd/yyyy";
  date = date ? format(new Date(date), dateForm) : "";
  return date;
};

export const mmDDYYYY_HHMMSS_Format = (date: string | Date | undefined) => {
  return date ? format(new Date(date), DATE_TIME_FORMAT_MMDDYYYY_HHMMSS) : "";
};


export const tryddmmyyyyToDate = (date?: string | Date | null): any | null => {
  if (!date) return null;
  if (date === DATE_FORMAT_YYYYMMDD) return null;

  // If date is already a Date object, just apply startOfDay
  if (date instanceof Date) {
    return startOfDay(date);
  }

  // Try to parse the date
  try {
    let parsedDate: Date | null = null;

    // Case 1: Try parsing as DD/MM/YYYY format
    if (date.includes('/') && date.split('/').length === 3) {
      const tempDate = parse(date, 'dd/MM/yyyy', new Date());
      if (isValid(tempDate)) {
        parsedDate = tempDate;
      }
    }

    // Case 2: Try parsing as ISO format
    if (!parsedDate) {
      const tempDate = parseISO(date);
      if (isValid(tempDate)) {
        parsedDate = tempDate;
      }
    }

    // Case 3: Try parsing JavaScript's toString format 
    // (e.g., "Sat Jan 13 2024 00:00:00 GMT-0500 (Eastern Standard Time)")
    if (!parsedDate && date.includes('GMT')) {
      const tempDate = new Date(date);
      if (isValid(tempDate)) {
        parsedDate = tempDate;
      }
    }

    // Case 4: Last resort - try generic Date constructor
    if (!parsedDate) {
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
