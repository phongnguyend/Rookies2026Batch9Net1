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
      className="fixed inset-0 z-50 flex min-h-screen items-center justify-center px-4 py-6"
      onClick={handleBackdropClick}
      role="dialog"
      aria-modal="true"
      aria-labelledby="user-detail-title"
    >
      <div
        ref={modalRef}
        className="w-full max-w-[calc(100vw-2rem)] overflow-hidden rounded-[10px] border-2 border-[#777d84] bg-white text-[#6f7378] shadow-[0_1px_3px_rgba(0,0,0,0.28)] sm:max-w-[570px]"
      >
        <div className="flex min-h-[64px] items-center justify-between gap-4 border-b-2 border-[#777d84] bg-[#f3f6fa] px-5 py-4 sm:min-h-[73px] sm:px-[58px]">
          <h2
            id="user-detail-title"
            className="text-lg font-bold leading-tight text-primary sm:text-[22px]"
          >
            Detailed User Information
          </h2>

          <button
            type="button"
            onClick={onClose}
            aria-label="Close detailed user information"
            className="flex h-[29px] w-[29px] items-center justify-center rounded-[5px] border-[3px] border-primary bg-white pb-[2px] text-[22px] font-bold leading-none text-primary transition hover:bg-red-50"
            data-testid="btnCloseUserDetail"
          >
            &times;
          </button>
        </div>

        <div className="max-h-[calc(100vh-11rem)] overflow-y-auto bg-white px-5 pb-8 pt-6 sm:min-h-[368px] sm:px-[58px] sm:pb-10 sm:pt-7">
          {isFetching ? (
            <div className="pt-10 text-center text-lg text-[#6f7378]">
              Loading...
            </div>
          ) : isError ? (
            <div className="pt-10 text-center text-lg text-primary">
              Cannot load user information.
            </div>
          ) : (
            <dl className="grid grid-cols-[minmax(96px,0.42fr)_1fr] gap-x-4 gap-y-3 text-base leading-[1.45] sm:grid-cols-[126px_1fr] sm:gap-x-6 sm:gap-y-[14px] sm:text-[19px]">
              {details.map(([label, value, testId]) => (
                <div key={label} className="contents">
                  <dt className="font-normal text-[#70757b]">{label}</dt>
                  <dd
                    className="min-w-0 break-words text-[#70757b]"
                    data-testid={testId}
                  >
                    {value || "-"}
                  </dd>
                </div>
              ))}
            </dl>
          )}
        </div>
      </div>
    </div>
  );
}
