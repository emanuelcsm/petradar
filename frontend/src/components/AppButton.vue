<script setup lang="ts">
interface Props {
  variant?: 'primary' | 'secondary' | 'ghost' | 'danger'
  type?: 'button' | 'submit' | 'reset'
  disabled?: boolean
}

defineProps<Props>()
</script>

<template>
  <button
    :type="type ?? 'button'"
    :disabled="disabled"
    :class="['btn', `btn--${variant ?? 'primary'}`]"
  >
    <slot />
  </button>
</template>

<style scoped>
.btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  padding: var(--space-3) var(--space-4);
  border: none;
  border-radius: var(--radius-sm);
  font-size: var(--font-size-base);
  font-family: var(--font-family-base);
  font-weight: var(--font-weight-medium);
  cursor: pointer;
  transition: background-color var(--transition-fast);
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn--primary {
  background-color: var(--color-primary-500);
  color: var(--color-surface);
}

.btn--primary:hover:not(:disabled) {
  background-color: var(--color-primary-600);
}

.btn--secondary {
  background-color: var(--color-neutral-100);
  color: var(--color-neutral-800);
  border: 1px solid var(--color-neutral-200);
}

.btn--secondary:hover:not(:disabled) {
  background-color: var(--color-neutral-200);
}

.btn--ghost {
  background-color: transparent;
  color: var(--color-neutral-600);
}

.btn--ghost:hover:not(:disabled) {
  background-color: var(--color-neutral-100);
}

.btn--danger {
  background-color: var(--color-error);
  color: var(--color-surface);
}

.btn--danger:hover:not(:disabled) {
  background-color: #b91c1c;
}
</style>
