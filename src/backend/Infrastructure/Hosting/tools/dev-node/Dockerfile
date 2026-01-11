# B2X Dev Node - GPU-Accelerated AI Development Container
# 
# This container runs Ollama with NVIDIA GPU support for local LLM inference.
# Deploy to a high-performance dev node (RTX 5090, etc.) for AI acceleration.
#
# Build:   docker build -t b2x-dev-node .
# Run:     docker run -d --gpus all -p 11434:11434 -v ollama-data:/root/.ollama b2x-dev-node
#
# Requirements:
# - NVIDIA GPU with CUDA support
# - NVIDIA Container Toolkit (nvidia-docker)
# - Docker with GPU passthrough enabled

FROM nvidia/cuda:12.4.0-runtime-ubuntu22.04

LABEL maintainer="B2X Team"
LABEL description="GPU-accelerated AI development node with Ollama"
LABEL version="1.0.0"

# Prevent interactive prompts during build
ENV DEBIAN_FRONTEND=noninteractive
ENV OLLAMA_HOST=0.0.0.0
ENV OLLAMA_ORIGINS=*

# Install dependencies
RUN apt-get update && apt-get install -y --no-install-recommends \
    curl \
    ca-certificates \
    git \
    python3 \
    python3-pip \
    jq \
    && rm -rf /var/lib/apt/lists/*

# Install Ollama
RUN curl -fsSL https://ollama.com/install.sh | sh

# Create data directory
RUN mkdir -p /root/.ollama

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:11434/api/tags || exit 1

# Expose Ollama API port
EXPOSE 11434

# Volume for model storage (persist models across restarts)
VOLUME ["/root/.ollama"]

# Start Ollama server
CMD ["ollama", "serve"]
