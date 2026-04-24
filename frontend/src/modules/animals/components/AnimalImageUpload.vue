<script setup lang="ts">
defineEmits<{ uploaded: [mediaId: string] }>()
</script>

<template>
  <div class="image-upload">
    <slot />
  </div>
</template>

<style scoped>
.image-upload {
  border: 2px dashed var(--color-neutral-200);
  border-radius: var(--radius-md);
  padding: var(--space-8);
  text-align: center;
}
</style>
