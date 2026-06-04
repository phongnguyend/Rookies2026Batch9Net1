"use client";

import { useState } from "react";
import { UsersLookupTable } from "./UsersLookupTable";
import { LookupUsers } from "@/features/users/users.types";
import { Search } from "lucide-react";

export interface UsersLookupInputProps {
  value?: LookupUsers.LookupUsersSummary | null;
  onChange?: (user: LookupUsers.LookupUsersSummary | null) => void;
  placeholder?: string;
}

const UsersLookupInput = ({
  value,
  onChange,
  placeholder = "Select a user",
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

  return (
    <div className="relative">
      <div className="flex flex-col gap-2 md:flex-row md:items-center md:gap-4">
        <label className="text-sm font-medium text-gray-700 md:w-32 md:shrink-0">
          User
        </label>
        <div className="flex-1">
          <div
            role="button"
            tabIndex={0}
            onClick={handleOpen}
            onKeyDown={(e) => e.key === "Enter" && handleOpen()}
            className="flex items-center justify-between w-full px-3 py-2 border border-gray-300 rounded cursor-pointer bg-white hover:border-primary focus:outline-none focus:ring-2 focus:ring-primary/30 transition-colors"
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
          <div
            className="fixed inset-0 z-50 flex items-center justify-center bg-black/40"
            onClick={handleClose}
          >
            <div
              className="bg-white rounded-lg shadow-xl p-6 w-full max-w-3xl mx-4 max-h-[90vh]"
              onClick={(e) => e.stopPropagation()}
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
        )}
      </div>
    </div>
  );
};

export default UsersLookupInput;
