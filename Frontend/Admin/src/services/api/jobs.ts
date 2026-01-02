import type { Job, ScheduledJob, JobLog } from "@/types/jobs";
import type { PaginatedResponse, PaginationParams } from "@/types/api";

export const jobsApi = {
  // Jobs

  getJobs(_status?: string, _pagination?: PaginationParams) {
    // TODO: Implement Jobs service in backend
    return Promise.resolve({ data: [], total: 0 } as PaginatedResponse<Job>);
  },

  getJob(_id: string) {
    // TODO: Implement Jobs service in backend
    return Promise.resolve({} as Job);
  },

  getJobLogs(_jobId: string) {
    // TODO: Implement Jobs service in backend
    return Promise.resolve([] as JobLog[]);
  },

  retryJob(_jobId: string) {
    // TODO: Implement Jobs service in backend
    return Promise.resolve({} as Job);
  },

  cancelJob(_jobId: string) {
    // TODO: Implement Jobs service in backend
    return Promise.resolve({} as Job);
  },

  // Scheduled Jobs

  getScheduledJobs(_pagination?: PaginationParams) {
    // TODO: Implement Jobs service in backend
    return Promise.resolve({
      data: [],
      total: 0,
    } as PaginatedResponse<ScheduledJob>);
  },

  getScheduledJob(_id: string) {
    // TODO: Implement Jobs service in backend
    return Promise.resolve({} as ScheduledJob);
  },

  createScheduledJob(
    _data: Omit<ScheduledJob, "id" | "createdAt" | "updatedAt">
  ) {
    // TODO: Implement Jobs service in backend
    return Promise.resolve({} as ScheduledJob);
  },

  updateScheduledJob(_id: string, _data: Partial<ScheduledJob>) {
    // TODO: Implement Jobs service in backend
    return Promise.resolve({} as ScheduledJob);
  },

  deleteScheduledJob(_id: string) {
    // TODO: Implement Jobs service in backend
    return Promise.resolve();
  },

  toggleScheduledJob(_id: string, _isActive: boolean) {
    // TODO: Implement Jobs service in backend
    return Promise.resolve({} as ScheduledJob);
  },

  // Metrics
  getMetrics() {
    return Promise.resolve({} as Record<string, unknown>);
  },
};
