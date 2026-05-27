import { NextRequest, NextResponse } from "next/server";
import { ENV_CONFIGS } from "./lib/config/env";

export function proxy(request: NextRequest) {
  const { pathname } = request.nextUrl;

  const accessTokenCookieName = ENV_CONFIGS.accessTokenCookieName;
  const refreshTokenCookieName = ENV_CONFIGS.refreshTokenCookieName;

  const hasAccessToken = request.cookies.has(accessTokenCookieName);
  const hasRefreshToken = request.cookies.has(refreshTokenCookieName);

  // route guard if don't have active session or refresh token
  if (!hasAccessToken && !hasRefreshToken && pathname !== "/") {
    return NextResponse.redirect(new URL("/", request.url));
  }

  // if authenticated or has refresh token to restore session, let request go through
  return NextResponse.next();
}

export const config = {
  matcher: [
    /*
     * match all request paths except for the ones starting with:
     * - _next/static (static files)
     * - _next/image (image optimization files)
     * - favicon.ico (favicon file)
     */
    "/((?!_next/static|_next/image|favicon.ico|.*\\..*$).*)",
    "/admin/:path*",
    "/staff/:path*",
    "/"
  ],
};
