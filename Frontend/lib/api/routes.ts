export const APP_ROUTES = {
  HOME: "/",

  ADMIN_HOME: "/admin",
  ADMIN_ACCOUNTS: "/admin/accounts",
  ADMIN_ASSETS: "/admin/assets",
  ADMIN_ASSIGNMENTS: "/admin/assignments",
  ADMIN_REPORT: "/admin/report",
  ADMIN_RETURNS: "/admin/returns",

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

export const APP_SIDEBAR_ADMIN_ITEMS = [
  { label: "Home", href: APP_ROUTES.ADMIN_HOME },
  { label: "Manage User", href: APP_ROUTES.ADMIN_ACCOUNTS },
  { label: "Manage Asset", href: APP_ROUTES.ADMIN_ASSETS },
  { label: "Manage Assignment", href: APP_ROUTES.ADMIN_ASSIGNMENTS },
  { label: "Request for Returning", href: APP_ROUTES.ADMIN_RETURNS },
  { label: "Report", href: APP_ROUTES.ADMIN_REPORT },
]

export const APP_SIDEBAR_STAFF_ITEMS = [{ label: "Home", href: APP_ROUTES.STAFF_HOME }];