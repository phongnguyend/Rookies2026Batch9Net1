"use client";

import { useState } from "react";
import { useGetAssetsQuery } from "@/features/Assets/assets.api";  // ← hook from api file ← type from types file
import { AssetState, type AssetListItem } from "@/features/Assets/assets.types";
import DataTable, { type ColumnDef } from "@/features/shared/components/DataTable";
import Pagination from "@/features/shared/components/Pagination";
import SearchInput from "@/features/shared/components/SearchInput";
import DropdownFilter from "@/features/shared/components/DropdownFilter";

const STATE_OPTIONS = Object.values(AssetState).map((s) => ({
  key: s,
  label: s,
}));

const CATEGORY_OPTIONS = [
  { key: "Laptop", label: "Laptop" },
  { key: "Monitor", label: "Monitor" },
  { key: "Keyboard", label: "Keyboard" },
];

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
];

export default function AssetsPage() {

  const [pageNumber, setPageNumber] = useState(1);
  const [searchInput, setSearchInput] = useState("");
  const [selectedCategories, setSelectedCategories] = useState<string[]>([]);
  const [selectedStates, setSelectedStates] = useState<string[]>([]);

  const { data, isLoading } = useGetAssetsQuery({  // ← correct hook name
    pageNumber,
    pageSize: 10,
    category: selectedCategories.length === 1 ? selectedCategories[0] : undefined,
    state: selectedStates.length === 1 ? selectedStates[0] as AssetState : undefined,
  });

  const handleCategoryChange = (values: string[]) => {
    setSelectedCategories(values);
    setPageNumber(1);
  };

  const handleStateChange = (values: string[]) => {
    setSelectedStates(values);
    setPageNumber(1);
  };

  return (
    <div className="p-6">
      <h1 className="mb-6 text-xl font-bold text-primary">Asset List</h1>

      <div className="mb-4 flex items-center gap-3">
        <DropdownFilter
          items={CATEGORY_OPTIONS}
          values={selectedCategories}
          placeholder="Category"
          getKey={(item) => item.key}
          getLabel={(item) => item.label}
          onChange={handleCategoryChange}
          allLabel="All Categories"
        />

        <DropdownFilter
          items={STATE_OPTIONS}
          values={selectedStates}
          placeholder="State"
          getKey={(item) => item.key}
          getLabel={(item) => item.label}
          onChange={handleStateChange}
          allLabel="All States"
        />

        <div className="ml-auto">
          <SearchInput
            value={searchInput}
            onChange={setSearchInput}
            onSearch={(value) => console.log("search:", value)} // TODO: wire to API
            placeholder="Search assets..."
          />
        </div>
      </div>

      <DataTable<AssetListItem>
        data={data?.items ?? []}
        columns={columns}
        isLoading={isLoading}
        emptyMessage="No assets found."
        onRowClick={(row) => console.log("clicked", row)} // TODO: open detail modal
      />

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
  );
}
