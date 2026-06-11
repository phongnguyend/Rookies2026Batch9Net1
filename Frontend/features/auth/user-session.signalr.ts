"use client";

import * as signalR from "@microsoft/signalr";
import { logoutAccount } from "@/features/auth/auth.slice";
import { authApi } from "./auth.api";
import { ENV_CONFIGS } from "@/lib/config/env";
import type { AppDispatch } from "@/lib/redux/store";
import { enqueueToast, ToastType } from "@/features/shared/toast.slice";
import { reportApi } from "@/features/report/report.api";
import dayjs from "dayjs";

const forceLogoutToastStorageKey = "forceLogoutToastMessage";
const defaultForceLogoutMessage =
  "Your account privilege has changed. Please login again.";

// Message for forcing logout
type ForceLogoutMessage = {
  reason?: string;
};

// Message for Report
type ReportReadyMessage = {
  completedAtUtc: string;
  downloadUrl: string;
};

let connection: signalR.HubConnection | null = null;
let isForceLoggingOut = false;

export const startUserSessionHub = async (dispatch: AppDispatch) => {
  if (connection?.state === signalR.HubConnectionState.Connected) {
    return;
  }

  connection = new signalR.HubConnectionBuilder()
    .withUrl(`${ENV_CONFIGS.apiUrl}/hubs/user-session`, {

      // backup sending accessToken in incognito mode
      accessTokenFactory: () => {
        return localStorage.getItem("accessToken") || "";
      },

      // cookie first
      withCredentials: true,
    })
    .withAutomaticReconnect()
    .build();

  connection.on("forceLogout", async (message?: ForceLogoutMessage) => {
    if (isForceLoggingOut) {
      return;
    }

    isForceLoggingOut = true;

    // keep the message alive and appears onthe login toast, after being redirect to the login page
    sessionStorage.setItem(
      forceLogoutToastStorageKey,
      message?.reason || defaultForceLogoutMessage,
    );

    // call logout endpoint by using imperative api to clear cookie
    dispatch(authApi.endpoints.logout.initiate());

    // clear user state
    dispatch(logoutAccount());

    // disconnect the signalr hub
    await stopUserSessionHub();
  });

  connection.on("reportReady", (message?: ReportReadyMessage) => {
    if (!message) {
      return;
    }

    const formattedDate = message.completedAtUtc
      ? dayjs(message.completedAtUtc).format("MMM D, YYYY HH:mm:ss")
      : "";

    dispatch(
      enqueueToast({
        message: `Report snapshot at ${formattedDate} is ready for downloading`,
        type: ToastType.Success,
      })
    );

    // Trigger manually to get the current State of the Report when reciving signal from Backend
    dispatch(
      reportApi.endpoints.getExportStatus.initiate(undefined, {
        subscribe: false,
        forceRefetch: true,
      })
    );
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
