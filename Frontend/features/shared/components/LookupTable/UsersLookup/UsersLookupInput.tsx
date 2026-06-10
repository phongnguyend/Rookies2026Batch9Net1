"use client";

import { useEffect, useState } from "react";
import { UsersLookupTable } from "./UsersLookupTable";
import { FocusTrap } from "focus-trap-react";
import { LookupUsers } from "@/features/users/users.types";
import { Search } from "lucide-react";

export interface UsersLookupInputProps {
  value?: LookupUsers.LookupUsersSummary | null;
  onChange?: (user: LookupUsers.LookupUsersSummary | null) => void;
  data_testid?: string;
  placeholder?: string;
}

const UsersLookupInput = ({
  value,
  onChange,
  placeholder = "Select a user",
  data_testid = "txtUserLookupInput",
}: UsersLookupInputProps) => {
  const [isOpen, setIsOpen] = useState(false);
  // pendingUser: in-dialog selection only, never drives the input display
  const [pendingUser, setPendingUser] =
    useState<LookupUsers.LookupUsersSummary | null>(null);

  const handleOpen = () => {
    // pre-check the currently committed user
    setPendingUser(value ?? null);
    setIsOpen(true);
  };

  const handleSave = (user: LookupUsers.LookupUsersSummary | null) => {
    // commit: parent updates value prop → input reflects new name
    onChange?.(user);
    setIsOpen(false);
    setPendingUser(null);
  };

  const handleClose = () => {
    setIsOpen(false);
    // discard dialog selection, input stays unchanged
    setPendingUser(null);
  };

  useEffect(() => {
    if (!isOpen) return;

    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === "Escape") handleClose();
    };

    document.addEventListener("keydown", handleKeyDown);
    return () => document.removeEventListener("keydown", handleKeyDown);
  }, [isOpen]);

  return (
    <div className="relative">
      <div className="flex flex-col gap-1 md:flex-row md:items-center md:gap-5">
        <label className="text-sm font-medium text-gray-700 md:w-28 md:shrink-0">
          User
        </label>
        <div className="flex-1" data-testid={data_testid}>
          <div
            role="button"
            tabIndex={0}
            onClick={handleOpen}
            onKeyDown={(e) => {
              if ((e.key === "Enter" || e.key === " ") && !isOpen) {
                e.preventDefault();
                handleOpen();
              }
            }}
            className="h-9 flex items-center justify-between w-full px-3 py-2 border border-gray-300 rounded cursor-pointer bg-white hover:border-primary focus:outline-none focus:ring-2 focus:ring-primary/30 transition-colors"
          >
            {/* Input display is driven by value prop only, never pendingUser */}
            {value ? (
              <span className="text-neutral-800">{value.fullName}</span>
            ) : (
              <span className="text-gray-400">{placeholder}</span>
            )}
            <Search className="w-4 h-4 text-gray-400 shrink-0 ml-2" />
          </div>
        </div>

        {/* Dialog overlay */}
        {isOpen && (
          <FocusTrap
            focusTrapOptions={{
              escapeDeactivates: false, // we handle Escape ourselves above
              allowOutsideClick: true,  // lets the backdrop click still close it
            }}
          >
            <div
              className="fixed inset-0 z-50 bg-black/40 flex items-end sm:items-center justify-center sm:p-4"
              onClick={handleClose}
            >
              <div
                className="bg-white rounded-t-2xl sm:rounded-lg shadow-xl w-full sm:max-w-3xl flex flex-col max-h-[90dvh]"
                onClick={(e) => e.stopPropagation()}
                onKeyDown={(e) => {
                  e.stopPropagation();
                  if (e.key === "Enter" || e.key === " ") e.preventDefault(); // ← ngăn form submit
                }}
              >
                <UsersLookupTable
                  isOpen={isOpen}
                  onConfirm={handleSave}
                  onClose={handleClose}
                  pendingUser={pendingUser}
                  onPendingUserChange={setPendingUser}
                />
              </div>
            </div>
          </FocusTrap>
        )}
      </div>
    </div>
  );
};

export default UsersLookupInput;
