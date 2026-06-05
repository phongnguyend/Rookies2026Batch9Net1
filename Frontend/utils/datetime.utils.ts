export function formatDate(utc: string) {
  return new Date(utc).toLocaleDateString("en-GB").replace(/\//g, "/");
}

export function formatDateTime(utc: string) {
  const d = new Date(utc);
  return (
    d.toLocaleDateString("en-GB").replace(/\//g, "/") +
    " " +
    d.toLocaleTimeString([], {
      hour: "2-digit",
      minute: "2-digit",
      hour12: false,
    })
  );
}

export function utcDateToLocalDate(utc: string | null) {
  if (!utc) return null;

  const date = new Date(utc);
  return new Date(
    date.getUTCFullYear(),
    date.getUTCMonth(),
    date.getUTCDate(),
  );
}

export function localDateToUtcIso(date: Date | null) {
  if (!date) return "";

  return new Date(
    Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()),
  ).toISOString();
}
