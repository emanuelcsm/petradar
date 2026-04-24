import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useNotificationsStore = defineStore('notifications', () => {
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const hasError = computed(() => error.value !== null)

  return { isLoading, error, hasError }
})
