<template>
  <div class="dispute-resolution-container">
    <!-- Header Section -->
    <section class="hero-section">
      <h1>{{ t("disputes.title") }}</h1>
      <p class="subtitle">{{ t("disputes.subtitle") }}</p>
    </section>

    <!-- ODR Information Section -->
    <section class="odr-info-section">
      <div class="content-wrapper">
        <h2>{{ t("disputes.odr_title") }}</h2>

        <div class="info-cards">
          <div class="info-card">
            <h3>{{ t("disputes.what_is_odr") }}</h3>
            <p>{{ t("disputes.what_is_odr_desc") }}</p>
          </div>

          <div class="info-card">
            <h3>{{ t("disputes.when_to_use") }}</h3>
            <ul>
              <li>{{ t("disputes.reason_1") }}</li>
              <li>{{ t("disputes.reason_2") }}</li>
              <li>{{ t("disputes.reason_3") }}</li>
              <li>{{ t("disputes.reason_4") }}</li>
            </ul>
          </div>

          <div class="info-card">
            <h3>{{ t("disputes.how_it_works") }}</h3>
            <ol>
              <li>{{ t("disputes.step_1") }}</li>
              <li>{{ t("disputes.step_2") }}</li>
              <li>{{ t("disputes.step_3") }}</li>
              <li>{{ t("disputes.step_4") }}</li>
            </ol>
          </div>
        </div>

        <!-- EU ODR Platform Link -->
        <div class="odr-platform-box">
          <h3>{{ t("disputes.eu_odr_title") }}</h3>
          <p>{{ t("disputes.eu_odr_desc") }}</p>
          <a
            href="https://ec.europa.eu/consumers/odr/"
            target="_blank"
            rel="noopener noreferrer"
            class="odr-link"
            :aria-label="t('disputes.eu_odr_link_label')"
          >
            {{ t("disputes.visit_odr_platform") }}
            <span class="icon">→</span>
          </a>
        </div>
      </div>
    </section>

    <!-- Contact Form Section -->
    <section class="contact-form-section">
      <div class="content-wrapper">
        <h2>{{ t("disputes.contact_form_title") }}</h2>
        <p>{{ t("disputes.contact_form_desc") }}</p>

        <form @submit.prevent="handleSubmit" class="dispute-form" novalidate>
          <!-- Order Number -->
          <div class="form-group">
            <label for="orderNumber">{{ t("disputes.order_number") }}</label>
            <input
              id="orderNumber"
              v-model="form.orderNumber"
              type="text"
              class="form-input"
              :placeholder="t('disputes.order_number_placeholder')"
              required
              @blur="validateField('orderNumber')"
            />
            <span v-if="errors.orderNumber" class="error-message">
              {{ errors.orderNumber }}
            </span>
          </div>

          <!-- Email -->
          <div class="form-group">
            <label for="email">{{ t("disputes.email") }}</label>
            <input
              id="email"
              v-model="form.email"
              type="email"
              class="form-input"
              :placeholder="t('disputes.email_placeholder')"
              required
              @blur="validateField('email')"
            />
            <span v-if="errors.email" class="error-message">
              {{ errors.email }}
            </span>
          </div>

          <!-- Full Name -->
          <div class="form-group">
            <label for="name">{{ t("disputes.full_name") }}</label>
            <input
              id="name"
              v-model="form.name"
              type="text"
              class="form-input"
              :placeholder="t('disputes.name_placeholder')"
              required
              @blur="validateField('name')"
            />
            <span v-if="errors.name" class="error-message">
              {{ errors.name }}
            </span>
          </div>

          <!-- Dispute Description -->
          <div class="form-group">
            <label for="description">{{
              t("disputes.dispute_description")
            }}</label>
            <textarea
              id="description"
              v-model="form.description"
              class="form-textarea"
              :placeholder="t('disputes.description_placeholder')"
              rows="6"
              required
              @blur="validateField('description')"
            />
            <span v-if="errors.description" class="error-message">
              {{ errors.description }}
            </span>
          </div>

          <!-- Dispute Type -->
          <div class="form-group">
            <label for="type">{{ t("disputes.dispute_type") }}</label>
            <select
              id="type"
              v-model="form.type"
              class="form-select"
              required
              @blur="validateField('type')"
            >
              <option value="">{{ t("disputes.select_type") }}</option>
              <option value="product_quality">
                {{ t("disputes.type_product_quality") }}
              </option>
              <option value="delivery">
                {{ t("disputes.type_delivery") }}
              </option>
              <option value="payment">{{ t("disputes.type_payment") }}</option>
              <option value="return">{{ t("disputes.type_return") }}</option>
              <option value="other">{{ t("disputes.type_other") }}</option>
            </select>
            <span v-if="errors.type" class="error-message">
              {{ errors.type }}
            </span>
          </div>

          <!-- Consent Checkbox -->
          <div class="form-group checkbox">
            <input
              id="consent"
              v-model="form.consent"
              type="checkbox"
              required
              @blur="validateField('consent')"
            />
            <label for="consent">
              {{ t("disputes.consent_text") }}
            </label>
            <span v-if="errors.consent" class="error-message">
              {{ errors.consent }}
            </span>
          </div>

          <!-- Submit Button -->
          <button
            type="submit"
            class="submit-button"
            :disabled="isSubmitting"
            :aria-busy="isSubmitting"
          >
            <span v-if="!isSubmitting">{{ t("disputes.submit_button") }}</span>
            <span v-else>{{ t("disputes.submitting") }}</span>
          </button>

          <!-- Success Message -->
          <div v-if="submitSuccess" class="success-message" role="alert">
            {{ t("disputes.submit_success") }}
          </div>

          <!-- Error Message -->
          <div v-if="submitError" class="error-message-box" role="alert">
            {{ submitError }}
          </div>
        </form>
      </div>
    </section>

    <!-- FAQ Section -->
    <section class="faq-section">
      <div class="content-wrapper">
        <h2>{{ t("disputes.faq_title") }}</h2>

        <div class="faq-items">
          <div v-for="(faq, index) in faqs" :key="index" class="faq-item">
            <button
              :id="`faq-${index}`"
              class="faq-header"
              @click="toggleFaq(index)"
              :aria-expanded="expandedFaq[index]"
              :aria-controls="`faq-content-${index}`"
            >
              <span class="faq-question">{{ faq.q }}</span>
              <span class="faq-toggle">{{
                expandedFaq[index] ? "−" : "+"
              }}</span>
            </button>
            <div
              v-show="expandedFaq[index]"
              :id="`faq-content-${index}`"
              class="faq-content"
            >
              <p>{{ faq.a }}</p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Alternative Contact -->
    <section class="contact-section">
      <div class="content-wrapper">
        <h2>{{ t("disputes.direct_contact_title") }}</h2>
        <p>{{ t("disputes.direct_contact_desc") }}</p>
        <a :href="`mailto:${supportEmail}`" class="contact-email">
          {{ supportEmail }}
        </a>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from "vue";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

// Support email (should come from config/store settings)
const supportEmail = "disputes@example.com";

// Form state
const form = reactive({
  orderNumber: "",
  email: "",
  name: "",
  description: "",
  type: "",
  consent: false,
});

const errors = reactive<Record<string, string>>({});
const isSubmitting = ref(false);
const submitSuccess = ref(false);
const submitError = ref("");

// FAQ state
const expandedFaq = reactive<Record<number, boolean>>({});

const faqs = [
  {
    q: "disputes.faq_1_q",
    a: "disputes.faq_1_a",
  },
  {
    q: "disputes.faq_2_q",
    a: "disputes.faq_2_a",
  },
  {
    q: "disputes.faq_3_q",
    a: "disputes.faq_3_a",
  },
  {
    q: "disputes.faq_4_q",
    a: "disputes.faq_4_a",
  },
];

// Validation
function validateField(field: string): void {
  delete errors[field];

  switch (field) {
    case "orderNumber":
      if (!form.orderNumber.trim()) {
        errors.orderNumber = t("disputes.error_order_required");
      }
      break;
    case "email":
      if (!form.email.trim()) {
        errors.email = t("disputes.error_email_required");
      } else if (!isValidEmail(form.email)) {
        errors.email = t("disputes.error_email_invalid");
      }
      break;
    case "name":
      if (!form.name.trim()) {
        errors.name = t("disputes.error_name_required");
      } else if (form.name.trim().length < 2) {
        errors.name = t("disputes.error_name_too_short");
      }
      break;
    case "description":
      if (!form.description.trim()) {
        errors.description = t("disputes.error_description_required");
      } else if (form.description.trim().length < 10) {
        errors.description = t("disputes.error_description_too_short");
      }
      break;
    case "type":
      if (!form.type) {
        errors.type = t("disputes.error_type_required");
      }
      break;
    case "consent":
      if (!form.consent) {
        errors.consent = t("disputes.error_consent_required");
      }
      break;
  }
}

function isValidEmail(email: string): boolean {
  const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return re.test(email);
}

function validateForm(): boolean {
  Object.keys(form).forEach((key) => validateField(key));
  return Object.keys(errors).length === 0;
}

async function handleSubmit(): Promise<void> {
  if (!validateForm()) {
    return;
  }

  isSubmitting.value = true;
  submitSuccess.value = false;
  submitError.value = "";

  try {
    // TODO: Send form data to backend API
    // const response = await disputeService.submitDispute(form)

    // Simulate API call
    await new Promise((resolve) => setTimeout(resolve, 1000));

    submitSuccess.value = true;

    // Reset form
    form.orderNumber = "";
    form.email = "";
    form.name = "";
    form.description = "";
    form.type = "";
    form.consent = false;

    // Hide success message after 5 seconds
    setTimeout(() => {
      submitSuccess.value = false;
    }, 5000);
  } catch (error) {
    submitError.value = t("disputes.submit_error");
    console.error("Error submitting dispute:", error);
  } finally {
    isSubmitting.value = false;
  }
}

function toggleFaq(index: number): void {
  expandedFaq[index] = !expandedFaq[index];
}
</script>

<style scoped>
.dispute-resolution-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
}

/* Hero Section */
.hero-section {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 4rem 2rem;
  text-align: center;
}

.hero-section h1 {
  font-size: 2.5rem;
  margin: 0 0 1rem 0;
  font-weight: 700;
}

.hero-section .subtitle {
  font-size: 1.1rem;
  opacity: 0.95;
  max-width: 600px;
  margin: 0 auto;
}

/* Content Wrapper */
.content-wrapper {
  max-width: 1000px;
  margin: 0 auto;
  padding: 3rem 2rem;
}

/* ODR Info Section */
.odr-info-section {
  background: white;
  padding: 3rem 0;
  margin: 2rem 0;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.odr-info-section h2 {
  text-align: center;
  margin-bottom: 2.5rem;
  font-size: 1.8rem;
  color: #333;
}

.info-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 2rem;
  margin-bottom: 2rem;
}

.info-card {
  background: #f8f9fa;
  padding: 2rem;
  border-radius: 8px;
  border-left: 4px solid #667eea;
}

.info-card h3 {
  color: #333;
  margin: 0 0 1rem 0;
  font-size: 1.2rem;
}

.info-card p,
.info-card li {
  color: #666;
  line-height: 1.6;
  margin: 0.5rem 0;
}

.info-card ul,
.info-card ol {
  margin: 1rem 0;
  padding-left: 1.5rem;
}

/* ODR Platform Box */
.odr-platform-box {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 2.5rem;
  border-radius: 8px;
  text-align: center;
  margin-top: 2rem;
}

.odr-platform-box h3 {
  margin: 0 0 1rem 0;
  font-size: 1.4rem;
}

.odr-platform-box p {
  margin: 1rem 0;
  opacity: 0.95;
}

.odr-link {
  display: inline-block;
  margin-top: 1.5rem;
  padding: 0.75rem 2rem;
  background: white;
  color: #667eea;
  text-decoration: none;
  border-radius: 4px;
  font-weight: 600;
  transition: all 0.3s ease;
  cursor: pointer;
}

.odr-link:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.odr-link .icon {
  margin-left: 0.5rem;
}

/* Contact Form Section */
.contact-form-section {
  background: white;
  padding: 3rem 0;
  margin: 2rem 0;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.contact-form-section h2 {
  text-align: center;
  margin-bottom: 1rem;
  font-size: 1.8rem;
  color: #333;
}

.contact-form-section > .content-wrapper > p {
  text-align: center;
  color: #666;
  margin-bottom: 2rem;
}

.dispute-form {
  max-width: 600px;
  margin: 0 auto;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  color: #333;
  font-weight: 500;
}

.form-input,
.form-select,
.form-textarea {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-family: inherit;
  font-size: 1rem;
  transition: border-color 0.3s;
}

.form-input:focus,
.form-select:focus,
.form-textarea:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.form-textarea {
  resize: vertical;
}

.form-group.checkbox {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
}

.form-group.checkbox input[type="checkbox"] {
  width: auto;
  margin-top: 0.25rem;
  cursor: pointer;
}

.form-group.checkbox label {
  margin: 0;
  cursor: pointer;
  color: #666;
}

.error-message {
  display: block;
  color: #e74c3c;
  font-size: 0.875rem;
  margin-top: 0.25rem;
}

.error-message-box {
  background: #fadbd8;
  color: #c0392b;
  padding: 1rem;
  border-radius: 4px;
  margin-top: 1rem;
  border-left: 4px solid #c0392b;
}

.success-message {
  background: #d4edda;
  color: #155724;
  padding: 1rem;
  border-radius: 4px;
  margin-top: 1rem;
  border-left: 4px solid #28a745;
}

.submit-button {
  width: 100%;
  padding: 0.875rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  margin-top: 1rem;
}

.submit-button:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
}

.submit-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* FAQ Section */
.faq-section {
  background: white;
  padding: 3rem 0;
  margin: 2rem 0;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.faq-section h2 {
  text-align: center;
  margin-bottom: 2rem;
  font-size: 1.8rem;
  color: #333;
}

.faq-items {
  max-width: 700px;
  margin: 0 auto;
}

.faq-item {
  margin-bottom: 1rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  overflow: hidden;
}

.faq-header {
  width: 100%;
  padding: 1rem;
  background: #f8f9fa;
  border: none;
  cursor: pointer;
  display: flex;
  justify-content: space-between;
  align-items: center;
  transition: background-color 0.3s;
  font-size: 1rem;
}

.faq-header:hover {
  background: #e9ecef;
}

.faq-question {
  text-align: left;
  color: #333;
  font-weight: 500;
}

.faq-toggle {
  color: #667eea;
  font-size: 1.5rem;
  font-weight: 300;
}

.faq-content {
  padding: 1rem;
  background: white;
}

.faq-content p {
  color: #666;
  margin: 0;
  line-height: 1.6;
}

/* Contact Section */
.contact-section {
  background: #f8f9fa;
  padding: 3rem 0;
  margin: 2rem 0;
  text-align: center;
}

.contact-section h2 {
  font-size: 1.8rem;
  color: #333;
  margin-bottom: 1rem;
}

.contact-section p {
  color: #666;
  margin-bottom: 1.5rem;
}

.contact-email {
  display: inline-block;
  padding: 0.75rem 2rem;
  background: #667eea;
  color: white;
  text-decoration: none;
  border-radius: 4px;
  font-weight: 600;
  transition: all 0.3s ease;
}

.contact-email:hover {
  background: #764ba2;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
}

/* Responsive Design */
@media (max-width: 768px) {
  .hero-section h1 {
    font-size: 1.8rem;
  }

  .hero-section .subtitle {
    font-size: 1rem;
  }

  .info-cards {
    grid-template-columns: 1fr;
  }

  .odr-platform-box {
    padding: 1.5rem;
  }

  .content-wrapper {
    padding: 2rem 1rem;
  }

  .faq-header {
    padding: 0.75rem;
  }
}
</style>
