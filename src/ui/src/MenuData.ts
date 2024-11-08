import { RouteCategory } from "smart-ui-library/dist/components/MenuBar/MenuBar";

const yearEndReports: RouteCategory = {
  menuLabel: "Year End Reports",
  parentRoute: "",
  items: [
    { caption: "Demographic Bages Not In Payprofit", route: "/demographic-badges-not-in-payprofit" },
    { caption: "Duplicate SSNs in Demographics", route: "/duplicate-ssns-demographics" },
    { caption: "Negative ETVA for SSNs on Payprofit", route: "/negative-etva-for-ssns-on-payprofit" },
    { caption: "Payroll Duplicate SSNs on Payprofit", route: "/payroll-duplicate-ssns-on-payprofit" },
    { caption: "Duplicate Names and Birthdays", route: "/duplicate-names-and-birthdays" },
    { caption: "Missing Comma in PY_NAME", route: "/missing-comma-in-py-name" },
    { caption: "Military and Rehire", route: "/military-and-rehire" },
    { caption: "Military and Rehire Forfeitures", route: "/military-and-rehire-forfeitures" },
    { caption: "Military and Rehire Profit Summary", route: "/military-and-rehire-profit-summary" },
    { caption: "Distributions and Forfeitures", route: "/distributions-and-forfeitures" },
    { caption: "Manage Executive Hours and Dollars", route: "/manage-executive-hours-and-dolars"},
    { caption: "Get Eligible Employees", route: "/eligible-employees" }
  ]
};

const MenuData: RouteCategory[] = [yearEndReports];

export default MenuData;
