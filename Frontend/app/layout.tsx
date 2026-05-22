import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import NavBar from "@/features/shared/components/NavBar/NavBar";
import StoreProvider from "./StoreProvider";
import ToastContainer from "@/features/shared/components/Toast/ToastContainer";
import GlobalModalContainer from "@/features/shared/components/Modal/GlobalModalContainer";
import { Fragment } from "react/jsx-runtime";

import Drawer from "@/features/shared/components/Drawer/Drawer";
import { AccountRole } from "@/features/users/accounts.types";
import DrawerCheckbox from "@/features/shared/components/Drawer/DrawerCheckbox";
import FloatingDrawerButton from "@/features/shared/components/Drawer/FloatingDrawerButton";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: {
    default: "Admin Panel",
    template: "%s | Nash Asset Management Panel",
  },
  description: "Admin Panel Management",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  // will implement auth later
  const authenticatedUser = {
    isAuthenticated: true,
    role: AccountRole.Admin,
  };

  return (
    <html
      lang="en"
      className={`${geistSans.variable} ${geistMono.variable} h-full antialiased`}
    >
      <body className="min-h-full flex flex-col">
        <StoreProvider>
          {authenticatedUser.isAuthenticated ? (
            <>
              {/* Navbar */}
              <NavBar />

              {/* Main */}
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

                  <Drawer role={authenticatedUser.role} />
                </div>
              </div>

              {/* Utilities Components */}
              <ToastContainer />
              <GlobalModalContainer />
            </>
          ) : (
            children
          )}
        </StoreProvider>
      </body>
    </html>
  );
}
