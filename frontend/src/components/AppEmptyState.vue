<script setup lang="ts">
interface Props {
  message: string
  ctaLabel?: string
}

defineProps<Props>()
defineEmits<{ action: [] }>()
</script>

<template>
  <div class="empty-state">
    <p>{{ message }}</p>
    <button v-if="ctaLabel" @click="$emit('action')">{{ ctaLabel }}</button>
  </div>
</template>

<style scoped>
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--space-4);
  padding: var(--space-12);
  color: var(--color-neutral-400);
  font-size: var(--font-size-sm);
}
</style>
