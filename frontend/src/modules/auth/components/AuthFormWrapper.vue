<script setup lang="ts">
</script>

<template>
  <div class="form-wrapper">
    <slot />
  </div>
</template>

<style scoped>
.form-wrapper {
  max-width: 400px;
  margin: 0 auto;
  padding: var(--space-6);
}
</style>
