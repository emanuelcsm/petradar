<script setup lang="ts">
</script>

<template>
  <button class="notification-bell" aria-label="Notificações">
    <slot />
  </button>
</template>

<style scoped>
.notification-bell {
  position: relative;
  background: none;
  border: none;
  cursor: pointer;
  padding: var(--space-2);
}
</style>
