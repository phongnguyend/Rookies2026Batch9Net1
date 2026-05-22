import { NextRequest, NextResponse } from "next/server";
import { ENV_CONFIGS } from "./lib/config/env";

export function proxy(request: NextRequest) {
  const { pathname } = request.nextUrl;

  const sessionCookieName = ENV_CONFIGS.sessionCookieName || "AspNet.Cookie.Api";
  const hasSession = request.cookies.has(sessionCookieName);

  // route guard
  if (!hasSession) {
    if (pathname !== "/") {
      return NextResponse.redirect(new URL("/", request.url));
    }
  }

  // if authenticated, let the request go through
  return NextResponse.next();
}

export const config = {
  matcher: [
    /*
     * match all request paths except for the ones starting with:
     * - _next/static (static files)
     * - _next/image (image optimization files)
     * - favicon.ico (favicon file)
     * - test (test route)
     */
    "/((?!_next/static|_next/image|favicon.ico|.*\\..*$).*)",
    "/admin/:path*",
    "/staff/:path*",
    "/"
  ],
};
