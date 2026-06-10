"use client";

import { useEffect } from "react";
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
import { startUserSessionHub } from "@/features/auth/user-session.signalr";
import { useGetExportStatusQuery } from "@/features/report/report.api";
import { ExportReportJobStatus } from "@/features/report/report.types";
import { enqueueToast, ToastType } from "@/features/shared/toast.slice";
import {
  setHasNotifiedReady,
  setLastJobStatus,
} from "@/features/report/report.slice";
import dayjs from "dayjs";

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

  const hasToken =
    typeof window !== "undefined" &&
    (!!localStorage.getItem("accessToken") ||
      !!localStorage.getItem("refreshToken"));

  // restore session on page mount/refresh if token exists
  // - do not call the hook when user is authenticated, or when user has the token from localStorage
  const { data: profile, isError } = useGetMeQuery(undefined, {
    skip: isAuthenticated || !hasToken,
  });

  useEffect(() => {
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
      void startUserSessionHub(dispatch);
    } else if (isError && !isAuthenticated) {
      dispatch(completeLoading());
    }
  }, [profile, isAuthenticated, isError, dispatch, hasToken]);

  // Export Report
  useAppSelector((state) => state.reportSlice);
  const { lastJobStatus } = useAppSelector((state) => state.reportSlice);
  const { data: exportStatus } = useGetExportStatusQuery(undefined, {
    pollingInterval: 2000,
    skip: !isAuthenticated || user?.role !== UserRoles.Admin,
    refetchOnFocus: true,
    refetchOnReconnect: true,
    skipPollingIfUnfocused: true,
  });

  const formattedDate = exportStatus?.completedAtUtc
    ? dayjs(exportStatus.completedAtUtc).format("MMM D, YYYY HH:mm:ss")
    : "";

  useEffect(() => {
    if (!isAuthenticated || user?.role !== UserRoles.Admin) {
      if (typeof window !== "undefined") {
        localStorage.removeItem("isAlreadyNotified");
      }
      dispatch(setHasNotifiedReady(false));
      return;
    }

    // Only show once when the download finishes at any page
    const isAlreadyNotified =
      typeof window !== "undefined" &&
      localStorage.getItem("isAlreadyNotified") === "true";

    const becameReady =
      lastJobStatus !== ExportReportJobStatus.ReadyToDownload &&
      exportStatus?.status === ExportReportJobStatus.ReadyToDownload &&
      !isAlreadyNotified;

    if (becameReady) {
      dispatch(
        enqueueToast({
          message: `Report snapshot at ${formattedDate} is ready for downloading`,
          type: ToastType.Success,
        }),
      );
      if (typeof window !== "undefined") {
        localStorage.setItem("isAlreadyNotified", "true");
      }
      dispatch(setHasNotifiedReady(true));
    }

    dispatch(setLastJobStatus(exportStatus?.status ?? null));
  }, [
    exportStatus?.status,
    lastJobStatus,
    isAuthenticated,
    user?.role,
    formattedDate,
    dispatch,
  ]);

  return (
    <RouteGuard>
      <>
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
                <label
                  htmlFor="admin-drawer"
                  className="drawer-overlay"
                ></label>

                <Drawer role={user?.role as UserRoles} />
              </div>
            </div>

            {/* Utility Components */}
            <GlobalModalContainer />
            <FirstChangePasswordModal isOpen={user?.isFirstLogin === true} />
          </>
        ) : (
          children
        )}
        <ToastContainer />
      </>
    </RouteGuard>
  );
}
