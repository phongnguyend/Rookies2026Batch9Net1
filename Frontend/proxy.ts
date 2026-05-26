import { NextRequest, NextResponse } from "next/server";
import { ENV_CONFIGS } from "./lib/config/env";

export function proxy(request: NextRequest) {
  const { pathname } = request.nextUrl;

  const accessTokenCookieName = ENV_CONFIGS.accessTokenCookieName || "NashAssetManagement.Cookie.AccessToken";
  const hasSession = request.cookies.has(accessTokenCookieName);

  // route guard if dont have cookie
  if (!hasSession && pathname !== "/") {
    return NextResponse.redirect(new URL("/", request.url));
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
     */
    "/((?!_next/static|_next/image|favicon.ico|.*\\..*$).*)",
    "/admin/:path*",
    "/staff/:path*",
    "/"
  ],
};
