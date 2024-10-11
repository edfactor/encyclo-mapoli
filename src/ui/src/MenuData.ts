import { MenuBarData } from "smart-ui-library/dist/components/MenuBar/MenuDataTypes";

const exampleMenu: MenuBarData = {
  header: "Example",
  subMenuItems: [
    { caption: "Example Route", route: "/example" },
    { caption: "Demographic Bages Not In Payprofit", route: "/demographic-badges-not-in-payprofit" },
    { caption: "Duplicate SSNs in Demographics", route: "/duplicate-ssns-demographics" },
  ]
};

const MenuData: MenuBarData[] = [exampleMenu];

export default MenuData;
