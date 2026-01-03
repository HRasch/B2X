<script setup lang="ts">
/**
 * CategoryNavWidget - Category tree navigation
 */
import { ref, computed } from 'vue';
import type { CategoryNavWidgetConfig } from '@/types/widgets';

const props = defineProps<{
  config: CategoryNavWidgetConfig;
  isEditing?: boolean;
}>();

// Mock categories for preview
const mockCategories = [
  {
    id: '1',
    name: 'Elektrowerkzeuge',
    slug: 'elektrowerkzeuge',
    productCount: 245,
    image: 'https://placehold.co/80x80/e2e8f0/475569?text=E',
    children: [
      { id: '1-1', name: 'Bohrmaschinen', slug: 'bohrmaschinen', productCount: 56 },
      { id: '1-2', name: 'Sägen', slug: 'saegen', productCount: 38 },
      { id: '1-3', name: 'Schleifgeräte', slug: 'schleifgeraete', productCount: 42 },
      { id: '1-4', name: 'Fräsen', slug: 'fraesen', productCount: 29 },
    ],
  },
  {
    id: '2',
    name: 'Handwerkzeuge',
    slug: 'handwerkzeuge',
    productCount: 312,
    image: 'https://placehold.co/80x80/e2e8f0/475569?text=H',
    children: [
      { id: '2-1', name: 'Schraubendreher', slug: 'schraubendreher', productCount: 67 },
      { id: '2-2', name: 'Zangen', slug: 'zangen', productCount: 54 },
      { id: '2-3', name: 'Hammer', slug: 'hammer', productCount: 28 },
    ],
  },
  {
    id: '3',
    name: 'Messtechnik',
    slug: 'messtechnik',
    productCount: 89,
    image: 'https://placehold.co/80x80/e2e8f0/475569?text=M',
    children: [
      { id: '3-1', name: 'Laser', slug: 'laser', productCount: 23 },
      { id: '3-2', name: 'Wasserwaagen', slug: 'wasserwaagen', productCount: 18 },
    ],
  },
  {
    id: '4',
    name: 'Arbeitsschutz',
    slug: 'arbeitsschutz',
    productCount: 156,
    image: 'https://placehold.co/80x80/e2e8f0/475569?text=A',
    children: [],
  },
];

const expandedCategories = ref<Set<string>>(new Set(['1']));

const toggleCategory = (id: string) => {
  if (expandedCategories.value.has(id)) {
    expandedCategories.value.delete(id);
  } else {
    expandedCategories.value.add(id);
  }
};

const displayCategories = computed(() => {
  if (props.config.maxItems) {
    return mockCategories.slice(0, props.config.maxItems);
  }
  return mockCategories;
});

const layoutClass = computed(() => {
  const layouts: Record<string, string> = {
    vertical: 'category-nav--vertical',
    horizontal: 'category-nav--horizontal',
    'mega-menu': 'category-nav--mega',
  };
  return layouts[props.config.layout] || 'category-nav--vertical';
});
</script>

<template>
  <nav :class="['category-nav', layoutClass]">
    <h2 v-if="config.title" class="category-nav__title">{{ config.title }}</h2>

    <!-- Vertical Layout -->
    <ul v-if="config.layout === 'vertical'" class="category-nav__list">
      <li
        v-for="category in displayCategories"
        :key="category.id"
        class="category-nav__item"
      >
        <div class="category-nav__row">
          <img
            v-if="config.showImages && category.image"
            :src="category.image"
            :alt="category.name"
            class="category-nav__image"
          />
          <a href="#" class="category-nav__link">
            {{ category.name }}
            <span v-if="config.showProductCount" class="category-nav__count">
              ({{ category.productCount }})
            </span>
          </a>
          <button
            v-if="config.expandable && category.children?.length"
            class="category-nav__toggle"
            @click="toggleCategory(category.id)"
          >
            <svg
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="2"
              :class="{ 'category-nav__toggle--expanded': expandedCategories.has(category.id) }"
            >
              <path d="M9 5l7 7-7 7" />
            </svg>
          </button>
        </div>

        <!-- Children (depth 2) -->
        <ul
          v-if="category.children?.length && config.depth >= 2"
          v-show="!config.expandable || expandedCategories.has(category.id)"
          class="category-nav__sublist"
        >
          <li
            v-for="child in category.children"
            :key="child.id"
            class="category-nav__subitem"
          >
            <a href="#" class="category-nav__sublink">
              {{ child.name }}
              <span v-if="config.showProductCount" class="category-nav__count">
                ({{ child.productCount }})
              </span>
            </a>
          </li>
        </ul>
      </li>
    </ul>

    <!-- Horizontal Layout -->
    <div v-else-if="config.layout === 'horizontal'" class="category-nav__horizontal">
      <a
        v-for="category in displayCategories"
        :key="category.id"
        href="#"
        class="category-nav__horizontal-item"
      >
        <img
          v-if="config.showImages && category.image"
          :src="category.image"
          :alt="category.name"
          class="category-nav__horizontal-image"
        />
        <span class="category-nav__horizontal-name">{{ category.name }}</span>
        <span v-if="config.showProductCount" class="category-nav__count">
          {{ category.productCount }}
        </span>
      </a>
    </div>

    <!-- Mega Menu Layout -->
    <div v-else-if="config.layout === 'mega-menu'" class="category-nav__mega">
      <div
        v-for="category in displayCategories"
        :key="category.id"
        class="category-nav__mega-column"
      >
        <div class="category-nav__mega-header">
          <img
            v-if="config.showImages && category.image"
            :src="category.image"
            :alt="category.name"
            class="category-nav__mega-image"
          />
          <a href="#" class="category-nav__mega-title">
            {{ category.name }}
            <span v-if="config.showProductCount" class="category-nav__count">
              ({{ category.productCount }})
            </span>
          </a>
        </div>
        <ul v-if="category.children?.length" class="category-nav__mega-list">
          <li v-for="child in category.children" :key="child.id">
            <a href="#" class="category-nav__mega-link">
              {{ child.name }}
              <span v-if="config.showProductCount" class="category-nav__count">
                ({{ child.productCount }})
              </span>
            </a>
          </li>
        </ul>
      </div>
    </div>

    <!-- Edit Mode Info -->
    <div v-if="isEditing" class="category-nav__edit-info">
      <span>Layout: {{ config.layout }} | Tiefe: {{ config.depth }}</span>
    </div>
  </nav>
</template>

<style scoped>
.category-nav {
  position: relative;
}

.category-nav__title {
  margin: 0 0 1rem;
  font-size: 1.125rem;
  font-weight: 600;
  color: #111827;
}

/* Vertical Layout */
.category-nav__list {
  list-style: none;
  margin: 0;
  padding: 0;
}

.category-nav__item {
  border-bottom: 1px solid #e5e7eb;
}

.category-nav__item:last-child {
  border-bottom: none;
}

.category-nav__row {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem 0;
}

.category-nav__image {
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 0.375rem;
  object-fit: cover;
}

.category-nav__link {
  flex: 1;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #374151;
  text-decoration: none;
  font-size: 0.9375rem;
  transition: color 0.2s;
}

.category-nav__link:hover {
  color: #3b82f6;
}

.category-nav__count {
  color: #9ca3af;
  font-size: 0.75rem;
}

.category-nav__toggle {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 1.5rem;
  height: 1.5rem;
  border: none;
  background: none;
  cursor: pointer;
  color: #6b7280;
}

.category-nav__toggle svg {
  width: 1rem;
  height: 1rem;
  transition: transform 0.2s;
}

.category-nav__toggle--expanded {
  transform: rotate(90deg);
}

.category-nav__sublist {
  list-style: none;
  margin: 0;
  padding: 0 0 0.5rem 1.25rem;
}

.category-nav__subitem {
  padding: 0.375rem 0;
}

.category-nav__sublink {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #6b7280;
  text-decoration: none;
  font-size: 0.875rem;
  transition: color 0.2s;
}

.category-nav__sublink:hover {
  color: #3b82f6;
}

/* Horizontal Layout */
.category-nav--horizontal .category-nav__horizontal {
  display: flex;
  gap: 1rem;
  overflow-x: auto;
  padding-bottom: 0.5rem;
}

.category-nav__horizontal-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  padding: 1rem;
  background: #f9fafb;
  border-radius: 0.5rem;
  text-decoration: none;
  min-width: 6rem;
  transition: background 0.2s;
}

.category-nav__horizontal-item:hover {
  background: #f3f4f6;
}

.category-nav__horizontal-image {
  width: 3rem;
  height: 3rem;
  border-radius: 50%;
  object-fit: cover;
}

.category-nav__horizontal-name {
  font-size: 0.875rem;
  font-weight: 500;
  color: #374151;
  text-align: center;
}

/* Mega Menu Layout */
.category-nav--mega .category-nav__mega {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 2rem;
  padding: 1.5rem;
  background: white;
  border-radius: 0.5rem;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
}

.category-nav__mega-column {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.category-nav__mega-header {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.category-nav__mega-image {
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 0.375rem;
  object-fit: cover;
}

.category-nav__mega-title {
  font-weight: 600;
  color: #111827;
  text-decoration: none;
  font-size: 1rem;
}

.category-nav__mega-title:hover {
  color: #3b82f6;
}

.category-nav__mega-list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 0.375rem;
}

.category-nav__mega-link {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #6b7280;
  text-decoration: none;
  font-size: 0.875rem;
  padding: 0.25rem 0;
  transition: color 0.2s;
}

.category-nav__mega-link:hover {
  color: #3b82f6;
}

.category-nav__edit-info {
  position: absolute;
  top: 0;
  right: 0;
  padding: 0.25rem 0.5rem;
  background: #dbeafe;
  color: #1e40af;
  font-size: 0.75rem;
  border-radius: 0.25rem;
}
</style>
