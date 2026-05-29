"use client";

import { useEffect, useState } from "react";
import "./globals.css";
import NavBar from "@/features/shared/components/NavBar/NavBar";
import StoreProvider from "./StoreProvider";
import ToastContainer from "@/features/shared/components/Toast/ToastContainer";
import GlobalModalContainer from "@/features/shared/components/Modal/GlobalModalContainer";
import Drawer from "@/features/shared/components/Drawer/Drawer";
import { UserRoles } from "@/features/users/users.types";
import DrawerCheckbox from "@/features/shared/components/Drawer/DrawerCheckbox";
import FloatingDrawerButton from "@/features/shared/components/Drawer/FloatingDrawerButton";
import { RouteGuard } from "@/features/shared/components/RouteGuard";
import { useAppSelector, useAppDispatch } from "@/lib/redux/hooks";
import { useGetMeQuery } from "@/features/auth/auth.api";
import { loginSuccess, completeLoading } from "@/features/auth/auth.slice";
import FirstChangePasswordModal from "@/features/auth/components/FirstChangePasswordModal";

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" className={`h-full antialiased`}>
      <head>
        <title>Nash Asset Management Panel</title>
        <meta name="description" content="Admin Panel Management" />
      </head>
      <body className="min-h-full flex flex-col">
        <StoreProvider>
          <RootLayoutContent>{children}</RootLayoutContent>
        </StoreProvider>
      </body>
    </html>
  );
}

// wrap layout content under store provider
function RootLayoutContent({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, user } = useAppSelector((state) => state.authSlice);
  const dispatch = useAppDispatch();

  const [hasToken, setHasToken] = useState<boolean>(false);
  const [isCheckingToken, setIsCheckingToken] = useState<boolean>(true);

  useEffect(() => {
    if (typeof window !== "undefined") {
      const token =
        localStorage.getItem("accessToken") ||
        localStorage.getItem("refreshToken");
      setHasToken(!!token);
    }
    setIsCheckingToken(false);
  }, []);

  // restore session on page mount/refresh if token exists
  // restore if having token
  const { data: profile, isError } = useGetMeQuery(undefined, {
    skip: isCheckingToken || !hasToken,
  });

  useEffect(() => {
    if (isCheckingToken) return;

    if (!hasToken) {
      dispatch(completeLoading());
      return;
    }

    if (profile && !isAuthenticated) {
      const userRole = profile.roles.includes(UserRoles.Admin)
        ? UserRoles.Admin
        : UserRoles.Staff;
      dispatch(
        loginSuccess({
          username: profile.userName,
          role: userRole,
          isFirstLogin: profile.isFirstLogin,
          locationName: profile.locationName,
        }),
      );
    } else if (isError && !isAuthenticated) {
      dispatch(completeLoading());
    }
  }, [profile, isAuthenticated, isError, dispatch, isCheckingToken, hasToken]);

  return (
    <RouteGuard>
      {isAuthenticated ? (
        <>
          {/* Navbar */}
          <NavBar />

          {/* Main Layout */}
          <div className="drawer md:drawer-open relative px-8 pt-12">
            <DrawerCheckbox />
            <FloatingDrawerButton />

            {/* Left Content */}
            <div className="drawer-content md:pl-15 md:pt-15">
              <main>{children}</main>
            </div>

            {/* Drawer Sidebar */}
            <div className="drawer-side">
              <label htmlFor="admin-drawer" className="drawer-overlay"></label>

              <Drawer role={user?.role as UserRoles} />
            </div>
          </div>

          {/* Utility Components */}
          <ToastContainer />
          <GlobalModalContainer />
          <FirstChangePasswordModal isOpen={user?.isFirstLogin === true} />
        </>
      ) : (
        children
      )}
    </RouteGuard>
  );
}
