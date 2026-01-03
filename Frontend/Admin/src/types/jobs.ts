/* eslint-disable @typescript-eslint/no-explicit-any -- Job parameters are dynamic JSON */

export interface Job {
  id: string;
  name: string;
  type:
    | 'data-sync'
    | 'report-generation'
    | 'email-campaign'
    | 'image-processing'
    | 'etl'
    | 'backup'
    | 'custom';
  status: 'pending' | 'running' | 'completed' | 'failed' | 'cancelled';
  progress: number;
  startedAt?: Date;
  completedAt?: Date;
  createdAt: Date;
  tenantId: string;
  createdBy: string;
  parameters?: Record<string, any>;
  result?: Record<string, any>;
  errorMessage?: string;
}

export interface JobLog {
  id: string;
  jobId: string;
  level: 'info' | 'warning' | 'error' | 'debug';
  message: string;
  timestamp: Date;
  data?: Record<string, any>;
}

export interface ScheduledJob {
  id: string;
  name: string;
  jobType: string;
  cronExpression: string;
  isActive: boolean;
  lastRun?: Date;
  nextRun: Date;
  createdAt: Date;
  updatedAt: Date;
  tenantId: string;
}

export interface JobMonitorMetrics {
  totalJobs: number;
  runningJobs: number;
  completedJobs: number;
  failedJobs: number;
  averageExecutionTime: number;
  successRate: number;
}

export interface JobState {
  jobs: Job[];
  scheduledJobs: ScheduledJob[];
  currentJob: Job | null;
  jobLogs: JobLog[];
  metrics: JobMonitorMetrics;
  isMonitoring: boolean;
  loading: boolean;
  error: string | null;
}

// ============================================================================
// API Error Types
// ============================================================================

export interface JobsApiError {
  message: string;
  code?: string;
  details?: unknown[];
}

export interface JobValidationError {
  field: string;
  message: string;
  code: string;
}

export interface JobExecutionError {
  jobId: string;
  errorCode: string;
  errorMessage: string;
  stackTrace?: string;
  timestamp: Date;
}

// ============================================================================
// API Response Types
// ============================================================================

export interface JobFilters {
  status?: Job['status'];
  type?: Job['type'];
  createdBy?: string;
  dateFrom?: Date;
  dateTo?: Date;
}

export interface ScheduledJobFilters {
  isActive?: boolean;
  jobType?: string;
  search?: string;
}
