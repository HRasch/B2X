/**
 * Jobs Store (Pinia)
 * @todo Refactor error handling from catch(err: any) to typed unknown pattern
 * @see https://typescript-eslint.io/rules/no-explicit-any/
 */
/* eslint-disable @typescript-eslint/no-explicit-any -- Legacy error handling, refactor in KB-STORE-TYPING sprint */

import { defineStore } from 'pinia';
import { ref } from 'vue';
import type { Job, ScheduledJob, JobLog } from '@/types/jobs';
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
    } catch (err: any) {
      error.value = err.message;
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
    } catch (err: any) {
      error.value = err.message;
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
    } catch (err: any) {
      error.value = err.message;
    } finally {
      loading.value = false;
    }
  }

  async function retryJob(jobId: string) {
    loading.value = true;
    error.value = null;
    try {
      const updated = await jobsApi.retryJob(jobId);
      const index = jobs.value.findIndex((j: any) => j.id === jobId);
      if (index !== -1) jobs.value[index] = updated;
      if (currentJob.value?.id === jobId) currentJob.value = updated;
      return updated;
    } catch (err: any) {
      error.value = err.message;
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
      const index = jobs.value.findIndex((j: any) => j.id === jobId);
      if (index !== -1) jobs.value[index] = updated;
      if (currentJob.value?.id === jobId) currentJob.value = updated;
      return updated;
    } catch (err: any) {
      error.value = err.message;
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
    } catch (err: any) {
      error.value = err.message;
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
    } catch (err: any) {
      error.value = err.message;
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
      const index = scheduledJobs.value.findIndex((j: any) => j.id === id);
      if (index !== -1) scheduledJobs.value[index] = updated;
      return updated;
    } catch (err: any) {
      error.value = err.message;
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
      scheduledJobs.value = scheduledJobs.value.filter((j: any) => j.id !== id);
    } catch (err: any) {
      error.value = err.message;
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
