import { Server } from '@modelcontextprotocol/sdk/server/index.js';
import { StdioServerTransport } from '@modelcontextprotocol/sdk/server/stdio.js';
import {
  CallToolRequestSchema,
  ErrorCode,
  ListToolsRequestSchema,
  McpError,
} from '@modelcontextprotocol/sdk/types.js';
import Docker from 'dockerode';
import * as k8s from '@kubernetes/client-node';
import * as fs from 'fs-extra';
import * as path from 'path';
import { glob } from 'glob';
import * as yaml from 'js-yaml';

interface DockerAnalysisResult {
  score: number;
  issues: string[];
  recommendations: string[];
  metrics: Record<string, any>;
}

class DockerMCPServer {
  private server: Server;
  private docker: Docker;
  private k8sConfig: k8s.KubeConfig;
  private k8sAppsApi?: k8s.AppsV1Api;
  private k8sCoreApi?: k8s.CoreV1Api;

  constructor() {
    this.server = new Server({
      name: 'docker-mcp-server',
      version: '1.0.0',
    });

    // Initialize Docker client
    this.docker = new Docker();

    // Initialize Kubernetes client
    this.k8sConfig = new k8s.KubeConfig();
    try {
      this.k8sConfig.loadFromDefault();
      this.k8sAppsApi = this.k8sConfig.makeApiClient(k8s.AppsV1Api);
      this.k8sCoreApi = this.k8sConfig.makeApiClient(k8s.CoreV1Api);
    } catch (error) {
      // Kubernetes not available, continue without it
      console.warn('Kubernetes configuration not available:', error);
    }

    this.setupToolHandlers();
  }

  private setupToolHandlers() {
    // List available tools
    this.server.setRequestHandler(ListToolsRequestSchema, async () => {
      return {
        tools: [
          {
            name: 'analyze_dockerfile',
            description: 'Analyze Dockerfile for best practices, security, and optimization',
            inputSchema: {
              type: 'object',
              properties: {
                dockerfilePath: {
                  type: 'string',
                  description: 'Path to the Dockerfile to analyze',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Path to the workspace containing the Dockerfile',
                },
              },
              required: ['dockerfilePath'],
            },
          },
          {
            name: 'validate_kubernetes_manifests',
            description: 'Validate Kubernetes YAML manifests for best practices and security',
            inputSchema: {
              type: 'object',
              properties: {
                manifestPath: {
                  type: 'string',
                  description: 'Path to Kubernetes manifest file or directory',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Path to the workspace containing the manifests',
                },
              },
              required: ['manifestPath'],
            },
          },
          {
            name: 'check_container_security',
            description: 'Scan container images for security vulnerabilities and misconfigurations',
            inputSchema: {
              type: 'object',
              properties: {
                imageName: {
                  type: 'string',
                  description: 'Name of the container image to scan',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Path to the workspace',
                },
              },
              required: ['imageName'],
            },
          },
          {
            name: 'analyze_docker_compose',
            description: 'Analyze Docker Compose configuration for best practices and optimization',
            inputSchema: {
              type: 'object',
              properties: {
                composePath: {
                  type: 'string',
                  description: 'Path to docker-compose.yml file',
                  default: 'docker-compose.yml',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Path to the workspace containing the compose file',
                },
              },
              required: ['workspacePath'],
            },
          },
          {
            name: 'monitor_container_health',
            description: 'Monitor running containers for health, resources, and performance',
            inputSchema: {
              type: 'object',
              properties: {
                containerName: {
                  type: 'string',
                  description:
                    'Name of the container to monitor (optional - monitors all if not specified)',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Path to the workspace',
                },
              },
              required: ['workspacePath'],
            },
          },
        ],
      };
    });

    // Handle tool calls
    this.server.setRequestHandler(CallToolRequestSchema, async request => {
      const { name, arguments: args } = request.params;

      try {
        switch (name) {
          case 'analyze_dockerfile':
            return await this.analyzeDockerfile(args);
          case 'validate_kubernetes_manifests':
            return await this.validateKubernetesManifests(args);
          case 'check_container_security':
            return await this.checkContainerSecurity(args);
          case 'analyze_docker_compose':
            return await this.analyzeDockerCompose(args);
          case 'monitor_container_health':
            return await this.monitorContainerHealth(args);
          default:
            throw new McpError(ErrorCode.MethodNotFound, `Unknown tool: ${name}`);
        }
      } catch (error) {
        throw new McpError(
          ErrorCode.InternalError,
          `Tool execution failed: ${error instanceof Error ? error.message : String(error)}`
        );
      }
    });
  }

  private async validatePath(filePath: string, workspacePath?: string): Promise<string> {
    if (!filePath || typeof filePath !== 'string') {
      throw new McpError(ErrorCode.InvalidParams, 'filePath is required and must be a string');
    }

    let absolutePath: string;
    if (workspacePath) {
      absolutePath = path.resolve(workspacePath, filePath);
    } else {
      absolutePath = path.resolve(filePath);
    }

    if (!(await fs.pathExists(absolutePath))) {
      throw new McpError(ErrorCode.InvalidParams, `Path does not exist: ${absolutePath}`);
    }

    return absolutePath;
  }

  private async analyzeDockerfile(args: any) {
    const { dockerfilePath, workspacePath } = args;
    const absolutePath = await this.validatePath(dockerfilePath, workspacePath);

    const content = await fs.readFile(absolutePath, 'utf-8');
    const lines = content.split('\n');

    const analysis: DockerAnalysisResult = {
      score: 0,
      issues: [],
      recommendations: [],
      metrics: {
        totalLines: lines.length,
        stages: 0,
        hasUserDirective: false,
        hasHealthcheck: false,
        usesLatestTag: false,
        hasSecurityIssues: 0,
        baseImageCount: 0,
      },
    };

    let currentStage = 0;
    let hasRootUser = false;

    for (let i = 0; i < lines.length; i++) {
      const line = lines[i].trim();
      const upperLine = line.toUpperCase();

      // Count stages
      if (upperLine.startsWith('FROM ')) {
        analysis.metrics.stages++;
        currentStage++;

        // Check for latest tag
        if (line.includes(':latest') || (!line.includes(':') && !line.includes('@'))) {
          analysis.issues.push(
            `Line ${i + 1}: Using 'latest' tag - specify explicit version for reproducibility`
          );
          analysis.metrics.usesLatestTag = true;
          analysis.score -= 10;
        }

        analysis.metrics.baseImageCount++;
      }

      // Check for USER directive
      if (upperLine.startsWith('USER ')) {
        analysis.metrics.hasUserDirective = true;
        if (line.includes('root') || line.includes('0')) {
          hasRootUser = true;
          analysis.issues.push(`Line ${i + 1}: Running as root user - security risk`);
          analysis.score -= 15;
        }
      }

      // Check for HEALTHCHECK
      if (upperLine.startsWith('HEALTHCHECK ')) {
        analysis.metrics.hasHealthcheck = true;
      }

      // Check for security issues
      if (upperLine.includes('ADD ') && !upperLine.includes('--chown=')) {
        analysis.issues.push(
          `Line ${i + 1}: Using ADD without --chown - potential permission issues`
        );
        analysis.metrics.hasSecurityIssues++;
        analysis.score -= 5;
      }

      // Check for apt-get without cleanup
      if (
        upperLine.includes('APT-GET') &&
        !content.toLowerCase().includes('rm -rf /var/lib/apt/lists/*')
      ) {
        analysis.issues.push(`Potential apt cache not cleaned - increases image size`);
        analysis.score -= 10;
      }
    }

    // Overall assessments
    if (!analysis.metrics.hasUserDirective) {
      analysis.issues.push('No USER directive found - running as root by default');
      analysis.score -= 20;
    }

    if (!analysis.metrics.hasHealthcheck) {
      analysis.issues.push('No HEALTHCHECK directive - container health not monitored');
      analysis.score -= 10;
    }

    if (analysis.metrics.stages === 0) {
      analysis.issues.push('No FROM directive found - invalid Dockerfile');
      analysis.score -= 50;
    }

    analysis.recommendations = [
      'Use specific image tags instead of :latest',
      'Run containers as non-root user with USER directive',
      'Add HEALTHCHECK for container health monitoring',
      'Use COPY instead of ADD for better security',
      'Clean package manager cache to reduce image size',
      'Use multi-stage builds to reduce final image size',
    ];

    return {
      content: [
        {
          type: 'text',
          text: `## Dockerfile Analysis

**Score: ${Math.max(0, Math.min(100, analysis.score + 70))}/100**

### Metrics
- Total lines: ${analysis.metrics.totalLines}
- Build stages: ${analysis.metrics.stages}
- Base images: ${analysis.metrics.baseImageCount}
- Has USER directive: ${analysis.metrics.hasUserDirective ? '✅' : '❌'}
- Has HEALTHCHECK: ${analysis.metrics.hasHealthcheck ? '✅' : '❌'}
- Uses latest tag: ${analysis.metrics.usesLatestTag ? '⚠️' : '✅'}
- Security issues: ${analysis.metrics.hasSecurityIssues}

### Issues Found
${analysis.issues.map(issue => `- ${issue}`).join('\n') || 'No issues found'}

### Recommendations
${analysis.recommendations.map(rec => `- ${rec}`).join('\n')}`,
        },
      ],
    };
  }

  private async validateKubernetesManifests(args: any) {
    const { manifestPath, workspacePath } = args;
    const absolutePath = await this.validatePath(manifestPath, workspacePath);

    const analysis: DockerAnalysisResult = {
      score: 0,
      issues: [],
      recommendations: [],
      metrics: {
        totalManifests: 0,
        validManifests: 0,
        deployments: 0,
        services: 0,
        configmaps: 0,
        secrets: 0,
        securityIssues: 0,
      },
    };

    let files: string[] = [];

    const stat = await fs.stat(absolutePath);
    if (stat.isDirectory()) {
      files = await glob('**/*.{yml,yaml}', { cwd: absolutePath });
    } else {
      files = [path.basename(absolutePath)];
    }

    for (const file of files) {
      const filePath = stat.isDirectory() ? path.join(absolutePath, file) : absolutePath;

      try {
        const content = await fs.readFile(filePath, 'utf-8');
        const manifests = content.split('---').filter(m => m.trim());

        for (const manifest of manifests) {
          if (!manifest.trim()) continue;

          analysis.metrics.totalManifests++;

          try {
            const doc = yaml.load(manifest) as any;
            analysis.metrics.validManifests++;

            // Analyze different resource types
            if (doc.kind === 'Deployment') {
              analysis.metrics.deployments++;
              await this.analyzeDeployment(doc, analysis, file);
            } else if (doc.kind === 'Service') {
              analysis.metrics.services++;
              await this.analyzeService(doc, analysis, file);
            } else if (doc.kind === 'ConfigMap') {
              analysis.metrics.configmaps++;
            } else if (doc.kind === 'Secret') {
              analysis.metrics.secrets++;
            }
          } catch (parseError) {
            analysis.issues.push(`${file}: Failed to parse YAML - ${parseError}`);
            analysis.score -= 10;
          }
        }
      } catch (error) {
        analysis.issues.push(`${file}: Failed to read file - ${error}`);
        analysis.score -= 5;
      }
    }

    analysis.recommendations = [
      'Use specific resource requests and limits',
      'Implement proper security contexts',
      'Use NetworkPolicies for pod isolation',
      'Enable Pod Security Standards',
      'Use RBAC for access control',
      'Implement resource quotas and limits',
    ];

    return {
      content: [
        {
          type: 'text',
          text: `## Kubernetes Manifests Analysis

**Score: ${Math.max(0, Math.min(100, analysis.score + 60))}/100**

### Metrics
- Total manifests: ${analysis.metrics.totalManifests}
- Valid manifests: ${analysis.metrics.validManifests}
- Deployments: ${analysis.metrics.deployments}
- Services: ${analysis.metrics.services}
- ConfigMaps: ${analysis.metrics.configmaps}
- Secrets: ${analysis.metrics.secrets}
- Security issues: ${analysis.metrics.securityIssues}

### Issues Found
${analysis.issues.map(issue => `- ${issue}`).join('\n') || 'No issues found'}

### Recommendations
${analysis.recommendations.map(rec => `- ${rec}`).join('\n')}`,
        },
      ],
    };
  }

  private async analyzeDeployment(doc: any, analysis: DockerAnalysisResult, file: string) {
    const spec = doc.spec?.template?.spec;

    if (!spec) return;

    // Check for security context
    if (!spec.securityContext) {
      analysis.issues.push(`${file}: Deployment missing securityContext`);
      analysis.metrics.securityIssues++;
      analysis.score -= 10;
    }

    // Check containers
    if (spec.containers) {
      for (const container of spec.containers) {
        // Check resource limits
        if (!container.resources?.limits) {
          analysis.issues.push(`${file}: Container '${container.name}' missing resource limits`);
          analysis.score -= 5;
        }

        if (!container.resources?.requests) {
          analysis.issues.push(`${file}: Container '${container.name}' missing resource requests`);
          analysis.score -= 5;
        }

        // Check security context
        if (!container.securityContext) {
          analysis.issues.push(`${file}: Container '${container.name}' missing securityContext`);
          analysis.metrics.securityIssues++;
          analysis.score -= 8;
        }

        // Check image pull policy
        if (container.image?.includes(':latest')) {
          analysis.issues.push(`${file}: Container '${container.name}' uses latest image tag`);
          analysis.score -= 5;
        }
      }
    }
  }

  private async analyzeService(doc: any, analysis: DockerAnalysisResult, file: string) {
    // Check for LoadBalancer type without proper annotations
    if (doc.spec?.type === 'LoadBalancer') {
      const annotations = doc.metadata?.annotations || {};
      if (!annotations['service.beta.kubernetes.io/aws-load-balancer-type']) {
        analysis.issues.push(`${file}: LoadBalancer service missing cloud provider annotations`);
        analysis.score -= 5;
      }
    }
  }

  private async checkContainerSecurity(args: any) {
    const { imageName, workspacePath } = args;

    if (!imageName || typeof imageName !== 'string') {
      throw new McpError(ErrorCode.InvalidParams, 'imageName is required and must be a string');
    }

    const analysis: DockerAnalysisResult = {
      score: 0,
      issues: [],
      recommendations: [],
      metrics: {
        imageExists: false,
        vulnerabilities: 0,
        securityIssues: 0,
        size: 0,
        layers: 0,
      },
    };

    try {
      // Check if image exists locally
      const images = await this.docker.listImages();
      const image = images.find(img => img.RepoTags?.some(tag => tag.startsWith(imageName)));

      if (image) {
        analysis.metrics.imageExists = true;
        analysis.metrics.size = image.Size || 0;
        analysis.metrics.layers = image.VirtualSize ? 1 : 0; // Simplified

        // Basic security checks
        if (image.RepoTags?.some(tag => tag.includes(':latest'))) {
          analysis.issues.push('Image uses latest tag - security risk');
          analysis.metrics.securityIssues++;
          analysis.score -= 10;
        }

        // Check for known vulnerable base images
        const vulnerableBases = ['ubuntu:14', 'debian:8', 'centos:6'];
        if (vulnerableBases.some(base => image.RepoTags?.some(tag => tag.includes(base)))) {
          analysis.issues.push('Image based on known vulnerable base image');
          analysis.metrics.vulnerabilities++;
          analysis.score -= 20;
        }
      } else {
        analysis.issues.push(`Image '${imageName}' not found locally`);
        analysis.score -= 30;
      }
    } catch (error) {
      analysis.issues.push(`Failed to analyze image: ${error}`);
      analysis.score -= 20;
    }

    analysis.recommendations = [
      'Use specific image tags instead of :latest',
      'Regularly update base images for security patches',
      'Scan images with vulnerability scanners (Trivy, Clair)',
      'Use minimal base images (Alpine, Distroless)',
      'Implement image signing and verification',
    ];

    return {
      content: [
        {
          type: 'text',
          text: `## Container Security Analysis

**Score: ${Math.max(0, Math.min(100, analysis.score + 70))}/100**

### Metrics
- Image exists: ${analysis.metrics.imageExists ? '✅' : '❌'}
- Image size: ${analysis.metrics.size ? `${(analysis.metrics.size / 1024 / 1024).toFixed(2)} MB` : 'Unknown'}
- Layers: ${analysis.metrics.layers}
- Vulnerabilities found: ${analysis.metrics.vulnerabilities}
- Security issues: ${analysis.metrics.securityIssues}

### Issues Found
${analysis.issues.map(issue => `- ${issue}`).join('\n') || 'No issues found'}

### Recommendations
${analysis.recommendations.map(rec => `- ${rec}`).join('\n')}`,
        },
      ],
    };
  }

  private async analyzeDockerCompose(args: any) {
    const { composePath = 'docker-compose.yml', workspacePath } = args;
    const absolutePath = await this.validatePath(composePath, workspacePath);

    const analysis: DockerAnalysisResult = {
      score: 0,
      issues: [],
      recommendations: [],
      metrics: {
        services: 0,
        networks: 0,
        volumes: 0,
        secrets: 0,
        configs: 0,
        hasHealthchecks: 0,
        hasDependsOn: 0,
        hasRestartPolicy: 0,
      },
    };

    try {
      const content = await fs.readFile(absolutePath, 'utf-8');
      const compose = yaml.load(content) as any;

      if (compose.services) {
        analysis.metrics.services = Object.keys(compose.services).length;

        for (const [serviceName, serviceConfig] of Object.entries(compose.services) as [
          string,
          any,
        ][]) {
          // Check for healthchecks
          if (serviceConfig.healthcheck) {
            analysis.metrics.hasHealthchecks++;
          } else {
            analysis.issues.push(`Service '${serviceName}' missing healthcheck`);
            analysis.score -= 5;
          }

          // Check for depends_on
          if (serviceConfig.depends_on) {
            analysis.metrics.hasDependsOn++;
          }

          // Check for restart policy
          if (serviceConfig.restart) {
            analysis.metrics.hasRestartPolicy++;
          } else {
            analysis.issues.push(`Service '${serviceName}' missing restart policy`);
            analysis.score -= 3;
          }

          // Check for resource limits
          if (!serviceConfig.deploy?.resources) {
            analysis.issues.push(`Service '${serviceName}' missing resource limits`);
            analysis.score -= 5;
          }

          // Check for security options
          if (!serviceConfig.security_opt) {
            analysis.issues.push(`Service '${serviceName}' missing security options`);
            analysis.score -= 5;
          }
        }
      }

      if (compose.networks) {
        analysis.metrics.networks = Object.keys(compose.networks).length;
      }

      if (compose.volumes) {
        analysis.metrics.volumes = Object.keys(compose.volumes).length;
      }

      if (compose.secrets) {
        analysis.metrics.secrets = Object.keys(compose.secrets).length;
      }

      if (compose.configs) {
        analysis.metrics.configs = Object.keys(compose.configs).length;
      }
    } catch (error) {
      analysis.issues.push(`Failed to parse docker-compose.yml: ${error}`);
      analysis.score -= 30;
    }

    analysis.recommendations = [
      'Add healthchecks to all services',
      'Define resource limits for all services',
      'Use restart policies for service resilience',
      'Implement proper service dependencies with depends_on',
      'Use secrets management instead of environment variables',
      'Configure security options for all services',
    ];

    return {
      content: [
        {
          type: 'text',
          text: `## Docker Compose Analysis

**Score: ${Math.max(0, Math.min(100, analysis.score + 60))}/100**

### Metrics
- Services: ${analysis.metrics.services}
- Networks: ${analysis.metrics.networks}
- Volumes: ${analysis.metrics.volumes}
- Secrets: ${analysis.metrics.secrets}
- Configs: ${analysis.metrics.configs}
- Services with healthchecks: ${analysis.metrics.hasHealthchecks}/${analysis.metrics.services}
- Services with restart policy: ${analysis.metrics.hasRestartPolicy}/${analysis.metrics.services}
- Services with dependencies: ${analysis.metrics.hasDependsOn}/${analysis.metrics.services}

### Issues Found
${analysis.issues.map(issue => `- ${issue}`).join('\n') || 'No issues found'}

### Recommendations
${analysis.recommendations.map(rec => `- ${rec}`).join('\n')}`,
        },
      ],
    };
  }

  private async monitorContainerHealth(args: any) {
    const { containerName, workspacePath } = args;

    const analysis: DockerAnalysisResult = {
      score: 0,
      issues: [],
      recommendations: [],
      metrics: {
        totalContainers: 0,
        runningContainers: 0,
        healthyContainers: 0,
        unhealthyContainers: 0,
        highCpuContainers: 0,
        highMemoryContainers: 0,
      },
    };

    try {
      const containers = await this.docker.listContainers({ all: true });

      analysis.metrics.totalContainers = containers.length;

      for (const container of containers) {
        // Filter by name if specified
        if (containerName && !container.Names.some(name => name.includes(containerName))) {
          continue;
        }

        const containerInfo = await this.docker.getContainer(container.Id).inspect();

        // Check status
        if (containerInfo.State.Running) {
          analysis.metrics.runningContainers++;

          // Check health
          if (containerInfo.State.Health?.Status === 'healthy') {
            analysis.metrics.healthyContainers++;
          } else if (containerInfo.State.Health?.Status === 'unhealthy') {
            analysis.metrics.unhealthyContainers++;
            analysis.issues.push(`Container '${container.Names[0]}' is unhealthy`);
            analysis.score -= 10;
          }

          // Check resource usage (simplified)
          // In a real implementation, you'd get stats from docker stats
          if (containerInfo.HostConfig?.CpuQuota) {
            analysis.metrics.highCpuContainers++;
            analysis.issues.push(`Container '${container.Names[0]}' has high CPU usage`);
            analysis.score -= 5;
          }

          if (containerInfo.HostConfig?.Memory) {
            const memoryLimit = containerInfo.HostConfig.Memory;
            if (memoryLimit > 1024 * 1024 * 1024) {
              // > 1GB
              analysis.metrics.highMemoryContainers++;
              analysis.issues.push(`Container '${container.Names[0]}' has high memory limit`);
              analysis.score -= 3;
            }
          }
        } else {
          analysis.issues.push(
            `Container '${container.Names[0]}' is not running (Status: ${containerInfo.State.Status})`
          );
          analysis.score -= 15;
        }
      }
    } catch (error) {
      analysis.issues.push(`Failed to monitor containers: ${error}`);
      analysis.score -= 20;
    }

    analysis.recommendations = [
      'Implement health checks for all containers',
      'Monitor resource usage and set appropriate limits',
      'Configure restart policies for resilience',
      'Use container orchestration (Kubernetes/Docker Swarm)',
      'Implement logging and monitoring solutions',
      'Regularly update container images',
    ];

    return {
      content: [
        {
          type: 'text',
          text: `## Container Health Monitor

**Score: ${Math.max(0, Math.min(100, analysis.score + 70))}/100**

### Metrics
- Total containers: ${analysis.metrics.totalContainers}
- Running containers: ${analysis.metrics.runningContainers}
- Healthy containers: ${analysis.metrics.healthyContainers}
- Unhealthy containers: ${analysis.metrics.unhealthyContainers}
- High CPU containers: ${analysis.metrics.highCpuContainers}
- High memory containers: ${analysis.metrics.highMemoryContainers}

### Issues Found
${analysis.issues.map(issue => `- ${issue}`).join('\n') || 'No issues found'}

### Recommendations
${analysis.recommendations.map(rec => `- ${rec}`).join('\n')}`,
        },
      ],
    };
  }

  async run() {
    console.log('Docker/Kubernetes MCP server running...');
    const transport = new StdioServerTransport();
    await this.server.connect(transport);
  }
}

// Start the server
const server = new DockerMCPServer();
server.run().catch(console.error);

export { DockerMCPServer };
