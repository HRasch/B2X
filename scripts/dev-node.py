#!/usr/bin/env python3
"""
Dev Node Access Script - Connect to optional high-performance development PC.

This script provides utilities to interact with a local network development node
running Ollama, Docker, and WSL for AI/ML acceleration and containerized workflows.

Configuration is stored in ~/.b2connect-dev-node.json (NOT in repository).
Use 'dev-node.py configure' to set up your local config.

Usage:
    python3 scripts/dev-node.py configure       # Set up dev node config
    python3 scripts/dev-node.py status          # Check dev node availability
    python3 scripts/dev-node.py ollama list     # List available Ollama models
    python3 scripts/dev-node.py ollama run      # Run Ollama prompt
    python3 scripts/dev-node.py ollama pull     # Pull a model
    python3 scripts/dev-node.py docker ps       # List Docker containers
    python3 scripts/dev-node.py docker start    # Start dev node stack
    python3 scripts/dev-node.py docker stop     # Stop dev node stack
    python3 scripts/dev-node.py docker logs     # View container logs
    python3 scripts/dev-node.py deploy          # Deploy stack to dev node
    python3 scripts/dev-node.py benchmark       # Run quick performance test
"""

import argparse
import json
import os
import subprocess
import sys
import time
from pathlib import Path
from typing import Optional
import urllib.request
import urllib.error

# Config file location (user home, NOT in repository)
CONFIG_FILE = Path.home() / ".b2connect-dev-node.json"

# Default ports
DEFAULT_OLLAMA_PORT = 11434
DEFAULT_SSH_PORT = 22
DEFAULT_DOCKER_PORT = 2375


def load_config() -> Optional[dict]:
    """Load dev node config from user home directory."""
    if not CONFIG_FILE.exists():
        return None
    try:
        with open(CONFIG_FILE, "r") as f:
            return json.load(f)
    except (json.JSONDecodeError, IOError) as e:
        print(f"⚠️  Error reading config: {e}")
        return None


def save_config(config: dict) -> bool:
    """Save dev node config to user home directory."""
    try:
        with open(CONFIG_FILE, "w") as f:
            json.dump(config, f, indent=2)
        os.chmod(CONFIG_FILE, 0o600)  # Restrict permissions
        return True
    except IOError as e:
        print(f"❌ Error saving config: {e}")
        return False


def configure_node(args):
    """Interactive configuration of dev node."""
    print("🔧 Dev Node Configuration")
    print("=" * 40)
    print(f"Config will be stored at: {CONFIG_FILE}")
    print("(This file is NOT tracked in git)\n")

    existing = load_config() or {}

    ip = input(f"Dev node IP [{existing.get('ip', '192.168.1.x')}]: ").strip()
    if not ip:
        ip = existing.get("ip", "")

    if not ip:
        print("❌ IP address is required.")
        return 1

    ollama_port = input(f"Ollama port [{existing.get('ollama_port', DEFAULT_OLLAMA_PORT)}]: ").strip()
    ollama_port = int(ollama_port) if ollama_port else existing.get("ollama_port", DEFAULT_OLLAMA_PORT)

    ssh_port = input(f"SSH port [{existing.get('ssh_port', DEFAULT_SSH_PORT)}]: ").strip()
    ssh_port = int(ssh_port) if ssh_port else existing.get("ssh_port", DEFAULT_SSH_PORT)

    ssh_user = input(f"SSH user [{existing.get('ssh_user', os.getlogin())}]: ").strip()
    ssh_user = ssh_user if ssh_user else existing.get("ssh_user", os.getlogin())

    config = {
        "ip": ip,
        "ollama_port": ollama_port,
        "ssh_port": ssh_port,
        "ssh_user": ssh_user,
        "docker_port": existing.get("docker_port", DEFAULT_DOCKER_PORT),
        "description": "B2Connect Dev Node (RTX 5090, 64GB DDR5, Ollama/Docker/WSL)",
        "configured_at": time.strftime("%Y-%m-%d %H:%M:%S"),
    }

    if save_config(config):
        print(f"\n✅ Config saved to {CONFIG_FILE}")
        print("\nTest with: python3 scripts/dev-node.py status")
        return 0
    return 1


def check_status(args):
    """Check dev node availability and services."""
    config = load_config()
    if not config:
        print("❌ Dev node not configured. Run: python3 scripts/dev-node.py configure")
        return 1

    ip = config["ip"]
    print(f"🔍 Checking dev node at {ip}...")
    print("=" * 40)

    # Ping test
    print("\n📡 Network connectivity...", end=" ")
    try:
        result = subprocess.run(
            ["ping", "-c", "1", "-W", "2", ip],
            capture_output=True,
            timeout=5,
        )
        if result.returncode == 0:
            print("✅ Reachable")
        else:
            print("❌ Unreachable")
            return 1
    except subprocess.TimeoutExpired:
        print("❌ Timeout")
        return 1

    # Ollama test
    print(f"🤖 Ollama (port {config['ollama_port']})...", end=" ")
    try:
        url = f"http://{ip}:{config['ollama_port']}/api/tags"
        req = urllib.request.Request(url, method="GET")
        with urllib.request.urlopen(req, timeout=5) as response:
            data = json.loads(response.read().decode())
            models = data.get("models", [])
            print(f"✅ Running ({len(models)} models)")
    except urllib.error.URLError as e:
        print(f"❌ Not accessible ({e.reason})")
    except Exception as e:
        print(f"⚠️  Error: {e}")

    # SSH test (quick port check)
    print(f"🔐 SSH (port {config['ssh_port']})...", end=" ")
    try:
        result = subprocess.run(
            ["nc", "-zv", "-w", "2", ip, str(config["ssh_port"])],
            capture_output=True,
            timeout=5,
        )
        if result.returncode == 0:
            print("✅ Open")
        else:
            print("❌ Closed/Filtered")
    except subprocess.TimeoutExpired:
        print("⚠️  Timeout")
    except FileNotFoundError:
        print("⚠️  nc not available")

    print("\n" + "=" * 40)
    print(f"📝 Config: {CONFIG_FILE}")
    return 0


def ollama_command(args):
    """Interact with Ollama on the dev node."""
    config = load_config()
    if not config:
        print("❌ Dev node not configured. Run: python3 scripts/dev-node.py configure")
        return 1

    base_url = f"http://{config['ip']}:{config['ollama_port']}"

    if args.ollama_cmd == "list":
        # List available models
        try:
            url = f"{base_url}/api/tags"
            with urllib.request.urlopen(url, timeout=10) as response:
                data = json.loads(response.read().decode())
                models = data.get("models", [])
                if models:
                    print("📦 Available Ollama Models:")
                    print("-" * 50)
                    for model in models:
                        name = model.get("name", "unknown")
                        size = model.get("size", 0) / (1024 ** 3)  # Convert to GB
                        print(f"  • {name} ({size:.1f} GB)")
                else:
                    print("⚠️  No models installed. Pull a model first.")
                return 0
        except Exception as e:
            print(f"❌ Error: {e}")
            return 1

    elif args.ollama_cmd == "run":
        # Run a prompt
        model = args.model or "llama3.2"
        prompt = args.prompt or "Hello, how are you?"

        print(f"🤖 Running prompt on {model}...")
        print("-" * 50)

        try:
            url = f"{base_url}/api/generate"
            payload = json.dumps({
                "model": model,
                "prompt": prompt,
                "stream": False,
            }).encode()

            req = urllib.request.Request(url, data=payload, method="POST")
            req.add_header("Content-Type", "application/json")

            with urllib.request.urlopen(req, timeout=120) as response:
                data = json.loads(response.read().decode())
                print(data.get("response", "No response"))
                print("-" * 50)
                print(f"⏱️  {data.get('total_duration', 0) / 1e9:.2f}s")
                return 0
        except Exception as e:
            print(f"❌ Error: {e}")
            return 1

    elif args.ollama_cmd == "pull":
        # Pull a model
        model = args.model or "llama3.2"
        print(f"📥 Pulling model {model}...")
        print("(This may take a while for large models)")
        print("-" * 50)

        try:
            url = f"{base_url}/api/pull"
            payload = json.dumps({
                "name": model,
                "stream": True,
            }).encode()

            req = urllib.request.Request(url, data=payload, method="POST")
            req.add_header("Content-Type", "application/json")

            with urllib.request.urlopen(req, timeout=600) as response:
                for line in response:
                    try:
                        data = json.loads(line.decode())
                        status = data.get("status", "")
                        if "pulling" in status:
                            completed = data.get("completed", 0)
                            total = data.get("total", 1)
                            pct = (completed / total * 100) if total > 0 else 0
                            print(f"\r  {status}: {pct:.1f}%", end="", flush=True)
                        elif status:
                            print(f"\n  {status}")
                    except json.JSONDecodeError:
                        pass
            print(f"\n✅ Model {model} pulled successfully")
            return 0
        except Exception as e:
            print(f"❌ Error: {e}")
            return 1

    else:
        print(f"Unknown ollama command: {args.ollama_cmd}")
        return 1


def docker_command(args):
    """Interact with Docker on the dev node (via SSH)."""
    config = load_config()
    if not config:
        print("❌ Dev node not configured. Run: python3 scripts/dev-node.py configure")
        return 1

    ssh_cmd = f"ssh -o ConnectTimeout=5 -p {config['ssh_port']} {config['ssh_user']}@{config['ip']}"
    compose_dir = config.get("compose_dir", "~/b2connect-dev-node")

    if args.docker_cmd == "ps":
        cmd = f"{ssh_cmd} docker ps --format 'table {{{{.Names}}}}\\t{{{{.Status}}}}\\t{{{{.Ports}}}}'"
        print(f"🐳 Docker containers on {config['ip']}:")
        os.system(cmd)

    elif args.docker_cmd == "images":
        cmd = f"{ssh_cmd} docker images --format 'table {{{{.Repository}}}}\\t{{{{.Tag}}}}\\t{{{{.Size}}}}'"
        print(f"🐳 Docker images on {config['ip']}:")
        os.system(cmd)

    elif args.docker_cmd == "start":
        print(f"🚀 Starting dev node stack on {config['ip']}...")
        cmd = f"{ssh_cmd} 'cd {compose_dir} && docker compose up -d'"
        result = os.system(cmd)
        if result == 0:
            print("✅ Stack started")
        return result

    elif args.docker_cmd == "stop":
        print(f"🛑 Stopping dev node stack on {config['ip']}...")
        cmd = f"{ssh_cmd} 'cd {compose_dir} && docker compose down'"
        result = os.system(cmd)
        if result == 0:
            print("✅ Stack stopped")
        return result

    elif args.docker_cmd == "logs":
        print(f"📋 Logs from dev node on {config['ip']}:")
        cmd = f"{ssh_cmd} 'cd {compose_dir} && docker compose logs --tail=50'"
        os.system(cmd)

    elif args.docker_cmd == "build":
        print(f"🔨 Building dev node image on {config['ip']}...")
        cmd = f"{ssh_cmd} 'cd {compose_dir} && docker compose build'"
        result = os.system(cmd)
        if result == 0:
            print("✅ Build complete")
        return result

    else:
        print(f"Unknown docker command: {args.docker_cmd}")
        return 1

    return 0


def deploy_command(args):
    """Deploy the dev node stack to the remote machine."""
    config = load_config()
    if not config:
        print("❌ Dev node not configured. Run: python3 scripts/dev-node.py configure")
        return 1

    # Find the dev-node directory in the repo
    script_dir = Path(__file__).parent.parent
    dev_node_dir = script_dir / "tools" / "dev-node"

    if not dev_node_dir.exists():
        print(f"❌ Dev node files not found at {dev_node_dir}")
        return 1

    ip = config["ip"]
    ssh_port = config["ssh_port"]
    ssh_user = config["ssh_user"]
    compose_dir = config.get("compose_dir", "~/b2connect-dev-node")

    print(f"🚀 Deploying dev node stack to {ip}...")
    print("=" * 40)

    # Create remote directory
    print("📁 Creating remote directory...")
    ssh_cmd = f"ssh -o ConnectTimeout=5 -p {ssh_port} {ssh_user}@{ip}"
    result = os.system(f"{ssh_cmd} 'mkdir -p {compose_dir}'")
    if result != 0:
        print("❌ Failed to create remote directory. Is SSH enabled?")
        return 1

    # Copy files
    print("📤 Copying files...")
    scp_cmd = f"scp -P {ssh_port}"
    files = ["Dockerfile", "docker-compose.yml", "README.md"]

    for f in files:
        src = dev_node_dir / f
        if src.exists():
            result = os.system(f"{scp_cmd} {src} {ssh_user}@{ip}:{compose_dir}/")
            if result != 0:
                print(f"❌ Failed to copy {f}")
                return 1
            print(f"  ✅ {f}")

    print("\n" + "=" * 40)
    print("✅ Deployment complete!")
    print(f"\n📍 Files deployed to: {ssh_user}@{ip}:{compose_dir}")
    print("\nNext steps on the dev node:")
    print(f"  1. cd {compose_dir}")
    print("  2. docker compose up -d")
    print("  3. docker compose exec ollama ollama pull llama3.2")
    print("\nOr from this machine:")
    print("  python3 scripts/dev-node.py docker start")
    print("  python3 scripts/dev-node.py ollama pull -m llama3.2")

    return 0


def benchmark(args):
    """Run a quick benchmark on the dev node."""
    config = load_config()
    if not config:
        print("❌ Dev node not configured. Run: python3 scripts/dev-node.py configure")
        return 1

    print("⚡ Dev Node Benchmark")
    print("=" * 40)

    # Ollama inference benchmark
    base_url = f"http://{config['ip']}:{config['ollama_port']}"

    try:
        url = f"{base_url}/api/tags"
        with urllib.request.urlopen(url, timeout=10) as response:
            data = json.loads(response.read().decode())
            models = data.get("models", [])
            if not models:
                print("⚠️  No Ollama models available for benchmark")
                return 1

            model = models[0]["name"]
            print(f"🤖 Testing with model: {model}")

            # Simple inference test
            prompt = "Write a one-line Python function that adds two numbers."
            print(f"📝 Prompt: {prompt}")
            print("-" * 40)

            url = f"{base_url}/api/generate"
            payload = json.dumps({
                "model": model,
                "prompt": prompt,
                "stream": False,
            }).encode()

            req = urllib.request.Request(url, data=payload, method="POST")
            req.add_header("Content-Type", "application/json")

            start = time.time()
            with urllib.request.urlopen(req, timeout=120) as response:
                data = json.loads(response.read().decode())
                elapsed = time.time() - start

            print(f"📤 Response: {data.get('response', 'N/A')[:200]}")
            print("-" * 40)
            print(f"⏱️  Total time: {elapsed:.2f}s")
            print(f"📊 Tokens/sec: {data.get('eval_count', 0) / (data.get('eval_duration', 1) / 1e9):.1f}")

            return 0

    except Exception as e:
        print(f"❌ Benchmark failed: {e}")
        return 1


def main():
    parser = argparse.ArgumentParser(
        description="B2Connect Dev Node Access Script",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog=__doc__,
    )

    subparsers = parser.add_subparsers(dest="command", help="Available commands")

    # Configure command
    subparsers.add_parser("configure", help="Configure dev node connection")

    # Status command
    subparsers.add_parser("status", help="Check dev node availability")

    # Ollama command
    ollama_parser = subparsers.add_parser("ollama", help="Ollama commands")
    ollama_parser.add_argument("ollama_cmd", choices=["list", "run", "pull"], help="Ollama subcommand")
    ollama_parser.add_argument("--model", "-m", help="Model to use (default: llama3.2)")
    ollama_parser.add_argument("--prompt", "-p", help="Prompt to run")

    # Docker command
    docker_parser = subparsers.add_parser("docker", help="Docker commands (via SSH)")
    docker_parser.add_argument("docker_cmd", choices=["ps", "images", "start", "stop", "logs", "build"], help="Docker subcommand")

    # Deploy command
    subparsers.add_parser("deploy", help="Deploy dev node stack to remote machine")

    # Benchmark command
    subparsers.add_parser("benchmark", help="Run quick performance benchmark")

    args = parser.parse_args()

    if args.command == "configure":
        return configure_node(args)
    elif args.command == "status":
        return check_status(args)
    elif args.command == "ollama":
        return ollama_command(args)
    elif args.command == "docker":
        return docker_command(args)
    elif args.command == "deploy":
        return deploy_command(args)
    elif args.command == "benchmark":
        return benchmark(args)
    else:
        parser.print_help()
        return 0


if __name__ == "__main__":
    sys.exit(main())
