<script setup lang="ts">
interface Props {
  status: 'Lost' | 'Found'
}

defineProps<Props>()
</script>

<template>
  <span class="status-badge" :class="status.toLowerCase()">
    {{ status === 'Lost' ? 'Perdido' : 'Encontrado' }}
  </span>
</template>

<style scoped>
.status-badge {
  display: inline-block;
  padding: var(--space-1) var(--space-2);
  border-radius: var(--radius-full);
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-medium);
}

.lost  { background: var(--color-primary-100); color: var(--color-primary-700); }
.found { background: var(--color-success);     color: var(--color-surface); }
</style>
