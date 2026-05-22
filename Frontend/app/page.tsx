"use client";

import LoginForm from "@/features/auth/components/LoginForm";

export default function HomePage() {
  const handleLoginSubmit = (data: any) => {
    console.log("Login credentials submitted:", data);
  };

  return (
    <main className="relative min-h-screen flex items-center justify-center bg-base-200 px-4">
      <div className="absolute inset-0 overflow-hidden pointer-events-none z-0">
        <div className="absolute -top-[20%] -left-[10%] w-[500px] h-[500px] rounded-full bg-primary/5 blur-[120px]"></div>
        <div className="absolute -bottom-[20%] -right-[10%] w-[500px] h-[500px] rounded-full bg-primary/5 blur-[120px]"></div>
      </div>

      <div className="relative z-10 w-full flex justify-center">
        <LoginForm onLogin={handleLoginSubmit} />
      </div>
    </main>
  );
}
