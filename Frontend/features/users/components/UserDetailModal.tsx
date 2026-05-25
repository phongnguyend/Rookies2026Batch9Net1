"use client";

import { useEffect, useRef, type MouseEvent } from "react";
import { useGetUserByIdQuery } from "@/features/users/users.api";
import { UserRoles, type UserDetail } from "@/features/users/users.types";

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

const formatDate = (value: string) => {
  if (!value) return "";

  const [year, month, day] = value.split("T")[0].split("-");
  if (!year || !month || !day) return value;

  return `${day}/${month}/${year}`;
};

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
    ["Staff Code", user.staffCode],
    ["Full Name", user.fullName],
    ["Username", user.userName],
    ["Date of Birth", formatDate(user.dateOfBirth)],
    ["Gender", user.gender],
    ["Joined Date", formatDate(user.joinedDate)],
    ["Type", user.userType],
    ["Location", user.location],
  ];

  return (
    <div
      className="absolute inset-0 z-20 flex min-h-[420px] items-center justify-center px-4 py-6"
      onClick={handleBackdropClick}
      role="dialog"
      aria-modal="true"
      aria-labelledby="user-detail-title"
    >
      <div
        ref={modalRef}
        className="w-full max-w-[570px] overflow-hidden rounded-[10px] border-2 border-[#777d84] bg-white text-[#6f7378] shadow-[0_1px_3px_rgba(0,0,0,0.28)]"
      >
        <div className="flex h-[73px] items-center justify-between border-b-2 border-[#777d84] bg-[#f3f6fa] px-6 sm:px-[58px]">
          <h2
            id="user-detail-title"
            className="text-[22px] font-bold leading-none text-primary"
          >
            Detailed User Information
          </h2>

          <button
            type="button"
            onClick={onClose}
            aria-label="Close detailed user information"
            className="flex h-[29px] w-[29px] items-center justify-center rounded-[5px] border-[3px] border-primary bg-white text-[26px] font-black leading-none text-primary transition hover:bg-red-50"
          >
            x
          </button>
        </div>

        <div className="min-h-[368px] bg-white px-6 pb-10 pt-7 sm:px-[58px]">
          {isFetching ? (
            <div className="pt-10 text-center text-lg text-[#6f7378]">
              Loading...
            </div>
          ) : isError ? (
            <div className="pt-10 text-center text-lg text-primary">
              Cannot load user information.
            </div>
          ) : (
            <dl className="grid grid-cols-[126px_1fr] gap-x-6 gap-y-[14px] text-[19px] leading-[1.45]">
              {details.map(([label, value]) => (
                <div key={label} className="contents">
                  <dt className="font-normal text-[#70757b]">{label}</dt>
                  <dd className="min-w-0 break-words text-[#70757b]">
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
