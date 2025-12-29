<template>
  <div class="checkout-terms-step">
    <div class="step-header">
      <h2>Bedingungen</h2>
      <p class="step-description">
        Bitte akzeptieren Sie die erforderlichen Bedingungen, um fortzufahren
      </p>
    </div>

    <div class="terms-container">
      <!-- Terms & Conditions Checkbox -->
      <div class="terms-checkbox-group">
        <label for="terms-checkbox" class="checkbox-label">
          <input
            id="terms-checkbox"
            v-model="acceptedTerms.termsAndConditions"
            type="checkbox"
            class="checkbox-input"
            :disabled="isSubmitting"
            aria-label="Ich akzeptiere die Allgemeinen Geschäftsbedingungen"
          />
          <span class="checkbox-text">
            Ich akzeptiere die
            <button
              type="button"
              @click="showTermsModal = true"
              class="document-link"
            >
              Allgemeinen Geschäftsbedingungen
            </button>
            *
          </span>
        </label>
      </div>

      <!-- Privacy Policy Checkbox -->
      <div class="terms-checkbox-group">
        <label for="privacy-checkbox" class="checkbox-label">
          <input
            id="privacy-checkbox"
            v-model="acceptedTerms.privacyPolicy"
            type="checkbox"
            class="checkbox-input"
            :disabled="isSubmitting"
            aria-label="Ich akzeptiere die Datenschutzerklärung"
          />
          <span class="checkbox-text">
            Ich akzeptiere die
            <button
              type="button"
              @click="showPrivacyModal = true"
              class="document-link"
            >
              Datenschutzerklärung
            </button>
            *
          </span>
        </label>
      </div>

      <!-- 14-Day Withdrawal Right (B2C only, optional for legal clarity) -->
      <div class="terms-checkbox-group">
        <label for="withdrawal-checkbox" class="checkbox-label">
          <input
            id="withdrawal-checkbox"
            v-model="acceptedTerms.withdrawalRight"
            type="checkbox"
            class="checkbox-input"
            :disabled="isSubmitting"
            aria-label="Ich habe mein Widerrufsrecht verstanden"
          />
          <span class="checkbox-text">
            Ich verstehe mein
            <button
              type="button"
              @click="showWithdrawalModal = true"
              class="document-link"
            >
              Widerrufsrecht (14 Tage)
            </button>
          </span>
        </label>
      </div>

      <!-- Required fields note -->
      <p class="required-note">* Erforderliche Felder</p>

      <!-- Error message -->
      <div v-if="errorMessage" class="error-message" role="alert">
        {{ errorMessage }}
      </div>

      <!-- Success message -->
      <div v-if="successMessage" class="success-message" role="status">
        {{ successMessage }}
      </div>
    </div>

    <!-- Action buttons -->
    <div class="step-actions">
      <button
        type="button"
        @click="goBack"
        class="btn-secondary"
        :disabled="isSubmitting"
      >
        Zurück
      </button>
      <button
        type="button"
        @click="continueToPayment"
        class="btn-primary"
        :disabled="!canContinue || isSubmitting"
        :aria-busy="isSubmitting"
      >
        <span v-if="isSubmitting" class="spinner"></span>
        {{ isSubmitting ? "Wird verarbeitet..." : "Zur Zahlung" }}
      </button>
    </div>

    <!-- Legal Documents Modals -->
    <!-- Terms & Conditions Modal -->
    <div
      v-if="showTermsModal"
      class="modal-overlay"
      @click="showTermsModal = false"
    >
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>Allgemeine Geschäftsbedingungen</h3>
          <button
            type="button"
            @click="showTermsModal = false"
            class="close-button"
            aria-label="Modal schließen"
          >
            ✕
          </button>
        </div>
        <div class="modal-body">
          <div class="document-content">
            <h4>1. Allgemeine Bestimmungen</h4>
            <p>
              Diese Allgemeinen Geschäftsbedingungen regeln die Beziehung
              zwischen dem Betreiber dieses Online-Shops und dem Käufer.
            </p>

            <h4>2. Produktbeschreibungen</h4>
            <p>
              Alle Produktbeschreibungen sind Angebote zum Verkauf. Ein Vertrag
              kommt nur zustande, wenn Sie eine Bestellung aufgeben und wir
              diese akzeptieren.
            </p>

            <h4>3. Preise und Zahlungsbedingungen</h4>
            <p>
              Alle Preise enthalten die gültige Mehrwertsteuer. Versandkosten
              werden separat berechnet und beim Checkout angezeigt.
            </p>

            <h4>4. Lieferung</h4>
            <p>
              Lieferzeiten sind unverbindlich. Bei Verzug haften wir nur bei
              Verschulden.
            </p>

            <h4>5. Widerrufsrecht</h4>
            <p>
              Sie haben ein Widerrufsrecht von 14 Tagen ab Erhalt der Ware.
              Siehe unten für Details.
            </p>

            <h4>6. Haftung</h4>
            <p>
              Haftung für Schäden begrenzt auf Direktschäden bis zur Höhe des
              Kaufpreises.
            </p>

            <h4>7. Datenschutz</h4>
            <p>Siehe Datenschutzerklärung für die Behandlung Ihrer Daten.</p>

            <h4>8. Schlussbestimmungen</h4>
            <p>
              Es gilt deutsches Recht. Gerichtsstand ist der Sitz des
              Unternehmens.
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- Privacy Policy Modal -->
    <div
      v-if="showPrivacyModal"
      class="modal-overlay"
      @click="showPrivacyModal = false"
    >
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>Datenschutzerklärung</h3>
          <button
            type="button"
            @click="showPrivacyModal = false"
            class="close-button"
            aria-label="Modal schließen"
          >
            ✕
          </button>
        </div>
        <div class="modal-body">
          <div class="document-content">
            <h4>1. Verantwortlicher</h4>
            <p>
              Verantwortlich für die Datenverarbeitung ist der Betreiber dieses
              Shops (siehe Impressum).
            </p>

            <h4>2. Erhebung und Verarbeitung</h4>
            <p>
              Wir erheben Ihre Daten nur zur Abwicklung Ihres Einkaufs und zum
              Versand.
            </p>

            <h4>3. Speicherdauer</h4>
            <p>
              Persönliche Daten werden 10 Jahre zur Erfüllung von
              Steuerpflichten gespeichert.
            </p>

            <h4>4. Ihre Rechte</h4>
            <p>
              Sie haben das Recht auf Auskunft, Berichtigung, Löschung und
              Datenportabilität.
            </p>

            <h4>5. Cookies</h4>
            <p>
              Wir verwenden technisch notwendige Cookies. Andere Cookies werden
              mit Ihrer Einwilligung gespeichert.
            </p>

            <h4>6. Sicherheit</h4>
            <p>
              Wir schützen Ihre Daten durch Verschlüsselung und sichere
              Übertragung.
            </p>

            <h4>7. Datenschutzbeauftragter</h4>
            <p>Bei Fragen: datenschutz@example.com</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Withdrawal Right Modal -->
    <div
      v-if="showWithdrawalModal"
      class="modal-overlay"
      @click="showWithdrawalModal = false"
    >
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>Widerrufsrecht (14 Tage)</h3>
          <button
            type="button"
            @click="showWithdrawalModal = false"
            class="close-button"
            aria-label="Modal schließen"
          >
            ✕
          </button>
        </div>
        <div class="modal-body">
          <div class="document-content">
            <h4>Ihr Widerrufsrecht</h4>
            <p>
              Sie haben das Recht, Ihren Kauf innerhalb von 14 Tagen nach Erhalt
              der Ware zu widerrufen, ohne einen Grund angeben zu müssen.
            </p>

            <h4>Widerrufsfristen</h4>
            <ul>
              <li>Beginn: Tag nach Erhalt der Ware</li>
              <li>Dauer: 14 Tage</li>
              <li>Form: Einfache schriftliche Mitteilung per E-Mail genügt</li>
            </ul>

            <h4>Ausnahmen</h4>
            <p>Widerrufsrecht gilt NICHT für:</p>
            <ul>
              <li>Digitale Inhalte nach Abruf</li>
              <li>Maßgeschneiderte oder personalisierte Waren</li>
              <li>
                Waren, die nach Zustellung beschädigt wurden (Ihr Verschulden)
              </li>
            </ul>

            <h4>Rückgabeverfahren</h4>
            <p>
              Senden Sie die Ware unverzüglich zurück. Versandkosten trägt der
              Käufer (außer bei berechtigter Rückgabe).
            </p>

            <h4>Kontakt</h4>
            <p>Widerrufe richten Sie an: widerruf@example.com</p>

            <p class="legal-basis">
              <strong>Rechtsgrundlage:</strong> §§ 355-359 BGB
              (Fernabsatzgesetz)
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from "vue";

interface TermsAcceptance {
  termsAndConditions: boolean;
  privacyPolicy: boolean;
  withdrawalRight: boolean;
}

const emit = defineEmits<{
  (e: "continue"): void;
  (e: "back"): void;
}>();

// State management
const acceptedTerms = ref<TermsAcceptance>({
  termsAndConditions: false,
  privacyPolicy: false,
  withdrawalRight: false,
});

const isSubmitting = ref(false);
const errorMessage = ref("");
const successMessage = ref("");

const showTermsModal = ref(false);
const showPrivacyModal = ref(false);
const showWithdrawalModal = ref(false);

// Computed properties
const canContinue = computed(
  () =>
    acceptedTerms.value.termsAndConditions && acceptedTerms.value.privacyPolicy
);

// Methods
const continueToPayment = async () => {
  if (!canContinue.value) {
    errorMessage.value =
      "Bitte akzeptieren Sie die Allgemeinen Geschäftsbedingungen und Datenschutzerklärung";
    return;
  }

  isSubmitting.value = true;
  errorMessage.value = "";
  successMessage.value = "";

  try {
    // Submit acceptance to backend
    const response = await fetch(
      "http://localhost:7005/api/checkout/accept-terms",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          customerId: "customer-123", // Would come from auth context
          acceptTermsAndConditions: acceptedTerms.value.termsAndConditions,
          acceptPrivacyPolicy: acceptedTerms.value.privacyPolicy,
          acceptWithdrawalRight: acceptedTerms.value.withdrawalRight,
        }),
      }
    );

    if (!response.ok) {
      throw new Error("Fehler beim Speichern der Bedingungsannahme");
    }

    const data = await response.json();
    if (data.success) {
      successMessage.value = "Bedingungen akzeptiert!";
      setTimeout(() => {
        emit("continue");
      }, 500);
    } else {
      errorMessage.value = data.message || "Fehler beim Verarbeiten";
    }
  } catch (error) {
    errorMessage.value =
      "Ein Fehler ist aufgetreten. Bitte versuchen Sie es später erneut.";
    console.error("Terms acceptance error:", error);
  } finally {
    isSubmitting.value = false;
  }
};

const goBack = () => {
  emit("back");
};
</script>

<style scoped>
.checkout-terms-step {
  max-width: 800px;
  margin: 0 auto;
  padding: 2rem;
}

.step-header {
  margin-bottom: 2rem;
  border-bottom: 2px solid #e0e0e0;
  padding-bottom: 1rem;
}

.step-header h2 {
  margin: 0 0 0.5rem 0;
  color: #333;
  font-size: 1.8rem;
}

.step-description {
  margin: 0;
  color: #666;
  font-size: 0.95rem;
}

.terms-container {
  background-color: #f9f9f9;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 1.5rem;
  margin-bottom: 2rem;
}

.terms-checkbox-group {
  margin-bottom: 1.5rem;
  display: flex;
  align-items: flex-start;
}

.checkbox-label {
  display: flex;
  align-items: flex-start;
  cursor: pointer;
  user-select: none;
  gap: 0.75rem;
}

.checkbox-input {
  width: 20px;
  height: 20px;
  margin-top: 2px;
  cursor: pointer;
  accent-color: #2563eb;
  flex-shrink: 0;
}

.checkbox-input:disabled {
  cursor: not-allowed;
  opacity: 0.6;
}

.checkbox-input:focus {
  outline: 2px solid #2563eb;
  outline-offset: 2px;
}

.checkbox-text {
  color: #333;
  line-height: 1.5;
}

.document-link {
  background: none;
  border: none;
  color: #2563eb;
  text-decoration: underline;
  cursor: pointer;
  font-size: inherit;
  padding: 0;
  margin: 0;
  font-weight: 500;
}

.document-link:hover {
  color: #1d4ed8;
}

.document-link:focus {
  outline: 2px solid #2563eb;
  outline-offset: 2px;
}

.required-note {
  margin-top: 1rem;
  font-size: 0.85rem;
  color: #666;
  margin-bottom: 1rem;
}

.error-message {
  padding: 0.75rem;
  background-color: #ffebee;
  border: 1px solid #ef5350;
  border-radius: 4px;
  color: #c62828;
  font-size: 0.875rem;
  margin-top: 1rem;
}

.success-message {
  padding: 0.75rem;
  background-color: #e8f5e9;
  border: 1px solid #4caf50;
  border-radius: 4px;
  color: #2e7d32;
  font-size: 0.875rem;
  margin-top: 1rem;
}

.step-actions {
  display: flex;
  gap: 1rem;
  justify-content: space-between;
  margin-top: 2rem;
}

.btn-primary,
.btn-secondary {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 4px;
  font-weight: 600;
  cursor: pointer;
  font-size: 1rem;
  transition: background-color 0.3s;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.btn-primary {
  background-color: #059669;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background-color: #047857;
}

.btn-primary:disabled {
  background-color: #cccccc;
  cursor: not-allowed;
}

.btn-secondary {
  background-color: #f3f4f6;
  color: #333;
  border: 1px solid #d1d5db;
}

.btn-secondary:hover:not(:disabled) {
  background-color: #e5e7eb;
}

.btn-secondary:disabled {
  cursor: not-allowed;
  opacity: 0.6;
}

.spinner {
  display: inline-block;
  width: 16px;
  height: 16px;
  border: 2px solid #f3f3f3;
  border-top: 2px solid #2563eb;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 1rem;
}

.modal-content {
  background-color: white;
  border-radius: 8px;
  max-width: 600px;
  max-height: 80vh;
  overflow-y: auto;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.3);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid #e0e0e0;
  position: sticky;
  top: 0;
  background-color: white;
}

.modal-header h3 {
  margin: 0;
  color: #333;
  font-size: 1.3rem;
}

.close-button {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #666;
  padding: 0;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.close-button:hover {
  color: #333;
}

.modal-body {
  padding: 1.5rem;
}

.document-content {
  line-height: 1.6;
  color: #333;
}

.document-content h4 {
  margin: 1.5rem 0 0.75rem 0;
  color: #2563eb;
  font-size: 1.05rem;
}

.document-content p {
  margin: 0.75rem 0;
  font-size: 0.95rem;
}

.document-content ul {
  margin: 0.75rem 0;
  padding-left: 1.5rem;
}

.document-content li {
  margin: 0.5rem 0;
}

.legal-basis {
  margin-top: 1.5rem;
  padding-top: 1rem;
  border-top: 1px solid #e0e0e0;
  font-size: 0.85rem;
  color: #666;
}

/* Mobile Responsiveness */
@media (max-width: 768px) {
  .checkout-terms-step {
    padding: 1rem;
  }

  .step-header h2 {
    font-size: 1.5rem;
  }

  .modal-overlay {
    padding: 0;
  }

  .modal-content {
    max-width: 100%;
    max-height: 100vh;
    border-radius: 0;
  }

  .step-actions {
    flex-direction: column-reverse;
  }

  .btn-primary,
  .btn-secondary {
    width: 100%;
    justify-content: center;
  }
}
</style>
