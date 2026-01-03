<script setup lang="ts">
/**
 * OrderHistoryWidget - Customer order history list
 */
import { computed, ref } from 'vue';
import type { OrderHistoryWidgetConfig } from '@/types/widgets';

const props = defineProps<{
  config: OrderHistoryWidgetConfig;
  isEditing?: boolean;
}>();

const emit = defineEmits<{
  (e: 'update:config', config: OrderHistoryWidgetConfig): void;
}>();

const searchQuery = ref('');
const statusFilter = ref('all');

// Mock order data for preview
const mockOrders = [
  { id: 'ORD-2026-001', date: '02.01.2026', status: 'processing', statusLabel: 'In Bearbeitung', total: '234,50 €', items: 3 },
  { id: 'ORD-2025-089', date: '28.12.2025', status: 'shipped', statusLabel: 'Versendet', total: '89,00 €', items: 1 },
  { id: 'ORD-2025-088', date: '15.12.2025', status: 'delivered', statusLabel: 'Geliefert', total: '156,75 €', items: 4 },
  { id: 'ORD-2025-087', date: '10.12.2025', status: 'delivered', statusLabel: 'Geliefert', total: '423,00 €', items: 2 },
  { id: 'ORD-2025-086', date: '01.12.2025', status: 'delivered', statusLabel: 'Geliefert', total: '67,50 €', items: 1 },
];

const columns = computed(() => props.config.columns ?? ['orderNumber', 'date', 'status', 'total', 'actions']);

const columnLabels: Record<string, string> = {
  orderNumber: 'Bestellnr.',
  date: 'Datum',
  status: 'Status',
  total: 'Summe',
  items: 'Artikel',
  actions: 'Aktionen',
};

const statusOptions = [
  { value: 'all', label: 'Alle Status' },
  { value: 'processing', label: 'In Bearbeitung' },
  { value: 'shipped', label: 'Versendet' },
  { value: 'delivered', label: 'Geliefert' },
  { value: 'cancelled', label: 'Storniert' },
];

function getStatusClass(status: string): string {
  const classes: Record<string, string> = {
    processing: 'order-history__status--processing',
    shipped: 'order-history__status--shipped',
    delivered: 'order-history__status--delivered',
    cancelled: 'order-history__status--cancelled',
  };
  return classes[status] ?? '';
}
</script>

<template>
  <div :class="['order-history', { 'order-history--editing': isEditing }]">
    <div class="order-history__header">
      <h2 class="order-history__title">Meine Bestellungen</h2>
    </div>

    <!-- Filters -->
    <div v-if="config.showFilters || config.showSearch" class="order-history__filters">
      <div v-if="config.showSearch" class="order-history__search">
        <svg class="order-history__search-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
        </svg>
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Bestellung suchen..."
          class="order-history__search-input"
          :disabled="isEditing"
        >
      </div>
      <div v-if="config.showFilters" class="order-history__filter">
        <select v-model="statusFilter" class="order-history__select" :disabled="isEditing">
          <option v-for="opt in statusOptions" :key="opt.value" :value="opt.value">
            {{ opt.label }}
          </option>
        </select>
      </div>
    </div>

    <!-- Orders Table -->
    <div class="order-history__table-wrapper">
      <table class="order-history__table">
        <thead>
          <tr>
            <th v-for="col in columns" :key="col" class="order-history__th">
              {{ columnLabels[col] }}
            </th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="order in mockOrders" :key="order.id" class="order-history__row">
            <td v-if="columns.includes('orderNumber')" class="order-history__td">
              <a href="#" class="order-history__order-link">{{ order.id }}</a>
            </td>
            <td v-if="columns.includes('date')" class="order-history__td">
              {{ order.date }}
            </td>
            <td v-if="columns.includes('status')" class="order-history__td">
              <span :class="['order-history__status', getStatusClass(order.status)]">
                {{ order.statusLabel }}
              </span>
            </td>
            <td v-if="columns.includes('total')" class="order-history__td order-history__td--total">
              {{ order.total }}
            </td>
            <td v-if="columns.includes('items')" class="order-history__td">
              {{ order.items }} Artikel
            </td>
            <td v-if="columns.includes('actions')" class="order-history__td">
              <div class="order-history__actions">
                <button class="order-history__action-btn" title="Details">
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                    <path d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                  </svg>
                </button>
                <button v-if="config.showTracking && order.status === 'shipped'" class="order-history__action-btn" title="Tracking">
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0118 0z" />
                    <path d="M12 13a3 3 0 100-6 3 3 0 000 6z" />
                  </svg>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Pagination -->
    <div class="order-history__pagination">
      <span class="order-history__pagination-info">
        1-{{ mockOrders.length }} von {{ mockOrders.length }} Bestellungen
      </span>
      <div class="order-history__pagination-btns">
        <button class="order-history__pagination-btn" disabled>Zurück</button>
        <button class="order-history__pagination-btn" disabled>Weiter</button>
      </div>
    </div>

    <!-- Edit Mode Indicator -->
    <div v-if="isEditing" class="order-history__edit-hint">
      <span>Bestellhistorie Widget - {{ config.ordersPerPage ?? 10 }} Einträge pro Seite</span>
    </div>
  </div>
</template>

<style scoped>
.order-history {
  padding: 1.5rem;
}

.order-history--editing {
  border: 2px dashed #d1d5db;
  border-radius: 8px;
  background-color: #fafafa;
}

.order-history__header {
  margin-bottom: 1.5rem;
}

.order-history__title {
  font-size: 1.25rem;
  font-weight: 600;
  color: #111827;
  margin: 0;
}

.order-history__filters {
  display: flex;
  gap: 1rem;
  margin-bottom: 1rem;
  flex-wrap: wrap;
}

.order-history__search {
  position: relative;
  flex: 1;
  min-width: 200px;
}

.order-history__search-icon {
  position: absolute;
  left: 0.75rem;
  top: 50%;
  transform: translateY(-50%);
  width: 18px;
  height: 18px;
  color: #9ca3af;
}

.order-history__search-input {
  width: 100%;
  padding: 0.625rem 0.75rem 0.625rem 2.5rem;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 0.875rem;
}

.order-history__search-input:focus {
  outline: none;
  border-color: var(--color-primary, #3b82f6);
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.order-history__select {
  padding: 0.625rem 2rem 0.625rem 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 0.875rem;
  background-color: white;
  cursor: pointer;
}

.order-history__table-wrapper {
  overflow-x: auto;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
}

.order-history__table {
  width: 100%;
  border-collapse: collapse;
}

.order-history__th {
  padding: 0.75rem 1rem;
  text-align: left;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: #6b7280;
  background-color: #f9fafb;
  border-bottom: 1px solid #e5e7eb;
}

.order-history__row {
  border-bottom: 1px solid #e5e7eb;
}

.order-history__row:last-child {
  border-bottom: none;
}

.order-history__row:hover {
  background-color: #f9fafb;
}

.order-history__td {
  padding: 1rem;
  font-size: 0.875rem;
  color: #374151;
}

.order-history__td--total {
  font-weight: 600;
}

.order-history__order-link {
  color: var(--color-primary, #3b82f6);
  text-decoration: none;
  font-weight: 500;
}

.order-history__order-link:hover {
  text-decoration: underline;
}

.order-history__status {
  display: inline-block;
  padding: 0.25rem 0.625rem;
  font-size: 0.75rem;
  font-weight: 500;
  border-radius: 9999px;
}

.order-history__status--processing {
  background-color: #fef3c7;
  color: #92400e;
}

.order-history__status--shipped {
  background-color: #dbeafe;
  color: #1e40af;
}

.order-history__status--delivered {
  background-color: #dcfce7;
  color: #166534;
}

.order-history__status--cancelled {
  background-color: #fee2e2;
  color: #991b1b;
}

.order-history__actions {
  display: flex;
  gap: 0.5rem;
}

.order-history__action-btn {
  padding: 0.375rem;
  background-color: transparent;
  border: 1px solid #e5e7eb;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
}

.order-history__action-btn:hover {
  background-color: #f3f4f6;
  border-color: #d1d5db;
}

.order-history__action-btn svg {
  width: 16px;
  height: 16px;
  color: #6b7280;
}

.order-history__pagination {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 1rem;
  padding-top: 1rem;
  border-top: 1px solid #e5e7eb;
}

.order-history__pagination-info {
  font-size: 0.875rem;
  color: #6b7280;
}

.order-history__pagination-btns {
  display: flex;
  gap: 0.5rem;
}

.order-history__pagination-btn {
  padding: 0.5rem 1rem;
  font-size: 0.875rem;
  background-color: white;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  cursor: pointer;
}

.order-history__pagination-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.order-history__pagination-btn:not(:disabled):hover {
  background-color: #f3f4f6;
}

.order-history__edit-hint {
  margin-top: 1rem;
  padding: 0.5rem;
  background-color: #fef3c7;
  border-radius: 4px;
  text-align: center;
  font-size: 0.75rem;
  color: #92400e;
}
</style>
