import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import NavBar from "@/features/shared/components/NavBar/NavBar";
import StoreProvider from "./StoreProvider";
import ToastContainer from "@/features/shared/components/Toast/ToastContainer";
import { Fragment } from "react/jsx-runtime";

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
  const isAuthenticated = true;

  return (
    <html lang="en" className={`${geistSans.variable} ${geistMono.variable} h-full antialiased`}>
      <body className="min-h-full flex flex-col">
        <StoreProvider>
          {isAuthenticated ? (
            <>
              <NavBar />
              {children}
              <ToastContainer />
            </>
          ) : (
            children
          )}
        </StoreProvider>
      </body>
    </html>
  );
}
