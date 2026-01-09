import { Server } from '@modelcontextprotocol/sdk/server/index.js';
import { StdioServerTransport } from '@modelcontextprotocol/sdk/server/stdio.js';
import {
  CallToolRequestSchema,
  ErrorCode,
  ListToolsRequestSchema,
  McpError,
} from '@modelcontextprotocol/sdk/types.js';
import simpleGit, { SimpleGit } from 'simple-git';
import * as fs from 'fs-extra';
import * as path from 'path';
import { glob } from 'glob';

interface GitAnalysisResult {
  score: number;
  issues: string[];
  recommendations: string[];
  metrics: Record<string, any>;
}

class GitMCPServer {
  private server: Server;
  private git: SimpleGit;

  constructor() {
    this.server = new Server({
      name: 'git-mcp-server',
      version: '1.0.0',
    });

    this.git = simpleGit();
    this.setupToolHandlers();
  }

  private setupToolHandlers() {
    // List available tools
    this.server.setRequestHandler(ListToolsRequestSchema, async () => {
      return {
        tools: [
          {
            name: 'analyze_commit_history',
            description: 'Analyze commit history patterns, quality, and frequency',
            inputSchema: {
              type: 'object',
              properties: {
                workspacePath: {
                  type: 'string',
                  description: 'Path to the git repository workspace',
                },
                days: {
                  type: 'number',
                  description: 'Number of days to analyze (default: 30)',
                  default: 30,
                },
              },
              required: ['workspacePath'],
            },
          },
          {
            name: 'check_branch_strategy',
            description: 'Validate branch naming strategy and workflow compliance',
            inputSchema: {
              type: 'object',
              properties: {
                workspacePath: {
                  type: 'string',
                  description: 'Path to the git repository workspace',
                },
                strategy: {
                  type: 'string',
                  description:
                    'Branching strategy to validate against (git-flow, github-flow, trunk-based)',
                  enum: ['git-flow', 'github-flow', 'trunk-based'],
                  default: 'github-flow',
                },
              },
              required: ['workspacePath'],
            },
          },
          {
            name: 'validate_commit_messages',
            description: 'Check commit message quality and conventional commit compliance',
            inputSchema: {
              type: 'object',
              properties: {
                workspacePath: {
                  type: 'string',
                  description: 'Path to the git repository workspace',
                },
                count: {
                  type: 'number',
                  description: 'Number of recent commits to validate (default: 50)',
                  default: 50,
                },
              },
              required: ['workspacePath'],
            },
          },
          {
            name: 'analyze_code_churn',
            description: 'Analyze code change patterns and identify high-churn files',
            inputSchema: {
              type: 'object',
              properties: {
                workspacePath: {
                  type: 'string',
                  description: 'Path to the git repository workspace',
                },
                days: {
                  type: 'number',
                  description: 'Number of days to analyze (default: 30)',
                  default: 30,
                },
              },
              required: ['workspacePath'],
            },
          },
          {
            name: 'detect_merge_conflicts',
            description: 'Identify files prone to merge conflicts based on change patterns',
            inputSchema: {
              type: 'object',
              properties: {
                workspacePath: {
                  type: 'string',
                  description: 'Path to the git repository workspace',
                },
                threshold: {
                  type: 'number',
                  description:
                    'Minimum number of contributors to flag as conflict-prone (default: 3)',
                  default: 3,
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
          case 'analyze_commit_history':
            return await this.analyzeCommitHistory(args);
          case 'check_branch_strategy':
            return await this.checkBranchStrategy(args);
          case 'validate_commit_messages':
            return await this.validateCommitMessages(args);
          case 'analyze_code_churn':
            return await this.analyzeCodeChurn(args);
          case 'detect_merge_conflicts':
            return await this.detectMergeConflicts(args);
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

  private async validateWorkspacePath(workspacePath: string): Promise<void> {
    if (!workspacePath || typeof workspacePath !== 'string') {
      throw new McpError(ErrorCode.InvalidParams, 'workspacePath is required and must be a string');
    }

    const absolutePath = path.resolve(workspacePath);
    if (!(await fs.pathExists(absolutePath))) {
      throw new McpError(ErrorCode.InvalidParams, `Workspace path does not exist: ${absolutePath}`);
    }

    const gitPath = path.join(absolutePath, '.git');
    if (!(await fs.pathExists(gitPath))) {
      throw new McpError(ErrorCode.InvalidParams, `Not a git repository: ${absolutePath}`);
    }
  }

  private async analyzeCommitHistory(args: any) {
    const { workspacePath, days = 30 } = args;
    await this.validateWorkspacePath(workspacePath);

    const git = simpleGit(workspacePath);
    const since = new Date();
    since.setDate(since.getDate() - days);

    const log = await git.log({
      since: since.toISOString(),
      format: {
        hash: '%H',
        date: '%ai',
        author: '%an',
        email: '%ae',
        message: '%s',
        refs: '%D',
      },
    });

    const commits = log.all;
    const analysis: GitAnalysisResult = {
      score: 0,
      issues: [],
      recommendations: [],
      metrics: {
        totalCommits: commits.length,
        uniqueAuthors: new Set(commits.map(c => c.email)).size,
        avgCommitsPerDay: commits.length / days,
        dateRange: `${since.toISOString().split('T')[0]} to ${new Date().toISOString().split('T')[0]}`,
      },
    };

    // Analyze commit frequency
    if (analysis.metrics.avgCommitsPerDay < 1) {
      analysis.issues.push('Low commit frequency - consider more frequent, smaller commits');
      analysis.score -= 20;
    } else if (analysis.metrics.avgCommitsPerDay > 10) {
      analysis.issues.push('High commit frequency - consider consolidating related changes');
      analysis.score -= 10;
    } else {
      analysis.score += 20;
    }

    // Analyze author diversity
    if (analysis.metrics.uniqueAuthors === 1) {
      analysis.issues.push('Single author only - consider team collaboration');
      analysis.score -= 15;
    } else if (analysis.metrics.uniqueAuthors >= 3) {
      analysis.score += 15;
    }

    // Check for large commits (potential issues)
    const largeCommits = commits.filter(c => c.message.length > 100);
    if (largeCommits.length > commits.length * 0.3) {
      analysis.issues.push('Many commits with long messages - ensure messages are concise');
      analysis.score -= 10;
    }

    analysis.recommendations = [
      'Aim for 1-5 commits per day for optimal reviewability',
      'Encourage team collaboration with multiple authors',
      'Keep commit messages clear and under 100 characters',
      'Use conventional commit format for better automation',
    ];

    return {
      content: [
        {
          type: 'text',
          text: `## Commit History Analysis

**Score: ${Math.max(0, Math.min(100, analysis.score + 50))}/100**

### Metrics
- Total commits: ${analysis.metrics.totalCommits}
- Unique authors: ${analysis.metrics.uniqueAuthors}
- Average commits/day: ${analysis.metrics.avgCommitsPerDay.toFixed(1)}
- Analysis period: ${analysis.metrics.dateRange}

### Issues Found
${analysis.issues.map(issue => `- ${issue}`).join('\n')}

### Recommendations
${analysis.recommendations.map(rec => `- ${rec}`).join('\n')}`,
        },
      ],
    };
  }

  private async checkBranchStrategy(args: any) {
    const { workspacePath, strategy = 'github-flow' } = args;
    await this.validateWorkspacePath(workspacePath);

    const git = simpleGit(workspacePath);
    const branches = await git.branch();
    const currentBranch = branches.current;

    const analysis: GitAnalysisResult = {
      score: 0,
      issues: [],
      recommendations: [],
      metrics: {
        currentBranch,
        totalBranches: branches.all.length,
        localBranches: branches.branches,
        strategy,
      },
    };

    // Strategy-specific validation
    switch (strategy) {
      case 'github-flow':
        if (
          !currentBranch.startsWith('feature/') &&
          !currentBranch.startsWith('bugfix/') &&
          !currentBranch.startsWith('hotfix/') &&
          currentBranch !== 'main' &&
          currentBranch !== 'develop'
        ) {
          analysis.issues.push(
            `Branch '${currentBranch}' doesn't follow GitHub Flow naming convention`
          );
          analysis.score -= 20;
        }
        break;

      case 'git-flow':
        const validPrefixes = ['feature/', 'bugfix/', 'hotfix/', 'release/', 'support/'];
        if (
          !validPrefixes.some(prefix => currentBranch.startsWith(prefix)) &&
          !['main', 'develop'].includes(currentBranch)
        ) {
          analysis.issues.push(
            `Branch '${currentBranch}' doesn't follow Git Flow naming convention`
          );
          analysis.score -= 20;
        }
        break;

      case 'trunk-based':
        if (currentBranch !== 'main' && currentBranch !== 'trunk') {
          analysis.issues.push(`Trunk-based development should use 'main' or 'trunk' branch`);
          analysis.score -= 15;
        }
        break;
    }

    // Check for stale branches
    const staleBranches = branches.all.filter(branch => {
      // This is a simplified check - in practice, you'd check last commit date
      return branch.includes('feature/') || branch.includes('bugfix/');
    });

    if (staleBranches.length > 5) {
      analysis.issues.push('Many feature/bugfix branches - consider cleaning up stale branches');
      analysis.score -= 10;
    }

    analysis.recommendations = [
      `Follow ${strategy} branching strategy consistently`,
      'Use descriptive branch names with appropriate prefixes',
      'Regularly clean up merged branches',
      'Consider branch protection rules for main/develop branches',
    ];

    return {
      content: [
        {
          type: 'text',
          text: `## Branch Strategy Analysis (${strategy})

**Score: ${Math.max(0, Math.min(100, analysis.score + 70))}/100**

### Current State
- Current branch: ${analysis.metrics.currentBranch}
- Total branches: ${analysis.metrics.totalBranches}
- Strategy: ${analysis.metrics.strategy}

### Issues Found
${analysis.issues.map(issue => `- ${issue}`).join('\n') || 'No issues found'}

### Recommendations
${analysis.recommendations.map(rec => `- ${rec}`).join('\n')}`,
        },
      ],
    };
  }

  private async validateCommitMessages(args: any) {
    const { workspacePath, count = 50 } = args;
    await this.validateWorkspacePath(workspacePath);

    const git = simpleGit(workspacePath);
    const log = await git.log({
      maxCount: count,
      format: {
        hash: '%H',
        message: '%s',
        author: '%an',
      },
    });

    const commits = log.all;
    const analysis: GitAnalysisResult = {
      score: 0,
      issues: [],
      recommendations: [],
      metrics: {
        totalCommits: commits.length,
        conventionalCommits: 0,
        avgMessageLength: 0,
        emptyMessages: 0,
      },
    };

    let totalLength = 0;

    commits.forEach(commit => {
      const message = commit.message.trim();

      // Check for empty messages
      if (!message) {
        analysis.metrics.emptyMessages++;
        analysis.issues.push(`Empty commit message in ${commit.hash.substring(0, 7)}`);
        analysis.score -= 10;
      }

      // Check conventional commit format
      const conventionalPattern =
        /^(feat|fix|docs|style|refactor|test|chore|perf|ci|build|revert)(\(.+\))?: .+/;
      if (conventionalPattern.test(message)) {
        analysis.metrics.conventionalCommits++;
      }

      totalLength += message.length;

      // Check message length
      if (message.length > 100) {
        analysis.issues.push(`Commit message too long in ${commit.hash.substring(0, 7)}`);
        analysis.score -= 5;
      }
    });

    analysis.metrics.avgMessageLength = totalLength / commits.length;

    // Score based on conventional commits
    const conventionalPercentage = (analysis.metrics.conventionalCommits / commits.length) * 100;
    if (conventionalPercentage < 30) {
      analysis.issues.push(
        'Low conventional commit adoption - consider standardizing commit format'
      );
      analysis.score -= 20;
    } else if (conventionalPercentage > 70) {
      analysis.score += 20;
    }

    analysis.recommendations = [
      'Use conventional commit format: type(scope): description',
      'Keep commit messages under 100 characters',
      'Avoid empty commit messages',
      'Use imperative mood in commit messages',
    ];

    return {
      content: [
        {
          type: 'text',
          text: `## Commit Message Validation

**Score: ${Math.max(0, Math.min(100, analysis.score + 60))}/100**

### Metrics
- Total commits analyzed: ${analysis.metrics.totalCommits}
- Conventional commits: ${analysis.metrics.conventionalCommits} (${((analysis.metrics.conventionalCommits / analysis.metrics.totalCommits) * 100).toFixed(1)}%)
- Average message length: ${analysis.metrics.avgMessageLength.toFixed(1)} characters
- Empty messages: ${analysis.metrics.emptyMessages}

### Issues Found
${
  analysis.issues
    .slice(0, 10)
    .map(issue => `- ${issue}`)
    .join('\n') || 'No issues found'
}
${analysis.issues.length > 10 ? `\n... and ${analysis.issues.length - 10} more issues` : ''}

### Recommendations
${analysis.recommendations.map(rec => `- ${rec}`).join('\n')}`,
        },
      ],
    };
  }

  private async analyzeCodeChurn(args: any) {
    const { workspacePath, days = 30 } = args;
    await this.validateWorkspacePath(workspacePath);

    const git = simpleGit(workspacePath);
    const since = new Date();
    since.setDate(since.getDate() - days);

    // Get diff stats
    const diff = await git.diffSummary([`--since="${since.toISOString()}"`]);

    const analysis: GitAnalysisResult = {
      score: 0,
      issues: [],
      recommendations: [],
      metrics: {
        totalFiles: diff.files.length,
        insertions: diff.insertions,
        deletions: diff.deletions,
        changedLines: diff.insertions + diff.deletions,
        churnRate: 0,
        highChurnFiles: [] as string[],
      },
    };

    // Calculate churn rate (changes per day)
    analysis.metrics.churnRate = analysis.metrics.changedLines / days;

    // Identify high-churn files
    const fileChanges = diff.files
      .filter((file): file is any => 'insertions' in file && 'deletions' in file)
      .map(file => ({
        file: file.file,
        changes: file.insertions + file.deletions,
      }))
      .sort((a, b) => b.changes - a.changes);

    analysis.metrics.highChurnFiles = fileChanges
      .filter(f => f.changes > analysis.metrics.changedLines * 0.1)
      .slice(0, 5)
      .map(f => f.file);

    // Analyze churn patterns
    if (analysis.metrics.churnRate > 100) {
      analysis.issues.push('High code churn - consider more focused changes');
      analysis.score -= 20;
    } else if (analysis.metrics.churnRate < 10) {
      analysis.issues.push('Low code activity - ensure regular development progress');
      analysis.score -= 10;
    } else {
      analysis.score += 15;
    }

    if (analysis.metrics.highChurnFiles.length > 0) {
      analysis.issues.push(`High churn in files: ${analysis.metrics.highChurnFiles.join(', ')}`);
      analysis.score -= 10;
    }

    analysis.recommendations = [
      'Aim for focused, incremental changes rather than large rewrites',
      'Review high-churn files for potential design issues',
      'Consider breaking large changes into smaller, reviewable commits',
      'Monitor churn patterns to identify problematic areas',
    ];

    return {
      content: [
        {
          type: 'text',
          text: `## Code Churn Analysis

**Score: ${Math.max(0, Math.min(100, analysis.score + 50))}/100**

### Metrics
- Total files changed: ${analysis.metrics.totalFiles}
- Lines added: ${analysis.metrics.insertions}
- Lines deleted: ${analysis.metrics.deletions}
- Total changes: ${analysis.metrics.changedLines}
- Churn rate: ${analysis.metrics.churnRate.toFixed(1)} lines/day
- Analysis period: ${days} days

### High-Churn Files
${analysis.metrics.highChurnFiles.map((file: string) => `- ${file}`).join('\n') || 'No high-churn files detected'}

### Issues Found
${analysis.issues.map(issue => `- ${issue}`).join('\n') || 'No issues found'}

### Recommendations
${analysis.recommendations.map(rec => `- ${rec}`).join('\n')}`,
        },
      ],
    };
  }

  private async detectMergeConflicts(args: any) {
    const { workspacePath, threshold = 3 } = args;
    await this.validateWorkspacePath(workspacePath);

    const git = simpleGit(workspacePath);

    // Get recent commits with file changes
    const log = await git.log({
      maxCount: 100,
      format: {
        hash: '%H',
        author: '%an',
      },
    });

    const fileContributors = new Map<string, Set<string>>();

    // Analyze recent commits for file changes
    for (const commit of log.all) {
      try {
        const show = await git.show([commit.hash, '--name-only', '--pretty=format:']);
        const files = show.split('\n').filter(line => line.trim() && !line.startsWith('commit'));

        files.forEach(file => {
          if (!fileContributors.has(file)) {
            fileContributors.set(file, new Set());
          }
          fileContributors.get(file)!.add(commit.author);
        });
      } catch (error) {
        // Skip commits that can't be analyzed
        continue;
      }
    }

    const conflictProneFiles = Array.from(fileContributors.entries())
      .filter(([, contributors]) => contributors.size >= threshold)
      .sort(([, a], [, b]) => b.size - a.size)
      .slice(0, 10);

    const analysis: GitAnalysisResult = {
      score: 0,
      issues: [],
      recommendations: [],
      metrics: {
        totalFiles: fileContributors.size,
        conflictProneFiles: conflictProneFiles.length,
        threshold,
        topConflictFiles: conflictProneFiles,
      },
    };

    if (conflictProneFiles.length > 0) {
      analysis.issues.push(`${conflictProneFiles.length} files identified as conflict-prone`);
      analysis.score -= Math.min(30, conflictProneFiles.length * 3);
    } else {
      analysis.score += 20;
    }

    analysis.recommendations = [
      'Consider breaking large files into smaller modules',
      'Use feature flags for concurrent development',
      'Establish clear ownership for conflict-prone files',
      'Consider trunk-based development for high-conflict areas',
    ];

    return {
      content: [
        {
          type: 'text',
          text: `## Merge Conflict Detection

**Score: ${Math.max(0, Math.min(100, analysis.score + 60))}/100**

### Metrics
- Total files analyzed: ${analysis.metrics.totalFiles}
- Conflict-prone files: ${analysis.metrics.conflictProneFiles}
- Contributor threshold: ${analysis.metrics.threshold}

### Conflict-Prone Files
${
  analysis.metrics.topConflictFiles
    .map(
      ([file, contributors]: [string, Set<string>]) =>
        `- ${file} (${contributors.size} contributors)`
    )
    .join('\n') || 'No conflict-prone files detected'
}

### Issues Found
${analysis.issues.map(issue => `- ${issue}`).join('\n') || 'No issues found'}

### Recommendations
${analysis.recommendations.map(rec => `- ${rec}`).join('\n')}`,
        },
      ],
    };
  }

  async run() {
    console.log('Git MCP server running...');
    const transport = new StdioServerTransport();
    await this.server.connect(transport);
  }
}

// Start the server
const server = new GitMCPServer();
server.run().catch(console.error);

export { GitMCPServer };
