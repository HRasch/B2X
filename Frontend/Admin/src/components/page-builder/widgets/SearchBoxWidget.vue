<script setup lang="ts">
/**
 * SearchBoxWidget - Product search with suggestions
 */
import { ref, computed } from 'vue';
import type { SearchBoxWidgetConfig } from '@/types/widgets';

const props = defineProps<{
  config: SearchBoxWidgetConfig;
  isEditing?: boolean;
}>();

const searchQuery = ref('');
const showDropdown = ref(false);

// Mock data for preview
const mockSuggestions = [
  {
    id: '1',
    type: 'product',
    name: 'Akku-Bohrschrauber 18V',
    price: 199.99,
    image: 'https://placehold.co/40x40/e2e8f0/475569?text=1',
  },
  {
    id: '2',
    type: 'product',
    name: 'Akku-Winkelschleifer 18V',
    price: 149.0,
    image: 'https://placehold.co/40x40/e2e8f0/475569?text=2',
  },
  {
    id: '3',
    type: 'product',
    name: 'Akku-Stichsäge 18V',
    price: 129.99,
    image: 'https://placehold.co/40x40/e2e8f0/475569?text=3',
  },
];

const mockCategories = [
  { id: 'cat-1', name: 'Elektrowerkzeuge', count: 245 },
  { id: 'cat-2', name: 'Akku-Werkzeuge', count: 89 },
];

const mockRecentSearches = ['Bohrmaschine', 'Säge', 'Werkzeugkoffer'];

const sizeClass = computed(() => {
  const sizes: Record<string, string> = {
    sm: 'search-box--sm',
    md: 'search-box--md',
    lg: 'search-box--lg',
  };
  return sizes[props.config.size] || 'search-box--md';
});

const variantClass = computed(() => {
  const variants: Record<string, string> = {
    default: 'search-box--default',
    minimal: 'search-box--minimal',
    expanded: 'search-box--expanded',
  };
  return variants[props.config.variant] || 'search-box--default';
});

const handleFocus = () => {
  if (props.isEditing) {
    showDropdown.value = true;
  }
};

const handleSearch = () => {
  // In real implementation, trigger search
  console.log('Search:', searchQuery.value);
};
</script>

<template>
  <div :class="['search-box', sizeClass, variantClass]">
    <div class="search-box__input-wrapper">
      <svg
        class="search-box__icon"
        viewBox="0 0 24 24"
        fill="none"
        stroke="currentColor"
        stroke-width="2"
      >
        <circle cx="11" cy="11" r="8" />
        <path d="M21 21l-4.35-4.35" />
      </svg>

      <input
        v-model="searchQuery"
        type="text"
        class="search-box__input"
        :placeholder="config.placeholder"
        @focus="handleFocus"
        @blur="showDropdown = false"
      />

      <button v-if="config.showSearchButton" class="search-box__button" @click="handleSearch">
        {{ config.searchButtonText }}
      </button>
    </div>

    <!-- Dropdown -->
    <div v-if="showDropdown || isEditing" class="search-box__dropdown">
      <!-- Recent Searches -->
      <div
        v-if="config.showRecentSearches && mockRecentSearches.length"
        class="search-box__section"
      >
        <div class="search-box__section-title">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <circle cx="12" cy="12" r="10" />
            <polyline points="12 6 12 12 16 14" />
          </svg>
          Letzte Suchen
        </div>
        <div class="search-box__recent">
          <button
            v-for="term in mockRecentSearches.slice(0, config.recentSearchesCount)"
            :key="term"
            class="search-box__recent-item"
          >
            {{ term }}
          </button>
        </div>
      </div>

      <!-- Category Suggestions -->
      <div v-if="config.showCategories && mockCategories.length" class="search-box__section">
        <div class="search-box__section-title">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M22 19a2 2 0 01-2 2H4a2 2 0 01-2-2V5a2 2 0 012-2h5l2 3h9a2 2 0 012 2z" />
          </svg>
          Kategorien
        </div>
        <ul class="search-box__categories">
          <li v-for="cat in mockCategories" :key="cat.id">
            <a href="#" class="search-box__category-link">
              {{ cat.name }}
              <span class="search-box__category-count">{{ cat.count }}</span>
            </a>
          </li>
        </ul>
      </div>

      <!-- Product Suggestions -->
      <div v-if="config.showSuggestions && mockSuggestions.length" class="search-box__section">
        <div class="search-box__section-title">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path
              d="M20.25 7.5l-.625 10.632a2.25 2.25 0 01-2.247 2.118H6.622a2.25 2.25 0 01-2.247-2.118L3.75 7.5M10 11.25h4M3.375 7.5h17.25c.621 0 1.125-.504 1.125-1.125v-1.5c0-.621-.504-1.125-1.125-1.125H3.375c-.621 0-1.125.504-1.125 1.125v1.5c0 .621.504 1.125 1.125 1.125z"
            />
          </svg>
          Produkte
        </div>
        <ul class="search-box__suggestions">
          <li
            v-for="product in mockSuggestions.slice(0, config.suggestionsCount)"
            :key="product.id"
          >
            <a href="#" class="search-box__suggestion">
              <img :src="product.image" :alt="product.name" class="search-box__suggestion-image" />
              <div class="search-box__suggestion-info">
                <span class="search-box__suggestion-name">{{ product.name }}</span>
                <span class="search-box__suggestion-price">{{ product.price.toFixed(2) }} €</span>
              </div>
            </a>
          </li>
        </ul>
      </div>

      <!-- View All Results -->
      <div class="search-box__footer">
        <a href="#" class="search-box__view-all">
          Alle Ergebnisse anzeigen
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M9 5l7 7-7 7" />
          </svg>
        </a>
      </div>
    </div>

    <!-- Edit Mode Info -->
    <div v-if="isEditing" class="search-box__edit-info">
      <span>Min. Zeichen: {{ config.minChars }} | Vorschläge: {{ config.suggestionsCount }}</span>
    </div>
  </div>
</template>

<style scoped>
.search-box {
  position: relative;
  width: 100%;
}

.search-box__input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.search-box__icon {
  position: absolute;
  left: 1rem;
  width: 1.25rem;
  height: 1.25rem;
  color: #9ca3af;
  pointer-events: none;
}

.search-box__input {
  width: 100%;
  padding: 0.75rem 1rem 0.75rem 2.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  font-size: 1rem;
  transition:
    border-color 0.2s,
    box-shadow 0.2s;
}

.search-box__input:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.search-box__input::placeholder {
  color: #9ca3af;
}

/* Size Variants */
.search-box--sm .search-box__input {
  padding: 0.5rem 0.75rem 0.5rem 2.25rem;
  font-size: 0.875rem;
}

.search-box--sm .search-box__icon {
  left: 0.75rem;
  width: 1rem;
  height: 1rem;
}

.search-box--lg .search-box__input {
  padding: 1rem 1.25rem 1rem 3rem;
  font-size: 1.125rem;
}

.search-box--lg .search-box__icon {
  left: 1rem;
  width: 1.5rem;
  height: 1.5rem;
}

/* Variant: Minimal */
.search-box--minimal .search-box__input {
  border: none;
  background: #f3f4f6;
}

/* Variant: Expanded */
.search-box--expanded .search-box__input-wrapper {
  background: white;
  border-radius: 0.5rem;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.search-box--expanded .search-box__input {
  border: none;
}

/* Search Button */
.search-box__button {
  padding: 0.75rem 1.5rem;
  background: #3b82f6;
  color: white;
  border: none;
  border-radius: 0 0.5rem 0.5rem 0;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: background 0.2s;
}

.search-box__button:hover {
  background: #2563eb;
}

/* Dropdown */
.search-box__dropdown {
  position: absolute;
  top: calc(100% + 0.5rem);
  left: 0;
  right: 0;
  background: white;
  border-radius: 0.5rem;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.15);
  z-index: 50;
  overflow: hidden;
}

.search-box__section {
  padding: 1rem;
  border-bottom: 1px solid #e5e7eb;
}

.search-box__section:last-of-type {
  border-bottom: none;
}

.search-box__section-title {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.75rem;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: #6b7280;
}

.search-box__section-title svg {
  width: 1rem;
  height: 1rem;
}

/* Recent Searches */
.search-box__recent {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.search-box__recent-item {
  padding: 0.375rem 0.75rem;
  background: #f3f4f6;
  border: none;
  border-radius: 1rem;
  font-size: 0.875rem;
  color: #374151;
  cursor: pointer;
  transition: background 0.2s;
}

.search-box__recent-item:hover {
  background: #e5e7eb;
}

/* Categories */
.search-box__categories {
  list-style: none;
  margin: 0;
  padding: 0;
}

.search-box__category-link {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.5rem 0;
  color: #374151;
  text-decoration: none;
  font-size: 0.9375rem;
  transition: color 0.2s;
}

.search-box__category-link:hover {
  color: #3b82f6;
}

.search-box__category-count {
  font-size: 0.75rem;
  color: #9ca3af;
}

/* Suggestions */
.search-box__suggestions {
  list-style: none;
  margin: 0;
  padding: 0;
}

.search-box__suggestion {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.5rem;
  border-radius: 0.375rem;
  text-decoration: none;
  transition: background 0.2s;
}

.search-box__suggestion:hover {
  background: #f9fafb;
}

.search-box__suggestion-image {
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 0.25rem;
  object-fit: cover;
  background: #f3f4f6;
}

.search-box__suggestion-info {
  display: flex;
  flex-direction: column;
  gap: 0.125rem;
}

.search-box__suggestion-name {
  font-size: 0.9375rem;
  color: #111827;
}

.search-box__suggestion-price {
  font-size: 0.875rem;
  font-weight: 600;
  color: #3b82f6;
}

/* Footer */
.search-box__footer {
  padding: 0.75rem 1rem;
  background: #f9fafb;
  border-top: 1px solid #e5e7eb;
}

.search-box__view-all {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  color: #3b82f6;
  text-decoration: none;
  font-size: 0.875rem;
  font-weight: 500;
}

.search-box__view-all:hover {
  text-decoration: underline;
}

.search-box__view-all svg {
  width: 1rem;
  height: 1rem;
}

/* Edit Mode */
.search-box__edit-info {
  position: absolute;
  top: -1.5rem;
  right: 0;
  padding: 0.25rem 0.5rem;
  background: #dbeafe;
  color: #1e40af;
  font-size: 0.75rem;
  border-radius: 0.25rem;
}
</style>
