<script setup lang="ts">
/**
 * AccountDashboardWidget - Customer account overview
 */
import { computed } from 'vue';
import type { AccountDashboardWidgetConfig } from '@/types/widgets';

const props = defineProps<{
  config: AccountDashboardWidgetConfig;
  isEditing?: boolean;
}>();

const quickLinks = computed(() => props.config.quickLinks ?? []);

// Mock data for preview
const mockUser = {
  name: 'Max Mustermann',
  email: 'max@example.com',
};

const mockRecentOrders = [
  { id: 'ORD-2024-001', date: '02.01.2026', status: 'Versendet', total: '234,50 €' },
  { id: 'ORD-2024-002', date: '28.12.2025', status: 'Geliefert', total: '89,00 €' },
  { id: 'ORD-2024-003', date: '15.12.2025', status: 'Geliefert', total: '156,75 €' },
];

const displayedOrders = computed(() => {
  const count = props.config.recentOrdersCount ?? 3;
  return mockRecentOrders.slice(0, count);
});

const iconPaths: Record<string, string> = {
  package:
    'M16.5 9.4l-9-5.19M21 16V8a2 2 0 00-1-1.73l-7-4a2 2 0 00-2 0l-7 4A2 2 0 003 8v8a2 2 0 001 1.73l7 4a2 2 0 002 0l7-4A2 2 0 0021 16z M3.27 6.96L12 12.01l8.73-5.05M12 22.08V12',
  'map-pin': 'M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0118 0z M12 13a3 3 0 100-6 3 3 0 000 6z',
  user: 'M20 21v-2a4 4 0 00-4-4H8a4 4 0 00-4 4v2 M12 11a4 4 0 100-8 4 4 0 000 8z',
  heart:
    'M20.84 4.61a5.5 5.5 0 00-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 00-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 000-7.78z',
};

function getIconPath(icon?: string): string {
  return iconPaths[icon ?? ''] ?? iconPaths['user'];
}
</script>

<template>
  <div :class="['account-dashboard', { 'account-dashboard--editing': isEditing }]">
    <!-- Welcome Message -->
    <div v-if="config.showWelcomeMessage" class="account-dashboard__welcome">
      <h1 class="account-dashboard__title">Willkommen zurück, {{ mockUser.name }}!</h1>
      <p class="account-dashboard__subtitle">
        {{ $t('pageBuilder.accountDashboard.welcome.subtitle') }}
      </p>
    </div>

    <div
      :class="[
        'account-dashboard__content',
        `account-dashboard__content--${config.layout ?? 'grid'}`,
      ]"
    >
      <!-- Quick Links -->
      <div v-if="config.showQuickLinks" class="account-dashboard__section">
        <h2 class="account-dashboard__section-title">
          {{ $t('pageBuilder.accountDashboard.quickLinks.title') }}
        </h2>
        <div class="account-dashboard__quick-links">
          <a
            v-for="link in quickLinks"
            :key="link.href"
            :href="isEditing ? '#' : link.href"
            class="account-dashboard__quick-link"
            @click.prevent="isEditing && $event.preventDefault()"
          >
            <div class="account-dashboard__quick-link-icon">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                <path :d="getIconPath(link.icon)" />
              </svg>
            </div>
            <span class="account-dashboard__quick-link-label">{{ link.label }}</span>
          </a>
        </div>
      </div>

      <!-- Recent Orders -->
      <div v-if="config.showRecentOrders" class="account-dashboard__section">
        <div class="account-dashboard__section-header">
          <h2 class="account-dashboard__section-title">
            {{ $t('pageBuilder.accountDashboard.recentOrders.title') }}
          </h2>
          <a href="#" class="account-dashboard__view-all">{{
            $t('pageBuilder.accountDashboard.recentOrders.viewAll')
          }}</a>
        </div>
        <div class="account-dashboard__orders">
          <div v-for="order in displayedOrders" :key="order.id" class="account-dashboard__order">
            <div class="account-dashboard__order-main">
              <span class="account-dashboard__order-id">{{ order.id }}</span>
              <span class="account-dashboard__order-date">{{ order.date }}</span>
            </div>
            <div class="account-dashboard__order-details">
              <span
                :class="[
                  'account-dashboard__order-status',
                  `account-dashboard__order-status--${order.status.toLowerCase()}`,
                ]"
              >
                {{ order.status }}
              </span>
              <span class="account-dashboard__order-total">{{ order.total }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Edit Mode Indicator -->
    <div v-if="isEditing" class="account-dashboard__edit-hint">
      <span>{{ $t('pageBuilder.accountDashboard.editHint') }}</span>
    </div>
  </div>
</template>

<style scoped>
.account-dashboard {
  padding: 1.5rem;
}

.account-dashboard--editing {
  border: 2px dashed #d1d5db;
  border-radius: 8px;
  background-color: #fafafa;
}

.account-dashboard__welcome {
  margin-bottom: 2rem;
}

.account-dashboard__title {
  font-size: 1.5rem;
  font-weight: 600;
  color: #111827;
  margin: 0 0 0.5rem;
}

.account-dashboard__subtitle {
  font-size: 0.875rem;
  color: #6b7280;
  margin: 0;
}

.account-dashboard__content--grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 1.5rem;
}

.account-dashboard__content--list {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.account-dashboard__section {
  background-color: white;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 1.25rem;
}

.account-dashboard__section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.account-dashboard__section-title {
  font-size: 1rem;
  font-weight: 600;
  color: #111827;
  margin: 0 0 1rem;
}

.account-dashboard__section-header .account-dashboard__section-title {
  margin: 0;
}

.account-dashboard__view-all {
  font-size: 0.875rem;
  color: var(--color-primary, #3b82f6);
  text-decoration: none;
}

.account-dashboard__view-all:hover {
  text-decoration: underline;
}

.account-dashboard__quick-links {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 0.75rem;
}

.account-dashboard__quick-link {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.875rem;
  background-color: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  text-decoration: none;
  color: #374151;
  transition: all 0.2s;
}

.account-dashboard__quick-link:hover {
  background-color: #f3f4f6;
  border-color: var(--color-primary, #3b82f6);
}

.account-dashboard__quick-link-icon {
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: white;
  border-radius: 6px;
}

.account-dashboard__quick-link-icon svg {
  width: 20px;
  height: 20px;
  color: var(--color-primary, #3b82f6);
}

.account-dashboard__quick-link-label {
  font-size: 0.875rem;
  font-weight: 500;
}

.account-dashboard__orders {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.account-dashboard__order {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.875rem;
  background-color: #f9fafb;
  border-radius: 6px;
}

.account-dashboard__order-main {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.account-dashboard__order-id {
  font-size: 0.875rem;
  font-weight: 500;
  color: #111827;
}

.account-dashboard__order-date {
  font-size: 0.75rem;
  color: #6b7280;
}

.account-dashboard__order-details {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 0.25rem;
}

.account-dashboard__order-status {
  font-size: 0.75rem;
  font-weight: 500;
  padding: 0.125rem 0.5rem;
  border-radius: 9999px;
}

.account-dashboard__order-status--versendet {
  background-color: #dbeafe;
  color: #1e40af;
}

.account-dashboard__order-status--geliefert {
  background-color: #dcfce7;
  color: #166534;
}

.account-dashboard__order-total {
  font-size: 0.875rem;
  font-weight: 600;
  color: #111827;
}

.account-dashboard__edit-hint {
  margin-top: 1rem;
  padding: 0.5rem;
  background-color: #fef3c7;
  border-radius: 4px;
  text-align: center;
  font-size: 0.75rem;
  color: #92400e;
}
</style>
