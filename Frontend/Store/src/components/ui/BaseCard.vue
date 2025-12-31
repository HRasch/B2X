<template>
  <component
    :is="tag"
    :class="cardClasses"
    :to="to"
    :href="href"
    :aria-label="ariaLabel"
  >
    <!-- Card image -->
    <figure v-if="$slots.image || image" class="card-image">
      <slot name="image">
        <img
          v-if="image"
          :src="image"
          :alt="imageAlt"
          class="w-full h-full object-cover"
          loading="lazy"
        />
      </slot>
      <!-- Overlay content -->
      <div v-if="$slots.overlay" class="card-overlay">
        <slot name="overlay" />
      </div>
    </figure>

    <!-- Card body -->
    <div class="card-body" :class="bodyClasses">
      <!-- Card title -->
      <h3 v-if="title || $slots.title" class="card-title" :class="titleClasses">
        <slot name="title">{{ title }}</slot>
      </h3>

      <!-- Card subtitle -->
      <h4 v-if="subtitle || $slots.subtitle" class="card-subtitle">
        <slot name="subtitle">{{ subtitle }}</slot>
      </h4>

      <!-- Card content -->
      <div v-if="$slots.default" class="card-content">
        <slot />
      </div>

      <!-- Card actions -->
      <div v-if="$slots.actions" class="card-actions" :class="actionsClasses">
        <slot name="actions" />
      </div>
    </div>
  </component>
</template>

<script setup lang="ts">
export type CardVariant = 'default' | 'elevated' | 'outlined' | 'filled'
export type CardSize = 'sm' | 'md' | 'lg' | 'xl'

interface Props {
  variant?: CardVariant
  size?: CardSize
  image?: string
  imageAlt?: string
  title?: string
  subtitle?: string
  ariaLabel?: string
  hover?: boolean
  compact?: boolean
  tag?: 'div' | 'article' | 'section' | 'router-link' | 'a'
  to?: string
  href?: string
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'default',
  size: 'md',
  tag: 'div',
  hover: false,
  compact: false
})

const cardClasses = computed(() => {
  const classes = [
    'card',
    'bg-base-100',
    // Variant styles
    getVariantClasses(props.variant),
    // Size styles
    getSizeClasses(props.size),
    // Interactive styles
    {
      'hover:shadow-lg hover:-translate-y-1 transition-all duration-200 cursor-pointer': props.hover,
      'card-compact': props.compact
    }
  ]

  return classes.filter(Boolean).join(' ')
})

const bodyClasses = computed(() => ({
  'p-4': props.size === 'sm',
  'p-6': props.size === 'md',
  'p-8': props.size === 'lg',
  'p-10': props.size === 'xl'
}))

const titleClasses = computed(() => ({
  'text-lg': props.size === 'sm' || props.size === 'md',
  'text-xl': props.size === 'lg',
  'text-2xl': props.size === 'xl'
}))

const actionsClasses = computed(() => ({
  'justify-start': true,
  'mt-4': true
}))

const getVariantClasses = (variant: CardVariant): string => {
  const variantMap = {
    default: 'shadow-md',
    elevated: 'shadow-lg',
    outlined: 'border border-base-300 shadow-sm',
    filled: 'bg-base-200'
  }
  return variantMap[variant] || 'shadow-md'
}

const getSizeClasses = (size: CardSize): string => {
  const sizeMap = {
    sm: 'text-sm',
    md: 'text-base',
    lg: 'text-lg',
    xl: 'text-xl'
  }
  return sizeMap[size] || 'text-base'
}
</script>

<style scoped>
.card {
  @apply rounded-box overflow-hidden;
}

.card-image {
  @apply relative overflow-hidden;
}

.card-overlay {
  @apply absolute inset-0 bg-black/50 flex items-center justify-center;
}

.card-title {
  @apply font-bold leading-tight;
}

.card-subtitle {
  @apply text-sm opacity-70 font-medium;
}

.card-content {
  @apply text-sm opacity-80 leading-relaxed;
}

.card-actions {
  @apply flex items-center gap-2;
}

/* Focus styles for interactive cards */
.card:focus-visible {
  @apply outline-none ring-2 ring-primary ring-offset-2;
}
</style>