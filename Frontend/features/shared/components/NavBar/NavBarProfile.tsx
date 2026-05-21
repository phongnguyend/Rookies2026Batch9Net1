"use client";

export default function NavbarProfile() {
  const handleSignOut = () => {
    // will add sign out later
    console.log("Sign out clicked");
  };

  return (
    <div className="dropdown dropdown-end">
      <div
        tabIndex={0}
        role="button"
        className="btn btn-ghost text-white text-base font-semibold normal-case flex items-center gap-1 hover:bg-white hover:text-primary">
        <span>binhnv</span>
        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor" className="w-4 h-4">
          <path strokeLinecap="round" strokeLinejoin="round" d="m19.5 8.25-7.5 7.5-7.5-7.5" />
        </svg>
      </div>
      <ul tabIndex={0} className="dropdown-content menu bg-base-100 rounded-box z-100 w-40 p-2 shadow text-base-content mt-2 border border-base-200">
        <li>
          <button
            onClick={handleSignOut}
            className="text-error font-semibold hover:bg-error hover:text-white active:bg-error/80 active:text-white w-full text-left">
            Sign out
          </button>
        </li>
      </ul>
    </div>
  );
}
