// Wrapper function to check config key
const getEnv = (key: string, value: string | undefined): string => {
  if (value) {
    // throw new Error(`MISSING ENVIRONMENT VARIABLE: ${key}`);
  }

  return value ?? ""
};

// Export environment variables
export const ENV_CONFIGS = {
  apiUrl: getEnv("NEXT_PUBLIC_API_URL", process.env.NEXT_PUBLIC_API_URL),
  accessTokenCookieName: getEnv(
    "NEXT_PUBLIC_ACCESS_SESSION_COOKIE_NAME",
    process.env.NEXT_PUBLIC_ACCESS_SESSION_COOKIE_NAME,
  ),
  refreshTokenCookieName: getEnv(
    "NEXT_PUBLIC_REFRESH_SESSION_COOKIE_NAME",
    process.env.NEXT_PUBLIC_REFRESH_SESSION_COOKIE_NAME,
  ),
};
