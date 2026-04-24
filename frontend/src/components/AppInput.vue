<script setup lang="ts">
interface Props {
  label?: string
  error?: string
  disabled?: boolean
}

defineProps<Props>()
defineEmits<{ 'update:modelValue': [value: string] }>()
</script>

<template>
  <div class="input-wrapper">
    <label v-if="label">{{ label }}</label>
    <input :disabled="disabled" />
    <span v-if="error" class="error">{{ error }}</span>
  </div>
</template>

<style scoped>
.input-wrapper {
  display: flex;
  flex-direction: column;
  gap: var(--space-1);
}

.error {
  color: var(--color-error);
  font-size: var(--font-size-sm);
}
</style>
