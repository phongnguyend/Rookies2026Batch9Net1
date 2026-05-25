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
