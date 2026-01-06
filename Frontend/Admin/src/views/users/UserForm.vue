<template>
  <div class="user-form-page">
    <PageHeader
      :title="isEdit ? $t('users.edit') : $t('users.create')"
    >
      <template #actions>
        <router-link to="/users" class="btn-secondary">
          {{ $t('users.back') }}
        </router-link>
      </template>
    </PageHeader>

    <div class="page-content">
      <CardContainer elevated>
        <form @submit.prevent="handleSubmit">
          <!-- Basic Info Section -->
          <FormSection :title="$t('users.form.basicInfo')">
            <FormRow :cols="2">
              <FormGroup
                :label="$t('users.form.firstName')"
                input-id="firstName"
                :error="errors.firstName"
                required
              >
                <input
                  id="firstName"
                  v-model="form.firstName"
                  type="text"
                  required
                  data-testid="firstName-input"
                />
              </FormGroup>

              <FormGroup
                :label="$t('users.form.lastName')"
                input-id="lastName"
                :error="errors.lastName"
                required
              >
                <input
                  id="lastName"
                  v-model="form.lastName"
                  type="text"
                  required
                  data-testid="lastName-input"
                />
              </FormGroup>
            </FormRow>

            <FormRow :cols="2">
              <FormGroup
                :label="$t('users.form.email')"
                input-id="email"
                :error="errors.email"
                required
              >
                <input
                  id="email"
                  v-model="form.email"
                  type="email"
                  required
                  data-testid="email-input"
                />
              </FormGroup>

              <FormGroup
                :label="$t('users.form.phone')"
                input-id="phoneNumber"
              >
                <input
                  id="phoneNumber"
                  v-model="form.phoneNumber"
                  type="tel"
                  data-testid="phoneNumber-input"
                />
              </FormGroup>
            </FormRow>

            <FormRow :cols="3">
              <div class="checkbox-group">
                <label>
                  <input
                    v-model="form.isEmailVerified"
                    type="checkbox"
                    data-testid="isEmailVerified-input"
                  />
                  {{ $t('users.form.emailVerified') }}
                </label>
              </div>

              <div class="checkbox-group">
                <label>
                  <input
                    v-model="form.isPhoneVerified"
                    type="checkbox"
                    data-testid="isPhoneVerified-input"
                  />
                  {{ $t('users.form.phoneVerified') }}
                </label>
              </div>

              <div class="checkbox-group">
                <label>
                  <input
                    v-model="form.isActive"
                    type="checkbox"
                    data-testid="isActive-input"
                  />
                  {{ $t('users.form.active') }}
                </label>
              </div>
            </FormRow>
          </FormSection>

          <!-- Profile Section -->
          <FormSection :title="$t('users.form.profile')">
            <FormRow :cols="2">
              <FormGroup
                :label="$t('users.form.company')"
                input-id="companyName"
              >
                <input
                  id="companyName"
                  v-model="profile.companyName"
                  type="text"
                  data-testid="companyName-input"
                />
              </FormGroup>

              <FormGroup
                :label="$t('users.form.jobTitle')"
                input-id="jobTitle"
              >
                <input
                  id="jobTitle"
                  v-model="profile.jobTitle"
                  type="text"
                  data-testid="jobTitle-input"
                />
              </FormGroup>
            </FormRow>

            <FormRow :cols="2">
              <FormGroup
                :label="$t('users.form.dateOfBirth')"
                input-id="dateOfBirth"
              >
                <input
                  id="dateOfBirth"
                  v-model="profile.dateOfBirth"
                  type="date"
                  data-testid="dateOfBirth-input"
                />
              </FormGroup>

              <FormGroup
                :label="$t('users.form.nationality')"
                input-id="nationality"
              >
                <input
                  id="nationality"
                  v-model="profile.nationality"
                  type="text"
                  data-testid="nationality-input"
                />
              </FormGroup>
            </FormRow>

            <FormRow :cols="2">
              <FormGroup
                :label="$t('users.form.preferredLanguage')"
                input-id="preferredLanguage"
              >
                <select
                  id="preferredLanguage"
                  v-model="profile.preferredLanguage"
                  data-testid="preferredLanguage-input"
                >
                  <option value="de">{{ $t('users.languages.de') }}</option>
                  <option value="en">{{ $t('users.languages.en') }}</option>
                  <option value="fr">{{ $t('users.languages.fr') }}</option>
                  <option value="it">{{ $t('users.languages.it') }}</option>
                </select>
              </FormGroup>

              <FormGroup
                :label="$t('users.form.timezone')"
                input-id="timezone"
              >
                <input
                  id="timezone"
                  v-model="profile.timezone"
                  type="text"
                  :placeholder="$t('users.form.timezonePlaceholder')"
                  data-testid="timezone-input"
                />
              </FormGroup>
            </FormRow>

            <FormGroup
              :label="$t('users.form.bio')"
              input-id="bio"
            >
              <textarea
                id="bio"
                v-model="profile.bio"
                rows="4"
                data-testid="bio-input"
              ></textarea>
            </FormGroup>

            <FormRow :cols="2">
              <div class="checkbox-group">
                <label>
                  <input
                    v-model="profile.receiveNewsletter"
                    type="checkbox"
                    data-testid="receiveNewsletter-input"
                  />
                  {{ $t('users.form.newsletter') }}
                </label>
              </div>

              <div class="checkbox-group">
                <label>
                  <input
                    v-model="profile.receivePromotionalEmails"
                    type="checkbox"
                    data-testid="receivePromotionalEmails-input"
                  />
                  {{ $t('users.form.promotionalEmails') }}
                </label>
              </div>
            </FormRow>
          </FormSection>

          <!-- Error Alert -->
          <div v-if="submitError" class="alert alert-danger">
            {{ submitError }}
          </div>

          <!-- Submit Buttons -->
          <div class="form-actions">
            <router-link to="/users" class="btn-secondary">
              {{ $t('ui.cancel') }}
            </router-link>
            <button type="submit" class="btn-primary" :disabled="submitting">
              {{ submitting ? $t('users.messages.saving') : $t('ui.save') }}
            </button>
          </div>
        </form>
      </CardContainer>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { useUserStore } from '@/stores/users';
import { PageHeader, FormSection, FormRow, FormGroup, CardContainer } from '@/components/layout';
import type { User, UserProfile } from '@/types/user';

const route = useRoute();
const router = useRouter();
const userStore = useUserStore();
const { t } = useI18n();

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
      submitError.value = t('users.messages.loadError');
    }
  }
});

const validateForm = () => {
  errors.firstName = !form.firstName ? t('users.validation.firstNameRequired') : '';
  errors.lastName = !form.lastName ? t('users.validation.lastNameRequired') : '';
  errors.email = !form.email ? t('users.validation.emailRequired') : '';

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
    const errorMessage = error instanceof Error ? error.message : t('users.messages.saveError');
    submitError.value = errorMessage;
  } finally {
    submitting.value = false;
  }
};
</script>

<style scoped>
.user-form-page {
  min-height: 100vh;
  background: #f9fafb;
}

.dark .user-form-page {
  background: #0f172a;
}

.page-content {
  max-width: 800px;
  margin: 0 auto;
  padding: 2rem;
}

.checkbox-group {
  display: flex;
  align-items: center;
}

.checkbox-group label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-weight: 500;
  color: #374151;
  cursor: pointer;
}

.dark .checkbox-group label {
  color: #d1d5db;
}

.checkbox-group input[type='checkbox'] {
  width: 1rem;
  height: 1rem;
  accent-color: #3b82f6;
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

.dark .alert-danger {
  background: rgba(239, 68, 68, 0.1);
  color: #fca5a5;
  border-color: rgba(239, 68, 68, 0.3);
}

.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  margin-top: 2rem;
  padding-top: 1.5rem;
  border-top: 1px solid #e5e7eb;
}

.dark .form-actions {
  border-top-color: #374151;
}

.btn-primary,
.btn-secondary {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 0.5rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  text-decoration: none;
  display: inline-block;
  font-size: 0.9375rem;
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

.dark .btn-secondary {
  background: #374151;
  color: #e5e7eb;
}

.dark .btn-secondary:hover {
  background: #4b5563;
}
</style>
