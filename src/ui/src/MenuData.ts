import { RouteCategory } from "smart-ui-library/dist/components/MenuBar/MenuBar";

const yearEndReports: RouteCategory = {
  menuLabel: "Year End Flow",
  parentRoute: "/yearend-flow",
  items: [
    { caption: "Distributions and Forfeitures", route: "/distributions-and-forfeitures" },
    { caption: "Manage Executive Hours and Dollars", route: "/manage-executive-hours-and-dolars"},
    { caption: "Get Eligible Employees", route: "/eligible-employees" },
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

const decemberFlow: RouteCategory = {
  menuLabel: "December Flow",
  parentRoute: "",
  items: [
    { caption: "Summary", route: "/december-process" },
    { caption: "Local API", route: "/december-process-local" },
    { caption: "Accordion", route: "/december-process-accordion" },
    { caption: "Demographic Badges Not In Payprofit", route: "/demographic-badges-not-in-payprofit" },
    { caption: "Duplicate SSNs in Demographics", route: "/duplicate-ssns-demographics" },
    { caption: "Negative ETVA for SSNs on Payprofit", route: "/negative-etva-for-ssns-on-payprofit" },
    { caption: "Duplicate Names and Birthdays", route: "/duplicate-names-and-birthdays" },
    { caption: "Missing Comma in Full Name", route: "/missing-comma-in-py-name" },
    { caption: "Military and Rehire", route: "/military-and-rehire" },
    { caption: "Military and Rehire Forfeitures", route: "/military-and-rehire-forfeitures" },
    { caption: "Military and Rehire Profit Summary", route: "/military-and-rehire-profit-summary" },
    { caption: "Profit Share Report", route: "/profit-share-report" },
    { caption: "Profit Share Update", route: "/profit-share-update" }
  ]
};

const beneficiaries: RouteCategory = {
  menuLabel: "Beneficiaries",
  parentRoute: "",
  items: [
    { caption: "Summary", route: "/frozen-summary" }
  ]
};

const distributions: RouteCategory = {
  menuLabel: "Distributions",
  parentRoute: "",
  items: [
    { caption: "Summary", route: "/frozen-summary" }
  ]
};

const reconciliation: RouteCategory = {
  menuLabel: "Reconciliation",
  parentRoute: "",
  items: [
    { caption: "Summary", route: "/frozen-summary" }
  ]
};

const miscellaneous: RouteCategory = {
  menuLabel: "Miscellaneous",
  parentRoute: "",
  items: [
    { caption: "Master Inquiry", route: "/master-inquiry" }
  ]
};

const decemberTest: RouteCategory = {
  menuLabel: "December Flow",
  parentRoute: "/december-process-accordion"
};

const MenuData: RouteCategory[] = [beneficiaries, distributions, reconciliation, decemberTest, yearEndReports, miscellaneous];

export default MenuData;
