"use client";
import {useState} from "react";

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

  const [openDotsIndex, setOpenDotsIndex] = useState<number | null>(null);
  const [goToPage, setGoToPage] = useState(String(pageNumber));

  const getPaginationItems = (
    currentPage: number,
    totalPages: number,
    siblingCount = 1
  ): (number | "...")[] => {
    const totalVisiblePages = siblingCount * 2 + 5;

    if (totalPages <= totalVisiblePages) {
      return Array.from({ length: totalPages }, (_, i) => i + 1);
    }

    const leftSibling = Math.max(currentPage - siblingCount, 1);
    const rightSibling = Math.min(currentPage + siblingCount, totalPages);

    const showLeftDots = leftSibling > 2;
    const showRightDots = rightSibling < totalPages - 1;

    if (!showLeftDots && showRightDots) {
      const leftRange = Array.from(
        { length: 3 + siblingCount * 2 },
        (_, i) => i + 1
      );

      return [...leftRange, "...", totalPages];
    }

    if (showLeftDots && !showRightDots) {
      const rightRange = Array.from(
        { length: 3 + siblingCount * 2 },
        (_, i) => totalPages - (3 + siblingCount * 2) + 1 + i
      );

      return [1, "...", ...rightRange];
    }

    const middleRange = Array.from(
      { length: rightSibling - leftSibling + 1 },
      (_, i) => leftSibling + i
    );

    return [1, "...", ...middleRange, "...", totalPages];
  };

  const handleGoToPage = () => {
    const page = Number(goToPage);

    // invalid input
    if (
      Number.isNaN(page) ||
      page < 1 ||
      page > totalPages
    ) {
      setGoToPage(String(pageNumber));
      setOpenDotsIndex(null);
      return;
    }

    onPageChange(page);
    setOpenDotsIndex(null);
  };

  return (
    <div className="mt-6 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      {typeof totalCount === "number" && typeof pageSize === "number" && (
        <p className="text-sm text-gray-500">
          Page {pageNumber} of {totalPages} — Total {totalCount} items
        </p>
      )}

      <div className="flex w-full justify-center sm:w-auto sm:justify-end">
        <div className="join max-w-full overflow-x-auto">
          <button
            type="button"
            className="join-item btn btn-sm shrink-0"
            disabled={!hasPreviousPage}
            onClick={() => onPageChange(pageNumber - 1)}
            data-testid={btnPreviousPageTestId}
          >
            Previous
          </button>

          {/* Page list */}
          {getPaginationItems(pageNumber, totalPages).map((item, index) => {
            if (item === "...") {
              return (
                <div key={`dots-${index}`} className="join-item relative shrink-0">
                  {/* ... button */}
                  <button
                    type="button"
                    className="btn btn-sm rounded-none"
                    onClick={() => {
                      setOpenDotsIndex(
                        openDotsIndex === index ? null : index
                      );

                      setGoToPage(String(pageNumber));
                    }}
                  >
                    ...
                  </button>

                  {/* GoToPage Modal */}
                  {openDotsIndex === index && (
                    <div className="fixed left-1/2 top-1/2 z-50 w-[90vw] max-w-xs -translate-x-1/2 -translate-y-1/2 rounded-lg border bg-white p-4 shadow-lg sm:absolute sm:top-14 sm:w-64 sm:translate-y-0">
                      <p className="mb-3 text-lg font-medium">
                        Go to page
                      </p>

                      <div className="flex flex-col gap-2 sm:flex-row">
                        <input
                          type="number"
                          min={1}
                          max={totalPages}
                          value={goToPage}
                          onChange={(e) =>
                            setGoToPage(e.target.value)
                          }
                          onKeyDown={(e) => {
                            if (e.key === "Enter") {
                              handleGoToPage();
                            }
                          }}
                          className="input input-bordered input-sm w-full"
                          data-testid="txtGoToPage"
                        />

                        <button
                          type="button"
                          className="btn btn-sm btn-primary w-full sm:w-auto"
                          onClick={handleGoToPage}
                          data-testid="btnGoToPage"
                        >
                          Go
                        </button>
                      </div>

                      <p className="text-center text-sm text-gray-500 sm:text-left">
                        Page 1 - {totalPages}
                      </p>
                    </div>
                  )}
                </div>
              );
            }

            const current = item;

            return (
              <button
                key={current}
                type="button"
                onClick={() => onPageChange(current)}
                className={`join-item btn btn-sm shrink-0 ${
                  pageNumber === current ? "btn-primary" : ""
                }`}
                data-testid={
                  pageNumber === current
                    ? btnCurrentPageTestId
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
  </div>
  );
}
