"use client";

import * as signalR from "@microsoft/signalr";
import { authApi } from "@/features/auth/auth.api";
import { logoutAccount } from "@/features/auth/auth.slice";
import { ENV_CONFIGS } from "@/lib/config/env";
import type { AppDispatch } from "@/lib/redux/store";

const forceLogoutToastStorageKey = "forceLogoutToastMessage";
const defaultForceLogoutMessage =
  "Your account privilege has changed. Please login again.";

type ForceLogoutMessage = {
  reason?: string;
};

let connection: signalR.HubConnection | null = null;
let isForceLoggingOut = false;

export const startUserSessionHub = async (dispatch: AppDispatch) => {
  if (connection?.state === signalR.HubConnectionState.Connected) {
    return;
  }

  connection = new signalR.HubConnectionBuilder()
    .withUrl(`${ENV_CONFIGS.apiUrl}/hubs/user-session`, {
      withCredentials: true,
    })
    .withAutomaticReconnect()
    .build();

  connection.on("forceLogout", async (message?: ForceLogoutMessage) => {
    if (isForceLoggingOut) {
      return;
    }

    isForceLoggingOut = true;
    sessionStorage.setItem(
      forceLogoutToastStorageKey,
      message?.reason || defaultForceLogoutMessage,
    );

    try {
      await dispatch(authApi.endpoints.logout.initiate()).unwrap();
    } catch (error) {
      console.error("Forced logout API failed:", error);
    } finally {
      dispatch(logoutAccount());
      localStorage.removeItem("accessToken");
      localStorage.removeItem("refreshToken");

      await stopUserSessionHub();
      window.location.replace("/");
    }
  });

  try {
    await connection.start();
  } catch (error) {
    console.error("SignalR connection failed:", error);
  }
};

export const stopUserSessionHub = async () => {
  if (!connection) {
    return;
  }

  try {
    await connection.stop();
  } finally {
    connection = null;
    isForceLoggingOut = false;
  }
};
