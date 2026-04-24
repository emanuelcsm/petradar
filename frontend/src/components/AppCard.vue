<script setup lang="ts">
</script>

<template>
  <div class="card">
    <slot />
  </div>
</template>

<style scoped>
.card {
  background: var(--color-surface-raised);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-md);
  padding: var(--space-4);
}
</style>
