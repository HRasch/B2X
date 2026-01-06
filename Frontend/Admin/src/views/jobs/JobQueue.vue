<template>
  <div class="space-y-6">
    <PageHeader :title="$t('jobs.queue.title')" />

    <CardContainer>
      <div v-if="loading" class="text-center py-8">
        <span class="loading loading-spinner loading-lg"></span>
        <p class="text-base-content/60 mt-2">{{ $t('jobs.queue.loading') }}</p>
      </div>

      <div v-else-if="jobsStore.jobs.length === 0" class="card bg-base-100 shadow">
        <div class="card-body text-center">
          <p class="text-base-content/60">{{ $t('jobs.queue.no_jobs') }}</p>
        </div>
      </div>

      <div v-else class="card bg-base-100 shadow">
        <div class="card-body p-0">
          <div class="overflow-x-auto">
            <table class="table">
              <thead>
                <tr>
                  <th>Job Name</th>
                  <th>Status</th>
                  <th>Progress</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="job in jobsStore.jobs" :key="job.id" class="hover">
                  <td class="font-medium">{{ job.name }}</td>
                  <td>
                    <div :class="['badge', statusBadgeClass(job.status)]">
                      {{ job.status }}
                    </div>
                  </td>
                  <td>
                    <div class="flex items-center gap-2">
                      <progress
                        class="progress progress-primary w-24"
                        :value="job.progress"
                        max="100"
                      ></progress>
                      <span class="text-sm text-base-content/60">{{ job.progress }}%</span>
                    </div>
                  </td>
                  <td>
                    <div class="flex gap-2">
                      <router-link :to="`/jobs/${job.id}`" class="btn btn-ghost btn-xs">
                        View
                      </router-link>
                      <button
                        v-if="job.status === 'failed'"
                        @click="retryJob(job.id)"
                        class="btn btn-success btn-xs"
                      >
                        Retry
                      </button>
                      <button
                        v-if="job.status === 'running'"
                        @click="cancelJob(job.id)"
                        class="btn btn-error btn-xs"
                      >
                        Cancel
                      </button>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </CardContainer>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';
import { useJobsStore } from '@/stores/jobs';

const jobsStore = useJobsStore();
const loading = ref(true);

let pollInterval: number | null = null;

const statusBadgeClass = (status: string): string => {
  const classes: Record<string, string> = {
    running: 'badge-info',
    completed: 'badge-success',
    failed: 'badge-error',
    pending: 'badge-warning',
  };
  return classes[status] || 'badge-ghost';
};

const loadJobs = async () => {
  try {
    await jobsStore.fetchJobs();
  } finally {
    loading.value = false;
  }
};

const retryJob = async (jobId: string) => {
  await jobsStore.retryJob(jobId);
};

const cancelJob = async (jobId: string) => {
  await jobsStore.cancelJob(jobId);
};

onMounted(() => {
  loadJobs();
  pollInterval = window.setInterval(loadJobs, 5000);
});

onUnmounted(() => {
  if (pollInterval) {
    clearInterval(pollInterval);
  }
});
</script>
