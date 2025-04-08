import { format, isValid, parse, parseISO, startOfDay } from "date-fns";

export const EPOCH_DATE = new Date(1970, 0, 1);
export const NULL_DATE = "01/01/1970";
export const DAY_OF_SUNDAY = 0;
export const DAY_OF_SATURDAY = 6;
export const TOTAL_WEEK_DAYS = 7;
export const DATE_FORMAT_MMDDYYYY = "MM/dd/yyyy";
export const DATE_TIME_FORMAT_MMDDYYYY_HHMMSS = "MM/dd/yyyy HH:mm:ss";
export const DATE_FORMAT_YYYYMMDD = "yyyy-MM-dd";
export const DATE_REGEX_MMDDYYYY = /^\d{1,2}\/\d{1,2}\/\d{4}$/;
export const DATE_REGEX_YYYYMMDD = /^\d{4}-\d{1,2}-\d{1,2}$/;

//This is the call for the API only
export function dateYYYYMMDDhhmmss(date: string): string {
  return format(new Date(date), "yyyy-MM-dd     h:mm:ss");
}

//This is the call for the API only
export function dateYYYYMMDD(date: Date): string {
  return format(date, "yyyy-MM-dd");
}

//Validate passing string date value as passing date format.
export const isValidDateByFormat = (date: string, format: string) => {
  const parsedDate = parse(date, format, new Date());
  return isValid(parsedDate);
};

// Validate 1) MM/dd/yyyy and 2) yyyy-dd-MM date format + value
export const isValidDateValue = (dateString: string, dateFormat: string) => {
  if (!isValidDateByFormat(dateString, dateFormat)) return false;

  if (
    (dateFormat === DATE_FORMAT_YYYYMMDD && !DATE_REGEX_YYYYMMDD.test(dateString)) ||
    (dateFormat === DATE_FORMAT_MMDDYYYY && !DATE_REGEX_MMDDYYYY.test(dateString))
  ) {
    return false;
  }

  // Parse the date parts to integers
  const parts = dateFormat === DATE_FORMAT_YYYYMMDD ? dateString.split("-") : dateString.split("/");
  const day = dateFormat === DATE_FORMAT_YYYYMMDD ? parseInt(parts[2], 10) : parseInt(parts[1], 10);
  const month = dateFormat === DATE_FORMAT_YYYYMMDD ? parseInt(parts[1], 10) : parseInt(parts[0], 10);
  const year = dateFormat === DATE_FORMAT_YYYYMMDD ? parseInt(parts[0], 10) : parseInt(parts[2], 10);

  // Check the ranges of month and year
  if (year < 1000 || year > 3000 || month === 0 || month > 12) {
    return false;
  }

  const monthLength = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

  // Adjust for leap years
  if (year % 400 === 0 || (year % 100 !== 0 && year % 4 === 0)) {
    monthLength[1] = 29;
  }

  // Check the range of the day
  return day > 0 && day <= monthLength[month - 1];
};

//Validate passing string date as 'MM/dd/yyyy' format.
export const isValidDate = (date: string) => {
  const dateFormat = "MM/dd/yyyy";
  return isValidDateByFormat(date, dateFormat);
};

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

export function stringYYYYMMDD(date: string): string {
  return date && date !== "Invalid Date" && isValidDate(date) ? dateYYYYMMDD(new Date(date)) : "";
}

export function currentDateZeroHour(): Date {
  return new Date(new Date().setUTCHours(0, 0, 0, 0));
}

export function dateComparator(date1: string, date2: string) {
  const date1Number = new Date(date1).getTime();
  const date2Number = new Date(date2).getTime();

  if (date1Number == null && date2Number == null) {
    return 0;
  }

  if (date1Number == null) {
    return -1;
  } else if (date2Number == null) {
    return 1;
  }

  return date1Number - date2Number;
}

export const getDateWithDefault = (date?: string): string => {
  return date ? dateYYYYMMDD(new Date(date)) : dateYYYYMMDD(EPOCH_DATE);
};

export function toDisplayDateFull(date?: Date) {
  const today = date ? date : new Date();
  const month = today.toLocaleString("default", { month: "short" });
  const year = today.getFullYear();
  const day = today.getDate();

  let hours = today.getHours();
  let minutes = today.getMinutes();
  const ampm = hours >= 12 ? "PM" : "AM";
  hours = hours % 12;
  hours = hours ? hours : 12; // the hour '0' should be '12'
  minutes = minutes < 10 ? Number("0" + minutes) : minutes;
  const strTime = hours + ":" + minutes + " " + ampm;

  return `${month} ${day}, ${year} ${strTime}`;
}

export const formatFullDateString = (dateString: string, outputFormat: string = DATE_FORMAT_YYYYMMDD): string => {
  try {
    // Create a new Date object from the string
    const date = new Date(dateString);

    // Check if the date is valid
    if (!isValid(date)) {
      return "";
    }

    // Format the date according to the specified format
    return format(date, outputFormat);
  } catch (error) {
    console.error("Error parsing date:", error);
    return "";
  }
};

export const tryddmmyyyyToDate = (date?: string | Date | null): Date | null => {
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
