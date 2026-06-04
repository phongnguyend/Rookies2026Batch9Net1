export const APP_ROUTES = {
  HOME: "/",

  ADMIN_HOME: "/admin",
  ADMIN_USERS: "/admin/users",
  ADMIN_ASSETS: "/admin/assets",
  ADMIN_ASSIGNMENTS: "/admin/assignments",
  ADMIN_REPORT: "/admin/report",
  ADMIN_RETURNS: "/admin/returns",

  // TEST
  ADMIN_TEST: "/admin/test",

  STAFF_HOME: "/staff",
};

export const APP_LABELS: Record<
  string,
  { parentLabel: string; itemLabel: string }
> = {
  [APP_ROUTES.HOME]: { parentLabel: "Home", itemLabel: "Home" },
  [APP_ROUTES.STAFF_HOME]: { parentLabel: "Home", itemLabel: "Home" },
  [APP_ROUTES.ADMIN_HOME]: { parentLabel: "Home", itemLabel: "Home" },
  [APP_ROUTES.ADMIN_USERS]: {
    parentLabel: "Manage User",
    itemLabel: "User",
  },
  [APP_ROUTES.ADMIN_ASSETS]: {
    parentLabel: "Manage Asset",
    itemLabel: "Asset",
  },
  [APP_ROUTES.ADMIN_ASSIGNMENTS]: {
    parentLabel: "Manage Assignment",
    itemLabel: "Assignment",
  },
  [APP_ROUTES.ADMIN_REPORT]: { parentLabel: "Report", itemLabel: "Report" },
  [APP_ROUTES.ADMIN_RETURNS]: {
    parentLabel: "Request for Returning",
    itemLabel: "Request",
  },
};

export const APP_SIDEBAR_ADMIN_ITEMS = [
  { label: "Home", href: APP_ROUTES.ADMIN_HOME, dataTestId: "mnuHome" },
  { label: "Manage User", href: APP_ROUTES.ADMIN_USERS, dataTestId: "mnuManageUser" },
  { label: "Manage Asset", href: APP_ROUTES.ADMIN_ASSETS, dataTestId: "mnuManageAsset" },
  { label: "Manage Assignment", href: APP_ROUTES.ADMIN_ASSIGNMENTS, dataTestId: "mnuManageAssignment" },
  { label: "Request for Returning", href: APP_ROUTES.ADMIN_RETURNS, dataTestId: "mnuRequestForReturning" },
  { label: "Report", href: APP_ROUTES.ADMIN_REPORT, dataTestId: "mnuReport" },
];

export const APP_SIDEBAR_STAFF_ITEMS = [
  { label: "Home", href: APP_ROUTES.STAFF_HOME, dataTestId: "mnuHome" },
];
