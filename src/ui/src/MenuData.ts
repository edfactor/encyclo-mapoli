import { RouteCategory } from "smart-ui-library/dist/components/MenuBar/MenuBar";
import { CAPTIONS, MENU_LABELS, ROUTES } from "./constants";
import { ImpersonationRoles } from "./reduxstore/types";

/*
const fiscalClose: RouteCategory = {
  menuLabel: MENU_LABELS.FISCAL_CLOSE,
  parentRoute: ROUTES.FISCAL_CLOSE,
  items: [
    { caption: CAPTIONS.SUMMARY, route: ROUTES.FISCAL_CLOSE },
    { caption: CAPTIONS.ELIGIBLE_EMPLOYEES, route: ROUTES.ELIGIBLE_EMPLOYEES },
    { caption: CAPTIONS.DISTRIBUTIONS_BY_AGE, route: ROUTES.DISTRIBUTIONS_BY_AGE },
    { caption: CAPTIONS.CONTRIBUTIONS_BY_AGE, route: ROUTES.CONTRIBUTIONS_BY_AGE },
    { caption: CAPTIONS.FORFEITURES_BY_AGE, route: ROUTES.FORFEITURES_BY_AGE },
    { caption: CAPTIONS.BALANCE_BY_AGE, route: ROUTES.BALANCE_BY_AGE },
    { caption: CAPTIONS.BALANCE_BY_YEARS, route: ROUTES.BALANCE_BY_YEARS },
    { caption: CAPTIONS.VESTED_AMOUNTS_BY_AGE, route: ROUTES.VESTED_AMOUNTS_BY_AGE },
    { caption: CAPTIONS.YTD_WAGES_EXTRACT, route: ROUTES.YTD_WAGES_EXTRACT },
    { caption: CAPTIONS.MILITARY_ENTRY_AND_MODIFICATION, route: ROUTES.MILITARY_ENTRY_AND_MODIFICATION },
    { caption: CAPTIONS.PROFIT_SHARE_REPORT_EDIT_RUN, route: ROUTES.PROFIT_SHARE_REPORT_EDIT_RUN },
    { caption: CAPTIONS.PROFIT_SHARE_REPORT_FINAL_RUN, route: ROUTES.PROFIT_SHARE_REPORT_FINAL_RUN },
    { caption: CAPTIONS.PAYMASTER_UPDATE, route: ROUTES.PAYMASTER_UPDATE },
    { caption: CAPTIONS.PROFIT_SHARE_BY_STORE, route: ROUTES.PROFIT_SHARE_BY_STORE },
    { caption: CAPTIONS.PROFIT_SHARE_GROSS_REPORT, route: ROUTES.PROFIT_SHARE_GROSS_REPORT },
    { caption: CAPTIONS.FORFEIT, route: ROUTES.FORFEIT },
    { caption: CAPTIONS.PAY450_SUMMARY, route: ROUTES.PAY450_SUMMARY },
    { caption: CAPTIONS.PROFALL, route: ROUTES.PROFALL },
    { caption: CAPTIONS.PAY426_ACTIVE_18_20, route: ROUTES.PAY426_ACTIVE_18_20 },
    { caption: CAPTIONS.PAY426_ACTIVE_21_PLUS, route: ROUTES.PAY426_ACTIVE_21_PLUS },
    { caption: CAPTIONS.PAY426_ACTIVE_21_PLUS, route: ROUTES.PAY426_ACTIVE_21_PLUS },
    { caption: CAPTIONS.PAY426_ACTIVE_UNDER_18, route: ROUTES.PAY426_ACTIVE_UNDER_18 },
    { caption: CAPTIONS.PAY426_ACTIVE_PRIOR_SHARING, route: ROUTES.PAY426_ACTIVE_PRIOR_SHARING },
    { caption: CAPTIONS.PAY426_ACTIVE_NO_PRIOR, route: ROUTES.PAY426_ACTIVE_NO_PRIOR },
    { caption: CAPTIONS.PAY426_TERMINATED_1000_PLUS, route: ROUTES.PAY426_TERMINATED_1000_PLUS },
    { caption: CAPTIONS.PAY426_TERMINATED_NO_PRIOR, route: ROUTES.PAY426_TERMINATED_NO_PRIOR },
    { caption: CAPTIONS.PAY426_TERMINATED_PRIOR, route: ROUTES.PAY426_TERMINATED_PRIOR },
    { caption: CAPTIONS.PAY426_SUMMARY, route: ROUTES.PAY426_SUMMARY },
    { caption: CAPTIONS.PAY426_NON_EMPLOYEE, route: ROUTES.PAY426_NON_EMPLOYEE }
  ]
};

const decemberFlow: RouteCategory = {
  menuLabel: MENU_LABELS.DECEMBER_ACTIVITIES,
  parentRoute: MENU_LABELS.DECEMBER_ACTIVITIES,
  items: [
    { caption: CAPTIONS.SUMMARY, route: ROUTES.DECEMBER_PROCESS_ACCORDION },
    { caption: CAPTIONS.DEMOGRAPHIC_BADGES, route: ROUTES.DEMOGRAPHIC_BADGES },
    { caption: CAPTIONS.DUPLICATE_SSNS, route: ROUTES.DUPLICATE_SSNS },
    { caption: CAPTIONS.NEGATIVE_ETVA, route: ROUTES.NEGATIVE_ETVA },
    { caption: CAPTIONS.DUPLICATE_NAMES, route: ROUTES.DUPLICATE_NAMES },
    { caption: CAPTIONS.MISSING_COMMA, route: ROUTES.MISSING_COMMA },
    { caption: CAPTIONS.EMPLOYEES_MILITARY, route: ROUTES.EMPLOYEES_MILITARY },
    { caption: CAPTIONS.REHIRE_FORFEITURES, route: ROUTES.REHIRE_FORFEITURES },
    { caption: CAPTIONS.TERMINATIONS, route: ROUTES.PROF_TERM },
    { caption: CAPTIONS.DISTRIBUTIONS_AND_FORFEITURES, route: ROUTES.DISTRIBUTIONS_AND_FORFEITURES },
    { caption: CAPTIONS.MANAGE_EXECUTIVE_HOURS, route: ROUTES.MANAGE_EXECUTIVE_HOURS },
    { caption: CAPTIONS.PROFIT_SHARE_REPORT, route: ROUTES.PROFIT_SHARE_REPORT },
    { caption: CAPTIONS.PROFIT_SHARE_UPDATE, route: ROUTES.PROFIT_SHARE_UPDATE }
  ]
};
*/

const beneficiaries: RouteCategory = {
  menuLabel: MENU_LABELS.BENEFICIARIES,
  parentRoute: MENU_LABELS.BENEFICIARIES,
  items: [{ caption: CAPTIONS.SUMMARY, route: ROUTES.FROZEN_SUMMARY }]
};

const distributions: RouteCategory = {
  menuLabel: MENU_LABELS.DISTRIBUTIONS,
  parentRoute: MENU_LABELS.DISTRIBUTIONS,
  items: [{ caption: CAPTIONS.SUMMARY, route: ROUTES.FROZEN_SUMMARY }]
};

const reconciliation: RouteCategory = {
  menuLabel: MENU_LABELS.RECONCILIATION,
  parentRoute: MENU_LABELS.RECONCILIATION,
  items: [{ caption: CAPTIONS.SUMMARY, route: ROUTES.FROZEN_SUMMARY }]
};

const inquiries: RouteCategory = {
  menuLabel: MENU_LABELS.INQUIRIES,
  parentRoute: MENU_LABELS.INQUIRIES,
  items: [{ caption: CAPTIONS.MASTER_INQUIRY, route: ROUTES.MASTER_INQUIRY }]
};

const drawer: RouteCategory = {
  menuLabel: MENU_LABELS.YEAR_END,
  parentRoute: MENU_LABELS.YEAR_END,
  items: [
    { caption: MENU_LABELS.DECEMBER_ACTIVITIES, route: ROUTES.DECEMBER_PROCESS_ACCORDION },
    { caption: MENU_LABELS.FISCAL_CLOSE, route: ROUTES.FISCAL_CLOSE }
  ]
};

const localStorageImpersonating: string | null = localStorage.getItem("impersonatingRole");

const it_operations: RouteCategory = {
  menuLabel: MENU_LABELS.IT_OPERATIONS,
  parentRoute: MENU_LABELS.IT_OPERATIONS,
  roles: [ImpersonationRoles.ItOperations], // Only users with this role can see this menu item
  items: [{ caption: CAPTIONS.DEMOGRAOHIC_FREEZE, route: ROUTES.DEMO_FREEZE }]
};

const MenuData: RouteCategory[] = [
  inquiries,
  beneficiaries,
  distributions,
  reconciliation,
  drawer,
  localStorageImpersonating == ImpersonationRoles.ItOperations ? it_operations : ""
];

interface MenuLevel {
  mainTitle: string;
  topPage: {
    topTitle: string;
    topRoute?: string;
    subPages: {
      subTitle?: string;
      subRoute?: string;
    }[];
  }[];
}

export const drawerTitle = "Profit Sharing Activities";

export const menuLevels: MenuLevel[] = [
  {
    mainTitle: MENU_LABELS.DECEMBER_ACTIVITIES,
    topPage: [
      {
        topTitle: CAPTIONS.SUMMARY,
        topRoute: ROUTES.DECEMBER_PROCESS_ACCORDION,
        subPages: []
      },
      {
        topTitle: CAPTIONS.CLEAN_UP_REPORTS,
        subPages: [
          { subTitle: CAPTIONS.DEMOGRAPHIC_BADGES, subRoute: ROUTES.DEMOGRAPHIC_BADGES },
          { subTitle: CAPTIONS.DUPLICATE_SSNS, subRoute: ROUTES.DUPLICATE_SSNS },
          { subTitle: CAPTIONS.NEGATIVE_ETVA, subRoute: ROUTES.NEGATIVE_ETVA },
          { subTitle: CAPTIONS.DUPLICATE_NAMES, subRoute: ROUTES.DUPLICATE_NAMES },
          { subTitle: CAPTIONS.MISSING_COMMA, subRoute: ROUTES.MISSING_COMMA }
        ]
      },
      {
        topTitle: CAPTIONS.MILITARY_LEAVE,
        topRoute: ROUTES.MILITARY_LEAVE,
        subPages: []
      },
      {
        topTitle: CAPTIONS.REHIRE_FORFEITURES,
        topRoute: ROUTES.REHIRE_FORFEITURES,
        subPages: []
      },
      {
        topTitle: CAPTIONS.MANAGE_EXECUTIVE_HOURS,
        topRoute: ROUTES.MANAGE_EXECUTIVE_HOURS,
        subPages: []
      },
      {
        topTitle: CAPTIONS.TERMINATIONS,
        topRoute: ROUTES.PROF_TERM,
        subPages: []
      },
      {
        topTitle: CAPTIONS.DISTRIBUTIONS_AND_FORFEITURES,
        topRoute: ROUTES.DISTRIBUTIONS_AND_FORFEITURES,
        subPages: []
      },
      {
        topTitle: CAPTIONS.PROFIT_SHARE_REPORT,
        topRoute: ROUTES.PROFIT_SHARE_REPORT,
        subPages: []
      },
      {
        topTitle: CAPTIONS.PROFIT_SHARE_UPDATE,
        topRoute: ROUTES.PROFIT_SHARE_UPDATE,
        subPages: []
      }
    ]
  },
  {
    mainTitle: MENU_LABELS.FISCAL_CLOSE,
    topPage: [
      {
        topTitle: CAPTIONS.SUMMARY,
        topRoute: ROUTES.FISCAL_CLOSE,
        subPages: []
      },
      {
        topTitle: CAPTIONS.ELIGIBLE_EMPLOYEES,
        topRoute: ROUTES.ELIGIBLE_EMPLOYEES,
        subPages: []
      },
      {
        topTitle: CAPTIONS.REPORTS_BY_AGE,
        subPages: [
          { subTitle: CAPTIONS.CONTRIBUTIONS_BY_AGE, subRoute: ROUTES.CONTRIBUTIONS_BY_AGE },
          { subTitle: CAPTIONS.DISTRIBUTIONS_BY_AGE, subRoute: ROUTES.DISTRIBUTIONS_BY_AGE },
          { subTitle: CAPTIONS.FORFEITURES_BY_AGE, subRoute: ROUTES.FORFEITURES_BY_AGE },
          { subTitle: CAPTIONS.BALANCE_BY_AGE, subRoute: ROUTES.BALANCE_BY_AGE },
          { subTitle: CAPTIONS.VESTED_AMOUNTS_BY_AGE, subRoute: ROUTES.VESTED_AMOUNTS_BY_AGE },
          { subTitle: CAPTIONS.BALANCE_BY_YEARS, subRoute: ROUTES.BALANCE_BY_YEARS }
        ]
      },
      {
        topTitle: CAPTIONS.YTD_WAGES_EXTRACT,
        topRoute: ROUTES.YTD_WAGES_EXTRACT,
        subPages: []
      },
      {
        topTitle: CAPTIONS.MILITARY_ENTRY_AND_MODIFICATION,
        topRoute: ROUTES.MILITARY_ENTRY_AND_MODIFICATION,
        subPages: []
      },
      {
        topTitle: CAPTIONS.PROFIT_SHARE_REPORT_EDIT_RUN,
        topRoute: ROUTES.PROFIT_SHARE_REPORT_EDIT_RUN,
        subPages: [
          { subTitle: CAPTIONS.PAY426_ACTIVE_18_20, subRoute: ROUTES.PAY426_ACTIVE_18_20 },
          { subTitle: CAPTIONS.PAY426_ACTIVE_21_PLUS, subRoute: ROUTES.PAY426_ACTIVE_21_PLUS },
          { subTitle: CAPTIONS.PAY426_ACTIVE_UNDER_18, subRoute: ROUTES.PAY426_ACTIVE_UNDER_18 },
          { subTitle: CAPTIONS.PAY426_ACTIVE_PRIOR_SHARING, subRoute: ROUTES.PAY426_ACTIVE_PRIOR_SHARING },
          { subTitle: CAPTIONS.PAY426_ACTIVE_NO_PRIOR, subRoute: ROUTES.PAY426_ACTIVE_NO_PRIOR },
          { subTitle: CAPTIONS.PAY426_TERMINATED_1000_PLUS, subRoute: ROUTES.PAY426_TERMINATED_1000_PLUS },
          { subTitle: CAPTIONS.PAY426_TERMINATED_NO_PRIOR, subRoute: ROUTES.PAY426_TERMINATED_NO_PRIOR },
          { subTitle: CAPTIONS.PAY426_TERMINATED_PRIOR, subRoute: ROUTES.PAY426_TERMINATED_PRIOR },
          { subTitle: CAPTIONS.PAY426_SUMMARY, subRoute: ROUTES.PAY426_SUMMARY },
          { subTitle: CAPTIONS.PAY426_NON_EMPLOYEE, subRoute: ROUTES.PAY426_NON_EMPLOYEE }
        ]
      },
      {
        topTitle: CAPTIONS.PROFIT_SHARE_REPORT_FINAL_RUN,
        topRoute: ROUTES.PROFIT_SHARE_REPORT_FINAL_RUN,
        subPages: [
          { subTitle: CAPTIONS.PAY426_ACTIVE_18_20, subRoute: ROUTES.PAY426_ACTIVE_18_20 },
          { subTitle: CAPTIONS.PAY426_ACTIVE_21_PLUS, subRoute: ROUTES.PAY426_ACTIVE_21_PLUS },
          { subTitle: CAPTIONS.PAY426_ACTIVE_UNDER_18, subRoute: ROUTES.PAY426_ACTIVE_UNDER_18 },
          { subTitle: CAPTIONS.PAY426_ACTIVE_PRIOR_SHARING, subRoute: ROUTES.PAY426_ACTIVE_PRIOR_SHARING },
          { subTitle: CAPTIONS.PAY426_ACTIVE_NO_PRIOR, subRoute: ROUTES.PAY426_ACTIVE_NO_PRIOR },
          { subTitle: CAPTIONS.PAY426_TERMINATED_1000_PLUS, subRoute: ROUTES.PAY426_TERMINATED_1000_PLUS },
          { subTitle: CAPTIONS.PAY426_TERMINATED_NO_PRIOR, subRoute: ROUTES.PAY426_TERMINATED_NO_PRIOR },
          { subTitle: CAPTIONS.PAY426_TERMINATED_PRIOR, subRoute: ROUTES.PAY426_TERMINATED_PRIOR },
          { subTitle: CAPTIONS.PAY426_SUMMARY, subRoute: ROUTES.PAY426_SUMMARY },
          { subTitle: CAPTIONS.PAY426_NON_EMPLOYEE, subRoute: ROUTES.PAY426_NON_EMPLOYEE }
        ]
      },
      {
        topTitle: CAPTIONS.PAYMASTER_UPDATE,
        topRoute: ROUTES.PAYMASTER_UPDATE,
        subPages: []
      },

      {
        topTitle: CAPTIONS.PROFIT_SHARE_BY_STORE,
        topRoute: ROUTES.PROFIT_SHARE_BY_STORE,
        subPages: []
      },

      {
        topTitle: CAPTIONS.PROFIT_SHARE_GROSS_REPORT,
        topRoute: ROUTES.PROFIT_SHARE_GROSS_REPORT,
        subPages: []
      },
      {
        topTitle: CAPTIONS.FORFEIT,
        topRoute: ROUTES.FORFEIT,
        subPages: []
      },

      {
        topTitle: CAPTIONS.PAY450_SUMMARY,
        topRoute: ROUTES.PAY450_SUMMARY,
        subPages: []
      },
      {
        topTitle: CAPTIONS.PROFALL,
        topRoute: ROUTES.PROFALL,
        subPages: []
      }
    ]
  }
];
export default MenuData;
