<script setup lang="ts">
interface Props {
  latitude: number
  longitude: number
}

defineProps<Props>()
</script>

<template>
  <div class="animal-map">
    <slot />
  </div>
</template>

<style scoped>
.animal-map {
  width: 100%;
  height: 300px;
  background: var(--color-neutral-100);
  border-radius: var(--radius-md);
}
</style>
