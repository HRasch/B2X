<template>
  <div class="space-y-6">
    <!-- Header -->
    <div>
      <h1 class="text-2xl font-bold text-base-content">Dashboard</h1>
      <p class="text-base-content/60 mt-1">Welcome back, {{ authStore.user?.email }}</p>
    </div>

    <!-- Stats Grid -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <div v-for="stat in stats" :key="stat.id" class="stat bg-base-100 shadow rounded-box">
        <div class="stat-figure text-primary">
          <div :class="['w-12 h-12 rounded-lg flex items-center justify-center', stat.bgClass]">
            <svg
              class="w-6 h-6"
              :class="stat.iconClass"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                :d="stat.iconPath"
              />
            </svg>
          </div>
        </div>
        <div class="stat-title">{{ stat.label }}</div>
        <div class="stat-value text-2xl">{{ stat.value }}</div>
        <div class="stat-desc">{{ stat.change }}</div>
      </div>
    </div>

    <!-- Content Grid -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Chart Card -->
      <div class="card bg-base-100 shadow">
        <div class="card-body">
          <h2 class="card-title">Sales Overview</h2>
          <p class="text-base-content/60 text-sm">Last 7 days</p>
          <div class="h-48 bg-base-200 rounded-lg flex items-center justify-center mt-4">
            <p class="text-base-content/40">Chart Placeholder</p>
          </div>
        </div>
      </div>

      <!-- Recent Activity -->
      <div class="card bg-base-100 shadow">
        <div class="card-body">
          <h2 class="card-title">Recent Activity</h2>
          <p class="text-base-content/60 text-sm">Latest updates</p>
          <div class="space-y-3 mt-4">
            <div
              v-for="activity in recentActivity"
              :key="activity.id"
              class="flex gap-3 pb-3 border-b border-base-200 last:border-0 last:pb-0"
            >
              <div class="avatar placeholder">
                <div :class="['w-10 rounded-lg', activity.bgClass]">
                  <span class="text-sm text-white">{{ activity.initials }}</span>
                </div>
              </div>
              <div class="flex-1 min-w-0">
                <p class="font-medium text-base-content">{{ activity.user }}</p>
                <p class="text-sm text-base-content/60 truncate">{{ activity.action }}</p>
                <p class="text-xs text-base-content/40 mt-1">{{ activity.time }}</p>
              </div>
              <div :class="['badge', badgeClass(activity.status)]">{{ activity.statusLabel }}</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Users Table -->
      <div class="card bg-base-100 shadow lg:col-span-2">
        <div class="card-body">
          <h2 class="card-title">Recent Users</h2>
          <p class="text-base-content/60 text-sm">New registrations</p>
          <div class="overflow-x-auto mt-4">
            <table class="table">
              <thead>
                <tr>
                  <th>Name</th>
                  <th>Email</th>
                  <th>Status</th>
                  <th>Joined</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="user in users" :key="user.id" class="hover">
                  <td>
                    <div class="flex items-center gap-3">
                      <div class="avatar placeholder">
                        <div class="bg-primary text-primary-content w-8 rounded-lg">
                          <span class="text-xs">{{ user.initials }}</span>
                        </div>
                      </div>
                      <span class="font-medium">{{ user.name }}</span>
                    </div>
                  </td>
                  <td class="text-base-content/70">{{ user.email }}</td>
                  <td>
                    <div :class="['badge badge-sm', badgeClass(user.status)]">
                      {{ user.statusLabel }}
                    </div>
                  </td>
                  <td class="text-base-content/70">{{ user.joined }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>

    <!-- Action Buttons -->
    <div class="flex flex-wrap gap-3 pt-4">
      <button class="btn btn-primary">Create New</button>
      <button class="btn btn-outline">View All</button>
      <button class="btn btn-ghost">More Options</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth';

type BadgeVariant = 'success' | 'warning' | 'danger' | 'info' | 'default';

const authStore = useAuthStore();

const badgeClass = (variant: BadgeVariant): string => {
  const classes: Record<BadgeVariant, string> = {
    success: 'badge-success',
    warning: 'badge-warning',
    danger: 'badge-error',
    info: 'badge-info',
    default: 'badge-ghost',
  };
  return classes[variant] || 'badge-ghost';
};

const stats = [
  {
    id: 1,
    label: 'Total Revenue',
    value: '$24,500',
    change: '+12% from last month',
    bgClass: 'bg-primary',
    iconClass: 'text-primary-content',
    iconPath:
      'M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z',
  },
  {
    id: 2,
    label: 'Total Users',
    value: '1,234',
    change: '+5% new users',
    bgClass: 'bg-secondary',
    iconClass: 'text-secondary-content',
    iconPath:
      'M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z',
  },
  {
    id: 3,
    label: 'Conversion Rate',
    value: '3.25%',
    change: '+0.5% improvement',
    bgClass: 'bg-accent',
    iconClass: 'text-accent-content',
    iconPath: 'M13 7h8m0 0v8m0-8l-8 8-4-4-6 6',
  },
  {
    id: 4,
    label: 'Active Orders',
    value: '456',
    change: '+8 today',
    bgClass: 'bg-info',
    iconClass: 'text-info-content',
    iconPath: 'M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z',
  },
];

const recentActivity: Array<{
  id: number;
  initials: string;
  user: string;
  action: string;
  time: string;
  status: BadgeVariant;
  statusLabel: string;
  bgClass: string;
}> = [
  {
    id: 1,
    initials: 'JD',
    user: 'John Doe',
    action: 'Updated product catalog',
    time: '2 hours ago',
    status: 'success',
    statusLabel: 'Completed',
    bgClass: 'bg-primary',
  },
  {
    id: 2,
    initials: 'SA',
    user: 'Sarah Anderson',
    action: 'Created new promotion',
    time: '4 hours ago',
    status: 'info',
    statusLabel: 'Active',
    bgClass: 'bg-secondary',
  },
  {
    id: 3,
    initials: 'MB',
    user: 'Michael Brown',
    action: 'Generated sales report',
    time: '1 day ago',
    status: 'success',
    statusLabel: 'Done',
    bgClass: 'bg-accent',
  },
];

const users: Array<{
  id: number;
  name: string;
  email: string;
  status: BadgeVariant;
  statusLabel: string;
  joined: string;
  initials: string;
}> = [
  {
    id: 1,
    name: 'Alice Johnson',
    email: 'alice@example.com',
    status: 'success',
    statusLabel: 'Active',
    joined: 'Dec 20, 2024',
    initials: 'AJ',
  },
  {
    id: 2,
    name: 'Bob Smith',
    email: 'bob@example.com',
    status: 'success',
    statusLabel: 'Active',
    joined: 'Dec 19, 2024',
    initials: 'BS',
  },
  {
    id: 3,
    name: 'Carol Davis',
    email: 'carol@example.com',
    status: 'info',
    statusLabel: 'Pending',
    joined: 'Dec 18, 2024',
    initials: 'CD',
  },
];
</script>
