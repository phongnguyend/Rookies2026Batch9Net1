export default function LoadingSpinner() {
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-base-100/80 backdrop-blur-sm">
      <span className="loading loading-spinner loading-lg text-primary"></span>
    </div>
  );
}
