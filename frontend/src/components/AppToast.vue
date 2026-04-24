<script setup lang="ts">
interface Props {
  message: string
  type?: 'success' | 'error' | 'info'
}

defineProps<Props>()
</script>

<template>
  <div class="toast" :class="type">
    {{ message }}
  </div>
</template>

<style scoped>
.toast {
  padding: var(--space-3) var(--space-4);
  border-radius: var(--radius-md);
  font-size: var(--font-size-sm);
  box-shadow: var(--shadow-lg);
}

.success { background: var(--color-success); color: var(--color-surface); }
.error   { background: var(--color-error);   color: var(--color-surface); }
.info    { background: var(--color-info);     color: var(--color-surface); }
</style>
