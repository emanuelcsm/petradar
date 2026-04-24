<script setup lang="ts">
interface Props {
  hasNextPage: boolean
  isLoading?: boolean
}

defineProps<Props>()
defineEmits<{ loadMore: [] }>()
</script>

<template>
  <div v-if="hasNextPage" class="pagination">
    <button :disabled="isLoading" @click="$emit('loadMore')">
      Carregar mais
    </button>
  </div>
</template>

<style scoped>
.pagination {
  display: flex;
  justify-content: center;
  padding: var(--space-4);
}
</style>
