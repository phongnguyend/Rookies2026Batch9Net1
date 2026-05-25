/* eslint-disable @typescript-eslint/no-explicit-any */
"use client";

import { useLoginMutation, useLazyGetMeQuery } from "@/features/auth/auth.api";
import LoginForm from "@/features/auth/components/LoginForm";
import { useAppDispatch } from "@/lib/redux/hooks";
import { loginSuccess, loginFailure } from "@/features/auth/auth.slice";
import { UserRoles } from "@/features/users/users.types";
import { useState } from "react";
import { Login } from "@/features/auth/auth.types";
import { ApiErrorResponse } from "@/lib/api/base.types";

export default function HomePage() {
  // Queries and hooks
  const [login, { isLoading: isLoggingIn }] = useLoginMutation();
  const [getMe] = useLazyGetMeQuery();
  const dispatch = useAppDispatch();

  // States
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const handleLoginSubmit = async (data: Login.Request) => {
    setErrorMessage(null);
    try {
      await login({
        username: data.username,
        password: data.password,
      }).unwrap();
      const profile = await getMe().unwrap();
      const userRole = profile.roles.includes("Admin")
        ? UserRoles.Admin
        : UserRoles.Staff;
      dispatch(
        loginSuccess({
          username: profile.userName,
          role: userRole,
          isFirstLogin: profile.isFirstLogin,
        }),
      );
    } catch (err: ApiErrorResponse | any) {
      console.error("Login failed error object:", err);
      const errorMsg =
        err?.detail ||
        err?.data?.detail ||
        err?.message ||
        "Username or password is incorrect. Please try again.";
      setErrorMessage(errorMsg);
      dispatch(loginFailure(errorMsg));
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
          errorMessage={errorMessage}
        />
      </div>
    </main>
  );
}
