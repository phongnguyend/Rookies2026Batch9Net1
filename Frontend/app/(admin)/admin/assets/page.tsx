"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAppSelector } from "@/lib/redux/hooks";
import DeleteAssetModal from "@/features/Assets/components/assetDeleteModel";
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
import {
  clearPinnedEditedAsset,
  getPinnedEditedAsset,
} from "@/features/Assets/editAssetStore";
import { CircleX, Pencil } from "lucide-react";
import { displayAssetState } from "@/utils/asset.utils";

const state_options = Object.values(AssetState).map((s) => ({
  key: s,
  label: displayAssetState(s),
}));

const default_states = [
  AssetState.Available,
  AssetState.NotAvailable,
  AssetState.Assigned,
];

function AssetsContent() {
  const router = useRouter();

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
  const isCreatedNewAsset = useAppSelector(
    (state) => state.asset.isCreatedNewAsset,
  );
  const [deleteTarget, setDeleteTarget] = useState<{
    id: string;
    name: string;
    hasHistory: boolean;
  } | null>(null);

  // ─── Default state check ───────────────────────
  const isDefaultStateSelection =
    selectedStates.length === default_states.length &&
    default_states.every((s) => selectedStates.includes(s));

  // ─── API ───────────────────────────────────────
  const { data: categoriesData, isLoading: categoriesLoading } =
    useGetCategoriesQuery();

  const { data, isLoading } = useGetAssetsQuery(
    {
      pageNumber,
      pageSize: 10,
      categories:
        selectedCategories.length > 0 ? selectedCategories : undefined,
      states: selectedStates.length > 0 ? selectedStates : undefined,
      search: search || undefined,
      sortBy: sort?.key,
      sortDirection: sort?.direction,
      isCreatedNewAsset,
    },
    {
      refetchOnMountOrArgChange: true,
    },
  );

  // ─── Display Item ───────────────────────────────────────
  // ─── Read pinned edited asset on mount ─────────
  const [pinnedEditedAsset] = useState<AssetListItem | null>(() =>
    getPinnedEditedAsset(),
  );

  // ─── Clear when user leaves assets page ────────
  useEffect(() => {
    if (!data) return;
    clearPinnedEditedAsset();
  });

  const displayItems = (() => {
    const items = data?.items ?? [];

    const isDefaultFilter =
      selectedCategories.length === 0 &&
      search === "" &&
      sort === null &&
      isDefaultStateSelection;

    if (!isDefaultFilter || !pinnedEditedAsset) {
      return items;
    }

    const filteredItems = items.filter(
      (item) => item.assetCode !== pinnedEditedAsset.assetCode,
    );

    return pageNumber === 1
      ? [pinnedEditedAsset, ...filteredItems]
      : filteredItems;
  })();

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
  };

  const handleEdit = (row: AssetListItem, e: React.MouseEvent) => {
    e.stopPropagation();
    router.push(`/admin/assets/edit?id=${row.id}`);
  };

  // ─── Columns ───────────────────────────────────
  const columns: ColumnDef<AssetListItem>[] = [
    {
      key: "assetCode",
      header: "Asset Code",
      sortable: true,
      testId: "btnSortAssetCode",
      className: "w-32",
    },
    {
      key: "name",
      header: "Asset Name",
      sortable: true,
      testId: "btnSortAssetName",
      className: "w-64",
    },
    {
      key: "category",
      header: "Category",
      sortable: true,
      testId: "btnSortCategory",
      className: "w-40",
    },
    {
      key: "state",
      header: "State",
      sortable: true,
      testId: "btnSortState",
      className: "w-32",
    },
    {
      key: "actions",
      header: "",
      className: "w-28",
      render: (row) => {
        return (
          <div className="flex items-center gap-3" onClick={(e) => e.stopPropagation()}>
            <button
              disabled={
                EditDisabledStates
                  ? EditDisabledStates
                  : row.state === AssetState.Assigned
              }
              data-testid="btnEdit"
              onKeyDown={(e) => e.stopPropagation()}
              onClick={(e) => handleEdit(row, e)}
              className="disabled:cursor-not-allowed disabled:opacity-30 cursor-pointer lucide lucide-pencil text-gray-500"
              title="Edit"
            >
              <Pencil size={20} />
            </button>
            <button
              data-testid="btnIconDelete"
              onKeyDown={(e) => e.stopPropagation()}
              onClick={(e) => {
                e.stopPropagation();

                setDeleteTarget({
                  id: row.id,
                  name: row.name,
                  hasHistory: row.hasHistory,
                });
              }}
              className="text-red-400 disabled:cursor-not-allowed disabled:opacity-30 cursor-pointer"
              disabled={
                DeleteDisabledStates
                  ? DeleteDisabledStates
                  : row.state === AssetState.Assigned
              }
              title="Delete"
            >
              <CircleX size={20} />
            </button>
          </div>
        );
      },
    },
  ];

  return (
    <>
      <AssetDetailModal
        assetId={selectedAssetId}
        onClose={() => setSelectedAssetId(null)}
      />
      <DeleteAssetModal
        assetId={deleteTarget?.id ?? null}
        assetName={deleteTarget?.name ?? ""}
        hasHistory={deleteTarget?.hasHistory ?? false}
        onClose={() => setDeleteTarget(null)}
      />
      <div>
        {/* Filters */}
        <div className="mb-4 flex flex-col gap-3 lg:flex-row lg:items-center">
          {/* Left filters */}
          <div className="flex flex-wrap flex-col gap-3 lg:flex-row lg:items-center">
            <div className="w-full sm:w-auto">
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

            <div data-testid="ddlCategory" className="w-full sm:w-auto">
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

          {/* Search + button */}
          <div className="flex flex-wrap gap-3 lg:items-center lg:ml-auto flex-col lg:flex-row">
            <div data-testid="txtSearch" className="w-full sm:w-auto">
              <SearchInput
                value={searchInput}
                onChange={setSearchInput}
                onSearch={handleSearch}
                placeholder="Search..."
              />
            </div>

            <button
              data-testid="btnCreateAsset"
              onClick={() => router.push("/admin/assets/create")}
              className="flex items-center justify-center h-9 hover:bg-red-600 w-full sm:w-64 rounded bg-primary px-5 py-2 font-semibold text-white whitespace-nowrap text-sm sm:text-base cursor-pointer"
            >
              Create new asset
            </button>
          </div>
        </div>

        {/* Table */}
        <div data-testid="dgdAsset">
          <DataTable<AssetListItem>
            data={displayItems}
            columns={columns}
            isLoading={isLoading}
            emptyMessage="No assets found."
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
            totalCount={data.totalCount}
            pageSize={data.pageSize}
            hasPreviousPage={data.hasPreviousPage}
            hasNextPage={data.hasNextPage}
            onPageChange={setPageNumber}
          />
        )}
      </div>
    </>
  );
}

export default function AssetsPage() {
  return (
    <div className="mb-10">
      <h1 className="text-primary font-bold text-xl mb-6">List Asset</h1>
      <AssetsContent />
    </div>
  );
}
