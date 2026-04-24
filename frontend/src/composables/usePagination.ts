import { ref } from 'vue'

export function usePagination() {
  const nextPageToken = ref<string | null>(null)
  const hasNextPage = ref(false)

  return { nextPageToken, hasNextPage }
}
