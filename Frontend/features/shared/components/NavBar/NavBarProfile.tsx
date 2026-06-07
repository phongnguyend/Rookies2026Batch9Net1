"use client";
import { useState } from "react";
import ChangePasswordModal from "@/features/auth/components/ChangePasswordModal";
import { useAppDispatch, useAppSelector } from "@/lib/redux/hooks";
import ConfirmModal from "../Modal/ConfirmModal";
import { useLogoutMutation } from "@/features/auth/auth.api";
import { logoutAccount } from "@/features/auth/auth.slice";
import { stopUserSessionHub } from "@/features/auth/user-session.signalr";

export default function NavbarProfile() {
  const { user, isLoading } = useAppSelector((state) => state.authSlice);
  const dispatch = useAppDispatch();

  const [isChangePasswordOpen, setIsChangePasswordOpen] = useState(false);
  const [isLogoutConfirmOpen, setIsLogoutConfirmOpen] = useState(false);

  const [logout, { isLoading: isLoggingOut }] = useLogoutMutation();

  const handleChangePassword = () => {
    setIsChangePasswordOpen(true);
  };

  const handleSignOut = () => {
    setIsLogoutConfirmOpen(true);
  };

  const handleConfirmLogout = async () => {
    try {
      await logout().unwrap();
    } catch (error) {
      console.error("Logout API failed:", error);
    } finally {
      await stopUserSessionHub();
      dispatch(logoutAccount());
      setIsLogoutConfirmOpen(false);
      window.location.replace("/");
    }
  };

  if (user == null || isLoading) {
    return;
  }

  return (
    <div className="flex flex-row space-x-2 align-center">
      <div className="hidden md:flex w-20 rounded-full bg-white text-primary items-center justify-center font-bold shadow-2xs">
        {user.role}
      </div>
      <div className="hidden md:flex w-auto px-3 rounded-full bg-white text-primary items-center justify-center font-bold shadow-2xs">
        {user.locationName}
      </div>
      <div className="dropdown dropdown-end" data-testid="mnuUserProfile">
        <div
          tabIndex={0}
          role="button"
          className="btn btn-ghost text-white text-base font-semibold normal-case flex items-center gap-1 hover:bg-white hover:text-primary"
        >
          <span data-testid="lblLocation">{user.username}</span>
          <svg
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
            strokeWidth={2.5}
            stroke="currentColor"
            className="w-4 h-4"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              d="m19.5 8.25-7.5 7.5-7.5-7.5"
            />
          </svg>
        </div>
        <ul
          tabIndex={0}
          className="dropdown-content menu bg-base-100 rounded-box z-100 w-42 p-2 shadow text-base-content mt-2 right-[-20px] border border-base-200"
        >
          <div className="px-3 py-2 text-xs select-text cursor-text md:hidden">
            Role: <span className="text-primary font-bold">{user.role}</span>
          </div>
          <div className="px-3 py-2 text-xs select-text cursor-text md:hidden">
            Location:{" "}
            <span className="text-primary font-bold">{user.locationName}</span>
          </div>

          {/* Separator */}
          <div className="border-t border-base-300 my-1 md:hidden" />

          <li>
            <button
              data-testid="mnuChangePassword"
              onClick={handleChangePassword}
              className="whitespace-nowrap font-semibold hover:bg-primary hover:text-white active:bg-primary/80 active:text-white w-full text-left"
            >
              Change Password
            </button>
          </li>

          {/* Logout  */}
          <li>
            <button
              data-testid="mnuLogout"
              onClick={handleSignOut}
              className="text-error font-semibold hover:bg-error hover:text-white active:bg-error/80 active:text-white w-full text-left"
            >
              Logout
            </button>
          </li>
        </ul>

        {/* Change Password Modal */}
        <ChangePasswordModal
          isOpen={isChangePasswordOpen}
          onClose={() => setIsChangePasswordOpen(false)}
        />

        {/* Confirm Logout Modal */}
        <ConfirmModal
          isOpen={isLogoutConfirmOpen}
          onClose={() => setIsLogoutConfirmOpen(false)}
          onYes={handleConfirmLogout}
          title="Are you sure?"
          body="Do you want to log out?"
          yesButtonLabel="Log out"
          noButtonLabel="Cancel"
          isLoading={isLoggingOut}
          size="sm"
          modalTestId="LogoutConfirmationModal"
          confirmBtnTestId="btnLogout"
          cancelBtnTestId="btnCancel"
        />
      </div>
    </div>
  );
}
