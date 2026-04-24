<script setup lang="ts">
interface Props {
  variant?: 'primary' | 'secondary' | 'ghost' | 'danger'
  disabled?: boolean
}

defineProps<Props>()
</script>

<template>
  <button :disabled="disabled">
    <slot />
  </button>
</template>

<style scoped>
button {
  cursor: pointer;
}
</style>
