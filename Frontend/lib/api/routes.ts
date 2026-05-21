export const APP_ROUTES = {
  HOME: "/",

  ADMIN_HOME: "/admin",
  ADMIN_ACCOUNTS: "/admin/accounts",
  ADMIN_ASSETS: "/admin/assets",
  ADMIN_ASSIGNMENTS: "/admin/assignments",
  ADMIN_REPORT: "/admin/report",
  ADMIN_RETURNS: "/admin/returns",

  // TEST
  ADMIN_TEST: "/admin/test",
  
  STAFF_HOME: "/staff",
};

export const APP_LABELS: Record<string, { parentLabel: string; itemLabel: string }> = {
  [APP_ROUTES.HOME]: { parentLabel: "Home", itemLabel: "Home" },
  [APP_ROUTES.STAFF_HOME]: { parentLabel: "Home", itemLabel: "Home" },
  [APP_ROUTES.ADMIN_HOME]: { parentLabel: "Home", itemLabel: "Home" },
  [APP_ROUTES.ADMIN_ACCOUNTS]: { parentLabel: "Manage User", itemLabel: "User" },
  [APP_ROUTES.ADMIN_ASSETS]: { parentLabel: "Manage Asset", itemLabel: "Asset" },
  [APP_ROUTES.ADMIN_ASSIGNMENTS]: { parentLabel: "Manage Assignment", itemLabel: "Assignment" },
  [APP_ROUTES.ADMIN_REPORT]: { parentLabel: "Report", itemLabel: "Report" },
  [APP_ROUTES.ADMIN_RETURNS]: { parentLabel: "Request for Returning", itemLabel: "Request" },
};