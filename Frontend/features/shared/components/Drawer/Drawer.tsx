"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useAppDispatch } from "@/lib/redux/hooks";
import { setDrawerOpen } from "@/features/shared/drawer.slice";
import { APP_SIDEBAR_ADMIN_ITEMS, APP_SIDEBAR_STAFF_ITEMS } from "@/lib/api/routes";
import { AccountRole } from "@/features/accounts/accounts.types";
import Image from "next/image";

export interface DrawerProps {
  role: AccountRole;
}

const sideBarItems = (role = AccountRole.Admin): { label: string; href: string }[] => {
  if (role === AccountRole.Admin) {
    return APP_SIDEBAR_ADMIN_ITEMS;
  }

  return APP_SIDEBAR_STAFF_ITEMS;
};

export default function Drawer({ role }: DrawerProps) {
  const pathname = usePathname();
  const dispatch = useAppDispatch();

  return (
    <div className="min-h-full w-64 flex flex-col shadow-sm bg-white">
      {/* Brand Header */}
      <div className="md:pb-8 md:pl-0 flex flex-col gap-1 p-6">
        <div className="flex items-center">
          <Image quality={100} src={"/assets/images/nashtech_logo.png"} alt="NashTech Logo" className="w-20 h-auto" width={100} height={20} />
        </div>
        <div className="text-primary font-bold text-lg leading-snug">Online Asset Management</div>
      </div>

      {/* Navigation */}
      <nav className="flex flex-col">
        {sideBarItems(role).map((item, index) => {
          const isActive = pathname === item.href || (item.href !== "/admin" && pathname.startsWith(item.href));

          return (
            <Link
              key={item.href}
              href={item.href}
              onClick={() => dispatch(setDrawerOpen(false))}
              className={`
                px-6 py-3.5 transition-all duration-150 font-bold text-base block
                ${index === 0 ? "border-t border-white" : ""}
                border-b border-white
                ${isActive ? "bg-primary text-white" : "bg-[#eff1f5] hover:bg-base-300/80 text-neutral-800 hover:text-black"}
              `}>
              {item.label}
            </Link>
          );
        })}
      </nav>
    </div>
  );
}
