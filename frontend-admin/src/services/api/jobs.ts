import { apiClient } from "../client";
import type { Job, ScheduledJob, JobLog } from "@/types/jobs";
import type { PaginatedResponse, PaginationParams } from "@/types/api";

export const jobsApi = {
  // Jobs
  getJobs(status?: string, pagination?: PaginationParams) {
    // TODO: Implement Jobs service in backend
    return Promise.resolve({ data: [], total: 0 } as PaginatedResponse<Job>);
  },

  getJob(id: string) {
    // TODO: Implement Jobs service in backend
    return Promise.resolve({} as Job);
  },

  getJobLogs(jobId: string) {
    // TODO: Implement Jobs service in backend
    return Promise.resolve([] as JobLog[]);
  },

  retryJob(jobId: string) {
    // TODO: Implement Jobs service in backend
    return Promise.resolve({} as Job);
  },

  cancelJob(jobId: string) {
    // TODO: Implement Jobs service in backend
    return Promise.resolve({} as Job);
  },

  // Scheduled Jobs
  getScheduledJobs(pagination?: PaginationParams) {
    // TODO: Implement Jobs service in backend
    return Promise.resolve({
      data: [],
      total: 0,
    } as PaginatedResponse<ScheduledJob>);
  },

  getScheduledJob(id: string) {
    // TODO: Implement Jobs service in backend
    return Promise.resolve({} as ScheduledJob);
  },

  createScheduledJob(
    data: Omit<ScheduledJob, "id" | "createdAt" | "updatedAt">
  ) {
    // TODO: Implement Jobs service in backend
    return Promise.resolve({} as ScheduledJob);
  },

  updateScheduledJob(id: string, data: Partial<ScheduledJob>) {
    // TODO: Implement Jobs service in backend
    return Promise.resolve({} as ScheduledJob);
  },

  deleteScheduledJob(id: string) {
    // TODO: Implement Jobs service in backend
    return Promise.resolve();
  },

  toggleScheduledJob(id: string, isActive: boolean) {
    // TODO: Implement Jobs service in backend
    return Promise.resolve({} as ScheduledJob);
  },

  // Metrics
  getMetrics() {
    return apiClient.get<any>("/jobs/metrics");
  },
};
