import { apiClient } from '../client';

/**
 * Information about available CLI tools
 */
export interface CliToolInfo {
  toolType: string;
  name: string;
  description: string;
  latestVersion: string;
  isAvailable: boolean;
  supportedOperatingSystems: string[];
  lastUpdated: string;
}

/**
 * Information about CLI tool versions
 */
export interface CliVersionInfo {
  version: string;
  releaseDate: string;
  releaseNotes: string;
  isPrerelease: boolean;
  breakingChanges: string[];
}

/**
 * Installation instructions for CLI tools
 */
export interface CliInstallationInstructions {
  toolType: string;
  operatingSystem: string;
  steps: string[];
  prerequisites: string[];
  configurationTemplate?: string;
  troubleshootingTips: string[];
}

/**
 * CLI Tools API service
 * Handles downloads and management of Administration-CLI and ERP-Connector
 */
export const cliToolsApi = {
  /**
   * Get available CLI tools for the current tenant
   */
  async getAvailableTools(): Promise<CliToolInfo[]> {
    const response = await apiClient.get<CliToolInfo[]>('/cli-tools/available');
    return response.data;
  },

  /**
   * Download Administration-CLI
   * @param version Version to download (e.g., "latest", "1.0.0")
   * @param osType Target OS (win, linux, osx)
   * @returns Blob with the CLI binary
   */
  async downloadAdministrationCli(
    version: string = 'latest',
    osType: string = 'win'
  ): Promise<Blob> {
    const response = await apiClient.instance.get(
      `/cli-tools/download/administration-cli?version=${version}&osType=${osType}`,
      {
        responseType: 'blob',
      }
    );
    return response.data;
  },

  /**
   * Download ERP-Connector
   * @param erpType ERP system type (e.g., "enventa")
   * @param version Version to download (e.g., "latest", "1.0.0")
   * @returns Blob with the connector binary
   */
  async downloadErpConnector(erpType: string, version: string = 'latest'): Promise<Blob> {
    const response = await apiClient.instance.get(
      `/cli-tools/download/erp-connector?erpType=${erpType}&version=${version}`,
      {
        responseType: 'blob',
      }
    );
    return response.data;
  },

  /**
   * Get installation instructions for a tool
   * @param toolType Tool type (administration-cli, erp-connector)
   * @param osType Target OS (win, linux, osx)
   */
  async getInstallationInstructions(
    toolType: string,
    osType: string = 'win'
  ): Promise<CliInstallationInstructions> {
    const response = await apiClient.get<CliInstallationInstructions>(
      `/cli-tools/instructions/${toolType}?osType=${osType}`
    );
    return response.data;
  },

  /**
   * Get available versions for a CLI tool
   * @param toolType Tool type (administration-cli, erp-connector)
   */
  async getAvailableVersions(toolType: string): Promise<CliVersionInfo[]> {
    const response = await apiClient.get<CliVersionInfo[]>(`/cli-tools/versions/${toolType}`);
    return response.data;
  },
};
