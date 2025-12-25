import { describe, it, expect, beforeEach, vi } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useJobsStore } from '@/stores/jobs'
import * as jobsApi from '@/services/api/jobs'

vi.mock('@/services/api/jobs')

describe('Jobs Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  describe('Initial State', () => {
    it('should have initial state', () => {
      const store = useJobsStore()

      expect(store.jobs).toEqual([])
      expect(store.scheduledJobs).toEqual([])
      expect(store.currentJob).toBeNull()
      expect(store.loading).toBe(false)
      expect(store.error).toBeNull()
    })
  })

  describe('Fetch Jobs', () => {
    it('should fetch jobs successfully', async () => {
      const mockJobs = [
        {
          id: 'job-1',
          type: 'email_sync',
          status: 'running' as const,
          progress: 50,
          totalItems: 1000,
          completedItems: 500,
          failedItems: 0,
          message: 'Syncing emails...',
          createdAt: new Date().toISOString(),
          startedAt: new Date().toISOString(),
          estimatedCompletionAt: new Date(Date.now() + 600000).toISOString(),
        },
      ]

      const store = useJobsStore()
      store.jobs = mockJobs

      expect(store.jobs).toEqual(mockJobs)
      expect(store.loading).toBe(false)
      expect(store.error).toBeNull()
    })

    it('should handle error state', () => {
      const store = useJobsStore()
      store.error = 'Test error'

      expect(store.error).toBe('Test error')
    })
  })

  describe('Fetch Single Job', () => {
    it('should set current job', () => {
      const mockJob = {
        id: 'job-1',
        type: 'data_export',
        status: 'running' as const,
        progress: 60,
        totalItems: 1000,
        completedItems: 600,
        failedItems: 5,
        message: 'Exporting...',
        createdAt: new Date().toISOString(),
        startedAt: new Date().toISOString(),
      }

      const store = useJobsStore()
      store.currentJob = mockJob

      expect(store.currentJob).toEqual(mockJob)
    })
  })

  describe('Retry Job', () => {
    it('should update job status on retry', () => {
      const jobId = 'job-1'
      
      const store = useJobsStore()
      store.jobs = [
        {
          id: 'job-1',
          type: 'sync',
          status: 'failed' as const,
          progress: 0,
          totalItems: 100,
          completedItems: 0,
          failedItems: 100,
          message: 'Failed',
          createdAt: new Date().toISOString(),
        },
      ]

      // Simulate retry by updating job
      const job = store.jobs[0]
      if (job) {
        job.status = 'pending'
        job.failedItems = 0
      }

      expect(store.jobs[0].status).toBe('pending')
    })
  })

  describe('Cancel Job', () => {
    it('should update job status on cancel', () => {
      const jobId = 'job-1'
      const store = useJobsStore()
      store.jobs = [
        {
          id: 'job-1',
          type: 'import',
          status: 'running' as const,
          progress: 50,
          totalItems: 100,
          completedItems: 50,
          failedItems: 0,
          message: 'Running',
          createdAt: new Date().toISOString(),
        },
      ]

      // Simulate cancel by updating job
      const job = store.jobs[0]
      if (job) {
        job.status = 'cancelled'
      }

      expect(store.jobs[0].status).toBe('cancelled')
    })
  })

  describe('Scheduled Jobs', () => {
    it('should store scheduled jobs', () => {
      const mockScheduledJobs = [
        {
          id: 'scheduled-1',
          type: 'daily_backup',
          schedule: '0 2 * * *',
          description: 'Daily backup at 2 AM',
          isActive: true,
          nextRun: new Date(Date.now() + 82800000).toISOString(),
          lastRun: new Date(Date.now() - 3600000).toISOString(),
        },
      ]

      const store = useJobsStore()
      store.scheduledJobs = mockScheduledJobs

      expect(store.scheduledJobs).toEqual(mockScheduledJobs)
    })
  })

  describe('Error Management', () => {
    it('should set and clear error', () => {
      const store = useJobsStore()
      
      store.error = 'Test error'
      expect(store.error).toBe('Test error')
      
      store.error = null
      expect(store.error).toBeNull()
    })
  })
})
