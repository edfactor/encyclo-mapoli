export const paymentTypeGetNumberMap: Record<string, number> = {
  all: 0,
  hardship: 1,
  payoffs: 2,
  rollovers: 3
};

export const paymentTypeGetStringMap: Record<number, string> = {
  0: "all",
  1: "hardship",
  2: "payoffs",
  3: "rollovers"
};

export const memberTypeGetNumberMap: Record<string, number> = {
  all: 0,
  employees: 1,
  beneficiaries: 2,
  none: 3
};

export const memberTypeGetStringMap: Record<number, string> = {
  0: "all",
  1: "employees",
  2: "beneficiaries",
  3: "none"
};
