# Docker/Kubernetes MCP Server

A Model Context Protocol (MCP) server that provides AI-assisted analysis for Docker containers and Kubernetes infrastructure.

## Features

### 🐳 Docker Analysis Tools

- **analyze_dockerfile**: Analyze Dockerfile for best practices, security, and optimization
- **check_container_security**: Scan container images for security vulnerabilities and misconfigurations
- **analyze_docker_compose**: Analyze Docker Compose configuration for best practices and optimization
- **monitor_container_health**: Monitor running containers for health, resources, and performance

### ☸️ Kubernetes Analysis Tools

- **validate_kubernetes_manifests**: Validate Kubernetes YAML manifests for best practices and security

## Installation

```bash
npm install
npm run build
```

## Usage

### As MCP Server

The server communicates via stdio and can be integrated with MCP-compatible clients.

### Available Tools

#### analyze_dockerfile

Analyzes a Dockerfile for best practices and security issues.

**Parameters:**
- `dockerfilePath` (required): Path to the Dockerfile to analyze
- `workspacePath` (optional): Path to the workspace containing the Dockerfile

**Example:**
```json
{
  "dockerfilePath": "Dockerfile",
  "workspacePath": "/path/to/project"
}
```

**Output:** Comprehensive analysis including score, issues found, and recommendations.

#### validate_kubernetes_manifests

Validates Kubernetes YAML manifests for best practices and security.

**Parameters:**
- `manifestPath` (required): Path to Kubernetes manifest file or directory
- `workspacePath` (optional): Path to the workspace containing the manifests

**Example:**
```json
{
  "manifestPath": "k8s/",
  "workspacePath": "/path/to/project"
}
```

**Output:** Analysis of deployments, services, security contexts, and resource limits.

#### check_container_security

Scans container images for security vulnerabilities.

**Parameters:**
- `imageName` (required): Name of the container image to scan
- `workspacePath` (optional): Path to the workspace

**Example:**
```json
{
  "imageName": "nginx:latest",
  "workspacePath": "/path/to/project"
}
```

**Output:** Security analysis including vulnerability assessment and recommendations.

#### analyze_docker_compose

Analyzes Docker Compose configuration files.

**Parameters:**
- `composePath` (optional): Path to docker-compose.yml file (default: "docker-compose.yml")
- `workspacePath` (required): Path to the workspace containing the compose file

**Example:**
```json
{
  "composePath": "docker-compose.yml",
  "workspacePath": "/path/to/project"
}
```

**Output:** Analysis of services, networks, volumes, and configuration best practices.

#### monitor_container_health

Monitors running containers for health and performance.

**Parameters:**
- `containerName` (optional): Name of specific container to monitor
- `workspacePath` (required): Path to the workspace

**Example:**
```json
{
  "containerName": "web-app",
  "workspacePath": "/path/to/project"
}
```

**Output:** Health status, resource usage, and recommendations for all containers.

## Development

### Building

```bash
npm run build
```

### Testing

```bash
npm test
```

### Running

```bash
npm start
```

## Dependencies

- `@modelcontextprotocol/sdk`: MCP server framework
- `dockerode`: Docker API client
- `@kubernetes/client-node`: Kubernetes API client
- `js-yaml`: YAML parsing
- `fs-extra`: Enhanced file system operations
- `glob`: File pattern matching

## Configuration

The server automatically detects:
- Local Docker daemon
- Kubernetes configuration (if available)
- Container images and running containers

## Security Considerations

- Requires Docker daemon access for container analysis
- Needs Kubernetes cluster access for manifest validation
- Analyzes container images for security vulnerabilities
- Validates configurations against security best practices

## Error Handling

The server provides detailed error messages for:
- Missing files or invalid paths
- Docker daemon connection issues
- Kubernetes API access problems
- Invalid YAML/JSON configurations
- Permission and access issues

## Contributing

1. Follow the existing code patterns and TypeScript conventions
2. Add comprehensive tests for new features
3. Update documentation for new tools or parameters
4. Ensure all tests pass before submitting changes

## License

See project root for license information.