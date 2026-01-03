<template>
  <div :data-admin-edit="`text-${_uid}`" class="editable-text" @click="handleEdit">
    <component :is="tag" v-bind="$attrs">
      <slot />
    </component>
  </div>
</template>

<script setup lang="ts">
import { useAdminStore } from '../stores/admin';

interface Props {
  tag?: string;
}

withDefaults(defineProps<Props>(), {
  tag: 'p',
});

const adminStore = useAdminStore();

const handleEdit = () => {
  if (adminStore.isActive && adminStore.canEditContent) {
    // Open edit modal or inline editor
    console.log('Edit text element:', `text-${getCurrentInstance()?.uid}`);
    // TODO: Implement edit functionality
  }
};
</script>

<style scoped>
.editable-text {
  cursor: default;
}

.editable-text:hover {
  cursor: pointer;
}
</style>
