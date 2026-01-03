<script setup lang="ts">
/**
 * AddressBookWidget - Customer address management
 */
import { computed, ref } from 'vue';
import type { AddressBookWidgetConfig } from '@/types/widgets';

const props = defineProps<{
  config: AddressBookWidgetConfig;
  isEditing?: boolean;
}>();

const emit = defineEmits<{
  (e: 'update:config', config: AddressBookWidgetConfig): void;
}>();

// Mock address data
const mockAddresses = [
  {
    id: '1',
    type: 'shipping',
    isDefault: true,
    name: 'Max Mustermann',
    company: 'Musterfirma GmbH',
    street: 'Musterstraße 123',
    zip: '12345',
    city: 'Musterstadt',
    country: 'Deutschland',
  },
  {
    id: '2',
    type: 'billing',
    isDefault: true,
    name: 'Max Mustermann',
    company: 'Musterfirma GmbH',
    street: 'Rechnungsweg 45',
    zip: '12345',
    city: 'Musterstadt',
    country: 'Deutschland',
  },
  {
    id: '3',
    type: 'shipping',
    isDefault: false,
    name: 'Max Mustermann',
    company: '',
    street: 'Alternativstraße 7',
    zip: '54321',
    city: 'Nebenstadt',
    country: 'Deutschland',
  },
];

const layout = computed(() => props.config.layout ?? 'grid');

function getTypeLabel(type: string): string {
  return type === 'billing' ? 'Rechnungsadresse' : 'Lieferadresse';
}

function getTypeIcon(type: string): string {
  return type === 'billing'
    ? 'M9 14l6-6m-5.5.5h.01m4.99 5h.01M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16l3.5-2 3.5 2 3.5-2 3.5 2zM10 8.5a.5.5 0 11-1 0 .5.5 0 011 0zm5 5a.5.5 0 11-1 0 .5.5 0 011 0z'
    : 'M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z M15 11a3 3 0 11-6 0 3 3 0 016 0z';
}
</script>

<template>
  <div :class="['address-book', { 'address-book--editing': isEditing }]">
    <div class="address-book__header">
      <h2 class="address-book__title">Meine Adressen</h2>
      <button v-if="config.allowAddNew" class="address-book__add-btn" :disabled="isEditing">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M12 4v16m8-8H4" />
        </svg>
        Neue Adresse
      </button>
    </div>

    <div :class="['address-book__list', `address-book__list--${layout}`]">
      <div
        v-for="address in mockAddresses"
        :key="address.id"
        class="address-book__card"
      >
        <div class="address-book__card-header">
          <div class="address-book__type">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
              <path :d="getTypeIcon(address.type)" />
            </svg>
            <span>{{ getTypeLabel(address.type) }}</span>
          </div>
          <span v-if="address.isDefault && config.showDefaultBadge" class="address-book__default-badge">
            Standard
          </span>
        </div>

        <div class="address-book__card-body">
          <p class="address-book__name">{{ address.name }}</p>
          <p v-if="address.company" class="address-book__company">{{ address.company }}</p>
          <p class="address-book__street">{{ address.street }}</p>
          <p class="address-book__city">{{ address.zip }} {{ address.city }}</p>
          <p class="address-book__country">{{ address.country }}</p>
        </div>

        <div class="address-book__card-actions">
          <button v-if="config.allowEdit" class="address-book__action-btn" :disabled="isEditing">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
            </svg>
            Bearbeiten
          </button>
          <button v-if="config.allowDelete && !address.isDefault" class="address-book__action-btn address-book__action-btn--danger" :disabled="isEditing">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
            </svg>
            Löschen
          </button>
        </div>
      </div>

      <!-- Add New Card -->
      <div v-if="config.allowAddNew && mockAddresses.length < (config.maxAddresses ?? 10)" class="address-book__card address-book__card--add">
        <button class="address-book__add-card-btn" :disabled="isEditing">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
            <path d="M12 4v16m8-8H4" />
          </svg>
          <span>Neue Adresse hinzufügen</span>
        </button>
      </div>
    </div>

    <!-- Edit Mode Indicator -->
    <div v-if="isEditing" class="address-book__edit-hint">
      <span>Adressbuch Widget - Max. {{ config.maxAddresses ?? 10 }} Adressen</span>
    </div>
  </div>
</template>

<style scoped>
.address-book {
  padding: 1.5rem;
}

.address-book--editing {
  border: 2px dashed #d1d5db;
  border-radius: 8px;
  background-color: #fafafa;
}

.address-book__header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.address-book__title {
  font-size: 1.25rem;
  font-weight: 600;
  color: #111827;
  margin: 0;
}

.address-book__add-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 1rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: white;
  background-color: var(--color-primary, #3b82f6);
  border: none;
  border-radius: 6px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.address-book__add-btn:hover:not(:disabled) {
  background-color: #2563eb;
}

.address-book__add-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.address-book__add-btn svg {
  width: 18px;
  height: 18px;
}

.address-book__list--grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1rem;
}

.address-book__list--list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.address-book__card {
  background-color: white;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 1.25rem;
  display: flex;
  flex-direction: column;
}

.address-book__card--add {
  border-style: dashed;
  background-color: #f9fafb;
  min-height: 200px;
  justify-content: center;
  align-items: center;
}

.address-book__card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
  padding-bottom: 0.75rem;
  border-bottom: 1px solid #e5e7eb;
}

.address-book__type {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.025em;
  color: #6b7280;
}

.address-book__type svg {
  width: 16px;
  height: 16px;
}

.address-book__default-badge {
  padding: 0.25rem 0.5rem;
  font-size: 0.625rem;
  font-weight: 600;
  text-transform: uppercase;
  background-color: #dcfce7;
  color: #166534;
  border-radius: 9999px;
}

.address-book__card-body {
  flex: 1;
  margin-bottom: 1rem;
}

.address-book__card-body p {
  margin: 0;
  font-size: 0.875rem;
  color: #374151;
  line-height: 1.5;
}

.address-book__name {
  font-weight: 600;
  color: #111827 !important;
}

.address-book__company {
  color: #6b7280 !important;
}

.address-book__card-actions {
  display: flex;
  gap: 0.5rem;
  padding-top: 0.75rem;
  border-top: 1px solid #e5e7eb;
}

.address-book__action-btn {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.5rem 0.75rem;
  font-size: 0.75rem;
  font-weight: 500;
  color: #374151;
  background-color: white;
  border: 1px solid #d1d5db;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
}

.address-book__action-btn:hover:not(:disabled) {
  background-color: #f3f4f6;
}

.address-book__action-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.address-book__action-btn svg {
  width: 14px;
  height: 14px;
}

.address-book__action-btn--danger {
  color: #dc2626;
  border-color: #fca5a5;
}

.address-book__action-btn--danger:hover:not(:disabled) {
  background-color: #fef2f2;
}

.address-book__add-card-btn {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.75rem;
  padding: 2rem;
  background: none;
  border: none;
  color: #6b7280;
  cursor: pointer;
  transition: color 0.2s;
}

.address-book__add-card-btn:hover:not(:disabled) {
  color: var(--color-primary, #3b82f6);
}

.address-book__add-card-btn:disabled {
  cursor: not-allowed;
}

.address-book__add-card-btn svg {
  width: 32px;
  height: 32px;
}

.address-book__add-card-btn span {
  font-size: 0.875rem;
  font-weight: 500;
}

.address-book__edit-hint {
  margin-top: 1rem;
  padding: 0.5rem;
  background-color: #fef3c7;
  border-radius: 4px;
  text-align: center;
  font-size: 0.75rem;
  color: #92400e;
}
</style>
