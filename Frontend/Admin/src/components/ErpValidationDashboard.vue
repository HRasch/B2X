<!-- filepath: src/components/admin/ErpValidationDashboard.vue -->
<template>
  <div class="erp-validation-dashboard">
    <h2>{{ $t('admin.erpValidation.title') }}</h2>

    <div class="validation-controls">
      <select v-model="selectedErpType" @change="loadValidationRules">
        <option value="enventa">{{ $t('erp.enventa') }}</option>
        <option value="sap">{{ $t('erp.sap') }}</option>
        <!-- Additional ERP types -->
      </select>

      <button @click="runValidation" :disabled="isValidating">
        {{ isValidating ? $t('common.validating') : $t('admin.erpValidation.runValidation') }}
      </button>

      <button @click="runConnectorValidation" :disabled="isValidating">
        {{ $t('admin.erpValidation.runConnectorValidation') }}
      </button>
    </div>

    <div v-if="validationResults.length > 0" class="results-section">
      <h3>{{ $t('admin.erpValidation.results') }}</h3>
      <div
        v-for="result in validationResults"
        :key="result.id"
        :class="['result-item', result.severity.toLowerCase()]"
      >
        <span>{{ result.message }}</span>
        <small>{{ result.fieldPath }}</small>
      </div>
    </div>

    <QuarantineManager
      v-if="quarantinedRecords.length > 0"
      :records="quarantinedRecords"
      @resolve="handleQuarantineResolution"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import QuarantineManager from './QuarantineManager.vue';

const { t } = useI18n();
const selectedErpType = ref('enventa');
const validationResults = ref([]);
const quarantinedRecords = ref([]);
const isValidating = ref(false);

const loadValidationRules = async () => {
  const response = await fetch(`/api/validation/rules/${selectedErpType.value}`);
  // Load ERP-specific rules
};

const runValidation = async () => {
  isValidating.value = true;
  try {
    const response = await fetch('/api/validation/erp/run', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ erpType: selectedErpType.value }),
    });
    const results = await response.json();
    validationResults.value = results;
    quarantinedRecords.value = results.filter(r => r.quarantined);
  } finally {
    isValidating.value = false;
  }
};

const runConnectorValidation = async () => {
  isValidating.value = true;
  try {
    const response = await fetch(`/api/connectors/${selectedErpType.value}/validate`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ data: {} }), // Sample data
    });
    const results = await response.json();
    validationResults.value = results;
  } finally {
    isValidating.value = false;
  }
};

const handleQuarantineResolution = resolution => {
  // Handle resolution
};
</script>
