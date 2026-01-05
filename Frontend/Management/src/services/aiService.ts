// Note: apiClient removed as it's not used in this service

export interface McpToolCall {
  name: string;
  arguments: Record<string, any>;
}

export interface McpToolResponse {
  content: Array<{
    type: string;
    text: string;
  }>;
  isError?: boolean;
}

export interface AiConsumptionMetrics {
  totalRequests: number;
  totalTokens: number;
  totalCost: number;
  requestsByModel: Record<string, number>;
  costByModel: Record<string, number>;
  recentRequests: Array<{
    timestamp: Date;
    model: string;
    tokens: number;
    cost: number;
  }>;
}

class AiService {
  private readonly baseUrl = import.meta.env.VITE_MCP_SERVER_URL || '/api/admin/mcp';

  /**
   * Call an MCP tool
   */
  async callMcpTool(toolCall: McpToolCall): Promise<string> {
    try {
      const response = await fetch(`${this.baseUrl}/mcp/tools/call`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${this.getAuthToken()}`,
        },
        body: JSON.stringify(toolCall),
      });

      if (!response.ok) {
        throw new Error('Failed to call MCP tool');
      }

      const result = await response.json();
      if (result.isError || !result.content?.[0]?.text) {
        throw new Error('AI assistant returned an error');
      }

      return result.content[0].text;
    } catch (error) {
      console.error('MCP tool call failed:', error);
      throw new Error('AI assistant is currently unavailable. Please try again later.');
    }
  }

  /**
   * Get AI consumption metrics
   */
  async getConsumptionMetrics(): Promise<AiConsumptionMetrics> {
    try {
      const response = await fetch(`${this.baseUrl}/api/admin/ai-consumption/metrics`, {
        method: 'GET',
        headers: {
          Authorization: `Bearer ${this.getAuthToken()}`,
        },
      });

      if (!response.ok) {
        throw new Error('Failed to fetch metrics');
      }

      return await response.json();
    } catch (error) {
      console.error('Failed to fetch AI consumption metrics:', error);
      throw new Error('Unable to load AI consumption data');
    }
  }

  /**
   * Get available MCP tools
   */
  async getAvailableTools(): Promise<
    Array<{
      name: string;
      description: string;
      inputSchema: any;
    }>
  > {
    try {
      const response = await fetch(`${this.baseUrl}/mcp/tools/list`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${this.getAuthToken()}`,
        },
        body: JSON.stringify({}),
      });

      if (!response.ok) {
        throw new Error('Failed to fetch tools');
      }

      const result = await response.json();
      return result.tools || [];
    } catch (error) {
      console.error('Failed to fetch available tools:', error);
      return [];
    }
  }

  /**
   * Analyze text with AI (general purpose)
   */
  async analyzeText(text: string, context?: string): Promise<string> {
    const toolCall: McpToolCall = {
      name: 'content_optimization',
      arguments: {
        contentType: 'text_analysis',
        content: text,
        targetKeywords: context ? [context] : undefined,
      },
    };

    return this.callMcpTool(toolCall);
  }

  /**
   * Get performance analysis
   */
  async getPerformanceAnalysis(component: string = 'system'): Promise<string> {
    const toolCall: McpToolCall = {
      name: 'performance_optimization',
      arguments: {
        component,
        metricType: 'response_time',
        includeHistoricalData: true,
      },
    };

    return this.callMcpTool(toolCall);
  }

  /**
   * Get security assessment
   */
  async getSecurityAssessment(scope: string = 'current'): Promise<string> {
    const toolCall: McpToolCall = {
      name: 'security_compliance',
      arguments: {
        assessmentType: 'comprehensive',
        scope,
        includeRecommendations: true,
      },
    };

    return this.callMcpTool(toolCall);
  }

  /**
   * Get store operations analysis
   */
  async getStoreAnalysis(storeId: string, operation: string = 'analyze'): Promise<string> {
    const toolCall: McpToolCall = {
      name: 'store_operations',
      arguments: {
        operation,
        storeId,
        analysisType: 'performance',
      },
    };

    return this.callMcpTool(toolCall);
  }

  /**
   * Get system health analysis
   */
  async getSystemHealth(component: string = 'all', timeRange: string = '24h'): Promise<string> {
    const toolCall: McpToolCall = {
      name: 'system_health_analysis',
      arguments: {
        component,
        timeRange,
      },
    };

    return this.callMcpTool(toolCall);
  }

  /**
   * Design CMS page
   */
  async designCmsPage(pageType: string, contentRequirements: string): Promise<string> {
    const toolCall: McpToolCall = {
      name: 'cms_page_design',
      arguments: {
        pageType,
        contentRequirements,
        targetAudience: 'general',
      },
    };

    return this.callMcpTool(toolCall);
  }

  /**
   * Design email template
   */
  async designEmailTemplate(emailType: string, contentPurpose: string): Promise<string> {
    const toolCall: McpToolCall = {
      name: 'email_template_design',
      arguments: {
        emailType,
        contentPurpose,
        brandGuidelines: 'professional',
      },
    };

    return this.callMcpTool(toolCall);
  }

  /**
   * Get user management assistance
   */
  async getUserManagementHelp(task: string): Promise<string> {
    const toolCall: McpToolCall = {
      name: 'user_management_assistant',
      arguments: {
        task,
        userContext: 'admin_portal',
      },
    };

    return this.callMcpTool(toolCall);
  }

  /**
   * Get tenant management advice
   */
  async getTenantManagementAdvice(action: string, tenantId: string): Promise<string> {
    const toolCall: McpToolCall = {
      name: 'tenant_management',
      arguments: {
        action,
        tenantId,
        resourceType: 'general',
      },
    };

    return this.callMcpTool(toolCall);
  }

  /**
   * Get integration management help
   */
  async getIntegrationHelp(integrationType: string, action: string): Promise<string> {
    const toolCall: McpToolCall = {
      name: 'integration_management',
      arguments: {
        integrationType,
        action,
      },
    };

    return this.callMcpTool(toolCall);
  }

  /**
   * Determine the most appropriate tool based on user message
   */
  determineToolFromMessage(message: string): string {
    const lowerMessage = message.toLowerCase();

    if (
      lowerMessage.includes('performance') ||
      lowerMessage.includes('optimize') ||
      lowerMessage.includes('speed') ||
      lowerMessage.includes('slow')
    ) {
      return 'performance_optimization';
    }
    if (
      lowerMessage.includes('security') ||
      lowerMessage.includes('audit') ||
      lowerMessage.includes('compliance') ||
      lowerMessage.includes('vulnerability')
    ) {
      return 'security_compliance';
    }
    if (
      lowerMessage.includes('content') ||
      lowerMessage.includes('seo') ||
      lowerMessage.includes('engagement') ||
      lowerMessage.includes('page')
    ) {
      return 'content_optimization';
    }
    if (
      lowerMessage.includes('store') ||
      lowerMessage.includes('sales') ||
      lowerMessage.includes('inventory') ||
      lowerMessage.includes('revenue')
    ) {
      return 'store_operations';
    }
    if (
      lowerMessage.includes('tenant') ||
      lowerMessage.includes('resource') ||
      lowerMessage.includes('onboard') ||
      lowerMessage.includes('multi-tenant')
    ) {
      return 'tenant_management';
    }
    if (
      lowerMessage.includes('integration') ||
      lowerMessage.includes('api') ||
      lowerMessage.includes('webhook') ||
      lowerMessage.includes('connect')
    ) {
      return 'integration_management';
    }
    if (
      lowerMessage.includes('user') ||
      lowerMessage.includes('admin') ||
      lowerMessage.includes('role') ||
      lowerMessage.includes('permission')
    ) {
      return 'user_management_assistant';
    }
    if (
      lowerMessage.includes('email') ||
      lowerMessage.includes('template') ||
      lowerMessage.includes('campaign') ||
      lowerMessage.includes('newsletter')
    ) {
      return 'email_template_design';
    }
    if (
      lowerMessage.includes('cms') ||
      lowerMessage.includes('layout') ||
      lowerMessage.includes('design') ||
      lowerMessage.includes('page')
    ) {
      return 'cms_page_design';
    }
    if (
      lowerMessage.includes('health') ||
      lowerMessage.includes('system') ||
      lowerMessage.includes('monitor') ||
      lowerMessage.includes('status')
    ) {
      return 'system_health_analysis';
    }

    // Default to content optimization for general queries
    return 'content_optimization';
  }

  /**
   * Extract arguments from user message for the determined tool
   */
  extractArgsFromMessage(message: string, toolName: string): Record<string, any> {
    const args: Record<string, any> = {};

    switch (toolName) {
      case 'performance_optimization':
        args.component = this.extractComponent(message) || 'system';
        args.metricType = 'response_time';
        args.includeHistoricalData = true;
        break;
      case 'security_compliance':
        args.assessmentType = 'comprehensive';
        args.scope = 'current';
        args.includeRecommendations = true;
        break;
      case 'content_optimization':
        args.contentType = 'web_content';
        args.content = message;
        break;
      case 'store_operations':
        args.operation = 'analyze';
        args.storeId = 'current';
        args.analysisType = 'performance';
        break;
      case 'tenant_management':
        args.action = 'analyze';
        args.tenantId = 'current';
        break;
      case 'integration_management':
        args.integrationType = this.extractIntegrationType(message) || 'api';
        args.action = 'analyze';
        break;
      case 'user_management_assistant':
        args.task = message;
        args.userContext = 'admin_request';
        break;
      case 'email_template_design':
        args.emailType = this.extractEmailType(message) || 'marketing';
        args.contentPurpose = 'engagement';
        break;
      case 'cms_page_design':
        args.pageType = this.extractPageType(message) || 'landing';
        args.contentRequirements = message;
        break;
      case 'system_health_analysis':
        args.component = 'all';
        args.timeRange = '24h';
        break;
    }

    return args;
  }

  private extractComponent(message: string): string {
    if (message.includes('database') || message.includes('db')) return 'database';
    if (message.includes('api') || message.includes('endpoint')) return 'api';
    if (message.includes('frontend') || message.includes('ui')) return 'frontend';
    if (message.includes('backend') || message.includes('server')) return 'backend';
    return 'system';
  }

  private extractIntegrationType(message: string): string {
    if (message.includes('erp') || message.includes('enventa')) return 'erp';
    if (message.includes('payment')) return 'payment';
    if (message.includes('shipping')) return 'shipping';
    if (message.includes('email') || message.includes('smtp')) return 'email';
    return 'api';
  }

  private extractEmailType(message: string): string {
    if (message.includes('welcome') || message.includes('onboarding')) return 'welcome';
    if (message.includes('marketing') || message.includes('promotion')) return 'marketing';
    if (message.includes('transaction') || message.includes('receipt')) return 'transactional';
    if (message.includes('newsletter')) return 'newsletter';
    return 'marketing';
  }

  private extractPageType(message: string): string {
    if (message.includes('home') || message.includes('landing')) return 'landing';
    if (message.includes('product')) return 'product';
    if (message.includes('category')) return 'category';
    if (message.includes('about')) return 'about';
    if (message.includes('contact')) return 'contact';
    return 'landing';
  }

  private getAuthToken(): string {
    // Get auth token from localStorage or auth service
    return localStorage.getItem('authToken') || '';
  }
}

export default new AiService();
