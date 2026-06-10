import { AssetState } from "@/features/Assets/assets.types";

export const displayAssetState = (assetState: AssetState) => {
  switch (assetState) {
    case AssetState.Available:
      return "Available";
    case AssetState.Assigned:
      return "Assigned";
    case AssetState.NotAvailable:
      return "Not available";
    case AssetState.WaitingForRecycling:
      return "Waiting for recycling";
    case AssetState.Recycled:
      return "Recycled";
    default:
      return "N/A";
  }
};
