import { ref } from 'vue'

export function useSignalR() {
  const isConnected = ref(false)

  return { isConnected }
}
