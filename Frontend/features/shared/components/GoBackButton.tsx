import Link from "next/link";

type GoBackButtonProps = {
  href: string;
  title: string;
};

export default function GoBackButton({ href, title }: GoBackButtonProps) {
  return (
    <div className="flex items-center gap-2">
      <Link href={href} className="btn btn-primary btn-circle btn-ghost">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          fill="none"
          viewBox="0 0 24 24"
          strokeWidth={1.5}
          stroke="currentColor"
          className="w-6 h-6"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            d="M10.5 19.5L3 12m0 0l7.5-7.5M3 12h18"
          />
        </svg>
      </Link>
      <h1 className="text-2xl font-bold text-primary">{title}</h1>
    </div>
  );
}
