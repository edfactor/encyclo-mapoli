import { RouteCategory } from "smart-ui-library/dist/components/MenuBar/MenuBar";

const yearEndReports: RouteCategory = {
  menuLabel: "Year End Reports",
  parentRoute: "",
  items: [
    { caption: "Demographic Badges Not In Payprofit", route: "/demographic-badges-not-in-payprofit" },
    { caption: "Duplicate SSNs in Demographics", route: "/duplicate-ssns-demographics" },
    { caption: "Negative ETVA for SSNs on Payprofit", route: "/negative-etva-for-ssns-on-payprofit" },
    { caption: "Duplicate Names and Birthdays", route: "/duplicate-names-and-birthdays" },
    { caption: "Missing Comma in Full Name", route: "/missing-comma-in-py-name" },
    { caption: "Military and Rehire", route: "/military-and-rehire" },
    { caption: "Military and Rehire Forfeitures", route: "/military-and-rehire-forfeitures" },
    { caption: "Military and Rehire Profit Summary", route: "/military-and-rehire-profit-summary" },
    { caption: "Distributions and Forfeitures", route: "/distributions-and-forfeitures" },
    { caption: "Manage Executive Hours and Dollars", route: "/manage-executive-hours-and-dolars"},
    { caption: "Get Eligible Employees", route: "/eligible-employees" },
    { caption: "Master Inquiry", route: "/master-inquiry" },
    { caption: "Get Distributions by Age (PROF 130)", route: "/distributions-by-age" },
    { caption: "Get Contributions by Age (PROF 130)", route: "/contributions-by-age" },
    { caption: "Get Forfeitures by Age (PROF 130)", route: "/forfeitures-by-age" },
    { caption: "Get Balance by Age (PROF 130B)", route: "/balance-by-age" },
    { caption: "Get Balance by Years (PROF 130Y)", route: "/balance-by-years" },
    { caption: "Get Vested Amounts by Age (PROF 130V)", route: "/vested-amounts-by-age" },
    { caption: "PROFTERM", route: "/prof-term" },
    { caption: "Military and Rehire Entry", route: "/military-and-rehire-entry" },
  ]
};

const MenuData: RouteCategory[] = [yearEndReports];

export default MenuData;
