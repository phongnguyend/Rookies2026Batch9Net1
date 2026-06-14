"use client";

import { usePathname } from "next/navigation";
import Link from "next/link";
import NavbarProfile from "./NavBarProfile";
import { APP_ROUTES, APP_LABELS } from "@/lib/api/routes";

interface BreadcrumbItem {
  label: string;
  href: string;
}

const getBreadcrumbs = (path: string): BreadcrumbItem[] => {
  if (!path) return [{ label: "Home", href: APP_ROUTES.HOME }];

  const homeHref = path.startsWith(APP_ROUTES.STAFF_HOME)
    ? APP_ROUTES.STAFF_HOME
    : APP_ROUTES.ADMIN_HOME;
  const parentRoute = Object.values(APP_ROUTES).find(
    (route) =>
      route !== APP_ROUTES.HOME &&
      route !== APP_ROUTES.ADMIN_HOME &&
      path.startsWith(route),
  );

  // by default, return Home, even Admin Home and Staff Home
  if (
    !parentRoute ||
    path === APP_ROUTES.ADMIN_HOME ||
    path === APP_ROUTES.STAFF_HOME
  ) {
    return [{ label: "Home", href: homeHref }];
  }

  const routeConfig = APP_LABELS[parentRoute] || {
    parentLabel: "Section",
    itemLabel: "Item",
  };
  const breadcrumbs: BreadcrumbItem[] = [
    { label: routeConfig.parentLabel, href: parentRoute },
  ];

  // check the path, if containing create or edit, add Create, Edit
  const lowerPath = path.toLowerCase();
  if (lowerPath.includes("/create")) {
    breadcrumbs.push({
      label: `Create New ${routeConfig.itemLabel}`,
      href: path,
    });
  } else if (lowerPath.includes("/edit")) {
    breadcrumbs.push({ label: `Edit ${routeConfig.itemLabel}`, href: path });
  }

  return breadcrumbs;
};

export default function NavBar() {
  const pathname = usePathname();
  const breadcrumbs = getBreadcrumbs(pathname || "");

  return (
    <div className="navbar bg-primary h-17.5 w-full px-8 flex items-center justify-between text-white shadow-sm">
      {/* Left - Breadcrumbs */}
      <div className="breadcrumbs text-lg font-bold [&_li::before]:opacity-100 [&_li::before]:text-white">
        <ul>
          {breadcrumbs.map((crumb, idx) => {
            const isLast = idx === breadcrumbs.length - 1;
            const href = crumb.href;

            return (
              <li key={idx}>
                {isLast ? (
                  <span className="text-white">{crumb.label}</span>
                ) : (
                  <Link
                    prefetch={false}
                    href={href}
                    className="text-white/80 hover:text-white transition-colors"
                  >
                    {crumb.label}
                  </Link>
                )}
              </li>
            );
          })}
        </ul>
      </div>

      {/* Right - Profile */}
      <div className="flex-none gap-4 pr-2">
        <NavbarProfile />
      </div>
    </div>
  );
}
