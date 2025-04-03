import { RouteCategory } from "smart-ui-library/dist/components/MenuBar/MenuBar";
import { CAPTIONS, MENU_LABELS, ROUTES } from "./constants";
import { ImpersonationRoles } from "./reduxstore/types";


const beneficiaries: RouteCategory = {
  menuLabel: MENU_LABELS.BENEFICIARIES,
  parentRoute: MENU_LABELS.BENEFICIARIES,
  items: [{ caption: CAPTIONS.SUMMARY, route: "" }]
};

const distributions: RouteCategory = {
  menuLabel: MENU_LABELS.DISTRIBUTIONS,
  parentRoute: MENU_LABELS.DISTRIBUTIONS,
  items: [{ caption: CAPTIONS.SUMMARY, route: "" }]
};

const reconciliation: RouteCategory = {
  menuLabel: MENU_LABELS.RECONCILIATION,
  parentRoute: MENU_LABELS.RECONCILIATION,
  items: [{ caption: CAPTIONS.SUMMARY, route: "" }]
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
  items: [{ caption: CAPTIONS.DEMOGRAPHIC_FREEZE, route: ROUTES.DEMO_FREEZE }]
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

export const drawerTitle = MENU_LABELS.YEAR_END;

export const menuLevels: MenuLevel[] = [
  {
    mainTitle: MENU_LABELS.DECEMBER_ACTIVITIES,
    topPage: [
      {
        topTitle: CAPTIONS.CLEAN_UP_REPORTS,
        subPages: [
          { subTitle: CAPTIONS.DEMOGRAPHIC_BADGES, subRoute: ROUTES.DEMOGRAPHIC_BADGES },
          { subTitle: CAPTIONS.DUPLICATE_SSNS, subRoute: ROUTES.DUPLICATE_SSNS },
          { subTitle: CAPTIONS.NEGATIVE_ETVA, subRoute: ROUTES.NEGATIVE_ETVA },
          { subTitle: CAPTIONS.DUPLICATE_NAMES, subRoute: ROUTES.DUPLICATE_NAMES }
        ]
      },
      {
        topTitle: CAPTIONS.MILITARY_CONTRIBUTIONS,
        topRoute: ROUTES.MILITARY_ENTRY_AND_MODIFICATION,
        subPages: []
      },
      {
        topTitle: CAPTIONS.REHIRE_FORFEITURES,
        topRoute: ROUTES.REHIRE_FORFEITURES,
        subPages: []
      },
      {
        topTitle: CAPTIONS.DISTRIBUTIONS_AND_FORFEITURES,
        topRoute: ROUTES.DISTRIBUTIONS_AND_FORFEITURES,
        subPages: []
      },
      {
        topTitle: CAPTIONS.TERMINATIONS,
        topRoute: ROUTES.PROF_TERM,
        subPages: []
      },
      {
        topTitle: CAPTIONS.PROFIT_SHARE_REPORT,
        topRoute: ROUTES.PROFIT_SHARE_REPORT,
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
        topTitle: CAPTIONS.MANAGE_EXECUTIVE_HOURS,
        topRoute: ROUTES.MANAGE_EXECUTIVE_HOURS,
        subPages: []
      },
      {
        topTitle: CAPTIONS.YTD_WAGES_EXTRACT,
        topRoute: ROUTES.YTD_WAGES_EXTRACT,
        subPages: []
      },
      

 
      {
        topTitle: CAPTIONS.PROFIT_SHARE_REPORT,
        topRoute: ROUTES.PROFIT_SHARE_REPORT,
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
        topTitle: CAPTIONS.ELIGIBLE_EMPLOYEES,
        topRoute: ROUTES.ELIGIBLE_EMPLOYEES,
        subPages: []
      },
      {
        topTitle: CAPTIONS.FORFEIT,
        topRoute: ROUTES.FORFEIT,
        subPages: []
      },
      {
        topTitle: CAPTIONS.PROFIT_SHARE_UPDATE,
        topRoute: ROUTES.PROFIT_SHARE_UPDATE,
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
        topTitle: CAPTIONS.PROFIT_SHARE_GROSS_REPORT,
        topRoute: ROUTES.PROFIT_SHARE_GROSS_REPORT,
        subPages: []
      },
      {
        topTitle: CAPTIONS.PROFIT_SHARE_BY_STORE,
        topRoute: ROUTES.PROFIT_SHARE_BY_STORE,
        subPages: [
          { subTitle: CAPTIONS.QPAY066_UNDER21, subRoute: ROUTES.QPAY066_UNDER21 },
          { subTitle: CAPTIONS.QPAY066TA_UNDER21, subRoute: ROUTES.QPAY066TA_UNDER21 },
          { subTitle: CAPTIONS.QPAY066TA, subRoute: ROUTES.QPAY066TA },
          { subTitle: CAPTIONS.PROFALL, subRoute: ROUTES.PROFALL },
          { subTitle: CAPTIONS.NEW_PS_LABELS, subRoute: ROUTES.NEW_PS_LABELS },
          { subTitle: CAPTIONS.PROFNEW, subRoute: ROUTES.PROFNEW }
        ]
      },
      {
        topTitle: CAPTIONS.PRINT_PROFIT_CERTS,
        topRoute: ROUTES.PRINT_PROFIT_CERTS,
        subPages: []
      },
      {
        topTitle: CAPTIONS.PAYMASTER_SAVE,
        topRoute: ROUTES.PAYMASTER_SAVE,
        subPages: []
      }
    ]
  }
];
export default MenuData;
