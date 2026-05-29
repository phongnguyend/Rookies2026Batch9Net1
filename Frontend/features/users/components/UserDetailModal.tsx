"use client";

import { useEffect, useRef, type MouseEvent } from "react";
import { useGetUserByIdQuery } from "@/features/users/users.api";
import { UserRoles, type UserDetail } from "@/features/users/users.types";
import { formatDate } from "@/utils/datetime.utils";

interface UserDetailModalProps {
  userId: string | null;
  isOpen: boolean;
  onClose: () => void;
}

const emptyUserDetail: UserDetail = {
  id: "",
  staffCode: "",
  fullName: "",
  userName: "",
  joinedDate: "",
  userType: UserRoles.Staff,
  dateOfBirth: "",
  gender: "",
  location: "",
};

const formatUserDate = (value: string) => (value ? formatDate(value) : "");

const Field = ({
  label,
  value,
  testId,
}: {
  label: string;
  value?: string;
  testId: string;
}) => (
  <div className="flex gap-4">
    <span className="w-24 shrink-0 text-gray-500">{label}</span>
    <span className="wrap-break-word" data-testid={testId}>
      {value || "-"}
    </span>
  </div>
);

export default function UserDetailModal({
  userId,
  isOpen,
  onClose,
}: UserDetailModalProps) {
  const modalRef = useRef<HTMLDivElement>(null);
  const { data, isFetching, isError } = useGetUserByIdQuery(userId ?? "", {
    skip: !isOpen || !userId,
  });
  const user = data ?? emptyUserDetail;

  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key === "Escape" && isOpen) {
        onClose();
      }
    };

    window.addEventListener("keydown", handleKeyDown);
    return () => window.removeEventListener("keydown", handleKeyDown);
  }, [isOpen, onClose]);

  useEffect(() => {
    if (isOpen) {
      document.body.style.overflow = "hidden";
    } else {
      document.body.style.overflow = "";
    }

    return () => {
      document.body.style.overflow = "";
    };
  }, [isOpen]);

  const handleBackdropClick = (event: MouseEvent<HTMLDivElement>) => {
    if (modalRef.current && !modalRef.current.contains(event.target as Node)) {
      onClose();
    }
  };

  if (!isOpen) return null;

  const details = [
    ["Staff Code", user.staffCode, "lblStaffCodeDetail"],
    ["Full Name", user.fullName, "lblFullNameDetail"],
    ["Username", user.userName, "lblUsernameDetail"],
    ["Date of Birth", formatUserDate(user.dateOfBirth), "lblDateOfBirthDetail"],
    ["Gender", user.gender, "lblGenderDetail"],
    ["Joined Date", formatUserDate(user.joinedDate), "lblJoinedDateDetail"],
    ["Type", user.userType, "lblUserTypeDetail"],
    ["Location", user.location, "lblLocationDetail"],
  ];

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-black/40"
      onClick={handleBackdropClick}
      role="dialog"
      aria-modal="true"
      aria-labelledby="user-detail-title"
    >
      <div
        ref={modalRef}
        className="relative w-full max-w-xl overflow-hidden rounded-lg border bg-white shadow-lg"
      >
        <div className="flex items-center justify-between border-b border-gray-300 bg-gray-200 px-10 py-4">
          <h2
            id="user-detail-title"
            className="text-lg font-semibold text-primary"
          >
            Detailed User Information
          </h2>

          <button
            type="button"
            onClick={onClose}
            aria-label="Close detailed user information"
            className="rounded border-3 border-primary px-2 py-0.5 text-sm font-semibold text-primary hover:cursor-pointer hover:font-bold"
            data-testid="btnCloseUserDetail"
          >
            x
          </button>
        </div>

        <div className="max-h-[calc(100vh-8rem)] overflow-y-auto px-10 py-4">
          {isFetching ? (
            <div className="py-8 text-center text-gray-400">
              Loading...
            </div>
          ) : isError ? (
            <div className="py-8 text-center text-primary">
              Cannot load user information.
            </div>
          ) : (
            <div className="space-y-3 text-sm">
              {details.map(([label, value, testId]) => (
                <Field
                  key={label}
                  label={label}
                  value={value}
                  testId={testId}
                />
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
