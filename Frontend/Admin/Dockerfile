# Multi-stage Dockerfile for Vue 3 + Vite Frontend (Development & Production)

# ===== DEVELOPMENT STAGE =====
# Note: For Aspire native integration (AddViteApp), this stage is NOT used.
# Aspire runs npm directly on the host machine for hot-reload support.
# This stage is kept for docker-compose or standalone Docker usage.
FROM node:20-alpine AS development

WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy source code
COPY . .

# Expose Vite dev server port
EXPOSE 5174

# Run Vite dev server (listens on all interfaces)
# The --host flag is required for Docker networking
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0", "--port", "5174"]

# ===== BUILDER STAGE =====
FROM node:20-alpine AS builder

WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies (use ci for deterministic builds if lock file exists)
RUN npm ci --ignore-scripts || npm install

# Copy source code
COPY . .

# Build the project
RUN npm run build

# ===== PRODUCTION STAGE =====
FROM nginx:alpine AS production

# Copy custom nginx config for SPA routing
COPY nginx.conf /etc/nginx/conf.d/default.conf 2>/dev/null || true

# Copy built app from builder
COPY --from=builder /app/dist /usr/share/nginx/html

# Expose port
EXPOSE 5174

# Use custom entrypoint to support runtime environment variables
CMD ["nginx", "-g", "daemon off;"]
