import { ColumnDef } from "../../SingleSortDataTable";
import { LookupAssetsSummary } from "@/features/Assets/assets.types";

export function createColumns(
  pendingAsset: LookupAssetsSummary | null,
  onSelect: (asset: LookupAssetsSummary) => void,
): ColumnDef<LookupAssetsSummary>[] {
  return [
    {
      key: "selected",
      header: "",
      sortable: false,
      cellTestId: (_, index) => `rdoAsset-${index}`,
      className: "w-8 overflow-visible text-center",
      render: (asset) => {
        return (
          <input
            type="radio"
            name="asset-selection"
            checked={pendingAsset?.id === asset.id}
            onChange={() => onSelect(asset)}
            onClick={(e) => e.stopPropagation()}
            className="w-4 h-4 appearance-auto accent-primary cursor-pointer"
          />
        );
      },
    },
    {
      key: "assetCode",
      header: "Asset Code",
      sortable: true,
      headerTestId: "btnSortAssetCode",
      cellTestId: (_, index) => `colAssetCode-${index}`,
      className: "w-16",
    },
    {
      key: "assetName",
      header: "Asset Name",
      sortable: true,
      headerTestId: "btnSortAssetName",
      cellTestId: (_, index) => `colAssetName-${index}`,
      className: "w-40",
    },
    {
      key: "category",
      header: "Category",
      sortable: true,
      headerTestId: "btnSortCategory",
      cellTestId: (_, index) => `colCategory-${index}`,
      className: "w-20",
    },
  ];
}
