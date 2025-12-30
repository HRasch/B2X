<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import { useRoute } from "vue-router";

interface InvoiceLineItem {
  id: string;
  productSku: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  lineSubTotal: number;
  lineTaxRate: number;
  lineTaxAmount: number;
  lineTotal: number;
}

interface Invoice {
  id: string;
  invoiceNumber: string;
  status: "Draft" | "Issued" | "Paid" | "Cancelled";
  issuedAt: string;
  dueAt: string;

  // Seller (B2Connect)
  sellerName: string;
  sellerVatId: string;
  sellerAddress: string;

  // Buyer
  buyerName: string;
  buyerVatId?: string;
  buyerAddress: string;
  buyerCountry?: string;

  // Pricing
  lineItems: InvoiceLineItem[];
  subTotal: number;
  taxAmount: number;
  taxRate: number;
  shippingCost: number;
  shippingTaxAmount: number;
  total: number;

  // Reverse Charge (Issue #32)
  reverseChargeApplies: boolean;
  reverseChargeNote?: string;

  // Payment
  paymentMethod: string;
  paymentStatus: string;
  paidAt?: string;

  // Audit
  createdAt: string;
  createdBy: string;
}

const props = defineProps<{
  invoiceId?: string;
  invoiceData?: Invoice;
}>();

const emit = defineEmits<{
  (e: "modify", invoiceId: string): void;
  (e: "download-pdf", invoiceId: string): void;
  (e: "send-email", invoiceId: string, email: string): void;
}>();

const loading = ref(false);
const error = ref<string | null>(null);
const invoice = ref<Invoice | null>(null);

// Computed properties
const formattedIssuedAt = computed(() => {
  if (!invoice.value) return "";
  return new Date(invoice.value.issuedAt).toLocaleDateString("de-DE", {
    year: "numeric",
    month: "long",
    day: "numeric",
  });
});

const formattedDueAt = computed(() => {
  if (!invoice.value) return "";
  return new Date(invoice.value.dueAt).toLocaleDateString("de-DE", {
    year: "numeric",
    month: "long",
    day: "numeric",
  });
});

const isB2b = computed(
  () => invoice.value?.buyerVatId && invoice.value.buyerVatId.length > 0
);

const isBOverdue = computed(() => {
  if (!invoice.value) return false;
  const dueDate = new Date(invoice.value.dueAt);
  return dueDate < new Date();
});

const statusBadgeClass = computed(() => {
  switch (invoice.value?.status) {
    case "Paid":
      return "bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200";
    case "Cancelled":
      return "bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200";
    case "Draft":
      return "bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-200";
    case "Issued":
      return "bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200";
    default:
      return "bg-gray-100 text-gray-800";
  }
});

// Methods
const fetchInvoice = async () => {
  if (!props.invoiceId) return;

  loading.value = true;
  error.value = null;

  try {
    const response = await fetch(`/api/invoices/${props.invoiceId}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("authToken")}`,
      },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data = await response.json();
    invoice.value = data.data || data;
  } catch (err) {
    error.value = err instanceof Error ? err.message : "Failed to load invoice";
    console.error("Failed to fetch invoice:", err);
  } finally {
    loading.value = false;
  }
};

const handleModify = () => {
  if (invoice.value) {
    emit("modify", invoice.value.id);
  }
};

const handleDownloadPdf = () => {
  if (invoice.value) {
    emit("download-pdf", invoice.value.id);
  }
};

const handleSendEmail = () => {
  if (invoice.value && invoice.value.buyerVatId) {
    emit("send-email", invoice.value.id, invoice.value.buyerVatId);
  }
};

// Lifecycle
onMounted(() => {
  if (props.invoiceData) {
    invoice.value = props.invoiceData;
  } else if (props.invoiceId) {
    fetchInvoice();
  }
});
</script>

<template>
  <div class="invoice-display">
    <!-- Loading State -->
    <div v-if="loading" class="text-center py-12">
      <div class="inline-flex items-center gap-2">
        <div class="animate-spin">
          <svg
            class="w-5 h-5 text-blue-500"
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
        <span class="text-gray-600 dark:text-gray-400">Loading invoice...</span>
      </div>
    </div>

    <!-- Error State -->
    <div
      v-else-if="error"
      class="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg p-4"
    >
      <h3 class="text-red-800 dark:text-red-300 font-semibold mb-1">
        Error Loading Invoice
      </h3>
      <p class="text-red-700 dark:text-red-400 text-sm">{{ error }}</p>
      <button
        v-if="invoiceId"
        @click="fetchInvoice"
        class="mt-3 px-4 py-2 bg-red-600 hover:bg-red-700 text-white text-sm rounded transition-colors"
      >
        Retry
      </button>
    </div>

    <!-- Invoice Content -->
    <div v-else-if="invoice" class="space-y-6 print:space-y-4">
      <!-- Header -->
      <div
        class="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-4 pb-6 border-b border-gray-200 dark:border-gray-700"
      >
        <div>
          <h1 class="text-3xl font-bold text-gray-900 dark:text-white">
            Invoice
          </h1>
          <p class="text-gray-600 dark:text-gray-400 mt-1">
            {{ invoice.invoiceNumber }}
          </p>
        </div>

        <div class="flex flex-col items-start sm:items-end gap-3">
          <!-- Status Badge -->
          <span
            :class="[
              'px-3 py-1 rounded-full text-sm font-medium',
              statusBadgeClass,
            ]"
            :aria-label="`Invoice status: ${invoice.status}`"
          >
            {{ invoice.status }}
          </span>

          <!-- Reverse Charge Badge (Issue #32) -->
          <span
            v-if="isB2b && invoice.reverseChargeApplies"
            class="px-3 py-1 rounded-full text-sm font-medium bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200"
            aria-label="Reverse Charge: 0% VAT applies"
          >
            ⚠️ Reverse Charge (0% VAT)
          </span>

          <!-- Overdue Badge -->
          <span
            v-if="
              isBOverdue &&
              invoice.status !== 'Paid' &&
              invoice.status !== 'Cancelled'
            "
            class="px-3 py-1 rounded-full text-sm font-medium bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200"
            aria-label="Invoice is overdue"
          >
            Overdue
          </span>
        </div>
      </div>

      <!-- Dates -->
      <div class="grid grid-cols-2 gap-4 sm:grid-cols-4">
        <div>
          <p class="text-sm font-medium text-gray-600 dark:text-gray-400">
            Issued
          </p>
          <p class="text-lg font-semibold text-gray-900 dark:text-white">
            {{ formattedIssuedAt }}
          </p>
        </div>
        <div>
          <p class="text-sm font-medium text-gray-600 dark:text-gray-400">
            Due
          </p>
          <p class="text-lg font-semibold text-gray-900 dark:text-white">
            {{ formattedDueAt }}
          </p>
        </div>
        <div v-if="invoice.paymentStatus">
          <p class="text-sm font-medium text-gray-600 dark:text-gray-400">
            Payment
          </p>
          <p class="text-lg font-semibold text-gray-900 dark:text-white">
            {{ invoice.paymentStatus }}
          </p>
        </div>
        <div v-if="invoice.paidAt">
          <p class="text-sm font-medium text-gray-600 dark:text-gray-400">
            Paid On
          </p>
          <p class="text-lg font-semibold text-gray-900 dark:text-white">
            {{ new Date(invoice.paidAt).toLocaleDateString("de-DE") }}
          </p>
        </div>
      </div>

      <!-- Addresses -->
      <div
        class="grid grid-cols-1 sm:grid-cols-2 gap-6 py-6 border-t border-b border-gray-200 dark:border-gray-700"
      >
        <!-- Seller -->
        <div>
          <h3
            class="text-sm font-bold text-gray-600 dark:text-gray-400 uppercase tracking-wide mb-2"
          >
            From
          </h3>
          <div class="text-gray-900 dark:text-white space-y-1">
            <p class="font-semibold">{{ invoice.sellerName }}</p>
            <p v-if="invoice.sellerVatId" class="text-sm">
              VAT ID: {{ invoice.sellerVatId }}
            </p>
            <p v-if="invoice.sellerAddress" class="text-sm whitespace-pre-line">
              {{ invoice.sellerAddress }}
            </p>
          </div>
        </div>

        <!-- Buyer -->
        <div>
          <h3
            class="text-sm font-bold text-gray-600 dark:text-gray-400 uppercase tracking-wide mb-2"
          >
            Bill To
          </h3>
          <div class="text-gray-900 dark:text-white space-y-1">
            <p class="font-semibold">{{ invoice.buyerName }}</p>
            <p v-if="isB2b && invoice.buyerVatId" class="text-sm font-medium">
              VAT ID: {{ invoice.buyerVatId }}
              <span
                v-if="invoice.reverseChargeApplies"
                class="text-yellow-600 dark:text-yellow-400 ml-2"
              >
                (Reverse Charge)
              </span>
            </p>
            <p v-if="invoice.buyerCountry" class="text-sm">
              Country: {{ invoice.buyerCountry }}
            </p>
            <p v-if="invoice.buyerAddress" class="text-sm whitespace-pre-line">
              {{ invoice.buyerAddress }}
            </p>
          </div>
        </div>
      </div>

      <!-- Line Items Table -->
      <div class="overflow-x-auto">
        <table
          class="w-full text-sm"
          role="table"
          aria-label="Invoice line items"
        >
          <thead class="bg-gray-100 dark:bg-gray-800">
            <tr>
              <th
                class="px-4 py-3 text-left font-semibold text-gray-900 dark:text-white"
              >
                Product
              </th>
              <th
                class="px-4 py-3 text-right font-semibold text-gray-900 dark:text-white"
              >
                Qty
              </th>
              <th
                class="px-4 py-3 text-right font-semibold text-gray-900 dark:text-white"
              >
                Unit Price
              </th>
              <th
                class="px-4 py-3 text-right font-semibold text-gray-900 dark:text-white"
              >
                Subtotal
              </th>
              <th
                v-if="!invoice.reverseChargeApplies"
                class="px-4 py-3 text-right font-semibold text-gray-900 dark:text-white"
              >
                Tax
              </th>
              <th
                class="px-4 py-3 text-right font-semibold text-gray-900 dark:text-white"
              >
                Total
              </th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
            <tr
              v-for="item in invoice.lineItems"
              :key="item.id"
              class="hover:bg-gray-50 dark:hover:bg-gray-800/50"
            >
              <td class="px-4 py-3 text-gray-900 dark:text-white">
                <div class="font-medium">{{ item.productName }}</div>
                <div class="text-xs text-gray-600 dark:text-gray-400">
                  SKU: {{ item.productSku }}
                </div>
              </td>
              <td class="px-4 py-3 text-right text-gray-900 dark:text-white">
                {{ item.quantity }}
              </td>
              <td class="px-4 py-3 text-right text-gray-900 dark:text-white">
                €{{ item.unitPrice.toFixed(2) }}
              </td>
              <td class="px-4 py-3 text-right text-gray-900 dark:text-white">
                €{{ item.lineSubTotal.toFixed(2) }}
              </td>
              <td
                v-if="!invoice.reverseChargeApplies"
                class="px-4 py-3 text-right text-gray-900 dark:text-white"
              >
                <div>€{{ item.lineTaxAmount.toFixed(2) }}</div>
                <div class="text-xs text-gray-600 dark:text-gray-400">
                  {{ (item.lineTaxRate * 100).toFixed(0) }}%
                </div>
              </td>
              <td
                class="px-4 py-3 text-right font-semibold text-gray-900 dark:text-white"
              >
                €{{ item.lineTotal.toFixed(2) }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Totals -->
      <div class="flex justify-end py-6">
        <div
          class="w-full sm:w-80 space-y-2 border-t border-gray-200 dark:border-gray-700 pt-6"
        >
          <div class="flex justify-between">
            <span class="text-gray-600 dark:text-gray-400">Subtotal:</span>
            <span class="font-semibold text-gray-900 dark:text-white"
              >€{{ invoice.subTotal.toFixed(2) }}</span
            >
          </div>

          <div v-if="invoice.shippingCost > 0" class="flex justify-between">
            <span class="text-gray-600 dark:text-gray-400">Shipping:</span>
            <span class="font-semibold text-gray-900 dark:text-white"
              >€{{ invoice.shippingCost.toFixed(2) }}</span
            >
          </div>

          <!-- Tax or Reverse Charge Row -->
          <div
            v-if="!invoice.reverseChargeApplies"
            class="flex justify-between bg-blue-50 dark:bg-blue-900/20 px-3 py-2 rounded"
          >
            <span class="text-gray-600 dark:text-gray-400">
              VAT ({{ (invoice.taxRate * 100).toFixed(0) }}%):
            </span>
            <span class="font-semibold text-gray-900 dark:text-white"
              >€{{ invoice.taxAmount.toFixed(2) }}</span
            >
          </div>

          <div
            v-else
            class="flex justify-between bg-yellow-50 dark:bg-yellow-900/20 px-3 py-2 rounded border border-yellow-200 dark:border-yellow-800"
          >
            <span class="text-yellow-800 dark:text-yellow-200">
              <strong>Reverse Charge (0% VAT):</strong>
            </span>
            <span class="font-semibold text-yellow-800 dark:text-yellow-200"
              >€0.00</span
            >
          </div>

          <div
            class="flex justify-between text-lg border-t border-gray-200 dark:border-gray-700 pt-2 mt-4"
          >
            <span class="font-bold text-gray-900 dark:text-white">Total:</span>
            <span class="font-bold text-gray-900 dark:text-white"
              >€{{ invoice.total.toFixed(2) }}</span
            >
          </div>
        </div>
      </div>

      <!-- Reverse Charge Notice (Issue #32) -->
      <div
        v-if="invoice.reverseChargeApplies && invoice.reverseChargeNote"
        class="bg-yellow-50 dark:bg-yellow-900/20 border-l-4 border-yellow-400 p-4 rounded"
        role="region"
        aria-label="Reverse Charge Notice"
      >
        <div class="flex gap-3">
          <div
            class="text-yellow-600 dark:text-yellow-400 flex-shrink-0 pt-0.5"
          >
            <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
              <path
                fill-rule="evenodd"
                d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z"
                clip-rule="evenodd"
              />
            </svg>
          </div>
          <div class="text-yellow-800 dark:text-yellow-200 text-sm">
            <p class="font-semibold mb-1">{{ invoice.reverseChargeNote }}</p>
            <p>
              The VAT shown above does not apply. As a taxable person, you are
              responsible for paying VAT according to applicable rules in your
              country.
            </p>
          </div>
        </div>
      </div>

      <!-- Payment Terms -->
      <div
        v-if="invoice.paymentMethod"
        class="bg-gray-50 dark:bg-gray-800 p-4 rounded-lg"
      >
        <h4 class="font-semibold text-gray-900 dark:text-white mb-2">
          Payment Information
        </h4>
        <p class="text-gray-600 dark:text-gray-400 text-sm">
          <strong>Method:</strong> {{ invoice.paymentMethod }}
        </p>
      </div>

      <!-- Actions -->
      <div
        class="flex flex-col sm:flex-row gap-3 pt-6 border-t border-gray-200 dark:border-gray-700 print:hidden"
      >
        <button
          @click="handleDownloadPdf"
          class="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition-colors font-medium flex items-center justify-center gap-2"
          aria-label="Download invoice as PDF"
        >
          <svg
            class="w-4 h-4"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4"
            ></path>
          </svg>
          Download PDF
        </button>

        <button
          @click="handleSendEmail"
          v-if="invoice.buyerVatId"
          class="px-4 py-2 bg-green-600 hover:bg-green-700 text-white rounded-lg transition-colors font-medium flex items-center justify-center gap-2"
          aria-label="Send invoice via email"
        >
          <svg
            class="w-4 h-4"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"
            ></path>
          </svg>
          Send Email
        </button>

        <button
          v-if="invoice.status !== 'Paid' && invoice.status !== 'Cancelled'"
          @click="handleModify"
          class="px-4 py-2 bg-gray-600 hover:bg-gray-700 text-white rounded-lg transition-colors font-medium flex items-center justify-center gap-2"
          aria-label="Modify invoice details"
        >
          <svg
            class="w-4 h-4"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"
            ></path>
          </svg>
          Modify
        </button>

        <button
          @click="window.print()"
          class="px-4 py-2 bg-gray-100 hover:bg-gray-200 dark:bg-gray-700 dark:hover:bg-gray-600 text-gray-900 dark:text-white rounded-lg transition-colors font-medium flex items-center justify-center gap-2"
          aria-label="Print invoice"
        >
          <svg
            class="w-4 h-4"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M17 17h2a2 2 0 002-2v-4a2 2 0 00-2-2H5a2 2 0 00-2 2v4a2 2 0 002 2h2m2 4H7a2 2 0 01-2-2v-4a2 2 0 012-2h10a2 2 0 012 2v4a2 2 0 01-2 2zm-6-4a2 2 0 100-4 2 2 0 000 4z"
            ></path>
          </svg>
          Print
        </button>
      </div>
    </div>

    <!-- Empty State -->
    <div v-else class="text-center py-12">
      <p class="text-gray-600 dark:text-gray-400">No invoice to display</p>
    </div>
  </div>
</template>

<style scoped>
@media print {
  .invoice-display {
    background: white;
    padding: 0;
  }
}
</style>
