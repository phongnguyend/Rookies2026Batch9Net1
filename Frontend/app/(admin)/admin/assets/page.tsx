"use client";

import { Suspense, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { useAppDispatch } from "@/lib/redux/hooks";
import { showModal } from "@/features/shared/modal.slice";
import {
  useGetAssetsQuery,
  useGetCategoriesQuery,
} from "@/features/Assets/assets.api";
import { AssetState, type AssetListItem } from "@/features/Assets/assets.types";
import Pagination from "@/features/shared/components/Pagination";
import SearchInput from "@/features/shared/components/SearchInput";
import DropdownFilter from "@/features/shared/components/DropdownFilter";
import AssetDetailModal from "@/features/Assets/components/assetDetailModal";
import DataTable, {
  ColumnDef,
  SortItem,
} from "@/features/Assets/components/assetDataTable";
import DropdownStateFilter from "@/features/Assets/components/stateDtopdown";

const STATE_OPTIONS = Object.values(AssetState).map((s) => ({
  key: s,
  label: s,
}));

function AssetsContent() {
  const router = useRouter();
  const dispatch = useAppDispatch();
  const searchParams = useSearchParams();
  const search = searchParams.get("search") ?? undefined;
  const [sort, setSort] = useState<SortItem | null>(null);
  const [selectedAssetId, setSelectedAssetId] = useState<string | null>(null);
  const [searchInput, setSearchInput] = useState(search ?? "");
  const [EditDisabledStates] = useState(false);
  const [DeleteDisabledStates] = useState(false);

  // ─── Read from URL ─────────────────────────────
  const pageNumber = Number(searchParams.get("pageNumber") ?? "1");
  const selectedCategories = searchParams.getAll("categories");
  const stateParams = searchParams.getAll("states") as AssetState[];

  // ─── Requirement for default View ─────────────────────────────
  const isFirstLoad = !searchParams.has("states");

  const defaultStates = [
      AssetState.Available,
      AssetState.NotAvailable,
      AssetState.Assigned,
    ];

  const selectedStates = isFirstLoad
    ? defaultStates
    : stateParams;

  const isDefaultStateSelection =
    selectedStates.length === defaultStates.length &&
    defaultStates.every((s) => selectedStates.includes(s));

  // ─── Write to URL ──────────────────────────────
  const updateMultipleUrl = (key: string, values: string[]) => {
    const current = new URLSearchParams(searchParams.toString());
    current.delete(key);
    values.forEach((v) => current.append(key, v));
    current.set("pageNumber", "1");
    router.push(`?${current.toString()}`);
  };

  // ─── API ───────────────────────────────────────
  const {
    data: categoriesData,
    isLoading: categoriesLoading,
    isError,
  } = useGetCategoriesQuery();
  const { data, isLoading } = useGetAssetsQuery({
    pageNumber,
    pageSize: 10,
    categories: selectedCategories.length > 0 ? selectedCategories : undefined,
    states: selectedStates.length > 0 ? selectedStates : undefined,
    search,
    sortBy: sort?.key,
    sortDirection: sort?.direction,
  });

  const categoryOptions =
    categoriesData?.map((c) => ({
      key: c.id,
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
        yesActionType: "", // TODO: wire to actual delete handler
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
    {
      key: "assetCode",
      header: "Asset Code",
      sortable: true,
      testId: "btnSortAssetCode",
    },
    {
      key: "name",
      header: "Asset Name",
      sortable: true,
      testId: "btnSortAssetName",
    },
    {
      key: "category",
      header: "Category",
      sortable: true,
      testId: "btnSortCategory",
    },
    {
      key: "state",
      header: "State",
      sortable: true,
      testId: "btnSortState",
      render: (row) => <span className="badge badge-outline">{row.state}</span>,
    },
    {
      key: "",
      header: "Actions",
      render: (row) => (
        <div className="flex gap-2" onClick={(e) => e.stopPropagation()}>
          <button
            disabled={
              EditDisabledStates
                ? EditDisabledStates
                : row.state === AssetState.Assigned
            }
            data-testid="btnEdit"
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
            data-testid="btnIconDelete"
            onClick={(e) => handleDelete(row, e)}
            className="btn btn-xs btn-error btn-outline"
            disabled={
              DeleteDisabledStates
                ? DeleteDisabledStates
                : row.state === AssetState.Assigned
            }
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
        <div data-testid="ddlState">
          <DropdownStateFilter
            items={STATE_OPTIONS}
            values={selectedStates}
            getKey={(item) => item.key}
            getLabel={(item) => item.label}
            onChange={(values) => updateMultipleUrl("states", values)}
            customLabel={isDefaultStateSelection ? "State" : undefined}
          />
        </div>
        <div data-testid="ddlCategory">
          <DropdownFilter
            items={categoryOptions}
            values={selectedCategories}
            placeholder={categoriesLoading ? "Loading..." : "Category"}
            getKey={(item) => item.key}
            getLabel={(item) => item.label}
            onChange={(values) => updateMultipleUrl("categories", values)}
            allLabel="All Categories"
          />
        </div>
        <div className="ml-auto" data-testid="txtSearch">
          <SearchInput
            value={searchInput}
            onChange={setSearchInput}
            onSearch={(value) => {
              const trimmedValue = value.trim();
              setSearchInput(trimmedValue);
              const current = new URLSearchParams(searchParams.toString());
              if (trimmedValue) {
                current.set("search", trimmedValue);
              } else {
                current.delete("search");
              }
              current.set("pageNumber", "1");
              router.push(`?${current.toString()}`);
            }}
            placeholder="Search by asset code or name..."
          />
        </div>
        <button
          data-testid="btnCreateAsset"
          onClick={() => router.push("")} // TODO: update route as needed
          className="btn btn-primary btn-sm"
        >
          + Create New Asset
        </button>
      </div>

      {/* Table */}
      <div data-testid="dgdAsset">
        <DataTable<AssetListItem>
          data={data?.items ?? []}
          columns={columns}
          isLoading={isLoading}
          emptyMessage={
            isError ? "No assets found." : "No assets found after filtering."
          }
          onRowClick={(row) => setSelectedAssetId(row.id)}
          sort={sort}
          onSortChange={setSort}
        />
      </div>

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
    <div>
      <h1 className="text-primary font-bold text-xl mb-6">List Asset</h1>
      <Suspense fallback={<div>Loading...</div>}>
        <AssetsContent />
      </Suspense>
    </div>
  );
}
