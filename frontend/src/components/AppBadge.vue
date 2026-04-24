<script setup lang="ts">
interface Props {
  label: string
}

defineProps<Props>()
</script>

<template>
  <span class="badge">{{ label }}</span>
</template>

<style scoped>
.badge {
  display: inline-block;
  padding: var(--space-1) var(--space-2);
  border-radius: var(--radius-full);
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-medium);
}
</style>
