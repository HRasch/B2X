<script setup lang="ts">
/**
 * WidgetConfigurator - Panel to configure selected widget
 */
import { computed } from 'vue';
import { usePageBuilderStore } from '@/stores/pageBuilder';
import type {
  WidgetConfig,
  TextWidgetConfig,
  ImageWidgetConfig,
  ButtonWidgetConfig,
  GridWidgetConfig,
  SectionWidgetConfig,
  SpacerWidgetConfig,
  DividerWidgetConfig,
  ContainerWidgetConfig,
  AccountDashboardWidgetConfig,
  OrderHistoryWidgetConfig,
  AddressBookWidgetConfig,
  ProfileFormWidgetConfig,
  WishlistWidgetConfig,
  ProductCardWidgetConfig,
  ProductGridWidgetConfig,
  CategoryNavWidgetConfig,
  SearchBoxWidgetConfig,
  CartSummaryWidgetConfig,
  MiniCartWidgetConfig,
  BreadcrumbWidgetConfig,
} from '@/types/widgets';

const store = usePageBuilderStore();

const selectedWidget = computed(() => store.selectedWidget);

const hasSelection = computed(() => selectedWidget.value !== null);

// Update widget config
function updateConfig<T extends WidgetConfig>(updates: Partial<T>) {
  if (!selectedWidget.value) return;
  store.updateWidget(selectedWidget.value.id, {
    ...selectedWidget.value,
    config: { ...selectedWidget.value.config, ...updates },
  } as typeof selectedWidget.value);
}

// Get responsive value for current breakpoint
function getResponsiveValue<T>(value: T | { mobile: T; tablet?: T; desktop?: T }): T {
  if (typeof value === 'object' && value !== null && 'mobile' in value) {
    return value.desktop ?? value.tablet ?? value.mobile;
  }
  return value;
}

// Update responsive value
function setResponsiveValue<T>(field: string, value: T) {
  const currentConfig = selectedWidget.value?.config as Record<string, unknown>;
  const currentValue = currentConfig?.[field];

  if (typeof currentValue === 'object' && currentValue !== null && 'mobile' in currentValue) {
    // Update desktop breakpoint for responsive values
    updateConfig({ [field]: { ...currentValue, desktop: value } });
  } else {
    updateConfig({ [field]: value });
  }
}
</script>

<template>
  <div class="widget-configurator">
    <div class="widget-configurator__header">
      <h3 class="widget-configurator__title">{{ $t('pageBuilder.configurator.title') }}</h3>
    </div>

    <div v-if="!hasSelection" class="widget-configurator__empty">
      <svg
        viewBox="0 0 24 24"
        fill="none"
        stroke="currentColor"
        stroke-width="1.5"
        class="widget-configurator__empty-icon"
      >
        <path
          d="M9.879 7.519c1.171-1.025 3.071-1.025 4.242 0 1.172 1.025 1.172 2.687 0 3.712-.203.179-.43.326-.67.442-.745.361-1.45.999-1.45 1.827v.75M21 12a9 9 0 11-18 0 9 9 0 0118 0zm-9 5.25h.008v.008H12v-.008z"
        />
      </svg>
      <p class="widget-configurator__empty-text">
        {{ $t('pageBuilder.configurator.selectWidget') }}
      </p>
    </div>

    <div v-else class="widget-configurator__content">
      <!-- Widget Name -->
      <div class="widget-configurator__section">
        <label class="widget-configurator__label">{{
          $t('pageBuilder.configurator.widget')
        }}</label>
        <div class="widget-configurator__widget-info">
          <span class="widget-configurator__widget-type">{{ selectedWidget?.type }}</span>
        </div>
      </div>

      <!-- Common Styles -->
      <div class="widget-configurator__section">
        <label class="widget-configurator__label">{{
          $t('pageBuilder.configurator.general')
        }}</label>
        <div class="widget-configurator__group">
          <!-- Visibility -->
          <label class="widget-configurator__checkbox">
            <input
              type="checkbox"
              :checked="selectedWidget?.style?.isVisible ?? true"
              @change="
                store.updateWidget(selectedWidget!.id, {
                  ...selectedWidget!,
                  style: {
                    ...selectedWidget?.style,
                    isVisible: ($event.target as HTMLInputElement).checked,
                  },
                })
              "
            />
            <span>{{ $t('pageBuilder.configurator.visible') }}</span>
          </label>
        </div>
      </div>

      <!-- Text Widget Config -->
      <template v-if="selectedWidget?.type === 'text'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">{{
            $t('pageBuilder.configurator.text')
          }}</label>
          <textarea
            :value="getResponsiveValue((selectedWidget.config as TextWidgetConfig).content || '')"
            @input="setResponsiveValue('content', ($event.target as HTMLTextAreaElement).value)"
            class="widget-configurator__textarea"
            rows="4"
            placeholder="Text eingeben..."
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">{{
            $t('pageBuilder.configurator.htmlTag')
          }}</label>
          <select
            :value="(selectedWidget.config as TextWidgetConfig).tag || 'p'"
            @change="
              updateConfig({
                tag: ($event.target as HTMLSelectElement).value as
                  | 'p'
                  | 'h1'
                  | 'h2'
                  | 'h3'
                  | 'h4'
                  | 'h5'
                  | 'h6'
                  | 'span',
              })
            "
            class="widget-configurator__select"
          >
            <option value="p">{{ $t('pageBuilder.configurator.paragraph') }}</option>
            <option value="h1">{{ $t('pageBuilder.configurator.heading1') }}</option>
            <option value="h2">{{ $t('pageBuilder.configurator.heading2') }}</option>
            <option value="h3">{{ $t('pageBuilder.configurator.heading3') }}</option>
            <option value="h4">{{ $t('pageBuilder.configurator.heading4') }}</option>
            <option value="h5">{{ $t('pageBuilder.configurator.heading5') }}</option>
            <option value="h6">{{ $t('pageBuilder.configurator.heading6') }}</option>
            <option value="span">{{ $t('pageBuilder.configurator.inline') }}</option>
          </select>
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">{{
            $t('pageBuilder.configurator.alignment')
          }}</label>
          <div class="widget-configurator__button-group">
            <button
              :class="[
                'widget-configurator__btn',
                {
                  'widget-configurator__btn--active':
                    (selectedWidget.config as TextWidgetConfig).align === 'left',
                },
              ]"
              @click="updateConfig({ align: 'left' })"
            >
              {{ $t('pageBuilder.configurator.left') }}
            </button>
            <button
              :class="[
                'widget-configurator__btn',
                {
                  'widget-configurator__btn--active':
                    (selectedWidget.config as TextWidgetConfig).align === 'center',
                },
              ]"
              @click="updateConfig({ align: 'center' })"
            >
              {{ $t('pageBuilder.configurator.center') }}
            </button>
            <button
              :class="[
                'widget-configurator__btn',
                {
                  'widget-configurator__btn--active':
                    (selectedWidget.config as TextWidgetConfig).align === 'right',
                },
              ]"
              @click="updateConfig({ align: 'right' })"
            >
              {{ $t('pageBuilder.configurator.right') }}
            </button>
          </div>
        </div>
      </template>

      <!-- Image Widget Config -->
      <template v-else-if="selectedWidget?.type === 'image'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">{{
            $t('pageBuilder.configurator.imageUrl')
          }}</label>
          <input
            type="text"
            :value="getResponsiveValue((selectedWidget.config as ImageWidgetConfig).src || '')"
            @input="setResponsiveValue('src', ($event.target as HTMLInputElement).value)"
            class="widget-configurator__input"
            placeholder="https://..."
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">{{
            $t('pageBuilder.configurator.altText')
          }}</label>
          <input
            type="text"
            :value="(selectedWidget.config as ImageWidgetConfig).alt || ''"
            @input="updateConfig({ alt: ($event.target as HTMLInputElement).value })"
            class="widget-configurator__input"
            placeholder="Bildbeschreibung..."
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">{{
            $t('pageBuilder.configurator.size')
          }}</label>
          <div class="widget-configurator__button-group">
            <button
              :class="[
                'widget-configurator__btn',
                {
                  'widget-configurator__btn--active':
                    (selectedWidget.config as ImageWidgetConfig).objectFit === 'contain',
                },
              ]"
              @click="updateConfig({ objectFit: 'contain' })"
            >
              {{ $t('pageBuilder.configurator.fit') }}
            </button>
            <button
              :class="[
                'widget-configurator__btn',
                {
                  'widget-configurator__btn--active':
                    (selectedWidget.config as ImageWidgetConfig).objectFit === 'cover',
                },
              ]"
              @click="updateConfig({ objectFit: 'cover' })"
            >
              {{ $t('pageBuilder.configurator.fill') }}
            </button>
          </div>
        </div>
      </template>

      <!-- Button Widget Config -->
      <template v-else-if="selectedWidget?.type === 'button'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Button-Text</label>
          <input
            type="text"
            :value="getResponsiveValue((selectedWidget.config as ButtonWidgetConfig).label || '')"
            @input="setResponsiveValue('label', ($event.target as HTMLInputElement).value)"
            class="widget-configurator__input"
            placeholder="Button"
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Link</label>
          <input
            type="text"
            :value="(selectedWidget.config as ButtonWidgetConfig).href || ''"
            @input="updateConfig({ href: ($event.target as HTMLInputElement).value })"
            class="widget-configurator__input"
            placeholder="/seite oder https://..."
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Variante</label>
          <select
            :value="(selectedWidget.config as ButtonWidgetConfig).variant || 'primary'"
            @change="
              updateConfig({
                variant: ($event.target as HTMLSelectElement).value as
                  | 'primary'
                  | 'secondary'
                  | 'outline'
                  | 'ghost',
              })
            "
            class="widget-configurator__select"
          >
            <option value="primary">Primär</option>
            <option value="secondary">Sekundär</option>
            <option value="outline">Umriss</option>
            <option value="ghost">Ghost</option>
          </select>
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Größe</label>
          <select
            :value="(selectedWidget.config as ButtonWidgetConfig).size || 'md'"
            @change="
              updateConfig({
                size: ($event.target as HTMLSelectElement).value as 'sm' | 'md' | 'lg',
              })
            "
            class="widget-configurator__select"
          >
            <option value="sm">Klein</option>
            <option value="md">Mittel</option>
            <option value="lg">Groß</option>
          </select>
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__checkbox">
            <input
              type="checkbox"
              :checked="(selectedWidget.config as ButtonWidgetConfig).fullWidth || false"
              @change="updateConfig({ fullWidth: ($event.target as HTMLInputElement).checked })"
            />
            <span>Volle Breite</span>
          </label>
        </div>
      </template>

      <!-- Grid Widget Config -->
      <template v-else-if="selectedWidget?.type === 'grid'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Spalten</label>
          <input
            type="number"
            :value="getResponsiveValue((selectedWidget.config as GridWidgetConfig).columns || 2)"
            @input="
              setResponsiveValue(
                'columns',
                parseInt(($event.target as HTMLInputElement).value) || 2
              )
            "
            class="widget-configurator__input"
            min="1"
            max="12"
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Abstand</label>
          <input
            type="text"
            :value="getResponsiveValue((selectedWidget.config as GridWidgetConfig).gap || '1rem')"
            @input="setResponsiveValue('gap', ($event.target as HTMLInputElement).value)"
            class="widget-configurator__input"
            placeholder="1rem"
          />
        </div>
      </template>

      <!-- Section Widget Config -->
      <template v-else-if="selectedWidget?.type === 'section'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Hintergrundfarbe</label>
          <input
            type="color"
            :value="(selectedWidget.config as SectionWidgetConfig).backgroundColor || '#ffffff'"
            @input="updateConfig({ backgroundColor: ($event.target as HTMLInputElement).value })"
            class="widget-configurator__color"
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Volle Breite</label>
          <label class="widget-configurator__checkbox">
            <input
              type="checkbox"
              :checked="(selectedWidget.config as SectionWidgetConfig).fullWidth || false"
              @change="updateConfig({ fullWidth: ($event.target as HTMLInputElement).checked })"
            />
            <span>Aktiviert</span>
          </label>
        </div>
      </template>

      <!-- Container Widget Config -->
      <template v-else-if="selectedWidget?.type === 'container'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Max. Breite</label>
          <select
            :value="
              getResponsiveValue(
                (selectedWidget.config as ContainerWidgetConfig).maxWidth || '1200px'
              )
            "
            @change="setResponsiveValue('maxWidth', ($event.target as HTMLSelectElement).value)"
            class="widget-configurator__select"
          >
            <option value="640px">Klein (640px)</option>
            <option value="768px">Mittel (768px)</option>
            <option value="1024px">Groß (1024px)</option>
            <option value="1200px">Extra Groß (1200px)</option>
            <option value="100%">Volle Breite</option>
          </select>
        </div>
      </template>

      <!-- Spacer Widget Config -->
      <template v-else-if="selectedWidget?.type === 'spacer'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Höhe</label>
          <input
            type="text"
            :value="
              getResponsiveValue((selectedWidget.config as SpacerWidgetConfig).height || '2rem')
            "
            @input="setResponsiveValue('height', ($event.target as HTMLInputElement).value)"
            class="widget-configurator__input"
            placeholder="2rem"
          />
        </div>
      </template>

      <!-- Divider Widget Config -->
      <template v-else-if="selectedWidget?.type === 'divider'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Stil</label>
          <select
            :value="(selectedWidget.config as DividerWidgetConfig).style || 'solid'"
            @change="
              updateConfig({
                style: ($event.target as HTMLSelectElement).value as 'solid' | 'dashed' | 'dotted',
              })
            "
            class="widget-configurator__select"
          >
            <option value="solid">Durchgezogen</option>
            <option value="dashed">Gestrichelt</option>
            <option value="dotted">Gepunktet</option>
          </select>
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Farbe</label>
          <input
            type="color"
            :value="(selectedWidget.config as DividerWidgetConfig).color || '#e5e7eb'"
            @input="updateConfig({ color: ($event.target as HTMLInputElement).value })"
            class="widget-configurator__color"
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Dicke</label>
          <input
            type="text"
            :value="(selectedWidget.config as DividerWidgetConfig).thickness || '1px'"
            @input="updateConfig({ thickness: ($event.target as HTMLInputElement).value })"
            class="widget-configurator__input"
            placeholder="1px"
          />
        </div>
      </template>

      <!-- Account Dashboard Widget Config -->
      <template v-else-if="selectedWidget?.type === 'account-dashboard'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Titel</label>
          <input
            type="text"
            :value="(selectedWidget.config as AccountDashboardWidgetConfig).title || ''"
            @input="updateConfig({ title: ($event.target as HTMLInputElement).value })"
            class="widget-configurator__input"
            placeholder="Mein Konto"
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Layout</label>
          <select
            :value="(selectedWidget.config as AccountDashboardWidgetConfig).layout || 'grid'"
            @change="
              updateConfig({
                layout: ($event.target as HTMLSelectElement).value as 'grid' | 'list',
              })
            "
            class="widget-configurator__select"
          >
            <option value="grid">Raster</option>
            <option value="list">Liste</option>
          </select>
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Optionen</label>
          <div class="widget-configurator__group">
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="
                  (selectedWidget.config as AccountDashboardWidgetConfig).showWelcomeMessage ?? true
                "
                @change="
                  updateConfig({ showWelcomeMessage: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>Willkommensnachricht</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="
                  (selectedWidget.config as AccountDashboardWidgetConfig).showQuickLinks ?? true
                "
                @change="
                  updateConfig({ showQuickLinks: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>Schnelllinks</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="
                  (selectedWidget.config as AccountDashboardWidgetConfig).showRecentOrders ?? true
                "
                @change="
                  updateConfig({ showRecentOrders: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>Letzte Bestellungen</span>
            </label>
          </div>
        </div>
      </template>

      <!-- Order History Widget Config -->
      <template v-else-if="selectedWidget?.type === 'order-history'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Titel</label>
          <input
            type="text"
            :value="(selectedWidget.config as OrderHistoryWidgetConfig).title || ''"
            @input="updateConfig({ title: ($event.target as HTMLInputElement).value })"
            class="widget-configurator__input"
            placeholder="Bestellverlauf"
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Einträge pro Seite</label>
          <input
            type="number"
            :value="(selectedWidget.config as OrderHistoryWidgetConfig).itemsPerPage || 10"
            @input="
              updateConfig({
                itemsPerPage: parseInt(($event.target as HTMLInputElement).value) || 10,
              })
            "
            class="widget-configurator__input"
            min="5"
            max="50"
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Optionen</label>
          <div class="widget-configurator__group">
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as OrderHistoryWidgetConfig).showFilters ?? true"
                @change="updateConfig({ showFilters: ($event.target as HTMLInputElement).checked })"
              />
              <span>Filter anzeigen</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as OrderHistoryWidgetConfig).showSearch ?? true"
                @change="updateConfig({ showSearch: ($event.target as HTMLInputElement).checked })"
              />
              <span>Suchfeld anzeigen</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as OrderHistoryWidgetConfig).showTracking ?? true"
                @change="
                  updateConfig({ showTracking: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>Tracking anzeigen</span>
            </label>
          </div>
        </div>
      </template>

      <!-- Address Book Widget Config -->
      <template v-else-if="selectedWidget?.type === 'address-book'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Titel</label>
          <input
            type="text"
            :value="(selectedWidget.config as AddressBookWidgetConfig).title || ''"
            @input="updateConfig({ title: ($event.target as HTMLInputElement).value })"
            class="widget-configurator__input"
            placeholder="Adressbuch"
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Layout</label>
          <select
            :value="(selectedWidget.config as AddressBookWidgetConfig).layout || 'grid'"
            @change="
              updateConfig({
                layout: ($event.target as HTMLSelectElement).value as 'grid' | 'list',
              })
            "
            class="widget-configurator__select"
          >
            <option value="grid">Raster</option>
            <option value="list">Liste</option>
          </select>
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Max. Adressen</label>
          <input
            type="number"
            :value="(selectedWidget.config as AddressBookWidgetConfig).maxAddresses || 10"
            @input="
              updateConfig({
                maxAddresses: parseInt(($event.target as HTMLInputElement).value) || 10,
              })
            "
            class="widget-configurator__input"
            min="1"
            max="50"
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Optionen</label>
          <div class="widget-configurator__group">
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as AddressBookWidgetConfig).allowAdd ?? true"
                @change="updateConfig({ allowAdd: ($event.target as HTMLInputElement).checked })"
              />
              <span>Neue Adressen erlauben</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as AddressBookWidgetConfig).allowEdit ?? true"
                @change="updateConfig({ allowEdit: ($event.target as HTMLInputElement).checked })"
              />
              <span>Bearbeiten erlauben</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as AddressBookWidgetConfig).allowDelete ?? true"
                @change="updateConfig({ allowDelete: ($event.target as HTMLInputElement).checked })"
              />
              <span>Löschen erlauben</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="
                  (selectedWidget.config as AddressBookWidgetConfig).showDefaultBadge ?? true
                "
                @change="
                  updateConfig({ showDefaultBadge: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>Standard-Badge anzeigen</span>
            </label>
          </div>
        </div>
      </template>

      <!-- Profile Form Widget Config -->
      <template v-else-if="selectedWidget?.type === 'profile-form'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Titel</label>
          <input
            type="text"
            :value="(selectedWidget.config as ProfileFormWidgetConfig).title || ''"
            @input="updateConfig({ title: ($event.target as HTMLInputElement).value })"
            class="widget-configurator__input"
            placeholder="Mein Profil"
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Layout</label>
          <select
            :value="(selectedWidget.config as ProfileFormWidgetConfig).layout || 'single-column'"
            @change="
              updateConfig({
                layout: ($event.target as HTMLSelectElement).value as
                  | 'single-column'
                  | 'two-column',
              })
            "
            class="widget-configurator__select"
          >
            <option value="single-column">Eine Spalte</option>
            <option value="two-column">Zwei Spalten</option>
          </select>
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Optionen</label>
          <div class="widget-configurator__group">
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as ProfileFormWidgetConfig).showAvatar ?? true"
                @change="updateConfig({ showAvatar: ($event.target as HTMLInputElement).checked })"
              />
              <span>Avatar anzeigen</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="
                  (selectedWidget.config as ProfileFormWidgetConfig).showPasswordChange ?? true
                "
                @change="
                  updateConfig({ showPasswordChange: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>Passwort ändern</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="
                  (selectedWidget.config as ProfileFormWidgetConfig).showNewsletterOptIn ?? true
                "
                @change="
                  updateConfig({ showNewsletterOptIn: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>Newsletter Opt-in</span>
            </label>
          </div>
        </div>
      </template>

      <!-- Wishlist Widget Config -->
      <template v-else-if="selectedWidget?.type === 'wishlist'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Titel</label>
          <input
            type="text"
            :value="(selectedWidget.config as WishlistWidgetConfig).title || ''"
            @input="updateConfig({ title: ($event.target as HTMLInputElement).value })"
            class="widget-configurator__input"
            placeholder="Merkzettel"
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Layout</label>
          <select
            :value="(selectedWidget.config as WishlistWidgetConfig).layout || 'grid'"
            @change="
              updateConfig({
                layout: ($event.target as HTMLSelectElement).value as 'grid' | 'list',
              })
            "
            class="widget-configurator__select"
          >
            <option value="grid">Raster</option>
            <option value="list">Liste</option>
          </select>
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Spalten (Raster)</label>
          <input
            type="number"
            :value="
              getResponsiveValue((selectedWidget.config as WishlistWidgetConfig).columns || 3)
            "
            @input="
              setResponsiveValue(
                'columns',
                parseInt(($event.target as HTMLInputElement).value) || 3
              )
            "
            class="widget-configurator__input"
            min="1"
            max="6"
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Optionen</label>
          <div class="widget-configurator__group">
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as WishlistWidgetConfig).showAddToCart ?? true"
                @change="
                  updateConfig({ showAddToCart: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>In Warenkorb Button</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as WishlistWidgetConfig).showRemoveButton ?? true"
                @change="
                  updateConfig({ showRemoveButton: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>Entfernen Button</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as WishlistWidgetConfig).showShareButton ?? true"
                @change="
                  updateConfig({ showShareButton: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>Teilen Button</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as WishlistWidgetConfig).showStockStatus ?? true"
                @change="
                  updateConfig({ showStockStatus: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>Verfügbarkeit anzeigen</span>
            </label>
          </div>
        </div>
      </template>

      <!-- Product Card Widget Config -->
      <template v-else-if="selectedWidget?.type === 'product-card'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Produkt-ID</label>
          <input
            type="text"
            :value="(selectedWidget.config as ProductCardWidgetConfig).productId || ''"
            @input="updateConfig({ productId: ($event.target as HTMLInputElement).value })"
            class="widget-configurator__input"
            placeholder="Produkt-ID eingeben..."
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Variante</label>
          <select
            :value="(selectedWidget.config as ProductCardWidgetConfig).variant || 'default'"
            @change="
              updateConfig({
                variant: ($event.target as HTMLSelectElement).value as
                  | 'default'
                  | 'compact'
                  | 'horizontal',
              })
            "
            class="widget-configurator__select"
          >
            <option value="default">Standard</option>
            <option value="compact">Kompakt</option>
            <option value="horizontal">Horizontal</option>
          </select>
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Bild-Verhältnis</label>
          <select
            :value="(selectedWidget.config as ProductCardWidgetConfig).imageAspectRatio || '1:1'"
            @change="
              updateConfig({
                imageAspectRatio: ($event.target as HTMLSelectElement).value as
                  | '1:1'
                  | '4:3'
                  | '3:4'
                  | '16:9',
              })
            "
            class="widget-configurator__select"
          >
            <option value="1:1">1:1 (Quadrat)</option>
            <option value="4:3">4:3</option>
            <option value="3:4">3:4 (Hochformat)</option>
            <option value="16:9">16:9</option>
          </select>
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Optionen</label>
          <div class="widget-configurator__group">
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as ProductCardWidgetConfig).showPrice ?? true"
                @change="updateConfig({ showPrice: ($event.target as HTMLInputElement).checked })"
              />
              <span>Preis anzeigen</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as ProductCardWidgetConfig).showAddToCart ?? true"
                @change="
                  updateConfig({ showAddToCart: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>In Warenkorb Button</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as ProductCardWidgetConfig).showWishlist ?? true"
                @change="
                  updateConfig({ showWishlist: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>Merkzettel Button</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as ProductCardWidgetConfig).showRating ?? true"
                @change="updateConfig({ showRating: ($event.target as HTMLInputElement).checked })"
              />
              <span>Bewertung anzeigen</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as ProductCardWidgetConfig).showBadges ?? true"
                @change="updateConfig({ showBadges: ($event.target as HTMLInputElement).checked })"
              />
              <span>Badges anzeigen</span>
            </label>
          </div>
        </div>
      </template>

      <!-- Product Grid Widget Config -->
      <template v-else-if="selectedWidget?.type === 'product-grid'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Titel</label>
          <input
            type="text"
            :value="(selectedWidget.config as ProductGridWidgetConfig).title || ''"
            @input="updateConfig({ title: ($event.target as HTMLInputElement).value })"
            class="widget-configurator__input"
            placeholder="Produktübersicht"
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Datenquelle</label>
          <select
            :value="(selectedWidget.config as ProductGridWidgetConfig).source || 'category'"
            @change="
              updateConfig({
                source: ($event.target as HTMLSelectElement).value as
                  | 'category'
                  | 'manual'
                  | 'bestseller'
                  | 'new'
                  | 'sale',
              })
            "
            class="widget-configurator__select"
          >
            <option value="category">Kategorie</option>
            <option value="manual">Manuelle Auswahl</option>
            <option value="bestseller">Bestseller</option>
            <option value="new">Neuheiten</option>
            <option value="sale">Angebote</option>
          </select>
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Spalten</label>
          <input
            type="number"
            :value="
              getResponsiveValue((selectedWidget.config as ProductGridWidgetConfig).columns || 4)
            "
            @input="
              setResponsiveValue(
                'columns',
                parseInt(($event.target as HTMLInputElement).value) || 4
              )
            "
            class="widget-configurator__input"
            min="1"
            max="6"
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Einträge pro Seite</label>
          <input
            type="number"
            :value="(selectedWidget.config as ProductGridWidgetConfig).itemsPerPage || 12"
            @input="
              updateConfig({
                itemsPerPage: parseInt(($event.target as HTMLInputElement).value) || 12,
              })
            "
            class="widget-configurator__input"
            min="4"
            max="48"
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Optionen</label>
          <div class="widget-configurator__group">
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as ProductGridWidgetConfig).showFilters ?? true"
                @change="updateConfig({ showFilters: ($event.target as HTMLInputElement).checked })"
              />
              <span>Filter anzeigen</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as ProductGridWidgetConfig).showSorting ?? true"
                @change="updateConfig({ showSorting: ($event.target as HTMLInputElement).checked })"
              />
              <span>Sortierung anzeigen</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as ProductGridWidgetConfig).showPagination ?? true"
                @change="
                  updateConfig({ showPagination: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>Paginierung anzeigen</span>
            </label>
          </div>
        </div>
      </template>

      <!-- Category Nav Widget Config -->
      <template v-else-if="selectedWidget?.type === 'category-nav'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Titel</label>
          <input
            type="text"
            :value="(selectedWidget.config as CategoryNavWidgetConfig).title || ''"
            @input="updateConfig({ title: ($event.target as HTMLInputElement).value })"
            class="widget-configurator__input"
            placeholder="Kategorien"
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Layout</label>
          <select
            :value="(selectedWidget.config as CategoryNavWidgetConfig).layout || 'vertical'"
            @change="
              updateConfig({
                layout: ($event.target as HTMLSelectElement).value as
                  | 'vertical'
                  | 'horizontal'
                  | 'mega-menu',
              })
            "
            class="widget-configurator__select"
          >
            <option value="vertical">Vertikal</option>
            <option value="horizontal">Horizontal</option>
            <option value="mega-menu">Mega-Menü</option>
          </select>
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Max. Tiefe</label>
          <input
            type="number"
            :value="(selectedWidget.config as CategoryNavWidgetConfig).maxDepth || 3"
            @input="
              updateConfig({ maxDepth: parseInt(($event.target as HTMLInputElement).value) || 3 })
            "
            class="widget-configurator__input"
            min="1"
            max="5"
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Optionen</label>
          <div class="widget-configurator__group">
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="
                  (selectedWidget.config as CategoryNavWidgetConfig).showProductCount ?? true
                "
                @change="
                  updateConfig({ showProductCount: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>Produktanzahl anzeigen</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as CategoryNavWidgetConfig).showImages ?? false"
                @change="updateConfig({ showImages: ($event.target as HTMLInputElement).checked })"
              />
              <span>Bilder anzeigen</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as CategoryNavWidgetConfig).collapsible ?? true"
                @change="updateConfig({ collapsible: ($event.target as HTMLInputElement).checked })"
              />
              <span>Einklappbar</span>
            </label>
          </div>
        </div>
      </template>

      <!-- Search Box Widget Config -->
      <template v-else-if="selectedWidget?.type === 'search-box'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Platzhalter</label>
          <input
            type="text"
            :value="(selectedWidget.config as SearchBoxWidgetConfig).placeholder || ''"
            @input="updateConfig({ placeholder: ($event.target as HTMLInputElement).value })"
            class="widget-configurator__input"
            placeholder="Suchen..."
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Größe</label>
          <select
            :value="(selectedWidget.config as SearchBoxWidgetConfig).size || 'md'"
            @change="
              updateConfig({
                size: ($event.target as HTMLSelectElement).value as 'sm' | 'md' | 'lg',
              })
            "
            class="widget-configurator__select"
          >
            <option value="sm">Klein</option>
            <option value="md">Mittel</option>
            <option value="lg">Groß</option>
          </select>
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Max. Vorschläge</label>
          <input
            type="number"
            :value="(selectedWidget.config as SearchBoxWidgetConfig).maxSuggestions || 6"
            @input="
              updateConfig({
                maxSuggestions: parseInt(($event.target as HTMLInputElement).value) || 6,
              })
            "
            class="widget-configurator__input"
            min="0"
            max="20"
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Optionen</label>
          <div class="widget-configurator__group">
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as SearchBoxWidgetConfig).showSuggestions ?? true"
                @change="
                  updateConfig({ showSuggestions: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>Vorschläge anzeigen</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="
                  (selectedWidget.config as SearchBoxWidgetConfig).showCategoryFilter ?? false
                "
                @change="
                  updateConfig({ showCategoryFilter: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>Kategoriefilter anzeigen</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="
                  (selectedWidget.config as SearchBoxWidgetConfig).showRecentSearches ?? true
                "
                @change="
                  updateConfig({ showRecentSearches: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>Letzte Suchen anzeigen</span>
            </label>
          </div>
        </div>
      </template>

      <!-- Cart Summary Widget Config -->
      <template v-else-if="selectedWidget?.type === 'cart-summary'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Titel</label>
          <input
            type="text"
            :value="(selectedWidget.config as CartSummaryWidgetConfig).title || ''"
            @input="updateConfig({ title: ($event.target as HTMLInputElement).value })"
            class="widget-configurator__input"
            placeholder="Zusammenfassung"
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Checkout-Button Text</label>
          <input
            type="text"
            :value="(selectedWidget.config as CartSummaryWidgetConfig).checkoutButtonText || ''"
            @input="updateConfig({ checkoutButtonText: ($event.target as HTMLInputElement).value })"
            class="widget-configurator__input"
            placeholder="Zur Kasse"
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Optionen</label>
          <div class="widget-configurator__group">
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as CartSummaryWidgetConfig).showSubtotal ?? true"
                @change="
                  updateConfig({ showSubtotal: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>Zwischensumme anzeigen</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as CartSummaryWidgetConfig).showShipping ?? true"
                @change="
                  updateConfig({ showShipping: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>Versandkosten anzeigen</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as CartSummaryWidgetConfig).showTax ?? true"
                @change="updateConfig({ showTax: ($event.target as HTMLInputElement).checked })"
              />
              <span>MwSt. anzeigen</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as CartSummaryWidgetConfig).showPromoCode ?? true"
                @change="
                  updateConfig({ showPromoCode: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>Gutscheinfeld anzeigen</span>
            </label>
          </div>
        </div>
      </template>

      <!-- Mini Cart Widget Config -->
      <template v-else-if="selectedWidget?.type === 'mini-cart'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Variante</label>
          <select
            :value="(selectedWidget.config as MiniCartWidgetConfig).variant || 'dropdown'"
            @change="
              updateConfig({
                variant: ($event.target as HTMLSelectElement).value as 'dropdown' | 'slide-out',
              })
            "
            class="widget-configurator__select"
          >
            <option value="dropdown">Dropdown</option>
            <option value="slide-out">Seitenpanel</option>
          </select>
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Max. angezeigte Artikel</label>
          <input
            type="number"
            :value="(selectedWidget.config as MiniCartWidgetConfig).maxItems || 5"
            @input="
              updateConfig({ maxItems: parseInt(($event.target as HTMLInputElement).value) || 5 })
            "
            class="widget-configurator__input"
            min="1"
            max="10"
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Optionen</label>
          <div class="widget-configurator__group">
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as MiniCartWidgetConfig).showItemCount ?? true"
                @change="
                  updateConfig({ showItemCount: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>Artikelanzahl anzeigen</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as MiniCartWidgetConfig).showTotal ?? true"
                @change="updateConfig({ showTotal: ($event.target as HTMLInputElement).checked })"
              />
              <span>Gesamtsumme anzeigen</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="
                  (selectedWidget.config as MiniCartWidgetConfig).showCheckoutButton ?? true
                "
                @change="
                  updateConfig({ showCheckoutButton: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>Checkout-Button anzeigen</span>
            </label>
          </div>
        </div>
      </template>

      <!-- Breadcrumb Widget Config -->
      <template v-else-if="selectedWidget?.type === 'breadcrumb'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Trennzeichen</label>
          <select
            :value="(selectedWidget.config as BreadcrumbWidgetConfig).separator || 'chevron'"
            @change="
              updateConfig({
                separator: ($event.target as HTMLSelectElement).value as
                  | 'slash'
                  | 'chevron'
                  | 'arrow'
                  | 'dot',
              })
            "
            class="widget-configurator__select"
          >
            <option value="chevron">Chevron (›)</option>
            <option value="slash">Schrägstrich (/)</option>
            <option value="arrow">Pfeil (→)</option>
            <option value="dot">Punkt (•)</option>
          </select>
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Max. Elemente</label>
          <input
            type="number"
            :value="(selectedWidget.config as BreadcrumbWidgetConfig).maxItems || 5"
            @input="
              updateConfig({ maxItems: parseInt(($event.target as HTMLInputElement).value) || 5 })
            "
            class="widget-configurator__input"
            min="2"
            max="10"
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Optionen</label>
          <div class="widget-configurator__group">
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as BreadcrumbWidgetConfig).showHomeIcon ?? true"
                @change="
                  updateConfig({ showHomeIcon: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>Home-Icon anzeigen</span>
            </label>
            <label class="widget-configurator__checkbox">
              <input
                type="checkbox"
                :checked="(selectedWidget.config as BreadcrumbWidgetConfig).showCurrentPage ?? true"
                @change="
                  updateConfig({ showCurrentPage: ($event.target as HTMLInputElement).checked })
                "
              />
              <span>Aktuelle Seite anzeigen</span>
            </label>
          </div>
        </div>
      </template>

      <!-- Delete Widget -->
      <div class="widget-configurator__section widget-configurator__section--danger">
        <button
          class="widget-configurator__delete-btn"
          @click="store.removeWidget(selectedWidget!.id)"
        >
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
            <path
              d="M14.74 9l-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 01-2.244 2.077H8.084a2.25 2.25 0 01-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 00-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 013.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 00-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 00-7.5 0"
            />
          </svg>
          Widget löschen
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.widget-configurator {
  display: flex;
  flex-direction: column;
  height: 100%;
  background-color: #f9fafb;
  border-left: 1px solid #e5e7eb;
}

.widget-configurator__header {
  padding: 1rem;
  border-bottom: 1px solid #e5e7eb;
}

.widget-configurator__title {
  font-size: 1rem;
  font-weight: 600;
  color: #111827;
  margin: 0;
}

.widget-configurator__empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 2rem;
  text-align: center;
  flex: 1;
}

.widget-configurator__empty-icon {
  width: 48px;
  height: 48px;
  color: #d1d5db;
  margin-bottom: 1rem;
}

.widget-configurator__empty-text {
  color: #6b7280;
  font-size: 0.875rem;
  margin: 0;
}

.widget-configurator__content {
  flex: 1;
  overflow-y: auto;
  padding: 0.5rem;
}

.widget-configurator__section {
  padding: 0.75rem;
  background-color: white;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  margin-bottom: 0.5rem;
}

.widget-configurator__section--danger {
  background-color: #fef2f2;
  border-color: #fee2e2;
}

.widget-configurator__label {
  display: block;
  font-size: 0.75rem;
  font-weight: 600;
  color: #374151;
  margin-bottom: 0.5rem;
  text-transform: uppercase;
  letter-spacing: 0.025em;
}

.widget-configurator__widget-info {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.widget-configurator__widget-type {
  display: inline-block;
  padding: 0.25rem 0.5rem;
  background-color: #e5e7eb;
  color: #374151;
  font-size: 0.75rem;
  font-weight: 500;
  border-radius: 4px;
  text-transform: capitalize;
}

.widget-configurator__input,
.widget-configurator__select,
.widget-configurator__textarea {
  width: 100%;
  padding: 0.5rem 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 0.875rem;
  color: #111827;
  background-color: white;
  transition: border-color 0.2s;
}

.widget-configurator__input:focus,
.widget-configurator__select:focus,
.widget-configurator__textarea:focus {
  outline: none;
  border-color: var(--color-primary, #3b82f6);
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.widget-configurator__textarea {
  resize: vertical;
  min-height: 80px;
}

.widget-configurator__color {
  width: 100%;
  height: 36px;
  padding: 2px;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  cursor: pointer;
}

.widget-configurator__checkbox {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
}

.widget-configurator__checkbox input {
  width: 16px;
  height: 16px;
  accent-color: var(--color-primary, #3b82f6);
}

.widget-configurator__checkbox span {
  font-size: 0.875rem;
  color: #374151;
}

.widget-configurator__group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.widget-configurator__button-group {
  display: flex;
  gap: 0.25rem;
}

.widget-configurator__btn {
  flex: 1;
  padding: 0.375rem 0.5rem;
  font-size: 0.75rem;
  font-weight: 500;
  color: #374151;
  background-color: white;
  border: 1px solid #d1d5db;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
}

.widget-configurator__btn:hover {
  background-color: #f3f4f6;
}

.widget-configurator__btn--active {
  color: white;
  background-color: var(--color-primary, #3b82f6);
  border-color: var(--color-primary, #3b82f6);
}

.widget-configurator__delete-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  width: 100%;
  padding: 0.625rem 1rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: #dc2626;
  background-color: white;
  border: 1px solid #fca5a5;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s;
}

.widget-configurator__delete-btn:hover {
  color: white;
  background-color: #dc2626;
  border-color: #dc2626;
}

.widget-configurator__delete-btn svg {
  width: 18px;
  height: 18px;
}
</style>
