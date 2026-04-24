<script setup lang="ts">
interface Props {
  modelValue?: string
  id: string
  label?: string
  type?: string
  placeholder?: string
  error?: string
  disabled?: boolean
}

const props = defineProps<Props>()
const emit = defineEmits<{ 'update:modelValue': [value: string] }>()

function onInput(event: Event) {
  emit('update:modelValue', (event.target as HTMLInputElement).value)
}
</script>

<template>
  <div class="input-wrapper">
    <label v-if="label" :for="id" class="input-label">{{ label }}</label>
    <input
      :id="id"
      :type="type ?? 'text'"
      :value="modelValue"
      :placeholder="placeholder"
      :disabled="disabled"
      class="input-field"
      :class="{ 'input-field--error': error }"
      @input="onInput"
    />
    <span v-if="error" class="input-error">{{ error }}</span>
  </div>
</template>

<style scoped>
.input-wrapper {
  display: flex;
  flex-direction: column;
  gap: var(--space-1);
}

.input-label {
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
  color: var(--color-neutral-800);
}

.input-field {
  padding: var(--space-2) var(--space-3);
  border: 1px solid var(--color-neutral-200);
  border-radius: var(--radius-sm);
  font-size: var(--font-size-base);
  font-family: var(--font-family-base);
  color: var(--color-neutral-800);
  background-color: var(--color-surface);
  transition: border-color var(--transition-fast);
  outline: none;
}

.input-field:focus {
  border-color: var(--color-primary-500);
}

.input-field:disabled {
  background-color: var(--color-neutral-100);
  color: var(--color-neutral-400);
  cursor: not-allowed;
}

.input-field--error {
  border-color: var(--color-error);
}

.input-error {
  color: var(--color-error);
  font-size: var(--font-size-sm);
}
</style>
