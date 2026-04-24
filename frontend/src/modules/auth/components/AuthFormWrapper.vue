<script setup lang="ts">
interface Props {
  title: string
}

defineProps<Props>()
</script>

<template>
  <div class="auth-page">
    <div class="auth-card">
      <h1 class="auth-title">{{ title }}</h1>
      <slot />
      <div v-if="$slots.footer" class="auth-footer">
        <slot name="footer" />
      </div>
    </div>
  </div>
</template>

<style scoped>
.auth-page {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  background-color: var(--color-background);
  padding: var(--space-4);
}

.auth-card {
  width: 100%;
  max-width: 400px;
  background-color: var(--color-surface);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-md);
  padding: var(--space-8);
  display: flex;
  flex-direction: column;
  gap: var(--space-6);
}

.auth-title {
  font-size: var(--font-size-2xl);
  font-weight: var(--font-weight-bold);
  color: var(--color-neutral-900);
  text-align: center;
  margin: 0;
}

.auth-footer {
  text-align: center;
  font-size: var(--font-size-sm);
  color: var(--color-neutral-600);
}
</style>
