"use client";

import { Suspense, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { useAppDispatch } from "@/lib/redux/hooks";
import { showModal } from "@/features/shared/modal.slice";
import {
  useGetAssetsQuery,
  useGetCategoriesQuery,
} from "@/features/assets/assets.api";
import { AssetState, type AssetListItem } from "@/features/assets/assets.types";
import DataTable, {
  type ColumnDef,
} from "@/features/shared/components/DataTable";
import Pagination from "@/features/shared/components/Pagination";
import SearchInput from "@/features/shared/components/SearchInput";
import DropdownFilter from "@/features/shared/components/DropdownFilter";
import AssetDetailModal from "./AssetDetailModal";

const STATE_OPTIONS = Object.values(AssetState).map((s) => ({
  key: s,
  label: s,
}));

function AssetsContent() {
  const router = useRouter();
  const dispatch = useAppDispatch();
  const searchParams = useSearchParams();

  const [selectedAssetId, setSelectedAssetId] = useState<string | null>(null);

  // ─── Read from URL ─────────────────────────────
  const pageNumber = Number(searchParams.get("pageNumber") ?? "1");
  const selectedCategories = searchParams.getAll("categories");
  const selectedStates = searchParams.getAll("states") as AssetState[];

  // ─── Write to URL ──────────────────────────────
  const updateMultipleUrl = (key: string, values: string[]) => {
    const current = new URLSearchParams(searchParams.toString());
    current.delete(key);
    values.forEach((v) => current.append(key, v));
    current.set("pageNumber", "1");
    router.push(`?${current.toString()}`);
  };

  // ─── API ───────────────────────────────────────
  const { data: categoriesData, isLoading: categoriesLoading } =
    useGetCategoriesQuery();
  const { data, isLoading } = useGetAssetsQuery({
    pageNumber,
    pageSize: 10,
    categories: selectedCategories.length > 0 ? selectedCategories : undefined,
    states: selectedStates.length > 0 ? selectedStates : undefined,
  });

  const categoryOptions =
    categoriesData?.map((c) => ({
      key: c.name,
      label: c.name,
    })) ?? [];

  // ─── Handle Delete ─────────────────────────────
  const handleDelete = (row: AssetListItem, e: React.MouseEvent) => {
    e.stopPropagation(); // ← prevent row click from firing
    dispatch(
      showModal({
        title: "Delete Asset",
        body: `Are you sure you want to delete "${row.name}"?`,
        yesButtonLabel: "Delete",
        noButtonLabel: "Cancel",
        yesActionType: "asset/delete", // TODO: wire to actual delete handler
        yesPayload: row.id,
      }),
    );
  };

  // ─── Handle Edit ───────────────────────────────
  const handleEdit = (row: AssetListItem, e: React.MouseEvent) => {
    e.stopPropagation(); // ← prevent row click from firing
    router.push(``); // TODO: update route as needed
  };

  // ─── Columns ───────────────────────────────────
  const columns: ColumnDef<AssetListItem>[] = [
    { key: "assetCode", header: "Asset Code", sortable: true },
    { key: "name", header: "Asset Name", sortable: true },
    { key: "category", header: "Category", sortable: true },
    {
      key: "state",
      header: "State",
      sortable: true,
      render: (row) => <span className="badge badge-outline">{row.state}</span>,
    },
    {
      key: "",
      header: "Actions",
      render: (row) => (
        <div className="flex gap-2" onClick={(e) => e.stopPropagation()}>
          <button
            onClick={(e) => handleEdit(row, e)}
            className="btn btn-xs btn-outline"
          >
            {/* Pencil icon */}
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="14"
              height="14"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
              strokeLinecap="round"
              strokeLinejoin="round"
            >
              <path d="M17 3a2.85 2.83 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5Z" />
              <path d="m15 5 4 4" />
            </svg>
          </button>
          <button
            onClick={(e) => handleDelete(row, e)}
            className="btn btn-xs btn-error btn-outline"
          >
            {/* X Circle icon */}
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="14"
              height="14"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
              strokeLinecap="round"
              strokeLinejoin="round"
            >
              <circle cx="12" cy="12" r="10" />
              <path d="m15 9-6 6" />
              <path d="m9 9 6 6" />
            </svg>
          </button>
        </div>
      ),
    },
  ];

  return (
    <>
      {/* Detail Modal */}
      <AssetDetailModal
        assetId={selectedAssetId}
        onClose={() => setSelectedAssetId(null)}
      />

      {/* Filters */}
      <div className="mb-4 flex items-center gap-3">
        <DropdownFilter
          items={categoryOptions}
          values={selectedCategories}
          placeholder={categoriesLoading ? "Loading..." : "Category"}
          getKey={(item) => item.key}
          getLabel={(item) => item.label}
          onChange={(values) => updateMultipleUrl("categories", values)}
          allLabel="All Categories"
        />

        <DropdownFilter
          items={STATE_OPTIONS}
          values={selectedStates}
          placeholder="State"
          getKey={(item) => item.key}
          getLabel={(item) => item.label}
          onChange={(values) => updateMultipleUrl("states", values)}
          allLabel="All States"
        />

        <div className="ml-auto">
          <SearchInput
            value={searchParams.get("search") ?? ""}
            onChange={() => {}}
            onSearch={(value) => {
              const current = new URLSearchParams(searchParams.toString());
              if (value) current.set("search", value);
              else current.delete("search");
              current.set("pageNumber", "1");
              router.push(`?${current.toString()}`);
            }}
            placeholder="Search assets..."
          />
        </div>

        <button
          onClick={() => router.push("")} // TODO: update route as needed
          className="btn btn-primary btn-sm"
        >
          + Create New Asset
        </button>
      </div>

      {/* Table */}
      <DataTable<AssetListItem>
        data={data?.items ?? []}
        columns={columns}
        isLoading={isLoading}
        emptyMessage="No assets found."
        onRowClick={(row) => setSelectedAssetId(row.id)} // ← opens detail modal
      />

      {/* Pagination */}
      {data && (
        <Pagination
          pageNumber={data.pageNumber}
          totalPages={data.totalPages}
          totalCount={data.totalCount}
          pageSize={data.pageSize}
          hasPreviousPage={data.hasPreviousPage}
          hasNextPage={data.hasNextPage}
          onPageChange={(page) => {
            const current = new URLSearchParams(searchParams.toString());
            current.set("pageNumber", String(page));
            router.push(`?${current.toString()}`);
          }}
        />
      )}
    </>
  );
}

export default function AssetsPage() {
  return (
    <div className="p-6">
      <Suspense fallback={<div>Loading...</div>}>
        <AssetsContent />
      </Suspense>
    </div>
  );
}
