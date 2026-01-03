<script setup lang="ts">
/**
 * ProfileFormWidget - Customer profile editing form
 */
import { computed, ref } from 'vue';
import type { ProfileFormWidgetConfig } from '@/types/widgets';

const props = defineProps<{
  config: ProfileFormWidgetConfig;
  isEditing?: boolean;
}>();

const emit = defineEmits<{
  (e: 'update:config', config: ProfileFormWidgetConfig): void;
}>();

// Mock user data
const mockUser = ref({
  name: 'Max Mustermann',
  email: 'max.mustermann@example.com',
  phone: '+49 123 456789',
  company: 'Musterfirma GmbH',
  taxId: 'DE123456789',
  birthday: '1985-06-15',
  avatar: '',
});

const fields = computed(() => props.config.fields ?? ['name', 'email', 'phone', 'company']);
const layout = computed(() => props.config.layout ?? 'two-column');

const fieldLabels: Record<string, string> = {
  name: 'Name',
  email: 'E-Mail',
  phone: 'Telefon',
  company: 'Firma',
  taxId: 'USt-IdNr.',
  birthday: 'Geburtstag',
};

const fieldTypes: Record<string, string> = {
  name: 'text',
  email: 'email',
  phone: 'tel',
  company: 'text',
  taxId: 'text',
  birthday: 'date',
};

function getInitials(name: string): string {
  return name
    .split(' ')
    .map(n => n[0])
    .join('')
    .toUpperCase()
    .substring(0, 2);
}
</script>

<template>
  <div :class="['profile-form', { 'profile-form--editing': isEditing }]">
    <div class="profile-form__header">
      <h2 class="profile-form__title">Mein Profil</h2>
    </div>

    <form class="profile-form__form" @submit.prevent>
      <!-- Avatar Section -->
      <div v-if="config.showAvatar" class="profile-form__avatar-section">
        <div class="profile-form__avatar">
          <img
            v-if="mockUser.avatar"
            :src="mockUser.avatar"
            alt="Profilbild"
            class="profile-form__avatar-img"
          />
          <span v-else class="profile-form__avatar-initials">
            {{ getInitials(mockUser.name) }}
          </span>
        </div>
        <div v-if="config.allowAvatarUpload" class="profile-form__avatar-actions">
          <button type="button" class="profile-form__avatar-btn" :disabled="isEditing">
            Bild ändern
          </button>
          <button
            type="button"
            class="profile-form__avatar-btn profile-form__avatar-btn--secondary"
            :disabled="isEditing"
          >
            Entfernen
          </button>
        </div>
      </div>

      <!-- Form Fields -->
      <div :class="['profile-form__fields', `profile-form__fields--${layout}`]">
        <div v-for="field in fields" :key="field" class="profile-form__field">
          <label :for="`profile-${field}`" class="profile-form__label">
            {{ fieldLabels[field] }}
          </label>
          <input
            :id="`profile-${field}`"
            :type="fieldTypes[field]"
            :value="mockUser[field as keyof typeof mockUser]"
            :disabled="isEditing || (field === 'email' && !config.allowEmailChange)"
            :readonly="field === 'email' && !config.allowEmailChange"
            class="profile-form__input"
            :class="{
              'profile-form__input--readonly': field === 'email' && !config.allowEmailChange,
            }"
          />
          <p v-if="field === 'email' && !config.allowEmailChange" class="profile-form__hint">
            E-Mail-Adresse kann nicht geändert werden
          </p>
        </div>
      </div>

      <!-- Password Change Section -->
      <div v-if="config.allowPasswordChange" class="profile-form__password-section">
        <h3 class="profile-form__section-title">Passwort ändern</h3>
        <div :class="['profile-form__fields', `profile-form__fields--${layout}`]">
          <div class="profile-form__field">
            <label for="current-password" class="profile-form__label">Aktuelles Passwort</label>
            <input
              id="current-password"
              type="password"
              placeholder="••••••••"
              class="profile-form__input"
              :disabled="isEditing"
            />
          </div>
          <div class="profile-form__field">
            <label for="new-password" class="profile-form__label">Neues Passwort</label>
            <input
              id="new-password"
              type="password"
              placeholder="••••••••"
              class="profile-form__input"
              :disabled="isEditing"
            />
          </div>
          <div class="profile-form__field">
            <label for="confirm-password" class="profile-form__label">Passwort bestätigen</label>
            <input
              id="confirm-password"
              type="password"
              placeholder="••••••••"
              class="profile-form__input"
              :disabled="isEditing"
            />
          </div>
        </div>
      </div>

      <!-- Submit Button -->
      <div class="profile-form__actions">
        <button type="submit" class="profile-form__submit" :disabled="isEditing">
          Änderungen speichern
        </button>
      </div>
    </form>

    <!-- Edit Mode Indicator -->
    <div v-if="isEditing" class="profile-form__edit-hint">
      <span>Profil-Formular Widget - {{ fields.length }} Felder</span>
    </div>
  </div>
</template>

<style scoped>
.profile-form {
  padding: 1.5rem;
}

.profile-form--editing {
  border: 2px dashed #d1d5db;
  border-radius: 8px;
  background-color: #fafafa;
}

.profile-form__header {
  margin-bottom: 1.5rem;
}

.profile-form__title {
  font-size: 1.25rem;
  font-weight: 600;
  color: #111827;
  margin: 0;
}

.profile-form__avatar-section {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  padding: 1.5rem;
  background-color: #f9fafb;
  border-radius: 8px;
  margin-bottom: 1.5rem;
}

.profile-form__avatar {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  background-color: var(--color-primary, #3b82f6);
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
  flex-shrink: 0;
}

.profile-form__avatar-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.profile-form__avatar-initials {
  font-size: 1.5rem;
  font-weight: 600;
  color: white;
}

.profile-form__avatar-actions {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.profile-form__avatar-btn {
  padding: 0.5rem 1rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: white;
  background-color: var(--color-primary, #3b82f6);
  border: none;
  border-radius: 6px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.profile-form__avatar-btn:hover:not(:disabled) {
  background-color: #2563eb;
}

.profile-form__avatar-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.profile-form__avatar-btn--secondary {
  background-color: white;
  color: #374151;
  border: 1px solid #d1d5db;
}

.profile-form__avatar-btn--secondary:hover:not(:disabled) {
  background-color: #f3f4f6;
}

.profile-form__fields--single-column {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.profile-form__fields--two-column {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1rem;
}

.profile-form__field {
  display: flex;
  flex-direction: column;
}

.profile-form__label {
  font-size: 0.875rem;
  font-weight: 500;
  color: #374151;
  margin-bottom: 0.375rem;
}

.profile-form__input {
  padding: 0.625rem 0.875rem;
  font-size: 0.875rem;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  background-color: white;
  transition:
    border-color 0.2s,
    box-shadow 0.2s;
}

.profile-form__input:focus {
  outline: none;
  border-color: var(--color-primary, #3b82f6);
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.profile-form__input:disabled {
  background-color: #f9fafb;
  cursor: not-allowed;
}

.profile-form__input--readonly {
  background-color: #f3f4f6;
  color: #6b7280;
}

.profile-form__hint {
  font-size: 0.75rem;
  color: #6b7280;
  margin: 0.25rem 0 0;
}

.profile-form__password-section {
  margin-top: 2rem;
  padding-top: 2rem;
  border-top: 1px solid #e5e7eb;
}

.profile-form__section-title {
  font-size: 1rem;
  font-weight: 600;
  color: #111827;
  margin: 0 0 1rem;
}

.profile-form__actions {
  margin-top: 2rem;
  padding-top: 1.5rem;
  border-top: 1px solid #e5e7eb;
}

.profile-form__submit {
  padding: 0.75rem 1.5rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: white;
  background-color: var(--color-primary, #3b82f6);
  border: none;
  border-radius: 6px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.profile-form__submit:hover:not(:disabled) {
  background-color: #2563eb;
}

.profile-form__submit:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.profile-form__edit-hint {
  margin-top: 1rem;
  padding: 0.5rem;
  background-color: #fef3c7;
  border-radius: 4px;
  text-align: center;
  font-size: 0.75rem;
  color: #92400e;
}
</style>
