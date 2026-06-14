# =================
# Build Stage
# =================

FROM node:26.0-alpine3.23 AS build
WORKDIR /build

COPY package.json package-lock.json ./
RUN npm ci

COPY . .

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