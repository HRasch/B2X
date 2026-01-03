<template>
  <div class="form-container">
    <div class="form-header">
      <h1>{{ isEdit ? 'Benutzer bearbeiten' : 'Neuer Benutzer' }}</h1>
      <router-link to="/users" class="btn-back">← Zurück</router-link>
    </div>

    <form @submit.prevent="handleSubmit" class="user-form">
      <!-- Basic Info Section -->
      <div class="form-section">
        <h2>Grundinformationen</h2>

        <div class="form-row">
          <div class="form-group">
            <label for="firstName">Vorname *</label>
            <input
              id="firstName"
              v-model="form.firstName"
              type="text"
              required
              :class="{ 'has-error': errors.firstName }"
              data-testid="firstName-input"
            />
            <span v-if="errors.firstName" class="error-text">
              {{ errors.firstName }}
            </span>
          </div>

          <div class="form-group">
            <label for="lastName">Nachname *</label>
            <input
              id="lastName"
              v-model="form.lastName"
              type="text"
              required
              :class="{ 'has-error': errors.lastName }"
              data-testid="lastName-input"
            />
            <span v-if="errors.lastName" class="error-text">
              {{ errors.lastName }}
            </span>
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="email">E-Mail *</label>
            <input
              id="email"
              v-model="form.email"
              type="email"
              required
              :class="{ 'has-error': errors.email }"
              data-testid="email-input"
            />
            <span v-if="errors.email" class="error-text">
              {{ errors.email }}
            </span>
          </div>

          <div class="form-group">
            <label for="phoneNumber">Telefon</label>
            <input
              id="phoneNumber"
              v-model="form.phoneNumber"
              type="tel"
              data-testid="phoneNumber-input"
            />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label>
              <input
                v-model="form.isEmailVerified"
                type="checkbox"
                data-testid="isEmailVerified-input"
              />
              E-Mail verifiziert
            </label>
          </div>

          <div class="form-group">
            <label>
              <input
                v-model="form.isPhoneVerified"
                type="checkbox"
                data-testid="isPhoneVerified-input"
              />
              Telefon verifiziert
            </label>
          </div>

          <div class="form-group">
            <label>
              <input v-model="form.isActive" type="checkbox" data-testid="isActive-input" />
              Aktiv
            </label>
          </div>
        </div>
      </div>

      <!-- Profile Section -->
      <div class="form-section">
        <h2>Profil</h2>

        <div class="form-row">
          <div class="form-group">
            <label for="companyName">Unternehmen</label>
            <input
              id="companyName"
              v-model="profile.companyName"
              type="text"
              data-testid="companyName-input"
            />
          </div>

          <div class="form-group">
            <label for="jobTitle">Berufsbezeichnung</label>
            <input
              id="jobTitle"
              v-model="profile.jobTitle"
              type="text"
              data-testid="jobTitle-input"
            />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="dateOfBirth">Geburtsdatum</label>
            <input
              id="dateOfBirth"
              v-model="profile.dateOfBirth"
              type="date"
              data-testid="dateOfBirth-input"
            />
          </div>

          <div class="form-group">
            <label for="nationality">Nationalität</label>
            <input
              id="nationality"
              v-model="profile.nationality"
              type="text"
              data-testid="nationality-input"
            />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="preferredLanguage">Bevorzugte Sprache</label>
            <select
              id="preferredLanguage"
              v-model="profile.preferredLanguage"
              data-testid="preferredLanguage-input"
            >
              <option value="de">Deutsch</option>
              <option value="en">English</option>
              <option value="fr">Français</option>
              <option value="it">Italiano</option>
            </select>
          </div>

          <div class="form-group">
            <label for="timezone">Zeitzone</label>
            <input
              id="timezone"
              v-model="profile.timezone"
              type="text"
              placeholder="Europe/Berlin"
              data-testid="timezone-input"
            />
          </div>
        </div>

        <div class="form-group">
          <label for="bio">Bio</label>
          <textarea id="bio" v-model="profile.bio" rows="4" data-testid="bio-input"></textarea>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label>
              <input
                v-model="profile.receiveNewsletter"
                type="checkbox"
                data-testid="receiveNewsletter-input"
              />
              Newsletter abonnieren
            </label>
          </div>

          <div class="form-group">
            <label>
              <input
                v-model="profile.receivePromotionalEmails"
                type="checkbox"
                data-testid="receivePromotionalEmails-input"
              />
              Werbe-E-Mails erhalten
            </label>
          </div>
        </div>
      </div>

      <!-- Error Alert -->
      <div v-if="submitError" class="alert alert-danger">
        {{ submitError }}
      </div>

      <!-- Submit Buttons -->
      <div class="form-actions">
        <router-link to="/users" class="btn btn-secondary"> Abbrechen </router-link>
        <button type="submit" class="btn btn-primary" :disabled="submitting">
          {{ submitting ? 'Wird gespeichert...' : 'Speichern' }}
        </button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useUserStore } from '@/stores/users';
import type { User, UserProfile } from '@/types/user';

const route = useRoute();
const router = useRouter();
const userStore = useUserStore();

const isEdit = ref(false);
const submitting = ref(false);
const submitError = ref('');

const form = reactive<Partial<User>>({
  firstName: '',
  lastName: '',
  email: '',
  phoneNumber: '',
  isEmailVerified: false,
  isPhoneVerified: false,
  isActive: true,
});

const profile = reactive<Partial<UserProfile>>({
  companyName: '',
  jobTitle: '',
  dateOfBirth: '',
  nationality: '',
  preferredLanguage: 'de',
  timezone: 'Europe/Berlin',
  bio: '',
  receiveNewsletter: false,
  receivePromotionalEmails: false,
});

const errors = reactive<Record<string, string>>({});

onMounted(async () => {
  if (route.params.id) {
    isEdit.value = true;
    try {
      const user = await userStore.fetchUser(route.params.id as string);
      Object.assign(form, user);
    } catch {
      submitError.value = 'Benutzer konnte nicht geladen werden';
    }
  }
});

const validateForm = () => {
  errors.firstName = !form.firstName ? 'Vorname ist erforderlich' : '';
  errors.lastName = !form.lastName ? 'Nachname ist erforderlich' : '';
  errors.email = !form.email ? 'E-Mail ist erforderlich' : '';

  return !errors.firstName && !errors.lastName && !errors.email;
};

const handleSubmit = async () => {
  if (!validateForm()) return;

  submitting.value = true;
  submitError.value = '';

  try {
    if (isEdit.value && route.params.id) {
      await userStore.updateUser(route.params.id as string, form);
    } else {
      await userStore.createUser(form);
    }

    await router.push('/users');
  } catch (error: unknown) {
    const errorMessage = error instanceof Error ? error.message : 'Fehler beim Speichern';
    submitError.value = errorMessage;
  } finally {
    submitting.value = false;
  }
};
</script>

<style scoped>
.form-container {
  max-width: 800px;
  margin: 0 auto;
  padding: 2rem;
}

.form-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
  padding-bottom: 1.5rem;
  border-bottom: 1px solid #e5e7eb;
}

.form-header h1 {
  margin: 0;
  font-size: 1.875rem;
  font-weight: 600;
  color: #1f2937;
}

.btn-back {
  color: #3b82f6;
  text-decoration: none;
  font-weight: 500;
}

.btn-back:hover {
  color: #2563eb;
  text-decoration: underline;
}

.user-form {
  background: white;
  border-radius: 0.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  padding: 2rem;
}

.form-section {
  margin-bottom: 2rem;
}

.form-section h2 {
  margin: 0 0 1.5rem 0;
  font-size: 1.25rem;
  font-weight: 600;
  color: #1f2937;
  padding-bottom: 1rem;
  border-bottom: 2px solid #f3f4f6;
}

.form-row {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-bottom: 1.5rem;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group label {
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: #374151;
}

.form-group input[type='checkbox'],
.form-group input[type='radio'] {
  width: auto;
  margin-right: 0.5rem;
}

.form-group input[type='checkbox'] + label,
.form-group input[type='radio'] + label {
  display: inline;
  margin-bottom: 0;
  font-weight: normal;
}

.form-group input,
.form-group select,
.form-group textarea {
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  font-size: 0.95rem;
  font-family: inherit;
}

.form-group input:focus,
.form-group select:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.form-group input.has-error,
.form-group select.has-error,
.form-group textarea.has-error {
  border-color: #ef4444;
}

.form-group textarea {
  resize: vertical;
  min-height: 100px;
}

.error-text {
  color: #ef4444;
  font-size: 0.875rem;
  margin-top: 0.25rem;
}

.alert {
  padding: 1rem;
  margin-bottom: 1.5rem;
  border-radius: 0.5rem;
}

.alert-danger {
  background: #fee2e2;
  color: #991b1b;
  border: 1px solid #fca5a5;
}

.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  margin-top: 2rem;
  padding-top: 2rem;
  border-top: 1px solid #e5e7eb;
}

.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 0.5rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  text-decoration: none;
  display: inline-block;
}

.btn-primary {
  background: #3b82f6;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background: #2563eb;
}

.btn-primary:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.btn-secondary {
  background: #e5e7eb;
  color: #374151;
}

.btn-secondary:hover {
  background: #d1d5db;
}
</style>
