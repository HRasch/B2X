import { apiClient } from '../client'
import type { Job, ScheduledJob, JobLog } from '@/types/jobs'
import type { PaginatedResponse, PaginationParams } from '@/types/api'

export const jobsApi = {
  // Jobs
  getJobs(status?: string, pagination?: PaginationParams) {
    return apiClient.get<PaginatedResponse<Job>>('/jobs/queue', { 
      params: { status, ...pagination } 
    })
  },

  getJob(id: string) {
    return apiClient.get<Job>(`/jobs/${id}`)
  },

  getJobLogs(jobId: string) {
    return apiClient.get<JobLog[]>(`/jobs/${jobId}/logs`)
  },

  retryJob(jobId: string) {
    return apiClient.post<Job>(`/jobs/${jobId}/retry`, {})
  },

  cancelJob(jobId: string) {
    return apiClient.post<Job>(`/jobs/${jobId}/cancel`, {})
  },

  // Scheduled Jobs
  getScheduledJobs(pagination?: PaginationParams) {
    return apiClient.get<PaginatedResponse<ScheduledJob>>('/jobs/scheduled', { params: pagination })
  },

  getScheduledJob(id: string) {
    return apiClient.get<ScheduledJob>(`/jobs/scheduled/${id}`)
  },

  createScheduledJob(data: Omit<ScheduledJob, 'id' | 'createdAt' | 'updatedAt'>) {
    return apiClient.post<ScheduledJob>('/jobs/scheduled', data)
  },

  updateScheduledJob(id: string, data: Partial<ScheduledJob>) {
    return apiClient.put<ScheduledJob>(`/jobs/scheduled/${id}`, data)
  },

  deleteScheduledJob(id: string) {
    return apiClient.delete<void>(`/jobs/scheduled/${id}`)
  },

  toggleScheduledJob(id: string, isActive: boolean) {
    return apiClient.put<ScheduledJob>(`/jobs/scheduled/${id}`, { isActive })
  },

  // Metrics
  getMetrics() {
    return apiClient.get<any>('/jobs/metrics')
  },
}
