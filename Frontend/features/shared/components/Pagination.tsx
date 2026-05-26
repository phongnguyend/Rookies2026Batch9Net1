"use client";

interface PaginationProps {
  pageNumber: number;
  totalPages: number;
  pageSize?: number;
  totalCount?: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
  onPageChange: (page: number) => void;
  btnPreviousPageTestId?: string;
  btnNextPageTestId?: string;
  btnCurrentPageTestId?: string;
}

export default function Pagination({
  pageNumber,
  totalPages,
  pageSize,
  totalCount,
  hasPreviousPage,
  hasNextPage,
  onPageChange,
  btnPreviousPageTestId = "btnPreviousPage",
  btnNextPageTestId = "btnNextPage",
  btnCurrentPageTestId = "btnPage",
}: PaginationProps) {
  if (totalPages <= 1) return null;

  return (
    <div className="mt-6 flex items-center justify-between">
      {typeof totalCount === "number" && typeof pageSize === "number" && (
        <p className="text-sm text-gray-500">
          Page {pageNumber} of {totalPages} — Total {totalCount} items
        </p>
      )}

      <div className="join">
        <button
          type="button"
          className="join-item btn btn-sm"
          disabled={!hasPreviousPage}
          onClick={() => onPageChange(pageNumber - 1)}
          data-testid={btnPreviousPageTestId}
        >
          Previous
        </button>

        {Array.from({ length: totalPages }).map((_, index) => {
          const current = index + 1;

          return (
            <button
              key={current}
              type="button"
              onClick={() => onPageChange(current)}
              className={`join-item btn btn-sm ${pageNumber === current ? "btn-primary" : ""}`}
              data-testid={
                pageNumber === current
                  ? (btnCurrentPageTestId ?? `btnPage${current}`)
                  : `btnPage${current}`
              }
            >
              {current}
            </button>
          );
        })}

        <button
          type="button"
          className="join-item btn btn-sm"
          disabled={!hasNextPage}
          onClick={() => onPageChange(pageNumber + 1)}
          data-testid={btnNextPageTestId}
        >
          Next
        </button>
      </div>
    </div>
  );
}
