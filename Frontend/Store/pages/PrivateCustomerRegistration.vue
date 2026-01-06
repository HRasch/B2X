<template>
  <div class="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 px-4 py-12">
    <div class="max-w-md mx-auto bg-white rounded-lg shadow-lg p-8">
      <!-- Header -->
      <h1 class="text-3xl font-bold text-gray-900 mb-2">
        {{ $t('registration.privateCustomerRegistration.title') }}
      </h1>
      <p class="text-gray-600 mb-6">
        {{ $t('registration.privateCustomerRegistration.subtitle') }}
      </p>

      <!-- Form -->
      <form @submit.prevent="submitForm" class="space-y-5" novalidate>
        <!-- Email Field -->
        <div>
          <label for="email" class="block text-sm font-medium text-gray-700 mb-1">
            {{ $t('registration.privateCustomerRegistration.form.email.label') }}
            <span class="text-red-500">*</span>
          </label>
          <div class="relative">
            <input
              id="email"
              v-model="formData.email"
              type="email"
              required
              :aria-label="$t('registration.privateCustomerRegistration.form.email.ariaLabel')"
              aria-describedby="email-error"
              :aria-invalid="!!errors.email"
              @blur="validateEmail"
              @input="checkEmailAvailability"
              class="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:opacity-50"
              :class="{ 'border-red-500': errors.email }"
              :disabled="isSubmitting"
              :placeholder="$t('registration.privateCustomerRegistration.form.email.placeholder')"
            />
            <!-- Email availability spinner -->
            <div v-if="emailChecking" class="absolute right-3 top-2.5" aria-hidden="true">
              <svg
                class="animate-spin h-5 w-5 text-blue-500"
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 24 24"
              >
                <circle
                  class="opacity-25"
                  cx="12"
                  cy="12"
                  r="10"
                  stroke="currentColor"
                  stroke-width="4"
                ></circle>
                <path
                  class="opacity-75"
                  fill="currentColor"
                  d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                ></path>
              </svg>
            </div>
          </div>
          <p v-if="errors.email" id="email-error" role="alert" class="text-red-500 text-sm mt-1">
            {{ errors.email }}
          </p>
          <p v-if="emailAvailable === true" class="text-green-600 text-sm mt-1">
            {{ $t('registration.emailAvailable') }}
          </p>
        </div>

        <!-- Password Field -->
        <div>
          <label for="password" class="block text-sm font-medium text-gray-700 mb-1">
            {{ $t('registration.privateCustomerRegistration.form.password.label') }}
            <span class="text-red-500">*</span>
          </label>
          <div class="relative">
            <input
              id="password"
              v-model="formData.password"
              :type="showPassword ? 'text' : 'password'"
              required
              :aria-label="$t('registration.privateCustomerRegistration.form.password.ariaLabel')"
              aria-describedby="password-error password-strength"
              :aria-invalid="!!errors.password"
              @input="validatePassword"
              class="w-full px-4 py-2 pr-10 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:opacity-50"
              :class="{ 'border-red-500': errors.password }"
              :disabled="isSubmitting"
              :placeholder="
                $t('registration.privateCustomerRegistration.form.password.placeholder')
              "
            />
            <!-- Password visibility toggle -->
            <button
              type="button"
              @click="showPassword = !showPassword"
              class="absolute right-3 top-2.5 text-gray-500 hover:text-gray-700 focus:outline-none focus:ring-2 focus:ring-blue-500 rounded"
              :aria-label="showPassword ? $t('form.hidePassword') : $t('form.showPassword')"
              :disabled="isSubmitting"
            >
              <svg v-if="showPassword" class="h-5 w-5" fill="currentColor" viewBox="0 0 20 20">
                <path d="M10 12a2 2 0 100-4 2 2 0 000 4z" />
                <path
                  fill-rule="evenodd"
                  d="M.458 10C1.732 5.943 5.522 3 10 3s8.268 2.943 9.542 7c-1.274 4.057-5.064 7-9.542 7S1.732 14.057.458 10zM14 10a4 4 0 11-8 0 4 4 0 018 0z"
                  clip-rule="evenodd"
                />
              </svg>
              <svg v-else class="h-5 w-5" fill="currentColor" viewBox="0 0 20 20">
                <path
                  fill-rule="evenodd"
                  d="M3.707 2.293a1 1 0 00-1.414 1.414l14 14a1 1 0 001.414-1.414l-14-14zM10 18a9.973 9.973 0 008.948-5.646 10.019 10.019 0 00-15.268-9.460A9.972 9.972 0 0010 18zm6.707-13.293a1 1 0 011.414 0l2 2a1 1 0 01-1.414 1.414l-2-2a1 1 0 010-1.414z"
                  clip-rule="evenodd"
                />
              </svg>
            </button>
          </div>
          <!-- Password strength meter -->
          <div v-if="formData.password" class="mt-2">
            <div class="flex items-center gap-2">
              <div class="flex-1 h-2 bg-gray-200 rounded-full overflow-hidden">
                <div
                  class="h-full transition-all duration-300"
                  :class="{
                    'w-1/3 bg-red-500': passwordStrength === 'weak',
                    'w-2/3 bg-yellow-500': passwordStrength === 'medium',
                    'w-full bg-green-500': passwordStrength === 'strong',
                  }"
                />
              </div>
              <span
                id="password-strength"
                class="text-xs font-medium"
                :class="{
                  'text-red-500': passwordStrength === 'weak',
                  'text-yellow-600': passwordStrength === 'medium',
                  'text-green-600': passwordStrength === 'strong',
                }"
              >
                {{ $t(`form.passwordStrength.${passwordStrength}`) }}
              </span>
            </div>
          </div>
          <p
            v-if="errors.password"
            id="password-error"
            role="alert"
            class="text-red-500 text-sm mt-1"
          >
            {{ errors.password }}
          </p>
          <p class="text-gray-500 text-xs mt-1">
            {{ $t('registration.passwordRequirements') }}
          </p>
        </div>

        <!-- Confirm Password Field -->
        <div>
          <label for="confirmPassword" class="block text-sm font-medium text-gray-700 mb-1">
            {{ $t('registration.privateCustomerRegistration.form.confirmPassword.label') }}
            <span class="text-red-500">*</span>
          </label>
          <input
            id="confirmPassword"
            v-model="formData.confirmPassword"
            :type="showPassword ? 'text' : 'password'"
            required
            :aria-label="
              $t('registration.privateCustomerRegistration.form.confirmPassword.ariaLabel')
            "
            aria-describedby="confirm-password-error"
            :aria-invalid="!!errors.confirmPassword"
            @input="validatePassword"
            class="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:opacity-50"
            :class="{ 'border-red-500': errors.confirmPassword }"
            :disabled="isSubmitting"
            :placeholder="
              $t('registration.privateCustomerRegistration.form.confirmPassword.placeholder')
            "
          />
          <p
            v-if="errors.confirmPassword"
            id="confirm-password-error"
            role="alert"
            class="text-red-500 text-sm mt-1"
          >
            {{ errors.confirmPassword }}
          </p>
        </div>

        <!-- First Name Field -->
        <div>
          <label for="firstName" class="block text-sm font-medium text-gray-700 mb-1">
            {{ $t('registration.privateCustomerRegistration.form.firstName.label') }}
            <span class="text-red-500">*</span>
          </label>
          <input
            id="firstName"
            v-model="formData.firstName"
            type="text"
            required
            :aria-label="$t('registration.privateCustomerRegistration.form.firstName.ariaLabel')"
            aria-describedby="firstName-error"
            :aria-invalid="!!errors.firstName"
            @blur="validateField('firstName')"
            class="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:opacity-50"
            :class="{ 'border-red-500': errors.firstName }"
            :disabled="isSubmitting"
            :placeholder="$t('registration.privateCustomerRegistration.form.firstName.placeholder')"
          />
          <p
            v-if="errors.firstName"
            id="firstName-error"
            role="alert"
            class="text-red-500 text-sm mt-1"
          >
            {{ errors.firstName }}
          </p>
        </div>

        <!-- Last Name Field -->
        <div>
          <label for="lastName" class="block text-sm font-medium text-gray-700 mb-1">
            {{ $t('registration.privateCustomerRegistration.form.lastName.label') }}
            <span class="text-red-500">*</span>
          </label>
          <input
            id="lastName"
            v-model="formData.lastName"
            type="text"
            required
            :aria-label="$t('registration.privateCustomerRegistration.form.lastName.ariaLabel')"
            aria-describedby="lastName-error"
            :aria-invalid="!!errors.lastName"
            @blur="validateField('lastName')"
            class="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:opacity-50"
            :class="{ 'border-red-500': errors.lastName }"
            :disabled="isSubmitting"
            :placeholder="$t('registration.privateCustomerRegistration.form.lastName.placeholder')"
          />
          <p
            v-if="errors.lastName"
            id="lastName-error"
            role="alert"
            class="text-red-500 text-sm mt-1"
          >
            {{ errors.lastName }}
          </p>
        </div>

        <!-- Phone Number Field (Optional) -->
        <div>
          <label for="phone" class="block text-sm font-medium text-gray-700 mb-1">
            {{ $t('registration.privateCustomerRegistration.form.phone.label') }}
            {{ !storeConfig.requirePhoneNumber ? '(' + $t('form.optional') + ')' : '' }}
          </label>
          <input
            id="phone"
            v-model="formData.phone"
            type="tel"
            :required="storeConfig.requirePhoneNumber"
            :aria-label="$t('registration.privateCustomerRegistration.form.phone.ariaLabel')"
            aria-describedby="phone-error"
            :aria-invalid="!!errors.phone"
            @blur="validateField('phone')"
            class="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:opacity-50"
            :class="{ 'border-red-500': errors.phone }"
            :disabled="isSubmitting"
            :placeholder="$t('registration.privateCustomerRegistration.form.phone.placeholder')"
          />
          <p v-if="errors.phone" id="phone-error" role="alert" class="text-red-500 text-sm mt-1">
            {{ errors.phone }}
          </p>
        </div>

        <!-- Address Field -->
        <div>
          <label for="address" class="block text-sm font-medium text-gray-700 mb-1">
            {{ $t('registration.privateCustomerRegistration.form.streetAddress.label') }}
            <span class="text-red-500">*</span>
          </label>
          <input
            id="address"
            v-model="formData.streetAddress"
            type="text"
            required
            :aria-label="
              $t('registration.privateCustomerRegistration.form.streetAddress.ariaLabel')
            "
            aria-describedby="address-error"
            :aria-invalid="!!errors.streetAddress"
            @blur="validateField('streetAddress')"
            class="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:opacity-50"
            :class="{ 'border-red-500': errors.streetAddress }"
            :disabled="isSubmitting"
            :placeholder="
              $t('registration.privateCustomerRegistration.form.streetAddress.placeholder')
            "
          />
          <p
            v-if="errors.streetAddress"
            id="address-error"
            role="alert"
            class="text-red-500 text-sm mt-1"
          >
            {{ errors.streetAddress }}
          </p>
        </div>

        <!-- City Field -->
        <div>
          <label for="city" class="block text-sm font-medium text-gray-700 mb-1">
            {{ $t('registration.privateCustomerRegistration.form.city.label') }}
            <span class="text-red-500">*</span>
          </label>
          <input
            id="city"
            v-model="formData.city"
            type="text"
            required
            :aria-label="$t('registration.privateCustomerRegistration.form.city.ariaLabel')"
            aria-describedby="city-error"
            :aria-invalid="!!errors.city"
            @blur="validateField('city')"
            class="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:opacity-50"
            :class="{ 'border-red-500': errors.city }"
            :disabled="isSubmitting"
            :placeholder="$t('registration.privateCustomerRegistration.form.city.placeholder')"
          />
          <p v-if="errors.city" id="city-error" role="alert" class="text-red-500 text-sm mt-1">
            {{ errors.city }}
          </p>
        </div>

        <!-- Postal Code Field -->
        <div>
          <label for="postalCode" class="block text-sm font-medium text-gray-700 mb-1">
            {{ $t('registration.privateCustomerRegistration.form.postalCode.label') }}
            <span class="text-red-500">*</span>
          </label>
          <input
            id="postalCode"
            v-model="formData.postalCode"
            type="text"
            required
            :aria-label="$t('registration.privateCustomerRegistration.form.postalCode.ariaLabel')"
            aria-describedby="postalCode-error"
            :aria-invalid="!!errors.postalCode"
            @blur="validateField('postalCode')"
            class="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:opacity-50"
            :class="{ 'border-red-500': errors.postalCode }"
            :disabled="isSubmitting"
            :placeholder="
              $t('registration.privateCustomerRegistration.form.postalCode.placeholder')
            "
          />
          <p
            v-if="errors.postalCode"
            id="postalCode-error"
            role="alert"
            class="text-red-500 text-sm mt-1"
          >
            {{ errors.postalCode }}
          </p>
        </div>

        <!-- Country Field -->
        <div>
          <label for="country" class="block text-sm font-medium text-gray-700 mb-1">
            {{ $t('registration.privateCustomerRegistration.form.country.label') }}
            <span class="text-red-500">*</span>
          </label>
          <select
            id="country"
            v-model="formData.country"
            required
            :aria-label="$t('registration.privateCustomerRegistration.form.country.ariaLabel')"
            aria-describedby="country-error"
            :aria-invalid="!!errors.country"
            @change="validateField('country')"
            class="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:opacity-50"
            :class="{ 'border-red-500': errors.country }"
            :disabled="isSubmitting"
          >
            <option value="">
              {{ $t('registration.privateCustomerRegistration.form.country.placeholder') }}
            </option>
            <option value="DE">
              {{ $t('registration.privateCustomerRegistration.form.country.options.DE') }}
            </option>
            <option value="AT">
              {{ $t('registration.privateCustomerRegistration.form.country.options.AT') }}
            </option>
            <option value="CH">
              {{ $t('registration.privateCustomerRegistration.form.country.options.CH') }}
            </option>
            <option value="FR">
              {{ $t('registration.privateCustomerRegistration.form.country.options.FR') }}
            </option>
            <option value="NL">
              {{ $t('registration.privateCustomerRegistration.form.country.options.NL') }}
            </option>
            <option value="BE">
              {{ $t('registration.privateCustomerRegistration.form.country.options.BE') }}
            </option>
            <option value="LU">
              {{ $t('registration.privateCustomerRegistration.form.country.options.LU') }}
            </option>
            <option value="PL">
              {{ $t('registration.privateCustomerRegistration.form.country.options.PL') }}
            </option>
            <option value="CZ">
              {{ $t('registration.privateCustomerRegistration.form.country.options.CZ') }}
            </option>
          </select>
          <p
            v-if="errors.country"
            id="country-error"
            role="alert"
            class="text-red-500 text-sm mt-1"
          >
            {{ errors.country }}
          </p>
        </div>

        <!-- State/Province Field (Optional) -->
        <div
          v-if="
            formData.country && !['DE', 'AT', 'CH', 'BE', 'LU', 'NL'].includes(formData.country)
          "
        >
          <label for="state" class="block text-sm font-medium text-gray-700 mb-1">
            {{ $t('registration.privateCustomerRegistration.form.state.label') }} ({{
              $t('form.optional')
            }})
          </label>
          <input
            id="state"
            v-model="formData.state"
            type="text"
            :aria-label="$t('registration.privateCustomerRegistration.form.state.ariaLabel')"
            aria-describedby="state-error"
            :aria-invalid="!!errors.state"
            @blur="validateField('state')"
            class="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:opacity-50"
            :class="{ 'border-red-500': errors.state }"
            :disabled="isSubmitting"
            :placeholder="$t('registration.privateCustomerRegistration.form.state.placeholder')"
          />
          <p v-if="errors.state" id="state-error" role="alert" class="text-red-500 text-sm mt-1">
            {{ errors.state }}
          </p>
        </div>

        <!-- Date of Birth (Optional) -->
        <div v-if="storeConfig.showBirthdayField">
          <label for="dateOfBirth" class="block text-sm font-medium text-gray-700 mb-1">
            {{ $t('registration.privateCustomerRegistration.form.dateOfBirth.label') }} ({{
              $t('form.optional')
            }})
          </label>
          <input
            id="dateOfBirth"
            v-model="formData.dateOfBirth"
            type="date"
            :aria-label="$t('registration.privateCustomerRegistration.form.dateOfBirth.ariaLabel')"
            aria-describedby="dateOfBirth-error"
            :aria-invalid="!!errors.dateOfBirth"
            @blur="validateField('dateOfBirth')"
            class="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:opacity-50"
            :class="{ 'border-red-500': errors.dateOfBirth }"
            :disabled="isSubmitting"
          />
          <p
            v-if="errors.dateOfBirth"
            id="dateOfBirth-error"
            role="alert"
            class="text-red-500 text-sm mt-1"
          >
            {{ errors.dateOfBirth }}
          </p>
        </div>

        <!-- Age Confirmation (Conditional) -->
        <div
          v-if="storeConfig.requiresAgeConfirmation"
          class="border-l-4 border-yellow-400 bg-yellow-50 p-4 rounded"
        >
          <label class="flex items-start gap-3 cursor-pointer">
            <input
              v-model="formData.ageConfirmed"
              type="checkbox"
              required
              :aria-label="
                $t('registration.privateCustomerRegistration.form.ageConfirmation.ariaLabel')
              "
              aria-describedby="age-confirm-error"
              ::aria-invalid="!!errors.ageConfirmed"
              class="mt-1 h-4 w-4 text-blue-600 rounded focus:ring-2 focus:ring-blue-500"
              :disabled="isSubmitting"
            />
            <span class="text-sm text-gray-700">
              {{
                $t('registration.ageConfirmation', {
                  age: storeConfig.ageConfirmationThreshold || 18,
                })
              }}
              <span class="text-red-500">*</span>
            </span>
          </label>
          <p
            v-if="errors.ageConfirmed"
            id="age-confirm-error"
            role="alert"
            class="text-red-500 text-sm mt-2"
          >
            {{ errors.ageConfirmed }}
          </p>
        </div>

        <!-- Terms and Conditions -->
        <div class="border-l-4 border-blue-400 bg-blue-50 p-4 rounded">
          <label class="flex items-start gap-3 cursor-pointer">
            <input
              v-model="formData.acceptTerms"
              type="checkbox"
              required
              :aria-label="
                $t('registration.privateCustomerRegistration.form.acceptTerms.ariaLabel')
              "
              aria-describedby="terms-error"
              ::aria-invalid="!!errors.acceptTerms"
              class="mt-1 h-4 w-4 text-blue-600 rounded focus:ring-2 focus:ring-blue-500"
              :disabled="isSubmitting"
            />
            <span class="text-sm text-gray-700">
              {{ $t('registration.privateCustomerRegistration.messages.acceptTerms') }}
              <a
                :href="storeConfig.termsOfServiceUrl || '#'"
                target="_blank"
                rel="noopener noreferrer"
                class="text-blue-600 hover:text-blue-800 underline"
              >
                {{ $t('registration.privateCustomerRegistration.links.termsLink') }}
              </a>
              <span class="text-red-500">*</span>
            </span>
          </label>
          <p
            v-if="errors.acceptTerms"
            id="terms-error"
            role="alert"
            class="text-red-500 text-sm mt-2"
          >
            {{ errors.acceptTerms }}
          </p>
          <!-- VVVG Notice -->
          <div
            v-if="storeConfig.withdrawalPolicyCustomText"
            class="mt-3 pt-3 border-t border-blue-200 text-xs text-gray-600"
          >
            <strong class="block mb-1">{{
              $t('registration.privateCustomerRegistration.messages.withdrawalNotice')
            }}</strong>
            <p v-html="storeConfig.withdrawalPolicyCustomText" />
          </div>
        </div>

        <!-- Privacy Policy -->
        <div class="border-l-4 border-purple-400 bg-purple-50 p-4 rounded">
          <label class="flex items-start gap-3 cursor-pointer">
            <input
              v-model="formData.acceptPrivacy"
              type="checkbox"
              required
              :aria-label="
                $t('registration.privateCustomerRegistration.form.acceptPrivacy.ariaLabel')
              "
              aria-describedby="privacy-error"
              ::aria-invalid="!!errors.acceptPrivacy"
              class="mt-1 h-4 w-4 text-purple-600 rounded focus:ring-2 focus:ring-purple-500"
              :disabled="isSubmitting"
            />
            <span class="text-sm text-gray-700">
              {{ $t('registration.privateCustomerRegistration.messages.acceptPrivacy') }}
              <a
                :href="storeConfig.privacyPolicyUrl || '#'"
                target="_blank"
                rel="noopener noreferrer"
                class="text-purple-600 hover:text-purple-800 underline"
              >
                {{ $t('registration.privateCustomerRegistration.links.privacyLink') }}
              </a>
              <span class="text-red-500">*</span>
            </span>
          </label>
          <p
            v-if="errors.acceptPrivacy"
            id="privacy-error"
            role="alert"
            class="text-red-500 text-sm mt-2"
          >
            {{ errors.acceptPrivacy }}
          </p>
        </div>

        <!-- Marketing Communications (Optional) -->
        <div
          v-if="storeConfig.enableMarketingConsent"
          class="border-l-4 border-green-400 bg-green-50 p-4 rounded"
        >
          <label class="flex items-start gap-3 cursor-pointer">
            <input
              v-model="formData.acceptMarketing"
              type="checkbox"
              :aria-label="
                $t('registration.privateCustomerRegistration.form.acceptMarketing.ariaLabel')
              "
              class="mt-1 h-4 w-4 text-green-600 rounded focus:ring-2 focus:ring-green-500"
              :disabled="isSubmitting"
            />
            <span class="text-sm text-gray-700">
              {{ $t('registration.privateCustomerRegistration.messages.acceptMarketing') }}
            </span>
          </label>
        </div>

        <!-- Submit Button -->
        <button
          type="submit"
          class="w-full min-h-[48px] bg-blue-600 text-white font-semibold rounded-lg hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
          :disabled="isSubmitting || !emailAvailable"
        >
          <span v-if="!isSubmitting">{{
            $t('registration.privateCustomerRegistration.actions.createAccount')
          }}</span>
          <span v-else class="flex items-center justify-center gap-2">
            <svg
              class="animate-spin h-5 w-5"
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
            >
              <circle
                class="opacity-25"
                cx="12"
                cy="12"
                r="10"
                stroke="currentColor"
                stroke-width="4"
              ></circle>
              <path
                class="opacity-75"
                fill="currentColor"
                d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
              ></path>
            </svg>
            {{ $t('registration.privateCustomerRegistration.actions.creating') }}
          </span>
        </button>

        <!-- Login Link -->
        <p class="text-center text-sm text-gray-600 mt-4">
          {{ $t('registration.privateCustomerRegistration.messages.alreadyHaveAccount') }}
          <router-link to="/login" class="text-blue-600 hover:text-blue-800 font-medium">
            {{ $t('registration.privateCustomerRegistration.links.loginLink') }}
          </router-link>
        </p>
      </form>

      <!-- Error Alert (General) -->
      <div
        v-if="generalError"
        role="alert"
        class="mt-6 p-4 bg-red-50 border border-red-200 rounded-lg"
      >
        <p class="text-red-800 font-semibold">
          {{ $t('registration.privateCustomerRegistration.messages.error') }}
        </p>
        <p class="text-red-700 text-sm mt-1">{{ generalError }}</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';

interface FormData {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
  phone: string;
  streetAddress: string;
  city: string;
  postalCode: string;
  country: string;
  state: string;
  dateOfBirth: string;
  acceptTerms: boolean;
  acceptPrivacy: boolean;
  acceptMarketing: boolean;
  ageConfirmed: boolean;
}

interface StoreConfig {
  requiresAgeConfirmation: boolean;
  ageConfirmationThreshold: number;
  showBirthdayField: boolean;
  requirePhoneNumber: boolean;
  enableMarketingConsent: boolean;
  enforcePasswordComplexity: boolean;
  passwordMinimumLength: number;
  termsOfServiceUrl: string;
  privacyPolicyUrl: string;
  withdrawalPolicyCustomText: string;
}

const router = useRouter();
const { t } = useI18n();

const formData = ref<FormData>({
  email: '',
  password: '',
  confirmPassword: '',
  firstName: '',
  lastName: '',
  phone: '',
  streetAddress: '',
  city: '',
  postalCode: '',
  country: '',
  state: '',
  dateOfBirth: '',
  acceptTerms: false,
  acceptPrivacy: false,
  acceptMarketing: false,
  ageConfirmed: false,
});

const storeConfig = ref<StoreConfig>({
  requiresAgeConfirmation: false,
  ageConfirmationThreshold: 18,
  showBirthdayField: false,
  requirePhoneNumber: false,
  enableMarketingConsent: true,
  enforcePasswordComplexity: true,
  passwordMinimumLength: 12,
  termsOfServiceUrl: '/terms',
  privacyPolicyUrl: '/privacy',
  withdrawalPolicyCustomText:
    'Sie haben das Recht, innerhalb von 14 Tagen nach dem Kauf die Ware ohne Angabe von Gründen zurückzugeben.',
});

const errors = ref<Partial<Record<keyof FormData, string>>>({});
const showPassword = ref(false);
const isSubmitting = ref(false);
const generalError = ref('');
const emailChecking = ref(false);
const emailAvailable = ref<boolean | null>(null);
let emailCheckTimeout: ReturnType<typeof setTimeout> | null = null;

const passwordStrength = computed(() => {
  const pwd = formData.value.password;
  if (!pwd) return '';

  let strength = 0;
  if (pwd.length >= 12) strength++;
  if (/[A-Z]/.test(pwd)) strength++;
  if (/[a-z]/.test(pwd)) strength++;
  if (/\d/.test(pwd)) strength++;
  if (/[!@#$%^&*()_+\-=[]{};"\\|,.<>?]/.test(pwd)) strength++;

  if (strength < 3) return 'weak';
  if (strength < 4) return 'medium';
  return 'strong';
});

const validateEmail = () => {
  const email = formData.value.email.trim();
  if (!email) {
    errors.value.email = t('validation.emailRequired');
    return false;
  }

  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailRegex.test(email)) {
    errors.value.email = t('validation.emailInvalid');
    return false;
  }

  delete errors.value.email;
  return true;
};

const checkEmailAvailability = () => {
  if (!validateEmail()) {
    emailAvailable.value = null;
    return;
  }

  // Clear previous timeout
  if (emailCheckTimeout) clearTimeout(emailCheckTimeout);

  // Debounce email availability check (500ms)
  emailChecking.value = true;
  emailCheckTimeout = setTimeout(async () => {
    try {
      const response = await fetch(
        `/api/auth/check-email?email=${encodeURIComponent(formData.value.email)}`
      );
      const data = await response.json();
      emailAvailable.value = data.available === true;
      if (!emailAvailable.value) {
        errors.value.email = t('validation.emailTaken');
      } else {
        delete errors.value.email;
      }
    } catch (error) {
      console.error('Email check error:', error);
      emailAvailable.value = null;
    } finally {
      emailChecking.value = false;
    }
  }, 500);
};

const validatePassword = () => {
  const pwd = formData.value.password;
  const confirm = formData.value.confirmPassword;

  if (!pwd) {
    errors.value.password = t('validation.passwordRequired');
    return false;
  }

  const minLength = storeConfig.value.passwordMinimumLength;
  if (pwd.length < minLength) {
    errors.value.password = t('validation.passwordTooShort', {
      length: minLength,
    });
    return false;
  }

  if (storeConfig.value.enforcePasswordComplexity) {
    if (
      !/[A-Z]/.test(pwd) ||
      !/[a-z]/.test(pwd) ||
      !/\d/.test(pwd) ||
      !/[!@#$%^&*()_+\-=[]{};"\\|,.<>?]/.test(pwd)
    ) {
      errors.value.password = t('validation.passwordWeak');
      return false;
    }
  }

  delete errors.value.password;

  if (confirm && pwd !== confirm) {
    errors.value.confirmPassword = t('validation.passwordsMismatch');
    return false;
  } else if (confirm && pwd === confirm) {
    delete errors.value.confirmPassword;
  }

  return true;
};

const validateField = (fieldName: keyof FormData) => {
  const value = formData.value[fieldName];

  switch (fieldName) {
    case 'firstName':
    case 'lastName':
      if (!value) {
        errors.value[fieldName] = t(`validation.${fieldName}Required`);
        return false;
      }
      if (typeof value === 'string' && (value.length < 2 || value.length > 50)) {
        errors.value[fieldName] = t(`validation.${fieldName}Length`);
        return false;
      }
      delete errors.value[fieldName];
      return true;

    case 'phone':
      if (value && typeof value === 'string') {
        const phoneRegex = /^\+?[1-9]\d{1,14}$/;
        if (!phoneRegex.test(value.replace(/\D/g, ''))) {
          errors.value.phone = t('validation.phoneInvalid');
          return false;
        }
      } else if (storeConfig.value.requirePhoneNumber) {
        errors.value.phone = t('validation.phoneRequired');
        return false;
      }
      delete errors.value.phone;
      return true;

    case 'streetAddress':
      if (!value || (typeof value === 'string' && value.trim().length === 0)) {
        errors.value.streetAddress = t('validation.addressRequired');
        return false;
      }
      if (typeof value === 'string' && value.length > 100) {
        errors.value.streetAddress = t('validation.addressTooLong');
        return false;
      }
      delete errors.value.streetAddress;
      return true;

    case 'city':
      if (!value || (typeof value === 'string' && value.trim().length === 0)) {
        errors.value.city = t('validation.cityRequired');
        return false;
      }
      if (typeof value === 'string' && value.length > 50) {
        errors.value.city = t('validation.cityTooLong');
        return false;
      }
      delete errors.value.city;
      return true;

    case 'postalCode':
      if (!value || (typeof value === 'string' && value.trim().length === 0)) {
        errors.value.postalCode = t('validation.postalCodeRequired');
        return false;
      }
      if (typeof value === 'string' && value.length > 20) {
        errors.value.postalCode = t('validation.postalCodeTooLong');
        return false;
      }
      delete errors.value.postalCode;
      return true;

    case 'country':
      if (!value) {
        errors.value.country = t('validation.countryRequired');
        return false;
      }
      delete errors.value.country;
      return true;

    case 'state':
      if (value && typeof value === 'string' && value.length > 50) {
        errors.value.state = t('validation.stateTooLong');
        return false;
      }
      delete errors.value.state;
      return true;

    case 'dateOfBirth':
      if (value && typeof value === 'string') {
        const birthDate = new Date(value);
        const today = new Date();
        if (birthDate > today) {
          errors.value.dateOfBirth = t('validation.dateOfBirthFuture');
          return false;
        }
      }
      delete errors.value.dateOfBirth;
      return true;

    default:
      return true;
  }
};

const submitForm = async () => {
  // Validate all fields
  if (!validateEmail() || !validatePassword()) {
    return;
  }

  if (
    !validateField('firstName') ||
    !validateField('lastName') ||
    !validateField('phone') ||
    !validateField('streetAddress') ||
    !validateField('city') ||
    !validateField('postalCode') ||
    !validateField('country') ||
    !validateField('state') ||
    !validateField('dateOfBirth')
  ) {
    return;
  }

  // Validate checkboxes
  if (!formData.value.acceptTerms) {
    errors.value.acceptTerms = t('validation.termsRequired');
    return;
  }

  if (!formData.value.acceptPrivacy) {
    errors.value.acceptPrivacy = t('validation.privacyRequired');
    return;
  }

  if (storeConfig.value.requiresAgeConfirmation && !formData.value.ageConfirmed) {
    errors.value.ageConfirmed = t('validation.ageConfirmRequired');
    return;
  }

  isSubmitting.value = true;
  generalError.value = '';

  try {
    const response = await fetch('/api/auth/register', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        email: formData.value.email,
        password: formData.value.password,
        firstName: formData.value.firstName,
        lastName: formData.value.lastName,
        phone: formData.value.phone || null,
        streetAddress: formData.value.streetAddress,
        city: formData.value.city,
        postalCode: formData.value.postalCode,
        country: formData.value.country,
        state: formData.value.state || null,
        dateOfBirth: formData.value.dateOfBirth || null,
        acceptMarketing: formData.value.acceptMarketing,
      }),
    });

    if (!response.ok) {
      const data = await response.json();
      generalError.value = data.message || t('registration.error');
      return;
    }

    // Success - redirect to login or dashboard
    await router.push('/login?registered=true');
  } catch (error) {
    console.error('Registration error:', error);
    generalError.value = t('registration.privateCustomerRegistration.messages.networkError');
  } finally {
    isSubmitting.value = false;
  }
};

onMounted(() => {
  // Load store configuration from localStorage (in production, this would come from API)
  const storedConfig = localStorage.getItem('storeConfig');
  if (storedConfig) {
    try {
      storeConfig.value = { ...storeConfig.value, ...JSON.parse(storedConfig) };
    } catch (e) {
      console.error('Failed to load store config:', e);
    }
  }
});
</script>

<style scoped>
/* Smooth transitions for input states */
input,
select {
  transition:
    border-color 0.2s,
    box-shadow 0.2s;
}

input:focus,
select:focus {
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

/* Password strength meter animation */
.animate-spin {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

/* Accessibility: High contrast focus states */
@media (prefers-contrast: more) {
  input:focus,
  select:focus,
  button:focus {
    outline: 3px solid currentColor !important;
    outline-offset: 2px;
  }
}

/* Reduced motion support */
@media (prefers-reduced-motion: reduce) {
  input,
  select,
  button {
    transition: none;
  }

  .animate-spin {
    animation: none;
  }
}
</style>
