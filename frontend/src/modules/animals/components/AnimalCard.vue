<script setup lang="ts">
</script>

<template>
  <div class="animal-card">
    <slot />
  </div>
</template>

<style scoped>
.animal-card {
  border-radius: var(--radius-lg);
  background: var(--color-surface);
  box-shadow: var(--shadow-sm);
  padding: var(--space-4);
}
</style>
