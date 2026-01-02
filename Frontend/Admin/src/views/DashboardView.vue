<template>
  <div class="space-y-safe">
    <!-- Header -->
    <div>
      <h1 class="heading-lg">Dashboard</h1>
      <p class="text-muted dark:text-soft-400 mt-1">
        Welcome back, {{ authStore.user?.email }}
      </p>
    </div>

    <!-- Stats Grid -->
    <div class="grid-soft-cols-4">
      <soft-card v-for="stat in stats" :key="stat.id" variant="gradient">
        <div class="flex items-start justify-between">
          <div>
            <p class="text-soft-600 text-sm font-medium">{{ stat.label }}</p>
            <p class="text-3xl font-bold text-soft-900 mt-2">
              {{ stat.value }}
            </p>
            <p class="text-sm text-soft-500 mt-1">{{ stat.change }}</p>
          </div>
          <div
            :class="[
              'w-12 h-12 rounded-soft-lg flex items-center justify-center text-white',
              stat.gradientClass,
            ]"
          >
            <component :is="stat.icon" class="w-6 h-6" />
          </div>
        </div>
      </soft-card>
    </div>

    <!-- Content Grid -->
    <div class="grid-soft-cols-2">
      <!-- Chart Card -->
      <soft-panel title="Sales Overview" description="Last 7 days">
        <div
          class="h-48 bg-linear-to-br from-soft-100 to-soft-50 rounded-soft flex items-center justify-center"
        >
          <p class="text-soft-500">Chart Placeholder</p>
        </div>
      </soft-panel>

      <!-- Recent Activity -->
      <soft-panel title="Recent Activity" description="Latest updates">
        <div class="space-y-3">
          <div
            v-for="activity in recentActivity"
            :key="activity.id"
            class="flex gap-3 pb-3 border-b border-soft-100 last:border-0 last:pb-0"
          >
            <div
              :class="[
                'w-10 h-10 rounded-soft flex items-center justify-center shrink-0 text-white text-sm font-semibold',
                activity.colorClass,
              ]"
            >
              {{ activity.initials }}
            </div>
            <div class="flex-1 min-w-0">
              <p class="text-sm font-medium text-soft-900">
                {{ activity.user }}
              </p>
              <p class="text-xs text-soft-500 truncate">
                {{ activity.action }}
              </p>
              <p class="text-xs text-soft-400 mt-1">{{ activity.time }}</p>
            </div>
            <soft-badge :variant="activity.status">{{
              activity.statusLabel
            }}</soft-badge>
          </div>
        </div>
      </soft-panel>

      <!-- Users Table -->
      <soft-panel
        title="Recent Users"
        description="New registrations"
        class="col-span-1 md:col-span-2"
      >
        <div class="overflow-x-auto">
          <table class="w-full text-sm">
            <thead>
              <tr class="border-b border-soft-100">
                <th class="text-left py-3 px-4 font-semibold text-soft-900">
                  Name
                </th>
                <th class="text-left py-3 px-4 font-semibold text-soft-900">
                  Email
                </th>
                <th class="text-left py-3 px-4 font-semibold text-soft-900">
                  Status
                </th>
                <th class="text-left py-3 px-4 font-semibold text-soft-900">
                  Joined
                </th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="user in users"
                :key="user.id"
                class="border-b border-soft-100 hover:bg-soft-50 transition-colors"
              >
                <td class="py-3 px-4">
                  <div class="flex items-center gap-2">
                    <div
                      class="w-8 h-8 rounded-soft-lg bg-gradient-soft-cyan flex items-center justify-center text-white text-xs font-semibold"
                    >
                      {{ user.initials }}
                    </div>
                    <span class="text-soft-900 font-medium">{{
                      user.name
                    }}</span>
                  </div>
                </td>
                <td class="py-3 px-4 text-soft-600">{{ user.email }}</td>
                <td class="py-3 px-4">
                  <soft-badge :variant="user.status">{{
                    user.statusLabel
                  }}</soft-badge>
                </td>
                <td class="py-3 px-4 text-soft-600">{{ user.joined }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </soft-panel>
    </div>

    <!-- Action Buttons -->
    <div class="flex gap-3 pt-safe">
      <soft-button variant="primary" size="md">Create New</soft-button>
      <soft-button variant="secondary" size="md">View All</soft-button>
      <soft-button variant="ghost" size="md">More Options</soft-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from "@/stores/auth";
import SoftCard from "@/components/soft-ui/SoftCard.vue";
import SoftPanel from "@/components/soft-ui/SoftPanel.vue";
import SoftBadge from "@/components/soft-ui/SoftBadge.vue";
import SoftButton from "@/components/soft-ui/SoftButton.vue";

type BadgeVariant = "success" | "warning" | "danger" | "info" | "default";

const authStore = useAuthStore();

const stats = [
  {
    id: 1,
    label: "Total Revenue",
    value: "$24,500",
    change: "+12% from last month",
    icon: "div",
    gradientClass: "bg-gradient-soft-blue",
  },
  {
    id: 2,
    label: "Total Users",
    value: "1,234",
    change: "+5% new users",
    icon: "div",
    gradientClass: "bg-gradient-soft-cyan",
  },
  {
    id: 3,
    label: "Conversion Rate",
    value: "3.25%",
    change: "+0.5% improvement",
    icon: "div",
    gradientClass: "bg-gradient-soft-green",
  },
  {
    id: 4,
    label: "Active Orders",
    value: "456",
    change: "+8 today",
    icon: "div",
    gradientClass: "bg-gradient-soft-purple",
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
  colorClass: string;
}> = [
  {
    id: 1,
    initials: "JD",
    user: "John Doe",
    action: "Updated product catalog",
    time: "2 hours ago",
    status: "success",
    statusLabel: "Completed",
    colorClass: "bg-gradient-soft-cyan",
  },
  {
    id: 2,
    initials: "SA",
    user: "Sarah Anderson",
    action: "Created new promotion",
    time: "4 hours ago",
    status: "info",
    statusLabel: "Active",
    colorClass: "bg-gradient-soft-blue",
  },
  {
    id: 3,
    initials: "MB",
    user: "Michael Brown",
    action: "Generated sales report",
    time: "1 day ago",
    status: "success",
    statusLabel: "Done",
    colorClass: "bg-gradient-soft-green",
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
    name: "Alice Johnson",
    email: "alice@example.com",
    status: "success",
    statusLabel: "Active",
    joined: "Dec 20, 2024",
    initials: "AJ",
  },
  {
    id: 2,
    name: "Bob Smith",
    email: "bob@example.com",
    status: "success",
    statusLabel: "Active",
    joined: "Dec 19, 2024",
    initials: "BS",
  },
  {
    id: 3,
    name: "Carol Davis",
    email: "carol@example.com",
    status: "info",
    statusLabel: "Pending",
    joined: "Dec 18, 2024",
    initials: "CD",
  },
];
</script>
