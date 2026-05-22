"use client";

import { useAppDispatch, useAppSelector } from "@/lib/redux/hooks";
import { setDrawerOpen } from "@/features/shared/drawer.slice";

export default function DrawerCheckbox() {
  const isSidebarOpen = useAppSelector(
    (state) => state.drawerSlice.isDrawerOpen,
  );
  const dispatch = useAppDispatch();

  return (
    <input
      id="admin-drawer"
      type="checkbox"
      className="drawer-toggle"
      checked={isSidebarOpen}
      onChange={(e) => dispatch(setDrawerOpen(e.target.checked))}
    />
  );
}
