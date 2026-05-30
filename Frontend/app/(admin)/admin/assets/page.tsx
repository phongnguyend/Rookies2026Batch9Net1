"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAppDispatch, useAppSelector } from "@/lib/redux/hooks";
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
import DropdownStateFilter from "@/features/Assets/components/stateDropdown";
import { setCreatedNewAsset } from "@/features/Assets/assets.slice";

const state_options = Object.values(AssetState).map((s) => ({
  key: s,
  label: s,
}));

const default_states = [
  AssetState.Available,
  AssetState.NotAvailable,
  AssetState.Assigned,
];

function AssetsContent() {
  const router = useRouter();
  const dispatch = useAppDispatch();

  // ─── All state via useState ────────────────────
  const [pageNumber, setPageNumber] = useState(1);
  const [search, setSearch] = useState("");
  const [searchInput, setSearchInput] = useState("");
  const [selectedCategories, setSelectedCategories] = useState<string[]>([]);
  const [selectedStates, setSelectedStates] =
    useState<AssetState[]>(default_states);
  const [sort, setSort] = useState<SortItem | null>(null);
  const [selectedAssetId, setSelectedAssetId] = useState<string | null>(null);
  const [EditDisabledStates] = useState(false);
  const [DeleteDisabledStates] = useState(false);
  const isCreatedNewAsset = useAppSelector((state)=>state.asset.isCreatedNewAsset);

  // ─── Default state check ───────────────────────
  const isDefaultStateSelection =
    selectedStates.length === default_states.length &&
    default_states.every((s) => selectedStates.includes(s));

  // ─── API ───────────────────────────────────────
  const {
    data: categoriesData,
    isLoading: categoriesLoading,
    isError,
  } = useGetCategoriesQuery();
  useEffect(() => {
    if (isCreatedNewAsset) {
      dispatch(setCreatedNewAsset(false));
    }
  }, [isCreatedNewAsset, dispatch]);
  const { data, isLoading } = useGetAssetsQuery({
    pageNumber,
    pageSize: 10,
    categories: selectedCategories.length > 0 ? selectedCategories : undefined,
    states: selectedStates.length > 0 ? selectedStates : undefined,
    search: search || undefined,
    sortBy: sort?.key,
    sortDirection: sort?.direction,
    isCreatedNewAsset,
  });

  const displayItems = data?.items ?? [];

  const categoryOptions =
    categoriesData?.map((c) => ({
      key: c.name,
      label: c.name,
    })) ?? [];

  // ─── Handlers ─────────────────────────────────
  const handleStateChange = (values: string[]) => {
    setSelectedStates(values as AssetState[]);
    setPageNumber(1);
  };

  const handleCategoryChange = (values: string[]) => {
    setSelectedCategories(values);
    setPageNumber(1);
  };

  const handleSearch = (value: string) => {
    const trimmed = value.trim();
    setSearch(trimmed);
    setSearchInput(trimmed);
    setPageNumber(1);
  };

  const handleSortChange = (newSort: SortItem | null) => {
    setSort(newSort);
    setPageNumber(1);
  };

  const handleDelete = (row: AssetListItem, e: React.MouseEvent) => {
    e.stopPropagation();
    dispatch(
      showModal({
        title: "Delete Asset",
        body: `Are you sure you want to delete "${row.name}"?`,
        yesButtonLabel: "Delete",
        noButtonLabel: "Cancel",
        yesActionType: "",
        yesPayload: row.id,
      }),
    );
  };

  const handleEdit = (row: AssetListItem, e: React.MouseEvent) => {
    e.stopPropagation();
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
      <AssetDetailModal
        assetId={selectedAssetId}
        onClose={() => setSelectedAssetId(null)}
      />

      {/* Filters */}
      <div className="mb-4 flex flex-col gap-3 lg:flex-row lg:items-center">
        {/* Left group */}
        <div className="flex flex-wrap gap-3">
          <div className="flex-1 min-w-[160px] sm:flex-none">
            <DropdownStateFilter
              items={state_options}
              values={selectedStates}
              defaultValue={default_states}
              getKey={(item) => item.key}
              getLabel={(item) => item.label}
              onChange={handleStateChange}
              customLabel={isDefaultStateSelection ? "State" : undefined}
            />
          </div>
          <div
            data-testid="ddlCategory"
            className="flex-1 min-w-[160px] sm:flex-none"
          >
            <DropdownFilter
              items={categoryOptions}
              values={selectedCategories}
              placeholder={categoriesLoading ? "Loading..." : "Category"}
              getKey={(item) => item.key}
              getLabel={(item) => item.label}
              onChange={handleCategoryChange}
              allLabel="All Categories"
            />
          </div>
        </div>

        {/* Right group */}
        <div className="flex flex-col gap-3 sm:flex-row sm:items-center lg:ml-auto">
          <div data-testid="txtSearch" className="w-full sm:w-60">
            <SearchInput
              value={searchInput}
              onChange={setSearchInput}
              onSearch={handleSearch}
              placeholder="Search by asset code or name..."
              width="w-full"
            />
          </div>
          <button
            data-testid="btnCreateAsset"
            onClick={() => router.push("/admin/assets/create")}
            className="btn btn-primary btn-sm w-full sm:w-auto whitespace-nowrap"
          >
            + Create New Asset
          </button>
        </div>
      </div>

      {/* Table */}
      <div data-testid="dgdAsset">
        <DataTable<AssetListItem>
          data={displayItems}
          columns={columns}
          isLoading={isLoading}
          emptyMessage={
            isError ? "No assets found." : "No assets found after filtering."
          }
          onRowClick={(row) => setSelectedAssetId(row.id)}
          sort={sort}
          onSortChange={handleSortChange}
        />
      </div>

      {/* Pagination */}
      {data && (
        <Pagination
          pageNumber={data.pageNumber}
          totalPages={data.totalPages}
          totalCount={displayItems.length}
          pageSize={data.pageSize}
          hasPreviousPage={data.hasPreviousPage}
          hasNextPage={data.hasNextPage}
          onPageChange={setPageNumber}
        />
      )}
    </>
  );
}

export default function AssetsPage() {
  return (
    <div className="p-4 sm:p-6">
      <h1 className="text-primary font-bold text-xl mb-6">List Asset</h1>
      <AssetsContent />
    </div>
  );
}
