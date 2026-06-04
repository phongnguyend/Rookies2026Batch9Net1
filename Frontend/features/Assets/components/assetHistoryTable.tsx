interface HistoryItem {
  date: string;
  assignedTo: string;
  assignedBy: string;
  returnedDate: string;
}

interface AssetHistoryTableProps {
  items: HistoryItem[];
}

export default function AssetHistoryTable({ items }: AssetHistoryTableProps) {
  return (
    <div className="mt-4 flex flex-col md:flex-row gap-4 items-start">
      {/* Left side */}
      <div className="min-w-[100px]">
        <p className="font-semibold text-gray-700">History</p>
      </div>

      {/* Right side */}
      <div className="flex-1 w-full overflow-x-auto" data-testid="tblAssetHistory">
        <table className="min-w-[600px] w-full text-xs">
          <thead>
            <tr className="border-b border-gray-300">
              <th className="text-left py-2 font-semibold text-gray-600">
                Date
              </th>
              <th className="text-left py-2 font-semibold text-gray-600">
                Assigned to
              </th>
              <th className="text-left py-2 font-semibold text-gray-600">
                Assigned by
              </th>
              <th className="text-left py-2 font-semibold text-gray-600">
                Returned Date
              </th>
            </tr>
          </thead>

          <tbody>
            {items.length === 0 ? (
              <tr>
                <td colSpan={4} className="py-4 text-center text-gray-400">
                  No history found.
                </td>
              </tr>
            ) : (
              items.map((item, index) => (
                <tr key={index} className="border-b border-gray-100">
                  <td className="py-2">{item.date}</td>
                  <td className="py-2">{item.assignedTo}</td>
                  <td className="py-2">{item.assignedBy}</td>
                  <td className="py-2">{item.returnedDate}</td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
