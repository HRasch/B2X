# B2X Dev Node

GPU-accelerated AI development environment for local network access.

## Overview

This setup provides a self-contained Docker environment for running local LLM inference with NVIDIA GPU acceleration. Deploy to a high-performance development PC (e.g., RTX 5090, 64GB DDR5) for fast AI-assisted development.

## Requirements

### On the Dev Node (Windows/WSL2)

1. **NVIDIA GPU** with CUDA support (RTX 30xx, 40xx, 50xx series)
2. **Docker Desktop** with WSL2 backend
3. **NVIDIA Container Toolkit** for GPU passthrough

### Setup NVIDIA Container Toolkit (WSL2)

```bash
# In WSL2 terminal
distribution=$(. /etc/os-release;echo $ID$VERSION_ID)
curl -s -L https://nvidia.github.io/libnvidia-container/gpgkey | sudo apt-key add -
curl -s -L https://nvidia.github.io/libnvidia-container/$distribution/libnvidia-container.list | \
    sudo tee /etc/apt/sources.list.d/nvidia-container-toolkit.list

sudo apt-get update
sudo apt-get install -y nvidia-container-toolkit
sudo systemctl restart docker
```

### Verify GPU Access

```bash
docker run --rm --gpus all nvidia/cuda:12.4.0-base-ubuntu22.04 nvidia-smi
```

## Quick Start

### On the Dev Node

1. **Copy files** to the dev node:
   ```bash
   # From your macOS machine
   scp -r tools/dev-node user@192.168.1.117:~/B2X-dev-node/
   ```

2. **Start the stack**:
   ```bash
   cd ~/B2X-dev-node
   docker compose up -d
   ```

3. **Pull a model**:
   ```bash
   docker compose exec ollama ollama pull llama3.2
   docker compose exec ollama ollama pull codellama
   ```

### From Development Machine (macOS)

Use the `dev-node.py` script:

```bash
# Configure connection
python3 scripts/dev-node.py configure

# Check status
python3 scripts/dev-node.py status

# List models
python3 scripts/dev-node.py ollama list

# Run a prompt
python3 scripts/dev-node.py ollama run -m llama3.2 -p "Explain CQRS pattern"

# Benchmark
python3 scripts/dev-node.py benchmark
```

## Services

| Service | Port | Description |
|---------|------|-------------|
| Ollama API | 11434 | LLM inference endpoint |
| Web UI | 3000 | Optional browser interface (disabled by default) |

## API Usage

### Direct API Access

```bash
# List models
curl http://192.168.1.117:11434/api/tags

# Generate response
curl http://192.168.1.117:11434/api/generate -d '{
  "model": "llama3.2",
  "prompt": "Write a Python function to add two numbers",
  "stream": false
}'

# Chat completion
curl http://192.168.1.117:11434/api/chat -d '{
  "model": "llama3.2",
  "messages": [{"role": "user", "content": "Hello!"}],
  "stream": false
}'
```

### From Python

```python
import requests

OLLAMA_URL = "http://192.168.1.117:11434"

def generate(prompt: str, model: str = "llama3.2") -> str:
    response = requests.post(
        f"{OLLAMA_URL}/api/generate",
        json={"model": model, "prompt": prompt, "stream": False}
    )
    return response.json()["response"]

print(generate("Explain async/await in C#"))
```

## Recommended Models

| Model | Size | Use Case |
|-------|------|----------|
| `llama3.2` | ~4GB | General purpose, fast |
| `llama3.2:70b` | ~40GB | High quality, slower |
| `codellama` | ~4GB | Code generation |
| `codellama:34b` | ~20GB | Advanced code tasks |
| `deepseek-coder` | ~7GB | Code completion |
| `mistral` | ~4GB | Fast, good quality |

Pull models:
```bash
docker compose exec ollama ollama pull llama3.2
docker compose exec ollama ollama pull codellama
```

## Troubleshooting

### GPU Not Detected

```bash
# Check NVIDIA driver
nvidia-smi

# Check Docker GPU access
docker run --rm --gpus all nvidia/cuda:12.4.0-base-ubuntu22.04 nvidia-smi

# Restart Docker
sudo systemctl restart docker
```

### Port Not Accessible

```bash
# Check if service is running
docker compose ps

# Check firewall (Windows)
# Allow port 11434 in Windows Defender Firewall

# Check WSL2 port forwarding
netsh interface portproxy show all
```

### High Memory Usage

```bash
# Check container resources
docker stats B2X-ollama

# Unload models from memory
curl http://localhost:11434/api/generate -d '{"model": "llama3.2", "keep_alive": 0}'
```

## Security Notes

⚠️ **Development Only** - This setup is for local network development use only.

- Ollama API has no authentication by default
- Only expose on trusted local networks
- Do not expose to the internet
- Configuration files with IPs are excluded from git (`.gitignore`)

## Maintenance

```bash
# Update containers
docker compose pull
docker compose up -d

# Clean up unused images
docker system prune -a

# Backup models
docker run --rm -v B2X-ollama-data:/data -v $(pwd):/backup \
    alpine tar czf /backup/ollama-models-backup.tar.gz /data

# View logs
docker compose logs -f ollama
```
