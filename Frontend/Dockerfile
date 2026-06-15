# =================
# Build Stage
# =================

FROM node:26.0-alpine3.23 AS build
WORKDIR /build

COPY package.json package-lock.json ./
RUN npm ci

COPY . .

# Since static files, envs wil be built at image building process
ARG NEXT_PUBLIC_API_URL
ARG NEXT_PUBLIC_ACCESS_SESSION_COOKIE_NAME
ARG NEXT_PUBLIC_REFRESH_SESSION_COOKIE_NAME

ENV NEXT_PUBLIC_API_URL=$NEXT_PUBLIC_API_URL
ENV NEXT_PUBLIC_ACCESS_SESSION_COOKIE_NAME=$NEXT_PUBLIC_ACCESS_SESSION_COOKIE_NAME
ENV NEXT_PUBLIC_REFRESH_SESSION_COOKIE_NAME=$NEXT_PUBLIC_REFRESH_SESSION_COOKIE_NAME

# /build/out folder for building as "export"
RUN npm run build

# =================
# Run Stage
# =================

FROM nginx:1.31.1-alpine AS runtime

# Rewrite rules from web.config
COPY nginx.conf /etc/nginx/conf.d/default.conf

COPY --from=build /build/out /usr/share/nginx/html

EXPOSE 80