/**
 * Jobs Store (Pinia)
 * Properly typed error handling and API responses
 */

import { defineStore } from 'pinia';
import { ref } from 'vue';
import type { Job, ScheduledJob, JobLog, JobsApiError, JobFilters } from '@/types/jobs';
import type { ApiError } from '@/types/api';
import { jobsApi } from '@/services/api/jobs';

export const useJobsStore = defineStore('jobs', () => {
  const jobs = ref<Job[]>([]);
  const scheduledJobs = ref<ScheduledJob[]>([]);
  const currentJob = ref<Job | null>(null);
  const jobLogs = ref<JobLog[]>([]);
  const isMonitoring = ref(false);
  const loading = ref(false);
  const error = ref<string | null>(null);

  async function fetchJobs(status?: string) {
    loading.value = true;
    error.value = null;
    try {
      const response = await jobsApi.getJobs(status);
      jobs.value = response.items;
    } catch (err: unknown) {
      const apiError = err as ApiError | JobsApiError;
      error.value = apiError.message || 'Failed to fetch jobs';
    } finally {
      loading.value = false;
    }
  }

  async function fetchJob(jobId: string) {
    loading.value = true;
    error.value = null;
    try {
      currentJob.value = await jobsApi.getJob(jobId);
      return currentJob.value;
    } catch (err: unknown) {
      const apiError = err as ApiError | JobsApiError;
      error.value = apiError.message || 'Failed to fetch job';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  async function fetchJobLogs(jobId: string) {
    loading.value = true;
    error.value = null;
    try {
      jobLogs.value = await jobsApi.getJobLogs(jobId);
    } catch (err: unknown) {
      const apiError = err as ApiError | JobsApiError;
      error.value = apiError.message || 'Failed to fetch job logs';
    } finally {
      loading.value = false;
    }
  }

  async function retryJob(jobId: string) {
    loading.value = true;
    error.value = null;
    try {
      const updated = await jobsApi.retryJob(jobId);
      const index = jobs.value.findIndex((j: Job) => j.id === jobId);
      if (index !== -1) jobs.value[index] = updated;
      if (currentJob.value?.id === jobId) currentJob.value = updated;
      return updated;
    } catch (err: unknown) {
      const apiError = err as ApiError | JobsApiError;
      error.value = apiError.message || 'Failed to retry job';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  async function cancelJob(jobId: string) {
    loading.value = true;
    error.value = null;
    try {
      const updated = await jobsApi.cancelJob(jobId);
      const index = jobs.value.findIndex((j: Job) => j.id === jobId);
      if (index !== -1) jobs.value[index] = updated;
      if (currentJob.value?.id === jobId) currentJob.value = updated;
      return updated;
    } catch (err: unknown) {
      const apiError = err as ApiError | JobsApiError;
      error.value = apiError.message || 'Failed to cancel job';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  async function fetchScheduledJobs() {
    loading.value = true;
    error.value = null;
    try {
      const response = await jobsApi.getScheduledJobs();
      scheduledJobs.value = response.items;
    } catch (err: unknown) {
      const apiError = err as ApiError | JobsApiError;
      error.value = apiError.message || 'Failed to fetch scheduled jobs';
    } finally {
      loading.value = false;
    }
  }

  async function createScheduledJob(data: Omit<ScheduledJob, 'id' | 'createdAt' | 'updatedAt'>) {
    loading.value = true;
    error.value = null;
    try {
      const created = await jobsApi.createScheduledJob(data);
      scheduledJobs.value.push(created);
      return created;
    } catch (err: unknown) {
      const apiError = err as ApiError | JobsApiError;
      error.value = apiError.message || 'Failed to create scheduled job';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  async function updateScheduledJob(id: string, data: Partial<ScheduledJob>) {
    loading.value = true;
    error.value = null;
    try {
      const updated = await jobsApi.updateScheduledJob(id, data);
      const index = scheduledJobs.value.findIndex((j: ScheduledJob) => j.id === id);
      if (index !== -1) scheduledJobs.value[index] = updated;
      return updated;
    } catch (err: unknown) {
      const apiError = err as ApiError | JobsApiError;
      error.value = apiError.message || 'Failed to update scheduled job';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  async function deleteScheduledJob(id: string) {
    loading.value = true;
    error.value = null;
    try {
      await jobsApi.deleteScheduledJob(id);
      scheduledJobs.value = scheduledJobs.value.filter((j: ScheduledJob) => j.id !== id);
    } catch (err: unknown) {
      const apiError = err as ApiError | JobsApiError;
      error.value = apiError.message || 'Failed to delete scheduled job';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  function startMonitoring() {
    isMonitoring.value = true;
    // WebSocket connection would be initiated here
  }

  function stopMonitoring() {
    isMonitoring.value = false;
    // WebSocket connection would be closed here
  }

  return {
    jobs,
    scheduledJobs,
    currentJob,
    jobLogs,
    isMonitoring,
    loading,
    error,
    fetchJobs,
    fetchJob,
    fetchJobLogs,
    retryJob,
    cancelJob,
    fetchScheduledJobs,
    createScheduledJob,
    updateScheduledJob,
    deleteScheduledJob,
    startMonitoring,
    stopMonitoring,
  };
});
