<script setup lang="ts">
</script>

<template>
  <span class="spinner" role="status" aria-label="Carregando" />
</template>

<style scoped>
.spinner {
  display: inline-block;
  width: 1em;
  height: 1em;
  border: 2px solid var(--color-neutral-200);
  border-top-color: var(--color-primary-500);
  border-radius: var(--radius-full);
  animation: spin 600ms linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>
