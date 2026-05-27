/* eslint-disable @typescript-eslint/no-explicit-any */
"use client";

import { useLoginMutation, useLazyGetMeQuery } from "@/features/auth/auth.api";
import LoginForm from "@/features/auth/components/LoginForm";
import { useAppDispatch } from "@/lib/redux/hooks";
import { loginSuccess, loginFailure } from "@/features/auth/auth.slice";
import { UserRoles } from "@/features/users/users.types";
import { useState } from "react";
import { Login } from "@/features/auth/auth.types";

export default function HomePage() {
  // Queries and hooks
  const [login, { isLoading: isLoggingIn }] = useLoginMutation();
  const [getMe] = useLazyGetMeQuery();
  const dispatch = useAppDispatch();

  // States
  const [serverErrors, setServerErrors] = useState<string[]>([]);

  const handleLoginSubmit = async (data: Login.Request) => {
    setServerErrors([]);
    try {
      await login({
        username: data.username,
        password: data.password,
      }).unwrap();

      const profile = await getMe().unwrap();
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
    } catch (err: any) {
      console.error("Login failed error object:", err);
      const parsedErrors: string[] = [];

      // parse validation error list if present
      if (err?.errors) {
        Object.values(err.errors).forEach((messages: any) => {
          if (Array.isArray(messages)) {
            parsedErrors.push(...messages);
          } else if (typeof messages === "string") {
            parsedErrors.push(messages);
          }
        });
      }

      // fallback to single error description
      if (parsedErrors.length === 0) {
        const fallbackMsg =
          err?.detail || "Username or password is incorrect. Please try again.";
        parsedErrors.push(fallbackMsg);
      }

      setServerErrors(parsedErrors);
      dispatch(loginFailure(parsedErrors[0]));
    }
  };

  return (
    <main className="relative min-h-screen flex items-center justify-center bg-base-200 px-4">
      <div className="absolute inset-0 overflow-hidden pointer-events-none z-0">
        <div className="absolute top-[-20%] left-[-10%] w-125 h-125 rounded-full bg-primary/5 blur-[120px]"></div>
        <div className="absolute bottom-[-20%] right-[-10%] w-125 h-125 rounded-full bg-primary/5 blur-[120px]"></div>
      </div>

      <div className="relative z-10 w-full flex justify-center">
        <LoginForm
          onLogin={handleLoginSubmit}
          isLoading={isLoggingIn}
          serverErrors={serverErrors}
        />
      </div>
    </main>
  );
}
