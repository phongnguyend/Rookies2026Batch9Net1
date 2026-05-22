export default function FloatingDrawerButton() {
  return (
    <label
      htmlFor="admin-drawer"
      className="btn btn-circle btn-primary fixed top-3/6 right-4 md:hidden z-50 shadow-lg"
      aria-label="open sidebar"
    >
      <svg
        xmlns="http://www.w3.org/2000/svg"
        fill="none"
        viewBox="0 0 24 24"
        className="inline-block w-6 h-6 stroke-current"
      >
        <path
          strokeLinecap="round"
          strokeLinejoin="round"
          strokeWidth="2"
          d="M5 12h.01M12 12h.01M19 12h.01M6 12a1 1 0 11-2 0 1 1 0 012 0zm7 0a1 1 0 11-2 0 1 1 0 012 0zm7 0a1 1 0 11-2 0 1 1 0 012 0z"
        ></path>
      </svg>
    </label>
  );
}
