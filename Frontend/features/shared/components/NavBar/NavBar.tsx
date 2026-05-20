"use client";

import { usePathname } from "next/navigation";
import Link from "next/link";
import NavbarProfile from "./NavBarProfile";

interface RouteConfig {
  segment: string;
  parentLabel: string;
  itemLabel: string;
}

const ROUTE_CONFIGS: RouteConfig[] = [
  { segment: "accounts", parentLabel: "Manage User", itemLabel: "User" },
  { segment: "assets", parentLabel: "Manage Asset", itemLabel: "Asset" },
  { segment: "assignments", parentLabel: "Manage Assignment", itemLabel: "Assignment" },
];

const getHeaderBreadcrumbs = (path: string): string[] => {
  if (!path) return ["Home"];

  const segments = path.split("/").filter(Boolean);

  // Default to ["Home"] for root, admin, or staff homepages
  if (
    segments.length === 0 ||
    (segments.length === 1 && (segments[0] === "admin" || segments[0] === "staff"))
  ) {
    return ["Home"];
  }

  for (const config of ROUTE_CONFIGS) {
    if (path.includes(`/${config.segment}`)) {
      if (path.includes("/create")) {
        return [config.parentLabel, `Create New ${config.itemLabel}`];
      }
      if (path.includes("/edit")) {
        return [config.parentLabel, `Edit ${config.itemLabel}`];
      }
      return [config.parentLabel];
    }
  }

  // Basic single-level routes
  if (path.includes("/returns")) return ["Request for Returning"];
  if (path.includes("/report")) return ["Report"];

  return ["Home"];
};

export default function NavBar() {
  const pathname = usePathname();
  const breadcrumbs = getHeaderBreadcrumbs(pathname || "");

  // Map breadcrumb labels back to their respective routes via a clean lookup
  const getBreadcrumbLink = (label: string): string => {
    const isStaff = pathname?.startsWith("/staff");
    const homeHref = isStaff ? "/staff" : "/admin";

    const pathMap: Record<string, string> = {
      Home: homeHref,
      "Manage User": "/admin/accounts",
      "Manage Asset": "/admin/assets",
      "Manage Assignment": "/admin/assignments",
      "Request for Returning": "/admin/returns",
      Report: "/admin/report",
    };

    return pathMap[label] || pathname || "/";
  };

  return (
    <div className="navbar bg-primary h-[70px] w-full px-6 flex items-center justify-between text-white shadow-sm">
      {/* Left - Clickable Breadcrumbs */}
      <div className="breadcrumbs text-lg font-bold [&_li::before]:opacity-100 [&_li::before]:text-white">
        <ul>
          {breadcrumbs.map((crumb, idx) => {
            const isLast = idx === breadcrumbs.length - 1;
            const href = getBreadcrumbLink(crumb);

            return (
              <li key={idx}>
                {isLast ? (
                  <span className="text-white">{crumb}</span>
                ) : (
                  <Link href={href} className="text-white/80 hover:text-white transition-colors">
                    {crumb}
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
