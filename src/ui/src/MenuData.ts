import { MenuBarData } from "smart-ui-library/dist/components/MenuBar/MenuDataTypes";

const exampleMenu: MenuBarData = {
  header: "Year End Reports",
  subMenuItems: [
    { caption: "Demographic Bages Not In Payprofit", route: "/demographic-badges-not-in-payprofit" },
    { caption: "Duplicate SSNs in Demographics", route: "/duplicate-ssns-demographics" },
    { caption: "Negative ETVA for SSNs on Payprofit", route: "/negative-etva-for-ssns-on-payprofit" },
    { caption: "Payroll Duplicate SSNs on Payprofit", route: "/payroll-duplicate-ssns-on-payprofit" },
    { caption: "Duplicate Names and Birthdays", route: "/duplicate-names-and-birthdays" },
    { caption: "Missing Comma in PY_NAME", route: "/missing-comma-in-py-name" },
  ]
};

const MenuData: MenuBarData[] = [exampleMenu];

export default MenuData;
