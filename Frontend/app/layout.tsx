"use client";

import { useEffect } from "react";
import { usePathname, useRouter } from "next/navigation";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import NavBar from "@/features/shared/components/NavBar/NavBar";
import StoreProvider from "./StoreProvider";
import ToastContainer from "@/features/shared/components/Toast/ToastContainer";
import GlobalModalContainer from "@/features/shared/components/Modal/GlobalModalContainer";
import Drawer from "@/features/shared/components/Drawer/Drawer";
import { UserRoles } from "@/features/users/users.types";
import DrawerCheckbox from "@/features/shared/components/Drawer/DrawerCheckbox";
import FloatingDrawerButton from "@/features/shared/components/Drawer/FloatingDrawerButton";
import LoadingSpinner from "@/features/shared/components/LoadingSpinner";
import { useAppSelector } from "@/lib/redux/hooks";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html
      lang="en"
      className={`${geistSans.variable} ${geistMono.variable} h-full antialiased`}
    >
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

// wrap around the root layout to use the useAppSelector under the StoreProvider
function RootLayoutContent({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, user, isLoading } = useAppSelector((state) => state.authSlice);
  const pathname = usePathname();
  const router = useRouter();

  useEffect(() => {
    if (isLoading) return;
    if (isAuthenticated && pathname === "/") {
      switch (user?.role) {
        case UserRoles.Admin:
          router.replace("/admin");
          break;
        case UserRoles.Staff:
          router.replace("/staff");
          break;
        default:
          router.replace("/");
          break;
      }
    }
  }, [isLoading, isAuthenticated, user, pathname, router]);

  // if still loading, then load spinner
  if (isLoading) {
    return <LoadingSpinner />;
  }

  const isUnauthorizedAdmin = pathname.startsWith("/admin") && user?.role !== UserRoles.Admin;
  const isUnauthorizedStaff = pathname.startsWith("/staff") && user?.role !== UserRoles.Staff && user?.role !== UserRoles.Admin;
  const isLoggedOnHome = isAuthenticated && pathname === "/";

  if (isUnauthorizedAdmin || isUnauthorizedStaff || isLoggedOnHome) {
    //TODO: Will create unauthorized page later
    return <div>You are not authorized to view this page</div>
  }

  return (
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
          <ToastContainer />
          <GlobalModalContainer />
        </>
      ) : (
        children
      )}
    </>
  );
}
