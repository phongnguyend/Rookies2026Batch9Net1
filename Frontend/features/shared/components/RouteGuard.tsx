"use client";

import { useEffect } from "react";
import { usePathname, useRouter } from "next/navigation";
import { useAppSelector } from "@/lib/redux/hooks";
import { UserRoles } from "@/features/users/users.types";
import LoadingSpinner from "./LoadingSpinner";

export function RouteGuard({ children }: { children: React.ReactNode }) {
  const router = useRouter();
  const pathname = usePathname();
  const { isAuthenticated, user, isLoading } = useAppSelector(
    (state) => state.authSlice,
  );

  const isProtectedRoute =
    pathname.startsWith("/admin") || pathname.startsWith("/staff");
  const isHome = pathname === "/";

  // check if user role conflicts with secure namespaces
  const isUnauthorized =
    pathname.startsWith("/admin") && user?.role !== UserRoles.Admin;

  useEffect(() => {
    if (isLoading) return;

    if (!isAuthenticated && isProtectedRoute) {
      router.push("/"); // push back to login
    } else if (isAuthenticated) {
      if (isHome) {
        router.replace(user?.role === UserRoles.Admin ? "/admin" : "/staff");
      } else if (isUnauthorized) {
        router.replace(user?.role === UserRoles.Admin ? "/admin" : "/staff");
      }
    }
  }, [isLoading, isAuthenticated, user, isProtectedRoute, isHome, isUnauthorized, router]);

  // show spinner if fetching auth state from backend
  if (isLoading) return <LoadingSpinner />;

  // show spinner while guest is being redirected to login
  if (!isAuthenticated && isProtectedRoute) return <LoadingSpinner />;

  // show spinner while logged-in user is redirected away from login
  if (isAuthenticated && isHome) return <LoadingSpinner />;

  // show spinner while unauthorized user is redirected to allowed dashboard
  if (isAuthenticated && isUnauthorized) return <LoadingSpinner />;

  // otherwise, allow rendering the children page
  return <>{children}</>;
}
