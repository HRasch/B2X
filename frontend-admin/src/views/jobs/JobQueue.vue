<template>
  <div class="space-y-6">
    <h1 class="text-3xl font-bold text-gray-900 dark:text-white">Job Queue</h1>

    <div v-if="loading" class="text-center py-8">
      <div class="text-gray-500 dark:text-soft-300">Loading jobs...</div>
    </div>

    <div
      v-else-if="jobsStore.jobs.length === 0"
      class="bg-white dark:bg-soft-800 rounded-lg shadow p-8 text-center"
    >
      <p class="text-gray-500 dark:text-soft-300">No jobs in queue.</p>
    </div>

    <div
      v-else
      class="bg-white dark:bg-soft-800 rounded-lg shadow overflow-hidden"
    >
      <table class="w-full">
        <thead
          class="bg-gray-100 dark:bg-soft-700 border-b dark:border-soft-600"
        >
          <tr>
            <th
              class="px-6 py-3 text-left text-sm font-medium text-gray-900 dark:text-soft-100"
            >
              Job Name
            </th>
            <th
              class="px-6 py-3 text-left text-sm font-medium text-gray-900 dark:text-soft-100"
            >
              Status
            </th>
            <th
              class="px-6 py-3 text-left text-sm font-medium text-gray-900 dark:text-soft-100"
            >
              Progress
            </th>
            <th
              class="px-6 py-3 text-left text-sm font-medium text-gray-900 dark:text-soft-100"
            >
              Actions
            </th>
          </tr>
        </thead>
        <tbody class="divide-y dark:divide-soft-600">
          <tr
            v-for="job in jobsStore.jobs"
            :key="job.id"
            class="hover:bg-gray-50 dark:hover:bg-soft-700"
          >
            <td class="px-6 py-4 text-sm text-gray-900 dark:text-soft-100">
              {{ job.name }}
            </td>
            <td class="px-6 py-4 text-sm">
              <span
                :class="[
                  'px-3 py-1 rounded-full text-xs font-medium',
                  {
                    'bg-blue-100 text-blue-800': job.status === 'running',
                    'bg-green-100 text-green-800': job.status === 'completed',
                    'bg-red-100 text-red-800': job.status === 'failed',
                    'bg-yellow-100 text-yellow-800': job.status === 'pending',
                  },
                ]"
              >
                {{ job.status }}
              </span>
            </td>
            <td class="px-6 py-4 text-sm">
              <div class="w-32 bg-gray-200 rounded-full h-2">
                <div
                  class="bg-blue-600 h-2 rounded-full"
                  :style="{ width: job.progress + '%' }"
                />
              </div>
              <span class="text-xs text-gray-600">{{ job.progress }}%</span>
            </td>
            <td class="px-6 py-4 text-sm space-x-2">
              <router-link
                :to="`/jobs/${job.id}`"
                class="text-blue-600 hover:text-blue-800"
              >
                View
              </router-link>
              <button
                v-if="job.status === 'failed'"
                @click="retryJob(job.id)"
                class="text-green-600 hover:text-green-800"
              >
                Retry
              </button>
              <button
                v-if="job.status === 'running'"
                @click="cancelJob(job.id)"
                class="text-red-600 hover:text-red-800"
              >
                Cancel
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from "vue";
import { useJobsStore } from "@/stores/jobs";

const jobsStore = useJobsStore();
const loading = jobsStore.loading;

onMounted(() => {
  jobsStore.fetchJobs();
});

const retryJob = async (jobId: string) => {
  try {
    await jobsStore.retryJob(jobId);
  } catch (error) {
    console.error("Failed to retry job:", error);
  }
};

const cancelJob = async (jobId: string) => {
  try {
    await jobsStore.cancelJob(jobId);
  } catch (error) {
    console.error("Failed to cancel job:", error);
  }
};
</script>
