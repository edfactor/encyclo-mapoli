import { MenuBarData } from "smart-ui-library/dist/components/MenuBar/MenuDataTypes";

const exampleMenu: MenuBarData = {
  header: "Example",
  subMenuItems: [
    { caption: "Example Route", route: "/example" },
  ]
};

const MenuData: MenuBarData[] = [exampleMenu];

export default MenuData;
